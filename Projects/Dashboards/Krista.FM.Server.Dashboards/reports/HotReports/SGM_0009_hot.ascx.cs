using System;
using System.Data;
using System.Web.UI.WebControls;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.reports.SGM;
using Krista.FM.Server.Dashboards.SgmSupport;

namespace Krista.FM.Server.Dashboards.reports.HotReports
{
    public partial class SGM_0009_hot : GadgetControlBase, IHotReport
    {
        protected int year;
        protected string months;
        protected string dies;
        protected string groupName;
        protected DataTable dtFullData;
        private readonly SGMDataRotator dataRotator = new SGMDataRotator();
        private readonly SGMDataObject dataObject = new SGMDataObject();
        protected readonly SGMSupport supportClass = new SGMSupport();

        protected override void Page_Load(object sender, EventArgs e)
        {
            dataObject.reportFormRotator = dataRotator;
            dataRotator.subjectCode = String.Empty;
            dataRotator.isSubjectReport = false;
            dataRotator.formNumber = 1;
            base.Page_Load(sender, e);
            dataObject.InitObject();
            dataRotator.FillDeseasesList(null, 0);
            year = dataRotator.GetLastYear();
            months = dataRotator.GetMonthParamIphone();
            dies = dataRotator.GetDeseaseCodes(0);
            groupName = Convert.ToString(Convert.ToInt32(PeopleGroupType.pgtAll));
            grid.Width = CRHelper.GetGridWidth(350);
            FillComponentData();
        }

        protected virtual DataTable GetGridDataSet()
        {
            dataObject.mainColumn = SGMDataObject.MainColumnType.mctMapName;
            dataObject.useLongNames = true;

            for (int i = 0; i < 2; i++)
            {
                dataObject.AddColumn(
                    SGMDataObject.DependentColumnType.dctAbs,
                    Convert.ToString(year - i),
                    months,
                    string.Empty,
                    groupName,
                    dies);

                dataObject.AddColumn(
                    SGMDataObject.DependentColumnType.dctRelation,
                    Convert.ToString(i * 3 + 1));

                dataObject.AddColumn(
                    SGMDataObject.DependentColumnType.dctRank,
                    Convert.ToString(i * 3 + 2));
            }

            return dataObject.FillData(2);
        }

        protected virtual void FillComponentData()
        {
            DataTable dtGrid = GetGridDataSet();
            dtFullData = dataObject.CloneDataTable(dtGrid);

            dtGrid.Columns.RemoveAt(1);
            dtGrid.Columns.RemoveAt(3);
            dtGrid.Columns.RemoveAt(3);
            dtGrid.Columns.RemoveAt(3);

            grid.DataSource = dtGrid;
            grid.DataBind();

            Label1.Text = string.Format("Общая инфекционная заболеваемость<br>за {0} {1}",
                supportClass.GetMonthLabelFull(months, 0), year);
        }

        public int Width
        {
            get
            {
                return 360;
            }
        }

        public int Height
        {
            get
            {
                return 340;
            }
        }

        protected void grid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;

            grid.Width = 350;
            grid.Columns[0].Width = 175;
            grid.Columns[1].Width = 75;
            grid.Columns[2].Width = 40;

            grid.Columns[0].CellStyle.Wrap = true;
            CRHelper.FormatNumberColumn(grid.Columns[01], "N2");
            CRHelper.FormatNumberColumn(grid.Columns[02], "N0");

            grid.Columns[0].Header.Caption = "Территория";
            grid.Columns[1].Header.Caption = "На 100 тыс.";
            grid.Columns[2].Header.Caption = "Ранг";
            grid.Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            grid.Columns[2].CellStyle.HorizontalAlign = HorizontalAlign.Right;
        }

        protected void grid_InitializeRow(object sender, RowEventArgs e)
        {
            double relValue1 = 0;
            double relValue2 = 0;
            double absValue1 = 0;
            double absValue2 = 0;
            double rank1 = 0;
            double rank2 = 0;

            UltraGridCell cell1 = e.Row.Cells[1];
            UltraGridCell cell2 = e.Row.Cells[2];

            DataRow dr = dtFullData.Rows[e.Row.Index];

            if (supportClass.CheckValue(dr[2])) relValue1 = Convert.ToDouble(dr[2]);
            if (supportClass.CheckValue(dr[5])) relValue2 = Convert.ToDouble(dr[5]);
            if (supportClass.CheckValue(dr[1])) absValue1 = Convert.ToDouble(dr[1]);
            if (supportClass.CheckValue(dr[4])) absValue2 = Convert.ToDouble(dr[4]);
            if (supportClass.CheckValue(dr[3])) rank1 = Convert.ToDouble(dr[3]);
            if (supportClass.CheckValue(dr[6])) rank2 = Convert.ToDouble(dr[6]);

            if (relValue1 > relValue2)
            {
                cell1.Style.CssClass = "ArrowUpRed";
                cell1.Title = string.Format("Рост {0}", supportClass.GetDifferenceTextEx(absValue1, absValue2, relValue1, relValue2, false, true));
            }
            if (relValue1 < relValue2)
            {
                cell1.Style.CssClass = "ArrowDownGreen";
                cell1.Title = string.Format("Снижение {0}", supportClass.GetDifferenceTextEx(absValue1, absValue2, relValue1, relValue2, false, true));
            }

            if (rank1 < rank2)
            {
                cell2.Style.CssClass = "ArrowUpRed";
                cell2.Title = string.Format("-{0}", rank2 - rank1);
            }
            if (rank1 > rank2)
            {
                cell2.Style.CssClass = "ArrowDownGreen";
                cell2.Title = string.Format("+{0}", rank1 - rank2);
            }

            if (rank1 == 0) e.Row.Cells[0].Style.Font.Bold = true;
        }
    }
}