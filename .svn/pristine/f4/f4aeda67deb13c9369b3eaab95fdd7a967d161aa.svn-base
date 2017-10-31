using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.UI.WebControls;
using Infragistics.Documents.Excel;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.WebUI.UltraWebGrid.DocumentExport;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using EndExportEventArgs = Infragistics.WebUI.UltraWebGrid.ExcelExport.EndExportEventArgs;

namespace Krista.FM.Server.Dashboards.reports.FO_0002_0017_02
{
    public partial class Default : CustomReportPage
    {
        private static bool IsSmallResolution
        {
            get { return CRHelper.GetScreenWidth < 900; }
        }

        private static int MinScreenWidth
        {
            get { return IsSmallResolution ? 750 : CustomReportConst.minScreenWidth; }
        }

        private static int MinScreenHeight
        {
            get { return IsSmallResolution ? 700 : CustomReportConst.minScreenHeight; }
        }

        #region Поля

        private DataTable dtGrid = new DataTable();
        private DataTable dtChart = new DataTable();
        private int firstYear = 2007;
        private int endYear = 2011;

        #endregion

        #region Параметры запроса

        // множество выбранных лет
        private CustomParam yearSet;
        // выбранный период
        private CustomParam selectedPeriod;
        // выбранный счет
        private CustomParam selectedBill;

        #endregion

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            #region Инициализация параметров запроса

            if (yearSet == null)
            {
                yearSet = UserParams.CustomParam("year_set");
            }
            if (selectedPeriod == null)
            {
                selectedPeriod = UserParams.CustomParam("selected_period");
            }
            if (selectedBill == null)
            {
                selectedBill = UserParams.CustomParam("selected_bill");
            }

            #endregion

            #region Настройка диаграммы

            UltraChart.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 50);
            UltraChart.Height = CRHelper.GetChartWidth(CustomReportConst.minScreenHeight / 2);

            UltraChart.ChartType = ChartType.StackBarChart;
            UltraChart.Border.Thickness = 0;
            UltraChart.ColumnChart.SeriesSpacing = 2;

            UltraChart.Tooltips.FormatString = "<ITEM_LABEL>";

            UltraChart.Axis.X.Extent = 50;
            UltraChart.Axis.Y.Extent = 100;

            UltraChart.Axis.Y.Labels.SeriesLabels.Font = new Font("Verdana", 10);
            UltraChart.Axis.Y.Labels.SeriesLabels.Orientation = TextOrientation.Horizontal;

            UltraChart.Axis.X.Labels.Font = new Font("Verdana", 8);
            UltraChart.Axis.X.Labels.Layout.Behavior = AxisLabelLayoutBehaviors.Auto;
            UltraChart.Axis.X.Labels.ItemFormatString = "<DATA_VALUE:P0>";
            UltraChart.Axis.X.Labels.Orientation = TextOrientation.Horizontal;

            UltraChart.Legend.Visible = true;
            UltraChart.Legend.Location = LegendLocation.Bottom;
            UltraChart.Legend.Font = new Font("Verdana", 10);
            UltraChart.Legend.SpanPercentage = 10;
            
            //UltraChart.Data.SwapRowsAndColumns = true;

            UltraChart.FillSceneGraph += new Infragistics.UltraChart.Shared.Events.FillSceneGraphEventHandler(UltraChart_FillSceneGraph);

            #endregion

            UltraWebGrid.Width = IsSmallResolution ? CRHelper.GetGridWidth(MinScreenWidth + 20) : CRHelper.GetGridWidth(MinScreenWidth - 30);
            UltraWebGrid.Height = IsSmallResolution ? CRHelper.GetGridHeight(MinScreenHeight - 300) : CRHelper.GetGridHeight(MinScreenHeight - 225);

            UltraGridExporter1.MultiHeader = true;
            UltraGridExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            UltraGridExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
            UltraGridExporter1.ExcelExporter.EndExport += new EndExportEventHandler(ExcelExporter_EndExport);
            UltraGridExporter1.ExcelExporter.HeaderCellExporting += new HeaderCellExportingEventHandler(ExcelExporter_HeaderCellExporting);

            UltraGridExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
            UltraGridExporter1.PdfExporter.BeginExport += new EventHandler<DocumentExportEventArgs>(PdfExporter_BeginExport);
            UltraGridExporter1.PdfExporter.EndExport += new EventHandler<Infragistics.WebUI.UltraWebGrid.DocumentExport.EndExportEventArgs>(PdfExporter_EndExport);

            UltraWebGrid.InitializeRow += new Infragistics.WebUI.UltraWebGrid.InitializeRowEventHandler(UltraWebGrid_InitializeRow);
            UltraWebGrid.DataBound += new EventHandler(UltraWebGrid_DataBound);

        }
                
        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            FillBill();
            //PopupInformer1.HelpPageUrl = "/reports/FO_0002_0017_02/Описание показателей_СЭиФЭА.doc";

            if (!Page.IsPostBack)
            {
                ComboYear.Width = 150;
                ComboYear.Title = "Годы";
                ComboYear.MultiSelect = true;
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, endYear));
                ComboYear.SetСheckedState(endYear.ToString(), true);
                ComboYear.SetСheckedState((endYear - 1).ToString(), true);
                ComboYear.SetСheckedState((endYear - 2).ToString(), true);
                ComboYear.SetСheckedState((endYear - 3).ToString(), true);

                ComboBill.Width = 300;
                ComboBill.Title = "Лицевой счет";
                ComboBill.FillValues(bill);
                ComboBill.MultiSelect = false;
                ComboBill.SetСheckedState("ОБРАЗОВАНИЕ", true);

                ComboFKR.Width = 420;
                ComboFKR.Title = "РзПр";
                ComboFKR.MultiSelect = true;
                ComboFKR.ParentSelect = true;
                ComboFKR.ShowSelectedValue = true;
                ComboFKR.FillDictionaryValues(CustomMultiComboDataHelper.FillFOFKRNames(DataDictionariesHelper.OutcomesFOFKRTypes, DataDictionariesHelper.OutcomesFOFKRLevels, false));
                ComboFKR.RemoveTreeNodeByName("Расходы бюджета - ИТОГО");
                // ComboFKR.SetСheckedState("Расходы бюджета - ИТОГО", true);

                //ComboFKR.SetСheckedState("ОБЩЕГОСУДАРСТВЕННЫЕ ВОПРОСЫ", true);
                //ComboFKR.SetСheckedState("НАЦИОНАЛЬНАЯ ОБОРОНА", true);
                //ComboFKR.SetСheckedState("НАЦИОНАЛЬНАЯ БЕЗОПАСНОСТЬ И ПРАВООХРАНИТЕЛЬНАЯ ДЕЯТЕЛЬНОСТЬ", true);
                //ComboFKR.SetСheckedState("НАЦИОНАЛЬНАЯ ЭКОНОМИКА", true);
                //ComboFKR.SetСheckedState("ЖИЛИЩНО-КОММУНАЛЬНОЕ ХОЗЯЙСТВО", true);
                //ComboFKR.SetСheckedState("ОХРАНА ОКРУЖАЮЩЕЙ СРЕДЫ", true);
                //ComboFKR.SetСheckedState("ОБРАЗОВАНИЕ", true);
                //ComboFKR.SetСheckedState("КУЛЬТУРА, КИНЕМАТОГРАФИЯ, СРЕДСТВА МАССОВОЙ ИНФОРМАЦИИ", true);
                //ComboFKR.SetСheckedState("ЗДРАВООХРАНЕНИЕ, ФИЗИЧЕСКАЯ КУЛЬТУРА И СПОРТ", true);
                //ComboFKR.SetСheckedState("СОЦИАЛЬНАЯ ПОЛИТИКА", true);
                //ComboFKR.SetСheckedState("МЕЖБЮДЖЕТНЫЕ ТРАНСФЕРТЫ", true);
            }

            selectedBill.Value = billDict[ComboBill.SelectedValue];

            string yearInterval = string.Empty;
            Collection<string> selectedValues = ComboYear.SelectedValues;
            if (selectedValues.Count > 0)
            {
                yearInterval = string.Format("за {0} {1}",
                    CRHelper.GetDigitIntervals(ComboYear.SelectedValuesString, ','),
                    ComboYear.SelectedValues.Count == 1 ? "год" : "годы");

                string yearSetStr = string.Empty;

                for (int i = 0; i < selectedValues.Count; i++)
                {
                    string year = selectedValues[i];
                    yearSetStr += string.Format("[Период__Период].[Период__Период].[Данные всех периодов].[{0}],", year);
                }

                yearSetStr = yearSetStr.TrimEnd(',');
                yearSet.Value = yearSetStr;
            }
            else
            {
                PageSubTitle.Text = string.Empty;
                yearSet.Value = " ";
            }

            string RzPr = String.Empty;
            if (ComboFKR.SelectedValues.Count == 0)
            {
                UserParams.KDGroup.Value = @"[РзПр__Сопоставимый].[РзПр__Сопоставимый].[Все коды ФКР].[ОБЩЕГОСУДАРСТВЕННЫЕ ВОПРОСЫ],
                [РзПр__Сопоставимый].[РзПр__Сопоставимый].[Все коды ФКР].[НАЦИОНАЛЬНАЯ ОБОРОНА],
                [РзПр__Сопоставимый].[РзПр__Сопоставимый].[Все коды ФКР].[НАЦИОНАЛЬНАЯ БЕЗОПАСНОСТЬ И ПРАВООХРАНИТЕЛЬНАЯ ДЕЯТЕЛЬНОСТЬ],
                [РзПр__Сопоставимый].[РзПр__Сопоставимый].[Все коды ФКР].[НАЦИОНАЛЬНАЯ ЭКОНОМИКА],
                [РзПр__Сопоставимый].[РзПр__Сопоставимый].[Все коды ФКР].[ЖИЛИЩНО-КОММУНАЛЬНОЕ ХОЗЯЙСТВО],
                [РзПр__Сопоставимый].[РзПр__Сопоставимый].[Все коды ФКР].[ОХРАНА ОКРУЖАЮЩЕЙ СРЕДЫ],
                [РзПр__Сопоставимый].[РзПр__Сопоставимый].[Все коды ФКР].[ОБРАЗОВАНИЕ],
                [РзПр__Сопоставимый].[РзПр__Сопоставимый].[Все коды ФКР].[КУЛЬТУРА, КИНЕМАТОГРАФИЯ, СРЕДСТВА МАССОВОЙ ИНФОРМАЦИИ],
                [РзПр__Сопоставимый].[РзПр__Сопоставимый].[Все коды ФКР].[ЗДРАВООХРАНЕНИЕ, ФИЗИЧЕСКАЯ КУЛЬТУРА И СПОРТ],
                [РзПр__Сопоставимый].[РзПр__Сопоставимый].[Все коды ФКР].[СОЦИАЛЬНАЯ ПОЛИТИКА],
                [РзПр__Сопоставимый].[РзПр__Сопоставимый].[Все коды ФКР].[МЕЖБЮДЖЕТНЫЕ ТРАНСФЕРТЫ]";
                RzPr = "все";
            }
            else
            {
                string fkrGroup = string.Empty;
                foreach (string group in ComboFKR.SelectedValues)
                {
                    fkrGroup = string.Format("{0},{1}", fkrGroup, DataDictionariesHelper.OutcomesFKRTypes[group]);
                }
                fkrGroup = fkrGroup.TrimStart(',');

                UserParams.KDGroup.Value = fkrGroup;
                RzPr = ComboFKR.SelectedValuesString.ToLower();
            }

            Page.Title = "Исполнение расходов областного бюджета";
            PageTitle.Text = "Исполнение расходов областного бюджета";
            PageSubTitle.Text = String.Format("Исполнение расходов областного бюджета за {0}.<br/>Лицевой счет: {1}.<br/>Раздел, подраздел классификации расходов: {2}.", yearInterval, ComboBill.SelectedValue.ToLower(), RzPr.ToLower());

            UltraWebGrid.Bands.Clear();
            UltraWebGrid.DataBind();
            UltraChart.DataBind();
        }

        Collection<string> bill = new Collection<string>();
        private Dictionary<string, string> billDict = new Dictionary<string, string>();
        private void FillBill()
        {
            DataTable dtBill = new DataTable();
            string query = DataProvider.GetQueryText("FO_0002_0017_02_Bill");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "dummy", dtBill);

            
            foreach (DataRow row in dtBill.Rows)
            {
                bill.Add(String.Format("({0}) {1}",row[1], row[0]));
                billDict.Add(String.Format("({0}) {1}",row[1], row[0]), row[0].ToString());
            }
        }

        #region Обработчики грида

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            int start = Environment.TickCount;
            string query = DataProvider.GetQueryText(String.Format("FO_0002_0017_02_Grid"));
            dtGrid = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Показатель", dtGrid);
            CRHelper.SaveToUserAgentLog("Запрос грида" + (Environment.TickCount - start).ToString());

            for (int i = 6; i < dtGrid.Columns.Count; i++)
            {
                if (i % 5 != 1)
                {
                    dtGrid.Rows[dtGrid.Rows.Count - 2][i] = dtGrid.Rows[dtGrid.Rows.Count - 1][i];
                }
            }
            dtGrid.Rows.RemoveAt(dtGrid.Rows.Count - 1);
            dtGrid.AcceptChanges();
            ((UltraWebGrid)sender).DataSource = dtGrid;
        }

        protected void UltraWebGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.AllowSortingDefault = AllowSorting.No;
            e.Layout.FilterOptionsDefault.AllowRowFiltering = RowFiltering.No;

            if (e.Layout.Bands[0].Columns.Count == 0)
            {
                return;
            }

            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(195, 1280);
            e.Layout.Bands[0].Columns[1].Width = CRHelper.GetColumnWidth(30, 1280);
            e.Layout.Bands[0].Columns[2].Width = CRHelper.GetColumnWidth(30, 1280);
            e.Layout.Bands[0].Columns[3].Width = CRHelper.GetColumnWidth(50, 1280);
            e.Layout.Bands[0].Columns[4].Width = CRHelper.GetColumnWidth(40, 1280);
            e.Layout.Bands[0].Columns[5].Width = CRHelper.GetColumnWidth(60, 1280);

            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[1], "00");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[2], "00");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[3], "0000000");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[4], "000");

            for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
            {
                if (i == 0 || i == 1 || i == 2 || i == 3 || i == 4)
                {
                    e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.OriginY = 0;
                    e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.SpanY = 2;
                    if (i != 0)
                    {
                        e.Layout.Bands[0].Columns[i].Header.Caption =
                            e.Layout.Bands[0].Columns[i].Header.Caption.Split(';')[1];
                    }
                }
                else
                {
                    e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.OriginY = 1;
                }
            }

            e.Layout.Bands[0].Columns[5].Hidden = true;
            int multiHeaderPos = 6;

            for (int i = 6; i < e.Layout.Bands[0].Columns.Count - 1; i = i + 7)
            {
                ColumnHeader ch = new ColumnHeader(true);
                ch.Caption = IsSmallResolution ? RegionsNamingHelper.ShortName(e.Layout.Bands[0].Columns[i].Header.Caption.Split(';')[0].Trim(' ')) : e.Layout.Bands[0].Columns[i].Header.Caption.Split(';')[0];

                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i,
                                          e.Layout.Bands[0].Columns[i].Header.Caption.Split(';')[1], "Бюджетные ассигнования по плану расходов АС Бюджет");
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i + 1, e.Layout.Bands[0].Columns[i + 1].Header.Caption.Split(';')[1], "Факт расходов по АС Бюджет");
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i + 2, e.Layout.Bands[0].Columns[i + 2].Header.Caption.Split(';')[1], "");
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i + 3, e.Layout.Bands[0].Columns[i + 3].Header.Caption.Split(';')[1], "Удельный вес в общем объеме ассигнований");
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i + 4, e.Layout.Bands[0].Columns[i + 4].Header.Caption.Split(';')[1], "Удельный вес в общем объеме расходов");

                ch.RowLayoutColumnInfo.OriginY = 0;
                ch.RowLayoutColumnInfo.OriginX = multiHeaderPos;
                ch.RowLayoutColumnInfo.SpanY = 1;
                multiHeaderPos += 7;
                ch.RowLayoutColumnInfo.SpanX = 5;
                e.Layout.Bands[0].HeaderLayout.Add(ch);

                // CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "N0");
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
                e.Layout.Bands[0].Columns[i].CellStyle.Padding.Right = 5;

                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "N2");
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i + 1], "N2");
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i + 2], "P2");
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i + 3], "P2");
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i + 4], "P2");

                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(130, 1280);
                e.Layout.Bands[0].Columns[i + 1].Width = CRHelper.GetColumnWidth(130, 1280);
                e.Layout.Bands[0].Columns[i + 2].Width = CRHelper.GetColumnWidth(75, 1280);
                e.Layout.Bands[0].Columns[i + 3].Width = CRHelper.GetColumnWidth(83, 1280);
                e.Layout.Bands[0].Columns[i + 4].Width = CRHelper.GetColumnWidth(83, 1280);
                e.Layout.Bands[0].Columns[i + 5].Hidden = true;
                e.Layout.Bands[0].Columns[i + 6].Hidden = true;
            }
        }

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            int fontSize = 10;
            bool bold = true;
            bool italic = false;

            if (e.Row.Cells[5] != null && e.Row.Cells[5].Value.ToString() != string.Empty)
            {
                string level = e.Row.Cells[5].Value.ToString();

                switch (level)
                {
                    case "1":
                        {
                            fontSize = 10;
                            bold = true;
                            italic = false;
                            break;
                        }
                    case "2":
                        {
                            fontSize = 9;
                            bold = true;
                            italic = false;
                            break;
                        }
                    case "3":
                        {
                            fontSize = 9;
                            bold = false;
                            italic = false;
                            break;
                        }
                    case "4":
                        {
                            fontSize = 9;
                            bold = false;
                            italic = false;
                            break;
                        }
                    case "5":
                        {
                            fontSize = 9;
                            bold = false;
                            italic = false;
                            break; ;
                        }
                    case "6":
                        {
                            fontSize = 9;
                            bold = false;
                            italic = false;
                            break;
                        }
                }
            }

            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
                if (i == 1 || i == 2 || i == 3 || i == 4)
                {
                    // DoNothing();
                }
                else
                {
                    e.Row.Cells[i].Style.Font.Size = fontSize;
                    e.Row.Cells[i].Style.Font.Bold = bold;
                    e.Row.Cells[i].Style.Font.Italic = italic;
                    e.Row.Cells[i].Style.CustomRules = "background-repeat: no-repeat; background-position: left center";


                    if (!e.Row.Cells[0].Value.ToString().Contains("Итого") && (i % 7 == 2 || i % 7 == 3) &&
                        e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != string.Empty &&
                        e.Row.Cells[i + 2].Value != null && e.Row.Cells[i + 2].Value.ToString() != string.Empty)
                    {
                        if (Convert.ToDouble(e.Row.Cells[i].Value) >
                            Convert.ToDouble(e.Row.Cells[i + 2].Value))
                        {
                            e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowGreenUpBB.png";
                            e.Row.Cells[i].Title = "Удельный вес вырос с прошлого года";
                        }
                        else if (Convert.ToDouble(e.Row.Cells[i].Value) <
                                 Convert.ToDouble(e.Row.Cells[i + 2].Value))
                        {
                            e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowRedDownBB.png";
                            e.Row.Cells[i].Title = "Удельный вес снизился с прошлого года";
                        }
                    }
                }
            }

            string name = e.Row.Cells[0].Value.ToString();

            while (Char.IsDigit(name[0]))
            {
                name = name.Remove(0, 1);
            }
            e.Row.Cells[0].Value = name;
        }

        void UltraWebGrid_DataBound(object sender, EventArgs e)
        {

        }

        #endregion

        #region Обработчики диаграммы

        protected void UltraChart_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FO_0002_0017_02_Chart");
            dtChart = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Дата", dtChart);
            UltraChart.Series.Clear();

            for (int i = 1; i < dtChart.Columns.Count; i++)
            {
                dtChart.Columns[i].ColumnName = dtChart.Columns[i].ColumnName.Replace(";", " ");
                dtChart.Columns[i].ColumnName = dtChart.Columns[i].ColumnName.Replace("Уд. вес в ассигн. ", "План");
                dtChart.Columns[i].ColumnName = dtChart.Columns[i].ColumnName.Replace("Уд. вес в расходах ", "Факт");
            }

            for (int i = 3; i < dtChart.Columns.Count - 1; i += 4)
            {
                 UltraChart.Series.Add(CRHelper.GetNumericSeries(i, dtChart));
                 UltraChart.Series.Add(CRHelper.GetNumericSeries(i + 1, dtChart));
            }

            //UltraChart.DataSource = dtChart;
        }

        void UltraChart_FillSceneGraph(object sender, Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e)
        {
            for (int i = 0; i < e.SceneGraph.Count; i++)
            {
                Primitive primitive = e.SceneGraph[i];
                if (primitive is Box)
                {
                    Box box = (Box)primitive;
                    if (box.DataPoint != null)
                    {
                        box.DataPoint.Label =
                            String.Format("{0}<br/>{1}<br/>{2:P2}<br/>{3:N2} руб.", GetYearHint(box).Replace("План", "Уд. вес в ассигн. ").Replace("Факт", "Уд. вес в расходах "), GetFkrHint(box), GetPercentHint(box), GetValuetHint(box));
                        // Год, план/факт, раздел, доля, значение
                    }
                }
            } 
        }

        private object GetValuetHint(Box box)
        {
            return box.Row % 2 == 0 ? dtChart.Rows[box.Column][(box.Row + 1) * 2 + 1 - 2] : dtChart.Rows[box.Column][(box.Row + 1) * 2 - 2];
        }

        private object GetPercentHint(Box box)
        {
            return box.Row % 2 == 0 ? dtChart.Rows[box.Column][(box.Row + 1) * 2 + 1] : dtChart.Rows[box.Column][(box.Row + 1) * 2];
        }

        private object GetFkrHint(Box box)
        {
           return dtChart.Rows[box.Column][0];
        }

        private string GetYearHint(Box box)
        {
            return box.Row % 2 == 0 ? dtChart.Columns[(box.Row + 1) * 2 + 1].ColumnName : dtChart.Columns[(box.Row + 1) * 2].ColumnName;
        }


        #endregion

        #region Экспорт в Excel

        private void ExcelExporter_BeginExport(object sender, BeginExportEventArgs e)
        {
            e.CurrentWorksheet.Rows[0].Cells[0].Value = PageTitle.Text + " " + PageSubTitle.Text;
        }

        private void ExcelExporter_EndExport(object sender, EndExportEventArgs e)
        {
            int width = 100;
            e.CurrentWorksheet.Columns[0].Width = 50 * 37;
            e.CurrentWorksheet.Columns[1].Width = 250 * 37;
            e.CurrentWorksheet.Columns[2].Width = width * 37;
            e.CurrentWorksheet.Columns[3].Width = width * 37;
            e.CurrentWorksheet.Columns[4].Width = width * 37;
            e.CurrentWorksheet.Columns[5].Width = width * 37;
            e.CurrentWorksheet.Columns[6].Width = width * 37;
            e.CurrentWorksheet.Columns[7].Width = width * 37;
            e.CurrentWorksheet.Columns[8].Width = width * 37;
            e.CurrentWorksheet.Columns[9].Width = width * 37;
            e.CurrentWorksheet.Columns[10].Width = width * 37;
            e.CurrentWorksheet.Columns[11].Width = width * 37;
            e.CurrentWorksheet.Columns[12].Width = width * 37;
            e.CurrentWorksheet.Columns[13].Width = width * 37;
            e.CurrentWorksheet.Columns[14].Width = width * 37;

            e.CurrentWorksheet.Columns[0].CellFormat.FormatString = "0";
            e.CurrentWorksheet.Columns[1].CellFormat.FormatString = "";
            //e.CurrentWorksheet.Columns[2].CellFormat.FormatString = "#,##0.00;[Red]-#,##0.00";
            //e.CurrentWorksheet.Columns[3].CellFormat.FormatString = "#,##0.00;[Red]-#,##0.00";
            //e.CurrentWorksheet.Columns[4].CellFormat.FormatString = "#,##0.00;[Red]-#,##0.00";
            //e.CurrentWorksheet.Columns[5].CellFormat.FormatString = "#,##0.00;[Red]-#,##0.00";
            //e.CurrentWorksheet.Columns[6].CellFormat.FormatString = "#,##0.00;[Red]-#,##0.00";
            //e.CurrentWorksheet.Columns[7].CellFormat.FormatString = "#,##0.00";
            //e.CurrentWorksheet.Columns[8].CellFormat.FormatString = "#,##0.00";
            //e.CurrentWorksheet.Columns[9].CellFormat.FormatString = "#,##0.00";
            //e.CurrentWorksheet.Columns[10].CellFormat.FormatString = "#,##0.00";
            //e.CurrentWorksheet.Columns[11].CellFormat.FormatString = "#,##0.00";
            //e.CurrentWorksheet.Columns[12].CellFormat.FormatString = "#,##0.00";
            //e.CurrentWorksheet.Columns[13].CellFormat.FormatString = "#,##0.00";
            //e.CurrentWorksheet.Columns[14].CellFormat.FormatString = "#,##0.00";
            //e.CurrentWorksheet.Columns[15].CellFormat.FormatString = "#,##0.00";

            int columnCount = UltraWebGrid.Columns.Count;
            int rowsCount = UltraWebGrid.Rows.Count;

            // расставляем стили у начальных колонок
            for (int i = 4; i < rowsCount; i++)
            {
                e.CurrentWorksheet.Rows[i].Cells[0].CellFormat.VerticalAlignment = VerticalCellAlignment.Center;
                e.CurrentWorksheet.Rows[i].Cells[0].CellFormat.Alignment = HorizontalCellAlignment.Center;

                e.CurrentWorksheet.Rows[i].Cells[1].CellFormat.VerticalAlignment = VerticalCellAlignment.Top;
                e.CurrentWorksheet.Rows[i].Cells[1].CellFormat.WrapText = ExcelDefaultableBoolean.True;
                e.CurrentWorksheet.Rows[i].Height = 20 * 37;
            }

            // расставляем стили у ячеек
            for (int i = 2; i < columnCount; i += 2)
            {
                for (int j = 4; j < rowsCount; j++)
                {
                    e.CurrentWorksheet.Rows[j].Cells[i].CellFormat.Alignment = HorizontalCellAlignment.Right;
                    e.CurrentWorksheet.Rows[j].Cells[i].CellFormat.VerticalAlignment = VerticalCellAlignment.Center;
                    e.CurrentWorksheet.Rows[j].Cells[i].CellFormat.FormatString = "#,##0.####;[Red]-#,##0.####";
                    double value;
                    if (e.CurrentWorksheet.Rows[j].Cells[i].Value != null && Double.TryParse(e.CurrentWorksheet.Rows[j].Cells[i].Value.ToString(), out value))
                    {
                        e.CurrentWorksheet.Rows[j].Cells[i].Value = value;
                    }

                    e.CurrentWorksheet.Rows[j].Cells[i + 1].CellFormat.Alignment = HorizontalCellAlignment.Right;
                    e.CurrentWorksheet.Rows[j].Cells[i + 1].CellFormat.VerticalAlignment = VerticalCellAlignment.Center;
                    e.CurrentWorksheet.Rows[j].Cells[i + 1].CellFormat.FormatString = "#,##0";

                    int value1;
                    if (e.CurrentWorksheet.Rows[j].Cells[i + 1].Value != null && Int32.TryParse(e.CurrentWorksheet.Rows[j].Cells[i + 1].Value.ToString(), out value1))
                    {
                        e.CurrentWorksheet.Rows[j].Cells[i + 1].Value = value1;
                    }

                    //e.CurrentWorksheet.Workbook.Styles.
                }
            }
        }

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            UltraGridExporter1.ExcelExporter.ExcelStartRow = 4;

            UltraGridExporter1.ExcelExporter.DownloadName = "report.xls";
            UltraGridExporter1.ExcelExporter.Export(UltraWebGrid);
        }

        private void ExcelExporter_HeaderCellExporting(object sender, HeaderCellExportingEventArgs e)
        {
            e.HeaderText = UltraWebGrid.DisplayLayout.Bands[0].Columns[e.CurrentColumnIndex].Header.Key.Split(';')[0];
        }

        #endregion

        #region Экспорт в PDF

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            UltraGridExporter1.PdfExporter.DownloadName = "report.pdf";
            UltraGridExporter1.PdfExporter.Export(UltraWebGrid);
        }

        private void PdfExporter_BeginExport(object sender, DocumentExportEventArgs e)
        {
            IText title = e.Section.AddText();
            Font font = new Font("Verdana", 16);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.Style.Font.Bold = true;
            title.AddContent(PageTitle.Text);

            title = e.Section.AddText();
            font = new Font("Verdana", 14);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.AddContent(PageSubTitle.Text);
        }

        private void PdfExporter_EndExport(object sender, Infragistics.WebUI.UltraWebGrid.DocumentExport.EndExportEventArgs e)
        {
            e.Section.AddPageBreak();

            IText title = e.Section.AddText();
            Font font = new Font("Verdana", 14);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);

        }

        #endregion
    }
}