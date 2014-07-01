using System.IO;
using System.Net;
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
            if (string.IsNullOrWhiteSpace(token) || !token.Equals(Unipluss.Sign.StorageService.Server.Code.Hash.GetHash(url,AppSettingsReader.UrlToken,HashType.SHA512,new UTF8Encoding())) )
            {
                context.Response.Write("Non authorized request");
                context.Response.StatusCode=(int) HttpStatusCode.Unauthorized  ;
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
}
