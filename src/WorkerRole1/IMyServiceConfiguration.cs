namespace AzureConfiguration
{
    public interface IMyServiceConfiguration
    {
        string MonitoringStorageAccount { get; }
        int NumberOfThreads { get; }
    }
}