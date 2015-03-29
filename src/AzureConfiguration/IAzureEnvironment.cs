using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureConfiguration
{
    public interface IAzureCloudServiceEnvironment
    {
        string CloudServiceConfigsFolder { get; }

        string EnvironmentName { get; }

        int NumberOfInstances { get; }

    }
}
