using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Krista.FM.Client.SchemeEditor.Gui
{
    /// <summary>
    /// Базовая функциональность, которую должны реализовывать 
    /// все представления контента.
    /// </summary>
    public interface IBaseViewContent : IDisposable
    {
        /// <summary>
        /// Компонент Windows.Forms отображаемый данным представлением.
        /// </summary>
        Control Control { get; }

        /// <summary>
        /// Окно дизайнера, в котором отображается представление.
        /// </summary>
        IWorkbenchWindow WorkbenchWindow { get; set; }

        /// <summary>
        /// Переинициализация и отрисовка содержимого.
        /// </summary>
        void RedrawContent();
    }
}
