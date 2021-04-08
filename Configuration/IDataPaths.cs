using System.Collections.Generic;

namespace Configuration
{
    public interface IDataPaths
    {
        string GroupName { get; set; }
        Dictionary<string, string> Paths { get; set; }
    }
}
