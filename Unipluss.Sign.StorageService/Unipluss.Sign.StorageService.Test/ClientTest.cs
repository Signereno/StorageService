using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Unipluss.Sign.StorageService.Client;

namespace Unipluss.Sign.StorageService.Test
{
    [TestFixture]
    public class ClientTest
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
            client = new StorageServiceClient("http://WIN-F6NMPNPRSJM/storage/", "test", "test", 
                "v3TBDIBx15V9B55xkKZ34cQZkh2FdnlXrgtX4wooeatkc1IEKxkwiUSOGyfhSp-YsWiE5W-zQsVEoCnm4ZQXdrGm4cjzY6WOToN7G3yLVEuMPXfJVixoaUPRxsPvi8XR2HsFzdyYAQPPfL0YstFPGl9JrQu7FlnBleMfRUVTrK9pBHkSDT8ngE1nTfURckGK7Bs1czLlQRpBQngFjZTnScR3t2LD");

        }

        [Test]
        public void UploadFileTest1()
        {
            var data = File.ReadAllBytes(fileName1);


            var result = client.UploadFile(data, fileName1,metadata);
            Assert.IsTrue(result);


            var result2 = client.DownloadFile(fileName1);
            Console.WriteLine(result2.FileName);
            foreach (string key in result2.MetaData)
            {
                Console.WriteLine("{0} - {1}",key,result2.MetaData[key]);
            }

        }

    }
}
