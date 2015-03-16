using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using OxyPlot;
using OxyPlot.Axes;

namespace BryggeprogramWPF
{
    /// <summary>
    /// Interaction logic for TrendWindow.xaml
    /// </summary>
    public partial class TrendWindow : Window
    {
        public TrendWindow()
        {
            
            InitializeComponent();
            System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {

            //Plot.InvalidatePlot();
        }
    }
}
