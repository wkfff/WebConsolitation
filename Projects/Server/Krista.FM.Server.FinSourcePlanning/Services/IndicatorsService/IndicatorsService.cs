using System.Data;

using Krista.FM.Common;
using Krista.FM.ServerLibrary.FinSourcePlanning;

namespace Krista.FM.Server.FinSourcePlanning.Services.IndicatorsService
{
    public abstract class IndicatorsService : DisposableObject, IIndicatorsService
    {
        /// <summary>
        /// Количество лет для расчета.
        /// </summary>
        public const int yearsCount = 3;
        
        public int YearsCount
        {
            get { return yearsCount; }
        }

        /// <summary>
        /// Производит вычисление индикаторов.
        /// </summary>
        /// <returns>Таблица с результатами.</returns>
        public abstract DataTable CalculateAndAssessIndicators(
                string variantIF, string variantIncome, string variantIncomeYear,
                string variantOutcome, int variantBorrowID);
    
    } 
}