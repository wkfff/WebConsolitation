using Krista.FM.Common.Services;

using Krista.FM.ServerLibrary.TemplatesService;

namespace Krista.FM.Client.ViewObjects.TemplatesUI.Gui
{
	internal class WebTemplatestUI : WebTemplatestUIBase
	{
		public WebTemplatestUI()
			: base(typeof(WebTemplatestUI).FullName, TemplateTypes.Web)
		{
			Caption = "Веб-отчеты";
		}

		public override System.Drawing.Icon Icon
		{
			get { return ResourceService.GetIcon("ReportWebStatistic"); }
		}

		/// <summary>
		/// Возвращает тип документа используемый по умолчанию.
		/// </summary>
		internal override TemplateDocumentTypes DefaultDocumentTypes
		{
			get { return TemplateDocumentTypes.WebReport; }
		}
	}
}
