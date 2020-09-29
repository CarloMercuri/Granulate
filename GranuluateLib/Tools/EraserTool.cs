using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace GranulateLibrary
{
    class EraserTool : IToolProperties
    {


        public string ToolName { get; set; }
        public int ToolSize { get; set; }
        public Image NormalButtonImage { get; set; }
        public Image SelectedButtonImage { get; set; }


        private List<PixelModification> modifiedPixels = new List<PixelModification>();
        private ActionPixelModification action;
        private List<Vec2> absoluteModifiedPixels = new List<Vec2>();

        /* 
        *  Need to make this better. Basically the idea is that instead of
        *  directly modifying the pixels, each time we modify some we create
        *  a Pixel Modification List, which we pass on to be handled in ImageEditing,
        *  which does the actual modification (especially useful when modifying multiple pixels
        *  at a time).
        *  
        *  At the same time, we're creating an Action whenever we have a New Action (When we
        *  press the mouse button down, we have a new action, untill we release the mouse).
        *  
        *  We keep adding the modified pixels to this action, untill a new action is detected,
        *  at that point we send the current Action to ActionsManager to be handled (this is 
        *  for ctrl z history basically), and we create a new Action and start over.
        *  
        */
        public void UseTool_MouseUp(int currentImage)
        {
            if (action != null)
            {
                ActionsManager.HandleLastAction(action);
                action = null;
            }
        }

        public void UseTool(Vec2 currentPoint, Vec2 lastPoint, int currentImage, bool newAction)
        {
            List<Vec2> points = new List<Vec2>();

            if (newAction)
            {
                absoluteModifiedPixels = new List<Vec2>();
            }

            modifiedPixels = new List<PixelModification>();

            if (currentPoint != lastPoint)
            {
                points = RayCast2D.RayCast(new Vec2(lastPoint.x, lastPoint.y), new Vec2(currentPoint.x, currentPoint.y));
            }
            else
            {
                if (!newAction)
                {
                    return;
                }


                points.Add(currentPoint);
            }


            Size imgSize = ProjectManager.openProjects[ProjectManager.CurrentProject].Bitmaps[currentImage].Size;

            Vec2 tempPoint;
            int size = ToolSize - 1;

            foreach (Vec2 point in points)
            {

                for (int _x = point.x - size; _x <= point.x + size; _x++)
                {
                    for (int _y = point.y - size; _y <= point.y + size; _y++)
                    {
                        tempPoint = new Vec2(_x, _y);

                        if (tempPoint.x >= 0 && tempPoint.x < imgSize.Width && tempPoint.y >= 0 && tempPoint.y < imgSize.Height)
                        {
                            // Avoid modifying the same pixel twice. Mostly for the action history sake
                            if (absoluteModifiedPixels.Contains(tempPoint))
                            {
                                continue;
                            }

                            absoluteModifiedPixels.Add(tempPoint);

                            modifiedPixels.Add(new PixelModification(tempPoint, ProjectManager.openProjects[
                                ProjectManager.CurrentProject].Bitmaps[currentImage].GetPixel(tempPoint.x, tempPoint.y),
                                Color.Transparent, currentImage));
                        }
                    }
                }
            }

            ImageEditing.ModifyPixels(modifiedPixels, ProjectManager.CurrentProject, currentImage);

            if (newAction)
            {
                action = new ActionPixelModification();
                action.AffectedBitmapIndex = ProjectManager.SelectedBitmap;

            }

            foreach (PixelModification pixMod in modifiedPixels)
            {

                action.pixelsList.Add(pixMod);
            }
        }

        private void ModifySinglePixel()
        {

        }

    }
}
