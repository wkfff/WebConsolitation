using System;
using System.ComponentModel;
using Krista.FM.Client.Common.Forms;
using Krista.FM.Client.MDXExpert.Data;
using System.Drawing;

namespace Krista.FM.Client.MDXExpert

{
    /// <summary>
    /// Свойства меры
    /// </summary>
    public class TablePivotTotalBrowseAdapter : PivotTotalBrowseAdapter
    {
        private ValueFormatBrowseClass valueFormatBrowse;

        public TablePivotTotalBrowseAdapter(Data.PivotTotal pivotTotal, CustomReportElement reportElement)
            : base(pivotTotal, reportElement)
        {
            valueFormatBrowse = new ValueFormatBrowseClass(pivotTotal.Format);
        }

        #region свойства

        private Data.PivotTotal CurrentPivotObject
        {
            get { return (Data.PivotTotal)base.PivotObject; }
        }

        [Category("Общие")]
        [DisplayName("Формат")]
        [Description("Формат меры")]
        [Browsable(true)]
        public ValueFormatBrowseClass ValueFormatBrowse
        {
            get 
            { 
                return valueFormatBrowse; 
            }
            set 
            { 
                valueFormatBrowse = value; 
            }
        }
        /*
        [Category("Общие")]
        [DisplayName("Сортировка")]
        [Description("Сортировка")]
        [TypeConverter(typeof(EnumTypeConverter))]
        [Browsable(true)]
        public SortType SortType
        {
            get
            {
                return CurrentPivotObject.SortType;
            }
            set
            {
                CurrentPivotObject.SortType = value;
            }
        }
        */

        #endregion


        [TypeConverter(typeof(ExpandableObjectConverter))]
        public class ValueFormatBrowseClass : FilterablePropertyBase
        {
            private Data.ValueFormat format;

            #region Свойства

            [Category("Формат")]
            [DisplayName("Тип")]
            [Description("Тип формата меры")]
            [TypeConverter(typeof(EnumTypeConverter))]
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
                }
            }

            [Category("Формат")]
            [DisplayName("Число десятичных знаков")]
            [Description("Количество десятичных знаков, отображаемых после запятой")]
            [Browsable(true)]
            //данное свойство будем отображать только при следующих типах значения показателя
            [DynamicPropertyFilter("FormatType", "Exponential, Currency, ThousandsCurrency, ThousandsCurrencyWitoutDivision,"
                + "MillionsCurrency, MillionsCurrencyWitoutDivision, MilliardsCurrency, MilliardsCurrencyWitoutDivision,"
                + "Percent, Numeric, ThousandsNumeric, MillionsNumeric, MilliardsNumeric")]
            public string DigitCount
            {
                get 
                { 
                    return format.DigitCount.ToString(); 
                }
                set 
                {
                    byte bValue = 0;
                    if ((byte.TryParse(value, out bValue)) && (bValue <= 20))
                    {
                        format.DigitCount = bValue;
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
            [Browsable(true)]
            //данное свойство будем отображать только при следующих типах значения показателя
            [DynamicPropertyFilter("FormatType", "Currency, ThousandsCurrency, ThousandsCurrencyWitoutDivision,"
                + "MillionsCurrency, MillionsCurrencyWitoutDivision, MilliardsCurrency, MilliardsCurrencyWitoutDivision,"
                + "Percent, Numeric, ThousandsNumeric, MillionsNumeric, MilliardsNumeric")]
            public bool ThousandDelimiter
            {
                get 
                {
                    return format.ThousandDelimiter; 
                }
                set 
                {
                    format.ThousandDelimiter = value; 
                }
            }

            [Category("Формат")]
            [DisplayName("Выравнивание значений")]
            [Description("Выравнивание значений")]
            [TypeConverter(typeof(StringAlignmentBarHorizontalConverter))]
            [Browsable(true)]
            public StringAlignment ValueAlignment
            {
                get
                {
                    return format.ValueAlignment;
                }
                set
                {
                    format.ValueAlignment = value;
                }
            }

            [Category("Формат")]
            [DisplayName("Отображение единиц измерения")]
            [Description("Отображение единиц измерения")]
            [TypeConverter(typeof(EnumTypeConverter))]
            [Browsable(true)]
            //данное свойство будем отображать только при следующих типах значения показателя
            [DynamicPropertyFilter("FormatType", "Currency, ThousandsCurrency, ThousandsCurrencyWitoutDivision,"
                + "MillionsCurrency, MillionsCurrencyWitoutDivision, MilliardsCurrency, MilliardsCurrencyWitoutDivision,"
                + "Percent, ThousandsNumeric, MillionsNumeric, MilliardsNumeric")]
            public Data.UnitDisplayType UnitDisplayType
            {
                get
                {
                    return format.UnitDisplayType;
                }
                set
                {
                    format.UnitDisplayType = value;
                }
            }




            #endregion

            public ValueFormatBrowseClass(Data.ValueFormat format)
            {
                this.format = format;
            }

            public override string ToString()
            {
                return StringAlignmentBarHorizontalConverter.ToString(ValueAlignment) + "; "
                    + (this.format.IsUnitDisplayTypeEnable ? EnumTypeConverter.ToString(UnitDisplayType, typeof(Data.UnitDisplayType)) + "; " : string.Empty)
                    + (this.format.IsThousandDelimiterEnable ? BooleanTypeConverter.ToString(ThousandDelimiter) + "; " : string.Empty)
                    + EnumTypeConverter.ToString(FormatType, typeof(FormatType)) + "; "
                    + (this.format.IsDigitCountEnable? DigitCount + "; " : string.Empty);
                    
            }

        }
    }
}
