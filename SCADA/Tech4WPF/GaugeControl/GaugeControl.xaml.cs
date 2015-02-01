using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Tech4WPF.Common;
using System.Windows.Markup;


namespace Tech4WPF.GaugeControl
{
    /// <summary>
    /// Interaction logic for GaugeControl.xaml
    /// </summary>
    [ContentProperty("Label")]
    [DefaultEvent("PropertyChanged")]
    public partial class GaugeControl : UserControl, INotifyPropertyChanged
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="GaugeControl"/> class.
        /// </summary>
        public GaugeControl()
        {
            InitializeComponent();

            this.scale.ScaleChanged += new EventHandler(onScaleChanged);
            this.pointer.PropertyChanged += new PropertyChangedEventHandler(onPointerPropertyChanged);
            this.pointer.OriginAngle = this.scale.StartAngle;

            Reset();
            this.pointer.Invalidate();
        }

        #endregion Constructors

        #region Fields

        private double defaultValue;
        private bool snap;
        private string valueFormatString = "{0:0.00}";

        #endregion Fields

        #region Events

        /// <summary>
        /// Occurs when a pointer changes angle => value is changed.
        /// </summary>
        [Description("Occurs when a pointer changes angle => value is changed.")]
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion Events

        #region Properties

        /// <summary>
        /// Gets or sets the Minimum value of the scale.
        /// </summary>
        /// <value>
        /// The minimum value.
        /// </value>
        [Description("Gets or sets the Minimum value of the scale.")]
        public double Minimum
        {
            get { return this.scale.Minimum; }
            set { this.scale.Minimum = value; }
        }

        /// <summary>
        /// Gets or sets the Maximum value of the scale.
        /// </summary>
        /// <value>
        /// The maximum value.
        /// </value>
        [Description("Gets or sets the Maximum value of the scale.")]
        public double Maximum
        {
            get { return this.scale.Maximum; }
            set { this.scale.Maximum = value; }
        }

        /// <summary>
        /// Gets the absolute value of range (Maximum - Minimum).
        /// </summary>
        [Description("Gets the absolute value of range (Maximum - Minimum).")]
        public double Range
        {
            get { return this.scale.Range; }
        }

        /// <summary>
        /// Gets or sets pointer to the value.
        /// If the value is lower then Minimum - Minimum is set.
        /// If the value is greather then Maximum - Maximum is set.
        /// If the value is NaN or Infinity - DefaultValue is set.
        /// If Snap mode is set to true, nearest step is set.
        /// </summary>
        [Description("Gets or sets pointer to the value. " +
                     "If the value is lower then Minimum - Minimum is set. " +
                     "If the value is greather then Maximum - Maximum is set. " +
                     "If the value is NaN or Infinity - DefaultValue is set. " +
                     "If Snap mode is set to true, nearest step is set. ")]
        public double Value
        {
            get
            {
                return ((this.pointer.Angle / this.scale.AngleRange) * this.scale.Range) + this.scale.Minimum;
            }
            set
            {
                if (double.IsNaN(value) || double.IsInfinity(value))
                {
                    value = this.DefaultValue;
                }
                else if (value > this.Maximum)
                {
                    value = this.Maximum;
                }
                else if (value < this.Minimum)
                {
                    value = this.Minimum;
                }

                double newAngle = ((value - this.scale.Minimum) / this.scale.Range) * this.scale.AngleRange;
                this.pointer.Angle = (this.Snap) ? snapAngle(newAngle, this.pointer.Angle, false) : newAngle;
            }
        }

        /// <summary>
        /// Gets or sets the label for GaugeControl.
        /// </summary>
        [Description("Gets or sets the label for GaugeControl.")]
        public string Label
        {
            get { return this.label.Text; }
            set { this.label.Text = value; }
        }

        /// <summary>
        /// Gets or sets the label foreground.
        /// </summary>
        /// <value>
        /// The label foreground.
        /// </value>
        [Description("Gets or sets the label foreground.")]
        public Brush LabelForeground
        {
            get { return this.label.Foreground; }
            set { this.label.Foreground = value; }
        }

        /// <summary>
        /// Gets or sets the format string for scale values.
        /// </summary>
        [Description("Gets or sets the format string for scale values.")]
        public string ScaleFormatString
        {
            get { return this.scale.FormatString; }
            set { this.scale.FormatString = value; }
        }

        /// <summary>
        /// Gets or sets the format string for the value display
        /// </summary>
        [Description("Gets or sets the format string for the value display")]
        public string ValueFormatString
        {
            get { return this.valueFormatString; }
            set { this.valueFormatString = value; }
        }

        /// <summary>
        /// Gets or sets the start angle in degrees for scale. Angle begins at the top.
        /// Begining point is at the top. Angle is unlimited and can be negative.
        /// It will be always converted to positive angle up to 360. 
        /// NaN and Infinity values will be replaced with 0.0
        /// </summary>
        /// <value>
        /// The start angle in degrees.
        /// </value>
        [Description("Gets or sets the start angle in degrees for scale. Angle begins at the top. " +
                     "Begining point is at the top. Angle is unlimited and can be negative. " +
                     "It will be always converted to positive angle up to 360. " +
                     "NaN and Infinity values will be replaced with 0.0")]
        public double StartAngle
        {
            get { return this.scale.StartAngle; }
            set
            {
                this.pointer.OriginAngle = value;
                this.scale.StartAngle = value;
            }
        }

        /// <summary>
        /// Gets or sets the end angle in degrees for scale. Angle begins at the top.
        /// Begining point is at the top. Angle is unlimited and can be negative.
        /// It will be always converted to positive angle up to 360. 
        /// NaN and Infinity values will be replaced with 0.0
        /// </summary>
        /// <value>
        /// The end angle in degrees.
        /// </value>
        [Description("Gets or sets the end angle in degrees for scale. Angle begins at the top. " +
                     "Begining point is at the top. Angle is unlimited and can be negative. " +
                     "It will be always converted to positive angle up to 360. " +
                     "NaN and Infinity values will be replaced with 0.0")]
        public double EndAngle
        {
            get { return this.scale.EndAngle; }
            set { this.scale.EndAngle = value; }
        }

        /// <summary>
        /// Gets the angle range of scale in degrees (EndAngle - StarAngle).
        /// </summary>
        [Description("Gets the angle range of scale in degrees (EndAngle - StarAngle).")]
        public double AngleRange
        {
            get { return this.scale.AngleRange; }
        }

        /// <summary>
        /// Gets or sets the minor step.
        /// </summary>
        /// <value>
        /// The minor step.
        /// </value>
        [Description("Gets or sets the minor step.")] 
        public double MinorStep
        {
            get { return this.scale.MinorStep; }
            set { this.scale.MinorStep = value; }
        }

        /// <summary>
        /// Gets or sets the major step.
        /// </summary>
        /// <value>
        /// The major step.
        /// </value>
        [Description("Gets or sets the major step.")] 
        public double MajorStep
        {
            get { return this.scale.MajorStep; }
            set { this.scale.MajorStep = value; }
        }

        /// <summary>
        /// Gets or sets the minor stamp stroke.
        /// </summary>
        /// <value>
        /// The minor stamp stroke.
        /// </value>
        [Description("Gets or sets the minor stamp stroke.")] 
        public Brush MinorStampStroke
        {
            get { return this.scale.MinorStampStroke; }
            set { this.scale.MinorStampStroke = value; }
        }

        /// <summary>
        /// Gets or sets the major stamp stroke.
        /// </summary>
        /// <value>
        /// The major stamp stroke.
        /// </value>
        [Description("Gets or sets the major stamp stroke.")] 
        public Brush MajorStampStroke
        {
            get { return this.scale.MajorStampStroke; }
            set { this.scale.MajorStampStroke = value; }
        }

        /// <summary>
        /// Gets or sets the scale stroke.
        /// </summary>
        /// <value>
        /// The scale stroke.
        /// </value>
        [Description("Gets or sets the scale stroke.")]
        public Brush ScaleStroke
        {
            get { return this.scale.ScaleStroke; }
            set { this.scale.ScaleStroke = value; }
        }

        /// <summary>
        /// Gets or sets the pointer fill.
        /// </summary>
        /// <value>
        /// The pointer fill.
        /// </value>
        [Description("Gets or sets the pointer fill.")]
        public Brush PointerFill
        {
            get { return this.pointer.PointerFill; }
            set { this.pointer.PointerFill = value; }
        }

        /// <summary>
        /// Gets or sets the pointer stroke.
        /// </summary>
        /// <value>
        /// The pointer stroke.
        /// </value>
        [Description("Gets or sets the pointer stroke.")]
        public Brush PointerStroke
        {
            get { return this.pointer.PointerStroke; }
            set { this.pointer.PointerStroke = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="GaugeControl"/> 
        /// has snap mode enabled.
        /// </summary>
        /// <value>
        ///   <c>true</c> if snap enabled; otherwise, <c>false</c>.
        /// </value>
        [Description("Gets or sets a value indicating whether this GaugeControl " +
                     "has snap mode enabled.")]
        public bool Snap
        {
            get { return this.snap; }
            set { this.snap = value; }
        }

        /// <summary>
        /// Gets or sets the default value for the gauge.
        /// Default value is set when attempting assign NaN or Infinity to gauge value
        /// or when scale changes value bounds or steps.
        /// </summary>
        /// <value>
        /// The default value.
        /// </value>
        [Description("Gets or sets the default value for the gauge. " +
                     "Default value is set when attempting assign NaN or Infinity to gauge value " +
                     "or when scale changes value bounds or steps. ")]
        public double DefaultValue
        {
            get { return this.defaultValue; }
            set { this.defaultValue = value; }
        }

        /// <summary>
        /// Gets or sets the scale value foreground.
        /// </summary>
        /// <value>
        /// The scale value foreground.
        /// </value>
        [Description("Gets or sets the scale value foreground.")] 
        public Brush ScaleValueForeground
        {
            get { return this.scale.ValueForeground; }
            set { this.scale.ValueForeground = value; }
        }

        /// <summary>
        /// Gets or sets the value display foreground.
        /// </summary>
        /// <value>
        /// The value display foreground.
        /// </value>
        [Description("Gets or sets the value display foreground.")] 
        public Brush ValueDisplayForeground
        {
            get { return this.valueDisplay.Foreground; }
            set { this.valueDisplay.Foreground = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating horizontal position of the scale values.
        /// </summary>
        /// <value>
        ///   <c>true</c> if values of the scale are horizontally; otherwise, <c>false</c>.
        /// </value>
        [Description("Gets or sets bool value indicating horizontal position of the scale values")] 
        public bool ValuesHorizontally
        {
            get { return this.scale.ValuesHorizontally; }
            set { this.scale.ValuesHorizontally = value; }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Resets this value of the gauge to default value.
        /// </summary>
        public void Reset()
        {
            this.Value = this.DefaultValue;
            this.pointer.Invalidate();
        }

        /// <summary>
        /// Snaps the angle to nearest stamp or
        /// in case wheel scrolling next nearest stamp in scrolling direction.
        /// </summary>
        /// <param name="newAngle">The new angle.</param>
        /// <param name="oldAngle">The old angle.</param>
        /// <param name="isWheelScrolling"><c>true</c> indicating wheel scrolling.</param>
        /// <returns>Suitable angle matching with some of stamps</returns>
        private double snapAngle(double newAngle, double oldAngle, bool isWheelScrolling)
        {
            TechUtils.MathRoundingMode mathRoundingMode = Math.Round;
            if (isWheelScrolling)
            {
                if (newAngle <= oldAngle)
                {
                    mathRoundingMode = Math.Floor;
                }
                else if (newAngle > oldAngle)
                {
                    mathRoundingMode = Math.Ceiling;
                }
            }

            double minorAngleStep = this.scale.AngleRange * (this.MinorStep / this.Range);
            double majorAngleStep = this.scale.AngleRange * (this.MajorStep / this.Range);
            double minorNear = mathRoundingMode(newAngle / minorAngleStep) * minorAngleStep;
            double majorNear = mathRoundingMode(newAngle / majorAngleStep) * majorAngleStep;
            double minorRating = Math.Abs(minorNear - newAngle);
            double majorRating = Math.Abs(majorNear - newAngle);
            double endRating = Math.Abs(this.scale.AngleRange - newAngle);

            if (isWheelScrolling && (oldAngle == this.scale.AngleRange))
            {
                //prevent stack up on the end of the scale
                endRating = double.PositiveInfinity;
            }


            if ((minorRating <= majorRating) && (minorRating <= endRating))
            {
                newAngle = minorNear;
            }
            else if ((majorRating <= minorRating) && (majorRating <= endRating))
            {
                newAngle = majorNear;
            }
            else if ((endRating <= minorRating) && (endRating <= majorRating))
            {
                newAngle = this.scale.AngleRange;
            }
            return newAngle;
        }

        /// <summary>
        /// Fires the value changed event.
        /// </summary>
        /// <param name="property">The string representating name of changed property</param>
        private void fireValueChanged(string property)
        {
            //Maintaining the temporary copy of event to avoid race condition 
            PropertyChangedEventHandler valueChanged = PropertyChanged;
            if (valueChanged != null)
            {
                valueChanged(this, new PropertyChangedEventArgs(property));
            }
        }

        /// <summary>
        /// Invoked when pointer changed its property.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">
        /// The <see cref="System.ComponentModel.PropertyChangedEventArgs"/> instance 
        /// containing the event data.
        /// </param>
        private void onPointerPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Angle")
            {
                fireValueChanged("Value");
                updateValueDisplay();
            }
        }

        /// <summary>
        /// Resets the value when scale changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void onScaleChanged(object sender, EventArgs e)
        {
            Reset();
        }

        /// <summary>
        /// Updates the value display number.
        /// </summary>
        private void updateValueDisplay()
        {
            this.valueDisplay.Text = string.Format(this.ValueFormatString, this.Value);
        }

        #endregion Methods
    }
}
