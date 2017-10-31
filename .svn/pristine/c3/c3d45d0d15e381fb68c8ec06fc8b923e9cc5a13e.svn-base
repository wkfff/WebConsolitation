using System;
using System.Collections.Generic;
using System.Text;

namespace Krista.FM.Client.SchemeEditor.Gui
{
    /// <summary>
    /// Отвечает за способ организации представлений рабочего пространства,
    /// отображение представлений, выбор реализации IWorkbenchWindow и т.д.
    public interface IWorkbenchLayout
    {
        /// <summary>
        /// Активное окно рабочего пространства.
        /// </summary>
        IWorkbenchWindow ActiveWorkbenchWindow { get; }

        /// <summary>
        /// Подключает IWorkbenchLayout к IWorkbench.
        /// </summary>
        void Attach(IWorkbench workbench);

        /// <summary>
        /// Отключает IWorkbenchLayout от текущего IWorkbench.
        /// </summary>
        void Detach();

        /// <summary>
        /// Перерисовывает все компоненты менеджера организации представлений.
        /// </summary>
        void RedrawAllComponents();

        /// <summary>
        /// Добавляет новый <see cref="IViewContent"/>.
        /// </summary>
        IWorkbenchWindow ShowView(IViewContent content);

        /// <summary>
        /// Вызывается после смены активного окна рабочего пространства.
        /// </summary>
        event EventHandler ActiveWorkbenchWindowChanged;

        //void OnActiveWorkbenchWindowChanged(EventArgs e);
    }
}
