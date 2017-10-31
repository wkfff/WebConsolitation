using System;
using System.Data;
using Krista.FM.Client.Reports.Database.ClsBridge;

namespace Krista.FM.Client.Reports.Common.CommonParamForm.Books
{
    class ParamOKVED : ParamInfo
    {
        private DataTable dtItems = new DataTable();

        public ParamOKVED()
        {
            Description = "ОКВЭД";
            BookInfo = new ParamBookInfo
                           {
                               MultiSelect = true,
                               HasHierarchy = true,
                               DeepSelect = true,
                               FullScreen = true,
                               EntityKey = b_OKVED_Bridge.InternalKey,
                               DefaultSort = b_OKVED_Bridge.Code,
                               ItemTemplate = String.Format("{0} {1}", b_OKVED_Bridge.Code, b_OKVED_Bridge.Name)
                           };
        }

        public virtual DataTable CreateItemsList()
        {
            if (dtItems.Rows.Count > 0)
            {
                return dtItems;
            }

            const int codeLength = 10;
            const string maskCode = "maskCode";
            dtItems = BookInfo.CreateDataList();

            var column = dtItems.Columns.Add(maskCode, typeof(string));

            foreach (DataRow row in dtItems.Rows)
            {
                var code = Convert.ToString(row[b_OKVED_Bridge.Code]);
                code = code.PadLeft(codeLength, '0');

                for (var i = 1; i < codeLength / 2; i++)
                {
                    code = code.Insert(i * 3 - 1, ".");
                }

                row[maskCode] = code;
            }

            dtItems.Columns.Remove(b_OKVED_Bridge.Code);
            column.ColumnName = b_OKVED_Bridge.Code;
            return dtItems;
        }
    }
}
