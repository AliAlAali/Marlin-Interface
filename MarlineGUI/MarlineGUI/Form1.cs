using MarlinCSharpNet.Communication;
using MarlinCSharpNet.GCode;
using MarlinCSharpNet.Machine;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MarlineGUI
{
    public partial class Form1 : Form
    {
        private Machine Machine { get; set; }
        private List<string> Responses = new List<string>();

        public Form1()
        {
            InitializeComponent();

            var serialConnection = new SerialConnection();
            CCBox_ports.DataSource = serialConnection.GetOpenPorts().ToList();
            

        }

        private void Btn_connect_Click(object sender, EventArgs e)
        {
            var serialConnection = new SerialConnection();
            var port = CCBox_ports.SelectedValue.ToString();
            serialConnection.InitalizePort(port, 115200);

            var communicator = new BufferedCommunicator()
            {
                Connection = serialConnection,
            };

            communicator.OnResponseReceived += Communicator_OnResponseReceived;

            Machine = new Machine() { Communicator = communicator};
          
            Machine.Connect();

            Lbl_status.Text = "Connected";


        }

        private void Communicator_OnResponseReceived(string obj)
        {
            LSTB_response.Invoke((MethodInvoker)(() =>
            {
                LSTB_response.Items.Add(obj);
                LSTB_response.SelectedIndex = LSTB_response.Items.Count - 1;
            }));
            
          
        }

        private void Lbl_status_Click(object sender, EventArgs e)
        {

        }

        private void Btn_sendFile_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            var path = openFileDialog1.FileName;
            var reader = GCodeFileReader.Open(path);
            Machine.Execute(reader);


        }

        private void Btn_x_neg_Click(object sender, EventArgs e)
        {
            Machine.Execute(new GCodeCommand() { Command = "G0 X0" });
        }

        private void Btn_x_pos_Click(object sender, EventArgs e)
        {
            Machine.Execute(new GCodeCommand() { Command = "G0 X40" });
        }
    }
}
