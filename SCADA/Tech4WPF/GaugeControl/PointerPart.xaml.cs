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


namespace Tech4WPF.GaugeControl
{
    /// <summary>
    /// Interaction logic for PointerPart.xaml
    /// </summary>
    internal partial class PointerPart : AbstractRotatePointer, IRotatePointer
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PointerPart"/> class.
        /// </summary>
        public PointerPart()
        {
            InitializeComponent();

            this.arrow.RenderTransform = this.rotateTransform;
        }

        #endregion Constructors

        #region Fields

        /// <summary>
        /// Rotate transform represents render transform for pointer
        /// </summary>
        private RotateTransform rotateTransform = new RotateTransform();

        #endregion Fields

        #region Methods

        /// <summary>
        /// Updates the render transformation angle of the pointer.
        /// </summary>
        protected override void updateRenderAngle()
        {
            this.rotateTransform.Angle = this.Angle + this.OriginAngle;
        }

        #endregion Methods
    }
}
