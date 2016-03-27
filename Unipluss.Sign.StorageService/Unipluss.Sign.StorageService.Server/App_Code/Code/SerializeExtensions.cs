using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace System.Runtime.CompilerServices
{
    public class ExtensionAttribute : Attribute { }
}

namespace Unipluss.Sign.StorageService.Server.Code
{
    public static class SerializeExtensions
    {

        public static void Serialize(this NameValueCollection metaData, string filepath)
        {
            File.WriteAllText(filepath, JsonConvert.SerializeObject(metaData.AllKeys.ToDictionary(x => x, x => metaData[x])));
        }

        public static NameValueCollection DeSerialize(this string filepath)
        {
            var dict = JsonConvert.DeserializeObject<IDictionary<string, string>>(File.ReadAllText(filepath));

            var collection = new NameValueCollection();
            foreach (var pair in dict)
                collection.Add(pair.Key, pair.Value.ToString());
            return collection;
        }

    }
}