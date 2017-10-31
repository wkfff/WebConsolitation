using System.Data;

using Krista.FM.Common;
using Krista.FM.Server.FinSourcePlanning.Services.IndicatorsService;
using Krista.FM.ServerLibrary.FinSourcePlanning;

namespace Krista.FM.Server.FinSourcePlanning.Services.BKKUIndicators
{
    public class BKKUIndicatorsService : IndicatorsService.IndicatorsService
    {
        /// <summary>
        /// Производит вычисление индикаторов.
        /// </summary>
        /// <returns>Таблица с результатами.</returns>
        public override DataTable CalculateAndAssessIndicators(string variantIF, string variantIncome, string variantIncomeYear, string variantOutcome, int variantBorrowID)
        {
            BKKUIndicatorsList indicatorsList = new BKKUIndicatorsList();
            indicatorsList.LoadIndicatorsData();
            // Создаем кэш для показателей.
            string[] marksClassifiersName = new string[1];
            marksClassifiersName[0] = "d_marks_estimatesdata";
            MarksCasche casche = new MarksCasche(
                variantIF, variantIncome, variantIncomeYear, variantOutcome, variantBorrowID, marksClassifiersName);
            try
            {
                // Проходим по списку индикаторов.
                foreach (Indicator indicator in indicatorsList)
                {
                    indicator.CalculateAndAssess(casche);
                }
            }
            finally
            {
                casche.Dispose();
            }
            // возвращаем таблицу.
            return indicatorsList.ToDataTable();
        }

        #region Одиночка
        private BKKUIndicatorsService()
        {
        }

        private static BKKUIndicatorsService instance;

        public static IIndicatorsService Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new BKKUIndicatorsService();
                }
                return instance;
            }
        }
        #endregion
    }
}