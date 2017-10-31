using System;
using System.Data;
using Krista.FM.Client.Reports.Database.ClsBridge;

namespace Krista.FM.Client.Reports.Common.CommonParamForm.Books.EGRUL
{
    class ParamINN : ParamInfo
    {
        protected class LocalINNBookInfo : ParamBookInfo
        {
            private readonly ReportDBHelper dbHelper;

            public LocalINNBookInfo(ReportDBHelper dbHelper)
            {
                this.dbHelper = dbHelper;
            }

            public virtual string GetTextName()
            {
                return b_Org_EGRUL.INN;
            }

            public override DataRow[] GetDataRows(string id = null)
            {
                if (tblItems.Rows.Count == 0)
                {
                    CreateDataList();
                }

                return tblItems.Select(GetIdFilter(id));
            }

            public override DataTable CreateDataList()
            {
                var entity = ConvertorSchemeLink.GetEntity(b_Org_EGRUL.InternalKey);
                var selectStr = String.Format(
                    "select distinct {0} as id, {0} as name from {1} where {0} is not null and {2} order by {0}",
                    GetTextName(),
                    entity.FullDBName,
                    String.Format("{0} = 1", b_Org_EGRUL.Last));
                tblItems = dbHelper.GetTableData(selectStr);
                return tblItems;
            }
        }

        public ParamINN()
        {
            Description = "ИНН";

            BookInfo = new LocalINNBookInfo(dbHelper)
                           {
                               FullScreen = true,
                               MultiSelect = true
                           };
        }
    }
}
