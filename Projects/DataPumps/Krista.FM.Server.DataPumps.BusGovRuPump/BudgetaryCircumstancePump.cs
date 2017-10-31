using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using bus.gov.ru.external.Item1;
using bus.gov.ru.types.Item1;
using Krista.FM.Common;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.Extensions;

namespace Krista.FM.Server.DataPumps.BusGovRuPump
{
    /// <summary>
    /// закачка budgetaryCircumstances
    /// </summary>
    public partial class BusGovRuPumpModule
    {
        private List<D_Fin_RazdPodr> finRazdPodrsCache;
        private List<D_Fin_VidRash> finVidRashesCache;
        private List<D_KOSGY_KOSGY> kosgyCache;
        private List<D_Fin_nsiBudget> nsiBudgetsCache;

        private void ProcessBudgetaryCircumstancesPumpFile(FileInfo fileInfo)
        {
            budgetaryCircumstancesType pumpData = budgetaryCircumstances.Load(fileInfo.OpenText()).body.position;

            refNsiConsRegExtendedStrongType targetOrg = pumpData.initiator ?? pumpData.placer;

            if (!CheckDocumentTarget(targetOrg))
            {
                return;
            }

            const string KBKPattern = @"(\d{3}|null|)(\d{4}|null)(\d{7}|null)(\d{3}|null)(\d{3}|null)";

            var headerRepository = Resolver.Get<ILinqRepository<F_F_ParameterDoc>>();

            headerRepository.DbContext.BeginTransaction();

            var header =
                new F_F_ParameterDoc
                    {
                        PlanThreeYear = false,
                        RefPartDoc = _typeDocCache.First(typeDoc => typeDoc.ID == FX_FX_PartDoc.SmetaDocTypeID),
                        RefSost = _stateDocCache.First(stateDoc => stateDoc.ID == FX_Org_SostD.ExportedStateID),
                        RefUchr = _orgStructuresByRegnumCache[targetOrg.regNum],
                        RefYearForm = _yearFormCache.First(form => form.ID == Convert.ToInt32(pumpData.financialYear)),
                    }.Do(SetDocumentSourceId);

            pumpData.budgetaryCircumstance.Each(
                type =>
                    {
                        Match kbkBudget = Regex.Match(type.kbkBudget.code, KBKPattern);
                        new F_Fin_Smeta
                            {
                                Funds = type.circumstance,
                                RefRazdPodr =
                                    finRazdPodrsCache.FirstOrDefault(podr => podr.Code.Equals(kbkBudget.Groups[2].Value)),
                                CelStatya = kbkBudget.Groups[3].Value,
                                RefVidRash =
                                    finVidRashesCache.FirstOrDefault(rash => rash.Code.Equals(kbkBudget.Groups[4].Value)),
                                RefKosgy = kosgyCache.FirstOrDefault(kosgy => kosgy.Code.Equals(kbkBudget.Groups[5].Value)),
                                RefBudget =
                                    nsiBudgetsCache.First(budget => budget.Code.Equals(type.kbkBudget.budget.code)),
                            }
                            .Do(finSmeta => finSmeta.RefParametr = header)
                            .Do(header.Smetas.Add);
                    });

            ProcessDocumentsHeader(
                header,
                pumpData.document,
                type => type.With(x => _documentTypesCache.First(doc => doc.Code.Equals("B"))));

            headerRepository.Save(header);
            headerRepository.DbContext.CommitChanges();
            headerRepository.DbContext.CommitTransaction();
        }

        private void BeforeProcessBudgetaryCircumstancesPumpFiles()
        {
            finRazdPodrsCache = Resolver.Get<ILinqRepository<D_Fin_RazdPodr>>().FindAll().ToList();
            finVidRashesCache = Resolver.Get<ILinqRepository<D_Fin_VidRash>>().FindAll().ToList();
            nsiBudgetsCache = Resolver.Get<ILinqRepository<D_Fin_nsiBudget>>().FindAll().ToList();
            kosgyCache = Resolver.Get<ILinqRepository<D_KOSGY_KOSGY>>().FindAll().ToList();
        }
    }
}
