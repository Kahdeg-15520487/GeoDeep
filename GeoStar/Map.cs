
using System;
using System.IO;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;

using GoRogue;

using GeoStar.MapObjects;
using GeoStar.Entities;
using GeoStar.Services;
using System.Collections;

namespace GeoStar
{
    class Map : ISettableMapView<bool>, IMapView<double>
    {
        TextWriter logger;

        public int Width { get; private set; }
        public int Height { get; private set; }

        double IMapView<double>.this[Coord pos] => GetTileBlockingLOS(pos.X, pos.Y) ? 0 : 1;

        double IMapView<double>.this[int x, int y] => GetTileBlockingLOS(x, y) ? 0 : 1;

        public bool this[Coord pos]
        {
            get => GetTileWalkable(pos.X, pos.Y);
            set
            {
                SetTileWalkable(pos.X, pos.Y, value);
            }
        }

        public bool this[int x, int y]
        {
            get => GetTileWalkable(x, y);
            set
            {
                SetTileWalkable(x, y, value);
            }
        }

        private bool GetTileWalkable(int x, int y)
        {
            return IsTileWalkable(x, y);
        }

        private void SetTileWalkable(int x, int y, bool isWalkable)
        {
            Tiles[y * Width + x] = isWalkable ? new Floor() : (TileBase)new Wall();
        }

        private bool GetTileBlockingLOS(int x, int y)
        {
            return IsTileBlockingLOS(x, y);
        }

        private void SetTileBlockingLOS(int x, int y, bool isBlockingLOS)
        {
            Tiles[y * Width + x].IsBlockingLOS = isBlockingLOS;
        }

        public ObservableDictionary<Point, EntityBase> Entities;

        public TileBase[] Tiles;

        public GoRogue.FOV fovmap;

        public Map(int width, int height)
        {
            Width = width;
            Height = height;

            // Create our tiles for the map
            Tiles = new TileBase[width * height];

            // Fill the map with floors.
            for (int i = 0; i < Tiles.Length; i++)
                Tiles[i] = new Floor();

            fovmap = new FOV(this);

            // Holds all entities on the map
            Entities = new ObservableDictionary<Point, EntityBase>();
            logger = LoggingServiceLocator.GetLoggingService();
        }

        public void UpdateFOV(int Px, int Py)
        {
            fovmap.Calculate(Px, Py, 20, Distance.EUCLIDEAN);
            foreach (var ns in fovmap.NewlySeen)
            {
                var tileIndex = ns.Y * Width + ns.X;
                if (Tiles[tileIndex] is Floor)
                {
                    Tiles[tileIndex].ReColor();
                }
                else if (Tiles[tileIndex] is MineralVein)
                {
                    (Tiles[tileIndex] as MineralVein).ReColor();
                    if (!Tiles[tileIndex].IsVisible)
                    {
                        var mcolor = Tiles[tileIndex].Foreground;
                        logger.WriteLine("You found a vein of [c:r f:{1},{2},{3}]{0}", (Tiles[tileIndex] as MineralVein).Type, mcolor.R, mcolor.G, mcolor.B);
                    }
                }
                else if (Tiles[tileIndex] is Wall)
                {
                    Tiles[tileIndex].ReColor();
                }
                Tiles[tileIndex].IsVisible = true;
            }

            foreach (var ns in fovmap.NewlyUnseen)
            {
                var tileIndex = ns.Y * Width + ns.X;
                if (Tiles[tileIndex] is Floor)
                {
                    Tiles[tileIndex].Foreground = new Color(10, 10, 10);
                }
                else if (Tiles[tileIndex] is Wall)
                {
                    Tiles[tileIndex].Foreground = Color.Gray;
                }
            }
        }

        public bool IsTileWalkable(int x, int y)
        {
            if (x < 0 || y < 0 || x >= Width || y >= Height)
                return false;

            if (Entities.ContainsKey(new Point(x, y)))
            {
                return false;
            }

            return !Tiles[y * Width + x].IsBlockingMove;
        }
        public bool IsTileWalkable(Point p)
        {
            return IsTileWalkable(p.X, p.Y);
        }

        public bool IsTileBlockingLOS(int x, int y)
        {
            if (x < 0 || y < 0 || x >= Width || y >= Height)
                return false;

            return !Tiles[y * Width + x].IsBlockingLOS;
        }
        public bool IsTileBlockingLOS(Point p)
        {
            return IsTileBlockingLOS(p.X, p.Y);
        }

        public bool AddEntity(EntityBase entity)
        {
            var key = entity.Position;
            if (Entities.ContainsKey(key))
            {
                return false;
            }
            else
            {
                Entities.Add(key, entity);
                return true;
            }
        }

        public bool IsEntityThere(Point p)
        {
            return Entities.ContainsKey(p);
        }

        public bool FindEmptyPointAround(Point center, out Point result)
        {
            var point = center.GetNearbyPoint(Direction.North);
            if (!IsEntityThere(point) && IsTileWalkable(point))
            {
                goto end;
            }

            point = center.GetNearbyPoint(Direction.East);
            if (!IsEntityThere(point) && IsTileWalkable(point))
            {
                goto end;
            }

            point = center.GetNearbyPoint(Direction.South);
            if (!IsEntityThere(point) && IsTileWalkable(point))
            {
                goto end;
            }

            point = center.GetNearbyPoint(Direction.West);
            if (!IsEntityThere(point) && IsTileWalkable(point))
            {
                goto end;
            }

            result = point;
            return false;

end:
            result = point;
            return true;
        }
    }
}
