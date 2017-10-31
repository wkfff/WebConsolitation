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
namespace Krista.FM.Server.Dashboards.reports.ST_HMAO
{
    public partial class _default : CustomReportPage
    {
        private string page_title = "Отчет о ходе строительства объектов, предусмотренных Адресной инвестиционной программой ХМАО – Югры";
        private string page_sub_title = "Данные ежеквартального мониторинга строительства объектов, предусмотренных Адресной инвестиционной программой ХМАО – Югры, в тыс.руб.";
        private CustomParam baseRegion { get { return (UserParams.CustomParam("baseRegion")); } }
        private CustomParam selectedPeriod { get { return (UserParams.CustomParam("selectedPeriod")); } }
        private CustomParam selectedRegion { get { return (UserParams.CustomParam("selectedRegion")); } }
        private CustomParam selectedCustomer { get { return (UserParams.CustomParam("selectedCustomer")); } }
        private CustomParam selectedProgram { get { return (UserParams.CustomParam("selectedProgram")); } }
        private GridHeaderLayout headerLayout;
        private bool IsSmallResolution
        {
            get { return CRHelper.GetScreenWidth < 1200; }
        }
        private int minScreenWidth
        {
            get { return IsSmallResolution ? 750 : CustomReportConst.minScreenWidth; }
        }

        private int minScreenHeight
        {
            get { return IsSmallResolution ? 700 : CustomReportConst.minScreenHeight; }
        }

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);
            Year.Width = 220;
            ComboProgramm.Width = 300;
            ComboCustomer.Width = 270;
            ComboRegion.Width = 320;
            G.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 15 - 5);
            G.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight * 0.67); ;
            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportExcelExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);

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

                ComboProgramm.Title = "Программа";
                ComboProgramm.FillDictionaryValues(ProgramLoad("Programs"));
                ComboProgramm.ParentSelect = true;
                 
                

                ComboRegion.Title = "МО";
                ComboRegion.FillDictionaryValues(RegionLoad("Regions"));
                
            }
            ComboCustomer.Title = "Заказчик";
            ComboCustomer.FillDictionaryValues(CustomerLoad("Customers"));
            ComboCustomer.ParentSelect = true;
            if (Year.SelectedValue.EndsWith("1") || Year.SelectedValue.EndsWith("2"))
            {
                selectedPeriod.Value = "[Период__Период].[Период__Период].[Данные всех периодов].["+Year.SelectedValue.Split(' ')[2]+"].[Полугодие 1].[Квартал "+Year.SelectedValue.Split(' ')[1]+"]";
            }
            else
            {
                selectedPeriod.Value = "[Период__Период].[Период__Период].[Данные всех периодов].[" + Year.SelectedValue.Split(' ')[2] + "].[Полугодие 2].[Квартал " + Year.SelectedValue.Split(' ')[1] + "]";
            }

            
            selectedRegion.Value = "[Территории__РФ].[Территории__РФ].[Все территории].[Российская Федерация].[Уральский федеральный округ].[Тюменская область с Ханты-Мансийским автономным округом, Ямало-Ненецким автономным округом].[Ханты-Мансийский автономный округ - Югра].["+ComboRegion.SelectedValue+"]";
            

            if (ComboProgramm.SelectedNode.Level == 0)
            {

                if (ComboProgramm.SelectedNode.Nodes.Count > 0)
                {
                    selectedProgram.Value =  " " + "[Исполнение расходов__Реестр программ].[Исполнение расходов__Реестр программ].[" + ComboProgramm.SelectedValue + "].children";
                }
                else
                {
                    selectedProgram.Value = "[Исполнение расходов__Реестр программ].[Исполнение расходов__Реестр программ].[" + ComboProgramm.SelectedValue + "].datamember";
                }
            }
            else
            {
                selectedProgram.Value = "[Исполнение расходов__Реестр программ].[Исполнение расходов__Реестр программ].[" + ComboProgramm.SelectedNode.Parent.Text + "].["+ComboProgramm.SelectedValue+"]";
            }

            if (ComboCustomer.SelectedNode.Nodes.Count > 0)
            {
                selectedCustomer.Value = CutomerUname[ComboCustomer.SelectedValue]+".children";
            }
            else
            {
                selectedCustomer.Value = CutomerUname[ComboCustomer.SelectedValue];
            }

                //if (ComboCustomer.SelectedNode.Level==0)
                //{
                //    selectedCustomer.Value = "[Исполнение расходов__Заказчики объектов].[Исполнение расходов__Заказчики объектов].[" + ComboCustomer.SelectedValue+"].datamember";
                //    if (ComboCustomer.SelectedNode.Nodes.Count > 0)
                //    {
                //        selectedCustomer.Value += ", " + selectedCustomer.Value + ".children";
                //    }
                //}
                //else
                //{
                //    if (ComboCustomer.SelectedNode.Level == 1)
                //    {
                //        selectedCustomer.Value = "[Исполнение расходов__Заказчики объектов].[Исполнение расходов__Заказчики объектов].[" + ComboCustomer.SelectedNode.Parent.Text + "].[" + ComboCustomer.SelectedValue + "].datamember";
                //        if (ComboCustomer.SelectedNode.Nodes.Count > 0)
                //        {
                //            selectedCustomer.Value += ", " + "[Исполнение расходов__Заказчики объектов].[Исполнение расходов__Заказчики объектов].[" + ComboCustomer.SelectedNode.Parent.Text + "].[" + ComboCustomer.SelectedValue + "].children";
                //        }
                //    }
                //    else
                //    {
                //        selectedCustomer.Value = "[Исполнение расходов__Заказчики объектов].[Исполнение расходов__Заказчики объектов].[" + ComboCustomer.SelectedNode.Parent.Text + "].[" + ComboCustomer.SelectedNode.Parent.Parent.Text + "].["+ComboCustomer.SelectedValue+"]"; 
                //    }
                //}
                headerLayout = new GridHeaderLayout(G);
                G.DataBind();
                Hederglobal.Text = page_title;
                PageSubTitle.Text = page_sub_title;
                Page.Title = page_title;
                G.DisplayLayout.NoDataMessage = "Нет данных";
                try
                { }
            catch { }   
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

        Dictionary<string, int> ProgramLoad(string sql)
        {
            DataTable dt = new DataTable();
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText(sql), "programs", dt);

            Dictionary<string, int> d = new Dictionary<string, int>();
            for (int i = 1; i < dt.Rows.Count; i++)
            {
                if (dt.Rows[i][2].ToString() == "1")
                {
                    d.Add(dt.Rows[i][0].ToString(), 0);
                }
                else
                {
                    d.Add(dt.Rows[i][0].ToString(), 1);
                }
            }
            return d;
        }
        Dictionary<string, string> CutomerUname = new Dictionary<string, string>();
        Dictionary<string, int> CustomerLoad(string sql)
        {
            DataTable dt = new DataTable();
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText(sql), "programs", dt);

            Dictionary<string, int> d = new Dictionary<string, int>();

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                DataRow row = dt.Rows[i];
                if (dt.Rows[i][2].ToString() == "1")
                {
                    AddItemToDictCust(d, row, 0);
                }
                else
                {
                    if (dt.Rows[i][2].ToString() == "2")
                    {
                        //d.Add(row[0].ToString(), 1);
                        AddItemToDictCust(d, row, 1);
                    }
                    else
                    {
                        //d.Add(row[0].ToString(), 2);
                        AddItemToDictCust(d, row, 2);
                    }
                }
            }
            return d;
        }

        private void AddItemToDictCust(Dictionary<string, int> d, DataRow row, int level)
        {
            if (!Page.IsPostBack)
            {
                d.Add(row[0].ToString(), level);
            }
            if (level != 2)
                CutomerUname.Add(row[0].ToString(), row["uname"].ToString());// + ".children");
            else
                CutomerUname.Add(row[0].ToString(), row["uname"].ToString());//+ ".children");
        }

        Dictionary<string, int> RegionLoad(string sql)
        {
            DataTable dt = new DataTable();
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText(sql), "regions", dt);

            Dictionary<string, int> d = new Dictionary<string, int>();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                d.Add(dt.Rows[i][0].ToString(), 0);
            }
            return d;
        }

        protected void G_DataBinding(object sender, EventArgs e)
        {
            G.Columns.Clear();
            G.Bands.Clear();
            DataTable dt = new DataTable();
            DataTable resDt = new DataTable();
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("Grid"), "Программа", dt);
            bool emptyRow = true;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                emptyRow = true;

                for (int j = 15; j < dt.Columns.Count; j++)
                {
                    if (dt.Rows[i][j] != DBNull.Value)
                    {
                        emptyRow = false;
                    }
                }
                if (emptyRow)
                {
                    dt.Rows.Remove(dt.Rows[i]);
                    i -= 1;
                }
            }
            if (dt.Rows.Count > 1)
            {
                
                resDt.Columns.Add(dt.Columns[0].ColumnName, dt.Columns[0].DataType);
                resDt.Columns.Add("Территория", dt.Columns[0].DataType);
                resDt.Columns.Add("Заказчик", dt.Columns[0].DataType);
                resDt.Columns.Add("Объект", dt.Columns[0].DataType);
                for (int i = 1; i < dt.Columns.Count; i++)
                {
                    resDt.Columns.Add(dt.Columns[i].ColumnName, dt.Columns[i].DataType);
                }
                object[] o = new object[resDt.Columns.Count];
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    o[3] = dt.Rows[i][0].ToString().Split(';')[3];
                    o[2] = dt.Rows[i][0].ToString().Split(';')[2];
                    o[1] = dt.Rows[i][0].ToString().Split(';')[1];
                    if (dt.Rows[i][0].ToString().Split(';')[0].EndsWith("ДАННЫЕ)"))
                    {
                        o[0] = dt.Rows[i][0].ToString().Split(';')[0].Replace("ДАННЫЕ)", string.Empty).Remove(0, 1);
                    }
                    else
                    {
                        o[0] = dt.Rows[i][0].ToString().Split(';')[0];
                    }
                    for (int j = 1; j < dt.Columns.Count; j++)
                    { 
                        o[j + 3] = dt.Rows[i][j];
                    }
                    o[6] = o[6].ToString().Split(' ')[0];
                    o[7] = o[7].ToString().Split(' ')[0];
                    if (o[6].ToString() == "0:00:00")
                    {
                        o[6] = DBNull.Value;
                    }
                    if (o[7].ToString() == "0:00:00")
                    {
                        o[7] = DBNull.Value;
                    }
                    resDt.Rows.Add(o);
                }
                for (int i = 0; i < resDt.Rows.Count-1; i++) 
                {

                    if (resDt.Rows[i+1][3].ToString().EndsWith("ДАННЫЕ)"))
                    {
                        resDt.Rows[i + 1][3] = resDt.Rows[i + 1][3].ToString().Replace("ДАННЫЕ)", String.Empty);
                        resDt.Rows[i + 1][3] = resDt.Rows[i + 1][3].ToString().Remove(0, 2);
                        resDt.Rows.Remove(resDt.Rows[i]);
                        i -= 1;
                    }
                }
                G.DataSource = resDt;
            }
            else
            {
                G.DataSource = null;
            }
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
            headerLayout.childCells.Clear();
            e.Layout.AllowSortingDefault = AllowSorting.No;
            e.Layout.RowSelectorsDefault = RowSelectors.Yes;
            e.Layout.AllowColSizingDefault = AllowSizing.Free;
            e.Layout.CellClickActionDefault = CellClickAction.RowSelect;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.HeaderStyleDefault.VerticalAlign = VerticalAlign.Middle;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(minScreenWidth * 0.12);
            e.Layout.Bands[0].Columns[1].Width = CRHelper.GetColumnWidth(minScreenWidth * 0.07);
            e.Layout.Bands[0].Columns[2].Width = CRHelper.GetColumnWidth(minScreenWidth * 0.15);
            e.Layout.Bands[0].Columns[3].Width = CRHelper.GetColumnWidth(minScreenWidth * 0.15);
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[1].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[2].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[3].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[0].Header.Style.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            headerLayout.AddCell("Программа");
            headerLayout.AddCell("Территория");
            headerLayout.AddCell("Заказчик");
            headerLayout.AddCell("Объект");
            e.Layout.Bands[0].Columns[0].MergeCells = true;
            e.Layout.Bands[0].Columns[1].MergeCells = true; 
            e.Layout.Bands[0].Columns[2].MergeCells = true;
            e.Layout.Bands[0].Columns[3].MergeCells = true;
            for (int i = 4; i < e.Layout.Bands[0].Columns.Count; i++)
            { 
                e.Layout.Bands[0].Columns[i].Header.Style.Wrap = true;
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(minScreenWidth * colWidth);
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
                e.Layout.Bands[0].Columns[i].CellStyle.CustomRules = "padding-right:5px";
                e.Layout.Bands[0].Columns[i].CellStyle.Wrap = true;
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "N2");
                headerLayout.AddCell(e.Layout.Bands[0].Columns[i].Key);
            }

            headerLayout.ApplyHeaderInfo();
            
        }

        protected void G_InitializeRow(object sender, RowEventArgs e)
        {
         /*   e.Row.Cells[3].Text = e.Row.Cells[0].Text.Split(';')[3];
            e.Row.Cells[2].Text = e.Row.Cells[0].Text.Split(';')[2];
            e.Row.Cells[1].Text = e.Row.Cells[0].Text.Split(';')[1];
            e.Row.Cells[0].Text = e.Row.Cells[0].Text.Split(';')[0];*/
            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
                e.Row.Cells[i].Style.BackColor = Color.White;
            }
            foreach (UltraGridCell cell in e.Row.Cells)
            {
                if (!string.IsNullOrEmpty(cell.Text))
                {

                    if (cell.Text.Contains("ДАННЫЕ)"))
                    {
                        cell.Text = cell.Text.Replace("ДАННЫЕ)", "").Replace("(", "");
                    }
                    if ((cell.Text[0] == ' ')&&(cell.Column.Header.Caption.Contains("Объект")))
                    {
                        cell.Style.Padding.Left = 15;
                    }
                }
                
            }
            
        }

        #region Экспорт в Excel
        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            ReportExcelExporter1.WorksheetTitle = Hederglobal.Text;
            ReportExcelExporter1.WorksheetSubTitle = PageSubTitle.Text;
            Infragistics.Documents.Excel.Workbook workbook = new Infragistics.Documents.Excel.Workbook();
            Infragistics.Documents.Excel.Worksheet sheet1 = workbook.Worksheets.Add("Таблица");
            ReportExcelExporter1.HeaderCellFont = new System.Drawing.Font("Verdana", 9);
            ReportExcelExporter1.TitleFont = new System.Drawing.Font("Verdana", 11, FontStyle.Bold);
            ReportExcelExporter1.SubTitleFont = new System.Drawing.Font("Verdana", 9);
            ReportExcelExporter1.TitleAlignment = HorizontalCellAlignment.Center;

            ReportExcelExporter1.TitleStartRow = 3;
            ReportExcelExporter1.Export(headerLayout, sheet1, 6);
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

        }
        #endregion
    }
}