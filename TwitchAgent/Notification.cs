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
        public Notification(Channel channel)
        {
            InitializeComponent();

            if (channel.Icon != null)
            {
                channelIcon.BackgroundImage = channel.Icon.ToBitmap();
            }

            name.Text = channel.DisplayName;
            if (channel.IsOnline)
            {
                status.Text = "is online!";
            }
            else
            {
                status.Text = "Is Offline!";
            }

            ThreadManager.StartThread(TimerThread);
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
            this.Left = Screen.PrimaryScreen.WorkingArea.Right - this.Width;
            this.Top = Screen.PrimaryScreen.WorkingArea.Bottom - this.Height;

            this.BackgroundImage = Resources.notification_background;

            base.OnLoad(e);
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
