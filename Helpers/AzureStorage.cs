using System.IO;
using System.Threading.Tasks;
using Azure.Storage.Files.Shares;
using Azure.Storage.Files.Shares.Models;

namespace com.businesscentral
{
    public class AzureStorage : IAzureStorage
    {
        public async Task UploadAsync(ConnectorConfig config, string fullFileName, MemoryStream stream)
        {
            var dirName = Path.GetDirectoryName(fullFileName).ToString();
            var fileName = Path.GetFileName(fullFileName).ToString();

            ShareClient share = new ShareClient(config.connectionString, config.shareName);
            if (!share.Exists())
                await share.CreateAsync();

            ShareDirectoryClient directory = share.GetDirectoryClient(dirName);
            if (!directory.Exists())
                await directory.CreateAsync();

            ShareFileClient file = directory.GetFileClient(fileName);
            await file.CreateAsync(stream.Length);
            await file.UploadAsync(stream);
            /*
            await file.UploadRange(
                    ShareFileRangeWriteType.Update,
                    new HttpRange(0, stream.Length),
                    stream);*/
        }


        public async Task<MemoryStream> DownloadAsync(ConnectorConfig config, string fullFileName)
        {
            var dirName = Path.GetDirectoryName(fullFileName).ToString();
            var fileName = Path.GetFileName(fullFileName).ToString();

            ShareClient share = new ShareClient(config.connectionString, config.shareName);
            ShareDirectoryClient directory = share.GetDirectoryClient(dirName);
            ShareFileClient file = directory.GetFileClient(fileName);

            ShareFileDownloadInfo download = await file.DownloadAsync();
            var stream = new MemoryStream();
            await download.Content.CopyToAsync(stream);
            return stream;
        }
    }
}
