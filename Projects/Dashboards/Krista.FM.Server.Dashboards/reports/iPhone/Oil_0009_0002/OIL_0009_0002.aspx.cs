using System;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using System.Web;

namespace Krista.FM.Server.Dashboards.reports.iPad
{
    public partial class Oil_0009_0002 : CustomReportPage
    {
        private DateTime currentDate;
        private DateTime lastDate;
        private DateTime firstDate;

        double minValue = Double.MaxValue;
        double maxValue = Double.MinValue;

        private GridHeaderLayout headerLayout1;

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            UserParams.Oil.Value = UserParams.Oil.Value.Replace("Бензин марки ", String.Empty);

            string subjectId = "0";
            if (HttpContext.Current.Session["CurrentMOID"] != null)
            {
                subjectId = HttpContext.Current.Session["CurrentMOID"].ToString();
            }

            if (subjectId == "0")
            {
                CustomParam.CustomParamFactory("selected_subject").Value = "Ханты-Мансийский автономный округ - Югра";
            }
            else
            {
                string moName = CustomParam.CustomParamFactory("Mo").Value;

                if (moName.Contains("г."))
                {
                    moName = moName.Replace("г.", "Город ");
                }
                else
                {
                    moName = String.Format("{0} муниципальный район", moName);
                }

                CustomParam.CustomParamFactory("selected_subject").Value = moName;
            }

            InitializeTable1();
        }

        #region Таблица
        private DataTable dt;

        private void InitializeTable1()
        {
            UltraWebGrid1.InitializeLayout += new InitializeLayoutEventHandler(UltraWebGrid1_InitializeLayout);
            UltraWebGrid1.InitializeRow += new InitializeRowEventHandler(UltraWebGrid1_InitializeRow);

            UltraWebGrid2.InitializeLayout += new InitializeLayoutEventHandler(UltraWebGrid1_InitializeLayout);
            UltraWebGrid2.InitializeRow += new InitializeRowEventHandler(UltraWebGrid1_InitializeRow);

            string query;

            DataTable dtDateFederal = new DataTable();
            query = DataProvider.GetQueryText("Oil_0009_0002_incomes_federal_date");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, " ", dtDateFederal);

            currentDate = CRHelper.DateByPeriodMemberUName(dtDateFederal.Rows[2][1].ToString(), 3);
            lastDate = CRHelper.DateByPeriodMemberUName(dtDateFederal.Rows[1][1].ToString(), 3);
            firstDate = CRHelper.DateByPeriodMemberUName(dtDateFederal.Rows[0][1].ToString(), 3);

            UserParams.PeriodCurrentDate.Value = dtDateFederal.Rows[2][1].ToString();
            UserParams.PeriodLastDate.Value = dtDateFederal.Rows[1][1].ToString();
            UserParams.PeriodFirstYear.Value = dtDateFederal.Rows[0][1].ToString();

            DataTable dtFederal = new DataTable();
            query = DataProvider.GetQueryText("Oil_0009_0002_table2");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, " ", dtFederal);

            dtFederal.Columns.RemoveAt(0);
            dtFederal.AcceptChanges();

            UltraWebGrid1.DataSource = dtFederal;
            UltraWebGrid1.DataBind();
            
            DataTable dtDate = new DataTable();
            query = DataProvider.GetQueryText("Oil_0009_0002_incomes_regional_date");
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, " ", dtDate);

            currentDate = CRHelper.DateByPeriodMemberUName(dtDate.Rows[2][1].ToString(), 3);
            lastDate = CRHelper.DateByPeriodMemberUName(dtDate.Rows[1][1].ToString(), 3);
            firstDate = CRHelper.DateByPeriodMemberUName(dtDate.Rows[0][1].ToString(), 3);

            UserParams.PeriodCurrentDate.Value = dtDate.Rows[2][1].ToString();
            UserParams.PeriodLastDate.Value = dtDate.Rows[1][1].ToString();
            UserParams.PeriodFirstYear.Value = dtDate.Rows[0][1].ToString();

            dt = new DataTable();
            query = DataProvider.GetQueryText("Oil_0009_0002_table1");
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, " ", dt);

            dt.Columns.RemoveAt(0);
            dt.AcceptChanges();

            foreach (DataRow row in dt.Rows)
            {
                string level = String.Empty;
                if (row["Уровень"] != DBNull.Value)
                {
                    level = row["Уровень"].ToString();
                }

                if (level != "1")
                {
                    double value = Convert.ToDouble(row[1]);
                    minValue = Math.Min(minValue, value);
                    maxValue = Math.Max(maxValue, value);
                }
            }

            fillingCounter = 1;
            UltraWebGrid2.DataSource = dt;
            UltraWebGrid2.DataBind();
        }

        private int fillingCounter;
        void UltraWebGrid1_InitializeRow(object sender, RowEventArgs e)
        {
            iPadBricks.iPadBricks.iPadBricksHelper.SetConditionArrow(e, 3, 0, false);
            SetConditionBall(e, 5, false);

            if (e.Row.Index < 1 || (e.Row.Cells[1].Value.ToString().ToLower().Contains("федера") && !e.Row.Cells[1].Value.ToString().ToLower().Contains("азс ")))
            {
                e.Row.Style.Font.Bold = true;
            }

            if (e.Row.Cells[7].Value.ToString() == "2")
            {
                e.Row.Cells[0].Value = Convert.ToInt32(fillingCounter).ToString("N0");
                fillingCounter++;

                e.Row.Cells[1].Style.Padding.Left = 20;

                SetRowRank(e.Row, 2, minValue.ToString(), "min.png");
                SetRowRank(e.Row, 2, maxValue.ToString(), "max.png");
            }
            else
            {
                e.Row.Style.Font.Bold = true;
            }
        }

        public static void SetConditionBall(RowEventArgs e, int index, bool directAssesment)
        {
            if (e.Row.Cells[index] != null &&
                e.Row.Cells[index].Value != null)
            {
                double value = Convert.ToDouble(e.Row.Cells[index].Value.ToString());
                double compareValue = 0;
                string positiveImg = directAssesment ? "~/images/ballGreenBB.png" : "~/images/ballRedBB.png";
                string negativeImg = directAssesment ? "~/images/ballRedBB.png" : "~/images/ballGreenBB.png";
                string img = String.Empty;
                if (value < compareValue)
                {
                    img = negativeImg;

                }
                else if (value > compareValue)
                {
                    img = positiveImg;

                }
                e.Row.Cells[index].Style.BackgroundImage = img;
                e.Row.Cells[index].Style.CustomRules = "background-repeat: no-repeat; background-position: 20px center; padding-left: 2px";
            }
        }

        public void SetRowRank(UltraGridRow row, int columnIndex, string pattern, string img)
        {
            for (int i = 0; i < row.Cells.Count; i++)
            {
                UltraGridCell cell = row.Cells[i];

                bool isIndicatorColumn = i == columnIndex;

                if (isIndicatorColumn && cell.Value != null)
                {
                    string value = cell.Value.ToString();
                    if (value == pattern)
                    {
                        cell.Style.BackgroundImage = String.Format("~/images/{0}", img);
                        cell.Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px; padding-left: 2px; ";
                    }
                }
            }
        }

        void UltraWebGrid1_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.Bands[0].Grid.Width = Unit.Empty;
            e.Layout.Bands[0].Grid.Height = Unit.Empty;

            UltraGridColumn numberColumn = new UltraGridColumn();
            numberColumn.Width = 40;
            numberColumn.CellStyle.Padding.Right = 5;
            numberColumn.CellStyle.BackColor = numberColumn.Header.Style.BackColor;
            numberColumn.SortingAlgorithm = SortingAlgorithm.NotSet;
            numberColumn.CellStyle.HorizontalAlign = HorizontalAlign.Right;
            e.Layout.Bands[0].Columns.Insert(0, numberColumn);

            headerLayout1 = new GridHeaderLayout(e.Layout.Bands[0].Grid);
            headerLayout1.AddCell(sender == UltraWebGrid1 ? String.Empty : "№ п/п");
            headerLayout1.AddCell("");
            headerLayout1.AddCell(String.Format("Средняя розничная цена на {0:dd.MM.yyyy}, руб.", currentDate));

            GridHeaderCell headerCell = headerLayout1.AddCell(String.Format("Изменение к {0:dd.MM.yyyy}", lastDate));
            headerCell.AddCell("Абс. откл, руб.");
            headerCell.AddCell("Темп прироста");

            headerCell = headerLayout1.AddCell(String.Format("Изменение к {0:dd.MM.yyyy}", firstDate));
            headerCell.AddCell("Абс. откл, руб.");
            headerCell.AddCell("Темп прироста");

            headerLayout1.ApplyHeaderInfo();

            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[2], "N2");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[3], "N2");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[4], "P2");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[5], "N2");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[6], "P2");

            e.Layout.Bands[0].HeaderStyle.BorderColor = Color.FromArgb(60, 60, 60);

            e.Layout.Bands[0].Columns[1].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[1].Width = 245;
            e.Layout.Bands[0].Columns[2].Width = 110;
            e.Layout.Bands[0].Columns[3].Width = 90;
            e.Layout.Bands[0].Columns[4].Width = 90;
            e.Layout.Bands[0].Columns[5].Width = 90;
            e.Layout.Bands[0].Columns[6].Width = 90;

            e.Layout.Bands[0].Columns[7].Hidden = true;

            e.Layout.Bands[0].Columns[2].CellStyle.Font.Size = 14;

            int borderWidth = 3;
            e.Layout.Bands[0].Columns[1].CellStyle.BorderDetails.WidthRight = borderWidth;
            e.Layout.Bands[0].Columns[2].CellStyle.BorderDetails.WidthRight = borderWidth;
            e.Layout.Bands[0].Columns[3].CellStyle.BorderDetails.WidthLeft = borderWidth;
            e.Layout.Bands[0].Columns[4].CellStyle.BorderDetails.WidthRight = borderWidth;
            e.Layout.Bands[0].Columns[5].CellStyle.BorderDetails.WidthLeft = borderWidth;
        }

        #endregion
    }
}
