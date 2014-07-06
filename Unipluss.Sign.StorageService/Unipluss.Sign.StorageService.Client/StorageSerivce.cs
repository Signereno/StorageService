using System.Net;
using Unipluss.Sign.StorageService.Client.Code;

namespace Unipluss.Sign.StorageService.Client
{
    public class StorageSerivce
    {
        protected readonly string _serviceUrl;
        protected readonly string _securityToken;

        public StorageSerivce(string serviceUrl, string securityToken)
        {
            if (!serviceUrl.EndsWith("/"))
                serviceUrl += "/";
            _serviceUrl = serviceUrl;
            _securityToken = securityToken;


            System.Net.ServicePointManager.ServerCertificateValidationCallback +=
            delegate(object sender, System.Security.Cryptography.X509Certificates.X509Certificate certificate,
                                    System.Security.Cryptography.X509Certificates.X509Chain chain,
                                    System.Net.Security.SslPolicyErrors sslPolicyErrors)
            {
                return true; // **** Always accept
            };
        }

        protected WebRequest CreateGetRequest(string url)
        {
            WebRequest request = WebRequest.Create(url);
            request.Method = "GET";
            request.AddSecurityToken(_securityToken);

            return request;
        }

        protected WebRequest CreateDeleteRequest(string url)
        {
            WebRequest request = WebRequest.Create(url);
            request.Method = "DELETE";
            request.AddSecurityToken(_securityToken);

            return request;
        }

        protected WebRequest CreatePostRequest(string url,string fileCheckSum=null)
        {
            WebRequest request = WebRequest.Create(url);
            request.Method = "POST";
            request.AddSecurityToken(_securityToken,fileCheckSum);
        
            return request;
        }


    }
}