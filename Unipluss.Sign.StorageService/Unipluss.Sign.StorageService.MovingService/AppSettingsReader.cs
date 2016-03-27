using System.Configuration;
using dotnet.common.files;
using dotnet.common.numbers;

namespace Unipluss.Sign.StorageService.MovingService
{
    public static class AppSettingsReader
    {
        public static string MoveFromFolder => ConfigurationManager.AppSettings["MoveFromFolder"].EnsureLastSlashInWindowsPath();

        public static string MoveToFolder => ConfigurationManager.AppSettings["MoveToFolder"].EnsureLastSlashInWindowsPath();

        public static int MinutesBeforeMovingFiles => ConfigurationManager.AppSettings["MinutesBeforeMovingFiles"].ParseInt(60);

        public static string CertificateThumbprint => ConfigurationManager.AppSettings["CertificateThumbprint"];

    }

   
}