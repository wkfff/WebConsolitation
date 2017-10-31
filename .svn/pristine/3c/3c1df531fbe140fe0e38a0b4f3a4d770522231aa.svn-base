using System;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Table;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Infragistics.WebUI.Misc;
using Microsoft.AnalysisServices.AdomdClient;

using Infragistics.WebUI.UltraWebChart;
using Infragistics.UltraChart.Core.Primitives;

using System.Collections.ObjectModel;
using System.Text;
using System.Collections.Generic;

using Infragistics.UltraChart.Core;

using Dundas.Maps.WebControl;



namespace Krista.FM.Server.Dashboards.reports.EO.EO_0007.EO_0005
{
    public partial class _default : CustomReportPage
    {
        string HederGlobal = "Реестр контрактов";
        string HederLC = "Структура количества контрактов";
        string HederRC = "Структура стоимости контрактов";

        string C0 = "Наименование продукцыи по ОКПД";
        string C1 = "Реквизиты контрактов";
        string C2 = "Способ закупки";
        string C3 = "Поставщик";
        string C4 = "Стоимость, тыс. р.";

        string GRBS_;

        private CustomParam p1;// { get { return (new CustomParam("1")); } }
        private CustomParam p2;// { get { return (new CustomParam("2")); } }

        private DataTable T1 = new DataTable();
        private CellSet CS;
        public static void setChartErrorFont(Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventArgs e)
        {
            e.Text = "В настоящий момент данные отсутствуют";

            e.LabelStyle.FontColor = Color.LightGray;
            e.LabelStyle.VerticalAlign = StringAlignment.Center;
            e.LabelStyle.HorizontalAlign = StringAlignment.Center;
            e.LabelStyle.Font = new Font("Verdana", 30);
        }

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            p1 = UserParams.CustomParam("1");
            p2 = UserParams.CustomParam("2");

            UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 5);
            UltraWebGrid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight / 3);
            LC.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth * 0.308);
            RC.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth * 0.308);
            RC.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight / 2);
            LC.Height = RC.Height;
            TEXT.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight / 2 + 30);
            TEXT.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth * 0.33);

            GridSearch1.LinkedGridId = this.UltraWebGrid.ClientID;

            UltraGridExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            UltraGridExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
            UltraGridExporter1.ExcelExporter.EndExport += new EndExportEventHandler(ExcelExporter_EndExport);
            UltraGridExporter1.ExcelExporter.HeaderCellExporting += new HeaderCellExportingEventHandler(ExcelExporter_HeaderCellExporting);

            UltraGridExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
            UltraGridExporter1.PdfExporter.BeginExport += new EventHandler
                    <Infragistics.WebUI.UltraWebGrid.DocumentExport.DocumentExportEventArgs>(PdfExporter_BeginExport);
            UltraGridExporter1.PdfExporter.EndExport += new EventHandler
                <Infragistics.WebUI.UltraWebGrid.DocumentExport.EndExportEventArgs>(PdfExporter_EndExport);

            UltraGridExporter1.MultiHeader = true;
        }

        string GetMax(string sql)
        {
            int d;
            string s = "";
            try
            {
                CellSet cs = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(DataProvider.GetQueryText(sql));
                d = 0;
                for (int i = 0; i < cs.Cells.Count; i++)
                {
                    if (double.Parse(cs.Cells[d].Value.ToString()) > double.Parse(cs.Cells[i].Value.ToString())) { d = i; }
                }
                s = cs.Axes[1].Positions[d].Members[0].Caption;
            }
            catch { }
            return s;
        }

        Dictionary<string, int> ForParam3(string sql)
        {
            string s = DataProvider.GetQueryText(sql);
            CellSet cs = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(s.Replace("{0}", "GRBS"));

            Dictionary<string, int> d = new Dictionary<string, int>();

            string bs = "";
            for (int i = 0; i < cs.Cells.Count; i++)
            {
                try
                {
                    if (bs == cs.Cells[i].Value.ToString())
                    {
                        d.Add(cs.Axes[1].Positions[i].Members[0].Caption, 1);
                    }


                    if (cs.Cells[i].Value.ToString() == "Все")
                    {
                        bs = cs.Axes[1].Positions[i].Members[0].Caption;
                        d.Add(bs, 0);
                    }

                }
                catch
                { }
            }


            return d;
        }


        Dictionary<string, int> ForParam(string sql)
        {
            string s = DataProvider.GetQueryText(sql);
            CellSet cs = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(s.Replace("{0}", "ГРБС"));

            Dictionary<string, int> d = new Dictionary<string, int>();
            d.Add("ГРБС", 0);

            for (int i = cs.Axes[1].Positions.Count - 1; i >= 0; i--)
            {
                d.Add(cs.Axes[1].Positions[i].Members[0].Caption, 1);
            }
            d.Add("ПБС", 0);
            cs = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(DataProvider.GetQueryText(sql).Replace("{0}", "ПБС"));
            for (int i = cs.Axes[1].Positions.Count - 1; i >= 0; i--)
            {
                try
                {
                    d.Add(cs.Axes[1].Positions[i].Members[0].Caption, 1);
                }
                catch
                { }
            }

            GRBS_ = cs.Axes[1].Positions[0].Members[0].Caption;
            return d;
        }
        Dictionary<string, int> ForParam2(string sql)
        {
            CellSet cs = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(DataProvider.GetQueryText(sql));
            Dictionary<string, int> d = new Dictionary<string, int>();

            for (int i = cs.Axes[1].Positions.Count - 1; i >= 0; i--)
            {
                d.Add(cs.Axes[1].Positions[i].Members[0].Caption, 0);
            }
            GRBS_ = cs.Axes[1].Positions[0].Members[0].Caption;
            return d;
        }

        protected override void Page_PreInit(object sender, EventArgs e)
        {
            base.Page_PreInit(sender, e);
        }
        protected override void Page_LoadComplete(object sender, EventArgs e)
        {
            base.Page_LoadComplete(sender, e);
        }

        static string GRB2;
        static string GOD;
        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            #region Забивка параметров
            if (!Page.IsPostBack)
            {
                Dictionary<string, string> ConvertNamesArray = new Dictionary<string, string>();
                ComboYear.FillDictionaryValues(ForParam2("lastdate"));
                p2.Value = ComboYear.SelectedValue;
                GRBS.ParentSelect = 1 == 1;

                GRBS.Title = "Заказчик";

                GRBS.Width = 600;

                ComboYear.Title = "Год";
                GRBS.ClearNodes();
                GRBS.FillDictionaryValues(ForParam3("GRBS"));

                ForParam("PLD");
                GRBS.SetСheckedState(GRBS_, true);
                GRB2 = GRBS.SelectedValue;
                GOD = ComboYear.SelectedValue;

                UltraWebGrid.DisplayLayout.CellClickActionDefault = CellClickAction.NotSet;



            }
            else
            {
                p2.Value = ComboYear.SelectedValue;
            }




            #endregion



            UltraWebGrid.DataBind();
            LC.DataBind();
            RC.DataBind();

            #region Размер столбов
            try
            {
                for (int i = 0; i < UltraWebGrid.Columns.Count; i++)
                {
                    UltraWebGrid.Columns[i].Width = CRHelper.GetColumnWidth(CustomReportConst.minScreenWidth * 0.185);
                    UltraWebGrid.Columns[i].CellStyle.Wrap = 1 == 1;
                    UltraWebGrid.Columns[UltraWebGrid.Columns.Count - 1].CellStyle.HorizontalAlign = HorizontalAlign.Left;
                    UltraWebGrid.Columns[i].Header.Style.Wrap = 1 == 1;

                }
                UltraWebGrid.Columns[0].MergeCells = true;
                CRHelper.FormatNumberColumn(UltraWebGrid.Columns[UltraWebGrid.Columns.Count - 1], "N3");
                UltraWebGrid.Columns[UltraWebGrid.Columns.Count - 1].CellStyle.HorizontalAlign = HorizontalAlign.Right;

                UltraWebGrid.Columns[0].Header.Caption = C0;
                UltraWebGrid.Columns[1].Header.Caption = C1;
                UltraWebGrid.Columns[2].Header.Caption = C2;
                UltraWebGrid.Columns[3].Header.Caption = C3;
                UltraWebGrid.Columns[4].Header.Caption = C4;
                UltraWebGrid.Columns[0].CellStyle.TextOverflow = TextOverflow.Ellipsis;
                UltraWebGrid.Columns[0].CellStyle.VerticalAlign = VerticalAlign.Top;
            }
            catch
            { }



            #endregion

            #region Текст
            string Max1 = GetMax("T1");
            string Max2 = GetMax("T2");

            TEXT.Text = string.Format(@"Анализ структуры расходов бюджета региона на закупки государственого заказчика «{3}» в {2} году, показывает, что:<br>
            •	по количеству заключенных контрактов, основным способом размещения заказов является «{0}»;<br>
            •	по стоимости заключенных контрактов, основным способом размещения является «{1}».
            ", GetMax("T1"), GetMax("T2"), ComboYear.SelectedValue,GRBS.SelectedValue);


            HLC.Text = HederLC;
            HRC.Text = HederRC;
            Label1.Text = HederGlobal;


            for (int i = 0; i < UltraWebGrid.Columns.Count; i++) { CRHelper.FormatNumberColumn(UltraWebGrid.Columns[i], "N2"); }
            #endregion



        }

        #region Экспорт в Excel

        private void ExcelExporter_BeginExport(object sender, BeginExportEventArgs e)
        {
            e.CurrentWorksheet.Rows[0].Cells[0].Value = Label1.Text;// +" " + Label1.Text;
        }

        private void ExcelExporter_EndExport(object sender, EndExportEventArgs e)
        {
            for (int i = 2; i < UltraWebGrid.Bands[0].Columns.Count; i = i + 1)
            {
                string formatString = "#,##0.00;[Red]-#,##0.00";
                int widthColumn = 70;

                int j = (i - 1) % 5;
                switch (j)
                {
                    case 1:
                        {
                            formatString = "#,##0.000;[Red]-#,##0.000";
                            widthColumn = 95;
                            break;
                        }
                    case 2:
                        {
                            formatString = "0.00%";
                            widthColumn = 75;
                            break;
                        }
                }

                e.CurrentWorksheet.Columns[i].CellFormat.FormatString = formatString;
                e.CurrentWorksheet.Columns[i].Width = widthColumn * 37;
            }
        }

        private void ExcelExporter_HeaderCellExporting(object sender, HeaderCellExportingEventArgs e)
        {
            e.HeaderText = UltraWebGrid.DisplayLayout.Bands[0].Columns[e.CurrentColumnIndex].Header.Key.Split(';')[0];
        }

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            UltraGridExporter1.ExcelExporter.ExcelStartRow = 4;

            UltraGridExporter1.ExcelExporter.DownloadName = "report.xls";
            UltraGridExporter1.ExcelExporter.Export(UltraWebGrid);
        }

        #endregion

        #region Экспорт в Pdf

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            UltraGridExporter1.PdfExporter.DownloadName = "report.pdf";
            UltraGridExporter1.PdfExporter.Export(UltraWebGrid);
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
            title.AddContent(Label1.Text);
        }

        private void PdfExporter_EndExport(object sender, Infragistics.WebUI.UltraWebGrid.DocumentExport.EndExportEventArgs e)
        {
            IText title = e.Section.AddText();
            Font font = new Font("Verdana", 14);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.Style.Font.Bold = true;
            title.AddContent(HederGlobal);

            title = e.Section.AddText();
            font = new Font("Verdana", 12);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.AddContent(HederLC);

            Infragistics.Documents.Reports.Graphics.Image img = UltraGridExporter.GetImageFromChart(LC);
            e.Section.AddImage(img);

            title = e.Section.AddText();
            font = new Font("Verdana", 12);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.AddContent(HederRC);

            img = UltraGridExporter.GetImageFromChart(RC);
            e.Section.AddImage(img);
        }

        #endregion
        string AddZero(string s)
        {
            try
            {
                int i;
                for (i = s.Length - 1; s[i] != ','; i--) ;
                if (s.Length - i == 2)
                {
                    return s + "0";
                }

            }
            catch
            {
                return s + ",00";
            }
            return s;

        }
        string AddSpace(string s, char cg)
        {
            int i;
            try
            {
                for (i = 0; s[i] != cg; i++) ;

                int j = 0;

                for (j = i - 3; j > 0; j -= 3)
                {
                    try
                    {
                        s = s.Insert(j, " ");
                    }
                    catch { }
                }
            }
            catch
            {
            }
            return s;
        }

        protected void G_DataBinding(object sender, EventArgs e)
        {
            try
            {
                if (GRBS.SelectedNodeParent == GRBS.SelectedValue)
                {
                    p1.Value = "[" + GRBS.SelectedNodeParent + "]";
                }
                else
                {
                    p1.Value = "[" + GRBS.SelectedNodeParent + "].[" + GRBS.SelectedValue + "]";
                }

                string s = DataProvider.GetQueryText("G");

                CS = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(s);

                int colcount = CS.Axes[0].Positions.Count + CS.Axes[1].Positions[0].Members.Count;

                for (int i = 0; i < CS.Axes[1].Positions[0].Members.Count; i++)
                {
                    T1.Columns.Add(CS.Axes[1].Positions[0].Members[i].Caption.Replace("\"", "&quot;"));
                }
                T1.Columns.Add("value");

                for (int i = 0; i < (CS.Cells.Count); i++)
                {
                    object[] o = new object[colcount];
                    o[colcount - 1] = AddSpace(AddZero(Math.Round(float.Parse((CS.Cells[i].Value.ToString())) / 1000, 2).ToString()),',');

                    for (int j = 0; j < CS.Axes[1].Positions[i].Members.Count; j++)
                    {
                        o[j] = CS.Axes[1].Positions[i].Members[j].Caption.Replace("\"", "&quot;");
                    }


                    T1.Rows.Add(o);
                }
            }
            catch
            {
                T1 = null;
            }
            UltraWebGrid.DataSource = T1;
        }

        protected void LG_DataBinding(object sender, EventArgs e)
        {
            LC.DataSource = GetDSForChart("LC");
        }



        #region
        DataTable dtChart = new DataTable();
        public DataTable GetDSForChart(string sql)
        {
            dtChart = new DataTable();
            string s = DataProvider.GetQueryText(sql);
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(s, "Показатель", dtChart);
            return dtChart;
        }
        #endregion

        protected void RC_DataBinding(object sender, EventArgs e)
        {
            RC.DataSource = GetDSForChart("RC");
        }

        protected void LC_InvalidDataReceived(object sender, Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventArgs e)
        {
            setChartErrorFont(e);
        }

        protected void RC_InvalidDataReceived(object sender, Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventArgs e)
        {
            setChartErrorFont(e);
        }

        protected void UltraWebGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[e.Layout.Bands[0].Columns.Count - 1], "<DATA_VALUE:### ##0.##>");
        }
        protected void chart_FillSceneGraph(object sender, Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e)
        {
            int selectedYear = 2009;

            Color[] colors = new Color[5];
            colors[0] = Color.Green;
            colors[1] = Color.Coral;
            colors[2] = Color.DarkBlue;
            colors[3] = Color.Firebrick;
            colors[4] = Color.Ivory;

            IAdvanceAxis xAxis = (IAdvanceAxis)e.Grid["X"];
            IAdvanceAxis yAxis = (IAdvanceAxis)e.Grid["Y"];

            if (xAxis == null || yAxis == null)
                return;

            int xMin = (int)xAxis.MapMinimum;
            int yMin = (int)yAxis.MapMinimum;
            double yMinValue = Convert.ToDouble(yAxis.Minimum);
            int yMax = (int)yAxis.MapMaximum;
            int xMax = (int)xAxis.MapMaximum;
            double yMaxValue = Convert.ToDouble(yAxis.Maximum);

            Line line = new Line();
            line.lineStyle.DrawStyle = LineDrawStyle.Dash;
            line.PE.Stroke = Color.DarkGray;
            line.PE.StrokeWidth = 1;
            int yPosition = 0;
            if (yMaxValue != 0) yPosition = yMin - Convert.ToInt32(Convert.ToDouble(yMin - yMax) * (100 / yMaxValue));
            line.p1 = new Point(xMin, yPosition);
            line.p2 = new Point(xMax, yPosition);
            e.SceneGraph.Add(line);

            for (int i = 0; i < e.SceneGraph.Count; i++)
            {
                Primitive primitive = e.SceneGraph[i];
                if (primitive is Box)
                {
                    Box box = (Box)primitive;
                    if (box.DataPoint != null)
                    {
                        if (box.DataPoint.Label == ComboYear.SelectedValue)
                        {
                            if (Convert.ToDouble(dtChart.Rows[box.Row][dtChart.Columns.Count - 1]) < 100)
                            {
                                box.DataPoint.Label = string.Format("(ниже КУ) {0}", box.DataPoint.Label);
                                box.PE.Fill = Color.Green;
                                box.PE.FillStopColor = Color.ForestGreen;
                            }
                            else
                            {
                                box.DataPoint.Label = string.Format("(выше КУ) {0}", box.DataPoint.Label);
                                box.PE.Fill = Color.Red;
                                box.PE.FillStopColor = Color.Maroon;
                            }
                        }
                        else
                        {
                            box.PE.ElementType = PaintElementType.Hatch;
                            box.PE.Fill = colors[selectedYear - Convert.ToInt32(box.DataPoint.Label)];
                            box.PE.FillStopColor = Color.Transparent;
                            box.PE.Hatch = FillHatchStyle.Weave;
                            box.PE.FillOpacity = 100;
                            box.lineStyle.DrawStyle = LineDrawStyle.Dash;
                        }
                    }
                    else if (i != 0 && box.Path == "Legend")
                    {
                        Primitive primitive1 = e.SceneGraph[i - 0];
                        Primitive primitive2 = e.SceneGraph[i - 1];
                        if (primitive2 is Text && primitive1 is Box)
                        {
                            Text text = (Text)primitive2;
                            Box box1 = (Box)primitive1;
                            if (text.GetTextString() != ComboYear.SelectedValue)
                            {
                                box1.PE.ElementType = PaintElementType.Hatch;
                                box1.PE.Fill = colors[selectedYear - Convert.ToInt32(text.GetTextString())];
                                box1.PE.FillStopColor = Color.Transparent;
                                box1.PE.Hatch = FillHatchStyle.Weave;
                                box1.PE.FillOpacity = 100;
                                box1.lineStyle.DrawStyle = LineDrawStyle.Dash;
                            }
                            else
                            {
                                box.PE.Fill = Color.Green;
                                box.PE.FillStopColor = Color.ForestGreen;
                            }

                        }
                    }
                }
            }

            int textWidth = 200;
            int textHeight = 12;

            Text textLabel = new Text();
            textLabel.PE.Fill = Color.Black;
            textLabel.bounds = new Rectangle(xMax - textWidth, ((int)yAxis.Map(100)) - textHeight, textWidth, textHeight);
            textLabel.SetTextString(string.Format("100% контрольного уровня {0} года", ComboYear.SelectedValue));
            e.SceneGraph.Add(textLabel);
        }

        protected void LC_FillSceneGraph(object sender, Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e)
        {
        }
    }
}
