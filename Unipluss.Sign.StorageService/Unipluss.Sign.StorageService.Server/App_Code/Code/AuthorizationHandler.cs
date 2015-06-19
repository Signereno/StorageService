using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using System.Web;

namespace Unipluss.Sign.StorageService.Server.Code
{
    public static class AuthorizationHandler
    {
        public static bool VerifyIfRequestIsAuthed(HttpContext context, bool adminAccess = false)
        {
            context.Response.Headers["Server"] = "SignereStorage 1.0";
            var token = context.Request.Headers["token"];
            var url = context.Request.Url.ToString();
            var timestamp = context.Request.Headers["timestamp"];
            string tohash = string.Format("{0}&httpverb={1}&timestamp={2}", url.ToLowerInvariant(), context.Request.HttpMethod.ToLowerInvariant(), timestamp);

            if (context.Request.Headers["filesha1"] != null &&
                !string.IsNullOrWhiteSpace(context.Request.Headers["filesha1"]))
            {
                tohash += string.Format("&filesha1={0}", context.Request.Headers["filesha1"]);
            }

            bool requestIsAuthed = true;

            //If Timestamp is missing
            if (string.IsNullOrWhiteSpace(timestamp))
            {
                context.Response.Write("Timestamp is missing");
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                requestIsAuthed = false;
            }
            //2014-07-03T08:45:33.7403013Z
            DateTime httpTimestamp = DateTime.MinValue;
            DateTime.TryParseExact(timestamp, "o", CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out httpTimestamp);

            //If timestamp does not parse correctly
            if (httpTimestamp.Equals(DateTime.MinValue))
            {
                context.Response.Write("Timestamp is not in ISO 8601 format.");
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                requestIsAuthed = false;
            }

            //If timestamp is more than 10 minutes of with the server clock
            if (DateTime.UtcNow.AddMinutes(10) < httpTimestamp || DateTime.UtcNow.AddMinutes(-10) > httpTimestamp)
            {
                context.Response.Write(string.Format("Timestamp have expired or out of sync with server clock. ({0})", DateTime.UtcNow.ToString("o", System.Globalization.CultureInfo.InvariantCulture)));
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                requestIsAuthed = false;
            }

            //If security token is correct
            if (string.IsNullOrWhiteSpace(token) || !token.Equals(Unipluss.Sign.StorageService.Server.Code.Hash.GetHash(tohash, AppSettingsReader.UrlToken, HashType.SHA512, new UTF8Encoding())))
            {
                context.Response.Write("Non authorized request");
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                requestIsAuthed = false;
            }

            if (adminAccess && !context.Request.QueryString["adminkey"].Equals(AppSettingsReader.AdminKey))
            {
                context.Response.Write("Non authorized for admin request");
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                requestIsAuthed = false;
            }

            return requestIsAuthed;
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
}
