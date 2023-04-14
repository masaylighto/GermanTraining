using LinqToExcel;
using LinqToExcel.Query;
using Logic.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic.Repositories;

public class ExcelRepo: IExcelRepo
{
    public ExcelQueryFactory ExcelQuery { get; }
    public ExcelRepo(ExcelQueryFactory excelQuery)
    {
        ExcelQuery = excelQuery;
    }
    string GetWorkSheetName(int i) => ExcelQuery.GetWorksheetNames().ElementAt(i);


    ExcelQueryable<RowNoHeader> WorksheetNoHeader(int workSheet = 0) => ExcelQuery.WorksheetNoHeader(GetWorkSheetName(workSheet));
    ExcelQueryable<Row> Worksheet(int workSheet = 0) =>ExcelQuery.Worksheet(GetWorkSheetName(workSheet));
    public IEnumerable<ExcelRow> Words => WorksheetNoHeader()
        .Select(col => new ExcelRow
        {
            Article = (col[0] ?? string.Empty).Trim(),
            GermanWord = (col[1] ?? string.Empty).Trim(),
            Translation = (col[2] ?? string.Empty).Trim()
        });
    public IEnumerable<ExcelRow> Nouns => Words.Where(x => !string.IsNullOrEmpty(x.Article));


}
