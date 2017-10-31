using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Generic;

namespace ProgressIndicator
{
    /// <summary>
    /// Firefox like circular progress indicator.
    /// </summary>
    public partial class ProgressIndicator : UserControl
    {
        #region Constructor

        /// <summary>
        /// Default constructor for the ProgressIndicator.
        /// </summary>
        public ProgressIndicator()
        {
            InitializeComponent();

            if (this.Visible && AutoStart)
                timerAnimation.Enabled = true;
        }

        #endregion

        #region Private Fields

        private int _value = 1;

        private int _interval = 80;

        private Color _circleColor = Color.FromArgb(20, 20, 20);

        private bool _autoStart = true;

        private float _circleSize = 1.0F;
        
        private Bitmap bmDoubleBuffer;

        const float angle = 360.0F / 8;

        private float _radius;
        private float _sizeRate;
        private float _size;
        private Point _centr;
        private Dictionary<float, Point> _circlesPoints = new Dictionary<float, Point>();

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the base color for the circles.
        /// </summary>
        public Color CircleColor
        {
            get { return _circleColor; }
            set
            {
                _circleColor = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating if the animation should start automatically.
        /// </summary>
        [DefaultValue(true)]
        public bool AutoStart
        {
            get { return _autoStart; }
            set
            {
                _autoStart = value;

                if (_autoStart)
                    Start();
                else
                    Stop();
            }
        }

        /// <summary>
        /// Gets or sets the scale for the circles raging from 0.1 to 1.0.
        /// </summary>
        [DefaultValue(1.0F)]
        public float CircleSize
        {
            get { return _circleSize; }
            set
            {
                if (value <= 0.0F)
                    _circleSize = 0.1F;
                else
                    _circleSize = value > 1.0F ? 1.0F : value;

                Invalidate();
            }
        }

        /// <summary>
        /// Gets or sets the animation speed.
        /// </summary>
        [DefaultValue(80)]
        public int AnimationSpeed
        {
            get { return (-_interval + 400) / 4; }
            set
            {
                checked
                {
                    int interval = 400 - (value * 4);

                    if (interval < 10)
                        _interval = 10;
                    else
                        _interval = interval > 400 ? 400 : interval;

                    timerAnimation.Interval = _interval;
                }
            }
        }

        public new bool Visible
        {
            get { return base.Visible; }
            set 
            {
                base.Visible = value;
                if (value)
                    this.Start();
                else
                    this.Stop();
            }
        }

        private Bitmap DoubleBufferImage
        {
            get
            {
                if (bmDoubleBuffer == null)
                    bmDoubleBuffer = new Bitmap(this.ClientSize.Width, this.ClientSize.Height);
                return bmDoubleBuffer;
            }
            set
            {
                if (bmDoubleBuffer != null)
                    bmDoubleBuffer.Dispose();
                bmDoubleBuffer = value;
            }
        }
        #endregion

        #region Public Methods

        /// <summary>
        /// Starts the animation.
        /// </summary>
        public void Start()
        {
            timerAnimation.Interval = _interval;
            timerAnimation.Enabled = true;
        }

        /// <summary>
        /// Stops the animation.
        /// </summary>
        public void Stop()
        {
            timerAnimation.Enabled = false;
            _value = 1;
            Invalidate();
        }

        /// <summary>
        /// Выровняем индикатор по центру
        /// </summary>
        public void AlignByCentr()
        {
            try
            {
                if (this.Parent != null)
                {
                    int halfReportHeight = this.Parent.Height / 2;
                    int halfReportWidth = this.Parent.Width / 2;
                    int haltIndicatorHeigt = this.Height / 2;

                    this.Location = new Point(halfReportWidth - haltIndicatorHeigt,
                        halfReportHeight - haltIndicatorHeigt);
                }
            }
            catch 
            { 
            }
        }

        #endregion

        #region Overrides

        protected override void OnPaint(PaintEventArgs e)
        {
            using (Graphics g = Graphics.FromImage(this.DoubleBufferImage))
            {
                g.Clear(this.BackColor);

                for (int i = 1; i <= 8; i++)
                {
                    Color drawColor = Color.FromArgb(_circleColor.R / (9 - i), _circleColor.G / (9 - i), _circleColor.B / (9 - i));

                    using (SolidBrush brush = new SolidBrush(drawColor))
                    {
                        Point p = this.PointOnCircle(_centr, _radius, angle * i + (angle * (_value - 1)));
                        g.FillEllipse(brush, p.X, p.Y, (int)_size, (int)_size);
                    }
                }
            }
            e.Graphics.DrawImage(bmDoubleBuffer, 0, 0);
            base.OnPaint(e);
        }

        protected override void  OnPaintBackground(PaintEventArgs e)
        {
        }

        protected override void OnResize(EventArgs e)
        {
            SetNewSize();
            base.OnResize(e);
        }

        private void ProgressIndicator_Resize(object sender, EventArgs e)
        {
            SetNewSize();
        }

        #endregion

        #region Private Methods

        private void SetNewSize()
        {
            int size_ = Math.Max(Width, Height);
            Size = new Size(size_, size_);

            _radius = this.Height / 2.5f;
            _sizeRate = 4.5F / _circleSize;
            _size = Width / _sizeRate;
            _centr = new Point((int)_radius, (int)_radius);
            _circlesPoints.Clear();
        }

        private void IncreaseValue()
        {
            if (_value + 1 <= 8)
                _value++;
            else
                _value = 1;
        }

        private Point PointOnCircle(Point center, float radius, float angle)
        {
            Point result = Point.Empty;
            //если для данного угла уже вычисляли точку, сразу вернем ее
            if (_circlesPoints.ContainsKey(angle))
            {
                _circlesPoints.TryGetValue(angle, out result);
                return result;
            }

            float angleInRadians = angle * (float)Math.PI / 180;
            Point top = new Point(center.X, (int)(center.Y + radius));
            result = new Point(
                (int)(center.X + Math.Cos(angleInRadians) * (center.X - top.X)
                - Math.Sin(angleInRadians) * (center.Y - top.Y)),
                (int)(center.Y + Math.Sin(angleInRadians) * (center.X - top.X)
                + Math.Cos(angleInRadians) * (center.Y - top.Y)));
            _circlesPoints.Add(angle, result);
            return result;
        }

        #endregion

        #region Timer

        private void timerAnimation_Tick(object sender, EventArgs e)
        {
            IncreaseValue();
            Invalidate();
        }

        #endregion
    }
}
