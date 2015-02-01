using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Windows.Markup;

namespace Tech4WPF.ChartControl
{
    /// <summary>
    /// Interaction logic for ChartControl.xaml
    /// </summary>
    [ContentProperty("Title")]
    public partial class ChartControl : UserControl
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ChartControl"/> class.
        /// </summary>
        public ChartControl()
        {
            InitializeComponent();

            this.gridlineColorBinding.Source = this;
            this.gridlineColorBinding.Path = new PropertyPath("GridlineColor");

            this.legend = new Legend(this);
        }

        #endregion Constructors

        #region Constants

        private const double DEFAULT_CANVAS_WIDTH = 270;
        private const double DEFAULT_CANVAS_HEIGHT = 250;

        #endregion Constants

        #region Fields

        private double chartLeftOffset = 20;
        private double chartRightOffset = 10;
        private double chartTopOffset = 8;
        private double chartBottomOffset = 15;
        Binding gridlineColorBinding = new Binding();
        Legend legend = null;

        #endregion Fields

        #region Properties

        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
            "Title", typeof(string), typeof(ChartControl), new PropertyMetadata("Chart"));
        /// <summary>
        /// Gets or sets the title.
        /// Default value is <c>"Chart"</c>
        /// </summary>
        /// <value>
        /// The title.
        /// </value>
        [Description("Gets or sets the title. " +
                     "Default value is \"Chart\".")]
        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public static readonly DependencyProperty XMinProperty = DependencyProperty.Register(
            "XMin", typeof(double), typeof(ChartControl), new PropertyMetadata(0.0, onPropertyChanged));
        /// <summary>
        /// Gets or sets the X-axis minimum.
        /// Default value is <c>0.0</c>.
        /// </summary>
        /// <value>
        /// The X-axis minimum.
        /// </value>
        [Description("Gets or sets the X-axis minimum. " +
                     "Default value is 0.0.")]
        public double XMin
        {
            get { return (double)GetValue(XMinProperty); }
            set { SetValue(XMinProperty, value); }
        }

        public static readonly DependencyProperty YMinProperty = DependencyProperty.Register(
            "YMin", typeof(double), typeof(ChartControl), new PropertyMetadata(0.0, onPropertyChanged));
        /// <summary>
        /// Gets or sets the Y-axis minimum.
        /// Default value is <c>0.0</c>.
        /// </summary>
        /// <value>
        /// The Y-axis minimum.
        /// </value>
        [Description("Gets or sets the Y-axis minimum. " +
                     "Default value is 0.0.")]
        public double YMin
        {
            get { return (double)GetValue(YMinProperty); }
            set { SetValue(YMinProperty, value); }
        }

        public static readonly DependencyProperty XMaxProperty = DependencyProperty.Register(
            "XMax", typeof(double), typeof(ChartControl), new PropertyMetadata(10.0, onPropertyChanged));
        /// <summary>
        /// Gets or sets the X-axis maximum.
        /// Default value is <c>10.0</c>.
        /// </summary>
        /// <value>
        /// The X-axis maximum.
        /// </value>
        [Description("Gets or sets the X-axis maximum. " +
                     "Default value is 10.0.")]
        public double XMax
        {
            get { return (double)GetValue(XMaxProperty); }
            set { SetValue(XMaxProperty, value); }
        }

        public static readonly DependencyProperty YMaxProperty = DependencyProperty.Register(
            "YMax", typeof(double), typeof(ChartControl), new PropertyMetadata(10.0, onPropertyChanged));
        /// <summary>
        /// Gets or sets the Y-axis maximum.
        /// Default value is <c>10.0</c>.
        /// </summary>
        /// <value>
        /// The Y-axis maximum.
        /// </value>
        [Description("Gets or sets the Y-axis maximum. " +
                     "Default value is 10.0.")]
        public double YMax
        {
            get { return (double)GetValue(YMaxProperty); }
            set { SetValue(YMaxProperty, value); }
        }

        public static readonly DependencyProperty XTickProperty = DependencyProperty.Register(
            "XTick", typeof(double), typeof(ChartControl), new PropertyMetadata(2.0, onPropertyChanged));
        /// <summary>
        /// Gets or sets the X tick.
        /// Default value is <c>2.0</c>.
        /// </summary>
        /// <value>
        /// The X ticks
        /// </value>
        [Description("Gets or sets the X tick. " +
                     "Default value is 2.0.")]
        public double XTick
        {
            get { return (double)GetValue(XTickProperty); }
            set { SetValue(XTickProperty, value); }
        }

        public static readonly DependencyProperty YTickProperty = DependencyProperty.Register(
            "YTick", typeof(double), typeof(ChartControl), new PropertyMetadata(2.0, onPropertyChanged));
        /// <summary>
        /// Gets or sets the Y tick.
        /// Default value is <c>2.0</c>.
        /// </summary>
        /// <value>
        /// The Y tick.
        /// </value>
        [Description("Gets or sets the Y tick. " +
                     "Default value is 2.0.")]
        public double YTick
        {
            get { return (double)GetValue(YTickProperty); }
            set { SetValue(YTickProperty, value); }
        }

        public static readonly DependencyProperty XLabelProperty = DependencyProperty.Register(
            "XLabel", typeof(string), typeof(ChartControl), new PropertyMetadata("X axis"));
        /// <summary>
        /// Gets or sets the label for X axis.
        /// Default value is <c>"X axis"</c>.
        /// </summary>
        /// <value>
        /// The label for X axis.
        /// </value>
        [Description("Gets or sets the lable for X axis. " +
                     "Default value is \"X axis\".")]
        public string XLabel
        {
            get { return (string)GetValue(XLabelProperty); }
            set { SetValue(XLabelProperty, value); }
        }

        public static readonly DependencyProperty YLabelProperty = DependencyProperty.Register(
            "YLabel", typeof(string), typeof(ChartControl), new PropertyMetadata("Y axis"));
        /// <summary>
        /// Gets or sets the label for Y axis.
        /// Default value is <c>"Y axis"</c>.
        /// </summary>
        /// <value>
        /// The label for Y axis.
        /// </value>
        [Description("Gets or sets the label for Y axis. " +
                     "Default value is \"Y axis\".")]
        public string YLabel
        {
            get { return (string)GetValue(YLabelProperty); }
            set { SetValue(YLabelProperty, value); }
        }

        public static readonly DependencyProperty GridlinePatternProperty = DependencyProperty.Register(
            "GridlinePattern", typeof(StrokePattern), typeof(ChartControl),
            new PropertyMetadata(StrokePattern.Dot, onPropertyChanged));
        /// <summary>
        /// Gets or sets the gridline pattern.
        /// Default value is <c>StrokePattern.Dot</c>.
        /// </summary>
        /// <value>
        /// The gridline pattern.
        /// </value>
        [Description("Gets or sets the gridline pattern. " +
                     "Default value is StrokePattern.Dot.")]
        public StrokePattern GridlinePattern
        {
            get { return (StrokePattern)GetValue(GridlinePatternProperty); }
            set { SetValue(GridlinePatternProperty, value); }
        }

        public static readonly DependencyProperty GridlineColorProperty = DependencyProperty.Register(
            "GridlineColor", typeof(Brush), typeof(ChartControl),
            new PropertyMetadata(Brushes.Gray.Clone()));
        /// <summary>
        /// Gets or sets the color of the gridline.
        /// Default value is <c>Brushes.Gray</c>.
        /// </summary>
        /// <value>
        /// The color of the gridline.
        /// </value>
        [Description("Gets or sets the color of the gridline. " +
                     "Default value is Brushes.Gray.")]
        public Brush GridlineColor
        {
            get { return (Brush)GetValue(GridlineColorProperty); }
            set { SetValue(GridlineColorProperty, value); }
        }

        public static readonly DependencyProperty IsXGridProperty = DependencyProperty.Register(
            "IsXGrid", typeof(bool), typeof(ChartControl), new PropertyMetadata(true, onPropertyChanged));
        /// <summary>
        /// Gets or sets a value indicating whether is X grid.
        /// Default value is <c>true</c>.
        /// </summary>
        /// <value>
        ///   <c>true</c> if is X grid; otherwise, <c>false</c>.
        /// </value>
        [Description("Gets or sets a value indicating whether is X grid. " +
                     "Default value is true.")]
        public bool IsXGrid
        {
            get { return (bool)GetValue(IsXGridProperty); }
            set { SetValue(IsXGridProperty, value); }
        }

        public static readonly DependencyProperty IsYGridProperty = DependencyProperty.Register(
            "IsYGrid", typeof(bool), typeof(ChartControl), new PropertyMetadata(true, onPropertyChanged));
        /// <summary>
        /// Gets or sets a value indicating whether is Y grid.
        /// Default value is <c>true</c>.
        /// </summary>
        /// <value>
        ///   <c>true</c> if is Y grid; otherwise, <c>false</c>.
        /// </value>
        [Description("Gets or sets a value indicating whether is Y grid. " +
                     "Default value is true.")]
        public bool IsYGrid
        {
            get { return (bool)GetValue(IsYGridProperty); }
            set { SetValue(IsYGridProperty, value); }
        }

        public static readonly DependencyProperty DataCollectionProperty = DependencyProperty.Register(
            "DataCollection", typeof(List<DataSeries>), typeof(ChartControl),
            new PropertyMetadata(new List<DataSeries>(), onPropertyChanged));
        /// <summary>
        /// Gets or sets the data collection for this chart.
        /// </summary>
        /// <value>
        /// The data collection for this chart.
        /// </value>
        [Description("Gets or sets the data collection for this chart.")]
        public List<DataSeries> DataCollection
        {
            get { return (List<DataSeries>)GetValue(DataCollectionProperty); }
            set { SetValue(DataCollectionProperty, value); }
        }

        public static readonly DependencyProperty IsLegendProperty = DependencyProperty.Register(
            "IsLegend", typeof(bool), typeof(ChartControl), new PropertyMetadata(true, onPropertyChanged));
        /// <summary>
        /// Gets or sets a value indicating whether is legend displayed.
        /// Default value is <c>true</c>.
        /// </summary>
        /// <value>
        ///   <c>true</c> if is legend displayed; otherwise, <c>false</c>.
        /// </value>
        [Description("Gets or sets a value indicating whether is legend displayed. " +
                     "Default value is true.")]
        public bool IsLegend
        {
            get { return (bool)GetValue(IsLegendProperty); }
            set { SetValue(IsLegendProperty, value); }
        }

        public static readonly DependencyProperty IsLegendBorderProperty = DependencyProperty.Register(
            "IsLegendBorder", typeof(bool), typeof(ChartControl), new PropertyMetadata(true, onPropertyChanged));
        /// <summary>
        /// Gets or sets a value indicating whether is legend displayed.
        /// Default value is <c>true</c>.
        /// </summary>
        /// <value>
        ///   <c>true</c> if is legend displayed; otherwise, <c>false</c>.
        /// </value>
        [Description("Gets or sets a value indicating whether is legend displayed. " +
                     "Default value is true.")]
        public bool IsLegendBorder
        {
            get { return (bool)GetValue(IsLegendBorderProperty); }
            set { SetValue(IsLegendBorderProperty, value); }
        }

        public static readonly DependencyProperty LegendPositionProperty = DependencyProperty.Register(
            "LegendPosition", typeof(LegendPosition), typeof(ChartControl),
            new PropertyMetadata(LegendPosition.NorthWest, onPropertyChanged));
        /// <summary>
        /// Gets or sets the legend position for this chart.
        /// Default value is <c>LegendPosition.NorthWest</c>.
        /// </summary>
        /// <value>
        /// The legend position for this chart.
        /// </value>
        [Description("Gets or sets the legend position for this chart. " +
                     "Default value is LegendPosition.NorthWest.")]
        public LegendPosition LegendPosition
        {
            get { return (LegendPosition)GetValue(LegendPositionProperty); }
            set { SetValue(DataCollectionProperty, value); }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Property callback.
        /// Repaints the chart.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void onPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ChartControl chartControl = (ChartControl)sender;
            chartControl.repaint();
        }

        /// <summary>
        /// Grid SizeChanged handler.
        /// Repaints the chart.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.SizeChangedEventArgs"/> instance containing the event data.</param>
        private void onChartGridSizeChanged(object sender, SizeChangedEventArgs e)
        {
            repaint();
        }

        /// <summary>
        /// Repaints the chart.
        /// </summary>
        private void repaint()
        {
            this.rootCanvas.Width = this.chartGrid.ActualWidth;
            this.rootCanvas.Height = this.chartGrid.ActualHeight;
            this.legendCanvas.Children.Clear();
            this.chartCanvas.Children.RemoveRange(1, chartCanvas.Children.Count - 1);
            this.rootCanvas.Children.RemoveRange(1, rootCanvas.Children.Count - 1);
            setChartCanvasSizeAndPosition();
            addAxisValuesAndBorder();
            addGridlines();
            addChart();
            AddLegend();
        }

        /// <summary>
        /// Sets the chart canvas size and position.
        /// </summary>
        private void setChartCanvasSizeAndPosition()
        {
            determineRightOffset();
            determineLeftOffset();

            Canvas.SetLeft(this.chartCanvas, this.chartLeftOffset);
            Canvas.SetBottom(this.chartCanvas, this.chartBottomOffset);
            this.chartCanvas.Width = Math.Abs(this.rootCanvas.Width - this.chartLeftOffset - this.chartRightOffset);
            this.chartCanvas.Height = Math.Abs(this.rootCanvas.Height - this.chartBottomOffset - this.chartTopOffset);
        }

        /// <summary>
        /// Determines the right offset of chart canvas.
        /// </summary>
        private void determineRightOffset()
        {
            TextBlock textBlock = new TextBlock();
            textBlock.Text = this.XMax.ToString();
            textBlock.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
            Size size = textBlock.DesiredSize;

            this.chartRightOffset = (size.Width / 2) + 2;
        }

        /// <summary>
        /// Determines the left offset of chart canvas.
        /// </summary>
        private void determineLeftOffset()
        {
            double offset = 0;
            for (double dy = this.YMin; dy <= this.YMax; dy += this.YTick)
            {
                Point pt = NormalizePoint(new Point(XMin, dy));
                TextBlock textBlock = new TextBlock();
                textBlock.Text = dy.ToString();
                textBlock.TextAlignment = TextAlignment.Right;
                textBlock.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
                Size size = textBlock.DesiredSize;
                if (offset < size.Width)
                {
                    offset = size.Width;
                }
            }
            this.chartLeftOffset = offset + 5;
        }

        /// <summary>
        /// Adds the axis values and border.
        /// </summary>
        private void addAxisValuesAndBorder()
        {
            addChartBorder();
            addXAxisTickMarks();
            addYAxisTickMarks();
        }

        /// <summary>
        /// Adds the chart border.
        /// </summary>
        private void addChartBorder()
        {
            Rectangle chartRect = new Rectangle();
            chartRect.Stroke = Brushes.Black;
            chartRect.Width = this.chartCanvas.Width;
            chartRect.Height = this.chartCanvas.Height;
            Canvas.SetZIndex(chartRect, 10);
            this.chartCanvas.Children.Add(chartRect);
        }

        /// <summary>
        /// Adds the X axis tick marks.
        /// </summary>
        private void addXAxisTickMarks()
        {
            for (double dx = this.XMin; dx <= this.XMax; dx += this.XTick)
            {
                Point tickPoint = NormalizePoint(new Point(dx, YMin));
                Line tick = new Line();
                tick.Stroke = Brushes.Black;
                tick.X1 = tickPoint.X;
                tick.Y1 = tickPoint.Y;
                tick.X2 = tickPoint.X;
                tick.Y2 = tickPoint.Y - 5;
                this.chartCanvas.Children.Add(tick);

                TextBlock tblTickMark = new TextBlock();
                tblTickMark.Text = dx.ToString();
                tblTickMark.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
                Size size = tblTickMark.DesiredSize;

                this.rootCanvas.Children.Add(tblTickMark);

                double leftTickOffset = this.chartLeftOffset + tickPoint.X - (size.Width / 2);
                double topTickOffset = tickPoint.Y + 2 + (size.Height / 2);
                Canvas.SetLeft(tblTickMark, leftTickOffset);
                Canvas.SetTop(tblTickMark, topTickOffset);
            }
        }

        /// <summary>
        /// Adds the Y axis tick marks.
        /// </summary>
        private void addYAxisTickMarks()
        {
            for (double dy = this.YMin; dy <= this.YMax; dy += this.YTick)
            {
                Point tickPoint = NormalizePoint(new Point(XMin, dy));
                Line tick = new Line();
                tick.Stroke = Brushes.Black;
                tick.X1 = tickPoint.X;
                tick.Y1 = tickPoint.Y;
                tick.X2 = tickPoint.X + 5;
                tick.Y2 = tickPoint.Y;
                this.chartCanvas.Children.Add(tick);

                TextBlock tblTickMark = new TextBlock();
                tblTickMark.Text = dy.ToString();
                tblTickMark.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
                Size size = tblTickMark.DesiredSize;

                this.rootCanvas.Children.Add(tblTickMark);

                double rightTickOffset = this.chartCanvas.Width + this.chartRightOffset + 3;
                double topTickOffset = tickPoint.Y;
                Canvas.SetRight(tblTickMark, rightTickOffset);
                Canvas.SetTop(tblTickMark, topTickOffset);
            }
        }

        /// <summary>
        /// Adds the gridlines.
        /// </summary>
        private void addGridlines()
        {
            if (this.IsYGrid)
            {
                addVerticalGridlines();
            }

            if (this.IsXGrid)
            {
                addHrizontalGridlines();
            }
        }

        /// <summary>
        /// Adds the vertical gridlines.
        /// </summary>
        private void addVerticalGridlines()
        {
            for (double dx = this.XMin + this.XTick; dx < this.XMax; dx += this.XTick)
            {
                Point gridlineStart = NormalizePoint(new Point(dx, this.YMin));
                Point gridlineEnd = NormalizePoint(new Point(dx, this.YMax));

                Line gridline = new Line();
                gridline.X1 = gridlineStart.X;
                gridline.Y1 = gridlineStart.Y;
                gridline.X2 = gridlineEnd.X;
                gridline.Y2 = gridlineEnd.Y;
                gridline.SetBinding(Line.StrokeProperty, this.gridlineColorBinding);
                gridline.StrokeDashArray = GetShapePattern(this.GridlinePattern);

                this.chartCanvas.Children.Add(gridline);
            }
        }

        /// <summary>
        /// Adds the hrizontal gridlines.
        /// </summary>
        private void addHrizontalGridlines()
        {
            for (double dy = YMin + YTick; dy < YMax; dy += YTick)
            {
                Point gridlineStart = NormalizePoint(new Point(this.XMin, dy));
                Point gridlineEnd = NormalizePoint(new Point(this.XMax, dy));

                Line gridline = new Line();
                gridline.X1 = gridlineStart.X;
                gridline.Y1 = gridlineStart.Y;
                gridline.X2 = gridlineEnd.X;
                gridline.Y2 = gridlineEnd.Y;
                gridline.SetBinding(Line.StrokeProperty, this.gridlineColorBinding);
                gridline.StrokeDashArray = GetShapePattern(this.GridlinePattern);

                this.chartCanvas.Children.Add(gridline);
            }
        }

        /// <summary>
        /// Adds the legend.
        /// </summary>
        private void AddLegend()
        {
            this.legend.SetLegend();
        }

        /// <summary>
        /// Gets stroke dash array for shape according given stroke pattern type.
        /// </summary>
        /// <param name="strokePattern">The stroke pattern.</param>
        /// <returns></returns>
        internal static DoubleCollection GetShapePattern(StrokePattern strokePattern)
        {
            switch (strokePattern)
            {
                case StrokePattern.Dash:
                    return new DoubleCollection(new double[2] { 4, 3 });
                    break;
                case StrokePattern.DashDot:
                    return new DoubleCollection(new double[4] { 4, 2, 1, 2 });
                    break;
                case StrokePattern.Dot:
                    return new DoubleCollection(new double[2] { 1, 2 });
                    break;
                case StrokePattern.None:
                    return new DoubleCollection(new double[1] { 0 });
                    break;
                case StrokePattern.Solid:
                default:
                    return new DoubleCollection(new double[0] { });
                    break;
            }
        }

        /// <summary>
        /// Adds the all DataSeries from DataCollection to chart.
        /// </summary>
        private void addChart()
        {
            foreach (DataSeries dataSeries in this.DataCollection)
            {
                dataSeries.DrawChart(this);
            }
        }    

        /// <summary>
        /// Convert point from values to point in coordinate system
        /// </summary>
        /// <param name="point">Point to plot.</param>
        /// <returns>Point in coordinate system of the chart</returns>
        public Point NormalizePoint(Point point)
        {
            if (double.IsNaN(this.chartCanvas.Width))
                this.chartCanvas.Width = DEFAULT_CANVAS_WIDTH;
            if (double.IsNaN(this.chartCanvas.Height))
                this.chartCanvas.Height = DEFAULT_CANVAS_HEIGHT;

            point.X = replaceInfinity(point.X);
            point.Y = replaceInfinity(point.Y);

            Point result = new Point();
            result.X = (point.X - this.XMin) * this.chartCanvas.Width / (this.XMax - this.XMin);
            result.Y = this.chartCanvas.Height - (point.Y - this.YMin) * this.chartCanvas.Height / (this.YMax - this.YMin);
            return result;
        }

        /// <summary>
        /// Replaces the <c>PositiveInfinity</c> with <c>MaxValue</c>;
        /// <c>NegateiveInfinity</c> with <c>MinValue</c>
        /// </summary>
        /// <param name="d">The number for infinity replacement.</param>
        /// <returns>Corrected number with no infinity value</returns>
        private double replaceInfinity(double d)
        {
            if (d == double.PositiveInfinity)
                return double.MaxValue;
            if (d == double.NegativeInfinity)
                return double.MinValue;
            return d;
        }

        #endregion Methods
    }
}
