using Krista.FM.Client.Design;

namespace Krista.FM.Client.DiagramEditor.Commands
{
    public class CommandBackGround : CommandPosition, IUndoCommand
    {
        public CommandBackGround(DiagramEntity entity)
            : base(entity)
        {
        }

        protected override void Transfer()
        {
            Entity.Diagram.Entities.Insert(0, Entity);
        }
    }
}
