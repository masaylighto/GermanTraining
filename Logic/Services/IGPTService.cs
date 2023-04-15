
using OneOf;

namespace Logic.Services;

public interface IGPTService
{
    /// <summary>
    /// will validate a phrase using gpt and it will check it it contain the required word and return a correction to you if its not currect
    /// </summary>
    /// <param name="phrases"></param>
    /// <param name="requiredWord">word you want to ensure its used in the sentence</param>
    /// <returns></returns>
    Task<OneOf<string, Exception>> ValidatePhrase(string phrases,string requiredWord);
}
