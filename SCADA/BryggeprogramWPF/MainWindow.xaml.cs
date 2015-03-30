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
using OxyPlot;
using OxyPlot.Axes;


namespace BryggeprogramWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private MainViewModel mainViewModel;

        BrewingData brewingData = new BrewingData();
        
        SerialPort mySerialPort = new SerialPort();
        TankData hotLiqureTank = new TankData();
        TankData mashTank = new TankData();
        TankData boilTank = new TankData();
        
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
            mainViewModel = new MainViewModel();
            InitializeComponent();
            this.DataContext = mainViewModel;
            
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

            //Random r = new Random();

            //var _now = DateTime.Now;
            //chart.HLT.Add(new DataPoint(DateTimeAxis.ToDouble(_now), r.NextDouble()));
            //chart.MashTank.Add(new DataPoint(DateTimeAxis.ToDouble(_now), r.NextDouble()));
            //chart.BoilTank.Add(new DataPoint(DateTimeAxis.ToDouble(_now), r.NextDouble()));
            
            //Plot.InvalidatePlot();


            if (mySerialPort.IsOpen)
            {
                indConnected.Fill = myGreenBrush;
                btnStartBrewing.IsEnabled = true;
                btnPrepareBrewing.IsEnabled = true;
            }
            else
            {
                indConnected.Fill = myGrayBrush;
                btnStartBrewing.IsEnabled = false;
                btnPrepareBrewing.IsEnabled = false;
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
                try
                {
                    string indata = sp.ReadLine();
                    Dispatcher.Invoke(DispatcherPriority.Send, new UpdateUiTextDelegate(WriteData), indata);
                }
                catch (Exception)
                {
                    
            
                }
               
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
            var values = new List<double>();
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
                        var trimmed = item.Remove(0, 5).Replace(',', '.');
                        double.TryParse(trimmed, out value);
                        values.Add(value);
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
                        var trimmed = item.Remove(0, 5).Replace(',', '.');
                        double.TryParse(trimmed, out value);
                        values.Add(value);
                        hotLiqureTank.TemperatureActual.SensorValue = value;
                        HLT.GauageActTemp.Value = value;
                        HLT.TextActuelTemp.Text =value.ToString();
                    }
                    else if (item.StartsWith("HltE1"))
                    {
                        if (item.Remove(0,5).StartsWith("1"))
                        {
                            hotLiqureTank.HeatingElement.On = true;
                            HLT.indicatorHeatingElementOn.Fill = myRedBrush;
                        }
                        else
                        {
                            hotLiqureTank.HeatingElement.On = false;
                            HLT.indicatorHeatingElementOn.Fill = myGrayBrush;
                        }
                    }
                    else if (item.StartsWith("HltCp"))
                    {
                        if (item.Remove(0, 5).StartsWith("1"))
                        {
                            hotLiqureTank.CirculationPump.On = true;
                            HLT.indicatorCirculationPumpOn.Fill = myRedBrush;
                        }
                        else
                        {
                            hotLiqureTank.CirculationPump.On = false;
                            HLT.indicatorCirculationPumpOn.Fill = myGrayBrush;
                        }
                    }
                    else if (item.StartsWith("HltTp"))
                    {
                        if (item.Remove(0, 5).StartsWith("1"))
                        {
                            hotLiqureTank.TransferPump.On = true;
                            HLT.indicatorTransferPumpOn.Fill = myRedBrush;
                        }
                        else
                        {
                            hotLiqureTank.TransferPump.On = false;
                            HLT.indicatorTransferPumpOn.Fill = myGrayBrush;
                        }
                    }
                    else if (item.StartsWith("HltVo"))
                    {
                        double value;
                        var trimmed = item.Remove(0, 5).Replace(',', '.');
                        double.TryParse(trimmed, out value);
                        hotLiqureTank.Volume.SensorValue = value;
                        HLT.TxtTankVolume.Text = value.ToString();
                    }

                    else if (item.StartsWith("MatTe"))
                    {
                        double value;
                        var trimmed = item.Remove(0, 5).Replace(',', '.');
                        double.TryParse(trimmed, out value);
                        values.Add(value);
                        mashTank.TemperatureActual.SensorValue = value;
                        MashTank.GauageActTemp.Value = value;
                        MashTank.TextActuelTemp.Text = value.ToString();
                    }

                    else if (item.StartsWith("MarTe"))
                    {
                                              double value;
                        var trimmed = item.Remove(0, 5).Replace(',', '.');
                        double.TryParse(trimmed, out value);
                        mashTank.HeatingElementReturTemperature.SensorValue = value;
                        MashTank.txtTemperatureAfterHeate.Text = value.ToString();
                    }
                    else if (item.StartsWith("MatSp"))
                    {
                        double value;
                        double.TryParse(item.Remove(0, 5).Replace(',', '.'), out value);
                        mashTank.TemperatureSetpoint = value;
                        MashTank.TextSetTemp.Text = value.ToString();

                    }
                    else if (item.StartsWith("MatE1"))
                    {
                        if (item.Remove(0, 5).StartsWith("1"))
                        {
                            mashTank.HeatingElement.On = true;
                            MashTank.indicatorHeatingElementOn.Fill = myRedBrush;
                        }
                        else
                        {
                            mashTank.HeatingElement.On = false;
                            MashTank.indicatorHeatingElementOn.Fill = myGrayBrush;
                        }
                    }
                    else if (item.StartsWith("MatCp"))
                    {
                        if (item.Remove(0, 5).StartsWith("1"))
                        {
                            mashTank.CirculationPump.On = true;
                            MashTank.indicatorCirculationPumpOn.Fill = myRedBrush;
                        }
                        else
                        {
                            mashTank.CirculationPump.On = false;
                            MashTank.indicatorCirculationPumpOn.Fill = myGrayBrush;
                        }
                    }
                    else if (item.StartsWith("MatTp"))
                    {
                        if (item.Remove(0, 5).StartsWith("1"))
                        {
                            mashTank.TransferPump.On = true;
                            MashTank.indicatorTransferPumpOn.Fill = myRedBrush;
                        }
                        else
                        {
                            mashTank.TransferPump.On = false;
                            MashTank.indicatorTransferPumpOn.Fill = myGrayBrush;
                        }
                    }
                    else if (item.StartsWith("MatVo"))
                    {
                        double value;
                        var trimmed = item.Remove(0, 5).Replace(',', '.');
                        double.TryParse(trimmed, out value);
                        values.Add(value);
                        mashTank.Volume.SensorValue = value;
                        MashTank.TxtTankVolume.Text = value.ToString();
                    }

                    else if (item.StartsWith("BotTe"))
                    {
                        double value;
                        var trimmed = item.Remove(0, 5).Replace(',', '.');
                        double.TryParse(trimmed, out value);
                        values.Add(value);
                        boilTank.TemperatureActual.SensorValue = value;
                        BoilTank.GauageActTemp.Value = value;
                        BoilTank.TextActuelTemp.Text = value.ToString();
                    }
                    else if (item.StartsWith("BotSp"))
                    {
                        double value;
                        double.TryParse(item.Remove(0, 5).Replace(',', '.'), out value);
                        boilTank.TemperatureSetpoint = value;
                        BoilTank.TextSetTemp.Text = value.ToString();

                    }
                    else if (item.StartsWith("BotE1"))
                    {
                        if (item.Remove(0, 5).StartsWith("1"))
                        {
                            boilTank.HeatingElement.On = true;
                            BoilTank.indicatorHeatingElementOn.Fill = myRedBrush;
                        }
                        else
                        {
                            boilTank.HeatingElement.On = false;
                            BoilTank.indicatorHeatingElementOn.Fill = myGrayBrush;
                        }
                    }
                    else if (item.StartsWith("BotCp"))
                    {
                        if (item.Remove(0, 5).StartsWith("1"))
                        {
                            boilTank.CirculationPump.On = true;
                            BoilTank.indicatorCirculationPumpOn.Fill = myRedBrush;
                        }
                        else
                        {
                            boilTank.CirculationPump.On = false;
                            BoilTank.indicatorCirculationPumpOn.Fill = myGrayBrush;
                        }
                    }
                    else if (item.StartsWith("BotTp"))
                    {
                        if (item.Remove(0, 5).StartsWith("1"))
                        {
                            boilTank.TransferPump.On = true;
                            BoilTank.indicatorTransferPumpOn.Fill = myRedBrush;
                        }
                        else
                        {
                            boilTank.TransferPump.On = false;
                            BoilTank.indicatorTransferPumpOn.Fill = myGrayBrush;
                        }
                    }
                    else if (item.StartsWith("BotVo"))
                    {
                        double value;
                        var trimmed = item.Remove(0, 5).Replace(',', '.');
                        double.TryParse(trimmed, out value);
                        boilTank.Volume.SensorValue = value;
                        BoilTank.TxtTankVolume.Text = value.ToString();
                    }

                    else if (item.StartsWith("ConSe"))
                    {
                        //var values = new List<string>();
                        var ConSe = Regex.Split(item, ":");
                    }


                }
               
                try
                {

                    if (values.Count > 0)
                    {
                        mainViewModel.UpdateModel(values);
                        Plot.InvalidatePlot();
                    }

                }
                catch (Exception e)
                {
                    String message = "Error in updating chart: " + e.ToString();
                    MessageBox.Show(message);

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
            

            sendString = "SET";
            sendString += TxtMashInTemp.Text + "_";                //MITe
            sendString += TxtMashInHltTemp.Text + "_";             //MIHT
            sendString += TxtMashInVolume.Text + "_";              //MIVo
            sendString += TxtMashStep1Temperature.Text + "_";      //M1Te
            sendString += TxtMashStep1Time.Text + "_";             //M1Ti
            sendString += TxtMashStep2Temperature.Text + "_";      //M2Te
            sendString += TxtMashStep2Time.Text + "_";             //M2Ti
            sendString += TxtMashStep3Temperature.Text + "_";      //M3Te
            sendString += TxtMashStep3Time.Text + "_";             //M3Ti
            sendString += TxtSpargeTemperature.Text + "_";         //SpTe
            sendString += TxtSpargeVolume.Text + "_";              //SpVo
            sendString += TxtBoilTime.Text + "_";                  //BoTi

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
                mySerialPort.PortName = DropDownComPorts.SelectedItem.ToString();
                mySerialPort.BaudRate = Convert.ToInt32(DropDownBaudRate.SelectedItem.ToString());
                mySerialPort.Parity = Parity.None;
                mySerialPort.StopBits = StopBits.One;
                mySerialPort.DataBits = 8;
                mySerialPort.Handshake = Handshake.None;
 //               mySerialPort.WriteTimeout = 500;
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
            try
            {
                TxtMashInTemp.Text = data.MashInTemperature.ToString();
                TxtMashInHltTemp.Text = data.MashInHltTemperature.ToString();
                TxtMashInVolume.Text = data.MashInVolume.ToString();
                TxtMashStep1Temperature.Text = data.MashStep1Temperature.ToString();
                TxtMashStep1Time.Text = data.MashStep1Time.ToString();
                TxtMashStep2Temperature.Text = data.MashStep2Temperature.ToString();
                TxtMashStep2Time.Text = data.MashStep2Time.ToString();
                TxtMashStep3Temperature.Text = data.MashStep3Temperature.ToString();
                TxtMashStep3Time.Text = data.MashStep3Time.ToString();
                TxtSpargeTemperature.Text = data.SpargeTemperature.ToString();
                TxtSpargeVolume.Text = data.SpargeVolume.ToString();
                TxtBoilTime.Text = data.BoilTime.ToString();
            }
            catch
            {}

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
            if (double.TryParse(TxtMashInHltTemp.Text.Replace(',', '.'), out dvalue))
            { brewingData.MashInHltTemperature = dvalue; }

            if (double.TryParse(TxtMashInTemp.Text.Replace(',', '.'), out dvalue))
            { brewingData.MashInTemperature = dvalue; }

            if (int.TryParse(TxtMashInVolume.Text.Replace(',', '.'), out ivalue))
            { brewingData.MashInVolume = ivalue; }

            if (double.TryParse(TxtMashStep1Temperature.Text.Replace(',', '.'), out dvalue))
            { brewingData.MashStep1Temperature = dvalue; }

            if (int.TryParse(TxtMashStep1Time.Text.Replace(',', '.'), out ivalue))
            { brewingData.MashStep1Time = ivalue; }

            if (double.TryParse(TxtMashStep2Temperature.Text.Replace(',', '.'), out dvalue))
            { brewingData.MashStep2Temperature = dvalue; }

            if (int.TryParse(TxtMashStep2Time.Text.Replace(',', '.'), out ivalue))
            { brewingData.MashStep2Time = ivalue; }

            if (double.TryParse(TxtMashStep3Temperature.Text.Replace(',', '.'), out dvalue))
            { brewingData.MashStep3Temperature = dvalue; }

            if (int.TryParse(TxtMashStep3Time.Text.Replace(',', '.'), out ivalue))
            { brewingData.MashStep3Time = ivalue; }

            if (double.TryParse(TxtSpargeTemperature.Text.Replace(',', '.'), out dvalue))
            { brewingData.SpargeTemperature = dvalue; }

            if (double.TryParse(TxtSpargeVolume.Text.Replace(',', '.'), out dvalue))
            { brewingData.SpargeVolume = dvalue; }

            if (int.TryParse(TxtBoilTime.Text.Replace(',', '.'), out ivalue))
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

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            if (mySerialPort.IsOpen)
            {
                mySerialPort.WriteLine("CMD0");
            }
        }

        public void SendOverrideCommandHeatingElement(object sender, EventArgs args)
        {
            string _overrideMessage = "OVERRIDE";
            var tank = (TankData)sender;
            MessageBox.Show(tank.HeatingElement.Override.ToString());
  
            int o = tank.HeatingElement.Override ? 1 : 0;
            

            _overrideMessage += 20 + o;
   
	      
            
        }

        public void SendOverrideCommandCirculationPump(object sender, EventArgs args)
        {
            var tank = (TankData)sender;
            MessageBox.Show(tank.CirculationPump.Override.ToString());
        }

        public void SendOverrideCommandTransferPump(object sender, EventArgs args)
        {
            var tank = (TankData)sender;
            MessageBox.Show(tank.TransferPump.Override.ToString());
        }

        private void btnAddPoint_Click(object sender, RoutedEventArgs e)
        {

            
        }

    }
}
