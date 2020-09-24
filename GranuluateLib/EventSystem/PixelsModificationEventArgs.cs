using System;
using System.Collections.Generic;
using System.Text;

namespace GranulateLibrary.EventSystem
{
    public class PixelsModificationEventArgs : EventArgs
    {
        /// <summary>
        /// The list of modified pixels
        /// </summary>
        public List<PixelModification> pixelModificationList { get; private set; }

        /// <summary>
        /// The bitmap ID the modifications refer to
        /// </summary>
        public int BitmapID { get; private set; }

        /// <summary>
        /// If true, signals the change goes from new color to old color, and vice versa
        /// </summary>
        public  bool IsReverse { get; private set; }


        public PixelsModificationEventArgs(List<PixelModification> pixelModList, int _bitmapID, bool _isReverse)
        {
            pixelModificationList = pixelModList;
            BitmapID = _bitmapID;
            IsReverse = _isReverse;
        }
    }
}
