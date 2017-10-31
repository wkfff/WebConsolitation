using System;
using System.Drawing;
using System.Xml;
using Krista.FM.Common.Xml;

namespace Krista.FM.Client.MDXExpert.Grid.Style
{
    public class CellStyle
    {
        private Color _backColorStart;
        private Color _backColorEnd;
        private Color _borderColor;
        private Color _foreColor;
        private StringFormat _stringFormat;
        private System.Drawing.Font _font;
        private Gradient _gradient;
        private Color _progressBarColor;
        private Brush _foreColorBrush;
        private Brush _backColorBrush;
        private Pen _borderPen;
        private ExpertGrid _grid;

        //эти поля иницализируются только при загрузке шаблона
        private Color _templateBackColorStart;
        private Color _templateBackColorEnd;
        private Color _templateBorderColor;
        private Color _templateForeColor;
        private Font _templateFont;
        private StringFormat _templateStringFormat;


        public CellStyle(ExpertGrid grid, Color backColorStart, Color backColorEnd, 
            Color foreColor, Color borderColor)
        {
            this.Grid = grid;
            this.ProgressBarColor = Color.Blue;
            this.StringFormat = new StringFormat();
            this.BackColorStart = backColorStart;
            this.BackColorEnd = backColorEnd;
            this.BorderColor = borderColor;
            this.ForeColor = foreColor;

            this._templateFont = null;
            this._templateStringFormat = new StringFormat();
        }

        public bool ShouldSerializeFont()
        {
            return (this.Font != null);
        }

        /// <summary>
        /// Загрузка параметров стиля из Xml
        /// </summary>
        /// <param name="node"></param>
        public void Load(XmlNode styleNode, bool isLoadTemplate)
        {
            if (styleNode == null)
                return;
            
            if (!isLoadTemplate)
                this.SetDefaultStyle();
            
            XmlNode colorsNode = styleNode.SelectSingleNode(GridConsts.colors);
            if (colorsNode != null)
            {
                ColorConverter colorConvertor = new ColorConverter();
                string color;
                color = XmlHelper.GetStringAttrValue(colorsNode, GridConsts.backColorStart, string.Empty);
                if (color != string.Empty)
                {
                    this.BackColorStart = (Color)colorConvertor.ConvertFromString(color);
                }

                color = XmlHelper.GetStringAttrValue(colorsNode, GridConsts.backColorEnd, string.Empty);
                if (color != string.Empty)
                {
                    this.BackColorEnd = (Color)colorConvertor.ConvertFromString(color);  
                }

                color = XmlHelper.GetStringAttrValue(colorsNode, GridConsts.borderColor, string.Empty);
                if (color != string.Empty)
                {
                    this.BorderColor = (Color)colorConvertor.ConvertFromString(color);  
                }

                color = XmlHelper.GetStringAttrValue(colorsNode, GridConsts.foreColor, string.Empty);
                if (color != string.Empty)
                {
                    this.ForeColor = (Color)colorConvertor.ConvertFromString(color);
                }

                this.Gradient = (Gradient)Enum.Parse(typeof(Gradient),
                        XmlHelper.GetStringAttrValue(colorsNode, GridConsts.gradient, "Horizontal"));
            }

            //загрузим настройки шрифта
            XmlNode fontNode = styleNode.SelectSingleNode(GridConsts.font);
            if (fontNode != null)
            {
                FontConverter fontConverter = new FontConverter();
                string sfont = XmlHelper.GetStringAttrValue(fontNode, GridConsts.sfont, string.Empty);
                if (sfont != string.Empty)
                {
                    this.Font = (Font)fontConverter.ConvertFromString(sfont);
                }
            }

            //загрузим формат строки
            XmlNode stringFormatNode = styleNode.SelectSingleNode(GridConsts.stringFormat);
            if (stringFormatNode != null)
            {
                //горизонтальное выравнивание
                string hAligment = XmlHelper.GetStringAttrValue(stringFormatNode, GridConsts.hAligment, string.Empty);
                if (hAligment != string.Empty)
                {
                    this.StringFormat.Alignment = (StringAlignment)Enum.Parse(typeof(StringAlignment), hAligment);
                }
                //вертикальное выравнивание
                string vAligment = XmlHelper.GetStringAttrValue(stringFormatNode, GridConsts.vAligment, string.Empty);
                if (vAligment != string.Empty)
                {
                    this.StringFormat.LineAlignment = (StringAlignment)Enum.Parse(typeof(StringAlignment), vAligment);
                }
                //обрезание строки
                string trimming = XmlHelper.GetStringAttrValue(stringFormatNode, GridConsts.trimming, string.Empty);
                if (trimming != string.Empty)
                {
                    this.StringFormat.Trimming = (StringTrimming)Enum.Parse(typeof(StringTrimming), trimming);
                }
                //дополнительные флаги форматирования
                string formatFlags = XmlHelper.GetStringAttrValue(stringFormatNode, GridConsts.formatFlags, string.Empty);
                if (formatFlags != string.Empty)
                {
                    this.StringFormat.FormatFlags = (StringFormatFlags)Enum.Parse(typeof(StringFormatFlags), formatFlags);
                }
            }
            
            if (isLoadTemplate)
            {
                //сохраним настройки шаблона
                this._templateBackColorStart = this.BackColorStart;
                this._templateBackColorEnd = this.BackColorEnd;
                this._templateBorderColor = this.BorderColor;
                this._templateForeColor = this.ForeColor;
                this._templateFont = this.Font;
                this._templateStringFormat = this.StringFormat;
            }
        }

        public void Save(XmlNode styleNode)
        {
            if (styleNode == null)
                return;

            //сохранение цветов
            ColorConverter colorConvertor = new ColorConverter();
            XmlNode colorsNode = XmlHelper.AddChildNode(styleNode, GridConsts.colors);

            //записывать будем только то что отличается от шаблона
            if (this._templateBackColorStart != this.BackColorStart)
            {
                XmlHelper.SetAttribute(colorsNode, GridConsts.backColorStart, 
                    colorConvertor.ConvertToString(this.BackColorStart));
            }

            if (this._templateBackColorEnd != this.BackColorEnd)
            {
                XmlHelper.SetAttribute(colorsNode, GridConsts.backColorEnd,
                    colorConvertor.ConvertToString(this.BackColorEnd));
            }

            if (this._templateBorderColor != this.BorderColor)
            {
                XmlHelper.SetAttribute(colorsNode, GridConsts.borderColor,
                    colorConvertor.ConvertToString(this.BorderColor));
            }

            if (this._templateForeColor != this.ForeColor)
            {
                XmlHelper.SetAttribute(colorsNode, GridConsts.foreColor,
                    colorConvertor.ConvertToString(this.ForeColor));
            }

            XmlHelper.SetAttribute(colorsNode, GridConsts.gradient, this.Gradient.ToString());


            //сохраним шрифт
            XmlNode fontNode = XmlHelper.AddChildNode(styleNode, GridConsts.font);
            if ((this._templateFont == null) || !this._templateFont.Equals(this.OriginalFont))
            {
                FontConverter fontConverter = new FontConverter();
                XmlHelper.SetAttribute(fontNode, GridConsts.sfont, fontConverter.ConvertToString(this.OriginalFont));
            }

            //сохраним формат строк
            XmlNode stringFormatNode = XmlHelper.AddChildNode(styleNode, GridConsts.stringFormat);
            if ((this._templateStringFormat == null) || !this._templateStringFormat.Equals(this.StringFormat))
            {
                XmlHelper.SetAttribute(stringFormatNode, GridConsts.hAligment, 
                    this.StringFormat.Alignment.ToString());
                XmlHelper.SetAttribute(stringFormatNode, GridConsts.vAligment,
                    this.StringFormat.LineAlignment.ToString());
                XmlHelper.SetAttribute(stringFormatNode, GridConsts.trimming,
                    this.StringFormat.Trimming.ToString());
                XmlHelper.SetAttribute(stringFormatNode, GridConsts.formatFlags,
                    this.StringFormat.FormatFlags.ToString());
            }
        }

        /// <summary>
        /// Копирует настройки стилья по умолчанию в указанную ячейку
        /// </summary>
        /// <param name="cellStyle"></param>
        public void CopyDefaultStyle(CellStyle cellStyle)
        {
            if (cellStyle != null)
            {
                this.CopyDefaultColor(cellStyle);
                this.CopyDefaultFont(cellStyle);
                this.CopyDefaultStringFormat(cellStyle);
            }
        }

        private void CopyDefaultColor(CellStyle cellStyle)
        {
            if (this.TemplateBackColorStart != null)
                cellStyle.TemplateBackColorStart = this.TemplateBackColorStart;
            if (this.TemplateBackColorEnd != null)
                cellStyle.TemplateBackColorEnd = this.TemplateBackColorEnd;
            if (this.TemplateBorderColor != null)
                cellStyle.TemplateBorderColor = this.TemplateBorderColor;
            if (this.TemplateForeColor != null)
                cellStyle.TemplateForeColor = this.TemplateForeColor;
        }

        private void CopyDefaultFont(CellStyle cellStyle)
        {
            if (this.TemplateFont != null)
                cellStyle.TemplateFont = this.TemplateFont;
        }

        private void CopyDefaultStringFormat(CellStyle cellStyle)
        {
            if (this.TemplateStringFormat != null)
                cellStyle.TemplateStringFormat = this.TemplateStringFormat;
        }

        /// <summary>
        /// Установить ячейке настройки стиля по умолчанию
        /// </summary>
        public void SetDefaultStyle()
        {
            this.SetDefaultColor();
            this.SetDefaultFont();
            this.SetDefaultStringFormat();
        }

        private void SetDefaultColor()
        {
            if (this.TemplateBackColorStart != null)
                this.BackColorStart = this.TemplateBackColorStart;
            if (this.TemplateBackColorEnd != null)
                this.BackColorEnd = this.TemplateBackColorEnd;
            if (this.TemplateBorderColor != null)
                this.BorderColor = this.TemplateBorderColor;
            if (this.TemplateForeColor != null)
                this.ForeColor = this.TemplateForeColor;
        }

        private void SetDefaultFont()
        {
            if (this.TemplateFont != null)
                this.Font = this.TemplateFont;
        }

        private void SetDefaultStringFormat()
        {
            if (this.TemplateStringFormat != null)
                this.StringFormat = this.TemplateStringFormat;
        }

        public Color BackColorStart
        {
            get { return this._backColorStart; }
            set
            {
                this._backColorStart = value;
                this.BackColorBrush = new SolidBrush(value);
            }
        }                        

        public Color BackColorEnd
        {
            get { return this._backColorEnd; }
            set
            {
                this._backColorEnd = value;
                this.BackColorBrush = new SolidBrush(value);
            }
        }

        public Color BorderColor
        {
            get { return this._borderColor; }
            set
            {
                this._borderColor = value;
                this.BorderPen = new Pen(value);
            }
        }                     

        public StringFormat StringFormat
        {
            get { return this._stringFormat; }
            set { this._stringFormat = value; }
        }

        public System.Drawing.Font OriginalFont
        {
            get
            {
                if (this._font == null)
                {
                    this._font = new System.Drawing.Font("Arial", 10f);
                }
                return this._font;
            }
            set
            {
                if (value != null)
                {
                    this._font = value;
                }
            }
        }

        public System.Drawing.Font Font
        {
            get
            {
                if (this._font == null)
                {
                    this._font = new System.Drawing.Font("Arial", 10f);
                }

                //если стиль привязан к определенной таблице, учитываем масштаб таблицы
                float fontSize = this.Grid != null
                                     ? this.Grid.GridScale.GetScaledValue(this._font.Size)
                                     : this._font.Size;
                
                return new Font(this._font.FontFamily, fontSize, this._font.Style, this._font.Unit);
            }
            set
            {
                if (value != null)
                {
                    this._font = value;
                }
            }
        }

        public int TextHeight
        {
            get { return this.Font.Height + 1; }
        }

        public int OriginalTextHeight
        {
            get { return this._font.Height + 1; }
        }

        public Color ForeColor
        {
            get { return this._foreColor; }
            set 
            { 
                this._foreColor = value;
                this.ForeColorBrush = new SolidBrush(value);
            }
        }

        public Gradient Gradient
        {
            get { return this._gradient; }
            set { this._gradient = value; }
        }

        public Color ProgressBarColor
        {
            get { return this._progressBarColor; }
            set { this._progressBarColor = value; }
        }

        public Brush ForeColorBrush
        {
            get { return this._foreColorBrush; }
            set { this._foreColorBrush = value; }
        }

        public Brush BackColorBrush
        {
            get { return this._backColorBrush; }
            set { this._backColorBrush = value; }
        }

        public Pen BorderPen
        {
            get { return this._borderPen; }
            set { this._borderPen = value; }
        }

        #region Стиль шаблона
        public Color TemplateBackColorStart
        {
            get { return _templateBackColorStart; }
            set { _templateBackColorStart = value; }
        }

        public Color TemplateBackColorEnd
        {
            get { return _templateBackColorEnd; }
            set { _templateBackColorEnd = value; }
        }

        public Color TemplateBorderColor
        {
            get { return _templateBorderColor; }
            set { _templateBorderColor = value; }
        }

        public Color TemplateForeColor
        {
            get { return _templateForeColor; }
            set { _templateForeColor = value; }
        }

        public Font TemplateFont
        {
            get { return _templateFont; }
            set { _templateFont = value; }
        }

        public StringFormat TemplateStringFormat
        {
            get { return _templateStringFormat; }
            set { _templateStringFormat = value; }
        }

        public ExpertGrid Grid
        {
            get { return _grid; }
            set { _grid = value; }
        }

        #endregion
    }
}