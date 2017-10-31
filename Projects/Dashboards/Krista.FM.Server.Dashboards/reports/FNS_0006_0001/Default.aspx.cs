using System;
using System.Collections.Generic;
using System.Data;
using System.Web.UI.WebControls;
using Infragistics.Documents.Excel;
using Infragistics.Documents.Reports.Graphics;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Section;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.Documents.Excel;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.FNS_0006_0001
{
    public partial class Default : CustomReportPage
    {
        private int year = 2008;

        private GridHeaderLayout headerLayout;
        private GridHeaderLayout headerLayout1;
        private GridHeaderLayout headerLayout2;

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            UltraWebGrid.DataBinding += new EventHandler(UltraWebGrid_DataBinding);
            UltraWebGrid1.DataBinding += new EventHandler(UltraWebGrid1_DataBinding);
            UltraWebGrid2.DataBinding += new EventHandler(UltraWebGrid2_DataBinding);
            UltraWebGrid.InitializeLayout += new InitializeLayoutEventHandler(UltraWebGrid_InitializeLayout);
            UltraWebGrid1.InitializeLayout += new InitializeLayoutEventHandler(UltraWebGrid1_InitializeLayout);
            UltraWebGrid2.InitializeLayout += new InitializeLayoutEventHandler(UltraWebGrid2_InitializeLayout);
            UltraWebGrid.InitializeRow += new InitializeRowEventHandler(UltraWebGrid_InitializeRow);
            UltraWebGrid1.InitializeRow += new InitializeRowEventHandler(UltraWebGrid1_InitializeRow);
            UltraWebGrid2.InitializeRow += new InitializeRowEventHandler(UltraWebGrid2_InitializeRow);
            UltraWebGrid.DataBound += new EventHandler(UltraWebGrid_DataBound);
            UltraWebGrid1.DataBound += new EventHandler(UltraWebGrid1_DataBound);
            UltraWebGrid2.DataBound += new EventHandler(UltraWebGrid2_DataBound);

            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
        }
        
        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            DataTable dtDate = new DataTable();
            string query = DataProvider.GetQueryText("FNS_0006_0001_date");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);

            int startYear = 2007;
            int endYear = Convert.ToInt32(dtDate.Rows[0][0]);

            if (!Page.IsPostBack)
            {
                ComboFin.Width = 300;
                ComboFin.MultiSelect = false;
                ComboFin.FillDictionaryValues(GetPercentDictionary());
                ComboFin.Title = "Выбор ставки налогообложения";
                ComboFin.SetСheckedState("13%", true);

                ComboYear.Title = "Год";
                ComboYear.Width = 100;
                ComboYear.MultiSelect = false;
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(startYear, endYear));
                ComboYear.SetСheckedState(endYear.ToString(), true);
            }

            UserParams.Filter.Value = String.Format("[{0}]", GetFilterValue(ComboFin.SelectedIndex));

            UserParams.PeriodFirstYear.Value = (Convert.ToInt32(ComboYear.SelectedValue) + 1).ToString();
            UserParams.PeriodLastYear.Value = (Convert.ToInt32(ComboYear.SelectedValue)).ToString();
            UserParams.PeriodEndYear.Value = (Convert.ToInt32(ComboYear.SelectedValue) - 1).ToString();

            year = Convert.ToInt32(ComboYear.SelectedValue);

            headerLayout = new GridHeaderLayout(UltraWebGrid);
            headerLayout1 = new GridHeaderLayout(UltraWebGrid1);
            headerLayout2 = new GridHeaderLayout(UltraWebGrid2);

            UltraWebGrid.Bands.Clear();
            UltraWebGrid1.Bands.Clear();
            UltraWebGrid2.Bands.Clear();

            UltraWebGrid.Bands.Clear();

            UltraWebGrid.DataBind();
            UltraWebGrid1.DataBind();
            UltraWebGrid2.DataBind();

            Label1.Text = "Анализ налога на доходы физических лиц";
            Label2.Text = String.Format("за {0}-{1} годы по ставке {2}", UserParams.PeriodEndYear.Value, UserParams.PeriodLastYear.Value, ComboFin.SelectedValue);

            Label5.Text = String.Format("Уровень собираемости налога на доходы физических лиц {0}-{1} (на основании данных отчетности 1-НМ, 5-НДФЛ)", UserParams.PeriodEndYear.Value, UserParams.PeriodLastYear.Value);
        }

        void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            DataTable physFaces = new DataTable();
            string query = DataProvider.GetQueryText("FNS_0006_0001_PhysFaces");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Наименование показателей", physFaces);
            if (physFaces.Rows.Count > 0)
            {
                UltraWebGrid.DataSource = physFaces;
            }
        }

        void UltraWebGrid1_DataBinding(object sender, EventArgs e)
        {
            DataTable taxDeduction = new DataTable();
            string query = DataProvider.GetQueryText("FNS_0006_0001_TaxDeduction");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Наименование показателей", taxDeduction);

            if (taxDeduction.Rows.Count > 0)
            {
              UltraWebGrid1.DataSource = taxDeduction;
            }
            
        }

        void UltraWebGrid2_DataBinding(object sender, EventArgs e)
        {
            DataTable taxLevel = new DataTable();
            string query = DataProvider.GetQueryText("FNS_0006_0001_TaxLevel");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Ставка налога", taxLevel);

            for (int i = 0; i < taxLevel.Rows.Count; i++)
            {
                taxLevel.Rows[i][0] = TableRowName(i);
            }

            UltraWebGrid2.DataSource = taxLevel;
        }

        private void UltraWebGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.AllowColSizingDefault = AllowSizing.Fixed;
            e.Layout.AllowColumnMovingDefault = AllowColumnMoving.None;
            e.Layout.AllowSortingDefault = AllowSorting.No;

            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(150);
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;

            e.Layout.Bands[0].Columns[1].CellStyle.HorizontalAlign = HorizontalAlign.Center;

            headerLayout.AddCell("Наименование показателей");
            headerLayout.AddCell("Код строки", "Код строки в форме 5-НДФЛ");
            GridHeaderCell cell = headerLayout.AddCell("Значение показателей, руб");
            cell.AddCell((year - 1).ToString(), String.Format("Значение показателя за {0} год", year - 1));
            cell.AddCell(year.ToString(), String.Format("Значение показателя за {0} год", year));
            headerLayout.AddCell(String.Format("Динамика {0} к {1}", year, year - 1), String.Format("Отношение показателя {0} года к аналогичному периоду {1} года", year, year - 1));

            headerLayout.ApplyHeaderInfo();

            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[2], "N0");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[3], "N0");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[4], "P2");

            e.Layout.Bands[0].Columns[1].Width = CRHelper.GetColumnWidth(80, 1280);
            e.Layout.Bands[0].Columns[2].Width = CRHelper.GetColumnWidth(90, 1280);
            e.Layout.Bands[0].Columns[3].Width = CRHelper.GetColumnWidth(90, 1280);
            e.Layout.Bands[0].Columns[4].Width = CRHelper.GetColumnWidth(90, 1280);

        }

        private void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            bool direct = e.Row.Index < 5;
            SetConditionArrow(e, 4, direct, "показателя", 1);
        }

        private void UltraWebGrid1_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.AllowColSizingDefault = AllowSizing.Fixed;
            e.Layout.AllowColumnMovingDefault = AllowColumnMoving.None;
            e.Layout.AllowSortingDefault = AllowSorting.No;

            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(150);
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[1].CellStyle.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.Bands[0].Columns[e.Layout.Bands[0].Columns.Count - 1].Hidden = true;

            for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
            {
                if (i == 0 || i == 1 || i == 2 || i == 5)
                {
                    e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.OriginY = 0;
                    e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.SpanY = 2;
                }
                else
                {
                    e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.OriginY = 1;
                }
            }

            headerLayout1.AddCell("Наименование показателей");
            headerLayout1.AddCell("Код строки", "Код строки в форме 5-НДФЛ");
            headerLayout1.AddCell(String.Format("Количество справок по форме № 2-НДФЛ<br/>{0}, шт", year));
            GridHeaderCell cell = headerLayout1.AddCell("Значение показателей, руб");
            cell.AddCell((year - 1).ToString(), String.Format("Значение показателя за {0} год", year - 1));
            cell.AddCell(year.ToString(), String.Format("Значение показателя за {0} год", year));
            headerLayout1.AddCell(String.Format("Динамика {0} к {1}", year, year - 1), String.Format("Отношение показателя {0} года к аналогичному периоду {1} года", year, year - 1));

            headerLayout1.ApplyHeaderInfo();

            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[2], "N0");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[3], "N0");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[4], "N0");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[5], "P2");

            e.Layout.Bands[0].Columns[1].Width = CRHelper.GetColumnWidth(80, 1280);
            e.Layout.Bands[0].Columns[2].Width = CRHelper.GetColumnWidth(90, 1280);
            e.Layout.Bands[0].Columns[3].Width = CRHelper.GetColumnWidth(90, 1280);
            e.Layout.Bands[0].Columns[4].Width = CRHelper.GetColumnWidth(90, 1280);
            e.Layout.Bands[0].Columns[5].Width = CRHelper.GetColumnWidth(80, 1280);
        }

        private void UltraWebGrid1_InitializeRow(object sender, RowEventArgs e)
        {
            //SetConditionArrow(e, 5, true);
            string level = string.Empty;
            if (e.Row.Cells[e.Row.Cells.Count - 1] != null && e.Row.Cells[e.Row.Cells.Count - 1].Value.ToString() != string.Empty)
            {
                level = e.Row.Cells[e.Row.Cells.Count - 1].Value.ToString();
            }

            if (e.Row.Cells[0] != null && e.Row.Cells[0].Value.ToString() != string.Empty)
            {
                if (level == "1")
                {
                    e.Row.Cells[0].Style.Padding.Left = 20;
                    
                }
                else
                {
                    e.Row.Cells[0].Style.Font.Bold = true;
                }
            }

                string number = e.Row.Cells[0].Value.ToString().Replace("по коду ", "").Replace("вычета ", "");
                switch (number)
                {
                    case "101":
                        {
                            e.Row.Cells[0].Title = string.Format("600 руб. на каждого ребенка в возрасте до 18 лет, научащегося очной формы  обучения,  аспиранта,  ординатора,студента,  курсанта  в  возрасте  до  24  лет  родителям, супругам родителей", e.Row.Cells[0].Value);
                            break;
                        }
                    case "102":
                        {
                            e.Row.Cells[0].Title = string.Format("1200 руб. на каждого ребенка в возрасте до 18 лет, на учащегося очной формы  обучения,  аспиранта,  ординатора, студента, курсанта в возрасте до 24 лет  вдове  (вдовцу), одинокому  родителю,  опекуну  или  попечителю,  приемным родителям ", e.Row.Cells[0].Value);
                            break;
                        }
                    case "103":
                        {
                            e.Row.Cells[0].Title = string.Format("400 руб. на налогоплательщика, не относящегося к категориям, перечисленным в пп.1 -2  п. 1 ст.218 Налогового кодекса Российской Федерации", e.Row.Cells[0].Value);
                            break;
                        }
                    case "104":
                        {
                            e.Row.Cells[0].Title = string.Format("500 рублей на налогоплательщика, относящегося к категориям, перечисленным в пп. 2 п. 1 ст. 218 Налогового кодекса Российской Федерации ", e.Row.Cells[0].Value);
                            break;
                        }
                    case "105":
                        {
                            e.Row.Cells[0].Title = string.Format("3000 рублей на налогоплательщика, относящегося к категориям, перечисленным в пп. 1 п. 1 ст. 218 Налогового кодекса Российской Федерации ", e.Row.Cells[0].Value);
                            break;
                        }
                    case "106":
                        {
                            e.Row.Cells[0].Title = string.Format("1200 руб. на каждого ребенка-инвалида  в  возрасте до  18 лет,  на  учащегося  очной  формы  обучения,   аспиранта, ординатора, студента, курсанта  в  возрасте  до  24  лет, являющегося  инвалидом  I  или  II   группы,   родителям, супругам родителей ", e.Row.Cells[0].Value);
                            break;
                        }
                    case "107":
                        {
                            e.Row.Cells[0].Title = string.Format("2400 руб. на каждого ребенка-инвалида в  возрасте  до  18 лет,  на  учащегося  очной  формы  обучения,   аспиранта, ординатора, студента в возрасте до  24  лет,  являющегося инвалидом I или  II  группы,  вдове  (вдовцу),  одинокому родителю, опекуну или попечителю, приемным родителям ", e.Row.Cells[0].Value);
                            break;
                        }
                    case "108":
                        {
                            e.Row.Cells[0].Title = string.Format("1000 руб. на каждого ребенка в возрасте  до  18  лет,  на учащегося очной формы  обучения,  аспиранта,  ординатора, студента,    курсанта    в    возрасте    до    24    лет налогоплательщикам,  на  обеспечении  которых   находится ребенок  (родители,  супруги   родителей,   опекуны   или попечители,   приемные   родители,    супруги    приемных родителей) (начиная с доходов 2009 года) ", e.Row.Cells[0].Value);
                            break;
                        }
                    case "109":
                        {
                            e.Row.Cells[0].Title = string.Format("2000 руб. на каждого ребенка-инвалида в  возрасте  до  18 лет,  на  учащегося  очной  формы  обучения,   аспиранта, ординатора, студента в возрасте до  24  лет,  являющегося инвалидом  I  или  II  группы,   налогоплательщикам,   на обеспечении которых находится ребенок (родители,  супруги родителей, опекуны  или  попечители,  приемные  родители, супруги приемных родителей) (начиная с доходов 2009 года)", e.Row.Cells[0].Value);
                            break;
                        }
                    case "110":
                        {
                            e.Row.Cells[0].Title = string.Format("2000  руб.  на  каждого  ребенка  единственному  родителю (приемному  родителю),  опекуну,  попечителю  (начиная  с доходов 2009 года) ", e.Row.Cells[0].Value);
                            break;
                        }
                    case "111":
                        {
                            e.Row.Cells[0].Title = string.Format("2000 руб. на каждого ребенка родителю (приемному родителю) при условии отказа второго родителя (приемного родителя) от получения вычета в отношении этого ребенка (начиная с доходов 2009 года) ", e.Row.Cells[0].Value);
                            break;
                        }
                    case "112":
                        {
                            e.Row.Cells[0].Title = string.Format("4000 руб. на каждого ребенка-инвалида в  возрасте  до  18 лет,  на  учащегося  очной  формы  обучения,   аспиранта, ординатора, студента в возрасте до  24  лет,  являющегося инвалидом  I  или  II  группы,   единственному   родителю (приемному  родителю),  опекуну,  попечителю  (начиная  с доходов 2009 года) ", e.Row.Cells[0].Value);
                            break;
                        }
                    case "113":
                        {
                            e.Row.Cells[0].Title = string.Format("4000 руб. на каждого ребенка-инвалида в  возрасте  до  18 лет,  на  учащегося  очной  формы  обучения,   аспиранта, ординатора, студента в возрасте до  24  лет,  являющегося инвалидом I или II группы, родителю (приемному  родителю) при условии отказа второго родителя (приемного  родителя) от получения вычета в отношении этого ребенка (начиная  с доходов 2009 года) ", e.Row.Cells[0].Value);
                            break;
                        }
                    case "114":
                        {
                            e.Row.Cells[0].Title = "На первого ребенка в возрасте до 18 лет, а также на каждого учащегося очной формы обучения, аспиранта, ординатора, студента, курсанта в возрасте до 24 лет родителю, супруге (супругу) родителя, опекуну, попечителю, приемному родителю, супруге (супругу) приемного родителя, на обеспечении которых находится ребенок";
                            break;
                        }
                    case "115":
                        {
                            e.Row.Cells[0].Title = "На второго ребенка в возрасте до 18 лет, а также на каждого учащегося очной формы обучения, аспиранта, ординатора, студента, курсанта в возрасте до 24 лет родителю, супруге (супругу) родителя, опекуну, попечителю, приемному родителю, супруге (супругу) приемного родителя, на обеспечении которых находится ребенок";
                            break;
                        }
                    case "116":
                        {
                            e.Row.Cells[0].Title = "На третьего и каждого последующего ребенка в возрасте до 18 лет, а также на каждого учащегося очной формы обучения, аспиранта, ординатора, студента, курсанта в возрасте до 24 лет родителю, супруге (супругу) родителя, опекуну, попечителю, приемному родителю, супруге (супругу) приемного родителя, на обеспечении которых находится ребенок";
                            break;
                        }
                    case "117":
                        {
                            e.Row.Cells[0].Title = "На ребенка-инвалида в возрасте до 18 лет или учащегося очной формы обучения, аспиранта, ординатора, студента в возрасте до 24 лет, являющегося инвалидом I или II группы родителю, супруге (супругу) родителя, опекуну, попечителю, приемному родителю, супруге (супругу) приемного родителя, на обеспечении которых находится ребенок";
                            break;
                        }
                    case "118":
                        {
                            e.Row.Cells[0].Title = "В двойном размере на первого ребенка в возрасте до 18 лет, а также на каждого учащегося очной формы обучения, аспиранта, ординатора, студента, курсанта в возрасте до 24 лет единственному родителю (приемному родителю) опекуну, попечителю";
                            break;
                        }
                    case "119":
                        {
                            e.Row.Cells[0].Title = "В двойном размере на второго ребенка в возрасте до 18 лет, а также на каждого учащегося очной формы обучения, аспиранта, ординатора, студента, курсанта в возрасте до 24 лет единственному родителю (приемному родителю) опекуну, попечителю";
                            break;
                        }
                    case "120":
                        {
                            e.Row.Cells[0].Title = "В двойном размере на третьего и каждого последующего ребенка в возрасте до 18 лет, а также на каждого учащегося очной формы обучения, аспиранта, ординатора, студента, курсанта в возрасте до 24 лет единственному родителю (приемному родителю) опекуну, попечителю";
                            break;
                        }
                    case "121":
                        {
                            e.Row.Cells[0].Title = "В двойном размере на ребенка-инвалида в возрасте до 18 лет или учащегося очной формы обучения, аспиранта, ординатора, студента в возрасте до 24 лет, являющегося инвалидом I или II группы единственному родителю (приемному родителю) опекуну, попечителю";
                            break;
                        }
                    case "122":
                        {
                            e.Row.Cells[0].Title = "В двойном размере на первого ребенка в возрасте до 18 лет, а также на каждого учащегося очной формы обучения, аспиранта, ординатора, студента, курсанта в возрасте до 24 лет одному из родителей (приемных родителей) по их выбору на основании заявления об отказе одного из родителей (приемных родителей) от получения налогового вычета";
                            break;
                        }
                    case "123":
                        {
                            e.Row.Cells[0].Title = "В двойном размере на второго ребенка в возрасте до 18 лет, а также на каждого учащегося очной формы обучения, аспиранта, ординатора, студента, курсанта в возрасте до 24 лет одному из родителей (приемных родителей) по их выбору на основании заявления об отказе одного из родителей (приемных родителей) от получения налогового вычета";
                            break;
                        }
                    case "124":
                        {
                            e.Row.Cells[0].Title = "В двойном размере на третьего и каждого последующего ребенка в возрасте до 18 лет, а также на каждого учащегося очной формы обучения, аспиранта, ординатора, студента, курсанта в возрасте до 24 лет одному из родителей (приемных родителей) по их выбору на основании заявления об отказе одного из родителей (приемных родителей) от получения налогового вычета";
                            break;
                        }
                    case "125":
                        {
                            e.Row.Cells[0].Title = "В двойном размере на ребенка-инвалида в возрасте до 18 лет или учащегося очной формы обучения, аспиранта, ординатора, студента в возрасте до 24 лет, являющегося инвалидом I или II группы, одному из родителей (приемных родителей) по их выбору на основании заявления об отказе одного из родителей (приемных родителей) от получения налогового вычета";
                            break;
                        }
                    case "201":
                        {
                            e.Row.Cells[0].Title = "Расходы по операциям с ценными бумагами, обращающимися на организованном рынке ценных бумаг";
                            break;
                        }
                    case "202":
                        {
                            e.Row.Cells[0].Title = "Расходы по операциям с ценными бумагами, не обращающимися на организованном рынке ценных бумаг";
                            break;
                        }
                    case "203":
                        {
                            e.Row.Cells[0].Title = "Расходы по операциям с ценными бумагами, не обращающимися на организованном рынке ценных бумаг, которые на момент их приобретения относились к ценным бумагам, обращающимся на организованном рынке ценных бумаг";
                            break;
                        }
                    case "204":
                        {
                            e.Row.Cells[0].Title = "Сумма убытка по операциям с ценными бумагами, не обращающимися на организованном рынке ценных бумаг, которые на момент их приобретения относились к ценным бумагам, обращающимся на организованном рынке ценных бумаг, уменьшающая налоговую базу по операциям с ценными бумагами, обращающимися на организованном рынке ценных бумаг";
                            break;
                        }
                    case "205":
                        {
                            e.Row.Cells[0].Title = "Сумма убытка по операциям с ценными бумагами, обращающимися на организованном рынке ценных бумаг, уменьшающая налоговую базу по операциям с финансовыми инструментами срочных сделок которые обращаются на организованном рынке и базисным активом которых являются ценные бумаги, фондовые индексы или иные финансовые инструменты срочных сделок, базисным активом которых являются ценные бумаги или фондовые индексы";
                            break;
                        }
                    case "206":
                        {
                            e.Row.Cells[0].Title = "Расходы по операциям с финансовыми инструментами срочных сделок, которые обращаются на организованном рынке и базисным активом которых являются ценные бумаги, фондовые индексы или иные финансовые инструменты срочных сделок, базисным активом которых являются ценные бумаги или фондовые индексы";
                            break;
                        }
                    case "207":
                        {
                            e.Row.Cells[0].Title = "Расходы по операциям с финансовыми инструментами срочных сделок, которые обращаются на организованном рынке и базисным активом которых не являются ценные бумаги, фондовые индексы или иные финансовые инструменты срочных сделок, базисным активом которых являются ценные бумаги или фондовые индексы";
                            break;
                        }
                    case "208":
                        {
                            e.Row.Cells[0].Title = "Сумма убытка по операциям с финансовыми инструментами срочных сделок, которые обращаются на организованном рынке и базисным активом которых являются ценные бумаги, фондовые индексы или иные финансовые инструменты срочных сделок, базисным активом которых являются ценные бумаги или фондовые индексы, уменьшающая налоговую базу по операциям с ценными бумагами, обращающимися на организованном рынке ценных бумаг";
                            break;
                        }
                    case "209":
                        {
                            e.Row.Cells[0].Title = "Сумма убытка по операциям с финансовыми инструментами срочных сделок которые обращаются на организованном рынке и базисным активом которых не являются ценные бумаги, фондовые индексы или иные финансовые инструменты срочных сделок, базисным активом которых являются ценные бумаги или фондовые индексы, уменьшающая налоговую базу по операциям с финансовыми инструментами срочных сделок которые обращаются на организованном рынке";
                            break;
                        }
                    case "210":
                        {
                            e.Row.Cells[0].Title = "Сумма убытка по операциям с финансовыми инструментами срочных сделок, обращающимися на организованном рынке ценных бумаг и базисным активом которых являются ценные бумаги, фондовые индексы или иные финансовые инструменты срочных сделок, базисным активом которых являются ценные бумаги или фондовые индексы, уменьшающая налоговую базу по операциям с финансовыми инструментами срочных сделок, которые обращаются на организованном рынке";
                            break;
                        }
                    case "211":
                        {
                            e.Row.Cells[0].Title = "Расходы, в виде процентов по займу, произведенные по совокупности операций РЕПО";
                            break;
                        }
                    case "212":
                        {
                            e.Row.Cells[0].Title = "Сумма превышения расходов по операциям РЕПО над доходами по операциям РЕПО, уменьшающая налоговую базу по операциям с ценными бумагами, обращающимися на организованном рынке ценных бумаг, рассчитанная в соответствии с пропорцией, с учетом положений абзаца третьего пункта 6 статьи 214.3 Налогового кодекса Российской Федерации";
                            break;
                        }
                    case "213":
                        {
                            e.Row.Cells[0].Title = "Расходы по операциям, связанным с закрытием короткой позиции, и затраты, связанные с приобретением и реализацией ценных бумаг, являющимся объектом операций РЕПО";
                            break;
                        }
                    case "214":
                        {
                            e.Row.Cells[0].Title = "Убытки по операциям, связанным с закрытием короткой позиции, и затраты, связанные с приобретением и реализацией ценных бумаг, являющимся объектом операций РЕПО";
                            break;
                        }
                    case "215":
                        {
                            e.Row.Cells[0].Title = "Расходы в виде процентов, уплаченных в налоговом периоде по совокупности договоров займа";
                            break;
                        }
                    case "216":
                        {
                            e.Row.Cells[0].Title =
                                "Сумма превышения расходов в виде процентов, уплаченных по совокупности договоров займа над доходами, полученными по совокупности договоров займа, уменьшающая налоговую базу по операциям с ценными бумагами, обращающимися на организованном рынке ценных бумаг, рассчитанная в соответствии с пропорцией, с учетом положений абзаца шестого пункта 5 статьи 214.4 Налогового кодекса Российской Федерации";
                            break;
                        }
                    case "217":
                        {
                            e.Row.Cells[0].Title = "Сумма превышения расходов в виде процентов, уплаченных по совокупности договоров займа над доходами, полученными по совокупности договоров займа, уменьшающая налоговую базу по операциям с ценными бумагами, не обращающимися на организованном рынке ценных бумаг, рассчитанная в соответствии с пропорцией, с учетом положений абзаца шестого пункта 5 статьи 214.4 Налогового кодекса Российской Федерации";
                            break;
                        }
                    case "301":
                        {
                            e.Row.Cells[0].Title = string.Format("Суммы частичной уплаты налогоплательщиком стоимости полученных им товаров, выполненных для него работ, оказанных ему услуг", e.Row.Cells[0].Value);
                            break;
                        }
                    case "305":
                        {
                            e.Row.Cells[0].Title = string.Format("Сумма фактически произведенных и документально подтвержденных   расходов,   связанных   с   заключением, исполнением и с прекращением срочных сделок ", e.Row.Cells[0].Value);
                            break;
                        }
                    case "306":
                        {
                            e.Row.Cells[0].Title = string.Format("Сумма фактически произведенных и документально подтвержденных  расходов  по  приобретению,  хранению   и реализации (погашению) инвестиционных паев паевых инвестиционных фондов ", e.Row.Cells[0].Value);
                            break;
                        }
                    case "307":
                        {
                           e.Row.Cells[0].Title = string.Format("Сумма фактически произведенных и документально подтвержденных расходов  на  приобретение,  реализацию  и хранение ценных  бумаг, обращающихся на организованном рынке ценных бумаг, включая суммы, с которых был исчислен и  уплачен  налог  при  приобретении   ценных   бумаг   в собственность (в том числе при получении на безвозмездной основе или с частичной оплатой), а также суммы процентов, уплаченных   за   пользование    денежными    средствами, привлеченными для совершения сделки купли-продажи  ценных бумаг ",e.Row.Cells[0].Title);
                            break;
                        }
                    case "308":
                        {
                           e.Row.Cells[0].Title = string.Format("Сумма фактически произведенных и документально подтвержденных расходов  на  приобретение,  реализацию  и хранение ценных бумаг, не обращающихся на  организованном рынке ценных бумаг, включая суммы, с которых был исчислен и  уплачен  налог  при  приобретении   ценных   бумаг   в собственность (в том числе при получении на безвозмездной основе или с частичной оплатой) ",e.Row.Cells[0].Title);
                            break;
                        }
                    case "309":
                        {
                           e.Row.Cells[0].Title = string.Format("Сумма фактически произведенных и документально подтвержденных расходов  на  приобретение,  реализацию  и хранение ценных бумаг, не обращающихся на  организованном рынке ценных бумаг, которые  на  момент  их  приобретения отвечали требованиям, предъявляемым к обращающимся ценным бумагам, включая суммы, с которых был исчислен и  уплачен налог при приобретении ценных бумаг  в  собственность  (в том числе при получении на  безвозмездной  основе  или  с частичной оплатой), а также сумма убытка,  полученного  в налоговом  периоде  по  операциям  купли-продажи   ценных бумаг, обращающихся на организованном рынке ценных бумаг, учитываемая при определении налоговой базы  по  операциям купли-продажи   ценных   бумаг,   не   обращающихся    на организованном рынке ценных бумаг, которые на  момент  их приобретения  отвечали  требованиям,  установленным   для ценных бумаг, обращающихся на организованном рынке ценных бумаг ", e.Row.Cells[0].Value);
                            break;
                        }
                    case "310":
                        {
                           e.Row.Cells[0].Title = string.Format("Убыток, полученный в налоговом периоде по операциям купли-продажи ценных бумаг, обращающихся на организованном рынке ценных бумаг, учитываемый при определении налоговой базы по операциям купли-продажи ценных бумаг, обращающихся на организованном рынке ценных бумаг, которые на момент их приобретения отвечали требованиям, установленным для ценных бумаг, обращающихся на организованном рынке ценных бумаг", e.Row.Cells[0].Value);
                            break;
                        }
                    case "316":
                        {
                           e.Row.Cells[0].Title = string.Format("Суммы, полученные от продажи ценных бумаг, находившихся в собственности  налогоплательщика  менее  3  лет,  но   не превышающих 125  000  руб.  (по  доходам,  полученным  до 01.01.2007) ", e.Row.Cells[0].Value);
                            break;
                        }
                    case "317":
                        {
                           e.Row.Cells[0].Title = string.Format("Суммы, полученные от продажи ценных бумаг, находившихся в собственности  налогоплательщика  3  года  и  более   (по доходам, полученным до 01.01.2007) ", e.Row.Cells[0].Value);
                            break;
                        }
                    case "311":
                        {
                           e.Row.Cells[0].Title = string.Format("Сумма,  израсходованная   налогоплательщиком   на   новое строительство либо приобретение на территории  Российской Федерации жилого дома, квартиры, комнаты или доли (долей) в   них,   земельных   участков,   предоставленных    для индивидуального  жилищного  строительства,  и   земельных участков,  на  которых  расположены  приобретаемые  жилые дома, или доли (долей) в них (кроме сумм, направленных на погашение  процентов  по  целевым  займам  (кредитам)   в размере   фактически   произведенных   и    документально подтвержденных расходов в пределах установленного размера имущественного  налогового   вычета   в   соответствующем налоговом периоде ", e.Row.Cells[0].Value);
                            break;
                        }
                    case "312":
                        {
                           e.Row.Cells[0].Title = string.Format("Сумма,  направленная на погашение  процентов  по  целевым займам (кредитам), полученным от  российских  организаций  или   индивидуальных   предпринимателей   и    фактически израсходованная налогоплательщиком на новое строительство или  приобретение  на  территории  Российской   Федерации жилого дома, квартиры, комнаты или доли  (долей)  в  них, земельных участков, предоставленных  для  индивидуального жилищного строительства, и земельных участков, на которых расположены приобретаемые жилые дома, или доли (долей)  в них ", e.Row.Cells[0].Value);
                            break;
                        }
                    case "318":
                        {
                           e.Row.Cells[0].Title = string.Format("Сумма, направленная на погашение процентов по  кредитам, полученным   от   банков,   находящихся   на   территории Российской   Федерации,    в    целях    рефинансирования (перекредитования) кредитов на новое  строительство  либо приобретение на территории  Российской  Федерации  жилого  дома, квартиры, комнаты или доли (долей) в них, земельных участков, предоставленных для  индивидуального  жилищного строительства,   и   земельных   участков,   на   которых расположены приобретаемые жилые дома, или доли (долей)  в них ", e.Row.Cells[0].Value);
                            break;
                        }
                    case "319":
                        {
                            e.Row.Cells[0].Title = string.Format("Сумма  уплаченных налогоплательщиком в налоговом  периоде пенсионных    взносов     по     договору     (договорам) негосударственного пенсионного обеспечения,  заключенному (заключенным)  налогоплательщиком   с   негосударственным пенсионным фондом в свою пользу и (или) в пользу  супруга (в том числе в пользу вдовы, вдовца),  родителей  (в  том числе  усыновителей),  детей-инвалидов   (в   том   числе усыновленных, находящихся под опекой (попечительством), и (или) в сумме уплаченных налогоплательщиком  в  налоговом периоде  страховых  взносов   по   договору   (договорам) добровольного   пенсионного   страхования,   заключенному (заключенным) со страховой организацией в свою  пользу  и (или) в пользу  супруга  (в  том  числе  вдовы,  вдовца), родителей (в том числе усыновителей), детей-инвалидов  (в том   числе   усыновленных,   находящихся   под    опекой (попечительством), - в размере  фактически  произведенных расходов с учетом ограничения, установленного  пунктом  2 статьи 219 Налогового кодекса Российской Федерации ", e.Row.Cells[0].Value);
                            break;
                        }
                    case "313":
                        {
                            e.Row.Cells[0].Title = string.Format("Сумма,  израсходованная   налогоплательщиком   на   новое  строительство либо приобретение на территории  Российской Федерации жилого дома, квартиры, комнаты или доли (долей) в них (кроме сумм, направленных на погашение процентов по целевым займам (кредитам) и фактически израсходованных на новое  строительство  или  приобретение   на   территории Российской Федерации жилого дома, квартиры,  комнаты  или доли (долей) в них), в размере фактически произведенных и документально подтвержденных расходов в  пределах  2  000 000 руб. (начиная с доходов 2008 года) ", e.Row.Cells[0].Value);
                            break;
                        }
                    case "403":
                        {
                            e.Row.Cells[0].Title = string.Format("Сумма фактически произведенных и документально подтвержденных  расходов,  непосредственно  связанных   с выполнением  работ   (оказанием   услуг)   по   договорам гражданско-правового характера ", e.Row.Cells[0].Value);
                            break;
                        }
                    case "404":
                        {
                            e.Row.Cells[0].Title = string.Format("Сумма фактически произведенных и документально  подтвержденных расходов, связанных с получением авторских  вознаграждений или вознаграждений за  создание,  издание, исполнение или  иное  использование  произведений  науки, литературы и искусства, вознаграждений авторам  открытий, изобретений и промышленных образцов ", e.Row.Cells[0].Value);
                            break;
                        }
                    case "405":
                        {
                            e.Row.Cells[0].Title = string.Format("Сумма  в  пределах   нормативов   затрат,   связанных   с получением авторских вознаграждений и  вознаграждений  за создание, исполнение или иное использование  произведений науки, литературы  и  искусства,  вознаграждений  авторам открытий,  изобретений   и   промышленных   образцов   (в процентах к сумме начисленного дохода)", e.Row.Cells[0].Value);
                            break;
                        }
                    case "501":
                        {
                            e.Row.Cells[0].Title = string.Format("Вычет из стоимости подарков, полученных от организаций  и индивидуальных предпринимателей ", e.Row.Cells[0].Value);
                            break;
                        }
                    case "502":
                        {
                            e.Row.Cells[0].Title = string.Format("Вычет  из  стоимости  призов  в  денежной  и  натуральной формах,  полученных   на   конкурсах   и   соревнованиях, проводимых  в  соответствии  с  решениями   Правительства  Российской Федерации, законодательных  (представительных) органов  государственной  власти   или   представительных органов местного самоуправления ", e.Row.Cells[0].Value);
                            break;
                        }
                    case "503":
                        {
                            e.Row.Cells[0].Title = string.Format("Вычет  из суммы материальной помощи,   оказываемой  работодателями своим работникам,  а  также  бывшим  своим работникам, уволившимся в связи с выходом  на  пенсию  по инвалидности или по возрасту ", e.Row.Cells[0].Value);
                            break;
                        }
                    case "504":
                        {
                            e.Row.Cells[0].Title = string.Format("Вычет из суммы возмещения (оплаты)  работодателями  своим работникам, их супругам, родителям и детям, бывшим  своим  работникам (пенсионерам по возрасту), а  также  инвалидам стоимости  приобретенных  ими  (для  них)   медикаментов, назначенных им лечащим врачом ", e.Row.Cells[0].Value);
                            break;
                        }
                    case "505":
                        {
                            e.Row.Cells[0].Title = string.Format("Вычет из стоимости  выигрышей  и  призов,  полученных  на конкурсах, играх и других мероприятиях  в  целях  рекламы  товаров (работ, услуг) ", e.Row.Cells[0].Value);
                            break;
                        }
                    case "506":
                        {
                            e.Row.Cells[0].Title = string.Format("Вычет из суммы материальной помощи, оказываемой инвалидам общественными организациями инвалидов", e.Row.Cells[0].Value);
                            break;
                        }
                    case "507":
                        {
                            e.Row.Cells[0].Title = string.Format(" Вычет из суммы помощи (в денежной и натуральной  формах), а также стоимости подарков, полученных ветеранами Великой  Отечественной  войны,  инвалидами  Великой  Отечественной войны, вдовами военнослужащих, погибших в период войны  с Финляндией, Великой Отечественной войны, войны с Японией, вдовами умерших инвалидов Великой Отечественной  войны  и бывшими узниками нацистских концлагерей, тюрем и гетто, а также бывшими несовершеннолетними  узниками  концлагерей, гетто и других мест принудительного содержания, созданных фашистами и их союзниками в период Второй мировой войны ", e.Row.Cells[0].Value);
                            break;
                        }
                    case "508":
                        {
                            e.Row.Cells[0].Title = string.Format(" Вычет  из  суммы  единовременной   материальной   помощи, оказываемой   работодателями    работникам    (родителям, усыновителям,   опекунам)   при   рождении   (усыновлении (удочерении)) ребенка (начиная с доходов 2008 года) ", e.Row.Cells[0].Value);
                            break;
                        }
                    case "509":
                        {
                            e.Row.Cells[0].Title = string.Format(" Вычет   из  суммы  доходов,  полученных   работниками   в натуральной форме в качестве оплаты труда от  организаций - сельскохозяйственных товаропроизводителей, определяемых в  соответствии  с  пунктом  2  статьи  346.2  Налогового кодекса Российской Федерации,  крестьянских  (фермерских) хозяйств  в  виде   сельскохозяйственной   продукции   их собственного  производства   и   (или)   работ   (услуг), выполненных   (оказанных)    такими    организациями    и крестьянскими  (фермерскими)  хозяйствами   в   интересах работника,  имущественных  прав,  переданных   указанными организациями и крестьянскими  (фермерскими)  хозяйствами работнику (начиная с доходов 2009 года и  применяется  до 01.01.2016 года)", e.Row.Cells[0].Value);
                            break;
                        }
                    case "601":
                        {
                            e.Row.Cells[0].Title = string.Format(" Сумма, уменьшающая  налоговую  базу  по  доходам  в  виде  дивидендов ", e.Row.Cells[0].Value);
                            break;
                        }
                    case "602":
                        {
                            e.Row.Cells[0].Title = string.Format(" Сумма внесенных налогоплательщиком страховых взносов ", e.Row.Cells[0].Value);
                            break;
                        }
                    case "603":
                        {
                            e.Row.Cells[0].Title = string.Format(" Вычет  в  сумме  внесенных  налогоплательщиком  страховых взносов   по    договорам    добровольного    пенсионного страхования в случае,  если  налогоплательщик  представил справку, выданную налоговым органом по  месту  жительства  налогоплательщика,       подтверждающую       неполучение налогоплательщиком социального налогового вычета ", e.Row.Cells[0].Value);
                            break;
                        }
                    case "604":
                        {
                            e.Row.Cells[0].Title = string.Format(" Рыночная  стоимость  застрахованного  имущества  на  дату заключения  договора  (на  дату  наступления   страхового случая   -   по    договору    страхования    гражданской ответственности),  увеличенная  на  сумму  уплаченных  по страхованию этого имущества страховых взносов ", e.Row.Cells[0].Value);
                            break;
                        }
                    case "605":
                        {
                            e.Row.Cells[0].Title = string.Format(" Расходы, необходимые для проведения ремонта (восстановления) имущества (в  случае,  если  ремонт  не  осуществлялся), или  стоимость  ремонта  (восстановления) этого  имущества  (в случае осуществления ремонта), увеличенные на сумму уплаченных по страхованию этого имущества страховых взносов ", e.Row.Cells[0].Value);
                            break;
                        }
                    case "607":
                        {
                            e.Row.Cells[0].Title = string.Format(" Вычет в сумме уплаченных работодателем страховых взносов за работника в  соответствии  с  Федеральным  законом  \"О дополнительных страховых взносах на  накопительную  часть трудовой пенсии и государственной поддержке  формирования пенсионных накоплений\", но не более 12000 рублей в год ", e.Row.Cells[0].Value);
                            break;
                        }
                    case "606":
                        {
                            e.Row.Cells[0].Title = string.Format(" Вычет в сумме внесенных  налогоплательщиком   платежей (взносов)  по  договорам  негосударственного  пенсионного обеспечения,  заключенным  с   имеющими   соответствующую лицензию   российскими   негосударственными   пенсионными фондами,  в  случае,  если  налогоплательщик   представил справку, выданную налоговым органом по  месту  жительства налогоплательщика,  подтверждающую неполучение налогоплательщиком социального налогового вычета ", e.Row.Cells[0].Value);
                            break;
                        }
                    case "620":
                        {
                            e.Row.Cells[0].Title = string.Format(" Иные суммы, уменьшающие налоговую базу в  соответствии  с положениями  главы  23  Налогового   кодекса   Российской Федерации ", e.Row.Cells[0].Value);
                            break;
                        }
              }
        }
        
        void UltraWebGrid2_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.AllowColSizingDefault = AllowSizing.Fixed;
            e.Layout.AllowColumnMovingDefault = AllowColumnMoving.None;
            e.Layout.AllowSortingDefault = AllowSorting.No;

            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(150);
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;

            e.Layout.Bands[0].Columns[1].CellStyle.HorizontalAlign = HorizontalAlign.Center;

            headerLayout2.AddCell("Ставка налога");

            GridHeaderCell cell = headerLayout2.AddCell((year - 1).ToString());
            cell.AddCell("Исчислено (5-НДФЛ), руб");
            cell.AddCell("Поступило (1-НМ), руб");
            cell.AddCell("Собираемость");

            cell = headerLayout2.AddCell((year).ToString());
            cell.AddCell("Исчислено (5-НДФЛ), руб");
            cell.AddCell("Поступило (1-НМ), руб");
            cell.AddCell("Собираемость");

            cell = headerLayout2.AddCell((year + 1) + "<br/>(справочно)");
            cell.AddCell("Поступило (1-НМ), руб");

            headerLayout2.AddCell(String.Format("Изменение собираемости {0} от уровня {1}", year, year - 1));

            headerLayout2.ApplyHeaderInfo();

            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[1], "N0");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[2], "N0");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[3], "P2");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[4], "N0");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[5], "N0");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[6], "P2");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[7], "N0");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[8], "P2");

            e.Layout.Bands[0].Columns[1].Width = CRHelper.GetColumnWidth(110, 1280);
            e.Layout.Bands[0].Columns[2].Width = CRHelper.GetColumnWidth(120, 1280);
            e.Layout.Bands[0].Columns[3].Width = CRHelper.GetColumnWidth(110, 1280);
            e.Layout.Bands[0].Columns[4].Width = CRHelper.GetColumnWidth(110, 1280);
            e.Layout.Bands[0].Columns[5].Width = CRHelper.GetColumnWidth(120, 1280);
            e.Layout.Bands[0].Columns[6].Width = CRHelper.GetColumnWidth(110, 1280);
            e.Layout.Bands[0].Columns[7].Width = CRHelper.GetColumnWidth(120, 1280);
            e.Layout.Bands[0].Columns[8].Width = CRHelper.GetColumnWidth(110, 1280);

            e.Layout.Bands[0].Columns[9].Hidden = true;
            e.Layout.Bands[0].Columns[10].Hidden = true;
        }

        void UltraWebGrid2_InitializeRow(object sender, RowEventArgs e)
        {
            SetRankImage(e, 9, 3);
            SetRankImage(e, 10, 6);
            SetConditionArrow(e, 8, true, "уровня собираемости", 1);
        }

        void UltraWebGrid_DataBound(object sender, EventArgs e)
        {
            UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth * 0.465);
            UltraWebGrid.Height = Unit.Empty;
        }

        void UltraWebGrid1_DataBound(object sender, EventArgs e)
        {
            UltraWebGrid1.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth * 0.535);
            UltraWebGrid1.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight / 1.8 ); ;
        }

        void UltraWebGrid2_DataBound(object sender, EventArgs e)
        {
            UltraWebGrid2.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth);
            UltraWebGrid2.Height = Unit.Empty;
        }
        
        public static void SetConditionArrow(RowEventArgs e, int index, bool direct, string description, int borderValue)
        {
            if (e.Row.Cells[index] != null &&
                e.Row.Cells[index].Value != null)
            {
                double value = Convert.ToDouble(e.Row.Cells[index].Value.ToString());
                string img;
                string title;
                if (direct)
                {
                    if (value > borderValue)
                    {
                        img = "~/images/arrowGreenUpBB.png";
                        title = String.Format("Рост {0} к аналогичному периоду прошлого года", description);
                    }
                    else
                    {
                        img = "~/images/arrowRedDownBB.png";
                        title = String.Format("Снижение {0} к аналогичному периоду прошлого года", description);
                    }
                }
                else
                {
                    if (value > borderValue)
                    {
                        img = "~/images/arrowRedUpBB.png";
                        title = String.Format("Рост {0} к аналогичному периоду прошлого года", description);
                    }
                    else
                    {
                        img = "~/images/arrowGreenDownBB.png";
                        title = String.Format("Снижение {0} к аналогичному периоду прошлого года", description);
                    }
                }
                e.Row.Cells[index].Style.BackgroundImage = img;
                e.Row.Cells[index].Style.CustomRules = "background-repeat: no-repeat; background-position: 10px center; padding-left: 0px";
                e.Row.Cells[index].Title = title;
            }
        }

        private Dictionary<string, int> GetPercentDictionary()
        {
            Dictionary<string, int> kinds = new Dictionary<string, int>();
            kinds.Add("13%", 0);
            kinds.Add("30%", 0);
            kinds.Add("9%", 0);
            kinds.Add("35%", 0);
            kinds.Add("Иные", 0);
            return kinds;
        }

        private string GetFilterValue(int index)
        {
            switch (index)
            {
                case 0:
                    {
                        return
                            "Раздел I. Налоговая база, подлежащая налогообложению по ставке 13%, общая сумма исчисленного, удержанного и перечисленного налога";
                    }
                case 1:
                    {
                        return
                            "Раздел II. Налоговая база, подлежащая налогообложению по ставке 30%, общая сумма исчисленного, удержанного и перечисленного налога";
                    }
                case 2:
                    {
                        return
                            "Раздел III. Налоговая база, подлежащая налогообложению по ставке 9%, общая сумма исчисленного удержанного и перечисленного налога";
                    }
                case 3:
                    {
                        return
                            "Раздел IV. Налоговая база, подлежащая налогообложению по ставке 35%, общая сумма исчисленного, удержанного и перечисленного налога";
                    }
                case 4:
                    {
                        return
                            "Раздел V. Налоговая база, подлежащая налогообложению по налоговым ставкам, установленных в Соглашениях об избежании двойного налогообложения, общая сумма исчисленного, удержанного и перечисленного налога (5%, 10%, 15%)";
                    }
            }
            return String.Empty;
        }

        private string TableRowName(int index)
        {
            switch (index)
            {
                case 0:
                    {
                        return
                            "13%";
                    }
                case 1:
                    {
                        return
                            "30%";
                    }
                case 2:
                    {
                        return
                            "9%";
                    }
                case 3:
                    {
                        return
                            "35%";
                    }
                case 4:
                    {
                        return
                            "Иные";
                    }
            }
            return String.Empty;
        }

        public static void SetRankImage(RowEventArgs e, int rankCellIndex, int indicateCellIndex)
        {
            if (e.Row.Cells[rankCellIndex] != null &&
                e.Row.Cells[rankCellIndex].Value != null)
            {
                int value = Convert.ToInt32(e.Row.Cells[rankCellIndex].Value.ToString());
                int worseRankValue = 5;
                string img = String.Empty;
                string title = String.Empty;
                if (value == 1)
                {
                    img = "~/images/StarYellowBB.png";
                    title = "Самый высокий уровень собираемости налога";
                }
                else if (value == worseRankValue)
                {
                    img = "~/images/StarGrayBB.png";
                    title = "Самый низкий уровень собираемости налога";
                }

                e.Row.Cells[indicateCellIndex].Style.BackgroundImage = img;
                e.Row.Cells[indicateCellIndex].Style.CustomRules = "background-repeat: no-repeat; background-position: 30px center; padding-left: 2px; padding-right: 10px";
                e.Row.Cells[indicateCellIndex].Title = title;
            }
        }


        #region  Экспорт

        void ExcelExportButton_Click(object sender, EventArgs e)
        {
            Workbook book = new Workbook();
            ReportExcelExporter1.SheetColumnCount = 20;
            ReportExcelExporter1.WorksheetTitle = Label1.Text + " " + Label2.Text;

            Worksheet sheet = book.Worksheets.Add("Анализ доходов физических лиц");
            // sheet.Rows[1].Cells[0].Value = "Анализ доходов физических лиц по ставкам налогообложения";
            ReportExcelExporter1.Export(headerLayout, Label3.Text,sheet, 4);

            sheet = book.Worksheets.Add("Анализ налоговых вычетов");
            // sheet.Rows[1].Cells[0].Value = "Анализ налоговых вычетов";
            ReportExcelExporter1.Export(headerLayout1, Label4.Text ,sheet, 4);

            sheet = book.Worksheets.Add("Уровень собираемости налога");
            // sheet.Rows[1].Cells[0].Value = Label5.Text;
            ReportExcelExporter1.Export(headerLayout2,Label5.Text, sheet, 4);
        }

        void PdfExportButton_Click(object sender, EventArgs e)
        {
            ReportPDFExporter1.PageTitle = String.Empty;
            ReportPDFExporter1.PageSubTitle = String.Empty;

            Report report = new Report();
            ISection section = report.AddSection();

            IText title = section.AddText();
            title.Style.Font = new Font("Verdana", 18);
            title.Style.Font.Bold = true;
            title.AddContent(Label1.Text);

            title = section.AddText();
            title.Style.Font = new Font("Verdana", 16);
            title.AddContent(Label2.Text);

            IText text = section.AddText();
            text.AddContent("Анализ доходов физических лиц по ставкам налогообложения");
            text.Style.Font = new Font("Verdana", 14);
            ReportPDFExporter1.Export(headerLayout, section);

            text = section.AddText();
            text.AddContent("Анализ налоговых вычетов");
            text.Style.Font = new Font("Verdana", 14);
            ReportPDFExporter1.Export(headerLayout1, section);

            text = section.AddText();
            text.AddContent(Label5.Text);
            text.Style.Font = new Font("Verdana", 14);
            ReportPDFExporter1.Export(headerLayout2, section);
        }
        #endregion



    }
}
