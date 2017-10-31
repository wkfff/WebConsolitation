using System.Web.UI;
using Ext.Net;
using Ext.Net.Utilities;
using Krista.FM.RIA.Extensions.EO15ExcCostsAIP.Properties;

namespace Krista.FM.RIA.Extensions.EO15ExcCostsAIP.Presentation.Controls
{
    public class CommonControl 
    {
        public const string Scope = "EO15AIP.Control.CommonWin";

        /// <summary>
        /// Создание модального окна для формы с новым объектом строительства
        /// </summary>
        /// <param name="page">Страница, на которомй будет окно</param>
        /// <param name="gridId">Идентификатор грида, который нужно обновить после добавления объекта</param>
        /// <param name="title">Заголовок окна</param>
        /// <param name="bookWindowId">Идентификатор модального окна</param>
        /// <param name="maskMsg">Текст при открытии окна</param>
        /// <param name="acceptHandler">Обработчик на нажатие кнопки "Сохранить"</param>
        /// <param name="btnSaveText">Текст кнопки "Сохранить"</param>
        /// <param name="isSaveBtnVisible">видна ли кнопка "Сохранить"</param>
        /// <returns>Модальное окно</returns>
        public static Window GetBookWindow(Page page, string gridId, string title, string bookWindowId, string maskMsg, string acceptHandler, string btnSaveText, bool isSaveBtnVisible)
        {
            var resourceManager = ResourceManager.GetInstance(page);
            resourceManager.RegisterClientScriptBlock("EO15AIPCommonControl", Resources.EO15AIPCommonControl);

            var win = new Window
            {
                ID = bookWindowId,
                Width = 600,
                Height = 340,
                Title = title,
                Icon = Icon.ApplicationFormEdit,
                Hidden = false,
                Modal = true,
                Constrain = true,
                Listeners =
                {
                    BeforeShow =
                    {
                        Handler = Scope + ".resizeFn('{0}')".FormatWith(bookWindowId)
                    }
                }
            };
            win.AutoLoad.Url = "/";
            win.AutoLoad.Mode = LoadMode.IFrame;
            win.AutoLoad.TriggerEvent = "show";
            win.AutoLoad.ReloadOnEvent = true;
            win.AutoLoad.ShowMask = true;
            win.AutoLoad.MaskMsg = maskMsg;

            var buttonSave = new Button
            {
                ID = "btnOk",
                Text = btnSaveText,
                Icon = Icon.Accept,
                Listeners =
                {
                    Click =
                    {
                        Handler = acceptHandler
                    }
                }
            };

            var buttonCancel = new Button
            {
                ID = "btnCancel",
                Text = @"Отмена",
                Icon = Icon.Cancel,
                Listeners =
                {
                    Click =
                    {
                        Handler = @"{0}.hide()".FormatWith(win.ID)
                    }
                }
            };

            win.Buttons.Add(buttonCancel);
            if (isSaveBtnVisible)
            {
                win.Buttons.Add(buttonSave);
            }

            return win;
        }
    }
}
