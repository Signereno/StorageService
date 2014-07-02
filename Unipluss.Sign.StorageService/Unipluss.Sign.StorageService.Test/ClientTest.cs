using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Unipluss.Sign.StorageService.Client;
using Unipluss.Sign.StorageService.Client.interfaces;
using Unipluss.Sign.StorageService.Server.Code;

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


    [TestFixture]
    public class ClientTest : BaseStorageServiceTest
    {

        private IStorageServiceClient client;
        private string fileName1 = "timestamptest.pdf";

        private NameValueCollection metadata = new NameValueCollection()
        {
            {"md5", "asfasdfkmaslkdfnaskdfnl"},
            {"encrypted", "false"}
        };

        [TestFixtureSetUp]
        public void setup()
        {
            if (!Directory.Exists((Unipluss.Sign.StorageService.Server.Code.AppSettingsReader.RootFolder)))
                Directory.CreateDirectory(Unipluss.Sign.StorageService.Server.Code.AppSettingsReader.RootFolder);
            var admin = new StorageServiceAdmin(base.serviceUrl, "test");
              var key=admin.CreateAccount("test");

              client = new StorageServiceClient(base.serviceUrl, AppSettingsReader.UrlToken, "test", key);
        }

  

        [Test]
        public void UploadFileTest1()
        {
            var data = File.ReadAllBytes(fileName1);
            
            var result = client.UploadFile(data, fileName1,metadata);
            Assert.IsTrue(result);
            

        }


  

        [Test]
        public void UploadFileAndDownloadTheSameFile()
        {
            var data = File.ReadAllBytes(fileName1);
            var testFilename = string.Format("{0}.pdf", Guid.NewGuid().ToString("n"));

            var result = client.UploadFile(data, testFilename, metadata);
            Assert.IsTrue(result,"Upload file");


            Assert.IsTrue(client.DoesFileExist(testFilename),"Does file exists");

            var result2 = client.DownloadFile(testFilename);
            Assert.IsNotNull(result2,"Download file");

            foreach (string key in result2.MetaData)
            {
                Console.WriteLine("{0} - {1}", key, result2.MetaData[key]);
            }
            Console.WriteLine("Filename: {0}",result2.FileName);

        }
    }

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

        [TestFixtureSetUp]
        public void setup()
        {
            admin = new StorageServiceAdmin(base.serviceUrl,AppSettingsReader.UrlToken);
            if (!Directory.Exists((Unipluss.Sign.StorageService.Server.Code.AppSettingsReader.RootFolder)))
                Directory.CreateDirectory(Unipluss.Sign.StorageService.Server.Code.AppSettingsReader.RootFolder);

        }

        [Test]
        public void CreateContainerTest()
        {
            string containerName = Guid.NewGuid().ToString("n");
            string result = admin.CreateAccount(containerName);
            Assert.IsNotNullOrEmpty(result);

            Console.WriteLine(result);

        }

        [Test]
        public void CreateContainerAnd_Test_if_it_exists_afterwards()
        {
            string containerName = Guid.NewGuid().ToString("n");
            string result = admin.CreateAccount(containerName);
            Assert.IsNotNullOrEmpty(result);

            Console.WriteLine(result);


            var result2 = admin.DoesAccountExist(containerName);
            Assert.IsTrue(result2);


        }


    }
}
