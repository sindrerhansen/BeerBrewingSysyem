using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Tech4WPF.Common
{
    #region Delegates

    /// <summary>
    /// Represents the method that handle try to change angle.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="Tech4WPF.Common.TryAngleChangeEventArgs"/> instance containing the event data.</param>
    internal delegate void TryAngleChangeHandler(UserControl sender, TryAngleChangeEventArgs e);

    #endregion Delegates

    /// <summary>
    /// This class provides trying change the angle.
    /// </summary>
    internal class TryAngleChangeEventArgs : RoutedEventArgs
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TryAngleChangeEventArgs"/> class.
        /// </summary>
        /// <param name="newAngle">The new angle in degrees.</param>
        /// <param name="oldAngle">The old angle in degrees.</param>
        /// <param name="isWheelScrolling"><c>true</c> indicating wheel scrolling.</param>
        public TryAngleChangeEventArgs(double newAngle, double oldAngle, bool isWheelScrolling)
            : base()
        {
            this.NewAngle = newAngle;
            this.OldAngle = oldAngle;
            this.IsWheelScrolling = isWheelScrolling;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets the new angle trying to set.
        /// </summary>
        public double NewAngle
        {
            private set;
            get;
        }

        /// <summary>
        /// Gets the old angle before change.
        /// </summary>
        public double OldAngle
        {
            private set;
            get;
        }

        /// <summary>
        /// Gets a bool value indicating whether is wheel scrolling.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if is wheel scrolling; otherwise, <c>false</c>.
        /// </value>
        public bool IsWheelScrolling
        {
            private set;
            get;
        }

        #endregion Properties
    }
}
