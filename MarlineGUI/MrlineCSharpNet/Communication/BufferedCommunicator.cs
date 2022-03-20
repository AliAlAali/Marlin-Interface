using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MarlinCSharpNet.GCode;

namespace MarlinCSharpNet.Communication
{
    public class BufferedCommunicator : Communicator
    {
        public bool SyncBuffer { get; set; }

        private StreamWriter Writer;
        private StreamReader Reader;
        private Thread WorkerThread;

        Queue Sent = Queue.Synchronized(new Queue());
        Queue toSend = Queue.Synchronized(new Queue());
        Queue ToSendP = Queue.Synchronized(new Queue());


        public BufferedCommunicator()
        {
        }

        private void Work()
        {
            try
            {
                var stream = Connection.GetStream();
                Writer = new StreamWriter(stream);
                Reader = new StreamReader(stream);

                while (true)
                {
                    
                    Task<string> lineTask = Reader.ReadLineAsync();

                    while (!lineTask.IsCompleted)
                    {
                        if (!Connection.IsOpen() || Paused)
                        {
                            return;
                        }

                        while(ToSendP.Count > 0)
                        {
                            var commandP = ToSendP.Dequeue();
                            Writer.Write(commandP.ToString());
                            Writer.Flush();
                        }

                        if(toSend.Count > 0)
                        {
                            var command = toSend.Dequeue();
                            Writer.Write(command.ToString());
                            Writer.Write("\n");
                            Writer.Flush();

                            Sent.Enqueue(command);
                            Thread.Sleep(10);
                        }

                    }

                    string response = lineTask.Result;
                    RaiseOnResponseReceived(response);
                }

            }
            catch (System.Exception ex)
            {
                var debug = 5;
            }
        }

        private void ClearQueue()
        {
            toSend.Clear();
            ToSendP.Clear();
            Sent.Clear();
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
            WorkerThread.Join();
            ClearQueue();
        }

        public override void Halt()
        {
            base.Halt();
            WorkerThread.Join();
            ClearQueue();
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
