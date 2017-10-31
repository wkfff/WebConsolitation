using System;
using System.Data;
using System.Web.UI.WebControls;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.iPhone
{
    public partial class FO_0035_0031 : CustomReportPage
    {
        private DataTable dtDate = new DataTable();
        private DataTable dtGrid = new DataTable();

        // ��������� ������
        private CustomParam selectedPeriod;

        private DateTime date;
        private GridHeaderLayout headerLayout;

        protected override void Page_Load(object sender, EventArgs e)
        {
            #region ������������� ���������� �������

            selectedPeriod = UserParams.CustomParam("selected_period");

            #endregion

            dtDate = new DataTable();
            string query = DataProvider.GetQueryText("FO_0035_0031_date");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "dummy", dtDate);

            date = CRHelper.DateByPeriodMemberUName(dtDate.Rows[0][1].ToString(), 3);

            selectedPeriod.Value = dtDate.Rows[0][1].ToString();

            OutcomesGrid.Width = Unit.Empty;
            OutcomesGrid.Height = Unit.Empty;
            OutcomesGrid.DisplayLayout.NoDataMessage = "��� ������";

            OutcomesGrid.DataBind();

            DateTime proceededDate = DataProvidersFactory.PrimaryMASDataProvider.GetCubeLastProcessedDate("��_����������� � ������");
            lbDescription.Text = String.Format("����������� � ��������� ������ ������������� ������� �� ��������� �� &nbsp;<b><span class='DigitsValue'>{0:dd.MM.yyyy}</span></b>, ���.���.", proceededDate);
        }


        #region ����������� �����

        protected void OutcomesGrid_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FO_0035_0031_Grid");
            dtGrid = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "����������", dtGrid);

            OutcomesGrid.DataSource = dtGrid;
        }

        protected void OutcomesGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.AllowRowNumberingDefault = RowNumbering.None;

            e.Layout.Bands[0].Columns[0].Width = 190;
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;

            e.Layout.Bands[0].Columns[1].Width = 100;
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[1], "N2");

            e.Layout.Bands[0].Columns[2].Width = 100;
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[2], "N2");

            e.Layout.Bands[0].Columns[3].Width = 100;
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[3], "N2");

            e.Layout.Bands[0].Columns[4].Width = 80;
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[4], "P2");

            e.Layout.Bands[0].Columns[5].Width = 80;
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[5], "P2");

            e.Layout.Bands[0].Columns[6].Width = 95;
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[6], "P2");

            e.Layout.Bands[0].Columns[7].Hidden = true;

            for (int i = 1; i < 8; i++)
            {
                e.Layout.Bands[0].Columns[i].CellStyle.Padding.Right = 3;
            }

            headerLayout = new GridHeaderLayout(e.Layout.Grid);
            headerLayout.AddCell("����������");
            headerLayout.AddCell("���� �� ������� �����");

            GridHeaderCell cell = headerLayout.AddCell("���������");
            cell.AddCell(String.Format("�� {0:dd.MM.yyyy}", date));
            cell.AddCell("� ������ ������");            
            headerLayout.AddCell("% ���.");
            headerLayout.AddCell(String.Format("���� ����� � {0} {1} ����", CRHelper.RusMonthDat(date.Month), date.AddYears(-1).Year));
            headerLayout.AddCell("���� ����� � �������� ����");

            headerLayout.ApplyHeaderInfo();

            // e.Layout.Bands[0].Columns[1].HeaderStyle.BorderDetails.ColorRight = Color.FromArgb(85, 85, 85);
            //e.Layout.Bands[0].Columns[2].HeaderStyle.BorderDetails.ColorLeft = Color.FromArgb(85, 85, 85);
            //e.Layout.Bands[0].Columns[2].HeaderStyle.BorderDetails.WidthLeft = 1;
            //e.Layout.Bands[0].Columns[2].HeaderStyle.BorderDetails.StyleLeft = BorderStyle.Solid;

            //e.Layout.Bands[0].Columns[4].HeaderStyle.BorderDetails.ColorRight = Color.FromArgb(85, 85, 85);
            //e.Layout.Bands[0].Columns[5].HeaderStyle.BorderDetails.ColorLeft = Color.FromArgb(85, 85, 85);
        }

        protected void OutcomesGrid_InitializeRow(object sender, RowEventArgs e)
        {
            if (e.Row.Cells[0].Value.ToString() == "�����")
            {
                foreach (UltraGridCell cell in e.Row.Cells)
                {
                    cell.Style.Font.Bold = true;
                }
            }
            else if (e.Row.Cells[7].Value.ToString() == "������� 3")
            {
                e.Row.Cells[0].Style.Padding.Left = 20;
            }
            else if (e.Row.Cells[7].Value.ToString() == "������� 2")
            {
                e.Row.Cells[0].Style.Padding.Left = 10;
            }

            SetConditionArrow(e, 6, 1, true);
        }

        public static void SetConditionArrow(RowEventArgs e, int index, int borderValue, bool direct)
        {
            if (e.Row.Cells[index] != null &&
                e.Row.Cells[index].Value != null)
            {
                double value = Convert.ToDouble(e.Row.Cells[index].Value.ToString());
                string img = string.Empty;
                if (direct)
                {
                    if (value > borderValue)
                    {
                        img = "~/images/arrowGreenUpBB.png";
                    }
                    else if (value < borderValue)
                    {
                        img = "~/images/arrowRedDownBB.png";
                    }
                }
                else
                {
                    if (value > borderValue)
                    {
                        img = "~/images/arrowRedUpBB.png";
                    }
                    else if (value < borderValue)
                    {
                        img = "~/images/arrowGreenDownBB.png";
                    }
                }
                e.Row.Cells[index].Style.BackgroundImage = img;
                e.Row.Cells[index].Style.CustomRules = "background-repeat: no-repeat; background-position: 5px center; padding-left: 0px; padding-right: 5px";
            }
        }

        #endregion
    }
}
