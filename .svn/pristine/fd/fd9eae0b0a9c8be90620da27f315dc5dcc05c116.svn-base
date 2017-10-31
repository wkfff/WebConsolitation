using Krista.FM.Client.Common.Gui;
using Krista.FM.Client.ViewObjects.BaseViewObject;
using Krista.FM.Client.Workplace.Gui;

namespace Krista.FM.Client.Workplace.Commands
{
    public class ExitWorkplaceCommand : AbstractCommand
    {
		public ExitWorkplaceCommand()
		{
			key = "ExitWorkplaceCommand";
			caption = "Выход";
		}

        public override void Run()
        {
			WorkplaceSingleton.Workplace.Close();
        }
    }
}
