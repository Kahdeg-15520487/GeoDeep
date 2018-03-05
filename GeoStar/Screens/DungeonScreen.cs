using Microsoft.Xna.Framework;
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
    class DungeonScreen : SadConsole.Screen
    {
        private SurfaceRenderer renderer = new SurfaceRenderer();
        private BasicSurface surface;
        private SadConsole.DrawCallSurface drawCall;

        public Point MapViewPoint { get { return surface.RenderArea.Location; } set { surface.RenderArea = new Rectangle(value, new Point(Width, Height)); } }

        public int Width { get; private set; }
        public int Height { get; private set; }

        private BasicSurface borderSurface;

        public DungeonScreen(int screenX, int screenY, int screenWidth, int screenHeight, Font font)
        {
            Position = new Point(screenX, screenY);
            Width = screenWidth;
            Height = screenHeight;

            borderSurface = new SadConsole.Surfaces.BasicSurface(Width + 1, Height + 1, font);
            var editor = new SadConsole.Surfaces.SurfaceEditor(borderSurface);

            SadConsole.Shapes.Box box = SadConsole.Shapes.Box.Thick();
            box.Width = borderSurface.Width;
            box.Height = borderSurface.Height;
            box.Draw(editor);
            renderer = new SurfaceRenderer();
            renderer.Render(borderSurface);
        }

        public void LoadMap(Map map)
        {
            // Create a surface for drawing. It uses the tiles from a map object.
            surface = new BasicSurface(map.Width, map.Height, map.Tiles, SadConsole.Global.FontDefault, new Rectangle(0, 0, Width, Height));
            drawCall = new SadConsole.DrawCallSurface(surface, position, false);
        }

        public bool ContainViewPoint(Point viewPoint)
        {
            return surface.RenderArea.Contains(viewPoint);
        }

        public override void Draw(TimeSpan timeElapsed)
        {
            renderer.Render(surface);
            Global.DrawCalls.Add(drawCall);
            Global.DrawCalls.Add(new DrawCallSurface(borderSurface, position - new Point(1), true));

            base.Draw(timeElapsed);
        }
    }
}
