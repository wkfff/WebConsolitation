using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using Dundas.Maps.WinControl;
using System.Drawing;

namespace Krista.FM.Client.MDXExpert
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class ColorSwatchPanelBrowseAdapter : MapPanelBrowseAdapterBase
    {
        #region Поля

        private ColorSwatchPanel colorSwatchPanel;
        private ColorSwatchLabelAppearance labelAppearance;
        private ColorSwatchTitleAppearance titleAppearance;

        #endregion

        #region Свойства

        
        [Description("Метки")]
        [DisplayName("Метки")]
        [Browsable(true)]
        public ColorSwatchLabelAppearance LabelAppearance
        {
            get { return labelAppearance; }
            set { labelAppearance = value; }
        }

        [Description("Заголовок")]
        [DisplayName("Заголовок")]
        [Browsable(true)]
        public ColorSwatchTitleAppearance TitleAppearance
        {
            get { return titleAppearance; }
            set { titleAppearance = value; }
        }

        [Description("Авторазмер")]
        [DisplayName("Авторазмер")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [Browsable(true)]
        public bool Autosize
        {
            get { return this.colorSwatchPanel.AutoSize; }
            set { this.colorSwatchPanel.AutoSize = value; }
        }
        /*
        [Description("Цвета")]
        [DisplayName("Цвета")]
        [Browsable(true)]
        public SwatchColorCollection Colors
        {
            get { return this.colorSwatchPanel.Colors; }
        }
        */
        [Description("Цвет контура")]
        [DisplayName("Цвет контура")]
        [Browsable(true)]
        public Color OutlineColor
        {
            get { return this.colorSwatchPanel.OutlineColor; }
            set { this.colorSwatchPanel.OutlineColor = value; }
        }

        [Description("Цвет интервала")]
        [DisplayName("Цвет интервала")]
        [Browsable(true)]
        public Color RangeGapColor
        {
            get { return this.colorSwatchPanel.RangeGapColor; }
            set { this.colorSwatchPanel.RangeGapColor = value; }
        }

        [Description("Длина отметки")]
        [DisplayName("Длина отметки")]
        [Browsable(true)]
        public int TickMarkLength
        {
            get { return this.colorSwatchPanel.TickMarkLength; }
            set { this.colorSwatchPanel.TickMarkLength = value; }
        }


        #endregion

        public ColorSwatchPanelBrowseAdapter(ColorSwatchPanel colorSwatchPanel) : base(colorSwatchPanel)
        {
            this.colorSwatchPanel = colorSwatchPanel;
            this.labelAppearance = new ColorSwatchLabelAppearance(colorSwatchPanel);
            this.titleAppearance = new ColorSwatchTitleAppearance(colorSwatchPanel);
        }

        public override string ToString()
        {
            return "";
        }


    }

    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class ColorSwatchLabelAppearance
    {
        #region Поля

        private ColorSwatchPanel colorSwatchPanel;

        #endregion

        #region Свойства

        [Description("Выравнивание метки")]
        [DisplayName("Выравнивание")]
        [Browsable(true)]
        public LabelAlignment Alignment
        {
            get { return colorSwatchPanel.LabelAlignment; }
            set { colorSwatchPanel.LabelAlignment = value; }
        }

        [Description("Цвет метки")]
        [DisplayName("Цвет")]
        [Browsable(true)]
        public Color Color
        {
            get { return colorSwatchPanel.LabelColor; }
            set { colorSwatchPanel.LabelColor = value; }
        }

        [Description("Интервал")]
        [DisplayName("Интервал")]
        [Browsable(true)]
        public int Interval
        {
            get { return colorSwatchPanel.LabelInterval; }
            set { colorSwatchPanel.LabelInterval = value; }
        }

        [Description("Тип метки")]
        [DisplayName("Тип")]
        [Browsable(true)]
        public SwatchLabelType LabelType
        {
            get { return colorSwatchPanel.LabelType; }
            set { colorSwatchPanel.LabelType = value; }
        }

        [Description("Показывать конечные метки")]
        [DisplayName("Показывать конечные метки")]
        [Browsable(true)]
        public bool ShowEndLabels
        {
            get { return colorSwatchPanel.ShowEndLabels; }
            set { colorSwatchPanel.ShowEndLabels = value; }
        }

        #endregion


        public ColorSwatchLabelAppearance(ColorSwatchPanel colorSwatchPanel)
        {
            this.colorSwatchPanel = colorSwatchPanel;
        }

        public override string ToString()
        {
            return "";
        }

    }


    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class ColorSwatchTitleAppearance
    {
        #region Поля

        private ColorSwatchPanel colorSwatchPanel;

        #endregion

        #region Свойства

        [Description("Текст заголовка")]
        [DisplayName("Текст")]
        [Browsable(true)]
        public string Title
        {
            get { return colorSwatchPanel.Title; }
            set { colorSwatchPanel.Title = value; }
        }

        [Description("Выравнивание заголовка")]
        [DisplayName("Выравнивание")]
        [TypeConverter(typeof(StringAlignmentBarHorizontalConverter))]
        [Browsable(true)]
        public StringAlignment Alignment
        {
            get { return colorSwatchPanel.TitleAlignment; }
            set { colorSwatchPanel.TitleAlignment = value; }
        }

        [Description("Цвет заголовка")]
        [DisplayName("Цвет")]
        [Browsable(true)]
        public Color Color
        {
            get { return colorSwatchPanel.TitleColor; }
            set { colorSwatchPanel.TitleColor = value; }
        }

        [Description("Шрифт заголовка")]
        [DisplayName("Шрифт")]
        [TypeConverter(typeof(FontTypeConverter))]
        [Browsable(true)]
        public Font Font
        {
            get { return colorSwatchPanel.TitleFont; }
            set { colorSwatchPanel.TitleFont = value; }
        }

        #endregion


        public ColorSwatchTitleAppearance(ColorSwatchPanel colorSwatchPanel)
        {
            this.colorSwatchPanel = colorSwatchPanel;
        }

        public override string ToString()
        {
            return "";
        }

    }

}
