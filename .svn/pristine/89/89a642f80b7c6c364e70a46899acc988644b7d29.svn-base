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
	/// детализация РФ, ФО - кадры (годовая динамика)
	/// </summary>
    public partial class Sport_0001_0008 : CustomReportPage
    {
    	public string TEMPORARY_URL_PREFIX = "../../..";
		public string REPORT_ID = "Sport_0001_0008";
		
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

			Header.Text = "Численность работников спорта в динамике";

			DataTable msTable = DataProvider.GetDataTableForChart("Sport_0001_0008_minsport", DataProvidersFactory.PrimaryMASDataProvider);
			DataTable fmTable = DataProvider.GetDataTableForChart("Sport_0001_0008_population", DataProvidersFactory.SecondaryMASDataProvider);
			
			ChartStaffHelper chartStaff = new ChartStaffHelper(UltraChartStaff);
			chartStaff.TemporaryUrlPrefix = TEMPORARY_URL_PREFIX;
    		chartStaff.FmTable = fmTable;
    		chartStaff.MsTable = msTable;
			chartStaff.SetStyleAndData();
			
			ChartStaffSporterHelper chartStaffSporter = new ChartStaffSporterHelper(UltraChartStaffSporter);
			chartStaffSporter.TemporaryUrlPrefix = TEMPORARY_URL_PREFIX;
			chartStaffSporter.MsTable = msTable;
			chartStaffSporter.SetStyleAndData();

			GridHelper gridHelper = new GridHelper(GridBrick);
			gridHelper.FmTable = fmTable;
			gridHelper.MsTable = msTable;
			gridHelper.SetStyleAndData();
			
        }
		
    }

	public class ChartStaffHelper : ChartBoxesWrapper
	{
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
			Table.Columns.Add(new DataColumn("value", typeof(double)) { Caption = "Работники спорта на 10 тыс. населения" });
			
			double maxValue = 0;
			for (int i = 0; i < MsTable.Rows.Count; i++)
			{
				string id = MsTable.Rows[i][0].ToString();
				object vStaff = MsTable.Rows[i][1];
				object vPopulation = FmTable.FindValue(id, 1);

				if (CRHelper.DBValueIsEmpty(vStaff) || CRHelper.DBValueIsEmpty(vPopulation))
					continue;

				DataRow row = Table.NewRow();
				row[0] = id;
				row[1] = 10000 * CRHelper.DBValueConvertToDoubleOrZero(vStaff) / CRHelper.DBValueConvertToDoubleOrZero(vPopulation);
				Table.Rows.Add(row); 

				if (maxValue < (double)row[1]) maxValue = (double)row[1];
			}

			// инфа за последние 10 лет
			while (Table.Rows.Count > 10)
			{
				Table.Rows.RemoveAt(0);
			}

			// настройка общего стиля
			TitleTop = "Работники спорта\nна 10 тыс. населения";
			LabelType = ItemLabelType.Year;
			ColorBox = Color.FromArgb(255, 44, 179, 44);
			base.SetStyle();

			// настройка индивидуального стиля
			ChartControl.Width = 250;

			// привязка данных
			NumericSeries series = CRHelper.GetNumericSeries(1, Table);
			series.Label = Table.Columns[1].Caption;
			ChartControl.Chart.Series.Add(series);
		}
		
	}

	public class ChartStaffSporterHelper : ChartBoxesWrapper
	{
		public DataTable MsTable { set; get; }

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
			
			double maxValue = 0;
			for (int i = 0; i < MsTable.Rows.Count; i++)
			{
				string id = MsTable.Rows[i][0].ToString();
				object vStaff = MsTable.Rows[i][1];
				object vSporter = MsTable.Rows[i][2];

				if (CRHelper.DBValueIsEmpty(vSporter) || CRHelper.DBValueIsEmpty(vStaff))
					continue;

				DataRow row = Table.NewRow();
				row[0] = id;
				row[1] = CRHelper.DBValueConvertToDoubleOrZero(vSporter) / CRHelper.DBValueConvertToDoubleOrZero(vStaff);
				Table.Rows.Add(row);

				if (maxValue < (double)row[1]) maxValue = (double)row[1];
			}

			// инфа за последние 10 лет
			while (Table.Rows.Count > 10)
			{
				Table.Rows.RemoveAt(0);
			}

			// настройка общего стиля
			TitleTop = "Занимающиеся на одного\nработника спорта";
			LabelType = ItemLabelType.Year;
			ColorBox = Color.DodgerBlue;
			base.SetStyle();

			// настройка индивидуального стиля
			ChartControl.Width = 480;

			// привязка данных
			NumericSeries series = CRHelper.GetNumericSeries(1, Table);
			series.Label = Table.Columns[1].Caption;
			ChartControl.Chart.Series.Add(series);
		}
		
	}

	public class GridHelper : UltraGridWrapper
	{
		public DataTable MsTable { set; get; }
		public DataTable FmTable { set; get; }

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
			Table.Columns.Add(new DataColumn("abs", typeof(string)));
			Table.Columns.Add(new DataColumn("norm", typeof(string)));
			Table.Columns.Add(new DataColumn("stat", typeof(string)));
			Table.Columns.Add(new DataColumn("rank1", typeof(int)));
			Table.Columns.Add(new DataColumn("rank2", typeof(int)));
			Table.Columns.Add(new DataColumn("rank3", typeof(int)));
			HiddenColumns = 3;

			for (int i = 0; i < MsTable.Rows.Count; i++)
			{
				string id = MsTable.Rows[i][0].ToString();
				object vStaff = MsTable.Rows[i][1];
				object vSporter = MsTable.Rows[i][2];
				object vPopulation = FmTable.FindValue(id, 1);

				if (CRHelper.DBValueIsEmpty(vStaff) || CRHelper.DBValueIsEmpty(vSporter) || CRHelper.DBValueIsEmpty(vPopulation)) 
					continue;

				DataRow row = Table.NewRow();
				row[0] = id;
				row[1] = Convert.ToDecimal(CRHelper.DBValueConvertToDoubleOrZero(vStaff)).ToString("N0");
				row[2] = Convert.ToDecimal(10000 * CRHelper.DBValueConvertToDoubleOrZero(vStaff) / CRHelper.DBValueConvertToDoubleOrZero(vPopulation)).ToString("N2");
				row[3] = Convert.ToDecimal(CRHelper.DBValueConvertToDoubleOrZero(vSporter) / CRHelper.DBValueConvertToDoubleOrZero(vStaff)).ToString("N2");
				Table.Rows.Add(row);
			}

			// ранги

			OrderedData data1 = new OrderedData(Table, 1);
			data1.Sort();
			OrderedData data2 = new OrderedData(Table, 2);
			data2.Sort();
			OrderedData data3 = new OrderedData(Table, 3);
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

			Band.Columns[1].Width = 180;
			Band.Columns[2].Width = 210;
			Band.Columns[3].Width = 250;

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
			foreach (int columnIndex in (new[] { /*1,*/ 2 }))
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
							offset = 50;
			    			break;
						case 2:
			    			offset = 115;//90;
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

			// max / min
			foreach (int columnIndex in (new[] { 3 }))
			{
				UltraGridCell cell = row.Cells[columnIndex + 3];
				UltraGridCell cellTarget = row.Cells[columnIndex];

				int value;
				if (cell.Value != null && Int32.TryParse(cell.Value.ToString(), out value))
				{
					int offset = 110;
					
					if (value == 1)
					{
						cellTarget.Value = String.Format("<div style=\"position: relative;\">{0}{1}</div>", Helper.MaxAbs(offset, -2), cellTarget.Value);
					}
					else if (value < 0)
					{
						cellTarget.Value = String.Format("<div style=\"position: relative;\">{0}{1}</div>", Helper.MinAbs(offset, -2), cellTarget.Value);
					}

				}
			}

			// стрелки
			foreach (int columnIndex in (new[] { 1, /*2,*/ 3 }))
			{
				int rowIndex = row.DataSourceIndex;

				if (rowIndex == Table.Rows.Count - 1)
					continue;

				if (CRHelper.DBValueIsEmpty(Table.Rows[rowIndex][columnIndex])
					|| CRHelper.DBValueIsEmpty(Table.Rows[rowIndex + 1][columnIndex]))
					continue;

				UltraGridCell cell = row.Cells[columnIndex];
				decimal value1 = CRHelper.DBValueConvertToDecimalOrZero(Table.Rows[rowIndex][columnIndex]);
				decimal value2 = CRHelper.DBValueConvertToDecimalOrZero(Table.Rows[rowIndex + 1][columnIndex]);
				
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
							cell.Style.CustomRules = Helper.CellIndicatorStyle(85);
							break;
						case 2:
							cell.Style.CustomRules = Helper.CellIndicatorStyle(125);
							break;
						case 3:
							cell.Style.CustomRules = Helper.CellIndicatorStyle(165);
							break;
					}
				}
			}
			
		}

		protected override void SetDataHeader()
		{
			GridHeaderLayout headerLayout = GridBrick.GridHeaderLayout;
			GridHeaderCell group;

			headerLayout.AddCell("Год");
			group = headerLayout.AddCell("Работники спорта");
			group.AddCell("абс.");
			group.AddCell("на 10 тыс. населения");
			headerLayout.AddCell("Занимающиеся на одного работника спорта");
			
			GridBrick.GridHeaderLayout.ApplyHeaderInfo();
		}
	}
}