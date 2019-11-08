using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PdfFilesManager
{
    public interface ILogger
    {
        void Log(Exception exception);
    }
}
