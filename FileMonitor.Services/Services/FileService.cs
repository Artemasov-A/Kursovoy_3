using FileMonitor.Domain;
using FileMonitor.Services.Common;
using System.Text.Json;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace FileMonitor.Services.Services
{
    public class FileService
    {
        private readonly string _folderPath = string.Empty;
        private readonly string _settingsFilePath = string.Empty;
        private readonly string _hashedValuesFilePath = string.Empty;

        public FileService()
        {
            _folderPath = $"{LocationHelper.GetLocation(Assembly.GetExecutingAssembly().Location)}\\files";

            if (!Directory.Exists(_folderPath))
            {
                Directory.CreateDirectory(_folderPath);
            }

            _settingsFilePath = $"{_folderPath}\\{FileNames.FoldersSettingsFile}";
            _hashedValuesFilePath = $"{_folderPath}\\{FileNames.HashedValuesFile}";
        }

        public Settings GetSettings()
        {
            var isExist = File.Exists(_settingsFilePath);

            if (!isExist)
            {
                return new Settings();
            }

            var data = File.ReadAllText(_settingsFilePath);

            return JsonConvert.DeserializeObject<Settings>(data);
        }

        public void SavePath(string path)
        {
            var settings = GetSettings();

            settings.Paths.Add(path);
            settings.ModifiedOn = DateTime.UtcNow;

            var serializedSettings = JsonConvert.SerializeObject(settings, Formatting.Indented);

            File.WriteAllText(_settingsFilePath, serializedSettings);
        }

        public void ResavePaths(IList<string> paths)
        {
            var settings = GetSettings();

            settings.ModifiedOn = DateTime.UtcNow;
            settings.Paths = paths;

            var serializedSettings = JsonConvert.SerializeObject(settings, Formatting.Indented);

            File.WriteAllText(_settingsFilePath, serializedSettings);
        }

        public IList<FolderInfo> GetFolderInfo()
        {
            if (!File.Exists(_hashedValuesFilePath))
            {
                return new List<FolderInfo>();
            }

            var bytes = File.ReadAllBytes(_hashedValuesFilePath);

            try
            {
                return System.Text.Json.JsonSerializer.Deserialize<IList<FolderInfo>>(bytes);
            }
            catch (Exception)
            {
                File.Delete(_hashedValuesFilePath);
            }

            return new List<FolderInfo>();
        }

        public void WriteFolderInfo(IList<FolderInfo> foldersInfo)
        {
            var bytes = System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(foldersInfo);

            File.WriteAllBytes(_hashedValuesFilePath, bytes);
        }
    }
}
