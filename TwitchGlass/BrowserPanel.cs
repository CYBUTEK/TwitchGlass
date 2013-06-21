using System;
using System.Windows.Input;
using System.Windows.Forms;

namespace TwitchGlass
{
    public class BrowserPanel : ScrollPanel
    {
        private WebBrowser webBrowser;

        /// <summary>
        /// Gets and sets the document text of the web browser.
        /// </summary>
        public string DocumentText
        {
            get { return webBrowser.DocumentText; }
            set { webBrowser.DocumentText = value; }
        }

        public BrowserPanel()
            : base()
        {
            InitializeComponent();
            this.Load += LoadPanel;
            this.webBrowser.PreviewKeyDown += KeyDownHandler;
            this.webBrowser.Navigating += Navigating;
            this.webBrowser.DocumentCompleted += DocumentCompleted;
        }

        private void InitializeComponent()
        {
            this.webBrowser = new System.Windows.Forms.WebBrowser();
            this.SuspendLayout();
            // 
            // webBrowser
            // 
            this.webBrowser.AllowWebBrowserDrop = false;
            this.webBrowser.Location = new System.Drawing.Point(0, 0);
            this.webBrowser.Name = "webBrowser";
            this.webBrowser.ScriptErrorsSuppressed = true;
            this.webBrowser.ScrollBarsEnabled = false;
            this.webBrowser.Size = new System.Drawing.Size(140, 79);
            this.webBrowser.TabIndex = 0;
            this.webBrowser.TabStop = false;
            this.webBrowser.Url = new System.Uri("", System.UriKind.Relative);
            this.webBrowser.WebBrowserShortcutsEnabled = false;
            // 
            // BrowserPanel
            // 
            this.Controls.Add(this.webBrowser);
            this.Name = "BrowserPanel";
            this.ResumeLayout(false);

        }

        /// <summary>
        /// Initialises the browser in the panel.
        /// </summary>
        private void LoadPanel(object sender, EventArgs e)
        {
            switch (this.Dock)
            {
                case DockStyle.Left:
                    this.webBrowser.Dock = DockStyle.Right;
                    this.webBrowser.Width = this.OpenSize;
                    break;

                case DockStyle.Right:
                    this.webBrowser.Dock = DockStyle.Left;
                    this.webBrowser.Width = this.OpenSize;
                    break;
            }
        }

        /// <summary>
        /// Handles keys pressed while the browser has focus.
        /// </summary>
        private void KeyDownHandler(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.webBrowser.Focus();
            }
        }

        /// <summary>
        /// Runs before the browser navigates.  This is to cure a bug with setting focus.
        /// </summary>
        private void Navigating(object sender, EventArgs e)
        {
            this.Enabled = false;
        }

        /// <summary>
        /// Runs after the browser has loaded the document.  This is to cure a bug with setting focus.
        /// </summary>
        private void DocumentCompleted(object sender, EventArgs e)
        {
            this.Enabled = true;
        }
    }
}
