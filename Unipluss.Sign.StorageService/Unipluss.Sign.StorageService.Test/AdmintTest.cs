using System;
using System.Collections.Specialized;
using System.IO;
using NUnit.Framework;
using Unipluss.Sign.StorageService.Client;
using Unipluss.Sign.StorageService.Client.interfaces;
using Unipluss.Sign.StorageService.Server.Code;

namespace Unipluss.Sign.StorageService.Test
{
    [TestFixture]
    public class AdmintTest : BaseStorageServiceTest
    {

        private IStorageServiceAdmin admin;
        private string fileName1 = "timestamptest.pdf";

        private NameValueCollection metadata = new NameValueCollection()
        {
            {"md5", "asfasdfkmaslkdfnaskdfnl"},
            {"encrypted", "false"}
        };

        [SetUp]
        public void setup()
        {
            admin = new StorageServiceAdmin(base.serviceUrl,AppSettingsReader.UrlToken,AppSettingsReader.AdminKey);
            if (!Directory.Exists((Unipluss.Sign.StorageService.Server.Code.AppSettingsReader.RootFolder)))
                Directory.CreateDirectory(Unipluss.Sign.StorageService.Server.Code.AppSettingsReader.RootFolder);

        }

        [Test]
        public void CreateContainerTest()
        {
            string containerName = Guid.NewGuid().ToString("n");
            string result = admin.CreateContainer(containerName);
            Assert.IsNotNullOrEmpty(result);

            Console.WriteLine(result);

        }

        [Test]
        public void CreateContainerRandom_and_get_key_and_check_that_they_match()
        {
            string containerName = Guid.NewGuid().ToString("n");
            string result = admin.CreateContainer(containerName);
            Assert.IsNotNullOrEmpty(result);

           Assert.AreEqual(result,admin.GetContainerKey(containerName));

        }


        [Test]
        public void CreateContainerWithIncorrectAdminKeyTest()
        {
            admin = new StorageServiceAdmin(base.serviceUrl, AppSettingsReader.UrlToken, "test");
            string containerName = Guid.NewGuid().ToString("n");

            var result = Assert.Throws<System.Net.WebException>(() => admin.CreateContainer(containerName));
          Assert.IsTrue(result.ToString().Contains("401"));
            Console.WriteLine(result);

        }

        [Test]
        public void CreateContainerAnd_Test_if_it_exists_afterwards()
        {
            string containerName = Guid.NewGuid().ToString("n");
            string result = admin.CreateContainer(containerName);
            Assert.IsNotNullOrEmpty(result);

            Console.WriteLine(result);


            var result2 = admin.DoesContainerExist(containerName);
            Assert.IsTrue(result2);


        }


    }
}