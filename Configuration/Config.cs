using System;
using System.Collections.Generic;

namespace Configuration
{
    [Serializable]
    public class Config : IConfiguration
    {
        public string Title { get; set; }

        public int Width { get; set; } = -1;

        public int Height { get; set; } = -1;

        public bool ClearSymlinks { get; set; }

        public string Executable { get; set; }

        public string DataFolder { get; set; }

        [System.Text.Json.Serialization.JsonConverter(typeof(DataPathConverter<DataPaths>))]
        public List<DataPaths> DataPaths { get; set; } = new List<DataPaths>();
    }
}
