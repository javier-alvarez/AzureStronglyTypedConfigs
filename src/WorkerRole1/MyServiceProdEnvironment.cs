namespace WorkerRole1
{
    using AzureConfiguration;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class MyServiceProdEnvironment : MyServiceConfiguration, IAzureCloudServiceEnvironment
    {
        public MyServiceProdEnvironment()
        {
            EnvironmentName = "Prod";
            CloudServiceConfigsFolder = @"\StronglyTypedAzureConfigs";
            NumberOfThreads = 500;
            MonitoringStorageAccount = "prod other storage account";
            NumberOfInstances = 10;
            BlobStorageAccounts = new List<string> { "storageaccount1", "storageaccount2" };
        }

        public string CloudServiceConfigsFolder { get; private set; }

        public string EnvironmentName { get; private set; }


        public int NumberOfInstances { get; private set; }
    }
}
