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

namespace Krista.FM.Server.Dashboards.REGION_0010.UnEffectLessHealthAdd_Novosib
{
    public partial class _default : CustomReportPage
    {
        public DataTable GetDSForChart(string sql)  
        {
            DataTable dt = new DataTable();
            string s = DataProvider.GetQueryText(sql);
            DataProvidersFactory.PrimaryMASDataProvider.PopulateDataTableForChart(DataProvidersFactory.PrimaryMASDataProvider.GetCellset(s), dt, "ОЛОЛО");
            return dt;
        }

        CustomParam SelectItemGrid;
        CustomParam Chn;
        CustomParam Ks;
        CustomParam P3;
        CustomParam region;
        CustomParam P4;
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
        int LenAdd = 0,HeiAdd = 0;



        protected override void Page_PreLoad(object sender, System.EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            System.Web.HttpBrowserCapabilities myBrowserCaps = Request.Browser;
            BN = ((System.Web.Configuration.HttpCapabilitiesBase)myBrowserCaps).Browser.ToUpper();

            int Hadd = 0;
            if (BN == "FIREFOX")
            {
                Hadd -= 20;
                //Grid.Height = colRows * 28;
                Coef = 0.908;
                HeiAdd = -30;
                LenAdd = -5;
            }
            if (BN == "IE")
            {
                Hadd += 40;
                //Grid.Height = colRows * 24;
                Coef = 0.95;
                LenAdd = -5;
            }
            if (BN == "APPLEMAC-SAFARI")
            {
                //Grid.Height = colRows * 24;
                Coef = 0.95;
            }




            SelectItemGrid = UserParams.CustomParam("Param");
            Chn = UserParams.CustomParam("Chn");
            Ks = UserParams.CustomParam("Ks");
            P3 = UserParams.CustomParam("p3");
            P4 = UserParams.CustomParam("p4");
            region = UserParams.CustomParam("region");
            region.Value = RegionSettingsHelper.Instance.RegionBaseDimension;
            
            G.Width = 1240 - 10+LenAdd;
            //G.Height = CRHelper.GetGridHeight(350 * (1.0 / Coef) + HeiAdd);
//GC, 
            G.Height = Unit.Empty;


            G2.Width = 1240 - 10 + LenAdd;
            //G2.Height = CRHelper.GetGridHeight(670 * (1.0 / Coef)+Hadd+HeiAdd);
            G2.Height = Unit.Empty;


            //G3.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 10);
            //G3.Height = 350;
            

            //C.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth * (2.0 / 3.0) - 3);
            //UltraGauge1.Height = 395;
            //C.Height = 400;

            C1.Width = CRHelper.GetChartWidth((1240 * 1 - 25));
            //C2.Width = CRHelper.GetChartWidth(Coef* CustomReportConst.minScreenWidth * (1.0 / 2.0) - 3 );



            //UltraGauge1.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth * (1.0 / 3.0) - 3);

            //WebAsyncRefreshPanel1.AddLinkedRequestTrigger(G);

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
            Chn.Value = dt.Rows[0][1].ToString().Replace(',','.');
            //Ks.Value = dt.Rows[0][1].ToString();
            //Yc.Value = dt.Rows[1][1].ToString();

        }


        protected override void Page_Load(object sender, EventArgs e)
        {
            //RegionSettingsHelper.Instance.SetWorkingRegion(RegionSettingsHelper.Instance.GetPropertyValue("setRegion"));
            base.Page_Load(sender, e);


            if (!Page.IsPostBack)
            {
                C1.Data.SwapRowsAndColumns = !(C1.Data.SwapRowsAndColumns);
                //C2.Data.SwapRowsAndColumns = !(C2.Data.SwapRowsAndColumns);
            }
            Panel1.Visible = CheckBox1.Checked;
            //Panel2.Visible = 1==2;
            GetOtherCubValue();
            G.DataBind();
            try
            { }
            catch { }
            G2.DataBind();
            if (G2.Columns.Count > 3)
            {
                G2.Columns[1].Hidden = 1 == 1;
            }

            


            C1.DataBind();

            ColumnFormula();

            ColumnFormula2(G2,columnFormul2);

            Hederglobal.Text = string.Format("Оценка дополнительных неэффективных расходов в сфере здравоохранения ({0})",
                RegionSettings.Instance.Name);
                //UserComboBox.getLastBlock(region.Value));
            Page.Title = Hederglobal.Text;
            Label5.Text = RegionSettingsHelper.Instance.GetPropertyValue("RegionSubTitle");

            G.Rows[0].Activate();
            G.Rows[0].Activated = 1 == 1;
            G.Rows[0].Selected = 1 == 1;

            G2.Rows[0].Activate();
            G2.Rows[0].Activated = 1 == 1;
            G2.Rows[0].Selected = 1 == 1;            
        }
        string[] columnFormul1 = {
            "Р4 = (Р14хЧн)/10000", 
            "P14 = (Кф - Кц х Кс) х Ск х Нц", 
            "Р5 = (Р15хЧн)/10000", 
            "P15 = (Hц - Hф) х Кф х Ск", 
            "Р6 = (Р16хЧн)/100", 
            "Р16 = (Уф - Уц) х Скд х Сдлф"};
        string[] columnFormul2 = {
            "Р4 = (Р14хЧн)/10000", 
            "Чн", 
            "P14 = (Кф - Кц х Кс) х Ск х Нц", 
            "Кф", 
            "Кц",
            "Ск",
            "Кс",            
            "Нц",
            "Р5 = (Р15хЧн)/10000",
            "Чн",
            "P15 = (Hц - Hф) х Кф х Ск", 
            "Нц", 
            "Нф", 
            "Ск",
            "Кф",
            "Р6 = (Р16хЧн)/100",
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
                    G.Rows[4 * 3].Cells[i].Style.BackgroundImage = System.Double.Parse(G.Rows[4 * 3].Cells[i].Text) > 0 ? "~/images/cornerRed.gif" : "~/images/cornerGreen.gif";
                    G.Rows[4 * 3].Cells[i].Title = System.Decimal.Parse(G.Rows[4 * 3].Cells[i].Text) > 0 ? "Имеются неэффективные расходы" : "Неэффективные расходы отсутствуют";
                     
                    G.Rows[2 * 3].Cells[i].Style.CustomRules = "background-repeat: no-repeat; background-position: right top; margin: 2px";
                    G.Rows[2 * 3].Cells[i].Style.BackgroundImage = System.Double.Parse(G.Rows[2 * 3].Cells[i].Text) > 0 ? "~/images/cornerRed.gif" : "~/images/cornerGreen.gif";
                    G.Rows[2 * 3].Cells[i].Title = System.Decimal.Parse(G.Rows[2 * 3].Cells[i].Text) > 0 ? "Имеются неэффективные расходы" : "Неэффективные расходы отсутствуют";

                    G.Rows[0 * 3].Cells[i].Style.CustomRules = "background-repeat: no-repeat; background-position: right top; margin: 2px";
                    G.Rows[0 * 3].Cells[i].Style.BackgroundImage = System.Double.Parse(G.Rows[0 * 3].Cells[i].Text) > 0 ? "~/images/cornerRed.gif" : "~/images/cornerGreen.gif";
                    G.Rows[0 * 3].Cells[i].Title = System.Decimal.Parse(G.Rows[0 * 3].Cells[i].Text) > 0 ? "Имеются неэффективные расходы" : "Неэффективные расходы отсутствуют";
                }
                catch { }
                //TODO: Доделать если ноль
            }
            int lastColumn = G.Columns.Count;
            G.Columns.Add("Формула");

            G.Rows[0 * 3].Cells[lastColumn].RowSpan = 3;
            G.Rows[0*3].Cells[lastColumn].Text = columnFormul1[0];
            G.Rows[0 * 3].Cells[0].Style.Font.Bold = 1==1;
                

            //G.Rows[1 * 3].Cells[lastColumn].RowSpan = 3;
            G.Rows[1 * 3].Cells[lastColumn].Text = columnFormul1[1];
            //G.Rows[1 * 3].Cells[0].Style.Font.Bold = 1 == 1;
            G.Rows[1 * 3].Cells[0].Style.Padding.Left = 15;
            G.Rows[1 * 3 + 1].Hidden = 1 == 1;
            G.Rows[1 * 3 + 2].Hidden = 1 == 1;
            
            G.Rows[2 * 3].Cells[lastColumn].RowSpan = 3;
            G.Rows[2 * 3].Cells[lastColumn].Text = columnFormul1[2];
            G.Rows[2 * 3].Cells[0].Style.Font.Bold = 1 == 1;
//string.Format(
//@"Добр об= Робр / Р общ х 100% <br><br>
//
//Показатель рассчитывается путем деления 
//объема неэффективных расходов в сфере общего образования на общий объем расходов консолидированного бюджета и умножения на 100%
//");
            //G.Rows[3 * 3].Cells[lastColumn].RowSpan = 3;
            G.Rows[3 * 3].Cells[lastColumn].Text =columnFormul1[3];
            //G.Rows[3 * 3].Cells[0].Style.Font.Bold = 1 == 1;
            G.Rows[3 * 3].Cells[0].Style.Padding.Left = 15;
            G.Rows[3 * 3 + 1].Hidden = 1 == 1;
            G.Rows[3 * 3 + 2].Hidden = 1 == 1;
//string.Format(
//@"Добр= Робр / Робр общ х 100%<br><br>
//
//Показатель рассчитывается путем деления 
//объема неэффективных расходов в сфере общего образования  на расходы консолидированного бюджета в сфере общего образования и  умножения на 100 %
//");
            G.Rows[4 * 3].Cells[lastColumn].RowSpan = 3;
            G.Rows[4 * 3].Cells[lastColumn].Text = columnFormul1[4];
            G.Rows[4 * 3].Cells[0].Style.Font.Bold = 1 == 1;

            //G.Rows[5 * 3].Cells[lastColumn].RowSpan = 3;
            G.Rows[5 * 3].Cells[lastColumn].Text = columnFormul1[5];
            //G.Rows[5 * 3].Cells[0].Style.Font.Bold = 1 == 1;
            G.Rows[5 * 3].Cells[0].Style.Padding.Left = 15;
            G.Rows[5 * 3 + 1].Hidden = 1 == 1;
            G.Rows[5 * 3 + 2].Hidden = 1 == 1;
            //G.Rows[6 * 3].Cells[lastColumn].RowSpan = 3;
            //G.Rows[6 * 3].Cells[lastColumn].Text = columnFormul1[6];
            //G.Rows[7 * 3].Cells[lastColumn].RowSpan = 3;
            //G.Rows[7 * 3].Cells[lastColumn].Text = columnFormul1[7];
//string.Format(
//@"Робр= О1 + О2 <br><br>
//
//Показатель рассчитывается путем суммирования объема неэффективных расходов на управление кадровыми ресурсами (О1)>>> и объема неэффективных расходов на управление наполняемостью классов в общеобразовательных учреждениях (O2)>>>
//");
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
            for (int i = 0; i < dtNew.Rows.Count/3; i++)
            {
                dtNew.Rows[i*3][0] = "<b>"+dt.Rows[i][0]+"</b>";
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
                        
                        //string[] TreeCell =CalcCell().Split(';');

                        dtNew.Rows[i * 3 + 2][j] = CalcCell();
                        if (dtNew.Rows[i * 3 + 2][j].ToString() != "-")
                        {
                            dtNew.Rows[i * 3 + 1][j] = ((System.Double)(down - up)).ToString("### ### ##0.00");
                        }
                        else
                        {
                            dtNew.Rows[i * 3 + 1][j] = "-";
                        }
                        
                    }
                    catch 
                    {
                        dtNew.Rows[i * 3 + 1][j] = "-"; 
                        dtNew.Rows[i * 3 + 2][j] = "-";
                    }
                }
            }

            G.DataSource = dtNew;
        }

        int ld;
        protected void G_InitializeLayout(object sender, LayoutEventArgs e)
        {
            int BigPig = 0;
            System.Double coef2 = 1;
            if ((UltraWebGrid)(sender) != G)
            {
                BigPig -= 60;
            }
            else
            {
                BigPig += -60;

            }
            if (BN == "FIREFOX")
            {
                coef2 = 1.05;
            }


            for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i++)
            {
                e.Layout.Bands[0].Columns[i].Header.Style.Wrap = 1 == 1;
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "### ### ##0.00");
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth((CustomReportConst.minScreenWidth / 6 + BigPig) * Coef);
            }
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = 1 == 1;
            e.Layout.Bands[0].Columns[0].Header.Caption = "Показатель";
            if ((UltraWebGrid)(sender) == G)
            {
                if (G.Columns.Count > 3)
                { e.Layout.Bands[0].Columns[1].Hidden = 1 == 1; }
            }


               e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth((520 + BigPig) * Coef * coef2);
               for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i++)
               {

                   //.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth((200 + BigPig) * Coef * coef2);
                   e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(((CustomReportConst.minScreenWidth - 520 - BigPig - 260) / (e.Layout.Bands[0].Columns.Count - 1) + BigPig));
               }
            e.Layout.AllowSortingDefault = AllowSorting.No;

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
                    if ((UltraWebGrid)(sender)!=G)
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
            SetCellFormat(e, 0, 0, 1000, -1, HorizontalCellAlignment.Left, VerticalCellAlignment.Center, new Font("Arial", 1, FontStyle.Bold), 280);
            e.CurrentWorksheet.MergedCellsRegions.Add(0, 0, 0, 3);

            e.CurrentWorksheet.Rows[1].Cells[0].Value = Label5.Text;
            SetCellFormat(e, 0, 1, 1000, -1, HorizontalCellAlignment.Left, VerticalCellAlignment.Center, new Font("Arial", 11), 240);
            e.CurrentWorksheet.MergedCellsRegions.Add(1, 0, 1, 3);

            e.CurrentWorksheet.Rows[2].Cells[0].Value = Label1.Text;
            SetCellFormat(e, 0, 2, 1000, -1, HorizontalCellAlignment.Left, VerticalCellAlignment.Center, new Font("Arial", 1, FontStyle.Bold), 260);
            e.CurrentWorksheet.MergedCellsRegions.Add(2, 0, 2, 3);

            DataTable dt = GetDSForChart("G");
            //dt.Columns.Remove(dt.Columns[1]);
            int offset = Export_DT(dt, 3, e, 1 == 1);
            e.CurrentWorksheet.Rows[3].Height = 350;

            e.CurrentWorksheet.Rows[4].Cells[0].CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            
            //e.CurrentWorksheet.Rows[7].Cells[0].Value =  "      "+e.CurrentWorksheet.Rows[7].Cells[0].Value.ToString();
            e.CurrentWorksheet.Rows[7].Cells[0].CellFormat.Indent = 4;



            e.CurrentWorksheet.Rows[8].Hidden = 1 == 1;
            e.CurrentWorksheet.Rows[9].Hidden = 1 == 1;


            e.CurrentWorksheet.Rows[10].Cells[0].CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            //e.CurrentWorksheet.Rows[13].Cells[0].Value = "      " + e.CurrentWorksheet.Rows[7].Cells[0].Value.ToString();
            e.CurrentWorksheet.Rows[13].Cells[0].CellFormat.Indent = 4;


            e.CurrentWorksheet.Rows[14].Hidden = 1 == 1;
            e.CurrentWorksheet.Rows[15].Hidden = 1 == 1;


            e.CurrentWorksheet.Rows[16].Cells[0].CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            //e.CurrentWorksheet.Rows[19].Cells[0].Value = "      " + e.CurrentWorksheet.Rows[7].Cells[0].Value.ToString();
            e.CurrentWorksheet.Rows[19].Cells[0].CellFormat.Indent = 4;


            e.CurrentWorksheet.Rows[20].Hidden = 1 == 1;
            e.CurrentWorksheet.Rows[21].Hidden = 1 == 1;
            
            e.CurrentWorksheet.Columns[0].Width = 24000;
            if (CheckBox1.Checked)
            {
                e.Workbook.Worksheets.Add("Расчет показателей");
                e.CurrentWorksheet = e.Workbook.Worksheets["Расчет показателей"];
                e.CurrentWorksheet.Rows[0].Cells[0].Value = Label3.Text; 
                SetCellFormat(e, 0, 0, 1000, -1, HorizontalCellAlignment.Left, VerticalCellAlignment.Center, new Font("Arial", 1, FontStyle.Bold), 260);
                e.CurrentWorksheet.MergedCellsRegions.Add(0, 0, 0, 3);

                //GlobalDT2.Columns.Remove(GlobalDT2.Columns[GlobalDT2.Columns.Count - 2]);
                dt = GetDSForChart("G2");
                //dt.Columns.Remove(dt.Columns[1]);
                offset = Export_DT(dt, 1, e, 1 == 2);                

                e.CurrentWorksheet.Rows[3].Cells[0].CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
                e.CurrentWorksheet.Rows[5].Cells[0].CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
                e.CurrentWorksheet.Rows[11].Cells[0].CellFormat.Font.Bold = ExcelDefaultableBoolean.True;

                e.CurrentWorksheet.Rows[13].Cells[0].CellFormat.Font.Bold = ExcelDefaultableBoolean.True; 
                e.CurrentWorksheet.Rows[18].Cells[0].CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
                e.CurrentWorksheet.Rows[20].Cells[0].CellFormat.Font.Bold = ExcelDefaultableBoolean.True;

                e.CurrentWorksheet.Rows[1].Height = 350;
            }
            e.CurrentWorksheet.Columns[0].Width = 24000;
        }

        int Export_DT(DataTable dt, int offset, EndExportEventArgs e, bool CalcTemp)
        {
            int xz = CalcTemp ? 3 : 1;
            e.CurrentWorksheet.Rows[offset].Height = 350;
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
                    //e.CurrentWorksheet.Rows[offset].Height = !CalcTemp ? 1500 : 700;

                    for (int j = 0; j < dt.Rows.Count * xz; j += xz)
                    {
                        e.CurrentWorksheet.Rows[j + offset + 1].Height = 1000;
                        if (dt.Rows[j / xz][i].ToString()[0] == '_')
                        {
                            dt.Rows[j / xz][i] = "  " + dt.Rows[j / xz][i].ToString().Remove(0, 1);
                        }

                        try
                        {
                            dt.Rows[j / xz][i] = dt.Rows[j / xz][i].ToString().Split('_')[1];
                        }
                        catch { }
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
                            else                            
                            {
                                if (((j + offset + 1) != 3) & ((j + offset + 1) != 5) & ((j + offset + 1) != 11) & ((j + offset + 1) != 13) & ((j + offset + 1) != 18) & ((j + offset + 1) != 20))
                                {
                                   // e.CurrentWorksheet.Rows[j + offset + 1].Cells[i].Value = "     " + e.CurrentWorksheet.Rows[j + offset + 1].Cells[i].Value;
                                    e.CurrentWorksheet.Rows[j + offset + 1].Cells[i].CellFormat.Indent = 4;
                                }
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
            dt.Rows[0][0] = @"Объем неэффективных расходов вследствие излишнего
 количества больничных коек в государственных (муниципальных) 
учреждениях здравоохранения";
            dt.Rows[1][0] = @"Объем неэффективных расходов, связанных
 с недостаточной занятостью больничных коек";
            dt.Rows[2][0] = @"Объем неэффективных расходов на стационарную медицинскую
 помощь вследствие высокого уровня госпитализации";
            System.Decimal max = 0;
            for (int j = 0; j < dt.Rows.Count; j++)
            for (int i = 1; i < dt.Columns.Count; i++)
            {
                if (max < (System.Decimal)(dt.Rows[j][i]))
                {
                    max = (System.Decimal)(dt.Rows[j][i]);
                }
                if (max < (System.Decimal)(dt.Rows[j][i]))
                {
                    max = (System.Decimal)(dt.Rows[j][i]);
                } 
            }
            C1.Axis.Y.RangeType = Infragistics.UltraChart.Shared.Styles.AxisRangeType.Custom;
            C1.Axis.Y.RangeMax =  (System.Double)max; 
            C1.Axis.Y.RangeMin = 0; 

            C1.Axis.X.Labels.Font = new Font("Arial", 10);
            C1.Axis.X.Labels.SeriesLabels.Font = new Font("Arial", 10);
            C1.Axis.X.Extent = 20;
            //C1.Legend.Font = new Font("Arial", 8);

            C1.DataSource = dt;
        }

        protected void C2_DataBinding(object sender, EventArgs e)
        {
                        
        }

        protected void UltraWebGrid1_DataBinding(object sender, EventArgs e)
        {

        }

        protected void G2_DataBinding(object sender, EventArgs e)
        {
            G2.Rows.Clear();
            G2.Columns.Clear();
            G2.Bands.Clear();
            G2.DataSource = GetDSForChart("G2");
        }

        protected void G3_DataBinding(object sender, EventArgs e)
        {
            
        }

        protected void G_DblClick(object sender, ClickEventArgs e)
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


            int textWidth = 1000;
            int textHeight = 15;
                try
                {
                    Infragistics.UltraChart.Shared.Styles.LabelStyle f = new Infragistics.UltraChart.Shared.Styles.LabelStyle();
                    f.Font = new Font("Arial", 9);

                    textLabel = new Infragistics.UltraChart.Core.Primitives.Text();
                    textLabel.labelStyle = f;
                    
                    textLabel.PE.Fill = Color.Black;
                    textLabel.bounds = new Rectangle(25, 243-21-3-5, textWidth, textHeight);
                    textLabel.SetTextString("Объем неэффективных расходов вследствие излишнего количества больничных коек в государственных (муниципальных) учреждениях здравоохранения");
                    e.SceneGraph.Add(textLabel);
                    textLabel = new Infragistics.UltraChart.Core.Primitives.Text();
                    textLabel.labelStyle = f;
                    textLabel.PE.Fill = Color.Black;
                    textLabel.bounds = new Rectangle(25, 243 + 22 - 21 - 3-5, textWidth, textHeight);
                    textLabel.SetTextString("Объем неэффективных расходов, связанных с недостаточной занятостью больничных коек");                    
                    e.SceneGraph.Add(textLabel);
                    textLabel = new Infragistics.UltraChart.Core.Primitives.Text();
                    textLabel.labelStyle = f;
                    textLabel.PE.Fill = Color.Black;
                    textLabel.bounds = new Rectangle(25, 243 + 22 * 2 - 21 - 3-5, textWidth, textHeight);
                    textLabel.SetTextString("Объем неэффективных расходов на стационарную медицинскую помощь вследствие высокого уровня госпитализации");                    
                    e.SceneGraph.Add(textLabel);
                }
                catch 
                {
                    
                }
            
        }




        //#########################################################






    }
}
