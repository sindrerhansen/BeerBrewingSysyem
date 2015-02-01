using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using Tech4WPF.Common;


namespace Tech4WPF.KnobControl
{
    /// <summary>
    /// Interaction logic for KnobPart.xaml
    /// </summary>
    internal partial class KnobPart : AbstractRotatePointer
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="KnobPart"/> class.
        /// </summary>
        public KnobPart()
        {
            InitializeComponent();

            this.arrow.RenderTransform = this.rotateTransform;
        }

        /// <summary>
        /// Initializes the <see cref="KnobPart"/> class static member data.
        /// </summary>
        static KnobPart()
        {
            AbstractRotatePointer.PointerFillProperty.OverrideMetadata(
                typeof(KnobPart), new PropertyMetadata(Brushes.GreenYellow));
        }

        #endregion Constructors

        #region Fields

        private Point arrowCenterPoint;
        private bool isMouseRotating;
        private double mouseDownAngle;
        private Vector mouseDownVector;
        private RotateTransform rotateTransform = new RotateTransform();

        #endregion Fields

        #region Constants

        private const double WHEEL_DIVIDER = 40;

        #endregion Constants

        #region Events

        public event TryAngleChangeHandler TryAngleChange;
        
        #endregion Events

        #region Methods

        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.Input.Mouse.MouseWheel"/> attached 
        /// event reaches an element in its route that is derived from this class. 
        /// Copute new angle and fires try angle change event
        /// </summary>
        /// <param name="e">
        /// The <see cref="T:System.Windows.Input.MouseWheelEventArgs"/> that contains 
        /// the event data.
        /// </param>
        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            double newAngle = this.Angle + (e.Delta / WHEEL_DIVIDER);
            fireTryAngleChange(newAngle, true);
            base.OnMouseWheel(e);
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.Input.Mouse.LostMouseCapture"/> attached 
        /// event reaches an element in its route that is derived from this class. 
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseEventArgs"/> that contains event data.</param>
        protected override void OnLostMouseCapture(MouseEventArgs e)
        {
            this.isMouseRotating = false;
            base.OnLostMouseCapture(e);
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.Input.Mouse.MouseDown"/> attached 
        /// event reaches an element in its route that is derived from this class. 
        /// </summary>
        /// <param name="e">
        /// The <see cref="T:System.Windows.Input.MouseButtonEventArgs"/> that contains 
        /// the event data. This event data reports details about the mouse button that was pressed 
        /// and the handled state.
        /// </param>
        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            var mouseDownPoint = e.GetPosition(this);
            this.mouseDownVector = mouseDownPoint - arrowCenterPoint;
            this.mouseDownAngle = this.Angle;
            e.MouseDevice.Capture(this);
            this.isMouseRotating = true;
            base.OnMouseDown(e);
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.Input.Mouse.MouseMove"/> attached 
        /// event reaches an element in its route that is derived from this class. 
        /// </summary>
        /// <param name="e">
        /// The <see cref="T:System.Windows.Input.MouseEventArgs"/> that contains 
        /// the event data.
        /// </param>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (isMouseRotating)
            {
                Point cursorPosition = e.GetPosition(this);
                Vector currentVector = cursorPosition - arrowCenterPoint;
                double newAngle = Vector.AngleBetween(mouseDownVector, currentVector) + mouseDownAngle;
                fireTryAngleChange(newAngle, false);
            }
            base.OnMouseMove(e);
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.Input.Mouse.MouseUp"/> routed event 
        /// reaches an element in its route that is derived from this class. 
        /// </summary>
        /// <param name="e">
        /// The <see cref="T:System.Windows.Input.MouseButtonEventArgs"/> that contains 
        /// the event data. The event data reports that the mouse button was released.
        /// </param>
        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            if (isMouseRotating)
            {
                e.MouseDevice.Capture(null);
                this.isMouseRotating = false;
            }
            base.OnMouseUp(e);
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.FrameworkElement.SizeChanged"/> event, 
        /// using the specified information as part of the eventual event data.
        /// </summary>
        /// <param name="sizeInfo">Details of the old and new size involved in the change.</param>
        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            this.arrowCenterPoint = new Point(ActualWidth / 2, ActualHeight / 2);
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Controls.Control.MouseDoubleClick"/> routed event.
        /// </summary>
        /// <param name="e">The event data.</param>
        protected override void OnMouseDoubleClick(MouseButtonEventArgs e)
        {
            var mouseClickPoint = e.GetPosition(this);
            var mouseClickVector = mouseClickPoint - arrowCenterPoint;
            var zeroVector = arrowCenterPoint - new Point(this.arrowCenterPoint.X, this.arrowCenterPoint.Y + 1);
            double newAngle = Vector.AngleBetween(zeroVector, mouseClickVector) - this.OriginAngle;
            fireTryAngleChange(newAngle, false);
            base.OnMouseDoubleClick(e);
        }

        /// <summary>
        /// Updates the render angle of the knob.
        /// </summary>
        protected override void updateRenderAngle()
        {
            this.rotateTransform.Angle = this.Angle + this.OriginAngle;
        }

        /// <summary>
        /// Fires the try angle change event.
        /// </summary>
        /// <param name="newAngle">The new angle.</param>
        /// <param name="isWheelScrolling"><c>true</c> when is wheel scrolling.</param>
        private void fireTryAngleChange(double newAngle, bool isWheelScrolling)
        {
            //Maintaining the temporary copy of event to avoid race condition 
            TryAngleChangeHandler tryAngleChange = TryAngleChange;
            if (tryAngleChange != null)
            {
                TryAngleChangeEventArgs angleChangedEventArgs = 
                    new TryAngleChangeEventArgs(newAngle, this.Angle, isWheelScrolling);
                tryAngleChange(this, angleChangedEventArgs);
            }
        }

        #endregion Methods
    }
}
