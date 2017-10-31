using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;
using Dundas.Maps.WinControl;
using Krista.FM.Client.MDXExpert.Data;

namespace Krista.FM.Client.MDXExpert
{
    public class SymbolRuleBrowseAdapter : FilterablePropertyBase
    {
        #region Поля

        private SymbolRule rule;
        private PredefinedSymbol symbol;
        private SymbolTextAppearance textAppearance;
        private SymbolSizeBrowseAdapter symbolSizeBrowse;
        private MapFormatBrowseClass symbolFormat;
        private SerieRule serieRule;
        private MapReportElement mapElement;

        #endregion

        #region Свойства

        [Browsable(false)]
        public MapReportElement MapElement
        {
            get { return this.mapElement; }
        }

        [Category("Значки")]
        [Description("Цвет бордюра")]
        [DisplayName("Цвет бордюра")]
        [Browsable(true)]
        public Color BorderColor
        {
            get { return symbol.BorderColor; }
            set { symbol.BorderColor = value; }
        }

        [Category("Значки")]
        [Description("Вид бордюра")]
        [DisplayName("Вид бордюра")]
        [TypeConverter(typeof(MapDashStyleConverter))]
        [Browsable(true)]
        public MapDashStyle BorderStyle
        {
            get { return symbol.BorderStyle; }
            set { symbol.BorderStyle = value; }
        }

        [Category("Значки")]
        [Description("Толщина бордюра")]
        [DisplayName("Толщина бордюра")]
        [Browsable(true)]
        public int BorderWidth
        {
            get { return symbol.BorderWidth; }
            set { symbol.BorderWidth = value; }
        }

        [Category("Значки")]
        [Description("Начальный цвет")]
        [DisplayName("Начальный цвет")]
        [Browsable(true)]
        public Color Color
        {
            get { return symbol.Color; }
            set 
            { 
                symbol.Color = value;
            }
        }

        [Category("Отображение значений")]
        [Description("Шрифт")]
        [DisplayName("Шрифт")]
        [TypeConverter(typeof(FontTypeConverter))]
        [Browsable(true)]
        public Font Font
        {
            get { return symbol.Font; }
            set
            {
                symbol.Font = value;
                SetSymbolFont(value);
            }
        }

        [Category("Отображение значений")]
        [Description("Цвет шрифта")]
        [DisplayName("Цвет шрифта")]
        [Browsable(true)]
        public Color FontColor
        {
            get { return symbol.TextColor; }
            set
            {
                symbol.TextColor = value;
                SetSymbolFontColor(value);
            }
        }

        [Category("Значки")]
        [Description("Градиент")]
        [DisplayName("Градиент")]
        [TypeConverter(typeof(GradientTypeConverter))]
        [Browsable(true)]
        public GradientType GradientType
        {
            get { return symbol.GradientType; }
            set { symbol.GradientType = value; }
        }

        [Category("Значки")]
        [Description("Узор")]
        [DisplayName("Узор")]
        [TypeConverter(typeof(MapHatchStyleConverter))]
        [Browsable(true)]
        public MapHatchStyle HatchStyle
        {
            get { return symbol.HatchStyle; }
            set { symbol.HatchStyle = value; }
        }

        [Category("Значки")]
        [Description("Обозначение")]
        [DisplayName("Обозначение")]
        [TypeConverter(typeof(MarkerTypeConverter))]
        [Browsable(true)]
        public MarkerStyle MarkerStyle
        {
            get { return symbol.MarkerStyle; }
            set { symbol.MarkerStyle = value; }
        }

        [Category("Значки")]
        [Description("Конечный цвет")]
        [DisplayName("Конечный цвет")]
        [Browsable(true)]
        public Color SecondaryColor
        {
            get { return symbol.SecondaryColor; }
            set { symbol.SecondaryColor = value; }
        }

        [Category("Значки")]
        [Description("Смещение тени")]
        [DisplayName("Смещение тени")]
        [Browsable(true)]
        public int ShadowOffset
        {
            get { return symbol.ShadowOffset; }
            set { symbol.ShadowOffset = value; }
        }

        [Category("Отображение значений")]
        [Description("Подсказки")]
        [DisplayName("Подсказки")]
        [Browsable(true)]
        public string ToolTip
        {
            get { return symbol.ToolTip; }
            set
            {
                symbol.ToolTip = value;
            }
        }

        [Category("Отображение значений")]
        [Description("Подписи данных")]
        [DisplayName("Подписи данных")]
        [Browsable(true)]
        public SymbolTextAppearance TextAppearance
        {
            get { return textAppearance; }
            set { textAppearance = value; }
        }

        [Browsable(false)]
        public bool ShowDataGrouping
        {
            get { return (!this.serieRule.Serie.Series.IsProportionalSymbolSize || !this.AllowProportionalSize); }
        }

        [Browsable(false)]
        public bool ShowRangeLimits
        {
            get
            {
                return ((!this.serieRule.Serie.Series.IsProportionalSymbolSize || !this.AllowProportionalSize) && 
                            (this.DataGrouping == DataGroupingExt.Custom)); }
        }

        [Category("Отображение значений")]
        [Description("Разбиение интервалов")]
        [DisplayName("Разбиение интервалов")]
        [TypeConverter(typeof(EnumTypeConverter))]
        [DynamicPropertyFilter("ShowDataGrouping", "True")]
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
            get { return rule.PredefinedSymbols.Count; }
            set 
            {
                SetIntervalCount(value);
            }
        }

        [Category("Отображение значений")]
        [Description("Применять пропорциональный размер")]
        [DisplayName("Применять пропорциональный размер")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [Browsable(true)]
        public bool AllowProportionalSize
        {
            get { return !serieRule.Serie.Series.NotProportionalMeasures.Contains(this.serieRule.Name); }
            set
            {
                SetAllowProportionalSize(value);
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
                for (int i = 0; i < rule.PredefinedSymbols.Count; i++)
                {
                    Double.TryParse(rule.PredefinedSymbols[i].FromValue, out rangeLimit);
                    rangeLimits.Add(rangeLimit);
                }
                Double.TryParse(rule.PredefinedSymbols[rule.PredefinedSymbols.Count - 1].ToValue,
                             out rangeLimit);
                rangeLimits.Add(rangeLimit);

                return rangeLimits;
            }
            set
            {
                for(int i = 0; i < value.Count - 1; i++)
                {
                    string strValue = value[i].ToString();
                    rule.PredefinedSymbols[i].FromValue = strValue;
                    strValue = value[i + 1].ToString();
                    rule.PredefinedSymbols[i].ToValue = strValue;
                }
                //this.mapElement.RefreshMapAppearance();
                //this.mapElement.MainForm.Saved = false;

            }
        }


        [Category("Отображение значений")]
        [Description("Наименования интервалов")]
        [DisplayName("Наименования интервалов")]
        [Editor(typeof(SymbolIntervalNameEditor), typeof(UITypeEditor))]
        [Browsable(true)]
        public PredefinedSymbolCollection Intervals
        {
            get { return this.rule.PredefinedSymbols; }
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
            get
            {
                return this.serieRule.Name;
                //return rule.SymbolField;
            }
        }

        [Category("Значки")]
        [Description("Размеры значков")]
        [DisplayName("Размеры значков")]
        [Browsable(true)]
        public SymbolSizeBrowseAdapter SymbolSizeBrowse
        {
            get
            {
                return this.symbolSizeBrowse;
            }
            set
            {
                this.symbolSizeBrowse = value;
            }
        }

        [Category("Отображение значений")]
        [Description("Формат значений")]
        [DisplayName("Формат значений")]
        [Browsable(true)]
        public MapFormatBrowseClass SymbolFormat
        {
            get
            {
                return symbolFormat;
            }
            set
            {
                symbolFormat = value;
            }
        }


        #endregion

        public SymbolRuleBrowseAdapter(SymbolRule rule, MapReportElement mapElement, SerieRule serieRule)
        {
            this.rule = rule;
            if (rule.PredefinedSymbols.Count == 0)
            {
                return;
            }

            this.serieRule = serieRule;

            this.symbol = rule.PredefinedSymbols[0];

            RefreshSymbolTextConverter();

            this.symbolFormat = new MapFormatBrowseClass(symbol.Text, symbol.ToolTip, false);

            this.symbolFormat.FormatChanged += new ValueFormatEventHandler(symbolFormat_FormatChanged);

            textAppearance = new SymbolTextAppearance(symbol, this.symbolFormat, serieRule);
            
            SymbolSizeBrowse = new SymbolSizeBrowseAdapter(mapElement);

            this.mapElement = mapElement;
            
            /*
            this.IntervalCount = this.serieRule.RangeCount;
            if (this.serieRule.DataGrouping == DataGroupingExt.Custom)
            {
                this.RangeLimits = this.serieRule.RangeLimits;
            }
            else
            {
                this.DataGrouping = this.serieRule.DataGrouping;
            }
            */
        }

        private void symbolFormat_FormatChanged()
        {
            this.symbol.Text = this.SymbolFormat.ApplyFormatToText(this.symbol.Text, this.SymbolFormat.FormatString);
            this.symbol.ToolTip = this.SymbolFormat.ApplyFormatToText(this.symbol.ToolTip, this.SymbolFormat.FormatString);
            RefreshSymbolTextConverter();
        }

        private void SetIntervalCount(int value)
        {
            if (value == rule.PredefinedSymbols.Count)
            {
                return;
            }

            if ((value > 0) && (value <= 20))
            {
                while (rule.PredefinedSymbols.Count > 1)
                {
                    rule.PredefinedSymbols.RemoveAt(rule.PredefinedSymbols.Count - 1);
                }

                for (int i = 0; i < value - 1; i++)
                {
                    PredefinedSymbol symbol = new PredefinedSymbol();
                    symbol.Color = this.symbol.Color;
                    this.rule.PredefinedSymbols.Add(symbol);

                }

                MapHelper.SetSizePredefinedSymbols(rule);
                if (this.DataGrouping == DataGroupingExt.Custom)
                {
                    ClearCustomIntervals();
                    this.RangeLimits = MapHelper.GetRangeLimits(this.rule.PredefinedSymbols.Count, this.serieRule.Serie.Table, this.serieRule.Name, this.serieRule.DigitCount);
                }

            }
        }

        private void RefreshSymbolTextConverter()
        {
            SymbolTextConverter.StringValues.Clear();
            SymbolTextConverter.StringValues.Add("");
            SymbolTextConverter.StringValues.Add("#" + MapHelper.CorrectFieldName(this.rule.SymbolField).ToUpper() + "{#,##0.00}");

            if (this.SymbolFormat != null)
            {
                string formatStr = "#" + MapHelper.CorrectFieldName(this.rule.SymbolField).ToUpper() +
                                                     "{" + this.SymbolFormat.FormatString + "}";
                if (!SymbolTextConverter.StringValues.Contains(formatStr))
                    this.serieRule.SymbolTextValues[0] = formatStr;
            }

            foreach(string value in this.serieRule.SymbolTextValues)
            {
                if (!SymbolTextConverter.StringValues.Contains(value))
                    SymbolTextConverter.StringValues.Add(value);
            }
        }

        private void SetSymbolFont(Font font)
        {
            if (this.serieRule == null)
            {
                return;
            }
            
            foreach(Symbol symbol in this.serieRule.Serie.Series.Element.Map.Symbols)
            {
                if ((symbol.Category == this.serieRule.Serie.Name) && ((string)symbol.Tag == this.serieRule.Name))
                {
                    symbol.Font = font;
                }
            }
        }

        private void SetSymbolFontColor(Color fontColor)
        {
            if (this.serieRule == null)
            {
                return;
            }

            foreach (Symbol symbol in this.serieRule.Serie.Series.Element.Map.Symbols)
            {
                if ((symbol.Category == this.serieRule.Serie.Name) && ((string)symbol.Tag == this.serieRule.Name))
                {
                    symbol.TextColor = fontColor;
                }
            }
        }


        private void ClearCustomIntervals()
        {
            for (int i = 0; i < rule.PredefinedSymbols.Count; i++)
            {
                rule.PredefinedSymbols[i].FromValue = "";
                rule.PredefinedSymbols[i].ToValue = "";
            }
        }

        private void SetDataGrouping(DataGroupingExt value)
        {
            ClearCustomIntervals();
            switch(value)
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
                    this.rule.DataGrouping = Dundas.Maps.WinControl.DataGrouping.EqualInterval;
                    this.RangeLimits = MapHelper.GetRangeLimits(this.rule.PredefinedSymbols.Count, this.serieRule.Serie.Table, this.serieRule.Name, this.serieRule.DigitCount);
                    break;
            }

        }

        private DataGroupingExt GetDataGrouping()
        {
            if (this.rule.PredefinedSymbols.Count > 0)
            {
                if(this.rule.PredefinedSymbols[0].FromValue != "")
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

        private void SetAllowProportionalSize(bool value)
        {
            if (value)
            {
                if (this.serieRule.Serie.Series.NotProportionalMeasures.Contains(this.serieRule.Name))
                {
                    this.serieRule.Serie.Series.NotProportionalMeasures.Remove(this.serieRule.Name);
                }
            }
            else
            {
                if (!this.serieRule.Serie.Series.NotProportionalMeasures.Contains(this.serieRule.Name))
                {
                    this.serieRule.Serie.Series.NotProportionalMeasures.Add(this.serieRule.Name);
                }
            }
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

    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class SymbolTextAppearance
    {
        #region Поля

        private PredefinedSymbol symbol;
        private MapFormatBrowseClass symbolFormat;
        private SerieRule serieRule;

        #endregion

        #region Свойства

        [Description("Расположение подписи")]
        [DisplayName("Расположение подписи")]
        [TypeConverter(typeof(TextAlignmentConverter))]
        [Browsable(true)]
        public TextAlignment Alignment
        {
            get { return symbol.TextAlignment; }
            set { symbol.TextAlignment = value; }
        }

        [Description("Текст подписи")]
        [DisplayName("Текст")]
        [TypeConverter(typeof(SymbolTextConverter))]
        [Browsable(true)]
        public string Text
        {
            get
            {
                return symbol.Text;
            }
            set
            {
                this.symbolFormat.FormatString = value != "" ? value : this.symbol.ToolTip;

                symbol.Text = value;
                AddStringValueToRule(value);
            }
        }

        [Description("Цвет шрифта")]
        [DisplayName("Цвет")]
        [Browsable(true)]
        public Color Color
        {
            get { return symbol.TextColor; }
            set { symbol.TextColor = value; }
        }

        [Description("Смещение тени подписи")]
        [DisplayName("Смещение тени")]
        [Browsable(true)]
        public int ShadowOffset
        {
            get { return symbol.TextShadowOffset; }
            set { symbol.TextShadowOffset = value; }
        }


        #endregion

        public SymbolTextAppearance(PredefinedSymbol symbol, MapFormatBrowseClass symbolFormat, SerieRule serieRule)
        {
            this.symbol = symbol;
            this.symbolFormat = symbolFormat;
            this.symbolFormat.FormatStringChanged += new ValueFormatEventHandler(symbolFormat_FormatStringChanged);

            this.serieRule = serieRule;
        }

        private void symbolFormat_FormatStringChanged()
        {
            this.symbolFormat.FormatString = this.symbol.Text;
        }

        private void AddStringValueToRule(string value)
        {
            if (SymbolTextConverter.StringValues.Contains(value))
            {
                return;
            }

            SymbolTextConverter.StringValues.Add(value);
            if (!serieRule.SymbolTextValues.Contains(value))
                serieRule.SymbolTextValues.Add(value);
        }


        public override string ToString()
        {
            return "";
        }
    }

}
