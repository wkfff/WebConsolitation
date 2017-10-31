using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;
using Infragistics.UltraChart.Core.Layers;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Common.GridIndicatorRules;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Krista.FM.Server.Dashboards.iPadBricks.Sport_0001;
using Krista.FM.Server.Dashboards.SgmSupport;
using OrderedData = Krista.FM.Server.Dashboards.iPadBricks.Sport_0001.OrderedData;
using OrderedValue = Krista.FM.Server.Dashboards.iPadBricks.Sport_0001.OrderedValue;

namespace Krista.FM.Server.Dashboards.reports.iPhone
{
	/// <summary>
	/// Отчетность Минспорта (по 6 показателям) #20203
	/// детализация ФО - занимающиеся, сравнение территорий
	/// </summary>
    public partial class Sport_0001_0004 : CustomReportPage
    {
    	public string TEMPORARY_URL_PREFIX = "../../..";
		public string REPORT_ID = "Sport_0001_0004";

		// параметры запросов
		public CustomParam paramYearLast;
		public CustomParam paramColumn;
		public CustomParam paramRow;
		public CustomParam paramTerritory;


		protected override void Page_PreLoad(object sender, EventArgs e)
		{
			base.Page_PreLoad(sender, e);

			// Инициализация параметров запроса
			paramYearLast = UserParams.CustomParam("param_year_last");
			paramColumn = UserParams.CustomParam("param_column");
			paramRow = UserParams.CustomParam("param_row");
			paramTerritory = UserParams.CustomParam("param_territory");
		}

    	protected override void Page_Load(object sender, EventArgs e)
        {
			base.Page_Load(sender, e);

			// параметры запроса
			
			paramRow.Value = "1";
			paramColumn.Value = "13";
			paramYearLast.Value = Convert.ToString(Helper.YEAR_LAST);

    		paramTerritory.Value = "[Российская  Федерация].[Центральный федеральный округ]";
			if (Session["CurrentFOID"] != null)
			{
				int foID = Convert.ToInt32(Session["CurrentFOID"].ToString());
				if (foID > 0)
				{
					paramTerritory.Value = String.Format("[Российская  Федерация].[{0}]", UserParams.FullRegionName.Value);
				}
			}

    		DataTable incomeTable = DataProvider.GetDataTableForChart("Sport_0001_0004_income", DataProvidersFactory.SecondaryMASDataProvider);
			SGMEasy diseases = new SGMEasy(Helper.DISEASES, Helper.YEAR_LAST);
    		OrderedData sporterPopulation = FillSporterOrderedData();

			HeaderIncome.Text = "Спорт и доходы населения по территориям";
			ChartIncomeHelper chartIncome = new ChartIncomeHelper(UltraChartIncome);
    		chartIncome.TemporaryUrlPrefix = TEMPORARY_URL_PREFIX;
    		chartIncome.SporterPopulation = sporterPopulation;
    		chartIncome.IncomeTable = incomeTable;
			chartIncome.SetStyleAndData();

			HeaderDisease.Text = "Спорт и заболеваемость по территориям";
			ChartDiseaseHelper chartDisease = new ChartDiseaseHelper(UltraChartDisease);
    		chartDisease.TemporaryUrlPrefix = TEMPORARY_URL_PREFIX;
			chartDisease.SporterPopulation = sporterPopulation;
    		chartDisease.DiseaseData = diseases;
			chartDisease.SetStyleAndData();

			GridHelper gridHelper = new GridHelper(GridBrick);
    		gridHelper.Page = this;
    		gridHelper.SporterPopulation = sporterPopulation;
			gridHelper.DiseaseData = diseases;
    		gridHelper.IncomeTable = incomeTable;
			gridHelper.SetStyleAndData();
			
        }
		
		/// <summary>
		/// доля населения, занимающегося спортом
		/// </summary>
		private OrderedData FillSporterOrderedData()
		{
			DataTable msTable = DataProvider.GetDataTableForChart("Sport_0001_0004_minsport", DataProvidersFactory.PrimaryMASDataProvider);
			DataTable fmTable = DataProvider.GetDataTableForChart("Sport_0001_0004_population", DataProvidersFactory.SecondaryMASDataProvider);

			OrderedData orderedData = new OrderedData();
			for (int i = 0; i < msTable.Rows.Count; i++)
			{
				string id = msTable.Rows[i][0].ToString();
				object msValue = msTable.Rows[i][1];
				object fmValue = fmTable.Rows[i][1];
				if (!CRHelper.DBValueIsEmpty(msValue) && !CRHelper.DBValueIsEmpty(fmValue))
				{
					double value = CRHelper.DBValueConvertToDoubleOrZero(msValue) / CRHelper.DBValueConvertToDoubleOrZero(fmValue);
					orderedData.Add(new OrderedValue(id, value, CRHelper.DBValueConvertToDoubleOrZero(fmValue)));
				}
				else
				{
					orderedData.Add(new OrderedValue(id));
				}
			}
			
			return orderedData;
		}
		
		
    }

	public class ChartIncomeHelper : CompositeChartWrapper
	{
		public OrderedData SporterPopulation { set; get; }
		public DataTable IncomeTable { set; get; }

		private string ToolTipFormatBox { set; get; }
		private string ToolTipFormatLine { set; get; }

		public ChartIncomeHelper(UltraChartItem chartItem)
			: base(chartItem)
		{
			
		}

		public new void SetStyleAndData()
		{
			// получение данных

			Table = new DataTable();
			Table.Columns.Add(new DataColumn("title", typeof(string)));
			Table.Columns.Add(new DataColumn("column1", typeof(double)) { Caption = "Доля населения, занимающегося спортом" });
			Table.Columns.Add(new DataColumn("column2", typeof(double)) { Caption = "Среднедушевой доход населения" });
			
			foreach (OrderedValue value in SporterPopulation.Data)
			{
				if (value.IsEmpty)
					continue;
				DataRow row = Table.NewRow();
				row[0] = RegionsNamingHelper.ShortName(value.ID);
				row[1] = value.Value;
				row[2] = IncomeTable.FindValue(value.ID, 1);
				Table.Rows.Add(row);
			}
			
			// настройка общего стиля
			
			X_Extent = 120;
			X_FormatString = "<ITEM_LABEL>";
			Y_Extent = 60;
			Y_FormatString = "<DATA_VALUE:P0>";
			Y2_Extent = 90;
			Y2_FormatString = "<DATA_VALUE:N2>";
			Text1_FormatString = String.Empty;
			Text2_FormatString = String.Empty;//"   <DATA_VALUE:N2>\n\n";
			ToolTipFormatString = "<ITEM_LABEL>";
			ToolTipFormatBox = "{0:P2}";
			ToolTipFormatLine = "{0:N2}";
			TitleLeft = "Доля населения, занимающегося спортом";
			TitleRight = "Среднедушевой доход населения, руб.";
			LegendBounds = new Rectangle(10, 88, 75, 12);
			IconColor = Color.FromArgb(unchecked((int)0xfffebe2e));

			double margin = 100 / Table.Rows.Count / 2;
			LeftMarginPercent = margin;
			RightMarginPercent = margin;
			
			base.SetStyle();
			
			// настройка индивидуального стиля
			
			double maxValue1 = 0;
			double maxValue2 = 0;
			foreach (DataRow row in Table.Rows)
			{
				if (maxValue1 < CRHelper.DBValueConvertToDoubleOrZero(row[1]))
				{
					maxValue1 = CRHelper.DBValueConvertToDoubleOrZero(row[1]);
				}
				if (maxValue2 < CRHelper.DBValueConvertToDoubleOrZero(row[2]))
				{
					maxValue2 = CRHelper.DBValueConvertToDoubleOrZero(row[2]);
				}
			}
			maxValue1 = Math.Ceiling((maxValue1 * 1.1) * 10) / 10;
			double zeros = Math.Pow(10, Math.Floor(Math.Log10(maxValue2)));
			maxValue2 = Math.Ceiling((maxValue2 * 1.1) / zeros) * zeros;
			
			AxisItem axisY = ChartControl.Chart.CompositeChart.ChartAreas[0].Axes.FromKey("Y");
			axisY.RangeMin = 0;
			axisY.RangeMax = maxValue1;

			AxisItem axisY2 = ChartControl.Chart.CompositeChart.ChartAreas[0].Axes.FromKey("Y2");
			axisY2.RangeMin = 0;
			axisY2.RangeMax = maxValue2;

			AxisItem axisX = ChartControl.Chart.CompositeChart.ChartAreas[0].Axes.FromKey("X");
			axisX.Labels.Orientation = TextOrientation.VerticalLeftFacing;
			
			ChartControl.Chart.TitleLeft.Margins.Bottom = 40;
			ChartControl.Chart.TitleRight.Margins.Bottom = 70;
			
			// привязка данных

			NumericSeries series;
			PaintElement pe;

			// занимающиеся
			series = CRHelper.GetNumericSeries(1, Table);
			series.Label = Table.Columns[1].Caption;
			series.PEs.Clear();
			pe = new PaintElement();
			pe.ElementType = PaintElementType.SolidFill;
			pe.Fill = Color.FromArgb(50, Color.ForestGreen);
			pe.FillOpacity = 100;
			series.PEs.Add(pe);
			ChartControl.Chart.CompositeChart.Series.Add(series);
			ChartControl.Chart.CompositeChart.ChartLayers[0].Series.Add(series);

			// больные
			series = CRHelper.GetNumericSeries(2, Table);
			series.Label = Table.Columns[2].Caption;
			series.PEs.Clear();
			pe = new PaintElement();
			pe.ElementType = PaintElementType.SolidFill;
			pe.Fill = Color.Transparent;
			pe.FillOpacity = 0;
			series.PEs.Add(pe);
			ChartControl.Chart.CompositeChart.Series.Add(series);
			ChartControl.Chart.CompositeChart.ChartLayers[1].Series.Add(series);
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
						item.DataPoint.Label = String.Format(
							"&nbsp;{0}&nbsp;<br />&nbsp;{1}&nbsp;<br />&nbsp;<b>{2}</b>&nbsp;",
							item.Series.Label,
							RegionsNamingHelper.FullName(item.DataPoint.Label),
							String.Format(ToolTipFormatBox, item.Value));
					}

				}

				if (primitive is Polyline)
				{
					Polyline point = (Polyline)primitive;
					foreach (DataPoint item in point.points)
					{
						if (item.DataPoint != null)
						{
							item.DataPoint.Label = String.Format(
								"&nbsp;{0}&nbsp;<br />&nbsp;{1}&nbsp;<br />&nbsp;<b>{2}</b> руб.&nbsp;",
								item.Series.Label,
								RegionsNamingHelper.FullName(item.DataPoint.Label),
								String.Format(ToolTipFormatLine, item.Value));
						}
					}
				}
			}
		}
	}

	public class ChartDiseaseHelper : CompositeChartWrapper
	{
		public OrderedData SporterPopulation { set; get; }
		public SGMEasy DiseaseData { set; get; }

		private string ToolTipFormatBox { set; get; }
		private string ToolTipFormatLine { set; get; }

		public ChartDiseaseHelper(UltraChartItem chartItem)
			: base(chartItem)
		{
			
		}

		public new void SetStyleAndData()
		{
			// получение данных

			Table = new DataTable();
			Table.Columns.Add(new DataColumn("title", typeof(string)));
			Table.Columns.Add(new DataColumn("column1", typeof(double)) { Caption = "Доля населения, занимающегося спортом" });
			Table.Columns.Add(new DataColumn("column2", typeof(double)) { Caption = "Заболеваемость гриппом и ОРВИ на 100 тыс. населения" });
			
			foreach (OrderedValue value in SporterPopulation.Data)
			{
				if (value.IsEmpty)
					continue;
				DataRow row = Table.NewRow();
				row[0] = RegionsNamingHelper.ShortName(value.ID);
				row[1] = value.Value;
				row[2] = 100000 * DiseaseData.GetDiseaseDecimal(value.ID) / value.ExtraValue;
				Table.Rows.Add(row);
			}
			
			// настройка общего стиля
			
			X_Extent = 120;
			X_FormatString = "<ITEM_LABEL>";
			Y_Extent = 60;
			Y_FormatString = "<DATA_VALUE:P0>";
			Y2_Extent = 90;
			Y2_FormatString = "<DATA_VALUE:N0>";
			Text1_FormatString = String.Empty;
			Text2_FormatString = String.Empty;//"   <DATA_VALUE:N0>\n\n";
			ToolTipFormatString = "<ITEM_LABEL>";
			ToolTipFormatBox = "{0:P2}";
			ToolTipFormatLine = "{0:N0}";
			TitleLeft = "Доля населения, занимающегося спортом";
			TitleRight = "Заболеваемость гриппом и ОРВИ\nна 100 тыс. населения";
			LegendBounds = new Rectangle(10, 88, 75, 12);
			IconColor = Color.Red;

			double margin = 100 / Table.Rows.Count / 2;
			LeftMarginPercent = margin;
			RightMarginPercent = margin;
			
			base.SetStyle();
			
			// настройка индивидуального стиля

			double maxValue1 = 0;
			double maxValue2 = 0;
			foreach (DataRow row in Table.Rows)
			{
				if (maxValue1 < CRHelper.DBValueConvertToDoubleOrZero(row[1]))
				{
					maxValue1 = CRHelper.DBValueConvertToDoubleOrZero(row[1]);
				}
				if (maxValue2 < CRHelper.DBValueConvertToDoubleOrZero(row[2]))
				{
					maxValue2 = CRHelper.DBValueConvertToDoubleOrZero(row[2]);
				}
			}
			maxValue1 = Math.Ceiling((maxValue1 * 1.1) * 10) / 10;
			double zeros = Math.Pow(10, Math.Floor(Math.Log10(maxValue2)));
			maxValue2 = Math.Ceiling((maxValue2 * 1.1) / zeros) * zeros;

			AxisItem axisY = ChartControl.Chart.CompositeChart.ChartAreas[0].Axes.FromKey("Y");
			axisY.RangeMin = 0;
			axisY.RangeMax = maxValue1;

			AxisItem axisY2 = ChartControl.Chart.CompositeChart.ChartAreas[0].Axes.FromKey("Y2");
			axisY2.RangeMin = 0;
			axisY2.RangeMax = maxValue2;

			AxisItem axisX = ChartControl.Chart.CompositeChart.ChartAreas[0].Axes.FromKey("X");
			axisX.Labels.Orientation = TextOrientation.VerticalLeftFacing;

			ChartControl.Chart.TitleLeft.Margins.Bottom = 40;
			ChartControl.Chart.TitleRight.Margins.Bottom = 110;

			// привязка данных

			NumericSeries series;
			PaintElement pe;

			// занимающиеся
			series = CRHelper.GetNumericSeries(1, Table);
			series.Label = Table.Columns[1].Caption;
			series.PEs.Clear();
			pe = new PaintElement();
			pe.ElementType = PaintElementType.SolidFill;
			pe.Fill = Color.FromArgb(50, Color.ForestGreen);
			pe.FillOpacity = 100;
			series.PEs.Add(pe);
			ChartControl.Chart.CompositeChart.Series.Add(series);
			ChartControl.Chart.CompositeChart.ChartLayers[0].Series.Add(series);

			// больные
			series = CRHelper.GetNumericSeries(2, Table);
			series.Label = Table.Columns[2].Caption;
			series.PEs.Clear();
			pe = new PaintElement();
			pe.ElementType = PaintElementType.SolidFill;
			pe.Fill = Color.Transparent;
			pe.FillOpacity = 0;
			series.PEs.Add(pe);
			ChartControl.Chart.CompositeChart.Series.Add(series);
			ChartControl.Chart.CompositeChart.ChartLayers[1].Series.Add(series);
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
							"&nbsp;{0}&nbsp;<br />&nbsp;{1}&nbsp;<br />&nbsp;<b>{2}</b>&nbsp;",
							item.Series.Label,
							RegionsNamingHelper.FullName(item.DataPoint.Label),
							String.Format(ToolTipFormatBox, item.Value));
					}

				}

				if (primitive is Polyline)
				{
					Polyline point = (Polyline) primitive;
					foreach (DataPoint item in point.points)
					{
						if (item.DataPoint != null)
						{
							item.DataPoint.Label = String.Format(
								"&nbsp;{0}&nbsp;<br />&nbsp;{1}&nbsp;<br />&nbsp;<b>{2}</b>&nbsp;",
								item.Series.Label,
								RegionsNamingHelper.FullName(item.DataPoint.Label),
								String.Format(ToolTipFormatLine, item.Value));
						}
					}
				}
			}
		}

	}

	public class GridHelper : UltraGridWrapper
	{
		public Sport_0001_0004 Page { set; get; }
		public OrderedData SporterPopulation { set; get; }
		public SGMEasy DiseaseData { set; get; }
		public DataTable IncomeTable { set; get; }
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
			Table.Columns.Add(new DataColumn("sporter", typeof(double)));
			Table.Columns.Add(new DataColumn("rank1", typeof(string)));
			Table.Columns.Add(new DataColumn("income", typeof(double)));
			Table.Columns.Add(new DataColumn("rank2", typeof(string)));
			Table.Columns.Add(new DataColumn("diseases", typeof(double)));
			Table.Columns.Add(new DataColumn("rank3", typeof(string)));

			OrderedData data1 = new OrderedData();
			OrderedData data2 = new OrderedData();
			OrderedData data3 = new OrderedData();

			foreach (OrderedValue value in SporterPopulation.Data)
			{
				if (value.IsEmpty)
					continue;
				
				IDs.Add(value.ID);

				DataRow row = Table.NewRow();
				row[0] = String.Format("<a href=\"webcommand?showReport=Sport_0001_0002_{0}\">{1}</a>", CustomParams.GetSubjectIdByName(value.ID), value.ID); ;
				row[1] = value.Value;
				row[3] = IncomeTable.FindValue(value.ID, 1);
				row[5] = 100000 * DiseaseData.GetDiseaseDecimal(value.ID) / value.ExtraValue;
				Table.Rows.Add(row);

				data1.Add(new OrderedValue(value.ID, CRHelper.DBValueConvertToDoubleOrZero(row[1])));
				data2.Add(new OrderedValue(value.ID, CRHelper.DBValueConvertToDoubleOrZero(row[3])));
				data3.Add(new OrderedValue(value.ID, CRHelper.DBValueConvertToDoubleOrZero(row[5])));
			}
			
			data1.Sort();
			data2.Sort();
			data3.Inversed = true;
			data3.Sort();
			
			for (int i = 0; i < Table.Rows.Count; i++)
			{
				DataRow row = Table.Rows[i];
				string id = IDs[i];
				
				int rank = data1.GetRank(id);
				row[2] = rank == data1.MaxRank ? -rank : rank;
				rank = data2.GetRank(id);
				row[4] = rank == data2.MaxRank ? -rank : rank;
				rank = data3.GetRank(id);
				row[6] = rank == data3.MaxRank ? -rank : rank;
			}

			GridBrick.DataTable = Table;
			
		}
		
		protected override void SetDataStyle()
		{
			Band.Columns[0].CellStyle.Wrap = true;
			Band.Columns[0].Width = 185;
			Band.Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;

			Band.HideColumns(HiddenColumns);

			Band.Columns[1].Width = 120;
			Band.Columns[2].Width = 50;
			Band.Columns[3].Width = 140;
			Band.Columns[4].Width = 50;
			Band.Columns[5].Width = 140;
			Band.Columns[6].Width = 50;
			
			Band.Columns[2].CellStyle.Padding.Right = 5;
			Band.Columns[4].CellStyle.Padding.Right = 5;
			Band.Columns[6].CellStyle.Padding.Right = 5;

			CRHelper.FormatNumberColumn(Band.Columns[1], "P2");
			CRHelper.FormatNumberColumn(Band.Columns[3], "N2");
			CRHelper.FormatNumberColumn(Band.Columns[5], "N0");
		}

		protected override void SetDataRules()
		{
			// empty here
		}

		protected override void InitializeRow(object sender, RowEventArgs e)
		{
			UltraGridRow row = e.Row;
			row.Style.Height = 34;

			row.Cells[0].Style.BorderDetails.WidthRight = 3;
			row.Cells[2].Style.BorderDetails.WidthRight = 3;
			row.Cells[4].Style.BorderDetails.WidthRight = 3;

			foreach (int columnIndex in (new[] { 1 }))
			{
				UltraGridCell cell = row.Cells[columnIndex];
				double value;
				if (cell.Value != null && Double.TryParse(cell.Value.ToString(), out value))
				{
					cell.Style.BackgroundImage = String.Format("{0}/TemporaryImages/{1}", Page.TEMPORARY_URL_PREFIX, GetPieChart(value));
					cell.Style.CustomRules = Helper.CellIndicatorStyle(10);
				}

			}

			// ранги
			foreach (int columnIndex in (new[] { 2, 4, 6 })) 
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
			headerLayout.AddCell("Занимаются спортом,<br />% населения");
			headerLayout.AddCell("Ранг");
			headerLayout.AddCell("Среднедушевой доход, руб.");
			headerLayout.AddCell("Ранг");
			headerLayout.AddCell("Заболеваемость<br />на 100 тыс. населения");
			headerLayout.AddCell("Ранг");
			
			GridBrick.GridHeaderLayout.ApplyHeaderInfo();
		}

		private string GetPieChart(double value)
		{
			SporterPieChart chart = new SporterPieChart(
				(UltraChartItem)Page.LoadControl("../../../Components/UltraChartItem.ascx"), Page.TEMPORARY_URL_PREFIX, value);
			chart.SetStyleAndData();

			string filename = String.Format("{0}_sporter_{1}.png", Page.REPORT_ID, value.ToString("N4"));
			chart.SaveTo(filename);
			return filename;
		}
		
	}
}