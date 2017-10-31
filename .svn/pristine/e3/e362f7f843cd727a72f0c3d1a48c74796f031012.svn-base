using System;
using System.Data;
using System.Configuration;
using Infragistics.WebUI.UltraWebGrid;
using System.Drawing;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.Dashboard
{
	public partial class FK0105Gadget : GadgetControlBase
	{
		protected override void Page_Load(object sender, EventArgs e)
		{
			base.Page_Load(sender, e);

			Grid.DataBind();
		}

		protected void Grid_DataBinding(object sender, EventArgs e)
		{
			DataTable dataTable = new DataTable();

			CustomReportPage dashboard = CustumReportPage;

			try
			{
				string query = DataProvider.GetQueryText("FK_0001_0005_Row3_6");
				DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Показатели", dataTable);

				dataTable.Rows[0][0] = "Темп роста налога на прибыль организаций";
				dataTable.Rows[1][0] = "Темп роста налога на доходы физических лиц";
				dataTable.Rows[2][0] = "Темп роста налога на имущество организаций";
				dataTable.Rows[3][0] = "Темп роста по акцизам";

				DataTable dataTableRow1 = new DataTable();
				query = DataProvider.GetQueryText("FK_0001_0005_Row1");
				DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Наименование показателя", dataTableRow1);
				DataRow row = dataTable.NewRow();
				row[0] = "Исполнение бюджета по доходам";
				row[1] = dataTableRow1.Rows[0][0];
				row[2] = dataTableRow1.Rows[0][1];
				row[3] = dataTableRow1.Rows[0][2];
				dataTable.Rows.InsertAt(row, 0);

				DataTable dataTableRow2 = new DataTable();
				query = DataProvider.GetQueryText("FK_0001_0005_Row2");
				DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Наименование показателя", dataTableRow2);
				row = dataTable.NewRow();
				row[0] = "Исполнение бюджета по расходам";
				row[1] = dataTableRow2.Rows[0][0];
				row[2] = dataTableRow2.Rows[0][1];
				row[3] = dataTableRow2.Rows[0][2];
				dataTable.Rows.InsertAt(row, 1);

				DataTable dataTableRow7 = new DataTable();
				query = DataProvider.GetQueryText("FK_0001_0005_Row7");
				DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Показатели", dataTableRow7);
				row = dataTable.NewRow();
				row[0] = "Коэффициент автономии";
				row[1] = dataTableRow7.Rows[0][1];
				row[2] = dataTableRow7.Rows[0][2];
				row[3] = dataTableRow7.Rows[0][3];
				dataTable.Rows.InsertAt(row, 6);

				Grid.DataSource = dataTable.DefaultView;
			}
			catch (Exception ex)
			{
				Trace.Warn(this.ToString(), "Ошибка", ex);
			}
		}

		protected void Grid_InitializeLayout(object sender, LayoutEventArgs e)
		{
			if (Grid.DisplayLayout.Bands.Count == 0)
				return;

			if (Grid.DisplayLayout.Bands[0].Columns.Count > 3)
			{
				CRHelper.FormatNumberColumn(Grid.DisplayLayout.Bands[0].Columns[1], "P0");
				CRHelper.FormatNumberColumn(Grid.DisplayLayout.Bands[0].Columns[2], "P0");
				CRHelper.FormatNumberColumn(Grid.DisplayLayout.Bands[0].Columns[3], "P0");

				Grid.DisplayLayout.Bands[0].Columns[0].CellStyle.BackColor = Color.FromArgb(239, 243, 251);

				Grid.DisplayLayout.Bands[0].Columns[2].Header.Caption = "Сибирский ФО";
				Grid.DisplayLayout.Bands[0].Columns[3].Header.Caption = "РФ";

				Grid.DisplayLayout.Bands[0].Columns[0].Width = 135;
				Grid.DisplayLayout.Bands[0].Columns[1].Width = 107;
				Grid.DisplayLayout.Bands[0].Columns[2].Width = 80;
				Grid.DisplayLayout.Bands[0].Columns[3].Width = 80;
			}
		}

		#region IWebPart Members

		public override string Description
		{
			get { return "Информация Федерального казначейства об исполнении бюджетов всех субъектов РФ на 1 июля 2008 г."; }
		}

		public override string Title
		{
			get { return "Исполнение бюджетов субъектов"; }
		}

		public override string TitleUrl
		{
			get { return String.Empty; }
		}

		#endregion
	}
}