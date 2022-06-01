using Controller;
using Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace View
{
    public partial class Form1 : Form
    {

        private ClientController gc;

        private DrawingPanel panel;

        public Form1()
        {
            gc = new ClientController();

            gc.RegisterHandler(GetName);
            gc.RegisterHandler(ShowError);
            gc.RegisterHandler(OnFrame);

            InitializeComponent();
            //Variable window dimension
            int screenHeight = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height;
            this.ClientSize = new System.Drawing.Size(Constants.DrawingPanelSize, Constants.DrawingPanelSize + Constants.OffsetHeight);

            //Variable help position
            menuStrip1.Location = new Point(this.ClientSize.Width - menuStrip1.Width - 5, menuStrip1.Location.Y + 5);

            // 
            // DrawingPanel
            // 
            panel = new DrawingPanel(gc.GetWorld());
            panel.BackColor = Color.Black;
            panel.Location = new Point(0, Constants.OffsetHeight);
            panel.Size = new Size(Constants.DrawingPanelSize, Constants.DrawingPanelSize);
            Controls.Add(panel);
            //register events to the drawing panel
            panel.MouseDown += Form1_MouseDown;
            panel.MouseUp += Form1_MouseUp;
            panel.MouseMove += Form1_MouseMove;

            this.Invalidate();
        }

        /// <summary>
        /// Displays the controls
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void controlsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("W:\t\tMove up\nA:\t\tMove left\nS:\t\tMove down\nD:\t\tMove right\nMouse:\t\tAim\nLeft click:\t\tFire projectile\nRight click:\tFire beam\nQ:\t\tQuit\n", "Controls", MessageBoxButtons.OK, MessageBoxIcon.None);
        }

        /// <summary>
        /// Displays the about menu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("TankWars Solution\nArtwork by Jolie Uk and Alex Smith\nGame design by Daniel Kopta\nImplementation by Aidan Lethaby and Sean Richens\nCS 3500 Fall 2019, University of Utah", "About", MessageBoxButtons.OK, MessageBoxIcon.None);
        }

        private void ConnectButton_Click(object sender, EventArgs e)
        {
            ConnectButton.Enabled = false;
            ServerTextBox.Enabled = false;
            NameTextBox.Enabled = false;
            gc.ConnectToServer(ServerTextBox.Text, NameTextBox.Text);

        }

        /// <summary>
        /// Displays an error
        /// </summary>
        /// <param name="message"></param>
        /// <param name="errorType"></param>
        private void ShowError(string message, string errorType)
        {
            MethodInvoker m = new MethodInvoker(() =>
            {
                ConnectButton.Enabled = true;
                ServerTextBox.Enabled = true;
                NameTextBox.Enabled = true;
            });
            this.Invoke(m);
            
            MessageBox.Show(message, errorType, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        /// <summary>
        /// Gets the name of the player
        /// </summary>
        /// <returns></returns>
        public string GetName()
        {
            return NameTextBox.Text;
        }

        /// <summary>
        /// Repaints the form on each frame
        /// </summary>
        private void OnFrame()
        {
            try
            {
                this.Invoke(new MethodInvoker(() => this.Invalidate(true)));
            }
            catch (ObjectDisposedException)
            {

            }
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            gc.KeyUp(e.KeyValue);
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            //If 'Q' is pressed, close the program
            if (e.KeyValue == 'Q')
            {
                gc.Close();
                this.Close();
            }
            gc.KeyDown(e.KeyValue);
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            gc.MouseDown((int)e.Button);
        }

        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            gc.MouseUp((int)e.Button);
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            gc.MouseMove(e.X, e.Y);
        }

        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {

        }
    }
}
