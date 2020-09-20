using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace GranulateLibrary
{
    public static class ToolsManager
    {
        public static int SelectedTool { get; set; }
        public static List<IToolProperties> toolsList = new List<IToolProperties>();


        public static void UseTool(Vec2 currentPixel, Vec2 lastPixel, bool newAction)
        {
            //IActionDefiner action = toolsList[SelectedTool].UseTool(currentPixel, lastPixel, ProjectManager.SelectedBitmap, mouseButton);
            //ProjectManager.openProjects[ProjectManager.CurrentProject].InsertLastAction(action);
            //return action;

            toolsList[SelectedTool].UseTool(currentPixel, lastPixel, ProjectManager.SelectedBitmap, newAction);
        }

        public static void UseTool_MouseUp()
        {
            toolsList[SelectedTool].UseTool_MouseUp(ProjectManager.SelectedBitmap);
        }

       

        public static void LoadToolsList()
        {
            toolsList.Add(new PencilTool());
            toolsList[0].ToolSize = 1;
            //toolsList[0].ToolButton = BrushTool;
            toolsList[0].NormalButtonImage = Image.FromFile(Path.Combine(Environment.CurrentDirectory, 
                "Textures/Icons/Tools_temp/Brush_Tool.png"));
            toolsList[0].SelectedButtonImage = Image.FromFile(Path.Combine(Environment.CurrentDirectory, 
                "Textures/Icons/Tools_temp/Brush_Tool.png"));
            //toolsList[0].SelectedButtonImage = Image.FromFile(Path.Combine(Environment.CurrentDirectory, 
            //    "Textures/Icons/Brush_Tool_Selected.png"));
           // toolsList[0].ToolCursor = new Cursor(Path.Combine(Environment.CurrentDirectory, 
            //    "Textures/Icons/Cursors/Invis_Cursor.cur"));
            //toolsList[0].ToolButton.Image = toolsList[0].NormalButtonImage;

            toolsList.Add(new EraserTool());
            toolsList[1].ToolSize = 3;
            //toolsList[1].ToolButton = Eraser_Tool;
            toolsList[1].NormalButtonImage = Image.FromFile(Path.Combine(Environment.CurrentDirectory, "Textures/Icons/Tools_temp/Eraser_Tool_35_34.png"));
            toolsList[1].SelectedButtonImage = Image.FromFile(Path.Combine(Environment.CurrentDirectory, "Textures/Icons/Tools_temp/Eraser_Tool_35_34.png"));
           // toolsList[1].ToolCursor = new Cursor(Path.Combine(Environment.CurrentDirectory, "Textures/Icons/Cursors/Invis_Cursor.cur"));
            //toolsList[1].ToolButton.Image = toolsList[1].NormalButtonImage;
        }
    }
}
