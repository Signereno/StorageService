using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using dotnet.common.hash;
using NUnit.Framework;
using Unipluss.Sign.StorageService.Client.Code;
using Unipluss.Sign.StorageService.Server.Code;

namespace Unipluss.Sign.StorageService.Test
{
    [TestFixture]
    [Ignore]
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

        [Test]
        public void GetSubDirectory()
        {
            var directories =
                Directory.GetDirectories(string.Format(@"{0}{1}", AppSettingsReader.RootFolder, "test"));
            foreach (string directory in directories)
            {
                Console.WriteLine(directory);
            }
            Console.WriteLine(new DirectoryInfo(directories.FirstOrDefault()).Name);
            Console.WriteLine(directories.FirstOrDefault());
            Console.WriteLine(Path.GetDirectoryName(directories.FirstOrDefault()+"\\"));

        }

        [Test]
        public void TestHashIsEqual()
        {
            string key = Guid.NewGuid().ToString("n");
            string text =
                "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vivamus auctor fermentum lacus, a consequat ex. Maecenas id sodales risus, sed sagittis est. Praesent consequat viverra placerat. Vivamus mollis mauris id ex facilisis, at hendrerit lacus sagittis. Ut sit amet imperdiet lorem, quis blandit enim. Cras blandit orci id risus rhoncus, non ornare turpis lacinia. Phasellus consequat euismod eros nec rutrum. Curabitur imperdiet, mauris quis efficitur molestie, metus justo condimentum ex, at volutpat ligula purus id arcu. Nulla laoreet mi quis mi fringilla molestie. Suspendisse potenti. Proin tincidunt eros nisi, sit amet ultrices ante scelerisque nec.";

            Assert.AreEqual(Hash.GetHash(text,key,HashType.SHA512),text.ToHmacSha512(key,HashFormat.HEX,new UnicodeEncoding()));
            Assert.AreEqual(Hash.GetHash(text, key, HashType.SHA256), text.ToHmacSha256(key, HashFormat.HEX, new UnicodeEncoding()));
            Assert.AreEqual(Hash.GetHash(text, key, HashType.SHA1), text.ToHmacSha1(key, HashFormat.HEX, new UnicodeEncoding()));
        }
    }



}