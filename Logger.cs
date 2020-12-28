using System;

namespace ControllProgram
{
    class Logger
    {
        //Logger levels, FATAL will stop the program
        public enum Level
        {
            DEBUG,
            INFO,
            WARNING,
            ERROR,
            FATAL
        }


        private string system;
        public Level minLogLevel;

        //System is the subsystem name, minloglevel is the lowest level it will broadcast.
        public Logger(string system, Level minLogLevel) 
        {
            this.system = system;
            this.minLogLevel = minLogLevel;
        }


        public void log(Level level, string message)
        {
            var color = new ConsoleColor();
            switch (level)
            {
                case Level.DEBUG:
                    color = ConsoleColor.Yellow;
                    break;
                case Level.INFO:
                    color = ConsoleColor.Cyan;
                    break;
                case Level.ERROR:
                    color = ConsoleColor.Red;
                    break;
                case Level.WARNING:
                    color = ConsoleColor.Magenta;
                    break;
                case Level.FATAL:
                    color = ConsoleColor.DarkBlue;
                    break;
            }
            if (level >= minLogLevel)
            {
                Console.Write("[LOG][" + DateTime.Now.ToString("HH:mm:ss.fff") + "][");
                Console.ForegroundColor = color;
                Console.Write(level.ToString("g"));
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine("][" + system + "]:: " + message);
                if(level == Level.FATAL)
                {
                    Environment.Exit(1);
                }
            }
        }
    }
}
