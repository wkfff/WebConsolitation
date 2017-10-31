using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

using Ext.Net.MVC;

using Krista.FM.Domain;
using Krista.FM.Extensions;
using Krista.FM.RIA.Extensions.E86N.Models.PfhdModel.Pfhd2017;
using Krista.FM.RIA.Extensions.E86N.Presentation.Controls;
using Krista.FM.RIA.Extensions.E86N.Utils;

using Microsoft.Practices.ObjectBuilder2;

namespace Krista.FM.RIA.Extensions.E86N.Presentation.Controllers.StateSysyem
{
    public class PfhdStateSysController : StateSysBaseController
    {
        private readonly ExpensePaymentIndexViewModel expensePaymentIndexViewModel = new ExpensePaymentIndexViewModel();
        private readonly PlanPaymentIndexViewModel planPaymentIndexViewModel = new PlanPaymentIndexViewModel();

        private bool targetedSubsidies;

        private int year;

        /// <summary>
        ///   настраиваем вместе с кнопками и редактируемость интерфейса
        ///   в зависимости от состояния и пользователя
        /// </summary>
        public override StringBuilder InitComponents(int docId)
        {
            StringBuilder resultScript = base.InitComponents(docId);

            if (Auth.IsSpectator())
            {
                // для наблюдателя только чтение
                resultScript.Append(ReadOnlyScript(true, docId));
                return resultScript;
            }

            if (!Auth.IsAdmin())
            {
                var stateCode = StateSystemService.GetCurrentStateID(docId);

                if (Auth.IsPpoUser() && !Auth.IsGrbsUser())
                {
                    // если ФО то только чтение
                    resultScript.Append(ReadOnlyScript(true, docId));
                }
                else
                {
                    // если ГРБС или учреждение
                    switch (stateCode)
                    {
                        case (int)StatesType.UnderConsideration:
                            resultScript.Append(Auth.IsGrbsUser() ? ReadOnlyScript(false, docId) : ReadOnlyScript(true, docId));
                            break;
                        case (int)StatesType.Completed:
                            resultScript.Append(ReadOnlyScript(true, docId));
                            break;
                        case (int)StatesType.Exported:
                            resultScript.Append(ReadOnlyScript(true, docId));
                            break;
                        default:
                            resultScript.Append(ReadOnlyScript(false, docId));
                            break;
                    }
                }
            }

            return resultScript;
        }

        /// <summary>
        ///   Экшен выполнения перехода на рассмотрение и завершение
        /// </summary>
        public virtual AjaxFormResult TransitionWithValidate(int docId, int transitionID)
        {
            try
            {
                string msg;
                if (DataValidations(docId, out msg))
                {
                    return DefaultTransition(docId, transitionID);
                }

                return GetScriptResult(GetShowMessageScript(msg));
            }
            catch (Exception e)
            {
                return GetScriptResult(GetShowMessageScript(e.Message));
            }
        }

        /// <summary>
        ///   Формирование скрипта закрытия\открытия интерфейса для редактирования
        /// </summary>
        private string ReadOnlyScript(bool readOnly, int docId)
        {
            var resultScript = new StringBuilder();
            resultScript.Append("window.E86n.Control.StateToolBar.ReadOnlyDocHandler({0}, {1});".FormatWith(readOnly.ToString().ToLower(), docId));
            return resultScript.ToString();
        }

        /// <summary>
        ///   Валидация данных
        /// </summary>
        /// <returns> true - если валидация прошла </returns>
        private bool DataValidations(int docId, out string message)
        {
            const string Msg = "Не прикреплен документ типа \"{0}\".<p/>";

            message = string.Empty;

            year = StateSystemService.GetItem<F_F_ParameterDoc>(docId).RefYearForm.ID;

            var docs = from p in StateSystemService.GetItems<F_Doc_Docum>() where (p.RefParametr.ID == docId) && (p.Url != "НетФайла") && (p.RefTypeDoc.Code == "P") select new { p.ID };

            if (!docs.Any())
            {
                message += Msg.FormatWith(GetNameTypeDoc("P"));
            }

            docs = from p in StateSystemService.GetItems<F_Doc_Docum>() where (p.RefParametr.ID == docId) && (p.Url != "НетФайла") && (p.RefTypeDoc.Code == "A") select new { p.ID };

            targetedSubsidies = docs.Any();

            message += CheckData(docId);

            if (year < 2017)
            {
                message += CheckBudgetaryFunds(docId);
            }

            return string.IsNullOrEmpty(message);
        }

        /// <summary>
        ///   Проверка значения бюджетных инвистиций
        /// </summary>
        private string CheckBudgetaryFunds(int docId)
        {
            string result = string.Empty;

            decimal? val = StateSystemService.GetItems<F_Fin_CapFunds>().Where(x => x.RefParametr.ID == docId).Sum(x => x.funds) == null
                               ? 0
                               : StateSystemService.GetItems<F_Fin_CapFunds>().Where(x => x.RefParametr.ID == docId).Sum(x => x.funds);

            val += StateSystemService.GetItems<F_Fin_realAssFunds>().Where(x => x.RefParametr.ID == docId).Sum(x => x.funds) == null
                       ? 0
                       : StateSystemService.GetItems<F_Fin_realAssFunds>().Where(x => x.RefParametr.ID == docId).Sum(x => x.funds);

            F_Fin_finActPlan plan = StateSystemService.GetItems<F_Fin_finActPlan>().First(x => (x.RefParametr.ID == docId) && (x.NumberStr == 0));

            if (plan.budgetaryFunds < val)
            {
                result += "Значение поля \"Бюджетные инвестиции\" должно быть больше или равно сумме значений"
                          + "полей \"Общая сумма\" детализаций \"Информация об объектах капитального строительства\" и "
                          + "\"Информация об объектах приобретаемого недвижимого имущества\"<br>";
            }

            if (plan.budgetaryFunds > 0 && val == 0)
            {
                result += "Необходимо заполнить интерфейсы \"Информация об объектах капитального строительства\" и \"Информация об объектах приобретаемого недвижимого имущества\"<br>";
            }

            return result;
        }

        /// <summary>
        ///   Проверка заполнения данных на интерфейсе
        /// </summary>
        private string CheckData(int docId)
        {
            string result = string.Empty;

            if (year < 2017)
            {
                result += OldPlanValidation(docId);
            }
            else
            {
                result += CheckFieldsFinancialIndex(docId);
                result += ValidateFinancialIndex(docId);
                result += ValiadationExpensePaymentIndex(docId);
                var resultPlanPaymentIndex = CheckFieldsPlanPaymentIndex(docId);
                if (resultPlanPaymentIndex.IsNullOrEmpty())
                {
                    result += ValiadationPlanPaymentIndex(docId);
                }
                else
                {
                    result += resultPlanPaymentIndex;
                }
                
                result += CrossTableControl(docId);
            }

            IQueryable<F_Fin_othGrantFunds> otherGrantFunds = StateSystemService.GetItems<F_Fin_othGrantFunds>().Where(x => x.RefParametr.ID == docId);

            if (!otherGrantFunds.Any() && targetedSubsidies)
            {
                result += "Не заполнена детализация \"Информация об операциях с субсидиями на иные цели\"<p/>";
            }

            if (otherGrantFunds.Any() && !targetedSubsidies)
            {
                result += "Не прикреплен документ типа \"{0}\".<p/>".FormatWith(GetNameTypeDoc("A"));
            }

            IQueryable<F_Fin_CapFunds> capFundses = StateSystemService.GetItems<F_Fin_CapFunds>().Where(x => x.RefParametr.ID == docId);
            foreach (F_Fin_CapFunds rec in capFundses)
            {
                if (string.IsNullOrEmpty(rec.Name))
                {
                    result += "Не указано наименование объекта капитального строительства <br>";
                }

                if (rec.funds == null || rec.funds == 0)
                {
                    result += "Не указана сумма объекта капитального строительства  <br>";
                }
            }

            IQueryable<F_Fin_realAssFunds> assFunds = StateSystemService.GetItems<F_Fin_realAssFunds>().Where(x => x.RefParametr.ID == docId);
            foreach (F_Fin_realAssFunds rec in assFunds)
            {
                if (string.IsNullOrEmpty(rec.Name))
                {
                    result += "Не указано наименование объекта приобретаемого недвижимого имущества <br>";
                }

                if (rec.funds == null || rec.funds == 0)
                {
                    result += "Не указана сумма объектах приобретаемого недвижимого имущества <br>";
                }
            }

            return result;
        }
        
        private string CheckFieldsPlanPaymentIndex(int docId)
        {
            const string Msg = "Не заполнена детализация \"Плановые показатели поступлений и выплат\" за {0} год<br>";

            string result = string.Empty;

            if (!StateSystemService.GetItem<F_F_ParameterDoc>(docId).PlanPaymentIndex.Any(x => x.Period.Equals(0)))
            {
                result += Msg.FormatWith(year);
            }

            if (!StateSystemService.GetItem<F_F_ParameterDoc>(docId).PlanPaymentIndex.Any(x => x.Period.Equals(1)))
            {
                result += Msg.FormatWith(year + 1);
            }

            if (!StateSystemService.GetItem<F_F_ParameterDoc>(docId).PlanPaymentIndex.Any(x => x.Period.Equals(2)))
            {
                result += Msg.FormatWith(year + 2);
            }

            return result;
        }

        /// <summary>
        /// контроль заполнения детализации "Плановые показатели поступлений и выплат"
        /// </summary>
        private string ValiadationPlanPaymentIndex(int docId)
        {
            const string Msg = "Детализация \"Плановые показатели поступлений и выплат\" за {0} год, код строки: {1} : ";
            
            string result = string.Empty;
            
            var modelData = StateSystemService.GetItem<F_F_ParameterDoc>(docId).PlanPaymentIndex
                .Where(x => x.Period.Equals(0)).Select(
                    p => new PlanPaymentIndexViewModel
                    {
                        ID = p.ID,
                        Name = p.Name,
                        LineCode = p.LineCode,
                        Kbk = p.Kbk,
                        Total = p.Total,
                        FinancialProvision = p.FinancialProvision,
                        SubsidyFinSupportFfoms = p.SubsidyFinSupportFfoms,
                        SubsidyOtherPurposes = p.SubsidyOtherPurposes,
                        CapitalInvestment = p.CapitalInvestment,
                        HealthInsurance = p.HealthInsurance,
                        ServiceTotal = p.ServiceTotal,
                        ServiceGrant = p.ServiceGrant
                    });

            modelData.Each(
                x =>
                    {
                        var valMsg = x.ValidateData(docId);
                        if (valMsg.IsNotNullOrEmpty())
                        {
                            result += Msg.FormatWith(year, x.LineCode) + x.ValidateData(docId);
                        }
                    });

            modelData = StateSystemService.GetItem<F_F_ParameterDoc>(docId).PlanPaymentIndex
               .Where(x => x.Period.Equals(1)).Select(
                   p => new PlanPaymentIndexViewModel
                   {
                       ID = p.ID,
                       Name = p.Name,
                       LineCode = p.LineCode,
                       Kbk = p.Kbk,
                       Total = p.Total,
                       FinancialProvision = p.FinancialProvision,
                       SubsidyFinSupportFfoms = p.SubsidyFinSupportFfoms,
                       SubsidyOtherPurposes = p.SubsidyOtherPurposes,
                       CapitalInvestment = p.CapitalInvestment,
                       HealthInsurance = p.HealthInsurance,
                       ServiceTotal = p.ServiceTotal,
                       ServiceGrant = p.ServiceGrant
                   });

            modelData.Each(
                x =>
                    {
                        var valMsg = x.ValidateData(docId);
                        if (valMsg.IsNotNullOrEmpty())
                        {
                            result += Msg.FormatWith(year + 1, x.LineCode) + x.ValidateData(docId);
                        }
                    });

            modelData = StateSystemService.GetItem<F_F_ParameterDoc>(docId).PlanPaymentIndex
               .Where(x => x.Period.Equals(2)).Select(
                   p => new PlanPaymentIndexViewModel
                   {
                       ID = p.ID,
                       Name = p.Name,
                       LineCode = p.LineCode,
                       Kbk = p.Kbk,
                       Total = p.Total,
                       FinancialProvision = p.FinancialProvision,
                       SubsidyFinSupportFfoms = p.SubsidyFinSupportFfoms,
                       SubsidyOtherPurposes = p.SubsidyOtherPurposes,
                       CapitalInvestment = p.CapitalInvestment,
                       HealthInsurance = p.HealthInsurance,
                       ServiceTotal = p.ServiceTotal,
                       ServiceGrant = p.ServiceGrant
                   });

            modelData.Each(
                x =>
                    {
                        var valMsg = x.ValidateData(docId);
                        if (valMsg.IsNotNullOrEmpty())
                        {
                            result += Msg.FormatWith(year + 2, x.LineCode) + x.ValidateData(docId);
                        }
                    });

            var lineCodes = new[] { 110, 120, 130, 140, 150, 160, 180 };
            var totalLineCode = "100";
            var data = StateSystemService.GetItem<F_F_ParameterDoc>(docId).PlanPaymentIndex.Where(x => x.Period.Equals(0)).ToList();
            result += ValiadationPlanPaymentIndexSummByYear(year, totalLineCode, lineCodes, data);
            data = StateSystemService.GetItem<F_F_ParameterDoc>(docId).PlanPaymentIndex.Where(x => x.Period.Equals(1)).ToList();
            result += ValiadationPlanPaymentIndexSummByYear(year + 1, totalLineCode, lineCodes, data);
            data = StateSystemService.GetItem<F_F_ParameterDoc>(docId).PlanPaymentIndex.Where(x => x.Period.Equals(2)).ToList();
            result += ValiadationPlanPaymentIndexSummByYear(year + 2, totalLineCode, lineCodes, data);

            lineCodes = new[] { 210, 220, 230, 240, 250, 260 };
            totalLineCode = "200";
            data = StateSystemService.GetItem<F_F_ParameterDoc>(docId).PlanPaymentIndex.Where(x => x.Period.Equals(0)).ToList();
            result += ValiadationPlanPaymentIndexSummByYear(year, totalLineCode, lineCodes, data);
            data = StateSystemService.GetItem<F_F_ParameterDoc>(docId).PlanPaymentIndex.Where(x => x.Period.Equals(1)).ToList();
            result += ValiadationPlanPaymentIndexSummByYear(year + 1, totalLineCode, lineCodes, data);
            data = StateSystemService.GetItem<F_F_ParameterDoc>(docId).PlanPaymentIndex.Where(x => x.Period.Equals(2)).ToList();
            result += ValiadationPlanPaymentIndexSummByYear(year + 2, totalLineCode, lineCodes, data);

            lineCodes = new[] { 310, 320 };
            totalLineCode = "300";
            data = StateSystemService.GetItem<F_F_ParameterDoc>(docId).PlanPaymentIndex.Where(x => x.Period.Equals(0)).ToList();
            result += ValiadationPlanPaymentIndexSummByYear(year, totalLineCode, lineCodes, data);
            data = StateSystemService.GetItem<F_F_ParameterDoc>(docId).PlanPaymentIndex.Where(x => x.Period.Equals(1)).ToList();
            result += ValiadationPlanPaymentIndexSummByYear(year + 1, totalLineCode, lineCodes, data);
            data = StateSystemService.GetItem<F_F_ParameterDoc>(docId).PlanPaymentIndex.Where(x => x.Period.Equals(2)).ToList();
            result += ValiadationPlanPaymentIndexSummByYear(year + 2, totalLineCode, lineCodes, data);

            lineCodes = new[] { 410, 420 };
            totalLineCode = "400";
            data = StateSystemService.GetItem<F_F_ParameterDoc>(docId).PlanPaymentIndex.Where(x => x.Period.Equals(0)).ToList();
            result += ValiadationPlanPaymentIndexSummByYear(year, totalLineCode, lineCodes, data);
            data = StateSystemService.GetItem<F_F_ParameterDoc>(docId).PlanPaymentIndex.Where(x => x.Period.Equals(1)).ToList();
            result += ValiadationPlanPaymentIndexSummByYear(year + 1, totalLineCode, lineCodes, data);
            data = StateSystemService.GetItem<F_F_ParameterDoc>(docId).PlanPaymentIndex.Where(x => x.Period.Equals(2)).ToList();
            result += ValiadationPlanPaymentIndexSummByYear(year + 2, totalLineCode, lineCodes, data);

            lineCodes = new[] { 211 };
            totalLineCode = "210";
            data = StateSystemService.GetItem<F_F_ParameterDoc>(docId).PlanPaymentIndex.Where(x => x.Period.Equals(0)).ToList();
            result += ValiadationPlanPaymentIndexSummByYear(year, totalLineCode, lineCodes, data);
            data = StateSystemService.GetItem<F_F_ParameterDoc>(docId).PlanPaymentIndex.Where(x => x.Period.Equals(1)).ToList();
            result += ValiadationPlanPaymentIndexSummByYear(year + 1, totalLineCode, lineCodes, data);
            data = StateSystemService.GetItem<F_F_ParameterDoc>(docId).PlanPaymentIndex.Where(x => x.Period.Equals(2)).ToList();
            result += ValiadationPlanPaymentIndexSummByYear(year + 2, totalLineCode, lineCodes, data);

            return result;
        }

        private string ValiadationPlanPaymentIndexSummByYear(int yearParam, string lineCode, int[] codeSums, List<F_F_PlanPaymentIndex> items)
        {
            string yearMsg = "За {0} год <br>".FormatWith(yearParam);
            string msg = "Сумма в колонке \"{0}\" строки \"{1}\" меньше суммы значений этой колонки во входящих строках. " + yearMsg;

            string result = string.Empty;

            var value = items.Single(x => x.LineCode.Equals(lineCode)).Total;
            var sum = items.Where(x => codeSums.Contains(int.Parse(x.LineCode))).SumWithNull(x => x.Total);
            if (value < sum.GetValueOrDefault())
            {
                result += msg.FormatWith(UiBuilders.DescriptionOf(() => planPaymentIndexViewModel.Total), lineCode);
            }

            value = items.Single(x => x.LineCode.Equals(lineCode)).FinancialProvision.GetValueOrDefault();
            sum = items.Where(x => codeSums.Contains(int.Parse(x.LineCode))).SumWithNull(x => x.FinancialProvision);
            if (value < sum.GetValueOrDefault())
            {
                result += msg.FormatWith(UiBuilders.DescriptionOf(() => planPaymentIndexViewModel.FinancialProvision), lineCode);
            }

            value = items.Single(x => x.LineCode.Equals(lineCode)).SubsidyFinSupportFfoms.GetValueOrDefault();
            sum = items.Where(x => codeSums.Contains(int.Parse(x.LineCode))).SumWithNull(x => x.SubsidyFinSupportFfoms);
            if (value < sum.GetValueOrDefault())
            {
                result += msg.FormatWith(UiBuilders.DescriptionOf(() => planPaymentIndexViewModel.SubsidyFinSupportFfoms), lineCode);
            }

            value = items.Single(x => x.LineCode.Equals(lineCode)).SubsidyOtherPurposes.GetValueOrDefault();
            sum = items.Where(x => codeSums.Contains(int.Parse(x.LineCode))).SumWithNull(x => x.SubsidyOtherPurposes);
            if (value < sum.GetValueOrDefault())
            {
                result += msg.FormatWith(UiBuilders.DescriptionOf(() => planPaymentIndexViewModel.SubsidyOtherPurposes), lineCode);
            }

            value = items.Single(x => x.LineCode.Equals(lineCode)).CapitalInvestment.GetValueOrDefault();
            sum = items.Where(x => codeSums.Contains(int.Parse(x.LineCode))).SumWithNull(x => x.CapitalInvestment);
            if (value < sum.GetValueOrDefault())
            {
                result += msg.FormatWith(UiBuilders.DescriptionOf(() => planPaymentIndexViewModel.CapitalInvestment), lineCode);
            }

            value = items.Single(x => x.LineCode.Equals(lineCode)).HealthInsurance.GetValueOrDefault();
            sum = items.Where(x => codeSums.Contains(int.Parse(x.LineCode))).SumWithNull(x => x.HealthInsurance);
            if (value < sum.GetValueOrDefault())
            {
                result += msg.FormatWith(UiBuilders.DescriptionOf(() => planPaymentIndexViewModel.HealthInsurance), lineCode);
            }

            value = items.Single(x => x.LineCode.Equals(lineCode)).ServiceTotal.GetValueOrDefault();
            sum = items.Where(x => codeSums.Contains(int.Parse(x.LineCode))).SumWithNull(x => x.ServiceTotal);
            if (value < sum.GetValueOrDefault())
            {
                result += msg.FormatWith(UiBuilders.DescriptionOf(() => planPaymentIndexViewModel.ServiceTotal), lineCode);
            }

            return result;
        }

        private string CheckFieldsPlan(F_Fin_finActPlan record)
        {
            const string Msg = "Не заполнены данные за {0} год.<p/>";

            if (record.totnonfinAssets != 0)
            {
                return string.Empty;
            }

            if (record.realAssets != 0)
            {
                return string.Empty;
            }

            if (record.highValPersAssets != 0)
            {
                return string.Empty;
            }

            if (record.finAssets != 0)
            {
                return string.Empty;
            }

            if (record.income != 0)
            {
                return string.Empty;
            }

            if (record.expense != 0)
            {
                return string.Empty;
            }

            if (record.finCircum != 0)
            {
                return string.Empty;
            }

            if (record.kreditExpir != 0)
            {
                return string.Empty;
            }

            if (record.stateTaskGrant != 0)
            {
                return string.Empty;
            }

            if (record.actionGrant != 0)
            {
                return string.Empty;
            }

            if (record.budgetaryFunds != 0)
            {
                return string.Empty;
            }

            if (record.paidServices != 0)
            {
                return string.Empty;
            }

            if (record.planPayments != 0)
            {
                return string.Empty;
            }

            if (record.planInpayments != 0)
            {
                return string.Empty;
            }

            if (record.labourRemuneration != 0)
            {
                return string.Empty;
            }

            if (record.telephoneServices != 0)
            {
                return string.Empty;
            }

            if (record.freightServices != 0)
            {
                return string.Empty;
            }

            if (record.publicServeces != 0)
            {
                return string.Empty;
            }

            if (record.rental != 0)
            {
                return string.Empty;
            }

            if (record.maintenanceCosts != 0)
            {
                return string.Empty;
            }

            if (record.mainFunds != 0)
            {
                return string.Empty;
            }

            if (record.fictitiousAssets != 0)
            {
                return string.Empty;
            }

            if (record.tangibleAssets != 0)
            {
                return string.Empty;
            }

            if (record.publish != 0)
            {
                return string.Empty;
            }

            return Msg.FormatWith(record.RefParametr.RefYearForm.ID + record.NumberStr);
        }

        private string CheckSummPlan(F_Fin_finActPlan record)
        {
            if (record.planInpayments < record.stateTaskGrant + record.actionGrant + record.budgetaryFunds + record.paidServices)
            {
                return "Значение \"Планируемая сумма поступлений, всего\" должно быть больше суммы значений " + "\"Субсидии на выполнение задания\", " + "\"Целевые субсидии\", "
                       + "\"Бюджетные инвестиции\" и " + "\"Оказание платных услуг (выполнения работ) и приносящая доход деятельность\"<br/>";
            }

            return string.Empty;
        }

        private string OldPlanValidation(int docId)
        {
            string result = string.Empty;

            bool planThreeYear = StateSystemService.GetItems<F_F_ParameterDoc>().First(x => x.ID == docId).PlanThreeYear;
            IQueryable<F_Fin_finActPlan> doc = StateSystemService.GetItems<F_Fin_finActPlan>().Where(x => x.RefParametr.ID == docId);

            F_Fin_finActPlan finActPlan = doc.First(x => x.NumberStr == 0);

            if (planThreeYear)
            {
                int count = 0;
                string msg = string.Empty;
                foreach (F_Fin_finActPlan rec in doc)
                {
                    result = CheckFieldsPlan(rec);
                    if (!string.IsNullOrEmpty(result))
                    {
                        ++count;
                    }

                    msg += CheckSummPlan(rec);
                }

                if (count == 3)
                {
                    result = "Не заполнены данные ни за один год.<br/>" + msg;
                }
                else
                {
                    result = msg;
                }
            }
            else
            {
                result += CheckFieldsPlan(finActPlan);
                result += CheckSummPlan(finActPlan);
            }

            var actionGrant = finActPlan.actionGrant ?? 0;

            if (!actionGrant.Equals(0) && !targetedSubsidies)
            {
                result += "Не прикреплен документ типа \"{0}\".<p/>".FormatWith(GetNameTypeDoc("A"));
            }

            return result;
        }

        // контроль заполнения детализации "Показатели финансового состояния учреждения"
        private string CheckFieldsFinancialIndex(int docId)
        {
            const string Msg = "Не заполнены данные в детализации \"Показатели финансового состояния учреждения\"<p/>";

            var record = StateSystemService.GetItem<F_F_ParameterDoc>(docId).FinancialIndex.FirstOrDefault();

            if (record != null)
            {
                if (record.NonFinancialAssets.HasValue)
                {
                    return string.Empty;
                }

                if (record.RealAssets.HasValue)
                {
                    return string.Empty;
                }

                if (record.RealAssetsDepreciatedCost.HasValue)
                {
                    return string.Empty;
                }

                if (record.HighValuePersonalAssets.HasValue)
                {
                    return string.Empty;
                }

                if (record.HighValuePADepreciatedCost.HasValue)
                {
                    return string.Empty;
                }

                if (record.FinancialAssets.HasValue)
                {
                    return string.Empty;
                }

                if (record.MoneyInstitutions.HasValue)
                {
                    return string.Empty;
                }

                if (record.FundsAccountsInstitution.HasValue)
                {
                    return string.Empty;
                }

                if (record.FundsPlacedOnDeposits.HasValue)
                {
                    return string.Empty;
                }

                if (record.OtherFinancialInstruments.HasValue)
                {
                    return string.Empty;
                }

                if (record.DebitIncome.HasValue)
                {
                    return string.Empty;
                }

                if (record.DebitExpense.HasValue)
                {
                    return string.Empty;
                }

                if (record.FinancialCircumstanc.HasValue)
                {
                    return string.Empty;
                }

                if (record.Debentures.HasValue)
                {
                    return string.Empty;
                }

                if (record.AccountsPayable.HasValue)
                {
                    return string.Empty;
                }

                if (record.KreditExpired.HasValue)
                {
                    return string.Empty;
                }
            }

            return Msg;
        }

        // контроль детализации "Показатели финансового состояния учреждения"
        private string ValidateFinancialIndex(int docId)
        {
            var record = StateSystemService.GetItem<F_F_ParameterDoc>(docId).FinancialIndex
                .Select(p => (FinancialIndexViewModel)new FinancialIndexViewModel().GetModelByDomain(p))
                .FirstOrDefault();
            if (record != null)
            {
                return record.ValidateData(docId);
            }

            return string.Empty;
        }

        // контроль заполнения детализации "Показатели выплат по расходам на закупку"
        private string ValiadationExpensePaymentIndex(int docId)
        {
            const string Msg = "Не заполнены данные в детализации \"Показатели выплат по расходам на закупку \"<br>";
            const string Msg1 = "Отсутствуют данные по показателю \"{0}\" в детализации \"Показатели выплат по расходам на закупку \"<br>";

            string result = string.Empty;

            var records = StateSystemService.GetItem<F_F_ParameterDoc>(docId).ExpensePaymentIndex;

            if (records.Any())
            {
                var lc0001 = records.FirstOrDefault(x => x.LineCode.Equals("0001"));
                var lc1001 = records.FirstOrDefault(x => x.LineCode.Equals("1001"));
                var lc2001 = records.FirstOrDefault(x => x.LineCode.Equals("2001"));

                if (lc0001 == null)
                {
                    result += Msg1.FormatWith("0001");
                }

                if (lc1001 == null)
                {
                    result += Msg1.FormatWith("1001");
                }

                if (lc2001 == null)
                {
                    result += Msg1.FormatWith("2001");
                }

                if (result.IsNullOrEmpty())
                {
                    result += ValiadationSummExpensePaymentIndex(() => expensePaymentIndexViewModel.TotalSumNextYear, lc0001, lc1001, lc2001);
                    result += ValiadationSummExpensePaymentIndex(() => expensePaymentIndexViewModel.TotalSumFirstPlanYear, lc0001, lc1001, lc2001);
                    result += ValiadationSummExpensePaymentIndex(() => expensePaymentIndexViewModel.TotalSumSecondPlanYear, lc0001, lc1001, lc2001);
                    result += ValiadationSummExpensePaymentIndex(() => expensePaymentIndexViewModel.Fz44SumNextYear, lc0001, lc1001, lc2001);
                    result += ValiadationSummExpensePaymentIndex(() => expensePaymentIndexViewModel.Fz44SumFirstPlanYear, lc0001, lc1001, lc2001);
                    result += ValiadationSummExpensePaymentIndex(() => expensePaymentIndexViewModel.Fz44SumSecondPlanYear, lc0001, lc1001, lc2001);
                    result += ValiadationSummExpensePaymentIndex(() => expensePaymentIndexViewModel.Fz223SumNextYear, lc0001, lc1001, lc2001);
                    result += ValiadationSummExpensePaymentIndex(() => expensePaymentIndexViewModel.Fz223SumFirstPlanYear, lc0001, lc1001, lc2001);
                    result += ValiadationSummExpensePaymentIndex(() => expensePaymentIndexViewModel.Fz223SumSecondPlanYear, lc0001, lc1001, lc2001);
                }

                var codes = new[] { "1002", "1003", "1004", "1005" };

                result += ValiadationExpensePaymentIndexRows(codes, lc1001, docId);

                codes = new[] { "2002", "2003", "2004", "2005" };

                result += ValiadationExpensePaymentIndexRows(codes, lc2001, docId);
            }
            else
            {
                result += Msg;
            }

            return result;
        }

        private string ValiadationExpensePaymentIndexRows(string[] codes, F_Fin_ExpensePaymentIndex lineCodeRow, int docId)
        {
            string msg = "Сумма в колонке \"{0}\" строки \"{1}\" меньше суммы значений этой колонки во входящих строках.<br>";

            string result = string.Empty;

            if (lineCodeRow == null)
            {
                return result;
            }

            var rows = StateSystemService.GetItem<F_F_ParameterDoc>(docId).ExpensePaymentIndex.Where(x => codes.Contains(x.LineCode)).ToList();

            if (lineCodeRow.LineCode.Equals("2001"))
            {
                if (rows.Any())
                {
                    if (lineCodeRow.Year.HasValue)
                    {
                        result += "Поле \"{0}\" с кодом строки \"2001\" должно быть пустым, т.к. заведены подчиненные строки<br>".FormatWith(UiBuilders.DescriptionOf(() => expensePaymentIndexViewModel.Year));
                    }

                    if (rows.Count != rows.Select(x => x.Year).Distinct().Count())
                    {
                        result += "Необходимо указать различный год начала закупки для детализирующих строк {0}".FormatWith(rows.Select(x => x.LineCode).JoinStrings(", "));
                    }
                    else
                    {
                        if (rows.Count == 1 && !rows.Single().Year.HasValue)
                        {
                            result += "Необходимо указать год начала закупки для строки {0}".FormatWith(rows.Single().LineCode);
                        }
                    }
                }
                else
                {
                    if (!lineCodeRow.Year.HasValue)
                    {
                        result += "Поле \"{0}\" с кодом строки \"2001\" должно быть заполнено, т.к. не заведены подчиненные строки<br>".FormatWith(UiBuilders.DescriptionOf(() => expensePaymentIndexViewModel.Year));
                    }
                }
            }

            var sum = rows.Where(x => codes.Contains(x.LineCode)).SumWithNull(x => x.TotalSumNextYear);
            if (lineCodeRow.TotalSumNextYear < sum.GetValueOrDefault())
            {
                result += msg.FormatWith(UiBuilders.DescriptionOf(() => expensePaymentIndexViewModel.TotalSumNextYear), lineCodeRow.LineCode);
            }

            sum = rows.Where(x => codes.Contains(x.LineCode)).SumWithNull(x => x.TotalSumFirstPlanYear);
            if (lineCodeRow.TotalSumFirstPlanYear < sum.GetValueOrDefault())
            {
                result += msg.FormatWith(UiBuilders.DescriptionOf(() => expensePaymentIndexViewModel.TotalSumFirstPlanYear), lineCodeRow.LineCode);
            }

            sum = rows.Where(x => codes.Contains(x.LineCode)).SumWithNull(x => x.TotalSumSecondPlanYear);
            if (lineCodeRow.TotalSumSecondPlanYear < sum.GetValueOrDefault())
            {
                result += msg.FormatWith(UiBuilders.DescriptionOf(() => expensePaymentIndexViewModel.TotalSumSecondPlanYear), lineCodeRow.LineCode);
            }

            sum = rows.Where(x => codes.Contains(x.LineCode)).SumWithNull(x => x.Fz44SumNextYear);
            if (lineCodeRow.Fz44SumNextYear < sum.GetValueOrDefault())
            {
                result += msg.FormatWith(UiBuilders.DescriptionOf(() => expensePaymentIndexViewModel.Fz44SumNextYear), lineCodeRow.LineCode);
            }

            sum = rows.Where(x => codes.Contains(x.LineCode)).SumWithNull(x => x.Fz44SumFirstPlanYear);
            if (lineCodeRow.Fz44SumFirstPlanYear < sum.GetValueOrDefault())
            {
                result += msg.FormatWith(UiBuilders.DescriptionOf(() => expensePaymentIndexViewModel.Fz44SumFirstPlanYear), lineCodeRow.LineCode);
            }

            sum = rows.Where(x => codes.Contains(x.LineCode)).SumWithNull(x => x.Fz44SumSecondPlanYear);
            if (lineCodeRow.Fz44SumSecondPlanYear < sum.GetValueOrDefault())
            {
                result += msg.FormatWith(UiBuilders.DescriptionOf(() => expensePaymentIndexViewModel.Fz44SumSecondPlanYear), lineCodeRow.LineCode);
            }

            sum = rows.Where(x => codes.Contains(x.LineCode)).SumWithNull(x => x.Fz223SumNextYear);
            if (lineCodeRow.Fz223SumNextYear < sum.GetValueOrDefault())
            {
                result += msg.FormatWith(UiBuilders.DescriptionOf(() => expensePaymentIndexViewModel.Fz223SumNextYear), lineCodeRow.LineCode);
            }

            sum = rows.Where(x => codes.Contains(x.LineCode)).SumWithNull(x => x.Fz223SumFirstPlanYear);
            if (lineCodeRow.Fz223SumFirstPlanYear < sum.GetValueOrDefault())
            {
                result += msg.FormatWith(UiBuilders.DescriptionOf(() => expensePaymentIndexViewModel.Fz223SumFirstPlanYear), lineCodeRow.LineCode);
            }

            sum = rows.Where(x => codes.Contains(x.LineCode)).SumWithNull(x => x.Fz223SumSecondPlanYear);
            if (lineCodeRow.Fz223SumSecondPlanYear < sum.GetValueOrDefault())
            {
                result += msg.FormatWith(UiBuilders.DescriptionOf(() => expensePaymentIndexViewModel.Fz223SumSecondPlanYear), lineCodeRow.LineCode);
            }

            return result;
        }

        /// <summary>
        /// Контроли 1 в рамках Таблицы 3
        /// </summary>
        private string ValiadationSummExpensePaymentIndex<T>(
                                            Expression<Func<T>> expr, 
                                            F_Fin_ExpensePaymentIndex value0001, 
                                            F_Fin_ExpensePaymentIndex value1001,
                                            F_Fin_ExpensePaymentIndex value2001)
        {
            const string Msg = "Сумма показателей \"1001\" и \"2001\" не равен значению показателя \"0001\" в графе \"{0}\" " 
                                        + "детализации \"Показатели выплат по расходам на закупку \"<br>";
            
            string result = string.Empty;

            var nameFld = UiBuilders.NameOf(expr);
            PropertyInfo pinfo = typeof(F_Fin_ExpensePaymentIndex).GetProperty(nameFld);
            var summ = (decimal)pinfo.GetValue(value1001, null) + (decimal)pinfo.GetValue(value2001, null);
            if (!((decimal)pinfo.GetValue(value0001, null)).Equals(summ))
            {
                result += Msg.FormatWith(UiBuilders.DescriptionOf(expr));
            }
            
            return result;
        }

        /// <summary>
        /// Контроли в рамках нескольких таблиц
        /// </summary>
        private string CrossTableControl(int docId)
        {
            const string Msg = "Значение графы \"Всего\" строки \"260\" детализации \"Плановые показатели поступлений и выплат\" за {0} год,"
                                + " не равно значению графы \"{1}\" строки \"0001\" детализации \"Показатели выплат по расходам на закупку\"<br>";

            const string Msg1 = "Значение графы \"в соответствии с 44-ФЗ {0}\" строки \"0001\" детализации \"Показатели выплат по расходам на закупку\""
                                + " не может быть меньше суммы показателей по строке \"260\" граф "
                                + "\"субсидии на финансовое обеспечение выполнения государственного (муниципального) задания\" "
                                + "\"субсидии на финансовое обеспечение выполнения государственного задания из бюджета ФФОМС\" "
                                + "\"субсидии на иные цели\" "
                                + "\"субсидии на кап. вложения\" " 
                                + "\"средства ОМС\""
                                + " детализации \"Плановые показатели поступлений и выплат\"<br>";

            const string Msg2 = "Значение графы \"в соответствии с 223-ФЗ {0}\" строки \"0001\" детализации \"Показатели выплат по расходам на закупку\""
                                + " не может быть больше показателя по строке \"260\" графы "
                                + "\"поступления от оказания платных услуг (выполнения работ) и от иной приносящей доход деятельности.всего\""
                                + " детализации \"Плановые показатели поступлений и выплат\"<br>";

            const string Msg3 = "Значение графы \"в соответствии с 44-ФЗ {0}\" строки \"0001\" детлаизации \"Показатели выплат по расходам на закупку\""
                                + " не может быть меньше значения показателя по строке \"260\" графы "
                                + "\"субсидии на кап. вложения\" "
                                + " детализации \"Плановые показатели поступлений и выплат\"<br>";

            string result = string.Empty;

            var typeInst = StateSystemService.GetItem<F_F_ParameterDoc>(docId).RefUchr.RefTipYc.ID;
            
            var expensePaymentIndex = StateSystemService.GetItem<F_F_ParameterDoc>(docId).ExpensePaymentIndex.FirstOrDefault(x => x.LineCode.Equals("0001"));

            if (expensePaymentIndex != null)
            {
                var planPaymentIndex = StateSystemService.GetItem<F_F_ParameterDoc>(docId).PlanPaymentIndex
                                                    .FirstOrDefault(x => x.Period.Equals(0) && x.LineCode.Equals("260"));
                if (planPaymentIndex != null)
                {
                    // контроль 1 за очередной год
                    if (!planPaymentIndex.Total.Equals(expensePaymentIndex.TotalSumNextYear))
                    {
                        result += Msg.FormatWith(
                                            year,
                                            UiBuilders.DescriptionOf(() => expensePaymentIndexViewModel.TotalSumNextYear));
                    }

                    // контроль 2 и 4 за очередной год
                    if (typeInst.Equals(FX_Org_TipYch.BudgetaryID))
                    {
                        var summPlanPaymentIndex = planPaymentIndex.Summ5678().GetValueOrDefault();
                        var fz44SumNextYear = expensePaymentIndex.Fz44SumNextYear;

                        var serviceTotal = planPaymentIndex.ServiceTotal.GetValueOrDefault();
                        var fz223SumNextYear = expensePaymentIndex.Fz223SumNextYear;

                        if (fz44SumNextYear < summPlanPaymentIndex)
                        {
                            result += Msg1.FormatWith("на {0}г.очередной финансовый год".FormatWith(year));
                        }

                        if (fz223SumNextYear > serviceTotal)
                        {
                            result += Msg2.FormatWith("на {0}г.очередной финансовый год".FormatWith(year));
                        }
                    }

                    // контроль 3 за очередной год
                    if (typeInst.Equals(FX_Org_TipYch.AutonomousID))
                    {
                        var сapitalInvestment = planPaymentIndex.CapitalInvestment.GetValueOrDefault();
                        var fz44SumNextYear = expensePaymentIndex.Fz44SumNextYear;

                        if (fz44SumNextYear < сapitalInvestment)
                        {
                            result += Msg3.FormatWith("на {0}г.очередной финансовый год".FormatWith(year));
                        }
                    }
                }
                 
                planPaymentIndex = StateSystemService.GetItem<F_F_ParameterDoc>(docId).PlanPaymentIndex
                                                .FirstOrDefault(x => x.Period.Equals(1) && x.LineCode.Equals("260"));

                if (planPaymentIndex != null)
                {
                    // контроль 1 за 1й плановый период
                    if (!planPaymentIndex.Total.Equals(expensePaymentIndex.TotalSumFirstPlanYear))
                    {
                        result += Msg.FormatWith(
                            year + 1,
                            UiBuilders.DescriptionOf(() => expensePaymentIndexViewModel.TotalSumFirstPlanYear));
                    }

                    // контроль 2 и 4 за 1й плановый период
                    if (typeInst.Equals(FX_Org_TipYch.BudgetaryID))
                    {
                        var summPlanPaymentIndex = planPaymentIndex.Summ5678().GetValueOrDefault();
                        var fz44SumFirstPlanYear = expensePaymentIndex.Fz44SumFirstPlanYear;

                        var serviceTotal = planPaymentIndex.ServiceTotal.GetValueOrDefault();
                        var fz223SumFirstPlanYear = expensePaymentIndex.Fz223SumFirstPlanYear;

                        if (fz44SumFirstPlanYear < summPlanPaymentIndex)
                        {
                            result += Msg1.FormatWith(
                                "на {0}г.1-ый год планового периода".FormatWith(year + 1));
                        }

                        if (fz223SumFirstPlanYear > serviceTotal)
                        {
                            result += Msg2.FormatWith("на {0}г.1-ый год планового периода".FormatWith(year + 1));
                        }
                    }

                    // контроль 3 за 1й плановый период
                    if (typeInst.Equals(FX_Org_TipYch.AutonomousID))
                    {
                        var сapitalInvestment = planPaymentIndex.CapitalInvestment.GetValueOrDefault();
                        var fz44SumFirstPlanYear = expensePaymentIndex.Fz44SumFirstPlanYear;

                        if (fz44SumFirstPlanYear < сapitalInvestment)
                        {
                            result += Msg3.FormatWith("на {0}г.1-ый год планового периода".FormatWith(year + 1));
                        }
                    }
                }
                    
                planPaymentIndex = StateSystemService.GetItem<F_F_ParameterDoc>(docId).PlanPaymentIndex
                                                                .FirstOrDefault(x => x.Period.Equals(2) && x.LineCode.Equals("260"));

                if (planPaymentIndex != null)
                {
                    // контроль 1 за 2й плановый период
                    if (!planPaymentIndex.Total.Equals(expensePaymentIndex.TotalSumSecondPlanYear))
                    {
                        result += Msg.FormatWith(
                                year + 2,
                                UiBuilders.DescriptionOf(() => expensePaymentIndexViewModel.TotalSumSecondPlanYear));
                    }

                    // контроль 2 и 4 за 2й плановый период
                    if (typeInst.Equals(FX_Org_TipYch.BudgetaryID))
                    {
                        var summPlanPaymentIndex = planPaymentIndex.Summ5678().GetValueOrDefault();
                        var fz44SumSecondPlanYear = expensePaymentIndex.Fz44SumSecondPlanYear;

                        var serviceTotal = planPaymentIndex.ServiceTotal.GetValueOrDefault();
                        var fz223SumSecondPlanYear = expensePaymentIndex.Fz223SumSecondPlanYear;

                        if (fz44SumSecondPlanYear < summPlanPaymentIndex)
                        {
                            result += Msg1.FormatWith(
                                "на {0}г.2-ый год планового периода".FormatWith(year + 2));
                        }

                        if (fz223SumSecondPlanYear > serviceTotal)
                        {
                            result += Msg2.FormatWith("на {0}г.2-ый год планового периода".FormatWith(year + 2));
                        }
                    }

                    // контроль 3 за 2й плановый период
                    if (typeInst.Equals(FX_Org_TipYch.AutonomousID))
                    {
                        var сapitalInvestment = planPaymentIndex.CapitalInvestment.GetValueOrDefault();
                        var fz44SumSecondPlanYear = expensePaymentIndex.Fz44SumSecondPlanYear;

                        if (fz44SumSecondPlanYear < сapitalInvestment)
                        {
                            result += Msg3.FormatWith("на {0}г.2-ый год планового периода".FormatWith(year + 2));
                        }
                    }
                }
            }

            return result;
        }
    }
}
