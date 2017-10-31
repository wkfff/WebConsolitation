using System.Collections.Generic;
using System.Linq;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.Domain.Reporitory.NHibernate;
using Krista.FM.Extensions;
using Krista.FM.RIA.Extensions.FO41.Presentation.Controllers;
using Krista.FM.RIA.Extensions.FO41.Presentation.Models;

namespace Krista.FM.RIA.Extensions.FO41.Services
{
    public class FactsService : NHibernateLinqRepository<F_Marks_DataPrivilege>, IFactsService
    {
        private readonly ILinqRepository<F_Marks_Privilege> repositoryEstimate;
        private readonly ILinqRepository<T_Marks_Detail> detailMarksRepository;
        private readonly IIndicatorService extension;
        private readonly AppPrivilegeService requestsRepostiory;
        private readonly IFO41Extension extension41;
        private readonly ICategoryTaxpayerService categoryRepository;

        public FactsService(
            IFO41Extension extension41,
            ILinqRepository<F_Marks_Privilege> repositoryEstimate,
            ILinqRepository<T_Marks_Detail> detailMarksRepository,
            IIndicatorService extension,
            AppPrivilegeService requestsRepostiory,
            ICategoryTaxpayerService categoryRepository)
        {
            this.repositoryEstimate = repositoryEstimate;
            this.detailMarksRepository = detailMarksRepository;
            this.extension = extension;
            this.requestsRepostiory = requestsRepostiory;
            this.extension41 = extension41;
            this.categoryRepository = categoryRepository;
        }

        public object GetQueryOne(int id)
        {
            var r = FindOne(id);
            return new
            {
                r.ID,
                r.PreviousFact,
                r.Fact,
                r.Estimate,
                r.Forecast,
                RefMarks = r.RefMarks.ID,
                RefApplication = r.RefApplication.ID
            };
        }

        public IList<F_Marks_DataPrivilege> GetFactForIndicatorApplic(int indicatorId, int applicationId)
        {
            // Выбираем факты соответствующие заданной заявке, НЕ!!!! только по тем показателям, 
            // у которых в поле «Тип показателя» стоит значение «Сбор» (Code = 1).
            return FindAll()
                .Where(
                    f => f.RefMarks.ID == indicatorId
                    && f.RefApplication.ID == applicationId)
                .ToList();
        }

        /// <summary>
        /// Возвращает данные детализирующий показателей для заданной заявки от налогоплательщика
        /// </summary>
        /// <param name="requestId">Идентификатор заявки от налогоплательщика</param>
        public IList<T_Marks_Detail> GetDetaiFactsForOrg(int requestId)
        {
            // показатели - детализации
            return detailMarksRepository
                .FindAll()
                .Where(f => f.RefApplicOrg.ID == requestId).ToList();
        }

        /// <summary>
        /// Возвращает данные показателей для заданного Id или пустые значения для новой записи
        /// </summary>
        /// <param name="requestId">Id заявки, данные по которой нужно вернуть.</param>
        public IList<F_Marks_DataPrivilege> GetFactsForOrganization(int requestId)
        {
            var facts = new List<F_Marks_DataPrivilege>();

            // если не новая заявка
            if (requestId != -1)
            {
                // Выбираем факты соответствующие заданной заявке, только по тем показателям, 
                // у которых в поле «Тип показателя» стоит значение «Сбор» (Code = 1).
                facts = FindAll()
                    .Where(f => f.RefApplication.ID == requestId && f.RefMarks.RefTypeMark.Code == 1).ToList();
            }

            var newApplication = new D_Application_Privilege();

            // Добавляем пустые записи для районов по которым в базе ни чего не нашлось
            var indicators = extension.FindAll().ToList();

            // добавляем недостающие пустые факты по показателям, 
            // у которых в поле «Тип показателя» стоит значение «Сбор».
            var emptyFacts = indicators
                .Where(i => i.RefTypeMark.Code == 1 && !facts.Any(x => x.RefMarks == i))
                .Select(indicator => new F_Marks_DataPrivilege
                {
                    Estimate = null, 
                    Fact = null, 
                    PreviousFact = null, 
                    Forecast = null, 
                    RefMarks = indicator, 
                    RefApplication = newApplication
                }).ToList();

            facts.AddRange(emptyFacts);

            return facts.OrderBy(x => x.RefMarks.Code).ToList();
        }
        
        /// <summary>
        /// Возвращает данные показателей для заданной категории из источника sourceId за период periodId
        /// </summary>
        /// <param name="sourceId">Идентификатор источника</param>
        /// <param name="periodId">Идентификатор периода</param>
        /// <param name="categoryId">Идентификатор категории</param>
        /// <returns>Список фактов</returns>
        public IList<F_Marks_DataPrivilege> GetFacts(int sourceId, int periodId, int categoryId)
        {
            // выводятся показатели, у которых в поле «Тип показателя» стоит значение «Норматив»
            var facts = FindAll()
                .Where(f => f.SourceID == sourceId &&
                            f.RefApplication.RefYearDayUNV.ID == periodId &&
                            f.RefApplication.RefOrgCategory.ID == categoryId &&
                            f.RefMarks.RefTypeMark.ID == 4)
                .ToList();

            var newApplication = new D_Application_Privilege();

            // Добавляем пустые записи для районов по которым в базе ни чего не нашлось
            var indicators = extension.FindAll().ToList();

            // добавляем недостающие пустые факты по показателям, 
            // у которых в поле «Тип показателя» стоит значение «Сбор».
            var emptyFacts = indicators
                .Where(i => i.RefTypeMark.ID == 4 && !facts.Any(x => x.RefMarks == i))
                .Select(indicator => new F_Marks_DataPrivilege
                                         {
                    Estimate = null,
                    Fact = null,
                    PreviousFact = null,
                    Forecast = null,
                    RefMarks = indicator,
                    RefApplication = newApplication
                }).ToList();

            facts.AddRange(emptyFacts);

            return facts.OrderBy(x => x.RefMarks.Code).ToList();
        }

        /// <summary>
        /// Возвращает данные показателей для заданного Id или пустые значения для новой записи
        /// </summary>
        /// <param name="requestId">Id заявки, данные по которой нужно вернуть.</param>
        public IList<F_Marks_DataPrivilege> GetFactsForOGV(int requestId)
        {
            var facts = new List<F_Marks_DataPrivilege>();

            // если не новая заявка
            if (requestId != -1)
            {
                // Выбираем факты соответствующие заданной заявке, только по тем показателям, 
                // у которых в поле «Тип показателя» стоит значение «Сбор» (Code = 1).
                facts = FindAll()
                    .Where(f => f.RefApplication.RefApplicOGV.ID == requestId && f.RefMarks.RefTypeMark.Code == 1)
                    .ToList();
            }

            var newApplication = new D_Application_Privilege();

            // Добавляем пустые записи для показателям по которым в базе ни чего не нашлось
            var indicators = extension.FindAll().ToList();

            // добавляем недостающие пустые факты по показателям, 
            // у которых в поле «Тип показателя» стоит значение «Сбор».
            var emptyFacts = indicators
                .Where(i => i.RefTypeMark.Code == 1 && !facts.Any(x => x.RefMarks == i))
                .Select(indicator => new F_Marks_DataPrivilege
                {
                    Estimate = null,
                    Fact = null,
                    PreviousFact = null,
                    Forecast = null,
                    RefMarks = indicator,
                    RefApplication = newApplication
                }).ToList();

            facts.AddRange(emptyFacts);

            return facts.OrderBy(x => x.RefMarks.Code).ToList();
        }

        public IList<F_Marks_Privilege> GetFactForIndicatorCategory(int indicatorId, int categoryId)
        {
            // Выбираем факты соответствующие заданной заявке
            return repositoryEstimate
                .FindAll()
                .Where(
                    f => f.RefMarks.ID == indicatorId
                    && f.RefCategory.ID == categoryId)
                .ToList();
        }

        /* для ХМАО */

        /// <summary>
        /// Возвращает данные показателей по заявке и типу налога, как доп. параметр
        /// </summary>
        /// <param name="requestId">Id заявки, данные по которой нужно вернуть.</param>
        /// <param name="taxTypeId">Идентификатор типа налога</param>
        public IList<F_Marks_DataPrivilege> GetFactsForOrganizationHMAO(int requestId, int taxTypeId)
        {
            // если выбран тип налога «Транспортный налог», то не выводить показатель 
            // «Общая сумма капитальных вложений организации на территории 
            // Ханты-Мансийского автономного округа – Югры» 
            var facts = new List<F_Marks_DataPrivilege>();

            // если не новая заявка
            if (requestId > 0)
            {
                // Выбираем факты соответствующие заданной заявке, только по тем показателям, 
                // у которых в поле «Тип показателя» стоит значение «Сбор» (Code = 1) или Справочно (Code = 5).
                facts = FindAll().Where(f => 
                    f.RefApplication.ID == requestId && 
                    f.RefMarks.RefTypeTax != null &&
                    ((f.RefMarks.RefTypeMark.Code == 1 && f.RefMarks.RefTypeTax.ID == taxTypeId) ||
                    (f.RefMarks.RefTypeMark.Code == 5)))
                    .ToList();
            }

            var newApplication = requestsRepostiory.Get(requestId) ?? new D_Application_Privilege();

            // Добавляем пустые записи для районов по которым в базе ни чего не нашлось
            var indicators1 = extension.FindAll()
                .Where(x => x.RefTypeMark.Code == 1 && x.RefTypeTax != null && x.RefTypeTax.ID == taxTypeId)
                .ToList();

            // добавляем недостающие пустые факты по показателям
            var emptyFacts1 = indicators1
                .Where(i => !facts.Any(x => x.RefMarks == i))
                .Select(indicator => new F_Marks_DataPrivilege
                {
                    Estimate = null,
                    Fact = null,
                    PreviousFact = null,
                    Forecast = null,
                    RefMarks = indicator,
                    RefApplication = newApplication
                }).ToList();

            facts.AddRange(emptyFacts1);

            // Добавляем пустые записи для районов по которым в базе ни чего не нашлось
            var indicators5 = extension.FindAll().Where(x => x.RefTypeMark.Code == 5 &&
                    x.RefTypeTax != null &&
                    (x.RefTypeTax.ID == taxTypeId || x.RefTypeTax.ID == -1)).ToList();

            // добавляем недостающие пустые факты по показателям
            AddHelpEmptyFacts(facts, newApplication, indicators5);

           return facts.OrderBy(x => x.RefMarks.Code).ToList();
        }

        /// <summary>
        /// Возвращает данные показателей по категории, периоду и типу налога как доп. параметру
        /// </summary>
        public IList<F_Marks_DataPrivilege> GetFactsForOgvHMAO(int categoryId, int taxTypeId, int periodId)
        {
            // если выбран тип налога «Транспортный налог», то не выводить показатель 
            // «Общая сумма капитальных вложений организации 
            // на территории Ханты-Мансийского автономного округа – Югры» 

            // Выбираем факты соответствующие заданной заявке, только по тем показателям, 
            // у которых в поле «Тип показателя» стоит значение «Сбор» (Code = 1) или Справочно (Code = 5).
            var facts = FindAll()
                .Where(f =>
                       f.RefApplication.RefOrgCategory.ID == categoryId &&
                       f.RefApplication.RefYearDayUNV.ID == periodId &&
                       ((f.RefMarks.RefTypeMark.ID == 1 && (f.RefMarks.RefTypeTax.ID == taxTypeId)) ||
                        (f.RefMarks.RefTypeMark.ID == 5)) &&
                       f.RefMarks.RefTypeTax != null)
                .ToList();
            
            var newApplication = new D_Application_Privilege();

            // Добавляем пустые записи для районов по которым в базе ни чего не нашлось
            var indicators = extension.FindAll().Where(x =>
                (x.RefTypeMark.Code == 1 && x.RefTypeTax != null && (x.RefTypeTax.ID == taxTypeId)) || 
                (x.RefTypeMark.Code == 5)).ToList();

            // добавляем недостающие пустые факты по показателям
            var emptyFacts = indicators
                .Where(i => !facts.Any(x => x.RefMarks == i))
                .Select(indicator => new F_Marks_DataPrivilege
                {
                    Estimate = null,
                    Fact = null,
                    PreviousFact = null,
                    Forecast = null,
                    RefMarks = indicator,
                    RefApplication = newApplication
                }).ToList();

            facts.AddRange(emptyFacts);

            return facts.OrderBy(x => x.RefMarks.Code).ToList();
        }

        public IList<Dictionary<string, object>> GetResultFormByTax(int taxTypeId, int periodId)
        {
            var categories = categoryRepository.GetByTax(taxTypeId).Select(x => x.ID).ToList();
            var marks = extension.FindAll().Where(f =>
                    (f.RefTypeMark.ID == 1 && f.RefTypeTax != null && f.RefTypeTax.ID == taxTypeId) ||
                    (f.RefTypeMark.ID == 5) || f.Code == 300)
                    .OrderBy(f => (f.Code == 300 ? 0 : f.Code));

            var data = new List<Dictionary<string, object>>();

            var requests = requestsRepostiory.FindAll()
                .Where(x => x.RefYearDayUNV.ID == periodId && x.RefStateOrg.ID != 1).ToList();

            foreach (var mark in marks)
            {
                var okeiName = FO41ResultIndicatorsController.GetOkeiName(mark.RefOKEI.ID, mark.RefOKEI.Designation);
                var record = new Dictionary<string, object>
                                 {
                                     { "RefName", mark.Name },
                                     { "RefNumberString", mark.NumberString },
                                     { "RefMarks", mark.Name },
                                     { "OKEI", mark.RefOKEI.Code },
                                     { "Symbol", mark.Symbol },
                                     { "OKEIName", okeiName },
                                     { "IsFormula", mark.Formula != null && !mark.Formula.Equals(string.Empty) },
                                     { "TempID", -mark.ID }
                                 };

                foreach (var category in categories)
                {
                    var cntOrgs = requests.Count(x => x.RefOrgCategory.ID == category);

                    // получили список значений показателя
                    var factsForCategory = GetFactsForOgvHMAO(category, taxTypeId, periodId)
                        .Where(x => x.RefMarks.ID == mark.ID);

                    var factValue = factsForCategory.Sum(f => f.Fact ?? 0);

                    if (mark.RefOKEI.Code == 744 && cntOrgs != 0)
                    {
                        factValue /= cntOrgs;
                    }

                    if (mark.Code == 300)
                    {
                        factValue = cntOrgs;
                    }

                    record.Add("Fact{0}".FormatWith(category), factValue);
                }

                data.Add(record);
            }

            return data;
        }

        /// <summary>
        /// Возвращает данные оценки по категории в периоде
        /// </summary>
        /// <param name="periodId">Идентификатор периода</param>
        /// <param name="categoryId">Идентификатор категории</param>
        /// <param name="taxTypeId">Идентификатор типа налога</param>
        public IEnumerable<IndicatorsModel> GetEstimateDataHMAO(int periodId, int categoryId, int taxTypeId)
        {
            var sourceId = extension41.DataSource(periodId / 10000).ID;
            var facts = repositoryEstimate.FindAll()
                .Where(f =>
                        f.SourceID == sourceId &&
                        f.RefYearDayUNV.ID == periodId &&
                        f.RefCategory.ID == categoryId &&
                        f.RefMarks.RefTypeMark.ID == 4 &&
                        (f.RefMarks.RefTypeTax.ID == taxTypeId || f.RefMarks.RefTypeTax == null || f.RefMarks.RefTypeTax.ID == -1))
                .OrderBy(f => f.RefMarks.Code).ToList();

            return from f in facts
                   select new IndicatorsModel
                   {
                       Fact = f.Fact,
                       RefName = f.RefMarks.Name,
                       RefNumberString = f.RefMarks.NumberString,
                       RefMarks = f.RefMarks.ID,
                       OKEI = f.RefMarks.RefOKEI.Code,
                       OKEIName = FO41ResultIndicatorsController.GetOkeiName(f.RefMarks.RefOKEI.ID, f.RefMarks.RefOKEI.Designation),
                       IsFormula = f.RefMarks.Formula != null && !f.RefMarks.Formula.Equals(string.Empty),
                       Symbol = f.RefMarks.Symbol,
                       TempID = f.ID > 0 ? f.ID : -f.RefMarks.ID,
                   };
        }

        private void AddHelpEmptyFacts(
            List<F_Marks_DataPrivilege> facts, 
            D_Application_Privilege newApplication, 
            IEnumerable<D_Marks_Privilege> indicators5)
        {
            var emptyFacts5 = new List<F_Marks_DataPrivilege>();

            // если заявка новая, указываем предыдущий период
            var periodID = (newApplication.ID > 0) 
                ? newApplication.RefYearDayUNV.ID 
                : extension41.GetPrevPeriod();

            var orgId = (newApplication.ID > 0)
                            ? newApplication.RefOrgPrivilege.ID
                            : (extension41.ResponsOrg == null ? 0 : extension41.ResponsOrg.ID);

            // существующие факты
            var existsFacts = FindAll().Where(x =>
                                              x.RefApplication.RefOrgPrivilege.ID == orgId && 
                                              x.RefApplication.RefYearDayUNV.ID == periodID);

            foreach (var indicator in indicators5.Where(i => !facts.Any(x => x.RefMarks == i)))
            {
                var existsFact = existsFacts.FirstOrDefault(x => x.RefMarks.ID == indicator.ID);
                emptyFacts5.Add(new F_Marks_DataPrivilege
                                    {
                                        Estimate = null,
                                        Fact = existsFact == null ? null : existsFact.Fact,
                                        PreviousFact = existsFact == null ? null : existsFact.PreviousFact,
                                        Forecast = null,
                                        RefMarks = indicator,
                                        RefApplication = newApplication
                                    });
            }

            facts.AddRange(emptyFacts5);
        }
    }
}
