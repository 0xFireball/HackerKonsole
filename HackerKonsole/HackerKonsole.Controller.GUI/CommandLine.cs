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
            SendCommandInBox();
        }

        private void SendCommandInBox()
        {
            pastCommand.Text += "HK$>" + enterCommand.Text + "\r\n"; //append the current command to the log
            var cmd = enterCommand.Text;
            Task.Factory.StartNew(() => SendCommand(cmd)); //asynchronously send the command
            enterCommand.Text = ""; //Clear entercommand box
        }

        private void SendCommand(string command)
        {
            _connectedController.SendGenericCommand(command);
        }

        #endregion Private Methods

        private void button2_Click(object sender, EventArgs e)
        {
            new UtilityChooser().Show();
        }

        private void CommandLine_Load(object sender, EventArgs e)
        {
            Task.Factory.StartNew(() => _connectedController.ReceiveDataWithCallback(OnDataReceived)); //Asynchronously subscribe for callbacks
        }

        private void OnDataReceived(string data)
        {
            pastCommand.Text += data + "\r\n";
        }

        private void enterCommand_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                SendCommandInBox();
            }
        }
    }
}