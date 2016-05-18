using System;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace HackerKonsole.Controller.GUI
{
    public partial class CommandLine : Form
    {
        public CommandLine()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            Task.Run(() => SendCommand());
        }

        private void SendCommand()
        {
            
        }

    }
}
