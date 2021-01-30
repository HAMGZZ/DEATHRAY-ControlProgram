using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Dynamic;
using System.Threading.Tasks;

namespace ControlProgram.Terminal
{
    class Terminal : ITerminal
    {
        private Window w_remoteData;
        private Window w_currentObjectData;
        private Window w_vision;
        private string SendBuffer;
        private string comPort;




        public Terminal(string comPort)
        {
            this.comPort = comPort;
            w_remoteData = new Window(0, 0, 10, 30, "Box Data");
            w_currentObjectData = new Window(w_remoteData.Left, 1, w_remoteData.Height, 80 - w_remoteData.Width + 1, "Object Data");
            w_vision = new Window(0, w_remoteData.Bottom, 11, 80, "Vision");
        }

        public void Open()
        {
            //Load new coms port to send and receive data from
        }

        public void DrawDisplay()
        {
            send(new string[] { w_remoteData.Draw(), w_currentObjectData.Draw(), w_vision.Draw() });
            send(new string[]
            {
                w_remoteData.text("Current AZ:", 0, 0),
                w_remoteData.text("Current EL:", 0, 1),
                w_remoteData.text("Desired AZ:", 0, 2),
                w_remoteData.text("Desired EL:", 0, 3),
                w_remoteData.text("Moving?", 0, 4),
                w_remoteData.text("GPS: N/A", 0, 5),
                w_remoteData.text("Remote power V:", 0, 6),
                w_currentObjectData.text("Name: ", 0, 0),
                w_currentObjectData.text("Distance: ", 0, 1),
                w_currentObjectData.text("Ra/Dec: ", 0, 2),
                w_currentObjectData.text("Cart.: ", 0, 3),
                w_currentObjectData.text("AZ: ", 0, 4),
                w_currentObjectData.text("EL: ", 0, 5),
                w_currentObjectData.text("LST: ", 35, 0),
                w_currentObjectData.text("HA: ", 35, 1),
                w_vision.text("N", 0, 8),
                w_vision.text("E", 19, 8),
                w_vision.text("S", 39, 8),
                w_vision.text("W", 58, 8),
                w_vision.text("N", 77, 8)
        });
        }

        public void CommandLine()
        {

        }

        private void send(string[] message)
        {
            string buffer = "";
            foreach (var str in message)
            {
                buffer += str;
            }

            //com send buffer;
        }

        private void CurrentObjectDisplay_Update(ObjectDataRecords currentObject)
        {
            throw new NotImplementedException();
        }

        private void RemoteDataDisplay_Update()
        {
            throw new NotImplementedException();
        }

        private void VisionDisplay_Update()
        {
            throw new NotImplementedException();
        }

        public void Update(ObjectDataRecords currentObject)
        {
            throw new NotImplementedException();
            CurrentObjectDisplay_Update(currentObject);
            RemoteDataDisplay_Update();
            VisionDisplay_Update();
        }
    }
}
