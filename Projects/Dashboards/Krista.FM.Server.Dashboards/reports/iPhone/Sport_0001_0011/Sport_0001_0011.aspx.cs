using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Drawing;
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
	/// детализация РФ, ФО - сооружения (годовая динамика)
	/// </summary>
    public partial class Sport_0001_0011 : CustomReportPage
    {
    	public string TEMPORARY_URL_PREFIX = "../../..";
		public string REPORT_ID = "Sport_0001_0011";
		
		// параметры запросов
		public CustomParam paramYearLast;
		public CustomParam paramTerritory;
		

		protected override void Page_PreLoad(object sender, EventArgs e)
		{
			base.Page_PreLoad(sender, e);
			
			// Инициализация параметров запроса
			paramYearLast = UserParams.CustomParam("param_year_last");
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

			Header.Text = "Число спортсооружений в динамике";

			DataTable msTable = DataProvider.GetDataTableForChart("Sport_0001_0011_minsport", DataProvidersFactory.PrimaryMASDataProvider);
			msTable.InvertRowsOrder();
			DataTable fmTable = DataProvider.GetDataTableForChart("Sport_0001_0011_population", DataProvidersFactory.SecondaryMASDataProvider);
			fmTable.InvertRowsOrder();

			ChartCount chartCount = new ChartCount(UltraChartCount);
			chartCount.TemporaryUrlPrefix = TEMPORARY_URL_PREFIX;
			chartCount.MsTable = msTable;
			chartCount.SetStyleAndData();
			
			ChartProvide chartProvide = new ChartProvide(UltraChartProvide);
			chartProvide.TemporaryUrlPrefix = TEMPORARY_URL_PREFIX;
			chartProvide.MsTable = msTable;
			chartProvide.FmTable = fmTable;
			chartProvide.SetStyleAndData();

			GridHelper gridHelper;

			HeaderGymn.Text = "Спортивные залы";
			gridHelper = new GridHelper(GridBrickGymn);
			gridHelper.Page = this;
			gridHelper.Data = FillOrderedData(msTable, fmTable, 2);
			gridHelper.AvgValue = Helper.PROVIDE_AVG_GYMN;
			gridHelper.NormValue = Helper.PROVIDE_NRM_GYMN;
			gridHelper.Measure = "тыс. м² на 10 тыс. чел.";
    		gridHelper.Color = Helper.GetRecreationColor(RecreationType.Gymnasium);
			gridHelper.SetStyleAndData();

			HeaderStad.Text = "Плоскостные сооружения";
			gridHelper = new GridHelper(GridBrickStad);
			gridHelper.Page = this;
			gridHelper.Data = FillOrderedData(msTable, fmTable, 3);
			gridHelper.AvgValue = Helper.PROVIDE_AVG_STAD;
			gridHelper.NormValue = Helper.PROVIDE_NRM_STAD;
			gridHelper.Measure = "тыс. м² на 10 тыс. чел.";
			gridHelper.Color = Helper.GetRecreationColor(RecreationType.Stadium);
			gridHelper.SetStyleAndData();

			HeaderSwim.Text = "Плавательные бассейны";
			gridHelper = new GridHelper(GridBrickSwim);
			gridHelper.Page = this;
			gridHelper.Data = FillOrderedData(msTable, fmTable, 4);
			gridHelper.AvgValue = Helper.PROVIDE_AVG_SWIM;
			gridHelper.NormValue = Helper.PROVIDE_NRM_SWIM;
			gridHelper.Measure = "м² на 10 тыс. чел.";
			gridHelper.Color = Helper.GetRecreationColor(RecreationType.Swimming);
			gridHelper.SetStyleAndData(); 
			
        }

		private static OrderedData FillOrderedData(DataTable msTable, DataTable fmTable, int columnIndex)
		{
			OrderedData orderedData = new OrderedData(msTable, columnIndex);
			foreach (OrderedValue value in orderedData.Data)
			{
				if (!value.IsEmpty)
				{
					object raw = fmTable.FindValue(value.ID, 1);
					if (!CRHelper.DBValueIsEmpty(raw))
					{
						value.SetExtraValue(CRHelper.DBValueConvertToDecimalOrZero(raw));
					}
				}
			}
			return orderedData;
		}

	}

	public class ChartCount : ChartWrapper
	{
		public DataTable MsTable { set; get; }
		
		public ChartCount(UltraChartItem chartItem)
			: base(chartItem)
		{

		}

		public new void SetStyleAndData()
		{
			// получение данных
			
			Table = new DataTable();
			Table.Columns.Add(new DataColumn("title", typeof(string)));
			Table.Columns.Add(new DataColumn("gymnasium", typeof(decimal)) { Caption = "Спортивные залы" });
			Table.Columns.Add(new DataColumn("stadium", typeof(decimal)) { Caption = "Плоскостные сооружения" });
			Table.Columns.Add(new DataColumn("swimming", typeof(decimal)) { Caption = "Плавательные бассейны" });
			Table.Columns.Add(new DataColumn("other", typeof(decimal)) { Caption = "Прочие сооружения" });

			for (int i = 0; i < MsTable.Rows.Count; i++)
			{
				string id = MsTable.Rows[i][0].ToString();
				object rawCount = MsTable.Rows[i][1];
				object rawGymn = MsTable.Rows[i][2];
				object rawStad = MsTable.Rows[i][3];
				object rawSwim = MsTable.Rows[i][4];

				if (CRHelper.DBValueIsEmpty(rawCount) || CRHelper.DBValueIsEmpty(rawGymn) || CRHelper.DBValueIsEmpty(rawStad) || CRHelper.DBValueIsEmpty(rawSwim))
					continue;

				decimal vCount = CRHelper.DBValueConvertToDecimalOrZero(rawCount);
				decimal vGymn = CRHelper.DBValueConvertToDecimalOrZero(rawGymn);
				decimal vStad = CRHelper.DBValueConvertToDecimalOrZero(rawStad);
				decimal vSwim = CRHelper.DBValueConvertToDecimalOrZero(rawSwim);

				DataRow row = Table.NewRow();
				row[0] = id;
				row[1] = vGymn;
				row[2] = vStad;
				row[3] = vSwim;
				row[4] = vCount - vGymn - vStad - vSwim;
				Table.Rows.Add(row);

			}
			
			// инфа за последние 10 лет
			while (Table.Rows.Count > 10)
			{
				Table.Rows.RemoveAt(Table.Rows.Count - 1);
			}

			// настройка общего стиля
			TitleLeft = "Число спортивных сооружений";
			ToolTipFormatString = "<ITEM_LABEL><br />&nbsp;<b><DATA_VALUE:N0></b>&nbsp;";
			base.SetStyle();

			// настройка индивидуального стиля

			ChartControl.ZeroAligned = true;
			ChartControl.SwapRowAndColumns = true;

			ChartControl.Chart.ChartType = ChartType.StackColumnChart;
			
			ChartControl.Chart.Axis.Y.Visible = true;
			ChartControl.Chart.Axis.Y.Extent = 50;
			ChartControl.Chart.Axis.Y.MajorGridLines.Color = DefaultColorDark;
			ChartControl.Chart.Axis.Y.Labels.LabelStyle.Dx = -2;
			ChartControl.Chart.Axis.Y.Labels.Font = DefaultFontSmall;
			ChartControl.Chart.Axis.Y.Labels.FontColor = DefaultColor;
			ChartControl.Chart.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N0>";
			
			ChartControl.Chart.Axis.X.Visible = true;
			ChartControl.Chart.Axis.X.Extent = 20;
			ChartControl.Chart.Axis.X.MajorGridLines.Color = DefaultColorDark;
			ChartControl.Chart.Axis.X.Labels.Visible = false;
			ChartControl.Chart.Axis.X.Labels.SeriesLabels.Visible = true;
			ChartControl.Chart.Axis.X.Labels.SeriesLabels.Font = DefaultFont;
			ChartControl.Chart.Axis.X.Labels.SeriesLabels.FontColor = DefaultColor;
			ChartControl.Chart.Axis.X.Labels.SeriesLabels.Orientation = TextOrientation.Horizontal;
			ChartControl.Chart.Axis.X.Labels.SeriesLabels.LabelStyle.Dy = 3;

			ChartControl.LegendVisible = true;
			ChartControl.Chart.Legend.Location = LegendLocation.Bottom;
			ChartControl.Chart.Legend.BackgroundColor = Color.FromArgb(unchecked((int)0xFF696969));
			ChartControl.Chart.Legend.BorderColor = Color.FromArgb(unchecked((int)0xFF151515));
			ChartControl.Chart.Legend.BorderThickness = 3;
			ChartControl.Chart.Legend.Font = DefaultFont;
			ChartControl.Chart.Legend.FontColor = DefaultColor;
			ChartControl.Chart.Legend.Margins.Left = 60;
			ChartControl.Chart.Legend.Margins.Right = 10;
			ChartControl.Chart.Legend.SpanPercentage = 10;

			// привязка данных
			Table.InvertRowsOrder();
			for (int i = 1; i <= 4; i++)
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
					Box item = (Box) primitive;
					if (item.DataPoint != null)
					{
						item.DataPoint.Label = String.Format(
							"&nbsp;{0}&nbsp;<br />&nbsp;{1} год&nbsp;",
							item.DataPoint.Label,
							item.Series.Label
							);
					}
				}

				if (primitive is Text)
				{
					Text item = (Text) primitive;
					if (item.GetTextString().Contains("Прочие сооружения"))
					{
						item.SetTextString("Прочие");
					}
				}
			}
		}
	}

	public class ChartProvide : ChartWrapper
	{
		public DataTable MsTable { set; get; }
		public DataTable FmTable { set; get; }

		public ChartProvide(UltraChartItem chartItem)
			: base(chartItem)
		{

		}

		public new void SetStyleAndData()
		{
			// получение данных

			Table = new DataTable();
			Table.Columns.Add(new DataColumn("title", typeof(string)));
			Table.Columns.Add(new DataColumn("gymnasium", typeof(decimal)) { Caption = "Спортивные залы" });
			Table.Columns.Add(new DataColumn("stadium", typeof(decimal)) { Caption = "Плоскостные сооружения" });
			Table.Columns.Add(new DataColumn("swimming", typeof(decimal)) { Caption = "Плавательные бассейны" });

			decimal maxValue = 0;
			for (int i = 0; i < MsTable.Rows.Count; i++)
			{
				if (i >= FmTable.Rows.Count)
					break;

				string id = MsTable.Rows[i][0].ToString();
				object rawGymn = MsTable.Rows[i][2];
				object rawStad = MsTable.Rows[i][3];
				object rawSwim = MsTable.Rows[i][4];
				object rawPopulation = FmTable.Rows[i][1];

				if (CRHelper.DBValueIsEmpty(rawGymn) || CRHelper.DBValueIsEmpty(rawStad) || CRHelper.DBValueIsEmpty(rawSwim) || CRHelper.DBValueIsEmpty(rawPopulation))
					continue;

				decimal population = CRHelper.DBValueConvertToDecimalOrZero(rawPopulation);

				DataRow row = Table.NewRow();
				row[0] = id;
				row[1] = 10 * Helper.PROVIDE_AVG_GYMN * CRHelper.DBValueConvertToDecimalOrZero(rawGymn) / population / Helper.PROVIDE_NRM_GYMN;
				row[2] = 10 * Helper.PROVIDE_AVG_STAD * CRHelper.DBValueConvertToDecimalOrZero(rawStad) / population / Helper.PROVIDE_NRM_STAD;
				row[3] = 10 * Helper.PROVIDE_AVG_SWIM * CRHelper.DBValueConvertToDecimalOrZero(rawSwim) / population / Helper.PROVIDE_NRM_SWIM;
				Table.Rows.Add(row);

				maxValue = Math.Max(maxValue, (decimal) row[1]);
				maxValue = Math.Max(maxValue, (decimal) row[2]);
				maxValue = Math.Max(maxValue, (decimal) row[3]);
			}
			maxValue = (decimal)(Math.Ceiling(((double)maxValue) * 10) / 10);
			

			// инфа за последние 10 лет
			while (Table.Rows.Count > 10)
			{
				Table.Rows.RemoveAt(Table.Rows.Count - 1);
			}

			// настройка общего стиля
			TitleLeft = "Обеспеченность спортивными\nсооружениями (% от норматива)";
			ToolTipFormatString = "<ITEM_LABEL>";
			base.SetStyle();

			// настройка индивидуального стиля

			ChartControl.ZeroAligned = true;
			ChartControl.SwapRowAndColumns = true;

			ChartControl.Chart.TitleLeft.Margins.Bottom = 50;

			ChartControl.Chart.ChartType = ChartType.ColumnChart;
			ChartControl.Chart.ColumnChart.SeriesSpacing = 0;

			ChartControl.Chart.Axis.Y.Visible = true;
			ChartControl.Chart.Axis.Y.Extent = 35;
			ChartControl.Chart.Axis.Y.MajorGridLines.Color = DefaultColorDark;
			ChartControl.Chart.Axis.Y.Labels.LabelStyle.Dx = -2;
			ChartControl.Chart.Axis.Y.Labels.Font = DefaultFontSmall;
			ChartControl.Chart.Axis.Y.Labels.FontColor = DefaultColor;
			ChartControl.Chart.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:P0>";
			ChartControl.Chart.Axis.Y.RangeType = AxisRangeType.Custom;
			ChartControl.Chart.Axis.Y.RangeMin = 0;
			ChartControl.Chart.Axis.Y.RangeMax = (double)maxValue;

			ChartControl.Chart.Axis.X.Visible = true;
			ChartControl.Chart.Axis.X.Extent = 20;
			ChartControl.Chart.Axis.X.MajorGridLines.Color = DefaultColorDark;
			ChartControl.Chart.Axis.X.Labels.Visible = false;
			ChartControl.Chart.Axis.X.Labels.SeriesLabels.Visible = true;
			ChartControl.Chart.Axis.X.Labels.SeriesLabels.Font = DefaultFont;
			ChartControl.Chart.Axis.X.Labels.SeriesLabels.FontColor = DefaultColor;
			ChartControl.Chart.Axis.X.Labels.SeriesLabels.Orientation = TextOrientation.Horizontal;
			ChartControl.Chart.Axis.X.Labels.SeriesLabels.LabelStyle.Dy = 3;
			
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
			Table.InvertRowsOrder();
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
						item.rect.Width = item.rect.Width - (int)Math.Round(space * 2 / 6);
						if (item.DataPoint.Label.ToLower().Contains("спортивные залы"))
						{
							item.rect.X = item.rect.X + (int)Math.Round(space * 3 / 6);
						}
						if (item.DataPoint.Label.ToLower().Contains("плоскостные сооружения"))
						{
							item.rect.X = item.rect.X + (int)Math.Round(space * 1 / 6);
						}
						if (item.DataPoint.Label.ToLower().Contains("плавательные бассейны"))
						{
							item.rect.X = item.rect.X - (int)Math.Round(space * 1 / 6);
						}

						item.DataPoint.Label = String.Format(
							"&nbsp;{0}&nbsp;<br />&nbsp;{1} год&nbsp;<br />&nbsp;<b>{2}</b>&nbsp;",
							item.DataPoint.Label,
							item.Series.Label,
							String.Format("{0:P2}", item.Value));
					}

				}

			}
		}

	}

	public class GridHelper : UltraGridWrapper
	{
		public Sport_0001_0011 Page { set; get; }
		public decimal AvgValue { set; get; }
		public decimal NormValue { set; get; }
		public string Measure { set; get; }
		public Color Color { set; get; }
		public OrderedData Data { set; get; }
		private Dictionary<string, OrderedData> ranks;

		public GridHelper(UltraGridBrick gridBrick) : base(gridBrick)
		{
			ranks = new Dictionary<string, OrderedData>();
			
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
			Table.Columns.Add(new DataColumn("norm", typeof(string)));
			Table.Columns.Add(new DataColumn("delta", typeof(string)));
			
			ranks.Add("norm", new OrderedData{ Precision = 4 });
			ranks.Add("delta", new OrderedData { Precision = 4 });

			decimal prevNorm = 0;
			DataRow prevRow = null;
			foreach (OrderedValue value in Data.Data)
			{
				string id = value.ID;
				if (value.IsEmpty || value.ExtraValue == 0)
					continue;
				
				DataRow row = Table.NewRow();

				row[0] = id;
				row[1] = value.Value;
				row[2] = 10 * AvgValue * value.Value / value.ExtraValue;
				decimal norm = (decimal)row[2] / NormValue;
				row[3] = norm.ToString("P2");
				if (prevRow != null)
				{
					prevRow[4] = prevNorm - norm;
					ranks["delta"].Add(new OrderedValue((string)prevRow[0], prevNorm - norm));
				}
				Table.Rows.Add(row);
				
				prevRow = row;
				prevNorm = norm;

				ranks["norm"].Add(new OrderedValue(id, norm));
			}

			// ранги
			ranks["norm"].Sort();
			ranks["delta"].Sort();
			
			GridBrick.DataTable = Table;
		}
		
		protected override void SetDataStyle()
		{
			Band.Columns[0].CellStyle.Wrap = true;
			Band.Columns[0].Width = 100;
			Band.Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Center;

			Band.HideColumns(HiddenColumns);

			Band.Columns[1].Width = 130;
			Band.Columns[2].Width = 200;
			Band.Columns[3].Width = 150;
			Band.Columns[4].Width = 160;

			CRHelper.FormatNumberColumn(Band.Columns[1], "N0");
			CRHelper.FormatNumberColumn(Band.Columns[2], "N2");
			CRHelper.FormatNumberColumn(Band.Columns[3], "P2");

			for (int i = 1; i <= 4; i++)
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
			string id = Table.Rows[rowIndex][0].ToString();

			row.Style.Height = 34;
			row.Cells[0].Style.BorderDetails.WidthRight = 3;
			row.Cells[1].Style.BorderDetails.WidthRight = 3;
			row.Cells[2].Style.BorderDetails.WidthRight = 3;

			// стрелки
			foreach (int columnIndex in (new[] { 1 }))
			{
				if (rowIndex == Table.Rows.Count - 1)
					continue;

				if (CRHelper.DBValueIsEmpty(Table.Rows[rowIndex][columnIndex])
					|| CRHelper.DBValueIsEmpty(Table.Rows[rowIndex + 1][columnIndex]))
					continue;

				UltraGridCell cell = row.Cells[columnIndex];
				decimal value1 = CRHelper.DBValueConvertToDecimalOrZero(Table.Rows[rowIndex][columnIndex]);
				decimal value2 = CRHelper.DBValueConvertToDecimalOrZero(Table.Rows[rowIndex + 1][columnIndex]);

				const int precision = 0;
				if (value1.CompareTo(value2, precision) > 0)
				{
					cell.Style.BackgroundImage = Helper.UpArrowFile();
				}
				else if (value1.CompareTo(value2, precision) < 0)
				{
					cell.Style.BackgroundImage = Helper.DnArrowFile();
				}
				if (!value1.EqualsTo(value2, precision))
				{
					cell.Style.CustomRules = Helper.CellIndicatorStyle(45);
				}
			}

			// диаграммы
			foreach (int columnIndex in (new[] { 3 }))
			{
				UltraGridCell cell = row.Cells[columnIndex];
				decimal value;
				if (cell.Value != null && Decimal.TryParse(cell.Value.ToString().Replace("%", String.Empty), out value))
				{
					cell.Style.BackgroundImage = String.Format("{0}/TemporaryImages/{1}", Page.TEMPORARY_URL_PREFIX, GetPieChart(value/100));
					cell.Style.CustomRules = Helper.CellIndicatorStyle(10);
				}

			}

			// звезды
			foreach (int columnIndex in (new[] { 3 }))
			{
				if (CRHelper.DBValueIsEmpty(Table.Rows[rowIndex][columnIndex])) 
					continue;
				
				OrderedValue orderedValue = ranks["norm"][id];
				if (orderedValue != null)
				{
					UltraGridCell cell = row.Cells[columnIndex];
					const int offset = 50;

					if (orderedValue.Rank == 1)
					{
						cell.Value = String.Format("<div style=\"position: relative;\">{0}{1}</div>", Helper.BestRankAbs(offset), cell.Value);
					}
					else if (orderedValue.Rank == ranks["norm"].MaxRank)
					{
						cell.Value = String.Format("<div style=\"position: relative;\">{0}{1}</div>", Helper.WorstRankAbs(offset), cell.Value);
					}

				}
			}

			// delta
			foreach (int columnIndex in (new[] { 4 }))
			{
				if (rowIndex == Table.Rows.Count - 1) 
					continue;
				if (CRHelper.DBValueIsEmpty(Table.Rows[rowIndex][columnIndex]))
					continue;

				UltraGridCell cell = row.Cells[columnIndex];
				decimal value = CRHelper.DBValueConvertToDecimalOrZero(Table.Rows[rowIndex][columnIndex]);
				
				const int precision = 4;
				if (value.CompareTo(0m, 4) > 0)
				{
					cell.Value = String.Format("+{0:P2}", Math.Abs(value));

					cell.Style.BackgroundImage = Helper.UpArrowFile();
				}
				else if (value.CompareTo(0m, precision) < 0)
				{
					cell.Value = String.Format("-{0:P2}", Math.Abs(value));
					cell.Style.BackgroundImage = Helper.DnArrowFile();
				}
				else
				{
					cell.Value = String.Format("{0:P2}", 0);
				}

				if (!value.EqualsTo(0, precision))
				{
					cell.Style.CustomRules = Helper.CellIndicatorStyle(70);
				}

				OrderedValue orderedValue = ranks["delta"][id];
				if (orderedValue != null)
				{
					const int offset = 30;
					if (orderedValue.Rank == 1)
					{
						cell.Value = String.Format("<div style=\"position: relative;\">{0}{1}</div>", Helper.BestRankAbs(offset), cell.Value);
					}
					else if (orderedValue.Rank == ranks["delta"].MaxRank)
					{
						cell.Value = String.Format("<div style=\"position: relative;\">{0}{1}</div>", Helper.WorstRankAbs(offset), cell.Value);
					}
				}
			}
			
		}

		protected override void SetDataHeader()
		{
			GridHeaderLayout headerLayout = GridBrick.GridHeaderLayout;

			headerLayout.AddCell("Год");
			headerLayout.AddCell("Количество");
			headerLayout.AddCell(String.Format("Обеспеченность<br />({0})", Measure));
			headerLayout.AddCell("% от<br />норматива");
			headerLayout.AddCell("Прирост/<br />снижение");

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