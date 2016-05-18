using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using HackerKonsole.Controller.Common;

namespace HackerKonsole.Controller.GUI
{
    public partial class CommandLine : Form
    {
        #region Private Fields

        private readonly ConnectedController _connectedController;

        #endregion Private Fields

        #region Public Constructor

        public CommandLine(ConnectedController connectedController)
        {
            this._connectedController = connectedController;
            InitializeComponent();
        }

        #endregion Public Constructor

        #region Private Methods

        private void button1_Click(object sender, EventArgs e)
        {
            Task.Run(() => SendCommand(enterCommand.Text));
            enterCommand.Text = "";
            pastCommand.Text = enterCommand.Text + "\r\n";
        }

        private void label1_Click(object sender, EventArgs e)
        {
        }

        private void SendCommand(string command)
        {
            _connectedController.SendCommand(command);
        }

        #endregion Private Methods

        private void CommandLine_Load(object sender, EventArgs e)
        {
            _connectedController.ReceiveDataWithCallback(OnDataReceived);
        }

        private void OnDataReceived(string data)
        {
            pastCommand.Text = data + "\r\n";
        }
    }
}