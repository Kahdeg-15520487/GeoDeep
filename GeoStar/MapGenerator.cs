using GeoStar.MapObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoStar
{
    class MapGenerator
    {
        public static Map Generate(int w, int h, float oreProbability = 30)
        {
            Map map = new Map(w, h);
            GoRogue.MapGeneration.Generators.CellularAutomataGenerator.Generate(map, connectUsingDefault: false);
            //GoRogue.MapGeneration.Generators.RandomRoomsGenerator.Generate(this, 50, 9, 20, 5);
            SpawnMineral(map, oreProbability);

            return map;
        }

        private static void SpawnMineral(Map map, float oreProbability)
        {
            //Simplex.Noise.Seed = 1;
            var oreNoise = Simplex.Noise.Calc2D(map.Width, map.Height, 0.03f);
            var minNoise = oreNoise.Min();
            var maxNoise = oreNoise.Max();
            oreNoise.Map(n => n.Map(minNoise, maxNoise, 0, 99));

            for (int x = 0; x < map.Width; x++)
            {
                for (int y = 0; y < map.Height; y++)
                {
                    int cellIndex = y * map.Width + x;
                    if (map.Tiles[cellIndex] is Wall)
                    {

                        var n = (int)oreNoise[x, y];
                        Mineral.MineralType mineralType = Mineral.MineralType.None;
                        if (1 <= n && n <= 2)
                        {
                            mineralType = Mineral.MineralType.Iron;
                        }
                        else if (30 <= n && n <= 31)
                        {
                            mineralType = Mineral.MineralType.Copper;
                        }
                        else if (50 <= n && n <= 51)
                        {
                            mineralType = Mineral.MineralType.Brass;
                        }
                        else if (91 == n)
                        {
                            mineralType = Mineral.MineralType.WaterCrystal;
                        }

                        if (mineralType != Mineral.MineralType.None)
                        {
                            map.Tiles[cellIndex] = new Mineral(mineralType);
                        }
                    }
                }
            }

            Console.WriteLine(oreNoise.Max());
            Console.WriteLine(oreNoise.Min());
        }
    }
}
