using System;
using System.Windows.Input;
using System.Windows.Forms;

namespace TwitchGlass
{
    public class ChannelPanel : ScrollPanel
    {
        private PictureBox pictureBox1;
        private PictureBox pictureBox2;
        private PictureBox pictureBox3;
        private PictureBox pictureBox4;
        private TextBox textChannel;

        public delegate void OnChannelSelected(string channel);
        public event OnChannelSelected ChannelSelected;

        public override bool IsHoldingFocus
        {
            get
            {
                return this.textChannel.Focused;
            }
        }

        public string Channel
        {
            get { return this.textChannel.Text; }
            set { this.textChannel.Text = value; }
        }
    
        public ChannelPanel()
            : base()
        {
            InitializeComponent();
            this.textChannel.KeyDown += TextBoxKeyDownHandler;
            this.Opening += IsOpening;
        }

        private void InitializeComponent()
        {
            this.textChannel = new System.Windows.Forms.TextBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.pictureBox3 = new System.Windows.Forms.PictureBox();
            this.pictureBox4 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).BeginInit();
            this.SuspendLayout();
            // 
            // textChannel
            // 
            this.textChannel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textChannel.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textChannel.Font = new System.Drawing.Font("Arial Black", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textChannel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(65)))), ((int)(((byte)(165)))));
            this.textChannel.Location = new System.Drawing.Point(245, 10);
            this.textChannel.Name = "textChannel";
            this.textChannel.Size = new System.Drawing.Size(235, 30);
            this.textChannel.TabIndex = 1;
            this.textChannel.WordWrap = false;
            // 
            // pictureBox2
            // 
            this.pictureBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.pictureBox2.Image = global::TwitchGlass.Properties.Resources.selected_channel_left;
            this.pictureBox2.Location = new System.Drawing.Point(230, 0);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(15, 50);
            this.pictureBox2.TabIndex = 3;
            this.pictureBox2.TabStop = false;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.pictureBox1.Image = global::TwitchGlass.Properties.Resources.selected_channel;
            this.pictureBox1.Location = new System.Drawing.Point(0, 0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(225, 50);
            this.pictureBox1.TabIndex = 2;
            this.pictureBox1.TabStop = false;
            // 
            // pictureBox3
            // 
            this.pictureBox3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox3.Image = global::TwitchGlass.Properties.Resources.selected_channel_right;
            this.pictureBox3.Location = new System.Drawing.Point(480, 0);
            this.pictureBox3.Name = "pictureBox3";
            this.pictureBox3.Size = new System.Drawing.Size(15, 50);
            this.pictureBox3.TabIndex = 4;
            this.pictureBox3.TabStop = false;
            // 
            // pictureBox4
            // 
            this.pictureBox4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox4.BackgroundImage = global::TwitchGlass.Properties.Resources.selected_channel_middle;
            this.pictureBox4.Location = new System.Drawing.Point(245, 0);
            this.pictureBox4.Name = "pictureBox4";
            this.pictureBox4.Size = new System.Drawing.Size(235, 50);
            this.pictureBox4.TabIndex = 5;
            this.pictureBox4.TabStop = false;
            // 
            // ChannelPanel
            // 
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(65)))), ((int)(((byte)(165)))));
            this.Controls.Add(this.pictureBox3);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.textChannel);
            this.Controls.Add(this.pictureBox4);
            this.Name = "ChannelPanel";
            this.Size = new System.Drawing.Size(500, 50);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        /// <summary>
        /// Handles keys while the channel selection text box has focus.
        /// </summary>
        private void TextBoxKeyDownHandler(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.RunScrollProcess();
            }

            if (e.KeyCode == Keys.Enter && ChannelSelected != null)
            {
                ChannelSelected(textChannel.Text);
                this.RunScrollProcess();
            }
        }

        private void IsOpening()
        {
            this.textChannel.Focus();
            this.textChannel.SelectAll();
        }
    }
}
