using System;
using System.Collections.Generic;
using System.Text;

namespace GranulateLibrary
{
    public struct Vec2
    {
        public int x;
        public int y;

        public Vec2(int x, int y) => (this.x, this.y) = (x, y);

        public static bool operator ==(Vec2 v1, Vec2 v2) => (v1.x, v1.y) == (v2.x, v2.y);
        public static bool operator !=(Vec2 v1, Vec2 v2) => (v1.x, v1.y) != (v2.x, v2.y);


        public override bool Equals(object obj) =>
            (obj is Vec2 otherV2) ?
            this == otherV2
            : false;

        public override int GetHashCode() => x.GetHashCode() ^ y.GetHashCode();

        public void SwapCoords() => (x, y) = (y, x);
    }
}
