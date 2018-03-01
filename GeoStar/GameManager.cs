using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GeoStar
{
    public class GameManager : SadConsole.Game
    {

        public static int ScreenWidth { get; internal set; } = 80;
        public static int ScreenHeight { get; internal set; } = 25;

        internal static Screens.Adventure AdventureScreen;

        public GameManager() : base("IBM.font", 80, 25, null)
        {

        }

        protected override void Initialize()
        {
            // Generally you don't want to hide the mouse from the user
            IsMouseVisible = true;

            // Finish the initialization of SadConsole
            base.Initialize();

            // Create the map
            AdventureScreen = new Screens.Adventure();
            AdventureScreen.LoadMap(new Map(100, 100));
            AdventureScreen.Player = new Entities.Player();
            AdventureScreen.Player.Position = new Point(13, 7);
            AdventureScreen.Map.Entities.Add(AdventureScreen.Player);

            SadConsole.Global.CurrentScreen.Children.Add(AdventureScreen);
        }
    }
}
