using System.Collections.Generic;
using Krista.FM.Client.Design;

namespace Krista.FM.Client.DiagramEditor.Commands
{
    public class CommandStereotype : Command, IUndoCommand
    {
        private bool vis = false;

        private List<DiagramEntity> entities = new List<DiagramEntity>();

        public CommandStereotype(List<DiagramEntity> selected, bool vis)
        {
            this.vis = vis;

            foreach (DiagramEntity entity in selected)
            {
                entities.Add(entity);
            }
        }

        public override void Execute()
        {
            foreach (DiagramEntity entity in entities)
            {
                entity.DisplayStereotype(vis);
            }
        }

        #region IUndoCommand Members

        public void Undo()
        {
            foreach (DiagramEntity entity in entities)
            {
                entity.DisplayStereotype(!vis);
            }
        }

        #endregion
    }
}
