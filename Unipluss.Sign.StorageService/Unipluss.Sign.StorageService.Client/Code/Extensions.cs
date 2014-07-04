using System;
using System.Net;
using System.Text;

namespace Unipluss.Sign.StorageService.Client.Code
{
    public static class Extensions
    {
        public static void AddSecurityToken(this WebRequest request, string token, string fileCheckSum=null)
        {
            string timetamp = DateTime.UtcNow.ToString("o", System.Globalization.CultureInfo.InvariantCulture);
            request.Headers.Add("timestamp",timetamp);
            string tohash = string.Format("{0}&httpverb={1}&timestamp={2}", request.RequestUri.ToString().ToLowerInvariant(),request.Method.ToLowerInvariant(),timetamp);
            if (!string.IsNullOrWhiteSpace(fileCheckSum))
            {
                tohash += string.Format("&filesha1={0}", fileCheckSum);
                request.Headers.Add("filesha1",fileCheckSum);
            }
            request.Headers.Add("token",Hash.GetHash(tohash,token,HashType.SHA512,new UTF8Encoding()));
        }
    }


}
