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



//////////////////////////////////////////Õ≈«¿¡€“‹!!! ¬≈–Õ”“‹!!! ÕŒ–Ã¿À‹Õ€… ’≈“ ¬≈–»“≈ —“!!!!
namespace Krista.FM.Server.Dashboards.reports.MO.MO_0002._0000
{
    public partial class _default : CustomReportPage
    {
        public DataTable GetDSForChart(string sql)
        {
            DataTable dt = new DataTable();
            ///‰‡‰‡‰ ËÏÂÌÓ ÚÛÚ‡
            string s = DataProvider.GetQueryText(sql);
            DataProvidersFactory.PrimaryMASDataProvider.PopulateDataTableForChart(DataProvidersFactory.PrimaryMASDataProvider.GetCellset(s), dt, "œÓÍ‡Á‡ÚÂÎ¸");
            return dt;
        }

        CustomParam SelectItemGrid;
        CustomParam P1;
        CustomParam P2;
        CustomParam P3;
        CustomParam region;
        CustomParam P4;

        protected override void Page_PreLoad(object sender, System.EventArgs e)
        {
            base.Page_PreLoad(sender, e);
            SelectItemGrid = UserParams.CustomParam("Param");
            P1 = UserParams.CustomParam("p1");
            P2 = UserParams.CustomParam("p2");
            P3 = UserParams.CustomParam("p3");
            P4 = UserParams.CustomParam("p4");
            region = UserParams.CustomParam("region");
            G.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 15);
            G.Height = Unit.Empty;
            //##################################################
            UltraGridExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            UltraGridExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
            UltraGridExporter1.ExcelExporter.EndExport += new EndExportEventHandler(ExcelExporter_EndExport);
            UltraGridExporter1.ExcelExporter.HeaderCellExporting += new HeaderCellExportingEventHandler(ExcelExporter_HeaderCellExporting);
            UltraGridExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
            UltraGridExporter1.PdfExporter.BeginExport += new EventHandler
                    <Infragistics.WebUI.UltraWebGrid.DocumentExport.DocumentExportEventArgs>(PdfExporter_BeginExport);
            UltraGridExporter1.PdfExporter.EndExport += new EventHandler
                <Infragistics.WebUI.UltraWebGrid.DocumentExport.EndExportEventArgs>(PdfExporter_EndExport);
            //##################################################

            UltraGridExporter1.PdfExportButton.Visible = 1 == 2;

        }

        protected Dictionary<string, int> GetParam(string q, string param)
        {
            Dictionary<string, int> areas = new Dictionary<string, int>();
            SelectItemGrid.Value = param;
            DataTable dt = GetDSForChart(q);

            for (int i = dt.Rows.Count - 1; i > 0; i--)
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
                //    RegionSettingsHelper.Instance.SetWorkingRegion("Gubkinski");
                base.Page_Load(sender, e);
                region.Value = RegionSettingsHelper.Instance.RegionBaseDimension;

                if (!Page.IsPostBack)
                {
                    Type3.FillDictionaryValues(GetParam("params", RegionSettingsHelper.Instance.GetPropertyValue("param3")));
                    Type3.Set—heckedState(RegionSettingsHelper.Instance.GetPropertyValue("param_default_3"), 1 == 1);
                }
                P3.Value = RegionSettingsHelper.Instance.GetPropertyValue("param3") + ".[" + Type3.SelectedValue + "]";
                //Label1.Text = string.Format(RegionSettingsHelper.Instance.GetPropertyValue("TitleGrid"), Type3.SelectedValue);
                G.DataBind();
                G.Bands[0].Columns[G.Bands[0].Columns.Count - 1].Move(1);
                Hederglobal.Text = string.Format(RegionSettingsHelper.Instance.GetPropertyValue("TitlePage"), Type3.SelectedValue);
                Page.Title = Hederglobal.Text;
                Label2.Text = RegionSettingsHelper.Instance.GetPropertyValue("Page");
                if (!Page.IsPostBack) { SelectItemGrid.Value = RegionSettingsHelper.Instance.GetPropertyValue("gridItems_" + (1).ToString()); }
                SelectItemGrid = ForMarks.SetMarks(SelectItemGrid, ForMarks.Getmarks("gridItems_"), 1 == 1);
            }
            catch { }

            //RegionSettingsHelper.Instance.SetWorkingRegion("Novoorsk");

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
                // 3,14*13,249211356466876971608832807571 = 42
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

            G.Columns.Clear();
            G.Bands[0].HeaderLayout.Clear();
            G.Rows.Clear();
            G.Bands.Clear();
            string[] al = RegionSettingsHelper.Instance.GetPropertyValue("gridItem").Split('\n');
            SelectItemGrid.Value = al[0].ToString().Split(';')[1];
            DataTable dt = GetDSForChart("G");
            dt.Rows[0].Delete();
            dt.Rows[0].Delete();
            dt.Columns.Add("“ÂÏÔ ÓÒÚ‡ Í ÔÂ‰˚‰Û˘ÂÏÛ „Ó‰Û", dt.Columns[1].DataType);
            for (int i = 1; i < al.Length - 1; i++)
            {
                string[] m = al[i].ToString().Split(';');
                try
                {
                    SelectItemGrid.Value = m[1];
                }
                catch
                {
                    SelectItemGrid.Value = string.Empty;
                }
                object[] o = new object[dt.Columns.Count];
                o[0] = m[0];
                if (string.IsNullOrEmpty((SelectItemGrid.Value)))
                {
                    o[0] = "0" + o[0].ToString();
                }
                else
                {
                    try
                    {
                        o[0] = "1" + o[0].ToString();
                        DataTable dt1 = GetDSForChart("G");

                        {

                            for (int j = 1; o.Length - 1 > j; j++)
                            {

                                if ((dt1.Rows[1].ItemArray[j] is System.Decimal) & (j != o.Length - 2) & (j != o.Length - 3))
                                { o[j] = dt1.Rows[1].ItemArray[j]; }
                                else
                                { o[j] = (System.Decimal)(0); }
                                if ((j == o.Length - 2) || (j != o.Length - 2))
                                { o[j] = dt1.Rows[1].ItemArray[j]; }
                            }
                            try
                            {                                
                                o[o.Length - 1] = 100 * ((Decimal)(dt1.Rows[1].ItemArray[1]) / (Decimal)(dt1.Rows[0].ItemArray[1]));   
                                //String.Format(o[o.Length - 1].ToString()+"|||||"+(o[2].ToString()));
                            }
                            catch { }
                        }
                    }
                    catch { }
                }
                dt.Rows.Add(o);

            }
            G.DataSource = dt;
        }
        bool[] hidenColumns = new bool[0];
        protected void G_InitializeRow(object sender, RowEventArgs e)
        {
            bool reverce = 1 == 1;

            if (e.Row.Cells[0].Text[0] == '0')
            { e.Row.Cells[0].ColSpan = 7; }
            else
            {
                reverce = "1" != e.Row.Cells[e.Row.Cells.Count - 2].Text;

            }
            e.Row.Cells[0].Text = e.Row.Cells[0].Text.Remove(0, 1);
            try
            {
                String.Format(e.Row.Cells[2].Value.ToString() + "|" + e.Row.Cells[2].Value.GetType().ToString());
                float val = float.Parse(e.Row.Cells[2].Value.ToString());
                String.Format(val.ToString());
                if (val != 100)
                {
                    if (((val < 100)))
                    {
                        //e.Row.Cells[3].Text = Math.Round(float.Parse(e.Row.Cells[2].Value.ToString()), 3).ToString();
                        e.Row.Cells[2].Text = (reverce ? "<img style=\"FLOAT: left;\" src=\"../../../../images/arrowRedDownBB.png\">" : "<img style=\"FLOAT: left;\" src=\"../../../../images/arrowGreenDownBB.png\">") + Math.Round(float.Parse(e.Row.Cells[2].Value.ToString()), 3).ToString("### ##0.00") + "%";

                    }
                    else
                    {
                        e.Row.Cells[2].Text = (reverce ? "<img style=\"FLOAT: left;\" src=\"../../../../images/arrowGreenUpBB.png\">" : "<img style=\"FLOAT: left;\" src=\"../../../../images/arrowRedUpBB.png\">") + Math.Round(float.Parse(e.Row.Cells[2].Value.ToString()), 3).ToString("### ##0.00") + "%";
                    }
                }
                else
                {


                    //e.Row.Cells[2].Text = e.Row.Cells[2].Text.Split(',', '.')[0];
                    //try
                    //{
                    //    e.Row.Cells[2].Text += "," + e.Row.Cells[2].Text.Split(',', '.')[1][0];
                    //}
                    //catch
                    //{
                    //    e.Row.Cells[2].Text += ",0";
                    //}
                    //try
                    //{
                    //    e.Row.Cells[2].Text += e.Row.Cells[2].Text.Split(',', '.')[0][1];
                    //}
                    //catch
                    //{
                    //    e.Row.Cells[2].Text += "0";
                    //}
                    e.Row.Cells[2].Text = string.Format("{0:### ### ##0.00}", float.Parse(e.Row.Cells[2].Value.ToString()));
                    e.Row.Cells[2].Text += "%";
                }
                


            }
            catch { }
            try
            {
                e.Row.Cells[1].Text = string.Format("{0:### ### ##0.00}", float.Parse(e.Row.Cells[1].Value.ToString()));
                e.Row.Cells[3].Text = string.Format("{0:### ### ##0.00}", float.Parse(e.Row.Cells[3].Value.ToString()));
                e.Row.Cells[4].Text = string.Format("{0:### ### ##0.00}", float.Parse(e.Row.Cells[4].Value.ToString()));
                e.Row.Cells[5].Text = string.Format("{0:### ### ##0.00}", float.Parse(e.Row.Cells[5].Value.ToString()));
                
            }
            catch { }

        }


        protected void ForCrossJoin(LayoutEventArgs e, int span)
        {
        }



        protected void G_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.Bands[0].Columns[e.Layout.Bands[0].Columns.Count - 1].Move(2);
            e.Layout.Bands[0].Columns[0].Header.Title = "";

            e.Layout.Bands[0].Columns[0].Width = 600 - 13;
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = 1 == 1;
            
            for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i++)
            {
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "### ### ### ##0.00");
                e.Layout.Bands[0].Columns[i].CellStyle.Wrap = 1 == 1;
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(6 * 13 + 13 / 3 - 1);
                e.Layout.Bands[0].Columns[i].Header.Style.Wrap = 1 == 1;
                e.Layout.Bands[0].Columns[i].Header.Style.HorizontalAlign = HorizontalAlign.Center;
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;

                Label2.Text += e.Layout.Bands[0].Columns[i].Header.Caption+" ,";
            }
            e.Layout.Bands[0].Columns[2].Width = (int)((6 * 13 + 13 / 3 - 1) * 1.13);

            e.Layout.Bands[0].Columns[e.Layout.Bands[0].Columns.Count - 2].Hidden = 1 == 1;
            e.Layout.Bands[0].Columns[e.Layout.Bands[0].Columns.Count - 1].CellStyle.HorizontalAlign = HorizontalAlign.Center;
           
            e.Layout.AllowSortingDefault = AllowSorting.No;

        }

        protected void C_DataBinding(object sender, EventArgs e)
        {
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
            //e.CurrentWorksheet.Rows[0].Cells[0].Value = Label1.Text;
            e.CurrentWorksheet.Rows[0].Cells[0].CellFormat.Font.Bold = Infragistics.Documents.Excel.ExcelDefaultableBoolean.True;
        }

        private void ExcelExporter_EndExport(object sender, EndExportEventArgs e)
        {
            for (int j = 0; j < G.Rows.Count + 1; j++)
            {
                for (int i = 0; i < G.Columns.Count; i++)
                {
                    try
                    {

                        e.CurrentWorksheet.Rows[j].Cells[i].Value = "";
                        string o_O = G.Rows[j].Cells[i].Text.Replace("<b>", "").Replace("</b>", "")
                            .Replace("<img style=\"FLOAT: left;\" src=\"../../../../images/arrowRedDownBB.png\">", "")
                            .Replace("<img style=\"FLOAT: left;\" src=\"../../../../images/arrowGreenDownBB.png\">", "")
                            .Replace("<img style=\"FLOAT: left;\" src=\"../../../../images/arrowGreenUpBB.png\">", "")
                            .Replace("<img style=\"FLOAT: left;\" src=\"../../../../images/arrowRedUpBB.png\">", "");
                        //o_O =  o_O.Split('%')[0];
                        e.CurrentWorksheet.Rows[j].Cells[i].Value = o_O;

                    }
                    catch { }
                }
                try
                {
                    //e.CurrentWorksheet.Rows[j].Cells[6].Value = G.Rows[j].Cells[7].Text;
                    e.CurrentWorksheet.Rows[j].Cells[7].Value = "";
                }
                catch { }
            }
            for (int i = 0; i < G.Columns.Count; i++)
            {
                e.CurrentWorksheet.Rows[0].Cells[i].Value = G.Columns[i].Header.Caption;
                e.CurrentWorksheet.Columns[i].Width = 5000;
            }
            e.CurrentWorksheet.Columns[0].Width = 20000;
            e.CurrentWorksheet.Rows[0].Cells[6].Value = e.CurrentWorksheet.Rows[0].Cells[6].Value;
            e.CurrentWorksheet.Rows[0].Cells[7].Value = "";
            e.CurrentWorksheet.Columns[8].Width = 6;
            //e.CurrentWorksheet.Rows[0].Cells[ i].Value = G.Columns[i].Header.Caption;
            //≈ÌÓÚ˚! ‚ËÌÓ‚‡Ú˚ ‚Ó ‚Ò∏Ï!
        }

        private int offset = 0;

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
            for (int j = 0; j < G.Rows.Count; j++)
            {
            }

            UltraGridExporter1.ExcelExporter.DownloadName = "report.xls";
            UltraGridExporter1.ExcelExporter.Export(G);
        }


        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            UltraGridExporter1.PdfExporter.DownloadName = "report.pdf";
            UltraGridExporter1.PdfExporter.Export(G);
        }

        //#########################################################


        private void PdfExporter_BeginExport(object sender, Infragistics.WebUI.UltraWebGrid.DocumentExport.DocumentExportEventArgs e)
        {
            IText title = e.Section.AddText();
            Font font = new Font("Verdana", 16);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.Style.Font.Bold = true;
            //title.AddContent(Label1.Text);

            title = e.Section.AddText();
            font = new Font("Verdana", 14);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
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
        }



    }
}
/////////////////////////////////////////////////////////////////////////////////////
////////////////////####################