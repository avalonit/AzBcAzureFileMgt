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
    public class GetGetSharedKeyLite
    {
        private readonly IAzureStorage _azurestorage;

        public GetGetSharedKeyLite(IAzureStorage azurestorage)
        {
            _azurestorage = azurestorage;
        }

        [FunctionName("GetGetSharedKeyLite")]
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
            //eg: /app365azurefiles/to-increase/pippo?comp=list
            var urlForMacEvaluation = String.Format("/{0}/{1}/{2}?comp=list", config.accountName, config.shareName, config.workingFolder);
            var contentUrl = "text/plain";
            sb.Append(_azurestorage.GetSharedKeyLite(config, urlForMacEvaluation, contentUrl));

            return new OkObjectResult(sb.ToString());
            #endregion

        }

    }
}