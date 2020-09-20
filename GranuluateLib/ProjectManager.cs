using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GranulateLibrary
{
    public static class ProjectManager
    {

        public static List<ProjectData> openProjects = new List<ProjectData>();
        public static int CurrentProject { get; set; }
        public static Color Color_Main { get; set; }
        public static Color Color_Secondary { get; set; }


        public static int SelectedBitmap
        {
            get { return GetSelectedBitmap(); }
            set { _selectedBitmap = value; }
        }

        private static int _selectedBitmap;

       
        // If it's a single sprite object, the bitmaps list is always gonna have only 1 bitmap
        // So to avoid any possible bug, we always return 0 for that type
        private static int GetSelectedBitmap()
        {
            if(openProjects[CurrentProject].projectType == ProjectType.SingleSprite)
            {
                return 0;
            }
            else
            {
                return _selectedBitmap;
            }
        }

        /// <summary>
        /// Creates a new black project with given size and type
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="_projectType"></param>
        public static void CreateNewProject(int width, int height, ProjectType _projectType)
        {
            // Add new project to the list of open ones
            openProjects.Add(new ProjectData(width, height, _projectType));
            // Set the current project to the last entry
            CurrentProject = openProjects.Count - 1;
            // Reset the colors to default black and white ones
            Color_Main = Color.Black;
            Color_Secondary = Color.White;
            // Set the selected bitmap to 0
            SelectedBitmap = 0;
        }

        /// <summary>
        /// Add a new empty image to the project, with the project's image sizes
        /// </summary>
        public static void AddNewProjectImage()
        {
            
            Bitmap bmp = new Bitmap(openProjects[CurrentProject].ImageWidth, openProjects[CurrentProject].ImageHeight);

            // Default image is just transparent
            ImageEditing.FillRectangle(Color.Transparent, new Rectangle(0, 0, bmp.Width, bmp.Height), ref bmp);

            // Add it to the open project's bitmaps list
            openProjects[CurrentProject].bitmaps.Add(bmp);
        }
    }
    
}
