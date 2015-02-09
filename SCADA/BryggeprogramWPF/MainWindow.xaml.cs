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
using System.Windows.Threading;
using System.IO.Ports;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Globalization;


namespace BryggeprogramWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
      
        SerialPort mySerialPort = new SerialPort();
        TankInfo hotLiqureTank = new TankInfo();
        TankInfo mashTank = new TankInfo();
        TankInfo boilTank = new TankInfo();
        
        TankVM BoilTankVm = new TankVM();
        TankVM HltVm = new TankVM();

        BrewingSettings brewingSettings = new BrewingSettings();

        SolidColorBrush myRedBrush = new SolidColorBrush(Colors.Red);
        SolidColorBrush myGrayBrush = new SolidColorBrush(Colors.LightGray);
        SolidColorBrush myGreenBrush = new SolidColorBrush(Colors.Green);

        public MainWindow()
        {
            InitializeComponent();

            System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            dispatcherTimer.Start();

            DropDownBaudRate.Items.Add("9600");
            DropDownBaudRate.SelectedItem = "9600";
            DropDownComPorts.ItemsSource=SerialPort.GetPortNames();

 
            
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            if (mySerialPort.IsOpen)
            {
                indConnected.Fill = myGreenBrush;
            }
            else
            {
                indConnected.Fill = myGrayBrush;
            }
        }
        

        void DataWindow_Closing(object sender, CancelEventArgs e)
        {

        }


        private delegate void UpdateUiTextDelegate(string text);

        private void DataReceivedHandler(object sender,SerialDataReceivedEventArgs e)
        {
            SerialPort sp = (SerialPort)sender;
            string indata = sp.ReadLine();
           
            Dispatcher.Invoke(DispatcherPriority.Send, new UpdateUiTextDelegate(WriteData), indata);
          
           
        }

        private void WriteData(string text)
        {
            // Assign the value of the recieved_data to the RichTextBox.
            text.Trim();
            string replacement = Regex.Replace(text, "\r", "");
            textBox.AppendText(replacement);
          //  textBox.Text += replacement;
            textBox.ScrollToEnd();
            
            var textList = Regex.Split(replacement, "_");
            try
            {
                //  Value from the Arduino are in the order: 
                //  [0] SequenseState, [1] TotaleVolumeAdded, [2] HLT TempActual, [3] HLT TempSetpoint, [4] HLT HeatingElementOn, [5] HLT CirculationPumpOn, [6] HLT TransferPumpOn, [7] HLT DrainValveOpen, [8] MeshTank TempActual, [9] MeshTank TempSetpoint, [10] MeshTank CirculationPumpOn, [11] MeshTank TransferPumpOn, [12] MeshTank DrainValveOpen, [13] BoilTank TempActual, [14] BoilTank TempSetpoint, [15] BoilTank CirculationPumpOn, [16] BoilTank TransferPumpOn, [17] BoilTank DrainValveOpen
                
                hotLiqureTank.TemperatureActual = Convert.ToDouble(textList[0], CultureInfo.InvariantCulture);
                HLT.GauageActTemp.Value = hotLiqureTank.TemperatureActual;
               
                mashTank.TemperatureActual = Convert.ToDouble(textList[1], CultureInfo.InvariantCulture);
                MashTank.GauageActTemp.Value = mashTank.TemperatureActual;
            }

            catch (Exception ex) {
                textBoxError.AppendText(ex.ToString());
            }
        }

        private void send_Click(object sender, RoutedEventArgs e)
        {
            mySerialPort.WriteLine(textBoxSend.Text);
        }

        private void ValveOpen(object sender, RoutedEventArgs e)
        {
         //   ValveShow.Fill = System.Windows.Media.Brushes.Green;
            if (mySerialPort.IsOpen)
            {
                mySerialPort.WriteLine("start");
            }
        }

        private void ValveClose(object sender, RoutedEventArgs e)
        {
         //   ValveShow.Fill = System.Windows.Media.Brushes.Gray;
            if (mySerialPort.IsOpen)
            {
                mySerialPort.WriteLine("stop");
            }
        }

        private void btnDownloadSettings_Click(object sender, RoutedEventArgs e)
        {
            string sendString="";

            sendString += Functions.GenerateSendValue(TxtMashInTemp.Text,"MITe");
            sendString += Functions.GenerateSendValue(TxtMashInVolume.Text, "MIVo");
            sendString += Functions.GenerateSendValue(TxtMashStep1Temperature.Text, "M1Te");
            sendString += Functions.GenerateSendValue(TxtMashStep1Time.Text, "M1Ti");
            sendString += Functions.GenerateSendValue(TxtMashStep2Temperature.Text, "M2Te");
            sendString += Functions.GenerateSendValue(TxtMashStep2Time.Text, "M2Ti");
            sendString += Functions.GenerateSendValue(TxtSpargeTemperature.Text, "SpTe");
            sendString += Functions.GenerateSendValue(TxtSpargeVolume.Text, "SpVo");
            sendString += Functions.GenerateSendValue(TxtBoilTime.Text, "BoTi");

            if (mySerialPort.IsOpen)
            {
                mySerialPort.WriteLine(sendString);
                
            }
            else
            {
                MessageBox.Show("Connect to Arduino before download");
            }
            sendString = "";
        }


        private void btnConnect_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                BoilTankVm.SetOn();
                HltVm.SetOn();
                mySerialPort.PortName = DropDownComPorts.SelectedItem.ToString();
                mySerialPort.BaudRate = Convert.ToInt32(DropDownBaudRate.SelectedItem.ToString());
                mySerialPort.Parity = Parity.None;
                mySerialPort.StopBits = StopBits.One;
                mySerialPort.DataBits = 8;
                mySerialPort.Handshake = Handshake.None;
                mySerialPort.Open();
                mySerialPort.DataReceived -= new SerialDataReceivedEventHandler(DataReceivedHandler);
                mySerialPort.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);
            }
            catch
            {
                MessageBox.Show("No connection to arduino", "Connection info", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }

        private void btnDisconnect_Click(object sender, RoutedEventArgs e)
        {
            if (mySerialPort.IsOpen)
            {
                try
                {
                    mySerialPort.Close();
                }
                catch 
                {

                    mySerialPort.Dispose();
                }
                
            }
        }

    }
}
