using System;

namespace CreatioClient.Core.Models.Domain
{
    internal interface IConfiguration
    {
        Uri AppUri { get; }
        bool IsNetCore { get; }
        bool UseUntrustedSSL { get; }
    }

    internal class Configuration : IConfiguration
    {
        
        public Configuration(Uri appUri, bool isNetCore, bool useUntrustedSSL)
        {
            AppUri = appUri;
            IsNetCore = isNetCore;
            UseUntrustedSSL = useUntrustedSSL;
        }

        public Uri AppUri { get; }
        public bool IsNetCore { get; }
        public bool UseUntrustedSSL { get; }
    }
}
