using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.Extensions;

namespace Krista.FM.RIA.Extensions.MarksOMSU.Services
{
    public class IneffExpencesService : IIneffExpencesService
    {
        private readonly IMarksRepository marksRepository;
        private readonly IMarksOmsuRepository marksOmsuRepository;
        private readonly IMarksCalculator marksCalculator;
        private readonly ILinqRepository<F_OMSU_Reg16> factsRepository;
        private readonly IMarksDataInitializer dataInitializer;

        public IneffExpencesService(
            IMarksRepository marksRepository,
            IMarksOmsuRepository marksOmsuRepository,
            IMarksCalculator marksCalculator,
            ILinqRepository<F_OMSU_Reg16> factsRepository,
            IMarksDataInitializer dataInitializer)
        {
            this.marksRepository = marksRepository;
            this.marksOmsuRepository = marksOmsuRepository;
            this.marksCalculator = marksCalculator;
            this.factsRepository = factsRepository;
            this.dataInitializer = dataInitializer;
        }

        public D_OMSU_MarksOMSU GetMarkIneffGkh()
        {
            return GetMarkIneff("ЖКХ");
        }

        public D_OMSU_MarksOMSU GetMarkIneffOmu()
        {
            return GetMarkIneff("ОМУ");
        }

        public D_OMSU_MarksOMSU GetMarkIneffEducation()
        {
            return GetMarkIneff("Образование");
        }

        public D_OMSU_MarksOMSU GetMarkIneffMedicine()
        {
            return GetMarkIneff("Здравоохранение");
        }

        public void CalculateFactToRam(F_OMSU_Reg16 fact)
        {
            marksCalculator.CalcForceProtectedDoNotSave(new List<F_OMSU_Reg16> { fact }, fact.RefRegions.ID, false);
        }

        public IEnumerable<F_OMSU_Reg16> GetFacts(int markId)
        {
            return marksOmsuRepository.GetCurrentYearFactsOfAllRegions(markId);
        }

        public F_OMSU_Reg16 GetFact(int markId, int regionId)
        {
            return marksOmsuRepository.GetFactForMarkRegion(markId, regionId);
        }

        public IDictionary<D_OMSU_MarksOMSU, int> GetMarkCalculationPlan(int markId)
        {
            return marksCalculator.GetMarkCalculationPlan(markId);
        }

        public DataTable GetTargetFactsViewModel(int targetMarkId, bool withSourceFacts)
        {
            var viewModel = new DataTable();

            viewModel.Columns.Add(new DataColumn("ID", typeof(int)));
            viewModel.Columns.Add(new DataColumn("RegionID", typeof(int)));
            viewModel.Columns.Add(new DataColumn("RefStatusData", typeof(int)));
            viewModel.Columns.Add(new DataColumn("HasWarning", typeof(string)));
            viewModel.Columns.Add(new DataColumn("NameMO", typeof(string)));

            viewModel.Columns.Add(new DataColumn("HasWarnPrior", typeof(string)));
            viewModel.Columns.Add(new DataColumn("PriorApproved", typeof(decimal)));
            viewModel.Columns.Add(new DataColumn("PriorCalc", typeof(decimal)));
            viewModel.Columns.Add(new DataColumn("HasWarnCurrent", typeof(string)));
            viewModel.Columns.Add(new DataColumn("CurrentApproved", typeof(decimal)));
            viewModel.Columns.Add(new DataColumn("CurrentCalc", typeof(decimal)));

            IDictionary<D_OMSU_MarksOMSU, int> calculationPlan = null;
            if (withSourceFacts)
            {
                calculationPlan = GetMarkCalculationPlan(targetMarkId);
                foreach (var sourceFactWithLevel in calculationPlan)
                {
                    viewModel.Columns.Add(new DataColumn("Prior" + sourceFactWithLevel.Key.ID, typeof(decimal)));
                    viewModel.Columns.Add(new DataColumn("Current" + sourceFactWithLevel.Key.ID, typeof(decimal)));
                }
            }

            foreach (var fact in GetFacts(targetMarkId))
            {
                var precision = fact.RefMarksOMSU.Capacity ?? 0;

                var priorValueOriginal = fact.PriorValue ?? 0;
                var currentValueOriginal = fact.CurrentValue ?? 0;
                CalculateFactToRam(fact);
                var priorValueRecalculated = fact.PriorValue ?? 0;
                var currentValueRecalculated = fact.CurrentValue ?? 0;

                var hasWarnPrior = Math.Round(priorValueOriginal, precision) != Math.Round(priorValueRecalculated, precision);
                var hasWarnCurrent = Math.Round(currentValueOriginal, precision) != Math.Round(currentValueRecalculated, precision);
                var hasWarning = hasWarnPrior || hasWarnCurrent;
                
                var currentRow = viewModel.NewRow();
                currentRow.BeginEdit();
                
                currentRow.SetField("ID", fact.ID);
                currentRow.SetField("RegionID", fact.RefRegions.ID);
                currentRow.SetField("RefStatusData", fact.RefStatusData.ID);
                currentRow.SetField("HasWarning", hasWarning ? "!" : string.Empty);
                currentRow.SetField("NameMO", fact.RefRegions.Name);

                currentRow.SetField("HasWarnPrior", hasWarnPrior ? "!" : string.Empty);
                currentRow.SetField("PriorApproved", priorValueOriginal);
                currentRow.SetField("PriorCalc", priorValueRecalculated);
                currentRow.SetField("HasWarnCurrent", hasWarnCurrent ? "!" : string.Empty);
                currentRow.SetField("CurrentApproved", currentValueOriginal);
                currentRow.SetField("CurrentCalc", currentValueRecalculated);

                if (withSourceFacts)
                {
                    foreach (var sourceFactWithLevel in GetSourceFactsWithHierarchy(calculationPlan, fact.RefRegions.ID))
                    {
                        currentRow.SetField("Prior" + sourceFactWithLevel.Key.RefMarksOMSU.ID, sourceFactWithLevel.Key.PriorValue);
                        currentRow.SetField("Current" + sourceFactWithLevel.Key.RefMarksOMSU.ID, sourceFactWithLevel.Key.CurrentValue);
                    }                    
                }

                currentRow.EndEdit();
                viewModel.Rows.Add(currentRow);
            }

            return viewModel;
        }

        public IDictionary<F_OMSU_Reg16, int> GetSourceFactsWithHierarchy(
            IDictionary<D_OMSU_MarksOMSU, int> markCalculationPlan, int regionId)
        {
            var result = new Dictionary<F_OMSU_Reg16, int>();

            // Следующий foreach в LINQ не конвертировать: со StyleCop придется долго спорить из-за вида результата.
            foreach (var sourceMarkWithLevel in markCalculationPlan)
            {
                var sourceMark = sourceMarkWithLevel.Key;
                var sourceMarkLevel = sourceMarkWithLevel.Value;

                var sourceFactQuery = factsRepository.FindAll().Where(x => x.RefMarksOMSU.ID == sourceMark.ID && x.RefRegions.ID == regionId);
                if (sourceFactQuery.Count() > 0)
                {
                    result.Add(sourceFactQuery.First(), sourceMarkLevel);
                }
            }

            return result;
        }

        private D_OMSU_MarksOMSU GetMarkIneff(string wayIneffExp)
        {
            var allMarks = marksRepository.FindAll()
                .Where(x =>
                    (x.WayIneffExp == wayIneffExp)
                    && (x.RefTypeMark.ID == (int)TypeMark.Calculated)
                    && (!x.Grouping)
                    && (x.CodeStructure == "1"));

            if (allMarks.Count() == 0)
            {
                throw new InvalidDataException("Целевой показатель неэффективных расходов {0} не найден".FormatWith(wayIneffExp));
            }

            if (allMarks.Count() > 1)
            {
                throw new InvalidDataException("Целевой показатель неэффективных расходов {0} существует более чем в одном экземпляре".FormatWith(wayIneffExp));
            }

            var mark = allMarks.First();
            dataInitializer.CreateRegionsForMark(wayIneffExp, mark.ID);

            return mark;
        }
    }
}
