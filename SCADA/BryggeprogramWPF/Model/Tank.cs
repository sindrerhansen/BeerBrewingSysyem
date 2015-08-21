using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BryggeprogramWPF.Model
{
    public class Tank
    {

        public double CurrentVolume
        {
            get;
            set;
        }
        public double AddedVolume
        { get; set; }
        public double TappedVolume
        { get; set; }
        public double Temperature
        { get; set; }
        public double TemperatureSetpoint;

        public Tank()
        {
            AddedVolume = 0;
            TappedVolume = 0;
            CurrentVolume = 0;
            Temperature = 0;
            TemperatureSetpoint = 0;

        }

        public Tank(double addedVolume,double tappedVolume, double currentVolume, double temp, double tempSet)
        {
            AddedVolume = addedVolume;
            TappedVolume = tappedVolume;
            CurrentVolume = currentVolume;
            Temperature = temp;
            TemperatureSetpoint = tempSet;
        }
    }

    
}
