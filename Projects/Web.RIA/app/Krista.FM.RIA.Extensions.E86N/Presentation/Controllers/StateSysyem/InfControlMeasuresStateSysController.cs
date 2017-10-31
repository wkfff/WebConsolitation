using System;
using System.Linq;
using System.Text;

using Ext.Net.MVC;

using Krista.FM.Domain;
using Krista.FM.Extensions;
using Krista.FM.RIA.Extensions.E86N.Services.InfControlMeasures;
using Krista.FM.RIA.Extensions.E86N.Utils;

namespace Krista.FM.RIA.Extensions.E86N.Presentation.Controllers.StateSysyem
{
    public class InfControlMeasuresStateSysController : StateSysBaseController
    {
        private const string Scope = "E86n.View.InfControlMeasures.";
        
        /// <summary>
        /// Экшен выполнения перехода на рассмотрение
        /// </summary>
        /// <param name="docId">Идентификатор документа</param>
        /// <param name="transitionID">Идентификатор перехода</param>
        /// <returns>Скрипт показа сообщения </returns>
        public virtual AjaxFormResult TransitionUnderConsideration(int docId, int transitionID)
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
        /// Экшен выполнения перехода на завершение
        /// </summary>
        /// <param name="docId">Идентификатор документа</param>
        /// <param name="transitionID">Идентификатор перехода</param>
        /// <returns>Скрипт показа сообщения </returns>
        public virtual AjaxFormResult TransitionFinished(int docId, int transitionID)
        {
            try
            {
                string msg;
                if (DataValidations(docId, out msg))
                {
                    if (StateSystemService.GetItems<T_Fact_ExtHeader>().Any(x => x.RefParametr.ID == docId && x.NotInspectionActivity))
                    {
                        // при завершении документа без мероприятий он сразу закрывается
                        VersioningService.CloseDocument(docId);

                        StateSystemService.Jump(docId, transitionID);
                        return GetScriptResult("window.location.reload();");
                    }

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
        /// <returns>Скрипт настройки интерфейса</returns>
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

                            if (StateSystemService.GetItems<T_Fact_ExtHeader>().Any(x => x.RefParametr.ID == docId && x.NotInspectionActivity))
                            {
                                // если галочка стоит то закрываем документ для редактирования
                                resultScript.Append(Scope + "SetReadOnly(true, {0});".FormatWith(docId));
                            }

                            break;
                    }
                }
            }

            return resultScript;
        }

        /// <summary>
        /// Валидация данных
        /// </summary>
        /// <param name="docId">идентификатор документа</param>
        /// <param name="message">Сообщение ошибки</param>
        /// <returns>true - если валидация прошла</returns>
        private bool DataValidations(int docId, out string message)
        {
            const string Msg = "Не заполнена закладка \"{0}\".<p/>";
            const string Msg3 = "Не прикреплен документ типа \"{0}\".<p/>";
            const string Msg2 = "Неправильно задан год \"{0}\" Должен быть {1} <br>";

            message = string.Empty;

            // если не стоит галочка "Нет контрольных мероприятий" то выполняем контроли
            if (!StateSystemService.GetItems<T_Fact_ExtHeader>().Any(x => x.RefParametr.ID == docId && x.NotInspectionActivity))
            {
                IQueryable<F_Fact_InspectionEvent> infControlMeasures = from p in StateSystemService.GetItems<F_Fact_InspectionEvent>()
                                                                        where (p.RefParametr.ID == docId)
                                                                        select p;
                if (!infControlMeasures.Any())
                {
                    message += Msg.FormatWith("Сведения о проведенных контрольных мероприятиях и их результатах");
                }

                var docs = from p in StateSystemService.GetItems<F_Doc_Docum>()
                           where (p.RefParametr.ID == docId) &&
                                 (p.Url != "НетФайла") &&
                                 (p.RefTypeDoc.Code == "G")
                           select new { p.ID };

                if (!docs.Any())
                {
                    message += Msg3.FormatWith(GetNameTypeDoc("G"));
                }

                docs = from p in StateSystemService.GetItems<F_Doc_Docum>()
                       where (p.RefParametr.ID == docId) &&
                             (p.Url != "НетФайла") &&
                             (p.RefTypeDoc.Code == "H")
                       select new { p.ID };

                if (!docs.Any())
                {
                    message += Msg3.FormatWith(GetNameTypeDoc("H"));
                }

                // ограничиваем повторы однотипных сообщений
                var eventBeginMes = false;
                var eventEndMes = false;

                foreach (var item in infControlMeasures)
                {
                    var yearForm = item.RefParametr.RefYearForm.ID;

                    if (item.EventBegin.Year != yearForm && !eventBeginMes)
                    {
                        eventBeginMes = true;
                        message += Msg2.FormatWith(InfControlMeasuresHelpers.InfControlMeasuresNameMapping(InfControlMeasures.EventBegin), yearForm);
                    }

                    var y = item.EventEnd.Year;
                    if ((y != yearForm) && (y != yearForm + 1) && !eventEndMes)
                    {
                        eventEndMes = true;
                        message += Msg2.FormatWith(InfControlMeasuresHelpers.InfControlMeasuresNameMapping(InfControlMeasures.EventEnd), yearForm)
                                            + "или {0}".FormatWith(yearForm + 1);
                    }

                    if (eventBeginMes && eventEndMes)
                    {
                        break;
                    }
                }
            }

            return string.IsNullOrEmpty(message);
        }

        private string ReadOnlyScript(bool readOnly, int docId)
        {
            var resultScript = new StringBuilder();
            resultScript.Append("window.E86n.Control.StateToolBar.ReadOnlyDocHandler({0}, {1});".FormatWith(readOnly.ToString().ToLower(), docId));
            return resultScript.ToString();
        }
    }
}