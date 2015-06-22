using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using Unipluss.Sign.StorageService.Client;
using Unipluss.Sign.StorageService.Client.interfaces;

namespace Unipluss.Sign.StorageService.Test
{
    [TestFixture]
    public class ClientLiveTest 
    {
        private IList<string> files = new List<string>();
        private IStorageServiceClient client;
        private string fileName1 = "timestamptest.pdf";
        private string fileName2 = "DUMMYDOCUMENT.pdf";
        private string fileName3 = "test.pdf";

        private string urltoken =
            "pFjWpfNwyfahFUpHvuEyaad7ySc4XQ2F7CbyUqcMXKZwr8k5TM2YfdqDx4JM8naLyp45FbFKZHNEevDHNzaUBshKdur5TdG7zVzWfbx5GB8DxY53WJgsL4AnmrVScfyn";

        private NameValueCollection metadata = new NameValueCollection()
            {
                {"md5", "asfasdfkmaslkdfnaskdfnl"},
                {"encrypted", "false"}
            };

        [SetUp]
        public void setup()
        {
            client = new StorageServiceClient("https://secure.signere.no/storagetest", urltoken, "unittest",
                                              "KZSkSrYuFJMzeA6VYgu2xPeXLDMA8unJnX5nkC3vb6K3NkHbRb4XtJ4fQS96T3PE2eM52uxZNLhkfyfB5zFb3HW2SThLsHGy2N5S");
        }

        [Test]
        public void UploadFileAndDownloadTheSameFile()
        {
            var data = File.ReadAllBytes(fileName1);
            var testFilename = string.Format("{0}.pdf", Guid.NewGuid().ToString("n"));
            files.Add(testFilename);

            // Create new stopwatch
            Stopwatch stopwatch = new Stopwatch();

            // Begin timing
            stopwatch.Start();

            // Do something
            var result = client.UploadFile(data, testFilename, metadata);

            // Stop timing
            stopwatch.Stop();

            // Write result
            Console.WriteLine("Time elapsed: {0}", stopwatch.Elapsed);

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

        [Test]
        public async void UploadAndDownloadMultipleFilesTest()
        {
            var data1 = File.ReadAllBytes(fileName1);
            var data2 = File.ReadAllBytes(fileName2);
            var data3 = File.ReadAllBytes(fileName3);

            var tasks1 = new List<Task>();
            var tasks2 = new List<Task>();
            var tasks3 = new List<Task>();

            // Create new stopwatch
            Stopwatch stopwatch = new Stopwatch();

            // Begin timing
            stopwatch.Start();

            for (int i = 0; i < 3000; i++)
            {
                var filename = fileName1 + i;
                files.Add(filename);
                // Do something
                tasks1.Add(Task.Factory.StartNew(async () =>
                {
                    await Task.Delay(TimeSpan.FromSeconds(new Random(DateTime.Now.Millisecond).Next(2, 5)));
                    
                    var uploadresult = client.UploadFile(data1, filename, metadata);
                    Assert.IsTrue(uploadresult);
                    Assert.IsTrue(client.DoesFileExist(filename));

                    var downloadresult = client.DownloadFile(filename);
                    Assert.IsNotNull(downloadresult, "Download file");
                }).Unwrap());
            }

            for (int i = 10000; i < 11000; i++)
            {
                var filename = fileName2 + i;
                files.Add(filename);
                // Do something
                tasks2.Add(Task.Factory.StartNew(async () =>
                {
                    await Task.Delay(TimeSpan.FromSeconds(new Random(DateTime.Now.Millisecond).Next(2, 5)));

                    var uploadresult = client.UploadFile(data2, filename, metadata);
                    Assert.IsTrue(uploadresult);
                    Assert.IsTrue(client.DoesFileExist(filename));

                    var downloadresult = client.DownloadFile(filename);
                    Assert.IsNotNull(downloadresult, "Download file");
                }).Unwrap());
            }

            for (int i = 30000; i < 30500; i++)
            {
                var filename = fileName3 + i;
                files.Add(filename);
                // Do something
                tasks3.Add(Task.Factory.StartNew(async () =>
                {
                    await Task.Delay(TimeSpan.FromSeconds(new Random(DateTime.Now.Millisecond).Next(2, 5)));

                    var uploadresult = client.UploadFile(data3, filename, metadata);
                    Assert.IsTrue(uploadresult);
                    Assert.IsTrue(client.DoesFileExist(filename));

                    var downloadresult = client.DownloadFile(filename);
                    Assert.IsNotNull(downloadresult, "Download file");
                }).Unwrap());
            }

            //Task.WaitAll(tasks.ToArray());
            await Task.WhenAll(Task.WhenAll(tasks1), Task.WhenAll(tasks2), Task.WhenAll(tasks3));

            // Stop timing
            stopwatch.Stop();

            // Write result
            Console.WriteLine("Time elapsed: {0}", stopwatch.Elapsed);
        }

        [TearDown]
        public void Cleanup() {
            foreach (string file in files) {
                client.DeleteFile(file);
            }
            files = new List<string>();
        }
    }
}