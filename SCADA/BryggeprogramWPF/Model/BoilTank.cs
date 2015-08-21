using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BryggeprogramWPF.Model
{
    public class BoilTank:Tank
    {
        public Pump Pump { get; set; }

        public BoilTank()
        {
            Pump = new Pump();
        }
    }
}
