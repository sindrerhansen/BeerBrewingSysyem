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
using System.Windows.Navigation;
using System.Windows.Shapes;
using OxyPlot;
using OxyPlot.Axes;


namespace ProsessView
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MainViewModel mod = new MainViewModel();
        
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = mod;
            
            
        }

        private void btnAddValue_Click(object sender, RoutedEventArgs e)
        {
            Random random = new Random();
            
            mod.Line1.Add(new DataPoint(DateTimeAxis.ToDouble(DateTime.Now), random.Next(0, 100)));
            mod.Line2.Add(new DataPoint(DateTimeAxis.ToDouble(DateTime.Now), random.Next(0, 50)));
            Plot.InvalidatePlot();
       

        }

    }
}
