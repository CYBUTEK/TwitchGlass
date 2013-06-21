namespace TwitchGlass
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.flashPlayer = new AxShockwaveFlashObjects.AxShockwaveFlash();
            this.flashPanel = new System.Windows.Forms.Panel();
            this.chatPanel = new TwitchGlass.BrowserPanel();
            this.channelPanel = new TwitchGlass.ChannelPanel();
            ((System.ComponentModel.ISupportInitialize)(this.flashPlayer)).BeginInit();
            this.flashPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // flashPlayer
            // 
            this.flashPlayer.Enabled = true;
            this.flashPlayer.Location = new System.Drawing.Point(0, 0);
            this.flashPlayer.Name = "flashPlayer";
            this.flashPlayer.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("flashPlayer.OcxState")));
            this.flashPlayer.Size = new System.Drawing.Size(192, 192);
            this.flashPlayer.TabIndex = 6;
            // 
            // flashPanel
            // 
            this.flashPanel.BackColor = System.Drawing.Color.Transparent;
            this.flashPanel.Controls.Add(this.flashPlayer);
            this.flashPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flashPanel.Location = new System.Drawing.Point(0, 50);
            this.flashPanel.Name = "flashPanel";
            this.flashPanel.Size = new System.Drawing.Size(1280, 670);
            this.flashPanel.TabIndex = 4;
            // 
            // chatPanel
            // 
            this.chatPanel.BackColor = System.Drawing.Color.Black;
            this.chatPanel.Dock = System.Windows.Forms.DockStyle.Right;
            this.chatPanel.DocumentText = "<HTML></HTML>\0";
            this.chatPanel.Location = new System.Drawing.Point(930, 50);
            this.chatPanel.Name = "chatPanel";
            this.chatPanel.OpenSize = 350;
            this.chatPanel.Size = new System.Drawing.Size(0, 670);
            this.chatPanel.State = TwitchGlass.ScrollPanel.PanelState.Closed;
            this.chatPanel.TabIndex = 1;
            // 
            // channelPanel
            // 
            this.channelPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(65)))), ((int)(((byte)(165)))));
            this.channelPanel.Channel = "";
            this.channelPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.channelPanel.Location = new System.Drawing.Point(0, 0);
            this.channelPanel.Name = "channelPanel";
            this.channelPanel.OpenSize = 50;
            this.channelPanel.Size = new System.Drawing.Size(1280, 50);
            this.channelPanel.State = TwitchGlass.ScrollPanel.PanelState.Open;
            this.channelPanel.TabIndex = 3;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(1280, 720);
            this.Controls.Add(this.flashPanel);
            this.Controls.Add(this.chatPanel);
            this.Controls.Add(this.channelPanel);
            this.DoubleBuffered = true;
            this.KeyPreview = true;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "TwitchGlass";
            ((System.ComponentModel.ISupportInitialize)(this.flashPlayer)).EndInit();
            this.flashPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private BrowserPanel chatPanel;
        private AxShockwaveFlashObjects.AxShockwaveFlash flashPlayer;
        private ChannelPanel channelPanel;
        private System.Windows.Forms.Panel flashPanel;











    }
}

