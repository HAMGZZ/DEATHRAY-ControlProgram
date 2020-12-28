using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Text;
using System.Threading;

namespace ControllProgram
{
    class Comms
    {
        private static SerialPort _SerialPort;
        private Logger logger;
        private string commName;
        public bool writing = true;
        

        public Comms(string commName, int baud, Parity parity = Parity.None, int dataBits = 8, StopBits stopBits = StopBits.One, Handshake handshake = Handshake.None)
        {
            this.commName = commName;
            logger = new Logger("COMMUNICATION", Logger.Level.INFO);
            Thread readThread = new Thread(ReceiveLoop);
            // Create a new SerialPort object with default settings.
            _SerialPort = new SerialPort();
            // Allow the user to set the appropriate properties.
            _SerialPort.PortName = SetPortName(_SerialPort.PortName, commName);
            _SerialPort.BaudRate = baud;
            _SerialPort.Parity = parity;
            _SerialPort.DataBits = dataBits;
            _SerialPort.StopBits = stopBits;
            _SerialPort.Handshake = handshake;
            // Set the read/write timeouts
            _SerialPort.ReadTimeout = 500;
            _SerialPort.WriteTimeout = 500;

            _SerialPort.Open();
            logger.log(Logger.Level.INFO, "Communication stream \"" + commName + "\" started");
            readThread.Start();
            logger.log(Logger.Level.INFO, "Communication reader \"" + commName + "\" started");
        }

        public void EndComms()
        {
            _SerialPort.Close();
            logger.log(Logger.Level.INFO, "Communication stream \"" + commName + "\" ended");
        }

        public void ClearBuffer()
        {
            logger.log(Logger.Level.DEBUG, "Communication stream \"" + commName + "\" cleared");
            _SerialPort.DiscardInBuffer();
        }

        public int DataAvailable()
        {
            try
            {
                return _SerialPort.BytesToRead;
            }
            catch (Exception)
            {

                return 0;
            }
        }

        //send data to port in form of string;
        public void Send(string data)
        {
            writing = true;
            try
            {
                byte[] dataHex = Encoding.Default.GetBytes(data);
                logger.log(Logger.Level.DEBUG, "Sending data : " + BitConverter.ToString(dataHex).Replace("-", ","));
                _SerialPort.Write(data);
            }
            catch (Exception ex)
            {
                logger.log(Logger.Level.WARNING, "Send data failed, exception : " + ex.ToString());
            }
            writing = false;
        }



        public void SendBytes(byte[] bytes, int count)
        {
            writing = true;
            try
            {
                logger.log(Logger.Level.DEBUG, "Sending char : " + BitConverter.ToString(bytes).Replace("-", ","));
                _SerialPort.Write(bytes, 0, count);
            }
            catch (Exception ex)
            {
                logger.log(Logger.Level.WARNING, "Send bytes : '" + BitConverter.ToString(bytes).Replace("-", ",") + "' failed, exception : " + ex.ToString());
            }
            
            writing = false;
        }

        public void SendChar(char data)
        {

            byte[] dataHex = { Convert.ToByte(data) };
            writing = true;
            try
            {
                logger.log(Logger.Level.DEBUG, "Sending char : " + BitConverter.ToString(dataHex).Replace("-", ","));
                _SerialPort.Write(data.ToString());
            }
            catch (Exception ex)
            {
                logger.log(Logger.Level.WARNING, "Send char : '" + BitConverter.ToString(dataHex).Replace("-", ",") + "' failed, exception : " + ex.ToString());
            }
            writing = false;
        }

        public string ReadLine()
        {
            if (_SerialPort.IsOpen)
            {
                try
                {
                    logger.log(Logger.Level.DEBUG, "Reading line...");
                    return _SerialPort.ReadLine();
                }
                catch (Exception ex)
                {
                    logger.log(Logger.Level.WARNING, "ReadLine failed, exception : " + ex.ToString());
                    return "";
                }
            }
            else
                return "";
        }

        public string ReadUntil(string charecter)
        {
            if (_SerialPort.IsOpen)
            {
                try
                {
                    logger.log(Logger.Level.DEBUG, "Reading until received '" + charecter + "'");
                    return _SerialPort.ReadTo(charecter);
                }
                catch (Exception ex)
                {
                    logger.log(Logger.Level.WARNING, "ReadUntil failed, exception : " + ex.ToString());
                    return null;
                }
            }
            else
                return null;
        }

        //reads x number of bytes then clears buffer.
        public int[] ReadBytes(int numberOfBytesToRead)
        {
            int[] toRead = new int[numberOfBytesToRead];
            if (_SerialPort.IsOpen && _SerialPort.BytesToRead > 0)
            {
                for (int i = 0; i < numberOfBytesToRead; i++)
                {
                    try
                    {
                        logger.log(Logger.Level.DEBUG, "Reading " + numberOfBytesToRead + " bytes received");
                        toRead[i] = _SerialPort.ReadChar();
                    }
                    catch (Exception ex) 
                    {
                        logger.log(Logger.Level.WARNING, "ReadBytes failed, exception : " + ex.ToString());
                    }
                }
            }
            return toRead;
        }

        public char ReadChar()
        {
            if (_SerialPort.IsOpen)
            {
                try
                {
                    var c = Convert.ToChar(_SerialPort.ReadChar());
                    logger.log(Logger.Level.DEBUG, "Reading char : " + Convert.ToByte(c).ToString("x2"));
                    return c;
                }
                catch (Exception ex)
                {
                    logger.log(Logger.Level.WARNING, "ReadChar failed, exception : " + ex.ToString());
                    return '\0';
                }
            }
            else
                return '\0';
        }

        public event EventHandler SerialIn;
        protected virtual void OnSerialIn(EventArgs e)
        {
            EventHandler handler = SerialIn;
            handler?.Invoke(this, e);
        }

        //Thread to check that data is coming in
        private void ReceiveLoop()
        {
            _SerialPort.DataReceived += _SerialPort_DataReceived;
        }

        //Through public SerialIn event;
        private void _SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            OnSerialIn(e);
        }

        // Display Port values and prompt user to enter a port.
        private static string SetPortName(string defaultPortName, string commName)
        {
            string portName;
            Console.Write("Enter COM port value for " + commName + " (Default: {0}): ", defaultPortName);
            portName = Console.ReadLine();

            if (portName == "" || !(portName.ToLower()).StartsWith("com"))
            {
                portName = defaultPortName;
            }
            return portName;
        }
        
    }
}
