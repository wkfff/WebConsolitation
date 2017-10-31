using System.Linq;
using System.Text;

using Krista.FM.Domain;
using Krista.FM.Extensions;
using Krista.FM.RIA.Extensions.E86N.Utils;

namespace Krista.FM.RIA.Extensions.E86N.Models.PfhdModel.Pfhd2017
{
    /// <summary>
    /// Показатели выплат по расходам на закупку
    /// </summary>
    public class ExpensePaymentIndexViewModel : ViewModelBase
    {
        [DataBaseBindingTable(typeof(F_Fin_ExpensePaymentIndex))]
        public int ID { get; set; }

        [DataBaseBindingTable(typeof(F_Fin_ExpensePaymentIndex))]
        public string Name { get; set; }

        [DataBaseBindingTable(typeof(F_Fin_ExpensePaymentIndex))]
        public string LineCode { get; set; }

        [DataBaseBindingTable(typeof(F_Fin_ExpensePaymentIndex))]
        public int? Year { get; set; }

        [DataBaseBindingTable(typeof(F_Fin_ExpensePaymentIndex))]
        public decimal? TotalSumNextYear { get; set; }

        [DataBaseBindingTable(typeof(F_Fin_ExpensePaymentIndex))]
        public decimal? TotalSumFirstPlanYear { get; set; }

        [DataBaseBindingTable(typeof(F_Fin_ExpensePaymentIndex))]
        public decimal? TotalSumSecondPlanYear { get; set; }

        [DataBaseBindingTable(typeof(F_Fin_ExpensePaymentIndex))]
        public decimal? Fz44SumNextYear { get; set; }

        [DataBaseBindingTable(typeof(F_Fin_ExpensePaymentIndex))]
        public decimal? Fz44SumFirstPlanYear { get; set; }

        [DataBaseBindingTable(typeof(F_Fin_ExpensePaymentIndex))]
        public decimal? Fz44SumSecondPlanYear { get; set; }

        [DataBaseBindingTable(typeof(F_Fin_ExpensePaymentIndex))]
        public decimal? Fz223SumNextYear { get; set; }

        [DataBaseBindingTable(typeof(F_Fin_ExpensePaymentIndex))]
        public decimal? Fz223SumFirstPlanYear { get; set; }

        [DataBaseBindingTable(typeof(F_Fin_ExpensePaymentIndex))]
        public decimal? Fz223SumSecondPlanYear { get; set; }

        public override string ValidateData(int docId)
        {
            const string Msg = "Необходимо заполнить поле \"{0}\"<br>";
            const string Msg1 = "Строка с {0} \"{1}\" уже заведена<br>";
            const string Msg2 = "В строке \"{0}\" значение графы \"{1}\" должно быть равно сумме граф \"{2}\" и \"{3}\"<br>";
            const string Msg3 = "Добавлять можно только строки с кодами: 1002, 1003, 1004, 1005, 2002, 2003, 2004, 2005<br>";
            
            var message = new StringBuilder();

            var codes = new[] { "1002", "1003", "1004", "1005", "2002", "2003", "2004", "2005" };
            if (ID < 0 && !codes.Contains(LineCode))
            {
                message.Append(Msg3);
            }

            if (Name.Trim().IsNullOrEmpty())
            {
                message.Append(Msg.FormatWith(DescriptionFromSchemeOf(() => Name)));
            }

            if (!TotalSumNextYear.HasValue)
            {
                message.Append(Msg.FormatWith(DescriptionFromSchemeOf(() => TotalSumNextYear)));
            }

            if (!TotalSumFirstPlanYear.HasValue)
            {
                message.Append(Msg.FormatWith(DescriptionFromSchemeOf(() => TotalSumFirstPlanYear)));
            }

            if (!TotalSumSecondPlanYear.HasValue)
            {
                message.Append(Msg.FormatWith(DescriptionFromSchemeOf(() => TotalSumSecondPlanYear)));
            }

            if (!Fz44SumNextYear.HasValue)
            {
                message.Append(Msg.FormatWith(DescriptionFromSchemeOf(() => Fz44SumNextYear)));
            }

            if (!Fz44SumFirstPlanYear.HasValue)
            {
                message.Append(Msg.FormatWith(DescriptionFromSchemeOf(() => Fz44SumFirstPlanYear)));
            }

            if (!Fz44SumSecondPlanYear.HasValue)
            {
                message.Append(Msg.FormatWith(DescriptionFromSchemeOf(() => Fz44SumSecondPlanYear)));
            }

            if (!Fz223SumNextYear.HasValue)
            {
                message.Append(Msg.FormatWith(DescriptionFromSchemeOf(() => Fz223SumNextYear)));
            }

            if (!Fz223SumFirstPlanYear.HasValue)
            {
                message.Append(Msg.FormatWith(DescriptionFromSchemeOf(() => Fz223SumFirstPlanYear)));
            }

            if (!Fz223SumSecondPlanYear.HasValue)
            {
                message.Append(Msg.FormatWith(DescriptionFromSchemeOf(() => Fz223SumSecondPlanYear)));
            }
             
            // todo проверка на дублирование по полю модели
            if (GetNewRestService().GetItems<F_Fin_ExpensePaymentIndex>()
                .Any(x => x.RefParametr.ID.Equals(docId) && !x.ID.Equals(ID) && x.LineCode.Equals(LineCode)))
            {
                message.Append(Msg1.FormatWith("кодом строки", LineCode));
            }

            if (GetNewRestService().GetItems<F_Fin_ExpensePaymentIndex>()
                .Any(x => x.RefParametr.ID.Equals(docId) && !x.ID.Equals(ID) && x.Name.Equals(Name)))
            {
                message.Append(Msg1.FormatWith("наименованием показателя", Name));
            }
             
            var sum = Fz44SumNextYear.GetValueOrDefault() + Fz223SumNextYear.GetValueOrDefault();
            codes = new[] { "1001", "2001" };
            if (!codes.Contains(LineCode))
            {
                if (!TotalSumNextYear.GetValueOrDefault().Equals(sum))
                {
                    message.Append(Msg2.FormatWith(
                                                    LineCode,
                                                    DescriptionFromSchemeOf(() => TotalSumNextYear),
                                                    DescriptionFromSchemeOf(() => Fz44SumNextYear),
                                                    DescriptionFromSchemeOf(() => Fz223SumNextYear)));
                }

                sum = Fz44SumFirstPlanYear.GetValueOrDefault() + Fz223SumFirstPlanYear.GetValueOrDefault();
                if (!TotalSumFirstPlanYear.GetValueOrDefault().Equals(sum))
                {
                    message.Append(Msg2.FormatWith(
                                                LineCode,
                                                DescriptionFromSchemeOf(() => TotalSumFirstPlanYear),
                                                DescriptionFromSchemeOf(() => Fz44SumFirstPlanYear),
                                                DescriptionFromSchemeOf(() => Fz223SumFirstPlanYear)));
                }

                sum = Fz44SumSecondPlanYear.GetValueOrDefault() + Fz223SumSecondPlanYear.GetValueOrDefault();
                if (!TotalSumSecondPlanYear.GetValueOrDefault().Equals(sum))
                {
                    message.Append(Msg2.FormatWith(
                                                    LineCode,
                                                    DescriptionFromSchemeOf(() => TotalSumSecondPlanYear),
                                                    DescriptionFromSchemeOf(() => Fz44SumSecondPlanYear),
                                                    DescriptionFromSchemeOf(() => Fz223SumSecondPlanYear)));
                }
            }

            return message.ToString();
        }
    }
}
