using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Unipluss.Sign.StorageService.Client.entities;

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
            WebRequest request = WebRequest.Create(url);
            request.AddSecurityToken(_securityToken);
            request.Method = "POST";
            // If required by the server, set the credentials.
            // Get the response.

            // add post data to request
            using (Stream postStream = request.GetRequestStream())
            {
                postStream.Write(data, 0, data.Length);
                postStream.Flush();
                postStream.Close();
            }

            foreach (string key in metaData.AllKeys)
            {
                var value = metaData[key];
                request.Headers.Add(string.Format("x-metadata-{0}",key),value);
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
            WebRequest request = WebRequest.Create(url);
            request.AddSecurityToken(_securityToken);
            // If required by the server, set the credentials.
            // Get the response.

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            return response.StatusCode == HttpStatusCode.Found;
        }

        public FileResponse DownloadFile(string fileName)
        {
            string url = string.Format("{0}File/Download?ContainerName={1}&key={2}&filename={3}", _serviceUrl, _containerName, _secretKey, fileName);
            WebRequest request = WebRequest.Create(url);
            request.AddSecurityToken(_securityToken);
            // If required by the server, set the credentials.
            // Get the response.

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            if (response.StatusCode == HttpStatusCode.OK)
            {
                FileResponse retur=new FileResponse(){MetaData = new NameValueCollection()};
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
                        return retur;

                    }
                }
            }
            return null;
        }
    }
}