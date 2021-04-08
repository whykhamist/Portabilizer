using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;

namespace Configuration
{
    public static class Extensions
    {
        public static Config Merge(this IConfiguration def, IConfiguration config)
        {
            //foreach (PropertyInfo PI in typeof(IConfiguration).GetProperties())
            //{
            //    def.GetType().GetProperty(PI.Name).SetValue(def, PI.GetValue(config));
            //}
            //return def;
            if (config == null) return (Config)def;

            var configInto = JObject.FromObject(def);
            var userConfig = JObject.FromObject(config);

            configInto.Merge(userConfig, new JsonMergeSettings
            {
                MergeArrayHandling = MergeArrayHandling.Union,
                MergeNullValueHandling = MergeNullValueHandling.Ignore,
                PropertyNameComparison = StringComparison.OrdinalIgnoreCase
            });
            var output = configInto.ToObject<Config>();

            output.DataPaths = def.DataPaths.Merge(config.DataPaths);
            output.Width = (config.Width < 0) ? def.Width : config.Width;
            output.Height = (config.Height < 0) ? def.Height : config.Height;

            return output;
        }

        public static Config Merge(this IConfiguration def, IConfiguration[] config, MergePath mergePath = MergePath.Ascending)
        {
            if (mergePath == MergePath.Descending)
            {
                Array.Reverse(config);
            }

            foreach (IConfiguration conf in config)
            {
                def.Merge(conf);
            }

            return (Config)def;
        }

        public static List<DataPaths> Merge(this List<DataPaths> dataPathMain, List<DataPaths> dataPaths)
        {
            var output = dataPathMain;
            foreach (DataPaths DP in dataPaths)
            {
                var tmp = dataPathMain.Find(x => x.GroupName == DP.GroupName);
                if (tmp != null)
                { output.Remove(tmp); }
                output.Add(DP);
            }
            return output;
        }

        public static DataPaths Merge(this DataPaths dataPathsMain, DataPaths dataPaths)
        {
            var output = dataPathsMain;

            foreach (KeyValuePair<string, string> kvp in dataPaths.Paths)
            {
                output.Paths[kvp.Key] = kvp.Value;
            }

            return output;
        }

        public static Dictionary<string, string> Simplify(this List<DataPaths> dataPaths, string dataFolder)
        {

            var output = new Dictionary<string, string>();

            foreach (DataPaths DP in dataPaths)
            {
                foreach (KeyValuePair<string, string> path in DP.Paths)
                {
                    string source = Environment.ExpandEnvironmentVariables(path.Value);
                    string target = Path.Combine(dataFolder, DP.GroupName, path.Key);
                    output[source] = target;
                }
            }
            return output;
        }
    }
}
