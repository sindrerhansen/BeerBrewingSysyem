using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BryggeprogramWPF.Model
{
    public class ProsessData
    {
        public HLTank HLT { get; set; }
        public MashTank MashTank { get; set; }
        public BoilTank BoilTank { get; set; }

        public ProsessData()
        {
            HLT = new HLTank();
            MashTank = new MashTank();
            BoilTank = new BoilTank();

        }
    }
}
