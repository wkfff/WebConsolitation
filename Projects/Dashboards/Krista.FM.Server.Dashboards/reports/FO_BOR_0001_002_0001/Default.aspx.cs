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
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Infragistics.UltraChart.Core;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Shared.Styles;
using System.Drawing;
using Infragistics.Documents.Excel;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Infragistics.WebUI.UltraWebGrid;


using Infragistics.Documents.Reports.Report.Band;
using Infragistics.Documents.Reports.Report.Flow;
using Infragistics.Documents.Reports.Report.Grid;
using Infragistics.Documents.Reports.Report.Index;
using Infragistics.Documents.Reports.Report.List;
using Infragistics.Documents.Reports.Report.QuickList;
using Infragistics.Documents.Reports.Report.QuickTable;
using Infragistics.Documents.Reports.Report.QuickText;
using Infragistics.Documents.Reports.Report.Segment;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.Documents.Reports.Report.TOC;
using Infragistics.Documents.Reports.Report.Tree;
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.WebUI.UltraWebGrid.DocumentExport;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Section;
using Infragistics.Documents.Reports.Report.Table;



namespace Krista.FM.Server.Dashboards.FO_BOR_0001_002_0001.Default
{
    public partial class _default : CustomReportPage
    {
        string HederReport = "Общая результативность деятельности ГРБС в {0} году";
        string LastDateFormatString = "{0}";
        string LinkGridFS = " <a href=\"{0}\">>></a>";

        //LastDate
        private CustomParam LD { get { return (UserParams.CustomParam("LD")); } }

        private CustomParam GRBS { get { return (UserParams.CustomParam("GRBS")); } }

        //Генерит словарик с годами для параметра
        System.Collections.Generic.Dictionary<string, int> GenUserParam(string query)
        {
            DataTable dt = GetDS(query);
            System.Collections.Generic.Dictionary<string, int> d = new System.Collections.Generic.Dictionary<string, int>();
            foreach (DataColumn Column in dt.Columns)
            {
                if ((Column.Caption != "LD_") && (Column.Caption != "Fo_BOR"))
                {
                    LD.Value = Column.Caption;
                    d.Add(LD.Value, 0);
                }
            }
            return d;
        }

        //GetDataSource
        protected DataTable GetDS(string q)
        {
            DataTable dt = new DataTable(q);
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText(q), q, dt);
            return dt;


        }


        
        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            G.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth);
            G.Height = Unit.Empty;
            LC.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth/2-5);
            RC.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth / 2 - 5);
            UltraGridExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            UltraGridExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
            UltraGridExporter1.ExcelExporter.EndExport += new EndExportEventHandler(ExcelExporter_EndExport);

            UltraGridExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
            UltraGridExporter1.PdfExporter.EndExport += new EventHandler
         <Infragistics.WebUI.UltraWebGrid.DocumentExport.EndExportEventArgs>(PdfExporter_EndExport);
            //UltraGridExporter1.PdfExporter.BeginExport += new EventHandler<DocumentExportEventArgs>(PdfExporter_BeginExport);

        }

        string BN = "IE";
        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            System.Web.HttpBrowserCapabilities myBrowserCaps = Request.Browser;
            BN = ((System.Web.Configuration.HttpCapabilitiesBase)myBrowserCaps).Browser.ToUpper();
            if (!Page.IsPostBack)
            {
                ComboYear.FillDictionaryValues(GenUserParam("LD_"));
                ComboYear.SetСheckedState(LD.Value, 1 == 1);
                ComboYear.Title = "Период";
                ComboYear.Width = 150;

                comboFo.FillDictionaryValues(GenUserParam("Fo_BOR"));

                
                comboFo.Title = "ГРБС";
                comboFo.Width = 1000;
            }
            string PrevReport = CustomParam.CustomParamFactory("CRepoort").Value;

            //Удалаяяем этот папраметр, шоб в следущий ра
            Session.Remove("CRepoort");
            //если пусто значит не с какого
            if (!string.IsNullOrEmpty(PrevReport))
            {
                comboFo.SetСheckedState(PrevReport.Replace('_', ' '), 1 == 1);
            }

            LD.Value = ComboYear.SelectedValue;
            GRBS.Value = comboFo.SelectedValue;
            try
            {
                Label1.Text = string.Format(HederReport,ComboYear.SelectedValue);
                Page.Title = Label1.Text;
                TG.Text = string.Format("Общие индикаторы деятельности: «{0}»",comboFo.SelectedValue);
                G.DataBind();
            
                LC.DataBind();
                RC.DataBind();
                if ( countNoTopLevel>0) 
                if (G.Rows.Count != 2)
                {
                    object[] o = { "Цели", 0 };
                    G.Rows.Insert(2, new UltraGridRow(o));
                    G.Rows[2].Cells[0].ColSpan = 2;
                    G.Rows[2].Cells[1].Text = "";

                    object[] o2 = { "3. Общая результативность решения тактических задач каждой цели ГРБС " + string.Format(string.Format(LinkGridFS, "../FO_BOR_0001_005_0001/Default.aspx?paramlist=CRepoort={0}"), comboFo.SelectedValue.Replace(' ', '_') + ":" + ComboYear.SelectedValue), 0 };
                    G.Rows.Insert(2, new UltraGridRow(o2));
                    G.Rows[2].Cells[1].Text = "";
                }
            Label3.Text = "Общая результативность достижения целей";
            Label4.Text = "Общая результативность деятельности органа исполнительной власти";
            if (G.Rows.Count == 0)
            {
                throw new Exception();
            }
        }
        catch
        {
            Label4.Text = "";
            Label3.Text = "";
            TG.Text = "";
        }
            
        }




        #region Экпорт в Excel

        private void ExcelExporter_BeginExport(object sender, BeginExportEventArgs e)
        {

        }

        private void ExcelExporter_EndExport(object sender, Infragistics.WebUI.UltraWebGrid.ExcelExport.EndExportEventArgs e)
        {
            foreach (Worksheet sheet in e.Workbook.Worksheets)
            {
                sheet.Columns[0].CellFormat.WrapText = ExcelDefaultableBoolean.True;
                sheet.Rows[0].Cells[0].Value = TG.Text;
                for (int i = 2; i < 1 +G.Rows.Count ; i++)
                {
                    try
                    {
                        sheet.Rows[i].Height = 37 * 15;
                        sheet.Rows[i].Cells[0].Value = sheet.Rows[i].Cells[0].Value.ToString().Split('<')[0];
                        
                    }
                    catch { }
                }

                sheet.Columns[0].Width = 1040 * 37;
                sheet.Columns[1].Width = 120 * 37;
                sheet.Columns[2].Width = 120 * 37;


                sheet.Columns[1].CellFormat.FormatString = "#,##0%";
                sheet.Columns[2].CellFormat.FormatString = "#,##0.0##";
                sheet.Columns[3].CellFormat.FormatString = "#,##0.0##";
                sheet.Columns[4].CellFormat.FormatString = UltraGridExporter.ExelPercentFormat;                

            }
        }

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            Workbook workbook = new Workbook();
            
            Worksheet sheet1 = workbook.Worksheets.Add("Отчёт");

            UltraGridExporter1.ExcelExporter.ExcelStartRow = 2;
            UltraGridExporter1.ExcelExporter.Export(G, sheet1);

        }

        #endregion

        protected void G_DataBinding(object sender, EventArgs e)
        {
            DataTable dt1 = GetDS("G1");
            DataTable dt2 = GetDS("G2");
            DataTable dt = new DataTable();

            dt.Columns.Add("1",typeof(System.String));
            dt.Columns.Add("2",typeof(System.Decimal));

            if (dt1.Rows.Count == 0)
            {
                G.DataSource = null;
                return;
            }
            for (int i = 0; i < dt1.Rows.Count; i++)
            {
                dt.Rows.Add(dt1.Rows[i][0].ToString(), (System.Decimal)(dt1.Rows[i][1]));
            }

            for (int i = 0; i < dt2.Rows.Count; i++)
            {
                dt.Rows.Add(dt2.Rows[i][0].ToString(), (System.Decimal)(dt2.Rows[i][1]));
            }


                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    
                    if (i == 0)
                    {
                        dt.Rows[i][0] = "Общая результативность деятельности органа исполнительной власти";                        
                    }
                    if (i == 1)
                    {
                        dt.Rows[i][0] = "Общая результативность достижения целей";
                    }
                    string LinkS = RegionSettingsHelper.Instance.GetPropertyValue(dt.Rows[i][0].ToString().Replace(' ', '_'));
                    dt.Rows[i][0] = dt.Rows[i][0].ToString().Split(';')[0];                    

                    if (!string.IsNullOrEmpty(LinkS))
                    {
                        dt.Rows[i][0] = dt.Rows[i][0].ToString() +  string.Format(string.Format(LinkGridFS, LinkS),comboFo.SelectedValue.Replace(' ','_')+":" +ComboYear.SelectedValue);
                    }
                }
            G.DataSource = dt;
        }

        int countTopLevel = 0;
        int countNoTopLevel = 0;
        protected void G_InitializeRow(object sender, Infragistics.WebUI.UltraWebGrid.RowEventArgs e)
        {
            if (e.Row.Index > 1)
            {
                if ((e.Row.Cells[0].Text != "Неизвестные данные") && (e.Row.Cells[0].Text != "Все цели"))
                {
                    countNoTopLevel++;
                    e.Row.Cells[0].Style.Padding.Left = 10;
                    e.Row.Cells[0].Text = countNoTopLevel.ToString() + ". " + e.Row.Cells[0].Text;

                }
                else
                {
                    e.Row.Hidden = 1 == 1;
                }
            }
            else
            {   
                countTopLevel++;
                e.Row.Cells[0].Text = countTopLevel.ToString() + ". " + e.Row.Cells[0].Text;
            }
        }

        protected void G_InitializeLayout(object sender, LayoutEventArgs e)
        {
            //int G_W = (int)G.Width.Value;
            int addW = 0;
            if (BN == "IE")
            {
            }
            else
            if (BN == "FIREFOX")
            {
                addW = -10;
            }
            else
            {
                addW = -16;
            }

            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth((((CustomReportConst.minScreenWidth-15) * 50) / 60) - 25+addW);
            e.Layout.Bands[0].Columns[1].Width = CRHelper.GetColumnWidth((((CustomReportConst.minScreenWidth - 15) * 10) / 60) - 20+addW);
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = 1 == 1;
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[1], "### ### ##0.## %");
            e.Layout.Bands[0].Columns[0].Header.Caption = "Индикатор";
            e.Layout.Bands[0].Columns[1].Header.Caption = "Оценка";
            e.Layout.Bands[0].Columns[0].Header.Style.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.Bands[0].Columns[1].Header.Style.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.Bands[0].Columns[1].CellStyle.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.NoDataMessage = "Нет данных";
        }

        DataTable dtBufChart = null;
        DataTable dtLeftChart = null;
        DataTable dtRightChart = null;
        protected void GenChart(Infragistics.WebUI.UltraWebChart.UltraChart C, string q)
        {
            DataTable dt = GetDS(q);
            DataTable dtRes = new DataTable();
            dtRes.Columns.Add("Год", typeof(int));
            dtRes.Columns.Add("Значение", typeof(System.Double));
            dtRes.Columns.Add("Значение2", typeof(System.Double));

            System.Decimal max = 100;
            dt.Rows[0].Delete();
            if (LD.Value == "2008")
            {
                dt.Rows[dt.Rows.Count - 1].Delete();
            }
            if (dt.Rows.Count == 0)
            {
                return;
            }
            for (int i = 0; i < dt.Rows.Count;i++ )
            {
                dtRes.Rows.Add(int.Parse(dt.Rows[i][0].ToString()), (System.Decimal)(dt.Rows[i][1]) * 100, (System.Decimal)(dt.Rows[i][2]) * 100);
                if ((System.Decimal)(dt.Rows[i][1])*100 > max)
                {
                    max = (System.Decimal)(dt.Rows[i][1])*100;
                }
                if ((System.Decimal)(dt.Rows[i][2])*100 > max)
                {
                    max = (System.Decimal)(dt.Rows[i][2])*100;
                }
            }

            C.Axis.Y.RangeMax = (System.Double)max+10;            
            C.Axis.Y.RangeMin = -20;
            C.Axis.Y.RangeType = AxisRangeType.Custom;

            C.Axis.X.RangeMin = (int)(dtRes.Rows[0][0]) - 1;
            C.Axis.X.RangeMax = (int)(dtRes.Rows[dtRes.Rows.Count-1][0]) + 1;
            C.Axis.X.RangeType = AxisRangeType.Custom;
            
            C.Tooltips.FormatString = "<DATA_VALUE_Y:#####0>%";

            C.DataSource = dtRes;
            dtBufChart = dtRes;

        }







        protected void LC_DataBinding(object sender, EventArgs e)
        {
            GenChart(LC, "C");
            dtLeftChart = dtBufChart;
        }

        protected void LC_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            try
            {
                IAdvanceAxis xAxis = (IAdvanceAxis)e.Grid["X"];
                IAdvanceAxis yAxis = (IAdvanceAxis)e.Grid["Y"];

                int i = 0;
                foreach (Primitive p in e.SceneGraph)
                {
                    if (p != null)
                    {
                        if (p is Symbol)
                        {
                            
                            Symbol Icon = (Symbol)(p);
                            
                            {
                                
                                if ((System.Double)dtRightChart.Rows[i][2] > 100)
                                {
                                    Icon.PE.Fill = Color.RoyalBlue;
                                }
                                else
                                {
                                    Icon.PE.Fill = Color.IndianRed;
                                }
                            }
                            
                            
                            i++;

                        }
                        if (p is Infragistics.UltraChart.Core.Primitives.Text)
                        {
                            Infragistics.UltraChart.Core.Primitives.Text t = (Infragistics.UltraChart.Core.Primitives.Text)p;

                            if (string.IsNullOrEmpty(t.Path))
                            {

                                t.SetTextString(string.Format("\n{0}", t.GetTextString()));
                            }
                            else
                            if ((t.Path == "Border.Title.Grid.Y") && (t.GetTextString() == "100%"))
                            {
                                t.SetTextString("План");
                                t.labelStyle.Font = new Font("Arial", 8, FontStyle.Bold);
                            }else
                            if ((t.Path == "Border.Title.Grid.Y") && (t.GetTextString() == "-20%"))
                            {
                               t.SetTextString("");
                               t.labelStyle.Font = new Font("Arial", 8, FontStyle.Bold);
                            }

                        }
                    }
                }

                Infragistics.UltraChart.Core.Primitives.Line l = new Infragistics.UltraChart.Core.Primitives.Line(new Point((int)xAxis.Map(xAxis.Minimum), (int)yAxis.Map(100)), new Point((int)xAxis.Map(xAxis.Maximum), (int)(yAxis.Map(100))));
                l.lineStyle.DrawStyle = LineDrawStyle.Dot;
                l.PE.Stroke = Color.DarkGray;
                l.PE.StrokeWidth = 2;
                
                e.SceneGraph.Add(l);
            }
            catch { }



        }

        protected void RC_DataBinding(object sender, EventArgs e)
        {
            GenChart(RC, "C");
            dtRightChart = dtBufChart;
        }

        protected void C_InvalidDataReceived(object sender, ChartDataInvalidEventArgs e)
        {
            CRHelper.UltraChartInvalidDataReceived(sender, e);
        }

        protected void RC_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            try
            { 
                IAdvanceAxis xAxis = (IAdvanceAxis)e.Grid["X"];
                IAdvanceAxis yAxis = (IAdvanceAxis)e.Grid["Y"];

                int i = 0;
                foreach (Primitive p in e.SceneGraph)
                {
                    if (p != null)
                    {
                        if (p is Symbol)
                        {
                            
                            Symbol Icon = (Symbol)(p);
                            if ((System.Double)dtLeftChart.Rows[i][1] > 100)
                            {
                                Icon.PE.Fill = Color.RoyalBlue;
                            }
                            else
                            {
                                Icon.PE.Fill = Color.IndianRed;
                            }
                            i++;




                        }
                        if (p is Infragistics.UltraChart.Core.Primitives.Text)
                        {
                            Infragistics.UltraChart.Core.Primitives.Text t = (Infragistics.UltraChart.Core.Primitives.Text)p;

                            if (string.IsNullOrEmpty(t.Path))
                            {

                                t.SetTextString(string.Format("\n{0}", t.GetTextString()));
                            }
                            else
                                if ((t.Path == "Border.Title.Grid.Y") && (t.GetTextString() == "100%"))
                                {
                                    t.SetTextString("План");
                                    t.labelStyle.Font = new Font("Arial", 8, FontStyle.Bold);
                                }
                                else
                                {                                    
                                    if ((t.Path == "Border.Title.Grid.Y") && (t.GetTextString() == "-20%"))
                                    {
                                        t.SetTextString("");
                                        t.labelStyle.Font = new Font("Arial", 8, FontStyle.Bold);
                                    }
                                }
                        }
                    }
                }

                Infragistics.UltraChart.Core.Primitives.Line l = new Infragistics.UltraChart.Core.Primitives.Line(new Point((int)xAxis.Map(xAxis.Minimum), (int)yAxis.Map(100)), new Point((int)xAxis.Map(xAxis.Maximum), (int)(yAxis.Map(100))));
                l.lineStyle.DrawStyle = LineDrawStyle.Dot;
                l.PE.Stroke = Color.DarkGray;
                l.PE.StrokeWidth = 2;

                e.SceneGraph.Add(l);
            }
            catch { }
           
        }
        #region Экспорт в PDF
        bool isPDFExport = true;
        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            Report r = new Report();
            ISection Section = r.AddSection();
            IText title = Section.AddText();
            Font font = new Font("Verdana", 16);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.Style.Font.Bold = true;
            title.AddContent(Label1.Text);

            title = Section.AddText();
            font = new Font("Verdana", 14);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.AddContent(Label2.Text);
            UltraGridExporter1.PdfExporter.DownloadName = "report.pdf";

                for (int i = 0; i < G.Rows.Count; i++)
                {
                    G.Rows[i].Cells[0].Text = G.Rows[i].Cells[0].Text.Split('<')[0];
                }

            UltraGridExporter1.PdfExporter.Export(G,Section);
        }

        private void PdfExporter_BeginExport(object sender, Infragistics.WebUI.UltraWebGrid.DocumentExport.DocumentExportEventArgs e)
        {
            ////for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i++)
            ////{
            ////    e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(165);
            ////}

            //IText title = e.Section.AddText();
            //Font font = new Font("Verdana", 16);
            //title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            //title.Style.Font.Bold = true;
            ////title.AddContent(PageTitle.Text);

            //title = e.Section.AddText();
            //font = new Font("Verdana", 14);
            //title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            ////title.AddContent(PageSubTitle.Text);
        }

        private void PdfExporter_EndExport(object sender, Infragistics.WebUI.UltraWebGrid.DocumentExport.EndExportEventArgs e)
        {            
            try
            {
                //Report r = new Report();
                //ISection section = r.AddSection();
                //G.Height = 50;
    

                //UltraGridExporter1.PdfExporter.Export(G, section);

                ISection section2 = e.Section;//r.AddSection();
                ITable table = section2.AddTable();
                Infragistics.Documents.Reports.Graphics.Image img = UltraGridExporter.GetImageFromChart(LC);
                Infragistics.Documents.Reports.Graphics.Image img2 = UltraGridExporter.GetImageFromChart(RC);
                ITableRow row0 = table.AddRow();

                IText title = row0.AddCell().AddText();
                //title = e.Section.AddText();
                Font font = new Font("Verdana", 14);
                title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
                title.AddContent(Label3.Text);

                title = row0.AddCell().AddText();
                //title = e.Section.AddText();
                font = new Font("Verdana", 14);
                title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
                title.AddContent(Label4.Text);


                ITableRow row = table.AddRow();

                row.AddCell().AddImage(img);





                row.AddCell().AddImage(img2);
                //UltraGridExporter1.PdfExporter.Export(new UltraWebGrid(), section2);
                
            }
            catch { }
        }


        #endregion
    }
}
