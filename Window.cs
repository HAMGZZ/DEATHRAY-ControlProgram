using System.Threading;

namespace ControllProgram
{
    class Window
    {
        private byte x;
        private byte y;
        private int height;
        private int width;
        private string windowName;
        private Logger logger;
        private Term term;

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


        public Window(byte startX, byte startY, int height, int width, string windowName, Term term)
        {
            var logger = new Logger("WINDOW", Logger.Level.INFO);
            this.logger = logger;
            x = startX;
            y = startY;
            this.height = height;
            this.width = width;
            this.windowName = windowName;
            this.term = term;
            DrawRectangle(x, y, this.height, this.width);
            term.SetCursorAddress(new Term.CursorAddress { x = (byte)(this.x + 2), y = this.y });
            term.Send(this.windowName);
            logger.log(Logger.Level.INFO, "New window created \"" + windowName + "\"");
        }

        public void text(string data, byte localx, byte localy, bool clear = false)
        {
            Thread.Sleep(15);
            logger.log(Logger.Level.DEBUG, "Adding text to \"" + windowName + "\" at : " + localx + " " + localy);
            term.SetCursorAddress(new Term.CursorAddress { x = (byte)(localx + this.x + 1), y = (byte)(localy + this.y + 1) });
            if(clear)
            {
                var clears = width - data.Length - 2;
                term.Send(data.PadRight(clears, ' '));
            }
            else
                term.Send(data);

        }

        

        private void DrawRectangle(byte startX, byte startY, int height, int width)
        {
            logger.log(Logger.Level.DEBUG, "Drawing rectangle at : " + startX + " " + startY);
            term.SetCursorAddress(new Term.CursorAddress { x = startX, y = startY });
            for (int i = 0; i < width; i++)
            {
                term.SendChar('-');
                Thread.Sleep(1);
            }
            term.SetCursorAddress(new Term.CursorAddress { x = (byte)(startX), y = (byte)(startY + height - 1) });
            for (int i = 0; i < width; i++)
            {
                term.SendChar('-');
                Thread.Sleep(1);
            }
            for (int i = 0; i < height; i++)
            {
                term.SetCursorAddress(new Term.CursorAddress { x = startX, y = (byte)(startY + i) });
                term.SendChar('|');
                Thread.Sleep(1);
                term.SetCursorAddress(new Term.CursorAddress { x = (byte)(startX + width - 1), y = (byte)(startY + i) });
                term.SendChar('|');
                Thread.Sleep(1);
            }

        }





    }
}
