using System;
using System.Data;
using System.Web.UI.WebControls;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Section;
using Infragistics.Documents.Excel;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.Shared;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using System.Web;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Core.Primitives;
using System.Drawing;
using Infragistics.WebUI.UltraWebChart;
using System.IO;
using System.Drawing.Imaging;

namespace Krista.FM.Server.Dashboards.reports.STAT_0023_0006
{
    public enum SliceType
    {
        MiniOrg,
        MicroOrg
    }

    public partial class Default : CustomReportPage
    {
        #region Поля

        private DataTable chart1Dt = new DataTable();
        private DataTable chart2Dt = new DataTable();
        private DataTable gridDt = new DataTable();
        private DataTable legendChartDt = new DataTable();
        private DataTable gridTables = new DataTable();
        private string gridIndicatorCaption = "Компания";
        private int columnWidth = 100;
        private bool firstRowChange;
        private bool activeRowChange = true;
        private static MemberAttributesDigest periodDigest;
        private DateTime lastDate;

        #endregion

        private int GetScreenWidth
        {
            get
            {
                if (Request.Cookies != null)
                {
                    if (Request.Cookies[CustomReportConst.ScreenWidthKeyName] != null)
                    {
                        HttpCookie cookie = Request.Cookies[CustomReportConst.ScreenWidthKeyName];
                        int value = Int32.Parse(cookie.Value);
                        return value;
                    }
                }
                return (int)Session["width_size"];
            }
        }

        private bool IsSmallResolution
        {
            get { return GetScreenWidth < 1000; }
        }

        private int MinScreenWidth
        {
            get { return IsSmallResolution ? 800 : CustomReportConst.minScreenWidth; }
        }

        private int MinScreenHeight
        {
            get { return IsSmallResolution ? 600 : CustomReportConst.minScreenHeight; }
        }

        #region Параметры запроса

        // множество для среза данных
        private CustomParam sliceSet;

        // множество лет
        private CustomParam periodSet;

        //выбранный год
        private CustomParam lastPeriod;

        //выбранная строка в таблице
        private CustomParam chartParam;

        #endregion



        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            #region Настройка грида

            GridBrick.AutoSizeStyle = GridAutoSizeStyle.AutoHeight;
            GridBrick.AutoHeightRowLimit = 15;
            GridBrick.Grid.InitializeLayout += new InitializeLayoutEventHandler(Grid_InitializeLayout);
            GridBrick.Grid.InitializeRow += new InitializeRowEventHandler(Grid_InitializeRow);
            //GridBrick.Grid.ActiveRowChange += new ActiveRowChangeEventHandler(Grid_ActiveRowChange);
            GridBrick.Grid.DataBound += new EventHandler(Grid_DataBound);
            GridBrick.Grid.Height = Convert.ToInt32(CustomReportConst.minScreenHeight * 0.2 * 0.6);
            GridBrick.Grid.Width = Convert.ToInt32(CustomReportConst.minScreenWidth * 0.86);
            #endregion

            #region Инициализация параметров запроса

            sliceSet = UserParams.CustomParam("slice_set");
            periodSet = UserParams.CustomParam("period_set");
            lastPeriod = UserParams.CustomParam("last_period");
            chartParam = UserParams.CustomParam("chart_param");

            #endregion

            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
        }

        private static Color GetCustomColor(int i)
        {
            switch (i)
            {
                case 1:
                    {
                        return Color.LimeGreen;
                    }
                case 2:
                    {
                        return Color.LightSkyBlue;
                    }
                case 3:
                    {
                        return Color.Gold;
                    }
                case 4:
                    {
                        return Color.Peru;
                    }
                case 5:
                    {
                        return Color.DarkOrange;
                    }
                case 6:
                    {
                        return Color.PeachPuff;
                    }
                case 7:
                    {
                        return Color.MediumSlateBlue;
                    }
                case 8:
                    {
                        return Color.ForestGreen;
                    }
                case 9:
                    {
                        return Color.HotPink;
                    }
            }
            return Color.White;
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            if (!Page.IsPostBack)
            {

                ComboYear.Title = "Год";
                ComboYear.Width = 100;
                ComboYear.MultiSelect = false;
                periodDigest = new MemberAttributesDigest(DataProvidersFactory.PrimaryMASDataProvider, "STAT_0023_0006_periodDigest");
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillMemberUniqueNameList(periodDigest.UniqueNames, periodDigest.MemberLevels));

                ComboYear.SelectLastNode();
                firstRowChange = false;
            }


            string periods = String.Empty;
            lastPeriod.Value = ComboYear.SelectedValues[ComboYear.SelectedValues.Count - 1];

            foreach (string year in ComboYear.SelectedValues)
            {
                periods += String.Format("[Период__Год Квартал Месяц].[Период__Год Квартал Месяц].[{0}],", year);
            }
            periodSet.Value = periods.TrimEnd(',');

            string sliceText = String.Empty;

            UserParams.PeriodYear.Value = ComboYear.SelectedValue;
            Page.Title = String.Format("Мониторинг утилизации попутного нефтяного газа по предприятиям");
            Label1.Text = Page.Title;
            Label2.Text = String.Format("Данные ежегодного мониторинга утилизации попутного нефтяного газа в разрезе предприятий ХМАО-Югры (по состоянию на {0} год)", ComboYear.SelectedValue.ToString());
            GridDataBind();
        }

        #region Обработчики грида

        private void GridDataBind()
        {
            string query = DataProvider.GetQueryText("STAT_0023_0006_grid");
            gridDt = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Наименование показателей", gridDt);

            if (gridDt.Rows.Count > 0)
            {
                if (gridDt.Columns.Count > 1)
                {
                    gridDt.Columns.RemoveAt(0);
                }


                GridBrick.DataTable = gridDt;
            }
        } 
        protected void Grid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            if (e.Layout.Bands[0].Columns.Count == 0)
            {
                return;
            }
            e.Layout.Bands[0].AllowColumnMoving = AllowColumnMoving.None;
            e.Layout.Bands[0].HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.RowAlternateStylingDefault = DefaultableBoolean.False;
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(340);
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[0].MergeCells = true;

            int columnCount = e.Layout.Bands[0].Columns.Count;
            e.Layout.Bands[0].Columns[columnCount - 1].Hidden = true;
            e.Layout.Bands[0].Columns[columnCount - 2].Hidden = true;
            e.Layout.Bands[0].Columns[columnCount - 3].Hidden = true;

            GridBrick.GridHeaderLayout.AddCell("Компания");
            GridHeaderLayout headerLayout = GridBrick.GridHeaderLayout;
            GridHeaderCell cell = headerLayout.AddCell("Коэффициент утилизации газа, %");
            cell.AddCell(ComboYear.SelectedValue);
            cell.AddCell(Convert.ToString(Convert.ToInt32(ComboYear.SelectedValue) - 1));
            cell = headerLayout.AddCell("Сожжено в факелах, млн. м3");
            cell.AddCell(ComboYear.SelectedValue);
            cell.AddCell(Convert.ToString(Convert.ToInt32(ComboYear.SelectedValue) - 1));

            for (int i = 1; i < columnCount - 3; i = i + 1)
                {
                    e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(100);
                    e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
                    string year = e.Layout.Bands[0].Columns[i].Header.Caption;
                }

            headerLayout.ApplyHeaderInfo();
        }

        protected void Grid_InitializeRow(object sender, RowEventArgs e)
        {
            int cellCount = e.Row.Cells.Count;
            //тип строки
            int type = 0;
            if (e.Row.Cells[cellCount - 2].Value != null)
            {
                type = Convert.ToInt32(e.Row.Cells[cellCount - 2].Value.ToString());
            }

            //единица измерения строки


            //уровень строки
            if (e.Row.Cells[cellCount - 1].Value != DBNull.Value)
            {
                if (Convert.ToInt32(e.Row.Cells[cellCount - 1].Text) == 0)
                {
                    e.Row.Cells[0].Style.Padding.Left = 20;
                    e.Row.Cells[0].Style.Font.Bold = true;
                    for (int i = 1; i < cellCount - 3; i++)
                    {
                        if (type == 0)
                        {
                            e.Row.Cells[i].Style.Font.Bold = true;
                        }
                    }

                    e.Row.Cells[0].Value = e.Row.Cells[0].Value + String.Format(@"
<table style='border-collapse: collapse;border-style: none' width='100%'>
<tr>
<td class='ExpandBlockSecondState' onclick='resize(this)' style='padding-left:20px;background-position:5px Center;font-weight: Normal;'>+&nbsp;</td>
</tr>
</table>");
                }
                if (Convert.ToInt32(e.Row.Cells[cellCount - 1].Text) == 1)
                {
                    e.Row.Cells[0].Style.Padding.Left = 40;
                    e.Row.Cells[cellCount - 1].Style.CssClass = "hiddenLevel";
                }
            }

            for (int i = 1; i < cellCount - 3; i++)
            {
                UltraGridCell cell = e.Row.Cells[i];
                cell.Style.Padding.Right = 3;

                string[] nameParts = e.Row.Band.Columns[i].Header.Caption.Split(' ');
                int prevYear = lastDate.Year;


                switch (type)
                {
                    case 0:
                        {
                            if (cell.Value != null)
                            {
                                cell.Value = Convert.ToDouble(cell.Value.ToString()).ToString("N2");
                            }
                            cell.Style.BorderDetails.WidthBottom = 0;
                            break;
                        }
                    case 1:
                        {
                            if (cell.Value != null)
                            {

                                cell.Value = Convert.ToDouble(cell.Value.ToString()).ToString("N2");
                                cell.Title = String.Format("Отклонение к {0} году", Convert.ToInt32(ComboYear.SelectedValue) - 1);
                            }

                            cell.Style.BorderDetails.WidthTop = 0;
                            cell.Style.BorderDetails.WidthBottom = 0;
                            break;
                        }
                    case 2:
                        {
                            if (cell.Value != null)
                            {
                                double growRate = Convert.ToDouble(cell.Value.ToString());
                                cell.Value = growRate.ToString("P1");

                                if (growRate > 0)
                                {
                                    cell.Style.BackgroundImage = "~/images/arrowGreenUpBB.png";
                                }
                                else if (growRate < 0)
                                {
                                    cell.Style.BackgroundImage = "~/images/arrowRedDownBB.png";
                                }
                                cell.Title = String.Format("Темп прироста к {0} году", Convert.ToInt32(ComboYear.SelectedValue) - 1);
                                cell.Style.CustomRules = "background-repeat: no-repeat; background-position: center center; margin: 2px";
                            }
                            cell.Style.BorderDetails.WidthTop = 0;
                            break;
                        }
                }
            }
        }

        private void Grid_DataBound(object sender, EventArgs e)
        {

        }

        private void ActiveGridRow(UltraGridRow row)
        {
            if (row == null)
                return;

            if (!activeRowChange)
                return;

            string measure = "";
            {
                string value = row.Cells[row.Cells.Count - 1].Value.ToString();

            }

            activeRowChange = false;
        }

        protected void Grid_ActiveRowChange(object sender, RowEventArgs e)
        {
            ActiveGridRow(e.Row);
        }

        private string GetName(string value)
        {
            return value;
        }

        #endregion

        #region Экспорт в Excel

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            ReportExcelExporter1.WorksheetTitle = Label1.Text;
            ReportExcelExporter1.WorksheetSubTitle = Label2.Text;
            ReportExcelExporter1.SheetColumnCount = 15;
            ReportExcelExporter1.GridColumnWidthScale = 1.2;

            Workbook workbook = new Workbook();

            Worksheet sheet1 = workbook.Worksheets.Add("Таблица");
            for (int i = 0; i < GridBrick.Grid.Rows.Count; i++)
            {
                if (Convert.ToInt32(GridBrick.Grid.Rows[i].Cells[GridBrick.Grid.Columns.Count - 1].Text) == 0)
                {
                    int index = GridBrick.Grid.Rows[i].Cells[0].Text.IndexOf('\n');
                    GridBrick.Grid.Rows[i].Cells[0].Text = GridBrick.Grid.Rows[i].Cells[0].Text.Remove(index + 1);
                }
                else
                {
                    GridBrick.Grid.Rows[i].Cells[0].Text = "        " + GridBrick.Grid.Rows[i].Cells[0].Text;
                }
            }
            GridBrick.Grid.Rows[0].Activated = false;
            GridBrick.Grid.Rows[0].Selected = false;
            ReportExcelExporter1.Export(GridBrick.GridHeaderLayout, GridCaption.Text, sheet1, 3);
        }

        #endregion

        #region Экспорт в PDF

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            ReportPDFExporter1.PageTitle = Label1.Text;
            ReportPDFExporter1.PageSubTitle = Label2.Text;
            ReportPDFExporter1.HeaderCellHeight = 50;

            Report report = new Report();

            ISection section1 = report.AddSection();
            for (int i = 0; i < GridBrick.Grid.Rows.Count; i++)
            {
                if (Convert.ToInt32(GridBrick.Grid.Rows[i].Cells[GridBrick.Grid.Columns.Count - 1].Text) == 0)
                {
                    int index = GridBrick.Grid.Rows[i].Cells[0].Text.IndexOf('\n');
                    GridBrick.Grid.Rows[i].Cells[0].Text = GridBrick.Grid.Rows[i].Cells[0].Text.Remove(index + 1);
                }
                else
                {
                    GridBrick.Grid.Rows[i].Cells[0].Text = "    " + GridBrick.Grid.Rows[i].Cells[0].Text;
                }
            }

            int j = 0;

            while (j < GridBrick.Grid.Rows.Count)
            {
                int first = j;
                string value = GridBrick.Grid.Rows[j].Cells[0].ToString();
                GridBrick.Grid.Rows[j].Cells[0].Style.BorderDetails.WidthBottom = 0;
                GridBrick.Grid.Rows[j + 1].Cells[0].Style.BorderDetails.WidthBottom = 0;
                GridBrick.Grid.Rows[j + 1].Cells[0].Style.BorderDetails.WidthTop = 0;
                GridBrick.Grid.Rows[j + 2].Cells[0].Style.BorderDetails.WidthTop = 0;
                GridBrick.Grid.Rows[j].Cells[0].Text = "";
                GridBrick.Grid.Rows[j + 1].Cells[0].Text = value;
                GridBrick.Grid.Rows[j + 2].Cells[0].Text = "";

                j = j + 3;
            }

            ReportPDFExporter1.Export(GridBrick.GridHeaderLayout, GridCaption.Text, section1);
        }

        #endregion
    }
}