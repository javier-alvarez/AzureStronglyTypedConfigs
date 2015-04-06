namespace APIWorkerRole
{
    using System.Collections.Generic;

    using AzureStronglyTypedConfigs;

    public class CloudEnvironmentConfig : APIServiceConfiguration, IAzureCloudServiceEnvironment
    {
        public CloudEnvironmentConfig()
        {
            CloudServiceConfigsFolder = "ApiServiceAzure";
            EnvironmentName = "Cloud";
            NumberOfInstances = 1;

            Timeout = 10;
            MonitoringStorageAccount = "prod account";
            Ids = new List<int> { 1, 2 };
        }

        public string CloudServiceConfigsFolder { get; private set; }

        public string EnvironmentName { get; private set; }

        public int NumberOfInstances { get; private set; }
    }
}
