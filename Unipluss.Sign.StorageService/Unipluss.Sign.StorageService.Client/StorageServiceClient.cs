using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Unipluss.Sign.StorageService.Client.Code;
using Unipluss.Sign.StorageService.Client.entities;
using Unipluss.Sign.StorageService.Client.interfaces;

namespace Unipluss.Sign.StorageService.Client
{
    public class StorageServiceClient : StorageSerivce, IStorageServiceClient
    {
        private readonly string _containerName;
        private readonly string _secretKey;

        public StorageServiceClient(string serviceUrl, string securityToken,string containerName,string secretKey) : base(serviceUrl, securityToken)
        {
            _containerName = containerName;
            _secretKey = secretKey;
        }

        public bool UploadFile(byte[] data, string fileName, NameValueCollection metaData)
        {
            string url = string.Format("{0}File?ContainerName={1}&key={2}&filename={3}", _serviceUrl, _containerName, _secretKey, fileName);
            WebRequest request = base.CreatePostRequest(url,Hash.GetSHA1(data));
          

            // add post data to request
            using (Stream postStream = request.GetRequestStream())
            {
                postStream.Write(data, 0, data.Length);
                postStream.Flush();
                postStream.Close();
            }

            //add metadata to header
            if (metaData != null && metaData.HasKeys())
            {
                foreach (string key in metaData.AllKeys)
                {
                    var value = metaData[key];
                    request.Headers.Add(string.Format("x-metadata-{0}", key), value);
                }
            }


            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            return response.StatusCode == HttpStatusCode.Created;
        }

        public bool UploadFile(string filepath, NameValueCollection metaData)
        {
            return UploadFile(File.ReadAllBytes(filepath), Path.GetFileName(filepath), metaData);
        }

        public bool DoesFileExist(string fileName)
        {
            string url = string.Format("{0}File?ContainerName={1}&key={2}&filename={3}", _serviceUrl, _containerName,_secretKey,fileName);
            WebRequest request = base.CreateGetRequest(url);
            try
            {
                HttpWebResponse response = (HttpWebResponse) request.GetResponse();

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

        public bool DeleteFile(string fileName)
        {
            string url = string.Format("{0}File?ContainerName={1}&key={2}&filename={3}", _serviceUrl, _containerName, _secretKey, fileName);
            WebRequest request = base.CreateDeleteRequest(url);
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

        public FileResponse DownloadFile(string fileName)
        {
            string url = string.Format("{0}File/Download?ContainerName={1}&key={2}&filename={3}", _serviceUrl, _containerName, _secretKey, fileName);
            WebRequest request = base.CreateGetRequest(url);
            
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var retur=new FileResponse(){MetaData = new NameValueCollection()};
                    using (Stream stream = response.GetResponseStream())
                    {
                        using (var ms = new MemoryStream())
                        {
                            stream.CopyTo(ms);
                            retur.Bytes = ms.ToArray();

                            foreach (string key in response.Headers.AllKeys.Where(x=>x.StartsWith("x-metadata-")))
                            {
                                var value = response.Headers[key];
                                retur.MetaData.Add(key.Replace("x-metadata-",string.Empty),value);
                            }
                            retur.FileName = response.Headers["x-response-filename"];
                            return retur;

                        }
                    }
            }
            }
           catch (System.Net.WebException ex)
           {
               HttpWebResponse res = (HttpWebResponse)ex.Response;
               if (res.StatusCode == HttpStatusCode.NotFound)
                   return null;
               else
               {
                   throw ex;
               }
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
    }
}