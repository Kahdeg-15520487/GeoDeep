using GeoStar.Items;
using GeoStar.MapObjects;
using GoRogue;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoStar.Entities
{
    class EntityStatus
    {
        public int MaxHealth { get; private set; } = 100;
        public int MaxEnergy { get; private set; } = 100;
        public int MaxSP { get; private set; } = 100;

        public int Health { get; private set; } = 100;
        public int Energy { get; private set; } = 100;
        public int SpritualPower { get; private set; } = 100;

        public float MaxWeight { get; private set; } = 100f;
    }

    class EntityBase : SadConsole.GameHelpers.GameObject, IHasID
    {
        static uint lastID = 0;
        private uint id;
        public uint ID { get => id; }

        public EntityStatus EntityStatus { get; protected set; }

        public Inventory Inventory { get; protected set; }

        public EntityBase(Color foreground, Color background, int glyph) : base(1, 1)
        {
            Animation.CurrentFrame[0].Foreground = foreground;
            Animation.CurrentFrame[0].Background = background;
            Animation.CurrentFrame[0].Glyph = glyph;

            id = lastID++;

            EntityStatus = new EntityStatus();

            Inventory = new Inventory(EntityStatus.MaxWeight);
        }

        public void MoveBy(Point change, Map map)
        {
            var newPosition = Position + change;

            if (newPosition.X < 0 || newPosition.X >= map.Width
             || newPosition.Y < 0 || newPosition.Y >= map.Height)
            {
                return;
            }

            // Check the map if we can move to this new position
            if (map.IsTileWalkable(newPosition.X, newPosition.Y))
            {
                Position = newPosition;
            }
            else
            {
                //mine that wall
                if (Mine(map, newPosition.X, newPosition.Y, this, out MapObjects.MineralVein.MineralType mineralType))
                {
                    //that wall is mined
                    Position = newPosition;
                }
            }
        }

        public bool Mine(Map map, int x, int y, EntityBase miner, out MineralVein.MineralType minedMineral)
        {
            var cellIndex = y * map.Width + x;
            minedMineral = MineralVein.MineralType.None;

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
                        miner.Inventory.Add(new Items.ItemBase(minedMineral.ToString(), 5));
                    }

                    map.Tiles[cellIndex] = new Floor();
                    return true;
                }
                else
                {
                    return false;
                }
            }

            return false;
        }
    }
}
