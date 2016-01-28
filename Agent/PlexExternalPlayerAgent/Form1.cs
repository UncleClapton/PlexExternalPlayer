using System;
using System.Reflection;
using System.Windows.Forms;

namespace PlexExternalPlayerAgent
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            Visible = false;
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void exitToolStripMenuItem_VisibleChanged(object sender, EventArgs e)
        {
           
        }

        private void Form1_Load_1(object sender, EventArgs e)
        {
            ShowInTaskbar = false;
            displayCommandLineToolStripMenuItem.Checked = Settings.Current.ShowCommandLine;
            enableLoggingToolStripMenuItem.Checked = Settings.Current.EnableAdvancedLogging;
            Hide();
        }

        private void showVersionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show($"Plex external player agent v{Assembly.GetExecutingAssembly().GetName().Version}.", "Version",MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void displayCommandLineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Settings.Current.ShowCommandLine)
            {
                displayCommandLineToolStripMenuItem.Checked = false;
                Settings.Current.ShowCommandLine = false;
            }
            else
            {
                displayCommandLineToolStripMenuItem.Checked = true;
                Settings.Current.ShowCommandLine = true;
            }
        }

        private void enableLoggingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Settings.Current.EnableAdvancedLogging)
            {
                enableLoggingToolStripMenuItem.Checked = false;
                Settings.Current.EnableAdvancedLogging = false;
            }
            else
            {
                enableLoggingToolStripMenuItem.Checked = true;
                Settings.Current.EnableAdvancedLogging = true;
            }
        }
    }
}
