using Microsoft.Owin.Hosting;
using System;

namespace SignalRHub
{
    class Program
    {
        static void Main(string[] args)
        {
            string url = "http://192.168.3.103:8088/";
            WebApp.Start<Startup>(url);
            
                Console.WriteLine(string.Format("Server running at {0}", url));
                Console.ReadLine();
            
        }
    }
}