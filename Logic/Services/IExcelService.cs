using Logic.Contracts;
using Logic.Core;

namespace Logic.Services;

public interface IExcelService
{
    Task<ExcelRow> GetRandomNoun(ExcelGetRandomNoun Contract);
    Task<ExcelRow> GetRandomWord(ExcelGetRandomWord Contract);
}
