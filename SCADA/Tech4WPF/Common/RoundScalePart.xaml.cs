using System;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Data;
using System.Windows.Shapes;
using Microsoft.Expression.Drawing;
using System.ComponentModel;

namespace Tech4WPF.Common
{
    /// <summary>
    /// Interaction logic for RoundScalePart.xaml
    /// </summary>
    internal partial class RoundScalePart : UserControl, IRoundScale
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RoundScalePart"/> class.
        /// </summary>
        public RoundScalePart() 
        {
            InitializeComponent();

            this.valueForegroundBinding.Source = this;
            this.valueForegroundBinding.Path = new PropertyPath("ValueForeground");

            updateScale();
        }

        #endregion Constructors

        #region Fields

        /// <summary>
        /// List of drawn major stamps.
        /// </summary>
        private List<Path> majorStamps = new List<Path>();

        /// <summary>
        /// List of drawn minor stamps.
        /// </summary>
        private List<Path> minorStamps = new List<Path>();

        /// <summary>
        /// List of drawn values
        /// </summary>
        private List<TextBlock> stampValues = new List<TextBlock>();

        /// <summary>
        /// Binding for settig foreground binding to new generated stamp values
        /// </summary>
        Binding valueForegroundBinding = new Binding();

        #endregion Fields

        #region Constants

        private const double DEFAULT_SCALE_RADIUS = 100 / 2;
        private const double DEFAULT_SCALE_VALUES_RADIUS = DEFAULT_SCALE_RADIUS + 18;

        #endregion Constants

        #region Events

        /// <summary>
        /// Occurs when scale changes values, angles or stamps.
        /// </summary>
        [Description("Occurs when scale changes values, angles or stamps.")]
        public event EventHandler ScaleChanged;

        #endregion Events

        #region Properties

        public static readonly DependencyProperty MinimumProperty = DependencyProperty.Register(
            "Minimum", typeof(double), typeof(RoundScalePart),
            new PropertyMetadata(0.0, scaleChangedCallback, coerceValueCallback));
        /// <summary>
        /// Gets or sets the Minimum value of the scale.
        /// </summary>
        /// <value>
        /// The minimum value.
        /// </value>
        [Description("Gets or sets the Minimum value of the scale.")]
        public double Minimum
        {
            get { return (double)GetValue(MinimumProperty); }
            set { SetValue(MinimumProperty, value); }
        }

        public static readonly DependencyProperty MaximumProperty = DependencyProperty.Register(
            "Maximum", typeof(double), typeof(RoundScalePart),
            new PropertyMetadata(100.0, scaleChangedCallback, coerceValueCallback));
        /// <summary>
        /// Gets or sets the Maximum value of the scale.
        /// </summary>
        /// <value>
        /// The maximum value.
        /// </value>
        [Description("Gets or sets the Maximum value of the scale.")]
        public double Maximum
        {
            get { return (double)GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }


        /// <summary>
        /// Gets the absolute value of range (Maximum - Minimum).
        /// </summary>
        [Description("Gets the absolute value of range (Maximum - Minimum).")]
        public double Range
        {
            get { return Math.Abs(this.Maximum - this.Minimum); }
        }

        public static readonly DependencyProperty StartAngleProperty = DependencyProperty.Register(
            "StartAngle", typeof(double), typeof(RoundScalePart),
            new PropertyMetadata(225.0, scaleChangedCallback, coerceAngleCallback));
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
            get { return (double)GetValue(StartAngleProperty); }
            set { SetValue(StartAngleProperty, value); }
        }

        public static readonly DependencyProperty EndAngleProperty = DependencyProperty.Register(
            "EndAngle", typeof(double), typeof(RoundScalePart),
            new PropertyMetadata(135.0, scaleChangedCallback, coerceAngleCallback));
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
            get { return (double)GetValue(EndAngleProperty); }
            set { SetValue(EndAngleProperty, value); }
        }

        /// <summary>
        /// Gets the angle range of scale in degrees (EndAngle - StarAngle).
        /// </summary>
        [Description("Gets the angle range of scale in degrees (EndAngle - StarAngle).")]
        public double AngleRange
        {
            get { return TechUtils.AngleToPositive360(this.EndAngle - this.StartAngle); }
        }

        public static readonly DependencyProperty FormatStringProperty = DependencyProperty.Register(
            "FormatString", typeof(string), typeof(RoundScalePart),
            new PropertyMetadata("{0:0.##}", valueFormatChangedCallback));
        /// <summary>
        /// Gets or sets the format string for scale values.
        /// </summary>
        [Description("Gets or sets the format string for scale values.")]
        public string FormatString
        {
            get { return GetValue(FormatStringProperty).ToString(); }
            set { SetValue(FormatStringProperty, value); }
        }

        public static readonly DependencyProperty MinorStepProperty = DependencyProperty.Register(
            "MinorStep", typeof(double), typeof(RoundScalePart),
            new PropertyMetadata(5.0, scaleChangedCallback, coerceValueCallback));
        /// <summary>
        /// Gets or sets the minor step.
        /// </summary>
        /// <value>
        /// The minor step.
        /// </value>
        [Description("Gets or sets the minor step.")]
        public double MinorStep
        {
            get { return (double)GetValue(MinorStepProperty); }
            set { SetValue(MinorStepProperty, value); }
        }

        public static readonly DependencyProperty MajorStepProperty = DependencyProperty.Register(
            "MajorStep", typeof(double), typeof(RoundScalePart),
            new PropertyMetadata(10.0, scaleChangedCallback, coerceValueCallback));
        /// <summary>
        /// Gets or sets the major step.
        /// </summary>
        /// <value>
        /// The major step.
        /// </value>
        [Description("Gets or sets the major step.")]
        public double MajorStep
        {
            get { return (double)GetValue(MajorStepProperty); }
            set { SetValue(MajorStepProperty, value); }
        }

        public static readonly DependencyProperty MinorStampStrokeProperty = DependencyProperty.Register(
            "MinorStampStroke", typeof(Brush), typeof(RoundScalePart),
            new PropertyMetadata(Brushes.Gray));
        /// <summary>
        /// Gets or sets the minor stamp stroke.
        /// </summary>
        /// <value>
        /// The minor stamp stroke.
        /// </value>
        [Description("Gets or sets the minor stamp stroke.")]
        public Brush MinorStampStroke
        {
            get { return (Brush)GetValue(MinorStampStrokeProperty); }
            set { SetValue(MinorStampStrokeProperty, value); }
        }

        public static readonly DependencyProperty MajorStampStrokeProperty = DependencyProperty.Register(
            "MajorStampStroke", typeof(Brush), typeof(RoundScalePart),
            new PropertyMetadata(Brushes.Black));
        /// <summary>
        /// Gets or sets the major stamp stroke.
        /// </summary>
        /// <value>
        /// The major stamp stroke.
        /// </value>
        [Description("Gets or sets the major stamp stroke.")]
        public Brush MajorStampStroke
        {
            get { return (Brush)GetValue(MajorStampStrokeProperty); }
            set { SetValue(MajorStampStrokeProperty, value); }
        }

        public static readonly DependencyProperty ScaleStrokeProperty = DependencyProperty.Register(
            "ScaleStroke", typeof(Brush), typeof(RoundScalePart),
            new PropertyMetadata(Brushes.Black));
        /// <summary>
        /// Gets or sets the scale stroke.
        /// </summary>
        /// <value>
        /// The scale stroke.
        /// </value>
        [Description("Gets or sets the scale stroke.")]
        public Brush ScaleStroke
        {
            get { return (Brush)GetValue(ScaleStrokeProperty); }
            set { SetValue(ScaleStrokeProperty, value); }
        }

        public static readonly DependencyProperty ValueForegroundProperty = DependencyProperty.Register(
            "ValueForeground", typeof(Brush), typeof(RoundScalePart),
            new PropertyMetadata(Brushes.Black));
        /// <summary>
        /// Gets or sets the value foreground.
        /// </summary>
        /// <value>
        /// The value foreground.
        /// </value>
        [Description("Gets or sets the value foreground.")]
        public Brush ValueForeground
        {
            get { return (Brush)GetValue(ValueForegroundProperty); }
            set { SetValue(ValueForegroundProperty, value); }
        }

        public static readonly DependencyProperty ValuesHorizontallyProperty = DependencyProperty.Register(
            "ValuesHorizontally", typeof(bool), typeof(RoundScalePart),
            new PropertyMetadata(true, valueFormatChangedCallback));
        /// <summary>
        /// Gets or sets a value indicating horizontal position of the scale values.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if values of the scale are horizontally; otherwise, <c>false</c>.
        /// </value>
        [Description("Gets or sets a value indicating horizontal position of the scale values.")]
        public bool ValuesHorizontally
        {
            get { return (bool)GetValue(ValuesHorizontallyProperty); }
            set { SetValue(ValuesHorizontallyProperty, value); }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Updates scale when depedent property changed.
        /// </summary>
        /// <param name="d">The <see cref="System.Windows.DependencyObject"/> instance.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void scaleChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RoundScalePart roundScale = (RoundScalePart)d;
            roundScale.updateScale();
        }

        /// <summary>
        /// Updates scale values when depedent property changed..
        /// </summary>
        /// <param name="d">The <see cref="System.Windows.DependencyObject"/> instance.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void valueFormatChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RoundScalePart roundScale = (RoundScalePart)d;
            roundScale.updateValues();
        }

        /// <summary>
        /// Replaces case angle value Infinity or NaN with zero.
        /// Converts angle to positive angle &lt;0; 360) degrees
        /// </summary>
        /// <param name="d">The  <see cref="System.Windows.DependencyObject"/> instance.</param>
        /// <param name="baseValue">The base value assigned into property.</param>
        /// <returns>Non NaN/Infinity angle in degrees in &lt;0; 360) interval</returns>
        private static object coerceAngleCallback(DependencyObject d, object baseValue)
        {
            double value = (double)baseValue;
            value = TechUtils.ReplaceInfinityAndNaN(value);
            return TechUtils.AngleToPositive360(value);
        }

        /// <summary>
        /// Replaces case value Infinity or NaN with zero.
        /// </summary>
        /// <param name="d">The <see cref="System.Windows.DependencyObject"/> instance.</param>
        /// <param name="baseValue">The base value assigned into property.</param>
        /// <returns></returns>
        private static object coerceValueCallback(DependencyObject d, object baseValue)
        {
            double value = (double)baseValue;
            return TechUtils.ReplaceInfinityAndNaN(value);
        }

        /// <summary>
        /// Updates all stamps and values in the scale.
        /// Fires scale changed event
        /// </summary>
        private void updateScale()
        {
            updateStamps(this.minorStamps, this.MinorStep, this.templateMinorStamp);
            updateStamps(this.majorStamps, this.MajorStep, this.templateMajorStamp);
            updateValues();
            fireScaleChanged();
        }

        /// <summary>
        /// Updates all the values in the scale.
        /// </summary>
        private void updateValues()
        {
            int stampValuesCount = (int)Math.Ceiling(this.Range / this.MajorStep) + 1;
            double stampAngleStep = this.AngleRange * (this.MajorStep / this.Range);

            if (stampValuesCount != this.stampValues.Count)
            {
                removeStamps(this.stampValues);
                generateValueStamps(stampValuesCount);
            }
            setAnglesAndValuesToValueStamps(stampAngleStep);
        }

        /// <summary>
        /// Fires the scale changed event.
        /// </summary>
        private void fireScaleChanged()
        {
            //Maintaining the temporary copy of event to avoid race condition 
            EventHandler scaleChanged = ScaleChanged;
            if (scaleChanged != null)
            {
                scaleChanged(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Updates the stamps.
        /// </summary>
        /// <param name="stamps">The list for stamps</param>
        /// <param name="step">The step.</param>
        /// <param name="templateStamp">The template stamp.</param>
        private void updateStamps(List<Path> stamps, double step, Path templateStamp)
        {
            int stampCount = (int)Math.Ceiling(this.Range / step) + 1;
            double stampAngleStep = this.AngleRange * (step / this.Range);

            if (stampCount != stamps.Count)
            {
                removeStamps(stamps);
                generateStamps(stamps, stampCount, templateStamp);
            }
            setAnglesToStamps(stamps, stampAngleStep);
        }

        /// <summary>
        /// Removes the stamps from canvas and clears the list.
        /// </summary>
        /// <typeparam name="T"><see cref="System.Windows.FrameworkElement"/></typeparam>
        /// <param name="stampList">The stamp list.</param>
        private void removeStamps<T>(List<T> stampList) 
            where T : FrameworkElement
        {
            foreach (T stamp in stampList)
            {
                scaleGrid.Children.Remove(stamp);
            }
            stampList.Clear();
        }

        /// <summary>
        /// Generates the value stamps.
        /// </summary>
        /// <param name="stampValuesCount">Values count.</param>
        private void generateValueStamps(int stampValuesCount)
        {
            for (int i = 0; i < stampValuesCount; i++)
            {
                TextBlock tmpStamp = new TextBlock();
                tmpStamp.TextAlignment = TextAlignment.Center;
                tmpStamp.FontSize = 10;
                tmpStamp.SetBinding(TextBlock.ForegroundProperty, valueForegroundBinding);
                tmpStamp.VerticalAlignment = VerticalAlignment.Center;
                tmpStamp.HorizontalAlignment = HorizontalAlignment.Center;
                tmpStamp.SetValue(Panel.ZIndexProperty, 3);

                stampValues.Add(tmpStamp);
                scaleGrid.Children.Add(tmpStamp);
            }
        }

        /// <summary>
        /// Sets the angles and values to value stamps.
        /// </summary>
        /// <param name="stampAngleStep">The stamp angle step.</param>
        private void setAnglesAndValuesToValueStamps(double stampAngleStep)
        {
            int lastStamp = this.stampValues.Count - 1;
            for (int i = 0; i < this.stampValues.Count; i++)
            {
                double angleInDegrees;
                double value;

                if (i != lastStamp)
                {
                    angleInDegrees = this.StartAngle + i * stampAngleStep;
                    value = this.Minimum + i * this.MajorStep;
                }
                else
                {
                    //Last stamp is EVER at the end of the scale and
                    //it's value is EVER Maximum
                    angleInDegrees = this.EndAngle;
                    value = this.Maximum;
                }

                this.stampValues[i].RenderTransform = getValueRenderTransform(angleInDegrees);
                this.stampValues[i].LayoutTransform = getValueLayoutTransform(angleInDegrees);
                this.stampValues[i].Text = string.Format(this.FormatString, value);
            }
        }

        /// <summary>
        /// Generates the stamps.
        /// </summary>
        /// <param name="stamps">The stamps.</param>
        /// <param name="count">The stamps count.</param>
        /// <param name="templateStamp">The template stamp.</param>
        private void generateStamps(List<Path> stamps, int count, Path templateStamp)
        {
            BindingExpression templateStampStrokeBinding = templateStamp.GetBindingExpression(Path.StrokeProperty);
            for (int i = 0; i < count; i++)
            {
                Path tmpStamp = new Path();
                tmpStamp.Height = templateStamp.Height;
                tmpStamp.Fill = templateStamp.Fill;
                tmpStamp.Stretch = templateStamp.Stretch;
                tmpStamp.SetBinding(Path.StrokeProperty, templateStampStrokeBinding.ParentBindingBase);
                tmpStamp.StrokeThickness = templateStamp.StrokeThickness;
                tmpStamp.Data = templateStamp.Data;
                tmpStamp.Margin = templateStamp.Margin;
                tmpStamp.VerticalAlignment = templateStamp.VerticalAlignment;
                tmpStamp.RenderTransformOrigin = templateStamp.RenderTransformOrigin;
                tmpStamp.RenderTransform = new RotateTransform();
                tmpStamp.SetValue(Panel.ZIndexProperty, templateStamp.GetValue(Panel.ZIndexProperty));

                stamps.Add(tmpStamp);
                this.scaleGrid.Children.Add(tmpStamp);
            }
        }

        /// <summary>
        /// Sets the angles to stamps.
        /// </summary>
        /// <param name="stamps">The stamps.</param>
        /// <param name="step">The step in degrees.</param>
        private void setAnglesToStamps(List<Path> stamps, double step)
        {
            int lastStamp = stamps.Count - 1;
            for (int i = 0; i < stamps.Count; i++)
            {
                RotateTransform transform = new RotateTransform();
                if (i != lastStamp)
                {
                    transform.Angle = this.StartAngle + i * step;
                }
                else
                {
                    //Last stamp is EVER at the end of the scale
                    transform.Angle = this.EndAngle;
                }
                stamps[i].RenderTransform = transform;
            }
        }

        /// <summary>
        /// Gets the value render transform.
        /// </summary>
        /// <param name="angleInDegrees">The angle in degrees.</param>
        /// <returns><see cref="System.Windows.Media.TranslateTransform"/></returns>
        private TranslateTransform getValueRenderTransform(double angleInDegrees)
        {
            double angleInRadians = TechUtils.DegreeToRadian(angleInDegrees);
            TranslateTransform translateTransform = new TranslateTransform();
            translateTransform.X = DEFAULT_SCALE_VALUES_RADIUS * Math.Sin(angleInRadians);
            translateTransform.Y = -DEFAULT_SCALE_VALUES_RADIUS * Math.Cos(angleInRadians);
            return translateTransform;
        }

        /// <summary>
        /// Gets the value layout transform.
        /// If ValuesHorizontally is true, rentruns MatrixTransform.Identity
        /// otherwise returns returns <see cref="System.Windows.Media.RotateTransform"/>
        /// by given angle
        /// </summary>
        /// <param name="angleInDegrees">The angle in degrees.</param>
        /// <returns><see cref="System.Windows.Media.Transform"/></returns>
        private Transform getValueLayoutTransform(double angleInDegrees)
        {
            if (this.ValuesHorizontally)
            {
                return MatrixTransform.Identity;
            }
            else
            {
                RotateTransform rotateTransform = new RotateTransform();
                rotateTransform.Angle = angleInDegrees;
                return rotateTransform;
            }
        }

        #endregion Methods
    }
}
