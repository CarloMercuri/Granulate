namespace GranulateMainForm
{
    partial class GranulateMF
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.panel_Bottom = new System.Windows.Forms.Panel();
            this.panel_ProjectImageList = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // panel_Bottom
            // 
            this.panel_Bottom.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel_Bottom.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.panel_Bottom.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel_Bottom.Location = new System.Drawing.Point(0, 522);
            this.panel_Bottom.Name = "panel_Bottom";
            this.panel_Bottom.Size = new System.Drawing.Size(908, 22);
            this.panel_Bottom.TabIndex = 0;
            // 
            // panel_ProjectImageList
            // 
            this.panel_ProjectImageList.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel_ProjectImageList.AutoScroll = true;
            this.panel_ProjectImageList.BackColor = System.Drawing.SystemColors.ControlLight;
            this.panel_ProjectImageList.Location = new System.Drawing.Point(216, 12);
            this.panel_ProjectImageList.Name = "panel_ProjectImageList";
            this.panel_ProjectImageList.Size = new System.Drawing.Size(450, 91);
            this.panel_ProjectImageList.TabIndex = 2;
            // 
            // GranulateMF
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.ClientSize = new System.Drawing.Size(907, 542);
            this.Controls.Add(this.panel_ProjectImageList);
            this.Controls.Add(this.panel_Bottom);
            this.KeyPreview = true;
            this.Name = "GranulateMF";
            this.Text = "Form1";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MF_KeyDown);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel_Bottom;
        private System.Windows.Forms.Panel panel_ProjectImageList;
    }
}

