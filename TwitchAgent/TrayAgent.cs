using System;
using System.Drawing;
using System.Windows.Forms;
using System.Threading;
using GlassHouse;

namespace TwitchAgent
{
    public class TrayAgent : Form
    {
        [STAThread]
        public static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new TrayAgent());
        }

        private NotifyIcon _trayIcon;
        private ContextMenu _trayMenu;
        private Channel _myChannel;

        /// <summary>
        /// Constructs the tray icon and intialises the agent.
        /// </summary>
        public TrayAgent()
        {
            _trayMenu = new ContextMenu();
            _trayMenu.MenuItems.Add("Re-Show Notifications", ReShowNotifications);
            _trayMenu.MenuItems.Add("-");
            _trayMenu.MenuItems.Add("Exit", ExitClicked);

            _trayIcon = new NotifyIcon();
            _trayIcon.Text = "TwitchAgent";
            _trayIcon.Icon = Resources.icon;
            _trayIcon.ContextMenu = _trayMenu;
            _trayIcon.Visible = true;

            ChannelManager.Instance.TrayAgent = this;

            _myChannel = new Channel(Resources.icon);
            _myChannel.FollowingPopulated += FollowingPopulated;
            _myChannel.Initialise("cybutek");
            _myChannel.PopulateFollowing();
        }

        // Runs when the list of followers has been populated.
        private void FollowingPopulated(Channel sender)
        {
            ChannelManager.Instance.NotificationsEnabled = false;

            foreach (string channel in sender.Following)
            {
                ChannelManager.Instance.CreateChannel(channel);
            }

            ChannelManager.Instance.NotificationsEnabled = true;
        }

        private void ReShowNotifications(object sender, EventArgs e)
        {
            ChannelManager.Instance.ReShowNotifications();
        }

        // Runs when the exit item is clicked.
        public void ExitClicked(object sender, EventArgs e)
        {
            Application.Exit();
        }

        // Makes the form invisible when loaded.
        protected override void OnLoad(EventArgs e)
        {
            this.Visible = false;
            this.ShowInTaskbar = false;

            base.OnLoad(e);
        }

        // Handles the disposal of child objects.
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ThreadManager.CloseRequested = true;
                ChannelManager.Instance.Dispose();
                _myChannel.Dispose();
                _trayIcon.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
