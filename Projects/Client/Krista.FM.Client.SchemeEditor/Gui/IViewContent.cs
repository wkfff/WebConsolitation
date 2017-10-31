using System;
using System.Collections.Generic;
using System.Text;

namespace Krista.FM.Client.SchemeEditor.Gui
{
    /// <summary>
    /// Ѕазовый интерфейс дл€ всех представлений поддерживающих редактирование.
    /// </summary>
    public interface IViewContent : IBaseViewContent, ICanBeDirty
    {
/*
        /// <summary>
        /// ≈сли свойство возвращает true, то содержимое не может быть изменено.
        /// </summary>
        bool IsReadOnly { get; }

        /// <summary>
        /// ≈сли свойство возвращает true, то содержимое не может быть сохранено.
        /// </summary>
        bool IsViewOnly { get; }
*/
    }
}
