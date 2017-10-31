using System;
using System.Collections.Generic;
using System.Data;

namespace Krista.FM.Client.Reports.Common.CommonParamForm
{
    public enum ReportParamType
    {
        Number,
        Text,
        DateValue,
        Flag,
        List,
        Book
    }

    public class ParamInfo
    {
        public class FilterInfo
        {
            public string FieldName { get; set; }
            public List<object> FieldValue { get; set; }
        }

        protected const string ID = "ID";
        protected const string NAME = "NAME";
        protected ReportDBHelper dbHelper = new ReportDBHelper(ConvertorSchemeLink.scheme);

        public bool NotUncheckBeforeSelect { get; set; }
        public bool AutoFirstValue { get; set; }
        public FilterInfo Filter { set; get; }
        public string CategoryName { set; get; }
        public string ParentName { set; get; }
        public bool MultiSelect { get; set; }
        public bool ReadOnly { set; get; }
        public string Name { set; get; }
        public string DisplayFieldName { set; get; }
        public string Description { set; get; }
        public string MaskInput { set; get; }
        public ReportParamType ParamType { set; get; }
        public object DefaultValue { set; get; }
        public List<object> Values { set; get; }
        public Type EnumObj { set; get; }
        public DataTable Table { set; get; }
        public ParamBookInfo BookInfo { set; get; }
        public List<string> Columns { set; get; }


        public ParamInfo SetCaption(string caption)
        {
            Description = caption;
            return this;
        }

        public ParamInfo SetMultiSelect(bool value)
        {
            MultiSelect = value;
            return this;
        }

        public ParamInfo SetDeepSelect(bool value)
        {
            BookInfo.DeepSelect = value;
            return this;
        }

        public ParamInfo SetValue(object value)
        {
            DefaultValue = value;
            return this;
        }

        public ParamInfo SetReadonly()
        {
            ReadOnly = true;
            return this;
        }

        public ParamInfo SetFilter(string fieldName, List<object> fieldValue)
        {
            Filter = new FilterInfo { FieldName = fieldName, FieldValue = fieldValue };
            return this;
        }

        public ParamInfo SetItemFilter(string fieldName, object value)
        {
            BookInfo.ItemFilter = String.Format("{0} in ({1})", fieldName, value);
            return this;
        }

        public ParamInfo SetMask(string mask)
        {
            MaskInput = mask;
            return this;
        }

        public ParamInfo SetNonUncheckBeforeSelect(bool value)
        {
            NotUncheckBeforeSelect = value;
            return this;
        }

        public string GetListText(object value)
        {
            var text = Convert.ToString(value);

            if (String.IsNullOrEmpty(text))
            {
                return String.Empty;
            }

            var parts = text.Split(',');
            var values = new string[parts.Length];

            for (var i = 0; i < parts.Length; i++)
            {
                values[i] = Convert.ToString(Values[Convert.ToInt32(parts[i])]);
            }

            return String.Join(",", values);
        }
    }
}
