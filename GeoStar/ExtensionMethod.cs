using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoStar
{
    public enum Direction
    {
        NorthWest, North, NorthEast,
        West, Center, East,
        SouthWest, South, SouthEast,
        Void
    }

    static class ExtensionMethod
    {
        public static Color Darken(this Color c)
        {
            return new Color(c.R * 0.5f, c.G * 0.5f, c.B * 0.5f);
        }

        public static string ToColoredString(this MapObjects.Mineral.MineralType mineralType)
        {
            var mcolor = MapObjects.Mineral.MineralColors[mineralType];

            return string.Format("[c:r f:{1},{2},{3}]{0}", mineralType, mcolor.R, mcolor.G, mcolor.B);
        }

        public static float Map(this float value, float fromSource, float toSource, float fromTarget, float toTarget)
        {
            return (value - fromSource) / (toSource - fromSource) * (toTarget - fromTarget) + fromTarget;
        }

        public static T Min<T>(this T[,] arr) where T : IComparable
        {
            bool minSet = false;
            T min = arr[0, 0];
            for (int i = 0; i < arr.GetLength(0); i++)
            {
                for (int j = 0; j < arr.GetLength(1); j++)
                {
                    if (!minSet)
                    {
                        minSet = true;
                        min = arr[i, j];
                    }
                    else if (arr[i, j].CompareTo(min) < 0)
                        min = arr[i, j];
                }
            }
            return min;
        }

        public static T Max<T>(this T[,] arr) where T : IComparable
        {
            bool maxSet = false;
            T max = arr[0, 0];
            for (int i = 0; i < arr.GetLength(0); i++)
            {
                for (int j = 0; j < arr.GetLength(1); j++)
                {
                    if (!maxSet)
                    {
                        maxSet = true;
                        max = arr[i, j];
                    }
                    else if (arr[i, j].CompareTo(max) > 0)
                        max = arr[i, j];
                }
            }
            return max;
        }

        public static void Map(this float[,] arr, Func<float, float> mapper)
        {
            for (int i = 0; i < arr.GetLength(0); i++)
            {
                for (int j = 0; j < arr.GetLength(1); j++)
                {
                    arr[i, j] = mapper(arr[i, j]);
                }
            }
        }

        public static Point GetNearbyPoint(this Point p, Direction d)
        {
            Point output = new Point(p.X, p.Y);
            switch (d)
            {
                case Direction.NorthWest:
                    output = new Point(p.X - 1, p.Y - 1);
                    break;
                case Direction.North:
                    output = new Point(p.X, p.Y - 1);
                    break;
                case Direction.NorthEast:
                    output = new Point(p.X + 1, p.Y - 1);
                    break;
                case Direction.West:
                    output = new Point(p.X - 1, p.Y);
                    break;
                case Direction.East:
                    output = new Point(p.X + 1, p.Y);
                    break;
                case Direction.SouthWest:
                    output = new Point(p.X - 1, p.Y + 1);
                    break;
                case Direction.South:
                    output = new Point(p.X, p.Y + 1);
                    break;
                case Direction.SouthEast:
                    output = new Point(p.X + 1, p.Y + 1);
                    break;
                default:
                    break;
            }
            return output;
        }
    }
}
