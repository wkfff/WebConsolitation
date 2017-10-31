using Krista.FM.Client.Design;

namespace Krista.FM.Client.DiagramEditor.Commands
{
    public abstract class CommandPosition : Command, IUndoCommand
    {
        private DiagramEntity entity;
        private int position;

        public CommandPosition(DiagramEntity entity)
        {
            this.entity = entity;
            position = ProcessPosition();
        }

        protected DiagramEntity Entity
        {
            get { return entity; }
            set { entity = value; }
        }

        protected int Position
        {
            get { return position; }
            set { position = value; }
        }

        public override void Execute()
        {
            entity.Diagram.Entities.Remove(entity);

            Transfer();

            entity.Diagram.IsChanged = true;
            entity.Diagram.Site.Invalidate();
        }

        #region IUndoCommand Members

        public void Undo()
        {
            entity.Diagram.Entities.Remove(entity);
            entity.Diagram.Entities.Insert(position, entity);

            entity.Diagram.IsChanged = true;
            entity.Diagram.Site.Invalidate();
        }

        #endregion

        protected abstract void Transfer();

        private int ProcessPosition()
        {
            return entity.Diagram.Entities.IndexOf(entity);
        }
    }
}
