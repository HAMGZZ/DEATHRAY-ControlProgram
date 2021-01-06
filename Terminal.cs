using System;
using System.Collections.Generic;
using System.Threading;

namespace ControlProgram
{
    class Term
    {
        private List<char> inputBuffer = new List<char>();
        private Comms VT4100;
        private Logger logger;
        private bool echoTypedEnable = false;
        private StarDatabase database;
        private ObjectDataRecords currentObject;
        private ObjectDataRecords earth;
        private Remote remote;
        //WINDOWS
        private Window w_data;
        private Window w_objectData;
        private Window w_vision;
        private Window w_prompt;

        private List<char> toPrintBuffer = new List<char>();

        private OrbitCalculator calc;

        public Term(StarDatabase database, Remote remote)
        {
            VT4100 = new Comms("VT4100", 19200, System.IO.Ports.Parity.Even, 7);
            logger = new Logger("TERMINA", Logger.Level.INFO);
            this.database = database;
            this.remote = remote;
            VT4100.Send(clear);
            VT4100.Send("CZGZZ DEATHCOM - LEWIS HAMILTON 2020 - https://czgzz.space");
            Thread.Sleep(1000);
            logger.log(Logger.Level.DEBUG, "Input event created");
            Thread.Sleep(100);
            VT4100.ClearBuffer();
            loadingScreen();
            database.load(this);
            var earthLoc = database.search("EARTH");
            calc = new OrbitCalculator(database.data[earthLoc], 151, -33);
        }

        public struct CursorAddress
        {
            public byte x;
            public byte y;
            public byte[] ToSend()
            {
                byte[] toSend = new byte[4];
                toSend[0] = 0x1B;
                toSend[1] = 0x11;
                toSend[2] = x;
                toSend[3] = y;
                return toSend;
            }
        };

        //LIST OF VT4100 COMMANDS:
        public string clear =              "\x1B\x1C";
        public string cursorHome =         "\x1B\x12";
        public string clearLine =          "\n\x1B\x0F";
        public string test1 =              "\x1B\x22";
        public string test2 =               "\x1B\x38"; 
        public string underline =          "\x1b \x64\x47";
        public string deleteLine =          "\x1B\x13";

        public void end()
        {
            VT4100.EndComms();
        }
        public void SetCursorAddress(CursorAddress address)
        {
            var prevEcho = echoTypedEnable;
            echoTypedEnable = false;
            logger.log(Logger.Level.DEBUG, "Moving cursor to : " + address.x + " " + address.y);
            VT4100.SendBytes(address.ToSend(), 4);
            echoTypedEnable = prevEcho;
        }
        public CursorAddress GetCursorAddress()
        {
            var prevEcho = echoTypedEnable;
            CursorAddress address;
            address.x = 100;
            address.y = 100;
            echoTypedEnable = false;
            VT4100.Send("\x1B\x05");
            Thread.Sleep(1);
            var response = VT4100.ReadBytes(2);
            if (response[0] < 0x60)
                address.x = (byte)response[0];
            else
                address.x = (byte)(response[0] - 96);
            address.y = (byte)(response[1] - 96);
            echoTypedEnable = prevEcho;
            logger.log(Logger.Level.DEBUG, "Current cursor : " + address.x + " " + address.y);
            return address;
        }

        private bool checkCursor(CursorAddress address)
        {
            if (address.x > 80)
                return false;
            if (address.y > 24)
                return false;
            return true;

        }

        public void Send(string message)
        {
            VT4100.Send(message);
        }
        public void SendChar(char c)
        {
            VT4100.SendChar(c);
        }
        
        public void loadingScreen()
        {
            VT4100.Send(clear);
            VT4100.Send("Loading databse ...");
        }

        public void barGraph(CursorAddress startCursorAddress, CursorAddress returnCursorAddress, int width, string fillChar, float count, float maxCount)
        {
            SetCursorAddress(startCursorAddress);
            VT4100.Send("[");
            float totalCount = width - 6;
            var percentage = ((count / maxCount) * totalCount);
            logger.log(Logger.Level.DEBUG, "Bargraph percent: " + percentage);
            for (int i = 0; i < totalCount; i++)
            {
                if(i <= percentage)
                {
                    VT4100.Send(fillChar);
                }
                else
                {
                    VT4100.Send(" ");
                }
            }
            VT4100.Send("]" + (count/maxCount * 100).ToString("0").PadLeft(3) + "%");
            SetCursorAddress(returnCursorAddress);
        }

        public void draw()
        {
            echoTypedEnable = false;
            logger.log(Logger.Level.INFO, "Drawing screen...");
            VT4100.Send(clear);
            w_data = new Window(0, 1, 10, 30, "Data", this);
            w_objectData = new Window(w_data.Left, 1, w_data.Height, 80 - w_data.Width + 1, "Object Data", this);
            w_vision = new Window(0, w_data.Bottom, 11, 80, "Vision", this);
            w_prompt = new Window(0, 20, 3, 80, "Prompt", this);
            w_data.text("Current AZ:", 0, 0);
            w_data.text("Current EL:", 0, 1);
            w_data.text("Desired AZ:", 0, 2);
            w_data.text("Desired EL:", 0, 3);
            w_data.text("Moving?", 0, 4);
            w_data.text("GPS: N/A", 0, 5);
            w_data.text("Remote power V:", 0, 6);
            w_objectData.text("Name: ", 0, 0);
            w_objectData.text("Distance: ", 0, 1);
            w_objectData.text("Ra/Dec: ", 0, 2);
            w_objectData.text("Cart.: ", 0, 3);
            w_objectData.text("AZ: ", 0, 4);
            w_objectData.text("EL: ", 0, 5);
            w_objectData.text("LST: ", 35, 0);
            w_objectData.text("HA: ", 35, 1);
            w_vision.text("N", 0, 8);
            w_vision.text("E", 19, 8);
            w_vision.text("S", 39, 8);
            w_vision.text("W", 58, 8);
            w_vision.text("N", 77, 8);
            w_prompt.text("Command >> ", 0, 0);
            SetCursorAddress(new CursorAddress { x = 12, y = 21 });
            logger.log(Logger.Level.INFO, "Drawing screen done");
            inputBuffer.Clear();
            echoTypedEnable = true;
        }

        public void update()
        { 
            if (currentObject != null)
                calc.DirectionFinder(currentObject);
            echoTypedEnable = false;
            logger.log(Logger.Level.DEBUG, "Updating screen");
            SetCursorAddress(new CursorAddress { x = 0, y = 0 });
            VT4100.Send(underline + "DEATHCOM".PadRight(74, ' ') + DateTime.UtcNow.ToString("HH:mm"));
            Thread.Sleep(10);
            if (currentObject != null)
            {
                w_objectData.text(currentObject.Name.PadRight(20, ' '), 11, 0);
                w_objectData.text(currentObject.Distance.ToString().PadRight(20, ' '), 11, 1);
                w_objectData.text((currentObject.Ra.ToString("0.##") + "/" + currentObject.Dec.ToString("0.##")).PadRight(20, ' '), 11, 2);
                w_objectData.text((currentObject.CartX.ToString("0.##") + ", " + currentObject.CartY.ToString("0.##") + ", " + currentObject.CartZ.ToString("0.##")).PadRight(20, ' '), 11, 3);
                w_objectData.text((currentObject.Az.ToString("0.##").PadRight(20, ' ')), 11, 4);
                w_objectData.text((currentObject.El.ToString("0.##").PadRight(20, ' ')), 11, 5);
                w_objectData.text(currentObject.LST.ToString("0.##").PadRight(8, ' '), 40, 0);
                w_objectData.text(currentObject.HourAngle.ToString("0.##").PadRight(8, ' '), 40, 1);
            }
            if (remote.data.connect)
            {
                w_data.text(remote.data.rxCurrentAzEl.Az.ToString(), 15, 0);
                w_data.text(remote.data.rxCurrentAzEl.El.ToString(), 15, 0);
                w_data.text(remote.data.rxDesiredAzEl.Az.ToString(), 15, 0);
                w_data.text(remote.data.rxDesiredAzEl.El.ToString(), 15, 0);
                w_data.text(remote.data.moving.ToString(), 15, 0);
                w_data.text(remote.data.volts.ToString(), 15, 0);
            }

            SetCursorAddress(new CursorAddress { x = (byte)(12), y = 21 });
            foreach (var a in inputBuffer)
            {
                VT4100.SendChar(a);
            }
            echoTypedEnable = true;
            Terminal_SerialIn(VT4100, inputBuffer);
        }
        
        //Vision for major planets
        public void updateVision()
        {
            string[] planets = { "mercury", "venus", "mars", "jupiter", "saturn", "uranus", "neptune" };
            ObjectDataRecords[] planetData = new ObjectDataRecords[7];
            for (int i = 0; i < planets.Length - 1; i++)
            {
                var loc = database.search(planets[i].ToUpper());
                planetData[i] = database.data[loc];
                calc.DirectionFinder(planetData[i]);
            }

            for (int i = 0; i < planets.Length - 1; i++)
            {
                if(planetData[i].El > 0)
                {
                    var x = ExtensionMethods.Map(planetData[i].Az, 0, 360, 0, 77);
                    var y = ExtensionMethods.Map(planetData[i].El, 0, 90, 0, 7);
                    w_vision.text((i + 1).ToString(), (byte)x, (byte)(7 - y));
                }
            }

        }

        private bool acceptableChar(char a)
        {
            switch (a)
            {
                case '\u0008':
                    return true;
                case '\u000A':
                    return true;
                case '\u000D':
                    return true;
                case '\u001B':
                    return true;
                default:
                    break;
            }
            if (a < 32)
                return false;
            else
                return true;
            return false;
        }

        private void Terminal_SerialIn(Comms comms, List<Char> buffer)
        {
            if (VT4100.DataAvailable() > 0)
            {
                var inChar = comms.ReadChar();
                //foreach (var inChar in inString)
                //{
                    if (acceptableChar(inChar))
                    {
                        switch (inChar)
                        {
                            case '\b':
                                if (buffer.Count > 0)
                                {
                                    buffer.RemoveAt(buffer.Count - 1);
                                }
                                break;

                            case '\r':
                                string str = new string(buffer.ToArray());
                                command(str.ToUpper());
                                SetCursorAddress(new CursorAddress { x = 12, y = 21 });
                                for (int i = 0; i < str.Length; i++)
                                {
                                    if (echoTypedEnable)
                                        VT4100.Send(" ");
                                }
                                SetCursorAddress(new CursorAddress { x = 12, y = 21 });
                                buffer.Clear();
                                break;

                            default:
                                buffer.Add(inChar);
                                break;
                        }

                    }
                //}
               
            }
        }

        private void command(string command)
        {
            logger.log(Logger.Level.INFO, "Got command : " + command);
            var tokens = command.Split(' ');
            switch (tokens[0])
            {
                case "FOLLOW":
                    if(tokens.Length > 1)
                    {
                        var loc = database.search(tokens[1]);
                        if (loc < 0)
                        {
                            w_vision.text("Could not find " + tokens[1], 0, 0, true);
                        }
                        else
                        {
                            currentObject = database.data[loc];
                            calc.DirectionFinder(currentObject);
                        }
                    }

                    break;
                
                case "R":
                    Thread.Sleep(1000);
                    VT4100.Send(clear);
                    draw();
                    break;

                case "STOP":
                    currentObject = null;
                    break;

                default:
                    
                    break;
            }
        }


        


    }

    public static class ExtensionMethods
    {
        public static double Map(this double value, double fromSource, double toSource, double fromTarget, double toTarget)
        {
            return (value - fromSource) / (toSource - fromSource) * (toTarget - fromTarget) + fromTarget;
        }
    }
}
