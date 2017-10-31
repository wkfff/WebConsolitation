using System;
using System.Linq;
using System.Text;
using Ext.Net.MVC;
using Krista.FM.Domain;
using Krista.FM.Extensions;
using Krista.FM.RIA.Extensions.E86N.Services;
using Krista.FM.RIA.Extensions.E86N.Utils;

namespace Krista.FM.RIA.Extensions.E86N.Presentation.Controllers.StateSysyem
{
    public sealed class ResultsOfActivityStateSysController : StateSysBaseController
    {
        private readonly ICommonDataService commonDataService;

        public ResultsOfActivityStateSysController(ICommonDataService commonDataService)
        {
            this.commonDataService = commonDataService;
        }

        /// <summary>
        ///   настраиваем вместе с кнопками и редактируемость интерфейса
        ///   в зависимости от состояния и пользователя
        /// </summary>
        public override StringBuilder InitComponents(int docId)
        {
            var resultScript = base.InitComponents(docId);

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

                    // Если АУ/БУ полько экпорт из ГЗ(https://panda.krista.ru/issues/6689)
                    var profile = Auth.ProfileOrg.RefTipYc.ID;
                    if (profile == FX_Org_TipYch.BudgetaryID || profile == FX_Org_TipYch.AutonomousID)
                    {
                        resultScript.Append(ReadOnlySpecialScript());
                    }

                    // если ГРБС и текущий документ создан для АУ/БУ полько экпорт из ГЗ.
                    profile = commonDataService.GetTypeOfInstitution(docId).ID;
                    if (Auth.IsGrbsUser() && (profile == FX_Org_TipYch.BudgetaryID || profile == FX_Org_TipYch.AutonomousID))
                    {
                        resultScript.Append(ReadOnlySpecialScript());
                    }
                }
            }

            return resultScript;
        }

        /// <summary>
        ///   Экшен выполнения перехода на рассмотрение и на завершение
        /// </summary>
        public AjaxFormResult TransitionWithValidate(int docId, int transitionID)
        {
            try
            {
                string msg;
                return DataValidations(docId, out msg) ? DefaultTransition(docId, transitionID) : GetScriptResult(GetShowMessageScript(msg));
            }
            catch (Exception e)
            {
                return GetScriptResult(GetShowMessageScript(e.Message));
            }
        }

        /// <summary>
        ///   Валидация данных
        /// </summary>
        /// <returns> true - если валидация прошла </returns>
        private bool DataValidations(int docId, out string message)
        {
            const string Msg = "Не заполнена закладка \"{0}\".<p/>";
            const string Msg3 = "Не прикреплен документ типа \"{0}\".<p/>";

            var doc = StateSystemService.GetItem<F_F_ParameterDoc>(docId);
            var yearForm = doc.RefYearForm.ID;
            
            var outMes = new StringBuilder(string.Empty);

            var membersOfStaff = from p in StateSystemService.GetItems<F_ResultWork_Staff>()
                                                            where p.RefParametr.ID == docId
                                                            select p;
            if (!membersOfStaff.Any())
            {
                outMes.Append(Msg.FormatWith("Штат сотрудников"));
            }

            var finNFinAssets =
                from p in StateSystemService.GetItems<F_ResultWork_FinNFinAssets>()
                where p.RefParametr.ID == docId
                select p;
            if (!finNFinAssets.Any())
            {
                outMes.Append(Msg.FormatWith("Финансовые/Нефинансовые активы"));
            }

            if (!commonDataService.GetTypeOfInstitution(docId).ID.Equals(FX_Org_TipYch.GovernmentID))
            {
                if (yearForm < 2016)
                {
                    var showService =
                        from p in StateSystemService.GetItems<F_ResultWork_ShowService>()
                        where p.RefParametr.ID == docId
                        select p;

                    if (!showService.Any())
                    {
                        outMes.Append(Msg.FormatWith("Услуги/работы"));
                    }
                }
                else
                {
                    var showService2016 =
                        from p in StateSystemService.GetItems<F_F_ShowService2016>()
                        where p.RefParametr.ID == docId
                        select p;

                    if (!showService2016.Any())
                    {
                        outMes.Append(Msg.FormatWith("Услуги/работы"));
                    }
                }
            }

            var cashReceipts =
                from p in StateSystemService.GetItems<F_ResultWork_CashReceipts>()
                where p.RefParametr.ID == docId
                select p;
            if (!cashReceipts.Any())
            {
                outMes.Append(Msg.FormatWith("Кассовые поступления"));
            }

            if (!(yearForm >= 2016 && doc.RefUchr.RefTipYc.ID.Equals(FX_Org_TipYch.GovernmentID)))
            {
                var cashPay = from p in StateSystemService.GetItems<F_ResultWork_CashPay>()
                              where p.RefParametr.ID == docId
                              select p;
                if (!cashPay.Any())
                {
                    outMes.Append(Msg.FormatWith("Кассовые выплаты"));
                }
            }
            
            var useProperty =
                from p in StateSystemService.GetItems<F_ResultWork_UseProperty>()
                where p.RefParametr.ID == docId
                select p;
            if (!useProperty.Any())
            {
                outMes.Append(Msg.FormatWith("Использование имущества"));
            }

            var docs = from p in StateSystemService.GetItems<F_Doc_Docum>()
                        where (p.RefParametr.ID == docId) &&
                              (p.Url != "НетФайла") &&
                              (p.RefTypeDoc.Code == "D")
                        select new { p.ID };

            if (!docs.Any())
            {
                outMes.Append(Msg3.FormatWith(GetNameTypeDoc("D")));
            }

            message = outMes.ToString();

            return string.IsNullOrEmpty(message);
        }

        private string ReadOnlyScript(bool readOnly, int docId)
        {
            return "window.E86n.Control.StateToolBar.ReadOnlyDocHandler({0}, {1});".FormatWith(readOnly.ToString().ToLower(), docId);
        }

        private string ReadOnlySpecialScript()
        {
            return string.Concat(
                "E86n.View.ResultsOfActivityView.Common.",
                "SetReadOnlyResultsOfActivitySpecial();");
        }
    }
}
