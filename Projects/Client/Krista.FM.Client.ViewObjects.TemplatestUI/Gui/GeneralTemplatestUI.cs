using Krista.FM.Common.Services;

using Krista.FM.ServerLibrary.TemplatesService;

namespace Krista.FM.Client.ViewObjects.TemplatesUI.Gui
{
	internal class GeneralTemplatestUI : TemplatestUIBase
	{
		public GeneralTemplatestUI()
			: base(typeof(GeneralTemplatestUI).FullName, TemplateTypes.General)
		{
			Caption = "Репозиторий отчетов";
		}

		public override System.Drawing.Icon Icon
		{
			get { return ResourceService.GetIcon("ReportMDXExpert"); }
		}

		public override void Initialize()
		{
			base.Initialize();

			CommandList.Add(typeof(Commands.OpenTemplateCommand).Name, new Commands.OpenTemplateCommand());
			CommandList.Add(typeof(Commands.EditTemplateCommand).Name, new Commands.EditTemplateCommand());
			CommandList.Add(typeof(Commands.AddDocumentTemplateCommand).Name, new Commands.AddDocumentTemplateCommand());
			CommandList.Add(typeof(Commands.SaveTemplateCommand).Name, new Commands.SaveTemplateCommand());

			// добавляем новые кнопочки на основной тулбар
			ViewObject.InitializeGridToolBarCommands(CommandList);

			CommandList[typeof(Commands.OpenTemplateCommand).Name].IsEnabled = false;
			CommandList[typeof(Commands.EditTemplateCommand).Name].IsEnabled = false;
			CommandList[typeof(Commands.AddDocumentTemplateCommand).Name].IsEnabled = false;
			CommandList[typeof(Commands.SaveTemplateCommand).Name].IsEnabled = false;
		}

		/// <summary>
		/// Возвращает тип документа используемый по умолчанию.
		/// </summary>
		internal override TemplateDocumentTypes DefaultDocumentTypes
		{
			get { return TemplateDocumentTypes.MDXExpert3; }
		}
	}
}
