using System;
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
	/// детализация РФ,ФО - кадры (сравнение территорий)
	/// </summary>
    public partial class Sport_0001_0007 : CustomReportPage
    {
    	public string TEMPORARY_URL_PREFIX = "../../..";
		public string REPORT_ID = "Sport_0001_0007";
		
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

			Header.Text = "Численность работников спорта по территориям";

			DataTable msTable = DataProvider.GetDataTableForChart("Sport_0001_0007_minsport", DataProvidersFactory.PrimaryMASDataProvider);
			DataTable fmTable = DataProvider.GetDataTableForChart("Sport_0001_0007_population", DataProvidersFactory.SecondaryMASDataProvider);

			ChartStaffHelper chartStaff = new ChartStaffHelper(UltraChartStaff);
			chartStaff.TemporaryUrlPrefix = TEMPORARY_URL_PREFIX;
    		chartStaff.FoID = foID;
			chartStaff.FmTable = fmTable;
			chartStaff.MsTable = msTable;
			chartStaff.SetStyleAndData();

			ChartStaffSporterHelper chartStaffSporter = new ChartStaffSporterHelper(UltraChartStaffSporter);
			chartStaffSporter.TemporaryUrlPrefix = TEMPORARY_URL_PREFIX;
			chartStaffSporter.FoID = foID;
			chartStaffSporter.MsTable = msTable;
			chartStaffSporter.SetStyleAndData();

			GridHelper gridHelper = new GridHelper(GridBrick);
    		gridHelper.FoID = foID;
			gridHelper.SetStyleAndData();
			
        }
		
		
    }

	public class ChartStaffHelper : ChartBoxesWrapper
	{
		public int FoID { set; get; }
		public DataTable MsTable { set; get; }
		public DataTable FmTable { set; get; }
		
		public ChartStaffHelper(UltraChartItem chartItem)
			: base(chartItem)
		{

		}

		public new void SetStyleAndData()
		{
			// получение данных

			Table = new DataTable();
			Table.Columns.Add(new DataColumn("title", typeof(string)));
			Table.Columns.Add(new DataColumn("staff", typeof(double)) { Caption = "Работники спорта на 10 тыс. населения" });

			double maxValue = 0;
			for (int i = 0; i < MsTable.Rows.Count; i++)
			{
				string id = MsTable.Rows[i][0].ToString();
				object vStaff = MsTable.Rows[i][1];
				object vPopulation = FmTable.FindValue(id, 1);

				if (CRHelper.DBValueIsEmpty(vStaff) || CRHelper.DBValueIsEmpty(vPopulation))
					continue;

				DataRow row = Table.NewRow();
				row[0] = RegionsNamingHelper.ShortName(id);
				row[1] = 10000 * CRHelper.DBValueConvertToDoubleOrZero(vStaff) / CRHelper.DBValueConvertToDoubleOrZero(vPopulation);
				Table.Rows.Add(row); 

				if (maxValue < (double)row[1]) maxValue = (double)row[1];
			}
			
			// настройка общего стиля
			TitleTop = "Работники спорта\nна 10 тыс. населения";
			LabelType = ItemLabelType.Area;
			ColorBox = Color.FromArgb(255, 44, 179, 44);
			base.SetStyle();

			// настройка индивидуального стиля
			ChartControl.Width = 250;
			
			// привязка данных
			Table.InvertRowsOrder();
			NumericSeries series = CRHelper.GetNumericSeries(1, Table);
			series.Label = Table.Columns[1].Caption;
			ChartControl.Chart.Series.Add(series);
		}
		
	}

	public class ChartStaffSporterHelper : ChartBoxesWrapper
	{
		public int FoID { set; get; }
		public DataTable MsTable { set; get; }
		public DataTable FmTable { set; get; }

		public ChartStaffSporterHelper(UltraChartItem chartItem)
			: base(chartItem)
		{

		}

		public new void SetStyleAndData()
		{
			// получение данных

			Table = new DataTable();
			Table.Columns.Add(new DataColumn("title", typeof(string)));
			Table.Columns.Add(new DataColumn("value", typeof(double)) { Caption = "Занимающиеся на одного работника спорта" });
			
			for (int i = 0; i < MsTable.Rows.Count; i++)
			{
				string id = MsTable.Rows[i][0].ToString();
				object vStaff = MsTable.Rows[i][1];
				object vSporter = MsTable.Rows[i][2];

				if (CRHelper.DBValueIsEmpty(vSporter) || CRHelper.DBValueIsEmpty(vStaff))
					continue;

				DataRow row = Table.NewRow();
				row[0] = RegionsNamingHelper.ShortName(id);
				row[1] = CRHelper.DBValueConvertToDoubleOrZero(vSporter) / CRHelper.DBValueConvertToDoubleOrZero(vStaff);
				Table.Rows.Add(row);
			}
			
			// настройка общего стиля
			TitleTop = "Занимающиеся на одного\nработника спорта";
			LabelType = ItemLabelType.Area;
			ColorBox = Color.DodgerBlue;
			base.SetStyle();

			// настройка индивидуального стиля
			ChartControl.Width = 480;
			
			// привязка данных
			Table.InvertRowsOrder();
			NumericSeries series = CRHelper.GetNumericSeries(1, Table);
			series.Label = Table.Columns[1].Caption;
			ChartControl.Chart.Series.Add(series);
		}

	}

	public class GridHelper : UltraGridWrapper
	{
		public int FoID { set; get; }

		private Collection<string> IDs { set; get; }
		private decimal maxSporter;
		private decimal minSporter;
		
		public GridHelper(UltraGridBrick gridBrick) : base(gridBrick)
		{
			IDs = new Collection<string>();
			maxSporter = Decimal.MinValue;
			minSporter = Decimal.MaxValue;
		}

		protected override void SetStyle()
		{
			GridBrick.BrowserSizeAdapting = false;
			GridBrick.AutoSizeStyle = GridAutoSizeStyle.Auto;
			GridBrick.RedNegativeColoring = false;
		}

		protected override void SetData()
		{
			DataTable msTable = DataProvider.GetDataTableForChart("Sport_0001_0007_minsport_grid", DataProvidersFactory.PrimaryMASDataProvider);
			DataTable fmTable = DataProvider.GetDataTableForChart("Sport_0001_0007_population_grid", DataProvidersFactory.SecondaryMASDataProvider);

			Table = new DataTable();
			Table.Columns.Add(new DataColumn("title", typeof(string)));
			Table.Columns.Add(new DataColumn("staff_abs", typeof(decimal)));
			Table.Columns.Add(new DataColumn("staff_norm", typeof(decimal)));
			Table.Columns.Add(new DataColumn("staff_rank", typeof(int)));
			Table.Columns.Add(new DataColumn("sporter_staff", typeof(string)));

			Table.Columns.Add(new DataColumn("prev_staff_abs", typeof(decimal)));
			Table.Columns.Add(new DataColumn("prev_sporter_staff", typeof(decimal)));
			HiddenColumns = 2;

			OrderedData dataRank = new OrderedData();
			for (int i = 0; i < msTable.Rows.Count; i++)
			{
				string id = msTable.Rows[i][0].ToString();
				object rawStaff = msTable.Rows[i][1];
				object rawSporter = msTable.Rows[i][2];
				object rawPopulation = fmTable.Rows[i][1];

				if (CRHelper.DBValueIsEmpty(rawStaff) || CRHelper.DBValueIsEmpty(rawSporter) || CRHelper.DBValueIsEmpty(rawPopulation))
					continue;
				if (FoID > 0 && RegionsNamingHelper.IsFO(id))
					continue;

				IDs.Add(id);
				decimal vStaff = CRHelper.DBValueConvertToDecimalOrZero(rawStaff);
				decimal vSporter = CRHelper.DBValueConvertToDecimalOrZero(rawSporter);
				decimal vPopulation = CRHelper.DBValueConvertToDecimalOrZero(rawPopulation);

				DataRow row = Table.NewRow();
				
				if (RegionsNamingHelper.IsRF(id))
					row[0] = String.Format("<a href=\"webcommand?showReport=Sport_0001_0001_fo=0\">{0}</a>", id);
				else if (RegionsNamingHelper.IsFO(id))
					row[0] = String.Format("<a href=\"webcommand?showReport=Sport_0001_0001_fo={0}\">{1}</a>", CustomParams.GetFOIdByName(id), id);
				else if (RegionsNamingHelper.IsSubject(id))
					row[0] = String.Format("<a href=\"webcommand?showReport=Sport_0001_0002_{0}\">{1}</a>", CustomParams.GetSubjectIdByName(id), id);

				row[1] = vStaff;
				row[2] = 10000 * vStaff / vPopulation;
				row[4] = Convert.ToDecimal(vSporter / vStaff).ToString("N2");

				// сравнение с предыд годом
				rawStaff = msTable.Rows[i][3];
				rawSporter = msTable.Rows[i][4];
				rawPopulation = fmTable.Rows[i][2];
				if (!CRHelper.DBValueIsEmpty(rawPopulation) && !CRHelper.DBValueIsEmpty(rawStaff) && !CRHelper.DBValueIsEmpty(rawSporter))
				{
					vStaff = CRHelper.DBValueConvertToDecimalOrZero(rawStaff);
					vSporter = CRHelper.DBValueConvertToDecimalOrZero(rawSporter);
					//vPopulation = CRHelper.DBValueConvertToDecimalOrZero(rawPopulation);

					row[5] = vStaff;
					row[6] = vSporter / vStaff;
				}
				else
				{
					row[5] = DBNull.Value;
					row[6] = DBNull.Value;
				}

				Table.Rows.Add(row);

				if (RegionsNamingHelper.IsSubject(id))
				{
					dataRank.Add(new OrderedValue(id, Convert.ToDecimal(row[2])));
				}

				if (maxSporter < Convert.ToDecimal(row[4])) maxSporter = Convert.ToDecimal(row[4]);
				if (minSporter > Convert.ToDecimal(row[4])) minSporter = Convert.ToDecimal(row[4]);
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
						row[3] = DBNull.Value;
					else
						row[3] = rank == dataRank.MaxRank ? -rank : rank;
				}
			}
			GridBrick.DataTable = Table;
		}
		
		protected override void SetDataStyle()
		{
			Band.Columns[0].CellStyle.Wrap = true;
			Band.Columns[0].Width = 330;
			Band.Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;

			Band.HideColumns(HiddenColumns);

			Band.Columns[1].Width = 100;
			Band.Columns[2].Width = 100;
			Band.Columns[3].Width = 50;
			Band.Columns[4].Width = 160;
			
			CRHelper.FormatNumberColumn(Band.Columns[1], "N0");
			CRHelper.FormatNumberColumn(Band.Columns[2], "N2");

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

			row.Cells[0].Style.BorderDetails.WidthRight = 3;
			row.Cells[3].Style.BorderDetails.WidthRight = 3;

			string id = IDs[row.DataSourceIndex];

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
			
			// ранги
			foreach (int columnIndex in (new[] { 3 }))
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

			// max / min
			foreach (int columnIndex in (new[] { 4 }))
			{
				UltraGridCell cell = row.Cells[columnIndex];

				decimal value;
				if (cell.Value != null && Decimal.TryParse(cell.Value.ToString(), out value))
				{
					int offset = 15; 
					if (value.EqualsTo(maxSporter, 2))
					{
						cell.Value = String.Format("<div style=\"position: relative;\">{0}{1}</div>", Helper.MaxAbs(offset, -2), cell.Value);
					}
					if (value.EqualsTo(minSporter, 2))
					{
						cell.Value = String.Format("<div style=\"position: relative;\">{0}{1}</div>", Helper.MinAbs(offset, -2), cell.Value);
					}
					
				}
			}

			// стрелки
			foreach (int columnIndex in (new[] { 1, 4 }))
			{
				int columnCompare = columnIndex == 1 ? 5 : columnIndex == 4 ? 6 : 0;
				if (CRHelper.DBValueIsEmpty(Table.Rows[rowIndex][columnIndex]) 
					|| CRHelper.DBValueIsEmpty(Table.Rows[rowIndex][columnCompare]))
					continue;

				UltraGridCell cell = row.Cells[columnIndex];

				decimal value1 = CRHelper.DBValueConvertToDecimalOrZero(Table.Rows[rowIndex][columnIndex]);
				decimal value2 = CRHelper.DBValueConvertToDecimalOrZero(Table.Rows[rowIndex][columnCompare]);

				if (value1.CompareTo(value2, 2) > 0)
				{
					cell.Style.BackgroundImage = Helper.UpArrowFile(columnIndex == 4);
				}
				else if (value1.CompareTo(value2, 2) < 0)
				{
					cell.Style.BackgroundImage = Helper.DnArrowFile(columnIndex == 4);
				}
				if (!value1.EqualsTo(value2, 2))
				{
					switch (columnIndex)
					{
						case 1:
							cell.Style.CustomRules = Helper.CellIndicatorStyle(10);
							break;
						case 4:
							cell.Style.CustomRules = Helper.CellIndicatorStyle(70);
							break;
					}
				}
			}

		}

		protected override void SetDataHeader()
		{
			GridHeaderLayout headerLayout = GridBrick.GridHeaderLayout;
			GridHeaderCell group;

			headerLayout.AddCell("Территория");
			group = headerLayout.AddCell("Работники спорта");
			group.AddCell("абс.");
			group.AddCell("на 10 тыс. населения");
			group.AddCell("Ранг");
			headerLayout.AddCell("Занимающиеся на одного работника спорта");
			
			
			GridBrick.GridHeaderLayout.ApplyHeaderInfo();
		}
	}
}