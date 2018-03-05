using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace GeoStar.MapObjects
{
    class Wall : TileBase
    {
        public int Heath { get; private set; } = 5;

        public Wall() : base(Color.White, Color.Gray, 178)
        {
            IsBlockingLOS = true;
            IsBlockingMove = true;

            DefaultForeground = Color.White;
            DefaultBackground = Color.Gray;
        }

        public void Mine()
        {
            Heath--;
            if (Heath == 3)
            {
                Glyph = 177;
            }
            else if (Heath == 1)
            {
                Glyph = 176;
            }
        }
    }
}
