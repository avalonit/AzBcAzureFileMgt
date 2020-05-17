using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System.Text;

namespace com.businesscentral
{
    public class AzureGetFolderList
    {
        private readonly IAzureStorage _azurestorage;

        public AzureGetFolderList(IAzureStorage azurestorage)
        {
            _azurestorage = azurestorage;
        }

        [FunctionName("AzureGetFolderList")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)]
            HttpRequest req,
            ILogger log,
            ExecutionContext context)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            #region Load configuration
            var configBuilder = new ConfigurationBuilder()
                .SetBasePath(context.FunctionAppDirectory)
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();
            var config = new ConnectorConfig(configBuilder);
            #endregion

            #region GetSharedKeyLite
            var sb = new StringBuilder();
            var publicUrl = String.Format("{0}/{1}?restype=directory&comp=list", config.azurePublicUrl, config.workingFolder);
            var urlForMacEvaluation = String.Format("/{0}/{1}/{2}?comp=list", config.accountName, config.shareName, config.workingFolder);
            sb.Append(await _azurestorage.GetFolderListAsync(config, publicUrl, urlForMacEvaluation));

            return new OkObjectResult(sb.ToString());
            #endregion

        }

    }
}
