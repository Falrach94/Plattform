using LogUtils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using TestProject;

namespace ServerGUI
{
    public partial class Form1 : Form
    {
        private readonly TestServer _server;
             
        private int _commandId = 0;

        private int _nextHistoryCommand = -1;
        private readonly List<string> _commandHistory = new List<string>();

        public Form1()
        {
            InitializeComponent();

            _server = new TestServer();

            tb_commands.Text = _server.TextInterface.CommandInterface.Command("help");

            _server.LogModule.LogManager.NewMessage += logBox.LogMessageHandler;

            logBox.AddCategory("System", LogLevel.Debug, true, Color.Yellow);
            logBox.AddCategory("Network", LogLevel.Debug, true, Color.LightGreen);
            logBox.AddCategory("Messages", LogLevel.Debug, true, Color.Orange);
            logBox.AddCategory("Connection", LogLevel.Trace, true, Color.LightBlue);
            logBox.AddCategory("Data", LogLevel.Debug, true, Color.Pink);
            logBox.AddCategory("Dummy Module", false);

            logBox.AddCategory("Socket 0", false);
            logBox.AddCategory("Socket 1", false);
            logBox.AddCategory("Socket 2", false);
            logBox.AddCategory("Socket 3", false);


            logBox.InstanceFilter.Add("Server");
        }

        private void ProcessInput(string txt)
        {
            string output = _server.TextInterface.CommandInterface.Command(txt);

            tb_output.Invoke((MethodInvoker)delegate
            {
                tb_output.Text += "(" + _commandId + ") IN: " + txt + "\n";
                tb_output.Text += "(" + _commandId + ") OUT: " + output + "\n";
                _commandId++;
                tb_output.SelectionStart = tb_output.Text.Length;
                tb_output.ScrollToCaret();
            });
        }

        private void Input_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                string cmd = tb_input.Text;

                _commandHistory.Add(cmd);

                _nextHistoryCommand = _commandHistory.Count -1;

                Task.Run(()=>ProcessInput(cmd));

                tb_input.Text = "";

                e.SuppressKeyPress = true;
            }
            if (e.KeyCode == Keys.Up)
            {
                if (_nextHistoryCommand >= 0)
                {
                    tb_input.Text = _commandHistory[_nextHistoryCommand];
                    _nextHistoryCommand--;
                }
                e.SuppressKeyPress = true;
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Down)
            {
                if (_nextHistoryCommand < _commandHistory.Count-1)
                {
                    _nextHistoryCommand++;
                    tb_input.Text = _commandHistory[_nextHistoryCommand];
                }
                e.SuppressKeyPress = true;
                e.Handled = true;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            _ = _server.Start(100);
            ActiveControl = tb_input;
            this.WindowState = FormWindowState.Maximized;
        }

        private void StateTimer_Tick(object sender, EventArgs e)
        {
            var txt = _server.DiagnosticsModule.GetSystemState();
            if (tb_state.Text != txt)
            {
                tb_state.Text = txt;
            }
        }


        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            StateTimer.Stop();
            try
            {
                _ = _server.ShutDown();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}
