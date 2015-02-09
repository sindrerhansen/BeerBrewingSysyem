using System;
using System.Collections.Generic;
using System.Globalization;
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

namespace BryggeprogramWPF
{
    /// <summary>
    /// Interaction logic for Tank.xaml
    /// </summary>
    public partial class Tank : UserControl
    {
        public Tank()
        {
            InitializeComponent();
        }
        
        bool heatingElementOverride = false;

        private void indicatorHeatingElementOn_RightClick(object sender, MouseButtonEventArgs e)
        {
          if (!heatingElementOverride)
          {
            var mbRes = MessageBox.Show("Enable override heating element?", "Important Question", MessageBoxButton.YesNoCancel);
            if (mbRes == MessageBoxResult.Yes)
            {
              heatingElementOverride = true;
              txtHeatingElementOn.Background = new SolidColorBrush(Colors.Red);
            }
          }
          else if (heatingElementOverride)
	        {
                var mbRes = MessageBox.Show("Disable override heating element?", "Important Question", MessageBoxButton.YesNoCancel);
                if (mbRes == MessageBoxResult.Yes)
                {
                    heatingElementOverride = false;
                    
                    txtHeatingElementOn.Background = new SolidColorBrush(Colors.Transparent);
                }
	        }
        }

        private void TextActuelTemp_TextChanged(object sender, TextChangedEventArgs e)
        {
            double actTemp, setTemp;
            double.TryParse(TextActuelTemp.Text, out actTemp);
            double.TryParse(TextSetTemp.Text, out setTemp);
            if (actTemp>(setTemp-0.2)&&actTemp<(setTemp+0.2))
            {
                indicatorOnSetpoint.Fill = new SolidColorBrush(Colors.Green);
            }
            else
            {
                indicatorOnSetpoint.Fill = new SolidColorBrush(Colors.LightGray);
            }
        }

        private void GauageActTemp_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {

        }



    }
}
