using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.UltraChart.Core.Layers;
using System.Drawing;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Core;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.MFRF_0001_0004_FFK
{
	public partial class Default : CustomReportPage
	{
        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

			int currentWidth = (int)Session["width_size"] - 50;
			UltraWebGridFFK.Width = (int)(currentWidth * 0.5);
			UltraChartFFK1.Width = (int)(currentWidth * 0.5);
			UltraChartFFK2.Width = (int)(currentWidth * 0.5);

            int currentHeight = (int)Session["height_size"] - 200;
			UltraWebGridFFK.Height = (int)(currentHeight - 123);
			UltraChartFFK1.Height = (int)(currentHeight * 0.6);
			UltraChartFFK2.Height = (int)(currentHeight * 0.4 - 10);
		}

		protected override void Page_Load(object sender, EventArgs e)
		{
			base.Page_Load(sender, e);
			curStateAreaNameFFK = string.Empty;
			if (!Page.IsPostBack)
			{
				ComboYear.SelectedIndex = 10;
			}
			string pValue = string.Format("[Период].[Год].[Данные всех периодов].[{0}]", ComboYear.SelectedRow.Cells[0].Value);
            
			//Обновляем таблицу только, если либо параметр периода был изменен, либо загружаемся в первый раз
            lbFO.Text = string.Empty;
            lbSybject.Text = string.Empty;

			if (!Page.IsPostBack || !UserParams.PeriodYear.ValueIs(pValue))
			{
				UserParams.PeriodYear.Value = string.Format("[Период].[Год].[Данные всех периодов].[{0}]", ComboYear.SelectedRow.Cells[0].Value);
				UltraWebGridFFK.DataBind();
			}
			ShowHideCharts(false);
		}

		#region ФФК
		DataTable dtMasterFFK = new DataTable();
		DataTable dtDetailFFK = new DataTable();
		DataTable dtChartFFK1 = new DataTable();
		DataTable dtChartFFK2 = new DataTable();
		DataSet tableDataSetFFK = new DataSet();
		//имя текущего субъекта выбранного во втором бенде мастер-таблицы
		private string curStateAreaNameFFK = string.Empty;

		private void ShowHideCharts(bool show)
		{
			UltraChartFFK1.Visible = show;
			UltraChartFFK2.Visible = show;
		}

		protected void UltraWebGridFFK_DataBinding(object sender, EventArgs e)
		{
			string query = DataProvider.GetQueryText("FFKMasterTable");
			DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Федеральный округ", dtMasterFFK);

			query = DataProvider.GetQueryText("FFKDetailTable");
			DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Субъект РФ", dtDetailFFK);

			tableDataSetFFK.Tables.Add(dtMasterFFK);
			tableDataSetFFK.Tables.Add(dtDetailFFK);

			tableDataSetFFK.Relations.Add(dtMasterFFK.Columns[0], dtDetailFFK.Columns[1]);
			UltraWebGridFFK.DataSource = tableDataSetFFK.Tables[0].DefaultView;
		}

		protected void UltraChartFFK1_DataBinding(object sender, EventArgs e)
		{
			string query = DataProvider.GetQueryText("FFKChart1");
			DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "seriesName", dtChartFFK1);
			UltraChartFFK1.DataSource = dtChartFFK1;           
		}

		protected void UltraChartFFK2_DataBinding(object sender, EventArgs e)
		{
			string query = DataProvider.GetQueryText("FFKChart2");
			DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "seriesName", dtChartFFK2);
			UltraChartFFK2.DataSource = dtChartFFK2;			
		}

		protected void UltraWebGridFFK_ActiveRowChange(object sender, Infragistics.WebUI.UltraWebGrid.RowEventArgs e)
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
			curStateAreaNameFFK = e.Row.Cells[1].Text;
			UltraChartFFK1.DataBind();
			UltraChartFFK2.DataBind();
			ShowHideCharts(true);
		}

		protected void UltraWebGridFFK_InitializeLayout(object sender, Infragistics.WebUI.UltraWebGrid.LayoutEventArgs e)
		{
			for (int i = 1; i < UltraWebGridFFK.Bands[0].Columns.Count; i = i + 2)
			{
				CRHelper.FormatNumberColumn(UltraWebGridFFK.Bands[0].Columns[i], "N2");
				UltraWebGridFFK.Bands[0].Columns[i].Width = 100;
			}
			UltraWebGridFFK.Bands[0].Columns[0].CellStyle.Wrap = true;
            UltraWebGridFFK.Bands[0].Columns[0].Width = 250;

			for (int i = 2; i < UltraWebGridFFK.Bands[1].Columns.Count; i = i + 2)
			{
				CRHelper.FormatNumberColumn(UltraWebGridFFK.Bands[1].Columns[i], "N2");
				UltraWebGridFFK.Bands[1].Columns[i].Width = 100;
			}

			for (int i = 2; i < UltraWebGridFFK.Bands[0].Columns.Count; i = i + 2)
			{
				CRHelper.FormatNumberColumn(UltraWebGridFFK.Bands[0].Columns[i], "P0");
				UltraWebGridFFK.Bands[0].Columns[i].Width = 75;
			}
			UltraWebGridFFK.Bands[0].Columns[0].CellStyle.Wrap = true;

			for (int i = 3; i < UltraWebGridFFK.Bands[1].Columns.Count; i = i + 2)
			{
				CRHelper.FormatNumberColumn(UltraWebGridFFK.Bands[1].Columns[i], "P0");
				UltraWebGridFFK.Bands[1].Columns[i].Width = 75;
			}


			UltraWebGridFFK.Bands[1].Columns[0].CellStyle.Wrap = true;
			UltraWebGridFFK.Bands[1].Columns[0].Width = 250;
			UltraWebGridFFK.Bands[1].Columns[1].Hidden = true;

			if (IsPostBack)
				return;

			foreach (Infragistics.WebUI.UltraWebGrid.UltraGridColumn c in e.Layout.Bands[0].Columns)
			{
				c.Header.RowLayoutColumnInfo.OriginY = 1;
			}

			int multiHeaderPos = 1;
			string[] captions;

			for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i = i + 2)
			{

				Infragistics.WebUI.UltraWebGrid.ColumnHeader ch = new Infragistics.WebUI.UltraWebGrid.ColumnHeader(true);
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

			foreach (Infragistics.WebUI.UltraWebGrid.UltraGridColumn c in e.Layout.Bands[1].Columns)
			{
				c.Header.RowLayoutColumnInfo.OriginY = 1;
			}

			multiHeaderPos = 1;

			for (int i = 2; i < e.Layout.Bands[1].Columns.Count; i = i + 2)
			{
				Infragistics.WebUI.UltraWebGrid.ColumnHeader ch = new Infragistics.WebUI.UltraWebGrid.ColumnHeader(true);
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
            e.Layout.GroupByBox.Hidden = true;
		}

		#endregion
	}
}
