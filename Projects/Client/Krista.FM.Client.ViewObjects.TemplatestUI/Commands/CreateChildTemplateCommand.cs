using Infragistics.Win.UltraWinGrid;

using Krista.FM.Client.Workplace.Gui;

using Krista.FM.ServerLibrary.TemplatesService;

namespace Krista.FM.Client.ViewObjects.TemplatesUI.Commands
{
	/// <summary>
	/// Добавить подчиненный отчет.
	/// </summary>
	internal class CreateChildTemplateCommand : TemplatesCommand
	{
		internal CreateChildTemplateCommand()
		{
			key = GetType().Name;
			caption = "Добавить подчиненный отчет";
			iconKey = "AddFile";
			IsRowCommand = false;
		}

		public override void Run()
		{
			TemplatestUIBase content = (TemplatestUIBase)WorkplaceSingleton.Workplace.ActiveContent;

			if (content.Grid.ActiveRow != null)
			{
				UltraGridRow row = content.Grid.ActiveRow.ChildBands[0].Band.AddNew();
				row.Cells[TemplateFields.Type].Value = content.DefaultDocumentTypes;
				row.Cells[TemplateFields.Name].Value = "Новый шаблон";
				
				row.Update();
			}
		}
	}
}
