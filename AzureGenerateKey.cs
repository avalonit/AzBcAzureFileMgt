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
    public class AzureGenerateKey
    {
        private readonly IAzureStorage _azurestorage;

        public AzureGenerateKey(IAzureStorage azurestorage)
        {
            _azurestorage = azurestorage;
        }

        [FunctionName("AzureGenerateKey")]
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
            StringBuilder sb = new StringBuilder();
            sb.Append(_azurestorage.GetSharedKeyLite(config, "/app365azurefiles/to-increase/pippo?comp=list"));

            return new OkObjectResult(sb.ToString());
            #endregion

        }

    }
}
