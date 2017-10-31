using System.Collections.Generic;
using Krista.FM.Domain;
using Krista.FM.RIA.Extensions.EO15TargetPrograms.Services;

namespace Krista.FM.RIA.Extensions.EO15TargetPrograms.Helpers
{
    public static class F_ExcCosts_FinanceExtensions
    {
        /// <summary>
        /// Возвращает список по годам, начиная с года начала и заканчивая годом окончания программы
        /// </summary>
        public static IList<int> GetYears(this D_ExcCosts_ListPrg program)
        {
            var result = new List<int>();
            int beginYear = program.RefBegDate.GetYear();
            int endYear = program.RefEndDate.GetYear();

            for (int i = beginYear; i <= endYear; i++)
            {
                result.Add(i);
            }

            return result;
        }

        /// <summary>
        /// Возвращает список по годам, начиная с предыдущего от начала года и заканчивая годом окончания программы
        /// </summary>
        public static IList<int> GetYearsWithPreviousAndFollowing(this D_ExcCosts_ListPrg program)
        {
            var result = new List<int>();
            int beginYear = program.RefBegDate.GetYear();
            int endYear = program.RefEndDate.GetYear();

            for (int i = beginYear - 1; i <= endYear + 1; i++)
            {
                result.Add(i);
            }

            return result;
        }
    }
}
