using System.Collections.Generic;

using Krista.FM.Client.Design;

namespace Krista.FM.Client.DiagramEditor.Commands
{
    /// <summary>
    /// коллекция команд
    /// </summary>
    public class MacroCommand : Command
    {
        private List<Command> commandList = new List<Command>();

        public override void Execute()
        {
            foreach (Command command in commandList)
            {
                command.Execute();
            }
        }
    }
}
