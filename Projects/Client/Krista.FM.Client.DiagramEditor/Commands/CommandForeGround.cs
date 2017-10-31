using Krista.FM.Client.Design;

namespace Krista.FM.Client.DiagramEditor.Commands
{
    public class CommandForeGround : CommandPosition, IUndoCommand
    {
        public CommandForeGround(DiagramEntity entity)
            : base(entity)
        {
        }

        protected override void Transfer()
        {
            Entity.Diagram.Entities.Add(Entity);
        }
    }
}
