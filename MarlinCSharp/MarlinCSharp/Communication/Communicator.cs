using MarlinCSharp.Communication.Exception;
using MarlinCSharp.GCode;
using MarlinCSharp.Machine;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace MarlinCSharp.Communication
{
    /// <summary>
    /// Class for supporing communication between a Marline Machine and a connection
    /// </summary>
    public abstract class Communicator : ICommunicator
    {
        public Connection Connection { get; set; }

        public string SerialKey { get; set; }

        public string CommunicationKey { get; set; }

        public bool Paused { get; set; } = false;

        public MachineStatus Status { get; set; } = MachineStatus.Manual;

        public event Action<string> OnResponseReceived;

        public enum SerialCommunicatorEvent
        {
            COMMAND_SENT,
            COMMAND_SKIPPED,
            RAW_RESPONSE,
            PAUSED
        }


        private BlockingCollection<EventData> EventQueue = new BlockingCollection<EventData>();
        private bool Stop = false;
        private Thread EventThread;

        private void InitalizeThread()
        {
            EventThread = new Thread(this.Process);
        }

        protected void RaiseOnResponseReceived(string response)
        {
            OnResponseReceived?.Invoke(response);
        }

        private void Process()
        {
            while (!Stop)
            {
                try
                {
                    EventData data = EventQueue.Take();
                    Connection.SendString(data.ToString());
                }
                catch
                (System.Exception)
                {
                    Stop = true;
                }
            }
        }

        protected virtual void ClearQueue()
        {
        }

        public void Clear()
        {
            ClearQueue();
        }

        public virtual void Reset()
        {
            if (EventQueue != null)
            {
                EventQueue.Dispose();
            }
        }

        public virtual void Connect()
        {
            if (Connection != null && !Connection.IsOpen())
            {
                Connection.Connect();
                InitalizeThread();
                EventThread.Start();

                if (!string.IsNullOrEmpty(SerialKey))
                {
                    Thread.Sleep(1000);
                    Connection.Write(SerialKey + "\n");
                }
            }
            else
            {
                throw new CommunicationException("Could not connect, since connection is either open or null");
            }
        }

        public virtual bool IsConnected()
        {
            return Connection.IsOpen();
        }

        public virtual void Disconnect()
        {
            if (Connection == null || !Connection.IsOpen())
            {
                return;
            }

            Stop = true;
            EventThread.Interrupt();
            Connection.Close();
        }

        public void DispatchCommand(GCodeCommand command)
        {
            this.EventQueue.Add(new EventData(SerialCommunicatorEvent.COMMAND_SENT, command.ToString(), command));
        }

        public virtual void SendCommand(GCodeCommand command)
        {
            throw new NotImplementedException();
        }

        public virtual void SendPrioityCommand(string command)
        {
            throw new NotImplementedException();
        }

        public void Pause()
        {
            Paused = true;
            Status = MachineStatus.Manual;
        }

        public virtual void Halt()
        {
            Paused = true;
            Status = MachineStatus.Manual;
        }

        public void Resume()
        {
            Paused = false;
            Status = MachineStatus.Operating;
        }

        public virtual bool IsEmpty()
        {
            throw new NotImplementedException();
        }

        // Simple data class used to pass data to the event thread.
        private class EventData
        {
            public EventData(
                SerialCommunicatorEvent sevent,
                string text,
                GCodeCommand command)
            {
                this.SEvent = sevent;
                this.Command = command;
                this.Text = text;
            }

            public SerialCommunicatorEvent SEvent;
            public GCodeCommand Command;
            public string Text;
        }
    }
}
