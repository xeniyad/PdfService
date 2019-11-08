using PdfFilesManager;
using System;
using System.Collections.Generic;
using Topshelf;
using Topshelf.Runtime;

namespace PdfService
{
    class Program
    {
        static void Main(string[] args)
        {
            HostFactory.Run(
               x => {
                   x.Service(() => new PdfServiceTS(new FileSettingsDto()));
                   x.EnableServiceRecovery(
                            r => r.RestartService(0).RestartService(1));

               }
            );
        
        }

        
    }
}
