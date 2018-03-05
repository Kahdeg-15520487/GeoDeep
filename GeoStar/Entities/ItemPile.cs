using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeoStar.Items;
using Microsoft.Xna.Framework;

namespace GeoStar.Entities
{
    class ItemPile : EntityBase
    {
        public ItemPile(params ItemBase[] items) : base(Color.White, Color.Black, 240)
        {
            Inventory = new Inventory();
            foreach (var item in items)
            {
                Inventory.Add(item);
            }
        }
    }
}
