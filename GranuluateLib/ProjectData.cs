using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;


namespace GranulateLibrary
{
    public class ProjectData
    {
        /// <summary>
        /// List of the bitmaps contained in this project
        /// </summary>
        public List<Bitmap> Bitmaps = new List<Bitmap>();
        /// <summary>
        /// The ID of the bitmap in focus
        /// </summary>
        public int CurrentBitmapIndex { get; set; }
        /// <summary>
        /// Standard image width of the project
        /// </summary>
        public int ImageWidth { get; set; }
        /// <summary>
        /// Standard image height of the project
        /// </summary>
        public int ImageHeight { get; set; }
        /// <summary>
        /// The Type of project
        /// </summary>
        public ProjectType ProjectType { get; set; }
        /// <summary>
        /// CTRL+Z History
        /// </summary>
        public List<IActionDefiner> ActionHistory = new List<IActionDefiner>();
        /// <summary>
        /// The current index in the actionHistory list
        /// </summary>
        private int _ActionIndex;

        public int ActionIndex
        {
            get { return _ActionIndex; }
            set { SetActionIndex(value); }

        }

        public ProjectData(int _imageWidth, int _imageHeight, ProjectType _projectType)
        {
            CurrentBitmapIndex = 0;
            ProjectType = _projectType;
            ImageWidth = _imageWidth;
            ImageHeight = _imageHeight;
            Bitmap baseBitmap = new Bitmap(_imageWidth, _imageHeight);
            ImageEditing.FillRectangle(Color.Transparent, new Rectangle(0, 0, _imageWidth, _imageHeight), ref baseBitmap);
            Bitmaps.Add(baseBitmap);
        }

        /// <summary>
        /// Returns the first occurrence of an action
        /// </summary>
        /// <returns></returns>
        public IActionDefiner GetLastAction()
        {
            if(_ActionIndex > ActionHistory.Count || _ActionIndex < 0)
            {
                return null;
            }
            else
            {
                return ActionHistory[_ActionIndex];
            }


        }


        /// <summary>
        /// Changes the current index of the actions list
        /// </summary>
        /// <param name="value"></param>
        private void SetActionIndex(int value)
        {
            _ActionIndex = value;
            if (_ActionIndex < 0) 
                _ActionIndex = 0;

            if (_ActionIndex > GeneralSettings.MaxHistoryCount)
                _ActionIndex = GeneralSettings.MaxHistoryCount;

        }

        

        

        /// <summary>
        /// Inserts a new action into the list.
        /// </summary>
        /// <param name="_lastAction"></param>
        public void InsertLastAction(IActionDefiner _lastAction)
        {
            
            // If the current selected action is not at the top, we need to cut the ones above it,
            // and insert the new action at the top
            if(ActionIndex != 0)
            {
                List<IActionDefiner> tempList = new List<IActionDefiner>();

                for (int i = ActionIndex; i < ActionHistory.Count; i++)
                {
                    tempList.Add(ActionHistory[i]);
                }

                ActionHistory = tempList;

            }
            
            // At this point it's safe to insert the action on top
            ActionHistory.Insert(0, _lastAction);

            // Keep the list at the max count
            if(ActionHistory.Count > GeneralSettings.MaxHistoryCount)
            {
                ActionHistory.RemoveAt(ActionHistory.Count - 1);
            }

            // Reselect the top action
            ActionIndex = 0;
        }

        

    }
}
