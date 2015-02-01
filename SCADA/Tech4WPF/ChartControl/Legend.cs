using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Shapes;
using System.Windows.Media;

namespace Tech4WPF.ChartControl
{
    internal class Legend
    {
        #region Constructors

        public Legend(ChartControl chartControl) 
        {
            if (chartControl == null)
                throw new ArgumentNullException("legendCanvas can't be null");
            this.chartControl = chartControl;
        }

        #endregion Constructors

        #region Fields

        ChartControl chartControl = null;

        #endregion Fields

        #region Methods

        /// <summary>
        /// Sets the legend.
        /// </summary>
        public void SetLegend()
        {
            int seriesCount = this.chartControl.DataCollection.Count;
            if (!chartControl.IsLegend || seriesCount == 0)
                return;

            string[] seriesNames = getSeriesNames();

            double legendWidth = 0;
            double legendHeight = 0;
            TextBlock textBlock = new TextBlock();
            Size size = new Size();
            for (int i = 0; i < seriesNames.Length; i++)
            {
                textBlock.Text = seriesNames[i];
                textBlock.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
                size = textBlock.DesiredSize;
                if (legendWidth < size.Width)
                    legendWidth = size.Width;
            }
            legendWidth += 50;
            legendHeight = 17 * seriesCount;

            double sx = 6;
            double sy = 0;
            double textHeight = size.Height;
            double lineLength = 34;
            Rectangle legendRect = new Rectangle();
            legendRect.Stroke = Brushes.Black;
            legendRect.Fill = Brushes.White;
            legendRect.Width = legendWidth;
            legendRect.Height = legendHeight;

            if (this.chartControl.IsLegendBorder)
                this.chartControl.legendCanvas.Children.Add(legendRect);
            Canvas.SetZIndex(this.chartControl.legendCanvas, 10);

            int n = 1;
            foreach (DataSeries dataSeries in this.chartControl.DataCollection)
            {
                double xSymbol = sx + lineLength / 2;
                double xText = 2 * sx + lineLength;
                double yText = n * sy + (2 * n - 1) * textHeight / 2;
                Line line = new Line();
                line.Stroke = dataSeries.ChartStyle.Stroke;
                line.StrokeThickness = dataSeries.ChartStyle.StrokeThickness;
                line.StrokeDashArray = ChartControl.GetShapePattern(dataSeries.ChartStyle.StrokePattern);
                line.X1 = sx;
                line.Y1 = yText;
                line.X2 = sx + lineLength;
                line.Y2 = yText;
                if (dataSeries.ChartStyle.DrawType != DrawType.None)
                {
                    this.chartControl.legendCanvas.Children.Add(line);
                }
                Point point = new Point(0.5 * (line.X2 - line.X1 + dataSeries.ChartStyle.SymbolSize) + 1, line.Y1);

                SymbolType symbol = dataSeries.ChartStyle.SymbolType;
                if (symbol == SymbolType.Bar)
                {
                    symbol = SymbolType.Box;
                }

                dataSeries.AddSymbol(symbol, this.chartControl.legendCanvas, point);

                textBlock = new TextBlock();
                textBlock.Text = seriesNames[n-1];
                this.chartControl.legendCanvas.Children.Add(textBlock);
                Canvas.SetTop(textBlock, yText - size.Height / 2);
                Canvas.SetLeft(textBlock, xText);
                n++;
            }
            this.chartControl.legendCanvas.Width = legendRect.Width;
            this.chartControl.legendCanvas.Height = legendRect.Height;

            setLegendPosition(this.chartControl.chartCanvas, legendRect);
        }


        /// <summary>
        /// Sets the legend position.
        /// </summary>
        /// <param name="chartCanvas">The chart canvas.</param>
        /// <param name="legendRect">The legend rect.</param>
        private void setLegendPosition(Canvas chartCanvas, Rectangle legendRect)
        {
            double offSet = 7.0;
            switch (this.chartControl.LegendPosition)
            {
                case LegendPosition.East:
                    Canvas.SetRight(this.chartControl.legendCanvas, offSet);
                    Canvas.SetTop(this.chartControl.legendCanvas, chartCanvas.Height / 2 - legendRect.Height / 2);
                    break;
                case LegendPosition.NorthEast:
                    Canvas.SetTop(this.chartControl.legendCanvas, offSet);
                    Canvas.SetRight(this.chartControl.legendCanvas, offSet);
                    break;
                case LegendPosition.North:
                    Canvas.SetTop(this.chartControl.legendCanvas, offSet);
                    Canvas.SetLeft(this.chartControl.legendCanvas, chartCanvas.Width / 2 - legendRect.Width / 2);
                    break;
                case LegendPosition.NorthWest:
                    Canvas.SetTop(this.chartControl.legendCanvas, offSet);
                    Canvas.SetLeft(this.chartControl.legendCanvas, offSet);
                    break;
                case LegendPosition.West:
                    Canvas.SetTop(this.chartControl.legendCanvas, chartCanvas.Height / 2 - legendRect.Height / 2);
                    Canvas.SetLeft(this.chartControl.legendCanvas, offSet);
                    break;
                case LegendPosition.SouthWest:
                    Canvas.SetBottom(this.chartControl.legendCanvas, offSet);
                    Canvas.SetLeft(this.chartControl.legendCanvas, offSet);
                    break;
                case LegendPosition.South:
                    Canvas.SetBottom(this.chartControl.legendCanvas, offSet);
                    Canvas.SetLeft(this.chartControl.legendCanvas, chartCanvas.Width / 2 - legendRect.Width / 2);
                    break;
                case LegendPosition.SouthEast:
                    Canvas.SetBottom(this.chartControl.legendCanvas, offSet);
                    Canvas.SetRight(this.chartControl.legendCanvas, offSet);
                    break;
            }
        }

        /// <summary>
        /// Gets the series names.
        /// </summary>
        /// <returns></returns>
        private string[] getSeriesNames()
        {
            int seriesCount = this.chartControl.DataCollection.Count;
            int n = 0;
            string[] seriesNames = new string[seriesCount];
            foreach (DataSeries dataSeries in this.chartControl.DataCollection)
            {
                string seriesName = dataSeries.Name;
                if (seriesName == DataSeries.DEFAULT_NAME)
                    seriesName += n.ToString();
                seriesNames[n] = seriesName;
                n++;
            }
            return seriesNames;
        }

        #endregion Methods
    }
}
