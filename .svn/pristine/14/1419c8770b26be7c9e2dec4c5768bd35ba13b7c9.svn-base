using Infragistics.Win.UltraWinGrid;

using Krista.FM.Client.Workplace.Gui;

using Krista.FM.ServerLibrary.TemplatesService;

namespace Krista.FM.Client.ViewObjects.TemplatesUI.Commands
{
	/// <summary>
	/// Добавить отчет верхнего уровня.
	/// </summary>
	internal class CreateRootTemplateCommand : TemplatesCommand
	{
		internal CreateRootTemplateCommand()
		{
			key = GetType().Name;
			caption = "Добавить отчет верхнего уровня";
			iconKey = "AddFolder";
			IsRowCommand = false;
		}

		public override void Run()
		{
			TemplatestUIBase content = (TemplatestUIBase)WorkplaceSingleton.Workplace.ActiveContent;

			UltraGridRow row = content.Grid.DisplayLayout.Bands[0].AddNew();
			row.Cells[TemplateFields.Type].Value = (int)TemplateDocumentTypes.Group;
			row.Cells[TemplateFields.Name].Value = "Новая группа";

			row.Update();

		}
	}
}
