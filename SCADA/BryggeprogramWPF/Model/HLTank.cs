using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BryggeprogramWPF.Model
{
    public class HLTank : Tank
    {
        public virtual Pump CirculationPump { get; set; }
        public virtual Pump TransferPump {get; set;}
        public bool HeatingElementOn { get; set; }

        public HLTank()
        {
            CirculationPump = new Pump();
            TransferPump = new Pump();
         
        }
    }

    
}
