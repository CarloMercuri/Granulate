using GranulateLibrary;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GranulateMainForm
{
    public partial class GranulateMF : Form
    {
        public GranulateMF()
        {
            InitializeComponent();

            InitializeInitialStartup();
            
        }

        private void InitializeInitialStartup()
        {
            ToolsManager.LoadToolsList();

            GUI.mainForm = this;
            GUI.panel_Bottom = panel_Bottom;
            GUI.panel_ProjectImageList = panel_ProjectImageList;
            GUI.InitializeGUI();
        }

        private void MF_KeyDown(object sender, KeyEventArgs e)
        {
            GUI.MainForm_KeyDown(sender, e);
        }


    }
}
