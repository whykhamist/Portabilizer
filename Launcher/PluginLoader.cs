using Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Launcher
{
    public static class PluginLoader
    {
        public static IPlugin LoadPlugin(string library)
        {
            List<IPlugin> Plugin = new List<IPlugin>();
            if (File.Exists(library))
            {
                //Load the DLLs from the Plugins directory
                Assembly.LoadFile(Path.GetFullPath(library));


                Type interfaceType = typeof(IPlugin);
                //Fetch all types that implement the interface IPlugin and are a class
                Type[] types = AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(a => a.GetTypes())
                    .Where(p => interfaceType.IsAssignableFrom(p) && p.IsClass)
                    .ToArray();

                foreach (Type type in types)
                {
                    //Create a new instance of all found types
                    Plugin.Add((IPlugin)Activator.CreateInstance(type));
                }
            }
            return Plugin.FirstOrDefault();
        }
    }
}
