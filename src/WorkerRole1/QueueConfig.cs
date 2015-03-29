using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkerRole1
{
    public class QueueConfig
    {
        public string QueueName { get; set; }

        public IEnumerable<string> StorageAccounts { get; set; }
    }
}
