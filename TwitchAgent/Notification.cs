using System;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;
using GlassHouse;

namespace TwitchAgent
{
    public partial class Notification : Form
    {
        private static int _notificationNumber = 0;

        private static int NextNotificationNumber()
        {
            _notificationNumber++;
            return _notificationNumber;
        }

        private static void PrevNotificationNumber()
        {
            _notificationNumber--;
        }

        private string _channelName;

        public Notification(Channel channel)
        {
            NextNotificationNumber();

            InitializeComponent();

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

        private void TimerThread()
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

        protected override void OnLoad(EventArgs e)
        {
            this.Left = Screen.PrimaryScreen.WorkingArea.Right - this.Width - 2;
            this.Top = Screen.PrimaryScreen.WorkingArea.Bottom - ((this.Height + 2) * _notificationNumber);

            this.BackgroundImage = Resources.notification_background;

            base.OnLoad(e);
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
             PrevNotificationNumber();
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
