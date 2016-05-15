using Commen;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System;

namespace SignalRHub
{
    [HubName("BrewingHub")]
    public class BrewingHub : Hub
    {
        public void MulticastBrewingData(string message)
        {
            Console.WriteLine(message);
            Clients.All.ReceiveMulticastBrewingData(message);
        }

        public void SendCommand(string command)
        {
            Console.WriteLine(string.Format("Writing command {0} ", command));
        }

        public TestClass Get()
        {
            return new TestClass { Age = 12, EtterNavn = "Hansen", ForNavn = "Sindre" };
        }
    }
}
