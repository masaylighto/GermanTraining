using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic.Core.Helpers
{
    public class Logger : ILogger
    {
        public Serilog.ILogger SerilogLogger { get; }
        public Logger(Serilog.ILogger logger)
        {
            SerilogLogger = logger;
        }      

        public void Error(string message)
        {
            SerilogLogger.Error(message);
        }

        public void Fatal(string message)
        {
            SerilogLogger.Fatal(message);
        }

        public void Info(string message)
        {
            SerilogLogger.Information(message);
        }

        public void Warn(string message)
        {
            SerilogLogger.Warning(message);
        }
    }
}
