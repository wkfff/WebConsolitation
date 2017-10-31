using System;
using System.Linq;
using System.Text;

using bus.gov.ru;

using Ext.Net.MVC;

using Krista.FM.Domain;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Extensions.E86N.Models.AnnualBalance;
using Krista.FM.RIA.Extensions.E86N.Services.AnnualBalance;
using Krista.FM.RIA.Extensions.E86N.Services.Params;
using Krista.FM.RIA.Extensions.E86N.Utils;

namespace Krista.FM.RIA.Extensions.E86N.Presentation.Controllers.StateSysyem
{
    public class AnnualBalanceStateSysController : StateSysBaseController
    {
        private string name;

        /// <summary>
        /// Экшен выполнения перехода на рассмотрение и на завершение
        /// </summary>
        /// <param name="docId">Идентификатор документа</param>
        /// <param name="transitionID">Идентификатор перехода</param>
        public virtual AjaxFormResult TransitionWithValidate(int docId, int transitionID)
        {
            try
            {
                IDataPumpProtocolProvider dataPumpProtocolProvider = new ConfirmationDataPumpProtocolProvider();
                if (DataValidations(docId, dataPumpProtocolProvider))
                {
                    return DefaultTransition(docId, transitionID);
                }

                Resolver.Get<IParamsMap>().SetParam("Protocol", dataPumpProtocolProvider);

                return GetScriptResult(GetShowProtocolScript());
            }
            catch (Exception e)
            {
                return GetScriptResult(GetShowMessageScript(e.Message));
            }
        }

        /// <summary>
        /// настраиваем вместе с кнопками и редактируемость интерфейса
        /// в зависимости от состояния и пользователя
        /// </summary>
        /// <param name="docId">Идентификатор документа</param>
        /// <returns>StringBuilder - скрипт </returns>
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
                int stateCode = StateSystemService.GetCurrentStateID(docId);

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
        /// Валидация данных
        /// </summary>
        /// <param name="docId">Идентификатор документа</param>
        /// <param name="dataPumpProtocolProvider">Протокол валидации</param>
        /// <returns>true - если валидация прошла</returns>
        private bool DataValidations(int docId, IDataPumpProtocolProvider dataPumpProtocolProvider)
        {
            const string Msg = "Не заполнены общие атрибуты документа.";
            const string Msg1 = "Не прикреплен документ типа \"{0}\".";
            const string Msg2 = "Удалите файл с типом \"{0}\" из приложения либо заполните данные.";
            const string Msg3 = "Документ пуст.";

            var doc = StateSystemService.GetItem<F_F_ParameterDoc>(docId);

            name = "Валидация документа: {0}".FormatWith(doc.RefPartDoc.Name);

            var headAttribute = doc.ReportHeadAttribute;

            if (!headAttribute.Any())
            {
                dataPumpProtocolProvider.WriteEventIntoDataPumpProtocol(DataPumpEventType.Error, name, Msg);
            }

            var typeDoc = StateSystemService.GetTypeDocID(docId);

            switch (typeDoc)
            {
                case FX_FX_PartDoc.AnnualBalanceF0503730Type:
                    {
                        if (!CheckAttachment("K", docId))
                        {
                            dataPumpProtocolProvider.WriteEventIntoDataPumpProtocol(DataPumpEventType.Error, name, Msg1.FormatWith(GetNameTypeDoc("K")));
                        }

                        break;
                    }

                case FX_FX_PartDoc.AnnualBalanceF0503121Type:
                    {
                        if (!CheckAttachment("R", docId))
                        {
                            dataPumpProtocolProvider.WriteEventIntoDataPumpProtocol(DataPumpEventType.Error, name, Msg1.FormatWith(GetNameTypeDoc("R")));
                        }

                        break;
                    }

                case FX_FX_PartDoc.AnnualBalanceF0503127Type:
                    {
                        if (!CheckAttachment("T", docId))
                        {
                            dataPumpProtocolProvider.WriteEventIntoDataPumpProtocol(DataPumpEventType.Error, name, Msg1.FormatWith(GetNameTypeDoc("T")));
                        }

                        ValidateF0503127(docId, dataPumpProtocolProvider);

                        break;
                    }

                case FX_FX_PartDoc.AnnualBalanceF0503130Type:
                    {
                        if (!CheckAttachment("Q", docId))
                        {
                            dataPumpProtocolProvider.WriteEventIntoDataPumpProtocol(DataPumpEventType.Error, name, Msg1.FormatWith(GetNameTypeDoc("Q")));
                        }

                        break;
                    }

                case FX_FX_PartDoc.AnnualBalanceF0503137Type:
                    {
                        if (!CheckAttachment("U", docId))
                        {
                            dataPumpProtocolProvider.WriteEventIntoDataPumpProtocol(DataPumpEventType.Error, name, Msg1.FormatWith(GetNameTypeDoc("U")));
                        }

                        break;
                    }

                case FX_FX_PartDoc.AnnualBalanceF0503721Type:
                    {
                        if (!CheckAttachment("M", docId))
                        {
                            dataPumpProtocolProvider.WriteEventIntoDataPumpProtocol(DataPumpEventType.Error, name, Msg1.FormatWith(GetNameTypeDoc("M")));
                        }

                        ValidateF0503721(docId, dataPumpProtocolProvider);

                        break;
                    }

                case FX_FX_PartDoc.AnnualBalanceF0503737Type:
                    {
                        var typeFinSupportAttach =
                            StateSystemService.GetItems<F_Report_BalF0503737>().Where(x => x.RefParametr.ID == docId)
                                    .Where(
                                        x => x.approvePlanAssign != 0 || x.execPersonAuthorities != 0 || x.execBankAccounts != 0 ||
                                        x.execCashAgency != 0 || x.execNonCashOperation != 0 || x.execTotal != 0 || x.unexecPlanAssign != 0)
                                    .Select(x => x.RefTypeFinSupport.Code).Distinct().ToArray();

                        if (!typeFinSupportAttach.Any())
                        {
                            dataPumpProtocolProvider.WriteEventIntoDataPumpProtocol(DataPumpEventType.Error, name, Msg3);
                        }

                        foreach (var code in typeFinSupportAttach)
                        {
                            if (!CheckAttachment(code, docId))
                            {
                                dataPumpProtocolProvider.WriteEventIntoDataPumpProtocol(DataPumpEventType.Error, name, Msg1.FormatWith(GetNameTypeDoc(code)));
                            }
                        }

                        var typeFinSupports = StateSystemService.GetItems<FX_FX_typeFinSupport>().Select(x => x.Code);

                        foreach (var code in typeFinSupports)
                        {
                            if (CheckAttachment(code, docId) && !typeFinSupportAttach.Contains(code))
                            {
                                dataPumpProtocolProvider.WriteEventIntoDataPumpProtocol(DataPumpEventType.Error, name, Msg2.FormatWith(GetNameTypeDoc(code)));
                            }
                        }

                        break;
                    }
            }

            return dataPumpProtocolProvider.Confirmation.body.result.Equals("success");
        }

        private void ValidateF0503721(int docId, IDataPumpProtocolProvider dataPumpProtocolProvider)
        {
            var annualBalanceF0503721ViewModel = new AnnualBalanceF0503721ViewModel();
            var doc = StateSystemService.GetItem<F_F_ParameterDoc>(docId).AnnualBalanceF0503721;

            doc.Each(
                x =>
                    {
                        var model = annualBalanceF0503721ViewModel.GetModelByDomain(x);
                        var msg = model.ValidateData();
                        if (msg.IsNotNullOrEmpty())
                        {
                            dataPumpProtocolProvider.WriteEventIntoDataPumpProtocol(DataPumpEventType.Error, name, msg.Replace("<br>", string.Empty));
                        }
                    });
        }

        private void ValidateF0503127(int docId, IDataPumpProtocolProvider dataPumpProtocolProvider)
        {
            const string Msg = "Должна присутствовать строка с кодом \"{0}\" с незаполненным КБК в детализации \"{1}\"";
            var doc = StateSystemService.GetItem<F_F_ParameterDoc>(docId).AnnualBalanceF0503127;
            var lineCode = "010";
            if (!doc.Any(x => x.Section.Equals(0)
                            && x.lineCode.Equals(lineCode)
                            && x.budgClassifCode.IsNullOrEmpty()))
            {
                dataPumpProtocolProvider.WriteEventIntoDataPumpProtocol(
                    DataPumpEventType.Error,
                    name,
                    Msg.FormatWith(lineCode, AnnualBalanceHelpers.F0503127DetailsNameMapping(F0503127Details.BudgetIncomes)));
            }

            lineCode = "200";
            if (!doc.Any(x => x.Section.Equals(1)
                            && x.lineCode.Equals(lineCode)
                            && x.budgClassifCode.IsNullOrEmpty()))
            {
                dataPumpProtocolProvider.WriteEventIntoDataPumpProtocol(
                    DataPumpEventType.Error,
                    name,
                    Msg.FormatWith(lineCode, AnnualBalanceHelpers.F0503127DetailsNameMapping(F0503127Details.BudgetExpenses)));
            }
        }

        private string ReadOnlyScript(bool readOnly, int docId)
        {
            var resultScript = new StringBuilder();
            var typeDoc = StateSystemService.Load<F_F_ParameterDoc>(docId).RefPartDoc.ID;
            resultScript.Append("window.E86n.Control.StateToolBar.ReadOnlyDocHandler({0}, {1}, {2});".FormatWith(readOnly.ToString().ToLower(), docId, typeDoc));
            return resultScript.ToString();
        }

        /// <summary>
        /// Скрипт вывода протокола операции
        /// </summary>
        private string GetShowProtocolScript()
        {
            string script = @"var wnd = top.Ext.getCmp('HBWnd');
                              if (!wnd)
                                {
                                  Ext.MessageBox.alert('Ошибка', 'Не найдено окно: HBWnd', 1);
                                }
                              wnd.autoLoad.url = 'View/ProtocolView';
                              wnd.setTitle('Протокол операции');
                              wnd.show();";
            return script;
        }
}
}