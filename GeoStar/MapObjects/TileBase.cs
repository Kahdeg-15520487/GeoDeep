using Microsoft.Xna.Framework;

using SadConsole;

using GeoStar.Items;
using GoRogue;

namespace GeoStar.MapObjects
{
    class TileBase : Cell, IHasID
    {
        static uint LastID = 0;
        public uint ID { get; set; }

        public bool IsBlockingMove;
        public bool IsBlockingLOS;

        public Color DefaultForeground;
        public Color DefaultBackground;

        public Inventory Inventory { get; set; }

        public TileBase(Color foreground, Color background, int glyph) : base(foreground, background, glyph)
        {
            IsVisible = false;
            ID = LastID++;
        }

        public virtual void ReColor()
        {
            Foreground = DefaultForeground;
            Background = DefaultBackground;
        }
    }
}
