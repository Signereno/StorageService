using System;
using System.Net;
using System.Text;

namespace Unipluss.Sign.StorageService.Client.Code
{
    public static class Extensions
    {
        public static void AddSecurityToken(this WebRequest request,  string token)
        {
            string timetamp = DateTime.UtcNow.ToString("o", System.Globalization.CultureInfo.InvariantCulture);
            request.Headers.Add("timestamp",timetamp);
            string tohash = string.Format("{0}&httpverb={1}&timestamp={2}", request.RequestUri.ToString().ToLowerInvariant(),request.Method.ToLowerInvariant(),timetamp);
        
            request.Headers.Add("token",Hash.GetHash(tohash,token,HashType.SHA512,new UTF8Encoding()));
        }
    }


}
