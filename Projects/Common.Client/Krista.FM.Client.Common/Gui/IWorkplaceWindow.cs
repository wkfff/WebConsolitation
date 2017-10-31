using System;
using System.Collections.Generic;
using System.Text;

namespace Krista.FM.Client.Common.Gui
{
    /// <summary>
    /// Базовый интерфейс для окна воркплейса в котором отображается содержимое
    /// предоставляемое объектом IViewContent.
    /// </summary>
    public interface IWorkplaceWindow
    {
        /// <summary>
        /// Заголовок окна.
        /// </summary>
        string Title { get; set; }

        /// <summary>
        /// Возвращает true, если окно воркплейса было уничтожено.
        /// </summary>
        bool IsDisposed { get; }

        /// <summary>
        /// Содержимое объекта просмотра в окне.
        /// </summary>
        IViewContent ViewContent { get; }

        /// <summary>
        /// Текущий объекта просмотра в активном окне.
        /// This method is thread-safe.
        /// </summary>
        IViewContent ActiveViewContent { get; }

		/// <summary>
        /// Закрывает окно, если force == true, то закрывает окно
        /// без вопроса, даже если содержимое не сохранено.
        /// </summary>
        /// <returns>true, если окно закрыто.</returns>
        bool CloseWindow(bool force);

        void SelectWindow();

        void OnWindowSelected(EventArgs e);
        void OnWindowDeselected(EventArgs e);

        /// <summary>
        /// Срабатывает после того, как заголовок окна изменился.
        /// </summary>
        event EventHandler TitleChanged;

        /// <summary>
        /// Срабатывает после выделения окна.
        /// </summary>
        event EventHandler WindowSelected;

        /// <summary>
        /// Срабатывает после снятия выделения окна.
        /// </summary>
        event EventHandler WindowDeselected;

        /// <summary>
        /// Срабатывает после закрытия окна.
        /// </summary>
        event EventHandler CloseEvent;

    }
}
