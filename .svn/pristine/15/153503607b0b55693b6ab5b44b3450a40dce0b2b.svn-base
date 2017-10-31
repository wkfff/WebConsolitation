using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Krista.FM.Common;
using Krista.FM.ServerLibrary;
using Krista.FM.ServerLibrary.FinSourcePlanning;

namespace Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Server
{
    public partial class FinSourcePlanningServer
    {
        public static void MassCalculatePlanService(int refVariant, int year, CreditsTypes creditsType, IScheme scheme)
        {
            IEntity entity = scheme.RootPackage.FindEntityByName(SchemeObjectsKeys.f_S_Creditissued_Key);
            using (IDataUpdater du = entity.GetDataUpdater("RefVariant = ? and RefSTypeCredit = ?", null,
                new DbParameterDescriptor("p0", refVariant), new DbParameterDescriptor("p1", (int)creditsType)))
            {
                DataTable dtCredits = new DataTable();
                du.Fill(ref dtCredits);
                FinSourcePlanningServer server = new FinSourcePlanningServer();
                server.SetCreditIssuedParams();
                using (IDatabase db = scheme.SchemeDWH.DB)
                {
                    foreach (DataRow row in dtCredits.Rows)
                    {
                        try
                        {
                            Credit credit = new Credit(row);
                            db.ExecQuery("delete from t_S_PlanServiceCO where RefCreditInc = ?", QueryResultTypes.NonQuery,
                                         new DbParameterDescriptor("p0", credit.ID));
                            PercentCalculationParams calculationParams = new PercentCalculationParams();
                            calculationParams.StartDate = credit.StartDate;
                            calculationParams.EndDate = credit.EndDate;
                            calculationParams.EndPeriodDay = 31;
                            calculationParams.PaymentDay = 31;
                            calculationParams.PaymentDayCorrection = DayCorrection.NoCorrection;
                            calculationParams.PaymentsPeriodicity = PayPeriodicity.Year;
                            calculationParams.FormDate = DateTime.Today;
                            calculationParams.CalculationComment = "Массовый расчет плана обслуживания долга";
                        
                            server.CalcDebtServicePlan(credit, year, calculationParams);
                        }
                        catch (Exception)
                        {
                            // в массовых операциях будем гасить исключения.
                        }
                    }
                }
            }
        }

    }
}
