using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;

using GeoStar.MapObjects;
using GeoStar.Services;

namespace GeoStar.Entities
{
    class Player : EntityBase
    {
        private TextWriter logger;

        public Player(Map map) : base(Color.Yellow, Color.Black, '@', map, true)
        {
            logger = LoggingServiceLocator.GetService();
            this.map = map;
        }

        public override void UpdateFov()
        {
            base.UpdateFov();


            foreach (var ns in fovmap.NewlySeen)
            {
                var tileIndex = ns.Y * fovmap.Width + ns.X;
                if (map.Tiles[tileIndex] is ItemPile)
                {
                    map.Tiles[tileIndex].ReColor();
                }
                else if (map.Tiles[tileIndex] is Floor)
                {
                    map.Tiles[tileIndex].ReColor();
                }
                else if (map.Tiles[tileIndex] is MineralVein)
                {
                    (map.Tiles[tileIndex] as MineralVein).ReColor();
                    if (!map.Tiles[tileIndex].IsVisible)
                    {
                        var mcolor = map.Tiles[tileIndex].Foreground;
                        logger.WriteLine("You found a vein of [c:r f:{1},{2},{3}]{0}", (map.Tiles[tileIndex] as MineralVein).Type, mcolor.R, mcolor.G, mcolor.B);
                    }
                }
                else if (map.Tiles[tileIndex] is Wall)
                {
                    map.Tiles[tileIndex].ReColor();
                }
                map.Tiles[tileIndex].IsVisible = true;
            }

            foreach (var ns in fovmap.NewlyUnseen)
            {
                var tileIndex = ns.Y * fovmap.Width + ns.X;
                if (map.Tiles[tileIndex] is Floor)
                {
                    map.Tiles[tileIndex].Foreground = new Color(10, 10, 10);
                }
                else if (map.Tiles[tileIndex] is Wall)
                {
                    map.Tiles[tileIndex].Foreground = Color.Gray;
                }
            }
        }
    }
}
