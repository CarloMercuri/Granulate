using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GranulateLibrary
{
    public class ActionPixelModification : IActionDefiner
    {
        /// <summary>
        /// Type of action
        /// </summary>
        public ActionType ActionCategory { get; set; }

        /// <summary>
        /// ID of the affected Bitmap
        /// </summary>
        public int AffectedBitmapIndex { get; set; }

        /// <summary>
        /// List of PixelModifications in this action
        /// </summary>
        public List<PixelModification> pixelsList = new List<PixelModification>();

        /// <summary>
        /// Calls the UndoPixelModification function
        /// </summary>
        public void RevertAction()
        {
            ImageEditing.UndoPixelModification(this);
        }
    }
}
