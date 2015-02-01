using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Controls;

namespace Tech4WPF.ChartControl
{
    /// <summary>
    /// Represents style for drawing chart
    /// </summary>
    public class ChartStyle
    {
        #region Properties

        private double symbolSize = 8;
        /// <summary>
        /// Gets or sets the size of the symbol.
        /// Default value is <c>8.0</c>
        /// </summary>
        /// <value>
        /// The size of the symbol.
        /// </value>
        public double SymbolSize
        {
            get { return this.symbolSize; }
            set { this.symbolSize = value; }
        }

        /// <summary>
        /// Gets the half size of the symbol.
        /// </summary>
        /// <value>
        /// The half size of the symbol.
        /// </value>
        internal double HalfSymbolSize
        {
            get { return this.symbolSize / 2; }
        }

        private SymbolType symbols = SymbolType.Plus;
        /// <summary>
        /// Gets or sets the symbols. 
        /// Default value is <c>SymbolType.Plus</c>
        /// </summary>
        /// <value>
        /// The symbols for this chart.
        /// </value>
        public SymbolType SymbolType
        {
            get { return this.symbols; }
            set { this.symbols = value; }
        }

        private Brush symbolFill = Brushes.White.Clone();
        /// <summary>
        /// Gets or sets the fill of the symbol.
        /// Default value is <c>White</c>
        /// </summary>
        /// <value>
        /// The symbol fill.
        /// </value>
        public Brush SymbolFill
        {
            get { return this.symbolFill; }
            set { this.symbolFill = value; }
        }

        private DrawType drawType = DrawType.Polyline;
        /// <summary>
        /// Gets or sets the draw type.
        /// Default value is <c>DrawType.Polyline</c>
        /// </summary>
        /// <value>
        /// The draw type.
        /// </value>
        public DrawType DrawType
        {
            get { return this.drawType; }
            set { this.drawType = value; }
        }

        private double strokeThickness = 1;
        /// <summary>
        /// Gets or sets the stroke thickness.
        /// Default value is <c>1.0</c>
        /// </summary>
        /// <value>
        /// The stroke thickness.
        /// </value>
        public double StrokeThickness
        {
            get { return this.strokeThickness; }
            set { this.strokeThickness = value; }
        }

        private Brush stroke = Brushes.Blue.Clone();
        /// <summary>
        /// Gets or sets the stroke brush.
        /// Default value is <c>Blue</c>
        /// </summary>
        /// <value>
        /// The color of the stroke.
        /// </value>
        public Brush Stroke
        {
            get { return this.stroke; }
            set { this.stroke = value; }
        }

        private Brush fill = null;
        /// <summary>
        /// Gets or sets the fill brush.
        /// Default value is <c>null</c>
        /// </summary>
        /// <value>
        /// The color of the fill.
        /// </value>
        public Brush Fill
        {
            get { return this.fill; }
            set { this.fill = value; }
        }

        private StrokePattern strokePattern = StrokePattern.Solid;
        /// <summary>
        /// Gets or sets the stroke pattern.
        /// </summary>
        /// <value>
        /// The stroke pattern.
        /// </value>
        public StrokePattern StrokePattern
        {
            get { return this.strokePattern; }
            set { this.strokePattern = value; }
        }

        #endregion Properties
    }
}
