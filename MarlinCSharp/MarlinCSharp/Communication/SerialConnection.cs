using MarlinCSharp.Communication.Excaptoin;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Text;

namespace MarlinCSharp.Communication
{
    public class SerialConnection : Connection
    {
        private SerialPort Port;

        public event Action<string> ResponseHandler;


        public override void Close()
        {
            if(Port != null)
            {
                Port.DataReceived -= OnDataReceived;
                Port.Close();
            }
        }

        public override void Connect()
        {
            if (Port == null)
            {
                throw new ConnectionException("Serial Port was not initalized");
            }

            if (Port.IsOpen)
            {
                throw new ConnectionException("Could not connecto to Seial Port, since it is already open");
            }

            Port.Open();
        }

        public override IEnumerable<string> GetOpenPorts()
        {
            return SerialPort.GetPortNames();
        }

        public override void InitalizePort(string name, int buadrate)
        {
            if (Port != null && Port.IsOpen)
            {
                Close();
            }

            Port = new SerialPort()
            {
                PortName = name,
                BaudRate = buadrate,
                Parity = Parity.None,
                DataBits = 8,
                StopBits = StopBits.One,
                DtrEnable = true
            };

            Port.DataReceived += OnDataReceived;
        }

        public override bool IsOpen()
        {
            return Port.IsOpen;
        }

        public override void OnDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if(e.EventType != SerialData.Eof)
            {
                return;
            }

            string line = Port.ReadLine();
            ResponseHandler?.Invoke(line);

        }

        public override void SendBytePriority(byte b)
        {
            Port.Write(new byte[1] { b }, 0, 1);
        }

        public override void SendString(string command)
        {
            Port.WriteLine(command);
        }
    }
}
