using FileMonitor.Services.Common;
using Microsoft.Win32;
using System.Diagnostics;
using System.Reflection;

namespace FileMonitor.Services.Services
{
    public class ControlBackgroundService
    {
        private readonly InformationService _informationService;

        public ControlBackgroundService()
        {
            _informationService = new InformationService();
        }

        public void StartBackgroundApplication(string processName, string backgroundApplicationNestedFolder)
        {
            if (!_informationService.BackgroundServiceIsRunning(processName))
            {
                var fileLocation = $"{LocationHelper.GetLocation(Assembly.GetExecutingAssembly().Location)}\\{backgroundApplicationNestedFolder}";

                Process.Start($"{fileLocation}\\{processName}.exe");
            };
        }

        public void StopBackgroundApplication(string processName)
        {
            var processes = Process.GetProcessesByName(processName);

            foreach (var process in processes)
            {
                process?.Kill();
            };
        }

        public void SetStartup(string path)
        {
            RegistryKey rkApp = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

            if (!_informationService.IsSetStartup())
            {
                rkApp.SetValue("FileMonitor", path);
            }
        }

        public void DeleteStartup()
        {
            RegistryKey rkApp = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

            if (_informationService.IsSetStartup())
            {
                rkApp.DeleteValue("FileMonitor");
            }
        }
    }
}
