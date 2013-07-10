using System;
using System.Collections.Generic;
using System.Windows.Forms;
using GlassHouse;

namespace TwitchAgent
{
    public class ChannelManager : IDisposable
    {
        private static ChannelManager _instance;

        /// <summary>
        /// Gets the static instance of the channel manager object.
        /// </summary>
        public static ChannelManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ChannelManager();
                }
                return _instance;
            }
        }

        private ChannelManager() { }

        private TrayIcon _trayIcon;
        /// <summary>
        /// Gets and sets the main windows form to piggyback the notifications using its thread.
        /// </summary>
        public TrayIcon TrayIcon { get { return _trayIcon; } set { _trayIcon = value; } }

        private List<Channel> _channels = new List<Channel>();
        /// <summary>
        /// Gets and sets the list of channels currently being managed by the agent.
        /// </summary>
        public List<Channel> Channels { get { return _channels; } set { _channels = value; } }

        /// <summary>
        /// Creates a channel object and manages everything required for the notifications.
        /// </summary>
        public void CreateChannel(string name)
        {
            Channel channel = new Channel(Resources.icon);
            channel.OnlineStatusChanged += OnlineStatusChanged;
            channel.Initialise(name);

            _channels.Add(channel);
        }

        // Runs when the online status is changed for one of the channels.
        private void OnlineStatusChanged(Channel sender)
        {
            if (sender.IsOnline && Settings.Instance.NotifyOnline && _trayIcon != null)
            {
                if (_trayIcon.InvokeRequired)
                {
                    _trayIcon.Invoke((MethodInvoker)delegate
                    {
                        new Notification(sender).ShowDialog();
                    });
                }
            }
        }

        // Handles the garbage collection ready for disposal.
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // Handles the disposal of all child objects (eg. channel threads).
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                foreach (Channel channel in _channels)
                {
                    channel.Dispose();
                }
            }
        }
    }
}
