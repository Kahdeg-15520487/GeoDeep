using GeoStar.Entities;
using GeoStar.Items;
using GeoStar.MapObjects;
using GeoStar.Services;
using GoRogue;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoStar.Screens
{
    /// <summary>
    /// Represents the whole screen when you are adventuring in a town or dungeon.
    /// Coordinates betwene child screens.
    /// </summary>
    class AdventureScreen : SadConsole.Screen
    {
        private Map map;

        public Player Player;
        public DungeonScreen DungeonScreen;
        public StatusConsole StatusScreen;
        public ScrollingConsole ScrollingConsole;

        public InventoryViewerWindow InventoryViewerWindow;
        public Prompt Prompt;

        public Map Map { get { return map; } }
        public PlayerStatus PlayerStatus { get; private set; }

        public Point MapViewPoint
        {
            get { return DungeonScreen.MapViewPoint; }
            set
            {
                DungeonScreen.MapViewPoint = value;
                SyncEntityOffset();
            }
        }

        public AdventureScreen()
        {
            DungeonScreen = new DungeonScreen(0, 0, 100, 39, SadConsole.Global.Fonts["IBM"].GetFont(SadConsole.Font.FontSizes.One));
            StatusScreen = new StatusConsole(110, 2, 40, 19);
            ScrollingConsole = new ScrollingConsole(40, 15, 100);
            ScrollingConsole.Position = new Point(110, 22);
            InventoryViewerWindow = new InventoryViewerWindow()
            {
                Position = new Point(5, 5),
                Dragable = true
            };
            InventoryViewerWindow.Closed += InventoryViewerWindow_Closed;
            Prompt = new Prompt(12, 5, "Exit?", "Warning")
            {
                Position = new Point(70, 15)
            };
            Prompt.Closed += (o, e) =>
            {
                if (Prompt.DialogResult)
                {
                    Environment.Exit(0);
                }
            };
            Children.Add(DungeonScreen);
            Children.Add(StatusScreen);
            Children.Add(ScrollingConsole);
            Children.Add(InventoryViewerWindow);
            Children.Add(Prompt);

            Logger logger = new Logger(ScrollingConsole);
            LoggingServiceLocator.Provide(logger);
        }

        public void LoadMap(Map map)
        {
            // If we had an old map, unhook this event
            if (this.map != null)
            {
                map.Entities.CollectionChanged -= Entities_CollectionChanged;
            }

            // Tell the dungeon screen to load this new map
            DungeonScreen.LoadMap(map);

            // Anytime an entity is removed or added, we need to know about it
            map.Entities.CollectionChanged += Entities_CollectionChanged;

            this.map = map;
        }

        public void SpawnPlayer()
        {
            for (int x = 0; x < map.Width; x++)
            {
                for (int y = 0; y < map.Height; y++)
                {
                    if (map.IsTileWalkable(x, y))
                    {
                        Player = new Player()
                        {
                            Position = new Point(x, y)
                        };
                        break;
                    }
                }
            }
            map.UpdateFOV(Player.Position.X, Player.Position.Y);
            MapViewPoint = new Point(Player.Position.X - DungeonScreen.Width / 2, Player.Position.Y - DungeonScreen.Height / 2);

            PlayerStatus = new PlayerStatus();
            StatusScreen.LoadPlayerStatus(PlayerStatus);

            map.AddEntity(Player);
        }

        private void Entities_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    foreach (KeyValuePair<Point, EntityBase> item in e.NewItems)
                        DungeonScreen.Children.Add(item.Value);
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    foreach (KeyValuePair<Point, EntityBase> item in e.OldItems)
                        DungeonScreen.Children.Remove(item.Value);
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Replace:
                    foreach (KeyValuePair<Point, EntityBase> item in e.NewItems)
                        DungeonScreen.Children.Add(item.Value);
                    foreach (KeyValuePair<Point, EntityBase> item in e.OldItems)
                        DungeonScreen.Children.Remove(item.Value);
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Reset:
                    DungeonScreen.Children.Clear();
                    break;
                default:
                    break;
            }

            SyncEntityOffset();
        }

        private void SyncEntityOffset()
        {
            foreach (var item in map.Entities)
            {
                // Make sure that the entity draws based on the current map scrolling values
                item.Value.PositionOffset = new Point(-DungeonScreen.MapViewPoint.X, -DungeonScreen.MapViewPoint.Y);
            }
        }

        bool isPlayerMove = true;
        public override void Update(TimeSpan timeElapsed)
        {
            base.Update(timeElapsed);

            if (!InventoryViewerWindow.IsVisible)
            {
                HandlePlayerMovement();
            }

            MapViewPoint = new Point(Player.Position.X - DungeonScreen.Width / 2, Player.Position.Y - DungeonScreen.Height / 2);

            if (isPlayerMove)
            {
                map.UpdateFOV(Player.Position.X, Player.Position.Y);
                isPlayerMove = false;
            }

            if (SadConsole.Global.KeyboardState.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.I))
            {
                //show pickup and drop item menu
                InventoryViewerWindow.Show(Player, map, true);

                //lock for empty ground aroung player
                //if (map.FindEmptyPointAround(Player.Position, out Point emptyPoint))
                //{
                //    var pile = new ItemPile() { Position = emptyPoint };
                //    map.AddEntity(pile);
                //}
            }

            if (!InventoryViewerWindow.IsVisible && !Prompt.IsVisible && SadConsole.Global.KeyboardState.IsKeyReleased(Keys.Escape))
            {
                Prompt.Show(true);
            }
        }

        private void InventoryViewerWindow_Closed(object sender, EventArgs e)
        {
            if (InventoryViewerWindow.DialogResult)
            {
                if (map.FindEmptyPointAround(Player.Position, out Point emptyPoint))
                {
                    
                }
            }
        }

        private void HandlePlayerMovement()
        {
            var kbs = SadConsole.Global.KeyboardState;
            Point direction = Point.Zero;

            // Handle keyboard when this screen is being run
            if (kbs.IsKeyPressed(Keys.Left) || kbs.IsKeyPressed(Keys.NumPad4))
            {
                direction.X = -1;
                isPlayerMove = true;
            }
            else if (kbs.IsKeyPressed(Keys.Right) || kbs.IsKeyPressed(Keys.NumPad6))
            {
                direction.X = 1;
                isPlayerMove = true;
            }

            if (kbs.IsKeyPressed(Keys.Up) || kbs.IsKeyPressed(Keys.NumPad8))
            {
                direction.Y = -1;
                isPlayerMove = true;
            }
            else if (kbs.IsKeyPressed(Keys.Down) || kbs.IsKeyPressed(Keys.NumPad2))
            {
                direction.Y = 1;
                isPlayerMove = true;
            }

            if (kbs.IsKeyPressed(Keys.NumPad7))
            {
                direction.X = -1;
                direction.Y = -1;
                isPlayerMove = true;
            }
            else if (kbs.IsKeyPressed(Keys.NumPad9))
            {
                direction.X = 1;
                direction.Y = -1;
                isPlayerMove = true;
            }

            if (kbs.IsKeyPressed(Keys.NumPad1))
            {
                direction.X = -1;
                direction.Y = 1;
                isPlayerMove = true;
            }
            else if (kbs.IsKeyPressed(Keys.NumPad3))
            {
                direction.X = 1;
                direction.Y = 1;
                isPlayerMove = true;
            }

            if (isPlayerMove)
            {
                Player.MoveBy(direction, map);
            }
        }
    }
}
