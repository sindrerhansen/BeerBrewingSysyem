using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;

namespace ArduinoComunicationServiceConsoleHost
{
    class Program
    {
        static void Main()
        {
            using(ServiceHost host = new ServiceHost(typeof(ArduinoComunication.ArduinoComService)))
            {
                host.Open();
                Console.WriteLine("Service started @ " + DateTime.Now.ToString());
                Console.ReadLine();
            }
        }
    }
}
