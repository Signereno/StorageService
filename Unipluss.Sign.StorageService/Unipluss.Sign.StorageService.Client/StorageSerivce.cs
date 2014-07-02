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


    }
}