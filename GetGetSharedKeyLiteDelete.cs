using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System.Text;
using System;

namespace com.businesscentral
{
    public class GetGetSharedKeyLiteDelete
    {
        private readonly IAzureStorage _azurestorage;

        public GetGetSharedKeyLiteDelete(IAzureStorage azurestorage)
        {
            _azurestorage = azurestorage;
        }

        [FunctionName("GetGetSharedKeyLiteDelete")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]
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
            var urlForMacEvaluation = String.Format("/{0}/{1}/{2}/sample2.txt", config.accountName, config.shareName, config.workingFolder);
            var contentUrl = string.Empty;
            sb.AppendLine(_azurestorage.GetSharedKeyLiteDelete(config, urlForMacEvaluation, contentUrl));

            return new OkObjectResult(sb.ToString());
            #endregion

        }

    }
}
