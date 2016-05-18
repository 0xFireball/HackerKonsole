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
            pastCommand.Text = enterCommand.Text + "\r\n";
            enterCommand.Text = "";
            Task.Run(() => SendCommand(enterCommand.Text));
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

        private void button2_Click(object sender, EventArgs e)
        {
            new UtilityChooser().Show();
        }
    }
}