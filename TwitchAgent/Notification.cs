using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;
using GlassHouse;

namespace TwitchAgent
{
    public partial class Notification : Form
    {
        // Static list used for working out the positioning of the notification windows.
        private static List<Notification> _notificationForms = new List<Notification>();

        // Adds a notification form to the static list.
        private static int AddForm(Notification form)
        {
            int indexPosition = 0;

            for (int i = 0; i <= _notificationForms.Count; i++)
            {
                if (i == _notificationForms.Count)
                {
                    indexPosition = i;
                    _notificationForms.Add(form);
                    break;
                }

                if (_notificationForms[i] == null)
                {
                    indexPosition = i;
                    _notificationForms[i] = form;
                    break;
                }
            }

            return indexPosition;
        }

        // Removes a notification form from the static list.
        private static void RemoveForm(Notification form)
        {
            if (_notificationForms.Contains(form))
            {
                _notificationForms[_notificationForms.IndexOf(form)] = null;
            }
        }

        private int _indexPosition = 0;
        private int _targetTopPosition = 0;
        private double _currentTopPosition = 0d;
        private bool _scrolling = false;
        private string _channelName;

        /// <summary>
        /// Create a new notification using a specified channel for the details.
        /// </summary>
        public Notification(Channel channel)
        {
            _indexPosition = AddForm(this);

            InitializeComponent();

            this.MouseUp += FormMouseUp;
            this.name.MouseUp += FormMouseUp;
            this.status.MouseClick += FormMouseUp;

            _channelName = channel.Name;
            if (channel.Icon != null)
            {
                channelIcon.BackgroundImage = channel.Icon.ToBitmap();
                channelIcon.Click += IconClick;
            }

            name.Text = channel.DisplayName;
            status.Text = "is playing " + channel.Game;

            ThreadManager.StartThread(TimerThread);
        }

        // Enables clicking on the icon to open the stream.
        private void IconClick(object sender, EventArgs e)
        {
            Process.Start("http://www.twitch.tv/" + _channelName);
        }

        // Enables clicking on the form to close the notification.
        private void FormMouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                ThreadManager.StartThread(ScrollOutThread);
            }
        }

        // Counts down until automatic closing of the notification.
        private void TimerThread()
        {
            try
            {
                Stopwatch timer = new Stopwatch();
                long targetTime = 15000;

                timer.Start();
                while (!this.IsDisposed && timer.ElapsedMilliseconds < targetTime && !ThreadManager.CloseRequested)
                {
                    Thread.Sleep(100);
                }

                this.Invoke((MethodInvoker)delegate
                {
                    ThreadManager.StartThread(ScrollOutThread);
                });
            }
            catch { }
        }

        // This thread scrolls the notification in from the bottom.
        private void ScrollInThread()
        {
            if (_scrolling)
            {
                return;
            }

            _scrolling = true;

            Stopwatch timer = new Stopwatch();
            timer.Start();

            double elapsedTime = 0d;

            while (this.Top != _targetTopPosition)
            {
                try
                {
                    elapsedTime = ((double)timer.ElapsedTicks / (double)Stopwatch.Frequency) * 50d;
                    timer.Restart();

                    if (_currentTopPosition > _targetTopPosition)
                    {
                        _currentTopPosition = Math.Max(_currentTopPosition - (Math.Sqrt(_currentTopPosition - _targetTopPosition) * elapsedTime), _targetTopPosition);
                    }
                    else
                    {
                        break;
                    }

                    Invoke((MethodInvoker)delegate
                    {
                        this.Top = (int)_currentTopPosition;
                    });
                }
                catch { break; }
            }

            _scrolling = false;
        }

        // This thread scrolls the notification out to the bottom.
        private void ScrollOutThread()
        {
            if (_scrolling)
            {
                return;
            }

            _scrolling = true;

            Stopwatch timer = new Stopwatch();
            timer.Start();

            double elapsedTime = 0d;

            _targetTopPosition = Screen.PrimaryScreen.Bounds.Bottom;

            while (this.Top != _targetTopPosition)
            {
                try
                {
                    elapsedTime = ((double)timer.ElapsedTicks / (double)Stopwatch.Frequency) * 50d;
                    timer.Restart();

                    if (_currentTopPosition < _targetTopPosition)
                    {
                        _currentTopPosition = Math.Min(_currentTopPosition + (Math.Sqrt(_targetTopPosition - _currentTopPosition) * elapsedTime), _targetTopPosition);
                    }
                    else
                    {
                        break;
                    }

                    Invoke((MethodInvoker)delegate
                    {
                        this.Top = (int)_currentTopPosition;
                    });
                }
                catch { break; }
            }

            Invoke((MethodInvoker)delegate
            {
                this.Close();
            });
        }

        // Loads everything up when the notification window is created.
        protected override void OnLoad(EventArgs e)
        {
            this.Left = Screen.PrimaryScreen.WorkingArea.Right - this.Width - 2;
            this.Top = Screen.PrimaryScreen.Bounds.Bottom;
            _currentTopPosition = this.Top;
            _targetTopPosition = Screen.PrimaryScreen.WorkingArea.Bottom - ((this.Height + 2) * (_indexPosition + 1));

            this.BackgroundImage = Resources.notification_background;

            ThreadManager.StartThread(ScrollInThread);

            base.OnLoad(e);
        }

        // When closing remove this notification form from the static list.
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            RemoveForm(this);
 	        base.OnClosing(e);
        }

        protected override bool ShowWithoutActivation { get { return true; } }

        protected override CreateParams CreateParams
        {
            get
            {
                int WS_EX_TOPMOST = 0x00000008;
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= WS_EX_TOPMOST;
                return cp;
            }
        }
    }
}
