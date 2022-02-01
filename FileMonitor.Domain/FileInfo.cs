using System;

namespace FileMonitor.Domain
{
    public class FileInfo
    {
        public DateTime CreatedOn { get; set; }
        public string Path { get; set; }
        public string RawAttributes { get; set; }
        public string Hash { get; set; }
    }
}
