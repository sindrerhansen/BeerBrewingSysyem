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
            this.Title = "Test";
            this.Points = new List<DataPoint>();
        }

        public string Title { get; private set; }

        public List<DataPoint> Points { get; set; }
    }
}
