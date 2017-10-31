using System;
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
using Krista.FM.Server.Dashboards.SgmSupport;
using OrderedData = Krista.FM.Server.Dashboards.iPadBricks.Sport_0001.OrderedData;
using OrderedValue = Krista.FM.Server.Dashboards.iPadBricks.Sport_0001.OrderedValue;

namespace Krista.FM.Server.Dashboards.reports.iPhone
{
	/// <summary>
	/// Отчетность Минспорта (по 6 показателям) #20203
	/// детализация РФ, ФО - занимающиеся, годовая динамика
	/// </summary>
    public partial class Sport_0001_0005 : CustomReportPage
    {
    	public string TEMPORARY_URL_PREFIX = "../../..";
		public string REPORT_ID = "Sport_0001_0005";
		
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

    		string areaName = "Российская  Федерация";
			paramTerritory.Value = "[Российская  Федерация]";
			if (Session["CurrentFOID"] != null)
			{
				int foID = Convert.ToInt32(Session["CurrentFOID"].ToString());
				if (foID > 0)
				{
					areaName = UserParams.FullRegionName.Value;
					paramTerritory.Value = String.Format("[Российская  Федерация].[{0}]", UserParams.FullRegionName.Value);
				}
			}
			
			SGMEasy diseases = new SGMEasy(Helper.DISEASES, areaName);
			OrderedData sporterPopulation = FillSporterOrderedData();
			
			HeaderDisease.Text = "Спорт и заболеваемость в динамике";
			ChartDiseaseHelper chartDisease = new ChartDiseaseHelper(UltraChartDisease);
			chartDisease.TemporaryUrlPrefix = TEMPORARY_URL_PREFIX;
			chartDisease.SporterPopulation = sporterPopulation;
			chartDisease.DiseaseData = diseases;
			chartDisease.SetStyleAndData();

			GridHelper gridHelper = new GridHelper(GridBrick);
    		gridHelper.SporterPopulation = sporterPopulation;
			gridHelper.DiseaseData = diseases;
			gridHelper.SetStyleAndData();
			
        }
		
		/// <summary>
		/// доля населения, занимающегося спортом
		/// </summary>
		private OrderedData FillSporterOrderedData()
		{
			DataTable msTable = DataProvider.GetDataTableForChart("Sport_0001_0005_minsport", DataProvidersFactory.PrimaryMASDataProvider);
			DataTable fmTable = DataProvider.GetDataTableForChart("Sport_0001_0005_population", DataProvidersFactory.SecondaryMASDataProvider);
			
			OrderedData orderedData = new OrderedData();
			for (int i = 0; i < msTable.Rows.Count; i++)
			{
				string id = msTable.Rows[i][0].ToString();
				object msValue = msTable.Rows[i][1];
				object fmValue = fmTable.FindValue(id, 1);
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
			Table.Columns.Add(new DataColumn("column2", typeof(double)) { Caption = "Заболеваемость гриппом и ОРВИ" });

			foreach (OrderedValue value in SporterPopulation.Data)
			{
				if (value.IsEmpty)
					continue;
				object diseaseValue = DiseaseData.GetDiseaseValue(value.ID);
				if (CRHelper.DBValueIsEmpty(diseaseValue))
					continue;

				DataRow row = Table.NewRow();
				row[0] = RegionsNamingHelper.ShortName(value.ID);
				row[1] = value.Value;
				row[2] = 100000 * CRHelper.DBValueConvertToDecimalOrZero(diseaseValue) / value.ExtraValue;
				Table.Rows.Add(row);
			}
			
			// настройка общего стиля
			
			X_Extent = 60;
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
			axisX.Labels.Orientation = TextOrientation.Horizontal;

			ChartControl.Chart.TitleLeft.Margins.Bottom = 40;
			ChartControl.Chart.TitleRight.Margins.Bottom = 60;

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
			pe.Fill = IconColor;
			pe.FillOpacity = 100;
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
							"&nbsp;{0}&nbsp;<br />&nbsp;{1} год&nbsp;<br />&nbsp;<b>{2}</b>&nbsp;",
							item.Series.Label,
							item.DataPoint.Label,
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
								"&nbsp;{0}&nbsp;<br />&nbsp;{1} год&nbsp;<br />&nbsp;<b>{2}</b>&nbsp;",
								item.Series.Label,
								item.DataPoint.Label,
								String.Format(ToolTipFormatLine, item.Value));
						}
					}
				}
			}
		}
	}

	public class GridHelper : UltraGridWrapper
	{
		public OrderedData SporterPopulation { set; get; }
		public SGMEasy DiseaseData { set; get; }

		public GridHelper(UltraGridBrick gridBrick) 
			: base(gridBrick)
		{

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
			Table.Columns.Add(new DataColumn("rank3", typeof(string)));
			
			foreach (OrderedValue value in SporterPopulation.Data)
			{
				if (value.IsEmpty)
					continue;
				object diseaseValue = DiseaseData.GetDiseaseDouble(value.ID);
				
				DataRow row = Table.NewRow();
				row[0] = value.ID.Replace("федеральный округ", "ФО");
				row[1] = value.Value;
				
				if (CRHelper.DBValueIsEmpty(diseaseValue))
					row[3] = DBNull.Value;
				else
					row[3] = 100000 * CRHelper.DBValueConvertToDecimalOrZero(diseaseValue) / value.ExtraValue;

				Table.Rows.Add(row);
			}

			// ранги

			OrderedData data1 = new OrderedData(Table, 1);
			data1.Sort();
			OrderedData data2 = new OrderedData(Table, 3);
			data2.Inversed = true;
			data2.Sort();
			for (int i = 0; i < Table.Rows.Count; i++)
			{
				DataRow row = Table.Rows[i];

				int rank = data1.GetRank(row[0].ToString());
				if (rank == 0)
					row[2] = DBNull.Value;
				else 
					row[2] = rank == data1.MaxRank ? -rank : rank;

				rank = data2.GetRank(row[0].ToString());
				if (rank == 0)
					row[4] = DBNull.Value;
				else
					row[4] = rank == data2.MaxRank ? -rank : rank;
			}
			
			Table.InvertRowsOrder();
			GridBrick.DataTable = Table;
			
		}
		
		protected override void SetDataStyle()
		{
			Band.Columns[0].CellStyle.Wrap = true;
			Band.Columns[0].Width = 100;
			Band.Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Center;
			Band.Columns[0].CellStyle.Padding.Left = 0;

			Band.HideColumns(HiddenColumns);

			Band.Columns[1].Width = 270;
			Band.Columns[2].Width = 50;
			Band.Columns[3].Width = 270;
			Band.Columns[4].Width = 50;

			Band.Columns[2].CellStyle.Padding.Right = 5;
			Band.Columns[4].CellStyle.Padding.Right = 5;
			
			CRHelper.FormatNumberColumn(Band.Columns[1], "P2");
			CRHelper.FormatNumberColumn(Band.Columns[3], "N0");
		}

		protected override void SetDataRules()
		{
			// empty here
		}

		protected override void InitializeRow(object sender, RowEventArgs e)
		{
			UltraGridRow row = e.Row;

			row.Cells[0].Style.BorderDetails.WidthRight = 3;
			row.Cells[2].Style.BorderDetails.WidthRight = 3;

			// звезды
			foreach (int columnIndex in (new[] { 2, 4 }))
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

			// стрелки
			foreach (int columnIndex in (new[] { 1, 3 }))
			{
				int rowIndex = row.DataSourceIndex;
				
				if (rowIndex == Table.Rows.Count-1)
					continue;

				if (CRHelper.DBValueIsEmpty(Table.Rows[rowIndex][columnIndex])
					|| CRHelper.DBValueIsEmpty(Table.Rows[rowIndex + 1][columnIndex]))
					continue;

				UltraGridCell cell = row.Cells[columnIndex];
				decimal value1 = CRHelper.DBValueConvertToDecimalOrZero(Table.Rows[rowIndex][columnIndex]);
				decimal value2 = CRHelper.DBValueConvertToDecimalOrZero(Table.Rows[rowIndex + 1][columnIndex]);

				if (value1.CompareTo(value2, 4) > 0)
				{
					cell.Style.BackgroundImage = Helper.UpArrowFile(columnIndex == 3);
				}
				else if (value1.CompareTo(value2, 4) < 0)
				{
					cell.Style.BackgroundImage = Helper.DnArrowFile(columnIndex == 3);
				}
				if (!value1.EqualsTo(value2, 4))
				{
					switch (columnIndex)
					{
						case 1:
							cell.Style.CustomRules = Helper.CellIndicatorStyle(175);
							break;
						case 3:
							cell.Style.CustomRules = Helper.CellIndicatorStyle(180);
							break;
					}
				}
			}
		}

		protected override void SetDataHeader()
		{
			GridHeaderLayout headerLayout = GridBrick.GridHeaderLayout;
			
			headerLayout.AddCell("Год");
			headerLayout.AddCell("Занимаются спортом,<br />% населения");
			headerLayout.AddCell("Ранг");
			headerLayout.AddCell("Заболеваемость<br />на 100 тыс. населения");
			headerLayout.AddCell("Ранг");
			
			GridBrick.GridHeaderLayout.ApplyHeaderInfo();
		}

	}
}