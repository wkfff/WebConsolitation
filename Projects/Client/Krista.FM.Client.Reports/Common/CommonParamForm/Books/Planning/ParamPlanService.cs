using System;
using System.Collections.Generic;
using System.Data;
using Krista.FM.Client.Reports.Database.FactTables.Planning.CreditIncome;

namespace Krista.FM.Client.Reports.Common.CommonParamForm.Books.Planning
{
    class ParamPlanService : ParamInfo
    {
        private const string DisplayColumn = "CALC_NAME";
        public object num = null;
        public object key = null;

        public ParamPlanService()
        {
            Description = "План обслуживания";
            MultiSelect = false;
            Table = CreateTable();
            DisplayFieldName = DisplayColumn;

            Columns = new List<string>();

            for (var i = 2; i < Table.Columns.Count - 1; i++)
            {
                Columns.Add(Table.Columns[i].ColumnName);
            }
        }

        public DataTable CreateTable()
        {
            var tblResult = new DataTable();

            for (var i = 0; i < 4; i++)
            {
                tblResult.Columns.Add(Convert.ToString(i), typeof(string));
            }

            tblResult.Columns[0].ColumnName = ID;
            tblResult.Columns[1].ColumnName = NAME;
            tblResult.Columns[tblResult.Columns.Count - 1].ColumnName = DisplayColumn;

            if ((num != null && num != DBNull.Value) || (key != null && key != DBNull.Value))
            {
                var creditEntity = ConvertorSchemeLink.GetEntity(f_S_Creditincome.internalKey);
                var fltField = key == null ? f_S_Creditincome.Num : f_S_Creditincome.id;
                var fltValue = key ?? String.Format("'{0}'", num);
                var fltContract = String.Format("{0} = {1}", fltField, fltValue);
                var tblCredit = dbHelper.GetEntityData(creditEntity, fltContract);

                if (tblCredit.Rows.Count > 0)
                {
                    var rowCredit = tblCredit.Rows[0];
                    var planEntity = ConvertorSchemeLink.GetEntity(t_S_PlanServiceCI.internalKey);
                    var tblPlanVariant = dbHelper.GetTableData(
                        String.Format("select distinct {0}, {1}, {2} from {3} where {4} = {5} order by {0}, {2}",
                                      t_S_PlanServiceCI.EstimtDate,
                                      t_S_PlanServiceCI.CalcComment,
                                      t_S_PlanServiceCI.CalcDate,
                                      planEntity.FullDBName,
                                      t_S_PlanServiceCI.RefCreditInc,
                                      rowCredit[f_S_Creditincome.id]));

                    foreach (DataRow rowPlan in tblPlanVariant.Rows)
                    {
                        var date = rowPlan[t_S_PlanServiceCI.EstimtDate];

                        if (date == DBNull.Value)
                        {
                            continue;
                        }

                        var rowInsert = tblResult.Rows.Add();
                        rowInsert[0] = Convert.ToDateTime(date).ToShortDateString();
                        rowInsert[1] = rowInsert[0];
                        rowInsert[2] = rowPlan[t_S_PlanServiceCI.CalcComment];
                        rowInsert[DisplayColumn] = String.Format("{0} {1}", rowInsert[1], rowInsert[2]);
                    }
                }
            }

            return tblResult;
        }
    }
}
