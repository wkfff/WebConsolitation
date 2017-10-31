using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Web.UI.WebControls;
using Infragistics.Documents.Reports.Report;
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.WebUI.UltraWebGrid.DocumentExport;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.SgmSupport;

namespace Krista.FM.Server.Dashboards.reports.SGM.SGM_013
{
    public partial class sgm_013 : CustomReportPage
    {
        private readonly Dictionary<string, string> marginsIndexes = new Dictionary<string, string>();
        private readonly Dictionary<string, string> marginsF1 = new Dictionary<string, string>();
        readonly Collection<string> form1Exceptions = new Collection<string>();

        private DataTable dtMain = new DataTable();

        private string mapName = string.Empty;
        private string months = string.Empty;
        private int year1;
        private int year2;
        private Collection<string> deseaseCodes;

        ColumnHeader ch1;
        ColumnHeader ch2;

        private readonly SGMDataRotator dataRotator = new SGMDataRotator();
        private readonly SGMDataObject dataObject = new SGMDataObject();
        private readonly SGMSupport supportClass = new SGMSupport();
        private readonly SGMPdfExportHelper exportClass = new SGMPdfExportHelper();

        private const string headerCaption1 = "Всего";
        private const string headerCaption2 = "в том числе у детей до 17 лет включительно";
        private const string headerCaption3 = "Рост/снижение";

        protected override void Page_Load(object sender, EventArgs e)
        {
            dataObject.reportFormRotator = dataRotator;
            dataRotator.CheckSubjectReport();
            base.Page_Load(sender, e);
            dataRotator.formNumber = 1;
            dataObject.InitObject();
            dataRotator.FillRegionsDictionary(dataObject.dtAreaShort);
            if (!Page.IsPostBack)
            {
                dataRotator.FillSGMMapList(ComboMap, dataObject.dtAreaShort, true);
                dataRotator.FillYearList(ComboYear);
                dataRotator.FillYearList(ComboCompareYear);
                ComboCompareYear.SetСheckedState(Convert.ToString(Convert.ToInt32(ComboYear.SelectedValue) - 1), true);
                dataRotator.FillMonthListEx(ComboMonth, ComboYear.SelectedValue);
            }
            else
            {
                dataRotator.FillSGMMapList(null, dataObject.dtAreaShort, true);
            }

            year1 = Convert.ToInt32(ComboYear.SelectedValue);
            year2 = Convert.ToInt32(ComboCompareYear.SelectedValue);
            mapName = ComboMap.SelectedValue;
            months = dataRotator.GetMonthParamString(ComboMonth, year1.ToString());
            dataRotator.CheckFormNumber(Math.Max(year1, year2), ref months);
 
            FillDeseaseList();
            FillExceptions();
            FillMargins();

            DataTable dtFullData = FillDeseaseData();

            dtMain = new DataTable();
            dtMain.Columns.Add("c1", typeof(int));
            dtMain.Columns.Add("c2", typeof(string));
            for (int i = 0; i < 8; i++)
            {
                dtMain.Columns.Add(string.Format("c{0}", i + 3), typeof(double));
            }
            dtMain.Columns.Add("text1", typeof(string));
            dtMain.Columns.Add("text2", typeof(string));
            
            int rowCounter = 1;
            var deseasesInclude = new Collection<string>(SGMDataRotator.allDeseasesCodeForm1.Split(','));        
            for (int i = 0; i < deseaseCodes.Count; i++)
            {
                bool needInsert = true;
                if (dataRotator.formNumber == 1)
                {
                    needInsert = deseasesInclude.Contains(deseaseCodes[i]);
                    needInsert = needInsert || form1Exceptions.Contains(deseaseCodes[i]);
                }
                if (needInsert)
                {
                    string fullName = dataObject.GetDeseaseName(deseaseCodes[i], false);
                    DataRow drFind = supportClass.FindDataRow(dtFullData, fullName, dtFullData.Columns[0].ColumnName);
                    DataRow drAdd = dtMain.Rows.Add();
                    drAdd[0] = rowCounter++;
                    drAdd[1] = fullName;
                    if (drFind != null)
                    {
                        for (int j = 0; j < 10; j++)
                        {
                            drAdd[j + 2] = drFind[j + 1];
                        }
                    }
                }
            }

            grid.DataSource = dtMain;
            grid.DataBind();

            supportClass.CalculateGridColumnsWidth(grid, 2);

            string monthLabel = dataRotator.GetMonthParamLabel(ComboMonth, year1.ToString());
            Page.Title = "Информационный бюллетень инфекционной заболеваемости";
            LabelTitle.Text = Page.Title;
            LabelSubTitle.Text = String.Format("{0} за {1} {2}-{3}г.{4}",
                mapName.Trim(), monthLabel, year1, year2, dataRotator.GetFormHeader());
        }

        private string GetCollectionText(Collection<string> col, char splitter)
        {
            string result = String.Empty;
            for (int i = 0; i < col.Count; i++)
            {
                result = String.Format("{0}{1}{2}", result, splitter, col[i]);
            }
            return result.Trim(splitter);
        }

        protected DataTable FillDeseaseData()
        {
            const string groupName1 = "0";
            const string groupName2 = "4";
            string year1Str = year1.ToString();
            string year2Str = year2.ToString();
            dataObject.InitObject();
            // Структура данных по болезням
            dataObject.mainColumn = SGMDataObject.MainColumnType.mctDeseaseName;
            dataObject.useLongNames = true;
            dataObject.useHierarchicalNames = true;
            dataObject.mainColumnRange = GetCollectionText(deseaseCodes, ',');
            // Заболевание по субъекту текущий год
            dataObject.AddColumn(SGMDataObject.DependentColumnType.dctAbs,
                year1Str, months, mapName, groupName1, String.Empty);
            dataObject.AddColumn(SGMDataObject.DependentColumnType.dctRelation,
                "1");
            dataObject.AddColumn(SGMDataObject.DependentColumnType.dctAbs,
                year1Str, months, mapName, groupName2, String.Empty);
            dataObject.AddColumn(SGMDataObject.DependentColumnType.dctRelation,
                "3");
            // Заболевание по субъекту прошлый год
            dataObject.AddColumn(SGMDataObject.DependentColumnType.dctAbs,
                year2Str, months, mapName, groupName1, String.Empty);
            dataObject.AddColumn(SGMDataObject.DependentColumnType.dctRelation,
                "5");
            dataObject.AddColumn(SGMDataObject.DependentColumnType.dctAbs,
                year2Str, months, mapName, groupName2, String.Empty);
            dataObject.AddColumn(SGMDataObject.DependentColumnType.dctRelation,
                "7");
            // Рост \ снижение текстовка
            dataObject.AddColumn(SGMDataObject.DependentColumnType.dctUpDownPercentText,
                "1", "5", "2", "6");
            dataObject.AddColumn(SGMDataObject.DependentColumnType.dctUpDownPercentText,
                "3", "7", "4", "8");
            return dataObject.FillData();
        }

        // Исключения из заболеваний, которые в отчет не попадают
        protected void FillExceptions()
        {
            form1Exceptions.Clear();
            form1Exceptions.Add("122");
            form1Exceptions.Add("20");
            form1Exceptions.Add("21");
            form1Exceptions.Add("94");
            form1Exceptions.Add("107");
            form1Exceptions.Add("108");
            form1Exceptions.Add("22");
            form1Exceptions.Add("35");
            form1Exceptions.Add("120");
            form1Exceptions.Add("43");
            form1Exceptions.Add("49");
            form1Exceptions.Add("50");
            form1Exceptions.Add("51");
            form1Exceptions.Add("64");
            form1Exceptions.Add("65");
            form1Exceptions.Add("73");
            form1Exceptions.Add("53");
        }

        // отступы для разных форм
        protected void FillMargins()
        {
            marginsIndexes.Clear();
            marginsIndexes.Add("6", "0");
            marginsIndexes.Add("7", "0");
            marginsIndexes.Add("8", "0");
            marginsIndexes.Add("10", "0");
            marginsIndexes.Add("11", "0");
            marginsIndexes.Add("12", "0");
            marginsIndexes.Add("15", "0");
            marginsIndexes.Add("16", "0");
            marginsIndexes.Add("17", "0");
            marginsIndexes.Add("18", "0");
            marginsIndexes.Add("19", "0");
            marginsIndexes.Add("20", "0");
            marginsIndexes.Add("21", "0");
            marginsIndexes.Add("24", "0");
            marginsIndexes.Add("27", "0");
            marginsIndexes.Add("29", "0");
            marginsIndexes.Add("30", "0");
            marginsIndexes.Add("31", "0");
            marginsIndexes.Add("33", "0");
            marginsIndexes.Add("34", "0");
            marginsIndexes.Add("39", "0");
            marginsIndexes.Add("47", "0");
            marginsIndexes.Add("54", "0");
            marginsIndexes.Add("55", "0");
            marginsIndexes.Add("56", "0");
            marginsIndexes.Add("63", "0");
            marginsIndexes.Add("66", "0");
            marginsIndexes.Add("67", "0");
            marginsIndexes.Add("68", "0");
            marginsIndexes.Add("69", "0");
            marginsIndexes.Add("75", "0");
            marginsIndexes.Add("76", "0");
            
            if (year1 >= 2011 || year2 >= 2011)
            {
                marginsIndexes.Add("84", "0");
                marginsIndexes.Add("85", "0");
                marginsIndexes.Add("86", "0");
                marginsIndexes.Add("93", "0");
            }
            else
            {
                marginsIndexes.Add("89", "0");
            }

            marginsF1.Clear();
            marginsF1.Add("6", "0");
            marginsF1.Add("9", "0");
            marginsF1.Add("11", "0");
            marginsF1.Add("12", "0");
            marginsF1.Add("13", "0");
            marginsF1.Add("15", "0");
            marginsF1.Add("16", "0");
            marginsF1.Add("24", "0");
            marginsF1.Add("29", "0");
            marginsF1.Add("30", "0");
            marginsF1.Add("31", "0");
            marginsF1.Add("38", "0");
            marginsF1.Add("39", "0");
            marginsF1.Add("40", "0");
            marginsF1.Add("41", "0");
            marginsF1.Add("44", "0");
            marginsF1.Add("45", "0");
        }

        // коды заболеваний отчета
        protected void FillDeseaseList()
        {
            deseaseCodes = new Collection<string>();
            // 1 страница
            deseaseCodes.Add("1");
            deseaseCodes.Add("2");
            deseaseCodes.Add("3");
            deseaseCodes.Add("115");
            deseaseCodes.Add("116");
            deseaseCodes.Add("4");
            deseaseCodes.Add("5");
            deseaseCodes.Add("6");
            deseaseCodes.Add("7");
            deseaseCodes.Add("8");
            deseaseCodes.Add("9");
            deseaseCodes.Add("10");
            deseaseCodes.Add("11");
            deseaseCodes.Add("12");
            deseaseCodes.Add("13");
            deseaseCodes.Add("126");
            deseaseCodes.Add("127");
            deseaseCodes.Add("15");
            deseaseCodes.Add("17");
            deseaseCodes.Add("128");
            deseaseCodes.Add("14");
            deseaseCodes.Add("129");
            deseaseCodes.Add("18");
            deseaseCodes.Add("24");
            deseaseCodes.Add("120");
            deseaseCodes.Add("98");
            deseaseCodes.Add("121");
            deseaseCodes.Add("122");
            // 3 
            deseaseCodes.Add("19");
            deseaseCodes.Add("20");
            deseaseCodes.Add("21");
            deseaseCodes.Add("94");
            deseaseCodes.Add("117");
            deseaseCodes.Add("107");
            deseaseCodes.Add("108");
            deseaseCodes.Add("22");
            deseaseCodes.Add("25");
            deseaseCodes.Add("26");
            deseaseCodes.Add("27");
            deseaseCodes.Add("28");
            deseaseCodes.Add("29");
            deseaseCodes.Add("30");
            deseaseCodes.Add("32");
            deseaseCodes.Add("33");
            deseaseCodes.Add("87");
            deseaseCodes.Add("31");
            deseaseCodes.Add("34");
            deseaseCodes.Add("35");
            // 4 лист
            deseaseCodes.Add("125");
            deseaseCodes.Add("39");
            deseaseCodes.Add("36");
            deseaseCodes.Add("37");
            deseaseCodes.Add("38");
            deseaseCodes.Add("42");
            deseaseCodes.Add("130");
            deseaseCodes.Add("131");
            deseaseCodes.Add("43");
            deseaseCodes.Add("40");
            deseaseCodes.Add("41");
            deseaseCodes.Add("23");
            deseaseCodes.Add("55");
            deseaseCodes.Add("44");
            deseaseCodes.Add("45");
            deseaseCodes.Add("99");
            deseaseCodes.Add("47");
            // 5 лист
            deseaseCodes.Add("48");
            deseaseCodes.Add("49");
            deseaseCodes.Add("50");
            deseaseCodes.Add("51");
            deseaseCodes.Add("52");
            deseaseCodes.Add("53");
            deseaseCodes.Add("54");
            deseaseCodes.Add("61");
            deseaseCodes.Add("46");
            deseaseCodes.Add("63");
            deseaseCodes.Add("64");
            deseaseCodes.Add("65");
            deseaseCodes.Add("66");
            deseaseCodes.Add("67");
            deseaseCodes.Add("57");
            deseaseCodes.Add("56");
            deseaseCodes.Add("59");
            deseaseCodes.Add("58");
            
            // 6 лист
            if (year1 >= 2011 || year2 >= 2011)
            {
                deseaseCodes.Add("135");
                deseaseCodes.Add("136");
                deseaseCodes.Add("137");
                deseaseCodes.Add("138");
            }
            
            deseaseCodes.Add("60");
            deseaseCodes.Add("123");
            deseaseCodes.Add("69");
            deseaseCodes.Add("68");
            deseaseCodes.Add("70");
            deseaseCodes.Add("71");
            deseaseCodes.Add("132");
            deseaseCodes.Add("72");
            if (dataRotator.formNumber == 1)
            {
                deseaseCodes.Add("81");
            }
            deseaseCodes.Add("73");
        }

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            grid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 30);
            grid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight - 320);
            if (grid.Width.Value > 0)
            {
                LabelTitle.Width = (Unit) (grid.Width.Value - 50);
                LabelSubTitle.Width = LabelTitle.Width;
            }
            SetExportHandlers();
        }

        protected void grid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;

            e.Layout.Bands[0].Columns[0].Header.Caption = "№ п/п";
            e.Layout.Bands[0].Columns[0].Width = 40;
            e.Layout.Bands[0].Columns[1].Header.Caption = "наименования заболеваний";
            e.Layout.Bands[0].Columns[1].Width = 210;

            for (int i = 2; i < grid.Columns.Count; i++)
            {
                grid.Columns[i].Width = 84;
                CRHelper.FormatNumberColumn(grid.Columns[i], "N0");
            }

            grid.Columns[10].Width = 95;
            grid.Columns[11].Width = 95;

            CRHelper.FormatNumberColumn(grid.Columns[03], "N2");
            CRHelper.FormatNumberColumn(grid.Columns[05], "N2");
            CRHelper.FormatNumberColumn(grid.Columns[07], "N2");
            CRHelper.FormatNumberColumn(grid.Columns[09], "N2");

            grid.Columns[00].CellStyle.HorizontalAlign = HorizontalAlign.Center;
            grid.Columns[01].CellStyle.Wrap = true;

            for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
            {
                if (i == 0 || i == 1)
                {
                    e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.OriginY = 0;
                    e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.SpanY = 4;
                }
                else
                {
                    e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.OriginY = 3;
                }
            }

            const string captionTemplate1 = "Абсолютный показатель заболеваемости по группе Всего за {0} {1} года";
            const string captionTemplate2 = "Относительный показатель заболеваемости по группе Всего на 100 тысяч населения за {0} {1} года";
            const string captionTemplate3 = "Абсолютный показатель заболеваемости по группе Дети за {0} {1} года";
            const string captionTemplate4 = "Относительный показатель заболеваемости по группе Дети на 100 тысяч населения за {0} {1} года";
            const string shortCaption1 = "абс.";
            const string shortCaption2 = "на 100 тыс. населения";
            string monthCaption1 = dataRotator.GetMonthParamLabel(ComboMonth, year1.ToString());
            string monthCaption2 = dataRotator.GetMonthParamLabel(ComboMonth, year2.ToString());

            CRHelper.SetHeaderCaption(grid, 0, 2, shortCaption1, String.Format(captionTemplate1, monthCaption1, year1));
            CRHelper.SetHeaderCaption(grid, 0, 3, shortCaption2, String.Format(captionTemplate2, monthCaption1, year1));
            CRHelper.SetHeaderCaption(grid, 0, 4, shortCaption1, String.Format(captionTemplate3, monthCaption1, year1));
            CRHelper.SetHeaderCaption(grid, 0, 5, shortCaption2, String.Format(captionTemplate4, monthCaption1, year1));
            CRHelper.SetHeaderCaption(grid, 0, 6, shortCaption1, String.Format(captionTemplate1, monthCaption2, year2));
            CRHelper.SetHeaderCaption(grid, 0, 7, shortCaption2, String.Format(captionTemplate2, monthCaption2, year2));
            CRHelper.SetHeaderCaption(grid, 0, 8, shortCaption1, String.Format(captionTemplate3, monthCaption2, year2));
            CRHelper.SetHeaderCaption(grid, 0, 9, shortCaption2, String.Format(captionTemplate4, monthCaption2, year2));

            ch1 = new ColumnHeader(true) {RowLayoutColumnInfo = {OriginY = 0, OriginX = 2, SpanX = 4}};
            e.Layout.Bands[0].HeaderLayout.Add(ch1);

            ch2 = new ColumnHeader(true) {RowLayoutColumnInfo = {OriginY = 0, OriginX = 6, SpanX = 4}};
            e.Layout.Bands[0].HeaderLayout.Add(ch2);


            string yearAppendix = String.Empty;
            if (monthCaption1 != String.Empty) yearAppendix = "а";
            ch1.Caption = String.Format("За {0} {1} год{2}", monthCaption1, year1, yearAppendix);
            ch2.Caption = String.Format("За {0} {1} год{2}", monthCaption1, year2, yearAppendix);
            
            if (e.Layout.Bands[0].HeaderLayout.Count > 14)
            {
                e.Layout.Bands[0].HeaderLayout[12].Caption = ch1.Caption;
                e.Layout.Bands[0].HeaderLayout[13].Caption = ch2.Caption;
            }

            var ch = new ColumnHeader(true)
                         {
                             Caption = string.Format(headerCaption3),
                             RowLayoutColumnInfo =
                                 {
                                     OriginY = 0,
                                     OriginX = 10,
                                     SpanX = 2
                                 }
                         };

            e.Layout.Bands[0].HeaderLayout.Add(ch);

            for (int i = 0; i < 2; i++)
            {
                ch = new ColumnHeader(true)
                         {
                             Caption = String.Format(headerCaption1),
                             RowLayoutColumnInfo =
                                 {
                                     OriginY = 1,
                                     OriginX = 2 + i*4,
                                     SpanX = 2,
                                     SpanY = 2
                                 }
                         };

                e.Layout.Bands[0].HeaderLayout.Add(ch);

                ch = new ColumnHeader(true)
                         {
                             Caption = String.Format(headerCaption2),
                             RowLayoutColumnInfo =
                                 {
                                     OriginY = 1,
                                     OriginX = 4 + i*4,
                                     SpanX = 2,
                                     SpanY = 2
                                 }
                         };

                e.Layout.Bands[0].HeaderLayout.Add(ch);
            }


            e.Layout.Bands[0].Columns[10].Header.Caption = String.Empty;
            e.Layout.Bands[0].Columns[11].Header.Caption = String.Empty;
            ch = new ColumnHeader(true)
                     {
                         Caption = headerCaption1,
                         RowLayoutColumnInfo =
                             {
                                 OriginY = 1,
                                 OriginX = 10,
                                 SpanY = 2
                             }
                     };

            e.Layout.Bands[0].HeaderLayout.Add(ch);

            ch = new ColumnHeader(true)
                     {
                         Caption = headerCaption2,
                         RowLayoutColumnInfo =
                             {
                                 OriginY = 1,
                                 OriginX = 11,
                                 SpanY = 2
                             }
                     };

            e.Layout.Bands[0].HeaderLayout.Add(ch);

            supportClass.CalculateGridColumnsWidth(grid, 2);
        }

        protected void grid_InitializeRow(object sender, RowEventArgs e)
        {
            supportClass.SetCellImageEx(e.Row, 7, 3, 10);
            supportClass.SetCellImageEx(e.Row, 9, 5, 11);
            if (dataRotator.formNumber == 2)
            {
                if (marginsIndexes.ContainsKey(e.Row.Index.ToString()))
                {
                    e.Row.Cells[1].Style.Padding.Left = 20;
                }
            }
            else
            {
                if (marginsF1.ContainsKey(e.Row.Index.ToString()))
                {
                    e.Row.Cells[1].Style.Padding.Left = 20;
                }
            }
        }

        #region PDFExport

        private void SetExportHandlers()
        {
            UltraGridExporter1.PdfExportButton.Click += PdfExportButton_Click;
            UltraGridExporter1.PdfExporter.BeginExport += PdfExporter_BeginExport;
            UltraGridExporter1.ExcelExportButton.Visible = false;
            UltraGridExporter1.MultiHeader = false;
            UltraGridExporter1.PdfExporter.HeaderCellExporting += PdfExporter_HeaderCellExporting;
            UltraGridExporter1.WordExportButton.Visible = true;
        }

        private void PdfExporter_BeginExport(object sender, DocumentExportEventArgs e)
        {
            exportClass.ExportCaptionText(e, LabelTitle.Text);
            exportClass.ExportSubCaptionText(e, LabelSubTitle.Text);
        }

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            UltraGridExporter1.PdfExporter.DownloadName = string.Format("0013.pdf");
            UltraGridExporter1.PdfExporter.Export(grid);
        }

        private readonly Collection<float> cellsWidth = new Collection<float>();
        private readonly Collection<string> cellsCaption = new Collection<string>();

        private void PdfExporter_HeaderCellExporting(object sender, MarginCellExportingEventArgs e)
        {
            const int rowHeight = 35;
            // сохраняем информацию о родных ячейках
            cellsWidth.Add(((FixedWidth)e.ReportCell.Width).Value);
            cellsCaption.Add(e.ExportValue.ToString());
            e.ReportCell.Height = new FixedHeight(0);

            if (e.Column.Index < 11) return;

            var r = e.ReportCell.Parent.Parent.AddRow();
            UltraGridExporter.AddSelectorCell(e, r);
            var c = r.AddCell();
            var table = c.AddTable();
            var mainHeaderRow = table.AddRow();
            
            int columnIndex = 0;
            while (columnIndex < cellsWidth.Count)
            {
                if (columnIndex < 2)
                {
                    var headerCell = mainHeaderRow.AddCell();
                    headerCell.Width = new FixedWidth(cellsWidth[columnIndex]);
                    UltraGridExporter.SetCellStyle(e, headerCell);
                    var t = headerCell.AddText();
                    UltraGridExporter.SetFontStyle(t);
                    t.AddContent(cellsCaption[columnIndex]);
                    headerCell.Height = new FixedHeight(rowHeight * 3);

                    columnIndex++;
                }
                else if (columnIndex < 10)
                {
                    var headerContainerCell = mainHeaderRow.AddCell();
                    table = headerContainerCell.AddTable();
                    var headerRow = table.AddRow();
                    var headerCell = headerRow.AddCell();
                    headerCell.Height = new FixedHeight(rowHeight);
                    headerCell.Width = new FixedWidth(cellsWidth[columnIndex] * 4);
                    UltraGridExporter.SetCellStyle(e, headerCell);
                    var t = headerCell.AddText();
                    UltraGridExporter.SetFontStyle(t);
                    t.AddContent(headerCaption2);

                    headerRow = table.AddRow();
                    for (int i = 0; i < 2; i++)
                    {
                        headerCell = headerRow.AddCell();
                        headerCell.Height = new FixedHeight(rowHeight);
                        headerCell.Width = new FixedWidth(cellsWidth[columnIndex] * 2);
                        UltraGridExporter.SetCellStyle(e, headerCell);
                        t = headerCell.AddText();
                        UltraGridExporter.SetFontStyle(t);
                        if (i == 0) t.AddContent(headerCaption1);
                        if (i == 1) t.AddContent(headerCaption2);
                    }

                    headerRow = table.AddRow();
                    for (int i = 0; i < 4; i++)
                    {
                        headerCell = headerRow.AddCell();
                        headerCell.Height = new FixedHeight(rowHeight);
                        headerCell.Width = new FixedWidth(cellsWidth[columnIndex]);
                        UltraGridExporter.SetCellStyle(e, headerCell);
                        t = headerCell.AddText();
                        UltraGridExporter.SetFontStyle(t);
                        t.AddContent(cellsCaption[columnIndex + i]);
                    }
                    headerContainerCell.Width = new FixedWidth(cellsWidth[columnIndex] * 4);
                    columnIndex += 4;
                }
                else if (columnIndex == 10)
                {
                    var headerContainerCell = mainHeaderRow.AddCell();
                    table = headerContainerCell.AddTable();
                    var headerRow = table.AddRow();
                    var headerCell = headerRow.AddCell();
                    headerCell.Height = new FixedHeight(rowHeight);
                    headerCell.Width = new FixedWidth(cellsWidth[columnIndex] * 2);
                    UltraGridExporter.SetCellStyle(e, headerCell);
                    var t = headerCell.AddText();
                    UltraGridExporter.SetFontStyle(t);
                    t.AddContent(headerCaption3);

                    table = headerContainerCell.AddTable();
                    headerRow = table.AddRow();
                    for (int i = 0; i < 2; i++)
                    {
                        headerCell = headerRow.AddCell();
                        headerCell.Height = new FixedHeight(rowHeight);
                        headerCell.Width = new FixedWidth(cellsWidth[columnIndex]);
                        UltraGridExporter.SetCellStyle(e, headerCell);
                        t = headerCell.AddText();
                        UltraGridExporter.SetFontStyle(t);
                        if (i == 0) t.AddContent(headerCaption1);
                        if (i == 1) t.AddContent(headerCaption2);
                    }

                    headerRow = table.AddRow();
                    for (int i = 0; i < 2; i++)
                    {
                        headerCell = headerRow.AddCell();
                        headerCell.Height = new FixedHeight(rowHeight);
                        headerCell.Width = new FixedWidth(cellsWidth[columnIndex]);
                        UltraGridExporter.SetCellStyle(e, headerCell);
                        t = headerCell.AddText();
                        UltraGridExporter.SetFontStyle(t);
                        t.AddContent(" ");
                    }
                    headerContainerCell.Width = new FixedWidth(cellsWidth[columnIndex] * 2);
                    columnIndex += 2;
                }
            }
        }

        #endregion
    }
}
