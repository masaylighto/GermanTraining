using Logic.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic.Repositories;

public interface IExcelRepo
{
    IEnumerable<ExcelRow> Words { get; }
    IEnumerable<ExcelRow> Nouns { get; }
}
