using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BryggeprogramWPF.Model
{
    public class MashTank: Tank
    {
        public Pump CirculationPump { get; set; }
        public Pump TransferPump{get;set;}
        public RIMSelement RIMS { get; set; }

        public MashTank()
        {
            CirculationPump = new Pump();
            TransferPump = new Pump();
            RIMS = new RIMSelement();
            
        }
    }
}
