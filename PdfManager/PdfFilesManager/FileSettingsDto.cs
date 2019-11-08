using System;
using System.Collections.Generic;

namespace PdfFilesManager
{
    public class FileSettingsDto
    {
        public string InputFolderPath { get; set; }
        public string OutputFolderPath { get; set; }
        public string InputFilesPrefix { get; set; }
        public string OutputFilesPrefix { get; set; }
        public List<string> AllowedExtensions { get; set; }
        public int ImageAppearTimeout { get; set; }

        public FileSettingsDto()
        {
            AllowedExtensions = new List<string> { "png", "jpeg", "jpg" };
            InputFilesPrefix = "image";
            InputFolderPath = @"C:\Users\xeniya_denissova\Desktop\forService\input";
            OutputFolderPath = @"C:\Users\xeniya_denissova\Desktop\forService\output2";
            OutputFilesPrefix = "pdf";
            ImageAppearTimeout = 60000;
        }
    }


}
