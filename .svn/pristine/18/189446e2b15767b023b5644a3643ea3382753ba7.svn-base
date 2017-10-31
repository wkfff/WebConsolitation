using System;
using System.Data;
using System.Web;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Xml;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Infragistics.WebUI.UltraWebGrid;

namespace Krista.FM.Server.Dashboards.reports.iPad
{
    public partial class Food_0003_0003 : CustomReportPage
    {
		private DateTime lastDate;
		private DateTime currDate;

		private bool IsStatMO(string moName)
		{
			if (moName.Contains("Белоярский") ||
                (moName.Contains("Сургут") && !moName.Contains("Сургутский")) ||
				(moName.Contains("Ханты-Мансийск") && !moName.Contains("Ханты-Мансийский")) ||
				(moName.Contains("Нижневартовск") && !moName.Contains("Нижневартовский")))
				return true;
			else
				return false;
		}

		protected override void Page_Load(object sender, EventArgs e)
		{
			if (IsPostBack)
			{
				return;
			}

			if (!UserParams.Mo.Value.Contains("г."))
			{
				UserParams.Mo.Value = String.Format("{0} муниципальный район", UserParams.Mo.Value);
			}
			else
			{
				UserParams.Mo.Value = String.Format("{0}", UserParams.Mo.Value.Replace("г.", "Город "));
			}
			
			InitializeDate();

			if (IsStatMO(UserParams.Mo.Value))
			{
				lbInfo.Text = "Данные территориального отделения ФСГС по ХМАО-Югре";
				UltraWebGrid1.Visible = false;
			}
			else
			{
				lbInfo.Text = "Данные департамента экономики ХМАО-Югры";
                UltraWebGrid1.Visible = true;
                UltraWebGrid1.DataBinding += new EventHandler(UltraWebGrid1_DataBinding);
                UltraWebGrid1.InitializeLayout += new Infragistics.WebUI.UltraWebGrid.InitializeLayoutEventHandler(UltraWebGrid1_InitializeLayout);
                UltraWebGrid1.InitializeRow += new Infragistics.WebUI.UltraWebGrid.InitializeRowEventHandler(UltraWebGrid1_InitializeRow);
                UltraWebGrid1.Width = 730;
                UltraWebGrid1.Height = Unit.Empty;
                UltraWebGrid1.DataBind();
            }
		}

		#region Инициализация

		private bool InitializeDate()
		{
			string query = DataProvider.GetQueryText("Food_0003_0003_incomes_date");
			DataTable dtDate = new DataTable();
			DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Дата", dtDate);
			UserParams.PeriodCurrentDate.Value = dtDate.Rows[1]["ДанныеНа"].ToString();
			UserParams.PeriodLastDate.Value = dtDate.Rows[0]["ДанныеНа"].ToString();
			currDate = CRHelper.DateByPeriodMemberUName(UserParams.PeriodCurrentDate.Value, 3);
			lastDate = CRHelper.DateByPeriodMemberUName(UserParams.PeriodLastDate.Value, 3);
			return true;
		}

		#endregion

		#region Таблица


        private void UltraWebGrid1_InitializeRow(object sender, Infragistics.WebUI.UltraWebGrid.RowEventArgs e)
        {
        }

        private void UltraWebGrid1_InitializeLayout(object sender, Infragistics.WebUI.UltraWebGrid.LayoutEventArgs e)
        {
            UltraWebGrid grid = sender as UltraWebGrid;

            UltraGridBand band = grid.Bands[0];

            band.Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;

            band.Columns[0].Width = 215;
            band.Columns[1].Width = 110;
            band.Columns[2].Width = 110;
            band.Columns[3].Width = 110;
            band.Columns[4].Width = 110;

            CRHelper.FormatNumberColumn(grid.Columns[1], "N2");
            CRHelper.FormatNumberColumn(grid.Columns[2], "N2");
            CRHelper.FormatNumberColumn(grid.Columns[3], "N2");
            CRHelper.FormatNumberColumn(grid.Columns[4], "P2");

            GridHeaderLayout headerLayout = new GridHeaderLayout(grid);
            headerLayout.AddCell("Предприятие");
            headerLayout.AddCell(String.Format("{0:dd.MM.yyyy}", lastDate));
            headerLayout.AddCell(String.Format("{0:dd.MM.yyyy}", currDate));
            headerLayout.AddCell("Абс. отклон.");
            headerLayout.AddCell("Темп прироста");
            headerLayout.ApplyHeaderInfo();
        }

        private void UltraWebGrid1_DataBinding(object sender, EventArgs e)
        {
            DataTable dtGrid = new DataTable();
            string query = DataProvider.GetQueryText("Food_0003_0003_grid");
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Предприятие", dtGrid);
            UltraWebGrid1.DataSource = dtGrid;
        }

		#endregion

	}
}
