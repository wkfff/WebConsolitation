
using Krista.FM.Client.Reports.MOFO.Helpers;

namespace Krista.FM.Client.Reports.MOFO0024.Helpers
{
    internal class MOFO0024AteGrouping : MOFOAteGrouping
    {
        public const string Title = "По крупнейшим предприятиям";

        public MOFO0024AteGrouping(int style)
            : base(style)
        {
            SetFixedValues(string.Empty);
            Function = new Control(InsertMRBudjet) + InsertArrears;
        }
    }

    class ReportMOFO0024Helper
    {
        public const decimal LimitSumRate = 1000000;
        public const int OrgRefUndefined = -1;
        // уровни бюджета
        public const int BdgtLevelAll = 0;      // всего
        public const int BdgtLevelFederal = 1;  // федеральный бюджет
        public const int BdgtLevelSubject = 2;  // региональный бюджет
        // показатели
        public const int MarkUnknown = 0;   // Значение не указано
        public const int MarkAll = 1;       // Налог на прибыль (доход) организаций в целом
        public const int MarkPayment = 2;   // В том числе по крупным налогоплательщикам, заявившим сумму платежа по налогу на прибыль к доплате
        public const int MarkReduction = 3; // В том числе по крупным налогоплательщикам, заявившим сумму платежа по налогу на прибыль к уменьшению
        // Вариант.Проект доходов
        public const int VariantPlanIncomesMonth = -2;  // Ежемесячное планирование текущего года
    }
}
