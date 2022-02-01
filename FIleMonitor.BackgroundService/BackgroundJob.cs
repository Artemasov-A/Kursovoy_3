using FileMonitor.Domain;
using FileMonitor.Services.Services;
using FIleMonitor.BackgroundService;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;

namespace FileMonitor.BackgroundService
{
    public class BackgroundJob
    {
        private readonly FileService _fileService;
        private readonly NotificationService _notificationService;
        private Settings _settings;

        public BackgroundJob()
        {
            _fileService = new FileService();
            _notificationService = new NotificationService();
        }

        public void Execute()
        {
            _settings = _fileService.GetSettings();

            if (!_settings.Paths.Any())
            {
                return;
            }

            while (true)
            {
                var after = new List<FolderInfo>();

                var before = _fileService.GetFolderInfo();

                try
                {
                    after = GetFoldersInfo().ToList();
                }
                catch(Exception)
                {
                    Thread.Sleep(10000);
                    continue;
                }

                CheckChanges(before, after);

                Thread.Sleep(7000);
            }
        }

        private IList<FolderInfo> GetFoldersInfo()
        {
            var result = new List<FolderInfo>();

            foreach(var path in _settings.Paths)
            {
                if (!Directory.Exists(path))
                {
                    continue;
                }

                var fileNames = Directory.GetFiles(path);

                var filesInfo = GetFilesInfo(fileNames);

                result.Add(new FolderInfo
                {
                    Path = path,
                    CreatedOn = DateTime.UtcNow,
                    FilesInfo = filesInfo
                });
            }

            return result;
        }

        private IList<Domain.FileInfo> GetFilesInfo(IEnumerable<string> fileNames)
        {
            var result = new List<Domain.FileInfo>();

            foreach(var fileName in fileNames)
            {
                try
                {
                    if (!File.Exists(fileName))
                    {
                        continue;
                    }
                }
                catch (Exception)
                {
                    continue;
                }

                var fileInfo = new Domain.FileInfo
                {
                    CreatedOn = System.DateTime.UtcNow,
                    Path = fileName,
                    RawAttributes = File.GetAttributes(fileName).ToString()
                };

                using(var md5  = MD5.Create())
                {
                    using (var stream = File.OpenRead(fileName))
                    {
                        var hash = md5.ComputeHash(stream);
                        fileInfo.Hash = BitConverter.ToString(hash).Replace("-", String.Empty).ToLower();
                    }
                }

                result.Add(fileInfo);
            }

            return result;
        }

        private void CheckChanges(IList<FolderInfo> before, IList<FolderInfo> after)
        {
            foreach(var folderBefore in before)
            {
                var folderAfter = after.FirstOrDefault(p => string.Equals(p.Path, folderBefore.Path, StringComparison.CurrentCultureIgnoreCase));

                if (folderAfter == null)
                {
                    _notificationService.SendMessageAboutDeleteFolder(folderBefore.Path);
                    continue;
                }

                CheckFilesChanges(folderBefore.FilesInfo, folderAfter.FilesInfo);
            }

            _fileService.WriteFolderInfo(after);
        }

        private int CheckFilesChanges(IList<Domain.FileInfo> before, IList<Domain.FileInfo> after)
        {
            var changes = 0;

            foreach(var fileBefore in before)
            {
                var fileAfter = after.FirstOrDefault(p => p.Path == fileBefore.Path);

                if (fileAfter == null)
                {
                    _notificationService.SendMessageAboutDeleteFile(fileBefore.Path);
                    changes++;
                    continue;
                }

                var hashIsEqual = string.Equals(fileBefore.Hash, fileAfter.Hash, StringComparison.CurrentCultureIgnoreCase);

                if (!hashIsEqual)
                {
                    _notificationService.SendMessageAboutChangeFile(fileAfter.Path);
                    changes++;
                }
            }

            return changes;
        }
    }
}
