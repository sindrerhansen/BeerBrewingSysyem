using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System.Collections.Generic;

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
            private string timer;
            public string Timer
            {
                get { return timer; }
                set { timer = value; OnPropertyChanged("Timer"); }
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
            "HLT","MashTank","MashTank Heat Return","Boil Tank","Ambiant"
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
