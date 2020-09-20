using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;



namespace GranulateLibrary
{
    public static class ImageEditing
    {
        
        

        /// <summary>
        /// Returns a bitmap with a SQUARE checkered background image, of the specified size
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public static Bitmap CreateBackgroundImage(int size)
        {
            // For now fixed spacing
            int spacing = 10;

            // The two types of gray
            int lightColor = 211;
            int darkColor = 169;

            Color _Color1 = Color.FromArgb(255, lightColor, lightColor, lightColor);
            Color _Color2 = Color.FromArgb(255, darkColor, darkColor, darkColor);

            Color curColor = _Color1;

            Bitmap _bitMap = new Bitmap(size, size);

            for (int x = 0; x < size; x += spacing)
            {
                for (int y = 0; y < size; y += spacing)
                {
                    FillRectangle(curColor, new Rectangle(x, y, spacing, spacing), ref _bitMap);

                    // Switch the colors around
                    curColor = (curColor == _Color1) ? _Color2 : _Color1;

                }

                // Switch the colors around
                curColor = (curColor == _Color1) ? _Color2 : _Color1;

            }

           
            return _bitMap;

        }


        /// <summary>
        /// Undoes the pixel changes specified in the references ActionPixelModification
        /// </summary>
        /// <param name="pMod"></param>
        public static void UndoPixelModification(ActionPixelModification pMod)
        {
            // go through all the pixels and apply the old color value
            for (int i = 0; i < pMod.pixelsList.Count; i++)
            {
                ProjectManager.openProjects[ProjectManager.CurrentProject].bitmaps[pMod.AffectedBitmapIndex]
                    .SetPixel(pMod.pixelsList[i].pixelLoc.x, pMod.pixelsList[i].pixelLoc.y,
                    pMod.pixelsList[i].oldColor);
            }
            
            // FRONTEND
            //GUI.ActionPixelsModified(pMod.pixelsList, pMod.AffectedBitmapIndex, true);
            

        }

        /// <summary>
        /// Apply changes from a list of PixelModification
        /// </summary>
        /// <param name="pixelList"></param>
        /// <param name="projectID"></param>
        /// <param name="currentImage"></param>
        public static void ModifyPixels(List<PixelModification> pixelList, int projectID, int currentImage)
        {
            foreach (PixelModification pMod in pixelList)
            {
                ProjectManager.openProjects[projectID].bitmaps[pMod.bitmapID].SetPixel(
                    pMod.pixelLoc.x, pMod.pixelLoc.y, pMod.newColor);
            }

            // FRONTEND 
            //GUI.ActionPixelsModified(pixelList, currentImage, false);

        }

        /// <summary>
        /// Returns a new version of the bitmap, zoomed to the desired amount
        /// </summary>
        /// <param name="original"></param>
        /// <param name="zoom"></param>
        /// <returns></returns>
        public static Bitmap GetZoomedImage(Bitmap original, int zoom)
        {

            Bitmap bmp = new Bitmap(original.Width * zoom, original.Height * zoom);

            Color color;

            int newX = 0;
            int newY = 0;

            for (int x = 0; x < original.Width; x++)
            {
                newY = 0;

                for (int y = 0; y < original.Height; y++)
                {
                    color = original.GetPixel(x, y);
                   
                    FillRectangle(color,
                        new Rectangle(newX, newY, zoom, zoom),
                        ref bmp);
                    
                    newY += zoom;
                }

                newX += zoom;
            }


            return bmp;
        }

        /// <summary>
        /// Clamps a value between two values
        /// </summary>
        /// <param name="value"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        private static int Clamp(int value, int min, int max)
        {
            return (value < min) ? min : (value > max) ? max : value;
        }

        /// <summary>
        /// Makes sure that a rectangle fits into an image
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="imageSize"></param>
        /// <returns></returns>
        private static Rectangle ClampRectangleInImage(Rectangle rect, Size imageSize)
        {
            return new Rectangle(
                rect.X,
                rect.Y,
                (rect.X + rect.Width > imageSize.Width) ? imageSize.Width - rect.X : rect.Width,
                (rect.Y + rect.Height> imageSize.Height) ? imageSize.Height - rect.Y : rect.Height);
        }

        
 
        /// <summary>
        /// Fills a section of a referenced image based on a rectangle
        /// </summary>
        /// <param name="color"></param>
        /// <param name="rectangle"></param>
        /// <param name="image"></param>
        public unsafe static void FillRectangle(Color color, Rectangle rectangle, ref Bitmap image)
        {

            // EXTREMELY IMPORTANT CHECK. We're working with pointers, and if we try to change
            // a memory address that does not belong to the image, bad things happen
            // For now I'd rather check this twice (if i already check it before calling this 
            // method, than have a catastrophic error.

            rectangle = ClampRectangleInImage(rectangle, image.Size);

            // Lock the bitmap's bits
            BitmapData blurredData = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadWrite, image.PixelFormat);

            // Get bits per pixel for current PixelFormat
            int bitsPerPixel = Image.GetPixelFormatSize(image.PixelFormat);

            // Get pointer to first line
            byte* scan0 = (byte*)blurredData.Scan0.ToPointer();

            // look at every pixel in the rectangle
            for (int xx = rectangle.X; xx < rectangle.X + rectangle.Width; xx++)
            {
                for (int yy = rectangle.Y; yy < rectangle.Y + rectangle.Height; yy++)
                {

                    // Get pointer to RGB
                    byte* data = scan0 + yy * blurredData.Stride + xx * bitsPerPixel / 8;

                    // Change values
                    data[0] = color.B;
                    data[1] = color.G;
                    data[2] = color.R;
                    data[3] = color.A;
                }
            }

            // Unlock the bits
            image.UnlockBits(blurredData);

            
        }

        public static Bitmap CreatecolorSlider()
        {

            return null;
        }

        // Creates the vertical Color Selector image
        public static Bitmap CreateColorSelectorImage()
        {
            Size bmpSize = new Size(20, 120);
            int intervals = 20;
            Bitmap bmp = new Bitmap(bmpSize.Width, bmpSize.Height);

            // 255 0 0 to 255 0 255
            LinearGradientBrush brush = new LinearGradientBrush(
                new Point(0, 0),
                new Point(0, 20),
                Color.FromArgb(255, 255, 0, 0),
                Color.FromArgb(255, 255, 0, 255));

            Graphics graphics = Graphics.FromImage(bmp);

            graphics.FillRectangle(brush, new Rectangle(0, 0, bmpSize.Width, intervals));

            // 255 0 255 to 0 0 255
            brush = new LinearGradientBrush(
                new Point(0, 20),
                new Point(0, 40),
                Color.FromArgb(255, 255, 0, 255),
                Color.FromArgb(255, 0, 0, 255));

            graphics.FillRectangle(brush, new Rectangle(0, 20, bmpSize.Width, intervals));

            // 0 0 255 to 0 255 255
            brush = new LinearGradientBrush(
                new Point(0, 40),
                new Point(0, 60),
                Color.FromArgb(255, 0, 0, 255),
                Color.FromArgb(255, 0, 255, 255));

            graphics.FillRectangle(brush, new Rectangle(0, 40, bmpSize.Width, intervals));

            // from 0 255 255 to 0 255 0
            brush = new LinearGradientBrush(
                new Point(0, 60),
                new Point(0, 80),
                Color.FromArgb(255, 0, 255, 255),
                Color.FromArgb(255, 0, 255, 0));

            graphics.FillRectangle(brush, new Rectangle(0, 60, bmpSize.Width, intervals));

            // from 0 255 0 to 255 255 0
            brush = new LinearGradientBrush(
                new Point(0, 80),
                new Point(0, 100),
                Color.FromArgb(255, 0, 255, 0),
                Color.FromArgb(255, 255, 255, 0));

            graphics.FillRectangle(brush, new Rectangle(0, 80, bmpSize.Width, intervals));

            // from 255 255 0 to 255 0 0
            brush = new LinearGradientBrush(
                new Point(0, 100),
                new Point(0, 120),
                Color.FromArgb(255, 255, 255, 0),
                Color.FromArgb(255, 255, 0, 0));

            graphics.FillRectangle(brush, new Rectangle(0, 100, bmpSize.Width, intervals));

            return bmp;
        }

        /// <summary>
        /// Creates the main color palette selection image, based on the input color
        /// </summary>
        /// <param name="selectedColor"></param>
        /// <returns></returns>
        public static Bitmap CreateColorPaletteImage(Color selectedColor)
        {
            int bitmapSize = 121;
            Color rowStartColor = Color.FromArgb(255, 255, 255, 255);
            Color rowEndColor = selectedColor;
            Bitmap bmp = new Bitmap(bitmapSize, bitmapSize);


            // Start from 255 255 255 on the top left and interpolate down to 0 0 0
            // Start from the selected color on the top right and interpolate down to 0 0 0
            // Every line is going to be interpolated from left to right, from the current
            // color at the start of the line, to the current color at the end of the line
            decimal redStartInterp = (decimal)rowStartColor.R / (bitmapSize);
            decimal greenStartInterp = (decimal)rowStartColor.G / (bitmapSize);
            decimal blueStartInterp = (decimal)rowStartColor.B / (bitmapSize);

            decimal redEndInterp = (decimal)selectedColor.R / (bitmapSize);
            decimal greenEndInterp = (decimal)selectedColor.G / (bitmapSize);
            decimal blueEndInterp = (decimal)selectedColor.B / (bitmapSize);

            // Temporary Start Red, Green, Blue
            decimal tempSR = rowStartColor.R;
            decimal tempSG = rowStartColor.G;
            decimal tempSB = rowStartColor.B;

            // Temporary End Red, Green, Blue
            decimal tempER = rowEndColor.R;
            decimal tempEG = rowEndColor.G;
            decimal tempEB = rowEndColor.B;

            Graphics graphics = Graphics.FromImage(bmp);

            // Each line is going to be a linear gradient 
            LinearGradientBrush brush = new LinearGradientBrush(
                new Point(0, 0),
                new Point(bmp.Width, 0),
                rowStartColor,
                rowEndColor);


            // Still work to do
            for (int y = 0; y < bitmapSize; y++)
            {
                graphics.DrawLine(new Pen(brush), new Point(0, y), new Point(bmp.Width, y));

                tempSR -= redStartInterp;

                if (tempSR < 3)
                    tempSR = 0;

                tempSG -= greenStartInterp;
                if (tempSG < 3)
                    tempSG = 0;

                tempSB -= blueStartInterp;
                if (tempSB < 3)
                    tempSB = 0;

                rowStartColor = Color.FromArgb(255, (int)Math.Round(tempSR), (int)Math.Round(tempSG), (int)Math.Round(tempSB));


                tempER -= redEndInterp;
                if (tempER < 3)
                    tempER = 0;
                tempEG -= greenEndInterp;
                if (tempEG < 3)
                    tempEG = 0;
                tempEB -= blueEndInterp;
                if (tempEB < 3)
                    tempEB = 0;



                rowEndColor = Color.FromArgb(255, (int)Math.Round(tempER), (int)Math.Round(tempEG), (int)Math.Round(tempEB));


                brush = new LinearGradientBrush(
                new Point(0, y),
                new Point(bmp.Width, y),
                rowStartColor,
                rowEndColor);

            }


                return bmp;
        }
       

        /// <summary>
        /// Stiches the images together horizontally or vertically
        /// </summary>
        /// <param name="prj"></param>
        /// <param name="horizontal"></param>
        /// <returns></returns>
        public static Bitmap StitchBitmapsBasic(ProjectData prj, StitchMode stitchmode)
        {
            Bitmap stitchedBmp;

            if(stitchmode == StitchMode.Horizontal)
            {
                // If horizontal, the width of the new image is going to be the sum of the width of each image in the project
                stitchedBmp = new Bitmap(prj.ImageWidth * prj.bitmaps.Count, prj.ImageHeight);
                Graphics grPhoto = Graphics.FromImage(stitchedBmp);

                for (int i = 0; i < prj.bitmaps.Count; i++)
                {
                    grPhoto.DrawImage(prj.bitmaps[i], new Point(prj.ImageWidth * i, 0));
                }
            }
            else
            {
                // If vertical, the height of the new image is going to be the sum of the heights of each image in the project
                stitchedBmp = new Bitmap(prj.ImageWidth, prj.ImageHeight * prj.bitmaps.Count);
                Graphics grPhoto = Graphics.FromImage(stitchedBmp);

                for (int i = 0; i < prj.bitmaps.Count; i++)
                {
                    grPhoto.DrawImage(prj.bitmaps[i], new Point(0, prj.ImageHeight * i));
                }
            }

            

            return stitchedBmp;
        }

    }
}
