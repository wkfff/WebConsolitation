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
using Infragistics.UltraGauge.Resources;
using Dundas.Maps.WebControl;


namespace Krista.FM.Server.Dashboards.REGION_0010.UnEffectLessGKH_YANAO
{
    public partial class _default : CustomReportPage
    {
        public DataTable GetDSForChart(string sql)
        {
            DataTable dt = new DataTable();
            string s = DataProvider.GetQueryText(sql);
            DataProvidersFactory.SpareMASDataProvider.PopulateDataTableForChart(DataProvidersFactory.SpareMASDataProvider.GetCellset(s), dt, "ОЛОЛО");
            return dt;
        }
         
        CustomParam SelectItemGrid;
        CustomParam Chn;
        CustomParam Ks;
        CustomParam P3;
        CustomParam region;
        CustomParam RegionComparableDimention; 
        CustomParam P4;
        CustomParam LD;
        /*                        if (BN == "FIREFOX")
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
                        }*/

        string BN = "APPLEMAC-SAFARI";
        Double Coef = 1.0;
        Double Coef2 = 1.0;

        protected override void Page_PreLoad(object sender, System.EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            System.Web.HttpBrowserCapabilities myBrowserCaps = Request.Browser;
            BN = ((System.Web.Configuration.HttpCapabilitiesBase)myBrowserCaps).Browser.ToUpper();
            if (BN == "FIREFOX")
            {

                //Grid.Height = colRows * 28;
                Coef = 0.908;
                Coef2 = 1.2;
            }
            if (BN == "IE")
            {
                //Grid.Height = colRows * 24;
                Coef = 0.96;
                //Coef2 = 1.0;
            }
            if (BN == "APPLEMAC-SAFARI")
            {
                //Grid.Height = colRows * 24;
                Coef = 0.95;
                //Coef2 = 1.0;
            }




            SelectItemGrid = UserParams.CustomParam("Param");
            Chn = UserParams.CustomParam("Chn");
            Ks = UserParams.CustomParam("Ks");
            P3 = UserParams.CustomParam("p3");
            P4 = UserParams.CustomParam("p4");
            LD = UserParams.CustomParam("LD");
            
            region = UserParams.CustomParam("region");
            region.Value = RegionSettingsHelper.Instance.RegionBaseDimension;
            RegionComparableDimention = UserParams.CustomParam("RegionComparableDimention");
            RegionComparableDimention.Value = RegionSettingsHelper.Instance.GetPropertyValue("RegionComparableDimention"); 
            
            G.Width = (CustomReportConst.minScreenWidth - 10-5);
            G.Height = Unit.Empty;


            G2.Width = (CustomReportConst.minScreenWidth - 10-5);
            G2.Height = Unit.Empty;

            DundasMap.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 15-5);
            DundasMap.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.7);
            
            C1.Width = (CustomReportConst.minScreenWidth * 1-15-5);
            
            UltraGridExporter1.PdfExportButton.Visible = 1 == 2;

            ////###########################################################################
            UltraGridExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            UltraGridExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
            UltraGridExporter1.ExcelExporter.EndExport += new EndExportEventHandler(ExcelExporter_EndExport);
            UltraGridExporter1.ExcelExporter.HeaderCellExporting += new HeaderCellExportingEventHandler(ExcelExporter_HeaderCellExporting);
            //############################################################################



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

        void GetOtherCubValue()
        {
            DataTable dt = GetDSForChart("G0");
            Chn.Value = dt.Rows[0][1].ToString().Replace(',', '.');
            //Ks.Value = dt.Rows[0][1].ToString();
            //Yc.Value = dt.Rows[1][1].ToString();

        }


        protected override void Page_Load(object sender, EventArgs e)
        {
            //RegionSettingsHelper.Instance.SetWorkingRegion(RegionSettingsHelper.Instance.GetPropertyValue("setRegion"));
            base.Page_Load(sender, e);


            if (!Page.IsPostBack)
            {
                //C1.Data.SwapRowsAndColumns = !(C1.Data.SwapRowsAndColumns);
                //C2.Data.SwapRowsAndColumns = !(C2.Data.SwapRowsAndColumns);
            }
            Panel1.Visible = CheckBox1.Checked;
            Panel2.Visible = CheckBox1.Checked;
            //GetOtherCubValue();
            G.DataBind();
            LD.Value = G.Columns[G.Columns.Count - 1].Header.Caption;
            object[] xz = { "", "Р общий", "Ржкх общий", "Ржкх", "Джкх=Ржкх/Ржкх общий*100%", "Джкх общ=Ржкх/Робщий*100%", "", "Ид =(Джкхмакс - Джкх) / (Джкхмакс - Джкхмин)" };
            //Северо-Западный ФО
            object[] xz2 = { RegionSettingsHelper.Instance.GetPropertyValue("UpLevelName"), " ", "", "", "", "", "","" };            

            G2.DataBind();
            GetRang(G2, 5, 1 == 2);
            GetRang(G2, 4, 1 == 2);
            GetRang(G2, 7, 12 == 12);
            GoMap();


            
            //G2.Columns[1].Hidden = 1 == 1;




            C1.DataBind();


            //C2.DataBind();

            ////G3.DataBind();



            ColumnFormula();

            Hederglobal.Text = string.Format("Оценка неэффективных расходов в сфере жилищно-коммунального хозяйства ({0})", UserComboBox.getLastBlock(region.Value));
            Page.Title = Hederglobal.Text;
            try
            {
                ScanGridPreText(7, G2.Rows[IndexMaxG2_6].Cells[7].Text.Split('(')[0], "<img style=\"FLOAT: left;\" src=\"../../../../images/starYellowBB.png\">");
                //G2.Rows[IndexMaxG2_6].Cells[7].Title = "Самый высокий уровень эффективности";
                ScanGridPreText(7, G2.Rows[IndexMinG2_6].Cells[7].Text.Split('(')[0], "<img style=\"FLOAT: left;\" src=\"../../../../images/starGrayBB.png\">");
                //G2.Rows[IndexMinG2_6].Cells[7].Title = "Самый низкий уровень эффективности";

                ScanGridPreText(4, G2.Rows[IndexMinG2].Cells[4].Text.Split('(')[0], "<img style=\"FLOAT: left;\" src=\"../../../../images/starYellowBB.png\">");
                //G2.Rows[IndexMaxG2].Cells[4].Title = "Самая высокая доля неэффективных расходов";
                ScanGridPreText(4, G2.Rows[IndexMaxG2].Cells[4].Text.Split('(')[0], "<img style=\"FLOAT: left;\" src=\"../../../../images/starGrayBB.png\">");
                ///G2.Rows[IndexMaxG2].Cells[4].Title = "Самая низкая доля неэффективных расходов";


                ScanGridPreText(5, G2.Rows[IndexMinG2_5].Cells[5].Text.Split('(')[0], "<img style=\"FLOAT: left;\" src=\"../../../../images/starYellowBB.png\">");
                //G2.Rows[IndexMinG2_5].Cells[5].Title = "Самая высокая доля неэффективных расходов";
                ScanGridPreText(5, G2.Rows[IndexMaxG2_5].Cells[5].Text.Split('(')[0], "<img style=\"FLOAT: left;\" src=\"../../../../images/starGrayBB.png\">");
                //G2.Rows[IndexMaxG2_5].Cells[5].Title = "Самая низкая доля неэффективных расходов";
                /////////////
            }
            catch { } 




            G2.Rows.Insert(0, new UltraGridRow(xz));
            for (int i = 0; i < G2.Columns.Count; i++)
            {
                G2.Rows[0].Cells[i].Style.HorizontalAlign = HorizontalAlign.Left;
            }

            G2.Rows.Insert(1, new UltraGridRow(xz2));
            G2.Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            G2.Rows[1].Cells[0].ColSpan = 6;
            G2.Rows[1].Cells[0].Style.Font.Bold = 1 == 1;
                        
            Label2.Text = string.Format("Оценка эффективности расходов бюджетов субъектов РФ на {0} год",G.Columns[G.Columns.Count-1].Header.Caption);
            Label3.Text = String.Format("Уровень эффективности расходования бюджетных средств в сфере жкх на {0} год, %", G.Columns[G.Columns.Count - 1].Header.Caption);

            G.Rows[1].Hidden = 1 == 1;
            G.Rows[2].Hidden = 1 == 1;
            G.Rows[4].Hidden = 1 == 1;
            G.Rows[5].Hidden = 1 == 1;

            Label5.Text = Label5.Text = RegionSettingsHelper.Instance.GetPropertyValue("RegionSubTitle");

        }
        void ScanGridPreText(int Column, string Val, string image)
        {
            for (int i = 0; i < G2.Rows.Count; i++)
            {
                if (Val == G2.Rows[i].Cells[Column].Text.Split('(')[0])
                {
                    G2.Rows[i].Cells[Column].Text = string.Format("{0}{1:### ### ###0.00}", image, G2.Rows[i].Cells[Column].Value);

                    if (((image == "<img style=\"FLOAT: left;\" src=\"../../../../images/starYellowBB.png\">") & (Column == 7)) || ((image != "<img style=\"FLOAT: left;\" src=\"../../../../images/starYellowBB.png\">") & (Column != 7)))
                    {
                        G2.Rows[i].Cells[Column].Title = Column==7?"Самый высокий уровень эффективности": "Самая высокая доля неэффективных расходов";

                    }
                    else
                    {
                        G2.Rows[i].Cells[Column].Title = Column==7?"Самый низкий уровень эффективности":"Самая низкая доля неэффективных расходов";
                    }
                }
            }
        }
        string[] columnFormul1 = {
            "Робщий", 
            "Ржкх общий", 
            @"Джкх = Ржкх / Ржкх общий x 100%<br>
Показатель рассчитывается путем деления 
объема неэффективных расходов в сфере жкх на расходы субъекта РФ в сфере жкх и умножения на 100%
", 
            @"Джкх общ = Ржкх / Робщий x 100%<br>

Показатель рассчитывается путем деления 
объема неэффективных расходов в сфере жкх  на расходы консолидированного бюджета РФ и умножения на 100%
", 
            @"Ржкх<br>
Неэффективным расходованием бюджетных средств в сфере жилищно-коммунального хозяйства является направление бюджетных средств на компенсацию предприятиям жилищно-коммунальной сферы разницы между экономически обоснованными тарифами и тарифами, установленными для населения, и на покрытие убытков предприятий жилищно-коммунального хозяйства, возникших в связи с применением регулируемых цен на жилищно-коммунальные услуги (в соответствии с Постановление Правительства РФ №322 от 15 апреля 2009 г.).
    "
        };
        string[] columnFormul2 = {
            "Р4 = (Р14хЧн)/1000", 
            "Чн", 
            "P14 = (Кф - Кц х Кс) х Ск х Нц", 
            "Кф", 
            "Кц",
            "Ск",
            "Кс",            
            "Нц",
            "Р5 = (Р15хЧн)/1000",
            "Чн",
            "P15 = (Hц - Hф) х Кф х Ск", 
            "Нц", 
            "Нф", 
            "Ск",
            "Кф",
            "Р6 = (Р16хЧн)/1000",
            "Чн",
            "Р16 = (Уф - Уц) х Скд х Сдлф",
            "Уф",
            "Уц",
            "Сдлф",
            "Скд"};
        string[] columnFormul3 = {
            "О2= О21 + О22", 
            "О21= (Чуг / Нфг - Чуг /Нцг х Кс) х Ск /1000", 
            "Чуг", 
            "Нфг", 
            "Нцг", 
            "Ск",
            "Кс",
            "О22= (Чус / Нфс - Чус /Нцс х Кс) х Ск /1000",
            "Чус",
            "Нфс",
            "Нцс",
            "Кс",
            "Ск"
             };
        void ColumnFormula2(UltraWebGrid G_, string[] columnFormul)
        {
            int lastColumn = G_.Columns.Count;
            G_.Columns.Add("Формула");
            G_.Columns[lastColumn].Header.Caption = "Формула";
            try
            {
                for (int i = 0; i < G_.Rows.Count; i++)
                {
                    G_.Rows[i].Cells[lastColumn].Text += columnFormul[i];
                }
            }
            catch { }

            G_.Columns[lastColumn].Width = 300;
            G_.Columns[lastColumn].CellStyle.Wrap = 1 == 1;
            G_.Columns[lastColumn].Move(1);

        }
        void ColumnFormula()
        {

            for (int i = 1; i < G.Columns.Count; i++)
            {
                try
                {
                    G.Rows[4 * 3].Cells[i].Style.CustomRules = "background-repeat: no-repeat; background-position: right top; margin: 2px";
                    G.Rows[4 * 3].Cells[i].Style.BackgroundImage = "~/images/cornerRed.gif";
                    G.Rows[4 * 3].Cells[i].Title = "Имеются неэффективные расходы";
                }
                catch { }
                //TODO: Доделать если ноль
            }

            
            int lastColumn = G.Columns.Count;
            G.Columns.Add("Формула");
            try
            {

                G.Rows[0 * 3].Cells[lastColumn].RowSpan = 1;
                G.Rows[0 * 3].Cells[lastColumn].Text = columnFormul1[0];
                //G.Rows[0 * 3].Cells[0].Style.Font.Bold = 1 == 1;

                G.Rows[1 * 3].Cells[lastColumn].RowSpan = 1;
                G.Rows[1 * 3].Cells[lastColumn].Text = columnFormul1[1];
                //G.Rows[1 * 3].Cells[0].Style.Font.Bold = 1 == 1;

                G.Rows[2 * 3].Cells[lastColumn].RowSpan = 3;
                G.Rows[2 * 3].Cells[lastColumn].Text = columnFormul1[2];
                G.Rows[2 * 3].Cells[0].Style.Font.Bold = 1 == 1;

                G.Rows[3 * 3].Cells[lastColumn].RowSpan = 3;
                G.Rows[3 * 3].Cells[lastColumn].Text = columnFormul1[3];
                G.Rows[3 * 3].Cells[0].Style.Font.Bold = 1 == 1;

                G.Rows[4 * 3].Cells[lastColumn].RowSpan = 3;
                G.Rows[4 * 3].Cells[lastColumn].Text = columnFormul1[4];
                G.Rows[4 * 3].Cells[0].Style.Font.Bold = 1 == 1;

                G.Rows[5 * 3].Cells[lastColumn].RowSpan = 3;
                G.Rows[5 * 3].Cells[lastColumn].Text = columnFormul1[5];
                G.Rows[5 * 3].Cells[0].Style.Font.Bold = 1 == 1;

            }
            catch { }
            G.Columns[lastColumn].Width = 300;
            G.Columns[lastColumn].CellStyle.Wrap = 1 == 1;
            G.Columns[lastColumn].Header.Caption = "Формула";
            G.Columns[lastColumn].Move(1);

        }

        Dictionary<string, int> param_3;

        System.Double FirstCell = 0;
        System.Double up = 0;
        System.Double down = 0;

        string CalcCell()
        {
            string res = "";
            System.Double OLOLO = down - up;
            System.Double OlOlO = 100.0 * (down / up - 1);
            int xz = FirstCell > down ? -1 : FirstCell == down ? 0 : 1;

            string image = "";
            if (OlOlO < 0)
            {
                image = "<img style=\"FLOAT: left;\" src=\"../../../../images/arrowGreenDownBB.png\">";
            }
            if (OlOlO > 0)
            {
                image = "<img style=\"FLOAT: left;\" src=\"../../../../images/arrowRedUpBB.png\">";
            }

            if (System.Double.IsInfinity(OlOlO))
            {
                return "-";
            }            
            res = image + OlOlO.ToString("### ### ##0.00") + "%";
            return res;
        }

        protected void G_DataBinding(object sender, EventArgs e)
        {
            G.Rows.Clear();
            G.Columns.Clear();
            G.Bands.Clear();

            DataTable dt = GetDSForChart("G");
            DataTable dtNew = new DataTable();
            try
            { }
            catch { }
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                dtNew.Columns.Add(dt.Columns[i].Caption, dt.Columns[0].DataType);
            }

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                dtNew.Rows.Add(new object[dt.Columns.Count]);
                dtNew.Rows.Add(new object[dt.Columns.Count]);
                dtNew.Rows.Add(new object[dt.Columns.Count]);
            }
            for (int i = 0; i < dtNew.Rows.Count / 3; i++)
            {
                dtNew.Rows[i * 3][0] =  dt.Rows[i][0] ;//+ "<b>""</b>";
                dtNew.Rows[i * 3 + 1][0] = "<div style=\"FLOAT: Right;\">абсолютные отклонения&nbsp&nbsp</div>";
                dtNew.Rows[i * 3 + 2][0] = "<div style=\"FLOAT: Right;\">темп прироста&nbsp&nbsp</div>";
                //абсолютные отклонения

                for (int j = dt.Columns.Count - 1; j >= 1; j--)
                {
                    try
                    {
                        down = (System.Double)(System.Decimal)dt.Rows[i][j];
                        dtNew.Rows[i * 3][j] = down.ToString("### ### ##0.00");



                        up = (System.Double)(System.Decimal)dt.Rows[i][j - 1];



                        //int c = 1;
                        for (int x = 1; x < dt.Columns.Count - 1; x++)
                        {
                            try
                            {
                                FirstCell = (System.Double)(System.Decimal)dt.Rows[i][x];
                                if (FirstCell == 0) continue;
                                break;
                            }
                            catch { }
                        }

                        dtNew.Rows[i * 3 + 1][j] = ((System.Double)(down - up)).ToString("### ### ##0.00");
                        dtNew.Rows[i * 3 + 2][j] = CalcCell();
                    }
                    catch { }
                }
            }
            G.DataSource = dtNew;
        }

        int ld;
        protected void G_InitializeLayout(object sender, LayoutEventArgs e)
        {
            int BigPig = 0;
            if ((UltraWebGrid)(sender) != G)
            {
                BigPig -= 60;
            }
            else
            {
                BigPig += -60;

            }
            System.Double coef6 = 1;
            if (BN == "FIREFOX")
            {
                coef6 = 1.03;
            }

            if (BN == "IE")
            {
                //coef6 = 0.98;
            }

            if (BN == "APPLEMAC-SAFARI")
            {
                //coef6 = 0.98;
            }
            
            for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i++)
            {
                e.Layout.Bands[0].Columns[i].Header.Style.Wrap = 1 == 1;
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "### ### ##0.00");
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth((CustomReportConst.minScreenWidth / e.Layout.Bands[0].Columns.Count + BigPig) * Coef);
            }
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = 1 == 1;
            e.Layout.Bands[0].Columns[0].Header.Caption = "Показатель";
            if ((UltraWebGrid)(sender) == G) { 
            if(G.Columns.Count>3)
                e.Layout.Bands[0].Columns[1].Hidden = 1 == 1; }
            

            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth((520 + BigPig) * Coef*coef6);

            for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i++)
            {
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(((CustomReportConst.minScreenWidth - 520 - BigPig-240) / (e.Layout.Bands[0].Columns.Count - 1) + BigPig) * Coef * coef6);
            }
            //e.Layout.Bands[0].Columns[2].Width = CRHelper.GetColumnWidth((200 + BigPig) * Coef * coef6);
            //e.Layout.Bands[0].Columns[3].Width = CRHelper.GetColumnWidth((200 + BigPig) * Coef * coef6);
            e.Layout.AllowSortingDefault = AllowSorting.No;
            //e.Layout.Bands[0].







        }

        protected void G_InitializeRow(object sender, RowEventArgs e)
        {
            try
            {
                if (e.Row.Cells[0].Text[0] == '_')
                {
                    e.Row.Cells[0].Style.Padding.Left = 15;
                    e.Row.Cells[0].Text = e.Row.Cells[0].Text.Remove(0, 1);
                }
                else
                {
                    if ((UltraWebGrid)(sender) != G)
                        e.Row.Cells[0].Style.Font.Bold = 1 == 1;
                }
                try
                {
                    e.Row.Cells[0].Text = e.Row.Cells[0].Text.Split('_')[1];
                }
                catch { }
                if ((e.Row.Index == 0) & ((UltraWebGrid)(sender) != G))
                {
                    e.Row.Cells[0].Style.Font.Bold = 1 == 1;
                }
                for (int i = 0; i < e.Row.Cells.Count; i++)
                {
                    if (string.IsNullOrEmpty(e.Row.Cells[i].Text))
                    { 
                         e.Row.Cells[i].Text = "-";
                    }
                }

            }
            catch { }
        }

        protected void Type1_Load(object sender, EventArgs e)
        {

        }
        private void ExcelExporter_BeginExport(object sender, BeginExportEventArgs e)
        {
            e.CurrentWorksheet.Rows[0].Cells[0].Value = Label1.Text;
            e.CurrentWorksheet.Rows[0].Cells[0].CellFormat.Font.Bold = Infragistics.Documents.Excel.ExcelDefaultableBoolean.True;
        }

        void SetCellFormat(EndExportEventArgs e, int Col, int Row, int CellHeight, int CellWidth, HorizontalCellAlignment HCA, VerticalCellAlignment VCA, Font F, int FHeight)
        {
            if (CellHeight >= 0)
            {
                e.CurrentWorksheet.Rows[Row].Height = CellHeight;
            }
            if (CellWidth >= 0)
            {
                e.CurrentWorksheet.Columns[Col].Width = CellWidth;
            }
            if (HCA != HorizontalCellAlignment.Default)
            {
                e.CurrentWorksheet.Rows[Row].Cells[Col].CellFormat.Alignment = HCA;
            }
            if (VCA != VerticalCellAlignment.Default)
            {
                e.CurrentWorksheet.Rows[Row].Cells[Col].CellFormat.VerticalAlignment = VCA;
            }
            if (F != null)
            {
                e.CurrentWorksheet.Rows[Row].Cells[Col].CellFormat.Font.Name = F.Name;

                if (F.Bold)
                {
                    e.CurrentWorksheet.Rows[Row].Cells[Col].CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
                }

            }
            if (FHeight > 0)
            {
                e.CurrentWorksheet.Rows[Row].Cells[Col].CellFormat.Font.Height = FHeight;
            }
            e.CurrentWorksheet.Rows[Row].Cells[Col].CellFormat.WrapText = ExcelDefaultableBoolean.True;
        }

        private void ExcelExporter_EndExport(object sender, EndExportEventArgs e)
        {
            //DataTable dt = GetDSForChart("G");

            for (int i = 0; i < G.Columns.Count + 10; i++)
            {
                e.CurrentWorksheet.Columns[i].Width = 6000;
                for (int j = 0; j < G.Rows.Count + 10; j++)
                {
                    e.CurrentWorksheet.Rows[j].Cells[i].Value = "";
                    e.CurrentWorksheet.Rows[j].Cells[i].CellFormat.Font.Bold = ExcelDefaultableBoolean.False;
                    e.CurrentWorksheet.Rows[j].Cells[i].CellFormat.FillPatternBackgroundColor = Color.White;
                    e.CurrentWorksheet.Rows[j].Cells[i].CellFormat.FillPatternForegroundColor = Color.White;

                    e.CurrentWorksheet.Rows[j].Cells[i].CellFormat.BottomBorderColor = Color.LightGray;
                    e.CurrentWorksheet.Rows[j].Cells[i].CellFormat.BottomBorderStyle = CellBorderLineStyle.Default;

                    e.CurrentWorksheet.Rows[j].Cells[i].CellFormat.LeftBorderColor = Color.LightGray;
                    e.CurrentWorksheet.Rows[j].Cells[i].CellFormat.LeftBorderStyle = CellBorderLineStyle.Default;

                    e.CurrentWorksheet.Rows[j].Cells[i].CellFormat.RightBorderColor = Color.LightGray;
                    e.CurrentWorksheet.Rows[j].Cells[i].CellFormat.RightBorderStyle = CellBorderLineStyle.Default;

                    e.CurrentWorksheet.Rows[j].Cells[i].CellFormat.TopBorderColor = Color.LightGray;
                    e.CurrentWorksheet.Rows[j].Cells[i].CellFormat.TopBorderStyle = CellBorderLineStyle.Default;
                }
            }

            e.CurrentWorksheet.Name = "Неэффективные расходы";

            e.CurrentWorksheet.Rows[0].Cells[0].Value = Hederglobal.Text;
            SetCellFormat(e,0,0,1000,-1,HorizontalCellAlignment.Left,VerticalCellAlignment.Center,new Font("Arial",1,FontStyle.Bold),280);
            e.CurrentWorksheet.MergedCellsRegions.Add(0, 0, 0, 4);

            e.CurrentWorksheet.Rows[1].Cells[0].Value = Label5.Text;
            SetCellFormat(e, 0, 1, 1000, -1, HorizontalCellAlignment.Left, VerticalCellAlignment.Center, new Font("Arial",11), 240);
            e.CurrentWorksheet.MergedCellsRegions.Add(1, 0, 1, 4);

            e.CurrentWorksheet.Rows[2].Cells[0].Value = Label1.Text;
            SetCellFormat(e, 0, 2, 1000, -1, HorizontalCellAlignment.Left, VerticalCellAlignment.Center, new Font("Arial", 1, FontStyle.Bold), 260);
            e.CurrentWorksheet.MergedCellsRegions.Add(2, 0, 2, 4);
              
            DataTable dt = GetDSForChart("G");
            //dt.Columns.Remove(dt.Columns[1]);   
            int offset = Export_DT(dt, 3, e, 1 == 1);
           // e.CurrentWorksheet.Columns[1].Hidden = 1 == 1;
            e.CurrentWorksheet.Rows[4+1].Hidden = 1 == 1;
            e.CurrentWorksheet.Rows[5+1].Hidden = 1 == 1;
            e.CurrentWorksheet.Rows[7+1].Hidden = 1 == 1;
            e.CurrentWorksheet.Rows[8+1].Hidden = 1 == 1;
            e.CurrentWorksheet.Rows[4].Cells[0].CellFormat.Font.Bold = ExcelDefaultableBoolean.False;
            e.CurrentWorksheet.Rows[7].Cells[0].CellFormat.Font.Bold = ExcelDefaultableBoolean.False;
            //5 8 11 14 17 20 23 26

            e.CurrentWorksheet.Columns[0].Width = 24000;
            if (CheckBox1.Checked)
            {
                e.Workbook.Worksheets.Add("Сравнение субъектов");
                e.CurrentWorksheet = e.Workbook.Worksheets["Сравнение субъектов"];
                e.CurrentWorksheet.Rows[0].Cells[0].Value = Label3.Text;
                SetCellFormat(e, 0, 0, 1000, -1, HorizontalCellAlignment.Left, VerticalCellAlignment.Center, new Font("Arial", 1, FontStyle.Bold), 260);                
                e.CurrentWorksheet.MergedCellsRegions.Add(0, 0, 0, 6);
                
                GlobalDT2.Columns.Remove(GlobalDT2.Columns[GlobalDT2.Columns.Count - 2]);
                offset = Export_DT(GlobalDT2, 1, e, 1 == 2);
                e.CurrentWorksheet.Columns[0].Width = 12000;
           }
           
        }

        int Export_DT(DataTable dt, int offset, EndExportEventArgs e, bool CalcTemp)
        {
            int xz = CalcTemp ? 3 : 1;
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                try
                {
                    e.CurrentWorksheet.Rows[offset].Cells[i].Value = dt.Columns[i].Caption;

                    e.CurrentWorksheet.Rows[offset].Cells[i].CellFormat.FillPattern = FillPatternStyle.Default;
                    e.CurrentWorksheet.Rows[offset].Cells[i].CellFormat.FillPatternBackgroundColor = Color.Gray;
                    e.CurrentWorksheet.Rows[offset].Cells[i].CellFormat.FillPatternForegroundColor = Color.Gray;
                    SetCellFormat(e, i, offset, 1000, -1, HorizontalCellAlignment.Left, VerticalCellAlignment.Top, new Font("Arial", 1, FontStyle.Bold), 200);                
                    e.CurrentWorksheet.Columns[i].Width = 6000;
                    e.CurrentWorksheet.Rows[offset].Height = !CalcTemp ? 1500 : 700;

                    for (int j = 0; j < dt.Rows.Count * xz; j += xz)
                    {
                        if (CalcTemp)
                        {
                            e.CurrentWorksheet.Rows[j + offset + 1].Cells[0].CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
                        }
                        e.CurrentWorksheet.Rows[j + offset + 1].Height = 1000;
                        if (dt.Rows[j / xz][i].ToString()[0] == '_')
                        {
                            dt.Rows[j / xz][i] = "  " + dt.Rows[j / xz][i].ToString().Remove(0, 1);
                        }
                        
                        e.CurrentWorksheet.Rows[j + offset + 1].Cells[i].Value = dt.Rows[j / xz][i];
                        SetCellFormat(e, i, j + offset + 1, -1, -1, HorizontalCellAlignment.Right, VerticalCellAlignment.Bottom, new Font("Arial", 1), 200);
                        
                        try
                        {
                            e.CurrentWorksheet.Rows[j + offset + 1].Cells[i].Value = ((System.Decimal)(dt.Rows[j / xz][i])).ToString("### ### ##0.00");
                        }
                        catch { }                        
                        if (i != 0)
                        {                            
                            if (CalcTemp)
                            {

                                try
                                {
                                    SetCellFormat(e, i, j + offset + 2, -1, -1, HorizontalCellAlignment.Right, VerticalCellAlignment.Bottom, new Font("Arial", 1), 200);
                                    SetCellFormat(e, i, j + offset + 3, -1, -1, HorizontalCellAlignment.Right, VerticalCellAlignment.Bottom, new Font("Arial", 1), 200);
                                    e.CurrentWorksheet.Rows[j + offset + 2].Cells[i].Value = string.Format("{0:### ### ##0.00}", (System.Decimal)(dt.Rows[j / xz][i]) - (System.Decimal)(dt.Rows[j / xz][i - 1]));
                                    e.CurrentWorksheet.Rows[j + offset + 3].Cells[i].Value = string.Format("{0:### ### ##0.00}", (((System.Decimal)(dt.Rows[j / xz][i]) / (System.Decimal)(dt.Rows[j / xz][i - 1]) - 1) * 100));
                                    
                                }
                                catch { }
                            }

                        }
                        else
                        {
                            SetCellFormat(e, i, j + offset + 1, -1, -1, HorizontalCellAlignment.Left, VerticalCellAlignment.Top, new Font("Arial", 1), 210);
                            if (CalcTemp)
                            {
                                e.CurrentWorksheet.Rows[j + offset + 2].Cells[i].Value = "абсолютные отклонения";                                
                                e.CurrentWorksheet.Rows[j + offset + 3].Cells[i].Value = "темп прироста";
                                
                                SetCellFormat(e, i, j + offset + 2, -1, -1, HorizontalCellAlignment.Right, VerticalCellAlignment.Bottom, new Font("Arial", 1), 200);
                                SetCellFormat(e, i, j + offset + 3, -1, -1, HorizontalCellAlignment.Right, VerticalCellAlignment.Bottom, new Font("Arial", 1), 200);
                            }
                        }
                    }
                }
                catch { }

            }
            e.CurrentWorksheet.Rows[offset].Cells[0].Value = "Показатель";
            return dt.Rows.Count * xz + 6 + offset;
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

        protected void C_DataBinding(object sender, EventArgs e)
        {

        }

        protected void C1_DataBinding(object sender, EventArgs e)
        {
            DataTable dt = GetDSForChart("C2");
            dt.Rows[0][0] = @"Доля неэффективных расходов в сфере жкх в общем объеме 
расходов консолидированного бюджета субъекта РФ на жкх";
            dt.Rows[1][0] = @"Доля неэффективных расходов в сфере жкх в общем 
объеме расходов бюджета";
            System.Decimal max = 0;
            for (int i = 1; i < dt.Columns.Count; i++)
            {
                if (max < (System.Decimal)(dt.Rows[0][i]))
                {
                    max = (System.Decimal)(dt.Rows[0][i]);
                }   
                if (max < (System.Decimal)(dt.Rows[1][i]))
                {
                    max = (System.Decimal)(dt.Rows[1][i]);
                }
            }
            C1.Axis.Y.RangeType = Infragistics.UltraChart.Shared.Styles.AxisRangeType.Custom;
            C1.Axis.Y.RangeMax = (System.Double)max;
            C1.Axis.Y.RangeMin = 0;

            C1.DataSource = dt;
            C1.Axis.Y.TickmarkInterval = ((System.Double)max-0.00001) / 10;
        }

        protected void C2_DataBinding(object sender, EventArgs e)
        {
            DataTable dt = GetDSForChart("C2");
            dt.Rows[0][0] = @"Объем эффективных расходов в сфере общего образования";
            dt.Rows[1][0] = @"Объем неэффективных расходов в сфере общего образования";
        }

        protected void UltraWebGrid1_DataBinding(object sender, EventArgs e)
        {

        }

        int IndexMaxG2 = 0, IndexMinG2 = 0;
        int IndexMaxG2_5 = 0, IndexMinG2_5 = 0;
        int IndexMaxG2_6 = 0, IndexMinG2_6 = 0;
        protected void G2_DataBinding(object sender, EventArgs e)
        {
            G2.Rows.Clear();
            G2.Columns.Clear();
            G2.Bands.Clear();

            DataTable  dt= GetDSForChart("G2");
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (((System.Decimal)(dt.Rows[i][4])) > ((System.Decimal)(dt.Rows[IndexMaxG2][4])))
                {
                    IndexMaxG2 = i;
                }
                if (((System.Decimal)(dt.Rows[i][4])) < ((System.Decimal)(dt.Rows[IndexMinG2][4])))
                {
                    IndexMinG2 = i;
                }
                if (((System.Decimal)(dt.Rows[i][5])) > ((System.Decimal)(dt.Rows[IndexMaxG2_5][5])))
                {
                    IndexMaxG2_5 = i;
                }
                if (((System.Decimal)(dt.Rows[i][5])) < ((System.Decimal)(dt.Rows[IndexMinG2_5][5])))
                {
                    IndexMinG2_5 = i;
                }
            }

            dt.Columns.Add("Уровень эффективности расходования бюджетных средств, %",dt.Columns[1].DataType);
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                try
                {
                    dt.Rows[i][dt.Columns.Count - 1] = 100 * ((System.Decimal)(dt.Rows[IndexMaxG2][4]) - ((System.Decimal)(dt.Rows[i][4])))
                        / ((System.Decimal)(dt.Rows[IndexMaxG2][4]) - (System.Decimal)(dt.Rows[IndexMinG2][4]));
                }
                catch 
                {
                    dt.Rows[i][dt.Columns.Count - 1] = 0;
//                    String.Format(@"Деление на 0, dt.Rows[i][dt.Columns.Count - 1] = 100 * ((System.Decimal)(dt.Rows[IndexMaxG2][4]) - ((System.Decimal)(dt.Rows[i][4])))
//                        / ((System.Decimal)(dt.Rows[IndexMaxG2][4]) - (System.Decimal)(dt.Rows[IndexMinG2][4]));");
                }
            }
            for (int i = 0; i < dt.Rows.Count; i++)
            {

                if (((System.Decimal)(dt.Rows[i][7])) > ((System.Decimal)(dt.Rows[IndexMaxG2_6][7])))
                {
                    IndexMaxG2_6 = i;
                }
                if (((System.Decimal)(dt.Rows[i][7])) < ((System.Decimal)(dt.Rows[IndexMinG2_6][7])))
                {
                    IndexMinG2_6 = i;
                }
            }

            GlobalDT2 = dt;

            G2.DataSource = dt;
            
        }
        DataTable GlobalDT2;

        void GetRang(UltraWebGrid dt, int indexColumn, bool reverce)
        {
            System.Decimal[] Values = new System.Decimal[dt.Rows.Count];
            int[] Nomber = new int[dt.Rows.Count];
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                Values[i] = (System.Decimal)(dt.Rows[i].Cells[indexColumn].Value);
                Nomber[i] = i;
            }
            for (int i = 0; i < Values.Length; i++)
            {
                for (int j = i; j < Values.Length; j++)
                {


                    if (((Values[i] > Values[j]) & (!reverce)) || ((Values[i] < Values[j]) & (reverce)))
                    {
                        System.Decimal buf = Values[i];
                        Values[i] = Values[j];
                        Values[j] = buf;

                        int buf_ = Nomber[i];
                        Nomber[i] = Nomber[j];
                        Nomber[j] = buf_;
                    }
                }
            }

            int mine = 0;
            for (int i = 0; i < Nomber.Length; i++)
            {
                //String.Format(string.Format("{0:### ##0.00}({1})", Values[i], i));
                try
                {
                    if (Values[i] == Values[i - 1])
                    {
                        mine++;
                    }
                }
                catch { }
                if (reverce)
                {
                    // dt.Rows[Nomber[i]].Cells[indexColumn].Text = string.Format("{0:### ##0.00}({1})", Values[i], Nomber.Length-i-mine);
                    dt.Rows[Nomber[i]].Cells[indexColumn].Text = string.Format("{0:### ##0.00}({1})", Values[i], i + 1 - mine);
                }
                else
                {
                    dt.Rows[Nomber[i]].Cells[indexColumn].Text = string.Format("{0:### ##0.00}({1})", Values[i], i + 1 - mine);
                }
            }
        }



        protected void G3_DataBinding(object sender, EventArgs e)
        {

        }

        protected void G3_DataBound(object sender, EventArgs e)
        {

        }

        protected void CheckBox1_CheckedChanged(object sender, EventArgs e)
        {
            Panel1.Visible = CheckBox1.Checked;
        }

        protected void C1_FillSceneGraph(object sender, Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e)
        {
            Infragistics.UltraChart.Core.Primitives.Text textLabel;


            int textWidth = 400;
            int textHeight = 12;
            try
            {
                textLabel = new Infragistics.UltraChart.Core.Primitives.Text();
                textLabel.PE.Fill = Color.Black;
                textLabel.bounds = new Rectangle(25, 243 + 12+12, textWidth, textHeight);
                textLabel.SetTextString("Доля неэффективных расходов в общей сумме расходов на жкх");
                e.SceneGraph.Add(textLabel);
                textLabel = new Infragistics.UltraChart.Core.Primitives.Text();
                textLabel.PE.Fill = Color.Black;
                textLabel.bounds = new Rectangle(243+203-30-15+3, 243 + 12+12, textWidth, textHeight);
                textLabel.SetTextString("Доля неэффективных расходов в общей сумме расходов");
                e.SceneGraph.Add(textLabel);
            }
            catch
            {

            }

        }

        protected void G2_InitializeLayout(object sender, LayoutEventArgs e)
        {            
            e.Layout.Bands[0].Columns[0].Header.Caption = "Субъект";
            System.Double Coef2 = 1;
            if (BN == "FIREFOX")
            {                
                Coef2 = 0.945;
            }
            if (BN == "IE")
            {
                Coef2 = 0.95;
            }
            if (BN == "APPLEMAC-SAFARI")
            {
                Coef2 = 0.95;
            }

            for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
            { 
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(170*Coef2);
                e.Layout.Bands[0].Columns[i].Header.Style.Wrap = 1 == 1;
                e.Layout.Bands[0].Columns[i].CellStyle.Wrap = 1 == 1;
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "N2");
                
            }
            e.Layout.Bands[0].Columns[e.Layout.Bands[0].Columns.Count - 2].Hidden = 2 == 2;                
            //CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[e.Layout.Bands[0].Columns.Count-1], "### ##0.00");
        }

        protected void G2_InitializeRow(object sender, RowEventArgs e)
        {

        }
        //#########################################################
        #region map


        private void GoMap()
        {
            DundasMap.Shapes.Clear();


            DundasMap.LoadFromShapeFile(Server.MapPath(string.Format("../../../../maps/УрФО/УрФО.shp")), "NAME", true);
       
            SetupMap();
            // заполняем карту данными
            FillMapData();
        }



        private void SetupMap()
        {
            DundasMap.Meridians.Visible = false;
            DundasMap.Parallels.Visible = false;
            DundasMap.ZoomPanel.Visible = false;
            DundasMap.NavigationPanel.Visible = false;
            DundasMap.Viewport.EnablePanning = true;

            // добавляем легенду
            Legend legend = new Legend("CompleteLegend");
            legend.Visible = true;
            legend.BackColor = Color.White;
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
            legend.Title = "Значение показателя";
            legend.AutoFitMinFontSize = 7;
            DundasMap.Legends.Clear();
            DundasMap.Legends.Add(legend);

            // добавляем поля
            DundasMap.ShapeFields.Clear();
            DundasMap.ShapeFields.Add("Name");
            DundasMap.ShapeFields["Name"].Type = typeof(string);
            DundasMap.ShapeFields["Name"].UniqueIdentifier = true;
            DundasMap.ShapeFields.Add("Complete");
            DundasMap.ShapeFields["Complete"].Type = typeof(double);
            DundasMap.ShapeFields["Complete"].UniqueIdentifier = false;
            DundasMap.ShapeFields.Add("CompletePercent");
            DundasMap.ShapeFields["CompletePercent"].Type = typeof(double);
            DundasMap.ShapeFields["CompletePercent"].UniqueIdentifier = false;

            // добавляем правила раскраски
            DundasMap.ShapeRules.Clear();
            ShapeRule rule = new ShapeRule();
            rule.Name = "CompleteRule";
            rule.Category = String.Empty;
            rule.ShapeField = "CompletePercent";
            rule.DataGrouping = DataGrouping.EqualDistribution;
            rule.ColorCount = 7;
            rule.ColoringMode = ColoringMode.ColorRange;
            rule.FromColor = Color.Red;
            rule.MiddleColor = Color.Yellow;
            rule.ToColor = Color.Green;
            rule.BorderColor = Color.FromArgb(50, Color.Black);
            rule.GradientType = GradientType.None;
            rule.HatchStyle = MapHatchStyle.None;
            rule.ShowInColorSwatch = false;
            rule.ShowInLegend = "CompleteLegend";
            rule.LegendText = "#FROMVALUE{### ##0.0##}% - #TOVALUE{### ##0.0##}%";
            DundasMap.ShapeRules.Add(rule);
        }
        #region Обработчики карты

        /// <summary>
        /// Поиск формы карты
        /// </summary>
        /// <param name="map">карта</param>
        /// <param name="patternValue">искомое имя формы</param>
        /// <param name="searchFO">true, если ищем ФО</param>
        /// <returns>найденная форма</returns>
        public static Shape FindMapShape(MapControl map, string patternValue, bool searchFO)
        {
            string subject = string.Empty;
            bool isRepublic = patternValue.Contains("Республика");
            bool isTown = patternValue.Contains("г.");
            string[] subjects = patternValue.Split(' ');
            if (subjects.Length > 1)
            {
                // пока такой глупый способ сопоставления имен субъектов
                switch (subjects[0])
                {
                    case "Чеченская":
                        {
                            subject = "Чечня";
                            break;
                        }
                    case "Карачаево-Черкесская":
                        {
                            subject = "Карачаево-Черкессия";
                            break;
                        }
                    case "Кабардино-Балкарская":
                        {
                            subject = "Кабардино-Балкария";
                            break;
                        }
                    case "Удмуртская":
                        {
                            subject = "Удмуртия";
                            break;
                        }
                    case "Чувашская":
                        {
                            subject = "Чувашия";
                            break;
                        }
                    default:
                        {
                            subject = (isRepublic || isTown) ? subjects[1] : subjects[0];
                            break;
                        }
                }
            }

            ArrayList shapeList = map.Shapes.Find(subject, true, false);
            if (shapeList.Count > 0)
            {
                return (Shape)shapeList[0];
            }
            return null;
        }
        string GetShortName(string s)
        {
            return s.Replace(" обл.", "").Replace(" область", "").Replace(" автономный округ", "").Replace(" АО", "");
        }
        public void FillMapData()
        {
            //if (dtGrid == null || DundasMap == null)
            //{
            //    return;
            //}

            //foreach (DataRow row in dtGrid.Rows)
            //{
            //    // заполняем карту данными
            //    if (row[0] != DBNull.Value && row[0].ToString() != string.Empty &&
            //        row[2] != DBNull.Value && row[2].ToString() != string.Empty &&
            //        row[4] != DBNull.Value && row[4].ToString() != string.Empty)
            //    {
            //        string subject = row[0].ToString();
            //        if (AllFO && RegionsNamingHelper.IsFO(subject) || !AllFO && RegionsNamingHelper.IsSubject(subject))
            //        {
            //            Shape shape = FindMapShape(DundasMap, subject, AllFO);
            //            if (shape != null)
            //            {
            //                shape["Name"] = subject;
            //                shape["Complete"] = Convert.ToDouble(row[2]);
            //                shape["CompletePercent"] = 100 * Convert.ToDouble(row[4]);
            //                shape.ToolTip = "#NAME #COMPLETEPERCENT{N2}%";
            //                shape.TextVisibility = TextVisibility.Shown;
            //            }
            //        }
            //    }
            //}
            int CodeColumn = G2.Columns.Count-2;

            for (int j = 0; j < DundasMap.Shapes.Count; j++)
            
            {
                bool ShapeIsEmpty = 1 == 1;
                for (int i = 0; i < G2.Rows.Count; i++)    
                {
                    try
                    { 
                        //String.Format(G2.Rows[i].Cells[CodeColumn].Value.ToString() + "|||" + DundasMap.Shapes[j]["CODE"].ToString());

                        Shape s = DundasMap.Shapes[j];
                        string ss = s.Name;
                        //String.Format(GetShortName(G2.Rows[i].Cells[0].Value.ToString()) +"|"+ GetShortName(ss));
                        //String.Format(ss);
                        if (GetShortName(G2.Rows[i].Cells[0].Value.ToString()) == GetShortName(ss))

                        //if (G2.Rows[i].Cells[CodeColumn].Value.ToString() == DundasMap.Shapes[j]["CODE"].ToString())
                        {
                            //String.Format("Cool!");
                            Shape shape = DundasMap.Shapes[j];
                            shape.TextVisibility = TextVisibility.Shown;
                            shape["Name"] = G2.Rows[i].Cells[0].Text;
                            shape["Complete"] = Double.Parse(G2.Rows[i].Cells[CodeColumn + 1].Text.Split('(')[0]);
                            shape["CompletePercent"] = Double.Parse(G2.Rows[i].Cells[CodeColumn + 1].Text.Split('(')[0]);
                            shape.ToolTip = string.Format("{0}\n{1}%\nРанг по ФО:{2}", G2.Rows[i].Cells[0].Text, G2.Rows[i].Cells[CodeColumn + 1].Text.Split('(')[0], G2.Rows[i].Cells[CodeColumn + 1].Text.Split('(', ')')[1]);
                            shape.Text = string.Format("{0}\n{1}%({2})", G2.Rows[i].Cells[0].Text, G2.Rows[i].Cells[CodeColumn + 1].Text.Split('(')[0], G2.Rows[i].Cells[CodeColumn + 1].Text.Split('(', ')')[1]);

                            ShapeIsEmpty = 1 == 2;
                        }
                    }
                    catch { }
                }
                if (ShapeIsEmpty)
                {
                    try
                    {
                        DundasMap.Shapes[j].Text = DundasMap.Shapes[j]["CODE"].ToString();
                    }
                    catch { }
                }
                
            }



        }

        #endregion

        protected void G_DblClick(object sender, Infragistics.WebUI.UltraWebGrid.ClickEventArgs e)
        {
            //
        }
        #endregion

        protected void G2_DblClick(object sender, Infragistics.WebUI.UltraWebGrid.ClickEventArgs e)
        {
            //
        }





    }
}

