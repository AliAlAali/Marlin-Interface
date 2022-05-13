using MarlinCSharp.Communication;
using MarlinCSharp.Communication.Exception;
using MarlinCSharp.GCode;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace MarlinCSharp.Machine
{
    public class Machine
    {
        // Private variables
        private bool Online = false;
        private int WriteFails = 0;

        // Public variables
        public Communicator Communicator { get; set; }

        public int FeedRate { get; set; } = 1;

        public bool Ready { get; set; } = false;

        public bool ReadyForInitalization()
        {
            return Ready;
        }

        public void Connect()
        {
            Communicator?.Connect();
        }

        public void Disconnect()
        {
            Online = false;
            WriteFails = 0;
            Ready = false;

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
            Online = false;
            WriteFails = 0;
            Ready = false;

            Communicator?.Halt();
        }

        public void Restart()
        {
            Online = false;
            WriteFails = 0;
            Ready = false;

            if (Communicator != null && Communicator.IsConnected())
            {
                Communicator.Reset();
            }
        }

        public void SoftReset()
        {
            Online = false;
            WriteFails = 0;
            Ready = false;

            Communicator?.Clear();
        }

        public void Clear()
        {
            Communicator?.Clear();
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
            foreach (var command in reader)
            {
                Execute(command);
            }
        }

        private bool ContinueOnlineChecking()
        {
            return !Online && Communicator != null && Communicator.IsConnected();
        }

        /// <summary>
        /// Waits for the machine to be online for a certian amount of time
        /// </summary>
        /// <param name="timeout">Timeout for the checking to expire for a set number of milliseconds per attempt</param>
        /// <param name="attempts">Number of attempts to check online status</param>
        public void WaitUntilOnline(int timeout, int attempts)
        {
            if(Communicator == null)
            {
                throw new CommunicationException("Communicator was not set. Unable to check machine online status");
            }



            new Thread(() =>
            {
                Communicator.OnResponseReceived += Communicator_OnlineCheck_OnResponseReceived;
                Execute(new GCodeCommand() { Command = "M105" }); // Selected to read tempreature as a response, since it will not be available until machine is ready.
                if (WriteFails > attempts)
                {
                    throw new CommunicationException("Unable to communicate with machine and checking online status");
                }


                Communicator.OnResponseReceived -= Communicator_OnlineCheck_OnResponseReceived;

            }).Start();



        }

        private void Communicator_OnlineCheck_OnResponseReceived(string response)
        {
            if (String.IsNullOrEmpty(response))
            {
                WriteFails++;
            }
            else if(response.StartsWith("ok") || response.Contains("T:"))
            {
                Online = true;
                Ready = true;
            }
        }
    }
}
