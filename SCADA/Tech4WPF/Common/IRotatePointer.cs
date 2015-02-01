using System;
using System.Windows.Media;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;


namespace Tech4WPF.Common
{
    /// <summary>
    /// Prescription for rotate pointers interface
    /// </summary>
    internal interface IRotatePointer
    {
        #region Properties

        /// <summary>
        /// Gets or sets the angle from origin in 
        /// clockwise direction.
        /// </summary>
        /// <value>
        /// The angle in degrees.
        /// </value>
        double Angle { get; set; }

        /// <summary>
        /// Gets or sets the origin angle from sinus = 1
        /// representating begining of the scale. 
        /// </summary>
        /// <value>
        /// The origin angle in degrees.
        /// </value>
        double OriginAngle { get; set; }

        /// <summary>
        /// Gets or sets the pointer stroke.
        /// </summary>
        /// <value>
        /// The pointer stroke.
        /// </value>
        Brush PointerStroke { get; set; }

        /// <summary>
        /// Gets or sets the pointer fill.
        /// </summary>
        /// <value>
        /// The pointer fill.
        /// </value>
        Brush PointerFill { get; set; }

        #endregion Properties
    }
}
