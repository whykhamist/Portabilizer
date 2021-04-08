using System.Collections.Generic;

namespace Configuration
{
    public class DataPaths : IDataPaths
    {
        public string GroupName { get; set; }

        public Dictionary<string, string> Paths { get; set; }
    }
}
