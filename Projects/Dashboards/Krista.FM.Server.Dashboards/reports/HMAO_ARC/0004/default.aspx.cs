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

namespace Krista.FM.Server.Dashboards.reports.HMAO_ARC._0004
{
    public partial class _default : CustomReportPage
    {
        string page_title = "Мониторинг образования, использования и обезвреживания отходов производства и потребления (по состоянию на выбранную дату)";
        string page_sub_title = "Мониторинг образования, использования и обезвреживания отходов производства и потребления в муниципальных образованиях субъекта РФ по состоянию на {0} год";
        string map_title = "Образовалось отходов производства и потребления в {0} год, {1}";
        private CustomParam baseRegion { get { return (UserParams.CustomParam("baseRegion")); } }
        private CustomParam selectedYear { get { return (UserParams.CustomParam("selectedYear")); } }
        private GridHeaderLayout headerLayout;
        private string edIsm = "";
        private string legendTitle = "";
        private string style = "";
        private string mapFolderName = "";
        private bool IsSmallResolution
        {
            get { return CRHelper.GetScreenWidth < 1200; }
        }

        private int minScreenWidth
        {
            get { return IsSmallResolution ? 750 : CustomReportConst.minScreenWidth; }
        }

        private int minScreenHeight
        {
            get { return IsSmallResolution ? 700 : CustomReportConst.minScreenHeight; }
        }

        public static bool IsCalloutTownShape(Shape shape)
        {
            return shape.Layer == CRHelper.MapShapeType.Towns.ToString();
        }

        private static bool IsMozilla
        {
            get { return HttpContext.Current.Request.Browser.Browser == "Firefox"; }
        }

        abstract class ICustomizerSize
        {
            public enum BrouseName { IE, FireFox, SafariOrHrome }

            public enum ScreenResolution { _800x600, _1280x1024, Default }

            public BrouseName NameBrouse;

            public abstract int GetGridWidth();
            public abstract int GetChartWidth();

            protected abstract void ContfigurationGridIE(UltraWebGrid Grid);
            protected abstract void ContfigurationGridFireFox(UltraWebGrid Grid);
            protected abstract void ContfigurationGridGoogle(UltraWebGrid Grid);

            public virtual void ContfigurationGrid(UltraWebGrid Grid)
            {
                switch (this.NameBrouse)
                {
                    case BrouseName.IE:
                        this.ContfigurationGridIE(Grid);
                        return;
                    case BrouseName.FireFox:
                        this.ContfigurationGridFireFox(Grid);
                        return;
                    case BrouseName.SafariOrHrome:
                        this.ContfigurationGridGoogle(Grid);
                        return;
                    default:
                        throw new Exception("Охо!");
                }
            }

            public ICustomizerSize(BrouseName NameBrouse)
            {
                this.NameBrouse = NameBrouse;
            }

            public static BrouseName GetBrouse(string _BrouseName)
            {
                if (_BrouseName == "IE") { return BrouseName.IE; };
                if (_BrouseName == "FIREFOX") { return BrouseName.FireFox; };
                if (_BrouseName == "APPLEMAC-SAFARI") { return BrouseName.SafariOrHrome; };

                return BrouseName.IE;
            }

            public static ScreenResolution GetScreenResolution(int Width)
            {
                if (Width < 801)
                {
                    return ScreenResolution._800x600;
                }
                if (Width < 1281)
                {
                    return ScreenResolution._1280x1024;
                }
                return ScreenResolution.Default;
            }
        }

        class CustomizerSize_800x600 : ICustomizerSize
        {
            public override int GetGridWidth()
            {
                switch (this.NameBrouse)
                {
                    case BrouseName.IE:
                        return 740;
                    case BrouseName.FireFox:
                        return 750;
                    case BrouseName.SafariOrHrome:
                        return 755;
                    default:
                        return 800;
                }
            }

            public override int GetChartWidth()
            {
                switch (this.NameBrouse)
                {
                    case BrouseName.IE:
                        return 740;
                    case BrouseName.FireFox:
                        return 750;
                    case BrouseName.SafariOrHrome:
                        return 750;
                    default:
                        return 800;
                }
            }

            public CustomizerSize_800x600(BrouseName NameBrouse) : base(NameBrouse) { }

            #region Настрока размера колонок для грида
            protected override void ContfigurationGridIE(UltraWebGrid Grid)
            {
                int GridWidth = (int)(Grid.Width.Value);
                int OnePercent = GridWidth / 100;

                double coef = 1.0;

                coef = 1.01;

                Grid.DisplayLayout.RowSelectorsDefault = RowSelectors.No;

                int ColCount = Grid.Bands[0].Columns.Count;

                foreach (UltraGridColumn col in Grid.Bands[0].Columns)
                {
                    int indexCol = col.Index;

                    if (indexCol == 0)
                    {
                        col.CellStyle.Wrap = true;
                        col.Width = OnePercent * 25;
                    }
                    else
                    {
                        col.Width = (int)((75.0 / (ColCount - 1)) * OnePercent * coef);
                    }
                }
            }

            protected override void ContfigurationGridFireFox(UltraWebGrid Grid)
            {
                int GridWidth = (int)(Grid.Width.Value);
                int OnePercent = GridWidth / 100;

                double coef = 1.0;

                coef = 1.04;

                Grid.DisplayLayout.RowSelectorsDefault = RowSelectors.No;

                int ColCount = Grid.Bands[0].Columns.Count;

                foreach (UltraGridColumn col in Grid.Bands[0].Columns)
                {
                    int indexCol = col.Index;

                    if (indexCol == 0)
                    {
                        col.CellStyle.Wrap = true;
                        col.Width = OnePercent * 25;
                    }
                    else
                    {
                        col.Width = (int)((75.0 / (ColCount - 1)) * OnePercent * coef);
                    }
                }
            }

            protected override void ContfigurationGridGoogle(UltraWebGrid Grid)
            {
                int GridWidth = (int)(Grid.Width.Value);
                int OnePercent = GridWidth / 100;

                double coef = 1.0;

                coef = 1.1;

                Grid.DisplayLayout.RowSelectorsDefault = RowSelectors.No;

                int ColCount = Grid.Bands[0].Columns.Count;

                foreach (UltraGridColumn col in Grid.Bands[0].Columns)
                {
                    int indexCol = col.Index;

                    if (indexCol == 0)
                    {
                        col.CellStyle.Wrap = true;
                        col.Width = OnePercent * 25;
                    }
                    else
                    {
                        col.Width = (int)((75.0 / (ColCount - 1)) * OnePercent * coef);
                    }
                }
            }

            #endregion
        }

        class CustomizerSize_1280x1024 : ICustomizerSize
        {
            public override int GetGridWidth()
            {
                switch (this.NameBrouse)
                {//minScreenWidth
                    case BrouseName.IE:
                        return (int)CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 15).Value;
                    case BrouseName.FireFox:
                        return (int)CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 5).Value;
                    case BrouseName.SafariOrHrome:
                        return (int)CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 15).Value;
                    default:
                        return (int)CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 100).Value;
                }
            }

            public override int GetChartWidth()
            {
                switch (this.NameBrouse)
                {
                    case BrouseName.IE:
                        return (int)CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 17).Value;
                    case BrouseName.FireFox:
                        return (int)CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 30).Value;
                    case BrouseName.SafariOrHrome:
                        return (int)CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 15).Value;
                    default:
                        return (int)CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 50).Value;
                }
            }

            public CustomizerSize_1280x1024(BrouseName NameBrouse) : base(NameBrouse) { }

            #region Настрока размера колонок для грида
            protected override void ContfigurationGridIE(UltraWebGrid Grid)
            {

                int GridWidth = (int)(Grid.Width.Value * 3.0 / 3.0);
                int OnePercent = GridWidth / 100;

                double coef = 1.0;

                coef = 0.95;

                int ColCount = Grid.Bands[0].Columns.Count;

                foreach (UltraGridColumn col in Grid.Bands[0].Columns)
                {
                    int indexCol = col.Index;

                    if (indexCol == 0)
                    {
                        col.CellStyle.Wrap = true;
                        col.Width = (OnePercent * 25 - 5);
                    }
                    else
                    {
                        col.Width = (int)((75.0 / (ColCount - 1)) * OnePercent * coef);
                    }
                }
            }

            protected override void ContfigurationGridFireFox(UltraWebGrid Grid)
            {
                int GridWidth = (int)(Grid.Width.Value * 3.0 / 3.0);
                int OnePercent = GridWidth / 100;

                double coef = 1.0;

                coef = 1.01;
                coef = 0.95;

                int ColCount = Grid.Bands[0].Columns.Count;

                foreach (UltraGridColumn col in Grid.Bands[0].Columns)
                {
                    int indexCol = col.Index;

                    if (indexCol == 0)
                    {
                        col.CellStyle.Wrap = true;
                        col.Width = OnePercent * 25;
                    }
                    else
                    {
                        col.Width = (int)((75.0 / (ColCount - 1)) * OnePercent * coef);
                    }
                }
            }

            protected override void ContfigurationGridGoogle(UltraWebGrid Grid)
            {
                int GridWidth = (int)(Grid.Width.Value);
                int OnePercent = GridWidth / 100;

                double coef = 1.03;
                coef = 0.95;
                int ColCount = Grid.Bands[0].Columns.Count;

                foreach (UltraGridColumn col in Grid.Bands[0].Columns)
                {
                    int indexCol = col.Index;

                    if (indexCol == 0)
                    {
                        col.CellStyle.Wrap = true;
                        col.Width = OnePercent * 25;
                    }
                    else
                    {
                        col.Width = (int)((75.0 / (ColCount - 1)) * OnePercent * coef);
                    }
                }
            }
            #endregion

        }

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);
            Label1.Text = page_title;
            Page.Title = page_title;

            Grid.Height = Unit.Empty;
            ComboYear.Width = 200;
            DundasMap.Height = 500;
            Grid.Width = CRHelper.GetScreenWidth -45;
            DundasMap.Width = CRHelper.GetScreenWidth -45; 
           /* if (IsSmallResolution)
            {
                Grid.Width = minScreenWidth;
                DundasMap.Width = minScreenWidth - 5;
            }
            else
            {
                Grid.Width = (int)(minScreenWidth - 15);
                DundasMap.Width = (int)(minScreenWidth - 20);
            }*/
            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportExcelExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
            ReportExcelExporter1.ExcelExporter.EndExport += new EndExportEventHandler(ExcelExporter_EndExport);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            baseRegion.Value = RegionSettingsHelper.Instance.RegionBaseDimension;
            if (!Page.IsPostBack)
            {
                ComboYear.Title = "Выберите год";
                ComboYear.FillDictionaryValues(YearsLoad("years"));
                ComboYear.SelectLastNode();
            }
            selectedYear.Value = "[Период__Год Квартал Месяц].[Период__Год Квартал Месяц].[Год].[" + ComboYear.SelectedValue + "]";
            Label2.Text = String.Format(page_sub_title, ComboYear.SelectedValue);
            mapFolderName = "Субъекты/ХМАО";
            headerLayout = new GridHeaderLayout(Grid);
            Grid.DataBind();
            Label3.Text = String.Format(map_title, ComboYear.SelectedValue, edIsm);
            if (Grid.DataSource != null)
            {
                SetMapSettings();
            }
            Grid.DisplayLayout.AllowColSizingDefault = AllowSizing.Fixed;
        }

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

        protected void Grid_DataBinding(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("grid"), "Показатель", dt);
            if (dt.Rows[0][1].ToString() == "Тысяча тонн")
            {
                edIsm = "тыс.тонн";
            }
            else
            {
                edIsm = dt.Rows[0][1].ToString().ToLower();
            }
            dt.Rows.Remove(dt.Rows[0]);

            int gorCount = 0, moCount = 0;
            for (int i = 0; i < dt.Rows.Count; i+=4)
            {
                if (dt.Rows[i][0].ToString().StartsWith("Город"))
                {
                    gorCount += 1;
                }
                else
                {
                    moCount += 1;
                }
            }
            string[] goroda = new string[gorCount];
            string[] mo = new string[moCount];
            int k = 0, l = 0;
            for (int i = 0; i < dt.Rows.Count; i+=4)
            {
                if (dt.Rows[i][0].ToString().StartsWith("Город"))
                {
                    goroda[k] = dt.Rows[i][0].ToString().Replace("Город", String.Empty).Replace(" ", String.Empty);
                    k += 1;
                }
                else
                {
                    mo[l] = dt.Rows[i][0].ToString();
                    l += 1;
                }
            }

            Array.Sort(goroda);
            Array.Sort(mo);
            DataTable normDt = new DataTable();
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                normDt.Columns.Add(dt.Columns[i].ColumnName, dt.Columns[i].DataType);
            }

            for (int j = 0; j < dt.Rows.Count; j+=4) 
            {
                if (dt.Rows[j][0].ToString() == "Город Ханты-Мансийск")
                {
                    normDt.Rows.Add(dt.Rows[j].ItemArray);
                }
            }

            for (int i = 0; i < goroda.Length; i++)
            {
                for (int j = 0; j < dt.Rows.Count; j+=4)
                {
                    if (dt.Rows[j][0].ToString() == "Город " + goroda[i] && !dt.Rows[j][0].ToString().StartsWith("Город Ханты-Мансийск"))
                    {
                        normDt.Rows.Add(dt.Rows[j].ItemArray);
                        normDt.Rows.Add(dt.Rows[j+1].ItemArray);
                        normDt.Rows.Add(dt.Rows[j+2].ItemArray);
                        normDt.Rows.Add(dt.Rows[j + 3].ItemArray);
                    }
                }
            }

            for (int i = 0; i < mo.Length; i++)
            {
                for (int j = 0; j < dt.Rows.Count; j+=4)
                {
                    if (dt.Rows[j][0].ToString() == mo[i])
                    {
                        normDt.Rows.Add(dt.Rows[j].ItemArray);
                        normDt.Rows.Add(dt.Rows[j+1].ItemArray);
                        normDt.Rows.Add(dt.Rows[j+2].ItemArray);
                        normDt.Rows.Add(dt.Rows[j + 3].ItemArray);
                    }
                }
            }

            if (dt.Rows.Count < 1)
            {
                Grid.DataSource = null; 
            }
            else
            {
                Grid.DataSource = dt;
            }
        }

        protected void Grid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            double colWidth = 0,col1Width=0;
            if (IsSmallResolution)
            {
                colWidth = 0.17;
                col1Width = 0.22;
            }
            else
            {
                colWidth = 0.1;
                col1Width = 0.3;
            }
            headerLayout.childCells.Clear();
            e.Layout.AllowSortingDefault = AllowSorting.No;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.HeaderStyleDefault.VerticalAlign = VerticalAlign.Middle;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(minScreenWidth * col1Width);
            e.Layout.Bands[0].Columns[0].Header.Style.Wrap = true;
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = 1 == 1;
            e.Layout.Bands[0].Columns[0].Header.Style.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Center;

            headerLayout.AddCell(e.Layout.Bands[0].Columns[0].Header.Caption);

            for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i++)
            {
                e.Layout.Bands[0].Columns[i].Header.Style.Wrap = 1 == 1;
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(minScreenWidth * colWidth);
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
                e.Layout.Bands[0].Columns[i].CellStyle.CustomRules = "padding-right:5px";
                e.Layout.Bands[0].Columns[i].CellStyle.Wrap = 1 == 1;
                e.Layout.Bands[0].Columns[i].Header.Caption = e.Layout.Bands[0].Columns[i].Key.Replace("муниципальный район", "МР").Replace("Город", "г.");
                headerLayout.AddCell(e.Layout.Bands[0].Columns[i].Header.Caption);
            }
            headerLayout.ApplyHeaderInfo();
        }

        protected void Grid_InitializeRow(object sender, RowEventArgs e)
        {
            if (IsSmallResolution)
            { style = "background-repeat: no-repeat;background-position:15px"; }
            else
            { style = "background-repeat: no-repeat;background-position: 40px"; }
            
            if (e.Row.Cells[0].Text.EndsWith("Темп прироста"))
            {

                e.Row.Cells[0].Text = "";
                Grid.Rows[e.Row.Index - 1].Cells[0].Text = "";
                Grid.Rows[e.Row.Index - 2].Cells[0].Text = Grid.Rows[e.Row.Index - 2].Cells[0].Text.Split(';')[0]+", "+edIsm;

                Grid.Rows[e.Row.Index - 2].Cells[0].RowSpan = 3;
                Grid.Rows[e.Row.Index - 2].Cells[0].Style.Font.Bold =true;

             //   Grid.Rows[e.Row.Index - 2].Cells[0].Style.BorderDetails.StyleBottom = BorderStyle.None;
                Grid.Rows[e.Row.Index - 1].Cells[0].Style.BorderDetails.StyleBottom = BorderStyle.None;
                e.Row.Cells[0].Style.BorderDetails.StyleBottom = BorderStyle.None;
                Grid.Rows[e.Row.Index - 2].Cells[0].Style.BackColor = Color.White;
                Grid.Rows[e.Row.Index - 1].Cells[0].Style.BackColor = Color.White;
                e.Row.Cells[0].Style.BackColor = Color.White;
             //   Grid.Rows[e.Row.Index - 2].Cells[0].Style.Font.Size =8;
                //Grid.Rows[e.Row.Index - 2].Cells[0].Style.Font.Name = "Verdana";

                for (int i = 1; i < e.Row.Cells.Count; i++)
                {
                    Grid.Rows[e.Row.Index - 2].Cells[i].Style.BorderDetails.StyleBottom = BorderStyle.None;
                    Grid.Rows[e.Row.Index - 1].Cells[i].Style.BorderDetails.StyleBottom = BorderStyle.None;
                    
                    Grid.Rows[e.Row.Index - 2].Cells[i].Style.BackColor = Color.White;
                    Grid.Rows[e.Row.Index - 1].Cells[i].Style.BackColor = Color.White;
                    e.Row.Cells[i].Style.BackColor = Color.White;
                    if (ComboYear.SelectedIndex != 0)
                    {
                        if (Math.Round(Convert.ToDouble(Grid.Rows[e.Row.Index - 1].Cells[i].Value), 2) < 0)
                        {
                            Grid.Rows[e.Row.Index - 1].Cells[i].Title = "Снижение относительно " + ComboYear.SelectedNode.PrevNode.Text.Split(' ')[0] + " года";
                            if (!Grid.Rows[e.Row.Index - 2].Cells[0].Text.StartsWith("Образовалось отходов"))
                            {
                                Grid.Rows[e.Row.Index - 1].Cells[i].Style.BackgroundImage = "~/images/arrowRedDownBB.png";
                            }
                            else
                            {
                                Grid.Rows[e.Row.Index - 1].Cells[i].Style.BackgroundImage = "~/images/arrowGreenDownBB.png";
                            }
                            Grid.Rows[e.Row.Index - 1].Cells[i].Style.CustomRules = style;

                        }
                        if (Math.Round(Convert.ToDouble(Grid.Rows[e.Row.Index - 1].Cells[i].Value),2) > 0)
                        {
                            Grid.Rows[e.Row.Index - 1].Cells[i].Title = "Рост относительно " + ComboYear.SelectedNode.PrevNode.Text.Split(' ')[0] + " года";
                            if (!Grid.Rows[e.Row.Index - 2].Cells[0].Text.StartsWith("Образовалось отходов"))
                            {
                                Grid.Rows[e.Row.Index - 1].Cells[i].Style.BackgroundImage = "~/images/arrowGreenUpBB.png";
                            }
                            else
                            {
                                Grid.Rows[e.Row.Index - 1].Cells[i].Style.BackgroundImage = "~/images/arrowRedUpBB.png";
                            }
                            Grid.Rows[e.Row.Index - 1].Cells[i].Style.CustomRules = style;

                        }
                        Grid.Rows[e.Row.Index - 1].Cells[i].Value = String.Format("{0:N2}", Convert.ToDouble(Grid.Rows[e.Row.Index - 1].Cells[i].Value));
                        Grid.Rows[e.Row.Index - 2].Cells[i].Value = String.Format("{0:N2}", Convert.ToDouble(Grid.Rows[e.Row.Index - 2].Cells[i].Value));
                        if ((e.Row.Cells[i].Value != null))
                        {
                             e.Row.Cells[i].Title = "Темп прироста к " + ComboYear.SelectedNode.PrevNode.Text.Split(' ')[0] + " году";
                            e.Row.Cells[i].Value = String.Format("{0:P2}", Convert.ToDouble(e.Row.Cells[i].Value));
                        }
                    }
                }
            }
            else
            {
                if (e.Row.Cells[0].Text.EndsWith("Доля"))
                {
                    e.Row.Cells[0].Text = "";
                    Grid.Rows[e.Row.Index - 3].Cells[0].RowSpan = 4;
                    Grid.Rows[e.Row.Index - 3].Cells[0].Style.BackColor = Color.White;
                 //   Grid.Rows[e.Row.Index - 1].Cells[0].Style.BorderDetails.StyleBottom = BorderStyle.None;
                    e.Row.Cells[0].Style.BackColor = Color.White;
                    for (int i = 1; i < e.Row.Cells.Count; i++)
                    {
                        Grid.Rows[e.Row.Index - 1].Cells[i].Style.BackColor = Color.White;
                        e.Row.Cells[i].Style.BackColor = Color.White;
                        Grid.Rows[e.Row.Index - 1].Cells[i].Style.BorderDetails.StyleBottom = BorderStyle.None;
                        if ((e.Row.Cells[i].Value != null))
                        {
                            e.Row.Cells[i].Title = "Доля от образования отходов производства и потребления";
                            e.Row.Cells[i].Value = String.Format("{0:P2}", Convert.ToDouble(e.Row.Cells[i].Value));
                        }
                    }

                }
            }
            if (e.Row.Cells[0].Text.Contains(";"))
            {
                //e.Row.Cells[0].Text = e.Row.Cells[0].Text.Split(';')[0];
              //  e.Row.Cells[0].Style.Font.Bold = true;
              //  e.Row.Cells[0].Style.Font.Size = 8;
                //e.Row.Cells[0].Style.Font.Name = "Verdana";
            }
        }
        #region Обработчики карты
        DataTable dtMap;
        bool smallLegend = false;
        bool revPok = false;
        public void SetMapSettings()
        {
            legendTitle = "Образовалось отходов\n производства и потребления, " + edIsm;
            DundasMap.ShapeFields.Clear();
            DundasMap.ShapeRules.Clear();
            DundasMap.Shapes.Clear();
            DundasMap.Meridians.Visible = false;
            DundasMap.Parallels.Visible = false;
            DundasMap.ZoomPanel.Visible = true;
            DundasMap.NavigationPanel.Visible = true;
            //DundasMap.NavigationPanel.
            DundasMap.Viewport.EnablePanning = true;
            DundasMap.ColorSwatchPanel.Visible = false;
            DundasMap.Viewport.LocationUnit = CoordinateUnit.Pixel;
            if (IsSmallResolution)
            {
                DundasMap.Viewport.Zoom = 88;
                DundasMap.Viewport.ViewCenter.Y -= 33;
            }
            else
            {
                DundasMap.Viewport.Zoom = 112;
                DundasMap.Viewport.ViewCenter.X += 5;
            }
            DundasMap.Legends.Clear();
            Legend legend = new Legend("CostLegend");
            legend.Visible = true;
            legend.BackColor = Color.White;
            legend.Dock = PanelDockStyle.Right;
            legend.BackSecondaryColor = Color.Gainsboro;
            legend.BackGradientType = GradientType.DiagonalLeft;
            legend.BackHatchStyle = MapHatchStyle.None;
            legend.BorderColor = Color.Gray;
            legend.BorderWidth = 1;
            legend.BorderStyle = MapDashStyle.Solid;
            legend.BackShadowOffset = 4;
            legend.TextColor = Color.Black;
            legend.Font = new System.Drawing.Font("MS Sans Serif", 7, FontStyle.Regular);
            legend.AutoFitText = true;
            legend.Title = legendTitle;
            legend.AutoFitMinFontSize = 7;

            DundasMap.Legends.Add(legend);

            Legend legend2 = new Legend("SymbolLegend");
            legend2.Visible = true;
            legend2.Dock = PanelDockStyle.Right;
            legend2.BackColor = Color.White;
            legend2.BackSecondaryColor = Color.Gainsboro;
            legend2.BackGradientType = GradientType.DiagonalLeft;
            legend2.BackHatchStyle = MapHatchStyle.None;
            legend2.BorderColor = Color.Gray;
            legend2.BorderWidth = 1;
            legend2.BorderStyle = MapDashStyle.Solid;
            legend2.BackShadowOffset = 4;
            legend2.TextColor = Color.Black;
            legend2.Font = new System.Drawing.Font("MS Sans Serif", 7, FontStyle.Regular);
            legend2.AutoFitText = true;
            legend2.Title = "Доля использованных\n и обезвреженных отходов\n от количества образованных, %";
            legend2.AutoFitMinFontSize = 7;

            DundasMap.Legends.Add(legend2);
            // добавляем правила раскраски
            DundasMap.ShapeRules.Clear();

            dtMap = new DataTable();
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("map"), "Карта", dtMap);
            double[] values = new double[dtMap.Rows.Count];
            for (int i = 0; i < dtMap.Rows.Count; i++)
            {
                values[i] = Convert.ToDouble(dtMap.Rows[i].ItemArray[1]);
            }
            Array.Sort(values);
            revPok = true;
            if (Math.Abs(values[0] - values[values.Length - 1]) <= 4)
            {
                smallLegend = true;
                LegendItem item = new LegendItem();
                item.Text = String.Format("0");
                item.Color = GetColor(0);
                DundasMap.Legends["CostLegend"].Items.Add(item);

                item = new LegendItem();
                item.Text = String.Format("1");
                item.Color = GetColor(1);
                DundasMap.Legends["CostLegend"].Items.Add(item);

                item = new LegendItem();
                item.Text = String.Format("2");
                item.Color = GetColor(2);
                DundasMap.Legends["CostLegend"].Items.Add(item);

                item = new LegendItem();
                item.Text = String.Format("3");
                item.Color = GetColor(3);
                DundasMap.Legends["CostLegend"].Items.Add(item);

                item = new LegendItem();
                item.Text = String.Format("4");
                item.Color = GetColor(4);
                DundasMap.Legends["CostLegend"].Items.Add(item);
            }
            else
            {
                smallLegend = false;
                ShapeRule rule = new ShapeRule();
                rule.Name = "CostRule";
                rule.Category = String.Empty;
                rule.ShapeField = "Cost";
                rule.DataGrouping = DataGrouping.EqualDistribution;
                rule.ColorCount = 5;
                rule.ColoringMode = ColoringMode.ColorRange;
                rule.FromColor = Color.PaleGreen;
                rule.MiddleColor = Color.Green;
                rule.ToColor = Color.DarkGreen;
                rule.BorderColor = Color.FromArgb(50, Color.Black);
                rule.GradientType = GradientType.None;
                rule.HatchStyle = MapHatchStyle.None;
                rule.ShowInLegend = "CostLegend";

                rule.LegendText = "#FROMVALUE{N2} - #TOVALUE{N2}";
                DundasMap.ShapeRules.Add(rule);

            }

            // добавляем поля
            DundasMap.Shapes.Clear();
            DundasMap.ShapeFields.Add("Name");
            DundasMap.ShapeFields["Name"].Type = typeof(string);
            DundasMap.ShapeFields["Name"].UniqueIdentifier = true;
            DundasMap.ShapeFields.Add("Cost");
            DundasMap.ShapeFields["Cost"].Type = typeof(double);
            DundasMap.ShapeFields["Cost"].UniqueIdentifier = false;

            // добавляем правила расстановки символов

            DundasMap.SymbolFields.Add("PollutedWater");
            DundasMap.SymbolFields["PollutedWater"].Type = typeof(double);
            DundasMap.SymbolFields["PollutedWater"].UniqueIdentifier = false;

            // добавляем правила расстановки символов
            DundasMap.SymbolRules.Clear();
            SymbolRule symbolRule = new SymbolRule();
            symbolRule.Name = "SymbolRule";
            symbolRule.Category = string.Empty;
            symbolRule.DataGrouping = DataGrouping.EqualDistribution;
            symbolRule.SymbolField = "PollutedWater";
            symbolRule.ShowInLegend = "SymbolLegend";
            symbolRule.LegendText = "#FROMVALUE{N2}% - #TOVALUE{N2}%";
            DundasMap.SymbolRules.Add(symbolRule);

            // звезды для легенды
            for (int i = 1; i < 6; i++)
            {
                PredefinedSymbol predefined = new PredefinedSymbol();
                predefined.Name = "PredefinedSymbol" + i;
                predefined.MarkerStyle = MarkerStyle.Triangle;
                predefined.Width = 5 + (i * 5);
                predefined.Height = predefined.Width;
                predefined.Color = Color.Red;
                DundasMap.SymbolRules["SymbolRule"].PredefinedSymbols.Add(predefined);
            }
            AddMapLayer(DundasMap, mapFolderName, "Территор", CRHelper.MapShapeType.CalloutTowns);
            FillMapData();

        }

        public static string GetShapeName(Shape shape)
        {
            string shapeName = shape.Name;
            if (IsCalloutTownShape(shape) && shape.Name.Split('_').Length > 1)
            {
                shapeName = shape.Name;//.Split('_')[0];
            }

            return shapeName;
        }

        public static ArrayList FindMapShape(MapControl map, string patternValue, out bool hasCallout)
        {
            hasCallout = false;
            ArrayList shapeList = new ArrayList();
            foreach (Shape shape in map.Shapes)
            {
                if (GetShapeName(shape).ToLower() == patternValue.ToLower())
                {
                    shapeList.Add(shape);
                    if (IsCalloutTownShape(shape))
                    {
                        hasCallout = true;
                    }
                }
                else
                {
                    shape.TextVisibility = TextVisibility.Shown;
                }
            }
            return shapeList;
        }

        private void AddMapLayer(MapControl map, string mapFolder, string layerFileName, CRHelper.MapShapeType shapeType)
        {
            string layerName = Server.MapPath(string.Format("../../../maps/{0}/{1}.shp", mapFolder, layerFileName));
            int oldShapesCount = map.Shapes.Count;

            map.LoadFromShapeFile(layerName, "Name", true);
            map.Layers.Clear();
            map.Layers.Add(shapeType.ToString());
            for (int i = oldShapesCount; i < map.Shapes.Count; i++)
            {
                Shape shape = map.Shapes[i];
                shape.Layer = shapeType.ToString();
            }
        }

        public void FillMapData()
        {
            bool hasCallout;
            string valueSeparator = IsMozilla ? ". " : "\n";
            string shapeHint = "Образовалось отходов производства и потребления в {0} г. {1} {2}";
            foreach (Shape shape in DundasMap.Shapes)
            {
                shape.Text = "";
                shape.Visible = false;
            }
            for (int j = 0; j < dtMap.Rows.Count; j++)
            {

                string subject = dtMap.Rows[j].ItemArray[0].ToString().Replace(" муниципальный район", String.Empty).Replace("Город ", "г. ");
                double value = Convert.ToDouble(dtMap.Rows[j].ItemArray[1].ToString());
                ArrayList shapeList = FindMapShape(DundasMap, subject, out hasCallout);
                foreach (Shape shape in shapeList)
                {
                    if (IsCalloutTownShape(shape))
                    {
                    }
                    else
                    {
                        if (!hasCallout)
                        {
                            shape.Visible = true;
                            shape["Name"] = subject;
                            shape["Cost"] = Convert.ToDouble(dtMap.Rows[j].ItemArray[1]);
                            shape.Name = subject;
                            shape.TextVisibility = TextVisibility.Shown;
                            if (smallLegend)
                            {
                                shape.Color = GetColor(Convert.ToDouble(dtMap.Rows[j].ItemArray[1]));
                            }
                            shape.BorderColor = Color.Gray;
                            shape.BorderWidth = 1;
                            Dundas.Maps.WebControl.Symbol symbol = new Dundas.Maps.WebControl.Symbol();
                            symbol.Name = shape.Name + DundasMap.Symbols.Count;
                            symbol.ParentShape = shape.Name;
                            symbol["PollutedWater"] = Convert.ToDouble(dtMap.Rows[j].ItemArray[1]) == 0 ? 0 : Convert.ToDouble(dtMap.Rows[j].ItemArray[2]) / Convert.ToDouble(dtMap.Rows[j].ItemArray[1]) * 100;
                            symbol.ToolTip = String.Format("Доля использованных и обезвреженных отходов от количества образованных в {0} г. {1}%", ComboYear.SelectedValue, String.Format("{0:0.##}", Convert.ToDouble(symbol["PollutedWater"])));
                            symbol.Color = Color.Red;
                            symbol.MarkerStyle = MarkerStyle.Triangle;
                            symbol.Offset.Y = -20;
                            symbol.Layer = shape.Layer;
                            DundasMap.Symbols.Add(symbol);
                            if (subject.StartsWith("г."))
                            {
                                shape.ToolTip = String.Format(shapeHint, ComboYear.SelectedValue, String.Format("{0:0.##}", Convert.ToDouble(dtMap.Rows[j].ItemArray[1])), edIsm) + ", " + subject.Split('.')[1];
                                shape.Text = subject + "\n" + String.Format("{0:0.##}", Convert.ToDouble(dtMap.Rows[j].ItemArray[1])) + ", " + edIsm + "\n" + String.Format("{0:0.##}", Convert.ToDouble(symbol["PollutedWater"]))+"%";
                            }
                            else
                            {
                                shape.ToolTip = String.Format(shapeHint, ComboYear.SelectedValue, String.Format("{0:0.##}", Convert.ToDouble(dtMap.Rows[j].ItemArray[1])), edIsm) + ", " + subject.Replace("р-н", String.Empty) + " МР";
                                shape.Text = subject + " р-н" + "\n" + String.Format("{0:0.##}", Convert.ToDouble(dtMap.Rows[j].ItemArray[1])) + ", " + edIsm + "\n" + String.Format("{0:0.##}", Convert.ToDouble(symbol["PollutedWater"]))+"%";
                            }
                        }
                        else
                        {
                        }
                    }
                        
                   
                }
            }
        }

        Color GetColor(Double val)
        {
            if (val < 1)
            {
                if (revPok)
                {
                    return Color.Green;
                }
                else
                {
                    return Color.Red;
                }
            }

            if (val < 2)
            {
                if (revPok)
                {
                    return Color.FromArgb(128, 192, 0);
                }
                else
                {
                    return Color.Orange;
                }
            }
            if (val < 3)
            {

                return Color.Yellow;

            }
            if (val < 4)
            {
                if (revPok)
                {
                    return Color.Orange;
                }
                else
                {
                    return Color.FromArgb(128, 192, 0);
                }
            }
            if (revPok)
            {
                return Color.Red;
            }
            else
            {
                return Color.Green;
            }

        }

        #endregion

        private void TransformGrid()
        {
            for (int i = 0; i < Grid.Rows.Count; i += 4)
            {
                Grid.Rows[i].Cells[0].Style.BorderDetails.StyleBottom = BorderStyle.None;
            }
        }
        #region Экспорт в Excel
        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            ReportExcelExporter1.WorksheetTitle = Label1.Text;
            ReportExcelExporter1.WorksheetSubTitle = Label2.Text;
            Workbook workbook = new Workbook();
            Worksheet sheet1 = workbook.Worksheets.Add("Таблица");
            Worksheet sheet2 = workbook.Worksheets.Add("Карта");
            ReportExcelExporter1.HeaderCellFont = new System.Drawing.Font("Verdana", 11);
            ReportExcelExporter1.TitleFont = new System.Drawing.Font("Verdana", 12, FontStyle.Bold);
            ReportExcelExporter1.SubTitleFont = new System.Drawing.Font("Verdana", 11);
            ReportExcelExporter1.TitleAlignment = HorizontalCellAlignment.Center;
            ReportExcelExporter1.TitleStartRow = 3;
            sheet2.Rows[2].Cells[0].Value = Label3.Text;
            DundasMap.Width = 1000;
            TransformGrid();
            ReportExcelExporter.MapExcelExport(sheet2.Rows[3].Cells[0], DundasMap);
            ReportExcelExporter1.Export(headerLayout, sheet1, 6);
            ReportExcelExporter1.WorksheetTitle = String.Empty;
            ReportExcelExporter1.WorksheetSubTitle = String.Empty;
            
            
           
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
          /*  e.Workbook.Worksheets["Таблица"].Rows[3].Height = 900;
            e.Workbook.Worksheets["Таблица"].Rows[4].Height = 950;
            e.Workbook.Worksheets["Таблица"].Rows[4].Cells[0].CellFormat.VerticalAlignment = VerticalCellAlignment.Center;
            e.Workbook.Worksheets["Диаграмма"].Rows[1].Cells[0].CellFormat.Alignment = HorizontalCellAlignment.Distributed;
            e.Workbook.Worksheets["Диаграмма"].Rows[1].Cells[0].CellFormat.VerticalAlignment = VerticalCellAlignment.Center;
            e.Workbook.Worksheets["Диаграмма"].Rows[1].Height = 800;*/
        }
        #endregion

        #region Экспорт в PDF
        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            ReportPDFExporter1.PageTitle = Label1.Text;
            ReportPDFExporter1.PageSubTitle = Label2.Text;
            ReportPDFExporter1.HeaderCellHeight = 30;
            Report report = new Report();
            ISection section1 = report.AddSection();
            ISection section2 = report.AddSection();
            ReportPDFExporter1.HeaderCellHeight = 60;
            TransformGrid();
            ReportPDFExporter1.Export(headerLayout, section1);
            IText title = section2.AddText();
            Infragistics.Documents.Reports.Graphics.Font font = new Infragistics.Documents.Reports.Graphics.Font("Verdana", 14);
            title.Style.Font = font;
            title.AddContent(Label3.Text + "\n\n   ");
            section2.PageSize = new PageSize(1000, 800);
          //  DundasMap.Viewport.LocationUnit = CoordinateUnit.Pixel;
          //  DundasMap.Viewport.ViewCenter.Y -= 10;
            DundasMap.Height = 600;
            DundasMap.Viewport.Location.X = 0;
            Infragistics.Documents.Reports.Graphics.Image img = UltraGridExporter.GetImageFromMap(DundasMap);
            section2.AddImage(img);
        }
        #endregion
    }
}