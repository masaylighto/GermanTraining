using Logic.Contracts;
using Logic.Core;
using Logic.Repositories;

namespace Logic.Services;

public class ExcelService: IExcelService
{
    IExcelRepo ExcelRepo { get; }
    int NounCount;
    int WordCount;
    public ExcelService(IExcelRepo excelRepo)
    {
        ExcelRepo = excelRepo;
        NounCount = ExcelRepo.Nouns.Count();
        WordCount = ExcelRepo.Words.Count();
    }

    public async Task<ExcelRow> GetRandomNoun(ExcelGetRandomNoun Contract)
    {

        ExcelRow? excelRow = null;
        while (excelRow is null)
        {
            int skip = Contract.Skip.HasValue ? Contract.Skip.Value : 0;
            int take = Contract.Take.HasValue ? Contract.Take.Value : NounCount;
            ExcelRepo.Nouns.ElementAtOrDefault(Random.Shared.Next(skip, take)); // we might encounter empty value , in this loop we ensure that we will return non empty value
        }
        return excelRow;
    }

    
    public async Task<ExcelRow> GetRandomWord(ExcelGetRandomWord Contract)
    {
        ExcelRow? excelRow = null;
        while (excelRow is null)
        {
            int skip = Contract.Skip.HasValue ? Contract.Skip.Value : 0;
            int take = Contract.Take.HasValue ? Contract.Take.Value : WordCount;
            excelRow = ExcelRepo.Words.ElementAtOrDefault(Random.Shared.Next(skip, take)); // we might encounter empty value , in this loop we ensure that we will return non empty value
        }
        return excelRow;        
    }
}
