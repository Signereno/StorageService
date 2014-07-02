using System.Net;

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
            request.AddSecurityToken(_securityToken);

            return request;
        }

        protected WebRequest CreatePostRequest(string url)
        {
            WebRequest request = WebRequest.Create(url);
            request.AddSecurityToken(_securityToken);
            request.Method = "POST";
            return request;
        }


    }
}