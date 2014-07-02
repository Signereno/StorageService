using System.IO;
using NUnit.Framework;

namespace Unipluss.Sign.StorageService.Test
{
    [TestFixture]
    public class BaseStorageServiceTest
    {
        protected string serviceUrl = "http://WIN-F6NMPNPRSJM/storage/";

        [TestFixtureTearDown]
        public void cleanup()
        {
            if (Directory.Exists((Unipluss.Sign.StorageService.Server.Code.AppSettingsReader.RootFolder)))
                Directory.Delete(Unipluss.Sign.StorageService.Server.Code.AppSettingsReader.RootFolder, true);
            Directory.CreateDirectory(Unipluss.Sign.StorageService.Server.Code.AppSettingsReader.RootFolder);
        }
    }
}