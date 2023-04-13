
namespace Logic.Contracts;

public class ExcelGetRandomNoun
{
    //limit the random returns in a range
    public int? Skip { get; set; }
    public int? Take { get; set; }
}
