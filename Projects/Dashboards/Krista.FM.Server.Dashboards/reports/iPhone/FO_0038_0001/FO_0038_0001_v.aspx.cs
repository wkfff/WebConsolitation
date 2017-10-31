using System;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.iPhone
{
    public partial class FO_0038_0001_V : CustomReportPage
    {       
        private int monthNum;
        private int yearNum;

        private GridHeaderLayout headerLayout;
        private string bujetName;


        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            DataTable dtDate = new DataTable();
            string query = DataProvider.GetQueryText("FO_0002_0002_date");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);

            //period_day_fk
            //[������].[������].[������ ���� ��������].[2010].[��������� 2].[������� 4].[�������]
            monthNum = CRHelper.MonthNum(dtDate.Rows[0][3].ToString());
            yearNum = Convert.ToInt32(dtDate.Rows[0][0]);

            DateTime CurrentDate = new DateTime(yearNum, monthNum, 1);
            UserParams.PeriodCurrentDate.Value = CRHelper.PeriodMemberUName("", CurrentDate, 4);
            UserParams.PeriodLastDate.Value = CRHelper.PeriodMemberUName("", CurrentDate.AddMonths(-1), 4);
            Label1.Text = String.Format("�� {0} {1} {2} ����", monthNum, CRHelper.RusManyMonthGenitive(monthNum), yearNum);
            Label3.Text = String.Format("���� ����� ����������� ����������� ���������� ������� �� {0} {1} {2} �.<br/>� ������������ ������� ����������� ���� �� ������", monthNum, CRHelper.RusManyMonthGenitive(monthNum), yearNum);

            

            UltraWebGrid1.InitializeLayout += new InitializeLayoutEventHandler(UltraWebGrid1_InitializeLayout);
            UltraWebGrid1.InitializeRow += new InitializeRowEventHandler(UltraWebGrid1_InitializeRow);

            bujetName = "����. ������";

            headerLayout = new GridHeaderLayout(UltraWebGrid1);
            UltraWebGrid1.Bands.Clear();

            UltraWebGrid1.DataSource = GetDataSource();
            UltraWebGrid1.DataBind();

            UltraWebGrid1.Width = Unit.Empty;
            UltraWebGrid1.Height = Unit.Empty;

            bujetName = "���. ������";

            UltraWebGrid2.InitializeLayout += new InitializeLayoutEventHandler(UltraWebGrid1_InitializeLayout);
            UltraWebGrid2.InitializeRow += new InitializeRowEventHandler(UltraWebGrid1_InitializeRow);

            headerLayout = new GridHeaderLayout(UltraWebGrid2);
            UltraWebGrid2.Bands.Clear();

            UltraWebGrid2.DataSource = GetDataSource1();
            UltraWebGrid2.DataBind();

            UltraWebGrid2.Width = Unit.Empty;
            UltraWebGrid2.Height = Unit.Empty;

            Label2.Text = string.Format("��������� {0:dd.MM.yyyy HH:mm:ss}", DateTime.Now);
        }

        private DataTable GetDataSource()
        {
            string query = DataProvider.GetQueryText("FO_0002_0002_incomes_v");
            DataTable dtGrid = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "���", dtGrid);

            dtGrid.Rows.RemoveAt(0);

            query = DataProvider.GetQueryText("FO_0002_0002_outcomes_v");
            DataTable dtGrid2 = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "���", dtGrid2);

            foreach (DataRow row in dtGrid2.Rows)
            {
                dtGrid.ImportRow(row);
            }

            query = DataProvider.GetQueryText("FO_0002_0002_deficite_v");
            dtGrid2 = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "���", dtGrid2);

            dtGrid2.Columns[0].ColumnName = "���";

            foreach (DataRow row in dtGrid2.Rows)
            {
                dtGrid.ImportRow(row);
            }

            query = DataProvider.GetQueryText("FO_0002_0002_finsources_v");
            dtGrid2 = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "���", dtGrid2);

            foreach (DataRow row in dtGrid2.Rows)
            {
                dtGrid.ImportRow(row);
            }
            return dtGrid;
        }

        private DataTable GetDataSource1()
        {
            string query = DataProvider.GetQueryText("FO_0002_0002_incomes_v1");
            DataTable dtGrid = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "���", dtGrid);

            dtGrid.Rows.RemoveAt(0);

            query = DataProvider.GetQueryText("FO_0002_0002_outcomes_v1");
            DataTable dtGrid2 = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "���", dtGrid2);

            foreach (DataRow row in dtGrid2.Rows)
            {
                dtGrid.ImportRow(row);
            }

            query = DataProvider.GetQueryText("FO_0002_0002_deficite_v1");
            dtGrid2 = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "���", dtGrid2);

            dtGrid2.Columns[0].ColumnName = "���";

            foreach (DataRow row in dtGrid2.Rows)
            {
                dtGrid.ImportRow(row);
            }

            query = DataProvider.GetQueryText("FO_0002_0002_finsources_v1");
            dtGrid2 = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "���", dtGrid2);

            foreach (DataRow row in dtGrid2.Rows)
            {
                dtGrid.ImportRow(row);
            }
            return dtGrid;
        }

        void UltraWebGrid1_InitializeRow(object sender, RowEventArgs e)
        {           
            SetConditionCorner(e, 1);

            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
                e.Row.Cells[i].Style.Padding.Right = 3;
            }
        }

        public static void SetConditionCorner(RowEventArgs e, int index)
        {
            if (e.Row.Cells[index] != null &&
                e.Row.Cells[index].Value != null)
            {
                double value = Convert.ToDouble(e.Row.Cells[index].Value.ToString());
                string img = string.Empty;

                if (value > 1)
                {
                    img = "~/images/cornerGreen.gif";
                }
                else if (value < 1)
                {
                    img = "~/images/cornerRed.gif";
                }

                e.Row.Cells[index].Style.BackgroundImage = img;
                e.Row.Cells[index].Style.CustomRules = "background-repeat: no-repeat; background-position: right top; padding-left: 0px";
            }
        }

        /// <summary>
        /// ��������� �������� ������ ���������.
        /// </summary>
        /// <returns></returns>
        private double CommonAssessionLimit()
        {
            return 1.0 / 12.0 * (Double)monthNum;
        }

        void UltraWebGrid1_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.Bands[0].HeaderStyle.Wrap = true;
            e.Layout.Bands[0].HeaderStyle.HorizontalAlign = HorizontalAlign.Center;

            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;

            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[1], "P1");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[2], "P1");

            e.Layout.Bands[0].Columns[0].Width = 155;
            e.Layout.Bands[0].Columns[1].Width = 80;
            e.Layout.Bands[0].Columns[2].Width = 80;

            headerLayout.AddCell("���");
            GridHeaderCell cell = headerLayout.AddCell(bujetName);
            GridHeaderCell childCell = cell.AddCell("���� ����� � ��������");
            childCell.AddCell("����");
            childCell.AddCell("���.");            

            headerLayout.ApplyHeaderInfo();

            e.Layout.Bands[0].Columns[1].Header.Style.Font.Size = FontUnit.Parse("14px");
            e.Layout.Bands[0].Columns[2].Header.Style.Font.Size = FontUnit.Parse("14px");
        }
    }
}