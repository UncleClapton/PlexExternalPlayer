using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
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
            Properties.Settings.Default.Save();
            Application.Exit();
        }

        private void exitToolStripMenuItem_VisibleChanged(object sender, EventArgs e)
        {
           
        }

        private void Form1_Load_1(object sender, EventArgs e)
        {
            ShowInTaskbar = false;
            displayCommandLineToolStripMenuItem.Checked = Properties.Settings.Default.ShowCommandLine;
            Hide();
        }

        private void showVersionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show($"Plex external player agent v{Assembly.GetExecutingAssembly().GetName().Version}.", "Version",MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void displayCommandLineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.ShowCommandLine)
            {
                displayCommandLineToolStripMenuItem.Checked = false;
                Properties.Settings.Default.ShowCommandLine = false;
            }
            else
            {
                displayCommandLineToolStripMenuItem.Checked = true;
                Properties.Settings.Default.ShowCommandLine = true;
            }
        }
    }
}
