using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Threading;
using GranulateLibrary;

namespace GranulateMainForm
{
    public class AnimationWindow
    {
        public PictureBox PB_Main;
        public PictureBox PB_Background;
        public bool running;


        private float frameDelay;
        private float timeStamp;
        private int frame;
        private int count;
        private Stopwatch timer;
        private int imageSize;
        private Rectangle sourceRect;
        private Rectangle destRect;



        public int Fps
        {
            get { return Fps; }
            set { SetNewFps(value); }
        }

        private void SetNewFps(int fps)
        {
            frameDelay = 1000f / fps;
        }

        public AnimationWindow(PictureBox pBox, int fps, int imageSize)
        {
            this.imageSize = imageSize;

            timer = new Stopwatch();
            timer.Start();
            running = false;
            PB_Main = pBox;
            PB_Main.Paint += AnimationPB_Paint;
            sourceRect = new Rectangle(0, 0, imageSize, imageSize);
            destRect = new Rectangle(0, 0, PB_Main.Width, PB_Main.Height);
            Fps = fps;
            frame = 0;

            if (ProjectManager.openProjects[ProjectManager.CurrentProject].Bitmaps[0] != null)
            {
                PB_Main.Invalidate();
            }
        }

        /// <summary>
        /// Stops the current animation, and resets it to the first frame
        /// </summary>
        public void StopAnimation()
        {
            running = false;
            frame = 0;
            //pictureBox.Image = ProjectManager.openProjects[ProjectManager.CurrentProject].bitmaps[frame];
            PB_Main.Invalidate();
        }

        /// <summary>
        /// Starts or restarts the animation
        /// </summary>
        public void StartAnimation()
        {
            running = true;
        }


        public void AnimationPB_Paint(object sender, PaintEventArgs p)
        {
            p.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;

            p.Graphics.DrawImage(
                ProjectManager.openProjects[ProjectManager.CurrentProject].Bitmaps[frame],
                destRect,
                sourceRect,
                GraphicsUnit.Pixel);
        }

        /// <summary>
        /// The main animation loop
        /// </summary>
        public void AnimationLoop()
        {
            while (true)
            {
                if (running)
                {
                    if (timer.ElapsedMilliseconds > timeStamp + frameDelay)
                    {
                        timeStamp = timer.ElapsedMilliseconds;
                        PB_Main.Invalidate();
                        frame++;

                        if (frame >= ProjectManager.openProjects[ProjectManager.CurrentProject].Bitmaps.Count)
                        {
                            frame = 0;
                        }
                    }
                }

                Thread.Sleep(1);

            }

        }

        /*
        public void AnimationLoop()
        {
            while (true)
            {
                if(running)
                {
                    if(timer.ElapsedMilliseconds > timeStamp + frameDelay)
                    {
                        timeStamp = timer.ElapsedMilliseconds;
                        pictureBox.Image = ProjectManager.openProjects[ProjectManager.CurrentProject].bitmaps[frame];
                        frame++;

                        if (frame >= ProjectManager.openProjects[ProjectManager.CurrentProject].bitmaps.Count)
                        {
                            frame = 0;
                        }
                    }

                    
                }

                Thread.Sleep(1);

            }

        }
        */
    }
}
