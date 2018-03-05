using GeoStar.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoStar.Items
{

    delegate void ItemBehaviour(EntityBase user,EntityBase target);

    class ItemBehaviourHelper
    {
        public static ItemBehaviour Nothing()
        {
            return (u, t) =>
            {

            };
        }
    }
}
