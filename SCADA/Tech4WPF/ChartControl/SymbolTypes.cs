using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tech4WPF.ChartControl
{
    /// <summary>
    /// Enumeration of symbol types
    /// </summary>
    public enum SymbolType
    {
        /// <summary>
        /// No symbol
        /// </summary>
        None,

        /// <summary>
        /// Bar from Y=0 to point
        /// </summary>
        Bar,

        /// <summary>
        /// Box
        /// </summary>
        Box,

        /// <summary>
        /// Circle
        /// </summary>
        Circle,

        /// <summary>
        /// X Cross
        /// </summary>
        Cross,

        /// <summary>
        /// Diamond
        /// </summary>
        Diamond,

        /// <summary>
        /// Monochromatic circle
        /// </summary>
        Dot,

        /// <summary>
        /// Triangle
        /// </summary>
        Triangle,

        /// <summary>
        /// Inverted triangle
        /// </summary>
        InvertedTriangle,

        /// <summary>
        /// Monochromatic triangle
        /// </summary>
        OpenTriangle,

        /// <summary>
        /// Monochromatic inverted triangle
        /// </summary>
        OpenInvertedTriangle,

        /// <summary>
        /// Monochromatic diamond
        /// </summary>
        OpenDiamond,

        /// <summary>
        /// Monochromatic square (box)
        /// </summary>
        Square,

        /// <summary>
        /// Lines star
        /// </summary>
        Star,

        /// <summary>
        /// Plus
        /// </summary>
        Plus
    }
}
