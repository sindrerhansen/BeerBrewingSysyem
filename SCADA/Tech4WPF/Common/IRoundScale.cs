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
    /// Prescription for round scales
    /// </summary>
    internal interface IRoundScale
    {
        #region Events

        /// <summary>
        /// Occurs when scale changes values, angles or stamps.
        /// </summary>
        event EventHandler ScaleChanged;

        #endregion Events

        #region Properties

        /// <summary>
        /// Gets or sets the Minimum value of the scale.
        /// </summary>
        /// <value>
        /// The minimum value.
        /// </value>
        double Minimum { get; set; }

        /// <summary>
        /// Gets or sets the Maximum value of the scale.
        /// </summary>
        /// <value>
        /// The maximum value.
        /// </value>
        double Maximum { get; set; }

        /// <summary>
        /// Gets the absolute value of range (Maximum - Minimum).
        /// </summary>
        double Range { get; }

        /// <summary>
        /// Gets or sets the start angle in degrees for scale. Angle begins at the top.
        /// Begining point is at the top. Angle is unlimited and can be negative.
        /// It will be always converted to positive angle up to 360. 
        /// NaN and Infinity values will be replaced with 0.0
        /// </summary>
        /// <value>
        /// The start angle in degrees.
        /// </value>
        double StartAngle { get; set; }

        /// <summary>
        /// Gets or sets the end angle in degrees for scale. Angle begins at the top.
        /// Begining point is at the top. Angle is unlimited and can be negative.
        /// It will be always converted to positive angle up to 360. 
        /// NaN and Infinity values will be replaced with 0.0
        /// </summary>
        /// <value>
        /// The end angle in degrees.
        /// </value>
        double EndAngle { get; set; }

        /// <summary>
        /// Gets the angle range of scale in degrees (EndAngle - StarAngle).
        /// </summary>
        double AngleRange { get; }

        /// <summary>
        /// Gets or sets the format string for scale values.
        /// </summary>
        string FormatString { get; set; }

        /// <summary>
        /// Gets or sets the minor step.
        /// </summary>
        /// <value>
        /// The minor step.
        /// </value>
        double MinorStep { get; set; }

        /// <summary>
        /// Gets or sets the major step.
        /// </summary>
        /// <value>
        /// The major step.
        /// </value>
        double MajorStep { get; set; }

        /// <summary>
        /// Gets or sets the minor stamp stroke.
        /// </summary>
        /// <value>
        /// The minor stamp stroke.
        /// </value>
        Brush MinorStampStroke { get; set; }

        /// <summary>
        /// Gets or sets the major stamp stroke.
        /// </summary>
        /// <value>
        /// The major stamp stroke.
        /// </value>
        Brush MajorStampStroke { get; set; }

        /// <summary>
        /// Gets or sets the scale stroke.
        /// </summary>
        /// <value>
        /// The scale stroke.
        /// </value>
        Brush ScaleStroke { get; set; }

        /// <summary>
        /// Gets or sets the value foreground.
        /// </summary>
        /// <value>
        /// The value foreground.
        /// </value>
        Brush ValueForeground { get; set; }

        /// <summary>
        /// Gets or sets a value indicating horizontal position of the scale values.
        /// </summary>
        /// <value>
        ///   <c>true</c> if values of the scale are horizontally; otherwise, <c>false</c>.
        /// </value>
        bool ValuesHorizontally { get; set; }

        #endregion Properties
    }
}
