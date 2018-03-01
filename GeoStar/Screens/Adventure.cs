using GeoStar.Entities;
using Microsoft.Xna.Framework;
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
    class Adventure : SadConsole.Screen
    {
        private Map map;

        public Player Player;
        public Dungeon DungeonScreen;

        public Map Map { get { return map; } }

        public Point MapViewPoint
        {
            get { return DungeonScreen.MapViewPoint; }
            set
            {
                DungeonScreen.MapViewPoint = value;
                SyncEntityOffset();
            }
        }
        public Adventure()
        {
            DungeonScreen = new Dungeon(0, 0, GameManager.ScreenWidth, GameManager.ScreenHeight);

            Children.Add(DungeonScreen);
        }
        public void LoadMap(Map map)
        {
            // If we had an old map, unhook this event
            if (this.map != null)
                map.Entities.CollectionChanged -= Entities_CollectionChanged;

            // Tell the dungeon screen to load this new map
            DungeonScreen.LoadMap(map);

            // Anytime an entity is removed or added, we need to know about it
            map.Entities.CollectionChanged += Entities_CollectionChanged;

            this.map = map;
        }
        private void Entities_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    foreach (var item in e.NewItems)
                        DungeonScreen.Children.Add((EntityBase)item);
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    foreach (var item in e.OldItems)
                        DungeonScreen.Children.Remove((EntityBase)item);
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Replace:
                    foreach (var item in e.NewItems)
                        DungeonScreen.Children.Add((EntityBase)item);
                    foreach (var item in e.OldItems)
                        DungeonScreen.Children.Remove((EntityBase)item);
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
                item.PositionOffset = new Point(-DungeonScreen.MapViewPoint.X, -DungeonScreen.MapViewPoint.Y);
            }
        }
        public override void Update(TimeSpan timeElapsed)
        {
            base.Update(timeElapsed);

            // Handle keyboard when this screen is being run
            if (SadConsole.Global.KeyboardState.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.Left))
                Player.MoveBy(new Point(-1, 0), map);

            else if (SadConsole.Global.KeyboardState.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.Right))
                Player.MoveBy(new Point(1, 0), map);

            if (SadConsole.Global.KeyboardState.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.Up))
                Player.MoveBy(new Point(0, -1), map);

            else if (SadConsole.Global.KeyboardState.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.Down))
                Player.MoveBy(new Point(0, 1), map);
        }
    }
}
