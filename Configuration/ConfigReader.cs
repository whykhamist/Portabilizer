using Newtonsoft.Json.Linq;
using System.IO;

namespace Configuration
{
    public static class ConfigReader
    {
        public static Config Read(string configFile)
        {
            string content = File.ReadAllText(configFile);
            var userConfig = JObject.Parse(content);
            //var defaultConfig = JObject.FromObject(new Config());
            //defaultConfig.Merge(userConfig, new JsonMergeSettings
            //{
            //    MergeArrayHandling = MergeArrayHandling.Union,
            //    MergeNullValueHandling = MergeNullValueHandling.Ignore,
            //    PropertyNameComparison = StringComparison.OrdinalIgnoreCase
            //});
            return userConfig.ToObject<Config>();
            //var config = JsonConvert.DeserializeObject<Config>(content);
            //return config;
        }
    }
}
