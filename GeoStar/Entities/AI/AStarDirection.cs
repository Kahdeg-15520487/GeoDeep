using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AStar;
using GeoStar.MapObjects;

namespace GeoStar.Entities.AI
{
    internal class AStarDirection : AStar.AStar
    {
        public AStarDirection(INode start, INode goal) : base(start, goal)
        {
        }

        /// <summary>
        /// Gets the path of the last solution of the AStar algorithm.
        /// Will return a partial path if the algorithm has not finished yet.
        /// </summary>
        /// <returns>Returns null if the algorithm has never been run.</returns>
        public IEnumerable<(Direction, bool)> GetPath(Map map)
        {
            if (current != null)
            {
                var next = current;
                var path = new List<(Direction, bool)>();
                while (next.Parent != null)
                {
                    var parent = next.Parent as GridNode;
                    var n = next as GridNode;
                    var d = HelperMethod.GetDirectionFromPointAtoPointB(parent.X, parent.Y, n.X, n.Y);
                    var mapCell = map.Tiles[map.GetCellIndex(n.X, n.Y)];

                    if (mapCell is Floor)
                    {
                        path.Add((d, false));
                    }
                    else
                    {
                        //path.Add((d, false));
                        var timeToDig = mapCell is Wall ? (mapCell as Wall).Heath : 1;
                        for (int i = 0; i < timeToDig; i++)
                        {
                            path.Add((d, true));
                        }
                    }

                    next = next.Parent;
                }
                path.Reverse();
                return path;
            }
            return null;
        }
    }
}
