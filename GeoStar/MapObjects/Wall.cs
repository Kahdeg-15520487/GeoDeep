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
        public Wall() : base(Color.White, Color.Gray, 176)
        {
            IsBlockingLOS = true;
            IsBlockingMove = true;
        }
    }
}
