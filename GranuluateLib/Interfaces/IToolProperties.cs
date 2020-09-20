using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace GranulateLibrary
{
    public interface IToolProperties
    {

        string ToolName { get; set; }
        int ToolSize { get; set; }
        Image NormalButtonImage { get; set; }
        Image SelectedButtonImage { get; set; }
        void UseTool(Vec2 currentPoint, Vec2 lastPoint, int currentImage, bool newAction);
        void UseTool_MouseUp(int currentImage);
    }
}
