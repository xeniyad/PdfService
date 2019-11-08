using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PdfFilesManager
{
    public class ConsoleLogger : ILogger
    {
        public void Log(Exception exception)
        {
            Console.WriteLine(exception.Message);
        }
    }
}
