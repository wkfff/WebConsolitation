using System;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls.WebParts;
using Infragistics.WebUI.Misc;
using Microsoft.AnalysisServices.AdomdClient;
using Infragistics.WebUI.UltraWebChart;
using Infragistics.UltraChart.Core.Primitives; 
using System.Collections.ObjectModel;
using System.Text;
using System.Configuration;
using System.Collections.Generic;
using Infragistics.UltraChart.Core;
using Dundas.Maps.WebControl;
using Infragistics.UltraChart.Shared.Styles;
using System.Net;
using System.IO;
using Color = System.Drawing.Color;
using EndExportEventArgs = Infragistics.WebUI.UltraWebGrid.ExcelExport.EndExportEventArgs;
using Font = Infragistics.Documents.Reports.Graphics.Font;
using Image = Infragistics.Documents.Reports.Graphics.Image;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Table;
using Infragistics.Documents.Reports.Report.Text;
using System.Web.Security;
using System.Web.UI.HtmlControls;
using Graphics = System.Drawing.Graphics;
using TextAlignment = Infragistics.Documents.Reports.Report.TextAlignment;
using System.Drawing.Imaging;
using Microsoft.VisualBasic;
using Infragistics.WebUI.UltraWebNavigator;
using System.Globalization;
using Infragistics.WebUI.UltraWebGrid.DocumentExport;
using Infragistics.Documents.Excel;
using Infragistics.Documents.Reports.Report.Section;
using Infragistics.UltraGauge.Resources;
using Infragistics.UltraChart.Core.Layers;

namespace Krista.FM.Server.Dashboards.reports.HMAO_ARC._0002
{
    public partial class _default : CustomReportPage
    {
        string page_title = "Водные ресурсы региона";
        string page_sub_title = "Информация о водных ресурсах, забранных из природных водных объектов городов и районов Ханты-Мансийского автономного округа – Югры, допустимом лимите забора воды, потерях воды при транспортировке, {0} год";
        string chart1_title = "Объем пресной воды, забранной из природных водных объектов за {0} год, {1}";
        string chart2_title = "Потери воды при транспортировке за {0} год, {1}";
        private CustomParam SelectedYear;
        private CustomParam currentWay;
        private GridHeaderLayout headerLayout;
        int[] offset; 
        Dictionary<string, int> YearsLoad(string sql)
        {
            CellSet cs = DataProvidersFactory.SpareMASDataProvider.GetCellset(DataProvider.GetQueryText(sql));
            Dictionary<string, int> d = new Dictionary<string, int>();
            for (int i = 0; i <= cs.Axes[1].Positions.Count - 1; i++)
            {
                d.Add(cs.Axes[1].Positions[i].Members[0].Caption, 0);
            }
            return d;
        }
        string BN = "IE";
        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            System.Web.HttpBrowserCapabilities myBrowserCaps = Request.Browser;
            BN = ((System.Web.Configuration.HttpCapabilitiesBase)myBrowserCaps).Browser.ToUpper();
            base.Page_PreLoad(sender, e);
            SelectedYear = UserParams.CustomParam("year");
            currentWay = UserParams.CustomParam("currentWay");
            ComboYear.Width = 200;
            Grid.Height = 400;                
            if (BN != "APPLEMAC-SAFARI")
            {
                if (BN == "FIREFOX")
                {
                    Grid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 5);
                    Chart1.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 30);
                    Chart2.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 30);
                }
                else
                {
                    Grid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 5);

                    Chart1.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 5);
                    Chart2.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 5);
                }
            }
            else 
            {
                Grid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 15);
                Chart1.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 15);
                Chart2.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 15);
            }

            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportExcelExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
            ReportExcelExporter1.ExcelExporter.EndExport += new EndExportEventHandler(ExcelExporter_EndExport);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
            Grid.DisplayLayout.AllowSortingDefault = AllowSorting.No;
            Grid.DisplayLayout.CellClickActionDefault = CellClickAction.RowSelect;

        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            if (!Page.IsPostBack)
            {
                ComboYear.Title = "Выберите год";
                ComboYear.FillDictionaryValues(YearsLoad("years"));
                ComboYear.SelectLastNode();
            }
            SelectedYear.Value = "[Период__Год Квартал Месяц].[Период__Год Квартал Месяц].[Год].["+ComboYear.SelectedValue+"]";
            Chart1.DataBind();
            headerLayout = new GridHeaderLayout(Grid);
            Grid.DataBind();
            Grid.Rows[0].Selected = 1 == 1;
            Grid.Rows[0].Activated = 1 == 1;
            currentWay.Value = Grid.Rows[0].Cells[0].Text;
            Chart2.DataBind();
            Label1.Text = page_title;
            Page.Title = Label1.Text;
            Label2.Text =String.Format(page_sub_title,ComboYear.SelectedValue);
            Grid.DisplayLayout.AllowColSizingDefault = AllowSizing.Fixed; 
        }

        decimal Injectvalue(object o)
        {
            try
            {
                return (decimal)o;
            }
            catch { return 0; }
        }

        protected void Chart1_DataBinding(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();
            DataTable resDt = new DataTable();
            DataTable normDt = new DataTable();
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("chart1"), "chart1", dt);
            resDt.Columns.Add(dt.Columns[0].ColumnName, dt.Columns[0].DataType);
            for (int i = 1; i < dt.Columns.Count-1; i++)
            {
                string s = dt.Columns[i].ColumnName.Remove(dt.Columns[i].ColumnName.IndexOf(';'));
                resDt.Columns.Add(s, dt.Columns[i].DataType);
            }
            object[] o = new object[resDt.Columns.Count];
            decimal maxAdeded = 0;
            int gorCount = 0, moCount = 0;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (dt.Rows[i].ItemArray[0].ToString().Contains("муниципальный район"))
                {
                    o[0] = dt.Rows[i].ItemArray[0].ToString().Replace("муниципальный район","МР");
                }
                else { o[0] = dt.Rows[i].ItemArray[0]; }

                if (dt.Rows[i][0].ToString().StartsWith("Город"))
                {
                    gorCount += 1;
                }
                else
                {
                    moCount += 1;
                }
                decimal valBase = Injectvalue(dt.Rows[i].ItemArray[1]);
                decimal valNedo = Injectvalue(dt.Rows[i].ItemArray[2]);
                decimal valAdeded = Injectvalue(dt.Rows[i].ItemArray[3]);

                maxAdeded = maxAdeded<valAdeded?valAdeded:maxAdeded;

                decimal Base = valBase + valNedo;

                o[1] = valBase / Base * 100;
                o[3] = valNedo / Base * 100;
                o[2] = valAdeded / Base * 100;

                resDt.Rows.Add(o);
            }
            string[] goroda = new string[gorCount];
            string[] mo = new string[moCount];
            int k = 0, l = 0;
            for (int i = 0; i < resDt.Rows.Count; i++)
            {
                if (resDt.Rows[i][0].ToString().StartsWith("Город"))
                {
                    goroda[k] = resDt.Rows[i][0].ToString().Replace("Город", String.Empty).Replace(" ", String.Empty);
                    k += 1;
                }
                else
                {
                    mo[l] = resDt.Rows[i][0].ToString().Replace("МР", String.Empty).Replace(" ", String.Empty);
                    l += 1;
                    
                }
            }
            Array.Sort(goroda);
            Array.Sort(mo);

            for (int i = 0; i < resDt.Columns.Count; i++)
            {
                normDt.Columns.Add(resDt.Columns[i].ColumnName, resDt.Columns[i].DataType);
            }

            for (int j = 0; j < resDt.Rows.Count; j++)
            {
                if (resDt.Rows[j][0].ToString() == "Город Ханты-Мансийск")
                {
                    normDt.Rows.Add(resDt.Rows[j].ItemArray);
                }
            }

            for (int i = 0; i < goroda.Length; i++)
            {
                for (int j = 0; j < resDt.Rows.Count; j++)
                {
                    if (resDt.Rows[j][0].ToString() == "Город " + goroda[i] && resDt.Rows[j][0].ToString() != "Город Ханты-Мансийск")
                    {
                        normDt.Rows.Add(resDt.Rows[j].ItemArray);
                    }
                }
            }

            for (int i = 0; i < mo.Length; i++)
            {
                for (int j = 0; j < resDt.Rows.Count; j++)
                {
                    if (resDt.Rows[j][0].ToString() == mo[i] + " МР")
                    {
                        normDt.Rows.Add(resDt.Rows[j].ItemArray);
                    }
                }
            }

            Chart1.DataSource = normDt;
            Chart1.Tooltips.FormatString = @"<SERIES_LABEL>, <b><DATA_VALUE:##0.##></b> " + "%";
            Label3.Text = String.Format(chart1_title, ComboYear.SelectedValue, "процентов");
                //Chart1.Axis.X.Labels.SeriesLabels
            Chart1.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:##0.##>%";             

            Chart1.Axis.Y.RangeMax = 100 + (1 + (int)maxAdeded / 10) * 10;
            Chart1.Axis.Y.RangeMin = 0;
            Chart1.Axis.Y.RangeType =  AxisRangeType.Custom;
            Chart1.Axis.Y.TickmarkInterval = 10;
            Chart1.Axis.Y.TickmarkStyle = AxisTickStyle.DataInterval;
        }

        protected void Grid_DataBinding(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();
            DataTable resDt = new DataTable();
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("grid"), "МР ГО", dt);
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                resDt.Columns.Add(dt.Columns[i].ColumnName, dt.Columns[i].DataType);
            }
            resDt.Columns.Add("Доля", dt.Columns[1].DataType);
            object[] o = new object[resDt.Columns.Count];
            double countAll = 0;
            int gorCount = 0, moCount = 0;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (dt.Rows[i].ItemArray[9].ToString() != "")
                {
                    countAll += double.Parse(dt.Rows[i].ItemArray[9].ToString());
                }
                if (dt.Rows[i][0].ToString().StartsWith("Город"))
                {
                    gorCount += 1;
                }
                else
                {
                    moCount += 1;
                }
            }
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    o[j] = dt.Rows[i].ItemArray[j];
                }
                if (ComboYear.GetRootNodesName(0) == ComboYear.SelectedValue)
                {
                    o[2] = null;
                    o[5] = null;
                }
                if (o[o.Length - 2] != null)
                {
                    o[o.Length - 1] = double.Parse(o[o.Length - 2].ToString()) / countAll;
                }
                resDt.Rows.Add(o);
            }
            string[] goroda = new string[gorCount];
            string[] mo = new string[moCount];
            int k = 0, l = 0;
            for (int i = 0; i < resDt.Rows.Count; i++)
            {
                if (resDt.Rows[i][0].ToString().StartsWith("Город"))
                {
                    goroda[k] = resDt.Rows[i][0].ToString().Replace("Город", String.Empty).Replace(" ", String.Empty);
                    k += 1;
                } 
                else
                {
                    mo[l] = resDt.Rows[i][0].ToString().Replace("муниципальный район", String.Empty).Replace(" ", String.Empty);
                    l += 1;
                }
            }


            Array.Sort(goroda);
            Array.Sort(mo);
            DataTable normDt = new DataTable();
            for (int i = 0; i < resDt.Columns.Count; i++)
            {
                normDt.Columns.Add(resDt.Columns[i].ColumnName, resDt.Columns[i].DataType);
            }

            for (int j = 0; j < resDt.Rows.Count; j++)
            {
                if (resDt.Rows[j][0].ToString() == "Город Ханты-Мансийск")
                {
                    normDt.Rows.Add(resDt.Rows[j].ItemArray);
                }
            } 

            for (int i = 0; i < goroda.Length; i++)
            {
                for (int j = 0; j < resDt.Rows.Count; j++)
                {
                    if (resDt.Rows[j][0].ToString() == "Город " + goroda[i] && resDt.Rows[j][0].ToString() != "Город Ханты-Мансийск")
                    {
                        normDt.Rows.Add(resDt.Rows[j].ItemArray);
                    }
                }
            }

            for (int i = 0; i < mo.Length; i++)
            {
                for (int j = 0; j < resDt.Rows.Count; j++)
                {
                    if (resDt.Rows[j][0].ToString() == mo[i] + " муниципальный район")
                    {
                        normDt.Rows.Add(resDt.Rows[j].ItemArray);
                    }
                }
            }

                Grid.DataSource = normDt;
        }

        protected void Chart2_DataBinding(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();
            DataTable resDt = new DataTable();
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("chart2"), "chart2", dt);
            for (int i = 0; i < dt.Columns.Count-1; i++)
            {
                resDt.Columns.Add(dt.Columns[i].ColumnName.Replace("За период", " потери воды при транспортировке"), dt.Columns[i].DataType);
            }
            object[] o=new object[resDt.Columns.Count];
            
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (dt.Rows[i].ItemArray[0].ToString().Contains("муниципальный район"))
                {
                    string s = dt.Rows[i].ItemArray[0].ToString().Remove(dt.Rows[i].ItemArray[0].ToString().IndexOf("муниципальный район"));
                    s = s.Insert(s.Length - 1, "  МР");
                    o[0] = s;
                }
                else { o[0] = dt.Rows[i].ItemArray[0]; }
                o[1] = dt.Rows[i].ItemArray[1];
                resDt.Rows.Add(o);
            }
            double max = double.Parse(dt.Rows[0].ItemArray[1].ToString());
            Chart2.Axis.Y.RangeType = AxisRangeType.Custom;
            Chart2.Axis.Y.RangeMin = 0;
            Chart2.Axis.Y.RangeMax = max*1.1;
            Chart2.Axis.Y2.RangeMin = 0;
            Chart2.Axis.Y2.RangeMax = 110;
            Chart2.Axis.Y2.RangeType = AxisRangeType.Custom;
            Label4.Text = String.Format(chart2_title, ComboYear.SelectedValue,dt.Rows[0].ItemArray[2].ToString().ToLower());
            Chart2.Tooltips.FormatString = "Потери воды при транспортировке, <b><DATA_VALUE:### ##0.00></b> " + dt.Rows[0].ItemArray[2].ToString().ToLower();
            Chart2.DataSource = resDt;

            Chart2.Legend.Visible = true;
            Chart2.Legend.SpanPercentage = 15;
            Chart2.Legend.Location = LegendLocation.Bottom;
            
        }
        void UltraWebGrid_SetSelectedFoodparams(UltraGridRow GridRow)
        {
            if (GridRow.Cells[0].Value.ToString().Contains("]"))
            {
                string s = GridRow.Cells[0].Value.ToString();
                currentWay.Value = s.Insert(s.IndexOf(']'), "]");
            }
            else
            {
                currentWay.Value = GridRow.Cells[0].Value.ToString();
            }
        }
        protected void Grid_ActiveRowChange(object sender, RowEventArgs e)
        {
        }

        protected void Grid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            headerLayout.childCells.Clear();
            e.Layout.AllowSortingDefault = AllowSorting.No;
            e.Layout.AllowColumnMovingDefault = AllowColumnMoving.None;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.HeaderStyleDefault.VerticalAlign = VerticalAlign.Middle;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = 1 == 1;
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(CustomReportConst.minScreenWidth * 0.12);
            e.Layout.Bands[0].Columns[0].Header.Style.HorizontalAlign = HorizontalAlign.Center;
            for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i++)
            {
                e.Layout.Bands[0].Columns[i].Header.Style.HorizontalAlign = HorizontalAlign.Center;
                e.Layout.Bands[0].Columns[i].Header.Style.Wrap = true;
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
                e.Layout.Bands[0].Columns[i].CellStyle.CustomRules = "padding-right:10px;";
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "N2");
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(CustomReportConst.minScreenWidth * 0.08);
            }
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[3], "P2");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[6], "P2");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[10], "P2");
            string measure = Label3.Text.Split(',')[Label3.Text.Split(',').Length - 1];



            headerLayout.childCells.Clear();
            GridHeaderCell header = headerLayout.AddCell("МР ГО");
            header = headerLayout.AddCell("Установленный лимит забора пресной воды," + measure);
            header.AddCell("всего, млн. куб. м");
            header.AddCell("Отклонение от предыдущего года");
            header.AddCell("Темп роста к предыдущему году");
            //Объем пресной воды, забранной из водных объектов," + measure
            header = headerLayout.AddCell("Объем пресной воды, забранной из водных объектов," + measure);
            header.AddCell("всего, млн. куб. м");
            header.AddCell("Отклонение от предыдущего года");
            header.AddCell("Темп роста к предыдущему году");

            header = headerLayout.AddCell("Отклонение от установленного лимита забора пресной воды," + measure);
            header.AddCell("всего, млн. куб. м");
            header.AddCell("в %");

            header = headerLayout.AddCell("Потери воды при транспортировке," + measure);
            header.AddCell("всего, млн. куб. м");
            header.AddCell("Доля");
            headerLayout.ApplyHeaderInfo();
         //   e.Layout.AllowColSizingDefault = AllowSizing.Fixed;
        }

        protected void Grid_InitializeRow(object sender, RowEventArgs e)
        {
            e.Row.Cells[2].Title = "Отклонение от предыдущего года";
            e.Row.Cells[5].Title = "Отклонение от предыдущего года";
            e.Row.Cells[6].Title = "Темп роста к предыдущему году";
            e.Row.Cells[3].Title = "Темп роста к предыдущему году";
            if (e.Row.Cells[8].Text != "")
            {
                double m = double.Parse(e.Row.Cells[8].Text);
                e.Row.Cells[8].Text = Math.Round(m, 2).ToString() + "%";
            }
            if (e.Row.Cells[7].Value!=null)
            {

                if (double.Parse(e.Row.Cells[7].Text) >= 0)
                {
                    e.Row.Cells[7].Style.BackgroundImage = "~/images/ballGreenBB.png";
                    e.Row.Cells[7].Style.CustomRules = "background-repeat: no-repeat; background-position: 10px center; margin-left: 20px";
                }
                if (double.Parse(e.Row.Cells[7].Text) < 0)
                {
                    e.Row.Cells[7].Style.BackgroundImage = "~/images/ballRedBB.png";
                    e.Row.Cells[7].Style.CustomRules = "background-repeat: no-repeat; background-position: 10px center; margin-left: 20px";
                }
            }
            if (e.Row.Cells[8].Value != null)
            {

                if (double.Parse(e.Row.Cells[8].Text.Remove(e.Row.Cells[8].Text.Length - 1, 1)) > 100)
                {
                    e.Row.Cells[8].Style.BackgroundImage = "~/images/ballRedBB.png";
                    e.Row.Cells[8].Style.CustomRules = "background-repeat: no-repeat; background-position: 10px center; margin-left: 20px";
                }
                else
                {
                    e.Row.Cells[8].Style.BackgroundImage = "~/images/ballGreenBB.png";
                    e.Row.Cells[8].Style.CustomRules = "background-repeat: no-repeat; background-position: 10px center; margin-left: 20px";
                }
            }
            if (e.Row.Cells[0].Text.Contains("муниципальный район"))
            {
                string s = e.Row.Cells[0].Text.Remove(e.Row.Cells[0].Text.IndexOf("муниципальный район"));
                s=s.Insert(s.Length-1," МР");
                e.Row.Cells[0].Text = s;
            }
        }

        #region Экспорт в Excel

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            ReportExcelExporter1.WorksheetTitle = Label1.Text;
            ReportExcelExporter1.WorksheetSubTitle = Label2.Text;
            Workbook workbook = new Workbook();
            Worksheet sheet1 = workbook.Worksheets.Add("Таблица");
            Worksheet sheet2 = workbook.Worksheets.Add("Диаграмма 1");
            Worksheet sheet3 = workbook.Worksheets.Add("Диаграмма 2");
            ReportExcelExporter1.HeaderCellFont = new System.Drawing.Font("Verdana", 11);
            ReportExcelExporter1.TitleFont = new System.Drawing.Font("Verdana", 12, FontStyle.Bold);
            ReportExcelExporter1.SubTitleFont = new System.Drawing.Font("Verdana", 11);
            ReportExcelExporter1.TitleAlignment = HorizontalCellAlignment.Center;
            ReportExcelExporter1.TitleStartRow = 1;
            ReportExcelExporter1.Export(headerLayout, sheet1,4);
            ReportExcelExporter1.WorksheetTitle = String.Empty;
            ReportExcelExporter1.WorksheetSubTitle = String.Empty;
            int width = int.Parse(Chart1.Width.Value.ToString()); 
            Chart1.Width = 900;  
            Chart2.Width = 900;
            ReportExcelExporter1.Export(Chart1, Label3.Text, sheet2, 2);
            ReportExcelExporter1.Export(Chart2, Label4.Text, sheet3, 2);
            Chart1.Width = width;
            Chart2.Width = width;
        }

        private static void ExcelExporter_BeginExport(object sender, BeginExportEventArgs e)
        {
            e.CurrentWorksheet.PrintOptions.Orientation = Infragistics.Documents.Excel.Orientation.Landscape;
            e.CurrentWorksheet.PrintOptions.PaperSize = PaperSize.A4;
            e.CurrentWorksheet.PrintOptions.MaxPagesHorizontally = 1;
            e.CurrentWorksheet.PrintOptions.MaxPagesHorizontally = 1;
            e.CurrentWorksheet.PrintOptions.ScalingType = ScalingType.FitToPages;
            e.CurrentWorksheet.PrintOptions.BottomMargin = 0;
            e.CurrentWorksheet.PrintOptions.TopMargin = 0;
            e.CurrentWorksheet.PrintOptions.LeftMargin = 0;
            e.CurrentWorksheet.PrintOptions.RightMargin = 0;
        }
        private void ExcelExporter_EndExport(object sender, EndExportEventArgs e)
        {
           /* string measure = Label3.Text.Split(',')[Label3.Text.Split(',').Length - 1];
            e.Workbook.Worksheets["Таблица"].MergedCellsRegions.Clear();
            e.Workbook.Worksheets["Таблица"].MergedCellsRegions.Add(2, 0, 2, 10);
            e.Workbook.Worksheets["Таблица"].Columns[0].CellFormat.WrapText = ExcelDefaultableBoolean.False;
            e.Workbook.Worksheets["Таблица"].Rows[2].Height =600;
            e.Workbook.Worksheets["Таблица"].Rows[1].Cells[0].CellFormat.Alignment = HorizontalCellAlignment.Left;
            e.Workbook.Worksheets["Таблица"].Rows[2].Cells[0].CellFormat.Alignment = HorizontalCellAlignment.Left;
            e.Workbook.Worksheets["Таблица"].Rows[2].Cells[0].CellFormat.VerticalAlignment = VerticalCellAlignment.Justify;
            
            e.Workbook.Worksheets["Таблица"].Rows[6].Height = e.Workbook.Worksheets["Таблица"].Rows[7].Height;
            */
            e.Workbook.Worksheets["Таблица"].Rows[2].Height = 600;
            e.Workbook.Worksheets["Таблица"].Rows[7].Height += 200;
            e.Workbook.Worksheets["Диаграмма 1"].Rows[1].Cells[0].CellFormat.Alignment = HorizontalCellAlignment.Left;
            e.Workbook.Worksheets["Диаграмма 2"].Rows[1].Cells[0].CellFormat.Alignment = HorizontalCellAlignment.Left;
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
            ISection section2 = report.AddSection();
            ISection section3 = report.AddSection();
            int width = int.Parse(Chart1.Width.Value.ToString());
            Chart1.Width = 1000;
            Chart2.Width = 1000;
            ReportPDFExporter1.Export(headerLayout, section1);
            ReportPDFExporter1.Export(Chart1,Label3.Text, section2);
            ReportPDFExporter1.Export(Chart2,Label4.Text, section3);
            Chart1.Width = width;
            Chart2.Width = width;
            section1.AddText();
        }

        #endregion
        /*        */
        protected void Chart1_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            for (int i = 0; i < e.SceneGraph.Count; i++)
            {
               Infragistics.UltraChart.Core.Primitives.Primitive primitive = e.SceneGraph[i];
               try
               {
                   if ((primitive is Infragistics.UltraChart.Core.Primitives.Text) 
                       || 
                       primitive.Path.Contains("Border.Title.Grid.Y"))
                   {
                       Infragistics.UltraChart.Core.Primitives.Text t = (Infragistics.UltraChart.Core.Primitives.Text)primitive;
                       if (t.GetTextString().Contains("10%")||
                           t.GetTextString().Contains("20%")||
                           t.GetTextString().Contains("40%"))
                       {
                           t.SetTextString("");
                       }
                       if (t.GetTextString().Contains("30%"))
                       {
                           IAdvanceAxis xAxis = (IAdvanceAxis)e.Grid["X"];
                           IAdvanceAxis yAxis = (IAdvanceAxis)e.Grid["Y"];
                           t.bounds.Y = (int)yAxis.Map(27);
                           
                           t.SetTextString("25%");
                       }
                       
                   }
                   
               }
               catch { }

               if (primitive is Infragistics.UltraChart.Core.Primitives.Box)
                {
                    Infragistics.UltraChart.Core.Primitives.Box box = (Infragistics.UltraChart.Core.Primitives.Box)primitive;
                    if (box.DataPoint != null)
                    {
                        Infragistics.UltraChart.Core.Primitives.Text text = new Infragistics.UltraChart.Core.Primitives.Text();
                        if (box.DataPoint.Label == "Недобор объема пресной воды до установленного лимита")
                        {
                            box.PE.Fill = Color.Blue;
                            box.PE.ElementType = PaintElementType.SolidFill;
                            if (box.rect.Height != 0)
                            {
                                Infragistics.UltraChart.Core.Primitives.Box box1 = (Infragistics.UltraChart.Core.Primitives.Box)e.SceneGraph[i - 2];


                                text.SetTextString(String.Format("{0:0.00}", (100 - (Convert.ToDouble(box1.Value)))));
                                text.bounds.Y = box.rect.Y + box.rect.Height / 2;
                                text.bounds.X = box.rect.X+box.rect.Width/4;
                                LabelStyle ls = new LabelStyle();
                                ls.Font= new System.Drawing.Font("Verdana",8);
                                text.SetLabelStyle(ls);
                                e.SceneGraph.Add(text);
                            }
                        }
                        else
                        {
                            if (box.DataPoint.Label == "Объем пресной воды, забранной из природных водных объектов сверх лимита")
                            {
                                box.PE.Fill = Color.Crimson;
                                box.PE.ElementType = PaintElementType.SolidFill;
                                box.PE.FillOpacity = 255;
                                Chart1.Data.SwapRowsAndColumns = true;
                                if (box.rect.Height != 0)
                                {
                                    text.SetTextString(String.Format("{0:0.00}", (Convert.ToDouble(box.Value) - 100)));
                                    text.bounds.Y = box.rect.Y + box.rect.Height / 2;
                                    text.bounds.X = box.rect.X + box.rect.Width / 4;
                                    LabelStyle ls = new LabelStyle();
                                    ls.Font = new System.Drawing.Font("Verdana", 8);
                                    text.SetLabelStyle(ls);
                                    e.SceneGraph.Add(text);
                                }
                            }
                            else
                            {
                                if (box.rect.Height != 0)
                                {
                                    text.SetTextString(String.Format("{0:0.00}", (Convert.ToDouble(box.Value))));
                                    text.bounds.Y = box.rect.Y + box.rect.Height / 2;
                                    text.bounds.X = box.rect.X + box.rect.Width / 4;
                                    LabelStyle ls = new LabelStyle();
                                    ls.Font = new System.Drawing.Font("Verdana", 8);
                                    text.SetLabelStyle(ls);
                                    e.SceneGraph.Add(text);
                                }
                            }
                        }
                 
                        
                    }
                    else
                    {                        
                        if (box.Column == 2)
                        {
                            //continue;
                            //box.PE.Fill = Color.SlateBlue;
                            box.PE.Fill = Color.Blue;
                            
                            box.PE.ElementType = PaintElementType.SolidFill;
                        }
                        if (box.Column==1)
                        {
                            box.PE.Fill = Color.Red;
                                
                            box.PE.ElementType = PaintElementType.SolidFill;
                            box.PE.FillOpacity = 255;
                        }
                        if (box.Column == 0)
                        {
                            continue;
                            //box.PE.Fill = Color.Red;
                            //Color.Crimson;
                            box.PE.ElementType = PaintElementType.SolidFill;
                            box.PE.FillOpacity = 255;
                        }
                    }
                } 
            }
        }

        protected void Chart2_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            for (int i = 0; i < e.SceneGraph.Count; i++)
            { 
                Infragistics.UltraChart.Core.Primitives.Primitive primitive = e.SceneGraph[i];
                if (primitive is Infragistics.UltraChart.Core.Primitives.Box)
                {
                    Infragistics.UltraChart.Core.Primitives.Box box = (Infragistics.UltraChart.Core.Primitives.Box)primitive;
                    if (box.DataPoint != null)
                    {
                        Infragistics.UltraChart.Core.Primitives.Text text = new Infragistics.UltraChart.Core.Primitives.Text();
                        text.SetTextString(box.Value.ToString());
                        text.bounds.X = box.rect.X;
                        text.bounds.Y = box.rect.Y-20;
                        text.bounds.Width = 25;
                        text.bounds.Height = 20;
                        text.labelStyle.HorizontalAlign = StringAlignment.Center;
                        e.SceneGraph.Add(text);
                        
                    }
                }
            }
        }
    }
}
