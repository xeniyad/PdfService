using PdfFilesManager;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Topshelf;

namespace PdfService
{
    public class PdfServiceTS : ServiceControl
    {
        private readonly Timer timer;
        private const int fileSearchRepeat = 5_000;
        private FilesManager _filesManager;

        public PdfServiceTS(FileSettingsDto settings)
        {
            timer = new Timer(WorkProcedure);
            _filesManager = new FilesManager(settings, new ConsoleLogger());
        }


        private void WorkProcedure(object target)
        {
            _filesManager.Execute();
        }

        public bool Start(HostControl hostControl)
        {
            timer.Change(0, fileSearchRepeat);
            return true;
        }

        public bool Stop(HostControl hostControl)
        {
            timer.Change(Timeout.Infinite, 0);
            _filesManager.CancelOperations();
            return true;
        }
    }
}
