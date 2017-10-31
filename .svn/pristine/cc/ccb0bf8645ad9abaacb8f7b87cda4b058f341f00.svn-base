using System;
using System.IO;
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
using Infragistics.Documents.Excel;
using Infragistics.WebUI.UltraWebChart;
using Infragistics.UltraChart.Core.Primitives;
using System.Collections.ObjectModel;
using System.Text;
using System.Collections.Generic;
using Infragistics.UltraChart.Core;
using Color = System.Drawing.Color;
using Graphics = System.Drawing.Graphics;
using Image = System.Drawing.Image;
using TextAlignment = Infragistics.Documents.Reports.Report.TextAlignment;
using Dundas.Maps.WebControl;
using System.Drawing.Imaging;
using Infragistics.UltraGauge.Resources;

using Infragistics.UltraChart.Core.Layers;

using Infragistics.UltraChart.Core;

namespace Krista.FM.Server.Dashboards.reports.EO.EO_0007.EO_0004
{
    public partial class _default : CustomReportPage
    {
        private CustomParam baseRegion { get { return (UserParams.CustomParam("baseRegion")); } }
        private CustomParam curYear { get { return (UserParams.CustomParam("curYear")); } }
        private CustomParam lastYear { get { return (UserParams.CustomParam("lastYear")); } }
        private CustomParam curGRBS { get { return (UserParams.CustomParam("curGRBS")); } }
        private CustomParam curPBS { get { return (UserParams.CustomParam("curPBS")); } }
        private CustomParam curOKDP { get { return (UserParams.CustomParam("curOKDP")); } }
        private Int32 screen_width { get { return (int)Session["width_size"]; } }
        private CustomParam curOKDPLevel { get { return (UserParams.CustomParam("curOKDPLevel")); } }
        string page_title = "Размещение заказа в разрезе статей ОКДП в {0} году ({1})";
        string Ultrachart1_title = "Структура заказа по способам размещения за {0} год";
        string Ultrachart2_title = "Динамика размещения заказа по способам размещения, тыс. руб.";
        string BN = "IE";
        double index = 0;
        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);
            System.Web.HttpBrowserCapabilities myBrowserCaps = Request.Browser;
            BN = ((System.Web.Configuration.HttpCapabilitiesBase)myBrowserCaps).Browser.ToUpper();
            GRBS.Width = 839;
            OKPD_Level.Width = 200;
            GRBS.ParentSelect = true;
            RefreshPanel1.AddRefreshTarget(UltraChart1);
            RefreshPanel1.AddRefreshTarget(UltraChart2);
            RefreshPanel1.AddLinkedRequestTrigger(Grid);
            UltraGridExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            UltraGridExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
            UltraGridExporter1.ExcelExporter.EndExport += new EndExportEventHandler(ExcelExporter_EndExport);
            UltraGridExporter1.ExcelExporter.HeaderCellExporting += new HeaderCellExportingEventHandler(ExcelExporter_HeaderCellExporting);
            UltraGridExporter1.PdfExportButton.Visible = false;
            UltraChart1.Width = (int)((screen_width - 55) * 0.4);
            UltraChart2.Width = (int)((screen_width - 55) * 0.6);
            Grid.Width = (int)((screen_width - 34));
            Label3.Width = UltraChart1.Width;
            Label5.Width = UltraChart1.Width;

        }
        private double ParseFunc(string number)
        {
            if (number == "-")
            {
                return 0;
            }
            else
            {
                return double.Parse(number);
            }
        }
        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender,e);
            string oldRegion = RegionSettings.Instance.Id;
            RegionSettingsHelper.Instance.SetWorkingRegion("Tula");

            baseRegion.Value = RegionSettingsHelper.Instance.RegionBaseDimension;
            if (!Page.IsPostBack)
            {
                ComboYear.Title = "Год";
                GRBS.Title = "Заказчик";
                OKPD_Level.Title = "Уровень ОКДП";
                ComboYear.FillDictionaryValues(YearsLoad("years"));
                ComboYear.SelectLastNode();
                curYear.Value = ComboYear.SelectedValue;
                GRBS.FillDictionaryValues(GRBSLoad("GRBS"));
                OKPD_Level.FillDictionaryValues(OKPD_LevelLoad());

            }
            try
            {
                
                curYear.Value = ComboYear.SelectedValue;
                lastYear.Value = ComboYear.GetLastNode(0).Text; 
                GRBS.FillDictionaryValues(GRBSLoad("GRBS"));
                int m = int.Parse(ComboYear.SelectedValue) - 1;
                if (GRBS.SelectedValue == "Все заказчики")
                {
                    curGRBS.Value = " ";
                    curPBS.Value = " ";
                }
                else
                {
                    if (GRBS.SelectedNode.Level == 0)
                    {
                        curPBS.Value = " ";
                        curGRBS.Value =".["+ GRBS.SelectedValue+"]";
                    }
                    else
                    {
                        curGRBS.Value = ".[" + GRBS.SelectedNode.Parent.Text + "]";

                        curPBS.Value = ".[" + GRBS.SelectedValue + "]";
                    }
                }
                
                curOKDPLevel.Value = OKPD_Level.SelectedValue;
                Page.Title = String.Format(page_title, curYear.Value, GRBS.SelectedValue);
                Label1.Text = String.Format(page_title, curYear.Value, GRBS.SelectedValue);
                Label2.Text = String.Format(Ultrachart1_title,curYear.Value);
                Label3.Text = "(Все виды продуктов)";
                Label4.Text = Ultrachart2_title;
                Label5.Text = "(Все виды продуктов)";
                Grid.DataBind();
                Grid.DisplayLayout.AllowSortingDefault = AllowSorting.No;
                curOKDP.Value = " ";

            }
            catch
            { }
                try
                {
                    if (Grid.DataSource != null)
                    {
                        Grid.Rows.Add();
                        double itogCol = 0;
                        Grid.Rows[Grid.Rows.Count - 1].Cells[0].Text = "Общий итог";
                        for (int i = 2; i < Grid.Columns.Count; i++)//обнуление итога по столбцам
                        {
                            Grid.Rows[Grid.Rows.Count - 1].Cells[i].Text = "0";
                            
                        }
                        for (int i = 0; i < Grid.Rows.Count - 1; i++)//подсчет итога по столбцам
                        {
                            for (int j = 2; j < Grid.Columns.Count; j++)
                            {

                                itogCol = ParseFunc(Grid.Rows[i].Cells[j].Text) + ParseFunc(Grid.Rows[Grid.Rows.Count - 1].Cells[j].Text);
                                Grid.Rows[Grid.Rows.Count - 1].Cells[j].Text = itogCol.ToString();
                            }
                        }
                        for (int i = 0; i < Grid.Rows.Count; i++)//удаление пустых строк
                        {
                            bool flag = false;
                            for (int j = 3; j < Grid.Columns.Count; j=j+2)
                            {
                                if (Grid.Rows[i].Cells[j].Text != "0")
                                {
                                    flag = true;
                                }
                                if (Grid.Rows[i].Cells[j].Text == "0")
                                {
                                    Grid.Rows[i].Cells[j].Text = "-";
                                }
                            }
                            if (flag == false)
                            {
                                Grid.Rows.Remove(Grid.Rows[i]);
                                i = i - 1;
                            }
                        }
                        Grid.Columns[1].Move(0);
                        UltraChart1.DataBind();
                        UltraChart2.DataBind();
                        double columnCol = 0;
                        if (curYear.Value != ComboYear.GetRootNodesName(0))
                        {
                            Grid.Columns[3].Move(Grid.Columns.Count - 1);
                            columnCol = (Grid.Columns.Count - 2) / 2;
                        }
                        else
                        {
                            Grid.Columns[2].Move(Grid.Columns.Count - 1);
                            columnCol = Grid.Columns.Count-2;
                        }
                        Grid.Columns[Grid.Columns.Count - 1].Header.Caption = "Общий итог";
                        int colRows = 0;
                        if (Grid.Rows.Count >= 12)
                        {
                            colRows = 12;
                        }
                        else
                        {
                            colRows = Grid.Rows.Count;
                        }
                        if (BN == "FIREFOX")
                        {

                            Grid.Height = colRows * 28;
                            index = 0.908;
                        }
                        if (BN=="IE")
                        {
                            Grid.Height = colRows * 24;
                            index = 0.9;
                        }
                        if (BN == "APPLEMAC-SAFARI")
                        {
                            Grid.Height = colRows * 24;
                            index = 0.89;
                        }
                         double widthcol=0;
                         bool flag1 = false;
                         for (int i = 2; i < Grid.Columns.Count-1; i++)//удаление пустых столбцов
                         {
                             flag1 = false;
                             for (int j = 0; j < Grid.Rows.Count - 1; j++)
                             {
                                 if (Grid.Rows[j].Cells[i].Text != "-")
                                 {
                                     flag1 = true;
                                 }
                             }
                             if (flag1 == false)
                             {
                                 Grid.Columns.Remove(Grid.Columns[i]);
                                 i = i - 1;
                                 columnCol -= 1;
                             }
                         
                         }
                             if (Grid.Rows.Count > 13)
                             {
                                 widthcol = index - (columnCol * 0.07 + 0.05);
                             }
                             else
                             {
                                 widthcol = index + 0.02 - (columnCol * 0.07 + 0.05);
                             }
                        Grid.Columns[1].Width = CRHelper.GetColumnWidth(CustomReportConst.minScreenWidth * widthcol);
                        Grid.Rows[Grid.Rows.Count - 1].Cells[0].Text = "Общий итог";
                        Grid.Rows[Grid.Rows.Count - 1].Cells[0].ColSpan = 2;
                        Grid.Rows[Grid.Rows.Count - 1].Cells[0].Style.Font.Bold = 1 == 1;
                        Grid.Rows[Grid.Rows.Count - 1].Cells[0].Style.HorizontalAlign = HorizontalAlign.Left;
                    }
                    else
                    {
                        Grid.DisplayLayout.NoDataMessage = "В настоящий момент данные отсутствуют";
                    }
            }

            catch 
            {

            }
            RegionSettingsHelper.Instance.SetWorkingRegion("Tula");
           
        }
        private int DotCount(string s)
        {
            int k = 0;
            for (int i = 0; i < s.Length; i++)
            {
                if (s[i] == '.')
                {
                    k += 1;
                }
            }
            return k;
        }
        Dictionary<string, int> YearsLoad(string sql)
        {
            CellSet cs = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(DataProvider.GetQueryText(sql));
            Dictionary<string, int> d = new Dictionary<string, int>();
            for (int i = 0; i <= cs.Axes[1].Positions.Count - 1; i++)
            {

                d.Add(cs.Axes[1].Positions[i].Members[0].Caption, 0);
            }

            return d;
        }
        Dictionary<string, int> GRBSLoad(string sql)
        {
            CellSet cs = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(DataProvider.GetQueryText(sql));
            Dictionary<string, int> d = new Dictionary<string, int>();
            d.Add("Все заказчики",0);
            for (int i = 1; i <= cs.Axes[1].Positions.Count - 1; i++)
            {

                    if (DotCount(cs.Axes[1].Positions[i].Members[0].UniqueName) == 3)
                    {
                        d.Add(cs.Axes[1].Positions[i].Members[0].Caption, 0);
                    }
                    else
                    {
                        
                        d.Add(cs.Axes[1].Positions[i].Members[0].Caption, 1);
                    }
          
            }

            return d;
        }
        Dictionary<string, int> OKPD_LevelLoad()
        {
            Dictionary<string, int> d = new Dictionary<string, int>();
            d.Add("Уровень 1", 0);
            d.Add("Уровень 2", 0);
            d.Add("Уровень 3", 0);
            d.Add("Уровень 4", 0);
            d.Add("Уровень 5", 0);
            d.Add("Уровень 6", 0);
            return d;
        }

        protected void Grid_DataBinding(object sender, EventArgs e)
        {
            try
            {
                Grid.Rows.Clear();
                Grid.Columns.Clear();
                DataTable dt1 = new DataTable();
                DataTable dt = new DataTable();
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("Grid"), "Наименование статьи", dt);

                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        dt1.Columns.Add(dt.Columns[i].ColumnName, dt.Columns[i].DataType);
                    }
                
                object[] o = new object[dt1.Columns.Count];
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    o[0] = dt.Rows[i].ItemArray[0];
                    o[1] = dt.Rows[i].ItemArray[1];
                    for (int j = 2; j < dt.Columns.Count; j++)
                    {
                        if (dt.Rows[i].ItemArray[j].ToString() == "")
                        {
                            o[j] = 0;
                        }
                        else
                        {
                            o[j] = double.Parse(dt.Rows[i].ItemArray[j].ToString()) / 1000;
                        }
                    }
                    dt1.Rows.Add(o);
                }
                for (int i = 0; i < o.Length; i++)
                {
                    o[i] = dt1.Rows[1].ItemArray[i];
                }
                dt1.Rows.Add(o);
                dt1.Rows.Remove(dt1.Rows[1]);
                string colName = "";
                int col = dt1.Columns.Count;
               /* if (curYear.Value != ComboYear.GetRootNodesName(0))//удадение столбцов предыдущего года
                {


                    for (int i = 2; i < dt1.Columns.Count; i+=2)
                    {
                        Label1.Text = Label1.Text + " " + i.ToString();
                        if (dt1.Columns[i].ColumnName.Split(';')[1] == dt1.Columns[i + 1].ColumnName.Split(';')[1])
                        {
                            dt1.Columns.Remove(dt1.Columns[i]);
                            i = i - 2;
                        }
                    }

                }*/
                if (dt1.Columns.Count <= 2)
                { 
                    Grid.DataSource = null;
                    Grid.Columns.Clear();
                    Grid.DisplayLayout.NoDataMessage = "В настоящее время данные отсутствуют";
                }
                else
                {
                    Grid.DataSource = dt1;
                }
            }
            catch { }

        }

        protected void Grid_InitializeRow(object sender, RowEventArgs e)
        {
            try
            {
                if (curYear.Value != ComboYear.GetRootNodesName(0))
                {
                    for (int i = 3; i < e.Row.Cells.Count; i=i+2)
                    {
                        if (!((ParseFunc(e.Row.Cells[i].Text)==0)||(ParseFunc(e.Row.Cells[i-1].Text)==0)))
                        {
                        double m= ParseFunc(e.Row.Cells[i].Text)-ParseFunc(e.Row.Cells[i-1].Text);
                        if (m > 0)
                        {
                            e.Row.Cells[i].Style.CssClass = "ArrowUpGreen";
                            e.Row.Cells[i].Title = "Выше на " + String.Format("{0:# ##0.00}", m) + " тыс.руб. больше, чем в прошлом году";
                        }
                        if (m < 0)
                        {
                            e.Row.Cells[i].Style.CssClass = "ArrowDownGreen";
                            m = Math.Abs(m);
                            e.Row.Cells[i].Title = "Ниже на " + String.Format("{0:# ##0.00}", m) + " тыс.руб. меньше, чем в прошлом году";
                        }
                        }
                    }
                }
            }
            catch { }
               
        }

        protected void Grid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            try
            {
                if (curYear.Value != ComboYear.GetRootNodesName(0))
                {
                    for (int i = 2; i < e.Layout.Bands[0].Columns.Count; i = i + 2)
                    {
                        e.Layout.Bands[0].Columns[i].Hidden = 1 == 1;
                    }
                }
                e.Layout.Bands[0].Columns[0].CellStyle.Wrap = 1 == 1;
                e.Layout.Bands[0].Columns[0].Header.Style.Wrap = 1 == 1;
                e.Layout.Bands[0].Columns[0].Header.Style.HorizontalAlign = HorizontalAlign.Center;
                e.Layout.Bands[0].Columns[1].CellStyle.HorizontalAlign = HorizontalAlign.Center;
                e.Layout.Bands[0].Columns[1].Header.Caption = e.Layout.Bands[0].Columns[1].Key.Split(';')[0];
                e.Layout.Bands[0].Columns[1].Header.Style.Wrap = 1 == 1;
                e.Layout.Bands[0].Columns[1].Header.Style.HorizontalAlign = HorizontalAlign.Center;
                e.Layout.Bands[0].Columns[1].Width = CRHelper.GetColumnWidth(CustomReportConst.minScreenWidth * 0.05);
                e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(CustomReportConst.minScreenWidth * 0.4);
                for (int i = 2; i < e.Layout.Bands[0].Columns.Count; i++)
                {
                    CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "### ### ##0.##");
                    e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Center;
                    e.Layout.Bands[0].Columns[i].Header.Caption = e.Layout.Bands[0].Columns[i].Key.Split(';')[0]+" тыс.руб.";
                    e.Layout.Bands[0].Columns[i].Header.Style.Wrap = 1 == 1;
                    e.Layout.Bands[0].Columns[i].Header.Style.HorizontalAlign = HorizontalAlign.Center;
                    e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(CustomReportConst.minScreenWidth * 0.07);
                }
            }
            catch
            { }



        }
        DataTable UltraChart1dt1;
        protected void UltraChart1_DataBinding(object sender, EventArgs e)
        {
            try
            {
            DataTable dt = new DataTable();
            UltraChart1dt1 = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("Chart1"), " ", dt);
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                UltraChart1dt1.Columns.Add(dt.Columns[i].ColumnName, dt.Columns[i].DataType);
            }
            object[] o = new object[UltraChart1dt1.Columns.Count];
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                o[0]=dt.Rows[i].ItemArray[0];
                o[1] = double.Parse(dt.Rows[i].ItemArray[1].ToString()) / 1000;
                UltraChart1dt1.Rows.Add(o);
            }
            UltraChart1.DataSource = UltraChart1dt1;
        }
        catch { }
        }

        protected void UltraChart2_DataBinding(object sender, EventArgs e)
        {
            try
            {
                DataTable dt = new DataTable();
                DataTable dt1 = new DataTable();
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("Chart2"), " ", dt);
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    dt1.Columns.Add(dt.Columns[i].ColumnName, dt.Columns[i].DataType);
                }
                object[] o = new object[dt1.Columns.Count];
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    o[0] = dt.Rows[i].ItemArray[0];
                    for (int j = 1; j < dt.Columns.Count; j++)
                    {
                        if (dt.Rows[i].ItemArray[j].ToString() == "")
                        {
                            o[j] = 0;
                        }
                        else
                        {
                            o[j] = double.Parse(dt.Rows[i].ItemArray[j].ToString()) / 1000;
                        }
                    }
                    dt1.Rows.Add(o);
                }
                UltraChart2.DataSource = dt1;

            }
            catch { }
        }

        protected void Grid_ActiveRowChange(object sender, RowEventArgs e)
        {
            try
            {
                if (e.Row.Cells[1].Text != "Общий итог")
                {
                    curOKDP.Value = ".[" + e.Row.Cells[1].Text + "]";
                    Label3.Text = e.Row.Cells[1].Text;
                    Label5.Text = e.Row.Cells[1].Text;
                    Label5.Height = Label3.Height;
                    UltraChart1.DataBind();
                    UltraChart2.DataBind();
                }
                else
                {
                    curOKDP.Value = " ";
                    Label3.Text = "(Все виды продуктов)";
                    Label5.Text = "(Все виды продуктов)";
                    UltraChart1.DataBind();
                    UltraChart2.DataBind();
                }
                UltraChart2.Axis.X.Labels.SeriesLabels.Font = new Font("Verdana", 9);
            }
            catch { }
        }
        protected void SetErorFonn(object sender, Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventArgs e)
        {
            try
            {
                e.Text = "В настоящий момент данные отсутствуют";
                e.LabelStyle.FontColor = System.Drawing.Color.LightGray;
                e.LabelStyle.VerticalAlign = StringAlignment.Center;
                e.LabelStyle.HorizontalAlign = StringAlignment.Center;
                e.LabelStyle.Font = new System.Drawing.Font("Verdana", 30);
            }
            catch { }
        }
        #region Экспорт Excel

        private void ExcelExporter_BeginExport(object sender, BeginExportEventArgs e)
        {


        }


        private void ExcelExporter_EndExport(object sender, EndExportEventArgs e)
        {
            try
            {
                string formatString = "#,##0.00;[Red]-#,##0.00";
                e.CurrentWorksheet.Columns[1].Width = 650 * 37;
                e.CurrentWorksheet.Columns[0].Width = 300 * 37;
                e.CurrentWorksheet.Rows[0].Height = 500;
                for (int i = 2; i < Grid.Bands[0].Columns.Count; i++)
                {
                    e.CurrentWorksheet.Columns[i].Width = 225 * 37;
                    e.CurrentWorksheet.Columns[i].Hidden = false;
                    e.CurrentWorksheet.Columns[i].CellFormat.WrapText = Infragistics.Documents.Excel.ExcelDefaultableBoolean.True;
                }
                //ширина первого столбца

                for (int i = 0; i < Grid.Rows.Count; i++)
                {
                    e.CurrentWorksheet.Rows[i + 1].Cells[0].CellFormat.WrapText = Infragistics.Documents.Excel.ExcelDefaultableBoolean.True;
                    e.CurrentWorksheet.Rows[i + 1].Cells[1].CellFormat.WrapText = Infragistics.Documents.Excel.ExcelDefaultableBoolean.True;
                    e.CurrentWorksheet.Rows[i + 1].Height = 500;

                }
            }
            catch { }

        }

        private void ExcelExporter_HeaderCellExporting(object sender, HeaderCellExportingEventArgs e)
        {

        }

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            try
            {
                Workbook workbook = new Workbook();
                Worksheet sheet1 = workbook.Worksheets.Add("Таблица");
                UltraGridExporter1.ExcelExporter.ExcelStartRow = 1;
                UltraGridExporter1.ExcelExporter.DownloadName = "report.xls";
                UltraGridExporter1.ExcelExporter.Export(Grid, sheet1);
            }
            catch { }
        }

        #endregion

        protected void UltraChart1_FillSceneGraph(object sender, Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e)
        {
            for (int i = 0; i < UltraChart1dt1.Rows.Count; i++)
            {
                Infragistics.UltraChart.Core.Primitives.Text textLabel = new Infragistics.UltraChart.Core.Primitives.Text(new Rectangle(34, 300 + i * 20 - i, 320, 10), UltraChart1dt1.Rows[i].ItemArray[0].ToString(), new LabelStyle(new Font("Verdana", 8), Color.Black, true, false, false, StringAlignment.Near, StringAlignment.Near, TextOrientation.Horizontal));
                e.SceneGraph.Add(textLabel);
            }
        }



    }
}
