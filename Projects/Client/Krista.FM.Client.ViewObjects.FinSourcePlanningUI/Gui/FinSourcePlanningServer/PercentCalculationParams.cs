using System;
using System.Data;

namespace Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Server
{
    public class PercentCalculationParams
    {
        /// <summary>
        /// начало периода расчета процентов
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// конец периода расчета процентов
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// периодичность выплаты процентов
        /// </summary>
        public PayPeriodicity PaymentsPeriodicity { get; set; }

        /// <summary>
        /// день окончания периодов выплат процентов
        /// </summary>
        public int EndPeriodDay { get; set; }

        /// <summary>
        /// день выплаты процентов
        /// </summary>
        public int PaymentDay { get; set; }

        /// <summary>
        /// корректировка дня окончания периодов выплат 
        /// </summary>
        public DayCorrection PaymentDayCorrection { get; set; }

        /// <summary>
        /// смещение дня окончания периодов выплат
        /// </summary>
        public bool EndPeriodDayShift { get; set; }

        /// <summary>
        /// начислять проценты с первого дня
        /// </summary>
        public bool FirstDayPayment { get; set; }

        /// <summary>
        /// досрочное погашение
        /// </summary>
        public bool PretermDischarge { get; set; }

        /// <summary>
        /// округление остатков
        /// </summary>
        public PercentRestRound RestRound { get; set; }

        /// <summary>
        /// произвольная периодичность выплаты процентов
        /// </summary>
        public DataTable CustomPeriods { get; set; }

        public string CalculationComment { get; set; }

        public DateTime FormDate { get; set; }

        public bool UseAllPercents { get; set; }

        public bool SplitPercentPeriods { get; set; }

        public VersionParams MainDebtVersion
        {
            get; set;
        }
    }
}
