using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace Tech4WPF
{
    /// <summary>
    /// Static class providing public utilities used in Tech4WPF library
    /// </summary>
    public static class TechUtils
    {
        #region Delegates

        /// <summary>
        /// Delegate for math rounding mode Round, Ceil and Floor
        /// </summary>
        /// <param name="d">The value for rounding.</param>
        /// <returns>Rounded value</returns>
        public delegate double MathRoundingMode(double d);

        #endregion Delegates

        #region Methods

        /// <summary>
        /// Converts positive or negative arbitrary angle to it's equivalent in interval &lt;0, 360).
        /// </summary>
        /// <param name="angleInDegrees">Angle in degrees</param>
        /// <returns>Angle in degrees in interval &lt;0, 360)</returns>
        /// <example>
        /// For angle -90 returns 270
        /// For angle 722 returns 2
        /// </example>
        public static double AngleToPositive360(double angleInDegrees)
        {
            angleInDegrees %= 360;
            if (angleInDegrees < 0)
            {
                angleInDegrees += 360;
            }
            return angleInDegrees;
        }

        /// <summary>
        /// When value is Infinity or NaN value, returns zero; otherwise returns value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>Non Infinity/NaN value</returns>
        public static double ReplaceInfinityAndNaN(double value)
        {
            if (double.IsNaN(value) || double.IsInfinity(value))
            {
                value = 0.0;
            }
            return value;
        }

        /// <summary>
        /// Convets degrees to radians.
        /// </summary>
        /// <param name="angleInDegrees">The angle in degrees.</param>
        /// <returns>The angle in radians</returns>
        public static double DegreeToRadian(double angleInDegrees)
        {
            return Math.PI * angleInDegrees / 180.0;
        }


        /// <summary>
        /// Converts radians to degrees.
        /// </summary>
        /// <param name="angleInRadians">The angle in radians.</param>
        /// <returns>The angle in degrees</returns>
        public static double RadianToDegree(double angleInRadians)
        {
            return angleInRadians * (180.0 / Math.PI);
        }
        #endregion Methods
    }
}
