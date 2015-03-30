namespace APIWorkerRole
{
    using AzureStronglyTypedConfigs;

    public class LocalEnvironmentConfig : APIServiceConfiguration, IAzureCloudServiceEnvironment
    {
        public LocalEnvironmentConfig()
        {
            CloudServiceConfigsFolder = "ApiServiceAzure";
            EnvironmentName = "Local";
            NumberOfInstances = 1;
        }

        public string CloudServiceConfigsFolder { get; private set; }

        public string EnvironmentName { get; private set; }

        public int NumberOfInstances { get; private set; }
    }
}
