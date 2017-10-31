using System;
using System.Collections;
using System.Data;
using System.IO;
//using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.Documents.Reports.Graphics;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Infragistics.Documents.Reports.Report.Tree;
using Infragistics.Documents.Excel;
using Infragistics.WebUI.UltraWebGrid.DocumentExport;
using Krista.FM.Server.Dashboards.Components;
using EndExportEventArgs = Infragistics.WebUI.UltraWebGrid.ExcelExport.EndExportEventArgs;
using InitializeRowEventHandler = Infragistics.WebUI.UltraWebGrid.InitializeRowEventHandler;
using SerializationFormat = Dundas.Maps.WebControl.SerializationFormat;
using System.Globalization;
using Dundas.Maps.WebControl;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Band;
using Infragistics.Documents.Reports.Report.Flow;
using Infragistics.Documents.Reports.Report.Grid;
using Infragistics.Documents.Reports.Report.Index;
using Infragistics.Documents.Reports.Report.QuickList;
using Infragistics.Documents.Reports.Report.QuickTable;
using Infragistics.Documents.Reports.Report.QuickText;
using Infragistics.Documents.Reports.Report.Section;
using Infragistics.Documents.Reports.Report.Segment;
using Infragistics.Documents.Reports.Report.Table;
using Infragistics.Documents.Reports.Report.TOC;
using ContentAlignment = Infragistics.Documents.Reports.Report.ContentAlignment;
using Font = System.Drawing.Font;
using IList = Infragistics.Documents.Reports.Report.List.IList;


namespace Krista.FM.Server.Dashboards.reports.FK_0001_0012
{

    public partial class DefaultAssessment : CustomReportPage
    {

        private int curYear;

        private bool IsSmallResolution
        {
            get { return CRHelper.GetScreenWidth < 1200; }
        }

        private int minScreenWidth
        {
            get { return IsSmallResolution ? 780 : CustomReportConst.minScreenWidth; }//750
        }

        private int minScreenHeight
        {
            get { return IsSmallResolution ? 700 : CustomReportConst.minScreenHeight; }
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            UltraWebGrid.Height = Unit.Empty;//CRHelper.GetGridHeight(minScreenHeight * 0.3544); //0.3354
            UltraWebGrid.Width = CRHelper.GetGridWidth(minScreenWidth - 0);

            UltraWebGrid1.Height = Unit.Empty;//CRHelper.GetGridHeight(minScreenHeight * 0.6574);
            UltraWebGrid1.Width = CRHelper.GetGridWidth(minScreenWidth - 0);

            UltraWebGrid2.Height = Unit.Empty;// CRHelper.GetGridHeight(minScreenHeight * 0.1904);
            UltraWebGrid2.Width = CRHelper.GetGridWidth(minScreenWidth - 0);

            UltraWebGrid.DataBound += new EventHandler(UltraWebGrid_DataBound);

            // GridSearch1.LinkedGridId = UltraWebGrid.ClientID;
            UltraGridExporter1.MultiHeader = true;
            UltraGridExporter1.ExcelExporter.HeaderCellExporting += new HeaderCellExportingEventHandler(ExcelExporter_HeaderCellExporting);
            UltraGridExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            UltraGridExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
            UltraGridExporter1.PdfExporter.BeginExport += new EventHandler
                    <Infragistics.WebUI.UltraWebGrid.DocumentExport.DocumentExportEventArgs>(PdfExporter_BeginExport);

            UltraGridExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
            UltraGridExporter1.ExcelExporter.EndExport += new EndExportEventHandler(ExcelExporter_EndExport);

            if (!Page.IsPostBack)
            {
                DataTable dtDate = new DataTable();
                string query = DataProvider.GetQueryText("FK_0001_0012_date");
                DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
                int endYear = Convert.ToInt32(dtDate.Rows[0][0]);
                string month = dtDate.Rows[0][3].ToString();
                int firstYear = 2000;

                UserParams.PeriodYear.Value = endYear.ToString();
                UserParams.PeriodMonth.Value = month;

                ComboYear.Title = "Год";
                ComboYear.Width = 100;
                ComboYear.MultiSelect = false;
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, endYear));
                ComboYear.SetСheckedState(UserParams.PeriodYear.Value, true);

                ComboMonth.Title = "Месяц";
                ComboMonth.Width = 130;
                ComboMonth.MultiSelect = false;
                ComboMonth.FillDictionaryValues(CustomMultiComboDataHelper.FillMonthValues());
                ComboMonth.SetСheckedState(UserParams.PeriodMonth.Value, true);

                regionsCombo.Width = 300;
                regionsCombo.Title = "Субъект РФ";
                regionsCombo.FillDictionaryValues(CustomMultiComboDataHelper.FillRegions(RegionsNamingHelper.FoNames, RegionsNamingHelper.RegionsFoDictionary));
                regionsCombo.ParentSelect = false;
                if (!string.IsNullOrEmpty(UserParams.StateArea.Value))
                {
                    regionsCombo.SetСheckedState(UserParams.StateArea.Value, true);
                }
                else if (RegionSettings.Instance != null && RegionSettings.Instance.Name != String.Empty)
                {
                    regionsCombo.SetСheckedState(RegionSettings.Instance.Name, true);
                }
            }
            string stateAreaValue = UserParams.StateArea == null ? String.Empty : UserParams.StateArea.Value;

            UserParams.Region.Value = regionsCombo.SelectedNodeParent;
            UserParams.StateArea.Value = regionsCombo.SelectedValue;

            int monthNum = ComboMonth.SelectedIndex + 1;
            int yearNum = Convert.ToInt32(ComboYear.SelectedValue);
            curYear = yearNum;

            if (monthNum == 12)
            {
                monthNum = 1;
                yearNum++;
            }
            else
            {
                monthNum++;
            }

            Page.Title = string.Format("Оценка исполнения консолидированного бюджета субъекта РФ");
            Label1.Text = string.Format("Оценка исполнения консолидированного бюджета субъекта РФ<br/>"); ;
            Label2.Text = string.Format("{0}, данные за {2}-{1} гг.(тыс. руб.)",
                UserParams.StateArea.Value, yearNum, yearNum - 2);

            string monthValue = ComboMonth.SelectedValue;
            string yearValue = ComboYear.SelectedValue;


            UserParams.PeriodMonth.Value = ComboMonth.SelectedValue;
            UserParams.PeriodYear.Value = ComboYear.SelectedValue;

            UserParams.PeriodLastYear.Value = (Convert.ToInt32(ComboYear.SelectedValue) - 1).ToString();
            UserParams.PeriodEndYear.Value = (Convert.ToInt32(ComboYear.SelectedValue) - 2).ToString();
            UserParams.PeriodHalfYear.Value = string.Format("Полугодие {0}", CRHelper.HalfYearNumByMonthNum(ComboMonth.SelectedIndex + 1));
            UserParams.PeriodQuater.Value = string.Format("Квартал {0}", CRHelper.QuarterNumByMonthNum(ComboMonth.SelectedIndex + 1));
            UltraWebGrid.Bands.Clear();
            UltraWebGrid.DataBind();
            UltraWebGrid1.Bands.Clear();
            UltraWebGrid1.DataBind();
            UltraWebGrid2.Bands.Clear();
            UltraWebGrid2.DataBind();

        }

        #region Обработчики гридов

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FK_0001_0012_grid_incomes");
            DataTable dtGrid = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Субъект", dtGrid);
            UltraWebGrid.DataSource = dtGrid;

            for (int k = 0; k < dtGrid.Rows.Count; k++)
            {
                DataRow row = dtGrid.Rows[k];

                for (int i = 1; i < row.ItemArray.Length - 1; i++)
                {
                    if (row[i] != DBNull.Value)
                    {
                        if ((i != 13) & (i != 11)) row[i] = Convert.ToDouble(row[i]) / 1000;
                    }
                }
            }

        }

        protected void UltraWebGrid1_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FK_0001_0012_grid_outcomes");
            DataTable dtGrid1 = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Субъект", dtGrid1);
            UltraWebGrid1.DataSource = dtGrid1;

            for (int k = 0; k < dtGrid1.Rows.Count; k++)
            {
                DataRow row = dtGrid1.Rows[k];

                for (int i = 1; i < row.ItemArray.Length; i++)
                {
                    if (row[i] != DBNull.Value)
                    {
                        if ((i % 2 != 0) & (i != 24) & (i != 23) & (i != 21) & (i != 25))
                        {
                            row[i] = Convert.ToDouble(row[i]) / 1000;
                        }
                        else
                        {
                            if ((k == dtGrid1.Rows.Count - 1) & (i != 22))
                            {
                                row[i] = DBNull.Value;
                            }
                        }
                    }
                }
            }

        }

        protected void UltraWebGrid2_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FK_0001_0012_grid_sources");
            DataTable dtGrid2 = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Субъект", dtGrid2);
            UltraWebGrid2.DataSource = dtGrid2;

            for (int k = 0; k < dtGrid2.Rows.Count; k++)
            {
                DataRow row = dtGrid2.Rows[k];

                for (int i = 1; i < row.ItemArray.Length - 1; i++)
                {
                    if (row[i] != DBNull.Value)
                    {
                        if ((i != 13) & (i != 11)) row[i] = Convert.ToDouble(row[i]) / 1000;
                    }
                }
            }

        }

        protected void UltraWebGrid_DataBound(object sender, EventArgs e)
        {
            //            UltraWebGrid.Height = Unit.Empty;
            //            UltraWebGrid.Width = Unit.Empty;
        }

        protected void UltraWebGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;

            if (e.Layout.Bands[0].Columns.Count > 13)
            {
                e.Layout.Bands[0].HeaderStyle.Wrap = true;
                e.Layout.Bands[0].HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;

                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 0, "I. Доходы", "");

                for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
                {
                    e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = (i == 0) ?
                                                                             HorizontalAlign.Left :
                                                                             HorizontalAlign.Right;
                    double width;
                    switch (i)
                    {
                        case 0:
                            {
                                width = 250;
                                break;
                            }
                        case 11:
                        case 13:
                            {
                                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "P2");
                                width = 100;
                                break;
                            }
                        default:
                            {
                                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "N2");
                                width = 135;
                                break;
                            }
                    }
                    e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(width, 2050);
                }

            }

            for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
            {
                if (i == 0)
                {
                    e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.OriginY = 0;
                    e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.SpanY = 2;
                }
                else
                {
                    e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.OriginY = 1;
                }
            }

            int multiHeaderPos = 1;
            int dif = 2;

            for (int i = 1; i < e.Layout.Bands[0].Columns.Count - 4; i = i + 4)
            {
                int lastColumnGroup = e.Layout.Bands[0].Columns.Count - 5;
                DateTime date = new DateTime(curYear - dif, ComboMonth.SelectedIndex + 1, 1);
                date = date.AddMonths(1);

                if (i != lastColumnGroup)
                {
                    CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i, "Годовое исполнение по конс. бюджету", "Сумма годового фактического исполнения по конс.бюджету субъекта РФ");
                    CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i + 1, "В том числе бюджет субъекта", "Сумма годового фактического исполнения по бюджету субъекта РФ");
                    CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i + 2, string.Format("Исполнение по конс. бюджету с начала года (на 1 {0})", CRHelper.RusMonthGenitive(ComboMonth.SelectedIndex + 2)), string.Format("Сумма фактического исполнения по конс. бюджету субъекта РФ с начала года по состоянию на {0:dd.MM.yyyy} г.", date));
                    CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i + 3, string.Format("Исполнение по конс. бюджету (за {0})", ComboMonth.SelectedValue.ToLower()), string.Format("Сумма фактического исполнения по конс. бюджету субъекта РФ за {0} {1:yyyy} г.", ComboMonth.SelectedValue.ToLower(), date));
                }
                else
                {
                    CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i, "Годовое плановое назначение по конс. бюджету", string.Format("Сумма годового планового назначения по конс. бюджету субъекта РФ по состоянию на {0:dd.MM.yyyy} г.", date));
                    CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i + 1, "В том числе бюджет субъекта", string.Format("Сумма годового планового назначения по бюджету субъекта РФ по состоянию на {0:dd.MM.yyyy} г.", date));
                    CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i + 2, string.Format("Темп роста к {0} г. ", Convert.ToInt32(ComboYear.SelectedValue) - 1), string.Format("Tемп роста плановых годовых назначений по конс. бюджету субъекта РФ к {0} г.", Convert.ToInt32(ComboYear.SelectedValue) - 1));
                    CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i + 3, string.Format("Исполнение по конс. бюджету с начала года (на 1 {0})", CRHelper.RusMonthGenitive(ComboMonth.SelectedIndex + 2)), string.Format("Сумма фактического исполнения по конс. бюджету субъекта РФ с начала года по состоянию на {0:dd.MM.yyyy} г.", date));
                    CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i + 4, string.Format("Темп роста к аналогичному периоду {0} г.", Convert.ToInt32(ComboYear.SelectedValue) - 1), string.Format("Tемп роста фактического исполнения по конс. бюджету к аналогичному периоду {0} г.", Convert.ToInt32(ComboYear.SelectedValue) - 1));
                }

                int groupNum = i % 4;
                int groupCount = groupNum == 2 ? 5 : 4;
                string level2HeaderCaption = groupNum == 2 ? "План по данным субъекта РФ" : "исполнение год";

                if (dif == 0)
                {
                    groupCount = 5;
                }

                CRHelper.AddHierarchyHeader(e.Layout.Grid,
                    0,
                    string.Format("{0} год", curYear - dif),
                    multiHeaderPos,
                    0,
                    groupCount,
                    1);

                CRHelper.AddHierarchyHeader(e.Layout.Grid,
                    0,
                    level2HeaderCaption,
                    multiHeaderPos,
                    0,
                    groupCount,
                    1);
                dif = dif - 1;
                multiHeaderPos += groupCount;
            }
        }

        protected void UltraWebGrid1_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;

            if (e.Layout.Bands[0].Columns.Count > 13)
            {
                e.Layout.Bands[0].HeaderStyle.Wrap = true;
                e.Layout.Bands[0].HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;

                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 0, "II. Расходы", "");

                for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
                {
                    e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = (i == 0) ?
                                                                             HorizontalAlign.Left :
                                                                             HorizontalAlign.Right;
                    double width;
                    switch (i)
                    {
                        case 0:
                            {
                                width = 200;
                                break;
                            }
                        case 24:
                        case 21:
                            {
                                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "P2");
                                width = 90;
                                break;
                            }
                        case 23:
                            {
                                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "P2");
                                width = 60;
                                break;
                            }
                        default:
                            {
                                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "N2");
                                width = 90;
                                break;
                            }

                    }

                    if ((i % 2 == 0) & (i != 0) & (i < e.Layout.Bands[0].Columns.Count - 4))
                    {
                        CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "P2");
                        width = 55;

                    }

                    e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(width, 2500);
                }
            }

            for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
            {
                if (i == 0)
                {
                    e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.OriginY = 0;
                    e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.SpanY = 2;
                }
                else
                {
                    e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.OriginY = 1;
                }
            }

            int multiHeaderPos = 1;
            int dif = 2;

            for (int i = 1; i < e.Layout.Bands[0].Columns.Count - 7; i = i + 8)
            {
                string[] captions = e.Layout.Bands[0].Columns[i].Header.Caption.Split(';');

                int lastColumnGroup = e.Layout.Bands[0].Columns.Count - 8;
                DateTime date = new DateTime(curYear - dif, ComboMonth.SelectedIndex + 1, 1);
                date = date.AddMonths(1);

                if (i != lastColumnGroup)
                {
                    CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i, "Годовое исполнение по конс. бюджету", "Сумма годового фактического исполнения по конс. бюджету субъекта РФ");
                    CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i + 2, "В том числе бюджет субъекта", "Сумма годового фактического исполнения по бюджету субъекта РФ");
                    CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i + 4, string.Format("Исполнение по конс. бюджету с начала года (на 1 {0})", CRHelper.RusMonthGenitive(ComboMonth.SelectedIndex + 2)), string.Format("Сумма фактического исполнения по конс. бюджету субъекта РФ с начала года по состоянию на {0:dd.MM.yyyy} г.", date));
                    CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i + 6, string.Format("Исполнение по конс. бюджету (за {0})", ComboMonth.SelectedValue.ToLower()), string.Format("Сумма фактического исполнения по конс. бюджету субъекта РФ за {0} {1:yyyy} г.", ComboMonth.SelectedValue.ToLower(), date));
                    CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i + 7, "Доля", "Удельный вес показателя в общей сумме расходов конс. бюджета субъекта РФ");
                    CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i + 1, "Доля", "Удельный вес показателя в общей сумме расходов конс. бюджета субъекта РФ");
                    CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i + 3, "Доля", "Удельный вес показателя в общей сумме расходов бюджета субъекта РФ");
                    CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i + 5, "Доля", "Удельный вес показателя в общей сумме расходов конс. бюджета субъекта РФ");
                }
                else
                {
                    CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i, "Годовое плановое назначение по конс. бюджету", string.Format("Сумма годового планового назначения по консолидированному бюджету субъекта РФ по состоянию на {0:dd.MM.yyyy} г.", date));
                    CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i + 1, "Доля", "Удельный вес показателя в общей сумме расходов конс. бюджета субъекта РФ");
                    CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i + 2, "В том числе бюджет субъекта", string.Format("Сумма годового планового назначения по бюджету субъекта РФ по состоянию на {0:dd.MM.yyyy} г.", date));
                    CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i + 3, "Доля", "Удельный вес показателя в общей сумме расходов бюджета субъекта РФ");
                    CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i + 4, string.Format("Темп роста к {0} г. ", Convert.ToInt32(ComboYear.SelectedValue) - 1), string.Format("Tемп роста плановых годовых назначений по конс. бюджету субъекта РФ к {0} г.", Convert.ToInt32(ComboYear.SelectedValue) - 1));
                    CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i + 5, string.Format("Исполнение по конс. бюджету с начала года (на 1 {0})", CRHelper.RusMonthGenitive(ComboMonth.SelectedIndex + 2)), string.Format("Сумма фактического исполнения по конс. бюджету субъекта РФ с начала года по состоянию на {0:dd.MM.yyyy} г.", date));
                    CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i + 6, "Доля", "Удельный вес показателя в общей сумме расходов консолидированного бюджета субъекта РФ");
                    CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i + 7, string.Format("Темп роста к аналогичному периоду {0} г.", Convert.ToInt32(ComboYear.SelectedValue) - 1), string.Format("Tемп роста фактического исполнения по конс. бюджету к аналогичному периоду {0} г.", Convert.ToInt32(ComboYear.SelectedValue) - 1));

                }

                int groupNum = i % 4;
                int groupCount = 8;
                string level2HeaderCaption = groupNum == 2 ? "План по данным субъекта РФ" : "исполнение год";

                CRHelper.AddHierarchyHeader(e.Layout.Grid,
                    0,
                    string.Format("{0} год", curYear - dif),
                    multiHeaderPos,
                    0,
                    groupCount,
                    1);

                CRHelper.AddHierarchyHeader(e.Layout.Grid,
                    0,
                    level2HeaderCaption,
                    multiHeaderPos,
                    0,
                    groupCount,
                    1);
                dif = dif - 1;
                multiHeaderPos += groupCount;
            }
        }

        protected void UltraWebGrid2_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;

            if (e.Layout.Bands[0].Columns.Count > 13)
            {
                e.Layout.Bands[0].HeaderStyle.Wrap = true;
                e.Layout.Bands[0].HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;

                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 0, "III. Источники финансирования дефицита", "");

                for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
                {
                    e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = (i == 0) ?
                                                                             HorizontalAlign.Left :
                                                                             HorizontalAlign.Right;
                    double width;
                    switch (i)
                    {
                        case 0:
                            {
                                width = 250;
                                break;
                            }
                        case 11:
                        case 13:
                            {
                                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "P2");
                                width = 100;
                                break;
                            }
                        default:
                            {
                                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "N2");
                                width = 110;
                                break;
                            }
                    }
                    e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(width, 2000);
                }
            }

            for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
            {
                if (i == 0)
                {
                    e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.OriginY = 0;
                    e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.SpanY = 2;
                }
                else
                {
                    e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.OriginY = 1;
                }
            }

            int multiHeaderPos = 1;
            int dif = 2;

            for (int i = 1; i < e.Layout.Bands[0].Columns.Count - 4; i = i + 4)
            {
                string[] captions = e.Layout.Bands[0].Columns[i].Header.Caption.Split(';');

                int lastColumnGroup = e.Layout.Bands[0].Columns.Count - 5;
                DateTime date = new DateTime(curYear - dif, ComboMonth.SelectedIndex + 1, 1);
                date = date.AddMonths(1);

                if (i != lastColumnGroup)
                {
                    CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i, "Годовое исполнение по конс. бюджету", "Сумма годового фактического исполнения по конс. бюджету субъекта РФ");
                    CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i + 1, "В том числе бюджет субъекта", "Сумма годового фактического исполнения по бюджету субъекта РФ");
                    CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i + 2, string.Format("Исполнение по конс. бюджету с начала года (на 1 {0})", CRHelper.RusMonthGenitive(ComboMonth.SelectedIndex + 2)), string.Format("Сумма фактического исполнения по конс. бюджету субъекта РФ с начала года по состоянию на {0:dd.MM.yyyy} г.", date));
                    CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i + 3, string.Format("Исполнение по конс. бюджету (за {0})", ComboMonth.SelectedValue.ToLower()), string.Format("Сумма фактического исполнения по конс. бюджету субъекта РФ за {0} {1:yyyy} г.", ComboMonth.SelectedValue.ToLower(), date));
                }
                else
                {
                    CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i, "Годовое плановое назначение по конс. бюджету", string.Format("Сумма годового планового назначения по конс. бюджету субъекта РФ по состоянию на {0:dd.MM.yyyy} г.", date));
                    CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i + 1, "В том числе бюджет субъекта", string.Format("Сумма годового планового назначения по бюджету субъекта РФ по состоянию на {0:dd.MM.yyyy} г.", date));
                    CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i + 2, string.Format("Темп роста к {0} г. ", Convert.ToInt32(ComboYear.SelectedValue) - 1), string.Format("Tемп роста плановых годовых назначений по конс. бюджету субъекта РФ к {0} г.", Convert.ToInt32(ComboYear.SelectedValue) - 1));
                    CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i + 3, string.Format("Исполнение по конс. бюджету с начала года (на 1 {0})", CRHelper.RusMonthGenitive(ComboMonth.SelectedIndex + 2)), string.Format("Сумма фактического исполнения по конс. бюджету субъекта РФ с начала года по состоянию на {0:dd.MM.yyyy} г.", date));
                    CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i + 4, string.Format("Темп роста к аналогичному периоду {0} г.", Convert.ToInt32(ComboYear.SelectedValue) - 1), string.Format("Tемп роста фактического исполнения по конс. бюджету к аналогичному периоду {0} г.", Convert.ToInt32(ComboYear.SelectedValue) - 1));
                }

                int groupNum = i % 4;
                int groupCount = groupNum == 2 ? 5 : 4;
                string level2HeaderCaption = groupNum == 2 ? "План по данным субъекта РФ" : "исполнение год";

                if (dif == 0)
                {
                    groupCount = 5;
                }

                CRHelper.AddHierarchyHeader(e.Layout.Grid,
                    0,
                    string.Format("{0} год", curYear - dif),
                    multiHeaderPos,
                    0,
                    groupCount,
                    1);

                CRHelper.AddHierarchyHeader(e.Layout.Grid,
                    0,
                    level2HeaderCaption,
                    multiHeaderPos,
                    0,
                    groupCount,
                    1);
                dif = dif - 1;
                multiHeaderPos += groupCount;
            }
        }

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            //            for (int i = 0; i < e.Row.Cells.Count; i++)
            //            {
            //                if (e.Row.Cells[i] == null)
            //                {
            //                    continue;
            //                }
            //
            //                if (i == 9 && e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != string.Empty)
            //                {
            //                    double percent = (ComboMonth.SelectedIndex + 1) * 100.0 / 12;
            //
            //                    if (100 * Convert.ToDouble(e.Row.Cells[i].Value) < percent)
            //                    {
            //                        e.Row.Cells[i].Style.BackgroundImage = "~/images/ballRedBB.png";
            //                        e.Row.Cells[i].Title = string.Format("Не соблюдается условие равномерности ({0:N2}%)", percent);
            //                    }
            //                    else
            //                    {
            //                        e.Row.Cells[i].Style.BackgroundImage = "~/images/ballGreenBB.png";
            //                        e.Row.Cells[i].Title = string.Format("Соблюдается условие равномерности ({0:N2}%)", percent);
            //                    }
            //                    e.Row.Cells[i].Style.Padding.Right = 2;
            //                }
            //
            //                if ((i == 4) && e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != string.Empty)
            //                {
            //                    if (100 * Convert.ToDouble(e.Row.Cells[i].Value) > 100)
            //                    {
            //                        e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowGreenUpBB.png";
            //                        e.Row.Cells[i].Title = "Рост к прошлому году";
            //                    }
            //                    else if (100 * Convert.ToDouble(e.Row.Cells[i].Value) < 100)
            //                    {
            //                        e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowRedDownBB.png";
            //                        e.Row.Cells[i].Title = "Падение к прошлому году";
            //                    }
            //                }
            //
            //                if (e.Row.Cells[5].Value != null && e.Row.Cells[5].Value.ToString() != string.Empty &&
            //                    e.Row.Cells[6].Value != null && e.Row.Cells[6].Value.ToString() != string.Empty)
            //                {
            //                    if (Convert.ToDouble(e.Row.Cells[5].Value) < Convert.ToDouble(e.Row.Cells[6].Value))
            //                    {
            //                        e.Row.Cells[5].Style.BackgroundImage = "~/images/arrowRedDownBB.png";
            //                        e.Row.Cells[5].Title = "Доля упала с прошлого года";
            //                    }
            //                    else if (Convert.ToDouble(e.Row.Cells[5].Value) > Convert.ToDouble(e.Row.Cells[6].Value))
            //                    {
            //                        e.Row.Cells[5].Style.BackgroundImage = "~/images/arrowGreenUpBB.png";
            //                        e.Row.Cells[5].Title = "Доля выросла с прошлого года";
            //                    }
            //                }
            /* for (int i = 0; i < e.Row.Cells.Count; i++)
             {
                 CRHelper.SaveToErrorLog(e.Row.Cells[14].Value.ToString());

                 if (e.Row.Cells[14] != null && e.Row.Cells[14].Value.ToString() != string.Empty) //&& i != 1)
                 {
                     string level = e.Row.Cells[14].Value.ToString();
                     int fontsize = 8;
                     bool bold = false;
                     bool italic = false;
                     switch (level)
                     {
                         case "группа":
                             {
                                 fontsize = 10;
                                 bold = false;
                                 italic = false;
                                 break;
                             }
                         case "подгруппа":
                             {
                                 fontsize = 10;
                                 bold = false;
                                 italic = true;
                                 break;
                             }
                         case "статья":
                             {
                                 fontsize = 8;
                                 bold = true;
                                 italic = false;
                                 break;
                             }
                     } 
                     e.Row.Cells[i].Style.Font.Size = fontsize;
                     e.Row.Cells[i].Style.Font.Bold = bold;
                     e.Row.Cells[i].Style.Font.Italic = italic;
                     e.Row.Cells[i].Style.CustomRules = "background-repeat: no-repeat; background-position: left center";
                 }
             } */

            //                UltraGridCell cell = e.Row.Cells[i];
            //                if (cell.Value != null && cell.Value.ToString() != string.Empty)
            //                {
            //                    decimal value;
            //                    if (decimal.TryParse(cell.Value.ToString(), out value))
            //                    {
            //                        if (value < 0)
            //                        {
            //                            cell.Style.ForeColor = Color.Red;
            //                        }
            //                    }
            //                }
            //            }
            // CRHelper.SaveToErrorLog(e.Row.BandIndex.ToString());
            int fontsize = 8;
            bool bold = false;
            bool italic = false;

            switch (e.Row.BandIndex)
            {
                case 3:
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                    {
                        fontsize = 8;
                        bold = false;
                        italic = true;
                        break;
                    }

                case 0:
                    {
                        fontsize = 10;
                        bold = true;
                        italic = false;
                        break;
                    }
                case 1:
                case 10:
                case 11:
                    {
                        fontsize = 8;
                        bold = true;
                        italic = false;
                        break;
                    }

            }
            e.Row.Cells[0].Style.Font.Size = fontsize;
            e.Row.Cells[0].Style.Font.Bold = bold;
            e.Row.Cells[0].Style.Font.Italic = italic;
            e.Row.Cells[0].Style.CustomRules = "background-repeat: no-repeat; background-position: left center";

            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
                UltraGridCell cell = e.Row.Cells[i];
                if (cell.Value != null && cell.Value.ToString() != string.Empty)
                {
                    decimal value;
                    if (decimal.TryParse(cell.Value.ToString(), out value))
                    {
                        if (value < 0)
                        {
                            cell.Style.ForeColor = System.Drawing.Color.Red;
                        }
                    }
                }
            }

        }

        protected void UltraWebGrid2_InitializeRow(object sender, RowEventArgs e)
        {
            //            for (int i = 0; i < e.Row.Cells.Count; i++)
            //            {
            //                if (e.Row.Cells[i] == null)
            //                {
            //                    continue;
            //                }
            //
            //                if (i == 9 && e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != string.Empty)
            //                {
            //                    double percent = (ComboMonth.SelectedIndex + 1) * 100.0 / 12;
            //
            //                    if (100 * Convert.ToDouble(e.Row.Cells[i].Value) < percent)
            //                    {
            //                        e.Row.Cells[i].Style.BackgroundImage = "~/images/ballRedBB.png";
            //                        e.Row.Cells[i].Title = string.Format("Не соблюдается условие равномерности ({0:N2}%)", percent);
            //                    }
            //                    else
            //                    {
            //                        e.Row.Cells[i].Style.BackgroundImage = "~/images/ballGreenBB.png";
            //                        e.Row.Cells[i].Title = string.Format("Соблюдается условие равномерности ({0:N2}%)", percent);
            //                    }
            //                    e.Row.Cells[i].Style.Padding.Right = 2;
            //                }
            //
            //                if ((i == 4) && e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != string.Empty)
            //                {
            //                    if (100 * Convert.ToDouble(e.Row.Cells[i].Value) > 100)
            //                    {
            //                        e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowGreenUpBB.png";
            //                        e.Row.Cells[i].Title = "Рост к прошлому году";
            //                    }
            //                    else if (100 * Convert.ToDouble(e.Row.Cells[i].Value) < 100)
            //                    {
            //                        e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowRedDownBB.png";
            //                        e.Row.Cells[i].Title = "Падение к прошлому году";
            //                    }
            //                }
            //
            //                if (e.Row.Cells[5].Value != null && e.Row.Cells[5].Value.ToString() != string.Empty &&
            //                    e.Row.Cells[6].Value != null && e.Row.Cells[6].Value.ToString() != string.Empty)
            //                {
            //                    if (Convert.ToDouble(e.Row.Cells[5].Value) < Convert.ToDouble(e.Row.Cells[6].Value))
            //                    {
            //                        e.Row.Cells[5].Style.BackgroundImage = "~/images/arrowRedDownBB.png";
            //                        e.Row.Cells[5].Title = "Доля упала с прошлого года";
            //                    }
            //                    else if (Convert.ToDouble(e.Row.Cells[5].Value) > Convert.ToDouble(e.Row.Cells[6].Value))
            //                    {
            //                        e.Row.Cells[5].Style.BackgroundImage = "~/images/arrowGreenUpBB.png";
            //                        e.Row.Cells[5].Title = "Доля выросла с прошлого года";
            //                    }
            //                }
            /* for (int i = 0; i < e.Row.Cells.Count; i++)
             {
                 CRHelper.SaveToErrorLog(e.Row.Cells[14].Value.ToString());

                 if (e.Row.Cells[14] != null && e.Row.Cells[14].Value.ToString() != string.Empty) //&& i != 1)
                 {
                     string level = e.Row.Cells[14].Value.ToString();
                     int fontsize = 8;
                     bool bold = false;
                     bool italic = false;
                     switch (level)
                     {
                         case "группа":
                             {
                                 fontsize = 10;
                                 bold = false;
                                 italic = false;
                                 break;
                             }
                         case "подгруппа":
                             {
                                 fontsize = 10;
                                 bold = false;
                                 italic = true;
                                 break;
                             }
                         case "статья":
                             {
                                 fontsize = 8;
                                 bold = true;
                                 italic = false;
                                 break;
                             }
                     } 
                     e.Row.Cells[i].Style.Font.Size = fontsize;
                     e.Row.Cells[i].Style.Font.Bold = bold;
                     e.Row.Cells[i].Style.Font.Italic = italic;
                     e.Row.Cells[i].Style.CustomRules = "background-repeat: no-repeat; background-position: left center";
                 }
             } */

            //                UltraGridCell cell = e.Row.Cells[i];
            //                if (cell.Value != null && cell.Value.ToString() != string.Empty)
            //                {
            //                    decimal value;
            //                    if (decimal.TryParse(cell.Value.ToString(), out value))
            //                    {
            //                        if (value < 0)
            //                        {
            //                            cell.Style.ForeColor = Color.Red;
            //                        }
            //                    }
            //                }
            //            }
            // CRHelper.SaveToErrorLog(e.Row.BandIndex.ToString());
            int fontsize = 8;
            bool bold = false;
            bool italic = false;

            switch (e.Row.BandIndex)
            {
                case 3:
                case 4:
                    {
                        fontsize = 8;
                        bold = false;
                        italic = true;
                        break;
                    }

                case 0:
                    {
                        fontsize = 10;
                        bold = true;
                        italic = false;
                        break;
                    }

            }
            e.Row.Cells[0].Style.Font.Size = fontsize;
            e.Row.Cells[0].Style.Font.Bold = bold;
            e.Row.Cells[0].Style.Font.Italic = italic;
            e.Row.Cells[0].Style.CustomRules = "background-repeat: no-repeat; background-position: left center";

            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
                UltraGridCell cell = e.Row.Cells[i];
                if (cell.Value != null && cell.Value.ToString() != string.Empty)
                {
                    decimal value;
                    if (decimal.TryParse(cell.Value.ToString(), out value))
                    {
                        if (value < 0)
                        {
                            cell.Style.ForeColor = System.Drawing.Color.Red;
                        }
                    }
                }
            }

        }

        protected void UltraWebGrid1_InitializeRow(object sender, RowEventArgs e)
        {
            //            for (int i = 0; i < e.Row.Cells.Count; i++)
            //            {
            //                if (e.Row.Cells[i] == null)
            //                {
            //                    continue;
            //                }
            //
            //                if (i == 9 && e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != string.Empty)
            //                {
            //                    double percent = (ComboMonth.SelectedIndex + 1) * 100.0 / 12;
            //
            //                    if (100 * Convert.ToDouble(e.Row.Cells[i].Value) < percent)
            //                    {
            //                        e.Row.Cells[i].Style.BackgroundImage = "~/images/ballRedBB.png";
            //                        e.Row.Cells[i].Title = string.Format("Не соблюдается условие равномерности ({0:N2}%)", percent);
            //                    }
            //                    else
            //                    {
            //                        e.Row.Cells[i].Style.BackgroundImage = "~/images/ballGreenBB.png";
            //                        e.Row.Cells[i].Title = string.Format("Соблюдается условие равномерности ({0:N2}%)", percent);
            //                    }
            //                    e.Row.Cells[i].Style.Padding.Right = 2;
            //                }
            //
            //                if ((i == 4) && e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != string.Empty)
            //                {
            //                    if (100 * Convert.ToDouble(e.Row.Cells[i].Value) > 100)
            //                    {
            //                        e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowGreenUpBB.png";
            //                        e.Row.Cells[i].Title = "Рост к прошлому году";
            //                    }
            //                    else if (100 * Convert.ToDouble(e.Row.Cells[i].Value) < 100)
            //                    {
            //                        e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowRedDownBB.png";
            //                        e.Row.Cells[i].Title = "Падение к прошлому году";
            //                    }
            //                }
            //
            //                if (e.Row.Cells[5].Value != null && e.Row.Cells[5].Value.ToString() != string.Empty &&
            //                    e.Row.Cells[6].Value != null && e.Row.Cells[6].Value.ToString() != string.Empty)
            //                {
            //                    if (Convert.ToDouble(e.Row.Cells[5].Value) < Convert.ToDouble(e.Row.Cells[6].Value))
            //                    {
            //                        e.Row.Cells[5].Style.BackgroundImage = "~/images/arrowRedDownBB.png";
            //                        e.Row.Cells[5].Title = "Доля упала с прошлого года";
            //                    }
            //                    else if (Convert.ToDouble(e.Row.Cells[5].Value) > Convert.ToDouble(e.Row.Cells[6].Value))
            //                    {
            //                        e.Row.Cells[5].Style.BackgroundImage = "~/images/arrowGreenUpBB.png";
            //                        e.Row.Cells[5].Title = "Доля выросла с прошлого года";
            //                    }
            //                }
            /* for (int i = 0; i < e.Row.Cells.Count; i++)
             {
                 CRHelper.SaveToErrorLog(e.Row.Cells[14].Value.ToString());

                 if (e.Row.Cells[14] != null && e.Row.Cells[14].Value.ToString() != string.Empty) //&& i != 1)
                 {
                     string level = e.Row.Cells[14].Value.ToString();
                     int fontsize = 8;
                     bool bold = false;
                     bool italic = false;
                     switch (level)
                     {
                         case "группа":
                             {
                                 fontsize = 10;
                                 bold = false;
                                 italic = false;
                                 break;
                             }
                         case "подгруппа":
                             {
                                 fontsize = 10;
                                 bold = false;
                                 italic = true;
                                 break;
                             }
                         case "статья":
                             {
                                 fontsize = 8;
                                 bold = true;
                                 italic = false;
                                 break;
                             }
                     } 
                     e.Row.Cells[i].Style.Font.Size = fontsize;
                     e.Row.Cells[i].Style.Font.Bold = bold;
                     e.Row.Cells[i].Style.Font.Italic = italic;
                     e.Row.Cells[i].Style.CustomRules = "background-repeat: no-repeat; background-position: left center";
                 }
             } */

            //                UltraGridCell cell = e.Row.Cells[i];
            //                if (cell.Value != null && cell.Value.ToString() != string.Empty)
            //                {
            //                    decimal value;
            //                    if (decimal.TryParse(cell.Value.ToString(), out value))
            //                    {
            //                        if (value < 0)
            //                        {
            //                            cell.Style.ForeColor = Color.Red;
            //                        }
            //                    }
            //                }
            //            }
            // CRHelper.SaveToErrorLog(e.Row.BandIndex.ToString());
            int fontsize = 8;
            bool bold = false;
            bool italic = false;

            switch (e.Row.BandIndex)
            {
                case 8:
                case 9:
                case 10:
                case 11:
                case 12:
                case 14:
                case 15:
                case 16:
                case 17:
                    {
                        fontsize = 8;
                        bold = false;
                        italic = true;
                        break;
                    }

                case 0:
                    {
                        fontsize = 10;
                        bold = true;
                        italic = false;
                        break;
                    }
                case 1:
                case 5:
                case 21:
                case 18:
                    {
                        fontsize = 8;
                        bold = true;
                        italic = false;
                        break;
                    }

            }
            e.Row.Cells[0].Style.Font.Size = fontsize;
            e.Row.Cells[0].Style.Font.Bold = bold;
            e.Row.Cells[0].Style.Font.Italic = italic;
            e.Row.Cells[0].Style.CustomRules = "background-repeat: no-repeat; background-position: left center";

            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
                UltraGridCell cell = e.Row.Cells[i];
                if (cell.Value != null && cell.Value.ToString() != string.Empty)
                {
                    decimal value;
                    if (decimal.TryParse(cell.Value.ToString(), out value))
                    {
                        if (value < 0)
                        {
                            cell.Style.ForeColor = System.Drawing.Color.Red;
                        }
                    }
                }
            }

        }


        #endregion

        #region Экспорт в Excel

        private void ExcelExporter_BeginExport(object sender, BeginExportEventArgs e)
        {
            e.CurrentWorksheet.Rows[0].Cells[0].Value = Label1.Text.Replace("<br/>", String.Empty);
            e.CurrentWorksheet.Rows[1].Cells[0].Value = Label2.Text;
        }

        private void ExcelExporter_HeaderCellExporting(object sender, HeaderCellExportingEventArgs e)
        {
            switch (e.CurrentWorksheet.Name)
            {

                case "Доходы":
                    {
                        int ColumnsCount = 13;
                        if ((e.CurrentColumnIndex != 0) & (e.CurrentColumnIndex != ColumnsCount)) e.HeaderText = Convert.ToString(curYear - (2 - (e.CurrentColumnIndex - 1) / 4)) + " год";
                        else if (e.CurrentColumnIndex == ColumnsCount) e.HeaderText = Convert.ToString(curYear) + " год";
                        break;
                    }
                case "Расходы":
                    {
                        int ColumnsCount = 24;
                        if ((e.CurrentColumnIndex != 0) & (e.CurrentColumnIndex != ColumnsCount)) e.HeaderText = Convert.ToString(curYear - (2 - (e.CurrentColumnIndex - 1) / 8)) + " год";
                        else if (e.CurrentColumnIndex == ColumnsCount) e.HeaderText = Convert.ToString(curYear) + " год";
                        break;
                    }
                case "Источники финансирования":
                    {
                        int ColumnsCount = 13;
                        if ((e.CurrentColumnIndex != 0) & (e.CurrentColumnIndex != ColumnsCount)) e.HeaderText = Convert.ToString(curYear - (2 - (e.CurrentColumnIndex - 1) / 4)) + " год";
                        else if (e.CurrentColumnIndex == ColumnsCount) e.HeaderText = Convert.ToString(curYear) + " год";
                        break;
                    }
            }
        }

        private void ExcelExporter_EndExport(object sender, EndExportEventArgs e)
        {
            string formatString = "#,##0.000;[Red]-#,##0.000";
            int widthColumn = 70;
            switch (e.CurrentWorksheet.Name)
            {
                case "Доходы":
                    {

                        for (int i = 1; i < UltraWebGrid.Bands[0].Columns.Count; i++)
                        {
                            if ((i == UltraWebGrid.Bands[0].Columns.Count - 1) || (i == UltraWebGrid.Bands[0].Columns.Count - 3))
                            {
                                formatString = "0.00%";
                                widthColumn = 120;
                            }
                            else
                            {
                                formatString = "#,##0.000;[Red]-#,##0.000";
                                widthColumn = 140;
                            }

                            e.CurrentWorksheet.Columns[i].CellFormat.FormatString = formatString;
                            e.CurrentWorksheet.Columns[i].Width = widthColumn * 37;
                        }
                        break;
                    }

                case "Расходы":
                    {
                        for (int i = 1; i < UltraWebGrid1.Bands[0].Columns.Count; i++)
                        {
                            if ((i % 2 != 0) & (i < UltraWebGrid1.Bands[0].Columns.Count - 4))
                            {
                                formatString = "#,##0.000;[Red]-#,##0.000";
                                widthColumn = 140;
                            }
                            else
                            {
                                if (i != 22)
                                {
                                    formatString = "0.00%";
                                    widthColumn = 120;
                                }
                                else
                                {
                                    formatString = "#,##0.000;[Red]-#,##0.000";
                                    widthColumn = 140;
                                }
                            }

                            e.CurrentWorksheet.Columns[i].CellFormat.FormatString = formatString;
                            e.CurrentWorksheet.Columns[i].Width = widthColumn * 37;
                        }

                        break;
                    }

                case "Источники финансирования":
                    {
                        for (int i = 1; i < UltraWebGrid2.Bands[0].Columns.Count; i++)
                        {
                            if ((i == UltraWebGrid.Bands[0].Columns.Count - 1) || (i == UltraWebGrid.Bands[0].Columns.Count - 3))
                            {
                                formatString = "0.00%";
                                widthColumn = 120;
                            }
                            else
                            {
                                formatString = "#,##0.000;[Red]-#,##0.000";
                                widthColumn = 140;
                            }

                            e.CurrentWorksheet.Columns[i].CellFormat.FormatString = formatString;
                            e.CurrentWorksheet.Columns[i].Width = widthColumn * 37;
                        }
                        break;
                    }
            }
        }

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            Workbook workbook = new Workbook();
            Worksheet sheet1 = workbook.Worksheets.Add("Доходы");
            Worksheet sheet2 = workbook.Worksheets.Add("Расходы");
            Worksheet sheet3 = workbook.Worksheets.Add("Источники финансирования");

            UltraGridExporter1.ExcelExporter.DownloadName = "report.xls";

            UltraGridExporter1.ExcelExporter.ExcelStartRow = 4;
            UltraGridExporter1.ExcelExporter.Export(UltraWebGrid, sheet1);

            UltraGridExporter1.ExcelExporter.ExcelStartRow = 4;
            UltraGridExporter1.ExcelExporter.Export(UltraWebGrid1, sheet2);

            UltraGridExporter1.ExcelExporter.ExcelStartRow = 4;
            UltraGridExporter1.ExcelExporter.Export(UltraWebGrid2, sheet3);
        }

        #endregion

        #region Экспорт в Pdf

        private void PdfExporter_BeginExport(object sender, DocumentExportEventArgs e)
        {
            IText title = e.Section.AddText();
            Font font = new Font("Verdana", 16);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.Style.Font.Bold = true;
            title.AddContent(Label1.Text.Replace("<br/>", String.Empty));

            title = e.Section.AddText();
            font = new Font("Verdana", 14);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.AddContent(Label2.Text);
        }



        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            UltraGridExporter1.PdfExporter.DownloadName = "report.pdf"; 
            UltraGridExporter1.HeaderChildCellHeight = 60;
            Report report = new Report();

            ReportSection section1 = new ReportSection(report, false);
            ReportSection section2 = new ReportSection(report, false);
            ReportSection section3 = new ReportSection(report, false);
            UltraGridExporter1.PdfExporter.Export(UltraWebGrid, section1);
            UltraGridExporter1.PdfExporter.Export(UltraWebGrid1, section2);
            UltraGridExporter1.PdfExporter.Export(UltraWebGrid2, section3);
        }


        #endregion
    }

    public class ReportSection : ISection
    {
        private readonly bool withFlowColumns;
        private readonly ISection section;
        private IFlow flow;
        private ITableCell titleCell;

        public ReportSection(Report report, bool withFlowColumns)
        {
            //this.withFlowColumns = withFlowColumns;
            section = report.AddSection();
            ITable table = section.AddTable();
            ITableRow row = table.AddRow();
            titleCell = row.AddCell();
            /*if (this.withFlowColumns)
            {
                flow = section.AddFlow();
                IFlowColumn col = flow.AddColumn();
                col.Width = new FixedWidth(815);
               // col = flow.AddColumn();
             //   col.Width = new FixedWidth(525);
            //}*/
        }

        public void AddFlowColumnBreak()
        {
            if (flow != null)
                flow.AddColumnBreak();
        }

        public ContentAlignment PageAlignment
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public IBand AddBand()
        {
            /*if (flow != null)
                return flow.AddBand();*/
            return section.AddBand();
        }

        #region ISection members
        public ISectionHeader AddHeader()
        {
            throw new NotImplementedException();
        }

        public ISectionFooter AddFooter()
        {
            throw new NotImplementedException();
        }

        public IStationery AddStationery()
        {
            throw new NotImplementedException();
        }

        public IDecoration AddDecoration()
        {
            throw new NotImplementedException();
        }

        public ISectionPage AddPage()
        {
            throw new NotImplementedException();
        }

        public ISectionPage AddPage(PageSize size)
        {
            throw new NotImplementedException();
        }

        public ISectionPage AddPage(float width, float height)
        {
            throw new NotImplementedException();
        }

        public ISegment AddSegment()
        {
            throw new NotImplementedException();
        }

        public IQuickText AddQuickText(string text)
        {
            throw new NotImplementedException();
        }

        public IQuickImage AddQuickImage(Infragistics.Documents.Reports.Graphics.Image image)
        {
            throw new NotImplementedException();
        }

        public IQuickList AddQuickList()
        {
            throw new NotImplementedException();
        }

        public IQuickTable AddQuickTable()
        {
            throw new NotImplementedException();
        }

        public IText AddText()
        {
            return this.titleCell.AddText();
        }

        public IImage AddImage(Infragistics.Documents.Reports.Graphics.Image image)
        {
            if (flow != null)
                return flow.AddImage(image);
            return this.section.AddImage(image);
        }

        public IMetafile AddMetafile(Metafile metafile)
        {
            throw new NotImplementedException();
        }

        public IMetafile AddMetafile(System.Drawing.Imaging.Metafile metafile)
        {
            throw new NotImplementedException();
        }

        public IRule AddRule()
        {
            throw new NotImplementedException();
        }

        public IGap AddGap()
        {
            throw new NotImplementedException();
        }

        public IGroup AddGroup()
        {
            throw new NotImplementedException();
        }

        public IChain AddChain()
        {
            throw new NotImplementedException();
        }

        public ITable AddTable()
        {
            /*if (flow != null)
                return flow.AddTable();*/
            return this.section.AddTable();
        }

        public IGrid AddGrid()
        {
            throw new NotImplementedException();
        }

        public IFlow AddFlow()
        {
            throw new NotImplementedException();
        }

        public IList AddList()
        {
            throw new NotImplementedException();
        }

        public ITree AddTree()
        {
            throw new NotImplementedException();
        }

        public ISite AddSite()
        {
            throw new NotImplementedException();
        }

        public ICanvas AddCanvas()
        {
            throw new NotImplementedException();
        }

        public IRotator AddRotator()
        {
            throw new NotImplementedException();
        }

        public IContainer AddContainer(string name)
        {
            throw new NotImplementedException();
        }

        public ICondition AddCondition(IContainer container, bool fit)
        {
            throw new NotImplementedException();
        }

        public IStretcher AddStretcher()
        {
            throw new NotImplementedException();
        }

        public void AddPageBreak()
        {
            throw new NotImplementedException();
        }

        public ITOC AddTOC()
        {
            throw new NotImplementedException();
        }

        public IIndex AddIndex()
        {
            throw new NotImplementedException();
        }

        public bool Flip
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public PageSize PageSize
        {
            get { throw new NotImplementedException(); }
            set
            {
               // this.section.PageSize = new PageSize(this.section.PageSize.Width, this.section.PageSize.Height);
                this.section.PageSize = new PageSize(1630, 800);
                //this.section.PageSize = new PageSize(2560, 1350);
            }
        }

        public PageOrientation PageOrientation
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }



        public Borders PageBorders
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public Infragistics.Documents.Reports.Report.Margins PageMargins
        {
            get { return this.section.PageMargins; }
            set { throw new NotImplementedException(); }
        }

        public Paddings PagePaddings
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public Background PageBackground
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public Infragistics.Documents.Reports.Report.Section.PageNumbering PageNumbering
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public SectionLineNumbering LineNumbering
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public Report Parent
        {
            get { return this.section.Parent; }
        }

        public IEnumerable Content
        {
            get { throw new NotImplementedException(); }
        }

        #endregion
    }
}
