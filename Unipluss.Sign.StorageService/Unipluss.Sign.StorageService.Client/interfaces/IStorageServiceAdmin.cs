namespace Unipluss.Sign.StorageService.Client.interfaces
{
    public interface IStorageServiceAdmin
    {
        string CreateContainer(string containerName);
        bool DoesContainerExist(string containerName);
        string GetContainerKey(string containerName);
    }
}