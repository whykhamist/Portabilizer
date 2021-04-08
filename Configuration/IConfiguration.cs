using System.Collections.Generic;

namespace Configuration
{
    public interface IConfiguration
    {
        string Title { get; set; }

        int Width { get; set; }

        int Height { get; set; }

        bool ClearSymlinks { get; set; }

        string Executable { get; set; }

        string DataFolder { get; set; }

        List<DataPaths> DataPaths { get; set; }
    }
}
