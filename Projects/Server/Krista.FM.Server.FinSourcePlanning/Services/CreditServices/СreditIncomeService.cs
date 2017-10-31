using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

using Krista.FM.Server.Common;

using Krista.FM.ServerLibrary;
using Krista.FM.ServerLibrary.FinSourcePlanning;

namespace Krista.FM.Server.FinSourcePlanning.Services
{
    public class СreditIncomeService : FinSourceBaseService, IСreditIncomeService
    {
        private static СreditIncomeService instance;

        public static СreditIncomeService Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new СreditIncomeService(null);
                }
                return instance;
            }
        }

		private СreditIncomeService(ServerSideObject owner)
			: base(owner)
        {
        }

        public override IFactTable Data
        {
            get
            {
                return FinSourcePlanningFace.Instance.Scheme.FactTables["d3a9668b-0a65-4a6a-bca6-090768c822d0"];
            }
        }

        /// <summary>
        /// расчет текущего остатка долга для всех кредитов полученных по указанному варианту
        /// </summary>
        /// <param name="refVariant"></param>
        public void FillDebtRemainder(int refVariant)
        {
            using (IDatabase db = FinSourcePlanningFace.Instance.Scheme.SchemeDWH.DB)
            {
                using (IDataUpdater du = Data.GetDataUpdater(string.Format("RefVariant = {0}", refVariant), null))
                {
                    DataTable dtData = new DataTable();
                    du.Fill(ref dtData);
                    foreach (DataRow row in dtData.Rows)
                    {
                        int refOKV = Convert.ToInt32(row["RefOKV"]);
                        if (refOKV == -1)
                        {
                            row["DebtRemainder"] = GetReminder(row, db);
                        }
                        else
                        {
                            row["CurrencyDebtRemainder"] = GetReminder(row, db); ;
                        }
                    }

                    DataTable dtChanges = dtData.GetChanges();
                    if (dtChanges != null)
                        du.Update(ref dtChanges);
                }
            }
        }

        private decimal GetReminder(DataRow creditRow, IDatabase db)
        {
            DataTable dtFactAttract = Utils.GetDetailTable(FinSourcePlanningFace.Instance.Scheme, db,
                Convert.ToInt32(creditRow["ID"]), SchemeObjectsKeys.a_S_FactAttractCI_RefCreditInc_Key);
            DataTable dtFactDebt = Utils.GetDetailTable(FinSourcePlanningFace.Instance.Scheme, db,
                Convert.ToInt32(creditRow["ID"]), SchemeObjectsKeys.a_S_FactDebtCI_RefCreditInc_Key);
            int refOKV = Convert.ToInt32(creditRow["RefOKV"]);
            decimal debtRemainder = 0;
            if (refOKV == -1)
            {
                foreach (DataRow detailRow in dtFactAttract.Rows)
                {
                    if (!detailRow.IsNull("Sum"))
                        debtRemainder += Convert.ToDecimal(detailRow["Sum"]);
                }
                foreach (DataRow detailRow in dtFactDebt.Rows)
                {
                    if (!detailRow.IsNull("Sum"))
                        debtRemainder -= Convert.ToDecimal(detailRow["Sum"]);
                }
                return debtRemainder;
            }

            foreach (DataRow detailRow in dtFactAttract.Rows)
            {
                if (!detailRow.IsNull("CurrencySum"))
                    debtRemainder += Convert.ToDecimal(detailRow["CurrencySum"]);
            }
            foreach (DataRow detailRow in dtFactDebt.Rows)
            {
                if (!detailRow.IsNull("CurrencySum"))
                    debtRemainder -= Convert.ToDecimal(detailRow["CurrencySum"]);
            }
            return debtRemainder;
        }
    }
}
