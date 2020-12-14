using System;
using System.Collections.Generic;
using System.Text;

namespace ControllProgram
{
    class Logger
    {
        public enum Level
        {
            DEBUG,
            INFO,
            WARNING,
            ERROR
        }


        private string system;
        public Level minLogLevel;

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
            }
            if (level >= minLogLevel)
            {
                Console.Write("[LOG][" + DateTime.Now.ToString("HH:mm:ss.fff") + "][");
                Console.ForegroundColor = color;
                Console.Write(level.ToString("g"));
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine("][" + system + "]:: " + message);
            }
        }
    }
}
