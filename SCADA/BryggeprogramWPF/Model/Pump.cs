using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BryggeprogramWPF.Model
{

    public class Pump
    {
        public virtual bool Running
        {
            get;
            set;
        }

        public Pump()
        {
            Running = false;
        }

    }
}
