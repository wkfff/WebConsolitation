using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Collections.Specialized;

namespace Krista.FM.Client.MDXExpert
{
    /// <summary>
    /// TypeConverter для списка
    /// </summary>
    class SymbolTextConverter : StringConverter
    {
        private static StringCollection stringValues = new StringCollection(); 

        public static StringCollection StringValues
        {
            get { return stringValues; }
            set { stringValues = value; }
        }

        /// <summary>
        /// Будем предоставлять выбор из списка
        /// </summary>
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            // false - можно вводить вручную
            // true - только выбор из списка
            return false;
        }

        /// <summary>
        /// А вот и список
        /// </summary>
        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            return new StandardValuesCollection(StringValues);
        }

      
    }

}