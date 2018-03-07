using System.IO;
using System.Collections.ObjectModel;

using Microsoft.Xna.Framework;

using GoRogue;

using GeoStar.MapObjects;
using GeoStar.Entities;
using GeoStar.Entities.AI;
using GeoStar.Services;
using System.Collections.Generic;

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

        public Map(int width, int height)
        {
            Width = width;
            Height = height;

            // Create our tiles for the map
            Tiles = new TileBase[width * height];

            // Fill the map with floors.
            for (int i = 0; i < Tiles.Length; i++)
                Tiles[i] = new Floor();

            // Holds all entities on the map
            Entities = new ObservableDictionary<Point, EntityBase>();
            logger = LoggingServiceLocator.GetService();
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

        public int GetCellIndex(int x, int y)
        {
            return (y * Width + x).Clamp(0, Width * Height);
        }

        public Grid2D GetGrid(int startX, int startY, int goalX, int goalY)
        {
            Grid2D grid = new Grid2D(Width, Height);

            grid[startX, startY] = new GridNode(grid, startX, startY, 0);
            grid.Start = grid[startX, startY];
            grid[goalX, goalY] = new GridNode(grid, goalX, goalY, 0);
            grid.Goal = grid[goalX, goalY];

            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    if (grid[x, y] != null)
                    {
                        continue;
                    }

                    int index = GetCellIndex(x, y);
                    grid[x, y] = new GridNode(grid, x, y, Tiles[index] is Floor ? 0 : 1);
                }
            }
            return grid;
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
