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
        private static List<Notification> _notificationForms = new List<Notification>();

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

        private static void RemoveForm(Notification form)
        {
            if (_notificationForms.Contains(form))
            {
                _notificationForms[_notificationForms.IndexOf(form)] = null;
            }
        }

        private int _indexPosition = 0;
        private string _channelName;

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

        private void IconClick(object sender, EventArgs e)
        {
            Process.Start("http://www.twitch.tv/" + _channelName);
        }

        private void FormMouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                this.Close();
            }
        }

        private void TimerThread()
        {
            try
            {
                Stopwatch timer = new Stopwatch();
                long targetTime = 5000;

                timer.Start();
                while (!this.IsDisposed && timer.ElapsedMilliseconds < targetTime)
                {
                    Thread.Sleep(100);
                }
            
                this.Invoke((MethodInvoker)delegate
                {
                    this.Close();
                });
            }
            catch { }
        }

        protected override void OnLoad(EventArgs e)
        {
            this.Left = Screen.PrimaryScreen.WorkingArea.Right - this.Width - 2;
            this.Top = Screen.PrimaryScreen.WorkingArea.Bottom - ((this.Height + 2) * (_indexPosition + 1));

            this.BackgroundImage = Resources.notification_background;

            base.OnLoad(e);
        }

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
