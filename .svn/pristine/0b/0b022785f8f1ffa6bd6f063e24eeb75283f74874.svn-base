using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Krista.FM.Client.Common.Gui
{
    /// <summary>
    /// Базовый функционал, который должны обеспечивать все объекты просмотра.
    /// </summary>
    public interface IViewContent : IDisposable
    {
        string Key { get; }

        Control Control { get; }

        /// <summary>
        /// Окно воркплейса в котором отображается IViewContent.
        /// </summary>
        IWorkplaceWindow WorkplaceWindow { get; set; }

        Icon Icon { get; }

        event EventHandler TitleChanged;
    }
}
