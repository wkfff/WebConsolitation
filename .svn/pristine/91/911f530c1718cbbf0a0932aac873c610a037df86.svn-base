using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using Krista.FM.Client.Common.Forms;
using Krista.FM.Client.MDXExpert.Data;
using Krista.FM.Client.MDXExpert.Grid.UserInterface;
using System.Drawing;

namespace Krista.FM.Client.MDXExpert

{
    /// <summary>
    /// Свойства меры
    /// </summary>
    public class MapPivotTotalBrowseAdapter : PivotTotalBrowseAdapter
    {
        private ValueFormatBrowseClass valueFormatBrowse;

        public MapPivotTotalBrowseAdapter(Data.PivotTotal pivotTotal, CustomReportElement reportElement)
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
            [DisplayName("Отображение единиц измерения")]
            [Description("Отображение единиц измерения")]
            [TypeConverter(typeof(BooleanTypeConverter))]
            [Browsable(true)]
            //данное свойство будем отображать только при следующих типах значения показателя
            [DynamicPropertyFilter("FormatType", "Currency, ThousandsCurrency, ThousandsCurrencyWitoutDivision,"
                + "MillionsCurrency, MillionsCurrencyWitoutDivision, MilliardsCurrency, MilliardsCurrencyWitoutDivision,"
                + "Percent, ThousandsNumeric, MillionsNumeric, MilliardsNumeric")]
            public bool UnitDisplayType
            {
                get
                {
                    return (format.UnitDisplayType != Krista.FM.Client.MDXExpert.Data.UnitDisplayType.None);
                }
                set
                {
                    format.UnitDisplayType = value ? Krista.FM.Client.MDXExpert.Data.UnitDisplayType.DisplayAtValue :
                                                     Krista.FM.Client.MDXExpert.Data.UnitDisplayType.None;
                }
            } 

            #endregion

            public ValueFormatBrowseClass(Data.ValueFormat format)
            {
                this.format = format;
            }

            public override string ToString()
            {
                return 
                     (this.format.IsUnitDisplayTypeEnable ? BooleanTypeConverter.ToString(UnitDisplayType) + "; " : string.Empty)
                    + (this.format.IsThousandDelimiterEnable ? BooleanTypeConverter.ToString(ThousandDelimiter) + "; " : string.Empty)
                    + EnumTypeConverter.ToString(FormatType, typeof(FormatType)) + "; "
                    + (this.format.IsDigitCountEnable? DigitCount + "; " : string.Empty);
                    
            }

        }
    }
}
