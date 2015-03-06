using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BryggeprogramWPF
{
    class TankInfo
    {
        public double TemperatureActual { get; set; }
        public double TemperatureSetpoint {get; set;}
        public double Volume { get; set; }
        public double HeatingElementReturTemperature { get; set; }
        public TankElement HeatingElement { get; set; }
        public TankElement CirculationPump { get; set; }
        public TankElement TransferPump { get; set; }
        public bool DrainValveOpen {get; set;}

        public TankInfo() 
        {
            TemperatureActual = 0;
            TemperatureSetpoint = 0;
            Volume = 0;
            HeatingElementReturTemperature = 0;
            DrainValveOpen = false;
            HeatingElement = new TankElement();
            CirculationPump = new TankElement();
            TransferPump = new TankElement();
        }


    }
    class TankElement
    {
        public bool On{get;set;}
        public bool Override{get;set;}
        public bool OverrideValue{get;set;}

        public TankElement()
        {
            On = false;
            Override = false;
            OverrideValue = false;
        }

    }
}
