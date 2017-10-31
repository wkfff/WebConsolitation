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
    public class BatchOperationFO28CreditIssued : BatchOperationFO28Base
    {

        public BatchOperationFO28CreditIssued(Guid batchId, IMDProcessingProtocol protocol, IScheme scheme)
            : base(batchId, protocol, scheme)
        {
        }

        public override string Name
        {
            get
            {
                return String.Format("Формирование служебной таблицы 'ИФ.Кредиты предоставленные Служебная'");
            }
        }

        private const string CO_MASTER_QUERY = "m.SourceId SourceId, m.Num Num, m.DocDate FactDate, m.Sum Sum, " +
                                               "m.RefOrganizations RefOrganizations, m.RefSTypeCredit RefSTypeCredit";
        private const string F_CREDIT_ISSUED_CUBE_GUID = "fdb6623f-43d3-4d4a-8938-a9efa5f9c5ef";
        public override string Execute()
        {
            startTime = DateTime.Now;

            try
            {
                string[] detailMapping = new string[] { "d.Sum PlanAttractCO, d.StartDate RefYearDayUNV", "t_S_PlanAttractCO", 
                                                        "d.Sum FactAttractCO, d.FactDate RefYearDayUNV", "t_S_FactAttractCO",                                           
                                                        "d.Sum PlanDebtCO, d.EndDate RefYearDayUNV", "t_S_PlanDebtCO",
                                                        "d.Sum FactDebtCO, d.FactDate RefYearDayUNV", "t_S_FactDebtCO",
                                                        "d.Sum PlanServiceCO, d.EndDate RefYearDayUNV", "t_S_PlanServiceCO", 
                                                        "d.Sum FactPercentCO, d.FactDate RefYearDayUNV", "t_S_FactPercentCO",
                                                        "d.Sum FactPenaltyDebtCO, d.FactDate RefYearDayUNV", "t_S_FactPenaltyDebtCO",
                                                        "d.Sum FactPenaltyPercentCO, d.FactDate RefYearDayUNV", "t_S_FactPenaltyPercentCO" };
                IFactTable fctCreditIssuedCube = scheme.FactTables[F_CREDIT_ISSUED_CUBE_GUID];
                FO28FormAuxCubeTable(fctCreditIssuedCube, "f_S_Creditissued", CO_MASTER_QUERY, detailMapping, "RefCreditInc");

                Trace.TraceVerbose(
                        "{0} Время выполнения дополнительных операций Формирование служебной таблицы 'ИФ.Кредиты предоставленные Служебная для объекта \"{1}\" {2}",
                        Authentication.UserDate, "ИФ.Кредиты предоставленные", DateTime.Now - startTime);

                protocol.WriteEventIntoMDProcessingProtocol("Krista.FM.Server.OLAP.BatchOperations",
                    MDProcessingEventKind.mdpeInformation,
                    String.Format("Время выполнения дополнительных операций Формирование служебной таблицы 'ИФ.Кредиты предоставленные Служебная {0}", DateTime.Now - startTime),
                    "ИФ.Кредиты предоставленные", F_CREDIT_ISSUED_CUBE_GUID, OlapObjectType.Cube, batchId.ToString());

                return string.Empty;
            }
            catch (FO28BaseException e)
            {
                string errorMessage = String.Format("Ошибка при выполнении доп.операций BatchOperationFO28CreditIssued для объекта \"{0}\": {1}",
                    "ИФ.Кредиты предоставленные",
                    Krista.Diagnostics.KristaDiagnostics.ExpandException(e));

                Trace.TraceError(errorMessage);

                protocol.WriteEventIntoMDProcessingProtocol(
                    "Krista.FM.Server.OLAP.BatchOperations",
                    MDProcessingEventKind.mdpeError,
                    Krista.Diagnostics.KristaDiagnostics.ExpandException(e),
                    "ИФ.Кредиты предоставленные", F_CREDIT_ISSUED_CUBE_GUID, OlapObjectType.Cube, batchId.ToString());

                return errorMessage;
            }
        }

    }

}
