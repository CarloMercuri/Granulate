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
        public Bitmap backgroundBitmap;
        public List<Bitmap> bitmaps = new List<Bitmap>();
        public int currentBitmapIndex = 0;
        public int ImageWidth { get; set; }
        public int ImageHeight { get; set; }
        public ProjectType projectType;
        public List<IActionDefiner> actionHistory = new List<IActionDefiner>();
        private int _ActionIndex;

        public int ActionIndex
        {
            get { return _ActionIndex; }
            set { SetActionIndex(value); }

        }

        /// <summary>
        /// Returns the first occurrence of an action
        /// </summary>
        /// <returns></returns>
        public IActionDefiner GetLastAction()
        {
            if(_ActionIndex > actionHistory.Count || _ActionIndex < 0)
            {
                return null;
            }
            else
            {
                return actionHistory[_ActionIndex];
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

        

        public ProjectData(int _imageWidth, int _imageHeight, ProjectType _projectType)
        {
            projectType = _projectType;
            ImageWidth = _imageWidth;
            ImageHeight = _imageHeight;
            Bitmap baseBitmap = new Bitmap(_imageWidth, _imageHeight);
            ImageEditing.FillRectangle(Color.Transparent, new Rectangle(0, 0, _imageWidth, _imageHeight), ref baseBitmap);
            bitmaps.Add(baseBitmap);
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

                for (int i = ActionIndex; i < actionHistory.Count; i++)
                {
                    tempList.Add(actionHistory[i]);
                }

                actionHistory = tempList;

            }
            
            // At this point it's safe to insert the action on top
            actionHistory.Insert(0, _lastAction);

            // Keep the list at the max count
            if(actionHistory.Count > GeneralSettings.MaxHistoryCount)
            {
                actionHistory.RemoveAt(actionHistory.Count - 1);
            }

            // Reselect the top action
            ActionIndex = 0;
        }

        

    }
}
