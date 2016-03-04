using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace SimulationDataGennerator
{

    class Program
    {
        static System.Timers.Timer _timer = new System.Timers.Timer();
        static Simulate sim = new Simulate();
        static HubClientStart hubClient;
        static void Main(string[] args)
        {
            hubClient = new HubClientStart();
            Task.Factory.StartNew(() =>
            {
                _timer.Interval = 500;
                _timer.Elapsed += _timer_Tick;
                _timer.Enabled = true;

            });


            Console.WriteLine("Press any key to exit...");
            Console.ReadKey(); // Block until you hit a key to prevent shutdown
        }

        static void _timer_Tick(object sender, ElapsedEventArgs e)
        {
            Console.WriteLine("Timer Elapsed!");
            if (hubClient.Connection.State== Microsoft.AspNet.SignalR.Client.ConnectionState.Connected)
            {
                var simData = sim.GennerateSimulatedArduinoValues();
                hubClient.Hub.Invoke("MulticastBrewingData", simData);
            }
        }
    }
}
