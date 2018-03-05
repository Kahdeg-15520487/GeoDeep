using GeoStar.Entities;
using Microsoft.Xna.Framework;
using SadConsole;
using SadConsole.Renderers;
using SadConsole.Surfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoStar.Screens
{
    class PlayerStatus
    {
    //    public int Width { get; private set; }
    //    public int Height { get; private set; }

    //    public ObservableCollection<EntityBase> Entities;
    //    public Cell[] Cells { get; private set; }

    //    private Player Player;

    //    public PlayerStatus(Player player)
    //    {
    //        Player = player;
    //        Width = 27;
    //        Height = 19;

    //        // Create our tiles for the map
    //        Cells = new Cell[Width * Height];

    //        // Fill the map with floors.
    //        for (int i = 0; i < Cells.Length; i++)
    //            Cells[i] = new Cell(Color.Green, Color.Black);

    //        var ct = new[]
    //        {
    //            @"_______",
    //            @"|     |",
    //            @"| [H] |",
    //            @"|_____|",
    //            @"  | |",
    //            @"=============",
    //            @"//  |     |  \\",
    //            @"   [LAL]  |     |  [RAL]",
    //            @"    //    | [D] |    \\",
    //            @"  [LH]    |     |    [RH]",
    //            @"|     |",
    //            @"=======",
    //            @"  //    \\",
    //            @" //      \\",
    //            @"     [LLL]      [RLL]",
    //            @"//          \\",
    //            @"     //            \\",
    //            @"   [LF]            [RF]"
    //        };

    //        for (int i = 0; i < ct.Length; i++)
    //        {
    //            SetCells(ct[i], i);
    //        }

    //        // Holds all entities on the map
    //        Entities = new ObservableCollection<EntityBase>();


    //    }

    //    private void SetCells(string cells, int line, int startIndex = 0, int endIndex = -1)
    //    {

    //        if (endIndex == -1)
    //        {
    //            endIndex = cells.Length;
    //        }

    //        for (int i = startIndex; i < endIndex; i++)
    //        {
    //            Cells[line * Width + i].Glyph = cells[i];
    //        }
    //    }
    }

    class StatusConsole : SadConsole.ControlsConsole
    {
        private ISurface borderSurface;

        public int Width { get; private set; }
        public int Height { get; private set; }

        public StatusConsole(int screenX, int screenY, int screenWidth, int screenHeight) : base(screenWidth, screenHeight)
        {
            Position = new Point(screenX, screenY);
            Width = screenWidth;
            Height = screenHeight;

            var lines = new[]
            {
                          @"_______",
                          @"|     |",
                          @"| [H] |",
                          @"|_____|",
                            @"| |",
                       @"=============",
                      @"//  |     |  \\",
                   @"[LAL]  |     |  [RAL]",
                    @"//    | [D] |    \\",
                  @"[LH]    |     |    [RH]",
                          @"|     |",
                          @"=======",
                         @"//    \\",
                        @"//      \\",
                     @"[LLL]      [RLL]",
                      @"//          \\",
                     @"//            \\",
                   @"[LF]            [RF]"
            };

            var offsets = new[]
            {
                10,
                10,
                10,
                10,
                12,
                7,
                6,
                3,
                4,
                2,
                10,
                10,
                9,
                8,
                5,
                6,
                5,
                3
            };

            for (int i = 0; i < lines.Length; i++)
            {
                Print(offsets[i], i, lines[i]);
            }

            var btHead = new SadConsole.Controls.Button(5, 1)
            {
                Text = " H ",
                Position = new Point(11, 2)
            };
            Add(btHead);

            var btLAL = new SadConsole.Controls.Button(5, 1)
            {
                Text = "LAL",
                Position = new Point(3, 7)
            };
            Add(btLAL);

            var btRAL = new SadConsole.Controls.Button(5, 1)
            {
                Text = "RAL",
                Position = new Point(19, 7)
            };
            Add(btRAL);

            var btDD = new SadConsole.Controls.Button(5, 1)
            {
                Text = " D ",
                Position = new Point(11, 8)
            };
            Add(btDD);

            var btLH = new SadConsole.Controls.Button(4, 1)
            {
                Text = "LH",
                Position = new Point(2, 9)
            };
            Add(btLH);

            var btRH = new SadConsole.Controls.Button(4, 1)
            {
                Text = "RH",
                Position = new Point(21, 9)
            };
            Add(btRH);

            var btLLL = new SadConsole.Controls.Button(5, 1)
            {
                Text = "LLL",
                Position = new Point(5, 14)
            };
            Add(btLLL);

            var btRLL = new SadConsole.Controls.Button(5, 1)
            {
                Text = "RLL",
                Position = new Point(16, 14)
            };
            Add(btRLL);

            var btLF = new SadConsole.Controls.Button(4, 1)
            {
                Text = "LF",
                Position = new Point(3, 17)
            };
            Add(btLF);

            var btRF = new SadConsole.Controls.Button(4, 1)
            {
                Text = "RF",
                Position = new Point(19, 17)
            };
            Add(btRF);

            SadConsole.Controls.ProgressBar health = new SadConsole.Controls.ProgressBar(2, 10, System.Windows.VerticalAlignment.Top);
            health.Position = new Point(27, 0);
            health.SetForeground(0, 9, Color.Green);
            Add(health);

            borderSurface = new SadConsole.Surfaces.BasicSurface(Width + 2, Height + 2, base.textSurface.Font);
            var editor = new SadConsole.Surfaces.SurfaceEditor(borderSurface);

            SadConsole.Shapes.Box box = SadConsole.Shapes.Box.Thick();
            box.Width = borderSurface.Width;
            box.Height = borderSurface.Height;
            box.Draw(editor);
            base.Renderer.Render(borderSurface);
        }

        public void LoadPlayerStatus(PlayerStatus playerStatus)
        {


            // Create a surface for drawing. It uses the tiles from a map object.
            //surface = new BasicSurface(playerStatus.Width, playerStatus.Height, playerStatus.Cells, Global.FontDefault, new Rectangle(0, 0, Width, Height));

            //drawCall = new DrawCallSurface(surface, position, false);
        }

        public override void Update(TimeSpan time)
        {
            

            base.Update(time);
        }

        public override void Draw(TimeSpan timeElapsed)
        {
            Global.DrawCalls.Add(new DrawCallSurface(borderSurface, position - new Point(1), UsePixelPositioning));

            base.Draw(timeElapsed);
        }
    }
}
