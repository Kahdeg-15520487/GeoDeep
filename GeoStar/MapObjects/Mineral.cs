using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoStar.MapObjects
{
    class Mineral : Wall
    {
        internal enum MineralType
        {
            None,
            Iron,
            Copper,
            Brass,
            WaterCrystal
        }

        internal static Dictionary<MineralType, Color> MineralColors = new Dictionary<MineralType, Color>()
        {
            {MineralType.None, Color.White},
            {MineralType.Iron, Color.DarkSlateBlue},
            {MineralType.Copper, Color.DarkOrange},
            {MineralType.Brass, Color.DarkRed},
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

        public Mineral(MineralType type) : base() {
            Type = type;
        }

        public void ReColor()
        {
            Foreground = MineralColors[type];
        }
    }
}
