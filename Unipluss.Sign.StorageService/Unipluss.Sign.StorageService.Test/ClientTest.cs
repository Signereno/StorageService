using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
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
    public class MiscTest
    {
        [Test]
        public void ParseDate()
        {
            var timestamp = DateTime.UtcNow.ToString("o");//"2014-07-03T08:45:33.7403013Z";
            DateTime httpTimestamp = DateTime.MinValue;

          DateTime.TryParseExact(timestamp, "o", CultureInfo.InvariantCulture,DateTimeStyles.RoundtripKind,out httpTimestamp);


          httpTimestamp=  httpTimestamp.AddMinutes(-11);
            Console.WriteLine(timestamp);
            Console.WriteLine(httpTimestamp.ToString("o", System.Globalization.CultureInfo.InvariantCulture));
            Console.WriteLine(DateTime.UtcNow.ToString("o", System.Globalization.CultureInfo.InvariantCulture));

           

            if (DateTime.UtcNow.AddMinutes(10) < httpTimestamp || DateTime.UtcNow.AddMinutes(-10) > httpTimestamp)
            {
                Console.WriteLine("Ut av sync");
            }
        }
    }
}
