using System;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
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

        public string CreateContainer(string containerName, NameValueCollection metaData = null)
        {
            string url = string.Format("{0}Admin/Container/Create?ContainerName={1}&adminkey={2}", _serviceUrl, containerName, _adminkey);
            WebRequest request = base.CreatePostRequest(url);
            // If required by the server, set the credentials.
            // Get the response.

            //add metadata to header
            if (metaData != null && metaData.HasKeys())
            {
                foreach (string key in metaData.AllKeys)
                {
                    var value = metaData[key];
                    request.Headers.Add(string.Format("x-metadata-{0}", key), value);
                }
            }
            
            request.ContentLength = 0;
            try
            {
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            if (response.StatusCode == HttpStatusCode.Created )
            {
                using (Stream stream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                    return reader.ReadToEnd();
                }
            }
            }
            catch (System.Net.WebException ex)
            {
                HttpWebResponse res = (HttpWebResponse)ex.Response;
                using (Stream stream = res.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                    throw new Exception(string.Format("Statuscode: {0} - {1}", res.StatusCode, reader.ReadToEnd()));
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
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                return response.StatusCode == HttpStatusCode.OK;
            }
            catch (System.Net.WebException ex)
            {
                HttpWebResponse res = (HttpWebResponse) ex.Response;
                if (res.StatusCode == HttpStatusCode.NotFound)
                    return false;
                else
                {
                    throw ex;
                }
            }


            return false;
        }

        public string GetContainerKey(string containerName)
        {
            string url = string.Format("{0}Admin/Container/Key?ContainerName={1}&adminkey={2}", _serviceUrl, containerName, _adminkey);
            WebRequest request = base.CreateGetRequest(url);
            // If required by the server, set the credentials.
            // Get the response.

            request.ContentLength = 0;
            try
            {
                HttpWebResponse response = (HttpWebResponse) request.GetResponse();

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    using (Stream stream = response.GetResponseStream())
                    {
                        StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                        return reader.ReadToEnd();
                    }
                }

            }
            catch (System.Net.WebException ex)
            {
                HttpWebResponse res = (HttpWebResponse)ex.Response;
                throw new Exception(string.Format("Statuscode: {0} - {1}",res.StatusCode,res.StatusDescription));
            }


            return null;
        }

        public NameValueCollection GetContainerMetaData(string containerName)
        {
            string url = string.Format("{0}Admin/Container/MetaData?ContainerName={1}&adminkey={2}", _serviceUrl, containerName, _adminkey);
            WebRequest request = base.CreateGetRequest(url);
            // If required by the server, set the credentials.
            // Get the response.

            request.ContentLength = 0;
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var retur = new NameValueCollection();
                    foreach (string key in response.Headers.AllKeys.Where(x => x.StartsWith("x-metadata-")))
                    {
                        var value = response.Headers[key];
                        retur.Add(key.Replace("x-metadata-", string.Empty), value);
                    }
                    return retur;
                }

            }
            catch (System.Net.WebException ex)
            {
                HttpWebResponse res = (HttpWebResponse)ex.Response;
                throw new Exception(string.Format("Statuscode: {0} - {1}", res.StatusCode, res.StatusDescription));
            }


            return null;
        }

        public bool TestConnection()
        {
            string url = string.Format("{0}Test", _serviceUrl);
            WebRequest request = base.CreateGetRequest(url);
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                return response.StatusCode == HttpStatusCode.OK;
            }
            catch (System.Net.WebException ex)
            {
                HttpWebResponse res = (HttpWebResponse)ex.Response;
                if (res.StatusCode == HttpStatusCode.NotFound)
                    return false;
                else
                {
                    throw ex;
                }
            }

            return false;
        }

        public bool DeleteContainer(string containerName)
        {
            string url = string.Format("{0}Admin/Container?ContainerName={1}&adminkey={2}", _serviceUrl, containerName, _adminkey);

            WebRequest request = base.CreateDeleteRequest(url);
            // If required by the server, set the credentials.
            // Get the response.

            request.ContentLength = 0;
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                return response.StatusCode == HttpStatusCode.OK;
             
            }
            catch (System.Net.WebException ex)
            {
                HttpWebResponse res = (HttpWebResponse)ex.Response;
                throw new Exception(string.Format("Statuscode: {0} - {1}", res.StatusCode, res.StatusDescription));
            }

            return false;
        }
    }
}