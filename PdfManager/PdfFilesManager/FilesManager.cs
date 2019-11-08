using System;
using System.IO;
using System.Linq;
using System.Drawing.Imaging;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.IO.Image;
using iText.Kernel.Geom;
using iText.Kernel.Events;
using iText.Kernel.Pdf.Canvas;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace PdfFilesManager
{
    public class FilesManager
    {
        private readonly FileSettingsDto _settings;
        private int _currentOutputNumber = 1;
        private List<string> _images = new List<string>();
        ILogger _logger;
        System.Threading.CancellationTokenSource _cancelTokenSource;
        private static Regex digitsRegex = new Regex(@"\d{3}");


        public FilesManager(FileSettingsDto _fileSettings, ILogger logger)
        {
            _settings = _fileSettings;
            _logger = logger;
            _cancelTokenSource = new System.Threading.CancellationTokenSource();
        }


        public int GetAllFiles()
        {
            if (!Directory.Exists(_settings.InputFolderPath))
                throw new DirectoryNotFoundException("Files directory not found. Please correct directory path in settings file");
            var newImages = Directory
                .EnumerateFiles(_settings.InputFolderPath, $"{_settings.InputFilesPrefix}_*.*")
                .Where(file => _settings.AllowedExtensions.Any(file.ToLower().EndsWith) && Regex.IsMatch(file, @"\w+_\d{3}.\w{3}"))
                .ToList();

            var newImagesCount = newImages.Count - _images.Count;
            if (newImagesCount > 0)
            {
                _images = newImages;
            }
            return newImagesCount;

        }

        public void StartNewOutputFile()
        {
            _currentOutputNumber++;
        }

        public void DivideImagesOnFiles()
        {
            var tmpImages = new List<string>();
            var previousIndex = 0;
            foreach (var img in _images)
            {
                if (_cancelTokenSource.Token.IsCancellationRequested) break;

                var digitMatch = digitsRegex.Match(img);
                if (digitMatch.Success)
                {
                    if (int.TryParse(digitMatch.Value, out int currentIndex))
                    {
                        if (currentIndex - previousIndex > 1)
                        {
                            AddNewPdf(tmpImages);
                            tmpImages.Clear();
                        }
                        tmpImages.Add(img);
                        previousIndex = currentIndex;
                    }
                }
            }
            if (tmpImages.Count > 0)
            {
                AddNewPdf(tmpImages);
            }
        }

        public void Execute()
        {
            var filesCount = GetAllFiles();
            if (filesCount > 0)
            {
               DivideImagesOnFiles();
            }
        }

        private void AddNewPdf(List<string> images)
        {
            var outputPdf = System.IO.Path.Combine(_settings.OutputFolderPath, $"{_settings.OutputFilesPrefix}_{_currentOutputNumber:D3}.pdf");
            try
            {
                using (var writer = new PdfWriter(outputPdf))
                {
                    using (var pdf = new PdfDocument(writer))
                    {
                        using (var document = new Document(pdf))
                        {
                            foreach (var img in images)
                            {
                                if (_cancelTokenSource.Token.IsCancellationRequested) break;

                                var imageData = ImageDataFactory.Create(img);
                                Image image = new Image(imageData);

                                var page = pdf.AddNewPage(new PageSize(image.GetImageWidth(), image.GetImageHeight()));

                                PdfCanvas aboveCanvas = new PdfCanvas(page);
                                Rectangle area = page.GetPageSize();
                                new Canvas(aboveCanvas, pdf, area).Add(image);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Log(ex);
            }
            _currentOutputNumber++;
            
        }

        public void CancelOperations()
        {
            _cancelTokenSource.Cancel();
        }
       
    }
}
