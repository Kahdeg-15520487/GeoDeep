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
            Console.WriteLine("generating a {0}x{1} map", w, h);
            Map map = new Map(w, h);
            GoRogue.MapGeneration.Generators.CellularAutomataGenerator.Generate(map, connectUsingDefault: false);
            //GoRogue.MapGeneration.Generators.RandomRoomsGenerator.Generate(this, 50, 9, 20, 5);
            SpawnMineral(map, oreProbability);

            return map;
        }

        struct OreSpawnSetting
        {
            public OreSpawnSetting(MineralVein.MineralType oretype, int maxspawncount, int veinmaxlength, int spawnprobability) : this()
            {
                OreType = oretype;
                MaxSpawnCount = maxspawncount;
                MaxVeinLength = veinmaxlength;
                SpawnProbability = spawnprobability;
            }

            public MineralVein.MineralType OreType { get; set; }
            public int MaxSpawnCount { get; set; }
            public int MaxVeinLength { get; set; }
            public int SpawnProbability { get; set; }
        }

        private static void SpawnMineral(Map map, float oreProbability)
        {
            List<OreSpawnSetting> list = new List<OreSpawnSetting>()
            {
                new OreSpawnSetting(MineralVein.MineralType.Rock,150,10,60),
                new OreSpawnSetting(MineralVein.MineralType.Coal,100,7,60),
                new OreSpawnSetting(MineralVein.MineralType.Copper,60,5,60),
                new OreSpawnSetting(MineralVein.MineralType.Tin,60,5,40),
                new OreSpawnSetting(MineralVein.MineralType.Iron,40,5,30),
                new OreSpawnSetting(MineralVein.MineralType.MetalCrystal,20,3,15),
                new OreSpawnSetting(MineralVein.MineralType.WaterCrystal,20,3,15)
            };

            Random random = new Random();
            foreach (var ore in list)
            {
                int spawncount = 0;
                while (spawncount < ore.MaxSpawnCount)
                {
                    int x = random.Next(0, map.Width);
                    int y = random.Next(0, map.Height);

                    if (map.Tiles[y * map.Width + x] is Wall)
                    {
                        if (random.Next(0, 100) < ore.SpawnProbability)
                        {
                            spawncount += SpawnOreVein(x, y, ore);
                        }
                    }
                }
                Console.WriteLine("spawned {1} {0} cell", ore.OreType, spawncount);
            }

            int SpawnOreVein(int x, int y, OreSpawnSetting ore)
            {
                int veinLength = 1;
                Direction dir;

                map.Tiles[y * map.Width + x] = new MineralVein(ore.OreType);

                while (veinLength < ore.MaxVeinLength)
                {
                    dir = (Direction)random.Next(0, 9);
                    if (dir == Direction.Center || dir == Direction.Void)
                    {
                        break;
                    }

                    HelperMethod.GetNearbyPoint(x, y, dir, out int xd, out int yd);

                    int cellIndex = yd * map.Width + xd;
                    if ((cellIndex < 0) || (cellIndex > (map.Width * map.Height - 1)))
                    {
                        break;
                    }

                    map.Tiles[cellIndex] = new MineralVein(ore.OreType);
                    veinLength++;
                }

                return veinLength;
            }
        }

        //private static void SpawnMineral(Map map, float oreProbability)
        //{
        //    //Simplex.Noise.Seed = 1;
        //    var oreNoise = Simplex.Noise.Calc2D(map.Width, map.Height, 0.03f);
        //    var minNoise = oreNoise.Min();
        //    var maxNoise = oreNoise.Max();
        //    oreNoise.Map(n => n.Map(minNoise, maxNoise, 0, 99));

        //    for (int x = 0; x < map.Width; x++)
        //    {
        //        for (int y = 0; y < map.Height; y++)
        //        {
        //            int cellIndex = y * map.Width + x;
        //            if (map.Tiles[cellIndex] is Wall)
        //            {

        //                var n = (int)oreNoise[x, y];
        //                Mineral.MineralType mineralType = Mineral.MineralType.None;
        //                if (1 <= n && n <= 2)
        //                {
        //                    mineralType = Mineral.MineralType.Iron;
        //                }
        //                else if (30 <= n && n <= 31)
        //                {
        //                    mineralType = Mineral.MineralType.Copper;
        //                }
        //                else if (50 <= n && n <= 51)
        //                {
        //                    mineralType = Mineral.MineralType.Tin;
        //                }
        //                else if (91 == n)
        //                {
        //                    mineralType = Mineral.MineralType.WaterCrystal;
        //                }

        //                if (mineralType != Mineral.MineralType.None)
        //                {
        //                    map.Tiles[cellIndex] = new Mineral(mineralType);
        //                }
        //            }
        //        }
        //    }

        //    Console.WriteLine(oreNoise.Max());
        //    Console.WriteLine(oreNoise.Min());
        //}
    }
}
