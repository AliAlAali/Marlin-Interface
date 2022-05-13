using MarlinCSharpNet.Communication;
using MarlinCSharpNet.GCode;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace MarlinCSharpNet.Machine
{
    public class Machine
    {
        public Communicator Communicator { get; set; }

        public int FeedRate { get; set; } = 1;

        public void Connect()
        {
            Communicator?.Connect();
        }

        public void Disconnect()
        { 
            Communicator?.Disconnect();
        }

        public void Pause()
        {
            Communicator?.Pause();
        }

        public void Resume()
        {
            Communicator?.Resume();
        }

        public void Stop()
        {
            Communicator?.Halt();
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

        public void ExecutePriority(GCodeCommand command)
        {
            ExecutePriority(command.ToString());
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
