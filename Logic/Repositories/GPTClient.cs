using Logic.Core;
using Logic.Core.Helpers;
using Logic.Core.Models;
using OneOf;
using Polly;
using Polly.Retry;
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
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiConfig.GPTApiToken);
            retryPolicy = CreateRetryPolicy();
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

        public async Task<OneOf<String, Exception>> GetAnswer(string Question)
        {
            try
            {
                var messageContent = BuildRequestBody(Question);
                var gptResponse = await ExcuteWithPolicy(async () => await client.PostAsync(ApiConfig.ApiEndPoint, messageContent));
                var response = await gptResponse.Content.ReadAsStringAsync();
                if (!gptResponse.IsSuccessStatusCode)
                {
                    return new HttpRequestException(response);
                }
                var deserializeContent = JsonSerializer.Deserialize<GptResponse>(response);
                if (deserializeContent is null || deserializeContent.choices is null || deserializeContent.choices.Length == 0)
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
            });
            return new StringContent(serializedMessage, MediaTypeHeaderValue.Parse("application/json"));
        }
    }
}
