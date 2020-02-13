using Microsoft.SCIM;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mca.AzureAd.Scim.SqlProvider
{
    internal class ServiceProvider : Service
    {
        public override IMonitor MonitoringBehavior { get; set; }
        public override IProvider ProviderBehavior { get; set; }

        public ServiceProvider()
        {
            this.MonitoringBehavior = new ConsoleMonitor();
            this.ProviderBehavior = new SqlProvider();
        }
    }
}
