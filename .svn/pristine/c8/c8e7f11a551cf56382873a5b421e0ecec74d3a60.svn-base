using Krista.FM.Client.Design;

namespace Krista.FM.Client.DiagramEditor.Commands
{
    /// <summary>
    /// Базовый класс для всех команд редактора диаграмм.
    /// </summary>
    public abstract class DiagramEditorCommand : Command
    {
        private AbstractDiagram diagram;
        
        public DiagramEditorCommand(AbstractDiagram diagram)
        {
            this.diagram = diagram;
        }

        protected AbstractDiagram Diagram
        {
            get { return diagram; }
            set { diagram = value; }
        }
    }
}
