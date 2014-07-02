using System.Collections.Specialized;

namespace Unipluss.Sign.StorageService.Client.entities
{
    public class FileResponse
    {
        public byte[] Bytes { get; set; }
        public string FileName { get; set; }
        public NameValueCollection MetaData { get; set; }
    }
}