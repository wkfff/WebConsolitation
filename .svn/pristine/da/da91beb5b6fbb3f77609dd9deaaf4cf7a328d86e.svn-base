using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Krista.FM.Common;
using Krista.FM.Server.Common;

using Krista.FM.ServerLibrary;
using Krista.FM.ServerLibrary.FinSourcePlanning;

namespace Krista.FM.Server.FinSourcePlanning.Services
{
    public class GuarantIssuedService : FinSourceBaseService, IGuarantIssuedService
    {
        private static GuarantIssuedService instance;

        public static GuarantIssuedService Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new GuarantIssuedService(null);
                }
                return instance;
            }
        }

		private GuarantIssuedService(ServerSideObject owner)
			: base(owner)
        {
        }

        public override IFactTable Data
        {
            get
            {
                return FinSourcePlanningFace.Instance.Scheme.FactTables["042556fd-89a9-4b44-bc3e-2e645560a6bf"];
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
                        var reminderColumnName = Convert.ToInt32(row["RefOKV"]) == -1 ? "GDebtRemainder" : "GCurrDebtRemainder";
                        var reminder = GetReminder(row["ID"], db);
                        if (reminder != -1)
                            row[reminderColumnName] = reminder;
                    }

                    du.Update(ref dt);
                }
            }
        }

        private decimal GetReminder(object guarantyId, IDatabase db)
        {
            var s1 = db.ExecQuery("select sum(Sum) from t_S_FactAttractPrGrnt where RefGrnt = ?", QueryResultTypes.Scalar,
                            new DbParameterDescriptor("p0", guarantyId));
            var s2 = db.ExecQuery("select sum(Sum) from t_S_FactDebtPrGrnt where RefGrnt = ?", QueryResultTypes.Scalar,
                            new DbParameterDescriptor("p0", guarantyId));
            var s3 = db.ExecQuery("select sum(Sum) from t_S_FactAttractGrnt where RefGrnt = ? and (RefTypSum = -1 or RefTypSum = 1)", QueryResultTypes.Scalar,
                            new DbParameterDescriptor("p0", guarantyId));
            decimal remainder = -1;

            if (s1 != null && s1 != DBNull.Value)
            {
                remainder = 0;
                remainder += Convert.ToDecimal(s1);
            }
            if (s2 != null && s2 != DBNull.Value)
            {
                if (remainder == -1)
                    remainder = 0;
                remainder -= Convert.ToDecimal(s2);
            }
            if (s3 != null && s3 != DBNull.Value)
            {
                if (remainder == -1)
                    remainder = 0;
                remainder -= Convert.ToDecimal(s3);
            }

            return remainder;
        }
    }
}
