using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace GeoStar.Entities
{
    class NPC : EntityBase
    {
        public NPC(Color foreground, Color background, int glyph, Map map, bool haveVision, int visionRange) : base(foreground, background, glyph, map, haveVision, visionRange)
        {

        }

        public virtual void Act()
        {

        }
    }
}
