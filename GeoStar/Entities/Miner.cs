using System;

using Microsoft.Xna.Framework;

using FSMSharp;

using GeoStar.Items;
using GeoStar.MapObjects;
using System.Collections;
using System.Collections.Generic;
using GeoStar.Entities.AI;

namespace GeoStar.Entities
{
    class Miner : NPC
    {
        class MinerEvaluator : IEvaluator
        {
            public int Evaluate(Inventory inventory)
            {
                int totalValue = 0;
                foreach (var item in inventory)
                {
                    totalValue += item.Amount;
                }
                return totalValue;
            }
        }

        internal enum MinerState
        {
            Idle,
            WanderFind,
            GoTo,
            Mine
        }

        MinerEvaluator evaluator;

        AStarDirection astar;
        Queue<(Direction, bool)> path;
        FSM<MinerState> fsm;
        public MinerState CurrentAIState { get => fsm.CurrentState; }
        bool isFoundOreVein;
        Point OreVein;
        int wanderDistance = 0;
        Direction wanderDirection = Direction.Center;

        Inventory beforeMine;

        public Miner(Map map) : base(Color.Orange, Color.Black, 'm', map, true, 200)
        {
            evaluator = new MinerEvaluator();
            fsm = new FSMSharp.FSM<MinerState>("MinerLogic");
            //fsm.DebugLogHandler = logger.WriteLine;

            SetupMinerLogic();
        }

        private void SetupMinerLogic()
        {
            fsm.Add(MinerState.Idle)
                .Expires(() => true)
                .GoesTo(MinerState.WanderFind);

            fsm.Add(MinerState.WanderFind)
                .Expires(() => isFoundOreVein == true)
                .GoesTo(MinerState.GoTo)
                .OnLeave(() => isFoundOreVein = false)
                .Calls((e) => { Wander(); FindOreVein(); });

            fsm.Add(MinerState.GoTo)
               .Expires(() => path.Count == 0)
               .GoesTo(MinerState.Mine)
               .OnEnter(PlotPathToOreVein)
               .Calls((e) => { Goto(); });

            fsm.Add(MinerState.Mine)
                .Expires(() => FindSurroundingOreVein() == false)
                .GoesTo(MinerState.Idle)
                .OnEnter(() => beforeMine = Inventory.Clone() as Inventory)
                .OnLeave(InformMinedMineral)
                .Calls((e) => { Mine(OreVein); });

            fsm.CurrentState = MinerState.Idle;
        }

        /// <summary>
        /// Check immediate surrounding for orevein
        /// </summary>
        private bool FindOreVein()
        {
            foreach (var coord in fovmap.CurrentFOV)
            {
                int cellIndex = map.GetCellIndex(coord.X, coord.Y);
                if (map.Tiles[cellIndex] is MineralVein)
                {
                    OreVein = new Point(coord.X, coord.Y);
                    isFoundOreVein = true;
                    logger.WriteLine("Hey I found {0} at {1}x{2}", (map.Tiles[cellIndex] as MineralVein).Type, OreVein.X, OreVein.Y);
                    return true;
                }
            }
            return false;
        }

        private void PlotPathToOreVein()
        {
            Grid2D grid = map.GetGrid(position.X, position.Y, OreVein.X, OreVein.Y);

            astar = new AStarDirection(grid.Start, grid.Goal);
            var result = astar.Run();
            var p = astar.GetPath(map);

            path = new Queue<(Direction, bool)>();
            foreach (var d in p)
            {
                path.Enqueue(d);
            }
        }

        private void Wander()
        {
            if (wanderDistance == 0)
            {
                wanderDistance = random.Next(10, 30);
                wanderDirection = (Direction)random.Next(0, 8);
                logger.WriteLine("Wandering {0}", wanderDirection);
            }
            if (CheckTile(wanderDirection) is Wall)
            {
                Mine(wanderDirection);
            }
            else
            {
                MoveBy(wanderDirection);
                wanderDistance--;
            }

            UpdateFov();
        }

        private void Goto()
        {
            var d = path.Dequeue();
            if (d.Item2)
            {
                Mine(d.Item1);
            }
            else
            {
                MoveBy(d.Item1);
                UpdateFov();
            }
        }

        private bool FindSurroundingOreVein()
        {
            var surrouding = new Point(position.X, position.Y).GetNearbyPoints();
            foreach (var p in surrouding)
            {
                var index = map.GetCellIndex(p.X, p.Y);
                if (map.Tiles[index] is MineralVein)
                {
                    OreVein.X = p.X;
                    OreVein.Y = p.Y;
                    logger.WriteLine("There is {0} nearby", (map.Tiles[index] as MineralVein).Type);
                    return true;
                }
            }
            return false;
        }

        private void Mine(Direction dir)
        {
            Equip.ItemBehaviour(this, dir, map);
        }

        private void Mine(Point target)
        {
            if (Equip != null && Equip.Name == "Pickaxe")
            {
                Equip.ItemBehaviour(this, position.GetDirectionFromPointAtoPointB(target), map);
            }
        }

        private void InformMinedMineral()
        {
            foreach (var different in Inventory.CompareTo(beforeMine))
            {
                logger.WriteLine("I mined {0} {1}", different.Amount, different.Item.Name);
            }
        }

        public override void Act()
        {
            fsm.Process();
        }
    }
}
