using System;
using System.Collections.Generic;
using System.Text;
using Infragistics.Win.UltraWinExplorerBar;
using Krista.FM.ServerLibrary;
using Krista.FM.Client.Common.Forms;
using Infragistics.Win.UltraWinToolbars;
using Infragistics.Win.UltraWinStatusBar;
using Krista.FM.Client.Common;
using Infragistics.Win.UltraWinDock;
using Krista.FM.Client.Common.Gui;

namespace Krista.FM.Client.Workplace.Gui
{
    /// <summary>
    /// Интерфейс оболочки Workplace, доступный в объектах просмотра
    /// </summary>
    public interface IWorkbench : IWorkplace
    {
        DockableControlPane GetWorkplaceWindow(string key);

        //IWorkplaceLayout WorkplaceLayout { get; set; }

        /// <summary>
        /// Коллекция в которой содержатся все открытые окна.
        /// </summary>
        List<IViewContent> ViewContentCollection
        {
            get;
        }

        object ActiveContent { get; }

        IWorkplaceLayout WorkplaceLayout { get; }

        /// <summary>
        /// Закрывает открытый.
        /// </summary>
        void CloseContent(IViewContent content);

        /// <summary>
        /// Вызывается после открытия окна просмотра в воркплейсе.
        /// </summary>
        event ViewContentEventHandler ViewOpened;

        /// <summary>
        /// Вызывается после закрытия окна просмотра в воркплейсе.
        /// </summary>
        event ViewContentEventHandler ViewClosed;

        /// <summary>
        /// Вызывается после смены активного окна просмотра в воркплейсе.
        /// </summary>
        event EventHandler ActiveWorkplaceWindowChanged;

        /// <summary>
        /// Вызывается после смены закладки на панели навигации
        /// </summary>
        event ActiveGroupChangedEventHandler ActiveGroupChanged;
    }
}
