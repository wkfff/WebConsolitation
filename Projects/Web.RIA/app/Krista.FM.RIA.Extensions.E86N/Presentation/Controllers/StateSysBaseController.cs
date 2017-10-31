using System;
using System.Linq;
using System.Text;
using Ext.Net;
using Ext.Net.MVC;
using Krista.FM.Domain;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Extensions.E86N.Auth.Services;
using Krista.FM.RIA.Extensions.E86N.Services;
using Krista.FM.RIA.Extensions.E86N.Services.ExportGMU;
using GlobalConsts = Krista.FM.RIA.Extensions.E86N.Utils.GlobalConsts;

namespace Krista.FM.RIA.Extensions.E86N.Presentation.Controllers
{
    /// <summary>
    /// Базовый контроллер системы смены состояний
    /// </summary>
    public class StateSysBaseController : SchemeBoundController
    {
        public const string StateSysScope = "E86n.Control.StateToolBar";

        public readonly IStateSystemService StateSystemService;
        public readonly IVersioningService VersioningService;
        public readonly IAuthService Auth;
        
        public StateSysBaseController()
        {
            StateSystemService = Resolver.Get<IStateSystemService>();
            VersioningService = Resolver.Get<IVersioningService>();
            Auth = Resolver.Get<IAuthService>();
        }

        /// <summary>
        /// Получение типа документа общей информации
        /// </summary>
        /// <param name="code">код типа документа</param>
        /// <returns>Наименование типа документа</returns>
        public string GetNameTypeDoc(string code)
        {
            var data = from p in StateSystemService.GetItems<D_Doc_TypeDoc>()
                       where (p.Code == code)
                       select new { p.Name };
            return data.First().Name;
        }

        /// <summary>
        /// Проверка прикрепленного документа по коду
        /// </summary>
        /// <param name="code">Код документа</param>
        /// <param name="docId">Идентификатор документ</param>
        /// <returns>true - если прикреплен</returns>
        public bool CheckAttachment(string code, int docId)
        {
            return (from p in StateSystemService.GetItems<F_Doc_Docum>()
                    where (p.RefParametr.ID == docId) &&
                            (p.Url != "НетФайла") &&
                            (p.RefTypeDoc.Code == code)
                    select new { p.ID }).Any();
        }

        /// <summary>
        /// Скрипт вывода сообщения  Ext.Msg.show
        /// </summary>
        /// <param name="message">Выводимое сообщение</param>
        /// <param name="title">Заголовок сообщения</param>
        /// <param name="typeMsg">Тип сообщения</param>
        /// <returns>Скрипт отображения сообщения Ext.Msg.show</returns>
        public string GetShowMessageScript(
                                            string message = "",
                                            string title = "Ошибка",
                                            MessageBox.Icon typeMsg = MessageBox.Icon.ERROR)
        {
            string script = @" Ext.Msg.show({{title: '{0}',
                            msg: '{1}',
                            buttons: Ext.Msg.OK,
                            icon: Ext.MessageBox.{2},
                            maxWidth: 1000
                            }});"
                    .FormatWith(
                                title,
                                message.Replace("'", "\'").Replace("\r", string.Empty).Replace("\n", "<br>").Replace("\\", "\\\\"),
                                typeMsg);
            return script;
        }

        /// <summary>
        /// Формирование результата скриптом
        /// </summary>
        /// <param name="script">Java скрипт</param>
        /// <returns>Результат метода действия в виде AjaxFormResult</returns>
        public AjaxFormResult GetScriptResult(string script = "")
        {
            var result = new AjaxFormResult { Success = true, Script = script };

            return result;
        }

        /// <summary>
        /// инициализация кнопок перехода по состоянию документа
        /// при перекрытии можно настроить действия над любыми другими компонентами интерфейса
        /// </summary>
        /// <param name="docId">Идентификатор документа</param>
        /// <returns>Скрипт настройки интерфейса документа</returns>
        public virtual StringBuilder InitComponents(int docId)
        {
            var resultScript = new StringBuilder();

            int? schemTransitions =
                StateSystemService.GetSchemStateTransitionsID(StateSystemService.GetTypeDocID(docId));

            if (schemTransitions != null)
            {
                IQueryable<D_State_Transitions> transitions = StateSystemService
                                                    .GetTransitions(schemTransitions.Value);

                if (!VersioningService.GetCloseState(docId))
                {
                    foreach (D_State_Transitions transition in transitions)
                    {
                        if (StateSystemService.CheckAllowTranstion(
                                                    StateSystemService.GetCurrentStateID(docId),
                                                    transition.ID))
                        {
                            // переход разрешен для текущего состояния
                            if (StateSystemService.CheckRightsTransition(
                                                        StateSystemService.GetCurrentStateID(docId),
                                                        transition.ID))
                            {
                                // есть права на переход
                                resultScript.AppendLine(GlobalConsts.BtnTransition + transition.ID + ".setDisabled(false);");
                            }
                            else
                            {
                                // нет права на переход
                                resultScript.AppendLine(GlobalConsts.BtnTransition + transition.ID +
                                                                                ".setDisabled(true);");
                                resultScript.AppendLine(GlobalConsts.BtnTransition + transition.ID +
                                                        ".setTooltip('{0} <br/> {1}');"
                                                        .FormatWith(transition.Note, "Нет прав на переход"));
                            }
                        }
                        else
                        {
                            // данный переход не разрешен для текущего состояния
                            resultScript.AppendLine(GlobalConsts.BtnTransition + transition.ID + ".setDisabled(true);");
                        }
                    }
                }
            }

            return resultScript;
        }

        /// <summary>
        /// Инициализация интерфейса 
        /// выполняет открытие и закрытие кнопок переходов по условиям(роли, настройки переходов)
        /// и вызывает пользовательский метод определения пользовательского скрипта для закрытия 
        /// других элементов интерфейса
        /// </summary>
        /// <param name="docId">Идентификатор документа</param>
        /// <returns>Результат со скриптом настройки документа</returns>
        public virtual AjaxFormResult InitialView(int docId)
        {
            return GetScriptResult(InitComponents(docId).ToString());
        }

        /// <summary>
        /// Экшен выполнения перехода по умолчанию
        /// </summary>
        /// <param name="docId">Идентификатор документа</param>
        /// <param name="transitionID"> ID перехода </param>
        /// <returns>Скрипт настройки документа</returns>
        public virtual AjaxFormResult DefaultTransition(int docId, int transitionID)
        {
            try
            {
                StateSystemService.Jump(docId, transitionID);
                StringBuilder resultScript = InitComponents(docId);
                resultScript.AppendLine("dsParamDoc{0}.reload();".FormatWith(docId));
                return GetScriptResult(resultScript.ToString());
            }
            catch (Exception e)
            {
                return GetScriptResult(GetShowMessageScript(e.Message));
            }
        }

        /// <summary>
        /// Экшен Экспорт на сайт ГМУ
        /// </summary>
        /// <param name="name">Логин пользователя ГМУ</param>
        /// <param name="pass">Пароль пользователя ГМУ</param>
        /// <param name="docs">Идентификатор документа</param>
        /// <param name="transitionID">Идентификатор перехода</param>
        /// <returns>Результат экспорта на ГМУ</returns>
        public virtual AjaxFormResult Index(string name, string pass, int docs, int transitionID)
        {
            var result = new ExportGmuService(Scheme).ExportGmu(name, pass, docs);

            if (result.Success)
            {
                var resultScript = new StringBuilder();

                // выполняем переход
                resultScript.AppendLine(DefaultTransition(docs, transitionID).Script);

                // меняем примечание документа
                StateSystemService.ChangeNotes(docs, " Export confirm:{0}".FormatWith(result.ExtraParams["note"]));
                
                // закрываем документ
                VersioningService.CloseDocument(docs);

                result.Script = resultScript.ToString();

                return result;
            }

            return result;
        }

        /// <summary>
        /// Экшен смены состояния документа
        /// </summary>
        /// <param name="docId">Идентификатор документа</param>
        /// <param name="stateId">Идентификатор состояния </param>
        /// <returns>Скрипт настройки документа</returns>
        public virtual AjaxFormResult SetStateDoc(int docId, int stateId)
        {
            try
            {
                if (Auth.IsAdmin())
                {
                    StateSystemService.SetState(docId, stateId);
                    StringBuilder resultScript = InitComponents(docId);
                    resultScript.AppendLine("dsParamDoc{0}.reload();".FormatWith(docId));
                    return GetScriptResult(resultScript.ToString());
                }

                throw new Exception("У вас нет прав на смену состояния документа");
            }
            catch (Exception e)
            {
                return GetScriptResult(GetShowMessageScript(e.Message));
            }
        }
    }
}