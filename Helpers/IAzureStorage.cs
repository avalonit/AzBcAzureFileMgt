using System.IO;
using System.Threading.Tasks;

namespace com.businesscentral
{
    public interface IAzureStorage
    {
        Task UploadAsync(ConnectorConfig config, string fullFileName, MemoryStream stream);
        Task<MemoryStream> DownloadAsync(ConnectorConfig config, string fullFileName);
        string GetSharedKeyLiteGet(ConnectorConfig config, string url, string contentType);
        string GetSharedKeyLiteDelete(ConnectorConfig config, string url, string contentType);
        string GetSharedKeyLitePut(ConnectorConfig config, string url, string contentType, int contentLength, string subType);
        Task<string> GetFolderListAsync(ConnectorConfig config, string publicUrl, string macEvaluatedUrl);
        //string RestApiUpload(ConnectorConfig config);

    }
}
