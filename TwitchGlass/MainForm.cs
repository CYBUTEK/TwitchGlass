using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using System.IO;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TwitchGlass
{
    public partial class MainForm : Form, IMessageFilter
    {
        private bool _showVideoControls = false;
        private bool _filteringMouse = false;
        private string _currentChannel = "";
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

            MouseCheckThread();
        }

        /// <summary>
        /// Checks the mouse position and whether the mouse clicks need to be filtered.
        /// </summary>
        private void MouseCheckThread()
        {
            new Thread(() =>
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
                    }
                    catch { break; }
                }
            }).Start();
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
                    this.channelPanel.Channel = _currentChannel;
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
        private void ChannelSelected(string channel)
        {
            _currentChannel = channel;
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
            flashPlayer.FlashVars = "hostname=www.twitch.tv&channel=" + channel + "&auto_play=true&start_volume=100";
            flashPlayer.LoadMovie(0, "http://www.twitch.tv/widgets/live_embed_player.swf");

            FlashPanelResize(null,null);

            this.chatPanel.DocumentText = "<iframe frameborder=\"0\" scrolling=\"no\" id=\"chat_embed\" src=\"http://twitch.tv/chat/embed?channel=" + channel + "&amp;popout_chat=true\" height=\"100%\" width=\"100%\"></iframe>";

            SetCurrentChannelName();
            SetChannelIcon();          
        }

        /// <summary>
        /// Sets the current channel name with capitalised formatting.
        /// </summary>
        private void SetCurrentChannelName()
        {
            new Thread(() =>
            {
                try
                {
                    WebRequest requestGetURL = WebRequest.Create("https://api.twitch.tv/kraken/channels/" + _currentChannel);
                    Stream responseStream = requestGetURL.GetResponse().GetResponseStream();

                    JObject jsonObj = JObject.Parse(new StreamReader(responseStream).ReadToEnd());

                    this.Invoke((MethodInvoker)delegate
                    {
                        _currentChannel = (string)jsonObj["display_name"];
                        TitleProcessor();
                    });
                }
                catch { }
            }).Start();
        }

        /// <summary>
        /// Sets the window icon to the channel logo icon.
        /// </summary>
        private void SetChannelIcon()
        {
            new Thread(() =>
            {
                try
                {
                    WebRequest requestGetURL = WebRequest.Create("https://api.twitch.tv/kraken/channels/" + _currentChannel);
                    Stream responseStream = requestGetURL.GetResponse().GetResponseStream();

                    JObject jsonObj = JObject.Parse(new StreamReader(responseStream).ReadToEnd());

                    MemoryStream memStream;

                    using (Stream response = WebRequest.Create((string)jsonObj["logo"]).GetResponse().GetResponseStream())
                    {
                        memStream = new MemoryStream();
                        byte[] buffer = new byte[1024];
                        int byteCount;

                        do
                        {
                            byteCount = response.Read(buffer, 0, buffer.Length);
                            memStream.Write(buffer, 0, byteCount);
                        } while (byteCount > 0);
                    }

                    Bitmap bitmap = new Bitmap(Image.FromStream(memStream));

                    if (bitmap != null)
                    {
                        this.Invoke((MethodInvoker)delegate
                        {
                            this.Icon = Icon.FromHandle(bitmap.GetHicon());
                        });
                    }
                }
                catch
                {
                    this.Invoke((MethodInvoker)delegate
                    {
                        this.Icon = _defaultIcon;
                    });
                }
            }).Start();
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
        /// Processes and sets the title based on the applications current settings.
        /// </summary>
        private void TitleProcessor()
        {
            string title = "";

            if (_currentChannel != "")
            {
                title = "Watching: " + _currentChannel + " - TwitchGlass v" + Program.VERSION;
            }
            else
            {
                title = "TwitchGlass v" + Program.VERSION;
            }

            if (this.TopMost)
            {
                title += " - Always On Top";
            }

            this.Text = title + " - Press F1 for Help!";
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
