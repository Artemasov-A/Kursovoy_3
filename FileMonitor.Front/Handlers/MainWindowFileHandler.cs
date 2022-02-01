using FileMonitor.Services.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FileMonitor.Front.Handlers
{
    public class MainWindowFileHandler
    {
        private IList<string> Paths { get; set; }
        private readonly FileService _fileService;

        public MainWindowFileHandler()
        {
            Paths = new ObservableCollection<string>();
            _fileService = new FileService();
        }

        public IList<string> InitializePaths()
        {
            var settings = _fileService.GetSettings();

            Paths = settings.Paths;

            return Paths;
        }

        public string AddNewPathClick()
        {
            using var dialog = new FolderBrowserDialog
            {
                Description = "Time to select a folder",
                UseDescriptionForTitle = true,
                SelectedPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory),
                ShowNewFolderButton = true
            };

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                var path = dialog.SelectedPath;

                if (!Paths.Any(p => p.Equals(path, StringComparison.CurrentCultureIgnoreCase)) && !string.IsNullOrEmpty(path))
                {
                    Paths.Add(path);

                    Task.Run(() => { _fileService.SavePath(path); });

                    return path;
                }
            }

            return string.Empty;
        }

        public bool RemoveLastPath()
        {
            if (Paths.Count() == 0)
            {
                return false;
            }

            Paths.RemoveAt(Paths.Count - 1);

            Task.Run(() => { _fileService.ResavePaths(Paths); });

            return true;
        }
    }
}
