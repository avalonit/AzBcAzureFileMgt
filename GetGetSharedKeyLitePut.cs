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
    public class GetGetSharedKeyLitePut
    {
        private readonly IAzureStorage _azurestorage;

        public GetGetSharedKeyLitePut(IAzureStorage azurestorage)
        {
            _azurestorage = azurestorage;
        }

        [FunctionName("GetGetSharedKeyLitePut")]
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
            var urlForMacEvaluationRange = String.Format("/{0}/{1}/{2}?comp=range", config.accountName, config.shareName, "foo/foo_rest_api.txt");
            var urlForMacEvaluationInit = String.Format("/{0}/{1}/{2}", config.accountName, config.shareName, "foo/foo_rest_api.txt");
            var contentUrl = string.Empty;
            sb.AppendLine(_azurestorage.GetSharedKeyLitePut(config, urlForMacEvaluationInit, contentUrl, 999, "init"));
            sb.AppendLine(_azurestorage.GetSharedKeyLitePut(config, urlForMacEvaluationRange, contentUrl, 999, "range"));

            return new OkObjectResult(sb.ToString());
            #endregion

        }

    }
}
