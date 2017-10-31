using System;
using System.Collections.Generic;
using System.Text;

namespace Krista.FM.Client.SchemeEditor.Gui
{
    /// <summary>
    /// Интерфейс для классов, которые реализуют свойства IsDirty и событие DirtyChanged.
    /// </summary>
    public interface ICanBeDirty
    {
        /// <summary>
        /// Если свойство возвращает true, то содекжимое было изменено
        /// посде операции закгузки/сохранения.
        /// </summary>
        bool IsDirty { get; set; }

        /// <summary>
        /// Срабатывает после изменения сожержимого и говорит, 
        /// что изменения должны быть сохранены.
        /// </summary>
        event EventHandler DirtyChanged;
    }
}
