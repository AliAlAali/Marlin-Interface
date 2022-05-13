namespace MarlineGUI
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);

            Machine.Communicator.Disconnect();

        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.CCBox_ports = new System.Windows.Forms.ComboBox();
            this.Btn_connect = new System.Windows.Forms.Button();
            this.Lbl_status = new System.Windows.Forms.Label();
            this.Btn_sendFile = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.LSTB_response = new System.Windows.Forms.ListBox();
            this.Btn_x_pos = new System.Windows.Forms.Button();
            this.Btn_x_neg = new System.Windows.Forms.Button();
            this.Btn_pause = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // CCBox_ports
            // 
            this.CCBox_ports.FormattingEnabled = true;
            this.CCBox_ports.Location = new System.Drawing.Point(30, 25);
            this.CCBox_ports.Name = "CCBox_ports";
            this.CCBox_ports.Size = new System.Drawing.Size(179, 21);
            this.CCBox_ports.TabIndex = 0;
            // 
            // Btn_connect
            // 
            this.Btn_connect.Location = new System.Drawing.Point(215, 23);
            this.Btn_connect.Name = "Btn_connect";
            this.Btn_connect.Size = new System.Drawing.Size(75, 23);
            this.Btn_connect.TabIndex = 1;
            this.Btn_connect.Text = "Connect";
            this.Btn_connect.UseVisualStyleBackColor = true;
            this.Btn_connect.Click += new System.EventHandler(this.Btn_connect_Click);
            // 
            // Lbl_status
            // 
            this.Lbl_status.AutoSize = true;
            this.Lbl_status.Location = new System.Drawing.Point(30, 53);
            this.Lbl_status.Name = "Lbl_status";
            this.Lbl_status.Size = new System.Drawing.Size(35, 13);
            this.Lbl_status.TabIndex = 2;
            this.Lbl_status.Text = "label1";
            this.Lbl_status.Click += new System.EventHandler(this.Lbl_status_Click);
            // 
            // Btn_sendFile
            // 
            this.Btn_sendFile.Location = new System.Drawing.Point(30, 69);
            this.Btn_sendFile.Name = "Btn_sendFile";
            this.Btn_sendFile.Size = new System.Drawing.Size(257, 23);
            this.Btn_sendFile.TabIndex = 3;
            this.Btn_sendFile.Text = "Send File";
            this.Btn_sendFile.UseVisualStyleBackColor = true;
            this.Btn_sendFile.Click += new System.EventHandler(this.Btn_sendFile_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            this.openFileDialog1.FileOk += new System.ComponentModel.CancelEventHandler(this.openFileDialog1_FileOk);
            // 
            // LSTB_response
            // 
            this.LSTB_response.FormattingEnabled = true;
            this.LSTB_response.Location = new System.Drawing.Point(30, 111);
            this.LSTB_response.Name = "LSTB_response";
            this.LSTB_response.Size = new System.Drawing.Size(257, 277);
            this.LSTB_response.TabIndex = 4;
            // 
            // Btn_x_pos
            // 
            this.Btn_x_pos.Location = new System.Drawing.Point(429, 152);
            this.Btn_x_pos.Name = "Btn_x_pos";
            this.Btn_x_pos.Size = new System.Drawing.Size(75, 71);
            this.Btn_x_pos.TabIndex = 5;
            this.Btn_x_pos.Text = ">";
            this.Btn_x_pos.UseVisualStyleBackColor = true;
            this.Btn_x_pos.Click += new System.EventHandler(this.Btn_x_pos_Click);
            // 
            // Btn_x_neg
            // 
            this.Btn_x_neg.Location = new System.Drawing.Point(333, 152);
            this.Btn_x_neg.Name = "Btn_x_neg";
            this.Btn_x_neg.Size = new System.Drawing.Size(75, 71);
            this.Btn_x_neg.TabIndex = 6;
            this.Btn_x_neg.Text = "<";
            this.Btn_x_neg.UseVisualStyleBackColor = true;
            this.Btn_x_neg.Click += new System.EventHandler(this.Btn_x_neg_Click);
            // 
            // Btn_pause
            // 
            this.Btn_pause.Location = new System.Drawing.Point(333, 111);
            this.Btn_pause.Name = "Btn_pause";
            this.Btn_pause.Size = new System.Drawing.Size(75, 23);
            this.Btn_pause.TabIndex = 7;
            this.Btn_pause.Text = "Pause";
            this.Btn_pause.UseVisualStyleBackColor = true;
            this.Btn_pause.Click += new System.EventHandler(this.Btn_pause_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(636, 410);
            this.Controls.Add(this.Btn_pause);
            this.Controls.Add(this.Btn_x_neg);
            this.Controls.Add(this.Btn_x_pos);
            this.Controls.Add(this.LSTB_response);
            this.Controls.Add(this.Btn_sendFile);
            this.Controls.Add(this.Lbl_status);
            this.Controls.Add(this.Btn_connect);
            this.Controls.Add(this.CCBox_ports);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox CCBox_ports;
        private System.Windows.Forms.Button Btn_connect;
        private System.Windows.Forms.Label Lbl_status;
        private System.Windows.Forms.Button Btn_sendFile;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.ListBox LSTB_response;
        private System.Windows.Forms.Button Btn_x_pos;
        private System.Windows.Forms.Button Btn_x_neg;
        private System.Windows.Forms.Button Btn_pause;
    }
}

