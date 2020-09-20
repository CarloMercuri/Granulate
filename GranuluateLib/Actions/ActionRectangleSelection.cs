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
        public ActionType ActionCategory { get; set; }

        // Specifics
        public Rectangle rect;


        public void RevertAction()
        {

        }
    }
}
