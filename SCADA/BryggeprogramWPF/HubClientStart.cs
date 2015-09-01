using Microsoft.AspNet.SignalR.Client;
using System;


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
        string url = @"http://93.89.117.144:8088/";
           
        private HubConnection connection;
        public HubConnection Connection
        {
            get { return connection; }
            private set { connection = value; }
        }    
        
        public HubClientStart()
        {
            connection= new HubConnection(url);
            _hub = connection.CreateHubProxy("BrewingHub");
            connection.Start().Wait();
      
        }
        
        private void test(string s)
        {
            
        }



    }
}
