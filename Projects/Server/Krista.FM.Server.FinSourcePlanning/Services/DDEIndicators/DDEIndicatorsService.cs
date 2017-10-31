using System.Data;

using Krista.FM.Common;
using Krista.FM.Server.FinSourcePlanning.Services.IndicatorsService;
using Krista.FM.ServerLibrary.FinSourcePlanning;

namespace Krista.FM.Server.FinSourcePlanning.Services.DDEIndicators
{
    public class DDEIndicatorsService : IndicatorsService.IndicatorsService
    {
        /// <summary>
        /// ���������� ���������� �����������.
        /// </summary>
        /// <returns>������� � ������������.</returns>
        public override DataTable CalculateAndAssessIndicators(string variantIF, string variantIncome, string variantIncomeYear, string variantOutcome, int variantBorrowID)
        {
            // ������� ������ �����������, ������� ����� ����������
            DDEIndicatorsList indicatorsList = new DDEIndicatorsList();

            // ������� ��� ��� ��������.
            string[] marksClassifiersName = new string[2];
            marksClassifiersName[0] = "d_marks_contentdebt";
            marksClassifiersName[1] = "d_marks_contentdebtdata";
            MarksCasche casche = new MarksCasche(
                variantIF, variantIncome, variantIncomeYear, variantOutcome, variantBorrowID, marksClassifiersName);
            try
            {
                indicatorsList.CalculateIndicatorsValues(casche);
            }
            finally
            {
                casche.Dispose();
            }
            // ���������� �������.
            return indicatorsList.ToDataTable();
        }

        #region ��������
        private DDEIndicatorsService()
        {
        }

        private static DDEIndicatorsService instance;

        public static IIndicatorsService Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new DDEIndicatorsService();
                }
                return instance;
            }
        }
        #endregion
    }
}