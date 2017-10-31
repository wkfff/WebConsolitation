using System;
using System.Data;
using System.Linq;
using Krista.FM.Client.Reports.Database.ClsFx;

namespace Krista.FM.Client.Reports.Common.CommonParamForm.Books.FNS
{
    class ParamMarksUFNSFix : ParamInfo
    {
        protected class LocalBookInfo : ParamBookInfo
        {
            private readonly ReportDBHelper dbHelper;

            public LocalBookInfo(ReportDBHelper dbHelper)
            {
                this.dbHelper = dbHelper;
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
                var items = new DataTable();
                items.Columns.Add("id", typeof(int));
                items.Columns.Add("parentid", typeof(int));
                items.Columns.Add(fx_FX_DataMarks65n.Code, typeof(string));
                items.Columns.Add(fx_FX_DataMarks65n.Name, typeof(string));

                var tblMarks = dbHelper.GetEntityData(EntityKey, DefaultSort);

                var dicMarks = tblMarks.Rows.Cast<DataRow>().GroupBy(row => Convert.ToInt32(row[fx_FX_DataMarks65n.Code]));
                foreach (var group in dicMarks)
                {
                    var parent = items.Rows.Add();
                    parent["id"] = group.Key;
                    parent[fx_FX_DataMarks65n.Code] = parent["id"];

                    foreach (var mark in group.OrderBy(mark => mark[fx_FX_DataMarks65n.ID]))
                    {
                        parent["name"] = mark[fx_FX_DataMarks65n.Name];

                        if (Equals(parent["id"], mark[fx_FX_DataMarks65n.ID]))
                        {
                            continue;
                        }

                        var child = items.Rows.Add();
                        child["id"] = mark[fx_FX_DataMarks65n.ID];
                        child["parentid"] = group.Key;
                        child[fx_FX_DataMarks65n.Code] = child["id"];
                        child["name"] = mark[fx_FX_DataMarks65n.NameSubordinate];
                    }
                }

                tblItems = items;
                return items;
            }
        }

        public ParamMarksUFNSFix()
        {
            Description = "Показатели";
            BookInfo = new LocalBookInfo(dbHelper)
                           {
                               HasHierarchy = true,
                               MultiSelect = true,
                               DeepSelect = true,
                               FullScreen = true,
                               EntityKey = fx_FX_DataMarks65n.InternalKey,
                               ItemTemplate = String.Format("{0} {1}", fx_FX_DataMarks65n.Code, fx_FX_DataMarks65n.Name),
                               DefaultSort = "id <> 0"
                           };
        }
    }
}
