using GranulateLibrary;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GranulateMainForm
{
    public static class GUI
    {

        // Form
        private static PictureBox MainImage_PB_Main;
        public static Panel MainImagePanel;
        public static GranulateMF mainForm;
        public static Panel panel_Bottom;
        private static Label label_Coords;

        // Controls
        private static bool isMouseDown;
        private static Vec2 lastMouseClickLoc;
        private static Vec2 currentMouseLoc;
        private static Vec2 mouse_PbLocation;

        // Small preview images
        private static int smallPictureBoxSize = 75;
        private static int smallPictureBoxStartingX = 5;
        private static int smallPictureBoxStartingY = 5;
        private static int smallPictureBoxSpacing = 85;
        private static Bitmap smallPreviewBackground;

        // Main Canvas
        private static Bitmap test;
        private static bool MainImage_IsMouseInside;
        private static PictureBox MainImage_PB_Background;
        public static int currentZoom = 1;
        public static int standardZoom;
        private static Bitmap mainBackground_Bitmap;

        // Color selection Palette
        private static Vec2 PaletteSelectionLoc;
        private static int PaletteColorSelectorLoc = 4;
        private static Color Palette_SelectedColor;
        private static bool palette_MouseDown;
        private static bool selector_MouseDown;
        private static int palette_ImageSize = 120;
        public static PictureBox PB_ColorPalette;
        public static PictureBox PB_ColorSelector;
        private static NumericUpDown Color_R_Selector;
        private static NumericUpDown Color_G_Selector;
        private static NumericUpDown Color_B_Selector;
        private static Label Color_R_Label;
        private static Label Color_G_Label;
        private static Label Color_B_Label;
        private static Panel Panel_ColorSelectorBG;

        // Tools
        private static Panel toolsPanel;
        private static List<Button> toolsButtons;

        // Animation window
        public static PictureBox AnimationWindow_PB;
        public static Panel AnimationWindow_Panel;
        private static AnimationWindow _animWindow;
        public static int animationWindowImageIndex;
        private static Thread animationWindowthread;

        // Preview windows
        private static List<PreviewWindow> previewWindows = new List<PreviewWindow>();
        public static Panel panel_ProjectImageList;
        private static Bitmap plusSign;
        private static Button plusButton;

        public static void InitializeGUI()
        {
            test = new Bitmap(Path.Combine(Environment.CurrentDirectory,
                "Textures/Pictures/test.png"));
            smallPreviewBackground = ImageEditing.CreateBackgroundImage(75);
            mainBackground_Bitmap = ImageEditing.CreateBackgroundImage(1600);
            CreateNewProject(32, 32, ProjectType.SpriteAnimation);

            MainImage_PB_Background = new PictureBox();
            MainImage_PB_Main = new PictureBox();
            MainImage_PB_Main.BackColor = Color.Transparent;

            MainImage_PB_Main.Location = new Point(0, 0);
            MainImagePanel.Controls.Add(MainImage_PB_Background);
            MainImage_PB_Background.Controls.Add(MainImage_PB_Main);

            MainImage_PB_Background.Image = mainBackground_Bitmap;
            MainImage_PB_Background.Location = new Point(0, 0);
            MainImage_PB_Background.Width = MainImage_PB_Main.Width;
            MainImage_PB_Background.Height = MainImage_PB_Main.Height;

            //MainImage_PB_Main.SizeMode = PictureBoxSizeMode.StretchImage;
            MainImage_PB_Main.Paint += new PaintEventHandler(MainPB_Paint);
            MainImage_PB_Main.MouseWheel += new MouseEventHandler(MainPB_MouseWheel);
            MainImage_PB_Main.MouseDown += new MouseEventHandler(MainPB_MouseDown);
            MainImage_PB_Main.MouseUp += new MouseEventHandler(MainPB_MouseUp);
            MainImage_PB_Main.MouseMove += new MouseEventHandler(MainPB_MouseMove);

            MainImage_PB_Main.MouseEnter += new EventHandler(MainPB_MouseEnter);
            MainImage_PB_Main.MouseLeave += new EventHandler(MainPB_MouseLeave);
            MainImage_PB_Main.BorderStyle = BorderStyle.None;
            MainImage_PB_Main.SizeMode = PictureBoxSizeMode.Normal;

            //RecenterMainPB();
            ZoomMainImage(1);
            MainImage_PB_Main.Invalidate();

            plusSign = new Bitmap(Path.Combine(Environment.CurrentDirectory,
                "Textures/Icons/plus2.png"));





            switch (ProjectManager.openProjects[ProjectManager.CurrentProject].projectType)
            {
                case ProjectType.SpriteAnimation:
                    InitializeAnimationWindow();
                    plusButton = new Button();
                    CreateNewPreviewPB(0);

                    panel_ProjectImageList.Controls.Add(plusButton);
                    plusButton.Image = plusSign;
                    plusButton.Location = new Point(2 + previewWindows.Count * 90, 30);
                    plusButton.Size = new Size(30, 30);
                    plusButton.Click += new EventHandler(PlusButtonClick);
                    break;

                default:
                    panel_ProjectImageList.Dispose();
                    break;
            }

            InitializeToolBar();
            InitializeColorSelector();
        }

        /// <summary>
        /// Centers the Main Picturebox according to it's size and the main panel's size
        /// </summary>
        private static void RecenterMainPB()
        {

            MainImage_PB_Background.Location = new Point((MainImagePanel.Width / 2) - MainImage_PB_Background.Width / 2,
               (MainImagePanel.Height / 2) - MainImage_PB_Background.Height / 2);

            // The main PB is under the main BG
            MainImage_PB_Main.Location = new Point(0, 0);
        }

        private static void PlusButtonClick(object sender, EventArgs e)
        {
            AddImageToProject();
        }

        private static void InitializeColorSelector()
        {
            Panel_ColorSelectorBG = new Panel();
            mainForm.Controls.Add(Panel_ColorSelectorBG);
            Panel_ColorSelectorBG.BackColor = Color.FromArgb(255, 100, 100, 100);
            Panel_ColorSelectorBG.BorderStyle = BorderStyle.None;
            Panel_ColorSelectorBG.Location = new Point(12, 12);
            Panel_ColorSelectorBG.Size = new Size(180, 160);

            PaletteSelectionLoc = new Vec2(50, 50);

            PB_ColorPalette = new PictureBox();
            Panel_ColorSelectorBG.Controls.Add(PB_ColorPalette);
            PB_ColorPalette.Location = new Point(3, 3);
            PB_ColorPalette.SizeMode = PictureBoxSizeMode.AutoSize;
            PB_ColorPalette.Image = ImageEditing.CreateColorPaletteImage(Color.FromArgb(255, 0, 0, 255));



            PB_ColorSelector = new PictureBox();
            Panel_ColorSelectorBG.Controls.Add(PB_ColorSelector);
            PB_ColorSelector.Location = new Point(148, 3);
            PB_ColorSelector.SizeMode = PictureBoxSizeMode.AutoSize;
            PB_ColorSelector.Image = ImageEditing.CreateColorSelectorImage();

            // Assign controls
            PB_ColorPalette.Paint += new PaintEventHandler(GUI.PB_ColorPalette_Paint);
            PB_ColorPalette.MouseDown += new MouseEventHandler(PB_ColorPalette_MouseDown);
            PB_ColorPalette.MouseLeave += new EventHandler(PB_ColorPalette_MouseLeave);
            PB_ColorPalette.MouseMove += new MouseEventHandler(PB_ColorPalette_MouseMove);
            PB_ColorPalette.MouseUp += new MouseEventHandler(PB_ColorPalette_MouseUp);

            PB_ColorSelector.Paint += new PaintEventHandler(PB_ColorSelector_Paint);
            PB_ColorSelector.MouseDown += new MouseEventHandler(PB_ColorSelector_MouseDown);
            PB_ColorSelector.MouseLeave += new EventHandler(PB_ColorSelector_MouseLeave);
            PB_ColorSelector.MouseMove += new MouseEventHandler(PB_ColorSelector_MouseMove);

            int spacing = 60;
            int curLabelLocX = 0;
            int curSelectorLocX = 15;

            // R

            Color_R_Label = new Label();
            Panel_ColorSelectorBG.Controls.Add(Color_R_Label);
            Color_R_Label.Location = new Point(curLabelLocX, 133);
            Color_R_Label.BackColor = Color.FromArgb(0, 0, 0, 0);
            Color_R_Label.ForeColor = Color.White;
            Color_R_Label.Size = new Size(15, 15);
            //Color_R_Label.Font = new Font()
            Color_R_Label.Text = "R";

            Color_R_Selector = new NumericUpDown();
            Color_R_Selector.BackColor = Color.FromArgb(255, 90, 90, 90);
            Color_R_Selector.ForeColor = Color.FromArgb(255, 255, 255, 255);
            Panel_ColorSelectorBG.Controls.Add(Color_R_Selector);
            Color_R_Selector.Location = new Point(curSelectorLocX, 130);
            Color_R_Selector.Size = new Size(40, 10);
            Color_R_Selector.Minimum = 0;
            Color_R_Selector.Maximum = 255;

            curLabelLocX += spacing;
            curSelectorLocX += spacing;

            // G

            Color_G_Label = new Label();
            Panel_ColorSelectorBG.Controls.Add(Color_G_Label);
            Color_G_Label.Location = new Point(curLabelLocX, 133);
            Color_G_Label.BackColor = Color.FromArgb(0, 0, 0, 0);
            Color_G_Label.ForeColor = Color.White;
            Color_G_Label.Size = new Size(15, 15);
            //Color_R_Label.Font = new Font()
            Color_G_Label.Text = "G";

            Color_G_Selector = new NumericUpDown();
            Color_G_Selector.BackColor = Color.FromArgb(255, 90, 90, 90);
            Color_G_Selector.ForeColor = Color.FromArgb(255, 255, 255, 255);
            Panel_ColorSelectorBG.Controls.Add(Color_G_Selector);
            Color_G_Selector.Location = new Point(curSelectorLocX, 130);
            Color_G_Selector.Size = new Size(40, 10);
            Color_G_Selector.Minimum = 0;
            Color_G_Selector.Maximum = 255;

            curLabelLocX += spacing;
            curSelectorLocX += spacing;

            // B

            Color_B_Label = new Label();
            Panel_ColorSelectorBG.Controls.Add(Color_B_Label);
            Color_B_Label.Location = new Point(curLabelLocX, 133);
            Color_B_Label.BackColor = Color.FromArgb(0, 0, 0, 0);
            Color_B_Label.ForeColor = Color.White;
            Color_B_Label.Size = new Size(15, 15);
            //Color_R_Label.Font = new Font()
            Color_R_Label.Text = "B";

            Color_B_Selector = new NumericUpDown();
            Color_B_Selector.BackColor = Color.FromArgb(255, 90, 90, 90);
            Color_B_Selector.ForeColor = Color.FromArgb(255, 255, 255, 255);
            Panel_ColorSelectorBG.Controls.Add(Color_B_Selector);
            Color_B_Selector.Location = new Point(curSelectorLocX, 130);
            Color_B_Selector.Size = new Size(40, 10);
            Color_B_Selector.Minimum = 0;
            Color_B_Selector.Maximum = 255;


            // Coords label
            label_Coords = new Label();
            panel_Bottom.Controls.Add(label_Coords);
            label_Coords.Font = new Font(FontFamily.GenericSansSerif, 10, FontStyle.Bold);
            label_Coords.Text = "0, 0";
            label_Coords.Location = new Point(49, 3);
            label_Coords.Size = new Size(70, 22);



        }

        /// <summary>
        /// Adds a new image to the project, preview windows and puts it in focus
        /// </summary>
        private static void AddImageToProject()
        {
            // Add new image to the project
            ProjectManager.AddNewProjectImage();

            // Add a new preview window
            CreateNewPreviewPB(ProjectManager.openProjects[ProjectManager.CurrentProject].bitmaps.Count - 1);

            // Move the preview window's plus button accordingly
            plusButton.Location = new Point(10 + previewWindows.Count * 85, 30);


            // This is very bad
            Cursor.Position = new Point(
            panel_ProjectImageList.Location.X + plusButton.Location.X + plusButton.Size.Width / 2,
            panel_ProjectImageList.Location.Y + plusButton.Location.Y * 2 + 4);

            // Show the new image in the main picturebox 
            LoadNewMainImage(ProjectManager.openProjects[ProjectManager.CurrentProject].bitmaps[
                ProjectManager.openProjects[ProjectManager.CurrentProject].bitmaps.Count - 1], currentZoom);
        }


        /// <summary>
        /// Loads a new image on the main Picturebox
        /// </summary>
        /// <param name="image"></param>
        /// <param name="zoom"></param>
        private static void LoadNewMainImage(Bitmap image, int zoom)
        {
            Bitmap tmp = ImageEditing.GetZoomedImage(image, zoom);

            MainImage_PB_Main.Image = tmp;

        }


        /// <summary>
        /// Creates a new empty project. 
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="pType"></param>
        public static void CreateNewProject(int width, int height, ProjectType pType)
        {
            // Performs necessary resets to the form to be able to show a new clean project
            ResetForm();

            // Create new project
            ProjectManager.CreateNewProject(width, height, pType);

            // Sets the current zoom accordingly to the new image size
            currentZoom = (int)Math.Floor(800M / ProjectManager.openProjects[ProjectManager.CurrentProject].bitmaps[0].Width);

            // Maintenance
            standardZoom = currentZoom;
        }

        /// <summary>
        /// Selects an image from the preview windows and puts it in focus
        /// </summary>
        /// <param name="index"></param>
        private static void SelectNewImage(int index)
        {
            // Update the preview windows backpanels for GUI visual
            previewWindows[ProjectManager.SelectedBitmap].backPanel.BackColor = Color.Transparent;
            ProjectManager.SelectedBitmap = index;
            previewWindows[ProjectManager.SelectedBitmap].backPanel.BackColor = Color.Black;

            // Load the new image
            LoadNewMainImage(ProjectManager.openProjects[ProjectManager.CurrentProject].bitmaps[ProjectManager.SelectedBitmap], standardZoom);
            currentZoom = standardZoom;

            // Right now it recenters the image when selecting one. Might change.
            RecenterMainPB();

        }

        /// <summary>
        /// Resets the form to accomodate a new project
        /// </summary>
        private static void ResetForm()
        {
            previewWindows.Clear();
        }

        private static void InitializeAnimationWindow()
        {
            _animWindow = new AnimationWindow(AnimationWindow_PB, 5, 32);
            animationWindowthread = new Thread(new ThreadStart(_animWindow.AnimationLoop));
            animationWindowthread.Start();

            AnimationWindow_PB.Size = AnimationWindow_Panel.Size;
            AnimationWindow_PB.Location = new Point(0, 0);

            AnimationWindow_PB.Image = ImageEditing.CreateBackgroundImage(160);
            AnimationWindow_PB.Paint += new PaintEventHandler(_animWindow.AnimationPB_Paint);

        }

        private static void MainPB_Paint(object sender, PaintEventArgs e)
        {
            // For sharpness
            e.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;

            // Show the paint rectangle
            if (MainImage_IsMouseInside)
            {
                e.Graphics.DrawRectangle(new Pen(new SolidBrush(Color.Black)), new Rectangle(
                    (int)Math.Floor(currentMouseLoc.x * (decimal)currentZoom),
                    (int)Math.Floor(currentMouseLoc.y * (decimal)currentZoom),
                     currentZoom,
                     currentZoom));
            }
        }

        private static void InitializeToolBar()
        {
            toolsPanel = new Panel();
            toolsPanel.Location = new Point(20, 242);
            toolsPanel.Size = new Size(150, 100);
            toolsPanel.BackColor = Color.FromArgb(255, 80, 80, 80);
            mainForm.Controls.Add(toolsPanel);

            // Pencil
            toolsButtons = new List<Button>();
            toolsButtons.Add(new Button());

            toolsPanel.Controls.Add(toolsButtons[0]);
            toolsButtons[0].Size = new Size(35, 35);
            toolsButtons[0].Image = ToolsManager.toolsList[0].NormalButtonImage;
            toolsButtons[0].Name = "toolButton_0";
            toolsButtons[0].FlatStyle = FlatStyle.Flat;
            toolsButtons[0].Click += new EventHandler(ToolButtonPressed);

            // Eraser
            toolsButtons.Add(new Button());
            toolsPanel.Controls.Add(toolsButtons[1]);
            toolsButtons[1].Size = new Size(35, 35);
            toolsButtons[1].Location = new Point(38, 0);
            toolsButtons[1].Name = "toolButton_1";
            toolsButtons[1].FlatStyle = FlatStyle.Flat;
            toolsButtons[1].Image = ToolsManager.toolsList[1].NormalButtonImage;
            toolsButtons[1].Click += new EventHandler(ToolButtonPressed);

        }

        private static void CreateNewPreviewPB(int index)
        {
            previewWindows.Add(new PreviewWindow());
            previewWindows[index].id = index;
            previewWindows[index].backPanel = new Panel();
            panel_ProjectImageList.Controls.Add(previewWindows[index].backPanel);
            previewWindows[index].backPanel.Size = new Size(85, 85);
            previewWindows[index].backPanel.Location = new Point(3 + index * smallPictureBoxSpacing, 3);
            previewWindows[index].backPanel.BackColor = Color.Black;


            previewWindows[index].pb_Main = new PictureBox();
            previewWindows[index].pb_Background = new PictureBox();
            previewWindows[index].pb_Main.Name = "PreviewImage_" + index;
            previewWindows[index].pb_Main.Click += new EventHandler(PreviewImageClick);
            previewWindows[index].pb_Main.SizeMode = PictureBoxSizeMode.StretchImage;

            previewWindows[index].backPanel.Controls.Add(previewWindows[index].pb_Background);
            previewWindows[index].pb_Background.Controls.Add(previewWindows[index].pb_Main);

            previewWindows[index].pb_Background.Location = new Point(5, 5);
            previewWindows[index].pb_Background.Size = new Size(smallPictureBoxSize, smallPictureBoxSize);
            previewWindows[index].pb_Background.SizeMode = PictureBoxSizeMode.Normal;

            previewWindows[index].pb_Main.Location = new Point(0, 0);
            previewWindows[index].pb_Main.BackColor = Color.Transparent;

            previewWindows[index].pb_Main.Size = previewWindows[index].pb_Background.Size;
            previewWindows[index].pb_Background.Image = smallPreviewBackground;
            previewWindows[index].pb_Main.Image = ProjectManager.openProjects[ProjectManager.CurrentProject].bitmaps[index];

            previewWindows[ProjectManager.SelectedBitmap].backPanel.BackColor = Color.Transparent;
            ProjectManager.SelectedBitmap = index;
            previewWindows[ProjectManager.SelectedBitmap].backPanel.BackColor = Color.Black;



        }

        /// <summary>
        /// Called when a preview picturebox is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void PreviewImageClick(object sender, EventArgs e)
        {
            PictureBox pb = (PictureBox)sender;

            // Identify the image ID
            int ix = pb.Name.IndexOf('_');
            ix++;
            int number = Int32.Parse(pb.Name.Substring(ix));
            SelectNewImage(number);
        }

        private static void ZoomMainImage(int amount)
        {
            // Update the Zoom level
            currentZoom += amount;

            // Because the MAIN PB is under control of the MAIN BG PB, we need to resize the BG PB first, same with moving.
            MainImage_PB_Background.Size = new Size(ProjectManager.openProjects[ProjectManager.CurrentProject].bitmaps[ProjectManager.SelectedBitmap].Width * currentZoom,
                ProjectManager.openProjects[ProjectManager.CurrentProject].bitmaps[ProjectManager.SelectedBitmap].Height * currentZoom);
            MainImage_PB_Main.Size = MainImage_PB_Background.Size;

            // Assign the new zoomed image
            MainImage_PB_Main.Image = ImageEditing.GetZoomedImage(ProjectManager.openProjects[ProjectManager.CurrentProject].bitmaps[ProjectManager.SelectedBitmap], currentZoom);

            // Recenter it
            RecenterMainPB();


        }

        public static void ActionPixelsModified(List<PixelModification> pixelsList, int bitmapID, bool reverse)
        {
            // If we're currently showing the bitmap on the main Picturebox,
            // then we update it live
            if (ProjectManager.SelectedBitmap == bitmapID)
            {
                UpdateMainPictureBox(pixelsList, reverse);
            }

            // If the preview window is active, update it. Should add Animation window too
            if (ProjectManager.openProjects[ProjectManager.CurrentProject].projectType == ProjectType.SpriteAnimation)
            {
                UpdatePreviewWindow(bitmapID);
            }
        }

        public static void UpdateMainPictureBox(List<PixelModification> pixelsList, bool reversed)
        {
            // Cannot reference it, needs to be reassigned
            Bitmap bmp = (Bitmap)MainImage_PB_Main.Image;

            Color newColor;

            // Go through all the modified pixels in the base bmp, and draw the corresponding
            // rectangles in the zoomed in / out image
            for (int i = 0; i < pixelsList.Count; i++)
            {
                // Pick the correct color
                if (reversed)
                {
                    newColor = pixelsList[i].oldColor;
                }
                else
                {
                    newColor = pixelsList[i].newColor;
                }


                // Find the top left pixel
                Vec2 pictureBoxPixel = new Vec2(pixelsList[i].pixelLoc.x * currentZoom, pixelsList[i].pixelLoc.y * currentZoom);

                // Draw the rectangle as big as the zoom level
                ImageEditing.FillRectangle(newColor, new Rectangle(pictureBoxPixel.x, pictureBoxPixel.y, currentZoom, currentZoom), ref bmp);
            }

            // Reassign it
            MainImage_PB_Main.Image = bmp;
        }

        /// <summary>
        /// Updates the selected preview window's image
        /// </summary>
        /// <param name="index"></param>
        public static void UpdatePreviewWindow(int index)
        {
            previewWindows[index].pb_Main.Image = ProjectManager.openProjects[
                   ProjectManager.CurrentProject].bitmaps[index];
        }

        private static void UseTool(MouseEventArgs m, bool newAction)
        {
            currentMouseLoc = GetRealPixelCoordinates(m.Location);
            ToolsManager.UseTool(currentMouseLoc, lastMouseClickLoc, newAction);
            lastMouseClickLoc = currentMouseLoc; //keep assigning the lastPoint
        }

        /// <summary>
        /// Get the image PIXEL coordinates from the picturebox's MOUSE coordinates
        /// </summary>
        /// <param name="mouseLoc"></param>
        /// <returns></returns>
        private static Vec2 GetRealPixelCoordinates(Point mouseLoc)
        {
            int x = (int)Math.Floor((decimal)mouseLoc.X / (decimal)currentZoom);
            int y = (int)Math.Floor((decimal)mouseLoc.Y / (decimal)currentZoom);
            return new Vec2(x, y);
        }


        private static void PaletteSelectNewBaseColor(int y)
        {
            Bitmap bmp = (Bitmap)PB_ColorSelector.Image;

            if (y < 0)
                y = 0;
            if (y >= bmp.Height)
                y = bmp.Height - 1;

            Palette_SelectedColor = bmp.GetPixel(2, y);

            PB_ColorPalette.Image = ImageEditing.CreateColorPaletteImage(Palette_SelectedColor);

            PB_ColorSelector.Invalidate();

            PaletteSelectNewColor(PaletteSelectionLoc);
        }



        private static void PaletteSelectNewColor(Vec2 loc)
        {
            Bitmap bmp = (Bitmap)PB_ColorPalette.Image;
            if (loc.x < 0)
                loc.x = 0;
            if (loc.x >= bmp.Width)
                loc.x = bmp.Width - 1;
            if (loc.y < 0)
                loc.y = 0;
            if (loc.y >= bmp.Height)
                loc.y = bmp.Height - 1;

            Color color = bmp.GetPixel(loc.x, loc.y);
            //Console.WriteLine($"R: {color.R}, G: {color.G}, B: {color.B} - x: {loc.x}, y: {loc.y}");
            ProjectManager.Color_Main = bmp.GetPixel(loc.x, loc.y);

            Color_R_Selector.Value = color.R;
            Color_G_Selector.Value = color.G;
            Color_B_Selector.Value = color.B;

        }

        /////////////////////////     CONTROLS       ////////////////////////////

        private static void MainPB_MouseUp(object sender, MouseEventArgs m)
        {
            isMouseDown = false;
            ToolsManager.UseTool_MouseUp();
        }

        private static void MainPB_MouseDown(object sender, MouseEventArgs m)
        {
            isMouseDown = true;
            lastMouseClickLoc = GetRealPixelCoordinates(m.Location);

            switch (ToolsManager.SelectedTool)
            {
                default:
                    UseTool(m, true);
                    break;
            }

        }

        private static void MainPB_MouseEnter(object sender, EventArgs e)
        {
            MainImage_IsMouseInside = true;

        }

        private static void MainPB_MouseLeave(object sender, EventArgs e)
        {
            MainImage_IsMouseInside = false;
        }



        private static void MainPB_MouseMove(object sender, MouseEventArgs m)
        {


            //mouse_PbLocation = 
            currentMouseLoc = GetRealPixelCoordinates(m.Location);
            //Console.WriteLine($"x: {m.Location.X}, y: {m.Location.Y}");
            label_Coords.Text = $"{currentMouseLoc.x}, {currentMouseLoc.y}";
            MainImage_PB_Main.Invalidate();

            if (isMouseDown)
            {
                switch (ToolsManager.SelectedTool)
                {
                    default:
                        UseTool(m, false);
                        break;
                }
            }
        }



        /*
        private void testButton_Click(object sender, EventArgs e)
        {
            _animWindow.StopAnimation();
        }

        private void startButton_Click(object sender, EventArgs e)
        {
            _animWindow.StartAnimation();
        }
        */
        private static void MainPB_MouseWheel(object sender, MouseEventArgs m)
        {

            if (Control.ModifierKeys == Keys.Alt)
            {
                if (m.Delta > 0)
                {
                    ZoomMainImage(1);
                    RecenterMainPB();
                }
                else
                {
                    ZoomMainImage(-1);
                    RecenterMainPB();
                }
            }


        }

        public static void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {

                case Keys.U:
                    break;
                    Bitmap bmp = new Bitmap(1600, 1600);
                    Stopwatch watch = new Stopwatch();
                    Rectangle rect = new Rectangle(0, 0, 10, 10);
                    watch.Start();
                    for (int i = 0; i < 100; i++)
                    {
                        Bitmap sbmp = ImageEditing.CreateBackgroundImage(75);
                        //ImageEditing.FillRectangle(Color.Black, rect, ref bmp);
                    }
                    watch.Stop();
                    Console.WriteLine(watch.ElapsedMilliseconds);
                    break;

                case Keys.J:

                    break;

                case Keys.Z:
                    if (Control.ModifierKeys == Keys.Control)
                    {
                        ActionsManager.UndoSequential();
                    }
                    break;

            }
        }


        private static void PB_ColorPalette_MouseMove(object sender, MouseEventArgs e)
        {

            if (palette_MouseDown)
            {
                PaletteSelectionLoc = new Vec2(e.Location.X, e.Location.Y);
                PaletteSelectNewColor(PaletteSelectionLoc);

                if (PaletteSelectionLoc.x < 0)
                    PaletteSelectionLoc.x = 0;
                if (PaletteSelectionLoc.x > palette_ImageSize - 5)
                    PaletteSelectionLoc.x = palette_ImageSize - 5;
                if (PaletteSelectionLoc.y < 0)
                    PaletteSelectionLoc.y = 0;
                if (PaletteSelectionLoc.y > palette_ImageSize - 5)
                    PaletteSelectionLoc.y = palette_ImageSize - 5;


                PB_ColorPalette.Invalidate();

            }
        }

        private static void PB_ColorPalette_MouseDown(object sender, MouseEventArgs e)
        {
            palette_MouseDown = true;
            PaletteSelectionLoc = new Vec2(e.Location.X, e.Location.Y);
            PaletteSelectNewColor(PaletteSelectionLoc);

            if (PaletteSelectionLoc.x < 0)
                PaletteSelectionLoc.x = 0;
            if (PaletteSelectionLoc.x > palette_ImageSize - 5)
                PaletteSelectionLoc.x = palette_ImageSize - 5;
            if (PaletteSelectionLoc.y < 0)
                PaletteSelectionLoc.y = 0;
            if (PaletteSelectionLoc.y > palette_ImageSize - 5)
                PaletteSelectionLoc.y = palette_ImageSize - 5;


            PB_ColorPalette.Invalidate();
        }

        private static void PB_ColorPalette_MouseUp(object sender, MouseEventArgs e)
        {
            palette_MouseDown = false;
        }

        private static void PB_ColorPalette_MouseLeave(object sender, EventArgs e)
        {
            palette_MouseDown = false;
        }


        private static void PB_ColorSelector_MouseDown(object sender, MouseEventArgs e)
        {
            selector_MouseDown = true;

            PaletteColorSelectorLoc = e.Location.Y;

            if (PaletteColorSelectorLoc < 0)
                PaletteColorSelectorLoc = 0;
            if (PaletteColorSelectorLoc > 120)
                PaletteColorSelectorLoc = 120;

            PaletteSelectNewBaseColor(PaletteColorSelectorLoc);
        }

        private static void PB_ColorSelector_MouseMove(object sender, MouseEventArgs e)
        {
            if (selector_MouseDown)
            {
                PaletteColorSelectorLoc = e.Location.Y;

                if (PaletteColorSelectorLoc < 0)
                    PaletteColorSelectorLoc = 0;
                if (PaletteColorSelectorLoc > 120)
                    PaletteColorSelectorLoc = 120;

                PaletteSelectNewBaseColor(PaletteColorSelectorLoc);



            }
        }

        private static void PB_ColorSelector_MouseUp(object sender, MouseEventArgs e)
        {
            selector_MouseDown = false;
        }

        private static void PB_ColorSelector_MouseLeave(object sender, EventArgs e)
        {
            selector_MouseDown = false;
        }

        private static void PB_ColorPalette_Paint(object sender, PaintEventArgs e)
        {
            //e.Graphics.DrawRectangle(new Pen(new SolidBrush(Color.Black)), new Rectangle(PaletteSelectionLoc.x, PaletteSelectionLoc.y, 5, 5));
            e.Graphics.FillRectangle(new SolidBrush(Color.Black), new Rectangle(PaletteSelectionLoc.x, PaletteSelectionLoc.y, 5, 5));
        }

        private static void PB_ColorSelector_Paint(object sender, PaintEventArgs e)
        {
            int y;
            int rectHeight = 4;

            y = PaletteColorSelectorLoc;
            if (PB_ColorSelector.Image != null)
            {
                if (y > PB_ColorSelector.Image.Height - (rectHeight / 2))
                {
                    y = PB_ColorSelector.Image.Height - (rectHeight / 2);
                }

                e.Graphics.DrawRectangle(new Pen(new SolidBrush(Color.Black)), new Rectangle(
                    0, y - (rectHeight / 2), PB_ColorSelector.Image.Width, rectHeight));
            }


        }


        private static void ToolButtonPressed(object sender, EventArgs e)
        {
            Button pb = (Button)sender;
            int ix = pb.Name.IndexOf('_');
            ix++;
            int number = Int32.Parse(pb.Name.Substring(ix));

            // Reset the previous selected button
            toolsButtons[ToolsManager.SelectedTool].Image = ToolsManager.toolsList[
                ToolsManager.SelectedTool].NormalButtonImage;

            // Assign the new selected tool
            ToolsManager.SelectedTool = number;

            toolsButtons[ToolsManager.SelectedTool].Image = ToolsManager.toolsList[
                ToolsManager.SelectedTool].SelectedButtonImage;


        }
    }
}
