using Configuration;
using System.IO;

namespace Launcher
{
    public static class Factory
    {
        public static IPlugin Plugin { get { return PluginLoader.LoadPlugin(Constants.PortableLib); } }

        public static Config UserConfig
        {
            get
            {
                Config uConfig = null;
                if (File.Exists(Constants.ConfigFile))
                {
                    uConfig = ConfigReader.Read(Constants.ConfigFile);
                }
                return uConfig;
            }
        }

        public static Config Config { get; set; } = new Configuration.Configuration
        {
            Title = "JhyRish Portable App",
            Width = 550,
            Height = 110,
            DataFolder = "Portable"
        };

    }
}
