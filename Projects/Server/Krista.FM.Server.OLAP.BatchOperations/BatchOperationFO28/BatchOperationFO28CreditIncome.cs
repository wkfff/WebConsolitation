using System;
using System.Collections.Generic;
using System.Text;
using Krista.FM.Server.Common;
using Krista.FM.Server.OLAP.BatchOperations.BatchOperationFO28;
using Krista.FM.Server.ProcessorLibrary;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Server.OLAP.BatchOperations
{

    // формирует служебную таблицу "Кредиты полученные Служебная" (f_S_CreditincomeCube)
    public class BatchOperationFO28CreditIncome : BatchOperationFO28Base 
    {

        public BatchOperationFO28CreditIncome(Guid batchId, IMDProcessingProtocol protocol, IScheme scheme)
            : base(batchId, protocol, scheme)
        {
        }

        public override string Name
        {
            get
            {
                return String.Format("Формирование служебной таблицы 'ИФ.Кредиты полученные Служебная'");
            }
        }

        private const string CI_MASTER_QUERY = "m.SourceId SourceId, m.Num Num, m.ContractDate FactDate, m.Sum Sum, " +
                                               "m.RefOrganizations RefOrganizations, m.RefSTypeCredit RefSTypeCredit";
        private const string F_CREDIT_INCOME_CUBE_GUID = "712a6f76-72b8-4a16-8548-2ac70f8afa37";
        public override string Execute()
        {
            startTime = DateTime.Now;

            try
            {
                string[] detailMapping = new string[] { "d.Sum PlanAttractCI, d.StartDate RefYearDayUNV", "t_S_PlanAttractCI", 
                                                        "d.Sum FactAttractCI, d.FactDate RefYearDayUNV", "t_S_FactAttractCI",                                           
                                                        "d.Sum PlanDebtCI, d.EndDate RefYearDayUNV", "t_S_PlanDebtCI",
                                                        "d.Sum FactDebtCI, d.FactDate RefYearDayUNV", "t_S_FactDebtCI",
                                                        "d.Sum PlanServiceCI, d.EndDate RefYearDayUNV", "t_S_PlanServiceCI", 
                                                        "d.Sum FactPercentCI, d.FactDate RefYearDayUNV", "t_S_FactPercentCI",
                                                        "d.Sum FactPenaltyDebtCI, d.FactDate RefYearDayUNV", "t_S_FactPenaltyDebtCI",
                                                        "d.Sum FactPenaltyPercentCI, d.FactDate RefYearDayUNV", "t_S_FactPenaltyPercentCI" };
                IFactTable fctCreditIncomeCube = scheme.FactTables[F_CREDIT_INCOME_CUBE_GUID];
                FO28FormAuxCubeTable(fctCreditIncomeCube, "f_S_Creditincome", CI_MASTER_QUERY, detailMapping, "RefCreditInc");

                Trace.TraceVerbose(
                        "{0} Время выполнения дополнительных операций Формирование служебной таблицы 'ИФ.Кредиты полученные Служебная для объекта \"{1}\" {2}",
                        Authentication.UserDate, "ИФ.Кредиты полученные", DateTime.Now - startTime);

                protocol.WriteEventIntoMDProcessingProtocol("Krista.FM.Server.OLAP.BatchOperations",
                    MDProcessingEventKind.mdpeInformation,
                    String.Format("Время выполнения дополнительных операций Формирование служебной таблицы 'ИФ.Кредиты полученные Служебная {0}", DateTime.Now - startTime),
                    "ИФ.Кредиты полученные", F_CREDIT_INCOME_CUBE_GUID, OlapObjectType.Cube, batchId.ToString());

                return string.Empty;
            }
            catch (FO28BaseException e)
            {
                string errorMessage = String.Format("Ошибка при выполнении доп.операций BatchOperationFO28CreditIncome для объекта \"{0}\": {1}",
                    "ИФ.Кредиты полученные",
                    Krista.Diagnostics.KristaDiagnostics.ExpandException(e));

                Trace.TraceError(errorMessage);

                protocol.WriteEventIntoMDProcessingProtocol(
                    "Krista.FM.Server.OLAP.BatchOperations",
                    MDProcessingEventKind.mdpeError,
                    Krista.Diagnostics.KristaDiagnostics.ExpandException(e),
                    "ИФ.Кредиты полученные", F_CREDIT_INCOME_CUBE_GUID, OlapObjectType.Cube, batchId.ToString());

                return errorMessage;
            }
        }

    }

}
