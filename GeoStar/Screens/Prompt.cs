using SadConsole;
using SadConsole.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoStar.Screens
{
    class Prompt : Window
    {
        public string P { get; set; }

        public Prompt(int width, int height, string prompt = "", string title = "") : base(width, height)
        {
            if (!string.IsNullOrEmpty(title))
            {
                Title = title;
            }
            P = prompt;

            Button OK = new Button(4)
            {
                Position = new Microsoft.Xna.Framework.Point(1, 3),
                Text = "OK"
            };
            OK.MouseButtonClicked += (o, e) =>
            {
                DialogResult = true;
                Hide();
            };
            Button Cancel = new Button(4)
            {
                Position = new Microsoft.Xna.Framework.Point(width - 5, 3),
                Text = "X"
            };
            Cancel.MouseButtonClicked += (o, e) =>
            {
                DialogResult = false;
                Hide();
            };

            Print(1, 1, P);

            Add(OK);
            Add(Cancel);
        }

        public void Show(string p)
        {
            base.Show(true);
        }

        public override void Update(TimeSpan time)
        {
            if (IsVisible)
            {
                if (SadConsole.Global.KeyboardState.IsKeyReleased(Microsoft.Xna.Framework.Input.Keys.Enter))
                {
                    DialogResult = true;
                    Hide();
                }

                if (SadConsole.Global.KeyboardState.IsKeyReleased(Microsoft.Xna.Framework.Input.Keys.Back))
                {
                    DialogResult = false;
                    Hide();
                }
            }

            base.Update(time);
        }
    }
}
