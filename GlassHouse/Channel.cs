using System;
using System.Collections.Generic;
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
        /// Event is fired when the channel's online status changes.
        /// </summary>
        public event OnOnlineStatusChanged OnlineStatusChanged;

        public delegate void OnFollowingPopulated(Channel sender);
        /// <summary>
        /// Event is fired when the channel's following list has been populated.
        /// </summary>
        public event OnFollowingPopulated FollowingPopulated;

        public delegate void OnHasLoaded(Channel sender);
        /// <summary>
        /// Event is fired after the channel has gathered and loaded all data.
        /// </summary>
        public event OnHasLoaded Loaded;

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

                if (!_hasLoadedIcon)
                {
                    _hasLoadedIcon = true;
                }

                if (IconChanged != null)
                {
                    if (_hasLoaded)
                    {
                        IconChanged(this);
                    }
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

                if (!_hasLoadedName)
                {
                    _hasLoadedName = true;
                }

                if (DisplayNameChanged != null)
                {
                    if (_hasLoaded)
                    {
                        DisplayNameChanged(this);
                    }
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
                bool changed = false;

                if (value != _game)
                {
                    changed = true;
                }

                _game = value;

                if (!_hasLoadedGame)
                {
                    _hasLoadedGame = true;
                }

                if (changed && GameChanged != null)
                {
                    if (_hasLoaded)
                    {
                        GameChanged(this);
                    }
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

                if (!_hasLoadedOnline)
                {
                    _hasLoadedOnline = true;
                }

                if (changed && _hasLoaded)
                {
                    if (_hasLoadedOnline && _isOnline)
                    {
                        _hasLoadedGame = false;
                        HasLoaded = false;
                    }

                    if (OnlineStatusChanged != null)
                    {
                        OnlineStatusChanged(this);
                    }
                }
            }
        }

        private bool _hasLoaded = false;
        /// <summary>
        /// Gets whether the channel has loaded all data.
        /// </summary>
        public bool HasLoaded
        {
            get { return _hasLoaded; }
            private set
            {
                _hasLoaded = value;

                if (_hasLoaded)
                {
                    if (Loaded != null)
                    {
                        Loaded(this);
                    }
                }
                else
                {
                    ThreadManager.StartThread(LoadCheckerThread);
                }
            }
        }
        private bool _hasLoadedName = false;
        private bool _hasLoadedIcon = false;
        private bool _hasLoadedGame = false;
        private bool _hasLoadedOnline = false;

        private List<string> _following;
        /// <summary>
        /// Gets the names of the channels this channel is following.
        /// </summary>
        public List<string> Following { get { return _following; } private set { _following = value; } }

        public Channel() { }

        public Channel(Icon defaultIcon)
        {
            this.DefaultIcon = defaultIcon;
        }

        /// <summary>
        /// Initialises the channel with details from the twitch servers.
        /// </summary>
        public void Initialise(string name)
        {
            _name = name;
            _displayName = _name;

            ThreadManager.StartThread(LoadCheckerThread);
            ThreadManager.StartThread(ChannelSetupThread);
            ThreadManager.StartThread(IterativeUpdaterThread);
        }

        public void PopulateFollowing()
        {
            ThreadManager.StartThread(ChannelFollowingThread);
        }

        // Sets basic channel details.
        private void ChannelSetupThread()
        {
            try
            {
                WebRequest requestGetURL = WebRequest.Create("https://api.twitch.tv/kraken/channels/" + _name);
                requestGetURL.Headers.Add("Client-ID: GlassHouse v" + Versions.GlassHouse);
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

        // Populates the following list with channel names.
        private void ChannelFollowingThread()
        {
            _following = new List<string>();
            ChannelFollowingThreadRecursive("https://api.twitch.tv/kraken/users/" + _name + "/follows/channels");

            if (FollowingPopulated != null)
            {
                FollowingPopulated(this);
            }
        }

        // Populates the following list with channel names.  Recursively getting the pages from twitch.
        private void ChannelFollowingThreadRecursive(string url)
        {
            try
            {
                WebRequest requestGetURL = WebRequest.Create(url);
                requestGetURL.Headers.Add("Client-ID: GlassHouse v" + Versions.GlassHouse);
                Stream responseStream = requestGetURL.GetResponse().GetResponseStream();

                JObject jsonObject = JObject.Parse(new StreamReader(responseStream).ReadToEnd());

                if (jsonObject["follows"].HasValues)
                {
                    foreach (var channel in jsonObject["follows"].Children()["channel"])
                    {
                        _following.Add((string)channel["name"]);
                    }
                    
                    ChannelFollowingThreadRecursive((string)jsonObject["_links"]["next"]);
                }
            }
            catch { }
        }

        // Updates changable channel details.
        private void IterativeUpdaterThread()
        {
            while (!_isDisposed && this != null && !ThreadManager.CloseRequested)
            {
                try
                {
                    WebRequest requestGetURL = WebRequest.Create("https://api.twitch.tv/kraken/streams/" + _name);
                    requestGetURL.Headers.Add("Client-ID: GlassHouse v" + Versions.GlassHouse);
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
                catch { break; }
            }
        }

        // Runs until the channel has loaded all required data.
        private void LoadCheckerThread()
        {
            bool loaded = false;
            while (!_isDisposed && this != null && !ThreadManager.CloseRequested)
            {
                try
                {
                    if (_hasLoadedName && _hasLoadedIcon && _hasLoadedOnline)
                    {
                        if (_isOnline)
                        {
                            if (_hasLoadedGame)
                            {
                                loaded = true;
                            }
                        }
                        else
                        {
                            loaded = true;
                        }
                    }

                    if (loaded)
                    {
                        HasLoaded = true;
                        break;
                    }

                    Thread.Sleep(1000);
                }
                catch { break; }
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
