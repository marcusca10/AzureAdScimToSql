using Microsoft.SCIM;
using System;

namespace Mca.AzureAd.Scim.SqlProvider
{
    class Program
    {
        private const string baseAddress = "http://localhost:20000";

        static void Main()
        {
            Service service = new ServiceProvider();
            service.Start(new Uri(Program.baseAddress));

            //Console.ReadKey(true);
        }
    }
}
