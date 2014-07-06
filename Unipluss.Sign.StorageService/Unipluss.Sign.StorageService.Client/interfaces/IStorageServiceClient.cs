using System.Collections.Specialized;
using Unipluss.Sign.StorageService.Client.entities;

namespace Unipluss.Sign.StorageService.Client.interfaces
{
    public interface IStorageServiceClient{

        bool UploadFile(byte[] data, string filename,NameValueCollection metaData=null);
        bool UploadFile(string filepath, NameValueCollection metaData = null);

        bool DoesFileExist(string fileName);

        bool DeleteFile(string fileName);

        FileResponse DownloadFile(string fileName);

        bool TestConnection();

    }
}
