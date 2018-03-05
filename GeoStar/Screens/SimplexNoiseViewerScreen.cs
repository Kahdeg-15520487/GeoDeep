using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using SadConsole;
using SadConsole.Renderers;
using SadConsole.Surfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoStar.Screens
{
    class SimplexNoiseViewerScreen : SadConsole.Screen
    {
        private SurfaceRenderer renderer = new SurfaceRenderer();
        private BasicSurface surface;
        private SadConsole.DrawCallSurface drawCall;

        public Point MapViewPoint { get { return surface.RenderArea.Location; } set { surface.RenderArea = new Rectangle(value, new Point(Width, Height)); } }

        public int Width { get; private set; }
        public int Height { get; private set; }

        private int w, h;
        private float scale;
        private int layerRange = 4;
        private int layerRangeMin = 0;
        private int min = 0;
        private int max = 70;
        private bool isShowAll = true;
        private Dictionary<int, Color> colorRamp;

        Cell[] Tiles;

        public SimplexNoiseViewerScreen(int screenX, int screenY, int screenWidth, int screenHeight)
        {
            Position = new Point(screenX, screenY);
            Width = screenWidth;
            Height = screenHeight;
            List<Color> cr = new List<Color>()
            {
                new Color(0,255,255),
                new Color(0,252,255),
                new Color(0,249,255),
                new Color(0,247,255),
                new Color(0,244,255),
                new Color(0,242,255),
                new Color(0,239,255),
                new Color(0,236,255),
                new Color(0,234,255),
                new Color(0,231,255),
                new Color(0,229,255),
                new Color(0,226,255),
                new Color(0,224,255),
                new Color(0,221,255),
                new Color(0,218,255),
                new Color(0,216,255),
                new Color(0,213,255),
                new Color(0,211,255),
                new Color(0,208,255),
                new Color(0,206,255),
                new Color(0,203,255),
                new Color(0,200,255),
                new Color(0,198,255),
                new Color(0,195,255),
                new Color(0,193,255),
                new Color(0,190,255),
                new Color(0,188,255),
                new Color(0,185,255),
                new Color(0,182,255),
                new Color(0,180,255),
                new Color(0,177,255),
                new Color(0,175,255),
                new Color(0,172,255),
                new Color(0,170,255),
                new Color(0,167,255),
                new Color(0,164,255),
                new Color(0,162,255),
                new Color(0,159,255),
                new Color(0,157,255),
                new Color(0,154,255),
                new Color(0,151,255),
                new Color(0,149,255),
                new Color(0,146,255),
                new Color(0,144,255),
                new Color(0,141,255),
                new Color(0,139,255),
                new Color(0,136,255),
                new Color(0,133,255),
                new Color(0,131,255),
                new Color(0,128,255),
                new Color(0,126,255),
                new Color(0,123,255),
                new Color(0,121,255),
                new Color(0,118,255),
                new Color(0,115,255),
                new Color(0,113,255),
                new Color(0,110,255),
                new Color(0,108,255),
                new Color(0,105,255),
                new Color(0,103,255),
                new Color(0,100,255),
                new Color(0,97,255),
                new Color(0,95,255),
                new Color(0,92,255),
                new Color(0,90,255),
                new Color(0,87,255),
                new Color(0,85,255),
                new Color(0,82,255),
                new Color(0,79,255),
                new Color(0,77,255),
                new Color(0,74,255),
                new Color(0,72,255),
                new Color(0,69,255),
                new Color(0,66,255),
                new Color(0,64,255),
                new Color(0,61,255),
                new Color(0,59,255),
                new Color(0,56,255),
                new Color(0,54,255),
                new Color(0,51,255),
                new Color(0,48,255),
                new Color(0,46,255),
                new Color(0,43,255),
                new Color(0,41,255),
                new Color(0,38,255),
                new Color(0,36,255),
                new Color(0,33,255),
                new Color(0,30,255),
                new Color(0,28,255),
                new Color(0,25,255),
                new Color(0,23,255),
                new Color(0,20,255),
                new Color(0,18,255),
                new Color(0,15,255),
                new Color(0,12,255),
                new Color(0,10,255),
                new Color(0,7,255),
                new Color(0,5,255),
                new Color(0,2,255),
                new Color(0,0,255)
            };

            colorRamp = new Dictionary<int, Color>();

            int count = 0;
            foreach (var c in cr)
            {
                colorRamp.Add(count++, c);
            }
        }

        public void LoadMap(int width, int height, float scale = 0.1f)
        {
            this.scale = scale;
            w = width;
            h = height;

            Tiles = new Cell[width * height];
            Simplex.Noise.Seed = 1;
            var noise = Simplex.Noise.Calc2D(width, height, scale);
            var minNoise = noise.Min();
            var maxNoise = noise.Max();
            noise.Map(n => n.Map(minNoise, maxNoise, min, max));
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {

                    int cellIndex = y * width + x;

                    Tiles[cellIndex] = new Cell(colorRamp[(int)noise[x, y]], Color.Black);

                    if (isShowAll || ((int)noise[x, y] >= layerRangeMin && (int)noise[x, y] <= (layerRangeMin + layerRange)))
                    {
                        Tiles[cellIndex].Glyph = 178;
                    }
                }
            }

            // Create a surface for drawing. It uses the tiles from a map object.
            surface = new BasicSurface(width, height, Tiles, SadConsole.Global.FontDefault, new Rectangle(0, 0, Width, Height));
            drawCall = new SadConsole.DrawCallSurface(surface, position, false);
        }

        public bool ContainViewPoint(Point viewPoint)
        {
            return surface.RenderArea.Contains(viewPoint);
        }

        public override void Update(TimeSpan timeElapsed)
        {
            if (SadConsole.Global.KeyboardState.IsKeyReleased(Keys.Up))
            {
                scale += 0.01f;
                LoadMap(w, h, scale);
                System.Console.WriteLine(scale);
            }

            if (SadConsole.Global.KeyboardState.IsKeyReleased(Keys.Down))
            {
                if (scale > 0.01f)
                {
                    scale -= 0.01f;
                    LoadMap(w, h, scale);
                    System.Console.WriteLine(scale);
                }
            }

            if (!isShowAll)
            {
                if (SadConsole.Global.KeyboardState.IsKeyReleased(Keys.Left))
                {
                    if (layerRangeMin >= min)
                    {
                        layerRangeMin--;
                        LoadMap(w, h, scale);
                        System.Console.WriteLine(layerRangeMin + " -> " + (layerRangeMin + layerRange));
                    }
                }

                if (SadConsole.Global.KeyboardState.IsKeyReleased(Keys.Right))
                {
                    if (layerRangeMin + layerRange <= max)
                    {
                        layerRangeMin++;
                        LoadMap(w, h, scale);
                        System.Console.WriteLine(layerRangeMin + " -> " + (layerRangeMin + layerRange));
                    }
                }

                if (SadConsole.Global.KeyboardState.IsKeyReleased(Keys.Z))
                {
                    if (layerRange > 0)
                    {
                        layerRange--;
                        LoadMap(w, h, scale);
                        System.Console.WriteLine(layerRangeMin + " -> " + (layerRangeMin + layerRange));
                    }
                }

                if (SadConsole.Global.KeyboardState.IsKeyReleased(Keys.X))
                {
                    if (layerRangeMin + layerRange <= max)
                    {
                        layerRange++;
                        LoadMap(w, h, scale);
                        System.Console.WriteLine(layerRangeMin + " -> " + (layerRangeMin + layerRange));
                    }
                }
            }

            if (SadConsole.Global.KeyboardState.IsKeyReleased(Keys.R))
            {
                isShowAll = !isShowAll;
                LoadMap(w, h, scale);
                System.Console.WriteLine("show all");
            }

            base.Update(timeElapsed);
        }

        public override void Draw(TimeSpan timeElapsed)
        {
            renderer.Render(surface);
            SadConsole.Global.DrawCalls.Add(drawCall);

            base.Draw(timeElapsed);
        }
    }
}
