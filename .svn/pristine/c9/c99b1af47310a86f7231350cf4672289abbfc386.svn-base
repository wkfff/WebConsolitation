using System;
using System.ComponentModel;
using Krista.FM.Client.Common.Forms;
using Krista.FM.Client.MDXExpert.Data;

namespace Krista.FM.Client.MDXExpert
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class  MapFormatBrowseClass : FilterablePropertyBase
    {
        #region Поля

        private Data.ValueFormat format;

        private string thousandDelimiter = "#,##";

        private string formatStr;
        private bool displayUnits;

        private bool isLegendFormat;

        #endregion

        #region Свойства

        [Category("Формат")]
        [DisplayName("Тип")]
        [Description("Тип формата")]
        [TypeConverter(typeof(EnumTypeConverter))]
        [DefaultValue(Data.FormatType.Auto)]
        [Browsable(true)]
        public Data.FormatType FormatType
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

        [Browsable(false)]
        public bool ShowProperty_UnitDisplayType
        {
            get { return (this.isLegendFormat && FormatCanUnits(this.FormatType)); }
        }

        [Browsable(false)]
        public bool ShowProperty_DisplayUnits
        {
            get { return (!this.isLegendFormat && FormatCanUnits(this.FormatType)); }
        }

        [Category("Формат")]
        [DisplayName("Отображать единицы измерения")]
        [Description("Отображать или нет единицы измерения")]
        [DynamicPropertyFilter("ShowProperty_UnitDisplayType", "True")]
        [TypeConverter(typeof(EnumTypeConverter))]
        [Browsable(true)]
        public Data.UnitDisplayType UnitDisplayType
        {
            get { return this.format.UnitDisplayType; }
            set
            {
                this.format.UnitDisplayType = value;
                this.displayUnits = (UnitDisplayType == Data.UnitDisplayType.DisplayAtValue);
                DoFormatChange();
            }
        }

        [Category("Формат")]
        [DisplayName("Отображать единицы измерения")]
        [Description("Отображать или нет единицы измерения")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        //данное свойство будем отображать только при следующих типах значения показателя
        [DynamicPropertyFilter("ShowProperty_DisplayUnits", "True")]
        [DefaultValue(true)]
        [Browsable(true)]
        public bool DisplayUnits
        {
            get
            {
                if (isLegendFormat)
                {
                    this.displayUnits = (UnitDisplayType == Data.UnitDisplayType.DisplayAtValue);
                }

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
                return GetFormatStr(this.FormatType, this.DigitCount, this.UseThousandDelimiter, this.DisplayUnits);
            }
            set 
            {
                this.formatStr = GetMaskFromText(value);

                this.FormatType = GetFormatTypeByFormatString(this.formatStr);
                if (this.FormatType != Data.FormatType.Auto)
                {
                    this.format.DigitCount = GetDigitsCount(this.formatStr);
                    this.format.ThousandDelimiter = this.formatStr.Contains(thousandDelimiter);
                }

                SetDisplayUnits(this.formatStr);
            }
        }


        #endregion


        #region События
        private Data.ValueFormatEventHandler formatChanged = null;

        private static Data.ValueFormatEventHandler formatStringChanged = null;

        public event Data.ValueFormatEventHandler FormatChanged
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

        public event Data.ValueFormatEventHandler FormatStringChanged
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

        public void DoFormatStringChange()
        {
            if ((formatStringChanged != null)) //&&(FormatType != FormatType.Auto))
            {
                formatStringChanged();
            }
        }

        #endregion

        public MapFormatBrowseClass(string formatString, string formatString2, bool isLegendFormat)
        {
            this.format = new Data.ValueFormat();

            this.FormatString = formatString != "" ? formatString : formatString2;
            
            this.isLegendFormat = isLegendFormat;
        }

        public bool FormatCanUnits(Data.FormatType formatType)
        {
            switch(formatType)
            {
                case Data.FormatType.Currency:
                case Data.FormatType.ThousandsCurrency:
                case Data.FormatType.ThousandsCurrencyWitoutDivision:
                case Data.FormatType.MillionsCurrency:
                case Data.FormatType.MillionsCurrencyWitoutDivision:
                case Data.FormatType.MilliardsCurrency:
                case Data.FormatType.MilliardsCurrencyWitoutDivision:
                    return true;                    
            }
            return false;
        }

        public string ApplyFormatToText(string text, string formatString)
        {
            int startIndex = text.IndexOf("{");

            while (startIndex > -1)
            {
                int endIndex = text.IndexOf("}", startIndex);
                if (endIndex <= startIndex)
                {
                    break;
                }
                text = text.Remove(startIndex + 1, endIndex - startIndex - 1);
                text = text.Insert(startIndex + 1, formatString);
                startIndex = text.IndexOf("{", startIndex + 1);
            }
            return text;
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
            return (formatString.Contains("\\р\\."));
        }

        private void SetDisplayUnits(string formatStr)
        {
            switch (this.FormatType)
            {
                case Data.FormatType.Currency:
                case Data.FormatType.ThousandsCurrency:
                case Data.FormatType.ThousandsCurrencyWitoutDivision:
                case Data.FormatType.MillionsCurrency:
                case Data.FormatType.MillionsCurrencyWitoutDivision:
                case Data.FormatType.MilliardsCurrency:
                case Data.FormatType.MilliardsCurrencyWitoutDivision:
                    this.displayUnits = GetDisplayUnits(formatStr);
                    break;
                default:
                    this.displayUnits = true;
                    this.format.UnitDisplayType = Data.UnitDisplayType.DisplayAtValue;
                    break;
            }

        }

        private string GetMask(string digitsLabel, string unit, string dummyUnit, bool useThousandDelimiter, bool displayUnits)
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
            else
            {
                result += dummyUnit;
            }
            return result;
        }

        private string GetMaskFromText(string text)
        {
            int beginMask = text.IndexOf("{");
            if (beginMask > -1)
            {
                int endMask = text.IndexOf("}");
                if(beginMask < endMask)
                {
                    return text.Substring(beginMask + 1, endMask - beginMask - 1);
                }
            }
            return "";
        }

        private string GetFormatStr(Data.FormatType formatType, byte digits, bool useThousandDelimiter, bool displayUnits)
        {
            switch (formatType)
            {
                case Data.FormatType.Auto:
                    return "#,##0.00";
                case Data.FormatType.DateTime:
                    return @"d MMMM yyyy \г\. H:mm:ss:";
                case Data.FormatType.Exponential:
                    return "0E+00";
                case Data.FormatType.LongDate:
                    return @"d MMMM yyyy \г\.";
                case Data.FormatType.LongTime:
                    return "H:mm:ss";
                case Data.FormatType.Percent:
                    return GetMask("0" + GetDigitsLabel(digits), "%", "", useThousandDelimiter, true);
                case Data.FormatType.ShortDate:
                    return "dd.MM.yyyy";
                case Data.FormatType.ShortTime:
                    return "H:mm";
                case Data.FormatType.TrueFalse:
                    return "\"Истина\";\"Истина\";\"Ложь\"";
                case Data.FormatType.YesNo:
                    return "\"Да\";\"Да\";\"Нет\"";
                case Data.FormatType.None:
                    return "";
                case Data.FormatType.Currency:
                    return GetMask("0" + GetDigitsLabel(digits), @" \р\.", "''", useThousandDelimiter, displayUnits);
                case Data.FormatType.MilliardsCurrency:
                    return GetMask("0,,," + GetDigitsLabel(digits), @" \м\л\р\д\.\р\.", "''", useThousandDelimiter,
                                   displayUnits);
                case Data.FormatType.MilliardsCurrencyWitoutDivision:
                    return GetMask("0" + GetDigitsLabel(digits), @" \м\л\р\д\.\р\.", "'''''", useThousandDelimiter,
                                   displayUnits);
                case Data.FormatType.MilliardsNumeric:
                    return GetMask("0,,," + GetDigitsLabel(digits), "", "", useThousandDelimiter, displayUnits);
                case Data.FormatType.MillionsCurrency:
                    return GetMask("0,," + GetDigitsLabel(digits), @" \м\л\н\.\р\.", "''", useThousandDelimiter,
                                   displayUnits);
                case Data.FormatType.MillionsCurrencyWitoutDivision:
                    return GetMask("0" + GetDigitsLabel(digits), @" \м\л\н\.\р\.", "''''", useThousandDelimiter,
                                   displayUnits);
                case Data.FormatType.MillionsNumeric:
                    return GetMask("0,," + GetDigitsLabel(digits), "", "", useThousandDelimiter, displayUnits);
                case Data.FormatType.ThousandsCurrency:
                    return GetMask("0," + GetDigitsLabel(digits), @" \т\ы\с\.\р\.", "''", useThousandDelimiter,
                                   displayUnits);
                case Data.FormatType.ThousandsCurrencyWitoutDivision:
                    return GetMask("0" + GetDigitsLabel(digits), @" \т\ы\с\.\р\.", "'''", useThousandDelimiter,
                                   displayUnits);
                case Data.FormatType.ThousandsNumeric:
                    return GetMask("0," + GetDigitsLabel(digits), "", "", useThousandDelimiter, displayUnits);
                case Data.FormatType.Numeric:
                    return GetMask("0" + GetDigitsLabel(digits), "'", "'", useThousandDelimiter, displayUnits);
            }


            return "";
        }

        /// <summary>
        /// Получение типа формата по маске
        /// </summary>
        /// <param name="formatString">маска формата</param>
        /// <returns>тип формата</returns>
        private Data.FormatType GetFormatTypeByFormatString(string formatString)
        {
            switch (formatString)
            {
                case @"d MMMM yyyy \г\. H:mm:ss:":
                    return Data.FormatType.DateTime;
                case "0E+00":
                    return Data.FormatType.Exponential;
                case @"d MMMM yyyy \г\.":
                    return Data.FormatType.LongDate;
                case "H:mm:ss":
                    return Data.FormatType.LongTime;
                case "dd.MM.yyyy":
                    return Data.FormatType.ShortDate;
                case "H:mm":
                    return Data.FormatType.ShortTime;
                case "\"Истина\";\"Истина\";\"Ложь\"":
                    return Data.FormatType.TrueFalse;
                case "\"Да\";\"Да\";\"Нет\"":
                    return Data.FormatType.YesNo;
                case "":
                    return Data.FormatType.None;
            }

            if (formatString.Contains("%"))
            {
                return Data.FormatType.Percent;
            }
            else
                if (formatString.Contains("0,,,"))
            {
                if (formatString.Contains(@"\м\л\р\д\.\р\.") || formatString.Contains("''"))
               {
                   return Data.FormatType.MilliardsCurrency;
               }
               else
               {
                   return Data.FormatType.MilliardsNumeric;
               }
            }
            else
                if (formatString.Contains("0,,"))
            {
                if (formatString.Contains(@"\м\л\н\.\р\.") || formatString.Contains("''"))
               {
                   return Data.FormatType.MillionsCurrency;
               }
               else
               {
                   return Data.FormatType.MillionsNumeric;
               }
            }
            else
                if (formatString.Contains("0,"))
            {
                if (formatString.Contains(@"\т\ы\с\.\р\.")  || formatString.Contains("''"))
                {
                    return Data.FormatType.ThousandsCurrency;
                }
                else
                {
                    return Data.FormatType.ThousandsNumeric;
                }
            }
            else
                if (formatString.Contains(@"\м\л\р\д\.\р\.") || formatString.Contains("'''''"))
            {
                return Data.FormatType.MilliardsCurrencyWitoutDivision;
            }
            else
                if (formatString.Contains(@"\м\л\н\.\р\.") || formatString.Contains("''''"))
            {
                return Data.FormatType.MillionsCurrencyWitoutDivision;
            }
            else
                if (formatString.Contains(@"\т\ы\с\.\р\.") || formatString.Contains("'''"))
            {
                return Data.FormatType.ThousandsCurrencyWitoutDivision;
            }
            else
               if (formatString.Contains("0"))
            {
                if (formatString.Contains("''") || formatString.Contains(@"\р\."))
                {
                    return Data.FormatType.Currency;
                }
                else
                    if (formatString.Contains("'"))
                    {
                        return Data.FormatType.Numeric;
                    }
            }
          

            return Data.FormatType.Auto;

        }

        /// <summary>
        /// получение ед. измерения по типу формата
        /// </summary>
        /// <returns>ед. измерения в виде строки</returns>
        public string GetUnits()
        {
            switch(this.FormatType)
            {
                case Data.FormatType.Currency:
                    return @"\р\.";
                case Data.FormatType.MilliardsCurrency:
                case Data.FormatType.MilliardsCurrencyWitoutDivision:
                    return @"\м\л\р\д\.\р\.";
                case Data.FormatType.MillionsCurrency:
                case Data.FormatType.MillionsCurrencyWitoutDivision:
                    return @"\м\л\н\.\р\.";
                case Data.FormatType.ThousandsCurrency:
                case Data.FormatType.ThousandsCurrencyWitoutDivision:
                    return @"\т\ы\с\.\р\.";
            }
            return "";
        }


        public void SetUnitDisplayType(Data.UnitDisplayType value)
        {
            this.format.UnitDisplayType = value;
        }

        public override string ToString()
        {
            if (FormatType == Data.FormatType.Auto)
            {
                return EnumTypeConverter.ToString(FormatType, typeof(Data.FormatType));
            }
            else
            {
                return EnumTypeConverter.ToString(FormatType, typeof(FormatType)) + "; " + BooleanTypeConverter.ToString(UseThousandDelimiter) + "; " + DigitCount;
            }
        }

    }
       
}