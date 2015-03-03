using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.IO;
using System.IO.Ports;
using System.Configuration;


namespace ArduinoComunication
{
    public class ArduinoComService : IArduinoComService
    {
        SerialPort arduinoPort;
        

        public string GetMessage(string name)
        {
            return name;
        }


        public bool ArduinoWrite(string message)
        {

            arduinoPort.PortName = ConfigurationManager.AppSettings["ArduinoCom"];
            
            
            return true;

        }
    }
}
