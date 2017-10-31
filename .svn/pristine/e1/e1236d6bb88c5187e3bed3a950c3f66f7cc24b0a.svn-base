using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Ext.Net.MVC;
using Krista.FM.Domain;
using Krista.FM.Extensions;
using Krista.FM.RIA.Extensions.E86N.Utils;

namespace Krista.FM.RIA.Extensions.E86N.Presentation.Controllers.StateSysyem
{
    public class SmetaStateSysController : StateSysBaseController
    {
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
        ///   Проверка заполнения данных на интерфейсе
        /// </summary>
        public string CheckData(int docId)
        {
            const string Msg = "Нет данных.<br>";
            const string Msg1 = "Не указана целевая статья <br>";
            const string Msg2 = "Не указаны суммы ни за один год <br>";
            const string Msg3 = "Не указана сумма за {0}г. <br>";

            var smeta = StateSystemService.GetItems<F_Fin_Smeta>().Where(x => x.RefParametr.ID == docId);

            if (!smeta.Any())
            {
                return Msg;
            }

            var message = string.Empty;

            foreach (var rec in smeta)
            {
                if (string.IsNullOrEmpty(rec.CelStatya) || !Regex.IsMatch(rec.CelStatya, @"[^0\.]"))
                {
                    message += Msg1;
                }

                if (rec.RefParametr.PlanThreeYear)
                {
                    if ((rec.Funds == null || rec.Funds == 0) &&
                        (rec.FundsOneYear == 0) &&
                        (rec.FundsTwoYear == null || rec.FundsTwoYear == 0))
                    {
                        message += Msg2;
                    }
                }
                else
                {
                    if (rec.Funds == null || rec.Funds == 0)
                    {
                        message += Msg3.FormatWith(rec.RefParametr.RefYearForm.ID);
                    }
                }
            }

            return message;
        }

        /// <summary>
        ///   Экшен выполнения перехода на рассмотрение и на завершение
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

            var docs = from p in StateSystemService.GetItems<F_Doc_Docum>()
                        where (p.RefParametr.ID == docId) &&
                              (p.Url != "НетФайла") &&
                              (p.RefTypeDoc.Code == "B")
                        select new { p.ID };

            if (!docs.Any())
            {
                message += Msg.FormatWith(GetNameTypeDoc("B"));
            }

            message += CheckData(docId);

            return string.IsNullOrEmpty(message);
        }
    }
}
