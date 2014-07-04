using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using NUnit.Framework;
using Unipluss.Sign.StorageService.Client;
using Unipluss.Sign.StorageService.Client.interfaces;

namespace Unipluss.Sign.StorageService.Test
{
    [TestFixture]
    [Ignore]
    public class AdmintLiveTest 
    {

        private IStorageServiceAdmin admin;
        private string fileName1 = "timestamptest.pdf";

        private string urltoken =
            "BNsPmvn3SDsbsx9WEQ6LkM4c7wkxA6U5wk9zBteP9vjEmgrzmCkryd6yXfR6R47rTETuTKt2bFkNH6pvrrJ7r9dGJYsE3xLVpZ93KH3ubsRM7B9jVN7nTYSnXs8PrjaV";

        private string adminkey =
            "BthGmTxEn8E4ks6mmr7t5rx334A26mVdpx5CQWFcCrAUpc97P3NdyjPsccVE3YvYtyJCuWzgfKABASZzY4ENMUT6kXDY7HjfyNWF3vZjPdkvGmTepDdQCmTuQY7AZuJW";

        private NameValueCollection metadata = new NameValueCollection()
        {
            {"md5", "asfasdfkmaslkdfnaskdfnl"},
            {"encrypted", "false"}
        };

        [SetUp]
        public void setup()
        {
            admin = new StorageServiceAdmin("https://secure.signere.no/storage", urltoken,adminkey);
            if (!Directory.Exists((Unipluss.Sign.StorageService.Server.Code.AppSettingsReader.RootFolder)))
                Directory.CreateDirectory(Unipluss.Sign.StorageService.Server.Code.AppSettingsReader.RootFolder);

        }

        [Test]
        public void CreateContainerTest()
        {
            string containerName = Guid.NewGuid().ToString("n");
            string result = admin.CreateContainer(containerName);
            Assert.IsNotNullOrEmpty(result);
            Console.WriteLine("ContainerName: {0}",containerName);
            Console.WriteLine("Key: {0}",result);

            Assert.IsTrue(admin.DoesContainerExist(containerName));

            Assert.AreEqual(result,admin.GetContainerKey(containerName));
        }
    }

    [TestFixture]
    public class ClientLiveTest 
    {

        private IStorageServiceClient client;
        private string fileName1 = "timestamptest.pdf";
        private string urltoken =
          "BNsPmvn3SDsbsx9WEQ6LkM4c7wkxA6U5wk9zBteP9vjEmgrzmCkryd6yXfR6R47rTETuTKt2bFkNH6pvrrJ7r9dGJYsE3xLVpZ93KH3ubsRM7B9jVN7nTYSnXs8PrjaV";

        private NameValueCollection metadata = new NameValueCollection()
        {
            {"md5", "asfasdfkmaslkdfnaskdfnl"},
            {"encrypted", "false"}
        };

        [SetUp]
        public void setup()
        {
            client = new StorageServiceClient("https://secure.signere.no/storage", urltoken, "bce82f81b73546269c0d12e52ff2fc8c", "W8bJ64Jgi4_icG7TXTaK4XTFOgjAwVQ9DWkQxLCZncfsHjBMx67RaP3ZV9vGF6QKRbUDTnrL0ikcvI_JvNEoalJEqUrZjmQChHyl");
        }



        [Test]
        public void UploadFileAndDownloadTheSameFile()
        {
            var data = File.ReadAllBytes(fileName1);
            var testFilename = string.Format("{0}.pdf", Guid.NewGuid().ToString("n"));


            // Create new stopwatch
            Stopwatch stopwatch = new Stopwatch();

            // Begin timing
            stopwatch.Start();

            // Do something
            var result = client.UploadFile(data, testFilename, metadata);

            // Stop timing
            stopwatch.Stop();

            // Write result
            Console.WriteLine("Time elapsed: {0}",
                stopwatch.Elapsed);


            Assert.IsTrue(result, "Upload file");


            Assert.IsTrue(client.DoesFileExist(testFilename), "Does file exists");

            var result2 = client.DownloadFile(testFilename);
            Assert.IsNotNull(result2, "Download file");

            foreach (string key in result2.MetaData)
            {
                Console.WriteLine("{0} - {1}", key, result2.MetaData[key]);
            }
            Console.WriteLine("Filename: {0}", result2.FileName);

            Assert.AreEqual(result2.Bytes, data);
            Assert.AreEqual(result2.FileName, testFilename);

            Assert.AreEqual(2, result2.MetaData.Count);

        }
    }
}