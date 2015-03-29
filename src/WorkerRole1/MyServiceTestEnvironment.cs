using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkerRole1
{
    using AzureConfiguration;

    public class MyServiceTestEnvironment : MyServiceConfiguration, IAzureCloudServiceEnvironment
    {
        public MyServiceTestEnvironment()
        {
            EnvironmentName = "Test";
            CloudServiceConfigsFolder = @"\StronglyTypedAzureConfigs";
            NumberOfThreads = 100;
            MonitoringStorageAccount = "other storage account";
            NumberOfInstances = 1;
        }

        public string CloudServiceConfigsFolder { get; private set; }

        public string EnvironmentName { get; private set; }

        public int NumberOfInstances { get; private set; }
    }
}
