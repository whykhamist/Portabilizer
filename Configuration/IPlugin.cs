using System;

namespace Configuration
{
    public interface IPlugin
    {
        IConfiguration Config { get; }

        IFix Fix { get; }

        string RegistryContent { get; }

        byte[] IconBytes { get; }
    }
}
