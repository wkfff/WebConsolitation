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

namespace Krista.FM.Server.Dashboards.reports.STAT_0023_0005
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
        private string gridIndicatorCaption = "Показатели";
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

        private bool IsMidResolution
        {
            get { return GetScreenWidth < 1050; }
        }

        private int MinScreenWidth
        {
            get 
            {
                if (IsSmallResolution)
                {
                    return 800;
                }
                else
                {
                    return IsMidResolution ? 1024 : CustomReportConst.minScreenWidth;
                } 
            }
        }

        private int MinScreenHeight
        {
            get
            {
                if (IsSmallResolution)
                {
                    return 600;
                }
                else
                {
                    return IsMidResolution ? 768 : CustomReportConst.minScreenHeight;
                }
            }
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

        public SliceType SliceType
        {
            get
            {
                switch (SliceTypeButtonList.SelectedIndex)
                {
                    case 0:
                        {
                            return SliceType.MiniOrg;
                        }
                    case 1:
                        {
                            return SliceType.MicroOrg;
                        }
                }
                return SliceType.MiniOrg;
            }
        }

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            #region Настройка грида

            GridBrick.AutoSizeStyle = GridAutoSizeStyle.AutoHeight;
            GridBrick.AutoHeightRowLimit = 15;
            GridBrick.Height = Convert.ToInt32(CustomReportConst.minScreenHeight * 0.7 - 235);
            GridBrick.Grid.InitializeLayout += new InitializeLayoutEventHandler(Grid_InitializeLayout);
            GridBrick.Grid.InitializeRow += new InitializeRowEventHandler(Grid_InitializeRow);
            GridBrick.Grid.ActiveRowChange += new ActiveRowChangeEventHandler(Grid_ActiveRowChange);
            GridBrick.Grid.DataBound += new EventHandler(Grid_DataBound);
            #endregion

            #region Настройка диаграмм
            //круговая диаграмма
            UltraChart1.Width = Convert.ToInt32(MinScreenWidth/2 - 60);
            UltraChart1.Height = Convert.ToInt32(CustomReportConst.minScreenHeight * 0.6 - 100);
            UltraChart1.ChartType = ChartType.PieChart; 
            System.Drawing.Font font = new System.Drawing.Font("Verdana", 9);
            UltraChart1.Border.Thickness = 0;
            UltraChart1.Axis.X.Extent = 130;
            UltraChart1.Axis.Y.Extent = 130;
            UltraChart1.PieChart.Labels.Font = font;
            UltraChart1.TitleTop.Text = hiddenmeasureLabel.Text;
            UltraChart1.TitleTop.Extent = 30;
            UltraChart1.TitleTop.Font = new Font("Verdana", 10);
            UltraChart1.TitleTop.HorizontalAlign = StringAlignment.Center;
            UltraChart1.Tooltips.FormatString = string.Format("<ITEM_LABEL>\n<DATA_VALUE:N0>, {0}", hiddenmeasureLabel.Text);
            UltraChart1.Legend.Visible = false;
            UltraChart1.Legend.Location = LegendLocation.Bottom;
            UltraChart1.Legend.SpanPercentage = 30;
            UltraChart1.Legend.Margins.Right = 10;
            UltraChart1.Legend.Margins.Left = 10;
            UltraChart1.Legend.Margins.Top = 10;
            UltraChart1.Legend.Font = font;
            UltraChart1.PieChart.RadiusFactor = 80;
            UltraChart1.PieChart.StartAngle = 260;
            UltraChart1.Data.SwapRowsAndColumns = false;
            UltraChart1.PieChart.OthersCategoryPercent = 0;
            UltraChart1.ColorModel.ModelStyle = ColorModels.CustomSkin;
            for (int i = 1; i < 19; i++)
            {
                Color color = GetCustomColor(i);
                UltraChart1.ColorModel.Skin.PEs.Add(CRHelper.GetFillPaintElement(color, 150));
            }
            UltraChart1.InvalidDataReceived += new ChartDataInvalidEventHandler(UltraChart1_InvalidDataReceived);

            UltraChart2.Width = Convert.ToInt32(MinScreenWidth/2 - 60);
            UltraChart2.Height = Convert.ToInt32(CustomReportConst.minScreenHeight * 0.6 - 100);
            UltraChart2.ChartType = ChartType.StackColumnChart;
            UltraChart2.Border.Thickness = 0;
            UltraChart2.TitleLeft.Visible = true;
            UltraChart2.TitleLeft.Text = "";
            UltraChart2.TitleLeft.Extent = 25;
            UltraChart2.TitleLeft.Font = new Font("Verdana", 9);
            UltraChart2.TitleLeft.HorizontalAlign = StringAlignment.Center;
            UltraChart2.TitleTop.Text = "";
            UltraChart2.TitleTop.Extent = 30;
            UltraChart2.TitleTop.Font = new Font("Verdana", 10);
            UltraChart2.TitleTop.HorizontalAlign = StringAlignment.Center;
            UltraChart2.ColorModel.ModelStyle = ColorModels.CustomSkin;
            for (int i = 1; i < 19; i++)
            {
                Color color = GetCustomColor(i);
                UltraChart2.ColorModel.Skin.PEs.Add(CRHelper.GetFillPaintElement(color, 150));
            }

            UltraChart2.Legend.Visible = false;
            UltraChart2.Legend.Location = LegendLocation.Bottom;
            UltraChart2.Legend.SpanPercentage = 40;
        
            UltraChart2.ColorModel.Skin.ApplyRowWise = false;
            UltraChart2.Axis.X.Extent = 40;
            UltraChart2.Axis.Y.Extent = 60;
            UltraChart2.Axis.X.Labels.Orientation = TextOrientation.Horizontal;
            UltraChart2.Data.ZeroAligned = true;
            UltraChart2.Tooltips.FormatString = string.Format("<ITEM_LABEL> год\n<SERIES_LABEL>\n<b><DATA_VALUE:N0></b>, {0}", hiddenmeasureLabel.Text);
            UltraChart2.Data.SwapRowsAndColumns = true;
            UltraChart2.InvalidDataReceived += new ChartDataInvalidEventHandler(UltraChart2_InvalidDataReceived);

            LegendUltraChart.Width = Convert.ToInt32(MinScreenWidth - 60);
            LegendUltraChart.Height = IsMidResolution ? Convert.ToInt32(CustomReportConst.minScreenHeight * 0.6 - 100) : Convert.ToInt32(CustomReportConst.minScreenHeight * 0.38 - 100);
            LegendUltraChart.ChartType = ChartType.StackColumnChart;
            System.Drawing.Font font1 = new System.Drawing.Font("Verdana", 8);
            LegendUltraChart.Border.Thickness = 0;
            LegendUltraChart.Legend.Visible = true;
            LegendUltraChart.Legend.Location = LegendLocation.Top;
            LegendUltraChart.Legend.SpanPercentage = 100;
            LegendUltraChart.Legend.Font = font1;
            LegendUltraChart.Tooltips.Display = TooltipDisplay.Never;
            LegendUltraChart.Axis.X.Visible = false;
            LegendUltraChart.Axis.X.Labels.Visible = false;
            LegendUltraChart.Axis.Y.Visible = false;
            LegendUltraChart.Data.SwapRowsAndColumns = false;
            LegendUltraChart.ColorModel.ModelStyle = ColorModels.CustomSkin;
            for (int i = 1; i < 19; i++)
            {
                Color color = GetCustomColor(i);
                LegendUltraChart.ColorModel.Skin.PEs.Add(CRHelper.GetFillPaintElement(color, 150));
            }
            LegendUltraChart.InvalidDataReceived += new ChartDataInvalidEventHandler(LegendUltraChart_InvalidDataReceived);
              
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
                case 10:
                    {
                        return Color.AliceBlue;
                    }
                case 11:
                    {
                        return Color.Aqua;
                    }
                case 12:
                    {
                        return Color.Beige;
                    }
                case 13:
                    {
                        return Color.BurlyWood;
                    }
                case 14:
                    {
                        return Color.SeaGreen;
                    }
                case 15:
                    {
                        return Color.DarkMagenta;
                    }
                case 16:
                    {
                        return Color.DarkRed;
                    }
                case 17:
                    {
                        return Color.Honeydew;
                    }
                case 18:
                    {
                        return Color.Indigo;
                    }
                case 19:
                    {
                        return Color.Purple;
                    }
            }
            return Color.White;
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            if (!Page.IsPostBack)
            {
                chartWebAsyncPanel.AddRefreshTarget(ChartCaption.ClientID);
                chartWebAsyncPanel.AddRefreshTarget(hiddenindicatorLabel.ClientID);
                chartWebAsyncPanel.AddRefreshTarget(hiddenmeasureLabel.ClientID);
                chartWebAsyncPanel.AddRefreshTarget(UltraChart1);
                chartWebAsyncPanel.AddRefreshTarget(UltraChart2);
                chartWebAsyncPanel.AddRefreshTarget(LegendUltraChart);
                chartWebAsyncPanel.AddLinkedRequestTrigger(GridBrick.Grid.ClientID);
                
                ComboYear.Title = "Год";
                ComboYear.Width = 100;
                ComboYear.MultiSelect = true;
                periodDigest = new MemberAttributesDigest(DataProvidersFactory.SpareMASDataProvider, "STAT_0023_0005_periodDigest");
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillMemberUniqueNameList(periodDigest.UniqueNames, periodDigest.MemberLevels));
                
                lastDate = CubeInfo.GetLastDate(DataProvidersFactory.SpareMASDataProvider, "STAT_0023_0005_lastDate");
                ComboYear.SetСheckedState(lastDate.Year.ToString(), true);
                ComboYear.SetСheckedState((lastDate.Year - 1).ToString(), true);
                ComboYear.SetСheckedState((lastDate.Year - 2).ToString(), true);
                ComboYear.SetСheckedState((lastDate.Year - 3).ToString(), true);
                ComboYear.SetСheckedState((lastDate.Year - 4).ToString(), true);

                chartParam.Value = "Число малых предприятий на конец года";
                hiddenindicatorLabel.Text = "Число малых предприятий на конец года";
                hiddenmeasureLabel.Text = "единица";
                firstRowChange = false;
            }

            if (ComboYear.SelectedValues.Count == 0)
            {
                ComboYear.SetСheckedState(lastDate.Year.ToString(), true);
            }

            string periods = String.Empty;
            lastPeriod.Value = ComboYear.SelectedValues[ComboYear.SelectedValues.Count-1];

            foreach (string year in ComboYear.SelectedValues)
            {
                periods += String.Format("[Период__Год Квартал Месяц].[Период__Год Квартал Месяц].[{0}],", year);
            }
            periodSet.Value = periods.TrimEnd(',');
           
            string sliceText = String.Empty;
            switch (SliceType)
            {
                case SliceType.MiniOrg:
                    {
                        sliceSet.Value = "[Показатели для малых предприятий]";
                        GridCaption.Text = "Основные показатели деятельности малых предприятий";
                        break;
                    }
                case SliceType.MicroOrg:
                    {
                        sliceSet.Value = "[Показатели для микропредприятий]";
                        GridCaption.Text = "Основные показатели деятельности микропредприятий";
                        break;
                    }
            }

            Page.Title = String.Format("Показатели деятельности малых предприятий и микропредприятий");
            Label1.Text = Page.Title;
            Label2.Text = String.Format("Основные показатели деятельности малых предприятий и микропредприятий по данным органов государственной статистики, ХМАО-Югра, за {1} {2}", sliceText,
                    CRHelper.GetDigitIntervals(ComboYear.SelectedValuesString, ','),
                    ComboYear.SelectedValues.Count == 1 ? "год" : "годы");
            ChartCaption.Text = String.Format("Показатель \"{0}\" в разрезе ОКВЭД, {1}", chartParam.Value, hiddenmeasureLabel.Text);
        
            UltraChart1.Tooltips.FormatString = string.Format("<ITEM_LABEL>\n<DATA_VALUE:N0>, {0}", hiddenmeasureLabel.Text);
            UltraChart2.Tooltips.FormatString = string.Format("<ITEM_LABEL> год\n<SERIES_LABEL>\n<b><DATA_VALUE:N0></b>, {0}", hiddenmeasureLabel.Text);
            UltraChart2.TitleLeft.Text = hiddenmeasureLabel.Text;

            UltraChart1.TitleTop.Text = String.Format("Структура показателя за {0} год", lastPeriod.Value);
            UltraChart2.TitleTop.Text = String.Format("Структурная динамика показателя");
            GridDataBind();
        }

        #region Обработчики грида

        private void GridDataBind()
        {
            string query = DataProvider.GetQueryText("STAT_0023_0005_grid");
            gridDt = new DataTable();
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Наименование показателей", gridDt);
          
            if (gridDt.Rows.Count > 0)
            {
                if (gridDt.Columns.Count > 1)
                {
                    gridDt.Columns.RemoveAt(0);
                }

                for (int i = 0; i < gridDt.Rows.Count; i++)
                {
                    if (gridDt.Rows[i][gridDt.Columns.Count-1] == DBNull.Value)
                    {
                        gridDt.Rows[i][gridDt.Columns.Count - 1] = gridDt.Rows[i - 1][gridDt.Columns.Count - 1].ToString();
                    }

                    if (gridDt.Rows[i][gridDt.Columns.Count - 2] == DBNull.Value)
                    {
                        gridDt.Rows[i][gridDt.Columns.Count - 2] = gridDt.Rows[i - 1][gridDt.Columns.Count - 2].ToString();
                    }

                    if (gridDt.Rows[i][gridDt.Columns.Count - 4] == DBNull.Value)
                    {
                        gridDt.Rows[i][gridDt.Columns.Count - 4] = gridDt.Rows[i - 1][gridDt.Columns.Count - 4].ToString();
                    }

                    if (gridDt.Rows[i][gridDt.Columns.Count - 5] == DBNull.Value)
                    {
                        gridDt.Rows[i][gridDt.Columns.Count - 5] = gridDt.Rows[i - 1][gridDt.Columns.Count - 5].ToString();
                    }
                }
                bool f = true;
                int k = gridDt.Columns.Count - 6;
                while (k > 0)
                {
                    int j = 0;
                    while ((j < gridDt.Rows.Count - 1) && (f))
                    {
                        if ((Convert.ToInt32(gridDt.Rows[j][gridDt.Columns.Count - 2]) == 0) && (Convert.ToInt32(gridDt.Rows[j][gridDt.Columns.Count - 3]) == 0))
                        {
                            if (gridDt.Rows[j][k] != DBNull.Value)
                            {
                                f = false;
                            }
                        }
                        j++;
                    }
                    if (f)
                    {
                        gridDt.Columns.RemoveAt(k);
                    }
                    f = true;
                    k--;
                }

                f = true;
                int r = gridDt.Rows.Count - 1;

                while (r >= 0)
                {
                    if ((Convert.ToInt32(gridDt.Rows[r][gridDt.Columns.Count - 2]) == 0) && (Convert.ToInt32(gridDt.Rows[r][gridDt.Columns.Count - 3]) == 0))
                    {
                        int c = 1;
                        while ((c < gridDt.Columns.Count - 5) && (f))
                        {
                            if (gridDt.Rows[r][c] != DBNull.Value)
                            {
                                f = false;
                            }
                            c++;
                        }
                        if (f)
                        {
                            int i = r + 3;
                            while ((i < gridDt.Rows.Count) && (Convert.ToInt32(gridDt.Rows[i][gridDt.Columns.Count - 2]) == 1))
                            {
                                i++;
                            }
                            for (int j = i - 1; j >= r; j--)
                            {
                                gridDt.Rows.RemoveAt(j);
                            }
                        }
                        f = true;
                    }
                    r--;
                }

                if (gridDt.Columns.Count > 6)
                {
                    if (gridDt.Columns.Count > 14)
                    {
                        GridBrick.Width = Convert.ToInt32(MinScreenWidth - 25);
                    }
                    else
                    {
                        GridBrick.Width = IsSmallResolution ? 720 : IsMidResolution ? 950 : CRHelper.GetGridWidth(340 + (gridDt.Columns.Count - 5) * columnWidth + 20);
                    }

                    string periods = string.Empty;
                    for (int i = 1; i < gridDt.Columns.Count - 5; i++)
                    {
                        periods += gridDt.Columns[i].Caption + ", ";
                    }
                    periods = periods.Remove(periods.Length - 2);
                    Label2.Text = String.Format("Основные показатели деятельности малых предприятий и микропредприятий по данным органов государственной статистики, ХМАО-Югра, за {0} {1}",
                        CRHelper.GetDigitIntervals(periods, ','),
                        periods.Length == 4 ? "год" : "годы");
                    GridBrick.DataTable = gridDt;
                }
                else
                {
                    for (int i = gridDt.Rows.Count - 1; i >= 0; i--)
                    {
                        gridDt.Rows.RemoveAt(i);
                    }
                }
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
            e.Layout.Bands[0].Columns[0].AllowUpdate = AllowUpdate.No;
            int columnCount = e.Layout.Bands[0].Columns.Count;
            e.Layout.Bands[0].Columns[columnCount - 1].Hidden = true;
            e.Layout.Bands[0].Columns[columnCount - 2].Hidden = true;
            e.Layout.Bands[0].Columns[columnCount - 3].Hidden = true;
            e.Layout.Bands[0].Columns[columnCount - 4].Hidden = true;
            e.Layout.Bands[0].Columns[columnCount - 5].Hidden = true;

            GridHeaderLayout headerLayout = GridBrick.GridHeaderLayout;

            headerLayout.AddCell(gridIndicatorCaption);

            for (int i = 1; i < columnCount - 5; i = i + 1)
            {
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(100);
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;

                string year = e.Layout.Bands[0].Columns[i].Header.Caption;
                headerLayout.AddCell(String.Format("{0} год", year));
            }

            headerLayout.ApplyHeaderInfo();
        }

        protected void Grid_InitializeRow(object sender, RowEventArgs e)
        {
            int cellCount = e.Row.Cells.Count;
            //тип строки
            int type = 0;
            if (e.Row.Cells[cellCount - 3].Value != null)
            {
                type = Convert.ToInt32(e.Row.Cells[cellCount - 3].Value.ToString());
            }

            //единица измерения строки
            string measure = "";
            if (e.Row.Cells[cellCount - 5].Value != null)
            {
                measure = e.Row.Cells[cellCount - 5].Value.ToString().ToLower();
            }

            if (e.Row.Cells[0].Text == "Все виды экономической деятельности")
            {
                e.Row.Cells[0].Text = e.Row.Cells[e.Row.Cells.Count - 1].Text;
            }

            //уровень строки
            if (e.Row.Cells[cellCount - 2].Value != DBNull.Value)
            {
                if (Convert.ToInt32(e.Row.Cells[cellCount - 2].Text) == 0)
                {
                    e.Row.Cells[0].Style.Padding.Left = 20;
                    e.Row.Cells[0].Style.Font.Bold = true;
                    for (int i = 1; i < cellCount - 5; i++)
                    {
                        if (type == 0)
                        {
                            e.Row.Cells[i].Style.Font.Bold = true;
                        }
                    }

                    e.Row.Cells[0].Value = e.Row.Cells[0].Value + ", " + measure + String.Format(@"
                 <table style='border-collapse: collapse;border-style: none' width='100%'> 
                   <tr>
                     <td class='ExpandBlockSecondState' onclick='resize(this)' style='padding-left:20px;background-position:5px Center;font-weight: Normal;'>по&nbsp;ОКВЭД</td>
                   </tr>
                 </table>");
                }
                if (Convert.ToInt32(e.Row.Cells[cellCount - 2].Text) == 1)
                {
                    e.Row.Cells[0].Style.Padding.Left = 40;
                    e.Row.Cells[cellCount - 2].Style.CssClass = "hiddenLevel";
                }
            }

            for (int i = 1; i < cellCount - 5; i++)
            {               
                UltraGridCell cell = e.Row.Cells[i];
                cell.Style.Padding.Right = 3;            

                string[] nameParts = e.Row.Band.Columns[i].Header.Caption.Split(' ');
                int prevYear = lastDate.Year;
                if (nameParts.Length > 0)
                {
                    prevYear = Convert.ToInt32(nameParts[0]) - 1;
                }

                switch (type)
                {
                    case 0:
                        {
                            if (cell.Value != null)
                            {
                                if ((measure == "единица") || (measure == "человек"))
                                {
                                    cell.Value = Convert.ToDouble(cell.Value.ToString()).ToString("N0");
                                }
                                else
                                {
                                    cell.Value = Convert.ToDouble(cell.Value.ToString()).ToString("N2");
                                }
                            }
                            cell.Style.BorderDetails.WidthBottom = 0;
                            break;
                        }
                    case 1:
                        {
                            if (cell.Value != null)
                            {
                                if ((measure == "единица") || (measure == "человек"))
                                {
                                    cell.Value = Convert.ToDouble(cell.Value.ToString()).ToString("N0");
                                }
                                else
                                {
                                    cell.Value = Convert.ToDouble(cell.Value.ToString()).ToString("N2");
                                }
                                cell.Title = String.Format("Прирост к {0} году", prevYear);
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
                                cell.Title = String.Format("Темп прироста к {0} году", prevYear);
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
            if (!chartWebAsyncPanel.IsAsyncPostBack)
            {
                UltraGridRow row = CRHelper.FindGridRow(GridBrick.Grid, chartParam.Value, GridBrick.Grid.Columns.Count-4, 0);
                ActiveGridRow(row);
            }
        }

        private void ActiveGridRow(UltraGridRow row)
        {
            if (row == null)
                return;

            if (!activeRowChange)
                return;

            string measure = "";
            if (Convert.ToInt32(row.Cells[row.Cells.Count - 2].Value) == 0)
            {
                string value = row.Cells[0].Value.ToString();
                hiddenindicatorLabel.Text = GetName(value);
            }
            else
            {
                string value = row.Cells[row.Cells.Count-1].Value.ToString();
                hiddenindicatorLabel.Text = value;
            }
            chartParam.Value = hiddenindicatorLabel.Text;
            measure = row.Cells[row.Cells.Count - 5].Value.ToString().ToLower();
            hiddenmeasureLabel.Text = measure;
            ChartCaption.Text = String.Format("Показатель \"{0}\" в разрезе ОКВЭД, {1}", chartParam.Value, hiddenmeasureLabel.Text);
            
            if ((hiddenmeasureLabel.Text == "единица") || (hiddenmeasureLabel.Text == "человек"))
            {
                UltraChart1.Tooltips.FormatString = string.Format("<ITEM_LABEL>\n<DATA_VALUE:N0>, {0}", hiddenmeasureLabel.Text);
                UltraChart2.Tooltips.FormatString = string.Format("<ITEM_LABEL> год\n<SERIES_LABEL>\n<b><DATA_VALUE:N0></b>, {0}", hiddenmeasureLabel.Text);
            }
            else
            {
                UltraChart1.Tooltips.FormatString = string.Format("<ITEM_LABEL>\n<DATA_VALUE:N2>, {0}", hiddenmeasureLabel.Text);
                UltraChart2.Tooltips.FormatString = string.Format("<ITEM_LABEL> год\n<SERIES_LABEL>\n<b><DATA_VALUE:N2></b>, {0}", hiddenmeasureLabel.Text);
            }
           
            UltraChart2.TitleLeft.Text = hiddenmeasureLabel.Text;

            UltraChart1.TitleTop.Text = String.Format("Структура показателя за {0} год", lastPeriod.Value);
            UltraChart2.TitleTop.Text = String.Format("Структурная динамика показателя");
            UltraChart1.DataBind();
            UltraChart2.DataBind();
            LegendUltraChart.DataBind();
            activeRowChange = false;
        }

        protected void Grid_ActiveRowChange(object sender, RowEventArgs e)
        {
            ActiveGridRow(e.Row);
        }

        private string GetName(string value)
        {
            int index = value.IndexOf(',');
            value = value.Remove(index);
            return value;
        }

        #endregion

        #region Обработчики диаграммы

        protected void UltraChart1_DataBinding(object sender, EventArgs e)
        {
            string queryText = DataProvider.GetQueryText("STAT_0023_0005_chart1");
            chart1Dt = new DataTable();
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(queryText, "Показатель", chart1Dt);
            if (chart1Dt.Rows.Count > 0)
            {
           
                    UltraChart1.DataSource = chart1Dt;

            }
        }

        protected void UltraChart1_InvalidDataReceived(object sender, ChartDataInvalidEventArgs e)
        {
            e.Text = "Нет данных";
            e.LabelStyle.FontColor = Color.Black;
            e.LabelStyle.FontSizeBestFit = false;
            e.LabelStyle.HorizontalAlign = StringAlignment.Center;
            e.LabelStyle.VerticalAlign = StringAlignment.Center;
        }

        protected void UltraChart2_DataBinding(object sender, EventArgs e)
        {
            string queryText = DataProvider.GetQueryText("STAT_0023_0005_chart2");
            chart2Dt = new DataTable();
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(queryText, "Показатель", chart2Dt);
            if (chart2Dt.Columns.Count > 0)
            {
                if (chart2Dt.Columns.Count > 1)
                {
                    chart2Dt.Columns.RemoveAt(0);
                }

                bool f = true;
                int k = chart2Dt.Columns.Count - 1;

                while (k > 0)
                {
                    int j = 0;
                    while ((j < chart2Dt.Rows.Count - 1) && (f))
                    {
                        if ((chart2Dt.Rows[j][k] != DBNull.Value) && (Convert.ToInt32(chart2Dt.Rows[j][k]) != 0))
                        {
                            f = false;
                        }
                        j++;
                    }
                    if (f)
                    {
                        chart2Dt.Columns.RemoveAt(k);
                    }
                    f = true;
                    k--;
                }

                UltraChart2.DataSource = chart2Dt;
                
            }
        }

        protected void UltraChart2_InvalidDataReceived(object sender, ChartDataInvalidEventArgs e)
        {
            e.Text = "Нет данных";
            //Font font = new Font("Verdana", 10);
            //e.LabelStyle.Font = font;
            e.LabelStyle.FontColor = Color.Black;
            e.LabelStyle.FontSizeBestFit = false;
            e.LabelStyle.HorizontalAlign = StringAlignment.Center;
            e.LabelStyle.VerticalAlign = StringAlignment.Center;
        }

        protected void LegendUltraChart_DataBinding(object sender, EventArgs e)
        {
            legendChartDt = new DataTable();
            legendChartDt = chart2Dt.Copy();
           
            for (int i = 0; i < legendChartDt.Rows.Count; i++)
            {
                string value = legendChartDt.Rows[i][0].ToString();
               
                if (value.Length > 70)
                {
                    int j = 65;
                    while ((value[j] != ' ') && (j != value.Length-1))
                    {
                        j++;
                    }
                    value = value.Remove(j);
                    value += "...";
                    legendChartDt.Rows[i][0] = value;
                }
            }

            LegendUltraChart.DataSource = legendChartDt;
        }

        protected void LegendUltraChart_InvalidDataReceived(object sender, ChartDataInvalidEventArgs e)
        {
            e.Text = String.Empty;
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
                if (Convert.ToInt32(GridBrick.Grid.Rows[i].Cells[GridBrick.Grid.Columns.Count - 2].Text) == 0)
                {
                    int index = GridBrick.Grid.Rows[i].Cells[0].Text.IndexOf('\n');
                    GridBrick.Grid.Rows[i].Cells[0].Text = GridBrick.Grid.Rows[i].Cells[0].Text.Remove(index+1);
                }
                else
                {
                    GridBrick.Grid.Rows[i].Cells[0].Text = "        " + GridBrick.Grid.Rows[i].Cells[0].Text;
                }
            }
            GridBrick.Grid.Rows[0].Activated = false;
            GridBrick.Grid.Rows[0].Selected = false;
            ReportExcelExporter1.Export(GridBrick.GridHeaderLayout, GridCaption.Text, sheet1, 3);
            System.Drawing.Image img = GetMergeChartsImage();
            img = img.ScaleImage(0.9);
            Worksheet sheet2 = workbook.Worksheets.Add("Диаграммы");

            ReportExcelExporter1.Export(img, ChartCaption.Text, sheet2, 3);
        }

        private Graphics g;

        private System.Drawing.Image GetMergeChartsImage()
        {                    
            UltraChart1.Width = Convert.ToInt32(UltraChart1.Width.Value * 0.9);
            UltraChart2.Width = Convert.ToInt32(UltraChart2.Width.Value * 0.9);
            LegendUltraChart.Width = Convert.ToInt32(LegendUltraChart.Width.Value * 0.9);

            System.Drawing.Image chartImg1 = GetChartImage(UltraChart1);
            System.Drawing.Image chartImg2 = GetChartImage(UltraChart2);
            System.Drawing.Image chartImg3 = GetChartImage(LegendUltraChart);

            System.Drawing.Image img = new Bitmap(chartImg3.Width, chartImg1.Height + chartImg3.Height);
            g = Graphics.FromImage(img);

            g.DrawImage(chartImg1, 0, 0);
            g.DrawImage(chartImg2, chartImg1.Width, 0);
            g.DrawImage(chartImg3, 0, chartImg1.Height);
            
            return img;
        }

        private static System.Drawing.Image GetChartImage(UltraChart chart)
        {
            MemoryStream imageStream = new MemoryStream();
            chart.SaveTo(imageStream, ImageFormat.Png);
            return System.Drawing.Image.FromStream(imageStream);
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
                if (Convert.ToInt32(GridBrick.Grid.Rows[i].Cells[GridBrick.Grid.Columns.Count - 2].Text) == 0)
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

            ISection section2 = report.AddSection();
            ReportPDFExporter1.Export(GetMergeChartsImage(), ChartCaption.Text, section2);
        }

        #endregion
    }
}