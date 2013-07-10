using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using System.IO;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TwitchGlass.Panels;
using GlassHouse;

namespace TwitchGlass
{
    public partial class MainForm : Form, IMessageFilter
    {
        private bool _showVideoControls = false;
        private bool _filteringMouse = false;
        private string _currentChannel = "";
        private Channel _channel;
        private Icon _defaultIcon;

        public MainForm()
        {
            InitializeComponent();
            _defaultIcon = Properties.Resources.icon;
            this.Icon = _defaultIcon;
            TitleProcessor();
            this.KeyDown += KeyDownHandler;
            this.channelPanel.ChannelSelected += ChannelSelected;
            this.flashPanel.Resize += FlashPanelResize;

            ThreadManager.StartThread(MouseCheckThread);

            ThreadManager.CountChanged += ThreadCountChanged;
        }

        private void ThreadCountChanged()
        {
            TitleProcessor();
        }

        /// <summary>
        /// Checks the mouse position and whether the mouse clicks need to be filtered.
        /// </summary>
        private void MouseCheckThread()
        {
            bool mouseOver = false;

            while (!this.IsDisposed)
            {
                try
                {
                    this.Invoke((MethodInvoker)delegate
                    {
                        if (flashPlayer.RectangleToScreen(flashPanel.Bounds).Contains(MousePosition))
                        {
                            mouseOver = true;
                        }
                        else
                        {
                            mouseOver = false;
                        }

                        if (!_showVideoControls)
                        {
                            if (mouseOver && !_filteringMouse)
                            {
                                Application.AddMessageFilter(this);
                                _filteringMouse = true;
                            }

                            if (_filteringMouse && !mouseOver)
                            {
                                Application.RemoveMessageFilter(this);
                                _filteringMouse = false;
                            }
                        }
                        else
                        {
                            if (_filteringMouse)
                            {
                                Application.RemoveMessageFilter(this);
                                _filteringMouse = false;
                            }
                        }
                    });

                    Thread.Sleep(100);
                }
                catch { break; }
            }
        }

        /// <summary>
        /// Handles the keyboard's key presses.
        /// </summary>
        private void KeyDownHandler(object sender, KeyEventArgs e)
        {
            if (!channelPanel.IsHoldingFocus)
            {
                if (e.KeyCode == Keys.D)
                {       
                    this.chatPanel.RunScrollProcess();
                }
                if (e.KeyCode == Keys.W)
                {
                    this.channelPanel.Channel = (_channel != null) ?  _channel.DisplayName : "";
                    this.channelPanel.RunScrollProcess();
                }
                if (e.KeyCode == Keys.S)
                {
                    _showVideoControls = !_showVideoControls;
                    this.FlashPanelResize(null, null);
                }
            }

            if (e.KeyCode == Keys.F1)
            {
                new OptionsForm().ShowDialog(this);
            }
            if (e.KeyCode == Keys.F11)
            {
                ToggleFullscreen();
            }
            if (e.KeyCode == Keys.F12)
            {
                this.TopMost = !this.TopMost;
                TitleProcessor();
            }
        }

        /// <summary>
        /// Sets whether the window should go to or from fullscreen.
        /// </summary>
        private void ToggleFullscreen()
        {
            if (this.WindowState != FormWindowState.Maximized)
            {
                this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
                this.WindowState = FormWindowState.Maximized;
            }
            else
            {
                this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
                this.WindowState = FormWindowState.Normal;
            }
        }

        /// <summary>
        /// Changes the channel to the selected channel given.
        /// </summary>
        private void ChannelSelected(string name)
        {
            _currentChannel = name;
            TitleProcessor();

            this.flashPanel.Controls.Remove(flashPlayer);
            flashPlayer.Dispose();
            flashPlayer = new AxShockwaveFlashObjects.AxShockwaveFlash();
            flashPlayer.BeginInit();
            flashPlayer.Name = "flashPlayer";
            flashPlayer.EndInit();
            this.flashPanel.Controls.Add(flashPlayer);

            flashPlayer.WMode = "Direct";
            flashPlayer.EmbedMovie = false;
            flashPlayer.AllowNetworking = "all";
            flashPlayer.AllowScriptAccess = "always";
            flashPlayer.FlashVars = "hostname=www.twitch.tv&channel=" + name + "&auto_play=true&start_volume=100";
            flashPlayer.LoadMovie(0, "http://www.twitch.tv/widgets/live_embed_player.swf");

            FlashPanelResize(null,null);

            this.chatPanel.DocumentText = "<html><head></head><body style=\"margin: 0px; padding 0px; width: 350px; \"><iframe frameborder=\"0\" scrolling=\"no\" id=\"chat_embed\" src=\"http://twitch.tv/chat/embed?channel=" + name + "&amp;popout_chat=true\" height=\"100%\" width=\"350\"></iframe></body></html>";

            // Create a channel object to do all of the Twitch API stuff.
            if (_channel != null)
            {
                _channel.Dispose();
            }
            _channel = new Channel(Properties.Resources.icon);
            _channel.DisplayNameChanged += DisplayNameChanged;
            _channel.IconChanged += IconChanged;
            _channel.OnlineStatusChanged += OnlineStatusChanged;
            _channel.GameChanged += OnlineStatusChanged;
            _channel.Initialise(name);     
   
        }

        /// <summary>
        /// Calls the title processor to change the title based on the new display name.
        /// </summary>
        private void DisplayNameChanged(Channel sender)
        {
            TitleProcessor();
        }

        /// <summary>
        /// Sets the window icon to the channel logo icon. (Thread Safe)
        /// </summary>
        private void IconChanged(Channel sender)
        {
            if (InvokeRequired)
            {
                Invoke((MethodInvoker)delegate
                {
                    this.Icon = sender.Icon;
                });
            }
            else
            {
                this.Icon = sender.Icon;
            }
        }

        /// <summary>
        /// Calls the title processor to change the title based on the new online status.
        /// </summary>
        private void OnlineStatusChanged(Channel sender)
        {
            TitleProcessor();
        }

        /// <summary>
        /// Sets the size of the flash player on window resize.
        /// </summary>
        private void FlashPanelResize(object sender, EventArgs e)
        {
            if (!_showVideoControls)
            {
                flashPlayer.Width = this.flashPanel.Width;
                flashPlayer.Height = this.flashPanel.Height + 31;
            }
            else
            {
                flashPlayer.Width = this.flashPanel.Width;
                flashPlayer.Height = this.flashPanel.Height;
            }

            flashPlayer.Update();
        }

        /// <summary>
        /// Processes and sets the title based on the applications current settings. (Thread Safe)
        /// </summary>
        private void TitleProcessor()
        {
            string title = "";

            if (_channel != null && _channel.DisplayName != "")
            {
                if (_channel.IsOnline)
                {
                    if (_channel.Game != "")
                    {
                        title = _channel.DisplayName + " is playing " + _channel.Game + " - ";
                    }
                    else
                    {
                        title = _channel.DisplayName + " is Online - ";
                    }
                }
                else
                {
                    title = _channel.DisplayName + " is Offline - ";
                }
            }

            title += "TwitchGlass v" + Versions.TwitchGlass + " (Threads: " + (ThreadManager.ThreadCount + 1).ToString() + ")";

            if (this.TopMost)
            {
                title += " - Always On Top";
            }

            // Update the window title in a thread safe way.
            try
            {
                if (InvokeRequired)
                {
                    Invoke((MethodInvoker)delegate
                    {
                        this.Text = title + " - Press F1 for Help!";
                    });
                }
                else
                {
                    this.Text = title + " - Press F1 for Help!";
                }
            }
            catch { }
        }

        protected override void OnClosed(EventArgs e)
        {
            if (_channel != null)
            {
                ThreadManager.CloseRequested = true;
                _channel.Dispose();
            }
            base.OnClosed(e);
        }

        /// <summary>
        /// Used to filter out the mouse clicks over the flash player.
        /// </summary>
        public bool PreFilterMessage(ref Message m)
        {
            if (m.Msg == 0x201 || m.Msg == 0x203) return true;
            return false;
        }
    }
}
