using Microsoft.Xna.Framework;

using SadConsole;

using GeoStar.Items;

namespace GeoStar.MapObjects
{
    class TileBase : Cell
    {
        public bool IsBlockingMove;
        public bool IsBlockingLOS;

        public Color DefaultForeground;
        public Color DefaultBackground;

        public Inventory Inventory { get; protected set; }

        public TileBase(Color foreground, Color background, int glyph) : base(foreground, background, glyph)
        {
            IsVisible = false;
        }

        public virtual void ReColor()
        {
            Foreground = DefaultForeground;
            Background = DefaultBackground;
        }
    }
}
