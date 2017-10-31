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
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Section;
using Infragistics.Documents.Reports.Report.Table;
using System.Globalization;
using System.IO;


using Dundas.Maps.WebControl;
using Infragistics.Documents.Reports.Graphics;
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

using EndExportEventArgs = Infragistics.WebUI.UltraWebGrid.ExcelExport.EndExportEventArgs;
using InitializeRowEventHandler = Infragistics.WebUI.UltraWebGrid.InitializeRowEventHandler;
using SerializationFormat = Dundas.Maps.WebControl.SerializationFormat;

namespace Krista.FM.Server.Dashboards.FO_BOR_0001_001_0001.Default.reports.FO_BOR_0001_001_0001
{
    public partial class _default : CustomReportPage
    {
        string HederReport = "Общая результативность деятельности органов исполнительной власти в {1} году <!--({0})-->";
        string LastDateFormatString = "{0}";
        string LinkGridFS = "  <a href=\"{0}\">>></a>";

        //LastDate
        private CustomParam LD { get { return (UserParams.CustomParam("LD")); } }

        System.Collections.Generic.Dictionary<string, string> BigNameGRBS;// = new System.Collections.Generic.Dictionary<string, int>();

        //Генерит словарик с годами для параметра
        System.Collections.Generic.Dictionary<string, int> GenUserParam(string query)
        {
            DataTable dt = GetDS(query);
            System.Collections.Generic.Dictionary<string, int> d = new System.Collections.Generic.Dictionary<string, int>();
            foreach (DataColumn Column in dt.Columns)
            {
                if (Column.Caption != "LD_")
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

            CL.Width = CRHelper.GetChartWidth(610);
            CR.Width = CRHelper.GetChartWidth(610);
            GL.Width = CRHelper.GetGridWidth(612);
            GR.Width = CRHelper.GetGridWidth(612);
            UltraGridExporter1.PdfExporter.Visible = 1 == 2;

            UltraGridExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            UltraGridExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
            UltraGridExporter1.ExcelExporter.EndExport += new EndExportEventHandler(ExcelExporter_EndExport);


            UltraGridExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
            UltraGridExporter1.PdfExporter.BeginExport += new EventHandler<DocumentExportEventArgs>(PdfExporter_BeginExport);

            CR.DeploymentScenario.ImageURL = "../../TemporaryImages/Chart_fo_02_08_1.png";

        }

        string BN = "IE";
        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            System.Web.HttpBrowserCapabilities myBrowserCaps = Request.Browser;
            BN = ((System.Web.Configuration.HttpCapabilitiesBase)myBrowserCaps).Browser.ToUpper();

            BigNameGRBS = new System.Collections.Generic.Dictionary<string, string>();

            if (!Page.IsPostBack)
            {
                ComboYear.FillDictionaryValues(GenUserParam("LD_"));
                ComboYear.SetСheckedState(LD.Value, 1 == 1);
                ComboYear.Title = "Период";
                ComboYear.Width = 150;
                GL.DisplayLayout.NoDataMessage = "Нет данных";
                GR.DisplayLayout.NoDataMessage = "Нет данных";
                Label1.Text = string.Format(HederReport, "(неуказано)", LD.Value);


            }
            try
            {

            }
            catch (Exception ex)
            {
                CL.Series.Clear();
                CL.DataBind();

                CR.Series.Clear();
                CR.DataBind();

                GL.Rows.Clear();
                GL.Bands.Clear();
                GR.Rows.Clear();
                GR.Bands.Clear();
                String.Format(ex.StackTrace + "\n" + ex.Source + "\n" + ex.StackTrace);
                DoubleChart.Rows[0].Cells[0].Style.Clear();



            }

            LD.Value = ComboYear.SelectedValue;
            GL.DataBind();
            GR.DataBind();

            GR.Height = GL.Height = 250;

            int AllColuntGRBS = GL.Rows.Count + GR.Rows.Count;
            if (GL.Columns.Count > 0)
            {
                GL.Columns[0].Header.Caption = string.Format("Выполнили план {1} ГРБС ({0:#0.##}%)", (100 * GL.Rows.Count / (AllColuntGRBS)), GL.Rows.Count);
            }
            if (GR.Columns.Count > 0)
            {
                GR.Columns[0].Header.Caption = string.Format("Не выполнили план {1} ГРБС ({0:#0.##}%)", (100 * GR.Rows.Count / (AllColuntGRBS)), GR.Rows.Count);
            }

            CL.DataBind();
            CR.DataBind();

            Page.Title = Label1.Text;


        }

        DataTable dtGL_ = null;
        protected void GL_DataBinding(object sender, EventArgs e)
        {
            DataTable dt = GetDS("GL");

            string CupYear = LD.Value;
            LD.Value = (int.Parse(CupYear) - 1).ToString();
            dtGL_ = GetDS("GL");
            LD.Value = CupYear;

            dt.Columns.Add("Ранг");
            for (int i = 0; i < dt.Rows.Count; i++)
            {

                if (string.IsNullOrEmpty(dt.Rows[i][1].ToString()))
                {
                    dt.Rows[i].Delete();
                    i--;
                }
                else
                {
                    //GL_row_index++;
                    //dt.Rows[i][dt.Columns.Count - 1] = (dt.Rows.Count - i).ToString();                   
                }
            }
            GL.DataSource = dt.Rows.Count > 0 ? dt : null;
        }

        DataTable dtGR_ = null;
        protected void GR_DataBinding(object sender, EventArgs e)
        {
            DataTable dt = GetDS("GR");
            string CupYear = LD.Value;
            LD.Value = (int.Parse(CupYear) - 1).ToString();
            dtGR_ = GetDS("GR");
            LD.Value = CupYear;

            dt.Columns.Add("Ранг");
            for (int i = 0; i < dt.Rows.Count; i++)
            {

                if (string.IsNullOrEmpty(dt.Rows[i][1].ToString()))
                {
                    dt.Rows[i].Delete();
                    i--;
                }
                else
                {
                    //GL_row_index++;



                }
            }

            GR.DataSource = dt.Rows.Count > 0 ? dt : null;

        }

        protected void CL_DataBinding(object sender, EventArgs e)
        {
            DataTable dt = GetDS("CL");
            //String.Format(DataProvider.GetQueryText("CL"));

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                try
                {
                    //String.Format(dt.Rows[i][1].ToString());
                    BigNameGRBS.Add(dt.Rows[i][1].ToString(), dt.Rows[i][0].ToString());
                }
                catch { }
                dt.Rows[i][2] = (System.Decimal)(dt.Rows[i][2]) - 99;
            }
            CL.Series.Clear();

            dt.Columns.Remove(dt.Columns[0]);

            for (int i = 1; i < 3; i++)
            {
                try
                {
                    CL.Series.Add(CRHelper.GetNumericSeries(i, dt));

                }
                catch { }
            }
        }

        protected void CR_DataBinding(object sender, EventArgs e)
        {

            DataTable dt = GetDS("CR");

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                try
                {
                    //String.Format(dt.Rows[i][1].ToString());
                    BigNameGRBS.Add(dt.Rows[i][1].ToString(), dt.Rows[i][0].ToString());
                }
                catch { }
                dt.Rows[i][2] = -((System.Decimal)(dt.Rows[i][2]) - 100);
            }
            dt.Columns.Remove(dt.Columns[0]);
            CR.Series.Clear();
            for (int i = 1; i < 3; i++)
            {
                try
                {
                    CR.Series.Add(CRHelper.GetNumericSeries(i, dt));
                }
                catch { }

            }




        }

        protected void CL_FillSceneGraph(object sender, Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e)
        {
            if (GL.Rows.Count <= 0) { return; }
            for (int i = 0; i < e.SceneGraph.Count; i++)
            {
                if (e.SceneGraph[i] != null)
                {
                    if (e.SceneGraph[i] is Box)
                    {
                        if (((Box)e.SceneGraph[i]).DataPoint != null)
                        {
                            if (((Box)(e.SceneGraph[i])).Value != null)
                            {
                                //String.Format(e.SceneGraph[i].DataPoint.Label);
                                e.SceneGraph[i].DataPoint.Label = string.Format("{1}\n{0}", (99 + (System.Double)(e.SceneGraph[i].Value)).ToString(), BigNameGRBS[e.SceneGraph[i].DataPoint.Label]);
                            }

                        }
                    }
                }
            }


            //Типа ось х
            Infragistics.UltraChart.Core.Primitives.Line l = new Infragistics.UltraChart.Core.Primitives.Line(new System.Drawing.Point(90, CL.Axis.X.Extent + 4), new System.Drawing.Point((int)(CL.Width.Value), CL.Axis.X.Extent + 4));
            l.PE = Infragistics.UltraChart.Core.Primitives.Line.DefaultPE;
            l.PE.StrokeWidth = 2;
            l.PE.Stroke = System.Drawing.Color.Black;
            e.SceneGraph.Add(l);

            Text textLabel = new Text();
            textLabel.labelStyle.Orientation = TextOrientation.VerticalLeftFacing;
            textLabel.PE.Fill = System.Drawing.Color.Black;
            textLabel.labelStyle.Font = new System.Drawing.Font("Arial", 11);
            textLabel.labelStyle.HorizontalAlign = StringAlignment.Near;
            textLabel.labelStyle.VerticalAlign = StringAlignment.Near;
            textLabel.bounds = new System.Drawing.Rectangle(35, 80, 160, 160);
            textLabel.SetTextString("Выполнено");
            e.SceneGraph.Add(textLabel);

        }

        protected void CR_FillSceneGraph(object sender, Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e)
        {
            if (GR.Rows.Count <= 0) { return; }
            for (int i = 0; i < e.SceneGraph.Count; i++)
            {
                if (e.SceneGraph[i] != null)
                {
                    if (e.SceneGraph[i] is Box)
                    {
                        if (((Box)e.SceneGraph[i]).DataPoint != null)
                        {
                            if (((Box)(e.SceneGraph[i])).Value != null)
                            {
                                e.SceneGraph[i].DataPoint.Label = string.Format("{1}\n{0}", (100 - (System.Double)(e.SceneGraph[i].Value)).ToString(), BigNameGRBS[e.SceneGraph[i].DataPoint.Label]);
                            }

                        }
                    }
                }
            }



            //Типа ось х
            Infragistics.UltraChart.Core.Primitives.Line l = new Infragistics.UltraChart.Core.Primitives.Line(new System.Drawing.Point(0, CL.Axis.X.Extent + 4), new System.Drawing.Point((int)(CL.Width.Value - 90), CL.Axis.X.Extent + 4));
            l.PE.StrokeWidth = 2;
            l.PE.Stroke = System.Drawing.Color.Black;
            e.SceneGraph.Add(l);


            Text textLabel = new Text();
            textLabel.labelStyle.Orientation = TextOrientation.VerticalRightFacing;
            textLabel.PE.Fill = System.Drawing.Color.Black;
            textLabel.labelStyle.Font = new System.Drawing.Font("Arial", 11);
            textLabel.labelStyle.HorizontalAlign = StringAlignment.Near;
            textLabel.labelStyle.VerticalAlign = StringAlignment.Near;
            textLabel.bounds = new System.Drawing.Rectangle(410, 150, 160, 160);
            textLabel.SetTextString("Не выполнено");
            e.SceneGraph.Add(textLabel);

        }

        protected void GR_InitializeLayout(object sender, Infragistics.WebUI.UltraWebGrid.LayoutEventArgs e)
        {
            int addW = 0;
            if (BN == "FIREFOX")
            {
                addW = -4;
            }
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(400 + addW);
            e.Layout.Bands[0].Columns[1].Width = CRHelper.GetColumnWidth(71 + addW);
            e.Layout.Bands[0].Columns[2].Width = CRHelper.GetColumnWidth(76 + addW);
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = 1 == 1;
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[1], "N0");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[2], "N0");
            e.Layout.Bands[0].Columns[1].Header.Caption = "Оценка";
            e.Layout.Bands[0].Columns[2].Header.Caption = "Ранг";
            e.Layout.Bands[0].Columns[2].CellStyle.HorizontalAlign = HorizontalAlign.Center;
            for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
            {
                e.Layout.Bands[0].Columns[i].Header.Style.HorizontalAlign = HorizontalAlign.Center;
                e.Layout.Bands[0].Columns[i].Header.Style.Wrap = 1 == 1;
            }
            e.Layout.Bands[0].Columns[1].CellStyle.HorizontalAlign = HorizontalAlign.Center;
        }

        int GR_row_index = 0;
        int GL_row_index = 0;
        protected void GR_InitializeRow(object sender, Infragistics.WebUI.UltraWebGrid.RowEventArgs e)
        {

            if (dtGL_ != null)
            for (int i = 0; i < dtGL_.Rows.Count; i++)
            {
                try
                { 
                    
                    if (dtGL_.Rows[i][0].ToString() == e.Row.Cells[0].Text)
                    {
                        String.Format(dtGL_.Rows[i][0].ToString() + " | " + e.Row.Cells[0].Text);
                        System.Decimal val1 = (System.Decimal)(e.Row.Cells[1].Value);
                        System.Decimal val2 = (System.Decimal)(dtGL_.Rows[i][1]);
                        string UpOrDown = (val1 > val2 ? "Выше" : "Ниже");
                        e.Row.Cells[1].Title = string.Format("{0} на {1} % по сравнению с предыдущим периодом", UpOrDown, Math.Abs(val1 - val2));
                    }
                }
                catch { }
            }

            if (dtGR_!=null)
            for (int i = 0; i < dtGR_.Rows.Count; i++)
            {
                try
                { 
                
                    if (dtGR_.Rows[i][0].ToString() == e.Row.Cells[0].Text) 
                    {
                        String.Format(dtGR_.Rows[i][0].ToString() + " | " + e.Row.Cells[0].Text);
                        System.Decimal val1 = (System.Decimal)(e.Row.Cells[1].Value);
                        System.Decimal val2 = (System.Decimal)(dtGL_.Rows[i][1]);
                        string UpOrDown = (val1 > val2 ? "Выше" : "Ниже");
                        e.Row.Cells[1].Title = string.Format("{0} на {1} % по сравнению с предыдущим периодом", UpOrDown, Math.Abs(val1 - val2));
                    }
                }
                catch { }
            }


            if (e.Row.Index == 0)
            {
                GL_row_index = 0;
            }
            GR_row_index++;
            GL_row_index++;

            e.Row.Cells[2].Text = ((Infragistics.WebUI.UltraWebGrid.UltraWebGrid)sender == GL ? GR_row_index.ToString() : (GL_row_index).ToString());

            e.Row.Cells[1].Text += " %";

            string Link = string.Format("../FO_BOR_0001_002_0001/default.aspx?paramlist=CRepoort={0}", e.Row.Cells[0].Text.Replace(' ', '_'));
            //if (!string.IsNullOrEmpty(Link))
            {
                e.Row.Cells[0].Text += string.Format(LinkGridFS, Link);
            }

        }

        protected void CR_InvalidDataReceived(object sender, ChartDataInvalidEventArgs e)
        {
            CRHelper.UltraChartInvalidDataReceived(sender, e);
        }
        //#region Экпорт в Excel

        private void ExcelExporter_BeginExport(object sender, BeginExportEventArgs e)
        {

        }

        private void ExcelExporter_EndExport(object sender, EndExportEventArgs e)
        {
            foreach (Worksheet sheet in e.Workbook.Worksheets)
            {
                if (sheet.Index < 2)
                {
                    for (int i = 1; i < 1 + ((GL.Rows.Count + GR.Rows.Count) + Math.Abs(GL.Rows.Count + GR.Rows.Count)) / 2; i++)
                    {
                        try
                        {

                            sheet.Rows[i].Cells[0].Value = sheet.Rows[i].Cells[0].Value.ToString().Split('<')[0];
                            sheet.Rows[i].Height = 15 * 37;
                        }
                        catch { }
                    }


                    sheet.Columns[0].Width = 340 * 37;
                    sheet.Columns[1].Width = 120 * 37;
                    sheet.Columns[2].Width = 120 * 37;

                    sheet.Columns[0].CellFormat.WrapText = ExcelDefaultableBoolean.True;

                    sheet.Columns[1].CellFormat.FormatString = "#,##0";
                    sheet.Columns[2].CellFormat.FormatString = "#,##0.0##";
                    sheet.Columns[3].CellFormat.FormatString = "#,##0.0##";
                    sheet.Columns[4].CellFormat.FormatString = UltraGridExporter.ExelPercentFormat;
                }
                else
                {


                    Infragistics.Documents.Excel.WorksheetImage shape = new WorksheetImage(
                        System.Drawing.Image.FromFile(Server.MapPath("../../TemporaryImages/Chart_fo_02_08_2.png"))
                        );
                    shape.TopLeftCornerCell = sheet.Rows[0].Cells[0];
                    shape.BottomRightCornerCell = sheet.Rows[20].Cells[10];
                    sheet.Shapes.Add(shape);

                    Infragistics.Documents.Excel.WorksheetImage shape2 = new WorksheetImage(System.Drawing.Image.FromFile(Server.MapPath("../../TemporaryImages/Chart_fo_02_08_1.png")));
                    shape2.TopLeftCornerCell = sheet.Rows[0].Cells[10];
                    shape2.BottomRightCornerCell = sheet.Rows[20].Cells[20];
                    sheet.Shapes.Add(shape2);
                }

            }
        }

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            Workbook workbook = new Workbook();
            Worksheet sheet2 = workbook.Worksheets.Add("Не выполнено");
            Worksheet sheet1 = workbook.Worksheets.Add("Выполнено");
            Worksheet sheet3 = workbook.Worksheets.Add("Диаграмма");



            UltraGridExporter1.ExcelExporter.DownloadName = "report.xls";

            UltraGridExporter1.ExcelExporter.ExcelStartRow = 1;
            UltraGridExporter1.ExcelExporter.Export(GL, sheet1);

            UltraGridExporter1.ExcelExporter.ExcelStartRow = 1;
            UltraGridExporter1.ExcelExporter.Export(GR, sheet2);


        }
        //#endregion
        //#region Экспорт в PDF
        bool isPDFExport = true;
        private void PdfExporter_BeginExport(object sender, Infragistics.WebUI.UltraWebGrid.DocumentExport.DocumentExportEventArgs e)
        {
            if (!isPDFExport)
                return;
            else
                isPDFExport = !isPDFExport;


            IText title = e.Section.AddText();
            System.Drawing.Font font = new System.Drawing.Font("Verdana", 16);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.Style.Font.Bold = true;
            title.AddContent(Page.Title.Split('<')[0]); ;

            title = e.Section.AddText();
            font = new System.Drawing.Font("Verdana", 14);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.AddContent(Label2.Text);
        }

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            ReportSection se = new ReportSection(new Report(), 1 == 1);
            try
            {
                Infragistics.Documents.Reports.Graphics.Image img = UltraGridExporter.GetImageFromChart(CL);
                se.AddImage(img);
                for (int i = 0; i < GL.Rows.Count; i++)
                {
                    GL.Rows[i].Cells[0].Text = GL.Rows[i].Cells[0].Text.Split('<')[0];
                }
                UltraGridExporter1.PdfExporter.Export(GL, se);
                se.AddFlowColumnBreak();

                se.AddImage(new Infragistics.Documents.Reports.Graphics.Image(Server.MapPath("../../TemporaryImages/Chart_fo_02_08_1.png")));
                for (int i = 0; i < GR.Rows.Count; i++)
                {
                    GR.Rows[i].Cells[0].Text = GR.Rows[i].Cells[0].Text.Split('<')[0];
                }
                UltraGridExporter1.PdfExporter.Export(GR, se);
            }
            catch { }

            //seca.AddImage(UltraGridExporter.GetImageFromChart(CL));
            //seca.AddImage(UltraGridExporter.GetImageFromChart(CR));

            //UltraGridExporter1.PdfExporter.Export(new Infragistics.WebUI.UltraWebGrid.UltraWebGrid(),new 
        }




        public class ReportSection : ISection
        {
            private readonly bool withFlowColumns;
            private readonly ISection section;
            private IFlow flow;
            private ITableCell titleCell;

            public ReportSection(Report report, bool withFlowColumns)
            {
                this.withFlowColumns = withFlowColumns;
                section = report.AddSection();
                ITable table = section.AddTable();
                ITableRow row = table.AddRow();
                titleCell = row.AddCell();
                if (this.withFlowColumns)
                {
                    flow = section.AddFlow();
                    IFlowColumn col = flow.AddColumn();
                    col.Width = new FixedWidth(450);
                    col = flow.AddColumn();
                    col.Width = new FixedWidth(600);
                }
            }

            public void AddFlowColumnBreak()
            {
                if (flow != null)
                    flow.AddColumnBreak();
            }

            public IBand AddBand()
            {
                if (flow != null)
                    return flow.AddBand();
                return section.AddBand();
            }

            //#region ISection members
            public ISectionHeader AddHeader()
            {
                throw new NotImplementedException();
            }

            public ISectionFooter AddFooter()
            {
                throw new NotImplementedException();
            }

            public IStationery AddStationery()
            {
                throw new NotImplementedException();
            }

            public IDecoration AddDecoration()
            {
                throw new NotImplementedException();
            }

            public ISectionPage AddPage()
            {
                throw new NotImplementedException();
            }

            public ISectionPage AddPage(PageSize size)
            {
                throw new NotImplementedException();
            }

            public ISectionPage AddPage(float width, float height)
            {
                throw new NotImplementedException();
            }

            public ISegment AddSegment()
            {
                throw new NotImplementedException();
            }

            public IQuickText AddQuickText(string text)
            {
                throw new NotImplementedException();
            }

            public IQuickImage AddQuickImage(Infragistics.Documents.Reports.Graphics.Image image)
            {
                throw new NotImplementedException();
            }

            public IQuickList AddQuickList()
            {
                throw new NotImplementedException();
            }

            public IQuickTable AddQuickTable()
            {
                throw new NotImplementedException();
            }

            public IText AddText()
            {
                return this.titleCell.AddText();
            }

            public IImage AddImage(Infragistics.Documents.Reports.Graphics.Image image)
            {
                if (flow != null)
                    return flow.AddImage(image);
                return this.section.AddImage(image);
            }

            public IMetafile AddMetafile(Metafile metafile)
            {
                throw new NotImplementedException();
            }

            public IRule AddRule()
            {
                throw new NotImplementedException();
            }

            public IGap AddGap()
            {
                throw new NotImplementedException();
            }

            public IGroup AddGroup()
            {
                throw new NotImplementedException();
            }

            public IChain AddChain()
            {
                throw new NotImplementedException();
            }

            public ITable AddTable()
            {
                if (flow != null)
                    return flow.AddTable();
                return this.section.AddTable();
            }

            public IGrid AddGrid()
            {
                throw new NotImplementedException();
            }

            public IFlow AddFlow()
            {
                throw new NotImplementedException();
            }

            public Infragistics.Documents.Reports.Report.List.IList AddList()
            {
                throw new NotImplementedException();
            }

            public ITree AddTree()
            {
                throw new NotImplementedException();
            }

            public ISite AddSite()
            {
                throw new NotImplementedException();
            }

            public ICanvas AddCanvas()
            {
                throw new NotImplementedException();
            }

            public IRotator AddRotator()
            {
                throw new NotImplementedException();
            }

            public IContainer AddContainer(string name)
            {
                throw new NotImplementedException();
            }

            public ICondition AddCondition(IContainer container, bool fit)
            {
                throw new NotImplementedException();
            }

            public IStretcher AddStretcher()
            {
                throw new NotImplementedException();
            }

            public void AddPageBreak()
            {
                throw new NotImplementedException();
            }

            public ITOC AddTOC()
            {
                throw new NotImplementedException();
            }

            public IIndex AddIndex()
            {
                throw new NotImplementedException();
            }

            public bool Flip
            {
                get { throw new NotImplementedException(); }
                set { throw new NotImplementedException(); }
            }

            public PageSize PageSize
            {
                get { throw new NotImplementedException(); }
                set { this.section.PageSize = new PageSize(1200, 1600); }
            }

            public PageOrientation PageOrientation
            {
                get { throw new NotImplementedException(); }
                set { throw new NotImplementedException(); }
            }

            public Infragistics.Documents.Reports.Report.ContentAlignment PageAlignment
            {
                get { throw new NotImplementedException(); }
                set { throw new NotImplementedException(); }
            }

            public Borders PageBorders
            {
                get { throw new NotImplementedException(); }
                set { throw new NotImplementedException(); }
            }

            public Infragistics.Documents.Reports.Report.Margins PageMargins
            {
                get { return this.section.PageMargins; }
                set { throw new NotImplementedException(); }
            }

            public Paddings PagePaddings
            {
                get { throw new NotImplementedException(); }
                set { throw new NotImplementedException(); }
            }

            public Background PageBackground
            {
                get { throw new NotImplementedException(); }
                set { throw new NotImplementedException(); }
            }

            public Infragistics.Documents.Reports.Report.Section.PageNumbering PageNumbering
            {
                get { throw new NotImplementedException(); }
                set { throw new NotImplementedException(); }
            }

            public SectionLineNumbering LineNumbering
            {
                get { throw new NotImplementedException(); }
                set { throw new NotImplementedException(); }
            }

            public Report Parent
            {
                get { return this.section.Parent; }
            }

            public IEnumerable Content
            {
                get { throw new NotImplementedException(); }
            }
        }

    }
}
