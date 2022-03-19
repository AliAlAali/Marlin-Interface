using MarlinCSharpNet.Communication;
using MarlinCSharpNet.GCode;
using System;
using System.Collections.Generic;
using System.Text;

namespace MarlinCSharpNet.Machine
{
    public class Machine
    {
        public Communicator Communicator { get; set; }

        public void Connect()
        {
            if (Communicator.IsConnected())
            {
                return;
            }

            Communicator.Connect();
        }

        public void Execute(GCodeCommand command)
        {
            if (!Communicator.IsConnected())
            {
                return;
            }

            Communicator.SendCommand(command);
        }

        public void ExecutePriority(string command)
        {
            if (!Communicator.IsConnected())
            {
                return;
            }

            Communicator.SendPrioityCommand(command);
        }

        public void Execute(GCodeFileReader reader)
        {
            foreach(var command in reader)
            {
                Execute(command);
            }
        }
    }
}
