using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core.NHibernate;
using Krista.FM.RIA.Extensions.FO41.Services;

namespace Krista.FM.RIA.Extensions.FO41.Helpers
{
    public class FO41EstimateCalculator : Calculator
    {
        private readonly IndicatorService indicatorRepository;
        private readonly FormulaService formulaRepository;
        private readonly ILinqRepository<F_Marks_Privilege> factsEstimateRepository;
        private readonly IRepository<D_Marks_NormPrivilege> normRepository;
        private readonly CategoryTaxpayerService categoryRepository;
        private readonly IFO41Extension extension;
        private readonly IRepository<FX_Date_YearDayUNV> periodRepository;
        private D_Application_FromOGV appFromOGV;
        
        public FO41EstimateCalculator(
            IFO41Extension extension,
            FormulaService formulaRepository, 
            IndicatorService indicatorRepository, 
            ILinqRepository<F_Marks_Privilege> factsEstimateRepository,
            IRepository<D_Marks_NormPrivilege> normRepository,
            CategoryTaxpayerService categoryRepository, 
            IRepository<FX_Date_YearDayUNV> periodRepository)
        {
            this.extension = extension;
            this.periodRepository = periodRepository;
            this.formulaRepository = formulaRepository;
            this.indicatorRepository = indicatorRepository;
            this.factsEstimateRepository = factsEstimateRepository;
            this.normRepository = normRepository;
            this.categoryRepository = categoryRepository;
            CacheEstimate = new Dictionary<long, F_Marks_Privilege>();
            Pattern = "([A-Za-zА-Яа-я]+[A-Za-zА-Яа-я0-9]*\\[((Fact)|(PreviousFact)|(Estimate)|(Forecast))\\])";
        }

        protected Dictionary<long, F_Marks_Privilege> CacheEstimate { get; set; }

        [Transaction]
        public void Calc(
            IList<F_Marks_Privilege> facts, 
            D_Application_FromOGV applicationFromOGV, 
            bool calculatePrognoz)
        {
            appFromOGV = applicationFromOGV;

            // Этап 1. Вычисление зависимостей показателей
            if (CalcTree == null)
            {
                CalcTree = PrepareCalcTree();
            }

            var categoryId = applicationFromOGV.RefOrgCategory.ID;

            CalcPeriod(
                facts, 
                categoryId, 
                fact => fact.PreviousFact, 
                (value, fact) => fact.PreviousFact = value, 
                "PreviousFact");
            CalcPeriod(
                facts, 
                categoryId, 
                fact => fact.Fact, 
                (value, fact) => fact.Fact = value, 
                "Fact");
            CalcPeriod(
                facts, 
                categoryId, 
                fact => fact.Estimate, 
                (value, fact) => fact.Estimate = value, 
                    "Estimate");
            CalcPeriod(
                facts, 
                categoryId, 
                fact => fact.Forecast, 
                (value, fact) => fact.Forecast = value,
                "Forecast");
        }

        [Transaction]
        public void Calc(
            IList<F_Marks_Privilege> facts, 
            int categoryId, 
            int periodId, 
            int taxTypeId, 
            bool calculatePrognoz)
        {
            // Этап 1. Вычисление зависимостей показателей
            if (CalcTree == null)
            {
                CalcTree = PrepareCalcTree();
            }

            CalcPeriod(facts, categoryId, periodId, fact => fact.Fact, (value, fact) => fact.Fact = value, "Fact");
        }

        public F_Marks_Privilege FactResolverEstimate(int markId, int categoryId, int sourceId, int periodId)
        {
            long key = markId + (categoryId << 16);
            F_Marks_Privilege fact;
            CacheEstimate.TryGetValue(key, out fact);
            if (fact == null)
            {
                var facts = factsEstimateRepository.FindAll().Where(
                    f => f.RefMarks.ID == markId
                    && f.RefCategory.ID == categoryId
                    && f.RefYearDayUNV.ID == periodId
                    && f.SourceID == sourceId)
                .ToList();

                fact = (facts.Count > 0)
                    ? facts.First()
                    : new F_Marks_Privilege
                    {
                        RefMarks = indicatorRepository.Get(markId),
                        RefCategory = categoryRepository.Get(categoryId),
                        RefYearDayUNV = periodRepository.Get(periodId),
                        SourceID = sourceId
                    };

                CacheEstimate.Add(key, fact);
            }

            return fact;
        }

        protected override Node GetDependedNode(IEnumerable<Node> data, Match match1)
        {
            var start = match1.Value.IndexOf("[");
            var finish = match1.Value.IndexOf("]");
            var len = match1.Value.Length + start - finish - 1;
            var match1PeriodName = match1.Value.Substring(start, len);

            return data.FirstOrDefault(x => 
                x.PeriodName == match1PeriodName && 
                x.Symbol == match1.Value.Substring(0, start));
        }

        private static void PreEdit(IEnumerable<Node> data, string pattern)
        {
            foreach (var node in data)
            {
                if (node.Formula.IsNullOrEmpty())
                {
                    continue;
                }

                var start = node.Formula.IndexOf("=");
                if (start > 0)
                {
                    node.Formula = node.Formula.Remove(0, start + 1);
                }

                var matches = Regex.Matches(node.Formula, pattern);

                foreach (Match match in matches)
                {
                    start = match.Value.IndexOf('[');
                    var newValue = "get({0},{1})".FormatWith(
                        match.Value.Substring(0, start),
                        match.Value.Substring(start + 1, match.Value.Length - start - 2));
                    node.Formula = node.Formula.Replace(match.Value, newValue);
                }
            }
        }

        private void CalcPeriod(
            IEnumerable<F_Marks_Privilege> facts,
            int categoryId,
            FO41EstimateFormulaEvaluter.ParameterResolverDelegat resolver,
            FO41EstimateFormulaEvaluter.ParameterSetterDelegat setter,
            string periodName)
        {
            var periodId = appFromOGV.RefYearDayUNV.ID;
            CalcPeriod(facts, categoryId, periodId, resolver, setter, periodName);
        }

        private void CalcPeriod(
            IEnumerable<F_Marks_Privilege> facts,
            int categoryId,
            int periodId,
            FO41EstimateFormulaEvaluter.ParameterResolverDelegat resolver,
            FO41EstimateFormulaEvaluter.ParameterSetterDelegat setter,
            string periodName)
        {
            // Этап 2. Нахождение оптимальной последовательности вычислений
            var marks = facts
                .Select(fact => CalcTree.FirstOrDefault(x =>
                    x.ID == fact.RefMarks.ID &&
                    x.PeriodName.Equals(periodName)))
                .ToList();
            marks = marks.Where(x => x != null).ToList();
            var sequence = SearchSolutions(marks);

            // Этап 3. Выполнение вычислений
            var sourceId = extension.DataSource(periodId / 10000).ID;

            foreach (var formula in from node in sequence
                                    where !node.Formula.IsNullOrEmpty()
                                    select new FO41EstimateFormulaEvaluter(
                                        node,
                                        CalcTree,
                                        factsEstimateRepository,
                                        normRepository,
                                        categoryRepository,
                                        categoryId,
                                        sourceId,
                                        periodId,
                                        FactResolverEstimate,
                                        indicatorRepository))
            {
                formula.Calc(resolver, setter);
            }
        }

        private IEnumerable<Node> PrepareCalcTree()
        {
            var data = (from m in indicatorRepository.FindAll()
                        where m.Symbol != null
                        select new Node
                                   {
                                       ID = m.ID, 
                                       Symbol = m.Symbol,
                                       Formula = formulaRepository.GetForIndicator(m.ID, "Fact")
                                   }).ToList();
            
            var dataAll = new List<Node>();
            foreach (var node in data)
            {
                node.PeriodName = "Fact";
                if (!dataAll.Contains(node))
                {
                    dataAll.Add(node);
                }

                var newPreviousFact = new Node
                                  {
                                      ID = node.ID,
                                      Symbol = node.Symbol,
                                      Formula = formulaRepository.GetForIndicator(node.ID, "PreviousFact"),
                                      PeriodName = "PreviousFact"
                                  };

                if (!data.Contains(newPreviousFact))
                {
                    dataAll.Add(newPreviousFact);
                }

                var newEstimate = new Node
                {
                    ID = node.ID,
                    Symbol = node.Symbol,
                    Formula = formulaRepository.GetForIndicator(node.ID, "Estimate"),
                    PeriodName = "Estimate"
                };

                if (!data.Contains(newEstimate))
                {
                    dataAll.Add(newEstimate);
                }

                var newForecast = new Node
                {
                    ID = node.ID,
                    Symbol = node.Symbol,
                    Formula = formulaRepository.GetForIndicator(node.ID, "Forecast"),
                    PeriodName = "Forecast"
                };

                if (!data.Contains(newForecast))
                {
                    dataAll.Add(newForecast);
                }
            }

            // удаляем присваивание вычисляемому параметру: из A = B + C получаем B + C
            PreEdit(dataAll, Pattern);

            // По формулам определяем от кого зависит вычисляемый элемент
            FindDependency(dataAll);

            // По зависимостям вычисляемых элементов строим обратные зависимости
            BuildDependTree(dataAll);

            return dataAll;
        }
    }
}
