namespace Krista.FM.Client.DiagramEditor.Commands
{
    /// <summary>
    /// Интерфеис для операций Undo/Redo
    /// </summary>
    public interface IUndoRedo
    {
        /// <summary>
        /// Отмена изменения
        /// </summary>
        void Undo();

        /// <summary>
        /// Отмена отмены
        /// </summary>
        void Redo();

        /// <summary>
        /// Сброс кэша отката
        /// </summary>
        void Flush();
    }
}
