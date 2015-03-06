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

        public event EventHandler SendOverrideCommandHeatingElement;
        public event EventHandler SendOverrideCommandCirculationPump;
        public event EventHandler SendOverrideCommandTransferElement;
        TankInfo _tank = new TankInfo();
        

        public Tank()
        {
            InitializeComponent();
        }
           

        private void GauageActTemp_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {

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

        private void indicatorHeatingElementOn_LeftClick(object tank, MouseButtonEventArgs e)
        {
            SendOverrideCommandHeatingElement(this._tank, e);
        }

        private void indicatorHeatingElementOn_RightClick(object sender, MouseButtonEventArgs e)
        {

            if (_tank.HeatingElement.Override)
            {
                var mbRes = MessageBox.Show("Disable override heating element?", "Important Question", MessageBoxButton.YesNoCancel);
                if (mbRes == MessageBoxResult.Yes)
                {
                    _tank.HeatingElement.Override = false;

                    txtHeatingElement.Background = new SolidColorBrush(Colors.Transparent);
                }
            }

            else
            {

                var mbRes = MessageBox.Show("Enable override heating element?", "Important Question", MessageBoxButton.YesNoCancel);
                if (mbRes == MessageBoxResult.Yes)
                {
                    _tank.HeatingElement.Override = true;
                    txtHeatingElement.Background = new SolidColorBrush(Colors.Red);
                }
            }
        }

        private void indicatorCirculationPumpOn_LeftClick(object sender, MouseButtonEventArgs e)
        {
            SendOverrideCommandCirculationPump(this._tank, e);
        }

        private void indicatorCirculationPumpOn_RightClick(object sender, MouseButtonEventArgs e)
        {
            
            if (_tank.CirculationPump.Override)
            {
                var mbRes = MessageBox.Show("Disable override circulation pump?", "Important Question", MessageBoxButton.YesNoCancel);
                if (mbRes == MessageBoxResult.Yes)
                {
                    _tank.CirculationPump.Override = false;

                    txtCirculationPump.Background=new SolidColorBrush(Colors.Transparent);
         
                }
            }
            else
            {

                var mbRes = MessageBox.Show("Enable override circulation pump?", "Important Question", MessageBoxButton.YesNoCancel);
                if (mbRes == MessageBoxResult.Yes)
                {
                    _tank.CirculationPump.Override = true;
                    txtCirculationPump.Background = new SolidColorBrush(Colors.Red);
                }
            }
        }

        private void indicatorTransferPumpOn_LeftClick(object sender, MouseButtonEventArgs e)
        {
            SendOverrideCommandTransferElement(this._tank, e);
        }

        private void indicatorTransferPumpOn_RightClick(object sender, MouseButtonEventArgs e)
        {
            if (_tank.TransferPump.Override)
            {
                var mbRes = MessageBox.Show("Disable override transfer pump?", "Important Question", MessageBoxButton.YesNoCancel);
                if (mbRes == MessageBoxResult.Yes)
                {
                    _tank.TransferPump.Override = false;


                    txtTransferPump.Background = new SolidColorBrush(Colors.Transparent);

                }
            }

            else
            {

                var mbRes = MessageBox.Show("Enable override transfer pump?", "Important Question", MessageBoxButton.YesNoCancel);
                if (mbRes == MessageBoxResult.Yes)
                {
                    _tank.TransferPump.Override = true;
                    txtTransferPump.Background = new SolidColorBrush(Colors.Red);
                }
            }
        }

    }
}
