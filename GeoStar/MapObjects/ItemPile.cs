using GeoStar.Entities;
using GeoStar.Items;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoStar.MapObjects
{
    class ItemPile : Floor
    {
        Floor Floor;

        public ItemPile(Floor floor)
        {
            IsBlockingLOS = false;
            IsBlockingMove = false;

            Floor = floor;

            (DefaultForeground, DefaultBackground, Glyph) = (Color.White, Color.Black, 15);
            Foreground = DefaultForeground;
            Background = DefaultBackground;

            Inventory = Floor.Inventory;
        }

        public override void ReColor()
        {
            base.ReColor();
        }
    }
}
