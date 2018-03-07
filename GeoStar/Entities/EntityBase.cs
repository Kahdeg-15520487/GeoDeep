using GeoStar.Items;
using GeoStar.MapObjects;
using GeoStar.Services;
using GoRogue;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
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
        public uint ID { get; private set; }

        public EntityStatus EntityStatus { get; protected set; }

        public Inventory Inventory { get; protected set; }

        public bool HaveVision = false;
        public int VisionRange = 20;
        protected Map map;
        protected FOV fovmap;
        bool isMoved = false;

        protected RandomWrapper random;
        protected TextWriter logger;

        public ItemBase Equip { get; set; }

        public EntityBase(Color foreground, Color background, int glyph, Map map, bool haveVision = false, int visionRange = 20) : base(1, 1)
        {
            Animation.CurrentFrame[0].Foreground = foreground;
            Animation.CurrentFrame[0].Background = background;
            Animation.CurrentFrame[0].Glyph = glyph;

            EntityStatus = new EntityStatus();

            Inventory = new Inventory(EntityStatus.MaxWeight);

            this.map = map;

            HaveVision = haveVision;
            if (HaveVision)
            {
                fovmap = new FOV(map);
                VisionRange = visionRange;
            }

            Equip = new ItemBase("Pickaxe", 10f);
            Equip.ItemBehaviour = ItemBehaviourHelper.Mine();

            random = RandomNumberServiceLocator.GetService();
            logger = LoggingServiceLocator.GetService();

            ID = random.NextUint();
        }

        public virtual void UpdateFov()
        {
            fovmap.Calculate(Position.X, Position.Y, VisionRange, Distance.EUCLIDEAN);
        }

        public bool MoveBy(Direction direction)
        {
            return MoveBy(direction.GetVector());
        }

        public bool MoveBy(Point change)
        {
            var newPosition = Position + change;

            if (newPosition.X < 0 || newPosition.X >= map.Width
             || newPosition.Y < 0 || newPosition.Y >= map.Height)
            {
                return false;
            }

            // Check the map if we can move to this new position
            if (map.IsTileWalkable(newPosition.X, newPosition.Y))
            {
                Position = newPosition;
                return true;
            }
            return false;

        }

        public TileBase CheckTile(Direction dir)
        {
            var target = new Point(position.X, position.Y).GetNearbyPoint(dir);
            var x = target.X;
            var y = target.Y;
            var cellIndex = map.GetCellIndex(x, y);
            return map.Tiles[cellIndex];
        }
    }
}
