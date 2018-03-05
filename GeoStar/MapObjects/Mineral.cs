using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoStar.MapObjects
{
    class MineralVein : Wall
    {
        internal enum MineralType
        {
            None,
            Rock,
            Iron,
            Copper,
            Tin,
            Coal,

            MetalCrystal,
            WoodCrystal,
            FireCrystal,
            WaterCrystal,
            EarthCrystal
        }

        internal static Dictionary<MineralType, Color> MineralColors = new Dictionary<MineralType, Color>()
        {
            {MineralType.None, Color.White},
            {MineralType.Rock, Color.DimGray},
            {MineralType.Coal, Color.DarkSlateGray},
            {MineralType.Iron, Color.DarkSlateBlue},
            {MineralType.Copper, Color.DarkOrange},
            {MineralType.Tin, Color.DarkRed},
            {MineralType.MetalCrystal, Color.Silver},
            {MineralType.WaterCrystal, Color.DarkCyan}
        };

        MineralType type = MineralType.None;
        internal MineralType Type
        {
            get => type;
            set
            {
                type = value;

                Foreground = MineralColors[type];
            }
        }

        public MineralVein(MineralType type) : base()
        {
            Type = type;
        }

        public override void ReColor()
        {
            base.ReColor();
            Foreground = MineralColors[type];
        }
    }
}
