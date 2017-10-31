using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using Krista.FM.Client.Design;

namespace Krista.FM.Client.DiagramEditor.Commands
{
    /// <summary>
    /// Операция удаления/восстановления объекта
    /// </summary>
    public class CommandRemove : Command, IUndoCommand
    {
        private List<DiagramEntity> entities = new List<DiagramEntity>();

        public CommandRemove(List<DiagramEntity> entitiesCollection)
        {
            foreach (DiagramEntity entity in entitiesCollection)
            {
                entities.Add(entity);
            }
        }

        #region IUndoCommand Members

        /// <summary>
        /// Операция, обратная удалению - тобишь восстановление объекта
        /// </summary>
        public void Undo()
        {
            foreach (DiagramEntity entity in entities)
            {
                entity.RestoreEntity();
            }
        }

        #endregion

        public override void Execute()
        {
            foreach (DiagramEntity entity in entities)
            {
                entity.RemoveEntity();
            }
        }
    }
}
