using Microsoft.Win32;
using System.Diagnostics;

namespace FileMonitor.Services.Services
{
    public class InformationService
    {
        public bool BackgroundServiceIsRunning(string processName)
        {
            var processes = Process.GetProcessesByName(processName);

            return processes.Length >= 1;
        }

        public bool IsSetStartup()
        {
            RegistryKey rkApp = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

            if (rkApp.GetValue("FileMonitor") == null)
            {
                return false;
            }

            return true;
        }
    }
}
