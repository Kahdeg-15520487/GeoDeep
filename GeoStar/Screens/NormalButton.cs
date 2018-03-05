using SadConsole.Controls;
using SadConsole.Themes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoStar.Screens
{
    class NormalButton : ButtonBase<ButtonTheme>
    {
        public NormalButton(int width, int height = 1) : base(width, height, Library.Default.ButtonTheme)
        {
            DetermineAppearance();
        }

        public override void Compose()
        {
            if (this.IsDirty)
            {
                // Redraw the control
                this.Fill(currentAppearance.Foreground, currentAppearance.Background, currentAppearance.Glyph, null);

                this.Print(0, 0, (Text).Align(TextAlignment, this.TextSurface.Width));

                OnComposed?.Invoke(this);
                this.IsDirty = false;
            }
        }
    }
}
