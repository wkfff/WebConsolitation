using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Drawing;

namespace Krista.FM.Client.MDXExpert.Data
{
    public delegate void ValueFormatEventHandler();

    public class ValueFormat
    {
        #region Поля

        private bool _isMayHook;
        private FormatType _formatType;
        private byte _digitCount;
        private bool _thousandDelimiter;
        private StringAlignment _valueAlignment;
        private UnitDisplayType _unitDisplayType;

        #endregion

        public ValueFormat()
        {
            this._formatType = FormatType.Auto;
            this._digitCount = 2;
            this._unitDisplayType = UnitDisplayType.DisplayAtValue;
            this._valueAlignment = StringAlignment.Far;
            this._thousandDelimiter = true;
        }

        #region Свойства

        /// <summary>
        /// Учитывается ли у текущего формата меры, свойство "Количество цифр, после запятой"
        /// </summary>
        public bool IsDigitCountEnable
        {
            get
            {
                return this.IsThousandDelimiterEnable || (this.FormatType == FormatType.Exponential);
            }
        }

        /// <summary>
        /// Учитывается ли у текущего формата меры, свойство "Разделитель разрядов"
        /// </summary>
        public bool IsThousandDelimiterEnable
        {
            get 
            { 
                return this.IsUnitDisplayTypeEnable || (this.FormatType == FormatType.Numeric); 
            }
        }

        /// <summary>
        /// Учитывается ли у текущего формата меры, "Место отображение, едениц измерения"
        /// </summary>
        public bool IsUnitDisplayTypeEnable
        {
            get
            {
                return (this.FormatType == FormatType.Currency) || (this.FormatType == FormatType.ThousandsCurrency)
                || (this.FormatType == FormatType.ThousandsCurrencyWitoutDivision) || (this.FormatType == FormatType.MillionsCurrency)
                || (this.FormatType == FormatType.MillionsCurrencyWitoutDivision) || (this.FormatType == FormatType.MilliardsCurrency)
                || (this.FormatType == FormatType.MilliardsCurrencyWitoutDivision) || (this.FormatType == FormatType.Percent)
                || (this.FormatType == FormatType.ThousandsNumeric) || (this.FormatType == FormatType.MillionsNumeric)
                || (this.FormatType == FormatType.MilliardsNumeric);
            }
        }

        public FormatType FormatType
        {
            get
            {
                return _formatType;
            }
            set
            {
                _formatType = value;
                DoChanged();
            }
        }

        public byte DigitCount
        {
            get
            {
                return _digitCount;
            }
            set
            {
                _digitCount = value;
                DoChanged();
            }
        }

        public bool ThousandDelimiter
        {
            get
            {
                return _thousandDelimiter;
            }
            set
            {
                _thousandDelimiter = value;
                DoChanged();
            }
        }

        public StringAlignment ValueAlignment
        {
            get
            {
                return _valueAlignment;
            }
            set
            {
                _valueAlignment = value;
                DoChanged();
            }
        }

        public UnitDisplayType UnitDisplayType
        {
            get
            {
                return _unitDisplayType;
            }
            set
            {
                _unitDisplayType = value;
                DoChanged();
            }
        }

        /// <summary>
        /// Признак, что никакие события не должны происходить
        /// </summary>
        public bool IsMayHook
        {
            get { return _isMayHook; }
            set { _isMayHook = value; }
        }

        #endregion

        #region События

        private ValueFormatEventHandler changed = null;


        public event ValueFormatEventHandler Changed
        {
            add 
            { 
                changed += value; 
            }
            remove 
            { 
                changed -= value; 
            }
        }

        private void DoChanged()
        {
            if ((changed != null) && !this.IsMayHook)
            {
                changed();
            }
        }

        #endregion

        public override string ToString()
        {
            return string.Empty;
        }
    }

    [Flags]
    public enum FormatType
    {
        [Description ("Автоматический")]
        Auto,
        [Description("Общий")] 
        None,       //что есть то и отображаем  
        [Description("Экспоненциальный")]
        Exponential,

        //Денежные форматы
        [Description("Денежный")]
        Currency,  
  
        [Description("Денежный, тыс.р.")]
        ThousandsCurrency,
        [Description("Денежный, тыс.р. без деления")]
        ThousandsCurrencyWitoutDivision,

        [Description("Денежный, млн.р.")]
        MillionsCurrency,
        [Description("Денежный, млн.р. без деления")]
        MillionsCurrencyWitoutDivision,

        [Description("Денежный, млрд.р.")]
        MilliardsCurrency,
        [Description("Денежный, млрд.р. без деления")]
        MilliardsCurrencyWitoutDivision,

        //Процентные форматы
        [Description("Процентный")]
        Percent,

        //Числовый форматы
        [Description("Числовой")]
        Numeric,    //числовой (123 456.00)

        [Description("Числовой, тыс.")]
        ThousandsNumeric,
        [Description("Числовой, млн.")]
        MillionsNumeric,
        [Description("Числовой, млрд.")]
        MilliardsNumeric,

        //Форматы даты и времени
        [Description("Дата время")]
        DateTime,
        [Description("Длинный формат даты")]
        LongDate,
        [Description("Длинный формат времени")]
        LongTime,
        [Description("Короткий формат даты")]
        ShortDate,
        [Description("Короткий формат времени")]
        ShortTime,
        [Description("Да/Нет")]
        YesNo,
        [Description("Истина/Ложь")]
        TrueFalse
    }

    public enum UnitDisplayType
    {
        [Description("Не выводить")]
        None,
        [Description("В колонке показателя")]
        DisplayAtValue,
        [Description("В заголовке показателя")]
        DisplayAtCaption
    }
}
