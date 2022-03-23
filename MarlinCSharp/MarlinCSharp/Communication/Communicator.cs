using MarlinCSharp.Communication.Exception;
using MarlinCSharp.GCode;
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

        public bool Paused { get; set; } = false;

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

        public void Reset()
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
        }

        public virtual void Halt()
        {
            Paused = true;
        }

        public void Resume()
        {
            Paused = false;
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
