using System;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Windows.Forms;
using HackerKonsole.ConnectionServices;
using HackerKonsole.Controller.Common;

namespace HackerKonsole.Controller.GUI
{
    public partial class MainForm : Form
    {
        #region Public Constructors

        public MainForm()
        {
            InitializeComponent();
            Form.CheckForIllegalCrossThreadCalls = false;
            FormBorderStyle = FormBorderStyle.Fixed3D;
        }

        #endregion Public Constructors

        #region Private Methods

        private async void button1_Click(object sender, System.EventArgs e)
        {
            await Task.Run(() => ConnectToServer());
        }

        private void ConnectToServer()
        {
            button1.Text = "Connecting...";
            button1.Enabled = false;
            ConnectionInfo connInfo = null;
            string remoteIp = textBox1.Text;
            int portSepIndex = remoteIp.IndexOf(':');
            int port = 25000;
            if (portSepIndex > 0)
            {
                string[] hBits = remoteIp.Split(':');
                port = int.Parse(hBits[1]);
                remoteIp = hBits[0];
            }
            connInfo = new ConnectionInfo
            {
                RemoteHost = remoteIp,
                RemotePort = port
            };
            using (var tcpConnection = new TcpClient(connInfo.RemoteHost, connInfo.RemotePort))
            {
                var cryptConnection = new CryptTcpClient(tcpConnection);
                try
                {
                    Console.WriteLine("Attempting to establish connection...");
                    cryptConnection.ClientPerformKeyExchange();
                    Console.WriteLine("Connection successfully established!");

                    var connectedController = new ConnectedController(cryptConnection);
                    connectedController.InitializeSession(); //Send the basic stuff
                    var frontendManager = new CommandLine(connectedController);
                    Hide();
                    frontendManager.ShowDialog();
                }
                catch (Exception ex)
                {
                    cryptConnection.Close();
                    MessageBox.Show("An error occurred with the connection: " + ex);
                    button1.Text = "Connect";
                    button1.Enabled = true;
                }
            }
        }

        #endregion Private Methods

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}