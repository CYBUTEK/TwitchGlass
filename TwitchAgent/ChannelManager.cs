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

        private TrayAgent _trayAgent;
        /// <summary>
        /// Gets and sets the main windows form to piggyback the notifications using its thread.
        /// </summary>
        public TrayAgent TrayAgent { get { return _trayAgent; } set { _trayAgent = value; } }

        private List<Channel> _channels = new List<Channel>();
        /// <summary>
        /// Gets and sets the list of channels currently being managed by the agent.
        /// </summary>
        public List<Channel> Channels { get { return _channels; } set { _channels = value; } }

        private bool _notificationsEnabled;
        /// <summary>
        /// Gets and sets whether the channel manager should handle notifications.
        /// </summary>
        public bool NotificationsEnabled { get { return _notificationsEnabled; } set { _notificationsEnabled = value; } }

        /// <summary>
        /// Creates a channel object and manages everything required for the notifications.
        /// </summary>
        public void CreateChannel(string name)
        {
            Channel channel = new Channel(Resources.icon);
            channel.Loaded += ChannelLoaded;
            channel.GameChanged += ChannelLoaded;
            channel.Initialise(name);

            _channels.Add(channel);
        }

        public void ReShowNotifications()
        {
            foreach (Channel channel in _channels)
            {
                if (channel.HasLoaded)
                {
                    ChannelLoaded(channel);
                }
            }
        }

        // Runs when the status of the channel changes.
        private void ChannelLoaded(Channel sender)
        {
            if (_notificationsEnabled && _trayAgent != null && sender.IsOnline)
            {
                if (_trayAgent.InvokeRequired)
                {
                    _trayAgent.Invoke((MethodInvoker)delegate
                    {
                        new Notification(sender).Show();
                    });
                }
                else
                {
                    new Notification(sender).Show();
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
