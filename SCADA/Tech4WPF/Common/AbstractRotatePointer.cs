using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;


namespace Tech4WPF.Common
{
    /// <summary>
    /// Simulates abstract class. Is not abstract declared because of designer VS2010
    /// Do not create its instance.
    /// This class implements common interface for rotate pointers.
    /// </summary>
    internal class AbstractRotatePointer : UserControl, IRotatePointer, INotifyPropertyChanged
    {
        #region Constructors

        /// <summary>
        /// Do not create a instance of this <see cref="AbstractRotatePointer"/> class.
        /// Use derived types.
        /// </summary>
        public AbstractRotatePointer() { }

        #endregion Constructors

        #region Events

        /// <summary>
        /// Occurs when a property Angle changes its value.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion Events

        #region Properties

        public static readonly DependencyProperty AngleProperty = DependencyProperty.Register(
            "Angle", typeof(double), typeof(AbstractRotatePointer),
            new PropertyMetadata(0.0, angleChangedCallback, coerceAngleCallback));
        /// <summary>
        /// Gets or sets the angle from origin in
        /// clockwise direction.
        /// </summary>
        /// <value>
        /// The angle in degrees.
        /// </value>
        public double Angle
        {
            get { return (double)GetValue(AngleProperty); }
            set { SetValue(AngleProperty, value); }
        }

        public static readonly DependencyProperty OriginAngleProperty = DependencyProperty.Register(
            "OriginAngle", typeof(double), typeof(AbstractRotatePointer),
            new PropertyMetadata(0.0, null, coerceAngleCallback));
        /// <summary>
        /// Gets or sets the origin angle from sinus = 1
        /// representating begining of the scale.
        /// </summary>
        /// <value>
        /// The origin angle in degrees.
        /// </value>
        public double OriginAngle
        {
            get { return (double)GetValue(OriginAngleProperty); }
            set { SetValue(OriginAngleProperty, value); }
        }

        public static readonly DependencyProperty PointerFillProperty = DependencyProperty.Register(
            "PointerFill", typeof(Brush), typeof(AbstractRotatePointer),
            new PropertyMetadata(Brushes.Red));
        /// <summary>
        /// Gets or sets the pointer fill.
        /// </summary>
        /// <value>
        /// The pointer fill.
        /// </value>
        public Brush PointerFill
        {
            get { return (Brush)GetValue(PointerFillProperty); }
            set { SetValue(PointerFillProperty, value); }
        }

        public static readonly DependencyProperty PointerStrokeProperty = DependencyProperty.Register(
            "PointerStroke", typeof(Brush), typeof(AbstractRotatePointer),
            new PropertyMetadata(Brushes.Black));
        /// <summary>
        /// Gets or sets the pointer stroke.
        /// </summary>
        /// <value>
        /// The pointer stroke.
        /// </value>
        public Brush PointerStroke
        {
            get { return (Brush)GetValue(PointerStrokeProperty); }
            set { SetValue(PointerStrokeProperty, value); }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Invalidates render, forces update according Angle property. 
        /// </summary>
        public void Invalidate()
        {
            updateRenderAngle();
        }

        /// <summary>
        /// Updates render angle of the pointer and fires PropertyChanged event.
        /// </summary>
        /// <param name="d">The <see cref="System.Windows.DependencyObject"/> instance.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void angleChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            AbstractRotatePointer rotatePointer = (AbstractRotatePointer)d;
            rotatePointer.updateRenderAngle();
            rotatePointer.firePropertyChanged(e.Property.Name);
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
        /// Updates the render angle of the pointer. Have to be overridden.
        /// </summary>
        protected virtual void updateRenderAngle()
        {
            throw new NotImplementedException("updateRenderAngle() has to be overriden");
        }

        /// <summary>
        /// Fires the property changed event.
        /// </summary>
        /// <param name="property">The string representating name of changed property</param>
        protected void firePropertyChanged(string property)
        {
            //Maintaining the temporary copy of event to avoid race condition 
            PropertyChangedEventHandler propertyChanged = PropertyChanged;
            if (propertyChanged != null)
            {
                propertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

        #endregion Mehods
    }
}
