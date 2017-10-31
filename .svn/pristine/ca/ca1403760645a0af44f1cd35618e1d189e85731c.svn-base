using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

using Krista.FM.Expert.PivotData;
using Krista.FM.Client.Common.Forms;
using Krista.FM.Client.MDXExpert.Grid.UserInterface;
using System.Drawing;

namespace Krista.FM.Client.MDXExpert

{
    /// <summary>
    /// Свойства меры
    /// </summary>
    public class PivotTotalBrowseAdapter : PivotObjectBrowseAdapterBase
    {
        private ValueFormatBrowseClass valueFormatBrowse;

        public PivotTotalBrowseAdapter(PivotTotal pivotTotal, CustomReportElement reportElement)
            : base(pivotTotal, pivotTotal.Caption, reportElement)
        {
            valueFormatBrowse = new ValueFormatBrowseClass(pivotTotal.Format);
        }

        #region свойства

        private PivotTotal CurrentPivotObject
        {
            get { return (PivotTotal)base.PivotObject; }
        }

        [Category("Общие")]
        [DisplayName("Заголовок")]
        [Description("Заголовок меры")]
        public string Caption
        {
            get 
            {
                return this.CurrentPivotObject.Caption; 
            }
            set 
            {
                base.Header = value;
                this.CurrentPivotObject.Caption = value;
            }
        }


        [Category("Общие")]
        [DisplayName("Уникальное имя")]
        [Description("Уникальное имя меры")]
        public string UniqueName
        {
            get 
            {
                return this.CurrentPivotObject.UniqueName; 
            }
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
            private ValueFormat format;

            #region Свойства

            [Category("Формат")]
            [DisplayName("Тип")]
            [Description("Тип формата меры")]
            [TypeConverter(typeof(EnumTypeConverter))]
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
            public UnitDisplayType UnitDisplayType
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

            public ValueFormatBrowseClass(ValueFormat format)
            {
                this.format = format;
            }

            public override string ToString()
            {
                return StringAlignmentBarHorizontalConverter.ToString(ValueAlignment) + "; "
                    + (this.format.IsUnitDisplayTypeEnable ? EnumTypeConverter.ToString(UnitDisplayType, typeof(UnitDisplayType)) + "; " : string.Empty)
                    + (this.format.IsThousandDelimiterEnable ? BooleanTypeConverter.ToString(ThousandDelimiter) + "; " : string.Empty)
                    + EnumTypeConverter.ToString(FormatType, typeof(FormatType)) + "; "
                    + (this.format.IsDigitCountEnable? DigitCount + "; " : string.Empty);
                    
            }

        }
    }
}
