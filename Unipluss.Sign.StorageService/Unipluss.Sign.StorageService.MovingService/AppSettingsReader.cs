using System.Configuration;

namespace Unipluss.Sign.StorageService.MovingService
{
    public static class AppSettingsReader
    {
        public static string MoveFromFolder => ConfigurationManager.AppSettings["MoveFromFolder"] != null 
            ? EnsureLastSlash(ConfigurationManager.AppSettings["MoveFromFolder"]) : null;

        public static string MoveToFolder => ConfigurationManager.AppSettings["MoveToFolder"] != null 
            ? EnsureLastSlash(ConfigurationManager.AppSettings["MoveToFolder"]) : null;

        public static int MinutesBeforeMovingFiles
        {
            get
            {
                int result = 60;
                int.TryParse(ConfigurationManager.AppSettings["MinutesBeforeMovingFiles"], out result);
                return result;
            }
        }





        public static string EnsureLastSlash(this string path)
        {
            if (!path.EndsWith("\\"))
                path += "\\";
            return path;
        }
    }

   
}