using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;

namespace GeoStar.Services
{
    class Logger : TextWriter
    {
        Screens.ScrollingConsole scrollingConsole;

        public Logger(Screens.ScrollingConsole scrollingConsole)
        {
            this.scrollingConsole = scrollingConsole;
        }

        public override void Write(char value)
        {
            int x = scrollingConsole.mainConsole.VirtualCursor.Position.X;
            int y = scrollingConsole.mainConsole.VirtualCursor.Position.Y;

            if (value == '\r')
            {
                x = 0;
                y++;
            }
            else if (value == '\n')
            {
            }
            else
            {
                scrollingConsole.mainConsole.Print(x++, y, value.ToString());
            }
            scrollingConsole.mainConsole.VirtualCursor.Position = new Point(x, y);
        }

        public override void WriteLine(string value)
        {
            int x = scrollingConsole.mainConsole.VirtualCursor.Position.X;
            int y = scrollingConsole.mainConsole.VirtualCursor.Position.Y;

            scrollingConsole.mainConsole.Print(0, y++, value.ToString());
            scrollingConsole.mainConsole.VirtualCursor.Position = new Point(0, y);
        }

        public override Encoding Encoding
        {
            get { return Encoding.Default; }
        }
    }

    class LoggingServiceLocator
    {
        public static TextWriter GetLoggingService() => loggingService;
        private static TextWriter loggingService;

        public static void Provide(TextWriter logger)
        {
            loggingService = logger;
        }
    }
}
