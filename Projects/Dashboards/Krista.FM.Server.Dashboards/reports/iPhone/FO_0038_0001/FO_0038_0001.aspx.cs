using System;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.UltraGauge.Resources;
using Infragistics.WebUI.UltraWebGauge;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Krista.FM.Server.Dashboards.Core;

namespace Krista.FM.Server.Dashboards.reports.iPhone
{
    public partial class FO_0038_0001 : CustomReportPage
    {
        private DataTable dtGrid = new DataTable();

        private int monthNum;
        private int yearNum;

        private GridHeaderLayout headerLayout;

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            DataTable dtDate = new DataTable();
            string query = DataProvider.GetQueryText("FO_0002_0002_date");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);

            monthNum = CRHelper.MonthNum(dtDate.Rows[0][3].ToString());
            yearNum = Convert.ToInt32(dtDate.Rows[0][0]);

            DateTime CurrentDate = new DateTime(yearNum, monthNum, 1);
            UserParams.PeriodCurrentDate.Value = CRHelper.PeriodMemberUName("", CurrentDate, 4);
            UserParams.PeriodLastDate.Value = CRHelper.PeriodMemberUName("", CurrentDate.AddYears(-1), 4);
            Label1.Text = string.Format("�� {0} {1} {2} ����, ���.���.", monthNum, CRHelper.RusManyMonthGenitive(monthNum), yearNum);

            GetDataSource();

            UltraWebGrid1.InitializeLayout += new InitializeLayoutEventHandler(UltraWebGrid1_InitializeLayout);
            UltraWebGrid1.InitializeRow += new InitializeRowEventHandler(UltraWebGrid1_InitializeRow);

            headerLayout = new GridHeaderLayout(UltraWebGrid1);
            UltraWebGrid1.Bands.Clear();

            UltraWebGrid1.DataSource = dtGrid;
            UltraWebGrid1.DataBind();

            UltraWebGrid1.Width = Unit.Empty;
            UltraWebGrid1.Height = Unit.Empty;
        }

        private void GetDataSource()
        {
            dtGrid = new DataTable();

            dtGrid.Columns.Add("1", typeof(string));
            dtGrid.Columns.Add("2", typeof(string));
            dtGrid.Columns.Add("3", typeof(string));

            string query;
            DataTable dtGrid1;

            dtGrid1 = GetDataTable("FO_0002_0002_incomes");
            dtGrid1.Rows.RemoveAt(0);
            ImportRows(dtGrid1);

            query = DataProvider.GetQueryText("FO_0002_0002_outcomes");
            dtGrid1 = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "���", dtGrid1);
            ImportRows(dtGrid1);

            query = DataProvider.GetQueryText("FO_0002_0002_deficite");
            dtGrid1 = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "���", dtGrid1);
            ImportRows(dtGrid1);

            query = DataProvider.GetQueryText("FO_0002_0002_finsources");
            dtGrid1 = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "���", dtGrid1);
            ImportRows(dtGrid1);       
        }

        private static DataTable GetDataTable(string queryName)
        {
            DataTable dtGrid1 = new DataTable();
            string query = DataProvider.GetQueryText(queryName);
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "���", dtGrid1);
            return dtGrid1;
        }

        private void ImportRows(DataTable dtGrid1)
        {
            foreach (DataRow row in dtGrid1.Rows)
            {
                DataRow newRow = dtGrid.NewRow();
                newRow[0] = row[0];
                newRow[1] = String.Format("{0:N2}<br/>{1:P2}", row[1], row[2]);
                newRow[2] = String.Format("{0:N2}<br/>{1:P2}", row[3], row[4]);
                dtGrid.Rows.Add(newRow);
            }
        }

        void UltraWebGrid1_InitializeRow(object sender, RowEventArgs e)
        {
            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
                e.Row.Cells[i].Style.Padding.Right = 3;
                e.Row.Cells[i].Style.Padding.Top = 1;
                e.Row.Cells[i].Style.Padding.Bottom = 2;
            }
        }

        private string GetConditionBall(object cell)
        {
            if (cell != null && cell != DBNull.Value)
            {
                double value = Convert.ToDouble(cell.ToString());
                string positiveImg = "<img src='../../../images/ballGreenBB.png'>";
                string negativeImg =  "<img src='../../../images/ballRedBB.png'>";                
                if (value < CommonAssessionLimit())
                {
                    return negativeImg;                    
                }
                else
                {
                    return positiveImg;
                }                
            }
            return string.Empty;
        }

        public static void SetConditionArrow(RowEventArgs e, int index)
        {
            if (e.Row.Cells[index] != null &&
                e.Row.Cells[index].Value != null)
            {
                double value = Convert.ToDouble(e.Row.Cells[index].Value.ToString());
                string img = string.Empty;
                string title = string.Empty;
                if (value > 1)
                {
                    img = "~/images/arrowGreenUpBB.png";
                    title = "���� � �������� ����";
                }
                else if (value < 1)
                {
                    img = "~/images/arrowRedDownBB.png";
                    title = "�������� � �������� ����";
                }

                e.Row.Cells[index].Style.BackgroundImage = img;
                e.Row.Cells[index].Style.CustomRules = "background-repeat: no-repeat; background-position: 2px center; padding-left: 0px";
                e.Row.Cells[index].Title = title;
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

            e.Layout.Bands[0].Columns[0].Width = 138;
            e.Layout.Bands[0].Columns[1].Width = 88;
            e.Layout.Bands[0].Columns[2].Width = 88;

            headerLayout.AddCell("���");
            GridHeaderCell cell = headerLayout.AddCell("����. ������");            
            cell.AddCell("����<br/>� % ���.");
            cell = headerLayout.AddCell("���. ������");
            cell.AddCell("����<br/>� % ���.");

            headerLayout.ApplyHeaderInfo();

            e.Layout.Bands[0].Columns[1].Header.Style.Font.Size = FontUnit.Parse("14px");
            e.Layout.Bands[0].Columns[2].Header.Style.Font.Size = FontUnit.Parse("14px");
        }       
    }
}