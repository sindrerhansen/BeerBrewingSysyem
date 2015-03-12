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
            this.Points = new List<DataPoint>();
        }

        public string Title { get; private set; }

        public List<DataPoint> Points { get; set; }
    }
}
