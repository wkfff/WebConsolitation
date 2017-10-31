using System;
using System.Linq;
using System.Text;
using Ext.Net.MVC;
using Krista.FM.Domain;
using Krista.FM.Extensions;
using Krista.FM.RIA.Extensions.E86N.Utils;

namespace Krista.FM.RIA.Extensions.E86N.Presentation.Controllers.StateSysyem
{
    public class DiverseInfoStateSysController : StateSysBaseController
    {
        /// <summary>
        /// Экшен выполнения перехода на рассмотрение и на завершение
        /// </summary>
        /// <param name="docId">Идентификатор документа</param>
        /// <param name="transitionID">Идентификатор перехода</param>
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
                resultScript.Append(ReadOnlyScript(true));
                return resultScript;
            }

            if (!Auth.IsAdmin())
            {
                int stateCode = StateSystemService.GetCurrentStateID(docId);

                if (Auth.IsPpoUser() && !Auth.IsGrbsUser())
                {
                    // если ФО то только чтение
                    resultScript.Append(ReadOnlyScript(true));
                }
                else
                {
                    // если ГРБС или учреждение
                    switch (stateCode)
                    {
                        case (int)StatesType.UnderConsideration:
                            resultScript.Append(Auth.IsGrbsUser() ? ReadOnlyScript(false) : ReadOnlyScript(true));
                            break;
                        case (int)StatesType.Completed:
                            resultScript.Append(ReadOnlyScript(true));
                            break;
                        case (int)StatesType.Exported:
                            resultScript.Append(ReadOnlyScript(true));
                            break;
                        default:
                            resultScript.Append(ReadOnlyScript(false));
                            break;
                    }
                }
            }

            return resultScript;
        }

        private string ReadOnlyScript(bool readOnly)
        {
            var resultScript = new StringBuilder();
            resultScript.Append("window.E86n.Control.StateToolBar.ReadOnlyDocHandler({0});".FormatWith(readOnly.ToString().ToLower()));
            return resultScript.ToString();
        }

        /// <summary>
        /// Валидация данных
        /// </summary>
        /// <param name="docId">Идентификатор документа</param>
        /// <param name="message">сообщение об ошибке</param>
        /// <returns>true - если валидация прошла</returns>
        private bool DataValidations(int docId, out string message)
        {
            const string Msg = "Не заполнена детализация \"{0}\".<br>";
            
            message = string.Empty;

            var emptyTargets = string.Empty;

            var doc = StateSystemService.GetItem<F_F_ParameterDoc>(docId);

            doc.PaymentDetails.Each(x =>
                                        {
                                            if (!x.PaymentDetailsTargets.Any())
                                            {
                                                emptyTargets = Msg.FormatWith("Назначение платежа");
                                            }
                                        });

            message += emptyTargets;

            if (!doc.TofkList.Any())
            {
                message = Msg.FormatWith("Перечень организаций, в которых открыты счета");
            }

            return string.IsNullOrEmpty(message);
        }
    }
}