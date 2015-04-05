namespace WorkerRole1
{
    using AzureStronglyTypedConfigs;

    public class MyServiceCloudEnvironment : MyServiceTestEnvironment
    {
        public MyServiceCloudEnvironment()
        {
            EnvironmentName = "Cloud";
        }
    }
}
