using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwitchAgent
{
    public class Settings
    {
        private static Settings _instance;

        /// <summary>
        /// Gets the static instance of the settings object.
        /// </summary>
        public static Settings Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new Settings();
                }
                return _instance;
            }
        }

        private Settings() { }

        private bool _notifyOnline = true;
        /// <summary>
        /// Gets and sets the notify when online option.
        /// </summary>
        public bool NotifyOnline { get { return _notifyOnline; } set { _notifyOnline = value; } }

        private bool _notifyOffline = false;
        /// <summary>
        /// Gets and sets the notify when offline option.
        /// </summary>
        public bool NotifyOffline { get { return _notifyOffline; } set { _notifyOffline = value; } }

        private bool _notifyGameChange = true;
        /// <summary>
        /// Gets and sets the notify on game change option.
        /// </summary>
        public bool NotifyGameChange { get { return _notifyGameChange; } set { _notifyGameChange = value; } }
    }
}
