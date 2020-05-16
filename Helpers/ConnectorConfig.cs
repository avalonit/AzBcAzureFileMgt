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


            }
            // If you are customizing here it means you
            //  should give a look on how use azure configuration file
            if (String.IsNullOrEmpty(connectionString))
                connectionString = "";
            if (String.IsNullOrEmpty(shareName))
                shareName = "";
           
        }

        public String connectionString;
        public String shareName;

    }
}
