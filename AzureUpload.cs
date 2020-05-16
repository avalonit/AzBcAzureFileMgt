using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace com.businesscentral
{
    public class AzureUpload
    {
        private readonly IAzureStorage _azurestorage;

        public AzureUpload(IAzureStorage azurestorage)
        {
            _azurestorage = azurestorage;
        }

        [FunctionName("AzureUpload")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)]
            HttpRequest req,
            ILogger log,
            ExecutionContext context)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            #region Process parameters
            string fileName = req.Query["fileName"];
            if (String.IsNullOrEmpty(fileName))
            {
                log.LogInformation("Mandatory parameter omitted.");
                return new BadRequestResult();
            }
            #endregion

            #region Load configuration
            var configBuilder = new ConfigurationBuilder()
                .SetBasePath(context.FunctionAppDirectory)
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();
            var config = new ConnectorConfig(configBuilder);
            #endregion

            #region Download file
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            byte[] byteArray = Encoding.ASCII.GetBytes(requestBody);
            MemoryStream stream = new MemoryStream(byteArray);

            await _azurestorage.UploadAsync(config, fileName, stream);

            var responseMessage = string.IsNullOrEmpty(fileName)
                ? "This HTTP triggered function executed successfully. Pass a name in the query string and the file in the request body."
                : $"Hello, This HTTP triggered function executed successfully {fileName}.";

            return new OkObjectResult(responseMessage);
            #endregion
        }


    }
}
