using System;
using System.ComponentModel;
using Krista.FM.Client.Common.Forms;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.UltraChart.Resources;
using Krista.FM.Client.MDXExpert.Data;

namespace Krista.FM.Client.MDXExpert
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class  ChartFormatBrowseClass : FilterablePropertyBase
    {
        #region Поля

        private ValueFormat format;

        private string thousandDelimiter = "#,##";

        private string formatStr;
        private string formatStrWithPattern;

        private bool displayUnits;

        private LabelType labelType;
        private AxisLabelFormatPattern axisLabelFormatPattern;
        private SeriesLabelFormatPattern seriesLabelFormatPattern;
        private LegendFormatPattern legendFormatPattern;
        private TooltipFormatPattern tooltipFormatPattern;
        private PieLabelFormatPattern pieLabelFormatPattern;
        private static Dictionary<string, string> formatPatterns = MakePatternTable();

        private IChartComponent chart;
        private BubbleChartDataValueType bubbleValueType;
        private ScatterChartDataValueType scatterValueType;
        private PolarChartDataValueType polarValueType;
        private HeatMapChartDataValueType heatMapValueType;
        private TreeMapChartDataValueType treeMapValueType;
        private BoxChartDataValueType boxValueType;


        #endregion

        #region Свойства

        [Category("Формат")]
        [DisplayName("Тип")]
        [Description("Тип формата")]
        [TypeConverter(typeof(EnumTypeConverter))]
        [DefaultValue(FormatType.Auto)]
        [Browsable(true)]
        public FormatType FormatType
        {
            get
            {
                return format.FormatType;
            }
            set
            {
                format.FormatType = value;
                DoFormatChange();
            }
        }

        [Category("Формат")]
        [DisplayName("Число десятичных знаков")]
        [Description("Количество десятичных знаков, отображаемых после запятой")]
        //данное свойство будем отображать только при следующих типах значения показателя
        [DynamicPropertyFilter("FormatType", "Currency, ThousandsCurrency, ThousandsCurrencyWitoutDivision,"
                                             + "MillionsCurrency, MillionsCurrencyWitoutDivision, MilliardsCurrency, MilliardsCurrencyWitoutDivision,"
                                             + "Percent, Numeric, ThousandsNumeric, MillionsNumeric, MilliardsNumeric")]
        [DefaultValue(typeof(byte),"0")]
        [Browsable(true)]
        public byte DigitCount
        {
            get
            {
                return format.DigitCount;
            }
            set
            {
                //byte bValue = 0;
                //if ((byte.TryParse(value, out bValue)) && (bValue <= 20))
                if (value <= 20)
                {
                    format.DigitCount = value;
                    DoFormatChange();
                }
                else
                {
                    FormException.ShowErrorForm(new Exception("MDXExpert-PropertyGrid-DigitCount."),
                                                ErrorFormButtons.WithoutTerminate);
                }
            }
        }

        [Category("Формат")]
        [DisplayName("Разделитель групп разрядов")]
        [Description("Показывать или нет разделитель групп разрядов")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        //данное свойство будем отображать только при следующих типах значения показателя
        [DynamicPropertyFilter("FormatType", "Currency, ThousandsCurrency, ThousandsCurrencyWitoutDivision,"
                                             + "MillionsCurrency, MillionsCurrencyWitoutDivision, MilliardsCurrency, MilliardsCurrencyWitoutDivision,"
                                             + "Percent, Numeric, ThousandsNumeric, MillionsNumeric, MilliardsNumeric")]
        [DefaultValue(true)]
        [Browsable(true)]
        public bool UseThousandDelimiter
        {
            get
            {
                return format.ThousandDelimiter;
            }
            set
            {
                format.ThousandDelimiter = value;
                DoFormatChange();
            }
        }

        [Category("Формат")]
        [DisplayName("Отображать единицы измерения")]
        [Description("Отображать или нет единицы измерения")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        //данное свойство будем отображать только при следующих типах значения показателя
        [DynamicPropertyFilter("FormatType", "Currency, ThousandsCurrency, ThousandsCurrencyWitoutDivision,"
                                             + "MillionsCurrency, MillionsCurrencyWitoutDivision, MilliardsCurrency, MilliardsCurrencyWitoutDivision,"
                                             + "Percent")]
        [DefaultValue(true)]
        [Browsable(true)]
        public bool DisplayUnits
        {
            get
            {
                return this.displayUnits;
            }
            set
            {
                this.displayUnits = value;
                DoFormatChange();
            }
        }

        [Browsable(false)]
        public string FormatString
        {
            get 
            {
                //return GetFormatString(this.formatStr, this.FormatType, this.DigitCount, this.UseThousandDelimiter, this.displayUnits);
                this.formatStrWithPattern = GetFormatStringWithPattern(GetFormatString(this.formatStr, this.FormatType, this.DigitCount, this.UseThousandDelimiter, this.displayUnits));
                return this.formatStrWithPattern;
            }
            set 
            {
                this.formatStrWithPattern = value;
                this.SetPattern(value);
                
                this.formatStr = GetFormatStrWithoutPattern(value);


                this.FormatType = GetFormatTypeByFormatString(this.formatStr);
                if (this.FormatType != FormatType.Auto)
                {
                    this.SetDataValueType(this.formatStr);
                    this.format.DigitCount = GetDigitsCount(this.formatStr);
                    this.format.ThousandDelimiter = this.formatStr.Contains(thousandDelimiter);
                }
                else
                {
                    SetAutoFormatStr(this.formatStr);
                }

                SetDisplayUnits(this.formatStr);
            }
        }

        [Browsable(false)]
        public LabelType LabelPatternType
        {
            get 
            {
                if (this.FormatType == FormatType.Auto)
                {
                    return LabelType.None;
                }
                else
                {
                    return this.labelType;
                }
            }
        }

        [Browsable(false)]
        public AxisLabelFormatPattern AxisLabelPattern
        {
            get 
            { 
                return this.axisLabelFormatPattern; 
            }
            set 
            { 
                this.axisLabelFormatPattern = value;
                if (value != AxisLabelFormatPattern.Custom)
                {
                    DoFormatChange();
                }
            }
        }

        [Browsable(false)]
        public SeriesLabelFormatPattern SeriesLabelPattern
        {
            get
            {
                return this.seriesLabelFormatPattern;
            }
            set
            {
                this.seriesLabelFormatPattern = value;
                if (value != SeriesLabelFormatPattern.Custom)
                {
                    DoFormatChange();
                }
            }
        }

        [Browsable(false)]
        public LegendFormatPattern LegendPattern
        {
            get 
            { 
                return this.legendFormatPattern; 
            }
            set 
            { 
                this.legendFormatPattern = value;
                if (value != LegendFormatPattern.Custom)
                {
                    DoFormatChange();
                }

            }
        }

        [Browsable(false)]
        public TooltipFormatPattern TooltipPattern
        {
            get 
            {
                return this.tooltipFormatPattern; 
            }
            set 
            { 
                this.tooltipFormatPattern = value;
                if (value != TooltipFormatPattern.Custom)
                {
                    DoFormatChange();
                }

            }
        }

        [Browsable(false)]
        public PieLabelFormatPattern PieLabelPattern
        {
            get 
            { 
                return this.pieLabelFormatPattern; 
            }
            set 
            { 
                this.pieLabelFormatPattern = value;
                if (value != PieLabelFormatPattern.Custom)
                {
                    DoFormatChange();
                }

            }
        }

        [Browsable(false)]
        public ChartType ChartType
        {
            get { return this.chart.ChartType; }
        }

        [Browsable(false)]
        public bool DisplayBubbleValueType
        {
            get
            {
                return ((this.chart.ChartType == ChartType.BubbleChart) && (this.labelType == LabelType.Tooltip));
            }
        }

        [Browsable(false)]
        public bool DisplayScatterValueType
        {
            get
            {
                return (((this.chart.ChartType == ChartType.ScatterChart) ||
                        (this.chart.ChartType == ChartType.ScatterLineChart)||
                        (this.chart.ChartType == ChartType.ProbabilityChart)) &&
                        (this.labelType == LabelType.Tooltip));
            }
        }

        [Browsable(false)]
        public bool DisplayPolarValueType
        {
            get
            {
                return ((this.chart.ChartType == ChartType.PolarChart) && (this.labelType == LabelType.Tooltip));
            }
        }

        [Browsable(false)]
        public bool DisplayHeatMapValueType
        {
            get
            {
                return ((this.chart.ChartType == ChartType.HeatMapChart) && (this.labelType == LabelType.Tooltip));
            }
        }

        [Browsable(false)]
        public bool DisplayTreeMapValueType
        {
            get
            {
                return ((this.chart.ChartType == ChartType.TreeMapChart) && (this.labelType == LabelType.Tooltip));
            }
        }

        [Browsable(false)]
        public bool DisplayBoxValueType
        {
            get
            {
                return ((this.chart.ChartType == ChartType.BoxChart) && (this.labelType == LabelType.Tooltip));
            }
        }

        [Category("Формат")]
        [DisplayName("Показатель")]
        [Description("Показатель")]
        [TypeConverter(typeof(EnumTypeConverter))]
        [DynamicPropertyFilter("DisplayBubbleValueType", "True")]
        [Browsable(true)]
        public BubbleChartDataValueType BubbleValueType
        {
            get
            {
                return this.bubbleValueType;
            }
            set
            {
                this.bubbleValueType = value;
                DoFormatChange();
            }
        }

        [Category("Формат")]
        [DisplayName("Показатель")]
        [Description("Показатель")]
        [TypeConverter(typeof(EnumTypeConverter))]
        [DynamicPropertyFilter("DisplayScatterValueType", "True")]
        [Browsable(true)]
        public ScatterChartDataValueType ScatterValueType
        {
            get
            {
                return this.scatterValueType;
            }
            set
            {
                this.scatterValueType = value;
                DoFormatChange();
            }
        }

        [Category("Формат")]
        [DisplayName("Показатель")]
        [Description("Показатель")]
        [TypeConverter(typeof(EnumTypeConverter))]
        [DynamicPropertyFilter("DisplayPolarValueType", "True")]
        [Browsable(true)]
        public PolarChartDataValueType PolarValueType
        {
            get
            {
                return this.polarValueType;
            }
            set
            {
                this.polarValueType = value;
                DoFormatChange();
            }
        }

        [Category("Формат")]
        [DisplayName("Показатель")]
        [Description("Показатель")]
        [TypeConverter(typeof(EnumTypeConverter))]
        [DynamicPropertyFilter("DisplayHeatMapValueType", "True")]
        [Browsable(true)]
        public HeatMapChartDataValueType HeatMapValueType
        {
            get
            {
                return this.heatMapValueType;
            }
            set
            {
                this.heatMapValueType = value;
                DoFormatChange();
            }
        }

        [Category("Формат")]
        [DisplayName("Показатель")]
        [Description("Показатель")]
        [TypeConverter(typeof(EnumTypeConverter))]
        [DynamicPropertyFilter("DisplayTreeMapValueType", "True")]
        [Browsable(true)]
        public TreeMapChartDataValueType TreeMapValueType
        {
            get
            {
                return this.treeMapValueType;
            }
            set
            {
                this.treeMapValueType = value;
                DoFormatChange();
            }
        }

        [Category("Формат")]
        [DisplayName("Показатель")]
        [Description("Показатель")]
        [TypeConverter(typeof(EnumTypeConverter))]
        [DynamicPropertyFilter("DisplayBoxValueType", "True")]
        [Browsable(true)]
        public BoxChartDataValueType BoxValueType
        {
            get
            {
                return this.boxValueType;
            }
            set
            {
                this.boxValueType = value;
                DoFormatChange();
            }
        }


        #endregion


        #region События
        private ValueFormatEventHandler formatChanged = null;

        private static ValueFormatEventHandler formatStringChanged = null;

        public event ValueFormatEventHandler FormatChanged
        {
            add
            {
                formatChanged += value;
            }
            remove
            {
                formatChanged -= value;
            }
        }

        public event ValueFormatEventHandler FormatStringChanged
        {
            add
            {
                formatStringChanged += value;
            }
            remove
            {
                formatStringChanged -= value;
            }
        }


        private void DoFormatChange()
        {
            if ((formatChanged != null)) //&&(FormatType != FormatType.Auto))
            {
                formatChanged();
            }
        }

        public static void DoFormatStringChange()
        {
            if ((formatStringChanged != null)) //&&(FormatType != FormatType.Auto))
            {
                formatStringChanged();
            }
        }

        #endregion

        public ChartFormatBrowseClass(string formatString, LabelType labelType, IChartComponent chart)
        {
            this.chart = chart;

            this.format = new ValueFormat();
            this.labelType = labelType;

            this.FormatString = formatString;
        }

        private byte GetDigitsCount(string formatString)
        {
            byte digits = 0;
            int dIndex = formatString.IndexOf(".0");
            if (dIndex > -1)
            {
                dIndex += 1;
                while ((formatString.Length > dIndex) && (formatString[dIndex] == '0'))
                {
                    dIndex++;
                    digits++;
                }
            }
            return digits;
        }

        private string GetDigitsLabel(byte digits)
        {
            string result = "";

            for (int i = 0; i < digits; i++)
            {
                result += "0";
            }

            if (result != "")
            {
                result = "." + result;
            }
            else
            {
                result = "";
            }

            return result;
        }

        private bool GetDisplayUnits(string formatString)
        {
            return (formatString.Contains("\\р\\.") || formatString.Contains("%"));
        }

        private void SetDisplayUnits(string formatStr)
        {
            switch (this.FormatType)
            {
                case FormatType.Currency:
                case FormatType.ThousandsCurrency:
                case FormatType.ThousandsCurrencyWitoutDivision:
                case FormatType.MillionsCurrency:
                case FormatType.MillionsCurrencyWitoutDivision:
                case FormatType.MilliardsCurrency:
                case FormatType.MilliardsCurrencyWitoutDivision:
                case FormatType.Percent:
                    this.displayUnits = GetDisplayUnits(formatStr);
                    break;
                default:
                    this.displayUnits = true;
                    break;
            }

        }

        private const string formatLabel = ":";

        /// <summary>
        /// Получение начала подстроки с маской формата
        /// </summary>
        /// <param name="formatString">строка формата</param>
        /// <returns>начальный индекс подстроки маски формата, если маска найдена, иначе = -1</returns>
        private int GetBeginFormatMask(string formatString)
        {
            int maskBegin = formatString.IndexOf(formatLabel);

            if (maskBegin > -1)
            {
                maskBegin += formatLabel.Length;
            }
            return maskBegin;
        }

        private string GetMaskKey(string formatString)
        {
            string key = formatString;

            key = key.Replace("<", "");
            key = key.Replace(">", "");

            int maskBegin = GetBeginFormatMask(key);
            if (maskBegin > 0)
            {
                key = key.Substring(0, maskBegin - 1);
            }

            if (GetValueTypeFromFormatString(key) != "")
            {
                key = key.Replace(GetValueTypeFromFormatString(key), "");
            }

            return key;
        }

        private string GetValueTypeFromFormatString(string formatString)
        {
            int beginValueType = formatString.IndexOf("[");
            int endValueType = formatString.IndexOf("]");

            if ((beginValueType > -1) && (endValueType > -1))
            {
                return formatString.Substring(beginValueType, endValueType - beginValueType + 1);
            }
            return "";
        }

        private string GetMask(string digitsLabel, string unit, bool useThousandDelimiter, bool displayUnits)
        {
            string result = digitsLabel;
            if (useThousandDelimiter)
            {
                result = this.thousandDelimiter + result;
            }
            if (displayUnits)
            {
                result += unit;
            }
            return result;
        }


        #region Тип показателя
        
        /// <summary>
        /// Получение типа показателя, который редактируем, в виде строки
        /// </summary>
        /// <param name="useMask">будет использоваться маска в строке формата или нет</param>
        /// <returns>строковый модификатор типа показателя</returns>
        private string GetDataTypeStr(bool useMask)
        {
            string result = "";

            switch (this.ChartType)
            {
                case ChartType.BubbleChart:
                    result = GetBubbleValueTypeStr();
                    break;
                case ChartType.ScatterChart:
                    result = GetScatterValueTypeStr();
                    break;
                case ChartType.PolarChart:
                    result = GetPolarValueTypeStr();
                    break;
                case ChartType.HeatMapChart:
                    result = GetHeatMapValueTypeStr();
                    break;
                case ChartType.TreeMapChart:
                    result = GetTreeMapValueTypeStr();
                    break;
                case ChartType.BoxChart:
                    result = GetBoxValueTypeStr();
                    break;
            }
            return (useMask || (result != "")) ? ":" + result : result;
        }


        private string GetBubbleValueTypeStr()
        {
            switch (this.BubbleValueType)
            {
                case BubbleChartDataValueType.DataValue:
                    return "";
                case BubbleChartDataValueType.DataValueRadius:
                    return "[RADIUS]";
                case BubbleChartDataValueType.DataValueX:
                    return "[X]";
                case BubbleChartDataValueType.DataValueY:
                    return "[Y]";
            }
            return "";
        }

        private string GetScatterValueTypeStr()
        {
            switch (this.ScatterValueType)
            {
                case ScatterChartDataValueType.DataValue:
                    return "";
                case ScatterChartDataValueType.DataValueX:
                    return "[X]";
                case ScatterChartDataValueType.DataValueY:
                    return "[Y]";
            }
            return "";
        }

        private string GetPolarValueTypeStr()
        {
            switch (this.PolarValueType)
            {
                case PolarChartDataValueType.DataValue:
                    return "";
                /*case PolarChartDataValueType.DegreesValue:
                    return "[DEGREES]";
                case PolarChartDataValueType.RadiansValue:
                    return "[RADIANS]";*/
                case PolarChartDataValueType.DataValueX:
                    return "[X]";
                case PolarChartDataValueType.DataValueY:
                    return "[Y]";
            }
            return "";
        }

        private string GetHeatMapValueTypeStr()
        {
            switch (this.HeatMapValueType)
            {
                case HeatMapChartDataValueType.DataValue:
                    return "";
                case HeatMapChartDataValueType.DataValueBottomLeft:
                    return "[BOTTOMLEFT]";
                case HeatMapChartDataValueType.DataValueBottomRight:
                    return "[BOTTOMRIGHT]";
                case HeatMapChartDataValueType.DataValueTopLeft:
                    return "[TOPLEFT]";
                case HeatMapChartDataValueType.DataValueTopRight:
                    return "[TOPRIGHT]";
            }
            return "";
        }

        private string GetTreeMapValueTypeStr()
        {
            switch (this.TreeMapValueType)
            {
                case TreeMapChartDataValueType.DataValue:
                    return "";
                case TreeMapChartDataValueType.SizeLabel:
                    return "[SIZE_VALUE:00.##]";
                case TreeMapChartDataValueType.ColorLabel:
                    return "[COLOR_VALUE:00.##]";
                case TreeMapChartDataValueType.SizeAndColorLabel:
                    return "[SIZE_VALUE:00.##] \n [COLOR_VALUE:00.##]";
            }
            return "";
        }

        private string GetBoxValueTypeStr()
        {
            switch (this.BoxValueType)
            {
                case BoxChartDataValueType.DataValue:
                    return "";
                case BoxChartDataValueType.DataValueMax:
                    return "[MAX]";
                case BoxChartDataValueType.DataValueMin:
                    return "[MIN]";
                case BoxChartDataValueType.DataValueQ1:
                    return "[Q1]";
                case BoxChartDataValueType.DataValueQ2:
                    return "[Q2]";
                case BoxChartDataValueType.DataValueQ3:
                    return "[Q3]";
            }
            return "";
        }


        private void SetDataValueType(string formatString)
        {
            string dataTypeStr = GetValueTypeFromFormatString(formatString);

            switch (this.ChartType)
            {
                case ChartType.BubbleChart:
                    SetBubbleValueType(dataTypeStr);
                    return;
                case ChartType.ScatterChart:
                    SetScatterValueTypeStr(dataTypeStr);
                    return;
                case ChartType.PolarChart:
                    SetPolarValueTypeStr(dataTypeStr);
                    return;
                case ChartType.HeatMapChart:
                    SetHeatMapValueTypeStr(dataTypeStr);
                    return;
                case ChartType.TreeMapChart:
                    SetTreeMapValueTypeStr(dataTypeStr);
                    return;
                case ChartType.BoxChart:
                    SetBoxValueTypeStr(dataTypeStr);
                    return;
            }

        }

        private void SetBubbleValueType(string dataTypeStr)
        {
            switch (dataTypeStr)
            {
                case "":
                    BubbleValueType = BubbleChartDataValueType.DataValue;
                    return;
                case "[RADIUS]":
                    BubbleValueType = BubbleChartDataValueType.DataValueRadius;
                    return;
                case "[X]":
                    BubbleValueType = BubbleChartDataValueType.DataValueX;
                    return;
                case "[Y]":
                    BubbleValueType = BubbleChartDataValueType.DataValueY;
                    return;
            }
        }

        private void SetScatterValueTypeStr(string dataTypeStr)
        {
            switch (dataTypeStr)
            {
                case "":
                    ScatterValueType = ScatterChartDataValueType.DataValue;
                    return;
                case "[X]":
                    ScatterValueType = ScatterChartDataValueType.DataValueX;
                    return;
                case "[Y]":
                    ScatterValueType = ScatterChartDataValueType.DataValueY;
                    return;
            }
        }

        private void SetPolarValueTypeStr(string dataTypeStr)
        {
            switch (dataTypeStr)
            {
                case "":
                    PolarValueType = PolarChartDataValueType.DataValue;
                    return;
               /* case "[DEGREES]":
                    PolarValueType = PolarChartDataValueType.DegreesValue;
                    return;
                case "[RADIANS]":
                    PolarValueType = PolarChartDataValueType.RadiansValue;
                    return;*/
                case "[X]":
                    PolarValueType = PolarChartDataValueType.DataValueX;
                    return;
                case "[Y]":
                    PolarValueType = PolarChartDataValueType.DataValueY;
                    return;
            }
        }

        private void SetHeatMapValueTypeStr(string dataTypeStr)
        {
             switch (dataTypeStr)
            {
                case "":
                    HeatMapValueType = HeatMapChartDataValueType.DataValue;
                    return;
                case "[BOTTOMLEFT]":
                    HeatMapValueType = HeatMapChartDataValueType.DataValueBottomLeft;
                    return;
                case "[BOTTOMRIGHT]":
                    HeatMapValueType = HeatMapChartDataValueType.DataValueBottomRight;
                    return;
                case "[TOPLEFT]":
                    HeatMapValueType = HeatMapChartDataValueType.DataValueTopLeft;
                    return;
                case "[TOPRIGHT]":
                    HeatMapValueType = HeatMapChartDataValueType.DataValueTopRight;
                    return;

            }
       }

        private void SetTreeMapValueTypeStr(string dataTypeStr)
        {
            switch (dataTypeStr)
            {
                case "":
                    TreeMapValueType = TreeMapChartDataValueType.DataValue;
                    return;
                case "[SIZE_VALUE:00.##]":
                    TreeMapValueType = TreeMapChartDataValueType.SizeLabel;
                    return;
                case "[COLOR_VALUE:00.##]":
                    TreeMapValueType = TreeMapChartDataValueType.ColorLabel;
                    return;
                case "[SIZE_VALUE:00.##] \n [COLOR_VALUE:00.##]":
                    TreeMapValueType = TreeMapChartDataValueType.SizeAndColorLabel;
                    return;
            }
        }

        private void SetBoxValueTypeStr(string dataTypeStr)
        {
            switch (dataTypeStr)
            {
                case "":
                    BoxValueType = BoxChartDataValueType.DataValue;
                    return;
                case "[MAX]":
                    BoxValueType = BoxChartDataValueType.DataValueMax;
                    return;
                case "[MIN]":
                    BoxValueType = BoxChartDataValueType.DataValueMin;
                    return;
                case "[Q1]":
                    BoxValueType = BoxChartDataValueType.DataValueQ1;
                    return;
                case "[Q2]":
                    BoxValueType = BoxChartDataValueType.DataValueQ2;
                    return;
                case "[Q3]":
                    BoxValueType = BoxChartDataValueType.DataValueQ3;
                    return;
            }

        }


        #endregion

        #region Автоформат

        private string GetAutoFormatStr()
        {
            string result = "<DATA_VALUE:00.##>";

            switch (this.ChartType)
            {
                case ChartType.BubbleChart:
                    result = GetBubbleChartAutoFormat();
                    break;
                case ChartType.ScatterChart:
                    result = GetScatterChartAutoFormat();
                    break;
                case ChartType.PolarChart:
                    result = GetPolarChartAutoFormat();
                    break;
                case ChartType.HeatMapChart:
                    result = GetHeatMapChartAutoFormat();
                    break;
                case ChartType.TreeMapChart:
                    result = GetTreeMapChartAutoFormat();
                    break;
                case ChartType.BoxChart:
                    result = GetBoxChartAutoFormat();
                    break;
            }
            return result;
        }

        private string GetBubbleChartAutoFormat()
        {
            switch (this.BubbleValueType)
            {
                case BubbleChartDataValueType.DataValue:
                    return "<DATA_VALUE:00.##>";
                case BubbleChartDataValueType.DataValueRadius:
                    return "<DATA_VALUE_RADIUS:00.##>";
                case BubbleChartDataValueType.DataValueX:
                    return "<DATA_VALUE_X:00.##>";
                case BubbleChartDataValueType.DataValueY:
                    return "<DATA_VALUE_Y:00.##>";
            }
            return "";
        }

        private string GetScatterChartAutoFormat()
        {
            switch (this.ScatterValueType)
            {
                case ScatterChartDataValueType.DataValue:
                    return "<DATA_VALUE:00.##>";
                case ScatterChartDataValueType.DataValueX:
                    return "<DATA_VALUE_X:00.##>";
                case ScatterChartDataValueType.DataValueY:
                    return "<DATA_VALUE_Y:00.##>";
            }
            return "";
        }

        private string GetPolarChartAutoFormat()
        {
            switch (this.PolarValueType)
            {
                case PolarChartDataValueType.DataValue:
                    return "<DATA_VALUE:00.##>";
                /*case PolarChartDataValueType.DegreesValue:
                    return "[DEGREES]";
                case PolarChartDataValueType.RadiansValue:
                    return "[RADIANS]";*/
                case PolarChartDataValueType.DataValueX:
                    return "<DATA_VALUE_X:00.##>";
                case PolarChartDataValueType.DataValueY:
                    return "<DATA_VALUE_Y:00.##>";
            }
            return "";
        }

        private string GetHeatMapChartAutoFormat()
        {
            switch (this.HeatMapValueType)
            {
                case HeatMapChartDataValueType.DataValue:
                    return "<DATA_VALUE:00.##>";
                case HeatMapChartDataValueType.DataValueBottomLeft:
                    return "<DATA_VALUE_BOTTOMLEFT:00.##>";
                case HeatMapChartDataValueType.DataValueBottomRight:
                    return "<DATA_VALUE_BOTTOMRIGHT:00.##>";
                case HeatMapChartDataValueType.DataValueTopLeft:
                    return "<DATA_VALUE_TOPLEFT:00.##>";
                case HeatMapChartDataValueType.DataValueTopRight:
                    return "<DATA_VALUE_TOPRIGHT:00.##>";
            }
            return "";
        }

        private string GetTreeMapChartAutoFormat()
        {
            switch (this.TreeMapValueType)
            {
                case TreeMapChartDataValueType.DataValue:
                    return "<DATA_VALUE:00.##>";
                case TreeMapChartDataValueType.SizeLabel:
                    return "<SIZE_VALUE_LABEL> : <SIZE_VALUE:0.##>";
                case TreeMapChartDataValueType.ColorLabel:
                    return "<COLOR_VALUE_LABEL> : <COLOR_VALUE:0.##>";
                case TreeMapChartDataValueType.SizeAndColorLabel:
                    return "<SIZE_VALUE_LABEL>: <SIZE_VALUE:0.##>\n<COLOR_VALUE_LABEL>: <COLOR_VALUE:0.##>";
            }
            return "";
        }

        private string GetBoxChartAutoFormat()
        {
            switch (this.BoxValueType)
            {
                case BoxChartDataValueType.DataValue:
                    return "<DATA_VALUE:00.##>";
                case BoxChartDataValueType.DataValueMax:
                    return "<DATA_VALUE_MAX:00.##>";
                case BoxChartDataValueType.DataValueMin:
                    return "<DATA_VALUE_MIN:00.##>";
                case BoxChartDataValueType.DataValueQ1:
                    return "<DATA_VALUE_Q1:00.##>";
                case BoxChartDataValueType.DataValueQ2:
                    return "<DATA_VALUE_Q2:00.##>";
                case BoxChartDataValueType.DataValueQ3:
                    return "<DATA_VALUE_Q3:00.##>";
            }
            return "";
        }


        private void SetAutoFormatStr(string formatStr)
        {
            switch (this.ChartType)
            {
                case ChartType.BubbleChart:
                    SetBubbleChartAutoFormat(formatStr);
                    break;
                case ChartType.ScatterChart:
                    SetScatterChartAutoFormat(formatStr);
                    break;
                case ChartType.PolarChart:
                    SetPolarChartAutoFormat(formatStr);
                    break;
                case ChartType.HeatMapChart:
                    SetHeatMapChartAutoFormat(formatStr);
                    break;
                case ChartType.TreeMapChart:
                    SetTreeMapChartAutoFormat(formatStr);
                    break;
                case ChartType.BoxChart:
                    SetBoxChartAutoFormat(formatStr);
                    break;
            }
        }

        private void SetBubbleChartAutoFormat(string formatStr)
        {
            switch (formatStr)
            {
                case "<DATA_VALUE:00.##>":
                    BubbleValueType = BubbleChartDataValueType.DataValue;
                    return;
                case "<DATA_VALUE_RADIUS:00.##>":
                    BubbleValueType = BubbleChartDataValueType.DataValueRadius;
                    return;
                case "<DATA_VALUE_X:00.##>":
                    BubbleValueType = BubbleChartDataValueType.DataValueX;
                    return;
                case "<DATA_VALUE_Y:00.##>":
                    BubbleValueType = BubbleChartDataValueType.DataValueY;
                    return;
            }
        }

        private void SetScatterChartAutoFormat(string formatStr)
        {
            switch (formatStr)
            {
                case "<DATA_VALUE:00.##>":
                    ScatterValueType = ScatterChartDataValueType.DataValue;
                    return;
                case "<DATA_VALUE_X:00.##>":
                    ScatterValueType = ScatterChartDataValueType.DataValueX;
                    return;
                case "<DATA_VALUE_Y:00.##>":
                    ScatterValueType = ScatterChartDataValueType.DataValueY;
                    return;
            }
        }

        private void SetPolarChartAutoFormat(string formatStr)
        {
            switch (formatStr)
            {
                case "<DATA_VALUE:00.##>":
                    PolarValueType = PolarChartDataValueType.DataValue;
                    return;
                /*case PolarChartDataValueType.DegreesValue:
                    return "[DEGREES]";
                case PolarChartDataValueType.RadiansValue:
                    return "[RADIANS]";*/
                case "<DATA_VALUE_X:00.##>":
                    PolarValueType = PolarChartDataValueType.DataValueX;
                    return;
                case "<DATA_VALUE_Y:00.##>":
                    PolarValueType = PolarChartDataValueType.DataValueY;
                    return;
            }
        }

        private void SetHeatMapChartAutoFormat(string formatStr)
        {
            switch (formatStr)
            {
                case "<DATA_VALUE:00.##>":
                    HeatMapValueType = HeatMapChartDataValueType.DataValue;
                    return;
                case "<DATA_VALUE_BOTTOMLEFT:00.##>":
                    HeatMapValueType = HeatMapChartDataValueType.DataValueBottomLeft;
                    return;
                case "<DATA_VALUE_BOTTOMRIGHT:00.##>":
                    HeatMapValueType = HeatMapChartDataValueType.DataValueBottomRight;
                    return;
                case "<DATA_VALUE_TOPLEFT:00.##>":
                    HeatMapValueType = HeatMapChartDataValueType.DataValueTopLeft;
                    return;
                case "<DATA_VALUE_TOPRIGHT:00.##>":
                    HeatMapValueType = HeatMapChartDataValueType.DataValueTopRight;
                    return;
            }
        }

        private void SetTreeMapChartAutoFormat(string formatStr)
        {
            switch (formatStr)
            {
                case "<DATA_VALUE>":
                    TreeMapValueType = TreeMapChartDataValueType.DataValue;
                    return;
                case "<SIZE_VALUE_LABEL> : <SIZE_VALUE:0.##>":
                    TreeMapValueType = TreeMapChartDataValueType.SizeLabel;
                    return;
                case "<COLOR_VALUE_LABEL> : <COLOR_VALUE:0.##>":
                    TreeMapValueType = TreeMapChartDataValueType.ColorLabel;
                    return;
                case "<SIZE_VALUE_LABEL>: <SIZE_VALUE:0.##>\n<COLOR_VALUE_LABEL>: <COLOR_VALUE:0.##>":
                    TreeMapValueType = TreeMapChartDataValueType.SizeAndColorLabel;
                    return;
            }
        }

        private void SetBoxChartAutoFormat(string formatStr)
        {
            switch (formatStr)
            {
                case "<DATA_VALUE:00.##>":
                    BoxValueType = BoxChartDataValueType.DataValue;
                    return;
                case "<DATA_VALUE_MAX:00.##>":
                    BoxValueType = BoxChartDataValueType.DataValueMax;
                    return;
                case "<DATA_VALUE_MIN:00.##>":
                    BoxValueType = BoxChartDataValueType.DataValueMin;
                    return;
                case "<DATA_VALUE_Q1:00.##>":
                    BoxValueType = BoxChartDataValueType.DataValueQ1;
                    return;
                case "<DATA_VALUE_Q2:00.##>":
                    BoxValueType = BoxChartDataValueType.DataValueQ2;
                    return;
                case "<DATA_VALUE_Q3:00.##>":
                    BoxValueType = BoxChartDataValueType.DataValueQ3;
                    return;
            }
        }


        #endregion

        private static string GetSizeColorValueStr(string formatName, string maskStr)
        {
            return "<SIZE_VALUE_LABEL> : <" + formatName + ":[SIZE_VALUE:0.##]" + maskStr + "\n" +
                   "<COLOR_VALUE_LABEL> : <" + formatName + ":[COLOR_VALUE:0.##]" + maskStr;
        }

        private string GetTreeMapFormatStr(FormatType formatType, byte digits, bool useThousandDelimiter, bool displayUnits)
        {
            switch (formatType)
            {
                case FormatType.Auto:
                    return GetAutoFormatStr();
                case FormatType.DateTime:
                    return GetSizeColorValueStr("DATE_TIME", ">");
                case FormatType.Exponential:
                    return GetSizeColorValueStr("EXP", " 0E+00>");
                case FormatType.LongDate:
                    return GetSizeColorValueStr("LONG_DATE", ">");
                case FormatType.LongTime:
                    return GetSizeColorValueStr("LONG_TIME", ">");
                case FormatType.Percent:
                    if (displayUnits)
                    {
                        return GetSizeColorValueStr("PERCENT", GetMask("0" + GetDigitsLabel(digits), "%", useThousandDelimiter, displayUnits) + ">");
                    }
                    else
                    {
                        return GetSizeColorValueStr("PERCENT2", GetMask("0" + GetDigitsLabel(digits), "%", useThousandDelimiter, displayUnits) + ">");
                    }
                case FormatType.ShortDate:
                    return GetSizeColorValueStr("SHORT_DATE", ">");
                case FormatType.ShortTime:
                    return GetSizeColorValueStr("SHORT_TIME", ">");
                case FormatType.TrueFalse:
                    return GetSizeColorValueStr("TRUEFALSE", ">");
                case FormatType.YesNo:
                    return GetSizeColorValueStr("YESNO", ">");
                case FormatType.None:
                    return GetSizeColorValueStr("GENERAL", ">");
                case FormatType.Currency:
                    return GetSizeColorValueStr("CRR", GetMask("0" + GetDigitsLabel(digits), @" \р\.", useThousandDelimiter, displayUnits) + ">");
                case FormatType.MilliardsCurrency:
                    return GetSizeColorValueStr("MLRD_CRR", GetMask("0,,," + GetDigitsLabel(digits), @" \м\л\р\д\.\р\.", useThousandDelimiter, displayUnits) + ">");
                case FormatType.MilliardsCurrencyWitoutDivision:
                    return GetSizeColorValueStr("MLRD_CRR_ND", GetMask("0" + GetDigitsLabel(digits), @" \м\л\р\д\.\р\.", useThousandDelimiter, displayUnits) + ">");
                case FormatType.MilliardsNumeric:
                    return GetSizeColorValueStr("MLRD_NUM", GetMask("0,,," + GetDigitsLabel(digits), "", useThousandDelimiter, displayUnits) + ">");
                case FormatType.MillionsCurrency:
                    return GetSizeColorValueStr("MLN_CRR", GetMask("0,," + GetDigitsLabel(digits), @" \м\л\н\.\р\.", useThousandDelimiter, displayUnits) + ">");
                case FormatType.MillionsCurrencyWitoutDivision:
                    return GetSizeColorValueStr("MLN_CRR_ND", GetMask("0" + GetDigitsLabel(digits), @" \м\л\н\.\р\.", useThousandDelimiter, displayUnits) + ">");
                case FormatType.MillionsNumeric:
                    return GetSizeColorValueStr("MLN_NUM", GetMask("0,," + GetDigitsLabel(digits), "", useThousandDelimiter, displayUnits) + ">");
                case FormatType.ThousandsCurrency:
                    return GetSizeColorValueStr("THS_CRR", GetMask("0," + GetDigitsLabel(digits), @" \т\ы\с\.\р\.", useThousandDelimiter, displayUnits) + ">");
                case FormatType.ThousandsCurrencyWitoutDivision:
                    return GetSizeColorValueStr("THS_CRR_ND", GetMask("0" + GetDigitsLabel(digits), @" \т\ы\с\.\р\.", useThousandDelimiter, displayUnits) + ">");
                case FormatType.ThousandsNumeric:
                    return GetSizeColorValueStr("THS_NUM", GetMask("0," + GetDigitsLabel(digits), "", useThousandDelimiter, displayUnits) + ">");
                case FormatType.Numeric:
                    return GetSizeColorValueStr("NUM", GetMask("0" + GetDigitsLabel(digits), "", useThousandDelimiter, displayUnits) + ">");
            }

            return "";
        }

        private string GetFormatStr(FormatType formatType, byte digits, bool useThousandDelimiter, bool displayUnits)
        {
            if (chart.ChartType == ChartType.TreeMapChart && treeMapValueType == TreeMapChartDataValueType.SizeAndColorLabel)
            {
                return GetTreeMapFormatStr(formatType, digits, useThousandDelimiter, displayUnits);
            }
            {
                switch (formatType)
                {
                    case FormatType.Auto:
                        return GetAutoFormatStr();
                    case FormatType.DateTime:
                        return "<DATE_TIME" + GetDataTypeStr(false) + ">";
                    case FormatType.Exponential:
                        return "<EXP" + GetDataTypeStr(true) + "0E+00>";
                    case FormatType.LongDate:
                        return "<LONG_DATE" + GetDataTypeStr(false) + ">";
                    case FormatType.LongTime:
                        return "<LONG_TIME" + GetDataTypeStr(false) + ">";
                    case FormatType.Percent:
                        if (displayUnits)
                        {
                            return "<PERCENT" + GetDataTypeStr(true) + GetMask("0" + GetDigitsLabel(digits), "%", useThousandDelimiter, displayUnits) + ">";
                        }
                        else
                        {
                            return "<PERCENT2" + GetDataTypeStr(true) + GetMask("0" + GetDigitsLabel(digits), "", useThousandDelimiter, displayUnits) + ">";
                        }
                    case FormatType.ShortDate:
                        return "<SHORT_DATE" + GetDataTypeStr(false) + ">";
                    case FormatType.ShortTime:
                        return "<SHORT_TIME" + GetDataTypeStr(false) + ">";
                    case FormatType.TrueFalse:
                        return "<TRUEFALSE" + GetDataTypeStr(false) + ">";
                    case FormatType.YesNo:
                        return "<YESNO" + GetDataTypeStr(false) + ">";
                    case FormatType.None:
                        return "<GENERAL" + GetDataTypeStr(false) + ">";
                    case FormatType.Currency:
                        return "<CRR" + GetDataTypeStr(true) + GetMask("0" + GetDigitsLabel(digits), @" \р\.", useThousandDelimiter, displayUnits) + ">";
                    case FormatType.MilliardsCurrency:
                        return "<MLRD_CRR" + GetDataTypeStr(true) + GetMask("0,,," + GetDigitsLabel(digits), @" \м\л\р\д\.\р\.", useThousandDelimiter, displayUnits) + ">";
                    case FormatType.MilliardsCurrencyWitoutDivision:
                        return "<MLRD_CRR_ND" + GetDataTypeStr(true) + GetMask("0" + GetDigitsLabel(digits), @" \м\л\р\д\.\р\.", useThousandDelimiter, displayUnits) + ">";
                    case FormatType.MilliardsNumeric:
                        return "<MLRD_NUM" + GetDataTypeStr(true) + GetMask("0,,," + GetDigitsLabel(digits), "", useThousandDelimiter, displayUnits) + ">";
                    case FormatType.MillionsCurrency:
                        return "<MLN_CRR" + GetDataTypeStr(true) + GetMask("0,," + GetDigitsLabel(digits), @" \м\л\н\.\р\.", useThousandDelimiter, displayUnits) + ">";
                    case FormatType.MillionsCurrencyWitoutDivision:
                        return "<MLN_CRR_ND" + GetDataTypeStr(true) + GetMask("0" + GetDigitsLabel(digits), @" \м\л\н\.\р\.", useThousandDelimiter, displayUnits) + ">";
                    case FormatType.MillionsNumeric:
                        return "<MLN_NUM" + GetDataTypeStr(true) + GetMask("0,," + GetDigitsLabel(digits), "", useThousandDelimiter, displayUnits) + ">";
                    case FormatType.ThousandsCurrency:
                        return "<THS_CRR" + GetDataTypeStr(true) + GetMask("0," + GetDigitsLabel(digits), @" \т\ы\с\.\р\.", useThousandDelimiter, displayUnits) + ">";
                    case FormatType.ThousandsCurrencyWitoutDivision:
                        return "<THS_CRR_ND" + GetDataTypeStr(true) + GetMask("0" + GetDigitsLabel(digits), @" \т\ы\с\.\р\.", useThousandDelimiter, displayUnits) + ">";
                    case FormatType.ThousandsNumeric:
                        return "<THS_NUM" + GetDataTypeStr(true) + GetMask("0," + GetDigitsLabel(digits), "", useThousandDelimiter, displayUnits) + ">";
                    case FormatType.Numeric:
                        return "<NUM" + GetDataTypeStr(true) + GetMask("0" + GetDigitsLabel(digits), "", useThousandDelimiter, displayUnits) + ">";
                }
            }

            return "";
        }

        /// <summary>
        /// Получение типа формата по маске
        /// </summary>
        /// <param name="key">маска формата</param>
        /// <returns>тип формата</returns>
        private FormatType GetFormatTypeByMaskKey(string key)
        {
            switch (key)
            {
                case "DATE_TIME":
                    return FormatType.DateTime;
                case "EXP":
                    return FormatType.Exponential;
                case "LONG_DATE":
                    return FormatType.LongDate;
                case "LONG_TIME":
                    return FormatType.LongTime;
                case "SHORT_DATE":
                    return FormatType.ShortDate;
                case "SHORT_TIME":
                    return FormatType.ShortTime;
                case "TRUEFALSE":
                    return FormatType.TrueFalse;
                case "YESNO":
                    return FormatType.YesNo;
                case "PERCENT":
                case "PERCENT2":
                    return FormatType.Percent;
                case "CRR":
                    return FormatType.Currency;
                case "THS_CRR":
                    return FormatType.ThousandsCurrency;
                case "THS_CRR_ND":
                    return FormatType.ThousandsCurrencyWitoutDivision;
                case "MLN_CRR":
                    return FormatType.MillionsCurrency;
                case "MLN_CRR_ND":
                    return FormatType.MillionsCurrencyWitoutDivision;
                case "MLRD_CRR":
                    return FormatType.MilliardsCurrency;
                case "MLRD_CRR_ND":
                    return FormatType.MilliardsCurrencyWitoutDivision;
                case "NUM":
                    return FormatType.Numeric;
                case "THS_NUM":
                    return FormatType.ThousandsNumeric;
                case "MLN_NUM":
                    return FormatType.MillionsNumeric;
                case "MLRD_NUM":
                    return FormatType.MilliardsNumeric;
                case "GENERAL":
                    return FormatType.None;
                

            }

            return FormatType.Auto;

        }

        /// <summary>
        /// Получение типа формата по строке формата
        /// </summary>
        /// <param name="formatString">строка формата</param>
        /// <returns>тип формата</returns>
        private FormatType GetFormatTypeByFormatString(string formatString)
        {
            return GetFormatTypeByMaskKey(GetMaskKey(formatString));
        }
      

        private string GetFormatString(string formatStr, FormatType formatType, byte digits, bool useThousandDelimiter, bool displayUnits)
        {
            return GetFormatStr(formatType, digits, useThousandDelimiter, displayUnits);
        }


        #region добавление шаблона 

        private string GetFormatStringWithPattern(string formatStr)
        {
            switch (this.labelType)
            {
                case LabelType.AxisLabel:
                    return GetFormatStringWithAxisPattern(formatStr);
                case LabelType.SeriesLabel:
                    return GetFormatStringWithSeriesPattern(formatStr);
                case LabelType.Legend:
                    return GetFormatStringWithLegendPattern(formatStr);
                case LabelType.PieLabel:
                    return GetFormatStringWithPieLabelPattern(formatStr);
                case LabelType.Tooltip:
                    return GetFormatStringWithTooltipPattern(formatStr);

            }
            return formatStr;
        }

        private string GetFormatStringWithAxisPattern(string formatStr)
        {
            switch (this.AxisLabelPattern)
            {
                case AxisLabelFormatPattern.None:
                    return "";
                case AxisLabelFormatPattern.DataValue:
                    return formatStr;
                case AxisLabelFormatPattern.ItemLabel:
                    return "<ITEM_LABEL>";
                case AxisLabelFormatPattern.LabelAndData:
                    return "<ITEM_LABEL> (" + formatStr + ")";
            }
            return formatStr;
        }

        private string GetFormatStringWithSeriesPattern(string formatStr)
        {
            switch (this.SeriesLabelPattern)
            {
                case SeriesLabelFormatPattern.None:
                    return "";
                case SeriesLabelFormatPattern.DataValue:
                    return formatStr;
                case SeriesLabelFormatPattern.SeriesLabel:
                    return "<SERIES_LABEL>";
                case SeriesLabelFormatPattern.SeriesLabelAndData:
                    return "<SERIES_LABEL> (" + formatStr + ")";
            }
            return formatStr;
        }

        private string GetFormatStringWithLegendPattern(string formatStr)
        {
            switch (this.LegendPattern)
            {
                case LegendFormatPattern.None:
                    return "";
                case LegendFormatPattern.ItemLabel:
                    return "<ITEM_LABEL>";
                case LegendFormatPattern.LabelAndData:
                    return "<ITEM_LABEL> (" + formatStr + ")";
            }
            return formatStr;
        }

        private string GetFormatStringWithTooltipPattern(string formatStr)
        {
            switch (this.TooltipPattern)
            {
                case TooltipFormatPattern.None:
                    return "";
                case TooltipFormatPattern.DataValue:
                    return formatStr;
                case TooltipFormatPattern.LabelAndData:
                    return "<ITEM_LABEL> (" + formatStr + ")";
                case TooltipFormatPattern.RowColumnAndData:
                    return "<DATA_ROW>, <DATA_COLUMN>: " + formatStr;
            }
            return formatStr;
        }

        private string GetFormatStringWithPieLabelPattern(string formatStr)
        {
            switch (this.PieLabelPattern)
            {
                case PieLabelFormatPattern.None:
                    return "";
                case PieLabelFormatPattern.DataValue:
                    return formatStr;
                case PieLabelFormatPattern.ItemLabel:
                    return "<ITEM_LABEL>";
                case PieLabelFormatPattern.LabelAndPercentValue:
                    return "<ITEM_LABEL>\n<PERCENT_VALUE:#0.00>%";
                case PieLabelFormatPattern.PercentValue:
                    return "<PERCENT_VALUE:#0.00>%";
            }
            return formatStr;
        }


        private void SetPattern(string formatStr)
        {
            switch (this.labelType)
            {
                case LabelType.AxisLabel:
                    SetAxisPattern(formatStr);
                    break;
                case LabelType.SeriesLabel:
                    SetSeriesPattern(formatStr);
                    break;
                case LabelType.Legend:
                    SetLegendPattern(formatStr);
                    break;
                case LabelType.PieLabel:
                    SetPieLabelPattern(formatStr);
                    break;
                case LabelType.Tooltip:
                    SetTooltipPattern(formatStr);
                    break;
            }

        }

        private string GetPatternStr(string formatStr)
        {
            foreach (string pattern in formatPatterns.Keys)
            {
                Regex regExp = new Regex("^" + pattern + "$");
                if (regExp.Match(formatStr).Success)
                {
                    return formatPatterns[pattern];
                }
            }
            return "";
        }

        private void SetAxisPattern(string formatStr)
        {
            if (formatStr == "")
            {
                this.AxisLabelPattern = AxisLabelFormatPattern.None;
                return;
            }

            switch (GetPatternStr(formatStr))
            {
                case "":
                    this.AxisLabelPattern = AxisLabelFormatPattern.Custom;
                    break;
                case "ItemLabel":
                    this.AxisLabelPattern = AxisLabelFormatPattern.ItemLabel;
                    break;
                case "LabelAndData":
                    this.AxisLabelPattern = AxisLabelFormatPattern.LabelAndData;
                    break;
                case "DataValue":
                    this.AxisLabelPattern = AxisLabelFormatPattern.DataValue;
                    break;
            }
        }

        private void SetSeriesPattern(string formatStr)
        {
            if (formatStr == "")
            {
                this.SeriesLabelPattern = SeriesLabelFormatPattern.None;
                return;
            }

            switch (GetPatternStr(formatStr))
            {
                case "":
                    this.SeriesLabelPattern = SeriesLabelFormatPattern.Custom;
                    break;
                case "SeriesLabel":
                    this.SeriesLabelPattern = SeriesLabelFormatPattern.SeriesLabel;
                    break;
                case "SeriesAndData":
                    this.SeriesLabelPattern = SeriesLabelFormatPattern.SeriesLabelAndData;
                    break;
                case "DataValue":
                    this.SeriesLabelPattern = SeriesLabelFormatPattern.DataValue;
                    break;
            }
        }

        private void SetLegendPattern(string formatStr)
        {
            if (formatStr == "")
            {
                this.LegendPattern = LegendFormatPattern.None;
                return;
            }

            switch (GetPatternStr(formatStr))
            {
                case "":
                    this.LegendPattern = LegendFormatPattern.Custom;
                    break;
                case "ItemLabel":
                    this.LegendPattern = LegendFormatPattern.ItemLabel;
                    break;
                case "LabelAndData":
                    this.LegendPattern = LegendFormatPattern.LabelAndData;
                    break;
            }

        }

        private void SetPieLabelPattern(string formatStr)
        {
            if (formatStr == "")
            {
                this.PieLabelPattern = PieLabelFormatPattern.None;
                return;
            }

            switch (GetPatternStr(formatStr))
            {
                case "":
                    this.PieLabelPattern = PieLabelFormatPattern.Custom;
                    break;
                case "ItemLabel":
                    this.PieLabelPattern = PieLabelFormatPattern.ItemLabel;
                    break;
                case "DataValue":
                    this.PieLabelPattern = PieLabelFormatPattern.DataValue;
                    break;
                case "LabelAndPercentValue":
                    this.PieLabelPattern = PieLabelFormatPattern.LabelAndPercentValue;
                    break;
                case "PercentValue":
                    this.PieLabelPattern = PieLabelFormatPattern.PercentValue;
                    break;
            }

        }

        private void SetTooltipPattern(string formatStr)
        {
            if (formatStr == "")
            {
                this.TooltipPattern = TooltipFormatPattern.None;
                return;
            }

            switch (GetPatternStr(formatStr))
            {
                case "":
                    this.TooltipPattern = TooltipFormatPattern.Custom;
                    break;
                case "DataValue":
                    this.TooltipPattern = TooltipFormatPattern.DataValue;
                    break;
                case "LabelAndData":
                    this.TooltipPattern = TooltipFormatPattern.LabelAndData;
                    break;
                case "RowColumnAndData":
                    this.TooltipPattern = TooltipFormatPattern.RowColumnAndData;
                    break;
            }

        }

        /// <summary>
        /// Получение строки формата без шаблона. Тип шаблона должен быть заранее определен
        /// </summary>
        /// <param name="formatStr">строка формата с шаблоном</param>
        /// <returns>строка формата без шаблона</returns>
        private string GetFormatStrWithoutPattern(string formatStr)
        {
            switch (this.labelType)
            {
                case LabelType.AxisLabel:
                    return GetAxisFormatString(formatStr);
                case LabelType.SeriesLabel:
                    return GetSeriesFormatString(formatStr);
                case LabelType.Legend:
                    return GetLegendFormatString(formatStr);
                case LabelType.PieLabel:
                    return GetPieLabelFormatString(formatStr);
                case LabelType.Tooltip:
                    return GetTooltipFormatString(formatStr);
            }
            return formatStr;

        }

        private string GetAxisFormatString(string formatStr)
        {
            string result = formatStr;

            switch (this.AxisLabelPattern)
            {
                case AxisLabelFormatPattern.LabelAndData:
                    result = result.Replace("<ITEM_LABEL>", ""); 
                    result = result.Replace("(", ""); 
                    result = result.Replace(")", "");
                    return result.Trim();
                default:
                    return formatStr;

            }
        }

        private string GetSeriesFormatString(string formatStr)
        {
            string result = formatStr;

            switch (this.SeriesLabelPattern)
            {
                case SeriesLabelFormatPattern.SeriesLabelAndData:
                    result = result.Replace("<SERIES_LABEL>", "");
                    result = result.Replace("(", "");
                    result = result.Replace(")", "");
                    return result.Trim();
                default:
                    return formatStr;

            }
        }

        private string GetLegendFormatString(string formatStr)
        {
            string result = formatStr;

            switch (this.LegendPattern)
            {
                case LegendFormatPattern.LabelAndData:
                    result = result.Replace("<ITEM_LABEL>", "");
                    result = result.Replace("(", "");
                    result = result.Replace(")", "");
                    return result.Trim();
                default:
                    return formatStr;

            }
        }

        private string GetPieLabelFormatString(string formatStr)
        {
            return formatStr;
        }

        private string GetTooltipFormatString(string formatStr)
        {
            string result = formatStr;

            switch (this.TooltipPattern)
            {
                case TooltipFormatPattern.LabelAndData:
                    result = result.Replace("<ITEM_LABEL>", "");
                    result = result.Replace("(", "");
                    result = result.Replace(")", "");
                    return result.Trim();
                case TooltipFormatPattern.RowColumnAndData:
                    int ind = result.IndexOf("<DATA_COLUMN>:");
                    if (ind > -1)
                    {
                        result = result.Remove(0, ind + 14);
                    }
                    return result.Trim();
                default:
                    return formatStr;

            }

        }

        private static Dictionary<string, string> MakePatternTable()
        {
            Dictionary<string, string> patterns = new Dictionary<string, string>();

            patterns.Add("<ITEM_LABEL>.*<PERCENT_VALUE:.*>", "LabelAndPercentValue");
            patterns.Add("<ITEM_LABEL> (.*)", "LabelAndData");
            patterns.Add("<SERIES_LABEL> (.*)", "SeriesLabelAndData");
            patterns.Add("<ITEM_LABEL>", "ItemLabel");
            patterns.Add("<SERIES_LABEL>", "SeriesLabel");
            patterns.Add("<DATA_ROW>,.*<DATA_COLUMN>:.*", "RowColumnAndData");
            patterns.Add("<PERCENT_VALUE:.*>%", "PercentValue");

            patterns.Add("<DATE_TIME.*", "DataValue");
            patterns.Add("<EXP:.*", "DataValue");
            patterns.Add("<LONG_DATE.*", "DataValue");
            patterns.Add("<LONG_TIME.*", "DataValue");
            patterns.Add("<PERCENT:.*>",  "DataValue");
            patterns.Add("<PERCENT2:.*>", "DataValue");
            patterns.Add("<SHORT_DATE.*", "DataValue");
            patterns.Add("<SHORT_TIME.*", "DataValue");
            patterns.Add("<TRUEFALSE.*", "DataValue");
            patterns.Add("<YESNO.*",      "DataValue");
            patterns.Add("<GENERAL.*",    "DataValue");
            patterns.Add("<CRR:.*>",      "DataValue");
            patterns.Add("<MLRD_CRR:.*>", "DataValue");
            patterns.Add("<MLRD_CRR_ND:.*>", "DataValue");
            patterns.Add("<MLRD_NUM:.*>", "DataValue");
            patterns.Add("<MLN_CRR:.*>", "DataValue");
            patterns.Add("<MLN_CRR_ND:.*>", "DataValue");
            patterns.Add("<MLN_NUM:.*>", "DataValue");
            patterns.Add("<THS_CRR:.*>", "DataValue");
            patterns.Add("<THS_CRR_ND:.*>", "DataValue");
            patterns.Add("<THS_NUM:.*>", "DataValue");
            patterns.Add("<NUM:.*>", "DataValue");
            patterns.Add("<DATA_VALUE.*>", "DataValue");

            return patterns;
        }


        #endregion


        public override string ToString()
        {
            if (FormatType == FormatType.Auto)
            {
                return EnumTypeConverter.ToString(FormatType, typeof(FormatType));
            }
            else
            {
                return EnumTypeConverter.ToString(FormatType, typeof(FormatType)) + "; " + BooleanTypeConverter.ToString(UseThousandDelimiter) + "; " + DigitCount;
            }
        }

        /// <summary> 
        /// тип метки, для которой задается формат 
        /// </summary>
        public enum LabelType
        {
            None, //не показывать шаблон
            AxisLabel,
            SeriesLabel,
            Legend,
            Tooltip,
            PieLabel
        }



        /// <summary>
        /// Тип отображаемых данных для пузырьковой диаграммы
        /// </summary>
        public enum BubbleChartDataValueType
        {
            [Description("Общий")]
            DataValue,
            [Description("Значение по оси X")]
            DataValueX,
            [Description("Значение по оси Y")]
            DataValueY,
            [Description("Радиус")]
            DataValueRadius,

        }

        /// <summary>
        /// Тип отображаемых данных для ScatterChart, ScatterLineChart, ProbabilityChart
        /// </summary>
        public enum ScatterChartDataValueType
        {
            [Description("Общий")]
            DataValue,
            [Description("Значение по оси X")]
            DataValueX,
            [Description("Значение по оси Y")]
            DataValueY,
        }

        /// <summary>
        /// Тип отображаемых данных для полярной диаграммы
        /// </summary>
        public enum PolarChartDataValueType
        {
            [Description("Общий")]
            DataValue,
            [Description("Значение по оси X")]
            DataValueX,
            [Description("Значение по оси Y")]
            DataValueY
            /*
            [Description("Значение в градусах")]
            DegreesValue,
            [Description("Значение в радианах")]
            RadiansValue,*/
        }

        /// <summary>
        /// Тип отображаемых данных для теплокарты
        /// </summary>
        public enum HeatMapChartDataValueType
        {
            [Description("Общий")]
            DataValue,
            [Description("Верхнее левое значение")]
            DataValueTopLeft,
            [Description("Верхнее правое значение")]
            DataValueTopRight,
            [Description("Нижнее левое значение")]
            DataValueBottomLeft,
            [Description("Нижнее правое значение")]
            DataValueBottomRight
        }

        /// <summary>
        /// Тип отображаемых данных для теплокарты
        /// </summary>
        public enum TreeMapChartDataValueType
        {
            [Description("Общий")]
            DataValue,
            [Description("Размер")]
            SizeLabel,
            [Description("Цвет")]
            ColorLabel,
            [Description("Цвет и размер")]
            SizeAndColorLabel
        }

        /// <summary>
        /// Тип отображаемых данных для BoxChart
        /// </summary>
        public enum BoxChartDataValueType
        {
            [Description("Общий")]
            DataValue,
            [Description("Минимальное значение")]
            DataValueMin,
            [Description("Максимальное значение")]
            DataValueMax,
            [Description("Квартиль первого уровня")]
            DataValueQ1,
            [Description("Квартиль второго уровня")]
            DataValueQ2,
            [Description("Квартиль третьего уровня")]
            DataValueQ3
        }

    }

    #region Типы шаблонов
    /// <summary>
    /// Тип шаблона формата метки оси
    /// </summary>
    public enum AxisLabelFormatPattern
    {
        [Description("Нет")]
        None,
        [Description("Метка")]
        ItemLabel,
        [Description("Данные")]
        DataValue,
        [Description("Метка (данные)")]
        LabelAndData,
        [Description("Пользовательский")]
        Custom
    }

    /// <summary>
    /// Тип шаблона формата метки рядов
    /// </summary>
    public enum SeriesLabelFormatPattern
    {
        [Description("Нет")]
        None,
        [Description("Метка")]
        SeriesLabel,
        [Description("Данные")]
        DataValue,
        [Description("Метка (данные)")]
        SeriesLabelAndData,
        [Description("Пользовательский")]
        Custom
    }

    /// <summary>
    /// Тип шаблона формата легенды
    /// </summary>
    public enum LegendFormatPattern
    {
        [Description("Нет")]
        None,
        [Description("Метка")]
        ItemLabel,
        [Description("Метка (данные)")]
        LabelAndData,
        [Description("Пользовательский")]
        Custom
    }

    /// <summary>
    /// Тип шаблона формата подсказки
    /// </summary>
    public enum TooltipFormatPattern
    {
        [Description("Нет")]
        None,
        [Description("Данные")]
        DataValue,
        [Description("Метка (данные)")]
        LabelAndData,
        [Description("Ряд, категория: данные")]
        RowColumnAndData,
        [Description("Пользовательский")]
        Custom
    }

    /// <summary>
    /// Тип шаблона формата выноски
    /// </summary>
    public enum PieLabelFormatPattern
    {
        [Description("Нет")]
        None,
        [Description("Доля в процентах")]
        PercentValue,
        [Description("Метка")]
        ItemLabel,
        [Description("Данные")]
        DataValue,
        [Description("Метка и доля в процентах")]
        LabelAndPercentValue,
        [Description("Пользовательский")]
        Custom
    }
    #endregion
}