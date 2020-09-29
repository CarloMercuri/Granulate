using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GranulateLibrary
{
    public static class RayCast2D
    {
        private static void Swap<T>(ref T a, ref T b)
        {
            T c = a;
            a = b;
            b = c;
        }

        /// <summary>
        /// Performs a "Raycast" that returns all the intersected points between two points.
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        public static List<Vec2> RayCast(Vec2 v0, Vec2 v1)
        {
            return RayCast(v0.x, v0.y, v1.x, v1.y);
        }

        // Found on StackOverflow

        private static List<Vec2> RayCast(int x0, int y0, int x1, int y1)
        {

            // modified version of the Bresenham algorithm that only returns integral numbers
            List<Vec2> result = new List<Vec2>();

            
            bool steep = Math.Abs(y1 - y0) > Math.Abs(x1 - x0);
            if (steep)
            {
                Swap(ref x0, ref y0);
                Swap(ref x1, ref y1);
            }
            if (x0 > x1)
            {
                Swap(ref x0, ref x1);
                Swap(ref y0, ref y1);
            }


            int deltax = x1 - x0;
            int deltay = Math.Abs(y1 - y0);
            int error = 0;
            int ystep;
            int y = y0;

            if (y0 < y1)
            {
                ystep = 1;
            }
            else
            {
                ystep = -1;
            }

            for (int x = x0; x <= x1; x++)
            {
                if (steep)
                {
                    result.Add(new Vec2(y, x));
                }
                else
                {
                    result.Add(new Vec2(x, y));
                }
                error += deltay;
                if (2 * error >= deltax)
                {
                    y += ystep;
                    error -= deltax;
                }
            }

            return result;
        }
    }
}
