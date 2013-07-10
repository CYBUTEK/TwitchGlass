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
            this.channelIcon.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.channelIcon.Location = new System.Drawing.Point(6, 6);
            this.channelIcon.Name = "channelIcon";
            this.channelIcon.Size = new System.Drawing.Size(38, 38);
            this.channelIcon.TabIndex = 0;
            this.channelIcon.TabStop = false;
            // 
            // name
            // 
            this.name.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.name.Font = new System.Drawing.Font("Arial Black", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.name.Location = new System.Drawing.Point(50, 6);
            this.name.Name = "name";
            this.name.Size = new System.Drawing.Size(138, 18);
            this.name.TabIndex = 1;
            this.name.Text = "Channel Name";
            this.name.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            // 
            // status
            // 
            this.status.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.status.Font = new System.Drawing.Font("Arial", 6.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.status.ForeColor = System.Drawing.Color.Black;
            this.status.Location = new System.Drawing.Point(50, 28);
            this.status.Name = "status";
            this.status.Size = new System.Drawing.Size(138, 18);
            this.status.TabIndex = 2;
            this.status.Text = "Channel Status";
            this.status.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // Notification
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(200, 50);
            this.Controls.Add(this.name);
            this.Controls.Add(this.channelIcon);
            this.Controls.Add(this.status);
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