namespace TwitchAgent
{
    partial class Notification
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
            this.channelIcon = new System.Windows.Forms.PictureBox();
            this.name = new System.Windows.Forms.Label();
            this.status = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.channelIcon)).BeginInit();
            this.SuspendLayout();
            // 
            // channelIcon
            // 
            this.channelIcon.BackColor = System.Drawing.Color.White;
            this.channelIcon.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.channelIcon.Cursor = System.Windows.Forms.Cursors.Hand;
            this.channelIcon.Location = new System.Drawing.Point(5, 5);
            this.channelIcon.Name = "channelIcon";
            this.channelIcon.Size = new System.Drawing.Size(40, 40);
            this.channelIcon.TabIndex = 0;
            this.channelIcon.TabStop = false;
            // 
            // name
            // 
            this.name.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.name.BackColor = System.Drawing.Color.Transparent;
            this.name.Font = new System.Drawing.Font("Arial Black", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.name.ForeColor = System.Drawing.Color.White;
            this.name.Location = new System.Drawing.Point(51, 5);
            this.name.Name = "name";
            this.name.Size = new System.Drawing.Size(237, 24);
            this.name.TabIndex = 1;
            this.name.Text = "Channel Name";
            this.name.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // status
            // 
            this.status.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.status.AutoEllipsis = true;
            this.status.BackColor = System.Drawing.Color.Transparent;
            this.status.Font = new System.Drawing.Font("Arial Black", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.status.ForeColor = System.Drawing.Color.Gainsboro;
            this.status.Location = new System.Drawing.Point(51, 26);
            this.status.Name = "status";
            this.status.Size = new System.Drawing.Size(237, 15);
            this.status.TabIndex = 2;
            this.status.Text = "Channel Status";
            this.status.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.status.UseMnemonic = false;
            // 
            // Notification
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(65)))), ((int)(((byte)(165)))));
            this.ClientSize = new System.Drawing.Size(300, 50);
            this.Controls.Add(this.channelIcon);
            this.Controls.Add(this.status);
            this.Controls.Add(this.name);
            this.Font = new System.Drawing.Font("Arial Black", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.Color.Black;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Name = "Notification";
            this.ShowInTaskbar = false;
            this.Text = "Notification";
            ((System.ComponentModel.ISupportInitialize)(this.channelIcon)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox channelIcon;
        private System.Windows.Forms.Label name;
        private System.Windows.Forms.Label status;
    }
}