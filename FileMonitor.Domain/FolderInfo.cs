using System;
using System.Collections.Generic;

namespace FileMonitor.Domain
{
    public class FolderInfo
    {
        public FolderInfo()
        {
            FilesInfo = new List<FileInfo>();
        }
        public DateTime CreatedOn { get; set; }    
        public string Path { get; set; }
        public IList<FileInfo> FilesInfo { get; set; }
    }
}
