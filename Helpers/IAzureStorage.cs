using System.IO;
using System.Threading.Tasks;

namespace com.businesscentral
{
    public interface IAzureStorage
    {
        Task UploadAsync(ConnectorConfig config, string fullFileName, MemoryStream stream);
        Task<MemoryStream> DownloadAsync(ConnectorConfig config, string fullFileName);

    }
}
