using System;
using System.Data;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using System.IO.Ports;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Globalization;
using GalaSoft.MvvmLight;
using BryggeprogramWPF.Messages;
using BryggeprogramWPF.Model;
using BryggeprogramWPF.ViewModel;

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

        int systemState = 0;
        int systemCleaningState = 0;
        double ambiantTemperature;

        int x = 0;

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
            x++;
            mainViewModel.CurrentDateTime = DateTime.Now;
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
                if (tglSimulateArduino.IsChecked==true)
                {
                    var indata = GennerateSimulatedArduinoValues();
                    Dispatcher.Invoke(DispatcherPriority.Send, new UpdateUiTextDelegate(DecodeDataString), indata);
                }
            }
        }

        private string GennerateSimulatedArduinoValues()
        {
            string ret="";
            ret += "STATE" + "10" + "_";
            ret += "AmbTe" + (10+10*Math.Sin(x)).ToString().Replace(',','.') + "_";
            ret += "HltTe" + "30" + "_";
            ret += "MatTe" + "35" + "_";
            ret += "BotTe" + "39" + "_";
            ret += "MatVo" + "0" + "_";
            return ret; 
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
                    Dispatcher.Invoke(DispatcherPriority.Send, new UpdateUiTextDelegate(DecodeDataString), indata);
                }
                catch (Exception)
                {
                    
            
                }
               
            }         
        }

        private void DecodeDataString(string text)
        {
            // Assign the value of the recieved_data to the RichTextBox.
            text.Trim();
            string replacement = Regex.Replace(text, "\r", "");
            textBox.Text = replacement;
            ProsessData _prosessData = new ProsessData();
            var textList = Regex.Split(replacement, "_");
            var values = new List<double>();
            try
            {
                foreach (var item in textList)
                {
                    if (item.StartsWith("STATE"))
                    {                     
                        int.TryParse(item.Remove(0, 5), out systemState);
                        mainViewModel.BrewingState = systemState;
                    }
                    else if (item.StartsWith("CleSt"))
                    {
                        int.TryParse(item.Remove(0, 5), out systemCleaningState);
                        mainViewModel.CleaningState = systemCleaningState;
                    }
                    else if (item.StartsWith("Messa"))
                    {
                        var message=item.Remove(0,5);
                        TxtMessageFromSystem.Text= message;
                        mainViewModel.MessageFromSystem = message;
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
                        mainViewModel.Timer = string.Format("{0:D2}h:{1:D2}m:{2:D2}s",
                                        t.Hours,
                                        t.Minutes,
                                        t.Seconds);
                        
                    }
                    else if (item.StartsWith("AmbTe"))
                    {
                        double value;
                        value = double.Parse(item.Remove(0, 5), CultureInfo.InvariantCulture);
                        values.Add(value);
                        ambiantTemperature = value;
                        txtAmbientTemp.Text = value.ToString();
                 
                    }                               
                    else if (item.StartsWith("HltSp"))
                    {
                        double value;
                        value = double.Parse(item.Remove(0, 5), CultureInfo.InvariantCulture);
                        hotLiqureTank.TemperatureSetpoint = value;
                        HLT.TextSetTemp.Text = value.ToString();
                        mainViewModel.HotLiquidTankTemperatureSetpoint = value;
                        _prosessData.HLT.TemperatureSetpoint = value;
                    }
                    else if (item.StartsWith("HltTe"))
                    {
                        double value;
                        value = double.Parse(item.Remove(0, 5), CultureInfo.InvariantCulture);
                        values.Add(value);
                        hotLiqureTank.TemperatureActual.SensorValue = value;
                        HLT.GauageActTemp.Value = value;
                        HLT.TextActuelTemp.Text =value.ToString();
                        mainViewModel.HotLiquidTankTemperature = value;
                        _prosessData.HLT.Temperature = value;
                    }
                    else if (item.StartsWith("HltE1"))
                    {
                        if (item.Remove(0,5).StartsWith("1"))
                        {
                            hotLiqureTank.HeatingElement.On = true;
                            HLT.indicatorHeatingElementOn.Fill = myRedBrush;
                            mainViewModel.HotLiquidTankElement = true;
                            _prosessData.HLT.HeatingElementOn = true;
                        }
                        else
                        {
                            hotLiqureTank.HeatingElement.On = false;
                            HLT.indicatorHeatingElementOn.Fill = myGrayBrush;
                            mainViewModel.HotLiquidTankElement = false;
                            _prosessData.HLT.HeatingElementOn = false;
                        }
                    }
                    else if (item.StartsWith("HltCp"))
                    {
                        if (item.Remove(0, 5).StartsWith("1"))
                        {
                            hotLiqureTank.CirculationPump.On = true;
                            HLT.indicatorCirculationPumpOn.Fill = myRedBrush;
                            _prosessData.HLT.CirculationPump.Running = true;
                        }
                        else
                        {
                            hotLiqureTank.CirculationPump.On = false;
                            HLT.indicatorCirculationPumpOn.Fill = myGrayBrush;
                            _prosessData.HLT.CirculationPump.Running = false;
                        }
                    }
                    else if (item.StartsWith("HltTp"))
                    {
                        if (item.Remove(0, 5).StartsWith("1"))
                        {
                            hotLiqureTank.TransferPump.On = true;
                            HLT.indicatorTransferPumpOn.Fill = myRedBrush;
                            _prosessData.HLT.TransferPump.Running = true;
                        }
                        else
                        {
                            hotLiqureTank.TransferPump.On = false;
                            HLT.indicatorTransferPumpOn.Fill = myGrayBrush;
                            _prosessData.HLT.TransferPump.Running = false;
                        }
                    }
                    else if (item.StartsWith("HltVo"))
                    {
                        double value;
                        value = double.Parse(item.Remove(0, 5), CultureInfo.InvariantCulture);
                        hotLiqureTank.Volume.SensorValue = value;
                        HLT.TxtTankVolume.Text = value.ToString();
                        _prosessData.HLT.CurrentVolume = value;
                    }
                    else if (item.StartsWith("MatTe"))
                    {
                        double value;
                        value = double.Parse(item.Remove(0, 5), CultureInfo.InvariantCulture);
                        values.Add(value);
                        mashTank.TemperatureActual.SensorValue = value;
                        MashTank.GauageActTemp.Value = value;
                        MashTank.TextActuelTemp.Text = value.ToString();
                        mainViewModel.MeshTankTemperature = value;
                        _prosessData.MashTank.Temperature = value;
                    }
                    else if (item.StartsWith("RimsO"))
                    {
                        double value;
                        value = double.Parse(item.Remove(0, 5), CultureInfo.InvariantCulture);
                        mainViewModel.MeshTankRimsOutesideTemperature = value;
                        _prosessData.MashTank.RIMS.OutesideTemperature = value;
                    }
                    else if (item.StartsWith("MarTe"))
                    {
                        double value;
                        value = double.Parse(item.Remove(0, 5), CultureInfo.InvariantCulture);
                        mashTank.HeatingElementReturTemperature.SensorValue = value;
                        mainViewModel.MeshTankRimsReturTemperature = value;
                        MashTank.txtTemperatureAfterHeate.Text = value.ToString();
                        _prosessData.MashTank.Temperature = value;
                    }
                    else if (item.StartsWith("MatSp"))
                    {
                        double value;
                        value = double.Parse(item.Remove(0, 5), CultureInfo.InvariantCulture);
                        mashTank.TemperatureSetpoint = value;
                        MashTank.TextSetTemp.Text = value.ToString();
                        mainViewModel.MeshTankTemperatureSetpoint = value;
                        _prosessData.MashTank.TemperatureSetpoint = value;

                    }
                    else if (item.StartsWith("MatE1"))
                    {
                        if (item.Remove(0, 5).StartsWith("1"))
                        {
                            mashTank.HeatingElement.On = true;
                            MashTank.indicatorHeatingElementOn.Fill = myRedBrush;
                            _prosessData.MashTank.RIMS.ElementOn = true;
                        }
                        else
                        {
                            mashTank.HeatingElement.On = false;
                            MashTank.indicatorHeatingElementOn.Fill = myGrayBrush;
                            _prosessData.MashTank.RIMS.ElementOn = false;
                        }
                    }
                    else if (item.StartsWith("MatCp"))
                    {
                        if (item.Remove(0, 5).StartsWith("1"))
                        {
                            mashTank.CirculationPump.On = true;
                            MashTank.indicatorCirculationPumpOn.Fill = myRedBrush;
                            _prosessData.MashTank.CirculationPump.Running = true;
                        }
                        else
                        {
                            mashTank.CirculationPump.On = false;
                            MashTank.indicatorCirculationPumpOn.Fill = myGrayBrush;
                            _prosessData.MashTank.CirculationPump.Running = false;
                        }
                    }
                    else if (item.StartsWith("MatTp"))
                    {
                        if (item.Remove(0, 5).StartsWith("1"))
                        {
                            mashTank.TransferPump.On = true;
                            MashTank.indicatorTransferPumpOn.Fill = myRedBrush;
                            _prosessData.MashTank.TransferPump.Running=true;        
                        }
                        else
                        {
                            mashTank.TransferPump.On = false;
                            MashTank.indicatorTransferPumpOn.Fill = myGrayBrush;
                            _prosessData.MashTank.TransferPump.Running = false;
                        }
                    }
                    else if (item.StartsWith("MatAV"))
                    {
                        double value;
                        value = double.Parse(item.Remove(0, 5), CultureInfo.InvariantCulture);
                        _prosessData.MashTank.AddedVolume = value;
                        
                    }
                    else if (item.StartsWith("MatVo"))
                    {
                        double value;
                        value = double.Parse(item.Remove(0, 5), CultureInfo.InvariantCulture);
                        values.Add(value);
                        mashTank.Volume.SensorValue = value;
                        MashTank.TxtTankVolume.Text = value.ToString();
                        mainViewModel.MeshTankVolume = value;
                        _prosessData.MashTank.CurrentVolume = value;
                    }

                    else if (item.StartsWith("BotTe"))
                    {
                        double value;
                        value = double.Parse(item.Remove(0, 5), CultureInfo.InvariantCulture);
                        values.Add(value);
                        boilTank.TemperatureActual.SensorValue = value;
                        BoilTank.GauageActTemp.Value = value;
                        BoilTank.TextActuelTemp.Text = value.ToString();
                        mainViewModel.BoilTankTemperature = value;
                    }
                    else if (item.StartsWith("BotSp"))
                    {
                        double value;
                        value = double.Parse(item.Remove(0, 5), CultureInfo.InvariantCulture);
                        boilTank.TemperatureSetpoint = value;
                        BoilTank.TextSetTemp.Text = value.ToString();
                        mainViewModel.BoilTankTemperatureSetpoint = value;

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
                        value = double.Parse(item.Remove(0, 5), CultureInfo.InvariantCulture);
                        boilTank.Volume.SensorValue = value;
                        mainViewModel.BoilTankVolume = value;
                        BoilTank.TxtTankVolume.Text = value.ToString();
                    }

                    else if (item.StartsWith("ConSe"))
                    {
                        var ConSe = Regex.Split(item, ":");
                    }


                }

                var mes = new MessengerInstanceWrappr(_prosessData);
                mes.Cleanup();

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
            sendString += TxtMashInTemp.Text.Replace(',','.') + "_";                //MITe
            sendString += TxtMashInHltTemp.Text.Replace(',','.') + "_";             //MIHT
            sendString += TxtMashInVolume.Text.Replace(',', '.') + "_";              //MIVo
            sendString += TxtMashStep1Temperature.Text.Replace(',', '.') + "_";      //M1Te
            sendString += TxtMashStep1Time.Text.Replace(',', '.') + "_";             //M1Ti
            sendString += TxtMashStep2Temperature.Text.Replace(',', '.') + "_";      //M2Te
            sendString += TxtMashStep2Time.Text.Replace(',', '.') + "_";             //M2Ti
            sendString += TxtMashStep3Temperature.Text.Replace(',', '.') + "_";      //M3Te
            sendString += TxtMashStep3Time.Text.Replace(',', '.') + "_";             //M3Ti
            sendString += TxtSpargeTemperature.Text.Replace(',', '.') + "_";         //SpTe
            sendString += TxtSpargeVolume.Text.Replace(',', '.') + "_";              //SpVo
            sendString += TxtBoilTime.Text.Replace(',', '.') + "_";                  //BoTi

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

        private void btnSaveSettings_Click(object sender, RoutedEventArgs e)
        {
            BrewingData brewingData = new BrewingData();
            DataTable datatable = new DataTable();


            brewingData.MashInHltTemperature = double.Parse(TxtMashInHltTemp.Text.Replace(',','.'), CultureInfo.InvariantCulture);

            brewingData.MashInTemperature = double.Parse(TxtMashInTemp.Text.Replace(',', '.'), CultureInfo.InvariantCulture);

            brewingData.MashInVolume = double.Parse(TxtMashInVolume.Text.Replace(',','.'), CultureInfo.InvariantCulture);

            brewingData.MashStep1Temperature = double.Parse(TxtMashStep1Temperature.Text.Replace(',','.'), CultureInfo.InvariantCulture);

            brewingData.MashStep1Time = int.Parse(TxtMashStep1Time.Text.Replace(',','.'), CultureInfo.InvariantCulture);

            brewingData.MashStep2Temperature = double.Parse(TxtMashStep2Temperature.Text.Replace(',','.'), CultureInfo.InvariantCulture);

            brewingData.MashStep2Time = int.Parse(TxtMashStep2Time.Text.Replace(',','.'), CultureInfo.InvariantCulture);

            brewingData.MashStep3Temperature = double.Parse(TxtMashStep3Temperature.Text.Replace(',', '.'), CultureInfo.InvariantCulture);

            brewingData.MashStep3Time = int.Parse(TxtMashStep3Time.Text.Replace(',','.'),CultureInfo.InvariantCulture);

            brewingData.SpargeTemperature = double.Parse(TxtSpargeTemperature.Text.Replace(',','.'), CultureInfo.InvariantCulture);

            brewingData.SpargeVolume = double.Parse(TxtSpargeVolume.Text.Replace(',','.'), CultureInfo.InvariantCulture);

            brewingData.BoilTime = int.Parse(TxtBoilTime.Text.Replace(',','.'), CultureInfo.InvariantCulture);

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

        private void btnResetFlowSensor_Click(object sender, RoutedEventArgs e)
        {
            if (mySerialPort.IsOpen)
            {
                mySerialPort.WriteLine("FLOW_RES");
            }
        }

        private void txtTimerDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ProsessWindow prosessWindow = new ProsessWindow();
            ProsessViewModel prosessViewModel = new ProsessViewModel();
            prosessWindow.DataContext = prosessViewModel;
            prosessWindow.Show();

        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
         
        }

        private void tglSimulateArduino_Click(object sender, RoutedEventArgs e)
        {
            if (tglSimulateArduino.IsChecked == true)
            {

                btnConnect.IsEnabled = false;
            }
            else
            {
                btnConnect.IsEnabled = true;
            }
        }

        private void btnCleanSystem_Click(object sender, RoutedEventArgs e)
        {
            if (mySerialPort.IsOpen && mainViewModel.BrewingState == 0)
            {
                var mBoxRes = MessageBox.Show("Sure you want to start cleaning sequence?", "Important Question", MessageBoxButton.YesNo, MessageBoxImage.Exclamation);
                if (mBoxRes==MessageBoxResult.Yes)
                {
                    mySerialPort.WriteLine("CLEAN");
                }
            }
            else
            {
                MessageBox.Show("Can not start cleaning sequense when not connected or system state is not 0");
            }
            
        }

        private void btnPrepCleanSystem_Click(object sender, RoutedEventArgs e)
        {
            if (mySerialPort.IsOpen && mainViewModel.BrewingState == 0)
            {
                mySerialPort.WriteLine("PREPCLEAN");
            }
            else
            {
                MessageBox.Show("Can not start preparing cleaning sequense when not connected or system state is not 0");
            }
        }

        private void btnResetBoilFlowSensor_Click(object sender, RoutedEventArgs e)
        {
            {
                if (mySerialPort.IsOpen)
                {
                    mySerialPort.WriteLine("FLOW_REB");
                }
            }
        }

        public class MessengerInstanceWrappr : ViewModelBase
        {
            public MessengerInstanceWrappr(ProsessData data)
            {
                MessengerInstance.Send(new ProsessDataMessage(data));
            }

        }
    }
}
