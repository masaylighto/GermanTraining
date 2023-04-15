using Logic.Core;

namespace Logic.Repositories;

public interface IExcelRepo
{
    IEnumerable<ExcelRow> Words { get; }
    IEnumerable<ExcelRow> Nouns { get; }
}
