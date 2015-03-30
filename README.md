# AzureStronglyTypedConfigs
## Project goals
1. Simple type safe configuration system for Azure services
2. Support multiple environments for a service or services
3. Generation of .cscfg and .csdef from config environments with Visual Studio integration
4. Support collections
5. Support complex types (TODO)

## Usage
1. Install nuget package in the WorkerRole project or in the class project containing the configs
2. Create the config
```C#
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
```

