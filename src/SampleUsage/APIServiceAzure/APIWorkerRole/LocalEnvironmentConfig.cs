namespace APIWorkerRole
{
    using System.Collections.Generic;

    using AzureStronglyTypedConfigs;

    public class LocalEnvironmentConfig : APIServiceConfiguration, IAzureCloudServiceEnvironment
    {
        public LocalEnvironmentConfig()
        {
            CloudServiceConfigsFolder = "ApiServiceAzure";
            EnvironmentName = "Local";
            NumberOfInstances = 1;
            Ids = new List<int> { 1, 2, 3 };
        }

        public string CloudServiceConfigsFolder { get; private set; }

        public string EnvironmentName { get; private set; }

        public int NumberOfInstances { get; private set; }
    }
}
