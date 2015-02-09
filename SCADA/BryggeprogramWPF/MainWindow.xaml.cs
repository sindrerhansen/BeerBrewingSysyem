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

        BrewingData brewingData = new BrewingData();
        
        SerialPort mySerialPort = new SerialPort();
        TankInfo hotLiqureTank = new TankInfo();
        TankInfo mashTank = new TankInfo();
        TankInfo boilTank = new TankInfo();
        
        TankVM BoilTankVm = new TankVM();
        TankVM HltVm = new TankVM();

        BrewingSettings brewingSettings = new BrewingSettings();
        int systemState = 0;
        double ambiantTemperature;

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
            btnConfirm.IsEnabled = false;
            
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
            if (mySerialPort.IsOpen)
            {
                string indata = sp.ReadLine();
                Dispatcher.Invoke(DispatcherPriority.Send, new UpdateUiTextDelegate(WriteData), indata);
            }         
        }

        private void WriteData(string text)
        {
            // Assign the value of the recieved_data to the RichTextBox.
            text.Trim();
            string replacement = Regex.Replace(text, "\r", "");
            textBox.AppendText(replacement);
            textBox.ScrollToEnd();
            
            var textList = Regex.Split(replacement, "_");
            try
            {
                foreach (var item in textList)
                {
                    if (item.StartsWith("STATE"))
                    {
                       
                        int.TryParse(item.Remove(0, 5), out systemState);
                        SystemState.Text = systemState.ToString();
                    }
                    else if (item.StartsWith("Messa"))
                    {
                        var message=item.Remove(0,5);
                        TxtMessageFromSystem.Text= message;
                    }

                    else if (item.StartsWith("TimSp"))
                    {
                        int maxtTme;
                        var message = item.Remove(0, 5);
                        int.TryParse(message, out maxtTme);
                        progressBar.Maximum = maxtTme;
                    }

                    else if (item.StartsWith("RemTi"))
                    {
                        int time;
                        var message = item.Remove(0, 5);
                        int.TryParse(message, out time);
                        progressBar.Value = time;
                        TimeSpan t = TimeSpan.FromSeconds(time);
                        txtTimer.Text = string.Format("{0:D2}h:{1:D2}m:{2:D2}s",
                                        t.Hours,
                                        t.Minutes,
                                        t.Seconds);
                    }

                    else if (item.StartsWith("AmbTe"))
                    {
                        double value;
                        var trimmed = item.Remove(0, 5).Replace(".", ",");
                        double.TryParse(trimmed, out value);
                        ambiantTemperature = value;
                        txtAmbientTemp.Text = value.ToString();
                    }
                                 
                    else if (item.StartsWith("HltSp"))
                    {
                        double value;
                        double.TryParse(item.Remove(0, 5).Replace(".",","), out value);
                        hotLiqureTank.TemperatureSetpoint = value;
                        HLT.TextSetTemp.Text = value.ToString();
                        
                    }
                    else if (item.StartsWith("HltTe"))
                    {
                        double value;
                        var trimmed = item.Remove(0, 5).Replace(".", ",");
                        double.TryParse(trimmed, out value);
                        hotLiqureTank.TemperatureActual = value;
                        HLT.GauageActTemp.Value = value;
                        HLT.TextActuelTemp.Text =value.ToString();
                    }
                    else if (item.StartsWith("HltE1"))
                    {
                        if (item.Remove(0,5).StartsWith("1"))
                        {
                            hotLiqureTank.HeatingElementOn = true;
                            HLT.indicatorHeatingElementOn.Fill = myRedBrush;
                        }
                        else
                        {
                            hotLiqureTank.HeatingElementOn = false;
                            HLT.indicatorHeatingElementOn.Fill = myGrayBrush;
                        }
                    }
                    else if (item.StartsWith("HltCp"))
                    {
                        if (item.Remove(0, 5).StartsWith("1"))
                        {
                            hotLiqureTank.CirculationPumpRunning = true;
                            HLT.indicatorCirculationPumpOn.Fill = myRedBrush;
                        }
                        else
                        {
                            hotLiqureTank.CirculationPumpRunning = false;
                            HLT.indicatorCirculationPumpOn.Fill = myGrayBrush;
                        }
                    }
                    else if (item.StartsWith("HltTp"))
                    {
                        if (item.Remove(0, 5).StartsWith("1"))
                        {
                            hotLiqureTank.TransferPumpRunning = true;
                            HLT.indicatorTransferPumpOn.Fill = myRedBrush;
                        }
                        else
                        {
                            hotLiqureTank.TransferPumpRunning = false;
                            HLT.indicatorTransferPumpOn.Fill = myGrayBrush;
                        }
                    }
                    else if (item.StartsWith("HltVo"))
                    {
                        double value;
                        var trimmed = item.Remove(0, 5).Replace(".", ",");
                        double.TryParse(trimmed, out value);
                        hotLiqureTank.Volume = value;
                        HLT.TxtTankVolume.Text = value.ToString();
                    }

                    else if (item.StartsWith("MatTe"))
                    {
                        double value;
                        var trimmed = item.Remove(0, 5).Replace(".", ",");
                        double.TryParse(trimmed, out value);
                        mashTank.TemperatureActual = value;
                        MashTank.GauageActTemp.Value = value;
                        MashTank.TextActuelTemp.Text = value.ToString();
                    }
                    else if (item.StartsWith("MatSp"))
                    {
                        double value;
                        double.TryParse(item.Remove(0, 5).Replace(".", ","), out value);
                        mashTank.TemperatureSetpoint = value;
                        MashTank.TextSetTemp.Text = value.ToString();

                    }
                    else if (item.StartsWith("MatE1"))
                    {
                        if (item.Remove(0, 5).StartsWith("1"))
                        {
                            mashTank.HeatingElementOn = true;
                            MashTank.indicatorHeatingElementOn.Fill = myRedBrush;
                        }
                        else
                        {
                            mashTank.HeatingElementOn = false;
                            MashTank.indicatorHeatingElementOn.Fill = myGrayBrush;
                        }
                    }
                    else if (item.StartsWith("MatCp"))
                    {
                        if (item.Remove(0, 5).StartsWith("1"))
                        {
                            mashTank.CirculationPumpRunning = true;
                            MashTank.indicatorCirculationPumpOn.Fill = myRedBrush;
                        }
                        else
                        {
                            mashTank.CirculationPumpRunning = false;
                            MashTank.indicatorCirculationPumpOn.Fill = myGrayBrush;
                        }
                    }
                    else if (item.StartsWith("MatTp"))
                    {
                        if (item.Remove(0, 5).StartsWith("1"))
                        {
                            mashTank.TransferPumpRunning = true;
                            MashTank.indicatorTransferPumpOn.Fill = myRedBrush;
                        }
                        else
                        {
                            mashTank.TransferPumpRunning = false;
                            MashTank.indicatorTransferPumpOn.Fill = myGrayBrush;
                        }
                    }
                    else if (item.StartsWith("MatVo"))
                    {
                        double value;
                        var trimmed = item.Remove(0, 5).Replace(".", ",");
                        double.TryParse(trimmed, out value);
                        mashTank.Volume = value;
                        MashTank.TxtTankVolume.Text = value.ToString();
                    }

                    else if (item.StartsWith("BotTe"))
                    {
                        double value;
                        var trimmed = item.Remove(0, 5).Replace(".", ",");
                        double.TryParse(trimmed, out value);
                        boilTank.TemperatureActual = value;
                        BoilTank.GauageActTemp.Value = value;
                        BoilTank.TextActuelTemp.Text = value.ToString();
                    }
                    else if (item.StartsWith("BotSp"))
                    {
                        double value;
                        double.TryParse(item.Remove(0, 5).Replace(".", ","), out value);
                        boilTank.TemperatureSetpoint = value;
                        BoilTank.TextSetTemp.Text = value.ToString();

                    }
                    else if (item.StartsWith("BotE1"))
                    {
                        if (item.Remove(0, 5).StartsWith("1"))
                        {
                            boilTank.HeatingElementOn = true;
                            BoilTank.indicatorHeatingElementOn.Fill = myRedBrush;
                        }
                        else
                        {
                            boilTank.HeatingElementOn = false;
                            BoilTank.indicatorHeatingElementOn.Fill = myGrayBrush;
                        }
                    }
                    else if (item.StartsWith("BotCp"))
                    {
                        if (item.Remove(0, 5).StartsWith("1"))
                        {
                            boilTank.CirculationPumpRunning = true;
                            BoilTank.indicatorCirculationPumpOn.Fill = myRedBrush;
                        }
                        else
                        {
                            boilTank.CirculationPumpRunning = false;
                            BoilTank.indicatorCirculationPumpOn.Fill = myGrayBrush;
                        }
                    }
                    else if (item.StartsWith("BotTp"))
                    {
                        if (item.Remove(0, 5).StartsWith("1"))
                        {
                            boilTank.TransferPumpRunning = true;
                            BoilTank.indicatorTransferPumpOn.Fill = myRedBrush;
                        }
                        else
                        {
                            boilTank.TransferPumpRunning = false;
                            BoilTank.indicatorTransferPumpOn.Fill = myGrayBrush;
                        }
                    }
                    else if (item.StartsWith("BotVo"))
                    {
                        double value;
                        var trimmed = item.Remove(0, 5).Replace(".", ",");
                        double.TryParse(trimmed, out value);
                        boilTank.Volume = value;
                        BoilTank.TxtTankVolume.Text = value.ToString();
                    }


                }                
            }

            catch (Exception ex) {
                textBoxError.Text = ex.ToString();
            }
        }

        private void send_Click(object sender, RoutedEventArgs e)
        {
            mySerialPort.WriteLine(textBoxSend.Text);
        }

        private void btnDownloadSettings_Click(object sender, RoutedEventArgs e)
        {
            string sendString="";

            sendString += Functions.GenerateSendValue(TxtMashInTemp.Text,"MITe");
            sendString += Functions.GenerateSendValue(TxtMashInHltTemp.Text, "MIHT");
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

        private void SetBrewingData(BrewingData data)
        {
            TxtMashInTemp.Text = data.MashInTemperature.ToString();
            TxtMashInHltTemp.Text = data.MashInHltTemperature.ToString();
            TxtMashInVolume.Text = data.MashInVolume.ToString();
            TxtMashStep1Temperature.Text = data.MashStep1Temperature.ToString();
            TxtMashStep1Time.Text = data.MashStep1Time.ToString();
            TxtMashStep2Temperature.Text = data.MashStep2Temperature.ToString();
            TxtMashStep2Time.Text = data.MashStep2Time.ToString();
            TxtSpargeTemperature.Text = data.SpargeTemperature.ToString();
            TxtSpargeVolume.Text = data.SpargeVolume.ToString();
            TxtBoilTime.Text = data.BoilTime.ToString();
        }

        private void btnConfirm_Click(object sender, RoutedEventArgs e)
        {
            if (mySerialPort.IsOpen)
            {
                mySerialPort.WriteLine("CONFIRMED");
            }
            else { MessageBox.Show("No connection to arduino", "Connection info", MessageBoxButton.OK, MessageBoxImage.Exclamation); }
        }

        private void btnGetSettings_Click(object sender, RoutedEventArgs e)
        {
            var brewData = brewingData.ReadData();
            SetBrewingData(brewData);
        }

        private void btnPrepareBrewing_Click(object sender, RoutedEventArgs e)
        {
            if (mySerialPort.IsOpen)
            {
                mySerialPort.WriteLine("CMD10");
            }
        }

        private void btnStartBrewing_Click(object sender, RoutedEventArgs e)
        {
            if (mySerialPort.IsOpen)
            {
                mySerialPort.WriteLine("CMD20");
            }
        }

        private void btnStoreSettings_Click(object sender, RoutedEventArgs e)
        {
            BrewingData brewingData = new BrewingData();
            double dvalue;
            int ivalue;
            if (double.TryParse(TxtMashInHltTemp.Text.Replace(".", ","), out dvalue))
            { brewingData.MashInHltTemperature = dvalue; }

            if (double.TryParse(TxtMashInTemp.Text.Replace(".", ","), out dvalue))
            { brewingData.MashInTemperature = dvalue; }

            if (int.TryParse(TxtMashInVolume.Text.Replace(".", ","), out ivalue))
            { brewingData.MashInVolume = ivalue; }

            if (double.TryParse(TxtMashStep1Temperature.Text.Replace(".", ","), out dvalue))
            { brewingData.MashStep1Temperature = dvalue; }

            if (int.TryParse(TxtMashStep1Time.Text.Replace(".", ","), out ivalue))
            { brewingData.MashStep1Time = ivalue; }

            if (double.TryParse(TxtMashStep2Temperature.Text.Replace(".", ","), out dvalue))
            { brewingData.MashStep2Temperature = dvalue; }

            if (int.TryParse(TxtMashStep2Time.Text.Replace(".", ","), out ivalue))
            { brewingData.MashStep2Time = ivalue; }

            if (double.TryParse(TxtSpargeTemperature.Text.Replace(".", ","), out dvalue))
            { brewingData.SpargeTemperature = dvalue; }

            if (double.TryParse(TxtSpargeVolume.Text.Replace(".", ","), out dvalue))
            { brewingData.SpargeVolume = dvalue; }

            if (int.TryParse(TxtBoilTime.Text.Replace(".", ","), out ivalue))
            { brewingData.BoilTime = ivalue; }

            brewingData.SaveData(brewingData);
        }

        private void TxtMessageFromSystem_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (TxtMessageFromSystem.Text!="")
            {
                TxtMessageFromSystem.Background = new SolidColorBrush(Colors.Yellow);
                btnConfirm.IsEnabled = true;
            }
            else
            {
                btnConfirm.IsEnabled = false;
                TxtMessageFromSystem.Background = new SolidColorBrush(Colors.White);
            }
        }

    }
}
