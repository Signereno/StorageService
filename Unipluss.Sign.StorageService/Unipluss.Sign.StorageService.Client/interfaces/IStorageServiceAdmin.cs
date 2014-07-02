namespace Unipluss.Sign.StorageService.Client.interfaces
{
    public interface IStorageServiceAdmin
    {
        string CreateAccount(string containerName);
        bool DoesAccountExist(string containerName);
    }
}