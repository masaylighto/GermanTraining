using Logic.Core;
using Logic.Core.Helpers;
using Logic.Core.Models;
using OneOf;
using Polly;
using Polly.Retry;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;

namespace Logic.Repositories
{
    public class GPTClient : IGPTClient
    {
        HttpClient client = new HttpClient();
        public GPTApiConfig ApiConfig { get; }
        public ILogger Logger { get; }

        AsyncRetryPolicy<HttpResponseMessage> retryPolicy;
        public GPTClient(GPTApiConfig apiConfig, ILogger logger)
        {
            ApiConfig = apiConfig;
            Logger = logger;
            retryPolicy = CreateRetryPolicy();
            ApplyRequestHeaders();
        }
        void ApplyRequestHeaders() 
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ApiConfig.GPTApiToken);
            client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("zip"));
            client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("deflate"));
            client.DefaultRequestHeaders.Connection.Add("keep-alive");
        }
        private AsyncRetryPolicy<HttpResponseMessage> CreateRetryPolicy()
        {

            return Policy.Handle<HttpRequestException>()
                 .OrResult<HttpResponseMessage>(r => r.IsSuccessStatusCode)
                 .WaitAndRetryAsync(ApiConfig.RequestRetryCount, (_) => TimeSpan.FromSeconds(ApiConfig.BetweenFailedRequestDelayInSecond), (exception, timeSpan, retryCount, context) =>
                 {
                     if (exception.Exception is not null)
                     {
                         Logger.Error($"{DateTime.Now.ToString("yyyy/MM/dd/HH/mm/ss")} - GPT Request Exception: {exception.Exception.Message}");
                     }
                 });
        }
        private async Task<HttpResponseMessage> ExcuteWithPolicy(Func<Task<HttpResponseMessage>> HttpRequestFunc)
        {

            return await retryPolicy.ExecuteAsync(HttpRequestFunc);
        }
      
        HttpRequestMessage BuildRequestMessage(string Question)
        {
            var messageContent = BuildRequestBody(Question);
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, ApiConfig.ApiEndPoint);
            httpRequestMessage.Content = messageContent;
            return httpRequestMessage;
        }
        public async Task<OneOf<String, Exception>> GetAnswer(string Question)
        {
            try
            {
                //depend on if we want to receive the HTTP response streamed or EndtoEnd
                var CompletionOption = ApiConfig.Stream ? HttpCompletionOption.ResponseHeadersRead:
                                                          HttpCompletionOption.ResponseContentRead;
                var gptResponse = await ExcuteWithPolicy(async () => await client.SendAsync(BuildRequestMessage(Question), CompletionOption));
                gptResponse.EnsureSuccessStatusCode();
                if (!gptResponse.IsSuccessStatusCode)
                {
                    return new HttpRequestException(await gptResponse.Content.ReadAsStringAsync());
                }
                //GPT offer two way to response; a streamed response where it send the answer piece by piece
                //and an EndtoEnd where the result send as one piece,
                //we can define which one by including "stream(boolean)" parameter in our request
                //we here handle both cases 
                return ApiConfig.Stream ? await GetAnswerStreamed(gptResponse):
                                          await GetAnswerEndToEnd(gptResponse);
            }
            catch (Exception ex)
            {
                return ex;
            }
    
        }
   
        async Task<OneOf<String, Exception>> GetAnswerStreamed(HttpResponseMessage responseMessage)
        {
            try
            {
                //get steam get lines convert them to json combine answer return it               
                string answer = string.Empty;
                foreach (var response in ExtractAnswers(ResponseToLines(responseMessage)))
                {                   
                   answer += response;                  
                } 
                return answer;
            }
            catch (Exception ex)
            {
                return ex;
            }

}
        async Task<OneOf<String, Exception>> GetAnswerEndToEnd(HttpResponseMessage responseMessage)
        {
            try
            {
                var response = await responseMessage.Content.ReadAsStringAsync();
                if (!responseMessage.IsSuccessStatusCode)
                {
                    return new HttpRequestException(response);
                }
                var deserializeContent = JsonSerializer.Deserialize<GptResponse>(response);
                if (deserializeContent is null ||
                     deserializeContent.choices is null ||
                     deserializeContent.choices.Length == 0 ||
                     deserializeContent.choices.First().message is null ||
                     string.IsNullOrEmpty(deserializeContent.choices.First().message.content)) 
                { 

                    return new Exception("No Answer in Response");
                }
                return deserializeContent.choices.First().message.content;
            }
            catch (Exception ex)
            {
                return ex;
            }
       

        }
        IEnumerable<string> ResponseToLines(HttpResponseMessage stream)
        {
            var responseStream = stream.Content.ReadAsStream();
            var streamReader = new StreamReader(responseStream);
            while (!streamReader.EndOfStream)
            {
                var response = streamReader.ReadLine();
                if (string.IsNullOrEmpty(response))
                {
                    continue;
                }
                yield return response;
            }
        }
        IEnumerable<string> ExtractAnswers(IEnumerable<string> responses)
        {
            foreach (var response in responses)
            {
                GptResponse deserializeContent = default;
                try // not the best way out there to handle bad json, but its temporary 
                {
                    var indexOfjsonStart= response.IndexOf("{");
                    if (indexOfjsonStart==-1)
                    {
                        continue;
                    }
                    var jsonString=response.Substring(indexOfjsonStart, response.Length- indexOfjsonStart);// remove anything before json brackets
                    deserializeContent = JsonSerializer.Deserialize<GptResponse>(jsonString);
                }
                catch
                {

                }

                if (deserializeContent is null ||
                    deserializeContent.choices is null ||
                    deserializeContent.choices.Length == 0 ||
                    deserializeContent.choices.First().delta is null ||
                    string.IsNullOrEmpty(deserializeContent.choices.First().delta.content))
                {
                    continue;
                }
                yield return deserializeContent.choices.First().delta.content;

            }

        }
        public StringContent BuildRequestBody(string Question)
        {
            var serializedMessage = JsonSerializer.Serialize(new
            {
                model = ApiConfig.GPTModel,
                messages = new List<dynamic>
                {   new
                    {
                        role = ApiConfig.Role,
                        content = Question
                    }
                },
                temperature = ApiConfig.SamplingTemperature,
                top_p = ApiConfig.NucleusSampling,
                n = ApiConfig.ChatCompletionChoicesForEachMessage,
                stop = ApiConfig.FurtherTokens,
                stream=ApiConfig.Stream
            });
            return new StringContent(serializedMessage, MediaTypeHeaderValue.Parse("application/json"));
        }
    }
}
