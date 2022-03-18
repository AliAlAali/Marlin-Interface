using MarlinCSharp.GCode;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace MarlinCSharp.Communication
{
    public class Communicator
    {
        protected Connection Connection { get; set; }

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

        private void initalizeThread()
        {
            EventThread = new Thread(this.Process);
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
                (Exception e)
                {
                    Stop = true;
                }
            }
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
