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
namespace Krista.FM.Server.Dashboards.reports.EO.EO_0004new.EO_0004LeninRegionNew1
{
    public partial class _default : CustomReportPage
    {
        private CustomParam baseRegion { get { return (UserParams.CustomParam("baseRegion")); } }
        private CustomParam curYear { get { return (UserParams.CustomParam("curYear")); } }
        private CustomParam firstYear { get { return (UserParams.CustomParam("firstYear")); } }
        private CustomParam dataBase { get { return (UserParams.CustomParam("dataBase")); } }
        private CustomParam pokPrefix { get { return (UserParams.CustomParam("pokPrefix")); } }
        private string page_title = "Оценка качества жизни населения";
        string page_title2 = "Оценка качества жизни населения по основным направлениям: демография, образование, здравоохранение, социальная защита, культура и отдых, жилищные условия, экономика, {0}, {1} год";
        private string grid_title = "Интегральный показатель уровня жизни населения и его составляющие";
        private string chart1_title = "Структура интегрального показателя";
        private string chart2_title = "Темп прироста интегрального показателя по отношению к базовому году, %";
        private int offsetX = 0;
        private int startOffsetX = 0;
        private Dictionary<string, int> widths = new Dictionary<string, int>();
        private Dictionary<string, int> widths2 = new Dictionary<string, int>();
        private Dictionary<string, int> kindNumbers = new Dictionary<string, int>();
        private int widthAll = 0;
        private object[] ColumnNanes;
        private int screen_width { get { return (int)Session["width_size"]; } }
        private GridHeaderLayout headerLayout;
        private static bool IsSmallResolution
        {
            get { return CRHelper.GetScreenWidth < 900; }
        }
        string BN = "IE";

        protected override void Render(HtmlTextWriter writer)
        {
            base.Render(writer);

        }
        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);
            Grid.Width = 1200;
            Label6.Width = 1210;
            Chart1.Width = 600;
            Chart2.Width = 600;
            Label4.Width = 500;
            ComboRegion.Width = 450;
            #region Настройка диаграммы
                Chart1.Border.Thickness = 0;
                Chart1.Axis.Y.Extent = 28;
                Chart1.Axis.X.Extent = 35;
                Chart1.Tooltips.FormatString = "<ITEM_LABEL>";
                Chart1.Legend.Visible = true;
                Chart1.Legend.Location = LegendLocation.Bottom;
                Chart1.Legend.SpanPercentage = 24;
                Chart1.Axis.X.Labels.SeriesLabels.Visible = false;
                Chart1.Axis.X.Labels.Orientation = TextOrientation.Horizontal;
                Chart1.Axis.X.Labels.FontColor = Color.Black;
                Chart1.Data.ZeroAligned = true;
                Chart1.FillSceneGraph += new FillSceneGraphEventHandler(Chart1_FillSceneGraph);
                Chart1.ColorModel.ModelStyle = ColorModels.PureRandom;
                //Chart1.ChartDrawItem += new ChartDrawItemEventHandler(Chart1_ChartDrawItem);
                Chart1.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N0>";
                System.Drawing.Font font = new System.Drawing.Font("verdana", 10f);
                Chart1.Axis.Y.Labels.Font = font;
                Chart1.Axis.X.Labels.Font = font;
            #endregion
            HyperLink1.NavigateUrl = "../../../../EO/EO_0004new/EO_0004_LeninRegionNew2/default.aspx";
            if (BN == "IE")
            {
                Chart2.Height = 383;
            }
            else
            {
                if (BN == "FIREFOX")
                {
                    Chart2.Height = 383;
                }
                else
                {
                    Chart2.Height = 383;
                }
            }
            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportExcelExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
            ReportExcelExporter1.ExcelExporter.EndExport += new EndExportEventHandler(ExcelExporter_EndExport);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);

            ReportPDFExporter1.PdfExporter.RowExporting += new EventHandler<Infragistics.WebUI.UltraWebGrid.DocumentExport.RowExportingEventArgs>(ultraWebGridDocumentExporter_RowExporting);
            Grid.DisplayLayout.AllowSortingDefault = AllowSorting.No;
            Grid.DisplayLayout.AllowSortingDefault = AllowSorting.No;
            Grid.DisplayLayout.CellClickActionDefault = CellClickAction.NotSet;
            Grid.DisplayLayout.RowSelectorStyleDefault.Width = 0;
            Grid.DisplayLayout.NoDataMessage = "В настоящий момент данные не доступны";
        }
        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender,e);
            if (!Page.IsPostBack)
            {
                ComboRegion.Title = "Территория";
                ComboRegion.FillDictionaryValues(RegionsLoad("regions"));
                ComboRegion.ParentSelect = true;
                ComboRegion.SetСheckedState(ComboRegion.GetRootNodesName(0), true);

                if (ComboRegion.SelectedNode.Level == 0)
                {
                    baseRegion.Value = "[Территории__Сопоставимый].[Территории__Сопоставимый].[Все территории].[Российская Федерация].[Уральский федеральный округ].[Ханты-Мансийский автономный округ].DATAMEMBER";
                    dataBase.Value = "[ЭО_КЖН_Ленинградская область]";
                    pokPrefix.Value = "[КЖН__Ленинградская область].[КЖН__Ленинградская область]";
                }
                else
                {
                    baseRegion.Value = "[Территории__РФ].[Территории__РФ].[Все территории].[Российская Федерация].[Уральский федеральный округ].[Тюменская область с Ханты-Мансийским автономным округом, Ямало-Ненецким автономным округом].[Ханты-Мансийский автономный округ - Югра].[" + ComboRegion.SelectedValue + "]";
                    
                    dataBase.Value = "[ЭО_КЖН_Ленинградская область МО]";
                    
                    pokPrefix.Value = "[КЖН__Ленинградская область МО].[КЖН__Ленинградская область МО]";
                }

                ComboYear.Title = "Год";
                ComboYear.FillDictionaryValues(YearsLoad("Years"));
                ComboYear.SelectLastNode();
            }

            System.Web.HttpBrowserCapabilities myBrowserCaps = Request.Browser;
            BN = ((System.Web.Configuration.HttpCapabilitiesBase)myBrowserCaps).Browser.ToUpper();
            if (ComboRegion.SelectedNode.Level == 0)
            {
                baseRegion.Value = "[Территории__Сопоставимый].[Территории__Сопоставимый].[Все территории].[Российская Федерация].[Уральский федеральный округ].[Ханты-Мансийский автономный округ].DATAMEMBER";
                dataBase.Value = "[ЭО_КЖН_Ленинградская область]";
                pokPrefix.Value = "[КЖН__Ленинградская область].[КЖН__Ленинградская область]";
                Chart2.Axis.X.Labels.ItemFormatString = "2006";
            }
            else 
            {
                baseRegion.Value = "[Территории__РФ].[Территории__РФ].[Все территории].[Российская Федерация].[Уральский федеральный округ].[Тюменская область с Ханты-Мансийским автономным округом, Ямало-Ненецким автономным округом].[Ханты-Мансийский автономный округ - Югра].[" + ComboRegion.SelectedValue + "]";
                
                dataBase.Value = "[ЭО_КЖН_Ленинградская область МО]";
                
                pokPrefix.Value = "[КЖН__Ленинградская область МО].[КЖН__Ленинградская область МО]";
                Chart2.Axis.X.Labels.ItemFormatString = "2008";
            }
            Grid.Height = Unit.Empty;
            curYear.Value = ComboYear.SelectedValue;
            firstYear.Value = ComboYear.GetRootNodesName(0);
            Label1.Text = page_title;
            Label2.Text = grid_title;
            Label3.Text = chart1_title;
            Label4.Text = chart2_title;
            Label5.Text =String.Format(page_title2,ComboRegion.SelectedValue,ComboYear.SelectedValue);
            headerLayout = new GridHeaderLayout(Grid);
            Grid.DataBind();
            if (Grid.DataSource != null)
            {

                for (int i = 1; i < Grid.Rows[2].Cells.Count; i++)
                {
                    Grid.Rows[2].Cells[i].Text = "<font font-family='Verdana'>Весовой коэффициент<br>" + String.Format("{0:0.00}", GetNumber(Grid.Rows[2].Cells[i].Text)) + "<br> <a  href='../01/default1.aspx?paramlist=Pokazatel=[" + Grid.Columns[i].Header.Caption + "]'>Подробнее...</a></font>";
                }
                for (int i = 0; i < Grid.Rows.Count - 1; i++)
                {
                    Grid.Rows[i].Style.Height = 65;
                }

                Chart1.DataBind();
                Chart2.DataBind();
            }
            
        }
        Dictionary<string, int> YearsLoad(string sql)
        {
            DataTable dt = new DataTable();
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText(sql), "Года", dt);
            Dictionary<string, int> d = new Dictionary<string, int>();
            for (int i = 1; i < dt.Columns.Count; i++)
            {
                d.Add(dt.Columns[i].ColumnName, 0);
            }
                return d;
        }

        Dictionary<string, int> RegionsLoad(string sql)
        {
            DataTable dt = new DataTable();
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText(sql), "Регионы", dt);
            Dictionary<string, int> d = new Dictionary<string, int>();
            d.Add(dt.Rows[0].ItemArray[0].ToString(), 0);
            for (int i = 1; i < dt.Rows.Count; i++)
            {
                d.Add(dt.Rows[i].ItemArray[0].ToString(), 1);
            }
            return d;
        }


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

        #region Обработчики грида

        protected void Grid_DataBinding(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();
            DataTable dt1 = new DataTable();
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("Grid"), " ", dt);
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("Grid1"), " ", dt1);
            if (dt.Rows.Count < 1)
            {
                Grid.DataSource = null;
                Label6.Text = "Нет данных";
            }
            else
            {
                object[] o = new object[dt.Columns.Count];

                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    o[i] = dt.Rows[0].ItemArray[i];
                }
                dt.Rows.Add(o);
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    o[i] = dt1.Rows[0].ItemArray[i];
                }
                dt.Rows.Add(o);
                Grid.DataSource = dt;

                double d = (GetNumber(dt.Rows[0].ItemArray[0].ToString()) - 1) * 100;

                //Формирование динамичсекого текста
                Label6.Text = "&nbsp;&nbsp;&nbsp;В <b>" + ComboYear.SelectedValue + "</b> году <b>интегральный показатель уровня жизни населения</b> составил <b>" + String.Format("{0: 0.0000}", GetNumber(dt.Rows[0].ItemArray[0].ToString())) + "</b>.<br>&nbsp;&nbsp;&nbsp;Это свидетельствует ";
                string AdvancedString = "";
                if (GetNumber(dt.Rows[0].ItemArray[0].ToString()) > 1)
                {
                    Label6.Text = Label6.Text + "<b>об улучшении</b> общего социально-экономического состояния территории за отчетный год <b>на " + Math.Round(d, 2) /*String.Format("{0:# ##.00}", d).Remove(1, 1)*/ + "%. ";
                    AdvancedString = "</b><br>&nbsp;&nbsp;&nbsp;На изменение качества жизни населения повлияло";
                }
                if (GetNumber(dt.Rows[0].ItemArray[0].ToString()) < 1)
                {
                    Label6.Text = Label6.Text + "<b>об ухудшении</b> общего социально-экономического состояния территории за отчетный год <b>на " + Math.Round(d, 2) /*String.Format("{0:# ##.00}", d).Remove(1, 1)*/ + "%. ";
                    AdvancedString = "</b><br>&nbsp;&nbsp;&nbsp;На изменение качества жизни населения повлияло";
                }
                if (GetNumber(dt.Rows[0].ItemArray[0].ToString()) == 1)
                {
                    Label6.Text = Label6.Text + "о том, что общая социально-экономическая ситуация территории за отчетный год <b>не изменилась. ";
                    AdvancedString = "</b><br>&nbsp;&nbsp;&nbsp;В то же время наблюдалось";
                }
                string s = AdvancedString + " ухудшение состояния сфер: <b>";
                int k = 0;
                for (int i = 1; i < dt.Columns.Count; i++)
                {
                    if (GetNumber(dt.Rows[0].ItemArray[i].ToString()) < 1)
                    {
                        k += 1;
                        s = s + dt.Columns[i].ColumnName.ToLower() + ", ";
                    }

                }
                if (k == 0)
                {
                    s = AdvancedString + " улучшение состояния сфер: <b>";
                }
                else
                {
                    Label6.Text = Label6.Text + s + "</b>";
                    s = "</b>а также улучшение состояния сфер: <b>";
                }
                k = 0;
                for (int i = 1; i < dt.Columns.Count; i++)
                {
                    if (GetNumber(dt.Rows[0].ItemArray[i].ToString()) > 1)
                    {
                        k += 1;
                        s = s + dt.Columns[i].ColumnName.ToLower() + ", ";
                    }
                }
                if (k == 0)
                {
                    Label6.Text = Label6.Text.Remove(Label6.Text.Length - 6, 2);
                    Label6.Text += ".";
                }
                else
                {
                    Label6.Text = Label6.Text + s;
                    Label6.Text = Label6.Text.Remove(Label6.Text.Length - 2, 2);
                    Label6.Text += "</b>.";
                }
            }

        }

        protected void Grid_InitializeRow(object sender, RowEventArgs e)
        {
            if (e.Row.Index == 1)
            {
                for (int i = 0; i < e.Row.Cells.Count; i++)//
                {
                    if (GetNumber(e.Row.Cells[i].Text.ToString()) > 1)
                    {
                        Grid.Rows[0].Cells[i].Style.CustomRules = "background-repeat: no-repeat;background-position: center;";
                        Grid.Rows[0].Cells[i].Style.BackgroundImage = "~/images/k_green.png";
                        Grid.Rows[1].Cells[i].Style.BackColor = Color.White;
                        Grid.Rows[0].Cells[i].Style.BackColor = Color.White;
                        Grid.Rows[1].Cells[i].Style.Font.Bold = true;
                        Grid.Rows[1].Cells[i].Style.Font.Name = "Verdana";
                        Grid.Rows[1].Cells[i].Style.Font.Size = 16;
                        Grid.Rows[1].Cells[i].Text = Grid.Rows[0].Cells[i].Text;
                        Grid.Rows[1].Cells[i].Style.HorizontalAlign = HorizontalAlign.Center;
                        Grid.Rows[0].Cells[i].Text = "";
                    }
                    else
                    {
                        if (GetNumber(e.Row.Cells[i].Text.ToString()) < 1)
                        {
                            Grid.Rows[1].Cells[i].Style.CustomRules = "background-repeat: no-repeat;background-position: center;";
                            Grid.Rows[1].Cells[i].Style.BackgroundImage = "~/images/k_red.png";
                            Grid.Rows[0].Cells[i].Style.BackColor = Color.White;
                            Grid.Rows[1].Cells[i].Style.BackColor = Color.White;
                            Grid.Rows[0].Cells[i].Style.Font.Bold = true;
                            Grid.Rows[0].Cells[i].Style.Font.Name = "Verdana";
                            Grid.Rows[0].Cells[i].Style.Font.Size = 16;
                            Grid.Rows[0].Cells[i].Text = Grid.Rows[1].Cells[i].Text;
                            Grid.Rows[0].Cells[i].Style.HorizontalAlign = HorizontalAlign.Center;
                            Grid.Rows[1].Cells[i].Text = "";
                        }
                        else
                        {
                            Grid.Rows[0].Cells[i].Style.BackColor = Color.White;
                            Grid.Rows[1].Cells[i].Style.BackColor = Color.White;
                            Grid.Rows[0].Cells[i].Style.Font.Bold = true;
                            Grid.Rows[0].Cells[i].Style.Font.Name = "Verdana";
                            Grid.Rows[0].Cells[i].Style.Font.Size = 16;
                            Grid.Rows[0].Cells[i].Text = Grid.Rows[1].Cells[i].Text;
                            Grid.Rows[0].Cells[i].Style.HorizontalAlign = HorizontalAlign.Center;
                            Grid.Rows[1].Cells[i].Text = "";
                        }
                    }

                }

            }
            for (int i = 0; i < e.Row.Cells.Count; i++)//
            {
                e.Row.Cells[i].Style.Font.Name = "Verdana";
            }


        }

        protected void Grid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.Bands[0].Columns[0].Header.Caption = e.Layout.Bands[0].Columns[0].Key.ToUpper();
            for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
            {
                e.Layout.Bands[0].Columns[i].Header.Style.Wrap = 1 == 1;
                if (BN == "IE")
                {
                    e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(CustomReportConst.minScreenWidth * 0.113);
                }
                if (BN == "FIREFOX")
                {
                    e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(CustomReportConst.minScreenWidth * 0.113);
                }
                if (BN == "APPLEMAC-SAFARI")
                {
                    e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(CustomReportConst.minScreenWidth * 0.1145);
                }
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Center;
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "### ##0.0000");
                e.Layout.Bands[0].Columns[i].Header.Style.HorizontalAlign = HorizontalAlign.Center;
                e.Layout.Bands[0].Columns[i].CellStyle.Wrap = 1 == 1;
            }

            GridHeaderCell header = headerLayout.AddCell(e.Layout.Bands[0].Columns[0].Header.Caption);
            for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i++)
            {
                header = headerLayout.AddCell(e.Layout.Bands[0].Columns[i].Header.Caption);
            }
        }

        #endregion

        #region Обработчики Диаграммы1

        DataTable dtChartAverage;
        DataTable dtChart;
        DataTable dtChart12;
        DataTable dtChart13;
        DataTable orderedTable;
        protected void Chart1_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText(String.Format("Chart1"));
            dtChart = new DataTable();
            DataTable dt = new DataTable();
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Дата", dt);

            query = DataProvider.GetQueryText(String.Format("Chart1_2"));
            dtChart12 = new DataTable();
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Дата", dtChart12);
            orderedTable = new DataTable();
            dtChart13 = new DataTable();
            query = DataProvider.GetQueryText(String.Format("Chart1_3"));
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Дата", orderedTable);
            if (dt.Rows.Count < 1)
            {
                Chart1.DataSource = null;
            }
            else
            {
                for (int i = 0; i < orderedTable.Columns.Count; i++)
                {
                    dtChart13.Columns.Add(i.ToString(), orderedTable.Columns[i].DataType);
                }
                object[] rowsObj = new object[dtChart13.Columns.Count];
                for (int i = 0; i < orderedTable.Rows.Count; i++)
                {
                    for (int j = 0; j < orderedTable.Rows[i].ItemArray.Length; j++)
                    {
                        rowsObj[j] = orderedTable.Rows[i].ItemArray[j];
                    }
                    dtChart13.Rows.Add(rowsObj);
                }

                int counter = 1;
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    dtChart.Columns.Add(counter.ToString(), dt.Columns[i].DataType);
                    counter += 1;
                }
                object[] o = new object[dtChart.Columns.Count];
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    o[i] = dt.Rows[0].ItemArray[i];
                    dtChart.Columns[i].ColumnName = dt.Columns[i].ColumnName + "<br>оценка <b>" + String.Format("{0:# ##0.0000}", GetNumber(dt.Rows[0].ItemArray[i].ToString())) + "</b><br>вес <b>" + String.Format("{0:# ##0.00}", GetNumber(dtChart13.Rows[0].ItemArray[i + 1].ToString())) + "</b>";
                }
                dtChart.Rows.Add(o);
                Chart1.DataSource = dtChart;
            }
        }

        void Chart1_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            if (Chart1.DataSource != null)
            {
                int count = e.SceneGraph.Count;
                for (int i = 0; i < count; i++)
                {
                    Primitive primitive = e.SceneGraph[i];
                    if (primitive is Box)
                    {
                        Box box = (Box)primitive;
                        if (box.DataPoint != null)
                        {
                            widthAll += box.rect.Width - 2;
                        }
                        else
                        {
                            if ((box.Column >= 0) && (box.Column <= 7))
                            {
                                box.Visible = false;
                            }
                        }
                    }
                }
                offsetX = 0;
                int j = 1;
                int labelCount = 1;
                int sceneGraphCol = e.SceneGraph.Count;
                for (int i = 0; i < sceneGraphCol; i++)
                {
                    Primitive primitive = e.SceneGraph[i];
                    if (primitive is Box)
                    {
                        Box box = (Box)primitive;
                        if (box.DataPoint != null)
                        {
                            if (offsetX == 0)
                            {
                                offsetX = box.rect.X + 10;
                                startOffsetX = box.rect.X;
                            }
                            box.rect.X = offsetX;

                            double width = double.Parse(dtChart13.Rows[0].ItemArray[j].ToString()) * 500;
                            widths.Add(box.DataPoint.Label, (int)width);

                            box.rect.Width = (int)width;
                            j += 1;
                            offsetX += box.rect.Width + 2;
                            PaintElement pe = new PaintElement();
                            pe.FillOpacity = 255;
                            pe.ElementType = PaintElementType.SolidFill;
                            if (GetNumber(box.Value.ToString()) > 1)
                            {
                                pe.Fill = Color.Green;
                            }
                            else
                            {
                                pe.Fill = Color.Red;
                            }
                            box.PE = pe;
                            Text text = new Text();
                            text.labelStyle.Font = new System.Drawing.Font("Verdana", (float)(7.8));
                            text.PE.Fill = Color.Black;
                            text.bounds = new Rectangle(box.rect.X + box.rect.Width - 40, box.rect.Y - 22, 40, 25);

                            text.SetTextString(String.Format("{0:0.0000}", Convert.ToDecimal(box.Value)));
                            e.SceneGraph.Add(text);
                        }

                    }

                }
                offsetX = 0;
                int counter = 1;
                int counter2 = 1;

                for (int i = 0; i < e.SceneGraph.Count; i++)
                {
                    Primitive primitive = e.SceneGraph[i];
                    if (primitive is Infragistics.UltraChart.Core.Primitives.Text)
                    {

                        Infragistics.UltraChart.Core.Primitives.Text te = (Infragistics.UltraChart.Core.Primitives.Text)primitive;
                        if (widths.ContainsKey(te.GetTextString()))
                        {
                            if (offsetX == 0)
                            {
                                offsetX = startOffsetX;
                            }
                            LabelStyle labStyle = new LabelStyle();
                            labStyle.Font = new System.Drawing.Font("Verdana", (float)(7));
                            te.SetLabelStyle(labStyle);
                            te.bounds.X = offsetX - 8 + widths[te.GetTextString()] / 2 + 14;
                            te.bounds.Y = 245;
                            offsetX += widths[te.GetTextString()] + 2;
                            te.bounds.Width = 20;

                            te.SetTextString(counter.ToString());
                            counter += 1;
                        }
                        else
                        {
                            if (te.Path == "Border.Title.Legend")
                            {
                                string s = orderedTable.Columns[counter2].ColumnName;
                                s = counter2.ToString() + " - " + s;
                                te.bounds.X = te.bounds.X - 10;
                                te.SetTextString(s);
                                counter2 += 1;
                            }
                        }
                    }
                }

                Line lineLegend = new Line(new Point(234, 369), new Point(246, 369)); //создание дополнительной строчки в легенде
                lineLegend.lineStyle.DrawStyle = LineDrawStyle.Solid;
                lineLegend.PE.Stroke = Color.Red;
                lineLegend.PE.StrokeWidth = 2;
                e.SceneGraph.Add(lineLegend);

                Text textLegend = new Text();
                textLegend.labelStyle.Font = new System.Drawing.Font("Verdana", (float)(7.8));
                textLegend.SetTextString("Уровень интегрального показателя");
                textLegend.bounds.X = 250;
                textLegend.bounds.Y = 369;
                e.SceneGraph.Add(textLegend);

                IAdvanceAxis xAxis = (IAdvanceAxis)e.Grid["X"];
                IAdvanceAxis yAxis = (IAdvanceAxis)e.Grid["Y"];

                if (xAxis == null || yAxis == null)
                    return;

                int xMin = (int)xAxis.MapMinimum;
                int xMax = (int)xAxis.MapMaximum;
                double urfoAverage;
                if (double.TryParse(dtChart12.Rows[0].ItemArray[0].ToString(), out urfoAverage))
                {
                    int fmY = (int)yAxis.Map(urfoAverage);
                    Line line = new Line();
                    line.lineStyle.DrawStyle = LineDrawStyle.Solid;
                    line.PE.Stroke = Color.Red;
                    line.PE.StrokeWidth = 2;
                    line.p1 = new Point(xMin, fmY);
                    line.p2 = new Point(xMax, fmY);
                    e.SceneGraph.Add(line);

                    Text text = new Text();
                    text.labelStyle.Font = new System.Drawing.Font("Verdana", (float)(7.8));
                    text.PE.Fill = Color.Black;
                    text.bounds = new Rectangle(xMin - 46, fmY, 780, 15);
                    text.SetTextString(String.Format("{0:# ##0.0000}", double.Parse(dtChart12.Rows[0].ItemArray[0].ToString())));//String.Format(dtChart12.Rows[0].ItemArray[0].ToString(),""));
                    e.SceneGraph.Add(text);
                }
            }
        }

        protected void Chart1_InvalidDataReceived(object sender, ChartDataInvalidEventArgs e)
        {
            e.Text = "Нет данных";//chart_error_message;
            e.LabelStyle.Font = new System.Drawing.Font("Verdana", 20);
            e.LabelStyle.FontColor = Color.LightGray;
            e.LabelStyle.VerticalAlign = StringAlignment.Center;
            e.LabelStyle.HorizontalAlign = StringAlignment.Center;
        }

        #endregion

        #region  Обработчики Диаграммы2

        DataTable Chart2DataTable;
        protected void Chart2_DataBinding(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();
            DataTable dt1 = new DataTable();
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("Chart2"), " ", dt);
            if (dt.Rows.Count < 1)
            {
                Chart2.DataSource = null;
            }
            else
            {
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    dt1.Columns.Add(dt.Columns[i].ColumnName, dt.Columns[i].DataType);
                }
                object[] o = new object[dt1.Columns.Count];
                ColumnNanes = new object[dt1.Columns.Count - 1];

                o[0] = dt.Rows[0].ItemArray[0];
                o[1] = (GetNumber(dt.Rows[0].ItemArray[1].ToString()) - 1) * 100;
                ColumnNanes[0] = dt.Columns[1].ColumnName;
                dt1.Columns[1].ColumnName = "Значение интегрального показателя <b>" + Math.Round(double.Parse(dt.Rows[0].ItemArray[1].ToString()), 4).ToString() + "</b><br>" + "Темп прироста к предыдущему году <b>" + Math.Round(double.Parse(o[1].ToString()), 2).ToString() + "%</b><br>Темп прироста к базовому году <b>" + Math.Round(double.Parse(o[1].ToString()), 2).ToString() + "%</b>";
                if (dt1.Columns.Count >= 3)
                {
                    for (int i = 2; i < dt1.Columns.Count; i++)
                    {
                        o[i] = GetNumber(o[i - 1].ToString()) + (GetNumber(dt.Rows[0].ItemArray[i].ToString()) - 1) * 100;
                        ColumnNanes[i - 1] = dt.Columns[i].ColumnName;

                        dt1.Columns[i].ColumnName = "Значение интегрального показателя <b>" + Math.Round(double.Parse(dt.Rows[0].ItemArray[i].ToString()), 4).ToString() + "</b><br>" + "Темп прироста к предыдущему году <b>" + Math.Round((GetNumber(dt.Rows[0].ItemArray[i].ToString()) - 1) * 100, 2).ToString() + "%</b><br>Темп прироста к базовому году <b>" + Math.Round(GetNumber(o[i].ToString()), 2).ToString() + "%</b>";
                    }
                }
                dt1.Rows.Add(o);
                double max = GetNumber(o[1].ToString());
                double min = GetNumber(o[1].ToString());
                for (int i = 1; i < dt1.Columns.Count; i++)
                {
                    if (max < GetNumber(o[i].ToString()))
                    {
                        max = GetNumber(o[i].ToString());
                    }
                    if (min > GetNumber(o[i].ToString()))
                    {
                        min = GetNumber(o[i].ToString());
                    }

                }
                Chart2.Axis.Y.RangeType = AxisRangeType.Custom;
                Chart2.Axis.Y.RangeMax = max + 1;
                Chart2.Axis.Y.RangeMin = min - 1;
                Chart2.DataSource = dt1;
                Chart2DataTable = dt;
            }
        }

        protected void Chart2_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            int counterValue = 1;
            int counterMarks = 0;
            int countScen = e.SceneGraph.Count;
            for (int i = 0; i < countScen; i++)
            {
                Primitive primitive = e.SceneGraph[i];

                if (primitive is Infragistics.UltraChart.Core.Primitives.Text)
                {

                    Infragistics.UltraChart.Core.Primitives.Text te = (Infragistics.UltraChart.Core.Primitives.Text)primitive;

                    if ((te.Path == "Border.Title.Grid.X"))
                    {
                        te.SetTextString(ColumnNanes[counterMarks].ToString());
                        counterMarks += 1;
                    }
                    else
                    {

                        if ((Chart2.DataSource != null) && (ComboYear.SelectedValue != Chart2.Axis.X.Labels.ItemFormatString))
                        {
                            if (counterValue < Chart2DataTable.Rows[0].ItemArray.Length)
                            {
                                te.bounds.Y = te.bounds.Y - 10;
                                Box box = new Box(new Rectangle(te.bounds.X - 9, te.bounds.Y + 1, 17, 17));
                                PaintElement paintEl = new PaintElement();
                                paintEl.ElementType = PaintElementType.SolidFill;
                                paintEl.Fill = Color.Green;
                                paintEl.StrokeOpacity = 0;
                                box.PE = paintEl;
                                e.SceneGraph.Add(box);
                                double pers = (GetNumber(Chart2DataTable.Rows[0].ItemArray[counterValue].ToString()) - 1) * 100;
                                te.SetTextString(te.GetTextString() + "%");
                                counterValue += 1;

                            }
                        }
                        else
                        {
                            if ((i == 15) && (Chart2.DataSource != null))
                            {
                                te.bounds.Y = te.bounds.Y - 10;
                                Box box = new Box(new Rectangle(te.bounds.X - 9, te.bounds.Y + 1, 17, 17));
                                PaintElement paintEl = new PaintElement();
                                paintEl.ElementType = PaintElementType.SolidFill;
                                paintEl.Fill = Color.Green;
                                paintEl.StrokeOpacity = 0;
                                box.PE = paintEl;
                                e.SceneGraph.Add(box);
                                double pers = (GetNumber(Chart2DataTable.Rows[0].ItemArray[1].ToString()) - 1) * 100;
                                te.SetTextString(te.GetTextString() + "%");
                                counterValue += 1;
                            }
                            if (i < 15)
                            {
                                ComboYear.GetRootNodesName(0);
                            }

                        }
                    }


                }

            }
        }

        protected void Chart2_InvalidDataReceived(object sender, ChartDataInvalidEventArgs e)
        {
            e.Text = "Нет данных";//chart_error_message;
            e.LabelStyle.Font = new System.Drawing.Font("Verdana", 20);
            e.LabelStyle.FontColor = Color.LightGray;
            e.LabelStyle.VerticalAlign = StringAlignment.Center;
            e.LabelStyle.HorizontalAlign = StringAlignment.Center;
        }

        #endregion

        #region Экспорт в Excel

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            ReportExcelExporter1.WorksheetTitle = Label1.Text;
            ReportExcelExporter1.WorksheetSubTitle = Label5.Text;
            Workbook workbook = new Workbook();
            Worksheet sheet1 = workbook.Worksheets.Add("Таблица");
            Worksheet sheet2 = workbook.Worksheets.Add("Диаграмма 1");
            Worksheet sheet3 = workbook.Worksheets.Add("Диаграмма 2");
            ReportExcelExporter1.HeaderCellFont = new System.Drawing.Font("Verdana", 11);
            ReportExcelExporter1.TitleFont = new System.Drawing.Font("Verdana", 12, FontStyle.Bold);
            ReportExcelExporter1.SubTitleFont = new System.Drawing.Font("Verdana", 11);
            ReportExcelExporter1.TitleAlignment = HorizontalCellAlignment.Center;
            ReportExcelExporter1.TitleStartRow = 3;
            ReportExcelExporter1.Export(headerLayout, sheet1, 6);
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
            double m = 0;
            for (int i = 0; i < Grid.Columns.Count; i++)
            {
                if (!double.TryParse(e.Workbook.Worksheets["Таблица"].Rows[7].Cells[i].Value.ToString(),out m))
                {
                    e.Workbook.Worksheets["Таблица"].Rows[7].Cells[i].CellFormat.Font.Name = e.Workbook.Worksheets["Таблица"].Rows[8].Cells[i].CellFormat.Font.Name;
                    e.Workbook.Worksheets["Таблица"].Rows[7].Cells[i].CellFormat.Font.Height = e.Workbook.Worksheets["Таблица"].Rows[8].Cells[i].CellFormat.Font.Height;
                    e.Workbook.Worksheets["Таблица"].Rows[7].Cells[i].CellFormat.Font.Bold = e.Workbook.Worksheets["Таблица"].Rows[8].Cells[i].CellFormat.Font.Bold;
                    e.Workbook.Worksheets["Таблица"].Rows[7].Cells[i].Value = e.Workbook.Worksheets["Таблица"].Rows[8].Cells[i].Value;
                    e.Workbook.Worksheets["Таблица"].Rows[8].Cells[i].Value = null;
                    
                }
                e.Workbook.Worksheets["Таблица"].Rows[9].Cells[i].Value = null;
            }
            e.Workbook.Worksheets["Таблица"].Rows[4].Height = 800;
            e.Workbook.Worksheets["Диаграмма 1"].Rows[1].Cells[0].CellFormat.Alignment = HorizontalCellAlignment.Left;
            e.Workbook.Worksheets["Диаграмма 2"].Rows[1].Cells[0].CellFormat.Alignment = HorizontalCellAlignment.Left;
        }
        #endregion

        #region Экспорт в PDF
        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            ReportPDFExporter1.PageTitle = Label1.Text;
            ReportPDFExporter1.PageSubTitle = Label5.Text;
            Report report = new Report();
            ISection section1 = report.AddSection();
            ISection section2 = report.AddSection();
            ISection section3 = report.AddSection();
            int width = int.Parse(Chart1.Width.Value.ToString());
            Chart1.Width = 1000;
            Chart2.Width = 1000;
            Grid.DisplayLayout.RowSelectorStyleDefault.Width = 23;
            int width1 = int.Parse(Chart1.Width.Value.ToString());
            Grid.Width = 800;
            Grid.Rows[2].Hidden = true;

            ReportPDFExporter1.Export(headerLayout,  section1);
            Grid.DisplayLayout.RowSelectorStyleDefault.Width = 0;
            Grid.Width = width1;
            Grid.Rows[2].Hidden = false;
            ReportPDFExporter1.Export(Chart1,Label3.Text, section2);
            
            ReportPDFExporter1.Export(Chart2, Label4.Text, section3);
            Chart1.Width = width;
            Chart2.Width = width;
        }
        void ultraWebGridDocumentExporter_RowExporting(object sender,Infragistics.WebUI.UltraWebGrid.DocumentExport.RowExportingEventArgs e)
        {
            e.GridRow.Height = 500;
        }
        #endregion
    }
}
