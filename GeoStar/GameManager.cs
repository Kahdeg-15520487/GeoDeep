using GeoStar.Screens;
using GeoStar.Services;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GeoStar
{
    public class GameManager : SadConsole.Game
    {

        public const int SCREEN_WIDTH = 160;
        public const int SCREEN_HEIGHT = 40;

        internal static Screens.AdventureScreen AdventureScreen;

        public GameManager() : base("IBM.font", SCREEN_WIDTH, SCREEN_HEIGHT, null)
        {
            //foreach (var p in Point.Zero.GenerateOutTo(0, includeCenter: false))
            //{
            //    System.Console.WriteLine(p);
            //}
        }

        protected override void Initialize()
        {
            // Generally you don't want to hide the mouse from the user
            IsMouseVisible = true;

            // Finish the initialization of SadConsole
            base.Initialize();

            RandomWrapper randomWrapper = new RandomWrapper(2);

            RandomNumberServiceLocator.Provide(randomWrapper);

            // Create the map
            AdventureScreen = new AdventureScreen();
            AdventureScreen.LoadMap(MapGenerator.Generate(200, 200));
            AdventureScreen.SpawnPlayer();

            SadConsole.ControlsConsole startingConsole = new SadConsole.ControlsConsole(SCREEN_WIDTH, SCREEN_HEIGHT);

            var bt1 = new SadConsole.Controls.Button(5);
            startingConsole.Add(bt1);

            SadConsole.Global.CurrentScreen = startingConsole;

            SadConsole.Global.CurrentScreen.Children.Add(AdventureScreen);

            //SimplexNoiseViewer simplexNoiseViewer = new SimplexNoiseViewer(0, 0, SCREEN_WIDTH, SCREEN_HEIGHT);
            //simplexNoiseViewer.LoadMap(200, 200, 0.03f);
            //SadConsole.Global.CurrentScreen = simplexNoiseViewer;

            //SadConsole.Window window = new SadConsole.Window(10, 10);
            //window.Title = "status";
            //window.Dragable = true;
            //window.Show();
            //SadConsole.Global.CurrentScreen = startingConsole;
            //SadConsole.Global.CurrentScreen.Children.Add(window);
        }

        protected override void Update(GameTime gameTime)
        {
            if (!SadConsole.Global.GraphicsDeviceManager.IsFullScreen && SadConsole.Global.KeyboardState.IsKeyReleased(Keys.F5))
            {
                SadConsole.Settings.ToggleFullScreen();
            }

            //if (SadConsole.Global.KeyboardState.IsKeyReleased(Keys.Escape))
            //{
            //    Instance.Exit();
            //}

            base.Update(gameTime);
        }
    }
}
