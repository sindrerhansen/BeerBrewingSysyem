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
        public bool HeatingElementOn{get; set;}
        public bool CirculationPumpRunning {get; set;}
        public bool TransferPumpRunning {get; set;}
        public bool DrainValveOpen {get; set;}

        public TankInfo() { }

        public TankInfo(double temperatureActual, double temperatureSetpoint, double volume, double heatingElementReturTemperature, bool heatingElementOn, bool circulationPumpRunning, bool transferPumpRunning, bool drainValveOpen)
        {
            TemperatureActual = temperatureActual;
            TemperatureSetpoint = temperatureSetpoint;
            Volume = volume;
            HeatingElementReturTemperature = heatingElementReturTemperature;
            HeatingElementOn = heatingElementOn;
            CirculationPumpRunning = circulationPumpRunning;
            TransferPumpRunning = transferPumpRunning;
            DrainValveOpen = drainValveOpen;
        }
    }
}
