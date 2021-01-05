using GranulateLibrary;
using GranulateLibrary.EventSystem;
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

        // Colors
        private static Color Color_Default_DarkGray = Color.FromArgb(255, 150, 150, 150);

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
        private static bool MainImage_IsMouseInside;
        private static PictureBox MainImage_PB_Background;
        public static int currentZoom = 1;
        public static int standardZoom;
        private static Bitmap mainBackground_Bitmap;
        private static bool MainImage_IsGridEnabled = true;

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
        private static PictureBox PB_AnimationWindow_Main;
        private static PictureBox PB_AnimationWindow_BG;
        private static Panel Panel_AnimationWindow;
        private static int animationWindowSize = 180;
        private static AnimationWindow _animWindow;
        public static int animationWindowImageIndex;
        private static Thread animationWindowthread;
        private static Bitmap playPauseButtonImage;
        private static Button playPauseButton;

        // Preview windows
        private static List<PreviewWindow> previewWindows = new List<PreviewWindow>();
        public static Panel panel_ProjectImageList;
        private static Bitmap plusSign;
        private static Button plusButton;

        public static void InitializeGUI()
        {
            // Load assets
            plusSign = new Bitmap(Path.Combine(Environment.CurrentDirectory,
               "Textures/Icons/plus2.png"));
            playPauseButtonImage = new Bitmap(Path.Combine(Environment.CurrentDirectory,
               "Textures/Icons/Animation/play_pause_25.png"));

            smallPreviewBackground = ImageEditing.CreateBackgroundImage(75);
            mainBackground_Bitmap = ImageEditing.CreateBackgroundImage(1600);
            CreateNewProject(32, 32, ProjectType.SpriteAnimation);

            MainImagePanel = new Panel() 
            {
                Location = new Point(400, 142),
                Size = new Size(1200, 800),
                BackColor = Color_Default_DarkGray,
                BorderStyle = BorderStyle.Fixed3D
            };

            mainForm.Controls.Add(MainImagePanel);


            // Initialization

            MainImage_PB_Main = new PictureBox()
            {
                BackColor = Color.Transparent,
                Location = new Point(0, 0),
                BorderStyle = BorderStyle.None,
                SizeMode = PictureBoxSizeMode.Normal
            };

            MainImage_PB_Background = new PictureBox()
            {
                Image = mainBackground_Bitmap,
                Location = new Point(0, 0),
                Width = MainImage_PB_Main.Width,
                Height = MainImage_PB_Main.Height
            };


 
            // Controls
            MainImagePanel.Controls.Add(MainImage_PB_Background);
            MainImage_PB_Background.Controls.Add(MainImage_PB_Main);


            // Event Handlers
            MainImage_PB_Main.Paint += new PaintEventHandler(MainPB_Paint);
            MainImage_PB_Main.MouseWheel += new MouseEventHandler(MainPB_MouseWheel);
            MainImage_PB_Main.MouseDown += new MouseEventHandler(MainPB_MouseDown);
            MainImage_PB_Main.MouseUp += new MouseEventHandler(MainPB_MouseUp);
            MainImage_PB_Main.MouseMove += new MouseEventHandler(MainPB_MouseMove);
            MainImage_PB_Main.MouseEnter += new EventHandler(MainPB_MouseEnter);
            MainImage_PB_Main.MouseLeave += new EventHandler(MainPB_MouseLeave);


            ZoomMainImage(1);
            MainImage_PB_Main.Invalidate();

           

            // Subscribe to events
            ImageEditing.ImageModifiedEvent += PixelsModifiedEvent;


            // Initialize elements based on the type of project

            switch (ProjectManager.openProjects[ProjectManager.CurrentProject].ProjectType)
            {
                case ProjectType.SpriteAnimation:

                    
                    InitializeAnimationWindow();

                    // Creates the first preview window and add a plus button
                    CreateNewPreviewPB(0);

                    plusButton = new Button()
                    {
                        Image = plusSign,
                        Location = new Point(2 + previewWindows.Count * 90, 30),
                        Size = new Size(30, 30),
                    };

                    panel_ProjectImageList.Controls.Add(plusButton);
                    plusButton.Click += new EventHandler(PlusButtonClick);

                    break;

                default:
                    panel_ProjectImageList.Dispose();
                    break;
            }

            InitializeToolBar();
            InitializeColorSelector();
            
        }

        public static void InitializeTestGUI()
        {
            //ImageEditing.RaiseImageModifiedEvent += PixelsModifiedEvent;
        }

        private static void PixelsModifiedEvent(object sender, PixelsModificationEventArgs p)
        {
            ActionPixelsModified(p.pixelModificationList, p.BitmapID, p.IsReverse);
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
            Panel_ColorSelectorBG = new Panel()
            {
                BackColor = Color.FromArgb(255, 100, 100, 100),
                BorderStyle = BorderStyle.None,
                Location = new Point(12, 12),
                Size = new Size(180, 160)
            };

            mainForm.Controls.Add(Panel_ColorSelectorBG);

            PaletteSelectionLoc = new Vec2(50, 50);

            PB_ColorPalette = new PictureBox()
            {
                Location = new Point(3, 3),
                SizeMode = PictureBoxSizeMode.AutoSize,
                Image = ImageEditing.CreateColorPaletteImage(Color.FromArgb(255, 0, 0, 255))
            };

            Panel_ColorSelectorBG.Controls.Add(PB_ColorPalette);

            PB_ColorSelector = new PictureBox()
            {
                Location = new Point(148, 3),
                SizeMode = PictureBoxSizeMode.AutoSize,
                Image = ImageEditing.CreateColorSelectorImage()
            };

            Panel_ColorSelectorBG.Controls.Add(PB_ColorSelector);

            // EventHandlers
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

            Color_R_Label = new Label()
            {
                Location = new Point(curLabelLocX, 133),
                BackColor = Color.FromArgb(0, 0, 0, 0),
                ForeColor = Color.White,
                Size = new Size(15, 15),
                Text = "R"
            };

            Panel_ColorSelectorBG.Controls.Add(Color_R_Label);


            Color_R_Selector = new NumericUpDown()
            {
                BackColor = Color.FromArgb(255, 90, 90, 90),
                ForeColor = Color.FromArgb(255, 255, 255, 255),
                Location = new Point(curSelectorLocX, 130),
                Size = new Size(40, 10),
                Minimum = 0,
                Maximum = 255
            };

            Panel_ColorSelectorBG.Controls.Add(Color_R_Selector);

            curLabelLocX += spacing;
            curSelectorLocX += spacing;

            // G

            Color_G_Label = new Label()
            {
                Location = new Point(curLabelLocX, 133),
                BackColor = Color.FromArgb(0, 0, 0, 0),
                ForeColor = Color.White,
                Size = new Size(15, 15),
                Text = "G"
            };

            Panel_ColorSelectorBG.Controls.Add(Color_G_Label);

           

            Color_G_Selector = new NumericUpDown()
            {
                BackColor = Color.FromArgb(255, 90, 90, 90),
                ForeColor = Color.FromArgb(255, 255, 255, 255),
                Location = new Point(curSelectorLocX, 130),
                Size = new Size(40, 10),
                Minimum = 0,
                Maximum = 255
            };

            Panel_ColorSelectorBG.Controls.Add(Color_G_Selector);


            curLabelLocX += spacing;
            curSelectorLocX += spacing;

            // B

            Color_B_Label = new Label()
            {
                Location = new Point(curLabelLocX, 133),
                BackColor = Color.FromArgb(0, 0, 0, 0),
                ForeColor = Color.White,
                Size = new Size(15, 15),
                Text = "B"
            };

            Panel_ColorSelectorBG.Controls.Add(Color_B_Label);

            Color_B_Selector = new NumericUpDown()
            {
                BackColor = Color.FromArgb(255, 90, 90, 90),
                ForeColor = Color.FromArgb(255, 255, 255, 255),
                Location = new Point(curSelectorLocX, 130),
                Size = new Size(40, 10),
                Minimum = 0,
                Maximum = 255
            };

            Panel_ColorSelectorBG.Controls.Add(Color_B_Selector);

           


            // Coords label
            label_Coords = new Label()
            {
                Font = new Font(FontFamily.GenericSansSerif, 10, FontStyle.Bold),
                Text = "0, 0",
                Location = new Point(49, 3),
                Size = new Size(70, 22)
            };

            panel_Bottom.Controls.Add(label_Coords);
        }

        /// <summary>
        /// Adds a new image to the project, preview windows and puts it in focus
        /// </summary>
        private static void AddImageToProject()
        {
            // Add new image to the project
            ProjectManager.AddNewProjectImage();

            // Add a new preview window
            CreateNewPreviewPB(ProjectManager.openProjects[ProjectManager.CurrentProject].Bitmaps.Count - 1);

            // Move the preview window's plus button accordingly
            plusButton.Location = new Point(10 + previewWindows.Count * 85, 30);


            // This is very bad
            Cursor.Position = new Point(
            panel_ProjectImageList.Location.X + plusButton.Location.X + plusButton.Size.Width / 2,
            panel_ProjectImageList.Location.Y + plusButton.Location.Y * 2 + 4);

            // Show the new image in the main picturebox 
            LoadNewMainImage(ProjectManager.openProjects[ProjectManager.CurrentProject].Bitmaps[
                ProjectManager.openProjects[ProjectManager.CurrentProject].Bitmaps.Count - 1], currentZoom);
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
            currentZoom = (int)Math.Floor(800M / ProjectManager.openProjects[ProjectManager.CurrentProject].Bitmaps[0].Width);

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
            LoadNewMainImage(ProjectManager.openProjects[ProjectManager.CurrentProject].Bitmaps[ProjectManager.SelectedBitmap], standardZoom);
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
            Panel_AnimationWindow = new Panel()
            {
                Anchor = (AnchorStyles.Right | AnchorStyles.Top),
                Size = new Size(195, 220),
                BackColor = Color_Default_DarkGray,
                BorderStyle = BorderStyle.Fixed3D,
                Location = new Point(700, 12)
            };

            mainForm.Controls.Add(Panel_AnimationWindow);

            PB_AnimationWindow_BG = new PictureBox()
            { 
                Location = new Point(5, 5),
                Size = new Size(animationWindowSize, animationWindowSize),
                Image = ImageEditing.CreateBackgroundImage(animationWindowSize)
            };

            Panel_AnimationWindow.Controls.Add(PB_AnimationWindow_BG);


            PB_AnimationWindow_Main = new PictureBox()
            {
                BorderStyle = BorderStyle.Fixed3D,
                Size = PB_AnimationWindow_BG.Size,
                BackColor = Color.Transparent
            };

            PB_AnimationWindow_BG.Controls.Add(PB_AnimationWindow_Main);


            playPauseButton = new Button()
            {
                Size = new Size(26, 26),
                Location = new Point(5, 187),
                Image = playPauseButtonImage,
                Name = "Anim_Button_Play"
            };

            Panel_AnimationWindow.Controls.Add(playPauseButton);
            


            _animWindow = new AnimationWindow(PB_AnimationWindow_Main, 5, 32);
            animationWindowthread = new Thread(new ThreadStart(_animWindow.AnimationLoop));
            animationWindowthread.Start();

        }

        private static void MainPB_Paint(object sender, PaintEventArgs e)
        {
            // For sharpness
            e.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;

            if(MainImage_IsGridEnabled)
            {
                int middleLoc = MainImage_PB_Main.Width / 2;

                e.Graphics.DrawLine(new Pen(new SolidBrush(Color.Red)),
                    new Point(middleLoc, 0),
                    new Point(middleLoc, MainImage_PB_Main.Height));

                e.Graphics.DrawLine(new Pen(new SolidBrush(Color.Red)),
                    new Point(0, middleLoc),
                    new Point(MainImage_PB_Main.Width, middleLoc));
            }

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
            toolsPanel = new Panel()
            {
                Location = new Point(20, 242),
                Size = new Size(150, 100),
                BackColor = Color.FromArgb(255, 80, 80, 80)
            };
            
            mainForm.Controls.Add(toolsPanel);

            // Pencil
            toolsButtons = new List<Button>();
            toolsButtons.Add(new Button()
            {
                Size = new Size(35, 35),
                Image = ToolsManager.toolsList[0].NormalButtonImage,
                Name = "toolButton_0",
                FlatStyle = FlatStyle.Flat,

            });

            toolsButtons[0].Click += new EventHandler(ToolButtonPressed);
            toolsPanel.Controls.Add(toolsButtons[0]);

            // Eraser
            toolsButtons.Add(new Button()
            {
                Size = new Size(35, 35),
                Location = new Point(38, 0),
                Name = "toolButton_1",
                FlatStyle = FlatStyle.Flat,
                Image = ToolsManager.toolsList[1].NormalButtonImage
            });

            toolsButtons[1].Click += new EventHandler(ToolButtonPressed);
            toolsPanel.Controls.Add(toolsButtons[1]);

        }

        private static void CreateNewPreviewPB(int index)
        {
            previewWindows.Add(new PreviewWindow());
            previewWindows[index].id = index;
            previewWindows[index].backPanel = new Panel()
            {
                Size = new Size(85, 85),
                Location = new Point(3 + index * smallPictureBoxSpacing, 3),
                BackColor = Color.Black
            };

            panel_ProjectImageList.Controls.Add(previewWindows[index].backPanel);


            // This breaks otherwise. TO DO: Reformat it
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
            previewWindows[index].pb_Main.Image = ProjectManager.openProjects[ProjectManager.CurrentProject].Bitmaps[index];

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
            MainImage_PB_Background.Size = new Size(ProjectManager.openProjects[ProjectManager.CurrentProject].Bitmaps[ProjectManager.SelectedBitmap].Width * currentZoom,
                ProjectManager.openProjects[ProjectManager.CurrentProject].Bitmaps[ProjectManager.SelectedBitmap].Height * currentZoom);
            MainImage_PB_Main.Size = MainImage_PB_Background.Size;

            // Assign the new zoomed image
            MainImage_PB_Main.Image = ImageEditing.GetZoomedImage(ProjectManager.openProjects[ProjectManager.CurrentProject].Bitmaps[ProjectManager.SelectedBitmap], currentZoom);

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
            if (ProjectManager.openProjects[ProjectManager.CurrentProject].ProjectType == ProjectType.SpriteAnimation)
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
            // Check if the preview windows list contains index, and if it's been instantiated
            if(previewWindows.Count >= index)
                if(previewWindows[index].pb_Main != null)
                    previewWindows[index].pb_Main.Image = ProjectManager.openProjects[
                    ProjectManager.CurrentProject].Bitmaps[index];
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

        public static void MainForm_KeyDown(object sender, KeyEventArgs e)
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
                    
                    Bitmap bmp2 = new Bitmap(1600, 1600);

                    Rectangle rects = new Rectangle(200, 200, 400, 400);
                    
                    //Brush bru = new SolidBrush(Color.Red);
                    Stopwatch watch2 = Stopwatch.StartNew();
                    //Bitmap testBmp;


                    for (int i = 0; i < 100; i++)
                    {
                        //testBmp = ImageEditing.CreateBackgroundImage(160, false);
                        ImageEditing.FillRectangle(Color.Red, rects, ref bmp2);
                    }

                    watch2.Stop();
                    Console.WriteLine(watch2.ElapsedMilliseconds);
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

        public static void OnApplicationExit(object sender, EventArgs e)
        {
            animationWindowthread.Abort();
        }
    }
}
