
namespace Logic.Core.Models;

public class GptResponse
{
    public string id { get; set; }
    public string _object { get; set; }
    public int created { get; set; }
    public string model { get; set; }
    public Usage usage { get; set; }
    public Choice[] choices { get; set; }
}

public class Usage
{
    public int prompt_tokens { get; set; }
    public int completion_tokens { get; set; }
    public int total_tokens { get; set; }
}

public class Choice
{
    public Message message { get; set; } //will be filled in EndToEnd mode
    public Delta delta { get; set; } // will be filled in streaming mode
    public string finish_reason { get; set; }
    public int index { get; set; }
}
public class Delta
{
    public string content { get; set; }
}

public class Message
{
    public string role { get; set; }
    public string content { get; set; }
}

