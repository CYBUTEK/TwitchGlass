using System;
using System.Drawing;
using System.Windows.Forms;
using System.Threading;
using GlassHouse;

namespace TwitchAgent
{
    public class TrayIcon : Form
    {
        [STAThread]
        public static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new TrayIcon());
        }

        private NotifyIcon _trayIcon;
        private ContextMenu _trayMenu;

        /// <summary>
        /// Constructs the tray icon and intialises the agent.
        /// </summary>
        public TrayIcon()
        {
            _trayMenu = new ContextMenu();
            _trayMenu.MenuItems.Add("Notifications", new MenuItem[] {
                new MenuItem("Streamer Gone Online", NotifyOnlineClicked),
                new MenuItem("Streamer Gone Offline", NotifyOfflineClicked),
                new MenuItem("Streamer Changed Game", NotifyGameChangeClicked)
            });
            _trayMenu.MenuItems.Add("-");
            _trayMenu.MenuItems.Add("Exit", ExitClicked);

            _trayIcon = new NotifyIcon();
            _trayIcon.Text = "TwitchAgent";
            _trayIcon.Icon = Resources.icon;
            _trayIcon.ContextMenu = _trayMenu;
            _trayIcon.Visible = true;

            UpdateNotifications();

            ChannelManager.Instance.TrayIcon = this;

            ChannelManager.Instance.CreateChannel("ej_sa");
        }

        // Runs when the streamer gone online notification is clicked.
        private void NotifyOnlineClicked(object sender, EventArgs e)
        {
            Settings.Instance.NotifyOnline = !Settings.Instance.NotifyOnline;
            UpdateNotifications();
        }

        // Runs when the streamer gone offline notification is clicked.
        private void NotifyOfflineClicked(object sender, EventArgs e)
        {
            Settings.Instance.NotifyOffline = !Settings.Instance.NotifyOffline;
            UpdateNotifications();
        }

        // Runs when the streamer changed game notification is clicked.
        private void NotifyGameChangeClicked(object sender, EventArgs e)
        {
            Settings.Instance.NotifyGameChange = !Settings.Instance.NotifyGameChange;
            UpdateNotifications();
        }

        // Updates the checks against the notifications.
        private void UpdateNotifications()
        {
            _trayMenu.MenuItems[0].MenuItems[0].Checked = Settings.Instance.NotifyOnline;
            _trayMenu.MenuItems[0].MenuItems[1].Checked = Settings.Instance.NotifyOffline;
            _trayMenu.MenuItems[0].MenuItems[2].Checked = Settings.Instance.NotifyGameChange;
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
                _trayIcon.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
