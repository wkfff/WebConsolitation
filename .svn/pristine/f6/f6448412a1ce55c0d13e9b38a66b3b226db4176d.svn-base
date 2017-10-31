using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Xml;
using Krista.FM.Client.MDXExpert.Common;
using Krista.FM.Common.Xml;
using Krista.FM.Client.MDXExpert.Data;

namespace Krista.FM.Client.MDXExpert.Controls
{
    /// <summary>
    /// Легенда
    /// </summary>
    public partial class ExpertLegend : UserControl
    {
        private int _itemSize = 15;
        private int _itemSpacing = 5;
        private StringFormat _stringFormat;
        private LegendItemCollection _items;
        private LegendLocation _location;
        private int _legendSize;
        private Color _borderColor;
        private Color _color;
        private bool _rangeLimitsVisible;
        private string _formatString;
        //private UnitDisplayType _unitDisplayType;

        private Bitmap _backBuffer;

        public int ItemSize
        {
            get { return this._itemSize; }
            set
            {
                this._itemSize = value;
                this.DrawLegend();
            }
        }

        public LegendItemCollection Items
        {
            get { return _items; }
            set { _items = value; }
        }

        public int ItemSpacing
        {
            get { return _itemSpacing; }
            set
            {
                _itemSpacing = value;
                this.DrawLegend();
            }
        }

        public StringFormat StringFormat
        {
            get { return _stringFormat; }
            set { _stringFormat = value; }
        }

        public LegendLocation Location
        {
            get { return this._location; }
            set
            {
                this._location = value;
                SetLocation(value);
            }
        }

        public int LegendSize
        {
            get { return this._legendSize; }
            set
            {
                this._legendSize = value;
                SetLegendSize(value);
            }
        }

        public Color BorderColor
        {
            get { return this._borderColor; }
            set
            {
                this._borderColor = value;
                DrawLegend();
            }
        }

        public Color Color
        {
            get { return this._color; }
            set 
            {
                this._color = value;
                DrawLegend();
            }
        }

        /// <summary>
        /// Отображать значения интервалов
        /// </summary>
        public bool RangeLimitsVisible
        {
            get { return this._rangeLimitsVisible; }
            set
            {
                this._rangeLimitsVisible = value;
                DrawLegend();
            }
        }

        /// <summary>
        /// строка формата значений легенды
        /// </summary>
        public string FormatString
        {
            get { return this._formatString; }
            set
            {
                this._formatString = value;
                DrawLegend();
            }
        }


        public ExpertLegend()
        {
            InitializeComponent();
            
            this.BackColor = SystemColors.ControlLightLight;
            this._color = SystemColors.ControlLightLight;
            this._borderColor = Color.Gray;

            this._stringFormat = new StringFormat();
            this._stringFormat.Trimming = StringTrimming.EllipsisCharacter;
            this._stringFormat.FormatFlags = StringFormatFlags.LineLimit;

            //this._unitDisplayType = Data.UnitDisplayType.None;
            this._formatString = "#,##0.00";

            this._items = new LegendItemCollection(this);

            this._rangeLimitsVisible = false;
           
        }

        private void SetLocation(LegendLocation location)
        {
            switch(location)
            {
                case LegendLocation.Left:
                    this.Dock = DockStyle.Left;
                    this.Width = this.LegendSize;
                    break;
                case LegendLocation.Top:
                    this.Dock = DockStyle.Top;
                    this.Height = this.LegendSize;
                    break;
                case LegendLocation.Right:
                    this.Dock = DockStyle.Right;
                    this.Width = this.LegendSize;
                    break;
                case LegendLocation.Bottom:
                    this.Dock = DockStyle.Bottom;
                    this.Height = this.LegendSize;
                    break;
            }
        }

        private void SetLegendSize(int value)
        {
            switch (this.Location)
            {
                case LegendLocation.Left:
                case LegendLocation.Right:
                    this.Width = value;
                    break;
                case LegendLocation.Top:
                case LegendLocation.Bottom:
                    this.Height = value;
                    break;
            }
        }

        public void DrawLegend()
        {
            this.Invalidate();
        }

        /// <summary>
        /// Рисуем метку "еще…", если не все элементы влезли в легенду
        /// </summary>
        /// <param name="g"></param>
        private void DrawMoreLabel(Graphics g)
        {
            if (!this.Items.IsAllItemsDrawed)
            {
                Font f = new Font("Serif", 7);
                Rectangle bounds = new Rectangle(this.Width - 37, this.Height - 17, 32, 12);
                g.DrawString("еще…", f, Brushes.Black, bounds, this.StringFormat);
            }
        }

        private void ExpertLegend_Paint(object sender, PaintEventArgs e)
        {
            if (_backBuffer == null)
            {
                _backBuffer = new Bitmap(this.ClientSize.Width, this.ClientSize.Height);
            }

            Graphics g = null;
            g = Graphics.FromImage(_backBuffer);
            g.Clear(this.BackColor);

            //рисование
            g.FillRectangle(new SolidBrush(this.Color), 5, 5, this.Width - 10, this.Height - 10);
            g.DrawRectangle(new Pen(this.BorderColor), 5, 5, this.Width - 10, this.Height - 10);

            this.Items.Draw(g);
            DrawMoreLabel(g);

            g.Dispose();
            e.Graphics.DrawImageUnscaled(_backBuffer, 0, 0);
        }

        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
            //Don't allow the background to paint
        }

        private void ExpertLegend_SizeChanged(object sender, EventArgs e)
        {
            if (_backBuffer != null)
            {
                _backBuffer.Dispose();
                _backBuffer = null;
            }
            this.Invalidate();
        }

        /// <summary>
        /// Загрузка свойств легенды
        /// </summary>
        /// <param name="nodePropertys"></param>
        public void Load(XmlNode nodePropertys)
        {
            if (nodePropertys == null)
                return;
            this.LegendSize = XmlHelper.GetIntAttrValue(nodePropertys, Consts.legendSize, 200);
            this.Location = (LegendLocation)Enum.Parse(typeof(LegendLocation), 
                XmlHelper.GetStringAttrValue(nodePropertys, Consts.legendLocation, "Right"));
            this.Visible = XmlHelper.GetBoolAttrValue(nodePropertys, Consts.legendVisible, false);
            this.RangeLimitsVisible = XmlHelper.GetBoolAttrValue(nodePropertys, Consts.legendValuesVisible, false);

            //this.UnitDisplayType = (UnitDisplayType)Enum.Parse(typeof(UnitDisplayType),
            //    XmlHelper.GetStringAttrValue(nodePropertys, Consts.unitDisplayType, "None"));

            this.FormatString = XmlHelper.GetStringAttrValue(nodePropertys, Consts.formatString, "#,##0.00");

            ColorConverter colorConvertor = new ColorConverter();
            string color = XmlHelper.GetStringAttrValue(nodePropertys, Consts.legendColor, string.Empty);
            if (color != string.Empty)
            {
                this.Color = (Color)colorConvertor.ConvertFromString(color);
            }
            
            color = XmlHelper.GetStringAttrValue(nodePropertys, Consts.legendBorderColor, string.Empty);
            if (color != string.Empty)
            {
                this.BorderColor = (Color)colorConvertor.ConvertFromString(color);
            }

            //загрузим настройки шрифта
            FontConverter fontConverter = new FontConverter();
            string sfont = XmlHelper.GetStringAttrValue(nodePropertys, Consts.sfont, string.Empty);
            if (sfont != string.Empty)
            {
                this.Font = (Font)fontConverter.ConvertFromString(sfont);
            }


        }

        /// <summary>
        /// Сохранение свойств легенды
        /// </summary>
        /// <param name="propertysNode"></param>
        public void Save(XmlNode propertysNode)
        {
            if (propertysNode == null)
                return;
            XmlHelper.SetAttribute(propertysNode, Consts.legendSize, this.LegendSize.ToString());
            XmlHelper.SetAttribute(propertysNode, Consts.legendLocation, this.Location.ToString());
            XmlHelper.SetAttribute(propertysNode, Consts.legendVisible, this.Visible.ToString());
            XmlHelper.SetAttribute(propertysNode, Consts.legendValuesVisible, this.RangeLimitsVisible.ToString());

            XmlHelper.SetAttribute(propertysNode, Consts.formatString, this.FormatString);


            ColorConverter colorConvertor = new ColorConverter();
            XmlHelper.SetAttribute(propertysNode, Consts.legendColor, colorConvertor.ConvertToString(this.Color));
            XmlHelper.SetAttribute(propertysNode, Consts.legendBorderColor, colorConvertor.ConvertToString(this.BorderColor));

            //сохраним шрифт
            FontConverter fontConverter = new FontConverter();
            XmlHelper.SetAttribute(propertysNode, Consts.sfont, fontConverter.ConvertToString(this.Font));

        }



    }

    /// <summary>
    /// Размещение легенды
    /// </summary>
    public enum LegendLocation
    {
        [Description("Слева")]
        Left,
        [Description("Сверху")]
        Top,
        [Description("Справа")]
        Right,
        [Description("Снизу")]
        Bottom,
    }
}
