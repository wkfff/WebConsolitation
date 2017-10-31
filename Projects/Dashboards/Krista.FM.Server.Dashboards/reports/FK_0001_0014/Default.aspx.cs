using System;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebChart;
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.WebUI.UltraWebGrid.DocumentExport;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using EndExportEventArgs=Infragistics.WebUI.UltraWebGrid.DocumentExport.EndExportEventArgs;
using Font=Infragistics.Documents.Reports.Graphics.Font;

namespace Krista.FM.Server.Dashboards.reports.FK_0001_0014
{ 
    public partial class Default : CustomReportPage
    {
        private const int firstYear = 2008;

        private DataTable dtChart1;
        private DataTable dtChart2;

        private CustomParam bkkuDate;

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            if (!Page.IsPostBack)
            {
                string query = DataProvider.GetQueryText("FK_0001_0014_date");

                DataTable dtDate = new DataTable();
                DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
                int endYear = Convert.ToInt32(dtDate.Rows[0][0]);

                UserParams.PeriodYear.Value = endYear.ToString();
                UserParams.PeriodMonth.Value = dtDate.Rows[0][3].ToString();
                
                ComboYear.Title = "Год";
                ComboYear.Width = 100;
                ComboYear.MultiSelect = false;
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, endYear));
                ComboYear.SetСheckedState(UserParams.PeriodYear.Value, true);

                ComboMonth.Title = "Месяц";
                ComboMonth.Width = 130;
                ComboMonth.MultiSelect = false;
                ComboMonth.FillDictionaryValues(CustomMultiComboDataHelper.FillMonthValues());
                ComboMonth.SetСheckedState(UserParams.PeriodMonth.Value, true);

                regionsCombo.Width = 300;
                regionsCombo.Title = "Субъект РФ";
                regionsCombo.FillDictionaryValues(
                        CustomMultiComboDataHelper.FillRegions(RegionsNamingHelper.FoNames,
                        RegionsNamingHelper.RegionsFoDictionary));

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

            UserParams.Region.Value = regionsCombo.SelectedNodeParent;
            UserParams.StateArea.Value = regionsCombo.SelectedValue;

            Page.Title = "Основные показатели консолидированных бюджетов субъектов РФ";
            Label1.Text = String.Format("{0}: ", regionsCombo.SelectedValue);
            Label2.Text = String.Format("Основные показатели консолидированных бюджетов субъектов РФ за {0} {1} года.", ComboMonth.SelectedValue.ToLower(), ComboYear.SelectedValue);
                       
            int year = Convert.ToInt32(ComboYear.SelectedValue);

            DateTime date = new DateTime(year, ComboMonth.SelectedIndex + 1, 1);
            UserParams.PeriodCurrentDate.Value = CRHelper.PeriodMemberUName(String.Empty, date, 4);
            UserParams.Subject.Value = string.Format("[{0}].[{1}]", UserParams.Region.Value, UserParams.StateArea.Value);
            
            bkkuDate = UserParams.CustomParam("bkkuDate");

            if (date.Year == 2010)
            {
                date.AddYears(-1);
            }

            bkkuDate.Value = CRHelper.PeriodMemberUName(String.Empty, date, 4);
        }
    }
}
