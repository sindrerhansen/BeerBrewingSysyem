using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OxyPlot;

namespace BryggeprogramWPF
{
    class MainViewModel
    {
        public MainViewModel()
        {
            this.Title = "";
            this.HLT = new List<DataPoint>();
            this.MashTank = new List<DataPoint>();
            this.BoilTank = new List<DataPoint>();
        }

        public string Title { get; private set; }

        public List<DataPoint> HLT { get; set; }
        public List<DataPoint> MashTank { get; set; }
        public List<DataPoint> BoilTank { get; set; }
    }
}
