using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing.Design;
using System.Text;
using System.Windows.Forms.Design;
using Krista.FM.Client.MDXExpert.Common;

namespace Krista.FM.Client.MDXExpert.Data
{
    [Serializable]
    public class MemberFilter
    {
        private string _memberName;
        private Dictionary<string, string> _properties;

        public string MemberName
        {
            get { return _memberName; }
            set { _memberName = value; }
        }

        public Dictionary<string, string> Properties
        {
            get { return _properties; }
            set { _properties = value; }
        }

        public MemberFilter()
        {
            this.Properties = new Dictionary<string, string>();
        }
    }

    public class MemberFilterBrowse
    {
        private MemberFilter _mFilter;

        [DisplayName("Имя элемента")]
        [Description("Подстрока, входящая в имя элемента")]
        public string MemberName
        {
            get { return this._mFilter.MemberName; }
            set { this._mFilter.MemberName = value; }
        }

        [DisplayName("Свойства элемента")]
        [Description("Фильтр по значениям свойств элементов")]
        [Editor(typeof(GenericDictionaryEditor<string, string>), typeof(UITypeEditor))]
        [GenericDictionaryEditor(KeyConverterType = typeof(MemberPropertiesConverter), Title = "Фильтр по свойствам")]
        [BrowsableAttribute(true), ReadOnly(false), TypeConverter(typeof(System.ComponentModel.CollectionConverter))]
        public Dictionary<string, string> Properties
        {
            get { return this._mFilter.Properties; }
            set { this._mFilter.Properties = value; }
        }

        public MemberFilterBrowse(MemberFilter mFilter)
        {
            this._mFilter = mFilter;
        }
    }


    class MemberPropertiesConverter : StringConverter
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
        public override bool GetStandardValuesSupported(
           ITypeDescriptorContext context)
        {
            return true;
        }

        public override bool GetStandardValuesExclusive(
           ITypeDescriptorContext context)
        {
            // false - можно вводить вручную
            // true - только выбор из списка
            return true;
        }

        /// <summary>
        /// А вот и список
        /// </summary>
        public override StandardValuesCollection GetStandardValues(
           ITypeDescriptorContext context)
        {
            return new StandardValuesCollection(StringValues);
        }


    }

}
