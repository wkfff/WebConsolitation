using Krista.FM.Client.Design;

namespace Krista.FM.Client.DiagramEditor.Commands
{
    public class CommandSQLExpression : CommandWithPrm
    {
        private DiagramEntity entity;

        public CommandSQLExpression(DiagramEntity entity)
        {
            this.entity = entity;
        }

        public override void Execute(object obj)
        {
            entity.DisplaySQLExpression((bool)obj);
        }
    }
}
