[![Build status](https://ci.appveyor.com/api/projects/status/6bw9qovlxgbtpkb5?svg=true)](https://ci.appveyor.com/project/javier-alvarez/azurestronglytypedconfigs)
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
	
	[AzureConfigSetting(Capacity = 20)]
    public IEnumerable<int> Ids { get; set; }
}
```
3. Create environments matching the Azure Cloud Service environments. Check Sample projects: src\SampleUsage\
```C#
public class CloudEnvironmentConfig : APIServiceConfiguration, IAzureCloudServiceEnvironment
{
    public CloudEnvironmentConfig()
    {
       	CloudServiceConfigsFolder = "ApiServiceAzure";
       	EnvironmentName = "Cloud"; // Name of the cscfg environment that will represent this class
      	NumberOfInstances = 10;
        Timeout = 10;
        MonitoringStorageAccount = "prod account";
		Ids = new List<int> { 1, 2 };
    }

    public string CloudServiceConfigsFolder { get; private set; }

    public string EnvironmentName { get; private set; }

    public int NumberOfInstances { get; private set; }
}

public class LocalnvironmentConfig : APIServiceConfiguration, IAzureCloudServiceEnvironment
{
    public LocalEnvironmentConfig()
    {
       	CloudServiceConfigsFolder = "ApiServiceAzure";
       	EnvironmentName = "Local";
      	NumberOfInstances = 1;
        Timeout = 10;
        MonitoringStorageAccount = "test account";
		Ids = new List<int> { 1 };
    }

    public string CloudServiceConfigsFolder { get; private set; }

    public string EnvironmentName { get; private set; }

    public int NumberOfInstances { get; private set; }
}

```
4. Add After build target to the .csproj so configs are generated on build automatically every time you change the environments or configs classes
```xml
 <Target Name="AfterBuild">
    <Exec Command="$(ProjectDir)$(OutDir)\AzureConfigGenerator.exe $(ProjectDir)$(OutDir)\ApiWorkerRole.dll $(SolutionDir)" IgnoreExitCode="false" />
  </Target>
```
5. Read config on the WorkerRole
```C#
 var config = new APIServiceConfiguration();
 
 AzureConfigurationReader.ReadConfig(config);
 ```
