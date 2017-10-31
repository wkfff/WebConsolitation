using System;
using System.Data;
using System.Web.UI.WebControls;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.FK_0002_0001
{
    public partial class Default : CustomReportPage
    {
        private CustomParam StateAreaCompare;
        private CustomParam RegionCompare;

        private GridHeaderLayout headerLayout;

        private DateTime reportDate;

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            UltraWebGrid.DataBinding += new EventHandler(UltraWebGrid_DataBinding);
            UltraWebGrid.InitializeLayout += new InitializeLayoutEventHandler(UltraWebGrid_InitializeLayout);
            UltraWebGrid.InitializeRow += new InitializeRowEventHandler(UltraWebGrid_InitializeRow);
            UltraWebGrid.DataBound += new EventHandler(UltraWebGrid_DataBound);

            //ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);

            StateAreaCompare = CustomParam.CustomParamFactory("state_area_compare");
            RegionCompare = CustomParam.CustomParamFactory("region_compare");
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            DataTable dtDate = new DataTable();
            string query = DataProvider.GetQueryText("FK_0002_0001_date");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);

            int startYear = 2006;
            int endYear = Convert.ToInt32(dtDate.Rows[0][0]);

            if (!Page.IsPostBack)
            {
                RegionsCombo.Width = 360;
                RegionsCombo.Title = "Субъект РФ";
                RegionsCombo.FillDictionaryValues(CustomMultiComboDataHelper.FillRegions(RegionsNamingHelper.FoNames, RegionsNamingHelper.RegionsFoDictionary));
                RegionsCombo.ParentSelect = false;
                RegionsCombo.SetСheckedState("Самарская область", true);

                RegionsCombo1.Width = 510;
                RegionsCombo1.Title = "Субъект РФ для сравнения";
                RegionsCombo1.FillDictionaryValues(CustomMultiComboDataHelper.FillRegions(RegionsNamingHelper.FoNames, RegionsNamingHelper.RegionsFoDictionary));
                RegionsCombo1.ParentSelect = false;
                RegionsCombo1.SetСheckedState("Республика Татарстан (Татарстан)", true);

                ComboYear.Title = "Год";
                ComboYear.Width = 100;
                ComboYear.MultiSelect = false;
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(startYear, endYear));
                ComboYear.SetСheckedState(endYear.ToString(), true);

                ComboMonth.Title = "Месяц";
                ComboMonth.Width = 150;
                ComboMonth.MultiSelect = false;
                ComboMonth.FillDictionaryValues(CustomMultiComboDataHelper.FillMonthValues());
                ComboMonth.SetСheckedState(dtDate.Rows[0][3].ToString(), true);
            }

            UserParams.StateArea.Value = RegionsCombo.SelectedValue;
            UserParams.Region.Value = RegionsNamingHelper.GetFoBySubject(RegionsCombo.SelectedValue);

            StateAreaCompare.Value = RegionsCombo1.SelectedValue;
            RegionCompare.Value = RegionsNamingHelper.GetFoBySubject(RegionsCombo1.SelectedValue);

            reportDate =
                new DateTime(Convert.ToInt32(ComboYear.SelectedValue), CRHelper.MonthNum(ComboMonth.SelectedValue), 1);

            UserParams.PeriodYear.Value = CRHelper.PeriodMemberUName(String.Empty, reportDate, 4);
            UserParams.PeriodLastYear.Value = CRHelper.PeriodMemberUName(String.Empty, reportDate.AddYears(-1), 1);
            UserParams.PeriodEndYear.Value = CRHelper.PeriodMemberUName(String.Empty, reportDate.AddYears(-2), 1);

            headerLayout = new GridHeaderLayout(UltraWebGrid);
            UltraWebGrid.Bands.Clear();
            UltraWebGrid.DataBind();

            Label1.Text = String.Format("Сравнительный анализ бюджетов {0} и {1}", RegionsCombo.SelectedValue, RegionsCombo1.SelectedValue);
            Label2.Text = String.Format("на 1 {0} {1} года, тыс. руб.", CRHelper.RusMonthGenitive(reportDate.Month + 1), reportDate.Year);
        }

        void UltraWebGrid_DataBound(object sender, EventArgs e)
        {
            UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth);
            UltraWebGrid.Height = CRHelper.GetGridWidth(CustomReportConst.minScreenHeight - 260);
        }

        private void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            if (UserParams.StateArea.Value == StateAreaCompare.Value)
            {
                if (!(e.Row.Index == 36 || e.Row.Index == 37))
                {
                    bool direct = e.Row.Index < 36 || e.Row.Index == 42 || e.Row.Index > 56;

                    SetConditionArrow(e, 6, direct);
                    SetConditionArrow(e, 8, direct);
                    SetConditionArrow(e, 10, direct);
                }
            }
            else
            {
                if (!(e.Row.Index == 36 || e.Row.Index == 37))
                {
                    bool direct = e.Row.Index < 36 || e.Row.Index == 42 || e.Row.Index > 56;

                    SetConditionArrow(e, 6, direct);
                    SetConditionArrow(e, 8, direct);
                    SetConditionArrow(e, 10, direct);
                    SetConditionArrow(e, 12, direct);
                    SetConditionArrow(e, 14, direct);
                    SetConditionArrow(e, 16, direct);
                    SetConditionArrow(e, 18, direct);
                    SetConditionArrow(e, 20, direct);
                }
            }
            if (e.Row.Index == 0 || e.Row.Index == 1 ||
                    e.Row.Index == 2 || e.Row.Index == 10 ||
                    e.Row.Index == 11 || e.Row.Index == 17 ||
                    e.Row.Index == 18 || e.Row.Index == 36 ||
                    e.Row.Index == 37 || e.Row.Index == 38 ||
                    e.Row.Index == 47)
            {
                for (int i = 0; i < e.Row.Cells.Count; i++)
                {
                    e.Row.Cells[i].Style.Font.Bold = true;
                }
            }
            else if (e.Row.Index == 49 || e.Row.Index == 51 ||
                         e.Row.Index == 53 || e.Row.Index == 55 ||
                         e.Row.Index == 58 || e.Row.Index == 60)
            {
                for (int i = 0; i < e.Row.Cells.Count; i++)
                {
                    e.Row.Cells[i].Style.Font.Italic = true;
                }
            }
        }

        private void UltraWebGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.AllowSortingDefault = AllowSorting.No;

            if (UserParams.StateArea.Value == StateAreaCompare.Value)
            {
                e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(150, 1920);

                for (int i = 1; i < 3; i++)
                {
                    e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(130, 1920);
                }

                for (int i = 3; i < 11; i += 2)
                {
                    e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(100, 1920);
                    e.Layout.Bands[0].Columns[i + 1].Width = CRHelper.GetColumnWidth(90, 1920);
                }

                e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;

                e.Layout.Bands[0].Columns[1].CellStyle.HorizontalAlign = HorizontalAlign.Center;

                headerLayout.AddCell("Показатели");

                GridHeaderCell cell =
                    headerLayout.AddCell(String.Format("Исполнено за отчетный {0} год", reportDate.Year - 2));

                GridHeaderCell childCell1 = cell.AddCell(RegionsCombo.SelectedValue);

                childCell1.AddCell("Консолидированный бюджет");
                childCell1.AddCell("Собственный бюджет");

                cell = headerLayout.AddCell(String.Format("Исполнено за отчетный {0} год", reportDate.Year - 1));

                GridHeaderCell childCell = cell.AddCell(RegionsCombo.SelectedValue);

                GridHeaderCell childChildCell = childCell.AddCell("Консолидированный бюджет");
                childChildCell.AddCell("Сумма");
                childChildCell.AddCell("Темп роста");

                childChildCell = childCell.AddCell("Собственный бюджет");
                childChildCell.AddCell("Сумма");
                childChildCell.AddCell("Темп роста");

                cell = headerLayout.AddCell(String.Format("Бюджет на текущий {0} год", reportDate.Year));

                childCell = cell.AddCell(RegionsCombo.SelectedValue);

                childChildCell = childCell.AddCell("Консолидированный бюджет");
                childChildCell.AddCell("Сумма");
                childChildCell.AddCell("Темп роста");

                childChildCell = childCell.AddCell("Собственный бюджет");
                childChildCell.AddCell("Сумма");
                childChildCell.AddCell("Темп роста");

                headerLayout.ApplyHeaderInfo();
                
                for (int i = 1; i < 3; i++)
                {
                    CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "N2");
                }

                for (int i = 3; i < e.Layout.Bands[0].Columns.Count; i += 2)
                {
                    CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "N2");
                    CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i + 1], "P2");
                }
            }
            else
            {
                e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(150, 1920);

                for (int i = 1; i < 5; i++)
                {
                    e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(130, 1920);
                }

                for (int i = 5; i < 21; i += 2)
                {
                    e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(100, 1920);
                    e.Layout.Bands[0].Columns[i + 1].Width = CRHelper.GetColumnWidth(90, 1920);
                }

                e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;

                e.Layout.Bands[0].Columns[1].CellStyle.HorizontalAlign = HorizontalAlign.Center;

                headerLayout.AddCell("Показатели");

                GridHeaderCell cell =
                    headerLayout.AddCell(String.Format("Исполнено за отчетный {0} год", reportDate.Year - 2));

                GridHeaderCell childCell1 = cell.AddCell(RegionsCombo.SelectedValue);
                GridHeaderCell childCell2 = cell.AddCell(RegionsCombo1.SelectedValue);

                childCell1.AddCell("Консолидированный бюджет");
                childCell1.AddCell("Собственный бюджет");

                childCell2.AddCell("Консолидированный бюджет");
                childCell2.AddCell("Собственный бюджет");

                cell = headerLayout.AddCell(String.Format("Исполнено за отчетный {0} год", reportDate.Year - 1));

                GridHeaderCell childCell = cell.AddCell(RegionsCombo.SelectedValue);

                GridHeaderCell childChildCell = childCell.AddCell("Консолидированный бюджет");
                childChildCell.AddCell("Сумма");
                childChildCell.AddCell("Темп роста");

                childChildCell = childCell.AddCell("Собственный бюджет");
                childChildCell.AddCell("Сумма");
                childChildCell.AddCell("Темп роста");

                childCell = cell.AddCell(RegionsCombo1.SelectedValue);

                childChildCell = childCell.AddCell("Консолидированный бюджет");
                childChildCell.AddCell("Сумма");
                childChildCell.AddCell("Темп роста");

                childChildCell = childCell.AddCell("Собственный бюджет");
                childChildCell.AddCell("Сумма");
                childChildCell.AddCell("Темп роста");

                cell = headerLayout.AddCell(String.Format("Бюджет на текущий {0} год", reportDate.Year));

                childCell = cell.AddCell(RegionsCombo.SelectedValue);

                childChildCell = childCell.AddCell("Консолидированный бюджет");
                childChildCell.AddCell("Сумма");
                childChildCell.AddCell("Темп роста");

                childChildCell = childCell.AddCell("Собственный бюджет");
                childChildCell.AddCell("Сумма");
                childChildCell.AddCell("Темп роста");

                childCell = cell.AddCell(RegionsCombo1.SelectedValue);

                childChildCell = childCell.AddCell("Консолидированный бюджет");
                childChildCell.AddCell("Сумма");
                childChildCell.AddCell("Темп роста");

                childChildCell = childCell.AddCell("Собственный бюджет");
                childChildCell.AddCell("Сумма");
                childChildCell.AddCell("Темп роста");

                headerLayout.ApplyHeaderInfo();

                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[1], "N2");
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[2], "N2");
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[3], "N2");
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[4], "N2");

                for (int i = 1; i < 5; i++)
                {
                    CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "N2");
                }

                for (int i = 5; i < e.Layout.Bands[0].Columns.Count; i += 2)
                {
                    CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "N2");
                    CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i + 1], "P2");
                }
            }
        }

        void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();

            string query = DataProvider.GetQueryText("FK_0002_0001_incomes");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Показатель", dt);

            DataTable source = new DataTable();

            query = DataProvider.GetQueryText("FK_0002_0001_outcomes");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Показатель", source);
            foreach (DataRow row in source.Rows)
            {
                dt.ImportRow(row);
            }
            source = new DataTable();
            query = DataProvider.GetQueryText("FK_0002_0001_deficite");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Показатель", source);
            foreach (DataRow row in source.Rows)
            {
                dt.ImportRow(row);
            }

            source = new DataTable();
            query = DataProvider.GetQueryText("FK_0002_0001_finSources");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Показатель", source);
            foreach (DataRow row in source.Rows)
            {
                dt.ImportRow(row);
            }

            source = new DataTable();
            query = DataProvider.GetQueryText("FK_0002_0001_information_kosgu");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Показатель", source);
            foreach (DataRow row in source.Rows)
            {
                dt.ImportRow(row);
            }

            source = new DataTable();
            query = DataProvider.GetQueryText("FK_0002_0001_information");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Показатель", source);
            foreach (DataRow row in source.Rows)
            {
                dt.ImportRow(row);
            }

            UltraWebGrid.DataSource = dt;
        }

        private void SetConditionArrow(RowEventArgs e, int index, bool direct)
        {
            if (e.Row.Cells[index] != null &&
                e.Row.Cells[index].Value != null)
            {
                double value = Convert.ToDouble(e.Row.Cells[index].Value.ToString());
                string img;
                string title;
                if (direct)
                {
                    if (value > 1)
                    {
                        img = "~/images/arrowGreenUpBB.png";
                        title = "Рост показателя к аналогичному периоду прошлого года";
                    }
                    else
                    {
                        img = "~/images/arrowRedDownBB.png";
                        title = "Снижение показателя к аналогичному периоду прошлого года";
                    }
                }
                else
                {
                    if (value > 1)
                    {
                        img = "~/images/arrowRedUpBB.png";
                        title = "Рост показателя к аналогичному периоду прошлого года";
                    }
                    else
                    {
                        img = "~/images/arrowGreenDownBB.png";
                        title = "Снижение показателя к аналогичному периоду прошлого года";
                    }
                }
                e.Row.Cells[index].Style.BackgroundImage = img;
                e.Row.Cells[index].Style.CustomRules = "background-repeat: no-repeat; background-position: 10px center; padding-left: 0px";
                e.Row.Cells[index].Title = title;
            }
        }

        //void PdfExportButton_Click(object sender, EventArgs e)
        //{
        //    ReportPDFExporter1.PageTitle = Label1.Text;
        //    ReportPDFExporter1.PageSubTitle = Label2.Text;

        //    ReportPDFExporter1.Export(headerLayout);
        //}

        void ExcelExportButton_Click(object sender, EventArgs e)
        {
            ReportExcelExporter1.WorksheetTitle = Label1.Text;
            ReportExcelExporter1.WorksheetSubTitle = Label2.Text;
            ReportExcelExporter1.Export(headerLayout, 4);
        }
    }
}
