using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControlProgram.Remote
{
    class RemoteControl : IRemoteControl
    {
        public RemoteControl()
        {

        }

        public int Open()
        {

            return 0;
        }

        public int GetData()
        {

            return 0;
        }

        public int SetData()
        {

            return 0;
        }

        private int send()
        {

            return 0;
        }

        private int read()
        {
            return 0;
        }
    }

    class RemoteControlTest : IRemoteControl
    {
        public int GetData()
        {
            throw new NotImplementedException();
        }

        public int Open()
        {
            throw new NotImplementedException();
        }

        public int SetData()
        {
            throw new NotImplementedException();
        }
    }
}
