using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;
namespace Resin_Reprogramer
{

    public partial class Form1 : Form
    {
        bool ComSelected = false;
        private void RefreshComPort()
        {

            string[] Ports = SerialPort.GetPortNames();
            comboBox1.Items.Clear();
            comboBox1.Items.AddRange(Ports);
            
        }
        public Form1()
        {
            InitializeComponent();
            RefreshComPort();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            serialPort1.BaudRate = 115200;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComSelected = true;
            serialPort1.PortName = comboBox1.SelectedItem.ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            RefreshComPort();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            pictureBox1.Visible = false;
            pictureBox2.Visible = false;
            byte[] buffer = { 0 };
            if (!serialPort1.IsOpen)
            {
                button2.Focus();
                errorProvider1.SetError(ActiveControl, "Port série non connecté");
                return;
            }
            progressBar1.Visible = true;

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)  {
                label1.Text = saveFileDialog1.FileName;
                var file = saveFileDialog1.OpenFile();
                char[] command = { 'r' };
                serialPort1.Write(command,0,1);
                System.Threading.Thread.Sleep(50);
                serialPort1.Read(buffer, 0, 1024);
                file.Write(buffer, 0, 1024);
                file.Close();
                pictureBox1.Visible = true;
            }

        }

        private void button3_Click(object sender, EventArgs e)
        {
            pictureBox1.Visible = false;
            pictureBox2.Visible = false;
            if (!serialPort1.IsOpen)
            {
                button2.Focus();
                errorProvider1.SetError(ActiveControl, "Port série non connecté");
                return;
            }
            
            progressBar1.Visible = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                label2.Text = openFileDialog1.FileName;
                var file = openFileDialog1.OpenFile();
                byte[] buffer = { 0 };
                progressBar1.Value = 10;
                file.Read(buffer, 0, 1024);
                file.Close();
                progressBar1.Value = 30;
                char[] command = { 'w' };
                serialPort1.Write(command, 0, 1);
                System.Threading.Thread.Sleep(50);
                progressBar1.Value = 50;
                serialPort1.Write(buffer, 0, 1024);
                byte[] Check = { 0};
                char[] commandCheck = { 'r' };
                progressBar1.Value = 70;
                serialPort1.Write(commandCheck, 0, 1);
                System.Threading.Thread.Sleep(50);
                progressBar1.Value = 90;
                serialPort1.Read(Check, 0, 1024);
                progressBar1.Value = 100;
                bool same = true;
                for (int i = 0; i<1024;i++)
                {
                    same = same && Check[i] == buffer[i];
                }
                if (same)
                {
                    pictureBox1.Visible = true;
                }
                else
                {
                    pictureBox2.Visible = true;
                }
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            errorProvider1.Clear();
            if (!ComSelected)
            {
                return;
            }
            if (!serialPort1.IsOpen)
            {
                serialPort1.Open();
                button2.Text = "Fermer";
            }
            else
            {
                serialPort1.Close();
                button2.Text = "Ouvrir";
            }
        }
        


        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            serialPort1.Close();
        }

        private void progressBar1_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }
    }
    
}
