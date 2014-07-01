using System;
using System.Configuration;

namespace Unipluss.Sign.StorageService.Server.Code
{
    public static class AppSettingsReader
    {
        public static string RootFolder
        {
            get
            {
                return ConfigurationManager.AppSettings["rootfolder"] != null ? EnsureLastSlash(ConfigurationManager.AppSettings["rootfolder"]) : null;
            }
        }

        public static string UrlToken
        {
            get
            {
                return ConfigurationManager.AppSettings["urltoken"];
            }
        }

        public static bool Debug
        {
            get
            {
                return Boolean.Parse( ConfigurationManager.AppSettings["debug"]);
            }
        }


        public static string EnsureLastSlash(this string path)
        {
            if (!path.EndsWith("/"))
                path += "/";
            return path;
        }
    }
}