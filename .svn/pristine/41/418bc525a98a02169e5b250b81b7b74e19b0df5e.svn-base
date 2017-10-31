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
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.WebUI.Misc;
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
using System.Xml;
using Infragistics.Documents.Reports.Report.Section;
namespace Krista.FM.Server.Dashboards.reports.REGION.REGION_0010.WorkMarks_Novosib
{
    public partial class _default : CustomReportPage
    {
        private string BN = "IE";
        private string chart1_title = "Динамика показателя \x00ab{0}\x00bb, {1}";
        private string chart2_title = "Темп прироста показателя \x00ab{0}\x00bb, процент";
        private int counter;
        private GridHeaderLayout headerLayout;
        private CustomParam norefresh;
        private CustomParam norefresh2;
        private string page_title = "Оценка эффективности деятельности органов исполнительной власти субъекта Российской Федерации ({0})";
        private string s = "";
        private string stuff_id = "default";
        private string style = "";
        private string title = "Доклад сформирован в соответствии с формой, утвержденной в постановлении Правительства Российской Федерации от 15 апреля 2009 г. № 322 «О мерах по реализации Указа Президента Российской Федерации от 28 июня 2007 г. № 825» в редакции постановления Правительства Российской Федерации от 18 декабря 2010 г. № 1052";
        private DataTable UltraChart1Dt;
        CustomParam baseRegion;
        CustomParam currentArea;
        CustomParam currentPok;
        CustomParam lastYear;
        private String dir
        {
            get { return Server.MapPath("~") + "\\"; }
        }
        private bool IsSmallResolution
        {
            get
            {
                return (CRHelper.GetScreenWidth < 0x4b0);
            }
        }
        private int minScreenWidth
        {
            get
            {
                if (!this.IsSmallResolution)
                {
                    return CustomReportConst.minScreenWidth;
                }
                return 750;
            }
        }

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);
            HttpBrowserCapabilities browser = base.Request.Browser;
            this.BN = browser.Browser.ToUpper();
            this.Grid.Width = 0x4c9;
            this.DinamicChart.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth * 0.5) - 0x11;
            this.UltraChart1.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth * 0.5) - 0x11;
            this.Label2.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth * 0.5) - 0x23;
            if (this.BN == "APPLEMAC-SAFARI")
            {
                this.Label3.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth * 0.5) - 10;
            }
            else
            {
                this.Label3.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth * 0.5) - 10;
            }
            if (this.BN != "FIREFOX")
            {
                this.Grid.Height = 220;
            }
            this.AreaCombo.Width = 300.0;
            //this.RefreshPanel.AddLinkedRequestTrigger(this.Grid);
            //this.RefreshPanel.AddRefreshTarget(this.DinamicChart);
            //this.RefreshPanel.AddRefreshTarget(this.UltraChart1);
            this.norefresh = base.UserParams.CustomParam("r");
            this.norefresh2 = base.UserParams.CustomParam("r2");
            this.baseRegion = base.UserParams.CustomParam("baseRegion");
            this.currentArea = base.UserParams.CustomParam("currentArea");
            this.currentPok = base.UserParams.CustomParam("currentPok");
            this.lastYear = base.UserParams.CustomParam("lastYear");
            this.ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(this.ExcelExportButton_Click);
            this.ReportPDFExporter1.PdfExportButton.Click += new EventHandler(this.PdfExportButton_Click);
            this.ReportExcelExporter1.ExcelExporter.EndExport += new EndExportEventHandler(this.ExcelExporter_EndExport);
        }
        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            this.baseRegion.Value = RegionSettingsHelper.Instance.RegionBaseDimension;
            this.lastYear.Value = this.GetLastDate();
            if (!this.Page.IsPostBack)
            {
                this.AreaCombo.FillDictionaryValues(this.AreaLoad("areas"));
                this.AreaCombo.Title = "Сфера";
            }
            this.Label1.Text = string.Format(this.page_title, RegionSettings.Instance.Name);
            this.Label4.Text = this.title;
            this.Page.Title = this.Label1.Text;
            if (this.AreaCombo.SelectedIndex <= 8)
            {
                this.currentArea.Value = "0" + ((this.AreaCombo.SelectedIndex + 1)).ToString() + "." + this.AreaCombo.SelectedValue;
            }
            else
            {
                this.currentArea.Value = ((this.AreaCombo.SelectedIndex + 1)).ToString() + "." + this.AreaCombo.SelectedValue;
            }
            this.headerLayout = new GridHeaderLayout(this.Grid);
            this.Grid.DataBind();
            if (Grid.Rows.Count > 0)
            {

                this.currentPok.Value = this.Grid.Rows[0].Cells[Grid.Rows[0].Cells.Count - 1].Text;//this.Grid.Rows[0].Cells[0].Text;
                this.Grid.Rows[0].Selected = true;
                this.Grid.Rows[0].Activated = true;
                this.Grid.Rows[0].Activate();
                this.DinamicChart.DataBind();
                this.UltraChart1.DataBind();
                this.Label2.Text = string.Format(this.chart1_title, this.Grid.Rows[0].Cells[0].Text, this.Grid.Rows[0].Cells[1].Text.ToLower());
                this.DinamicChart.Tooltips.FormatString = "<b><DATA_VALUE:### ### ##0.00></b> " + this.Grid.Rows[0].Cells[1].Text.ToLower();
                this.Label3.Text = string.Format(this.chart2_title, this.Grid.Rows[0].Cells[0].Text);
            }
            else
            {
                Grid.Columns.Clear();
                Grid.Bands.Clear();
                DinamicChart.DataSource = null;
                UltraChart1.DataSource = null;
                Label2.Text = "Нет данных";
                Label3.Text = "Нет данных";
            }
            this.Grid.DisplayLayout.AllowSortingDefault = AllowSorting.No;
            this.Grid.DisplayLayout.CellClickActionDefault = CellClickAction.NotSet;
            this.Panel1.Visible = true;
            if (this.norefresh.Value != this.AreaCombo.SelectedValue)
            {
                this.norefresh2.Value = "no";
            }
            else
            {
                this.norefresh2.Value = "Yes";
            }
            this.norefresh.Value = this.AreaCombo.SelectedValue;
        }
        protected string GetLastDate()
        {
            try
            {
                if (RegionSettings.Instance.Id == "HMAO")
                {
                    return DataProvidersFactory.SpareMASDataProvider.GetCellset(DataProvider.GetQueryText("LastDate")).Axes[1].Positions[0].Members[0].UniqueName;
                }
                else
                {
                    return DataProvidersFactory.PrimaryMASDataProvider.GetCellset(DataProvider.GetQueryText("LastDate")).Axes[1].Positions[0].Members[0].UniqueName;
                }
            }
            catch
            {
                return "";
            }
        }
        protected Dictionary<string, int> AreaLoad(string q)
        {
            Dictionary<string, int> dictionary = new Dictionary<string, int>();
            DataTable dt = new DataTable();
            if (RegionSettings.Instance.Id == "HMAO")
            {
                DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("areas"), "Показатели", dt);
            }
            else
            {
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("areas"), "Показатели", dt);
            }
            string str = "";
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (str != dt.Rows[i][1].ToString())
                {
                    dictionary.Add(dt.Rows[i][1].ToString().Remove(0, 3), 0);
                    str = dt.Rows[i][1].ToString();
                }
            }
            return dictionary;
        }
        protected string DeleteProb(string s)
        {
            string str = "";
            for (int i = 0; i < s.Length; i++)
            {
                if (s[i] != ' ')
                {
                    str = str + s[i];
                }
            }
            return str;
        }
        protected void Grid_DataBinding(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();
            DataTable table2 = new DataTable();
            if (RegionSettings.Instance.Id == "HMAO")
            {
                DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("Grid"), "Показатели", dt);
            }
            else
            {
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("Grid"), "Показатели", dt);
            }
            table2.Columns.Add(dt.Columns[0].ColumnName, dt.Columns[0].DataType);
            table2.Columns.Add(dt.Columns[1].ColumnName, dt.Columns[1].DataType);
            for (int i = 3; i < dt.Columns.Count; i++)
            {
                table2.Columns.Add(dt.Columns[i].ColumnName, dt.Columns[i].DataType);
            }
            object[] values = new object[table2.Columns.Count];
            for (int j = 0; j < dt.Rows.Count; j++)
            {
                if (dt.Rows[j][dt.Rows[j].ItemArray.Length - 1].ToString() != "")
                {
                    values[0] = dt.Rows[j][dt.Rows[j].ItemArray.Length - 1];
                }
                else
                {
                    values[0] = dt.Rows[j][0];
                }
                values[1] = dt.Rows[j][1];
                for (int k = 2; k < (dt.Rows[j].ItemArray.Length - 1); k++)
                {
                    if ((((j % 3) == 0) && (dt.Rows[j][k] == DBNull.Value)) && (k != (dt.Rows[j].ItemArray.Length - 2)))
                    {
                        dt.Rows[j + 1][k + 1] = DBNull.Value;
                    }
                    values[k] = dt.Rows[j][k + 1];
                }
                values[values.Length - 1] = dt.Rows[j][0].ToString().Split(new char[] { ';' })[0];
                table2.Rows.Add(values);
            }
            this.Grid.DataSource = table2;
        }


        protected double GetNumber(string s)
        {
            try
            {
                if (!string.IsNullOrEmpty(s))
                {
                    return double.Parse(s);
                }
                return 0.0;
            }
            catch
            {
                return 0.0;
            }
        }

        decimal GetMinFormTable(DataTable T, int ValueCol)
        {
            decimal max = decimal.MaxValue;
            foreach (DataRow Row in T.Rows)
            {
                decimal iterVal = (decimal)(Row[ValueCol]);
                max = iterVal < max ? iterVal : max;
            }
            return max;
        }

        decimal GetMaxFormTable(DataTable T, int ValueCol)
        {
            decimal max = decimal.MinValue;
            foreach (DataRow Row in T.Rows)
            {
                decimal iterVal = (decimal)(Row[ValueCol]);
                max = iterVal > max ? iterVal : max;
            }
            return max;
        }

        protected void DinamicChart_DataBinding(object sender, EventArgs e)
        {


            DataTable dt = new DataTable();   
            DataTable table2 = new DataTable();

            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("Dinamic"), "dfd", dt);
             
            DinamicChart.Axis.Y.RangeMin = 0.9 * (double)GetMinFormTable(dt, 1);
            DinamicChart.Axis.Y.RangeMax = 1.1 * (double)GetMaxFormTable(dt, 1);
            DinamicChart.Axis.Y.RangeType = AxisRangeType.Custom;
            this.DinamicChart.DataSource = dt;
        }

        protected void Grid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.AllowColSizingDefault = AllowSizing.Fixed;
            e.Layout.AllowColumnMovingDefault = AllowColumnMoving.None;
            e.Layout.AllowSortingDefault = AllowSorting.No;
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetChartWidth(this.minScreenWidth * 0.47);
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[1].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[e.Layout.Bands[0].Columns.Count - 1].Hidden = true;

            this.headerLayout.AddCell("Показатели");
            for (int i = 1; i < (e.Layout.Bands[0].Columns.Count - 1); i++)
            {
                this.headerLayout.AddCell(e.Layout.Bands[0].Columns[i].Key);
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "N2");
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(this.minScreenWidth * 0.11);
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            }
            e.Layout.Bands[0].Columns[1].CellStyle.HorizontalAlign = HorizontalAlign.Center;
            this.headerLayout.ApplyHeaderInfo();
        }

        protected void Grid_InitializeRow(object sender, RowEventArgs e)
        {
            if (this.IsSmallResolution)
            {
                this.style = "background-repeat: no-repeat;background-position:15px";
            }
            else
            {
                this.style = "background-repeat: no-repeat;background-position: 20px";
            }
            if (((e.Row.Index + 1) % 3) == 0)
            {
                this.Grid.Rows[e.Row.Index - 2].Cells[0].Text = this.Grid.Rows[e.Row.Index - 2].Cells[0].Text.Split(new char[] { ';' })[0];

                this.Grid.Rows[e.Row.Index - 1].Cells[0].Text = "<div style='float:right; margin-right:10px'>абсолютное отклонение</div>";
                e.Row.Cells[0].Text = "<div style='float:right; margin-right:10px'>темп прироста</div>";


                e.Row.Cells[1].Text = "";
                this.Grid.Rows[e.Row.Index - 1].Cells[1].Text = "";
                this.Grid.Rows[e.Row.Index - 2].Cells[1].Style.BackColor = Color.White;
                this.Grid.Rows[e.Row.Index - 1].Cells[1].Style.BackColor = Color.White;
                this.Grid.Rows[e.Row.Index - 1].Cells[1].Style.BorderDetails.StyleBottom = BorderStyle.None;
                this.Grid.Rows[e.Row.Index - 2].Cells[1].Style.BorderDetails.StyleBottom = BorderStyle.None;
                e.Row.Cells[1].Style.BackColor = Color.White;
                if (this.Grid.Rows[e.Row.Index - 2].Cells[1].Text == "да/нет")
                {
                    for (int i = 2; i < (e.Row.Cells.Count - 1); i++)
                    {
                        if (Grid.Rows[e.Row.Index - 2].Cells[i].Value != null)
                        {
                            if (Convert.ToDouble(this.Grid.Rows[e.Row.Index - 2].Cells[i].Value) == 1)
                            {
                                this.Grid.Rows[e.Row.Index - 2].Cells[i].Text = "Да";
                            }
                            else
                            {
                                this.Grid.Rows[e.Row.Index - 2].Cells[i].Text = "Нет";
                            }
                        }
                        this.Grid.Rows[e.Row.Index - 1].Cells[i].Text = "";
                        e.Row.Cells[i].Text = "";
                    }
                }
                else
                {
                    for (int j = 2; j < (e.Row.Cells.Count - 1); j++)
                    {
                        e.Row.Cells[j].Style.BackColor = Color.White;
                        if (Convert.ToDouble(this.Grid.Rows[e.Row.Index - 1].Cells[j].Value) < 0.0)
                        {
                            this.Grid.Rows[e.Row.Index - 1].Cells[j].Style.BackgroundImage = "~/images/arrowDownYellow.png";
                            this.Grid.Rows[e.Row.Index - 1].Cells[j].Style.CustomRules = this.style;
                        }
                        if (Convert.ToDouble(this.Grid.Rows[e.Row.Index - 1].Cells[j].Value) > 0.0)
                        {
                            this.Grid.Rows[e.Row.Index - 1].Cells[j].Style.BackgroundImage = "~/images/arrowUpYellow.png";
                            this.Grid.Rows[e.Row.Index - 1].Cells[j].Style.CustomRules = this.style;
                        }
                        if (e.Row.Cells[j].Value != null)
                        {
                            e.Row.Cells[j].Value = string.Format("{0:P2}", Convert.ToDouble(e.Row.Cells[j].Value));
                        }
                    }
                }
            }
        }

        protected void Grid_ActiveRowChange(object sender, RowEventArgs e)
        {
            if (this.norefresh2.Value == "Yes")
            {
                if (e.Row.Cells[0].Text.Contains("абсолютное отклонение"))
                {
                    this.currentPok.Value = this.Grid.Rows[e.Row.Index - 1].Cells[this.Grid.Rows[e.Row.Index - 1].Cells.Count - 1].Text;
                }
                else if (e.Row.Cells[0].Text.Contains("темп прироста"))
                {
                    this.currentPok.Value = this.Grid.Rows[e.Row.Index - 2].Cells[this.Grid.Rows[e.Row.Index - 2].Cells.Count - 1].Text;
                }
                else
                {
                    this.currentPok.Value = e.Row.Cells[e.Row.Cells.Count - 1].Text;
                }
                if (e.Row.Cells[0].Text != "Наличие в субъекте Российской Федерации утвержденных схем (схемы) территориального планирования субъекта Российской Федерации")
                {
                    this.Panel1.Visible = true;
                    this.DinamicChart.DataBind();
                    this.UltraChart1.DataBind();
                    e.Row.Activate();
                }
                else
                {
                    this.Panel1.Visible = false;
                    this.DinamicChart.DataSource = null;
                    this.UltraChart1.DataSource = null;
                }
                if (e.Row.Cells[0].Text.Contains("абсолютное отклонение"))
                {
                    this.Label2.Text = string.Format(this.chart1_title, this.Grid.Rows[e.Row.Index - 1].Cells[0].Text, this.Grid.Rows[e.Row.Index - 1].Cells[1].Text.ToLower());
                    this.DinamicChart.Tooltips.FormatString = "<b><DATA_VALUE:### ### ##0.00></b> " + this.Grid.Rows[e.Row.Index - 1].Cells[1].Text.ToLower();
                    this.Label3.Text = string.Format(this.chart2_title, this.Grid.Rows[e.Row.Index - 1].Cells[0].Text);
                }
                else if (e.Row.Cells[0].Text.Contains("темп прироста"))
                {
                    this.Label2.Text = string.Format(this.chart1_title, this.Grid.Rows[e.Row.Index - 2].Cells[0].Text, this.Grid.Rows[e.Row.Index - 2].Cells[1].Text.ToLower());
                    this.DinamicChart.Tooltips.FormatString = "<b><DATA_VALUE:### ### ##0.00></b> " + this.Grid.Rows[e.Row.Index - 2].Cells[1].Text.ToLower();
                    this.Label3.Text = string.Format(this.chart2_title, this.Grid.Rows[e.Row.Index - 2].Cells[0].Text);
                }
                else
                {
                    this.Label2.Text = string.Format(this.chart1_title, e.Row.Cells[0].Text, e.Row.Cells[1].Text.ToLower());
                    this.DinamicChart.Tooltips.FormatString = "<b><DATA_VALUE:### ### ##0.00></b> " + e.Row.Cells[1].Text.ToLower();
                    this.Label3.Text = string.Format(this.chart2_title, e.Row.Cells[0].Text);
                }
            }
        }
        protected void SetErorFonn(object sender, Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventArgs e)
        {
            e.Text = "В настоящий момент данные отсутствуют";
            e.LabelStyle.FontColor = System.Drawing.Color.LightGray;
            e.LabelStyle.VerticalAlign = StringAlignment.Center;
            e.LabelStyle.HorizontalAlign = StringAlignment.Center;
            e.LabelStyle.Font = new System.Drawing.Font("Verdana", 30);
        }

        DataTable GetDS(string QID, string FirstFieldName)
        {
            DataTable Table = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText(QID), FirstFieldName, Table);
            return Table;
        }

        protected void UltraChart1_DataBinding(object sender, EventArgs e)
        {
            DataTable TableChart = GetDS("Temp", "year");
            if (TableChart.Rows.Count < 2)
            {
                UltraChart1.DataSource = null;
            }
            else
            {
                int NumberValueCol = 1;

                List<DataRow> RemoveRow = new List<DataRow>();

                decimal PrevVal = decimal.MinValue;

                foreach (DataRow Row in TableChart.Rows)
                {
                    decimal curVal = (decimal)Row[NumberValueCol];
                    if (PrevVal != decimal.MinValue)
                    {
                        if (PrevVal == 0)
                        {
                            RemoveRow.Add(Row);    
                        }
                        else
                        {
                            Row[NumberValueCol] = (curVal / PrevVal - 1) * 100;
                        }
                    }
                    else
                    {
                        RemoveRow.Add(Row);
                    }
                    PrevVal = curVal;
                    
                }

                foreach (DataRow Row in RemoveRow)
                {
                    TableChart.Rows.Remove(Row);
                }

                UltraChart1.DataSource = TableChart;
            }

            #region old
            //try
            //{
            //    DataTable dt = new DataTable();
            //    DataTable table2 = new DataTable();

            //        DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("Temp"), "dfd", dt);

            //    for (int i = 0; i < dt.Columns.Count; i++)
            //    {
            //        table2.Columns.Add(dt.Columns[i].ColumnName, dt.Columns[i].DataType);
            //    }
            //    object[] values = new object[table2.Columns.Count];
            //    for (int j = 1; j < dt.Rows.Count; j++)
            //    {
            //        values = new object[table2.Columns.Count];
            //        values[0] = dt.Rows[j].ItemArray[0];
            //        if ((this.GetNumber(dt.Rows[j - 1].ItemArray[1].ToString()) != 0.0) && (this.GetNumber(dt.Rows[j].ItemArray[1].ToString()) != 0.0))
            //        {
            //            values[1] = ((this.GetNumber(dt.Rows[j].ItemArray[1].ToString()) / this.GetNumber(dt.Rows[j - 1].ItemArray[1].ToString())) - 1.0) * 100.0;
            //            if (this.GetNumber(values[1].ToString()) == -100.0)
            //            {
            //                values[1] = 0;
            //            }
            //        }
            //        else
            //        {
            //            values[1] = -1000;
            //        }
            //        table2.Rows.Add(values);
            //    }
            //    double num3 = 0.0;
            //    double num4 = 0.0;
            //    bool flag = false;
            //    for (int k = 0; k < table2.Rows.Count; k++)
            //    {
            //        if (num3 < double.Parse(table2.Rows[k].ItemArray[1].ToString()))
            //        {
            //            num3 = double.Parse(table2.Rows[k].ItemArray[1].ToString());
            //        }
            //        if ((num4 > double.Parse(table2.Rows[k].ItemArray[1].ToString())) && (double.Parse(table2.Rows[k].ItemArray[1].ToString()) != -1000.0))
            //        {
            //            num4 = double.Parse(table2.Rows[k].ItemArray[1].ToString());
            //        }
            //        if ((this.GetNumber(table2.Rows[k].ItemArray[1].ToString()) != 0.0) && (this.GetNumber(table2.Rows[k].ItemArray[1].ToString()) != -1000.0))
            //        {
            //            flag = true;
            //        }
            //    }
            //    if (flag)
            //    {
            //        this.UltraChart1.DataSource = table2;
            //    }
            //    else
            //    {
            //        this.UltraChart1.DataSource = null;
            //    }
            //    this.UltraChart1.Axis.Y.RangeType = AxisRangeType.Custom;
            //    this.UltraChart1.Axis.Y.RangeMax = num3 + 10.0;
            //    this.UltraChart1.Axis.Y.RangeMin = num4 - 10.0;
            //    this.UltraChart1.Tooltips.Display = TooltipDisplay.MouseMove;
            //    this.UltraChart1Dt = table2;
            //}
            //catch
            //{
            //}
            #endregion
        }
        protected void UltraChart1_FillSceneGraph(object sender, Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e)
        {
            //return;

            int count = e.SceneGraph.Count;
            for (int i = 0; i < count; i++)
            {
                Primitive primitive = e.SceneGraph[i];
                if (primitive is Text)
                {
                    Text text = (Text)primitive; 
                    try
                    {
                        text.Path.Contains("Title");
                    }
                    catch
                    {
                        if (this.UltraChart1.DataSource != null)
                        {
                            if ((text.GetTextString().Contains(",")))
                            {
                                
                                text.bounds.Y -= 10;
                                Box box = new Box(new Rectangle(text.bounds.X - 9, text.bounds.Y + 1, 0x11, 0x11));
                                PaintElement element = new PaintElement();
                                element.ElementType = PaintElementType.SolidFill;
                                if (this.GetNumber(text.GetTextString()) <= 0.0)
                                {
                                    element.Fill = Color.Yellow;
                                }
                                else
                                {
                                    element.Fill = Color.Green;
                                }
                                element.StrokeOpacity = 0;
                                box.PE = element;
                                e.SceneGraph.Add(box);
                            }
                        }
                    }
                }
            }
        }
        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            this.ReportPDFExporter1.PageTitle = this.Label1.Text;
            this.ReportPDFExporter1.PageSubTitle = this.Label4.Text;
            Infragistics.Documents.Reports.Report.Report report = new Infragistics.Documents.Reports.Report.Report();
            ISection section = report.AddSection();
            ISection section2 = report.AddSection();
            ISection section3 = report.AddSection();
            SetExportParametrs();
            this.ReportPDFExporter1.Export(this.headerLayout, section);
            this.ReportPDFExporter1.Export(this.DinamicChart, this.Label2.Text, section2);
            this.ReportPDFExporter1.Export(this.UltraChart1, this.Label3.Text, section3);
        }

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            this.ReportExcelExporter1.WorksheetTitle = this.Label1.Text;
            this.ReportExcelExporter1.WorksheetSubTitle = this.Label4.Text;
            Workbook workbook = new Workbook();
            Worksheet sheet = workbook.Worksheets.Add("Таблица");
            Worksheet worksheet2 = workbook.Worksheets.Add("Диаграмма 1");
            Worksheet worksheet3 = workbook.Worksheets.Add("Диаграмма 2");
            SetExportParametrs();
            this.ReportExcelExporter1.Export(this.headerLayout, sheet, 3);
            this.ReportExcelExporter1.WorksheetTitle = string.Empty;
            this.ReportExcelExporter1.WorksheetSubTitle = string.Empty;
            this.ReportExcelExporter1.Export(this.DinamicChart, this.Label2.Text, worksheet2, 2);
            this.ReportExcelExporter1.Export(this.UltraChart1, this.Label3.Text, worksheet3, 2);
        }

        private void ExcelExporter_EndExport(object sender, EndExportEventArgs e)
        {
            e.Workbook.Worksheets["Таблица"].Rows[1].Height = 500;
            e.Workbook.Worksheets["Таблица"].Rows[4].Cells[1].CellFormat.Alignment = HorizontalCellAlignment.Center;
        }

        private void SetExportParametrs()
        {
            for (int i = 0; i < this.Grid.Rows.Count; i += 3)
            {
                Grid.Rows[i + 1].Cells[0].Text = "абсолютное отклонение";
                Grid.Rows[i + 1].Cells[0].Style.HorizontalAlign = HorizontalAlign.Right;
                Grid.Rows[i + 2].Cells[0].Text = "темп прироста";
                Grid.Rows[i + 2].Cells[0].Style.HorizontalAlign = HorizontalAlign.Right;
            }
        }
    }
}
