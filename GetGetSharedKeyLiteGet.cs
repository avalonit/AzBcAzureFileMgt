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
    public class GetGetSharedKeyLiteGet
    {
        private readonly IAzureStorage _azurestorage;

        public GetGetSharedKeyLiteGet(IAzureStorage azurestorage)
        {
            _azurestorage = azurestorage;
        }

        [FunctionName("GetGetSharedKeyLiteGet")]
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
            var urlForMacEvaluationList = String.Format("/{0}/{1}/{2}?comp=list", config.accountName, config.shareName, config.workingFolder);
            var urlForMacEvaluationGet = String.Format("/{0}/{1}/{2}/sample2.txt", config.accountName, config.shareName, config.workingFolder);
            var contentUrl = string.Empty;
            sb.AppendLine(_azurestorage.GetSharedKeyLiteGet(config, urlForMacEvaluationList, contentUrl));
            sb.AppendLine(_azurestorage.GetSharedKeyLiteGet(config, urlForMacEvaluationGet, contentUrl));

            return new OkObjectResult(sb.ToString());
            #endregion

        }

    }
}
