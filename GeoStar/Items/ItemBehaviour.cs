using GeoStar.Entities;
using GeoStar.MapObjects;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoStar.Items
{
    /// <summary>
    /// An delegate that perform a Behaviour by user in direction on map with addition infos
    /// </summary>
    /// <param name="user">who use this item</param>
    /// <param name="direction">the direction in which the item is used</param>
    /// <param name="map">the map reference</param>
    /// <param name="infos">addition info</param>
    delegate object ItemBehaviour(EntityBase user, Direction direction, Map map, params object[] infos);

    class ItemBehaviourHelper
    {
        public static ItemBehaviour Nothing()
        {
            return (u, d, m, i) =>
            {
                return null;
            };
        }

        public static ItemBehaviour Mine()
        {
            MineralVein.MineralType Mine(Point position, Direction dir, EntityBase miner, Map map)
            {
                var target = new Point(position.X, position.Y).GetNearbyPoint(dir);
                var x = target.X;
                var y = target.Y;

                var cellIndex = map.GetCellIndex(x, y);
                var minedMineral = MineralVein.MineralType.None;

                if (map.Tiles[cellIndex] is Wall)
                {
                    (map.Tiles[cellIndex] as Wall).Mine();

                    if ((map.Tiles[cellIndex] as Wall).Heath == 0)
                    {
                        if (map.Tiles[cellIndex] is MineralVein)
                        {
                            //get mined mineral
                            minedMineral = (map.Tiles[cellIndex] as MineralVein).Type;
                            //miner.Inventory.Add(new Items.ItemBase(minedMineral.ToColoredString()));
                            miner.Inventory.Add(new ItemBase(minedMineral.ToString(), 5));
                        }

                        map.Tiles[cellIndex] = new Floor();
                    }
                    else
                    {
                    }
                }

                return minedMineral;
            }

            return (u, d, m, o) =>
            {
                return Mine(u.Position, d, u, m);
            };
        }
    }
}
