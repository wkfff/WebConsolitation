using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Krista.FM.Common;
using Krista.FM.ServerLibrary.FinSourcePlanning;
using Krista.FM.ServerLibrary;
using Krista.FM.Server.Common;

namespace Krista.FM.Server.FinSourcePlanning.Services
{
    public class CapitalService : FinSourceBaseService, ICapitalService
    {
        private static CapitalService instance;

        public static CapitalService Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new CapitalService(null);
                }
                return instance;
            }
        }

		private CapitalService(ServerSideObject owner)
			: base(owner)
        {
        }

        public override IFactTable Data
        {
            get
            {
                return FinSourcePlanningFace.Instance.Scheme.FactTables["799c95c4-1816-45dc-8faf-1326767c0a98"];
            }
        }

        public void FillDebtRemainder(int refVariant)
        {
            using (IDatabase db = FinSourcePlanningFace.Instance.Scheme.SchemeDWH.DB)
            {
                using (IDataUpdater du = Data.GetDataUpdater(string.Format("RefVariant = {0}", refVariant), null))
                {
                    DataTable dt = new DataTable();
                    du.Fill(ref dt);
                    foreach (DataRow row in dt.Rows)
                    {
                        var reminder = GetReminder(row, db);
                        if (reminder != -1)
                            row["CDebtRemainder"] = reminder;
                    }

                    du.Update(ref dt);
                }
            }
        }

        private decimal GetReminder(DataRow row, IDatabase db)
        {
            var s1 = db.ExecQuery("select sum(Sum) from t_S_CPFactCapital where RefCap = ?", QueryResultTypes.Scalar,
                            new DbParameterDescriptor("p0", row["ID"]));
            var s2 = db.ExecQuery("select sum(Sum) from t_S_CPFactDebt where RefCap = ?", QueryResultTypes.Scalar,
                         new DbParameterDescriptor("p0", row["ID"]));
            if (s1 != null && s1 != DBNull.Value && s2 != null && s2 != DBNull.Value)
            {
                return  Convert.ToDecimal(s1) - Convert.ToDecimal(s2);
            }
            return -1;
        }
    }
}
