using System;
using System.IO;
using System.Net;
using System.Text;
using Unipluss.Sign.StorageService.Client.interfaces;

namespace Unipluss.Sign.StorageService.Client
{
    public class StorageServiceAdmin :StorageSerivce, IStorageServiceAdmin
    {
        private readonly string _adminkey;

        public StorageServiceAdmin(string serviceUrl, string securityToken,string adminkey) : base(serviceUrl, securityToken)
        {
            _adminkey = adminkey;
        }

        public string CreateContainer(string containerName)
        {
            string url = string.Format("{0}Admin/Container/Create?ContainerName={1}&adminkey={2}", _serviceUrl, containerName, _adminkey);
            WebRequest request = base.CreatePostRequest(url);
            // If required by the server, set the credentials.
            // Get the response.
            
            request.ContentLength = 0;
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            if (response.StatusCode == HttpStatusCode.Created )
            {
                using (Stream stream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                    return reader.ReadToEnd();
                }
            }
            return null;

        }

        public bool DoesContainerExist(string containerName)
        {
            string url = string.Format("{0}Admin/Container?ContainerName={1}&adminkey={2}", _serviceUrl, containerName, _adminkey);
            WebRequest request = base.CreateGetRequest(url);
            // If required by the server, set the credentials.
            // Get the response.
           
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            return response.StatusCode == HttpStatusCode.OK;
        }

        public string GetContainerKey(string containerName)
        {
            string url = string.Format("{0}Admin/Container/Key?ContainerName={1}&adminkey={2}", _serviceUrl, containerName, _adminkey);
            WebRequest request = base.CreateGetRequest(url);
            // If required by the server, set the credentials.
            // Get the response.

            request.ContentLength = 0;
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            if (response.StatusCode == HttpStatusCode.OK)
            {
                using (Stream stream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                    return reader.ReadToEnd();
                }
            }
            return response.StatusDescription;
        }
    }
}