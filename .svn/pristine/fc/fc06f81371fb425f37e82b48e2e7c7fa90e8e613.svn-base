using System.Collections.Generic;
using Krista.FM.Client.Design;

namespace Krista.FM.Client.DiagramEditor.Commands
{
    public class CommandAttributes : Command, IUndoCommand
    {
        private bool vis;

        private List<DiagramEntity> entities = new List<DiagramEntity>();

        public CommandAttributes(List<DiagramEntity> selected, bool vis)
        {
            this.vis = vis;

            foreach (DiagramEntity entity in selected)
            {
                entities.Add(entity);
            }
        }

        public override void Execute()
        {
            Change(vis);
        }

        #region IUndoCommand Members

        public void Undo()
        {
            Change(!vis);
        }

        private void Change(bool vis)
        {
            foreach (DiagramEntity entity in entities)
            {
                if (entity is UMLEntityBase)
                {
                    ((UMLEntityBase)entity).IsSuppressAttribute = !vis;
                    ((UMLEntityBase)entity).AutoSizeRec();
                }
            }
        }

        #endregion
    }
}
