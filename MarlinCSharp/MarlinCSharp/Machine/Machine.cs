using MarlinCSharp.Communication;
using MarlinCSharp.Communication.Exception;
using MarlinCSharp.GCode;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace MarlinCSharp.Machine
{
    public class Machine
    {
        // Private variables
        private bool Online = false;
        private int WriteFails = 0;
        private Communicator _communicator;


        // Public variables
        public event EventHandler<EventArgs> OnPositionChanged;

        public Communicator Communicator
        {
            get
            {
                return _communicator;
            }

            set
            {
                _communicator = value;
                _communicator.OnResponseReceived += _communicator_OnResponseReceived;
            }
        }

        private void _communicator_OnResponseReceived(string response)
        {
             if (response.StartsWith("X:"))
            {
                var positionPattern = @"X:([0-9]+\.[0-9]+) Y:([0-9]+\.[0-9]+) Z:([0-9]+\.[0-9]+) E:([0-9]+\.[0-9]+)";
                var positionMatches = Regex.Match(response, positionPattern);

                if (positionMatches.Groups.Count >= 4)
                {
                    float xPos = (float)double.Parse(positionMatches.Groups[1].Value);
                    float yPos = (float)double.Parse(positionMatches.Groups[2].Value);
                    float zPos = (float)double.Parse(positionMatches.Groups[3].Value);
                    float ePos = (float)double.Parse(positionMatches.Groups[4].Value);

                    Position = new PointF(xPos, yPos);
                    OnPositionChanged?.Invoke(this, new EventArgs());
                }
            }
        }

        public PointF Position { get; set; } = new PointF(0, 0);

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
            Position = new Point(0, 0);

            Communicator?.Halt();
        }

        public void Restart()
        {
            Online = false;
            WriteFails = 0;
            Ready = false;

            Position = new Point(0, 0);
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
            Position = new Point(0, 0);


            Communicator?.Clear();
        }

        public void Clear()
        {
            Position = new Point(0, 0);
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
            if (Communicator == null)
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
            else if (response.StartsWith("ok") || response.Contains("T:"))
            {
                Online = true;
                Ready = true;
            }

        }
    }
}
