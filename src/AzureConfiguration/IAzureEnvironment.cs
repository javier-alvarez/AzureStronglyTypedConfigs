namespace AzureStronglyTypedConfigs
{
    public interface IAzureCloudServiceEnvironment
    {
        string CloudServiceConfigsFolder { get; }

        string EnvironmentName { get; }

        int NumberOfInstances { get; }

    }
}
