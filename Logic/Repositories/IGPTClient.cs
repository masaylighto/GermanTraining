using OneOf;

namespace Logic.Repositories;

public interface IGPTClient
{
    public Task<OneOf<String, Exception>> GetAnswer(string Question);
}
