using System.Configuration;
using System.IO;
using NUnit.Framework;

namespace Unipluss.Sign.StorageService.Test
{
    [TestFixture]
    public class BaseStorageServiceTest
    {
        protected string serviceUrl = ConfigurationManager.AppSettings["serviceurl"];

        [TearDown]
        public void cleanup()
        {
            if (Directory.Exists((Unipluss.Sign.StorageService.Server.Code.AppSettingsReader.RootFolder)))
                clearFolder(Unipluss.Sign.StorageService.Server.Code.AppSettingsReader.RootFolder);
            Directory.CreateDirectory(Unipluss.Sign.StorageService.Server.Code.AppSettingsReader.RootFolder);
        }

        private void clearFolder(string FolderName)
        {
            DirectoryInfo dir = new DirectoryInfo(FolderName);

            foreach (FileInfo fi in dir.GetFiles())
            {
                fi.Delete();
            }

            foreach (DirectoryInfo di in dir.GetDirectories())
            {
                clearFolder(di.FullName);
                di.Delete();
            }
        }
    }
}