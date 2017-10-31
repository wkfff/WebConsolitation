using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;

namespace Krista.FM.Client.Reports.Common.CommonParamForm
{
    public class ParamContainer
    {
        public const string ID = "ID";
        public const string NAME = "NAME";
        public string CurrentCategory { get; set; }

        public readonly List<ParamInfo> lstParams = new List<ParamInfo>();
        private readonly Dictionary<string, string> paramValues = new Dictionary<string, string>();
        public readonly Dictionary<string, Dictionary<string, UndercutParamBase>> links = new Dictionary<string, Dictionary<string, UndercutParamBase>>();

        public Dictionary<string, string> paramFilters = new Dictionary<string, string>();
        public Dictionary<string, string> paramItemFilters = new Dictionary<string, string>();

        public void AddParamLink(string childParam, string parentParam, UndercutParamBase paramUpdater)
        {
            // если первая связь, то список создаем
            if (!links.ContainsKey(childParam))
            {
                links[childParam] = new Dictionary<string, UndercutParamBase>();
            }

            // добавляем связь
            if (!links[childParam].ContainsKey(parentParam))
            {
                links[childParam].Add(parentParam, paramUpdater);
            }
        }

        public ParamInfo AddParam(string key)
        {
            var settings = new ParamInfo { Name = key, CategoryName = CurrentCategory};
            lstParams.Add(settings);
            return settings;
        }

        public ParamInfo AddBoolParam(string key)
        {
            var settings = AddParam(key);
            settings.ParamType = ReportParamType.Flag;
            return settings;
        }

        public ParamInfo AddStringParam(string key)
        {
            var settings = AddParam(key);
            settings.ParamType = ReportParamType.Text;
            return settings;
        }

        public ParamInfo AddNumParam(string key)
        {
            var settings = AddParam(key);
            settings.ParamType = ReportParamType.Number;
            return settings;
        }

        public ParamInfo AddIntParam(string key)
        {
            var settings = AddNumParam(key);
            settings.MaskInput = "nnnn";
            settings.DefaultValue = 0;
            return settings;
        }

        public ParamInfo AddYearParam(string key)
        {
            var settings = AddNumParam(key);
            settings.MaskInput = "nnnn";
            settings.DefaultValue = DateTime.Now.Year;
            return settings;
        }

        public ParamInfo AddExchangeParam(string key)
        {
            var settings = AddNumParam(key);
            settings.MaskInput = "nn.nnnn";
            settings.DefaultValue = 35;
            return settings;
        }

        public ParamInfo AddDateParam(string key)
        {
            var settings = AddParam(key);
            settings.ParamType = ReportParamType.DateValue;
            settings.DefaultValue = DateTime.Now;
            return settings;
        }

        public ParamInfo AddListParam(string key)
        {
            var settings = AddParam(key);
            settings.ParamType = ReportParamType.List;
            settings.AutoFirstValue = true;
            return settings;
        }

        public ParamInfo AddEnumParam(string key, Type enumType)
        {
            var settings = AddParam(key);
            settings.ParamType = ReportParamType.List;
            settings.AutoFirstValue = true;
            settings.EnumObj = enumType;
            var names = Enum.GetNames(enumType);
            var tbl = CreateTable();

            foreach (var name in names)
            {
                var row = tbl.Rows.Add();
                row[ID] = name;
                var fi = enumType.GetField(name);
                var dna = (DescriptionAttribute)Attribute.GetCustomAttribute(fi, typeof(DescriptionAttribute));
                row[NAME] = dna != null ? dna.Description : name;
            }

            settings.Table = tbl;
            return settings;
        }

        public DataTable CreateTable(List<string> columns)
        {
            var tbl = new DataTable();

            foreach (var column in columns)
            {
                tbl.Columns.Add(column, typeof(string));
            }

            return tbl;
        }

        private DataTable CreateTable()
        {
            return CreateTable(new List<string> { ID, NAME });
        }

        public ParamInfo AddBookParam(string key, ParamInfo info)
        {
            info.Name = key;

            if (info.BookInfo != null && info.BookInfo.FullScreen)
            {
                info.ParamType = ReportParamType.Book;                
            }
            else
            {
                info.ParamType = ReportParamType.List;

                if (info.BookInfo != null)
                {
                    info.Table = info.BookInfo.CreateDataList();
                    info.MultiSelect = info.BookInfo.MultiSelect;
                }
            }

            lstParams.Add(info);
            return info;
        }

        public ParamInfo AddParam(ParamInfo param)
        {
            lstParams.Add(param);
            return param;
        }

        public void SetParamValue(string key, object value)
        {
            paramValues[key] = Convert.ToString(value);
        }

        public string GetParamValue(string key)
        {
            return paramValues.ContainsKey(key) ? paramValues[key] : String.Empty;
        }

        public Dictionary<string, string> GetParams()
        {
            return paramValues;
        }

        public ParamInfo this[string key]
        {
            get { return lstParams.Where(f => f.Name == key).FirstOrDefault(); }
        }
    }
}
