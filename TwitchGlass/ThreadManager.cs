using System;
using System.Threading;

namespace TwitchGlass
{
    public sealed class ThreadManager
    {
        public delegate void OnCountChanged();
        /// <summary>
        /// Event is fired whenever the thread count changes.
        /// </summary>
        public static event OnCountChanged CountChanged;

        private static int _threadCount = 0;
        /// <summary>
        /// Gets the number of threads currently being handled by the thread manager.
        /// </summary>
        public static int ThreadCount
        {
            get { return _threadCount; }
            private set
            {
                _threadCount = value;
                if (CountChanged != null)
                {
                    CountChanged.Invoke();
                }
            }
        }

        private static bool _closeRequested = false;
        /// <summary>
        /// Gets or sets the close requested flag, which should be used by threads to know when to stop.
        /// </summary>
        public static bool CloseRequested { get; set; }

        /// <summary>
        /// Start a thread wrapped in the thread management system.
        /// </summary>
        public static void StartThread(Action method)
        {
            new Thread(new ThreadStart(() => ThreadWrapper(method))).Start();
        }

        // Wraps the action delegate up in a method that handles the thread count.
        private static void ThreadWrapper(Object methodObject)
        {
            Action method = (Action)methodObject;

            ThreadCount++;
            method.Invoke();
            ThreadCount--;
        }
    }
}
