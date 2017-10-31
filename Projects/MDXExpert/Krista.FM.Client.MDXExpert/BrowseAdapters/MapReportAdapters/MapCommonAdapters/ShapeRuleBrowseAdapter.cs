using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;
using Dundas.Maps.WinControl;
using Krista.FM.Client.MDXExpert.Data;
using Krista.FM.Client.MDXExpert.Common;

namespace Krista.FM.Client.MDXExpert
{
    public class ShapeRuleBrowseAdapter : FilterablePropertyBase
    {
        #region Поля

        private ShapeRule rule;
        private PropertyGrid propertyGrid;
        private MapFormatBrowseClass shapeFormat;
        private SerieRule serieRule;
        private MapReportElement mapElement;

        #endregion


        #region Свойства

        [Browsable(false)]
        public MapReportElement MapElement
        {
            get { return this.mapElement; }
        }

        [Category("Цветовое оформление")]
        [Description("Цвет бордюра")]
        [DisplayName("Цвет бордюра")]
        [Browsable(true)]
        public Color BorderColor
        {
            get { return rule.BorderColor; }
            set { rule.BorderColor = value; }
        }

        [Category("Цветовое оформление")]
        [Description("Цветовой режим")]
        [DisplayName("Цветовой режим")]
        [DynamicPropertyFilter("UseCustomColors", "False")]
        [TypeConverter(typeof(ColoringModeConverter))]
        [Browsable(true)]
        public ColoringMode ColoringMode
        {
            get { return rule.ColoringMode; }
            set { rule.ColoringMode = value; }
        }

        [Category("Цветовое оформление")]
        [Description("Палитра")]
        [DisplayName("Палитра")]
        [TypeConverter(typeof(MapColorPaletteConverter))]
        [DynamicPropertyFilter("ShowColorPalette", "True")]
        [Browsable(true)]
        public MapColorPalette ColorPalette
        {
            get { return rule.ColorPalette; }
            set { rule.ColorPalette = value; }
        }

        [Browsable(false)]
        public bool ShowColorPalette
        {
            get { return ((ColoringMode == ColoringMode.DistinctColors) && !UseCustomColors); }
        }

        [Browsable(false)]
        public bool ShowRangeColor
        {
            get { return ((ColoringMode == ColoringMode.ColorRange) && !UseCustomColors); }
        }

        [Browsable(false)]
        public bool ShowRangeLimits
        {
            get { return ((DataGrouping == DataGroupingExt.Custom) && UseCustomColors); }
        }

        [Category("Цветовое оформление")]
        [Description("Пользовательские цвета")]
        [DisplayName("Пользовательские цвета")]
        [DynamicPropertyFilter("UseCustomColors", "True")]
        [Editor(typeof(CustomColorCollectionEditor), typeof(UITypeEditor))]
        [Browsable(true)]
        public CustomColorCollection CustomColors
        {
            get
            {
                CustomColorCollectionEditor.MainForm = this.serieRule.Serie.Series.Element.MainForm;
                return rule.CustomColors;
            }
        }

        [Category("Цветовое оформление")]
        [Description("Применять пользовательские цвета")]
        [DisplayName("Применять пользовательские цвета")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [Browsable(true)]
        public bool UseCustomColors
        {
            get { return rule.UseCustomColors; }
            set { rule.UseCustomColors = value; }
        }

        [Category("Цветовое оформление")]
        [Description("Начальный цвет")]
        [DisplayName("Начальный цвет")]
        [DynamicPropertyFilter("ShowRangeColor", "True")]
        [Browsable(true)]
        public Color FromColor
        {
            get { return rule.FromColor; }
            set { rule.FromColor = value; }
        }

        [Category("Цветовое оформление")]
        [Description("Градиент")]
        [DisplayName("Градиент")]
        [TypeConverter(typeof(GradientTypeConverter))]
        [Browsable(true)]
        public GradientType GradientType
        {
            get { return rule.GradientType; }
            set
            {
                rule.GradientType = value;
                if (UseCustomColors)
                {
                    SetGradientToCustomColors();
                }
            }
        }

        [Category("Цветовое оформление")]
        [Description("Узор")]
        [DisplayName("Узор")]
        [TypeConverter(typeof(MapHatchStyleConverter))]
        [Browsable(true)]
        public MapHatchStyle HatchStyle
        {
            get { return rule.HatchStyle; }
            set
            {
                rule.HatchStyle = value;
                if(UseCustomColors)
                {
                    SetHatchToCustomColors();
                }
            }
        }

        [Category("Цветовое оформление")]
        [Description("Промежуточный цвет")]
        [DisplayName("Промежуточный цвет")]
        [DynamicPropertyFilter("ShowRangeColor", "True")]
        [Browsable(true)]
        public Color MiddleColor
        {
            get { return rule.MiddleColor; }
            set { rule.MiddleColor = value; }
        }

        [Category("Цветовое оформление")]
        [Description("Дополнительный цвет")]
        [DisplayName("Дополнительный цвет")]
        [Browsable(true)]
        public Color SecondaryColor
        {
            get { return rule.SecondaryColor; }
            set { rule.SecondaryColor = value; }
        }

        [Category("Отображение значений")]
        [Description("Подписи данных")]
        [DisplayName("Подписи данных")]
        [TypeConverter(typeof(ShapeTextConverter))]
        [Browsable(true)]
        public string Text
        {
            get { return rule.Text; }
            set
            {
                shapeFormat.FormatString = value != "" ? value : this.ToolTip;

                rule.Text = value;
                if(UseCustomColors)
                {
                    SetTextToCustomColors();
                }
                AddStringValueToRule(value);
            }
        }

        [Category("Отображение значений")]
        [Description("Формат значений")]
        [DisplayName("Формат значений")]
        [Browsable(true)]
        public MapFormatBrowseClass ShapeFormat
        {
            get
            {
                return shapeFormat;
            }
            set
            {
                shapeFormat = value;
            }
        }

        [Category("Цветовое оформление")]
        [Description("Конечный цвет")]
        [DisplayName("Конечный цвет")]
        [DynamicPropertyFilter("ShowRangeColor", "True")]
        [Browsable(true)]
        public Color ToColor
        {
            get { return rule.ToColor; }
            set { rule.ToColor = value; }
        }

        [Category("Отображение значений")]
        [Description("Подсказки")]
        [DisplayName("Подсказки")]
        [Browsable(true)]
        public string ToolTip
        {
            get { return rule.ToolTip; }
            set
            {
                rule.ToolTip = value;
                if (UseCustomColors)
                {
                    SetToolTipToCustomColors();
                }
            }
        }

        [Category("Отображение значений")]
        [Description("Разбиение интервалов")]
        [DisplayName("Разбиение интервалов")]
        [TypeConverter(typeof(EnumTypeConverter))]
        [Browsable(true)]
        public DataGroupingExt DataGrouping
        {
            get { return GetDataGrouping(); }
            set
            {
                SetDataGrouping(value);
            }
        }

        [Category("Отображение значений")]
        [Description("Количество интервалов")]
        [DisplayName("Количество интервалов")]
        [DynamicPropertyFilter("DataGrouping", "EqualDistribution, EqualInterval, Custom")]
        [Browsable(true)]
        public int IntervalCount
        {
            get { return rule.ColorCount; }
            set
            {
                if (value < 1)
                {
                    value = 5;
                }
                rule.ColorCount = value;
                if (UseCustomColors)
                {
                    SetCustomColors();
                }
            }
        }

        [Category("Отображение значений")]
        [Description("Границы интервалов")]
        [DisplayName("Границы интервалов")]
        [Editor(typeof(MapRangeEditor), typeof(UITypeEditor))]
        [DynamicPropertyFilter("ShowRangeLimits", "True")]
        [Browsable(true)]
        public List<double> RangeLimits
        {
            get
            {
                List<double> rangeLimits = new List<double>();

                double rangeLimit;
                for (int i = 0; i < rule.CustomColors.Count; i++)
                {
                    Double.TryParse(rule.CustomColors[i].FromValue, out rangeLimit);
                    rangeLimits.Add(rangeLimit);
                }
                Double.TryParse(rule.CustomColors[rule.CustomColors.Count - 1].ToValue,
                             out rangeLimit);
                rangeLimits.Add(rangeLimit);


                return rangeLimits;
            }
            set
            {
                for (int i = 0; i < value.Count - 1; i++)
                {
                    rule.CustomColors[i].FromValue = value[i].ToString();
                    rule.CustomColors[i].ToValue = value[i + 1].ToString();
                }
            }
        }

        [Category("Отображение значений")]
        [Description("Наименования интервалов")]
        [DisplayName("Наименования интервалов")]
        [Editor(typeof(ColorIntervalNameEditor), typeof(UITypeEditor))]
        [Browsable(true)]
        public CustomColorCollection Intervals
        {
            get { return this.rule.CustomColors; }
            set { this.mapElement.RefreshLegendContent(); }
        }

        [Category("Отображение значений")]
        [Description("Тип отображения интервалов в легенде")]
        [DisplayName("Отображать в легенде")]
        [TypeConverter(typeof(EnumTypeConverter))]
        [Browsable(true)]
        public LegendContentType ContentType
        {
            get { return GetContentType(); }
            set { SetContentType(value); }
        }


        [Category("Данные")]
        [Description("Серия")]
        [DisplayName("Серия")]
        [Browsable(true)]
        public string SerieName
        {
            get { return rule.Category; }
        }

        [Category("Данные")]
        [Description("Показатель")]
        [DisplayName("Показатель")]
        [Browsable(true)]
        public string MeasureName
        {
            get { return this.serieRule.Name; }
        }

        #endregion

        public ShapeRuleBrowseAdapter(ShapeRule rule, PropertyGrid pGrid, MapReportElement mapElement, SerieRule serieRule)
        {
            this.rule = rule;
            this.propertyGrid = pGrid;

            this.serieRule = serieRule;
            this.mapElement = mapElement;

            RefreshShapeTextConverter();

            this.shapeFormat = new MapFormatBrowseClass(this.Text, this.ToolTip, false);
            this.shapeFormat.FormatChanged += new ValueFormatEventHandler(shapeFormat_FormatChanged);
            this.shapeFormat.FormatStringChanged += new ValueFormatEventHandler(shapeFormat_FormatStringChanged);
        }


        #region Обработчики

        private void shapeFormat_FormatChanged()
        {
            this.rule.Text = this.shapeFormat.ApplyFormatToText(this.Text, this.shapeFormat.FormatString);
            this.rule.ToolTip = this.shapeFormat.ApplyFormatToText(this.ToolTip, this.shapeFormat.FormatString);
            if (UseCustomColors)
            {
                SetTextToCustomColors();
                SetToolTipToCustomColors();
            }
            RefreshShapeTextConverter();

        }

        private void shapeFormat_FormatStringChanged()
        {
            this.shapeFormat.FormatString = this.Text;
        }

        #endregion

        #region Сеттеры
         
        private void SetCustomColors()
        {
            int customColorsCount = CustomColors.Count;

            if (customColorsCount > IntervalCount)
            {
                for(int i = customColorsCount; i > IntervalCount; i--)
                {
                    CustomColors.RemoveAt(i - 1);
                }
            }

            if (customColorsCount < IntervalCount)
            {
                for (int i = customColorsCount; i < IntervalCount; i++)
                {
                    CustomColors.Add(new CustomColor());
                    CustomColors[i].Color = MapHelper.GetRandomColor();
                    CustomColors[i].HatchStyle = this.HatchStyle;
                    CustomColors[i].GradientType = this.GradientType;
                    CustomColors[i].Text = this.Text;
                    CustomColors[i].ToolTip = this.ToolTip;
                }
            }
            if (this.DataGrouping == DataGroupingExt.Custom)
            {
                ClearCustomIntervals();
                this.RangeLimits = MapHelper.GetRangeLimits(this.rule.CustomColors.Count, this.serieRule.Serie.Table, this.serieRule.Name, this.serieRule.DigitCount);
            }


        }

        private void SetHatchToCustomColors()
        {
            foreach(CustomColor cColor in this.CustomColors)
            {
                cColor.HatchStyle = this.HatchStyle;
            }
        }

        private void SetGradientToCustomColors()
        {
            foreach (CustomColor cColor in this.CustomColors)
            {
                cColor.GradientType = this.GradientType;
            }
        }

        private void SetTextToCustomColors()
        {
            foreach (CustomColor cColor in this.CustomColors)
            {
                cColor.Text = this.Text;
            }
        }

        private void SetToolTipToCustomColors()
        {
            foreach (CustomColor cColor in this.CustomColors)
            {
                cColor.ToolTip = this.ToolTip;
            }
        }

        #endregion

        private void RefreshShapeTextConverter()
        {
            ShapeTextConverter.StringValues.Clear();
            ShapeTextConverter.StringValues.Add("");
            ShapeTextConverter.StringValues.Add("#" + this.mapElement.GetShapeFieldCaption());
            ShapeTextConverter.StringValues.Add("#" + this.mapElement.GetShapeFieldCaption()+ "\n#" + MapHelper.CorrectFieldName(this.rule.ShapeField).ToUpper() + "{#,##0.00}");

            if ((this.ShapeFormat != null) && (this.ShapeFormat.FormatString != ""))
            {
                string formatStr = "#" + this.mapElement.GetShapeFieldCaption() + "\n#" + MapHelper.CorrectFieldName(this.rule.ShapeField).ToUpper() +
                                               "{" + this.ShapeFormat.FormatString + "}";

                if (!ShapeTextConverter.StringValues.Contains(formatStr))
                    serieRule.ShapeTextValues[0] = formatStr;
            }

            foreach (string value in serieRule.ShapeTextValues)
            {
                if (!ShapeTextConverter.StringValues.Contains(value))
                    ShapeTextConverter.StringValues.Add(value);
            }
        }

        private void AddStringValueToRule(string value)
        {
            if (ShapeTextConverter.StringValues.Contains(value))
            {
                return;
            }

            ShapeTextConverter.StringValues.Add(value);
            if (!serieRule.ShapeTextValues.Contains(value))
                serieRule.ShapeTextValues.Add(value);
        }

        private void ClearCustomIntervals()
        {
            for (int i = 0; i < rule.CustomColors.Count; i++)
            {
                rule.CustomColors[i].FromValue = "";
                rule.CustomColors[i].ToValue = "";
            }
        }

        private void SetDataGrouping(DataGroupingExt value)
        {
            ClearCustomIntervals();
            switch (value)
            {
                case DataGroupingExt.EqualDistribution:
                    this.rule.DataGrouping = Dundas.Maps.WinControl.DataGrouping.EqualDistribution;
                    break;
                case DataGroupingExt.EqualInterval:
                    this.rule.DataGrouping = Dundas.Maps.WinControl.DataGrouping.EqualInterval;
                    break;
                case DataGroupingExt.Optimal:
                    this.rule.DataGrouping = Dundas.Maps.WinControl.DataGrouping.Optimal;
                    break;
                case DataGroupingExt.Custom:
                    this.UseCustomColors = true;
                    this.rule.DataGrouping = Dundas.Maps.WinControl.DataGrouping.EqualInterval;
                    this.IntervalCount = this.rule.CustomColors.Count;
                    this.RangeLimits = MapHelper.GetRangeLimits(this.rule.CustomColors.Count, this.serieRule.Serie.Table, this.serieRule.Name, this.serieRule.DigitCount);
                    break;
            }

        }

        private DataGroupingExt GetDataGrouping()
        {
            if (this.rule.CustomColors.Count > 0)
            {
                if (this.rule.CustomColors[0].FromValue != "")
                {
                    return DataGroupingExt.Custom;
                }
            }

            switch (this.rule.DataGrouping)
            {
                case Dundas.Maps.WinControl.DataGrouping.EqualDistribution:
                    return DataGroupingExt.EqualDistribution;
                case Dundas.Maps.WinControl.DataGrouping.EqualInterval:
                    return DataGroupingExt.EqualInterval;
                case Dundas.Maps.WinControl.DataGrouping.Optimal:
                    return DataGroupingExt.Optimal;
            }

            return DataGroupingExt.EqualInterval;
        }

        public SerieRule GetSerieRule()
        {
            return this.serieRule;
        }

        private LegendContentType GetContentType()
        {
            LegendContent content = this.mapElement.LegendContents[this.rule.ShowInLegend];
            if (content != null)
                return content.ContentType;

            return LegendContentType.Values;
        }

        private void SetContentType(LegendContentType value)
        {
            LegendContent content = this.mapElement.LegendContents[this.rule.ShowInLegend];
            if (content != null)
                content.ContentType = value;
        }


        public override string ToString()
        {
            return "";
        }


    }

}
