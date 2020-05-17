using System.IO;
using System.Threading.Tasks;
using Azure.Storage.Files.Shares;
using Azure.Storage.Files.Shares.Models;
using System.Text;
using System;
using System.Globalization;
using System.Security.Cryptography;
using System.Net.Http;
using System.Linq;
using System.Collections;
using System.Collections.Specialized;
using System.Web;

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

        public async Task<string> GetFolderListAsync(ConnectorConfig config, string apiEndPoint)
        {
            var returnValue = string.Empty;
            var requestDateString = DateTime.UtcNow.ToString("R", CultureInfo.InvariantCulture);

            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Clear();
                httpClient.DefaultRequestHeaders.Add("Content-Type", "text/plain");
                httpClient.DefaultRequestHeaders.Add("x-ms-version", "2017-11-09");
                httpClient.DefaultRequestHeaders.Add("x-ms-date", "2017-11-09");
                var responseMessage = await httpClient.GetAsync(apiEndPoint);
                if (responseMessage.IsSuccessStatusCode)
                    returnValue = await responseMessage.Content.ReadAsStringAsync();
            }
            return returnValue;
        }

        public string GetSharedKeyLite(ConnectorConfig config, string url)
        {
            HttpMethod method = HttpMethod.Get;
            var StorageAccountName = config.accountName;
            var StorageKey = config.accountKey;
            //var requestDateString = DateTime.UtcNow.ToString("R", CultureInfo.InvariantCulture);
            var requestDateString = "Sat, 16 May 2020 20:50:00 GMT";
            var contentLength = string.Empty;
            var contentType = "text/plain";

            //var canonicalizedStringToBuild = string.Format("{0}\n{1}", RequestDateString, $"/{StorageAccountName}/{requestUri.AbsolutePath.TrimStart('/')}");
            var canonicalizedStringToBuild = String.Format("{0}\n{1}\n{2}\n{3}\n{4}\n{5}\n{6}",
                        method.ToString(),
                        (method == HttpMethod.Get || method == HttpMethod.Head) ? String.Empty : contentLength,
                        contentType,
                        string.Empty,
                        "x-ms-date:" + requestDateString,
                        "x-ms-version:2017-11-09",
                        url);
            var signature = string.Empty;
            using (var hmac = new HMACSHA256(Convert.FromBase64String(StorageKey)))
            {
                byte[] dataToHmac = Encoding.UTF8.GetBytes(canonicalizedStringToBuild);
                signature = Convert.ToBase64String(hmac.ComputeHash(dataToHmac));
            }

            var sharedKeyLite = string.Format("SharedKeyLite " + $"{StorageAccountName}:" + signature);
            return sharedKeyLite;
        }

        public string GetSharedKey(
            string storageAccountName, string storageAccountKey, DateTime now,
            HttpRequestMessage httpRequestMessage, string ifMatch = "", string md5 = "")
        {
            // This is the raw representation of the message signature.
            HttpMethod method = httpRequestMessage.Method;
            String MessageSignature = String.Format("{0}\n\n\n{1}\n{5}\n\n\n\n{2}\n\n\n\n{3}{4}",
                        method.ToString(),
                        (method == HttpMethod.Get || method == HttpMethod.Head) ? String.Empty
                          : httpRequestMessage.Content.Headers.ContentLength.ToString(),
                        ifMatch,
                        GetCanonicalizedHeaders(httpRequestMessage),
                        GetCanonicalizedResource(httpRequestMessage.RequestUri, storageAccountName),
                        md5);

            // Now turn it into a byte array.
            byte[] SignatureBytes = Encoding.UTF8.GetBytes(MessageSignature);

            // Create the HMACSHA256 version of the storage key.
            HMACSHA256 SHA256 = new HMACSHA256(Convert.FromBase64String(storageAccountKey));

            // Compute the hash of the SignatureBytes and convert it to a base64 string.
            string signature = Convert.ToBase64String(SHA256.ComputeHash(SignatureBytes));

            // This is the actual header that will be added to the list of request headers.
            var sharedKey = storageAccountName + ":" + signature;
            return sharedKey;
        }

        private string GetCanonicalizedHeaders(HttpRequestMessage httpRequestMessage)
        {
            var headers = from kvp in httpRequestMessage.Headers
                          where kvp.Key.StartsWith("x-ms-", StringComparison.OrdinalIgnoreCase)
                          orderby kvp.Key
                          select new { Key = kvp.Key.ToLowerInvariant(), kvp.Value };

            StringBuilder headersBuilder = new StringBuilder();

            // Create the string in the right format; this is what makes the headers "canonicalized" --
            //   it means put in a standard format. https://en.wikipedia.org/wiki/Canonicalization
            foreach (var kvp in headers)
            {
                headersBuilder.Append(kvp.Key);
                char separator = ':';

                // Get the value for each header, strip out \r\n if found, then append it with the key.
                foreach (string headerValue in kvp.Value)
                {
                    string trimmedValue = headerValue.TrimStart().Replace("\r\n", string.Empty);
                    headersBuilder.Append(separator).Append(trimmedValue);

                    // Set this to a comma; this will only be used
                    // if there are multiple values for one of the headers.
                    separator = ',';
                }

                headersBuilder.Append("\n");
            }

            return headersBuilder.ToString();
        }

        private string GetCanonicalizedResource(Uri address, string storageAccountName)
        {
            // The absolute path will be "/" because for we're getting a list of containers.
            StringBuilder sb = new StringBuilder("/").Append(storageAccountName).Append(address.AbsolutePath);

            // Address.Query is the resource, such as "?comp=list".
            // This ends up with a NameValueCollection with 1 entry having key=comp, value=list.
            // It will have more entries if you have more query parameters.
            NameValueCollection values = HttpUtility.ParseQueryString(address.Query);

            foreach (var item in values.AllKeys.OrderBy(k => k))
            {
                sb.Append('\n').Append(item.ToLower()).Append(':').Append(values[item]);
            }

            return sb.ToString();
        }
    }
}