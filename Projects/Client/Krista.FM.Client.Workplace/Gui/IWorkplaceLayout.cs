using System;
using System.Collections.Generic;
using System.Text;
using Krista.FM.Client.Common;
using Krista.FM.Client.Common.Gui;

namespace Krista.FM.Client.Workplace.Gui
{
    public interface IWorkplaceLayout
    {
        object ActiveContent { get; }

        void Attach(IWorkplace workplace);
        void Detach();

        void ShowPad(PaneDescriptor content);

        void ActivatePad(PaneDescriptor content);
        void ActivatePad(string fullyQualifiedTypeName);

        void HidePad(PaneDescriptor content);

        void UnloadPad(PaneDescriptor content);

        bool IsVisible(PaneDescriptor padContent);

        /// <summary>
        /// Показывает новый объект просмотра <see cref="IViewContent"/>.
        /// </summary>
        IWorkplaceWindow ShowView(IViewContent content);

        event EventHandler ActiveWorkplaceWindowChanged;
    }
}
