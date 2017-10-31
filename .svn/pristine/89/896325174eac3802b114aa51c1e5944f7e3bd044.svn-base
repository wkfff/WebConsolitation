using System;
using System.Web;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports
{
	public partial class DashboardFederal : CustomReportPage
	{
	    private const string cookieStateAreaParamName = "StateArea";

        /// <summary>
		/// Глобальный обработчик загрузки страницы
		/// </summary>
		protected override void Page_Load(object sender, EventArgs e)
		{
			base.Page_Load(sender, e);

			if (DataProvidersFactory.SecondaryMASDataProvider == null)
			{
				Server.Transfer(CustomReportConst.userErrorPageUrl);
			}

            if (!Page.IsPostBack)
            {
                regionsCombo.Width = 410;
                regionsCombo.Title = "Субъект РФ";
                regionsCombo.FillDictionaryValues(CustomMultiComboDataHelper.FillRegions(RegionsNamingHelper.FoNames, RegionsNamingHelper.RegionsFoDictionary));
                regionsCombo.ParentSelect = false;
                if (!string.IsNullOrEmpty(UserParams.StateArea.Value))
                {
                    regionsCombo.SetСheckedState(UserParams.StateArea.Value, true);
                }
                else if (RegionSettings.Instance != null && RegionSettings.Instance.Name != String.Empty)
                {
                    regionsCombo.SetСheckedState(RegionSettings.Instance.Name, true);
                }
            }
            
            CustomParams.MakeRegionParams(regionsCombo.SelectedValue, "name");
            
            Page.Title = string.Format("Сравнение субъектов РФ: {0}", UserParams.StateArea.Value);
            imgHerald.ImageUrl = String.Format("../../images/Heralds/{0}.png", HttpContext.Current.Session["CurrentSubjectID"]);
            CrossLink1.NavigateUrl = (string)HttpContext.Current.Session["CurrentSiteRef"];
            CrossLink1.Text = (string)HttpContext.Current.Session["CurrentSiteName"];
            imgHerald.Height = 65;
            TitleLabel.Text = string.Format("{0}", UserParams.StateArea.Value);
            SubTitleLabel.Text = "Сводная информация об исполнении консолидированного бюджета субъекта РФ";

            SetCookies(cookieStateAreaParamName, UserParams.StateArea.Value);

            ((IMasterPage)this.Master).SetTwitterButtonText(String.Format("<a href='http://twitter.com/share' class='twitter-share-button' data-url='http://iminfin.ru/reports/DashboardFederal/Dashboard.aspx?paramlist=subjectid={0}' data-text='iМониторинг' data-count='none'>Tweet</a><script type='text/javascript' src='http://platform.twitter.com/widgets.js'></script>", HttpContext.Current.Session["CurrentSubjectID"]));
		}
	}
}
