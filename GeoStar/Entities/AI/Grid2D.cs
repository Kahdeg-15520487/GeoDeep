/* 
astar-1.0.cs may be freely distributed under the MIT license.

Copyright (c) 2013 Josh Baldwin https://github.com/jbaldwin/astar.cs

Permission is hereby granted, free of charge, to any person
obtaining a copy of this software and associated documentation 
files (the "Software"), to deal in the Software without 
restriction, including without limitation the rights to use, 
copy, modify, merge, publish, distribute, sublicense, and/or sell 
copies of the Software, and to permit persons to whom the 
Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be 
included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, 
EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES 
OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND 
NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT 
HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, 
WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING 
FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR 
OTHER DEALINGS IN THE SOFTWARE.
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using AStar;

namespace GeoStar.Entities.AI
{
    public class Grid2D : IEnumerable<GridNode>
    {
        public GridNode[,] Grid;

        public int Width { get => Grid.GetLength(0); }
        public int Height { get => Grid.GetLength(1); }

        public GridNode Start;
        public GridNode Goal;

        public GridNode this[int x, int y]
        {
            get { return Grid[x, y]; }
            set { Grid[x, y] = value; }
        }

        public Grid2D(int width, int height)
        {
            Grid = new GridNode[width, height];
        }

        public Grid2D(GridNode[,] grid, GridNode start, GridNode goal)
        {
            Grid = grid;
            Start = start;
            Goal = goal;
        }

        public Grid2D(int width, int height, int wallPercentage, int swampPercentage, int startX, int startY, int goalX, int goalY)
        {
            var rand = new Random();
            Start = new GridNode(this, startX, startY, 0);
            Goal = new GridNode(this, goalX, goalY, 0);

            Grid = new GridNode[width, height];

            this[Start.X, Start.Y] = Start;
            this[Goal.X, Goal.Y] = Goal;

            for (var i = 0; i < width; i++)
            {
                for (var j = 0; j < height; j++)
                {
                    // don't overwrite start/goal nodes
                    if (this[i, j] != null)
                        continue;
                    var terrain = rand.Next(100);
                    this[i, j] = new GridNode(this, i, j, terrain > 100 - wallPercentage ? -1 : terrain > 100 - swampPercentage ? 1 : 0);
                }
            }
        }

        public int GetCellIndex(int x, int y)
        {
            return y * Width + x;
        }

        public string Print(IEnumerable<INode> path)
        {
            StringBuilder output = new StringBuilder();
            for (var j = 0; j < Height; j++)
            {
                for (var i = 0; i < Width; i++)
                {
                    output.Append(this[i, j].Print(Start, Goal, path));
                }
                output.AppendLine();
            }
            return output.ToString();
        }

        public IEnumerator<GridNode> GetEnumerator()
        {
            foreach (GridNode gn in Grid)
            {
                yield return gn;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
