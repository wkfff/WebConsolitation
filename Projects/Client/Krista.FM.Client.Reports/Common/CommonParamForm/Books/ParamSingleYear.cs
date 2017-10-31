using System;
using System.Collections.Generic;

namespace Krista.FM.Client.Reports.Common.CommonParamForm.Books
{
    class ParamSingleYear : ParamInfo
    {
        public class YearListCollection
        {
            public static List<string> GetValuesList()
            {
                var valuesList = new List<string> { Convert.ToString(DateTime.Now.Year) };

                for (var i = 0; i < 6; i++)
                {
                    valuesList.Insert(0, Convert.ToString(DateTime.Now.Year - i - 1));
                    valuesList.Add(Convert.ToString(DateTime.Now.Year + i + 1));
                }

                return valuesList;
            }
        }

        public ParamSingleYear()
        {
            Description = "Период";
            MultiSelect = false;
            Values = new List<object>();

            foreach (var value in YearListCollection.GetValuesList())
            {
                Values.Add(value);
            }
        }
    }
}
