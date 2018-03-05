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
    class ItemPile : TileBase
    {
        public ItemPile(params ItemBase[] items) : base(Color.White, Color.Black, 15)
        {
            IsBlockingLOS = false;
            IsBlockingMove = false;

            Inventory = new Inventory();
            foreach (var item in items)
            {
                Inventory.Add(item);
            }
        }

        public Inventory.InventoryIssue Add(ItemBase item)
        {
            return Inventory.Add(item);
        }
    }
}
