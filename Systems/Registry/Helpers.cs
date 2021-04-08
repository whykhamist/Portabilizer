using System;
using System.Collections.Generic;
using System.Text;

namespace Systems.Registry
{
    public static class Helpers
    {
        public static Dictionary<string, string> ValueFormatReplacement => new Dictionary<string, string>()
        {
            // The full startup location of the portable app
            { "{{StartLocation}}", AppDomain.CurrentDomain.BaseDirectory.Replace("\\", "\\\\") },

            // returns 64 for 64bit OS or 32 for 32bit systems 
            { "{{OSBit}}", Environment.Is64BitOperatingSystem ? "64" : "32" }
        };

        public static string ValueFormat(string value)
        {
            string output = value;
            foreach (KeyValuePair<string, string> kvp in ValueFormatReplacement)
            {
                output = output.Replace(kvp.Key, kvp.Value, StringComparison.OrdinalIgnoreCase);
            }
            return output;
        }

    }
}
