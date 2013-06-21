using System;
using System.Threading;
using System.ComponentModel;
using System.Windows.Forms;
using System.Diagnostics;

namespace TwitchGlass
{
    public partial class ScrollPanel : UserControl
    {
        public enum PanelState
        {
            Open,
            Closed
        }

        private PanelState _state = PanelState.Closed;
        private bool _scrolling = false;
        private int _openSize = 0;
        private double _size = 0d;

        public delegate void OnOpening();
        public event OnOpening Opening;

        /// <summary>
        /// Gets whether the current control is holding focus.
        /// </summary>
        public virtual bool IsHoldingFocus { get { return false; } }

        [Browsable(true)]
        public PanelState State { get { return _state; } set { _state = value; } }

        [Browsable(true)]
        public int OpenSize { get { return _openSize; } set { _openSize = value; } }

        public ScrollPanel()
        {
            InitializeComponent();
            this.Load += LoadPanel;
        }

        /// <summary>
        /// Scroll the panel to opposite state.
        /// </summary>
        public void RunScrollProcess()
        {
            if (!_scrolling)
            {
                _scrolling = true;
                DockStyle dock = this.Dock;

                switch (_state)
                {
                    case PanelState.Open:
                        _state = PanelState.Closed;
                        break;

                    case PanelState.Closed:
                        _state = PanelState.Open;
                        this.Visible = true;
                        if (Opening != null)
                        {
                            Opening();
                        }
                        break;
                }

                new Thread(() =>
                {
                    Stopwatch timer = new Stopwatch();
                    timer.Start();

                    double elapsedTime = 0d;

                    switch (_state)
                    {
                        case PanelState.Open:
                            while (_scrolling)
                            {
                                try
                                {
                                    elapsedTime = ((double)timer.ElapsedTicks / (double)Stopwatch.Frequency) * 100d;
                                    timer.Restart();
                                    if (dock == DockStyle.Left || dock == DockStyle.Right)
                                    {
                                        Invoke((MethodInvoker)delegate
                                        {
                                            if (this.Width >= _openSize)
                                            {
                                                _scrolling = false;
                                                _size = _openSize;
                                            }
                                            else
                                            {
                                                _size = Math.Min(_size + Math.Sqrt(_size + 1) * elapsedTime, _openSize);
                                            }

                                            this.Width = (int)_size;
                                            this.Update();
                                        });
                                    }
                                    else if (dock == DockStyle.Top || dock == DockStyle.Bottom)
                                    {
                                        Invoke((MethodInvoker)delegate
                                        {
                                            if (this.Height >= _openSize)
                                            {
                                                _scrolling = false;
                                                _size = _openSize;
                                            }
                                            else
                                            {
                                                _size = Math.Min(_size + Math.Sqrt(_size + 1) * elapsedTime, _openSize);
                                            }

                                            this.Height = (int)_size;
                                            this.Update();
                                        });
                                    }
                                }
                                catch { break; }
                            }
                            Thread.Sleep(100);
                            break;

                        case PanelState.Closed:
                            while (_scrolling)
                            {
                                try
                                {
                                    elapsedTime = ((double)timer.ElapsedTicks / (double)Stopwatch.Frequency) * 100d;
                                    timer.Restart();
                                    if (dock == DockStyle.Left || dock == DockStyle.Right)
                                    {
                                        Invoke((MethodInvoker)delegate
                                        {
                                            if (this.Width <= 0)
                                            {
                                                _scrolling = false;
                                                _size = 0d;
                                                this.Visible = false;
                                            }
                                            else
                                            {
                                                _size -= Math.Sqrt(_size + 1) * elapsedTime;
                                            }

                                            this.Width = (int)_size;
                                            this.Update();
                                        });
                                    }
                                    else if (dock == DockStyle.Top || dock == DockStyle.Bottom)
                                    {
                                        Invoke((MethodInvoker)delegate
                                        {
                                            if (this.Height <= 0)
                                            {
                                                _scrolling = false;
                                                _size = 0d;
                                                this.Visible = false;
                                            }
                                            else
                                            {
                                                _size -= Math.Sqrt(_size + 1) * elapsedTime;
                                            }

                                            this.Height = (int)_size;
                                            this.Update();
                                        });
                                    }
                                }
                                catch { break; }
                            }
                            Thread.Sleep(100);
                            break;
                    }
                }).Start();
            }
        }

        /// <summary>
        /// Initialises the panel and scroll context.
        /// </summary>
        private void LoadPanel(object sender, EventArgs e)
        {
            switch (_state)
            {
                case PanelState.Closed:
                    this.Visible = false;
                    if (this.Dock == DockStyle.Left || this.Dock == DockStyle.Right)
                    {
                        this.Width = 0;
                        _size = this.Width;
                    }
                    else if (this.Dock == DockStyle.Top || this.Dock == DockStyle.Bottom)
                    {
                        this.Height = 0;
                        _size = this.Height;
                    }
                    break;

                case PanelState.Open:
                    this.Visible = true;
                    if (this.Dock == DockStyle.Left || this.Dock == DockStyle.Right)
                    {
                        this.Width = _openSize;
                        _size = this.Width;
                    }
                    else if (this.Dock == DockStyle.Top || this.Dock == DockStyle.Bottom)
                    {
                        this.Height = _openSize;
                        _size = this.Height;
                    }
                    break;
            }
        }
    }
}
