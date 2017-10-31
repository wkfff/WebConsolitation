using System;
using System.Data;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.WebUI.UltraWebGrid;

using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.MFRF_0001_0004_FFSSR
{
	public partial class Default : CustomReportPage
	{
        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

			int currentWidth = (int)Session["width_size"] - 50;
			UltraWebGridFFSSR.Width = (int)(currentWidth * 0.5);
			UltraChartFFSSR1.Width = (int)(currentWidth * 0.5);
			UltraChartFFSSR2.Width = (int)(currentWidth * 0.5);

            int currentHeight = (int)Session["height_size"] - 200;
			UltraWebGridFFSSR.Height = (int)(currentHeight - 175);
			UltraChartFFSSR1.Height = (int)(currentHeight * 0.5);
			UltraChartFFSSR2.Height = (int)(currentHeight * 0.5 - 10);
		}

		protected override void Page_Load(object sender, EventArgs e)
		{
			base.Page_Load(sender, e);
			curStateAreaNameFFSSR = string.Empty;
			if (!Page.IsPostBack)
			{
				ComboYear.SelectedIndex = 10;
			}
			string pValue = string.Format("[Период].[Год].[Данные всех периодов].[{0}]", ComboYear.SelectedRow.Cells[0].Value);

            lbFO.Text = string.Empty;
            lbSybject.Text = string.Empty;

			//Обновляем таблицу только, если либо параметр периода был изменен, либо загружаемся в первый раз
			if (!Page.IsPostBack || !UserParams.PeriodYear.ValueIs(pValue))
			{
				UserParams.PeriodYear.Value = string.Format("[Период].[Год].[Данные всех периодов].[{0}]", ComboYear.SelectedRow.Cells[0].Value);
				UltraWebGridFFSSR.DataBind();
			}
			ShowHideCharts(false);
		}

		#region ФФССР
		DataTable dtMasterFFSSR = new DataTable();
		DataTable dtDetailFFSSR = new DataTable();
		DataTable dtChartFFSSR1 = new DataTable();
		DataTable dtChartFFSSR2 = new DataTable();
		DataSet tableDataSetFFSSR = new DataSet();
		//имя текущего субъекта выбранного во втором бенде мастер-таблицы
		private string curStateAreaNameFFSSR = string.Empty;

		private void ShowHideCharts(bool show)
		{
			UltraChartFFSSR1.Visible = show;
			UltraChartFFSSR2.Visible = show;
		}

		protected void UltraWebGridFFSSR_DataBinding(object sender, EventArgs e)
		{
			string query = DataProvider.GetQueryText("FFSSRMasterTable");
			DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Федеральный округ", dtMasterFFSSR);

			query = DataProvider.GetQueryText("FFSSRDetailTable");
			DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Субъект РФ", dtDetailFFSSR);

			tableDataSetFFSSR.Tables.Add(dtMasterFFSSR);
			tableDataSetFFSSR.Tables.Add(dtDetailFFSSR);

			tableDataSetFFSSR.Relations.Add(dtMasterFFSSR.Columns[0], dtDetailFFSSR.Columns[1]);

			UltraWebGridFFSSR.DataSource = tableDataSetFFSSR.Tables[0].DefaultView;
		}

		protected void UltraChartFFSSR1_DataBinding(object sender, EventArgs e)
		{
			string query = DataProvider.GetQueryText("FFSSRChart1");
			DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "seriesName", dtChartFFSSR1);
			UltraChartFFSSR1.DataSource = dtChartFFSSR1;
		}

		protected void UltraChartFFSSR2_DataBinding(object sender, EventArgs e)
		{
			string query = DataProvider.GetQueryText("FFSSRChart2");
			DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "seriesName", dtChartFFSSR2);
			UltraChartFFSSR2.DataSource = dtChartFFSSR2;
		}

		protected void UltraWebGridFFSSR_ActiveRowChange(object sender, RowEventArgs e)
		{
            // по бенду регионов пока делать ничего не будем.
            if (e.Row.Band.Index == 0)
                return;

			string stateArea = e.Row.Cells[1].Text;
			string region = e.Row.Cells[0].Text;

            lbFO.Text = stateArea;
            lbSybject.Text = region;

			string regionTemplate = string.Format("[Территории].[Сопоставимый].[Все территории].[Российская  Федерация].[{0}].[{1}]", stateArea, region);
			string stateAreaTemplate = string.Format("[Территории].[Сопоставимый].[Все территории].[Российская  Федерация].[{0}]", stateArea);
			UserParams.Region.Value = regionTemplate;
			UserParams.StateArea.Value = stateAreaTemplate;
			curStateAreaNameFFSSR = e.Row.Cells[0].Text;
			UltraChartFFSSR1.DataBind();
			UltraChartFFSSR2.DataBind();
			ShowHideCharts(true);
		}

		protected void UltraWebGridFFSSR_InitializeLayout(object sender, LayoutEventArgs e)
		{
			for (int i = 1; i < UltraWebGridFFSSR.Bands[0].Columns.Count; i = i + 2)
			{
				CRHelper.FormatNumberColumn(UltraWebGridFFSSR.Bands[0].Columns[i], "N2");
				UltraWebGridFFSSR.Bands[0].Columns[i].Width = 110;
			}
			for (int i = 2; i < UltraWebGridFFSSR.Bands[1].Columns.Count; i = i + 2)
			{
				CRHelper.FormatNumberColumn(UltraWebGridFFSSR.Bands[1].Columns[i], "N2");
				UltraWebGridFFSSR.Bands[1].Columns[i].Width = 110;
			}
			for (int i = 2; i < UltraWebGridFFSSR.Bands[0].Columns.Count; i = i + 2)
			{
				CRHelper.FormatNumberColumn(UltraWebGridFFSSR.Bands[0].Columns[i], "P0");
				UltraWebGridFFSSR.Bands[0].Columns[i].Width = 80;
			}
			for (int i = 3; i < UltraWebGridFFSSR.Bands[1].Columns.Count; i = i + 2)
			{
				CRHelper.FormatNumberColumn(UltraWebGridFFSSR.Bands[1].Columns[i], "P0");
				UltraWebGridFFSSR.Bands[1].Columns[i].Width = 80;
			}

			UltraWebGridFFSSR.Bands[0].Columns[0].Width = 200;
			UltraWebGridFFSSR.Bands[1].Columns[0].Width = 200;
			UltraWebGridFFSSR.Bands[0].Columns[0].CellStyle.Wrap = true;
			UltraWebGridFFSSR.Bands[1].Columns[0].CellStyle.Wrap = true;
			UltraWebGridFFSSR.Bands[1].Columns[1].Hidden = true;

			if (IsPostBack)
				return;

			foreach (UltraGridColumn c in e.Layout.Bands[0].Columns)
			{
				c.Header.RowLayoutColumnInfo.OriginY = 1;
			}

			int multiHeaderPos = 1;
			string[] captions;

			for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i = i + 2)
			{

				ColumnHeader ch = new ColumnHeader(true);
				captions = e.Layout.Bands[0].Columns[i].Header.Caption.Split(';');
				ch.Caption = captions[0];
				e.Layout.Bands[0].Columns[i].Header.Caption = captions[1];
				e.Layout.Bands[0].Columns[i + 1].Header.Caption = e.Layout.Bands[0].Columns[i + 1].Header.Caption.Split(';')[1];
				ch.RowLayoutColumnInfo.OriginY = 0;
				ch.RowLayoutColumnInfo.OriginX = multiHeaderPos;
                multiHeaderPos += 2;
				ch.RowLayoutColumnInfo.SpanX = 2;
                ch.Style.Wrap = true;
				e.Layout.Bands[0].HeaderLayout.Add(ch);
			}

			foreach (UltraGridColumn c in e.Layout.Bands[1].Columns)
			{
				c.Header.RowLayoutColumnInfo.OriginY = 1;
			}

			multiHeaderPos = 1;

			for (int i = 2; i < e.Layout.Bands[1].Columns.Count; i = i + 2)
			{
				ColumnHeader ch = new ColumnHeader(true);
				captions = e.Layout.Bands[1].Columns[i].Header.Caption.Split(';');
				ch.Caption = captions[0];
				e.Layout.Bands[1].Columns[i].Header.Caption = captions[1];
				e.Layout.Bands[1].Columns[i + 1].Header.Caption = e.Layout.Bands[1].Columns[i + 1].Header.Caption.Split(';')[1];
				ch.RowLayoutColumnInfo.OriginY = 0;
				ch.RowLayoutColumnInfo.OriginX = multiHeaderPos;
                multiHeaderPos += 2;
				ch.RowLayoutColumnInfo.SpanX = 2;
                ch.Style.Wrap = true;
				e.Layout.Bands[1].HeaderLayout.Add(ch);
			}
			UltraWebGridFFSSR.DisplayLayout.GroupByBox.Hidden = true;
		}

		#endregion

		protected void UltraChartFFSSR2_ChartDataClicked(object sender, ChartDataEventArgs e)
		{

		}
	}
}
