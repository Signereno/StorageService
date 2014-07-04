using System;
using System.Globalization;
using System.IO;
using System.Linq;
using NUnit.Framework;
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
    }



}