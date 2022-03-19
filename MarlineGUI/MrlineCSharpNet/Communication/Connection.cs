using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Text;

namespace MarlinCSharpNet.Communication
{
    public abstract class Connection
    {
        public enum ConnectionStatus
        {
            Connected,
            Disconnected
        }

        private ConnectionStatus Status { get; set; }


        public abstract Stream GetStream();

        public abstract void Connect();

        public abstract void Close();

        public abstract bool IsOpen();

        public abstract void InitalizePort(string name, int buadrate);

        #region Send & Receive
        public abstract void SendBytePriority(byte b);

        public abstract void SendString(string command);

        public abstract void OnDataReceived(object sender, SerialDataReceivedEventArgs e);
        #endregion

        public abstract IEnumerable<string> GetOpenPorts();
    }
}
