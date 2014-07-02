using System.Net;
using System.Text;

namespace Unipluss.Sign.StorageService.Client.Code
{
    public static class Extensions
    {
        public static void AddSecurityToken(this WebRequest request,  string token)
        {
            request.Headers.Add("token",Hash.GetHash(request.RequestUri.ToString().ToLowerInvariant(),token,HashType.SHA512,new UTF8Encoding()));
        }
    }


}
