using System;
using System.ComponentModel;
using System.Linq;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System.Collections.Generic;
using System.Speech.Synthesis;
using System.Windows.Input;
using Yuhan.WPF.Commands;
using System.IO;
using Microsoft.Win32;
using BryggeprogramWPF.ViewModel;


namespace BryggeprogramWPF
{

    public class MainViewModel : INotifyPropertyChanged
    {
        private PlotModel plotModel;
        public PlotModel PlotModel
        {
            get { return plotModel; }
            set { plotModel = value; OnPropertyChanged("PlotModel"); }
        }

        private string resivedStringFromArduino;
        public string ResivedStringFromArduino
        {
            get { return resivedStringFromArduino; }
            set { resivedStringFromArduino = value; OnPropertyChanged("StringFromArduino"); }
        }

        private DateTime currentDateTime;
        public DateTime CurrentDateTime
        {
            get { return currentDateTime; }
            set { currentDateTime = value; OnPropertyChanged("CurrentDateTime"); TimeDisplayUpdate(); }
        }

        private double hotLiquidTankTemperature;
        public double HotLiquidTankTemperature { get { return hotLiquidTankTemperature; } set { hotLiquidTankTemperature = value; OnPropertyChanged("HotLiquidTankTemperature"); ValueOfIntrestUpdate(); } }

        public string hotLiquidTankElement = "Black";
        public bool HotLiquidTankElement
        {
            set
            {
                if (value)
                {
                    hotLiquidTankElement = "Red"; OnPropertyChanged("HotLiquidTankElement");
                }
                else
                {
                    hotLiquidTankElement = "Black"; OnPropertyChanged("HotLiquidTankElement");
                }

            }

        }

        private double meshTankTemperature;
        public double MeshTankTemperature { get { return meshTankTemperature; } set { meshTankTemperature = value; OnPropertyChanged("MeshTankTemperature"); ValueOfIntrestUpdate(); } }

        private double meshTankRimsOutesideTemperature;
        public double MeshTankRimsOutesideTemperature { get { return meshTankRimsOutesideTemperature; } set { meshTankRimsOutesideTemperature = value; OnPropertyChanged("MeshTankRimsOutesideTemperature"); } }

        private double meshTankRimsReturTemperature;
        public double MeshTankRimsReturTemperature { get { return meshTankRimsReturTemperature; } set { meshTankRimsReturTemperature = value; OnPropertyChanged("MeshTankRimsReturTemperature"); } }

        private double meshTankVolume;
        public double MeshTankVolume { get { return meshTankVolume; } set { meshTankVolume = value; OnPropertyChanged("MeshTankVolume"); ValueOfIntrestUpdate(); } }

        private double meshTankAddedVolume;
        public double MeshTankAddedVolume { get { return meshTankAddedVolume; } set { meshTankAddedVolume = value; OnPropertyChanged("MeshTankAddedVolume"); } }

        private double boilTankTemperature;
        public double BoilTankTemperature { get { return boilTankTemperature; } set { boilTankTemperature = value; OnPropertyChanged("BoilTankTemperature"); ValueOfIntrestUpdate(); } }

        private double hotLiquidTankTemperatureSetpoint;
        public double HotLiquidTankTemperatureSetpoint { get { return hotLiquidTankTemperatureSetpoint; } set { hotLiquidTankTemperatureSetpoint = value; OnPropertyChanged("HotLiquidTankTemperatureSetpoint"); ValueOfIntrestUpdate(); } }

        private double meshTankTemperatureSetpoint;
        public double MeshTankTemperatureSetpoint { get { return meshTankTemperatureSetpoint; } set { meshTankTemperatureSetpoint = value; OnPropertyChanged("MeshTankTemperatureSetpoint"); ValueOfIntrestUpdate(); } }

        private double boilTankTemperatureSetpoint;
        public double BoilTankTemperatureSetpoint { get { return boilTankTemperatureSetpoint; } set { boilTankTemperatureSetpoint = value; OnPropertyChanged("BoilTankTemperatureSetpoint"); ValueOfIntrestUpdate(); } }

        private double boilTankVolume;
        public double BoilTankVolume { get { return boilTankVolume; } set { boilTankVolume = value; OnPropertyChanged("BoilTankVolume"); ValueOfIntrestUpdate(); } }

        private string timer;
        public string Timer
        {
            get { return timer; }
            set { timer = value; OnPropertyChanged("Timer"); TimeDisplayUpdate(); }
        }

        private double watchTemperature;
        public double WatchTemperature
        {
            get { return watchTemperature; }
            set { watchTemperature = value; OnPropertyChanged("WatchTemperature"); }
        }

        private int brewingState;
        public int BrewingState
        {
            get { return brewingState; }
            set { brewingState = value; OnPropertyChanged("BrewingState"); }
        }
        private int cleaningState;
        public int CleaningState
        {
            get { return cleaningState; }
            set { cleaningState = value; OnPropertyChanged("CleaningState"); }
        }

        private string timeDisplay;
        public string TimeDisplay { get { return timeDisplay; } set { timeDisplay = value; OnPropertyChanged("TimeDisplay"); } }
        private void TimeDisplayUpdate()
        {
            if (MessageFromSystem == null)
            {
                MessageFromSystem = "";
            }

            if (MessageFromSystem.Length > 0)
            {
                TimeDisplay = MessageFromSystem;
            }

            else
            {
                if (brewingState <= 20)
                {
                    TimeDisplay = CurrentDateTime.ToString("HH.mm.ss");
                }
                else
                {
                    TimeDisplay = Timer;
                }
            }

        }

        private double temperatureOfIntrest;
        public double ValueOfIntrest { get { return temperatureOfIntrest; } set { temperatureOfIntrest = value; OnPropertyChanged("ValueOfIntrest"); } }

        private string temperatureOfIntrestBacground;
        public string TemperatureOfIntrestBacground { get { return temperatureOfIntrestBacground; } set { temperatureOfIntrestBacground = value; OnPropertyChanged("TemperatureOfIntrestBacground"); } }

        private string messageFromSystem;
        public string MessageFromSystem
        {
            get
            {
                return messageFromSystem;
            }
            set
            {
                if (messageFromSystem != value)
                {
                    messageFromSystem = value; OnPropertyChanged("MessageFromSystem"); TimeDisplayUpdate();
                    if (value.Length > 0)
                    {
                        SpeechSynthesizer speak = new SpeechSynthesizer();
                        speak.Rate = -5;
                        speak.Volume = 100;
                        speak.SpeakAsync(value);
                    }
                }

            }
        }

        private void ValueOfIntrestUpdate()
        {
            double selectedTemperature = 0;
            double selectedTemperatureSetpoint = 0;
            if (BrewingState <= 10)
            {
                ValueOfIntrest = HotLiquidTankTemperature;
                selectedTemperature = ValueOfIntrest;
                selectedTemperatureSetpoint = HotLiquidTankTemperatureSetpoint;
            }
            else if (BrewingState >= 20 && BrewingState < 40)
            {
                ValueOfIntrest = MeshTankTemperature;
                selectedTemperature = ValueOfIntrest;
                selectedTemperatureSetpoint = MeshTankTemperatureSetpoint;
            }
            else if (brewingState >= 40 && brewingState < 50)
            {
                ValueOfIntrest = BoilTankVolume;
                selectedTemperature = 0;
                selectedTemperatureSetpoint = 0;
            }
            else if (BrewingState >= 50)
            {
                ValueOfIntrest = BoilTankTemperature;
                selectedTemperature = ValueOfIntrest;
                selectedTemperatureSetpoint = BoilTankTemperatureSetpoint;
            }
            if (selectedTemperature == 0 && selectedTemperatureSetpoint == 0)
            {
                TemperatureOfIntrestBacground = "White";
            }
            else if (selectedTemperature <= (selectedTemperatureSetpoint + 0.25) && selectedTemperature >= (selectedTemperatureSetpoint - 0.25))
            {
                TemperatureOfIntrestBacground = "Green";
            }
            else if (selectedTemperature <= (selectedTemperatureSetpoint + 0.5) && selectedTemperature >= (selectedTemperatureSetpoint - 0.5))
            {
                TemperatureOfIntrestBacground = "Yellow";
            }
            else
            {
                TemperatureOfIntrestBacground = "Red";
            }
            if (BrewingState < 10)
            {
                TemperatureOfIntrestBacground = "Transparent";
            }

        }

        public ICommand OpenProsessViewCommand
        {
            get;
            private set;
        }

        private bool CanExecuteOpenProsessViewCommand()
        {
            return true;
        }
        private void CreatePoenProsessViewCommand()
        {
            OpenProsessViewCommand = new RelayCommand(OpenProsessViewCommandExecute, CanExecuteOpenProsessViewCommand);
        }

        private void OpenProsessViewCommandExecute()
        {
            ProsessWindow prosessWindow = new ProsessWindow();
            ProsessViewModel prosessViewModel = new ProsessViewModel();
            prosessWindow.DataContext = prosessViewModel;
            prosessWindow.Show();
        }

        public MainViewModel()
        {
            PlotModel = new PlotModel();
            SetUpModel();
            LoadData();
            CreatePoenProsessViewCommand();

        }

        private readonly List<OxyColor> colors = new List<OxyColor>
                                            {   OxyColors.Blue,
                                                OxyColors.Green,
                                                OxyColors.Red,
                                                OxyColors.Yellow,
                                                OxyColors.Black
                                            };

        private readonly List<String> names = new List<string>
        {
            "HLT","MashTank","RIMS Outeside","Boil Tank","Mesh Volume"
        };

        private void SetUpModel()
        {
            plotModel.Background = OxyColors.LightGray;
            PlotModel.LegendTitle = "Temperatures";
            PlotModel.LegendOrientation = LegendOrientation.Horizontal;
            PlotModel.LegendPlacement = LegendPlacement.Outside;
            PlotModel.LegendPosition = LegendPosition.RightTop;
            PlotModel.LegendBackground = OxyColor.FromAColor(200, OxyColors.LightGray);
            PlotModel.LegendBorder = OxyColors.Black;

            var dateAxis = new DateTimeAxis(AxisPosition.Bottom, "Date", "HH:mm") { MajorGridlineStyle = LineStyle.Solid, MinorGridlineStyle = LineStyle.Dot, IntervalLength = 80 };
            PlotModel.Axes.Add(dateAxis);
            var valueAxis = new LinearAxis(AxisPosition.Left, 0) { MajorGridlineStyle = LineStyle.Solid, MinorGridlineStyle = LineStyle.Dot, Title = "C/dm3" };
            PlotModel.Axes.Add(valueAxis);

        }

        private void LoadData()
        {
            List<Measurement> measurements = Data.GetData();

            var dataPerDetector = measurements.GroupBy(m => m.DetectorId).OrderBy(m => m.Key).ToList();

            foreach (var data in dataPerDetector)
            {
                var lineSerie = new LineSeries
                {
                    StrokeThickness = 2,
                    MarkerSize = 3,
                    Color=colors[data.Key],
                    MarkerStroke = colors[data.Key],
                    //MarkerType = markerTypes[data.Key],
                    CanTrackerInterpolatePoints = false,
                    Title = names[data.Key],
                    Smooth = true,
                };

                // data.ToList().ForEach(d=>lineSerie.Points.Add(new DataPoint(DateTimeAxis.ToDouble(d.DateTime),d.Value)));
                PlotModel.Series.Add(lineSerie);
            }

        }

        public void UpdateModel(List<double> values)
        {

            List<Measurement> measurements = Data.UpdateData(values);
            var dataPerDetector = measurements.GroupBy(m => m.DetectorId).OrderBy(m => m.Key).ToList();

            foreach (var data in dataPerDetector)
            {
                var lineSerie = PlotModel.Series[data.Key] as LineSeries;
                if (lineSerie != null)
                {

                    data.ToList()
                        .ForEach(d => lineSerie.Points.Add(new DataPoint(DateTimeAxis.ToDouble(d.DateTime), d.Value)));
                }
            }

        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        public ICommand BrewDone_Click
        {
            get { return new DelegateCommand<object>(FuncToCall, FuncToEvaluate); }
        }

        private void FuncToCall(object context)
        {
            //this is called when the button is clicked
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            if (saveFileDialog.ShowDialog() == true)
            {
                using (var stream = File.Create(saveFileDialog.FileName))
                {
                    var pngExporter = new OxyPlot.Wpf.PngExporter();
                    pngExporter.Height = 500;
                    pngExporter.Width = 1500;
                    pngExporter.Background = OxyColors.White;
                    pngExporter.Resolution = 96;
                    pngExporter.Export(plotModel, stream);


                }
            }

        }

        private bool FuncToEvaluate(object context)
        {
            //this is called to evaluate whether FuncToCall can be called
            //for example you can return true or false based on some validation logic
            return true;
        }


    }


}