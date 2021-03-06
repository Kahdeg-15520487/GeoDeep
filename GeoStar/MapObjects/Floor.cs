﻿using GeoStar.Entities;
using GeoStar.Items;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoStar.MapObjects
{
    class Floor : TileBase
    {
        public Floor() : base(new Color(25, 25, 25), Color.Black, 46)
        {
            IsBlockingLOS = false;
            IsBlockingMove = false;

            Inventory = new Inventory(10000, 5);

            DefaultForeground = new Color(25, 25, 25);
            DefaultBackground = Color.Black;
        }

        public Inventory.InventoryIssue Add(ItemBase item)
        {
            return Inventory.Add(item);
        }
    }
}
