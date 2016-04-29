using Microsoft.Owin.Hosting;
using System;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace SignalRHub
{
    class Program
    {
        static void Main(string[] args)
        {
            var ip = ConfigurationManager.AppSettings["LocalIP"];
            string url = @"http://"+ip+":8088/";
            WebApp.Start<Startup>(url);

            Console.WriteLine(string.Format("Server running at {0}",url ));
                Console.ReadLine();
            
        }

        private static IPAddress LocalIPAddress()
        {
            if (!System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
            {
                return null;
            }
            var hostName= Dns.GetHostName();
            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());

            return host
                .AddressList
                .FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork);
        }
        
    }
}