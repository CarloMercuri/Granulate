using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GranulateLibrary
{
     [DebuggerDisplay("{pixelLoc.x}")]
    public struct PixelModification
    {
        /// <summary>
        /// The ID of the bitmap where the pixels are located
        /// </summary>
        public int bitmapID { get; set; }
        /// <summary>
        /// Location of the pixel
        /// </summary>
        public Vec2 pixelLoc { get; set; }

        public Color oldColor { get; set; }
        public Color newColor { get; set; }

        public PixelModification(Vec2 loc, Color oldColor, Color newColor, int bitmapID)
        {
            this.pixelLoc = loc;
            this.oldColor = oldColor;
            this.newColor = newColor;
            this.bitmapID = bitmapID;
        }
    }
}
