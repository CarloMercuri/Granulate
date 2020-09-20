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
        public ActionType ActionCategory { get; set; }

        // Specific
        public int AffectedBitmapIndex { get; set; }
        public List<PixelModification> pixelsList = new List<PixelModification>();

        public void RevertAction()
        {
            ImageEditing.UndoPixelModification(this);
        }
    }
}
