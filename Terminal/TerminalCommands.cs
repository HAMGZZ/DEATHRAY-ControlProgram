using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControlProgram.Terminal
{
    static class TerminalCommands
    {
        public struct CursorAddress
        {
            public byte x;
            public byte y;
            public byte[] ToBytes()
            {
                byte[] bytes = new byte[4];
                bytes[0] = 0x1B;
                bytes[1] = 0x11;
                bytes[2] = x;
                bytes[3] = y;
                return bytes;
            }

            public string ToString()
            {
                return Encoding.ASCII.GetString(ToBytes());
            }
        };


        public static string clear = "\x1B\x1C";
        public static string cursorHome = "\x1B\x12";
        public static string clearLine = "\n\x1B\x0F";
        public static string test1 = "\x1B\x22";
        public static string test2 = "\x1B\x38";
        public static string underline = "\x1b \x64\x47";
        public static string deleteLine = "\x1B\x13";


        public static string SetCursor(CursorAddress address)
        {
            return address.ToString();
        }
    }
}
