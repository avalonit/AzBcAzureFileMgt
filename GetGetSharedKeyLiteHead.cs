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
    public class GetGetSharedKeyLiteHead
    {
        private readonly IAzureStorage _azurestorage;

        public GetGetSharedKeyLiteHead(IAzureStorage azurestorage)
        {
            _azurestorage = azurestorage;
        }

        [FunctionName("GetGetSharedKeyLiteHead")]
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
            var urlForMacEvaluationGetFileExist = String.Format("/{0}/{1}/{2}/sample3.txt", config.accountName, config.shareName, config.workingFolder);
            var urlForMacEvaluationGetDontExist = String.Format("/{0}/{1}/{2}/dontexist.txt", config.accountName, config.shareName, config.workingFolder);
            var urlForMacEvaluationGetFolderExist = String.Format("/{0}/{1}/{2}", config.accountName, config.shareName, config.workingFolder);
            var urlForMacEvaluationGetFolderDoesNotExist = String.Format("/{0}/{1}/{2}", config.accountName, config.shareName, "nofoo");
            var contentUrl = string.Empty;
            sb.AppendLine("-----------------------------------------------------------------------------");
            sb.AppendLine("FILE EXISTS");
            sb.AppendLine(_azurestorage.GetSharedKeyLiteHead(config, urlForMacEvaluationGetFileExist, contentUrl));
            sb.AppendLine("-----------------------------------------------------------------------------");
            sb.AppendLine("FILE DOES NOT EXISTS");
            sb.AppendLine(_azurestorage.GetSharedKeyLiteHead(config, urlForMacEvaluationGetDontExist, contentUrl));
            sb.AppendLine("-----------------------------------------------------------------------------");
            sb.AppendLine("FOLDER EXISTS");
            sb.AppendLine(_azurestorage.GetSharedKeyLiteHead(config, urlForMacEvaluationGetFolderExist, contentUrl));
            sb.AppendLine("-----------------------------------------------------------------------------");
            sb.AppendLine("FOLDER DOES NOT EXISTS");
            sb.AppendLine(_azurestorage.GetSharedKeyLiteHead(config, urlForMacEvaluationGetFolderDoesNotExist, contentUrl));

            return new OkObjectResult(sb.ToString());
            #endregion

        }

    }
}
