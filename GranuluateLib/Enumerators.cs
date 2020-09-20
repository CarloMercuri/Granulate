using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GranulateLibrary
{
    public enum ProjectType
    {
        SpriteAnimation = 0,
        SingleSprite = 1,
        TileSheet = 2
    }

    public enum ActionType
    {
        None = 0,
        PixelChange = 1,

    }

    public enum StitchMode
    {
        Horizontal = 0,
        Vertical = 1
    }
}
