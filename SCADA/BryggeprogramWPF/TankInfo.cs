using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BryggeprogramWPF
{
    public class Sensor{
        public double SensorValue { get; set; }
        public string SensorIdentifier { get; set; }

        public Sensor()
        {
            SensorValue = 0;
            SensorIdentifier = "";
        }
    }
    public class TankData
    {
        public Sensor TemperatureActual { get; set; }
        public double TemperatureSetpoint {get; set;}
        public Sensor Volume { get; set; }
        public Sensor HeatingElementReturTemperature { get; set; }
        public HeatingElement HeatingElement { get; set; }
        public Pump CirculationPump { get; set; }
        public Pump TransferPump { get; set; }
        public bool DrainValveOpen {get; set;}

        public TankData() 
        {
            TemperatureActual = new Sensor();
            TemperatureSetpoint = 0;
            Volume = new Sensor();
            HeatingElementReturTemperature = new Sensor();
            DrainValveOpen = false;
            HeatingElement = new HeatingElement();
            CirculationPump = new Pump();
            TransferPump = new Pump();
        }


    }
    public class BaseElement
    {
        public bool On{get;set;}
        public bool Override{get;set;}
        public bool OverrideValue{get;set;}

        public BaseElement()
        {
            On = false;
            Override = false;
            OverrideValue = false;
        }

    }

    public class HeatingElement : BaseElement{ }

    public class Pump : BaseElement { }
}
