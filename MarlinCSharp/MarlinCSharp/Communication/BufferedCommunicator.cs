using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MarlinCSharp.GCode;
using MarlinCSharp.Machine;

namespace MarlinCSharp.Communication
{
    public class BufferedCommunicator : Communicator
    {
        public bool SyncBuffer { get; set; }

        private StreamWriter Writer;
        private StreamReader Reader;
        private Thread WorkerThread;

        List<GCodeCommand> Sent = new List<GCodeCommand>();
        Queue toSend = Queue.Synchronized(new Queue());
        Queue ToSendP = Queue.Synchronized(new Queue());

        private int LineNumber = 1;
        private int ResendFrom = -1;


        public BufferedCommunicator()
        {
        }

        private void Work()
        {
            Stream stream = null;
            try
            {
                stream = Connection.GetStream();
                Writer = new StreamWriter(stream);
                Reader = new StreamReader(stream);

                Writer.Write("M110 N0");
                Writer.Flush();


                while (true)
                {
                    Thread.Sleep(1); // Cause thread to block for a tiny amount => allows interrupts
                    Task<string> lineTask = Reader.ReadLineAsync();

                    while (!lineTask.IsCompleted)
                    {
                        if (!Connection.IsOpen())
                        {
                            return;
                        }

                        while (ToSendP.Count > 0)
                        {
                            var commandP = ToSendP.Dequeue();
                            Writer.Write(commandP.ToString());
                            Writer.Write("\n");
                            Writer.Flush();
                        }

                        if(ResendFrom != -1 && ResendFrom < LineNumber && Status == MachineStatus.Operating)
                        {
                            if(ResendFrom < Sent.Count)
                            {
                                // Detect a racing condition, commit suicide
                                //Thread.Sleep(1000);
                            }
                            var command = Sent[ResendFrom - 1];
                            Writer.Write(command.GetCheckedCommand(ResendFrom));
                            Writer.Write("\n");
                            Writer.Flush();

                            RaiseOnResponseReceived($"Resent command {command} #{ResendFrom}");

                            ResendFrom++;
                            Thread.Sleep(10);
                        }
                        else if (toSend.Count > 0 && Status == MachineStatus.Operating)
                        {
                            ResendFrom = -1;
                            var command = toSend.Dequeue() as GCodeCommand;
                            Writer.Write(command.GetCheckedCommand(LineNumber));
                            Writer.Write("\n");
                            Writer.Flush();

                            Sent.Add(command);
                            RaiseOnResponseReceived($"Send command {command} #{LineNumber}");
                            LineNumber++;

                            Thread.Sleep(50);
                        }

                    }

                    string response = lineTask.Result;
                    RaiseOnResponseReceived(response);

                    // Handle error or resend command requests
                    if (response.ToLower().StartsWith("resend"))
                    {
                        int startNum = response.IndexOf(":");
                        string num = response.Substring(startNum + 2);
                        int target = int.Parse(num);
                        ResendFrom = target;
                        Thread.Sleep(50);
                    }
                }

            }
            catch(ThreadInterruptedException thiex)
            {
                //Writer.Close();
                //Writer = null;
                //Reader.Close();
                //Reader = null;

                //stream.Close();
                //stream = null;

                // Read all stream to avoid reading old messages
              

                LineNumber = 1;
                ResendFrom = -1;
                ClearQueue();

                // After clearing the queue
                //SendPrioityCommand("M110 -1"); //Reset line number

            }
            catch (System.Exception ex)
            {
                var debug = 5;
            }
            
        }


        protected override void ClearQueue()
        {
            toSend.Clear();
            ToSendP.Clear();
            Sent.Clear();
        }

        public override void Reset()
        {
            base.Reset();
            //ResendFrom = -1;
            //ClearQueue();
            WorkerThread.Interrupt();
            WorkerThread.Join();
            WorkerThread = new Thread(Work)
            {
                Priority = ThreadPriority.AboveNormal
            };
            WorkerThread.Start();
        }

        public override void Connect()
        {
            base.Connect();
            //ClearQueue();


            WorkerThread = new Thread(Work)
            {
                Priority = ThreadPriority.AboveNormal
            };
            WorkerThread.Start();
        }

        public override void Disconnect()
        {
            base.Disconnect();
            //WorkerThread.Join();
            Status = MachineStatus.Manual;
            WorkerThread.Interrupt();
            ClearQueue();
        }

        public override void Halt()
        {
            base.Halt();
            Status = MachineStatus.Manual;
            ClearQueue();
            WorkerThread.Interrupt();
            //WorkerThread.Join();
            //ResendFrom = -1;
            //ClearQueue();

            WorkerThread = new Thread(Work)
            {
                Priority = ThreadPriority.AboveNormal
            };
            WorkerThread.Start();
        }

        public override bool IsEmpty()
        {
            return toSend.Count == 0;
        }

        public override void SendCommand(GCodeCommand command)
        {
            toSend.Enqueue(command);
        }

        public override void SendPrioityCommand(string command)
        {
            ToSendP.Enqueue(command);
        }

    }
}
