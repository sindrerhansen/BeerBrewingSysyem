using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tech4WPF.ChartControl
{
    /// <summary>
    /// Enumeration of possible draw types
    /// </summary>
    public enum DrawType
    {
        /// <summary>
        /// No lines 
        /// </summary>
        None,

        /// <summary>
        /// Polyline
        /// </summary>
        Polyline,

        /// <summary>
        /// Rectangular steps
        /// </summary>
        Steps,


        /// <summary>
        /// Impulses from Y=0 to points
        /// </summary>
        Impulses
    }
}
