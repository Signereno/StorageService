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
        public void DoesContainerNameWhenItDoesNotTest()
        {
            string containerName = Guid.NewGuid().ToString("n");
            bool result = admin.DoesContainerExist(containerName);
            Assert.False(result);

        }

        [Test]
        public void GetContainerKeyFromContainerThatDoesNotExistThrowsExceptions()
        {
            string containerName = Guid.NewGuid().ToString("n");
            var result = Assert.Throws<Exception>(()=>admin.GetContainerKey(containerName));
            Console.WriteLine(result);
            Assert.IsTrue(result.ToString().Contains(" Statuscode: NotFound"));
        }

        [Test]
        public void CreateContainerWithMetadataTest()
        {
            string containerName = Guid.NewGuid().ToString("n");
            string result = admin.CreateContainer(containerName,new NameValueCollection(){{"name","testname"},{"metakey","metadata"}});
            Assert.IsNotNullOrEmpty(result);

            Console.WriteLine(result);

            var result2 = admin.GetContainerMetaData(containerName);

            Assert.AreEqual(result2.Get("name"),"testname");
            Assert.AreEqual(result2.Get("metakey"), "metadata");

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

            var result = Assert.Throws<Exception>(() => admin.CreateContainer(containerName));
            Assert.IsTrue(result.ToString().Contains("Non authorized for admin request"));
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