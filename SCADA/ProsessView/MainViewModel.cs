using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using OxyPlot;

namespace ProsessView
{
    public class MainViewModel
    {
        public MainViewModel()
        {
            this.Title = "Test";
            this.Line1 = new List<DataPoint>();
            this.Line2 = new List<DataPoint>();
        }

        public string Title { get; private set; }

        public List<DataPoint> Line1 { get; set; }
        public List<DataPoint> Line2 { get; set; }
    }
}
