using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Media;


namespace Tech4WPF.ChartControl
{
    /// <summary>
    /// Represents series of points to plot
    /// with chosen symbols and lines
    /// </summary>
    public class DataSeries
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DataSeries"/> class.
        /// </summary>
        public DataSeries() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataSeries"/> class
        /// with given name
        /// </summary>
        /// <param name="name">The name of this data series</param>
        public DataSeries(string name)
        {
            this.Name = name;
        }

        #endregion Constructors

        #region Constants

        internal const string DEFAULT_NAME = "Data series";

        #endregion Constants

        #region Fields

        /// <summary>
        /// Normalized points
        /// </summary>
        private List<Point> normalizedPointSeries = new List<Point>();

        /// <summary>
        /// Normalized orgin point, default value is <c>Point(0, 0)</c>.
        /// </summary>
        private Point normalizedOriginPoint = new Point(0, 0);

        #endregion Fields

        #region Properties

        private List<Point> pointSeries = new List<Point>();
        /// <summary>
        /// Gets or sets the list of points.
        /// </summary>
        /// <value>
        /// The list of points.
        /// </value>
        public List<Point> PointSeries
        {
            get { return this.pointSeries; }
            set { this.pointSeries = value; }
        }

        private string name = DEFAULT_NAME;
        /// <summary>
        /// Gets or sets the name of this data series.
        /// Default value is "Data series"
        /// </summary>
        /// <value>
        /// The name of this data series.
        /// </value>
        public string Name
        {
            get { return this.name; }
            set { this.name = value; }
        }

        private ChartStyle chartStyle = new ChartStyle();
        /// <summary>
        /// Gets or sets the chart style for this data series.
        /// </summary>
        /// <value>
        /// The chart style for this data series.
        /// </value>
        public ChartStyle ChartStyle
        {
            get { return this.chartStyle; }
            set { this.chartStyle = value; }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Returns a <see cref="System.String"/> representating name of this data series.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> representating name of this data series.
        /// </returns>
        public override string ToString()
        {
            return this.Name;
        }

        /// <summary>
        /// Draws the chart.
        /// </summary>
        /// <param name="chartControl">The chart control.</param>
        internal void DrawChart(ChartControl chartControl)
        {
            normalizePoints(chartControl);
            addSymbols(chartControl.chartCanvas, this.normalizedPointSeries);
            drawLines(chartControl.chartCanvas, this.normalizedPointSeries);
        }

        /// <summary>
        /// Normalizes the points for given chart.
        /// </summary>
        /// <param name="chartControl">The chart control.</param>
        private void normalizePoints(ChartControl chartControl)
        {
            this.normalizedPointSeries.Clear();
            foreach (Point point in this.PointSeries)
            {
                Point normalizedPoint = chartControl.NormalizePoint(point);
                this.normalizedPointSeries.Add(normalizedPoint);
            }
            this.normalizedOriginPoint = chartControl.NormalizePoint(new Point(0, 0));
        }

        /// <summary>
        /// Adds the symbols in the list to the canvas.
        /// </summary>
        /// <param name="canvas">The canvas.</param>
        /// <param name="pointList">The list of points.</param>
        private void addSymbols(Canvas canvas, List<Point> pointList)
        {
            foreach (Point point in this.normalizedPointSeries)
            {
                AddSymbol(this.ChartStyle.SymbolType, canvas, point);
            }
        }

        /// <summary>
        /// Adds the symbol to convas to given point.
        /// </summary>
        /// <param name="symbol">The symbol.</param>
        /// <param name="canvas">The canvas.</param>
        /// <param name="point">The point.</param>
        internal void AddSymbol(SymbolType symbol, Canvas canvas, Point point)
        {
            switch (symbol)
            {
                case SymbolType.Square:
                    addSymbolSquare(canvas, point);
                    break;
                case SymbolType.OpenDiamond:
                    addSymbolOpenDiamond(canvas, point);
                    break;
                case SymbolType.Circle:
                    addSymbolCircle(canvas, point);
                    break;
                case SymbolType.OpenTriangle:
                    addSymbolOpenTriangle(canvas, point);
                    break;
                case SymbolType.None:
                    break;
                case SymbolType.Cross:
                    addSymbolCross(canvas, point);
                    break;
                case SymbolType.Star:
                    addSymbolStar(canvas, point);
                    break;
                case SymbolType.OpenInvertedTriangle:
                    addSymbolOpenInvertedTriangle(canvas, point);
                    break;
                case SymbolType.Plus:
                    addSymbolPlus(canvas, point);
                    break;
                case SymbolType.Dot:
                    addSymbolDot(canvas, point);
                    break;
                case SymbolType.Box:
                    addSymbolBox(canvas, point);
                    break;
                case SymbolType.Diamond:
                    addSymbolDiamond(canvas, point);
                    break;
                case SymbolType.InvertedTriangle:
                    addSymbolInvertedTriangle(canvas, point);
                    break;
                case SymbolType.Triangle:
                    addSymbolTriangle(canvas, point);
                    break;
                case SymbolType.Bar:
                    addSymbolBar(canvas, point);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Adds the symbol square.
        /// </summary>
        /// <param name="canvas">The canvas.</param>
        /// <param name="point">The point.</param>
        private void addSymbolSquare(Canvas canvas, Point point)
        {
            Polygon polygon = new Polygon();
            setSymbolStrokeFillAndIndex(polygon);
            polygon.Fill = this.ChartStyle.Stroke;
            polygon.Points.Add(new Point(point.X - this.ChartStyle.HalfSymbolSize, point.Y - this.ChartStyle.HalfSymbolSize));
            polygon.Points.Add(new Point(point.X + this.ChartStyle.HalfSymbolSize, point.Y - this.ChartStyle.HalfSymbolSize));
            polygon.Points.Add(new Point(point.X + this.ChartStyle.HalfSymbolSize, point.Y + this.ChartStyle.HalfSymbolSize));
            polygon.Points.Add(new Point(point.X - this.ChartStyle.HalfSymbolSize, point.Y + this.ChartStyle.HalfSymbolSize));
            canvas.Children.Add(polygon);
        }

        /// <summary>
        /// Adds the symbol open diamond.
        /// </summary>
        /// <param name="canvas">The canvas.</param>
        /// <param name="point">The point.</param>
        private void addSymbolOpenDiamond(Canvas canvas, Point point)
        {
            Polygon polygon = new Polygon();
            setSymbolStrokeFillAndIndex(polygon);
            polygon.Fill = this.ChartStyle.Stroke;
            polygon.Points.Add(new Point(point.X - this.ChartStyle.HalfSymbolSize, point.Y));
            polygon.Points.Add(new Point(point.X, point.Y - this.ChartStyle.HalfSymbolSize));
            polygon.Points.Add(new Point(point.X + this.ChartStyle.HalfSymbolSize, point.Y));
            polygon.Points.Add(new Point(point.X, point.Y + this.ChartStyle.HalfSymbolSize));
            canvas.Children.Add(polygon);
        }

        /// <summary>
        /// Adds the symbol circle.
        /// </summary>
        /// <param name="canvas">The canvas.</param>
        /// <param name="point">The point.</param>
        private void addSymbolCircle(Canvas canvas, Point point)
        {
            Ellipse ellipse = new Ellipse();
            setSymbolStrokeFillAndIndex(ellipse);
            ellipse.Width = this.ChartStyle.SymbolSize;
            ellipse.Height = this.ChartStyle.SymbolSize;
            Canvas.SetLeft(ellipse, point.X - this.ChartStyle.HalfSymbolSize);
            Canvas.SetTop(ellipse, point.Y - this.ChartStyle.HalfSymbolSize);
            canvas.Children.Add(ellipse);
        }

        /// <summary>
        /// Adds the symbol open triangle.
        /// </summary>
        /// <param name="canvas">The canvas.</param>
        /// <param name="point">The point.</param>
        private void addSymbolOpenTriangle(Canvas canvas, Point point)
        {
            Polygon polygon = new Polygon();
            setSymbolStrokeFillAndIndex(polygon);
            polygon.Fill = this.ChartStyle.Stroke;
            polygon.Points.Add(new Point(point.X - this.ChartStyle.HalfSymbolSize, point.Y + this.ChartStyle.HalfSymbolSize));
            polygon.Points.Add(new Point(point.X, point.Y - this.ChartStyle.HalfSymbolSize));
            polygon.Points.Add(new Point(point.X + this.ChartStyle.HalfSymbolSize, point.Y + this.ChartStyle.HalfSymbolSize));
            canvas.Children.Add(polygon);
        }

        /// <summary>
        /// Adds the symbol cross.
        /// </summary>
        /// <param name="canvas">The canvas.</param>
        /// <param name="point">The point.</param>
        private void addSymbolCross(Canvas canvas, Point point)
        {
            Line line = new Line();
            setSymbolStrokeFillAndIndex(line);
            line.X1 = point.X - this.ChartStyle.HalfSymbolSize;
            line.Y1 = point.Y + this.ChartStyle.HalfSymbolSize;
            line.X2 = point.X + this.ChartStyle.HalfSymbolSize;
            line.Y2 = point.Y - this.ChartStyle.HalfSymbolSize;
            canvas.Children.Add(line);

            line = new Line();
            setSymbolStrokeFillAndIndex(line);
            line.X1 = point.X - this.ChartStyle.HalfSymbolSize;
            line.Y1 = point.Y - this.ChartStyle.HalfSymbolSize;
            line.X2 = point.X + this.ChartStyle.HalfSymbolSize;
            line.Y2 = point.Y + this.ChartStyle.HalfSymbolSize;
            canvas.Children.Add(line);
        }

        /// <summary>
        /// Adds the symbol star.
        /// </summary>
        /// <param name="canvas">The canvas.</param>
        /// <param name="point">The point.</param>
        private void addSymbolStar(Canvas canvas, Point point)
        {
            Line line = new Line();
            setSymbolStrokeFillAndIndex(line);
            line.X1 = point.X - this.ChartStyle.HalfSymbolSize;
            line.Y1 = point.Y + this.ChartStyle.HalfSymbolSize;
            line.X2 = point.X + this.ChartStyle.HalfSymbolSize;
            line.Y2 = point.Y - this.ChartStyle.HalfSymbolSize;
            canvas.Children.Add(line);

            line = new Line();
            setSymbolStrokeFillAndIndex(line);
            line.X1 = point.X - this.ChartStyle.HalfSymbolSize;
            line.Y1 = point.Y - this.ChartStyle.HalfSymbolSize;
            line.X2 = point.X + this.ChartStyle.HalfSymbolSize;
            line.Y2 = point.Y + this.ChartStyle.HalfSymbolSize;
            canvas.Children.Add(line);

            line = new Line();
            setSymbolStrokeFillAndIndex(line);
            line.X1 = point.X - this.ChartStyle.HalfSymbolSize;
            line.Y1 = point.Y;
            line.X2 = point.X + this.ChartStyle.HalfSymbolSize;
            line.Y2 = point.Y;
            canvas.Children.Add(line);

            line = new Line();
            setSymbolStrokeFillAndIndex(line);
            line.X1 = point.X;
            line.Y1 = point.Y - this.ChartStyle.HalfSymbolSize;
            line.X2 = point.X;
            line.Y2 = point.Y + this.ChartStyle.HalfSymbolSize;
            canvas.Children.Add(line);
        }

        /// <summary>
        /// Adds the symbol open inverted triangle.
        /// </summary>
        /// <param name="canvas">The canvas.</param>
        /// <param name="point">The point.</param>
        private void addSymbolOpenInvertedTriangle(Canvas canvas, Point point)
        {
            Polygon polygon = new Polygon();
            setSymbolStrokeFillAndIndex(polygon);
            polygon.Fill = this.ChartStyle.Stroke;
            polygon.Points.Add(new Point(point.X, point.Y + this.ChartStyle.HalfSymbolSize));
            polygon.Points.Add(new Point(point.X - this.ChartStyle.HalfSymbolSize, point.Y - this.ChartStyle.HalfSymbolSize));
            polygon.Points.Add(new Point(point.X + this.ChartStyle.HalfSymbolSize, point.Y - this.ChartStyle.HalfSymbolSize));
            canvas.Children.Add(polygon);
        }

        /// <summary>
        /// Adds the symbol plus.
        /// </summary>
        /// <param name="canvas">The canvas.</param>
        /// <param name="point">The point.</param>
        private void addSymbolPlus(Canvas canvas, Point point)
        {
            Line line = new Line();
            setSymbolStrokeFillAndIndex(line);
            line.X1 = point.X - this.ChartStyle.HalfSymbolSize;
            line.Y1 = point.Y;
            line.X2 = point.X + this.ChartStyle.HalfSymbolSize;
            line.Y2 = point.Y;
            canvas.Children.Add(line);

            line = new Line();
            setSymbolStrokeFillAndIndex(line);
            line.X1 = point.X;
            line.Y1 = point.Y - this.ChartStyle.HalfSymbolSize;
            line.X2 = point.X;
            line.Y2 = point.Y + this.ChartStyle.HalfSymbolSize;
            canvas.Children.Add(line);
        }

        /// <summary>
        /// Adds the symbol dot.
        /// </summary>
        /// <param name="canvas">The canvas.</param>
        /// <param name="point">The point.</param>
        private void addSymbolDot(Canvas canvas, Point point)
        {
            Ellipse ellipse = new Ellipse();
            setSymbolStrokeFillAndIndex(ellipse);
            ellipse.Fill = this.ChartStyle.Stroke;
            ellipse.Width = this.ChartStyle.SymbolSize;
            ellipse.Height = this.ChartStyle.SymbolSize;
            Canvas.SetLeft(ellipse, point.X - this.ChartStyle.HalfSymbolSize);
            Canvas.SetTop(ellipse, point.Y - this.ChartStyle.HalfSymbolSize);
            canvas.Children.Add(ellipse);
        }

        /// <summary>
        /// Adds the symbol box.
        /// </summary>
        /// <param name="canvas">The canvas.</param>
        /// <param name="point">The point.</param>
        private void addSymbolBox(Canvas canvas, Point point)
        {
            Polygon polygon = new Polygon();
            setSymbolStrokeFillAndIndex(polygon);
            polygon.Points.Add(new Point(point.X - this.ChartStyle.HalfSymbolSize, point.Y - this.ChartStyle.HalfSymbolSize));
            polygon.Points.Add(new Point(point.X + this.ChartStyle.HalfSymbolSize, point.Y - this.ChartStyle.HalfSymbolSize));
            polygon.Points.Add(new Point(point.X + this.ChartStyle.HalfSymbolSize, point.Y + this.ChartStyle.HalfSymbolSize));
            polygon.Points.Add(new Point(point.X - this.ChartStyle.HalfSymbolSize, point.Y + this.ChartStyle.HalfSymbolSize));
            canvas.Children.Add(polygon);
        }

        /// <summary>
        /// Adds the symbol diamond.
        /// </summary>
        /// <param name="canvas">The canvas.</param>
        /// <param name="point">The point.</param>
        private void addSymbolDiamond(Canvas canvas, Point point)
        {
            Polygon polygon = new Polygon();
            setSymbolStrokeFillAndIndex(polygon);
            polygon.Points.Add(new Point(point.X - this.ChartStyle.HalfSymbolSize, point.Y));
            polygon.Points.Add(new Point(point.X, point.Y - this.ChartStyle.HalfSymbolSize));
            polygon.Points.Add(new Point(point.X + this.ChartStyle.HalfSymbolSize, point.Y));
            polygon.Points.Add(new Point(point.X, point.Y + this.ChartStyle.HalfSymbolSize));
            canvas.Children.Add(polygon);
        }

        /// <summary>
        /// Adds the symbol inverted triangle.
        /// </summary>
        /// <param name="canvas">The canvas.</param>
        /// <param name="point">The point.</param>
        private void addSymbolInvertedTriangle(Canvas canvas, Point point)
        {
            Polygon polygon = new Polygon();
            setSymbolStrokeFillAndIndex(polygon);
            polygon.Points.Add(new Point(point.X, point.Y + this.ChartStyle.HalfSymbolSize));
            polygon.Points.Add(new Point(point.X - this.ChartStyle.HalfSymbolSize, point.Y - this.ChartStyle.HalfSymbolSize));
            polygon.Points.Add(new Point(point.X + this.ChartStyle.HalfSymbolSize, point.Y - this.ChartStyle.HalfSymbolSize));
            canvas.Children.Add(polygon);
        }

        /// <summary>
        /// Adds the symbol triangle.
        /// </summary>
        /// <param name="canvas">The canvas.</param>
        /// <param name="point">The point.</param>
        private void addSymbolTriangle(Canvas canvas, Point point)
        {
            Polygon polygon = new Polygon();
            setSymbolStrokeFillAndIndex(polygon);
            polygon.Points.Add(new Point(point.X - this.ChartStyle.HalfSymbolSize, point.Y + this.ChartStyle.HalfSymbolSize));
            polygon.Points.Add(new Point(point.X, point.Y - this.ChartStyle.HalfSymbolSize));
            polygon.Points.Add(new Point(point.X + this.ChartStyle.HalfSymbolSize, point.Y + this.ChartStyle.HalfSymbolSize));
            canvas.Children.Add(polygon);
        }

        /// <summary>
        /// Adds the symbol bar.
        /// </summary>
        /// <param name="canvas">The canvas.</param>
        /// <param name="point">The point.</param>
        private void addSymbolBar(Canvas canvas, Point point)
        {
            Point origin = this.normalizedOriginPoint;
            Rectangle bar = new Rectangle();
            setSymbolStrokeFillAndIndex(bar);

            bar.Width = this.ChartStyle.SymbolSize;
            bar.Height = Math.Abs(origin.Y - point.Y);

            double leftOffset = point.X - (bar.Width / 2);
            double topOffset = point.Y;
            if (point.Y > origin.Y)
            {
                topOffset -= bar.Height;
            }
            Canvas.SetLeft(bar, leftOffset);
            Canvas.SetTop(bar, topOffset);
            canvas.Children.Add(bar);
        }

        /// <summary>
        /// Sets the index of the symbol stroke fill and.
        /// </summary>
        /// <param name="shape">The shape.</param>
        private void setSymbolStrokeFillAndIndex(Shape shape)
        {
            shape.Stroke = this.ChartStyle.Stroke;
            shape.StrokeThickness = this.ChartStyle.StrokeThickness;
            shape.Fill = this.ChartStyle.SymbolFill;
            Canvas.SetZIndex(shape, 5);
        }

        /// <summary>
        /// Draws the lines to canvas according point list.
        /// </summary>
        /// <param name="canvas">The canvas.</param>
        /// <param name="pointList">The point list.</param>
        private void drawLines(Canvas canvas, List<Point> pointList)
        {
            bool drawFill = true;
            if (this.ChartStyle.Fill == null || this.ChartStyle.Fill.Equals(Brushes.Transparent))
                drawFill = false;

            switch (this.ChartStyle.DrawType)
            {
                case DrawType.None:
                    break;
                case DrawType.Polyline:
                    drawPolyline(canvas, pointList);
                    if (drawFill)
                    {
                        drawPolylineFill(canvas, pointList);
                    }
                    break;
                case DrawType.Steps:
                    drawSteps(canvas, pointList);
                    if (drawFill)
                    {
                        drawStepsFill(canvas, pointList);
                    }
                    break;
                case DrawType.Impulses:
                    drawImpulses(canvas, pointList);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Draws the polyline.
        /// </summary>
        /// <param name="canvas">The canvas.</param>
        /// <param name="pointList">The point list.</param>
        private void drawPolyline(Canvas canvas, List<Point> pointList)
        {
            Polyline polyline = new Polyline();
            polyline.StrokeDashArray = ChartControl.GetShapePattern(this.ChartStyle.StrokePattern);
            polyline.Stroke = this.ChartStyle.Stroke;
            polyline.StrokeThickness = this.ChartStyle.StrokeThickness;

            polyline.Points = new PointCollection(pointList);
            canvas.Children.Add(polyline);
        }

        /// <summary>
        /// Draws the polyline fill.
        /// </summary>
        /// <param name="canvas">The canvas.</param>
        /// <param name="pointList">The point list.</param>
        private void drawPolylineFill(Canvas canvas, List<Point> pointList)
        {
            Polyline polylineFill = new Polyline();
            polylineFill.Fill = this.ChartStyle.Fill;

            polylineFill.Points = new PointCollection(pointList);
            addBasePointsToFillShape(polylineFill, pointList);
            canvas.Children.Add(polylineFill);
        }

        /// <summary>
        /// Draws the steps.
        /// </summary>
        /// <param name="canvas">The canvas.</param>
        /// <param name="pointList">The point list.</param>
        private void drawSteps(Canvas canvas, List<Point> pointList)
        {
            Polyline stepsPolyline = new Polyline();
            stepsPolyline.StrokeDashArray = ChartControl.GetShapePattern(this.ChartStyle.StrokePattern);
            stepsPolyline.Stroke = this.ChartStyle.Stroke;
            stepsPolyline.StrokeThickness = this.ChartStyle.StrokeThickness;

            double previousYValue = double.NaN;
            foreach (Point point in pointList)
            {
                if (!double.IsNaN(previousYValue))
                {
                    Point stepPoint = new Point(point.X, (double)previousYValue);
                    stepsPolyline.Points.Add(stepPoint);
                }
                stepsPolyline.Points.Add(point);
                previousYValue = point.Y;
            }
            canvas.Children.Add(stepsPolyline);
        }

        /// <summary>
        /// Draws the steps fill.
        /// </summary>
        /// <param name="canvas">The canvas.</param>
        /// <param name="pointList">The point list.</param>
        private void drawStepsFill(Canvas canvas, List<Point> pointList)
        {
            Polyline stepsFillPolyline = new Polyline();
            stepsFillPolyline.Fill = this.ChartStyle.Fill;

            double previousYValue = double.NaN;
            foreach (Point point in pointList)
            {
                if (!double.IsNaN(previousYValue))
                {
                    Point stepPoint = new Point(point.X, (double)previousYValue);
                    stepsFillPolyline.Points.Add(stepPoint);
                }
                stepsFillPolyline.Points.Add(point);
                previousYValue = point.Y;
            }
            addBasePointsToFillShape(stepsFillPolyline, pointList);
            canvas.Children.Add(stepsFillPolyline);
        }

        /// <summary>
        /// Draws the impulses.
        /// </summary>
        /// <param name="canvas">The canvas.</param>
        /// <param name="pointList">The point list.</param>
        private void drawImpulses(Canvas canvas, List<Point> pointList)
        {
            Point origin = this.normalizedOriginPoint;
            foreach (Point point in pointList)
            {
                Line impuls = new Line();
                impuls.StrokeDashArray = ChartControl.GetShapePattern(this.ChartStyle.StrokePattern);
                impuls.Stroke = this.ChartStyle.Stroke;
                impuls.StrokeThickness = this.ChartStyle.StrokeThickness;
                impuls.X1 = point.X;
                impuls.Y1 = origin.Y;
                impuls.X2 = point.X;
                impuls.Y2 = point.Y;
                canvas.Children.Add(impuls);
            }
        }

        /// <summary>
        /// Adds the base points to point list for fill shape.
        /// </summary>
        /// <param name="polylineFill">The polyline fill.</param>
        /// <param name="pointList">The point list dor adding base points.</param>
        private void addBasePointsToFillShape(Polyline polylineFill, List<Point> pointList)
        {
            Point point = this.normalizedOriginPoint;
            int lastPointIndex = pointList.Count - 1;
            point.X = pointList[lastPointIndex].X;
            polylineFill.Points.Add(point);
            point.X = pointList[0].X;
            polylineFill.Points.Add(point);
        }

        #endregion Methods
    }
}
