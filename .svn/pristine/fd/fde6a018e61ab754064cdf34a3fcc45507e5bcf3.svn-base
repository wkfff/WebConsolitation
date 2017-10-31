using Krista.FM.Client.Design;

namespace Krista.FM.Client.DiagramEditor.Commands
{
    /// <summary>
    /// Команда смены тени
    /// </summary>
    public class CommandShadowVisibleChange : CommandWithPrm
    {
        private DiagramEntity entity;

        public CommandShadowVisibleChange(DiagramEntity entity)
        {
            this.entity = entity;
        }

        public override void Execute(object obj)
        {
            ((DiagramRectangleEntity)entity).IsShadow = (bool)obj;
        }
    }
}
