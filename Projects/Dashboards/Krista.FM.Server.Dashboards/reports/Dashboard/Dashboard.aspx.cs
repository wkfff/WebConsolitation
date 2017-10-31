using System;

using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.Dashboard
{
	public partial class Dashboard : CustomReportPage
	{
		/// <summary>
		/// Глобальный обработчик загрузки страницы
		/// </summary>
		protected override void Page_Load(object sender, EventArgs e)
		{
			base.Page_Load(sender, e);

			if (DataProvidersFactory.SecondaryMASDataProvider == null)
			{
				Response.Redirect("../../UserError.aspx");
			}
		}
	}
}
