using System;
using Microsoft.Xna.Framework;
using SadConsole;
using SadConsole.Renderers;
using SadConsole.Surfaces;

namespace GeoStar.Screens
{
    class ScrollingConsole : SadConsole.ConsoleContainer
    {
        public SadConsole.Console mainConsole { get; private set; }
        SadConsole.ControlsConsole controlsContainer;
        SadConsole.Controls.ScrollBar scrollBar;

        int scrollingCounter;
        private BasicSurface borderSurface;
        private SurfaceRenderer renderer;

        public ScrollingConsole(int width, int height, int bufferHeight)
        {
            controlsContainer = new SadConsole.ControlsConsole(1, height);

            mainConsole = new SadConsole.Console(width - 1, bufferHeight);
            mainConsole.TextSurface.RenderArea = new Microsoft.Xna.Framework.Rectangle(0, 0, width - 1, height);
            mainConsole.VirtualCursor.IsVisible = false;

            scrollBar = SadConsole.Controls.ScrollBar.Create(System.Windows.Controls.Orientation.Vertical, height);
            scrollBar.IsEnabled = false;
            scrollBar.ValueChanged += ScrollBar_ValueChanged;

            controlsContainer.Add(scrollBar);
            controlsContainer.Position = new Microsoft.Xna.Framework.Point(1 + mainConsole.TextSurface.Width, Position.Y);

            Children.Add(mainConsole);
            Children.Add(controlsContainer);

            scrollingCounter = 0;

            borderSurface = new SadConsole.Surfaces.BasicSurface(width + 2, height + 2, mainConsole.TextSurface.Font);
            var editor = new SadConsole.Surfaces.SurfaceEditor(borderSurface);

            SadConsole.Shapes.Box box = SadConsole.Shapes.Box.Thick();
            box.Width = borderSurface.Width;
            box.Height = borderSurface.Height;
            box.Draw(editor);
            renderer = new SurfaceRenderer();
            renderer.Render(borderSurface);
        }

        private void ScrollBar_ValueChanged(object sender, System.EventArgs e)
        {
            // Do our scroll according to where the scroll bar value is
            mainConsole.TextSurface.RenderArea = new Microsoft.Xna.Framework.Rectangle(0, scrollBar.Value, mainConsole.TextSurface.Width, mainConsole.TextSurface.RenderArea.Height);
        }

        public override void Update(System.TimeSpan delta)
        {
            base.Update(delta);

            // If we detect that this console has shifted the data up for any reason (like the virtual cursor reached the
            // bottom of the entire text surface, OR we reached the bottom of the render area, we need to adjust the 
            // scroll bar and follow the cursor
            if (mainConsole.TimesShiftedUp != 0 | mainConsole.VirtualCursor.Position.Y >= mainConsole.TextSurface.RenderArea.Height + scrollingCounter)
            {
                // Once the buffer has finally been filled enough to need scrolling, turn on the scroll bar
                scrollBar.IsEnabled = true;

                // Make sure we've never scrolled the entire size of the buffer
                if (scrollingCounter < mainConsole.TextSurface.Height - mainConsole.TextSurface.RenderArea.Height)
                    // Record how much we've scrolled to enable how far back the bar can see
                    scrollingCounter += mainConsole.TimesShiftedUp != 0 ? mainConsole.TimesShiftedUp : 1;

                scrollBar.Maximum = (mainConsole.TextSurface.Height + scrollingCounter) - mainConsole.TextSurface.Height;

                // This will follow the cursor since we move the render area in the event.
                scrollBar.Value = scrollingCounter;

                // Reset the shift amount.
                mainConsole.TimesShiftedUp = 0;
            }
        }

        public override bool ProcessKeyboard(SadConsole.Input.Keyboard state)
        {
            // Send keyboard input to the main console
            return mainConsole.ProcessKeyboard(state);
        }

        public override bool ProcessMouse(SadConsole.Input.MouseConsoleState state)
        {
            // Check the scroll bar for mouse info first. If mouse not handled by scroll bar, then..

            // Create a mouse state based on the controlsContainer
            if (!controlsContainer.ProcessMouse(new SadConsole.Input.MouseConsoleState(controlsContainer, state.Mouse)))
            {
                // Process this console normally.
                return mainConsole.ProcessMouse(state);
            }

            // If we get here, then the mouse was over the scroll bar.
            return true;
        }

        public override void Draw(TimeSpan delta)
        {
            Global.DrawCalls.Add(new DrawCallSurface(borderSurface, position - new Point(1), UsePixelPositioning));

            base.Draw(delta);
        }
    }
}
