using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace GranulateLibrary
{
    class PixelData
    {
        /// <summary>
        /// Location of the pixel
        /// </summary>
        public Vec2 pixelLoc { get; set; }
        /// <summary>
        /// The color of the pixel
        /// </summary>
        public Color pixelColor { get; set; }
    }
}
