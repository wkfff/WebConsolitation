using System.Collections.Generic;
using System.Drawing;
using Krista.FM.Client.Design;

namespace Krista.FM.Client.DiagramEditor.Commands
{
    /// <summary>
    /// Команда перемещения объекта
    /// </summary>
    public class CommandChangeLocation : Command, IUndoCommand
    {
        /// <summary>
        /// Пара целых чисел, определяющих смещение 
        /// </summary>
        private Size offset;

        private List<DiagramEntity> entities = new List<DiagramEntity>();

        public CommandChangeLocation(List<DiagramEntity> entitiesCollection, Size offset)
        {
            foreach (DiagramEntity entity in entitiesCollection)
            {
                entities.Add(entity);
            }

            this.offset = offset;
        }

        #region Command Members

        public override void Execute()
        {
            Change(offset);
        }

        public void Undo()
        {
            Change(new Size(-offset.Width, -offset.Height));
        }

        private void Change(Size move)
        {
            foreach (DiagramEntity entity in entities)
            {
                entity.Move(new Point(move.Width, move.Height));
            }
        }

        #endregion
    }
}
