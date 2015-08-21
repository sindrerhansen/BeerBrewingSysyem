using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BryggeprogramWPF.Model
{
    public class RIMSelement
    {
        public double InnTemperature { get; set; }
        public double OutTeperature { get; set; }
        public double OutesideTemperature { get; set; }
        public bool ElementOn { get; set; }
        
        
        public RIMSelement()
        { }
    }
}
