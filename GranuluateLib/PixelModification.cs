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
        public int bitmapID;
        public Vec2 pixelLoc;
        public Color oldColor;
        public Color newColor;

        public PixelModification(Vec2 loc, Color oldColor, Color newColor, int bitmapID)
        {
            this.pixelLoc = loc;
            this.oldColor = oldColor;
            this.newColor = newColor;
            this.bitmapID = bitmapID;
        }
    }
}
