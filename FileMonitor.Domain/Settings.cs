using System;
using System.Collections.Generic;

namespace FileMonitor.Domain
{
    public class Settings
    {
        public Settings()
        {
            Paths = new List<string>();
        }

        public IList<string> Paths { get; set; }
        public DateTime ModifiedOn { get; set; }
    }
}
