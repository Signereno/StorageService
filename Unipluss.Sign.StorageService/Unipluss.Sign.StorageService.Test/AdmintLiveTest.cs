using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using NUnit.Framework;
using Unipluss.Sign.StorageService.Client;
using Unipluss.Sign.StorageService.Client.interfaces;

namespace Unipluss.Sign.StorageService.Test
{
    [TestFixture]
    [Ignore]
    public class AdmintLiveTest 
    {
        private IList<string> containers=new List<string>();
        private IStorageServiceAdmin admin;
        private string fileName1 = "timestamptest.pdf";

        private string urltoken =
            "pFjWpfNwyfahFUpHvuEyaad7ySc4XQ2F7CbyUqcMXKZwr8k5TM2YfdqDx4JM8naLyp45FbFKZHNEevDHNzaUBshKdur5TdG7zVzWfbx5GB8DxY53WJgsL4AnmrVScfyn";

        private string adminkey =
            "kf3EtUVFYG9sM23fgXT6WPxVE4teQwRueEfPLrwNNn3zyYkAMGxAzQWgSRm33exn9ZshDYkQYgbYaV2HDCxVaL7x2chULyyb6md8R7MtMp8pyu9xXH9gYvvUUfLV3VvF";

        private NameValueCollection metadata = new NameValueCollection()
        {
            {"md5", "asfasdfkmaslkdfnaskdfnl"},
            {"encrypted", "false"}
        };

        [SetUp]
        public void setup()
        {
            admin = new StorageServiceAdmin("https://secure.signere.no/storagetest", urltoken,adminkey);           

        }

        [Test]
        public void CreateContainerTest()
        {
            string containerName = Guid.NewGuid().ToString("n");
            containers.Add(containerName);
            string result = admin.CreateContainer(containerName);
            Assert.IsNotNullOrEmpty(result);
            Console.WriteLine("ContainerName: {0}",containerName);
            Console.WriteLine("Key: {0}",result);

            Assert.IsTrue(admin.DoesContainerExist(containerName));

            Assert.AreEqual(result,admin.GetContainerKey(containerName));
        }

        [TearDown]
        public void cleanup()
        {
            foreach (string container in containers)
            {
                admin.DeleteContainer(container);
            }
            containers=new List<string>();
        }
    }
}