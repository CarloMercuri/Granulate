using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GranulateLibrary
{
    public static class ActionsManager
    {
        /// <summary>
        /// Undoes the next action in the history
        /// </summary>
        public static void UndoSequential()
        {
            Console.WriteLine(ProjectManager.openProjects[ProjectManager.CurrentProject].ActionIndex);
            // Checks to see if we have some actions saved up overall etc
            if(ProjectManager.openProjects[ProjectManager.CurrentProject].ActionIndex >= GeneralSettings.MaxHistoryCount 
                || ProjectManager.openProjects[ProjectManager.CurrentProject].ActionIndex >=
                ProjectManager.openProjects[ProjectManager.CurrentProject].ActionHistory.Count
                || ProjectManager.openProjects[ProjectManager.CurrentProject].ActionIndex < 0)
            {
                return;
            }

            // Call the undo action function in the specific action
            UndoAction(ProjectManager.openProjects[ProjectManager.CurrentProject].GetLastAction());

            // Go up by one
            ProjectManager.openProjects[ProjectManager.CurrentProject].ActionIndex += 1;
        }

        /// <summary>
        /// Undoes an action
        /// </summary>
        /// <param name="action"></param>
        private static void UndoAction(IActionDefiner action)
        {
            action.RevertAction();
        }

        /// <summary>
        /// Gives the last action performed to the system to be handled and added to the list
        /// </summary>
        /// <param name="action"></param>
        public static void HandleLastAction(IActionDefiner action)
        {
            // The actions history is saved into the current open project
            ProjectManager.openProjects[ProjectManager.CurrentProject].InsertLastAction(action);

        }


    }
} 
