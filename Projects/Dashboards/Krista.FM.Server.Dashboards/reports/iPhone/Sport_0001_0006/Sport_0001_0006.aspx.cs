using System;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Events;
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
	/// детализация субъекта - занимающиеся, годовая динамика
	/// </summary>
    public partial class Sport_0001_0006 : CustomReportPage
    {
    	public string TEMPORARY_URL_PREFIX = "../../..";
		public string REPORT_ID = "Sport_0001_0006";
		
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
			paramTerritory.Value = String.Format("[Российская  Федерация].[{0}].[{1}]", UserParams.Region.Value, UserParams.StateArea.Value);
			
			DataTable incomeTable = DataProvider.GetDataTableForChart("Sport_0001_0006_income", DataProvidersFactory.SecondaryMASDataProvider);
			SGMEasy diseases = new SGMEasy(Helper.DISEASES, UserParams.StateArea.Value);
			OrderedData sporterPopulation = FillSporterOrderedData();

			HeaderIncome.Text = "Спорт и доходы населения в динамике";
			ChartIncomeHelper chartIncome = new ChartIncomeHelper(UltraChartIncome);
			chartIncome.TemporaryUrlPrefix = TEMPORARY_URL_PREFIX;
			chartIncome.SporterPopulation = sporterPopulation;
			chartIncome.IncomeTable = incomeTable;
			chartIncome.SetStyleAndData();

			HeaderDisease.Text = "Спорт и заболеваемость в динамике";
			ChartDiseaseHelper chartDisease = new ChartDiseaseHelper(UltraChartDisease);
			chartDisease.TemporaryUrlPrefix = TEMPORARY_URL_PREFIX;
			chartDisease.SporterPopulation = sporterPopulation;
			chartDisease.DiseaseData = diseases;
			chartDisease.SetStyleAndData();

			GridHelper gridHelper = new GridHelper(GridBrick);
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
			DataTable msTable = DataProvider.GetDataTableForChart("Sport_0001_0006_minsport", DataProvidersFactory.PrimaryMASDataProvider);
			DataTable fmTable = DataProvider.GetDataTableForChart("Sport_0001_0006_population", DataProvidersFactory.SecondaryMASDataProvider);
			
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
				object incomeValue = IncomeTable.FindValue(value.ID, 1);
				if (CRHelper.DBValueIsEmpty(incomeValue))
					continue;

				DataRow row = Table.NewRow();
				row[0] = value.ID;
				row[1] = value.Value;
				row[2] = CRHelper.DBValueConvertToDoubleOrZero(incomeValue);
				Table.Rows.Add(row);
			}

			if (Table.Rows.Count == 0)
			{
				ChartControl.Width = 740;
				ChartControl.Height = 100;
				ChartControl.InvalidDataColor = Color.Gainsboro;
				return;
			}

			// инфа за последние 10 лет
			while (Table.Rows.Count > 10)
			{
				Table.Columns.RemoveAt(0);
			}

			// настройка общего стиля
			
			X_Extent = 60;
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
								"&nbsp;{0}&nbsp;<br />&nbsp;{1} год&nbsp;<br />&nbsp;<b>{2}</b> руб.&nbsp;",
								item.Series.Label,
								item.DataPoint.Label,
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

			// инфа за последние 10 лет
			while (Table.Rows.Count > 10)
			{
				Table.Rows.RemoveAt(0);
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
		public DataTable IncomeTable { set; get; }

		public GridHelper(UltraGridBrick gridBrick) : base(gridBrick)
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
			Table.Columns.Add(new DataColumn("sporter", typeof(string)));
			Table.Columns.Add(new DataColumn("income", typeof(string)));
			Table.Columns.Add(new DataColumn("diseases", typeof(string)));
			Table.Columns.Add(new DataColumn("rank1", typeof(int)));
			Table.Columns.Add(new DataColumn("rank3", typeof(int)));
			Table.Columns.Add(new DataColumn("rank5", typeof(int)));
			HiddenColumns = 3;

			OrderedData data1 = new OrderedData();
			OrderedData data2 = new OrderedData();
			OrderedData data3 = new OrderedData();
			foreach (OrderedValue value in SporterPopulation.Data)
			{
				string id = value.ID;

				if (value.IsEmpty)
					continue;
				object incomeValue = IncomeTable.FindValue(value.ID, 1);
				object diseaseValue = DiseaseData.GetDiseaseDouble(value.ID);
				
				DataRow row = Table.NewRow();
				
				row[0] = id;
				
				row[1] = value.Value.ToString("P2");
				data1.Add(new OrderedValue(id, value.Value));

				if (CRHelper.DBValueIsEmpty(incomeValue))
					row[2] = DBNull.Value;
				else
				{
					decimal val = Convert.ToDecimal(incomeValue);
					row[2] = val.ToString("N2");
					data2.Add(new OrderedValue(id, val));
				}

				if (CRHelper.DBValueIsEmpty(diseaseValue))
					row[3] = DBNull.Value;
				else
				{
					decimal val = Convert.ToDecimal(100000*CRHelper.DBValueConvertToDecimalOrZero(diseaseValue)/value.ExtraValue);
					row[3] = val.ToString("N0");
					data3.Add(new OrderedValue(id, val));
				}

				Table.Rows.Add(row);
			}

			// ранги

			data1.Sort();
			data2.Sort();
			data3.Inversed = true;
			data3.Sort();

			for (int i = 0; i < Table.Rows.Count; i++)
			{
				DataRow row = Table.Rows[i];

				int rank = data1.GetRank(row[0].ToString());
				if (rank == 0)
					row[4] = DBNull.Value;
				else 
					row[4] = rank == data1.MaxRank ? -rank : rank;

				rank = data2.GetRank(row[0].ToString());
				if (rank == 0)
					row[5] = DBNull.Value;
				else
					row[5] = rank == data2.MaxRank ? -rank : rank;
				
				rank = data3.GetRank(row[0].ToString());
				if (rank == 0)
					row[6] = DBNull.Value;
				else
					row[6] = rank == data3.MaxRank ? -rank : rank;
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

			Band.Columns[1].Width = 210;
			Band.Columns[2].Width = 210;
			Band.Columns[3].Width = 210;

			Band.Columns[1].CellStyle.Padding.Right = 10;
			Band.Columns[2].CellStyle.Padding.Right = 10;
			Band.Columns[3].CellStyle.Padding.Right = 10;
		}

		protected override void SetDataRules()
		{
			// empty here
		}

		protected override void InitializeRow(object sender, RowEventArgs e)
		{
			UltraGridRow row = e.Row;
			
			// звезды
			foreach (int columnIndex in (new[] { 1, 2, 3 }))
			{
				UltraGridCell cell = row.Cells[columnIndex + 3];
				UltraGridCell cellTarget = row.Cells[columnIndex];

				int value;
				if (cell.Value != null && Int32.TryParse(cell.Value.ToString(), out value))
				{
					int offset = 0;
					switch (columnIndex)
					{
						case 1:
							offset = 80;
							break;
						case 2:
							offset = 65;
							break;
						case 3:
							offset = 85;
							break;
					}

					if (value == 1)
					{
						cellTarget.Value = String.Format("<div style=\"position: relative;\">{0}{1}</div>", Helper.BestRankAbs(offset), cellTarget.Value);
					}
					else if (value < 0)
					{
						cellTarget.Value = String.Format("<div style=\"position: relative;\">{0}{1}</div>", Helper.WorstRankAbs(offset), cellTarget.Value);
					}

				}
			}

			// стрелки
			foreach (int columnIndex in (new[] { 1, 2, 3 }))
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
				if (Table.Rows[rowIndex][columnIndex].ToString().Contains("%"))
					value1 = Decimal.Parse(Table.Rows[rowIndex][columnIndex].ToString().Replace("%", ""));
				if (Table.Rows[rowIndex+1][columnIndex].ToString().Contains("%"))
					value2 = Decimal.Parse(Table.Rows[rowIndex + 1][columnIndex].ToString().Replace("%", ""));

				if (value1.CompareTo(value2, 2) > 0)
				{
					cell.Style.BackgroundImage = Helper.UpArrowFile(columnIndex == 3);
				}
				else if (value1.CompareTo(value2, 2) < 0)
				{
					cell.Style.BackgroundImage = Helper.DnArrowFile(columnIndex == 3);
				}

				if (!value1.EqualsTo(value2, 2))
				{
					switch (columnIndex)
					{
						case 1:
							cell.Style.CustomRules = Helper.CellIndicatorStyle(115);
							break;
						case 2:
							cell.Style.CustomRules = Helper.CellIndicatorStyle(100);
							break;
						case 3:
							cell.Style.CustomRules = Helper.CellIndicatorStyle(120);
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
			headerLayout.AddCell("Среднедушевой доход, руб.");
			headerLayout.AddCell("Заболеваемость<br />на 100 тыс. населения");
			
			GridBrick.GridHeaderLayout.ApplyHeaderInfo();
		}

	}
}