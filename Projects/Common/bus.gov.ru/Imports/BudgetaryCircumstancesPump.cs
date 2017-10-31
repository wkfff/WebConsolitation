using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Schema;
using bus.gov.ru;
using bus.gov.ru.types.Item1;
using Krista.FM.Common;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.Extensions;

namespace Bus.Gov.Ru.Imports
{
    // TODO прикрутить этот класс к закачке ProcessBudgetaryCircumstancesPumpFile
    // TODO что бы не дублировать и использовать одно поведение
    
    /// <summary>
    /// Закачка бюджетной сметы
    /// </summary>
    public class BudgetaryCircumstancesPump
    {
        private readonly ILinqRepository<D_Fin_RazdPodr> finRazdPodrsCache;
        private readonly ILinqRepository<D_Fin_VidRash> finVidRashesCache;
        private readonly ILinqRepository<D_KOSGY_KOSGY> kosgyCache;
        private readonly ILinqRepository<D_Fin_nsiBudget> nsiBudgetsCache;

        private readonly CommonPump commonPump;

        public BudgetaryCircumstancesPump()
        {
            commonPump = CommonPump.GetCommonPump;
            
            finRazdPodrsCache = Resolver.Get<ILinqRepository<D_Fin_RazdPodr>>(); 

            finVidRashesCache = Resolver.Get<ILinqRepository<D_Fin_VidRash>>();
            nsiBudgetsCache = Resolver.Get<ILinqRepository<D_Fin_nsiBudget>>();
            kosgyCache = Resolver.Get<ILinqRepository<D_KOSGY_KOSGY>>();
            
            State = FX_Org_SostD.CreatedStateID;
            PlanThreeYear = false;
        }

        public int State { get; set; }

        public bool PlanThreeYear { get; set; }

        /// <summary>
        /// Выполняет закачку из файла
        /// </summary>
        /// <param name="pumpData">
        /// The pump Data.
        /// </param>
        public void PumpFile(budgetaryCircumstancesType pumpData)
        {
            string result;

            // Проверка по схеме
            if (!pumpData.Validate(Resolver.Get<XmlSchemaSet>(), out result))
            {
                throw new Exception(result);
            }

            refNsiConsRegExtendedStrongType targetOrg = pumpData.initiator ?? pumpData.placer;

            // Проверка учреждения
            if (!commonPump.CheckInstTarget(targetOrg))
            {
                /*return;*/
                throw new Exception(
                    "Учреждение regnum={0}{1} не найдено, требуется обновление nsiOgs"
                        .FormatWith(
                            targetOrg.regNum,
                            targetOrg.fullName.Return(
                                s => string.Format("({0})", s),
                                string.Empty)));
            }

            var year = Convert.ToInt32(pumpData.financialYear);
            var uchr = commonPump.OrgStructuresByRegnumCache[targetOrg.regNum];

            // Проверка предыдущих документов
            commonPump.CheckDocs(uchr.ID, FX_FX_PartDoc.SmetaDocTypeID, year, new[] { FX_Org_SostD.ExportedStateID });

            const string KBKPattern = @"(\d{3}|null|)(\d{4}|null)(\d{7}|null)(\d{3}|null)(\d{3}|null)";

            commonPump.HeaderRepository.DbContext.BeginTransaction();

            var header =
                new F_F_ParameterDoc
                    {
                        PlanThreeYear = PlanThreeYear,
                        RefPartDoc = commonPump.TypeDocCache.FindOne(FX_FX_PartDoc.SmetaDocTypeID),
                        RefSost = commonPump.StateDocCache.FindOne(State),
                        RefUchr = uchr,
                        RefYearForm = commonPump.YearFormCache.FindOne(year),
                        OpeningDate = DateTime.Now
                    };

            // Суммируем одинаковые КБК - бюджет
            var smeta = pumpData.budgetaryCircumstance
                            .GroupBy(
                                    s => s.kbkBudget,
                                    (key, rows) => new
                                                    {
                                                        KBKcode = key.code,
                                                        circumstance = rows.Sum(x => x.circumstance),
                                                        budget = key.budget.code
                                                    });

            var razd = finRazdPodrsCache.FindAll().Where(x => x.Code != null).ToList()
                        .Select(x => new 
                                    {
                                        x.ID,
                                        Code = x.Code.PadLeft(4, '0')
                                    }).ToList();
            smeta.Each(
                type =>
                {
                    Match kbkBudget = Regex.Match(type.KBKcode, KBKPattern);

                    var razdID = razd.FirstOrDefault(x => x.Code.Equals(kbkBudget.Groups[2].Value));

                    new F_Fin_Smeta
                        {
                            Funds = type.circumstance,
                            RefRazdPodr = razdID != null ? finRazdPodrsCache.FindOne(razdID.ID) : null,
                            CelStatya = kbkBudget.Groups[3].Value,
                            RefVidRash = finVidRashesCache.FindAll().FirstOrDefault(rash => rash.Code.Equals(kbkBudget.Groups[4].Value)),
                            RefKosgy = kosgyCache.FindAll().FirstOrDefault(kosgy => kosgy.Code.Equals(kbkBudget.Groups[5].Value)),
                            RefBudget = nsiBudgetsCache.FindAll().First(budget => budget.Code.Equals(type.budget)),
                        }
                        .Do(finSmeta => finSmeta.RefParametr = header)
                        .Do(header.Smetas.Add);
                });

            commonPump.ProcessDocumentsHeader(
                header,
                pumpData.document,
                type => type.With(x => commonPump.DocumentTypesCache.FindAll().First(doc => doc.Code.Equals("B"))));

            commonPump.HeaderRepository.Save(header);
            commonPump.HeaderRepository.DbContext.CommitChanges();
            commonPump.HeaderRepository.DbContext.CommitTransaction();
        }
    }
}
