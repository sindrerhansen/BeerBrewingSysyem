using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BryggeprogramWPF
{
    public class BrewingSettings
    {
        public double MashInnTemperature { get; set; }
        public double MashInnVolume { get; set; }
        public double MashStep1Temperature { get;set; }
        public double MashStep1Time { get; set; }
        public double MashStep2Temperature { get; set; }
        public double MashStep2Time { get; set; }
        public double SpargeTemperature { get; set; }
        public double SpargeTime { get; set; }
        public double SpargeVolum { get; set; }
        public double BoilTime { get; set; }
    }
}
