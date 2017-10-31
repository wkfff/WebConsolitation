using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Drawing;
using System.Web.UI;
using System.Web.UI.WebControls;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Krista.FM.Server.Dashboards.iPadBricks.Sport_0001;
using OrderedData = Krista.FM.Server.Dashboards.iPadBricks.Sport_0001.OrderedData;
using OrderedValue = Krista.FM.Server.Dashboards.iPadBricks.Sport_0001.OrderedValue;

namespace Krista.FM.Server.Dashboards.reports.iPhone
{
	/// <summary>
	/// Отчетность Минспорта (по 6 показателям) #20203
	/// детализация РФ,ФО - сооружения (сравнение территорий)
	/// </summary>
    public partial class Sport_0001_0010 : CustomReportPage
    {
    	public string TEMPORARY_URL_PREFIX = "../../..";
		public string REPORT_ID = "Sport_0001_0010";
		
		// параметры запросов
		public CustomParam paramYearLast;
		public CustomParam paramYearPrev;
		public CustomParam paramTerritory;
		

		protected override void Page_PreLoad(object sender, EventArgs e)
		{
			base.Page_PreLoad(sender, e);
			
			// Инициализация параметров запроса
			paramYearLast = UserParams.CustomParam("param_year_last");
			paramYearPrev = UserParams.CustomParam("param_year_prev");
			paramTerritory = UserParams.CustomParam("param_territory");
		}

    	protected override void Page_Load(object sender, EventArgs e)
        {
			base.Page_Load(sender, e);

			// параметры
			int foID = 0;
			string territory = "[Российская  Федерация]";
			if (Session["CurrentFOID"] != null)
			{
				foID = Convert.ToInt32(Session["CurrentFOID"].ToString());
				if (foID > 0)
				{
					territory = String.Format("[Российская  Федерация].[{0}]", UserParams.FullRegionName.Value);
				}
			}
			paramTerritory.Value = territory;
			paramYearLast.Value = Convert.ToString(Helper.YEAR_LAST);
			paramYearPrev.Value = Convert.ToString(Helper.YEAR_PREV);

    		Header.Text = "Спортсооружения по территориям";

			ChartRecreationHelper chartRecreation = new ChartRecreationHelper(UltraChartRecreation);
			chartRecreation.TemporaryUrlPrefix = TEMPORARY_URL_PREFIX;
    		chartRecreation.FoID = foID;
			chartRecreation.SetStyleAndData();

			DataTable msTable = DataProvider.GetDataTableForChart("Sport_0001_0010_minsport_grid", DataProvidersFactory.PrimaryMASDataProvider);
			DataTable fmTable = DataProvider.GetDataTableForChart("Sport_0001_0010_population_grid", DataProvidersFactory.SecondaryMASDataProvider);

    		GridHelper gridHelper;
			
			HeaderGymn.Text = "Спортивные залы";
			gridHelper = new GridHelper(GridBrickGymn);
    		gridHelper.Page = this;
    		gridHelper.FoID = foID;
			gridHelper.Color = Helper.GetRecreationColor(RecreationType.Gymnasium);
			gridHelper.Data = FillOrderedData(msTable, fmTable, 1);
			gridHelper.DataPrev = FillOrderedData(msTable, fmTable, 4);
    		gridHelper.AvgValue = Helper.PROVIDE_AVG_GYMN;
    		gridHelper.NormValue = Helper.PROVIDE_NRM_GYMN;
			gridHelper.SetStyleAndData();

    		HeaderStad.Text = "Плоскостные сооружения";
			gridHelper = new GridHelper(GridBrickStad);
			gridHelper.Page = this;
			gridHelper.FoID = foID;
			gridHelper.Color = Helper.GetRecreationColor(RecreationType.Stadium);
			gridHelper.Data = FillOrderedData(msTable, fmTable, 2);
			gridHelper.DataPrev = FillOrderedData(msTable, fmTable, 5);
			gridHelper.AvgValue = Helper.PROVIDE_AVG_STAD;
			gridHelper.NormValue = Helper.PROVIDE_NRM_STAD;
			gridHelper.SetStyleAndData();

    		HeaderSwim.Text = "Плавательные бассейны";
			gridHelper = new GridHelper(GridBrickSwim);
			gridHelper.Page = this;
			gridHelper.FoID = foID;
			gridHelper.Color = Helper.GetRecreationColor(RecreationType.Swimming);
			gridHelper.Data = FillOrderedData(msTable, fmTable, 3);
			gridHelper.DataPrev = FillOrderedData(msTable, fmTable, 6);
			gridHelper.AvgValue = Helper.PROVIDE_AVG_SWIM;
			gridHelper.NormValue = Helper.PROVIDE_NRM_SWIM;
			gridHelper.SetStyleAndData();

        }

		private static OrderedData FillOrderedData(DataTable msTable, DataTable fmTable, int columnIndex)
		{
			OrderedData orderedData = new OrderedData(msTable, columnIndex);
			foreach (OrderedValue value in orderedData.Data)
			{
				if (!value.IsEmpty)
				{
					value.SetExtraValue(CRHelper.DBValueConvertToDecimalOrZero(fmTable.FindValue(value.ID, 1)));
				}
			}
			return orderedData;
		}

    }

	public class ChartRecreationHelper : ChartWrapper
	{
		public int FoID { set; get; }
		
		public ChartRecreationHelper(UltraChartItem chartItem)
			: base(chartItem)
		{

		}
		
		public new void SetStyleAndData()
		{
			// получение данных
			DataTable msTable = DataProvider.GetDataTableForChart("Sport_0001_0010_minsport", DataProvidersFactory.PrimaryMASDataProvider);
			DataTable fmTable = DataProvider.GetDataTableForChart("Sport_0001_0010_population", DataProvidersFactory.SecondaryMASDataProvider);

			Table = new DataTable();
			Table.Columns.Add(new DataColumn("title", typeof(string)));
			Table.Columns.Add(new DataColumn("gymnasium", typeof(decimal)) { Caption = "Спортивные залы" });
			Table.Columns.Add(new DataColumn("stadium", typeof(decimal)) { Caption = "Плоскостные сооружения" });
			Table.Columns.Add(new DataColumn("swimming", typeof(decimal)) { Caption = "Плавательные бассейны" });
			
			for (int i = 0; i < msTable.Rows.Count; i++)
			{
				string id = msTable.Rows[i][0].ToString();
				object rawGymn = msTable.Rows[i][1];
				object rawStad = msTable.Rows[i][2];
				object rawSwim = msTable.Rows[i][3];
				object rawPopulation = fmTable.Rows[i][1];

				if (CRHelper.DBValueIsEmpty(rawGymn) || CRHelper.DBValueIsEmpty(rawStad) || CRHelper.DBValueIsEmpty(rawSwim) || CRHelper.DBValueIsEmpty(rawPopulation))
					continue;
				
				decimal population = CRHelper.DBValueConvertToDecimalOrZero(rawPopulation);

				DataRow row = Table.NewRow();
				row[0] = RegionsNamingHelper.ShortName(id);
				row[1] = 10 * Helper.PROVIDE_AVG_GYMN * CRHelper.DBValueConvertToDecimalOrZero(rawGymn) / population / Helper.PROVIDE_NRM_GYMN;
				row[2] = 10 * Helper.PROVIDE_AVG_STAD * CRHelper.DBValueConvertToDecimalOrZero(rawStad) / population / Helper.PROVIDE_NRM_STAD;
				row[3] = 10 * Helper.PROVIDE_AVG_SWIM * CRHelper.DBValueConvertToDecimalOrZero(rawSwim) / population / Helper.PROVIDE_NRM_SWIM;
				Table.Rows.Add(row); 
			}
			
			// настройка общего стиля
			TitleLeft = "Обеспеченность спортивными\nсооружениями (% от норматива)";
			ToolTipFormatString = "<ITEM_LABEL>";
			base.SetStyle();

			// настройка индивидуального стиля

			ChartControl.ZeroAligned = true;
			ChartControl.SwapRowAndColumns = true;
			
			ChartControl.Chart.TitleLeft.Margins.Bottom = 80;

			ChartControl.Chart.ChartType = ChartType.ColumnChart;
			ChartControl.Chart.ColumnChart.SeriesSpacing = 0;

			ChartControl.Chart.Axis.Y.Visible = true;
			ChartControl.Chart.Axis.Y.Extent = 35;
			ChartControl.Chart.Axis.Y.MajorGridLines.Color = DefaultColorDark;
			ChartControl.Chart.Axis.Y.Labels.LabelStyle.Dx = -2;
			ChartControl.Chart.Axis.Y.Labels.Font = DefaultFontSmall;
			ChartControl.Chart.Axis.Y.Labels.FontColor = DefaultColor;
			ChartControl.Chart.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:P0>"; 

			ChartControl.Chart.Axis.X.Visible = true;
			ChartControl.Chart.Axis.X.MajorGridLines.Color = DefaultColorDark;
			ChartControl.Chart.Axis.X.Labels.Visible = false;
			ChartControl.Chart.Axis.X.Labels.SeriesLabels.Visible = true;
			ChartControl.Chart.Axis.X.Labels.SeriesLabels.LabelStyle.Dy = 3;
			ChartControl.Chart.Axis.X.Labels.SeriesLabels.Font = DefaultFont;
			ChartControl.Chart.Axis.X.Labels.SeriesLabels.FontColor = DefaultColor;
			if (FoID == 0)
			{
				ChartControl.Chart.Axis.X.Extent = 20;
			}
			else
			{
				ChartControl.Chart.Axis.X.Extent = 70;
				ChartControl.Chart.Axis.X.Labels.SeriesLabels.LabelStyle.Dy = -2;
				ChartControl.Chart.Axis.X.Labels.SeriesLabels.Orientation = TextOrientation.VerticalLeftFacing;
			}

			ChartControl.LegendVisible = true;
			ChartControl.Chart.Legend.Location = LegendLocation.Bottom;
			ChartControl.Chart.Legend.BackgroundColor = Color.FromArgb(unchecked((int)0xFF696969));
			ChartControl.Chart.Legend.BorderColor = Color.FromArgb(unchecked((int)0xFF151515));
			ChartControl.Chart.Legend.BorderThickness = 3;
			ChartControl.Chart.Legend.Font = DefaultFont;
			ChartControl.Chart.Legend.FontColor = DefaultColor;
			ChartControl.Chart.Legend.Margins.Left = 45;
			ChartControl.Chart.Legend.Margins.Right = 10;
			ChartControl.Chart.Legend.SpanPercentage = 10;
			

			// привязка данных

			for (int i = 1; i <= 3; i++)
			{
				NumericSeries series = CRHelper.GetNumericSeries(i, Table);
				series.PEs.Add(Helper.GetRecreationPE((RecreationType)i));
				series.Label = Table.Columns[i].Caption;
				ChartControl.Chart.Series.Add(series);
			}
		}

		protected override void FillSceneGraph(object sender, Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e)
		{
			base.FillSceneGraph(sender, e);
			
			for (int i = 0; i < e.SceneGraph.Count; i++)
			{
				Primitive primitive = e.SceneGraph[i];

				if (primitive is Box)
				{
					Box item = (Box)primitive;
					if (item.DataPoint != null)
					{
						const double space = 6;
						item.rect.Width = item.rect.Width - (int)Math.Round(space*2/6);
						if (item.DataPoint.Label.ToLower().Contains("спортивные залы"))
						{
							item.rect.X = item.rect.X + (int) Math.Round(space*3/6);
						}
						if (item.DataPoint.Label.ToLower().Contains("плоскостные сооружения"))
						{
							item.rect.X = item.rect.X + (int) Math.Round(space*1/6);
						}
						if (item.DataPoint.Label.ToLower().Contains("плавательные бассейны"))
						{
							item.rect.X = item.rect.X - (int) Math.Round(space*1/6);
						}

						item.DataPoint.Label = String.Format(
							"&nbsp;{0}&nbsp;<br />&nbsp;{1}&nbsp;<br />&nbsp;<b>{2}</b>&nbsp;",
							item.DataPoint.Label,
							RegionsNamingHelper.FullName(item.Series.Label),
							String.Format("{0:P2}", item.Value));
					}

				}

			}
		}

	}
	
	public class GridHelper : UltraGridWrapper
	{
		public Sport_0001_0010 Page { set; get; }
		public int FoID { set; get; }
		public decimal AvgValue { set; get; }
		public decimal NormValue { set; get; }
		public Color Color { set; get; }
		public OrderedData Data { set; get; }
		public OrderedData DataPrev { set; get; }
		
		private Collection<string> IDs { set; get; }
		
		public GridHelper(UltraGridBrick gridBrick) : base(gridBrick)
		{
			IDs = new Collection<string>();
		}

		protected override void SetStyle()
		{
			GridBrick.BrowserSizeAdapting = false;
			GridBrick.AutoSizeStyle = GridAutoSizeStyle.Auto;
			GridBrick.RedNegativeColoring = false;
		}

		protected override void SetData()
		{
			Table = new DataTable();
			Table.Columns.Add(new DataColumn("title", typeof(string)));
			Table.Columns.Add(new DataColumn("abs", typeof(decimal)));
			Table.Columns.Add(new DataColumn("provide", typeof(decimal)));
			Table.Columns.Add(new DataColumn("norm", typeof(decimal)));
			Table.Columns.Add(new DataColumn("rank", typeof(int)));
			Table.Columns.Add(new DataColumn("prev_abs", typeof(decimal)));
			Table.Columns.Add(new DataColumn("prev_provide", typeof(decimal)));
			Table.Columns.Add(new DataColumn("prev_norm", typeof(decimal)));
			HiddenColumns = 3; 

			OrderedData dataRank = new OrderedData { Precision = 4 };

			foreach (OrderedValue value in Data.Data)
			{
				string id = value.ID;
				OrderedValue valuePrev = DataPrev[id];

				if (value.IsEmpty)
					continue;
				if (FoID > 0 && RegionsNamingHelper.IsFO(id))
					continue;
				
				IDs.Add(id);
				
				DataRow row = Table.NewRow();

				if (RegionsNamingHelper.IsRF(id))
					row[0] = String.Format("<a href=\"webcommand?showReport=Sport_0001_0001_fo=0\">{0}</a>", id);
				else if (RegionsNamingHelper.IsFO(id))
					row[0] = String.Format("<a href=\"webcommand?showReport=Sport_0001_0001_fo={0}\">{1}</a>", CustomParams.GetFOIdByName(id), id);
				else if (RegionsNamingHelper.IsSubject(id))
					row[0] = String.Format("<a href=\"webcommand?showReport=Sport_0001_0002_{0}\">{1}</a>", CustomParams.GetSubjectIdByName(id), id);
				
				row[1] = value.Value;
				row[2] = 10 * AvgValue * value.Value / value.ExtraValue;
				row[3] = (decimal) row[2] / NormValue;

				row[5] = valuePrev.Value;
				row[6] = 10 * AvgValue * valuePrev.Value / valuePrev.ExtraValue;
				row[7] = (decimal) row[6] / NormValue;

				Table.Rows.Add(row);
				
				if (RegionsNamingHelper.IsSubject(id))
				{
					dataRank.Add(new OrderedValue(id, (decimal)row[3]));
				}

			}

			// ранги
			dataRank.Sort();
			
			for (int i = 0; i < Table.Rows.Count; i++)
			{
				DataRow row = Table.Rows[i];
				string id = IDs[i];

				if (RegionsNamingHelper.IsSubject(id))
				{
					int rank = dataRank.GetRank(id);
					if (rank == 0)
						row[4] = DBNull.Value;
					else
						row[4] = rank == dataRank.MaxRank ? -rank : rank;
				}
			}

			GridBrick.DataTable = Table;
		}
		
		protected override void SetDataStyle()
		{
			Band.Columns[0].CellStyle.Wrap = true;
			Band.Columns[0].Width = 340;
			Band.Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;

			Band.HideColumns(HiddenColumns);

			Band.Columns[1].Width = 110;
			Band.Columns[2].Width = 100;
			Band.Columns[3].Width = 140;
			Band.Columns[4].Width = 50;

			CRHelper.FormatNumberColumn(Band.Columns[1], "N0");
			CRHelper.FormatNumberColumn(Band.Columns[2], "N2");
			CRHelper.FormatNumberColumn(Band.Columns[3], "P2");

			for (int i = 1; i <= 3; i++)
			{
				Band.Columns[i].CellStyle.Padding.Right = 5;
			}
			
		}

		protected override void SetDataRules()
		{
			// empty here
		}

		protected override void InitializeRow(object sender, RowEventArgs e)
		{
			UltraGridRow row = e.Row;
			int rowIndex = row.DataSourceIndex;
			string id = IDs[row.DataSourceIndex];

			row.Style.Height = 34;
			row.Cells[0].Style.BorderDetails.WidthRight = 3;
			row.Cells[1].Style.BorderDetails.WidthRight = 3;
			row.Cells[2].Style.BorderDetails.WidthRight = 3;

			// стрелки
			foreach (int columnIndex in (new[] { 1 }))
			{
				int columnCompare = columnIndex + 4;
				if (CRHelper.DBValueIsEmpty(Table.Rows[rowIndex][columnIndex])
					|| CRHelper.DBValueIsEmpty(Table.Rows[rowIndex][columnCompare]))
					continue;

				UltraGridCell cell = row.Cells[columnIndex];

				decimal value1 = CRHelper.DBValueConvertToDecimalOrZero(Table.Rows[rowIndex][columnIndex]);
				decimal value2 = CRHelper.DBValueConvertToDecimalOrZero(Table.Rows[rowIndex][columnCompare]);

				if (value1.CompareTo(value2, 2) > 0)
				{
					cell.Style.BackgroundImage = Helper.UpArrowFile();
				}
				else if (value1.CompareTo(value2, 2) < 0)
				{
					cell.Style.BackgroundImage = Helper.DnArrowFile();
				}
				if (!value1.EqualsTo(value2, 2))
				{
					cell.Style.CustomRules = Helper.CellIndicatorStyle(20);
				}
			}

			// отступы
			if (FoID == 0)
			{
				if (RegionsNamingHelper.IsRF(id) || RegionsNamingHelper.IsFO(id))
				{
					row.Style.Font.Bold = true;
				}
				else if (RegionsNamingHelper.IsSubject(id))
				{
					row.Cells[0].Style.Padding.Left = 20;
				}
			}

			// диаграммы
			foreach (int columnIndex in (new[] { 3 }))
			{
				UltraGridCell cell = row.Cells[columnIndex];
				decimal value;
				if (cell.Value != null && Decimal.TryParse(cell.Value.ToString(), out value))
				{
					cell.Style.BackgroundImage = String.Format("{0}/TemporaryImages/{1}", Page.TEMPORARY_URL_PREFIX, GetPieChart(value));
					cell.Style.CustomRules = Helper.CellIndicatorStyle(20);
				}

			}

			// ранги
			foreach (int columnIndex in (new[] { 4 }))
			{
				UltraGridCell cell = row.Cells[columnIndex];

				int value;
				if (cell.Value != null && Int32.TryParse(cell.Value.ToString(), out value))
				{
					if (value == 1)
					{
						cell.Style.BackgroundImage = Helper.BestRankFile();
						cell.Style.CustomRules = Helper.CellIndicatorStyle();
					}
					else if (value < 0)
					{
						cell.Value = -value;
						cell.Style.BackgroundImage = Helper.WorstRankFile();
						cell.Style.CustomRules = Helper.CellIndicatorStyle();
					}
				}
			}
			
		}

		protected override void SetDataHeader()
		{
			GridHeaderLayout headerLayout = GridBrick.GridHeaderLayout;
			
			headerLayout.AddCell("Территория");
			headerLayout.AddCell("Количество");
			headerLayout.AddCell("Обеспе-<br />ченность");
			headerLayout.AddCell("% от норматива");
			headerLayout.AddCell("Ранг");

			GridBrick.GridHeaderLayout.ApplyHeaderInfo();
		}

		private string GetPieChart(decimal value)
		{
			RecreationPieChart chart = new RecreationPieChart(
				(UltraChartItem)Page.LoadControl("../../../Components/UltraChartItem.ascx"), Page.TEMPORARY_URL_PREFIX, value, Color);
			chart.SetStyleAndData();

			string filename = String.Format("{0}_provide_{1}.png", Page.REPORT_ID, value.ToString("N4"));
			chart.SaveTo(filename);
			return filename;
		}

	}
}