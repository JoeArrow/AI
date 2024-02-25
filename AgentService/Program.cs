using System;
using AgentClient;
using System.ServiceModel;
using System.ServiceModel.Description;

namespace AgentService
{
    class Program
    {
        static void Main(string[] args)
        {
            // -------------------------------------------------
            // Step 1 Create a URI to serve as the base address.

            var baseAddress = new Uri("http://localhost:9090/");

            // ------------------------------------
            // Step 2 Create a ServiceHost instance

            var selfHost = new ServiceHost(typeof(AgentCommunicationService), baseAddress);

            try
            {
                // ------------------------------
                // Step 3 Add a service endpoint.

                selfHost.AddServiceEndpoint(typeof(IAgentCommunicationService),
                    new WSDualHttpBinding(WSDualHttpSecurityMode.None), "AgentCommunicationService");

                // ----------------------------------------------------
                // Step 4 Enable Metadata Exchange and Add MEX endpoint

                var smb = new ServiceMetadataBehavior { HttpGetEnabled = true };

                selfHost.Description.Behaviors.Add(smb);
                selfHost.AddServiceEndpoint(ServiceMetadataBehavior.MexContractName,
                    MetadataExchangeBindings.CreateMexHttpBinding(), baseAddress + "mex");
                
                // ------------------------
                // Step 5 Start the service.

                selfHost.Open();
                Console.WriteLine("The service is ready.");
                Console.WriteLine("Listening at: {0}", baseAddress);
                Console.WriteLine("Press <ENTER> to terminate service.");
                Console.WriteLine();
                Console.ReadLine();

                // --------------------------------------------------
                // Close the ServiceHostBase to shutdown the service.

                selfHost.Close();
            }
            catch (CommunicationException ce)
            {
                Console.WriteLine("An exception occurred: {0}", ce.Message);
                selfHost.Abort();
            }
        }
    }
}
