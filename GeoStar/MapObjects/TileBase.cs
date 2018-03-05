using Microsoft.Xna.Framework;
using SadConsole;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoStar.MapObjects
{
    class TileBase : Cell
    {
        public bool IsBlockingMove;
        public bool IsBlockingLOS;

        public TileBase(Color foreground, Color background, int glyph) : base(foreground, background, glyph)
        {
            IsVisible = false;
        }
    }
}
