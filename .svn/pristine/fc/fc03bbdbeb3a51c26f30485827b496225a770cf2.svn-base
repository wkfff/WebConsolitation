using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Infragistics.WebUI.UltraWebGrid;
using System.Drawing;
using Microsoft.AnalysisServices.AdomdClient;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Common;
using System.Collections.ObjectModel;
using System.Text;
using System.Collections.Generic;
using Infragistics.WebUI.UltraWebChart;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.Documents.Excel;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.Documents.Reports.Report.Table;
using Infragistics.Documents.Reports.Report;
using System.Xml;
using Infragistics.Documents.Reports.Report.Section;
namespace Krista.FM.Server.Dashboards.reports.ST_YANAO
{
    public partial class _default : CustomReportPage
    {
        private string page_title = "Отчет о ходе строительства объектов адресной инвестиционной программы Ямало-Ненецкого автономного округа";
        private string page_sub_title = "Мониторинг строительства объектов адресной инвестиционной программы Ямало-Ненецкого автономного округа по состоянию на {0}";
        private CustomParam baseRegion { get { return (UserParams.CustomParam("baseRegion")); } }
        private CustomParam selectedPeriod { get { return (UserParams.CustomParam("selectedPeriod")); } }
        private CustomParam dataSource { get { return (UserParams.CustomParam("dataSource")); } }
        private CustomParam digitYear { get { return (UserParams.CustomParam("digitYear")); } }
        private CustomParam nameYear { get { return (UserParams.CustomParam("nameYear")); } }
        private CustomParam Filter2_cp { get { return (UserParams.CustomParam("Filter2_cp")); } }
        private GridHeaderLayout headerLayout;
        private bool IsSmallResolution
        {
            get { return CRHelper.GetScreenWidth < 1200; }
        }
        private int minScreenWidth;
        //{
        //    get { return  }
        //    set { minScreenWidth = value; }
        //}

        private int minScreenHeight
        {
            get { return IsSmallResolution ? 700 : CustomReportConst.minScreenHeight; }
        }
        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);
            minScreenWidth  = IsSmallResolution ? 750 : CustomReportConst.minScreenWidth;

            Year.Width = 250;
            G.Width = IsSmallResolution ? 740 : CustomReportConst.minScreenWidth; ;
            G.Height = 
                CRHelper.GetGridHeight(CustomReportConst.minScreenHeight * 0.62); ;
            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportExcelExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);

            ReportExcelExporter1.ExcelExporter.EndExport += new EndExportEventHandler(ExcelExporter_EndExport);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);

            ReportPDFExporter1.PdfExportButton.Visible = false;


            GridSearch1.LinkedGridId = G.ClientID;
            
        }
        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            baseRegion.Value = RegionSettingsHelper.Instance.RegionBaseDimension;
            if (!Page.IsPostBack)
            {
                Year.FillDictionaryValues(YearsLoad("Years"));
                Year.Title = "Период";
                Year.SelectLastNode();

                filter2.ParentSelect = true;
                filter2.FillDictionaryValues(LoadFilter2("Filter2"));
                filter2.SetСheckedState("Все заказчики", true);

                filter2.Title = "Заказчик";
                filter2.Width = 400;
            }
            Hederglobal.Text = page_title;
            Page.Title = page_title;
            PageSubTitle.Text = string.Format(page_sub_title, Year.SelectedValue.ToLower()); 
            int monthNum = CRHelper.MonthNum(Year.SelectedValue.Split(' ')[0]);
            selectedPeriod.Value = "[Период__Период].[Период__Период].[Данные всех периодов].[" + Year.SelectedNode.Text.Split(' ')[1] + "].[Полугодие " + CRHelper.HalfYearNumByMonthNum(monthNum) + "].[Квартал " + CRHelper.QuarterNumByMonthNum(monthNum) + "].[" + Year.SelectedValue.Split(' ')[0] + "]";
            string monthNumStr = "";
            if (monthNum < 10)
            {
                monthNumStr = "0" + monthNum.ToString();
            }
            else
            {
                monthNumStr = monthNum.ToString();
            }
            dataSource.Value = monthNumStr + "." + Year.SelectedNode.Text.Split(' ')[1];
            nameYear.Value = Year.SelectedNode.Text.Split(' ')[1];
            digitYear.Value = "01.01." + Year.SelectedNode.Text.Split(' ')[1].Remove(0, 2);
            headerLayout = new GridHeaderLayout(G);
            if (filter2.SelectedValue.Contains("Все заказчики"))
            {
                Filter2_cp.Value = "[Строительство__Заказчики].[Строительство__Заказчики].[Заказчик].members";
            }
            else
            {
                Filter2_cp.Value = string.Format("[Строительство__Заказчики].[Строительство__Заказчики].[Заказчик].[{0}]", filter2.SelectedValue);
            }
            //Filter2_cp.Value = filter2.SelectedValue;
            G.DataBind();
            ///G.Rows.Remove(G.Rows[0]);
            G.Rows[0].Hidden = !filter2.SelectedValue.Contains("Все заказчики");

            RemoveHiidenRow(G);

        }

        private void RemoveHiidenRow(UltraWebGrid G)
        {
            List<UltraGridRow> RevedRow = new List<UltraGridRow>();
            foreach (UltraGridRow Row in G.Rows)
            {
                if (Row.Hidden)
                    RevedRow.Add(Row);
            }
            foreach (UltraGridRow Row in RevedRow)
            {
                G.Rows.Remove(Row);
            }
        }

        Dictionary<string, int> YearsLoad(string sql)
        {
            DataTable dt = new DataTable();
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText(sql), "years", dt);

            Dictionary<string, int> d = new Dictionary<string, int>();
            d.Add(dt.Rows[0][2].ToString() + " год", 0);
            d.Add(dt.Rows[0][0].ToString() + " " + dt.Rows[0][2].ToString() + " года", 1);
            for (int i = 1; i < dt.Rows.Count; i++)
            {
                if (dt.Rows[i][2].ToString() != dt.Rows[i - 1][2].ToString())
                {
                    d.Add(dt.Rows[i][2].ToString() + " год", 0);
                }
                d.Add(dt.Rows[i][0].ToString() + " " + dt.Rows[i][2].ToString() + " года", 1);
            }
            return d;
        }

        Dictionary<string, int> LoadFilter2(string sqlid)
        {
            DataTable dt = new DataTable();
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText(sqlid), "field", dt);
            Dictionary<string, int> d = new Dictionary<string, int>();
            d.Add("Все заказчики", 0);
            foreach (DataRow row in dt.Rows)
            {
                d.Add(row["field"].ToString(), 1);
            }
            return d;
        }

        protected void G_DataBinding(object sender, EventArgs e)
        {
            G.Bands.Clear();

            DataTable dt = new DataTable();
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("Grid"), "Наименование государственных заказчиков Адресной инвестиционной программы ЯНАО, государственных (муниципальных) заказчиков объектов программы, заказчиков Адресной программы, объектов", dt);
            double sum = 0;
            //for (int j = 1; j < dt.Rows.Count; j++)
            //String.Format(dt.Rows[i][0].ToString());
            for (int i = 13; i < dt.Columns.Count; i++)
            {
                sum = 0;
                for (int j = 1; j < dt.Rows.Count; j++)
                {

                    if (dt.Rows[j][0].ToString().Contains("Все контракты"))
                        if (MathHelper.IsDouble(dt.Rows[j][i]))
                        {
                            sum += Convert.ToDouble(dt.Rows[j][i]);
                        }
                }
                if (sum == 0)
                {
                    dt.Rows[0][i] = DBNull.Value;
                }
                else
                {
                    dt.Rows[0][i] = sum;
                }
                 //== 0 ? DBNull.Value : sum;
            } 
            bool emptyRow = true;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                dt.Rows[i][1] = dt.Rows[i][0];

                emptyRow = true;
                if (dt.Rows[i][1].ToString().Split(';')[1] != " Все контракты")
                {
                    for (int j = 2; j < dt.Columns.Count; j++)
                    {
                        if (dt.Rows[i][j] != String.Empty)
                        {
                            emptyRow = false;
                        }
                    }
                    if (emptyRow)
                    {
                        // dt.Rows.Remove(dt.Rows[i]);
                        //  i -= 1;
                    }
                }

            }
            int col = 0;
            dt.Rows[0][0] = DBNull.Value;
            for (int i = 1; i < dt.Rows.Count; i++)
            {
                if (dt.Rows[i][1].ToString().Split(';')[1] != " Все контракты")
                {
                    dt.Rows[i][0] = col + 1;
                    col += 1;
                }
                else
                {
                    dt.Rows[i][0] = DBNull.Value;
                    col = 0;
                }

            }
            //G.Rows[0].Delete();

            if (!detail.Checked)
                for (int i = 2; i < 17; i++)
                {
                    dt.Columns.RemoveAt(2);
                }

            G.DataSource = dt;

        }

        protected void G_InitializeRow(object sender, RowEventArgs e)
        {

            if (e.Row.Cells[1].Text.Split(';')[1] == " Все контракты" || e.Row.Cells[1].Text.Split(';')[1] == " пустой")
            {
                e.Row.Cells[1].Text = e.Row.Cells[1].Text.Split(';')[0];
                for (int i = 1; i < e.Row.Cells.Count; i++)
                {
                    e.Row.Cells[i].Style.Font.Bold = true;
                }
                if (detail.Checked)
                {
                    e.Row.Cells[13].Text = "";
                    e.Row.Cells[14].Text = "";

                    e.Row.Cells[15].Text = "";
                    e.Row.Cells[16].Text = "";
                }
            }
            else
            {
                e.Row.Cells[1].Text = e.Row.Cells[1].Text.Split(';')[1];
                if (MathHelper.IsDouble(e.Row.Cells[13].Value))
                {
                    e.Row.Cells[13].Text = String.Format("{0:0}", Convert.ToDouble(e.Row.Cells[13].Value));
                }
                if (MathHelper.IsDouble(e.Row.Cells[14].Value))
                {
                    e.Row.Cells[14].Text = String.Format("{0:0}", Convert.ToDouble(e.Row.Cells[14].Value));
                }
                e.Row.Hidden = !detail.Checked;
            } 
        }  

        string GetDisplayHeader(string s)
        {
            string[] SplitHedaer = s.Split('-');
            return SplitHedaer[SplitHedaer.Length - 1].Replace("01.01.11", digitYear.Value);
        }


        protected void G_InitializeLayout(object sender, LayoutEventArgs e)
        {
            double colWidth = 0;
            if (IsSmallResolution)
            {
                colWidth = 0.05;
            }
            else
            {
                colWidth = 0.1;
            }

            minScreenWidth = 1280;

            headerLayout.childCells.Clear();
            e.Layout.AllowSortingDefault = AllowSorting.No;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.HeaderStyleDefault.VerticalAlign = VerticalAlign.Middle;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.Bands[0].Columns[1].Width = CRHelper.GetColumnWidth(minScreenWidth * 0.25);
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(minScreenWidth * 0.03);
            e.Layout.Bands[0].Columns[1].Header.Style.Wrap = true;
            e.Layout.Bands[0].Columns[0].Header.Style.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[1].CellStyle.Wrap = true;

            for (int i = 2; i < e.Layout.Bands[0].Columns.Count - 1; i++)
            {
                e.Layout.Bands[0].Columns[i].Header.Style.Wrap = true;
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(minScreenWidth * colWidth);
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
                e.Layout.Bands[0].Columns[i].CellStyle.CustomRules = "padding-right:5px";
                e.Layout.Bands[0].Columns[i].CellStyle.Wrap = true;
                //CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "N2");
            }
            e.Layout.Bands[0].Columns[e.Layout.Bands[0].Columns.Count - 1].Width = CRHelper.GetColumnWidth(minScreenWidth * 0.2);
            e.Layout.Bands[0].Columns[e.Layout.Bands[0].Columns.Count - 1].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            e.Layout.Bands[0].Columns[e.Layout.Bands[0].Columns.Count - 1].CellStyle.CustomRules = "padding-right:5px";
            e.Layout.Bands[0].Columns[e.Layout.Bands[0].Columns.Count - 1].CellStyle.Wrap = true;

            for (int i = 15; i < e.Layout.Bands[0].Columns.Count - 1; i++)
            {
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "N2");
            }
            headerLayout.AddCell("№ п/п");
            headerLayout.AddCell("Наименование государственных заказчиков Адресной инвестиционной программы ЯНАО, государственных (муниципальных) заказчиков объектов программы, заказчиков Адресной программы, объектов");
            int shift = 0;

            GridHeaderCell headerCell = null;
            if (detail.Checked)
            {
                for (int i = 2; i < 13; i++)
                {
                    headerLayout.AddCell(e.Layout.Bands[0].Columns[i].Key);
                }

                headerCell = headerLayout.AddCell("Сроки строительства");
                headerCell.AddCell(GetDisplayHeader(e.Layout.Bands[0].Columns[13 + shift].Key));
                headerCell.AddCell(GetDisplayHeader(e.Layout.Bands[0].Columns[14 + shift].Key));

                headerCell = headerLayout.AddCell("Сметная стоимость");
                headerCell.AddCell(GetDisplayHeader(e.Layout.Bands[0].Columns[15 + shift].Key));
                headerCell.AddCell(GetDisplayHeader(e.Layout.Bands[0].Columns[16 + shift].Key));
            }
            else
            {
                shift = -15;
            }

            headerCell = headerLayout.AddCell("Объем незавершенного строительства в текущих ценах  на начало текущего года");
            headerCell.AddCell("всего");
            GridHeaderCell headerCell1 = headerCell.AddCell("в том числе за счёт средств");
            headerCell1.AddCell(GetDisplayHeader(e.Layout.Bands[0].Columns[18 + shift].Key));
            headerCell1.AddCell(GetDisplayHeader(e.Layout.Bands[0].Columns[19 + shift].Key));
            headerCell1.AddCell(GetDisplayHeader(e.Layout.Bands[0].Columns[20 + shift].Key));

            headerCell = headerLayout.AddCell("Объем финансирования с начала строительства на начало текущего года");
            headerCell.AddCell("всего");
            headerCell1 = headerCell.AddCell("в том числе за счёт средств");
            headerCell1.AddCell(GetDisplayHeader(e.Layout.Bands[0].Columns[22 + shift].Key));
            headerCell1.AddCell(GetDisplayHeader(e.Layout.Bands[0].Columns[23 + shift].Key));
            headerCell1.AddCell(GetDisplayHeader(e.Layout.Bands[0].Columns[24 + shift].Key));

            headerLayout.AddCell(GetDisplayHeader(e.Layout.Bands[0].Columns[25 + shift].Key));

            headerCell = headerLayout.AddCell("Объем капитальных вложений  на текущий год");
            headerCell.AddCell(GetDisplayHeader(e.Layout.Bands[0].Columns[26 + shift].Key));
            GridHeaderCell hc = headerCell.AddCell("в том числе");
            hc.AddCell("средства окружного бюджета (пост.от 18.08.2011 г. № 712-П)");
            hc.AddCell("средства ОАО «Гапром» расп. № 514-РП от 07.09.2011 г.");
            hc.AddCell("средства ОАО «Газпром» согласно компенсационному соглашению 16-РП от 27.01.2011г.");
            shift += 3;
            headerCell.AddCell(GetDisplayHeader(e.Layout.Bands[0].Columns[27 + shift].Key));
            headerCell.AddCell(GetDisplayHeader(e.Layout.Bands[0].Columns[28 + shift].Key));


            headerCell = headerLayout.AddCell("Освоено капитальных вложений в текущих ценах за отчетный период текущего года");
            headerCell.AddCell("всего");
            headerCell1 = headerCell.AddCell("в том числе за счёт средств");
            headerCell1.AddCell(GetDisplayHeader(e.Layout.Bands[0].Columns[30 + shift].Key));
            hc = headerCell1.AddCell("в том числе");
            hc.AddCell(GetDisplayHeader(e.Layout.Bands[0].Columns[31 + shift].Key));
            hc.AddCell(GetDisplayHeader(e.Layout.Bands[0].Columns[32 + shift].Key));
            shift += 2;
            headerCell1.AddCell(GetDisplayHeader(e.Layout.Bands[0].Columns[31 + shift].Key));
            headerCell1.AddCell(GetDisplayHeader(e.Layout.Bands[0].Columns[32 + shift].Key));

            headerCell = headerLayout.AddCell("Финансирование за отчётный период текущего года");
            headerCell.AddCell("всего");
            headerCell1 = headerCell.AddCell("в том числе за счёт средств");
            headerCell1.AddCell(GetDisplayHeader(e.Layout.Bands[0].Columns[34 + shift].Key));
            hc = headerCell1.AddCell("в том числе");
            hc.AddCell(GetDisplayHeader(e.Layout.Bands[0].Columns[35 + shift].Key));
            hc.AddCell(GetDisplayHeader(e.Layout.Bands[0].Columns[36 + shift].Key));
            shift += 2;
            headerCell1.AddCell(GetDisplayHeader(e.Layout.Bands[0].Columns[35 + shift].Key));
            headerCell1.AddCell(GetDisplayHeader(e.Layout.Bands[0].Columns[36 + shift].Key));

            headerCell = headerLayout.AddCell("Ввод объектов в эксплуатацию");
            headerCell1 = headerCell.AddCell("мощность");
            headerCell1.AddCell(GetDisplayHeader(e.Layout.Bands[0].Columns[37 + shift].Key));
            headerCell1.AddCell(GetDisplayHeader(e.Layout.Bands[0].Columns[38 + shift].Key));
            headerCell1.AddCell(GetDisplayHeader(e.Layout.Bands[0].Columns[39 + shift].Key));
            headerCell.AddCell(GetDisplayHeader(e.Layout.Bands[0].Columns[40 + shift].Key));

            headerLayout.AddCell(GetDisplayHeader(e.Layout.Bands[0].Columns[41 + shift].Key));
            headerLayout.ApplyHeaderInfo();
        }

        #region Экспорт в Excel
        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            ReportExcelExporter1.WorksheetTitle = Hederglobal.Text;
            ReportExcelExporter1.WorksheetSubTitle = PageSubTitle.Text;
            Infragistics.Documents.Excel.Workbook workbook = new Infragistics.Documents.Excel.Workbook();
            Infragistics.Documents.Excel.Worksheet sheet1 = workbook.Worksheets.Add("Таблица");
            ReportExcelExporter1.HeaderCellFont = new System.Drawing.Font("Verdana", 11);
            ReportExcelExporter1.TitleFont = new System.Drawing.Font("Verdana", 11, FontStyle.Bold);
            ReportExcelExporter1.SubTitleFont = new System.Drawing.Font("Verdana", 9);
            ReportExcelExporter1.TitleAlignment = HorizontalCellAlignment.Left;

            ReportExcelExporter1.TitleStartRow = 0;

            foreach (UltraGridColumn col in headerLayout.Grid.Columns)
            {
                col.Width = (int)(col.Width.Value * 2);
            }

            

            ReportExcelExporter1.Export(headerLayout, sheet1, 3);
            
            
        }

        private static void ExcelExporter_BeginExport(object sender, BeginExportEventArgs e)
        {
            e.CurrentWorksheet.PrintOptions.Orientation = Infragistics.Documents.Excel.Orientation.Landscape;
            e.CurrentWorksheet.PrintOptions.PaperSize = PaperSize.A4;
            e.CurrentWorksheet.PrintOptions.BottomMargin = 0.25;
            e.CurrentWorksheet.PrintOptions.TopMargin = 0.25;
            e.CurrentWorksheet.PrintOptions.LeftMargin = 0.25;
            e.CurrentWorksheet.PrintOptions.RightMargin = 0.25;
            e.CurrentWorksheet.PrintOptions.ScalingType = ScalingType.FitToPages;
        }
        private void ExcelExporter_EndExport(object sender, EndExportEventArgs e)
        {
            e.CurrentWorksheet.Rows[3].Height = 24 * 24;
            e.CurrentWorksheet.Rows[4].Height = 24 * 24;
            e.CurrentWorksheet.Rows[5].Height = 24 * 24;
            e.CurrentWorksheet.Rows[6].Height = 24 * 40; 

        }
        #endregion

        #region Экспорт в PDF


        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            ReportPDFExporter1.PageTitle = Hederglobal.Text;
            ReportPDFExporter1.PageSubTitle = PageSubTitle.Text;
            ReportPDFExporter1.HeaderCellHeight = 30;
            Infragistics.Documents.Reports.Report.Report report = new Infragistics.Documents.Reports.Report.Report();

            Infragistics.Documents.Reports.Report.Section.ISection section1 = report.AddSection();
            ReportPDFExporter1.HeaderCellHeight = 60;
            ReportPDFExporter1.Export(headerLayout, section1);
        }
        #endregion
    }
}