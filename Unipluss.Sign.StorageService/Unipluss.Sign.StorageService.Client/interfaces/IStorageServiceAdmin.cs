using System.Collections.Specialized;

namespace Unipluss.Sign.StorageService.Client.interfaces
{
    public interface IStorageServiceAdmin
    {
        string CreateContainer(string containerName, NameValueCollection metaData = null);
        bool DoesContainerExist(string containerName);
        string GetContainerKey(string containerName);
        NameValueCollection GetContainerMetaData(string containerName);
        bool TestConnection();
        bool DeleteContainer(string containerName);
    }
}