using System;
using Microsoft.Extensions.Configuration;


namespace com.businesscentral
{

    public partial class ConnectorConfig
    {
        public ConnectorConfig(IConfigurationRoot config)
        {
            if (config != null)
            {
                connectionString = config["connectionString"];
                shareName = config["shareName"];
                accountName = config["AccountName"];
                accountKey = config["AccountKey"];
                workingFolder = config["workingFolder"];
                azurePublicUrl = config["azurePublicUrl"];
            }
            // If you are customizing here it means you
            //  should give a look on how use azure configuration file
            if (String.IsNullOrEmpty(connectionString))
                connectionString = string.Empty;
            if (String.IsNullOrEmpty(shareName))
                shareName = string.Empty;
           
            if (String.IsNullOrEmpty(accountName))
                accountName = string.Empty;
            if (String.IsNullOrEmpty(accountKey))
                accountKey = string.Empty;

            if (String.IsNullOrEmpty(workingFolder))
                workingFolder = string.Empty;
            if (String.IsNullOrEmpty(azurePublicUrl))
                azurePublicUrl = string.Empty;
        }

        public String connectionString;
        public String shareName;
        public String accountName;
        public String accountKey;
        public String workingFolder;
        public String azurePublicUrl;


        

    }
}
