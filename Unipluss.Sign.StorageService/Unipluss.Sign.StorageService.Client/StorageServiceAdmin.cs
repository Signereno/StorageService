using System;
using System.IO;
using System.Net;
using System.Text;
using Unipluss.Sign.StorageService.Client.interfaces;

namespace Unipluss.Sign.StorageService.Client
{
    public class StorageServiceAdmin :StorageSerivce, IStorageServiceAdmin
    {
        public StorageServiceAdmin(string serviceUrl, string securityToken) : base(serviceUrl, securityToken)
        {
        }

        public string CreateAccount(string containerName)
        {
            string url = string.Format("{0}Admin?ContainerName={1}", _serviceUrl, containerName);
            WebRequest request = base.CreatePostRequest(url);
            // If required by the server, set the credentials.
            // Get the response.

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            if (response.StatusCode == HttpStatusCode.Created )
            {
                using (Stream stream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                    return reader.ReadToEnd();
                }
            }
            return response.StatusDescription;

        }

        public bool DoesAccountExist(string containerName)
        {
            string url = string.Format("{0}Admin?ContainerName={1}", _serviceUrl, containerName);
            WebRequest request = base.CreateGetRequest(url);
            // If required by the server, set the credentials.
            // Get the response.
           
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            return response.StatusCode == HttpStatusCode.Found;
        }
    }
}