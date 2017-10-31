using System.Collections.Generic;
using System.Linq;

namespace Krista.FM.Client.Reports.Common.CommonParamForm.Books.Lvls
{
    class ParamSingleBdgtLvlsFull : ParamInfo
    {
        private readonly List<object> _valuesList = new List<object>
        {
            "Федеральный бюджет",
            "Конс. бюджет субъекта",
            "Бюджет субъекта",
            "Местный бюджет",
            "Бюджеты МР и ГО",
            "Бюджет поселений",
            "Внебюджетные фонды",
            "Целевые фонды"
        };

        public string ValuesFilter
        {
            set
            {
                var ids = ReportDataServer.ConvertToIntList(value);
                ids.Sort();
                Values = new List<object>();
                foreach (var id in ids.Where(id => id >= 0 && id < _valuesList.Count))
                {
                    Values.Add(_valuesList[id]);
                }
            }
        }

        public ParamSingleBdgtLvlsFull()
        {
            Description = "Уровень бюджета";
            MultiSelect = false;
            Values = new List<object>(_valuesList);
        }
    }
}
