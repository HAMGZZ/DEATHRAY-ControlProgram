using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControlProgram.Control
{
    static class Control
    {
        private static Terminal.ITerminal terminal;
        private static ObjectDataRecords currentObject;

        public static void Setup(/* pass through all data here */)
        {
            //Ask for serial ports
            //loaddb in background
            //start for communication with remote
            //if Remote:
            // home tower ASK FIRST,
            // calibrate with position switches
            //if no coms, contuinnue without
            //start coms with terminal, no way to check
            terminal = new Terminal.Terminal("com12");
            terminal.Open();
            terminal.DrawDisplay();
        }

        public static void MainControlLoop()
        {
            terminal.Update(currentObject);
        }
        //start main loop::
        //call terminal update routine - get data back as well
        //call remote update routine - get data back as well
        //do calcuations with any updates
        //repeat

        //On exit : home tower - if comms die for tower, it will home automatically unless told otherwise


    }
}
