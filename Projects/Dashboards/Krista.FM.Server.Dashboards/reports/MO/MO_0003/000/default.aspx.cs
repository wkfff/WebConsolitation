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
//using Krista.FM.Server.Dashboards.reports.MO.MO_0001;
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


namespace Krista.FM.Server.Dashboards.MO_0003._000
{
    public partial class _default : CustomReportPage
    {
        public DataTable GetDSForChart(string sql)
        {
            DataTable dt = new DataTable();
            string s = DataProvider.GetQueryText(sql);
            DataProvidersFactory.PrimaryMASDataProvider.PopulateDataTableForChart(DataProvidersFactory.PrimaryMASDataProvider.GetCellset(s), dt, "Показатль");
            return dt;
        }

        CustomParam SelectItemGrid;
        CustomParam P1;
        CustomParam P2;
        CustomParam P3;
        CustomParam region;
        CustomParam P4;

        string BN = "IE";
 

        protected override void Page_PreLoad(object sender, System.EventArgs e)
        {
            base.Page_PreLoad(sender, e);
            System.Web.HttpBrowserCapabilities myBrowserCaps = Request.Browser;
            BN = ((System.Web.Configuration.HttpCapabilitiesBase)myBrowserCaps).Browser.ToUpper();
            SelectItemGrid = UserParams.CustomParam("Param");
            P1 = UserParams.CustomParam("p1");
            P2 = UserParams.CustomParam("p2");
            P3 = UserParams.CustomParam("p3");
            P4 = UserParams.CustomParam("p4");
            region = UserParams.CustomParam("region");
            G.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth);
            UltraGridExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
//UltraGridExporter1.ultraWebGridExcelExporter_RowExporting+=new UltraGridExporter1.
            UltraGridExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
            UltraGridExporter1.ExcelExporter.EndExport += new EndExportEventHandler(ExcelExporter_EndExport);
            UltraGridExporter1.ExcelExporter.HeaderCellExporting += new HeaderCellExportingEventHandler(ExcelExporter_HeaderCellExporting);            
            //UltraGridExporter1.ultraWebGridExcelExporter_RowExporting += new UltraGridExporter1.ultraWebGridExcelExporter_RowExporting(ExcelExporter_HeaderRowExporting);
            //UltraGridExporter1.MultiHeader = true;

            UltraGridExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
            UltraGridExporter1.PdfExporter.BeginExport += new EventHandler
                    <Infragistics.WebUI.UltraWebGrid.DocumentExport.DocumentExportEventArgs>(PdfExporter_BeginExport);
            UltraGridExporter1.PdfExporter.EndExport += new EventHandler
                <Infragistics.WebUI.UltraWebGrid.DocumentExport.EndExportEventArgs>(PdfExporter_EndExport);
//G.Height = CustomReportConst.minScreenHeight/3
                ;

        }

        protected Dictionary<string, int> GetParam(string q, string param)
        {
            Dictionary<string, int> areas = new Dictionary<string, int>();
            SelectItemGrid.Value = param;
            DataTable dt = GetDSForChart(q);

            for (int i = dt.Rows.Count - 1; i > -1; i--)
            {
                areas.Add(dt.Rows[i].ItemArray[0].ToString(), 0);
            }

            return areas;

        }


        string baseRegion;
        protected override void Page_Load(object sender, EventArgs e)
        {
            try
            {
                base.Page_Load(sender, e);
                RegionSettingsHelper.Instance.SetWorkingRegion("Gubkinski");
                region.Value = RegionSettingsHelper.Instance.RegionBaseDimension;

                if (!Page.IsPostBack)
                {
                    DataTable dt = GetDSForChart("LD");
                    int ld = int.Parse(dt.Rows[0].ItemArray[1].ToString());
                    P1.Value = ld.ToString();
                    P2.Value = (ld+1).ToString();
                    P3.Value = (ld + 2).ToString();

                }

                

             //   Label1.Text = string.Format(RegionSettingsHelper.Instance.GetPropertyValue("TitleGrid"));

                //G.DataBind();
                //G.Rows[0].Delete();
                
                Hederglobal.Text = RegionSettingsHelper.Instance.GetPropertyValue("TitlePage");

                Page.Title = Hederglobal.Text;
                if (!Page.IsPostBack) { SelectItemGrid.Value = RegionSettingsHelper.Instance.GetPropertyValue("gridItems_" + (1).ToString()); }
                SelectItemGrid = ForMarks.SetMarks(SelectItemGrid, ForMarks.Getmarks("gridItems_"), 1 == 1);

                
            }
            catch { }
            G.DataBind();
            G.Columns[1].Hidden = 1 == 1;
            //G.Columns[1].Hidden = 1 == 1;
            try
            {
                #region bred
                for (int i = 0; i < G.Rows.Count; i++)
                {
                    try
                    {                      

                        if (Math.Truncate(double.Parse(G.Rows[i].Cells[4].Value.ToString())) >= 100)
                        {
                            G.Rows[i].Cells[4].Text = string.Format("<img src=\"../../../../images/starYellow.png\">") + Math.Truncate(double.Parse(G.Rows[i].Cells[4].Value.ToString())).ToString();
                        }
                        if (Math.Truncate(double.Parse(G.Rows[i].Cells[4].Value.ToString())) < 100)
                        {
                            G.Rows[i].Cells[4].Text = string.Format("<img src=\"../../../../images/starGray.png\">") + Math.Truncate(double.Parse(G.Rows[i].Cells[4].Value.ToString())).ToString();
                        }
                    }
                    catch { }

                    try
                    {


                        if (Math.Truncate(double.Parse(G.Rows[i].Cells[7].Value.ToString())) >= 100)
                        {
                            G.Rows[i].Cells[7].Text = string.Format("<img src=\"../../../../images/starYellow.png\">") + Math.Truncate(double.Parse(G.Rows[i].Cells[7].Value.ToString())).ToString();
                        }
                        if (Math.Truncate(double.Parse(G.Rows[i].Cells[7].Value.ToString())) < 100)
                        {
                            G.Rows[i].Cells[7].Text = string.Format("<img src=\"../../../../images/starGray.png\">") + Math.Truncate(double.Parse(G.Rows[i].Cells[7].Value.ToString())).ToString();
                        }
                    }
                    catch { }

                    try
                    {                        
                        if (Math.Truncate(double.Parse(G.Rows[i].Cells[9].Value.ToString())) >= 100)
                        {
                            G.Rows[i].Cells[9].Text = string.Format("<img src=\"../../../../images/starYellow.png\">") + Math.Truncate(double.Parse(G.Rows[i].Cells[9].Value.ToString())).ToString();
                        }
                        if (Math.Truncate(double.Parse(G.Rows[i].Cells[9].Value.ToString())) < 100)
                        {
                            G.Rows[i].Cells[9].Text = string.Format("<img src=\"../../../../images/starGray.png\">") + Math.Truncate(double.Parse(G.Rows[i].Cells[9].Value.ToString())).ToString();
                        }
                    }
                    catch { }
                    try
                    {
                       
                    }
                    catch { }
                    try
                    {
                       
                    }
                    catch { }
                    // G.Rows[i].Cells[8].Text = G.Rows[i].Cells[8].Text.Remove(G.Rows[i].Cells[8].Text.IndexOf(',') + 3);

                }
                #endregion
                Double Coef = 1;
                if (BN == "IE") { Coef = 1; }
                if (BN == "FIREFOX") { Coef = 1.1; }
                if (BN == "APPLEMAC-SAFARI") { Coef = 1; };

                G.Height = CRHelper.GetGridHeight(G.Rows.Count * 31.5 * Coef + 50);
            }
            catch { };
            

        }

        static public class ForMarks
        {

            public static ArrayList Getmarks(string prefix)
            {
                ArrayList AL = new ArrayList();

                string CurMarks = RegionSettingsHelper.Instance.GetPropertyValue(prefix + "1");
                int i = 2;
                while (!string.IsNullOrEmpty(CurMarks))
                {
                    AL.Add(CurMarks.ToString());

                    CurMarks = RegionSettingsHelper.Instance.GetPropertyValue(prefix + i.ToString());

                    i++;
                }

                return AL;
            }

            public static CustomParam SetMarks(CustomParam param, ArrayList AL, params bool[] clearParam)
            {
                if (clearParam.Length > 0 && clearParam[0]) { param.Value = ""; }
                int i;
                for (i = 0; i < AL.Count - 1; i++)
                {
                    param.Value += AL[i].ToString() + ",";
                }
                param.Value += AL[i].ToString();

                return param;
            }


        }


        protected void G_DataBinding(object sender, System.EventArgs e)
        {
            try
            {
                DataTable dt = new DataTable();
                CellSet Cs = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(DataProvider.GetQueryText("G"));
                dt.Columns.Add("Показатель");
                for (int i = 0; i < Cs.Axes[0].Positions.Count; i++)
                {
                    dt.Columns.Add(Cs.Axes[0].Positions[i].Members[0].Caption);
                }


                for (int i = 0; i < Cs.Cells.Count / Cs.Axes[0].Positions.Count; i ++)
                {
                    object[] o = new object[Cs.Axes[0].Positions.Count + 1];
                    string lev = "";
                    if ((Cs.Axes[1].Positions[i].Members[0].LevelName.Split('.')[Cs.Axes[1].Positions[i].Members[0].LevelName.Split('.').Length - 1]) == "[Направление]")
                    {
                        lev = "0";
                    }
                    if ((Cs.Axes[1].Positions[i].Members[0].LevelName.Split('.')[Cs.Axes[1].Positions[i].Members[0].LevelName.Split('.').Length - 1]) == "[Показатель 1]")
                    {
                        lev = "1";
                    }
                    if ((Cs.Axes[1].Positions[i].Members[0].LevelName.Split('.')[Cs.Axes[1].Positions[i].Members[0].LevelName.Split('.').Length - 1]) == "[Показатель 2]")
                    {
                        lev = "2";
                    }
                    if ((Cs.Axes[1].Positions[i].Members[0].LevelName.Split('.')[Cs.Axes[1].Positions[i].Members[0].LevelName.Split('.').Length - 1]) == "[Показатель 3]")
                    {
                        lev = "3";
                    }
                    if ((Cs.Axes[1].Positions[i].Members[0].LevelName.Split('.')[Cs.Axes[1].Positions[i].Members[0].LevelName.Split('.').Length - 1]) == "[Показатель 4]")
                    {
                        lev = "4";
                    }
                    o[0] = lev + Cs.Axes[1].Positions[i].Members[0].Caption;
                    //o[0] = Cs.Axes[1].Positions[i].Members[0].LevelName + Cs.Axes[1].Positions[i].Members[0].Caption;
                    try
                    {
                        for (int j = 1; j < o.Length; j++)
                        {
                            o[j] = Cs.Cells[Cs.Axes[0].Positions.Count * i + j - 1].Value;
                        }

                        dt.Rows.Add(o);
                        //Label1.Text += "1";
                    }
                    catch { }

                }
                dt.Rows[0].Delete();
                G.DataSource = dt;
            }
            catch { }
            //G.DataSource = GetDSForChart("G");

        }
        bool[] hidenColumns = new bool[0];
        protected void G_InitializeRow(object sender, RowEventArgs e)
        {
            DataTable dt = (DataTable)(G.DataSource);
            int mas = 2;
            if (e.Row.Cells[0].Text[0] == '0')
            {
                e.Row.Cells[0].Text = e.Row.Cells[0].Text.Replace("0", "");
                
                e.Row.Cells[0].Style.Font.Bold = true;
                e.Row.Cells[0].Style.Font.Italic = true;
                try
                {
                    if (dt.Rows[e.Row.Index + 1].ItemArray[0].ToString()[0] == '1')
                    {
                        e.Row.Cells[0].ColSpan = G.Columns.Count;
                        //e.Row.Cells[0].
                        for (int i = 1; i < e.Row.Cells.Count; i++)
                        {
                            e.Row.Cells[i].ColSpan = 0;
                        }
                    }
                    else
                    { e.Row.Cells[0].Text += ", " + e.Row.Cells[1].Text.ToLower(); }
                }
                catch { e.Row.Cells[0].Text += ", " + e.Row.Cells[1].Text.ToLower(); }

            }

            if (e.Row.Cells[0].Text[0] == '1')
            {
                e.Row.Cells[0].Text =  e.Row.Cells[0].Text.Replace("1", "");
                e.Row.Cells[0].Style.Padding.Left = 4*mas;
                
                e.Row.Cells[0].Style.Font.Bold = true;
                try
                {
                    if (dt.Rows[e.Row.Index + 1].ItemArray[0].ToString()[0] == '2')
                    {
                        e.Row.Cells[0].ColSpan = G.Columns.Count;
                        //e.Row.Cells[0].
                        for (int i = 1; i < e.Row.Cells.Count; i++)
                        {
                            e.Row.Cells[i].ColSpan = 0;
                        }
                    }
                    else
                    { e.Row.Cells[0].Text += ", " + e.Row.Cells[1].Text.ToLower(); }
                }
                catch { e.Row.Cells[0].Text += ", " + e.Row.Cells[1].Text.ToLower(); }

            }
            if (e.Row.Cells[0].Text[0] == '2')
            {
                e.Row.Cells[0].Text = e.Row.Cells[0].Text.Replace("2", "");
                e.Row.Cells[0].Style.Font.Italic = true;//FontStyle.Regular;
                e.Row.Cells[0].Style.Padding.Left = 8 * mas;
                try
                {
                    if (dt.Rows[e.Row.Index + 1].ItemArray[0].ToString()[0] == '3')
                    {
                        e.Row.Cells[0].ColSpan = G.Columns.Count;
                        //e.Row.Cells[0].
                        for (int i = 1; i < e.Row.Cells.Count; i++)
                        {
                            e.Row.Cells[i].ColSpan = 0;
                        }
                    }
                    else
                    { e.Row.Cells[0].Text += ", " + e.Row.Cells[1].Text.ToLower(); }
                }
                catch { e.Row.Cells[0].Text += ", " + e.Row.Cells[1].Text.ToLower(); }

            }
            if (e.Row.Cells[0].Text[0] == '3')
            {
                e.Row.Cells[0].Text = e.Row.Cells[0].Text.Replace("3", "");
                //e.Row.Cells[0].Style.Font.Strikeout = true; //= FontStyle.Strikeout;
                e.Row.Cells[0].Style.Padding.Left = 12 * mas;
                try
                {
                    if (dt.Rows[e.Row.Index + 1].ItemArray[0].ToString()[0] == '4')
                    {
                        e.Row.Cells[0].ColSpan = G.Columns.Count;
                        //e.Row.Cells[0].
                        for (int i = 1; i < e.Row.Cells.Count; i++)
                        {
                            e.Row.Cells[i].ColSpan = 0;
                        }
                    }
                    else
                    { e.Row.Cells[0].Text += ", " + e.Row.Cells[1].Text.ToLower(); }
                }
                catch { e.Row.Cells[0].Text += ", " + e.Row.Cells[1].Text.ToLower(); }

            }
            if (e.Row.Cells[0].Text[0] == '4')
            {
                e.Row.Cells[0].Text = e.Row.Cells[0].Text.Replace("4", "");
                e.Row.Cells[0].Style.Font.Underline = 1==1;// = FontStyle.Underline;
                e.Row.Cells[0].Style.Padding.Left = 16 * mas;
                e.Row.Cells[0].RowSpan = G.Columns.Count;
                e.Row.Cells[0].Text += ", " + e.Row.Cells[1].Text.ToLower();
            }

        }


        protected void ForCrossJoin(LayoutEventArgs e, int span)
        { 
        }

        


        protected void G_InitializeLayout(object sender, LayoutEventArgs e)
        {
            //e.Layout.Bands[0].Columns[e.Layout.Bands[0].Columns.Count - 1].Move(2);
            Double Coef = 1;
            if (BN == "IE") { Coef = 0.99; }
            if (BN == "FIREFOX") { Coef = 1; }
            if (BN == "APPLEMAC-SAFARI") { Coef = 0.99; };
            
            
            e.Layout.Bands[0].Columns[0].Header.Title = "";

            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(300);
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = 1 == 1;
            for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i++)
            {
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "### ### ### ##0.00");
                e.Layout.Bands[0].Columns[i].CellStyle.Wrap = 1 == 1;
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(107*Coef);
                e.Layout.Bands[0].Columns[i].Header.Style.Wrap = 1 == 1;
            }
            try
            {
                //e.Layout.Bands[0].Columns[3].Width = CRHelper.GetColumnWidth(130);
                //e.Layout.Bands[0].Columns[6].Width = CRHelper.GetColumnWidth(130);
                //e.Layout.Bands[0].Columns[8].Width = CRHelper.GetColumnWidth(130);
            }
            catch { }
            //ForCrossJoin(e, 2);
            //for (int i = 0; i < (G.Columns.Count - 1) / 2; i++)
            //{
            //    CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i * 2 + 1], "### ### ### ##0.000 0## ###");
            //}

        }

        protected void C_DataBinding(object sender, EventArgs e)
        {
            //P4.Value = "--";
            //C.DataSource = GetDSForChart("G2");
            //P4.Value = "";
        }

        protected void G_ActiveRowChange(object sender, RowEventArgs e)
        {
            SelectItemGrid.Value = RegionSettingsHelper.Instance.GetPropertyValue("gridItems_" + (e.Row.Index + 1).ToString());

        }

        protected void C_InvalidDataReceived(object sender, Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventArgs e)
        {
            CRHelper.UltraChartInvalidDataReceived(sender, e);
        }


        private void ExcelExporter_BeginExport(object sender, BeginExportEventArgs e)
        {
            e.CurrentWorksheet.Rows[0].Cells[0].Value = Label1.Text;
            e.CurrentWorksheet.Rows[0].Cells[0].CellFormat.Font.Bold = Infragistics.Documents.Excel.ExcelDefaultableBoolean.True;
           // e.CurrentWorksheet.Rows[1].Cells[0].Value = ComboYear.SelectedValue + " год";
        }

        private void ExcelExporter_EndExport(object sender, EndExportEventArgs e)
        {
            //string formatString = "#,##0.00;[Red]-#,##0.00";
            //for (int i = 1; i < G.Bands[0].Columns.Count; i = i + 1)
            //{
            //    int widthColumn = 100;
            //    e.CurrentWorksheet.Columns[i].CellFormat.FormatString = formatString;
            //    if (i % 2 == 0)
            //        e.CurrentWorksheet.Columns[i].Width = widthColumn / 2 * 37;
            //    else
            //        e.CurrentWorksheet.Columns[i].Width = widthColumn * 3 / 2 * 37;
            //}
            //e.CurrentWorksheet.Columns[0].Width = 150 * 37;

            //e.CurrentWorksheet.Rows[0].Cells[0].Value = "10";// G.Rows[].Cells[i].Value;
            for (int i = 0; i < G.Columns.Count; i++)
            {
                for (int j = 0; j < G.Rows.Count; j++)
                {
             
                    try
                    {
                        int z = G.Rows[j].Cells[i].Text.IndexOf('>');
                        //e.CurrentWorksheet.Rows[0].Cells[0].Value += .ToString();
                    
                    
                    if (z!=-1)
                    {
                    
                            e.CurrentWorksheet.Rows[j].Cells[i].Value = G.Rows[j].Cells[i].Text.Remove(0, z+1);
                    
                    }
                }
                catch { }
                }
            }
            //e.CurrentWorksheet.Rows[4].Hidden = true;
        }
        
        private int offset = 0;

        private void ExcelExporter_HeaderCellExporting(object sender, HeaderCellExportingEventArgs e)
        {//UltraGridExporter1.ultraWebGridExcelExporter_RowExporting = new 
            UltraGridColumn col = G.DisplayLayout.Bands[0].Columns[e.CurrentColumnIndex + offset];
            e.HeaderText = e.HeaderText.Replace("&quot;", "\"");
            //e.HeaderText = col.Header.Key.Split(';')[0];
            if (col.Hidden)
            {
                offset++;
            }
        }

 


        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
//UltraGridExporter1.ExcelExporter.ExcelStartRow = 4;

            for (int j = 0; j < G.Rows.Count; j++)
            {
               // G.Rows[j].Cells[3].Value = G.Rows[j].Cells[3].Value.ToString().Remove(0, G.Rows[j].Cells[3].Value.ToString().IndexOf('>', 10));
                //G.Rows[j].Cells[6].Value;
                //G.Rows[j].Cells[8].Value;
                //e.CurrentWorksheet.Rows[j].Cells[i].CellFormat.
            }

            UltraGridExporter1.ExcelExporter.DownloadName = "report.xls";
            UltraGridExporter1.ExcelExporter.Export(G);
        }


        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            
                

            UltraGridExporter1.PdfExporter.DownloadName = "report.pdf";
            UltraGridExporter1.PdfExporter.Export(G);
        }




        private void PdfExporter_BeginExport(object sender, Infragistics.WebUI.UltraWebGrid.DocumentExport.DocumentExportEventArgs e)
        {
            IText title = e.Section.AddText();
            Font font = new Font("Verdana", 16);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.Style.Font.Bold = true;
            title.AddContent(Label1.Text);

            title = e.Section.AddText();
            font = new Font("Verdana", 14);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            //title.AddContent(ComboYear.SelectedValue + " год");
        }

        private void PdfExporter_EndExport(object sender, Infragistics.WebUI.UltraWebGrid.DocumentExport.EndExportEventArgs e)
        {
            ITable table = e.Section.AddTable();
            ITableRow row = table.AddRow();
            ITableCell cell = row.AddCell();

            IText title = cell.AddText();
            Font font = new Font("Verdana", 12);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.AddContent(" ");

            row = table.AddRow();
            cell = row.AddCell();
            title = cell.AddText();
            font = new Font("Verdana", 12);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            //title.AddContent(chart1_caption.Text.Replace("&quot;", "\""));

         //   Infragistics.Documents.Reports.Graphics.Image img = UltraGridExporter.GetImageFromChart(Chart1);
           // cell.AddImage(img);
            //cell.Width = new FixedWidth((float)Chart1.Width.Value);

        }




    }
}
