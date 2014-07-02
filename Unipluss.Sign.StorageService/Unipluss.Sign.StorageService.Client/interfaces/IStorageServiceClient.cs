using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using Unipluss.Sign.StorageService.Client.entities;

namespace Unipluss.Sign.StorageService.Client
{
    public interface IStorageServiceClient{

        bool UploadFile(byte[] data, string filename,NameValueCollection metaData);
        bool UploadFile(string filepath,NameValueCollection metaData);

        bool DoesFileExist(string fileName);

        FileResponse DownloadFile(string fileName);

    }
}
