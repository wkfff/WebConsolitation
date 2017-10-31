using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using bus.gov.ru.external.Item1;
using bus.gov.ru.types.Item1;

using Krista.FM.Common;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.Extensions;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Server.DataPumps.BusGovRuPump
{
    /// <summary>
    /// закачка pfhd
    /// financialActivityPlan, actionGrant
    /// </summary>
    public partial class BusGovRuPumpModule : DataPumpModuleBase, IDataPumpModule
    {
        //private List<D_Fin_OtherGant> _dFinOtherGantsCache;
        private Dictionary<string, F_F_ParameterDoc> _pfhdProcessedHeaderCache;

        private void ProcessPfhdPumpFile(FileInfo fileInfo)
        {
            if (fileInfo.Name.Contains("financialActivityPlan"))
            {
                ProcessFinancialActivityPlanPumpFile(fileInfo);
            }
            else if (fileInfo.Name.Contains("actionGrant"))
            {
                ProcessActionGrantPumpFile(fileInfo);
            }
        }

        private void ProcessFinancialActivityPlanPumpFile(FileInfo fileInfo)
        {
            var pumpData = financialActivityPlan.Load(fileInfo.OpenText()).body.position;

            refNsiConsRegExtendedStrongType targetOrg = pumpData.initiator ?? pumpData.placer;

            if (!CheckDocumentTarget(targetOrg))
            {
                return;
            }

            const int PfhdDocTypeID = FX_FX_PartDoc.PfhdDocTypeID;
            const int ExportedStateID = FX_Org_SostD.ExportedStateID;

            var headerRepository = Resolver.Get<ILinqRepository<F_F_ParameterDoc>>();

            headerRepository.DbContext.BeginTransaction();

            var header =
                _pfhdProcessedHeaderCache
                    .If(docs => docs.ContainsKey(targetOrg.regNum))
                    .With(docs => docs[targetOrg.regNum]) ??
                new F_F_ParameterDoc
                    {
                        PlanThreeYear = false,
                        RefPartDoc = _typeDocCache.First(typeDoc => typeDoc.ID == PfhdDocTypeID),
                        RefSost = _stateDocCache.First(stateDoc => stateDoc.ID == ExportedStateID),
                        RefUchr = _orgStructuresByRegnumCache[targetOrg.regNum],
                        RefYearForm = _yearFormCache.First(form => form.ID == Convert.ToInt32(pumpData.financialYear)),
                    }.Do(SetDocumentSourceId);

            header.FinancialActivityPlan.Add(
                new F_Fin_finActPlan
                    {
                        realAssets = pumpData.financialIndex.nonfinancialAssets.realAssets,
                        highValPersAssets = pumpData.financialIndex.nonfinancialAssets.highValuePersonalAssets,
                        totnonfinAssets = pumpData.financialIndex.nonfinancialAssets.total,

                        income = pumpData.financialIndex.financialAssets.debit.income,
                        expense = pumpData.financialIndex.financialAssets.debit.expense,
                        finAssets = pumpData.financialIndex.financialAssets.total,

                        kreditExpir = pumpData.financialIndex.financialCircumstances.kreditExpired,
                        finCircum = pumpData.financialIndex.financialCircumstances.total,

                        stateTaskGrant = pumpData.planPaymentIndex.planInpayments.stateTaskGrant,
                        actionGrant = pumpData.planPaymentIndex.planInpayments.actionGrant,
                        budgetaryFunds = pumpData.planPaymentIndex.planInpayments.budgetaryFunds,
                        paidServices = pumpData.planPaymentIndex.planInpayments.paidServices,
                        planInpayments = pumpData.planPaymentIndex.planInpayments.total,

                        labourRemuneration = pumpData.planPaymentIndex.planPayments.labourRemuneration,
                        telephoneServices = pumpData.planPaymentIndex.planPayments.telephoneServices,
                        freightServices = pumpData.planPaymentIndex.planPayments.freightServices,
                        publicServeces = pumpData.planPaymentIndex.planPayments.publicServices,
                        rental = pumpData.planPaymentIndex.planPayments.rental,
                        maintenanceCosts = pumpData.planPaymentIndex.planPayments.maintenanceCosts,
                        mainFunds = pumpData.planPaymentIndex.planPayments.mainFunds,
                        fictitiousAssets = pumpData.planPaymentIndex.planPayments.fictitiousAssets,
                        tangibleAssets = pumpData.planPaymentIndex.planPayments.tangibleAssets,
                        planPayments = pumpData.planPaymentIndex.planPayments.total,

                        publish = pumpData.planPublicCircumstances,

                        NumberStr = 0,

                        RefParametr = header,
                    });

            ProcessDocumentsHeader(
                header,
                pumpData.document,
                type => type.With(x => _documentTypesCache.First(doc => doc.Code.Equals("P"))));

            headerRepository.Save(header);
            headerRepository.DbContext.CommitChanges();
            headerRepository.DbContext.CommitTransaction();

            if (!_pfhdProcessedHeaderCache.ContainsKey(targetOrg.regNum))
            {
                _pfhdProcessedHeaderCache.Add(targetOrg.regNum, header);
            }
        }

        private void ProcessActionGrantPumpFile(FileInfo fileInfo)
        {
            var pumpData = actionGrant.Load(fileInfo.OpenText()).body.position;

            refNsiConsRegExtendedStrongType targetOrg = pumpData.initiator ?? pumpData.placer;

            if (!CheckDocumentTarget(targetOrg))
            {
                return;
            }

            const int PfhdDocTypeID = FX_FX_PartDoc.PfhdDocTypeID;
            const int ExportedStateID = FX_Org_SostD.ExportedStateID;

            var headerRepository = Resolver.Get<ILinqRepository<F_F_ParameterDoc>>();

            headerRepository.DbContext.BeginTransaction();

            var header =
                _pfhdProcessedHeaderCache
                    .If(docs => docs.ContainsKey(targetOrg.regNum))
                    .With(docs => docs[targetOrg.regNum]) ??
                new F_F_ParameterDoc
                    {
                        PlanThreeYear = false,
                        RefPartDoc = _typeDocCache.First(typeDoc => typeDoc.ID == PfhdDocTypeID),
                        RefSost = _stateDocCache.First(stateDoc => stateDoc.ID == ExportedStateID),
                        RefUchr = _orgStructuresByRegnumCache[targetOrg.regNum],
                        RefYearForm = _yearFormCache.First(form => form.ID == Convert.ToInt32(pumpData.financialYear)),
                    }.Do(SetDocumentSourceId);

            pumpData.realAssetsFunds.Each(
                type => header.RealAssFundses.Add(
                    new F_Fin_realAssFunds
                        {
                            RefParametr = header,
                            funds = type.funds,
                            Name = type.name,
                        }));

            pumpData.capitalConstructionFunds.Each(
                type => header.CapitalConstructionFundses.Add(
                    new F_Fin_CapFunds
                        {
                            RefParametr = header,
                            funds = type.funds,
                            Name = type.name,
                        }));

            pumpData.otherGrantFunds.Each(
                type =>
                    {
                        if (string.IsNullOrEmpty(type.code))
                        {
                            var msg = string.Format("'Информация об операциях с субсидиями на иные цели' regnum={0}, ({1})", targetOrg.regNum, type.Untyped.Value);
                            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeWarning, msg);
                        }
                        else
                        {
                            header.OtherGrantFundses.Add(
                                new F_Fin_othGrantFunds
                                    {
                                        RefParametr = header,
                                        funds = type.funds,
                                        KOSGY = type.kosgu.Return(kosguType => kosguType.code, string.Empty),
                                        RefOtherGrant = RefOtherGrant(type, header),
                                    });
                        }
                    });

            ProcessDocumentsHeader(
                header,
                pumpData.document,
                type => type.With(x => _documentTypesCache.First(doc => doc.Code.Equals("A"))));

            headerRepository.Save(header);
            headerRepository.DbContext.CommitChanges();
            headerRepository.DbContext.CommitTransaction();

            if (!_pfhdProcessedHeaderCache.ContainsKey(targetOrg.regNum))
            {
                _pfhdProcessedHeaderCache.Add(targetOrg.regNum, header);
            }
        }

        private D_Fin_OtherGant RefOtherGrant(actionGrantType.otherGrantFundsLocalType type, F_F_ParameterDoc header)
        {
            try
            {
                return
                    Resolver.Get<ILinqRepository<D_Fin_OtherGant>>().FindAll()
                        .SingleOrDefault(
                            gant => gant.Code.Equals(type.code)
                                    && gant.Name.Equals(type.name)
                                    && gant.RefOrgPPO == header.RefUchr.RefOrgPPO) ??
                    new D_Fin_OtherGant
                        {
                            Code = type.code,
                            Name = type.name,
                            RefOrgPPO = header.RefUchr.RefOrgPPO,
                        };
            }
            catch (InvalidOperationException)
            {
                throw new PumpDataFailedException(
                    string.Format(
                        "Супсидия {0}|{1}|{2}-{3} не найдена. Невозможно обработать документ",
                        type.code,
                        type.name,
                        header.RefUchr.RefOrgPPO.Code,
                        header.RefUchr.RefOrgPPO.Name));
            }
        }

        private void BeforeProcessPfhdPumpFiles()
        {
            _pfhdProcessedHeaderCache = new Dictionary<string, F_F_ParameterDoc>();
        }
    }
}
