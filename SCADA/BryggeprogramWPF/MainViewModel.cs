using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System.Collections.Generic;
using System.Speech.Synthesis;


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
                get {return resivedStringFromArduino; }
                set {resivedStringFromArduino = value; OnPropertyChanged("StringFromArduino"); }
            }

            private DateTime currentDateTime;
            public DateTime CurrentDateTime
            {
                get { return currentDateTime; }
                set { currentDateTime = value; OnPropertyChanged("CurrentDateTime"); TimeDisplayUpdate(); }
            }
            private double hotLiquidTankTemperature;
            public double HotLiquidTankTemperature { get { return hotLiquidTankTemperature; } set { hotLiquidTankTemperature = value; OnPropertyChanged("HotLiquidTankTemperature"); TemperatureOfIntrestUpdate(); } }
            
            private double meshTankTemperature;
            public double MeshTankTemperature { get { return meshTankTemperature; } set { meshTankTemperature = value; OnPropertyChanged("MeshTankTemperature"); TemperatureOfIntrestUpdate(); } }

            private double boilTankTemperature;
            public double BoilTankTemperature { get { return boilTankTemperature; } set { boilTankTemperature = value; OnPropertyChanged("BoilTankTemperature"); TemperatureOfIntrestUpdate(); } }

            private double hotLiquidTankTemperatureSetpoint;
            public double HotLiquidTankTemperatureSetpoint { get { return hotLiquidTankTemperatureSetpoint; } set { hotLiquidTankTemperatureSetpoint = value; OnPropertyChanged("HotLiquidTankTemperatureSetpoint"); TemperatureOfIntrestUpdate(); } }

            private double meshTankTemperatureSetpoint;
            public double MeshTankTemperatureSetpoint { get { return meshTankTemperatureSetpoint; } set { meshTankTemperatureSetpoint = value; OnPropertyChanged("MeshTankTemperatureSetpoint"); TemperatureOfIntrestUpdate(); } }

            private double boilTankTemperatureSetpoint;
            public double BoilTankTemperatureSetpoint { get { return boilTankTemperatureSetpoint; } set { boilTankTemperatureSetpoint = value; OnPropertyChanged("BoilTankTemperatureSetpoint"); TemperatureOfIntrestUpdate(); } }


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

            private string timeDisplay;
            public string TimeDisplay { get { return timeDisplay; } set { timeDisplay = value; OnPropertyChanged("TimeDisplay"); } }
            private void TimeDisplayUpdate()
            {
                if (brewingState<=20)
                {
                    TimeDisplay = CurrentDateTime.ToString("HH.mm.ss");
                }
                else
                {
                    TimeDisplay = Timer;
                }
            }

            private double temperatureOfIntrest;
            public double TemperatureOfIntrest { get { return temperatureOfIntrest; } set { temperatureOfIntrest = value; OnPropertyChanged("TemperatureOfIntrest"); } }

            private string temperatureOfIntrestBacground;
            public string TemperatureOfIntrestBacground { get { return temperatureOfIntrestBacground; } set { temperatureOfIntrestBacground = value; OnPropertyChanged("TemperatureOfIntrestBacground"); } }

            private string messageFromSystem;
            public string MessageFromSystem
            { 
                get {
                    return messageFromSystem; }
                set {
                    if (messageFromSystem!=value)
                    {
                        messageFromSystem = value; OnPropertyChanged("MessageFromSystem");
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

            private void TemperatureOfIntrestUpdate()
            {
                double selectedTemperature=0;
                double selectedTemperatureSetpoint=0;
                if (BrewingState<=10)
                {
                    TemperatureOfIntrest = HotLiquidTankTemperature;
                    selectedTemperature = TemperatureOfIntrest;
                    selectedTemperatureSetpoint = HotLiquidTankTemperatureSetpoint;
                }
                else if (BrewingState>=20 && BrewingState<50)
                {
                    TemperatureOfIntrest = MeshTankTemperature;
                    selectedTemperature = TemperatureOfIntrest;
                    selectedTemperatureSetpoint = MeshTankTemperatureSetpoint;
                }
                else if (BrewingState>=50)
                {
                    TemperatureOfIntrest = BoilTankTemperature;
                    selectedTemperature = TemperatureOfIntrest;
                    selectedTemperatureSetpoint = BoilTankTemperatureSetpoint;
                }

                if (selectedTemperature <= (selectedTemperatureSetpoint + 0.25) && selectedTemperature >= (selectedTemperatureSetpoint - 0.25))
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

            }

            public MainViewModel()
            {
                PlotModel = new PlotModel();               
                SetUpModel();
                LoadData();
                
            }

            private readonly List<OxyColor> colors = new List<OxyColor>
                                            {
                                                OxyColors.Green,
                                                OxyColors.IndianRed,
                                                OxyColors.Coral,
                                                OxyColors.Chartreuse,
                                                OxyColors.Azure
                                            };
            private readonly List<String> names = new List<string>
        {
            "HLT","MashTank","Boil Tank","Ambient","Mesh Volume"
        };

            private void SetUpModel()
            {
                PlotModel.LegendTitle = "Legend";
                PlotModel.LegendOrientation = LegendOrientation.Horizontal;
                PlotModel.LegendPlacement = LegendPlacement.Outside;
                PlotModel.LegendPosition = LegendPosition.RightTop;
                PlotModel.LegendBackground = OxyColor.FromAColor(200, OxyColors.White);
                PlotModel.LegendBorder = OxyColors.Black;

                var dateAxis = new DateTimeAxis(AxisPosition.Bottom, "Date", "HH:mm") { MajorGridlineStyle = LineStyle.Solid, MinorGridlineStyle = LineStyle.Dot, IntervalLength = 80 };
                PlotModel.Axes.Add(dateAxis);
                var valueAxis = new LinearAxis(AxisPosition.Left, 0) { MajorGridlineStyle = LineStyle.Solid, MinorGridlineStyle = LineStyle.Dot, Title = "Value" };
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
                        MarkerStroke = colors[data.Key],
                        //MarkerType = markerTypes[data.Key],
                        CanTrackerInterpolatePoints = false,
                        Title = names[data.Key],
                        Smooth = false,
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
        

        
        }


}
