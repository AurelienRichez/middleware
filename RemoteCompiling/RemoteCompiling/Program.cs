using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.ServiceModel.Description;
using System.Text;

namespace RemoteCompiling
{
    class Program
    {
        private static ServiceHost host = null;
        static void Main(string[] args)
        {
            host = new ServiceHost(typeof(Contract), new Uri("http://localhost:8080"));

            WebHttpBinding binding = new WebHttpBinding();
            binding.AllowCookies = true;
            binding.TransferMode = TransferMode.Streamed;

            WebHttpBehavior behavior = new WebHttpBehavior();

            ServiceEndpoint endpoint = host.AddServiceEndpoint(typeof(IContract), binding, "RemoteCompiling");
            endpoint.Behaviors.Add(behavior);

            host.Opened += new EventHandler(host_Opened);
            host.Closed += new EventHandler(host_Closed);
            host.Open();
            Console.ReadLine();
            host.Close();
            Console.ReadLine();
        }

        static void host_Closed(object sender, EventArgs e)
        {
            Console.WriteLine("Service Closed");
        }

        static void host_Opened(object sender, EventArgs e)
        {
            Console.WriteLine("Service Started");
        }
    }
}
