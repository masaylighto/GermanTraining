

using AnyOfTypes;
using Logic.Core.Helpers;
using Logic.Repositories;

namespace Logic.Services;

public class GPTService : IGPTService
{
    public IGPTClient GptClient { get; }
    public ILogger Logger { get; }

    public GPTService(IGPTClient gptClient,ILogger logger)
    {
        GptClient = gptClient;
        Logger = logger;
    }   

    public async Task<AnyOf<string,Exception>> ValidatePhrase(string phrases, string requiredWord)
    {
       var answer = await GptClient.GetAnswer(@$"
        Answer with correct or not is and if it's not,
        also give a short correction to this sentence ""{phrases}"" if it was wrong 
        and ensure it contain this word ""{requiredWord}""
        or one of its conjugation or declination in German 
        ");
        if (answer.IsSecond)
        {
            Logger.Error(answer.Second.Message);
        }
        return answer;
    }
}
