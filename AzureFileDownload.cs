using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace com.businesscentral
{
    public class AzureFileDownload
    {
        private readonly IAzureStorage _azurestorage;

        public AzureFileDownload(IAzureStorage azurestorage)
        {
            _azurestorage = azurestorage;
        }

        [FunctionName("AzureFileDownload")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]
            HttpRequest req,
            ILogger log,
            ExecutionContext context)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            #region Process parameters
            var fileName = req.Query["fileName"].ToString();
            var contentType = req.Query["contentType"].ToString();

            if (String.IsNullOrEmpty(fileName))
            {
                log.LogInformation("Mandatory parameter omitted.");
                return new BadRequestResult();
            }

            if (String.IsNullOrEmpty(contentType))
                contentType = "application/binary";
            #endregion

            #region Load configuration
            var configBuilder = new ConfigurationBuilder()
                .SetBasePath(context.FunctionAppDirectory)
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();
            var config = new ConnectorConfig(configBuilder);
            #endregion

            #region Download
            var outputStream = await _azurestorage.DownloadAsync(config, fileName);

            return new FileContentResult(outputStream.ToArray(), contentType);
            #endregion

        }

    }
}
