using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace ControlProgram
{
    public class Remote
    {
        private Logger logger;
        private Comms comms;
        public struct AzEl
        {
            public double Az;
            public double El;
        }

        public struct remoteData
        {
            public AzEl rxCurrentAzEl;
            public AzEl rxDesiredAzEl;
            public AzEl txAzEl;
            public bool moving;
            public double volts;
            public bool lights;
            public bool connect;
        }

        public remoteData data;
        public Remote()
        {
            //comms = new Comms("REMOTE", 115200);
            logger = new Logger("REMOTE", Logger.Level.INFO);
            logger.log(Logger.Level.INFO, "Setting up remote comms.");
            Thread remoteLoop = new Thread(remoteCommsLoop);
            remoteLoop.Start();
            logger.log(Logger.Level.INFO, "Started data loop");
        }

        private void remoteCommsLoop()
        {
            while(true)
            {
                /*comms.Send("autoupdate " + data.txAzEl.Az + " " + data.txAzEl.El +"\n\r");
                Thread.Sleep(50);
                if(comms.DataAvailable() > 0)
                {
                    data.connect = true;
                    var incoming = comms.ReadExisting().Split(' ');
                    if(incoming.Length > 1)
                    {
                        data.rxCurrentAzEl.Az = Convert.ToDouble(incoming[0]);
                        data.rxCurrentAzEl.El = Convert.ToDouble(incoming[1]);
                        data.rxDesiredAzEl.Az = Convert.ToDouble(incoming[2]);
                        data.rxDesiredAzEl.El = Convert.ToDouble(incoming[3]);
                        data.moving = Convert.ToBoolean(incoming[4]);
                        data.volts = Convert.ToDouble(incoming[5]);
                    }
                }
                else
                {
                    data.connect = false;
                    logger.log(Logger.Level.ERROR, "Did not receive data from remote box!");
                    Thread.Sleep(5000);
                }*/

                Thread.Sleep(100);
            }
        }
    }
}
