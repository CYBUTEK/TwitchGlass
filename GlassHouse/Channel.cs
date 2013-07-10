using System;
using System.Drawing;
using System.Threading;
using System.IO;
using System.Net;
using Newtonsoft.Json.Linq;

namespace GlassHouse
{
    public class Channel : IDisposable
    {
        public delegate void OnIconChanged(Channel sender);
        /// <summary>
        /// Event is fired when the channel's icon changes.
        /// </summary>
        public event OnIconChanged IconChanged;

        public delegate void OnDisplayNameChanged(Channel sender);
        /// <summary>
        /// Event is fired when the channel's display name changes.
        /// </summary>
        public event OnDisplayNameChanged DisplayNameChanged;

        public delegate void OnGameChanged(Channel sender);
        /// <summary>
        /// Event is fired when the channel's game changes.
        /// </summary>
        public event OnGameChanged GameChanged;

        public delegate void OnOnlineStatusChanged(Channel sender);
        /// <summary>
        /// Event is fires when the channel's online status changes.
        /// </summary>
        public event OnOnlineStatusChanged OnlineStatusChanged;

        private bool _isDisposed = false;
        /// <summary>
        /// Gets whether the object has been disposed.
        /// </summary>
        public bool IsDisposed { get { return _isDisposed; } private set { _isDisposed = value; } }

        private string _name = "";
        /// <summary>
        /// Gets the standard name of the channel.
        /// </summary>
        public string Name { get { return _name; } private set { _name = value; } }

        private Icon _icon;
        /// <summary>
        /// Gets and sets the channel icon.
        /// </summary>
        public Icon Icon
        {
            get { return _icon; }
            private set
            {
                _icon = value;
                if (IconChanged != null)
                {
                    IconChanged.Invoke(this);
                }
            }
        }

        private Icon _defaultIcon;
        /// <summary>
        /// Gets and sets the default channel icon.
        /// </summary>
        public Icon DefaultIcon { get { return _defaultIcon; } private set { _defaultIcon = value; } }

        private string _displayName = "";
        /// <summary>
        /// Gets the display name.
        /// </summary>
        public string DisplayName
        {
            get { return _displayName; }
            private set
            {
                _displayName = value;
                if (DisplayNameChanged != null)
                {
                    DisplayNameChanged.Invoke(this);
                }
            }
        }

        private string _game = "";
        /// <summary>
        /// Gets the game set as playing on the channel.
        /// </summary>
        public string Game
        {
            get { return _game; }
            private set
            {
                string oldGame = _game;
                _game = value;
                if (_game != oldGame && GameChanged != null)
                {
                    GameChanged.Invoke(this);
                }
            }
        }

        private bool _isOnline = false;
        /// <summary>
        /// Gets the online status of the channel.
        /// </summary>
        public bool IsOnline
        {
            get { return _isOnline; }
            private set
            {
                bool changed = false;

                if (value != _isOnline)
                {
                    changed = true;
                }

                _isOnline = value;
                if (changed && OnlineStatusChanged != null)
                {
                    OnlineStatusChanged.Invoke(this);
                }
            }
        }

        public Channel(Icon defaultIcon)
        {
            this.DefaultIcon = defaultIcon;
            this.Icon = _defaultIcon;
        }

        /// <summary>
        /// Initialises the channel with details from the twitch servers.
        /// </summary>
        public void Initialise(string name)
        {
            _name = name;
            _displayName = _name;

            ThreadManager.StartThread(ChannelSetupThread);
            ThreadManager.StartThread(IterativeUpdaterThread);
        }

        // Sets basic channel details.
        private void ChannelSetupThread()
        {
            try
            {
                WebRequest requestGetURL = WebRequest.Create("https://api.twitch.tv/kraken/channels/" + _name);
                Stream responseStream = requestGetURL.GetResponse().GetResponseStream();

                JObject jsonObject = JObject.Parse(new StreamReader(responseStream).ReadToEnd());

                // Sets the names as they are on twitch.
                this.Name = (string)jsonObject["name"];
                this.DisplayName = (string)jsonObject["display_name"];

                // Sets the icon.
                MemoryStream memStream;
                using (Stream response = WebRequest.Create((string)jsonObject["logo"]).GetResponse().GetResponseStream())
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
                    this.Icon = Icon.FromHandle(bitmap.GetHicon());
                }
            }
            catch
            {
                this.Icon = this.DefaultIcon;
            }
        }

        // Updates changable channel details.
        private void IterativeUpdaterThread()
        {
            while (!_isDisposed && this != null)
            {
                try
                {
                    WebRequest requestGetURL = WebRequest.Create("https://api.twitch.tv/kraken/streams/" + _name);
                    Stream responseStream = requestGetURL.GetResponse().GetResponseStream();

                    JObject jsonObject = JObject.Parse(new StreamReader(responseStream).ReadToEnd());
                    IsOnline = jsonObject["stream"].HasValues;

                    if (IsOnline)
                    {
                        this.Game = (string)jsonObject["stream"]["game"];
                    }

                    for (int i = 0; i < 30; i++)
                    {
                        if (!_isDisposed && this != null)
                        {
                            Thread.Sleep(1000);
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                catch { return; }
            }
        }

        /// <summary>
        /// Allows full disposal of the object and any contained objects.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            _isDisposed = true;
        }
    }
}
