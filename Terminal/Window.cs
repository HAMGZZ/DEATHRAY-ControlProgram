using System.Threading;

namespace ControlProgram.Terminal
{
    class Window
    {
        private byte x;
        private byte y;
        private int height;
        private int width;
        private string windowName;
        private Logger logger;

        //private port serialport;

        public byte X { get => x; set => x = value; }
        public byte Y { get => y; set => y = value; }
        public int Height { get => height; set => height = value; }
        public int Width { get => width; set => width = value; }
        public string WindowName { get => windowName; set => windowName = value; }
        public byte Bottom 
        {
            get
            {
                return (byte)(x + height);
            }
        }
        public byte Left
        {
            get
            {
                return (byte)(y + width - 2);
            }
        }


        public Window(byte startX, byte startY, int height, int width, string windowName)
        {
            var logger = new Logger(windowName + " WINDOW", Logger.Level.INFO);
            this.logger = logger;
            x = startX;
            y = startY;
            this.height = height;
            this.width = width;
            this.windowName = windowName;
            logger.log(Logger.Level.INFO, "New window created \"" + windowName + "\"");
        }

        public string Draw()
        {
            logger.log(Logger.Level.INFO, string.Format("New window drawn \"{0}\" at {1} {2}", windowName, x, y));
            return DrawRectangle(x, y, height, width) + TerminalCommands.SetCursor(new TerminalCommands.CursorAddress { x = (byte)(this.x + 2), y = this.y }) + windowName;

        }

        public string text(string data, byte localx, byte localy, bool clear = false)
        {
            logger.log(Logger.Level.DEBUG, "Adding text to \"" + windowName + "\" at : " + localx + " " + localy);
            return TerminalCommands.SetCursor( new TerminalCommands.CursorAddress { x = (byte)(localx + this.x + 1), y = (byte)(localy + this.y + 1) }) + data;

        }

        

        private string DrawRectangle(byte startX, byte startY, int height, int width)
        {
            logger.log(Logger.Level.DEBUG, "Drawing rectangle at : " + startX + " " + startY);
            string buffer = "";
            buffer += TerminalCommands.SetCursor(new TerminalCommands.CursorAddress { x = startX, y = startY });
            for (int i = 0; i < width; i++)
            {
                buffer += '-';
            }
            buffer += TerminalCommands.SetCursor(new TerminalCommands.CursorAddress { x = (byte)(startX), y = (byte)(startY + height - 1) });
            for (int i = 0; i < width; i++)
            {
                buffer += '-';
            }
            for (int i = 0; i < height; i++)
            {
                buffer += TerminalCommands.SetCursor(new TerminalCommands.CursorAddress { x = startX, y = (byte)(startY + i) });
                buffer += '|';
                buffer += TerminalCommands.SetCursor(new TerminalCommands.CursorAddress { x = (byte)(startX + width - 1), y = (byte)(startY + i) });
                buffer += '|';
            }
            return buffer;
        }







    }
}
