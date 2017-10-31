using System;
using System.Collections.Generic;
using System.Text;

namespace Krista.FM.Client.SchemeEditor.Gui
{
    /// <summary>
    /// Базовый интерфейс для форм, которые отображают
    /// содержимое реализующее интерфейс IViewContent.
    /// </summary>
    public interface IWorkbenchWindow
    {
        /// <summary>
        /// Содержимое окна.
        /// </summary>
        IViewContent ViewContent { get; }

        /// <summary>
        /// Текущее содержимое окна.
        /// </summary>
        IBaseViewContent ActiveViewContent { get; }

        /// <summary>
        /// Заголовок окна.
        /// </summary>
        string Title { get; set; }

        /// <summary>
        /// Закрывает окно, если force == true, то закрывает
        /// без вопроса о сохранении изменений содержимого окна.
        /// </summary>
        /// <returns>true, если окно закрыто.</returns>
        bool CloseWindow(bool force);

        /// <summary>
        /// Делает окно видимым и передает ему фокус.
        /// </summary>
        void SelectWindow();

        /// <summary>
        /// Переинициализация и отрисовка содержимого.
        /// </summary>
        void RedrawContent();

        /// <summary>
        /// Вызывается после закрытия окна.
        /// </summary>
        event EventHandler CloseEvent;
    }
}
