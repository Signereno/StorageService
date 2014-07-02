using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Policy;
using System.Text;
using System.Web;

namespace Unipluss.Sign.StorageService.Server.Code
{
    public  static class AuthorizationHandler
    {

        public static bool VerifyIfRequestIsAuthed(this HttpContext context)
        {
            var token = context.Request.Headers["token"];
            var url = context.Request.Url.ToString();
            string tohash = string.Format("{0}&httpverb={1}&timestamp={2}", url.ToLowerInvariant(), context.Request.HttpMethod.ToLowerInvariant(), context.Request.Headers["timestamp"]);
        

            if (string.IsNullOrWhiteSpace(token) || !token.Equals(Unipluss.Sign.StorageService.Server.Code.Hash.GetHash(tohash, AppSettingsReader.UrlToken, HashType.SHA512, new UTF8Encoding())))
            {
                context.Response.Write("Non authorized request");
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                context.Response.End();
                return false;
            }


            return true;
        }

        public static bool CheckIfFilenameIsValid(string fileName)
        {
            return !string.IsNullOrWhiteSpace(fileName) &&
                   fileName.IndexOfAny(Path.GetInvalidFileNameChars()) < 0;
        }

        public static bool CheckIfFolderNameIsInvalid(string folderName)
        {
            return !string.IsNullOrWhiteSpace(folderName) &&
                   folderName.IndexOfAny(Path.GetInvalidPathChars()) < 0;
        }
    }

    public static class Extensions
    {
        public static void Serialize(this NameValueCollection metaData, string filepath)
        {
            Stream stream = File.Open(filepath, FileMode.Create);
            BinaryFormatter bFormatter = new BinaryFormatter();
            bFormatter.Serialize(stream, metaData);
            stream.Close(); 
        }

        public static NameValueCollection DeSerialize(this string filepath)
        {
            NameValueCollection objectToSerialize;
            Stream stream = File.Open(filepath, FileMode.Open);
            BinaryFormatter bFormatter = new BinaryFormatter();
            objectToSerialize = (NameValueCollection)bFormatter.Deserialize(stream);
            stream.Close();
            return objectToSerialize;
        }
    }
}
