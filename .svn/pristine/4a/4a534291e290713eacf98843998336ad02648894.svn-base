using System;
using System.Collections.Generic;
using System.Text;

namespace Krista.FM.Client.DiagramEditor.Commands
{
    /// <summary>
    /// Общий интерфейс для всех команд
    /// </summary>
    public interface ICommand
    {
        void Execute();
    }

    /// <summary>
    /// Интерфейс для команд, реализующих команду Undo 
    /// </summary>
    public interface IUndoCommand
    {
        void Undo();
    }

    /// <summary>
    /// Интерфеис для операций Undo/Redo
    /// </summary>
    public interface IUndoRedo
    {
        /// <summary>
        /// Отмена
        /// </summary>
        void Undo();

        /// <summary>
        /// Отмена отмены
        /// </summary>
        void Redo();

        /// <summary>
        /// Сброс
        /// </summary>
        void Flush();
    }
}
