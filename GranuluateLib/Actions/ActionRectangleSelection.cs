using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GranulateLibrary
{
    class ActionRectangleSelection : IActionDefiner
    {
        /// <summary>
        /// Type of the action
        /// </summary>
        public ActionType ActionCategory { get; set; }

        // Specifics
        public Rectangle Rect { get; set; }


        public void RevertAction()
        {

        }
    }
}
