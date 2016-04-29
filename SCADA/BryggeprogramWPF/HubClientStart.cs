using Microsoft.AspNet.SignalR.Client;
using System;
using System.Windows;


namespace BryggeprogramWPF
{
    public class HubClientStart
    {
        private IHubProxy _hub;
        public IHubProxy Hub
        { 
            get { return _hub; }
            private set { _hub = value; }
        
        }
        string url = @"http://192.168.3.80:8088/";
           
        private HubConnection connection;
        public HubConnection Connection
        {
            get { return connection; }
            private set { connection = value; }
        }    
        
        public HubClientStart(string ip)
        {
            var connectingUrl = @"http://" + ip + ":8088/";
            try
            {
                connection = new HubConnection(connectingUrl);
                _hub = connection.CreateHubProxy("BrewingHub");
                connection.Start().Wait();
            }
            catch (Exception e)
            {
                MessageBox.Show("Hub error: {0}", e.Message.ToString());
                
            }

      
        }

    }
}
