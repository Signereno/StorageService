using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Text;
using NUnit.Framework;
using Unipluss.Sign.StorageService.Client;
using Unipluss.Sign.StorageService.Client.interfaces;
using Unipluss.Sign.StorageService.Server.Code;

namespace Unipluss.Sign.StorageService.Test
{
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

        [SetUp]
        public void setup()
        {
            if (!Directory.Exists((Unipluss.Sign.StorageService.Server.Code.AppSettingsReader.RootFolder)))
                Directory.CreateDirectory(Unipluss.Sign.StorageService.Server.Code.AppSettingsReader.RootFolder);
            var admin = new StorageServiceAdmin(base.serviceUrl, "test",AppSettingsReader.AdminKey);
              var key=admin.CreateContainer("test");

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
        public void DoesFileExistThatDoesNotTest()
        {
            var filename = string.Format("{0}.pdf", Guid.NewGuid().ToString("n"));
            Assert.IsFalse(client.DoesFileExist(filename));
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

            Assert.AreEqual(result2.Bytes,data);
            Assert.AreEqual(result2.FileName,testFilename);

            Assert.AreEqual(2, result2.MetaData.Count);

        }

     
    }
}
