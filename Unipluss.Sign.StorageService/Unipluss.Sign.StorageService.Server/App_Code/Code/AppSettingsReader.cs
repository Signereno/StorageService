using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Runtime.Serialization;
using dotnet.common.files;

namespace Unipluss.Sign.StorageService.Server.Code
{
    public static class AppSettingsReader
    {
        public static string RootFolder
        {
            get
            {
                return ConfigurationManager.AppSettings["rootfolder"].EnsureLastSlashInWindowsPath();
            }
        }

        public static string MoveToFolder
        {
            get { return ConfigurationManager.AppSettings["MoveToFolder"].EnsureLastSlashInWindowsPath(); }
        }

        public static string UrlToken
        {
            get { return ConfigurationManager.AppSettings["urltoken"]; }
        }

        public static string AdminKey
        {
            get { return ConfigurationManager.AppSettings["adminkey"]; }
        }

        public static bool Debug
        {
            get { return Boolean.Parse(ConfigurationManager.AppSettings["debug"]); }
        }


    }

}