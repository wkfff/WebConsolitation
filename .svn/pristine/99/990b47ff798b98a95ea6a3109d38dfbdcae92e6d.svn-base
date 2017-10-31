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
namespace Krista.FM.Server.Dashboards.reports.REGION.REGION_0010.Report825
{ 
    public partial class _default : CustomReportPage
    {
        private String dir
        {
            get { return Server.MapPath("~") + "\\"; }
        }
        private String stuff_id = "default";
        public DataTable GetDSForChart(string sql)
        {
            DataTable dt = new DataTable();
            string s = DataProvider.GetQueryText(sql);
            if (RegionSettings.Instance.Id == "HMAO")
            {
                DataProvidersFactory.SpareMASDataProvider.PopulateDataTableForChart(DataProvidersFactory.SpareMASDataProvider.GetCellset(s), dt, "sadad");
            }
            else
            {
                DataProvidersFactory.PrimaryMASDataProvider.PopulateDataTableForChart(DataProvidersFactory.PrimaryMASDataProvider.GetCellset(s), dt, "sadad");
            }
            
            return dt;
        }
        string[] RimCH = new string[11] {"I. ","II. ","III. ","IV.","V. ","VI. ","VII. ","VIII. ","IX. ","X. ","XI. "};
        CustomParam SelectItemGrid;
        CustomParam P1;
        CustomParam P2;
        CustomParam P3;
        CustomParam region;
        CustomParam P4;
        CustomParam baseRegion;
        private GridHeaderLayout headerLayout;
        String grid_title = "Показатели эффективности деятельности органов исполнительной власти";
        private string page_title = "Доклад высших должностных лиц (руководителей высших исполнительных органов государственной власти) субъектов РФ ({0})";
        private string pageSubTitle = "Доклад сформирован в соответствии с формой, утвержденной в постановлении Правительства Российской Федерации от 15 апреля 2009 г. № 322 «О мерах по реализации Указа Президента Российской Федерации от 28 июня 2007 г. № 825» (с учетом изменений)";
                                    //"Доклад сформирован в соответствии с формой, утвержденной в постановлении Правительства Российской Федерации от 15 апреля 2009 г. № 322 «О мерах по реализации Указа Президента Российской Федерации от 28 июня 2007 г. № 825» (с учетом изменений)";
        private int screen_width
        {
            get
            {
                return (int)this.Session["width_size"];
            }
        }
        protected override void Page_PreLoad(object sender, System.EventArgs e)
        {
            base.Page_PreLoad(sender, e);
            this.SelectItemGrid = base.UserParams.CustomParam("Param");
            this.P1 = base.UserParams.CustomParam("p1");
            this.P2 = base.UserParams.CustomParam("p2");
            this.P3 = base.UserParams.CustomParam("p3");
            this.P4 = base.UserParams.CustomParam("p4");
            this.region = base.UserParams.CustomParam("region");
            this.baseRegion = base.UserParams.CustomParam("baseRegion");
            this.G.Width = this.screen_width - 30;
            this.Hederglobal.Width = this.screen_width - 70;
            this.Label2.Width = this.screen_width - 70;
            this.G.DisplayLayout.AllowUpdateDefault = AllowUpdate.No;
            this.Type3.PanelHeaderToolTip = "";
            this.ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(this.ExcelExportButton_Click);
            this.ReportPDFExporter1.PdfExportButton.Click += new EventHandler(this.PdfExportButton_Click);
            this.ReportExcelExporter1.ExcelExporter.EndExport += new EndExportEventHandler(this.ExcelExporter_EndExport);
        }

        protected Dictionary<string, int> GetParam(string q, bool nofirst)
        {
            Dictionary<string, int> areas = new Dictionary<string, int>();
            

            DataTable dt = GetDSForChart(q);
            if (nofirst)
            {
                dt.Rows[0].Delete();
            }

            for (int i = dt.Rows.Count - 1; i > -1; i--)
            {
                areas.Add(dt.Rows[i].ItemArray[0].ToString(), 0);
            }

            return areas;

        }

        string realRegion = "Gubkinski";
        string BN = "IE";
        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            this.baseRegion.Value = RegionSettingsHelper.Instance.RegionBaseDimension;
            HttpBrowserCapabilities browser = base.Request.Browser;
            this.BN = browser.Browser.ToUpper();
            if (!this.Page.IsPostBack)
            {
                this.Type3.FillDictionaryValues(this.GetParam("LD", true));
            }
            this.P1.Value = this.Type3.SelectedValue;
            this.headerLayout = new GridHeaderLayout(this.G);
            this.G.DataBind();
            //this.G.Rows[0].Selected = true;
            //this.G.Rows[0].Activated = true;
            this.G.Height = Unit.Empty;
            this.G.DisplayLayout.CellClickActionDefault = CellClickAction.RowSelect;
            this.G.DisplayLayout.AllowSortingDefault = AllowSorting.No;
            this.Hederglobal.Text = string.Format(this.page_title, RegionSettings.Instance.Name);
            this.Page.Title = this.Hederglobal.Text;
            this.Label2.Text = this.pageSubTitle; 
            this.Label1.Text = this.grid_title;
        }

        Dictionary<string, int> param_3;
        protected void G_DataBinding(object sender, EventArgs e)
        {
            this.G.Rows.Clear();
            this.G.Columns.Clear();
            this.G.Bands[0].HeaderLayout.Clear();
            this.G.Bands.Clear();
            DataTable dt = new DataTable();
            DataTable table2 = new DataTable();
            if (RegionSettings.Instance.Id == "HMAO")
            {
                DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("G"), "Показатель", dt);
            }
            else
            {
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("G"), "Показатель", dt);
            }
            table2.Columns.Add("Номер в докладе");
            table2.Columns.Add(dt.Columns[0].ColumnName, dt.Columns[0].DataType);
            table2.Columns.Add(dt.Columns[2].ColumnName, dt.Columns[2].DataType);
            table2.Columns.Add(dt.Columns[3].ColumnName, dt.Columns[3].DataType);
            table2.Columns.Add(dt.Columns[4].ColumnName, dt.Columns[4].DataType);
            table2.Columns.Add(dt.Columns[5].ColumnName, dt.Columns[5].DataType);
            table2.Columns.Add(dt.Columns[6].ColumnName, dt.Columns[6].DataType);
            object[] values = new object[table2.Columns.Count];
            string str = "";
            int num = 1;
            int index = 0;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                bool flag = false;
                for (int j = 3; j < 7; j++)
                {
                    if ((dt.Rows[i][j] != DBNull.Value) //&& (Convert.ToDouble(dt.Rows[i][j]) != 0.0)
                        )
                    {
                        flag = true;
                    }
                }
                if (flag)
                {
                    values = new object[table2.Columns.Count];
                    if (str != dt.Rows[i][1].ToString())
                    {
                        values[0] = "_" + this.RimCH[index] + dt.Rows[i][1].ToString().Remove(0, 3);
                        str = dt.Rows[i][1].ToString();
                        index++;
                        table2.Rows.Add(values);
                    }
                    try
                    {
                        values[0] = dt.Rows[i]["Номер в докладе"].ToString().Replace("s","");
                    }
                    catch { }
                        //num;
                    //num++;
                    if (dt.Rows[i][dt.Rows[i].ItemArray.Length - 3].ToString() != "")
                    {
                        values[1] = dt.Rows[i][dt.Rows[i].ItemArray.Length - 3];
                    }
                    else
                    {
                        values[1] = dt.Rows[i][0];
                    }
                    values[2] = dt.Rows[i][2];
                    values[3] = dt.Rows[i][3];
                    values[4] = dt.Rows[i][4];
                    values[5] = dt.Rows[i][5];
                    values[6] = dt.Rows[i][6]; 
                    table2.Rows.Add(values);
                }
            }
            this.G.DataSource = table2;
        }

        int ld;
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
        
        protected void G_InitializeLayout(object sender, LayoutEventArgs e)
        {

            this.ld = int.Parse(this.Type3.SelectedValue);
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.AllowColSizingDefault = AllowSizing.Fixed;
            e.Layout.AllowColumnMovingDefault = AllowColumnMoving.None;
            e.Layout.AllowSortingDefault = AllowSorting.No;
            for (int i = 3; i < e.Layout.Bands[0].Columns.Count; i++)
            {
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "N2");
                e.Layout.Bands[0].Columns[i].Header.Style.HorizontalAlign = HorizontalAlign.Center;
            }
            e.Layout.Bands[0].Columns[1].Width = (int)(this.screen_width * 0.50);
            e.Layout.Bands[0].Columns[2].Width = (int)(this.screen_width * 0.08);
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.Bands[0].Columns[2].CellStyle.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[1].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[2].CellStyle.Wrap = true;
            this.headerLayout.AddCell("Номер в докладе");
            this.headerLayout.AddCell("Показатели");
            this.headerLayout.AddCell("Единица измерения");
            this.headerLayout.AddCell("Отчетный год").AddCell(this.ld.ToString());
            GridHeaderCell cell = this.headerLayout.AddCell("Плановый период");
            cell.AddCell((this.ld + 1).ToString());
            cell.AddCell((this.ld + 2).ToString());
            cell.AddCell((this.ld + 3).ToString());
            this.headerLayout.ApplyHeaderInfo();
            for (int j = 3; j < this.G.Columns.Count; j++)
            {
                this.G.Columns[j].Width = (int)(this.screen_width * 0.07);
            }
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[0].Width = 75;
        }

        protected void G_InitializeRow(object sender, RowEventArgs e)
        {
            if (e.Row.Cells[0].Text.Length>0)
            if (e.Row.Cells[0].Text[0] == '_')
            {
                e.Row.Cells[0].Text = e.Row.Cells[0].Text.Remove(0, 1);
                e.Row.Cells[0].ColSpan = 7;
                e.Row.Cells[0].Style.Font.Bold = true;
                e.Row.Cells[0].Style.HorizontalAlign = HorizontalAlign.Center;
            }
            if (e.Row.Cells[2].Text == "да/нет")
            {
                if (Convert.ToDouble(e.Row.Cells[3].Value) != 0.0)
                {
                    e.Row.Cells[3].Text = "Да";
                }
                else
                {
                    e.Row.Cells[3].Text = "Нет";
                }
                if (Convert.ToDouble(e.Row.Cells[4].Value) != 0.0)
                {
                    e.Row.Cells[4].Text = "Да";
                }
                else
                {
                    e.Row.Cells[4].Text = "Нет";
                }
                if (Convert.ToDouble(e.Row.Cells[5].Value) != 0.0)
                {
                    e.Row.Cells[5].Text = "Да";
                }
                else
                {
                    e.Row.Cells[5].Text = "Нет";
                }
                if (Convert.ToDouble(e.Row.Cells[6].Value) != 0.0)
                {
                    e.Row.Cells[6].Text = "Да";
                }
                else
                {
                    e.Row.Cells[6].Text = "Нет";
                }
            }
        }

        protected void Type1_Load(object sender, EventArgs e)
        {

        }
        private void ExcelExporter_BeginExport(object sender, BeginExportEventArgs e)
        {
            e.CurrentWorksheet.Rows[0].Cells[0].Value = Label1.Text;
            e.CurrentWorksheet.Rows[0].Cells[0].CellFormat.Font.Bold = Infragistics.Documents.Excel.ExcelDefaultableBoolean.True;
            
        }

       
        

        private int offset = 0;
        protected double GetNumber(string s)
        {
            try
            {
                if (!String.IsNullOrEmpty(s))
                {
                    return double.Parse(s);
                }
                else
                {
                    return 0;
                }
            }
            catch { return 0; }
        }
        private void ExcelExporter_HeaderCellExporting(object sender, HeaderCellExportingEventArgs e)
        {
            UltraGridColumn col = G.DisplayLayout.Bands[0].Columns[e.CurrentColumnIndex + offset];
            e.HeaderText = e.HeaderText.Replace("&quot;", "\"");
            if (col.Hidden)
            {
                offset++;
            }
            
        }


        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            this.ReportExcelExporter1.WorksheetTitle = this.Hederglobal.Text;
            this.ReportExcelExporter1.WorksheetSubTitle = this.Label2.Text;
            Workbook workbook = new Workbook();
            Worksheet sheet = workbook.Worksheets.Add("Таблица");
            this.SetExportParametrs();
            this.ReportExcelExporter1.Export(this.headerLayout, sheet, 3);
        }
        private void ExcelExporter_EndExport(object sender, EndExportEventArgs e)
        {
            e.Workbook.Worksheets["Таблица"].Rows[1].Height = 500;
        }
        private void loadFromXML()
        {
          
        }

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            this.ReportPDFExporter1.PageTitle = this.Hederglobal.Text;
            this.ReportPDFExporter1.PageSubTitle = this.Label2.Text;
            ISection section = new Infragistics.Documents.Reports.Report.Report().AddSection();
            this.SetExportParametrs();
            this.ReportPDFExporter1.Export(this.headerLayout, this.Label1.Text, section);
        }

        private void SetExportParametrs()
        {
            for (int i = 0; i < this.G.Rows.Count; i++)
            {
                if (this.G.Rows[i].Cells[0].Style.Font.Bold && (this.G.Rows[i].Cells[0].Text != ""))
                {
                    this.G.Rows[i].Cells[1].Text = this.G.Rows[i].Cells[0].Text;
                    this.G.Rows[i].Cells[1].Style.Font.Bold = true;
                    this.G.Rows[i].Cells[0].Text = "";
                }
            }
        }



    }

}
