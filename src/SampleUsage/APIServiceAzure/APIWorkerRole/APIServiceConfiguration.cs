﻿using System;
namespace APIWorkerRole
{
    using AzureStronglyTypedConfigs;

    public class APIServiceConfiguration
    {
        public APIServiceConfiguration()
        {
            Timeout = 60000;
            MonitoringStorageAccount = "my account";
            ThrottlingRatio = 0.5;
        }

        public double ThrottlingRatio { get; set; }

        public int Timeout { get; set; }

        [AzureConfigSetting(Name = "Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString", IncludeInCSDEF = false)]
        public string MonitoringStorageAccount { get; set; }
    }
}
