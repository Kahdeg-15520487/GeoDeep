using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoStar.Services
{
    interface IRandomNumberService
    {
        int Next();
    }
    class RandomWrapper : IRandomNumberService, GoRogue.Random.IRandom
    {
        public int Seed { get; private set; }

        Random random;

        public RandomWrapper(int seed = -1)
        {
            if (seed == -1)
            {
                random = new Random();
            }
            else random = new Random(seed);
            Seed = seed;
        }

        public int Next()
        {
            return random.Next();
        }

        public int Next(int max)
        {
            return random.Next(max);
        }

        public int Next(int min, int max)
        {
            return random.Next(min, max);
        }

        public uint NextUint()
        {
            return BitConverter.ToUInt32(BitConverter.GetBytes(random.Next()), 0);
        }
    }

    class RandomNumberServiceLocator
    {
        public static RandomWrapper GetService() => service;
        private static RandomWrapper service;

        public static void Provide(RandomWrapper sv)
        {
            service = sv;
        }
    }
}
