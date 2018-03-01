using GeoStar.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoStar
{
    class Map
    {
        public int Width { get; private set; }
        public int Height { get; private set; }

        public ObservableCollection<EntityBase> Entities;

        public MapObjects.TileBase[] Tiles;

        public Map(int width, int height)
        {
            Width = width;
            Height = height;

            // Create our tiles for the map
            Tiles = new MapObjects.TileBase[width * height];

            // Fill the map with floors.
            for (int i = 0; i < Tiles.Length; i++)
                Tiles[i] = new MapObjects.Floor();

            // Set some temp walls, will be replaced by random generation
            // y * width + x = index of that x,y combination
            Tiles[5 * width + 5] = new MapObjects.Wall();
            Tiles[2 * width + 5] = new MapObjects.Wall();
            Tiles[10 * width + 29] = new MapObjects.Wall();
            Tiles[17 * width + 44] = new MapObjects.Wall();

            // Holds all entities on the map
            Entities = new ObservableCollection<EntityBase>();
        }

        public bool IsTileWalkable(int x, int y)
        {
            if (x < 0 || y < 0 || x >= Width || y >= Height)
                return false;

            return !Tiles[y * Width + x].IsBlockingMove;
        }
    }
}
