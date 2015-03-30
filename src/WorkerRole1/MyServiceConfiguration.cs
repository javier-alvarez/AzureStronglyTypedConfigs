namespace AzureConfiguration
{
    using System.Collections.Generic;

    using AzureStronglyTypedConfigs;

    public class MyServiceConfiguration : IMyServiceConfiguration
    {
        public MyServiceConfiguration()
        {
            NumberOfThreads = 200;
            MonitoringStorageAccount = "prod other storage account";
            BlobStorageAccounts = new List<string> { "baseconfigstorageaccount1", "baseconfigstorageaccount2" };
        }

        public int NumberOfThreads { get; protected set; }

        [AzureConfigSetting(Name = "Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString", IncludeInCSDEF = false)]
        public string MonitoringStorageAccount { get; protected set; }

        [AzureConfigSetting(ExtraCapacity = 25)]
        public IEnumerable<string> BlobStorageAccounts { get; protected set; }
    }
}
