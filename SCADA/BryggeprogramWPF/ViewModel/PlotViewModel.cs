using OxyPlot;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BryggeprogramWPF.ViewModel
{
    public class PlotViewModel
    {
    }
    //private void LoadData(PlotModel plotModel)
    //{
    //    List<Measurement> measurements = Data.GetData();

    //    var dataPerDetector = measurements.GroupBy(m => m.DetectorId).OrderBy(m => m.Key).ToList();

    //    foreach (var data in dataPerDetector)
    //    {
    //        var lineSerie = new LineSeries
    //        {
    //            StrokeThickness = 2,
    //            MarkerSize = 3,
    //            Color = colors[data.Key],
    //            MarkerStroke = colors[data.Key],
    //            //MarkerType = markerTypes[data.Key],
    //            CanTrackerInterpolatePoints = false,
    //            Title = names[data.Key],
    //            Smooth = true,
    //        };

    //        // data.ToList().ForEach(d=>lineSerie.Points.Add(new DataPoint(DateTimeAxis.ToDouble(d.DateTime),d.Value)));
    //        plotModel.Series.Add(lineSerie);

    //    }

    //}
}
