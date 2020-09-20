using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GranulateLibrary
{
    public static class FileManager
    {
        public static void SaveAsPng(Bitmap bmp)
        {
            bmp.Save(Path.Combine(Environment.CurrentDirectory,
                "Saves/testSave.png"), ImageFormat.Png);
        }

        
    }
}
