using System;
using System.Collections.Generic;
using System.Text;

namespace Krista.FM.Client.SchemeEditor.Gui
{
    /// <summary>
    /// Главный интерфейс рабочей области приложения
    /// </summary>
    public interface IWorkbench
    {
        /// <summary>
        /// Текст отображаемый в заголовке окна.
        /// </summary>
        string Title { get; set; }

        /// <summary>
        /// Коллекция всех активных представлений рабочего пространства.
        /// </summary>
        List<IViewContent> ViewContentCollection { get; }

        /// <summary>
        /// Активное окно рабочего пространства.
        /// </summary>
        IWorkbenchWindow ActiveWorkbenchWindow { get; }

        /// <summary>
        /// Схема организации и расположения представлений рабочего пространства.
        /// </summary>
        IWorkbenchLayout WorkbenchLayout { get; set; }

        /// <summary>
        /// Добавляет новый <see cref="IViewContent"/> объект в рабочее пространство.
        /// </summary>
        void ShowView(IViewContent content);

        /// <summary>
        /// Закрывает представление IViewContent, если оно открыто.
        /// </summary>
        void CloseContent(IViewContent content);

        /// <summary>
        /// Закрывает все представления в рабочем пространстве.
        /// </summary>
        void CloseAllViews();

        /// <summary>
        /// Перерисовка всех компонентов рабочего пространства, 
        /// далжна вызываться при изменении специальных свойств,
        /// которые влияют на способ организации представлений.
        /// </summary>
        void RedrawAllComponents();

        /// <summary>
        /// Вызывается после открытия представления.
        /// </summary>
        event ViewContentEventHandler ViewOpened;

        /// <summary>
        /// Вызывается после закрытия представления.
        /// </summary>
        event ViewContentEventHandler ViewClosed;

        /// <summary>
        /// Вызывается после смены активного окна рабочего пространства.
        /// </summary>
        event EventHandler ActiveWorkbenchWindowChanged;
    }
}
