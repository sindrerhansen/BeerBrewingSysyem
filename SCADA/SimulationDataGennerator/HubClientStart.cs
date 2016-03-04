using System;
using Microsoft.AspNet.SignalR.Client;

namespace SimulationDataGennerator
{
    public class HubClientStart
    {
        private IHubProxy _hub;
        public IHubProxy Hub
        {
            get { return _hub; }
            private set { _hub = value; }

        }
        string url = @"http://localhost:8088/";

        private HubConnection connection;
        public HubConnection Connection
        {
            get { return connection; }
            private set { connection = value; }
        }

        public HubClientStart()
        {

            Connect();

        }

        private void Connect()
        {
            try
            {
                connection = new HubConnection(url);
                _hub = connection.CreateHubProxy("BrewingHub");
                connection.Start().Wait();
            }
            catch (Exception e)
            {
                Console.WriteLine("Hub error: {0}", e.Message.ToString());
                System.Threading.Thread.Sleep(1000);
                Connect();
            }
        }
    }
}
