using Logic.Core;
using Logic.Core.DataType;
using Logic.Core.Helpers;
using Logic.Core.Models;
using OneOf;
using Polly;
using Polly.Retry;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
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
            ApplyRequestHeaders();
            ApplyHttpConfig();
        }
        void ApplyHttpConfig()
        {

            client.DefaultRequestVersion = HttpVersion.Version30;
            client.DefaultVersionPolicy = HttpVersionPolicy.RequestVersionExact;

        }
        void ApplyRequestHeaders() 
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ApiConfig.GPTApiToken);
            client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("zip"));
            client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("deflate"));
            client.DefaultRequestHeaders.Connection.Add("keep-alive");
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
                var gptResponse =  await client.SendAsync(BuildRequestMessage(Question), CompletionOption);
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
                string completeResponse = string.Empty;
                var responseStream = responseMessage.Content.ReadAsStream();
                var streamReader = new StreamReader(responseStream);
                while (!streamReader.EndOfStream)
                {
                    var response = streamReader.ReadLine();
                    if (string.IsNullOrEmpty(response))
                    {
                        continue;
                    }
                    var answer = ExtractAnswer(response);
                    if (answer.IsT0)
                    {
                        completeResponse += answer.AsT0;
                    }

                }
                return completeResponse;
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
        OneOf<string,None> ExtractAnswer(string response)
        {
           
            GptResponse deserializeContent = default;
            try // not the best way out there to handle bad json, but its temporary 
            {
                var indexOfjsonStart= response.IndexOf("{");
                if (indexOfjsonStart==-1)
                {
                        return new None();
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
                return new None();
            }
            return deserializeContent.choices.First().delta.content;           

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
