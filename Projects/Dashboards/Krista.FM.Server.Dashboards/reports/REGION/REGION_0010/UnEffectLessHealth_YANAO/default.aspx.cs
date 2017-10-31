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

namespace Krista.FM.Server.Dashboards.REGION_0010.UnEffectLessHealth_YANAO
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

        CustomParam Ks;
        CustomParam Chsrv;
        CustomParam Chssr;
        CustomParam Chn;
        CustomParam Year;
        CustomParam AddYear;
        CustomParam region;
        string BN= "IE";

        protected override void Page_PreLoad(object sender, System.EventArgs e)
        {
            base.Page_PreLoad(sender, e);
            SelectItemGrid = UserParams.CustomParam("Param");
            Ks = UserParams.CustomParam("Ks");
            Chsrv = UserParams.CustomParam("Chsrv");
            Chssr = UserParams.CustomParam("Chssr");
            Chn = UserParams.CustomParam("Chn");
            AddYear = UserParams.CustomParam("addYear");
            //region = UserParams.CustomParam("region");

            System.Web.HttpBrowserCapabilities myBrowserCaps = Request.Browser;
            BN = ((System.Web.Configuration.HttpCapabilitiesBase)myBrowserCaps).Browser.ToUpper();
            Double Coef;
            if (BN == "FIREFOX")
            {             
                Coef = 0.908;
            }
            if (BN == "IE")
            {             
                Coef = 0.96;
            }
            if (BN == "APPLEMAC-SAFARI")
            {                
                Coef = 0.95;
            }

            G.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 10);            
            G.Height = Unit.Empty;


            G2.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 10);            
            G2.Height = Unit.Empty;


            G3.Width = CRHelper.GetGridWidth( CustomReportConst.minScreenWidth - 10);
            G3.Height = Unit.Empty;


            G4.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 10);
            G4.Height = Unit.Empty;

            C1.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth * (1.0 / 2.0) - 10);
            C2.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth * (1.0 / 2.0) - 10);


            UltraGridExporter1.PdfExportButton.Visible = 1 == 2;
            //############################################################################
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
            //Ks.Value = dt.Rows[0][1].ToString();
            //Yc.Value = dt.Rows[1][1].ToString();
        }

        protected override void Page_Load(object sender, EventArgs e)
        {            
            base.Page_Load(sender, e);


            if (!Page.IsPostBack)
            {
                C1.Data.SwapRowsAndColumns = !(C1.Data.SwapRowsAndColumns);
                region = UserParams.CustomParam("region");
                region.Value = RegionSettingsHelper.Instance.RegionBaseDimension;
            }
            
            G.DataBind();
            G.Rows[0].Cells[G.Columns.Count - 1].Value = ColumnFormula[0];
            G.Rows[1].Cells[G.Columns.Count - 1].Value = ColumnFormula[1];
            G.Rows[3].Cells[G.Columns.Count - 1].Value = ColumnFormula[1];
            
            Panel1.Visible = CheckBox1.Checked;
            Panel2.Visible = CheckBox1.Checked;
            Panel3.Visible = CheckBox1.Checked;
            G2.DataBind();
            
            G3.DataBind();
            G4.DataBind();

            C1.DataBind();
            C2.DataBind();
            
            Hederglobal.Text = string.Format(Hederglobal.Text, UserComboBox.getLastBlock(RegionSettingsHelper.Instance.RegionBaseDimension));
            
            Page.Title = Hederglobal.Text;

            _ColumnFormula2(G, ColumnFormula2);

            _ColumnFormula2(G2,ColumnFormula2);
            _ColumnFormula2(G3, ColumnFormula3);
            _ColumnFormula2(G4, ColumnFormula4);
            for (int i = 0; i < G.Rows.Count; i+=3)
            {
                G.Rows[i].Cells[1].RowSpan = ((i!=0)&(i!=3))?
                    ((i == 12) || (i == 6) ? 2 : 3)//(i == 9)||
                    :
                    1;
            }
            

            for (int i = 2; i < G.Columns.Count; i++)
            {
                try
                {
                    G.Rows[3 * 3].Cells[i].Style.CustomRules = "background-repeat: no-repeat; background-position: right top; margin: 2px";
                    G.Rows[3 * 3].Cells[i].Style.BackgroundImage = "~/images/cornerRed.gif";
                    G.Rows[3 * 3].Cells[i].Title = "Имеются неэффективные расходы";
                }
                catch { }
                //TODO: Доделать если ноль
            }
            G.Rows[1].Hidden = 1 == 1;
            G.Rows[2].Hidden = 1 == 1;
            G.Rows[4].Hidden = 1 == 1;
            G.Rows[5].Hidden = 1 == 1;
            G.Rows[8].Hidden = 1 == 1; 
            //G.Rows[11].Hidden = 1 == 1;
            G.Rows[14].Hidden = 1 == 1; 
            
            ActiveRow(G, 0);
            ActiveRow(G2, 5);
            ActiveRow(G3, 24);
            ActiveRow(G4, 31);
            Label6.Text =
                //RegionSettingsHelper.Instance.GetPropertyValue("RegionSubTitle");
                //"Доклад сформирован в соответствии с формой, утвержденной в постановлении Правительства Российской Федерации от 15 апреля 2009 г. № 322 «О мерах по реализации Указа Президента Российской Федерации от 28 июня 2007 г. № 825» (с учетом изменений)";
                "Оценка проведена в соответствии с постановлением Правительства Российской Федерации от 15 апреля 2009 г. № 322 «О мерах по реализации Указа Президента Российской Федерации от 28 июня 2007 г. № 825» (с учетом изменений)";
            G.Columns.Add("№","№");
            G.Columns.FromKey("№").Move(0);
            for (int i = 0; i < G.Rows.Count; i += 3)
            {
                G.Rows[i].Cells[0].Text = ((i/3)+1).ToString();
            }
            G.Columns[0].Width = 20;
            G.Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Center;
            
            SetNumberRow(G2,1);
            SetNumberRow(G3, 2);
            SetNumberRow(G4, 3);


        }

        private void SetNumberRow(UltraWebGrid G, int p)
        {

            int parentIndex = 0;
            int childrentIndex = 0;

            G.Columns.Add("№", "№");
            G.Columns.FromKey("№").Move(0);
            G.Columns[0].Width = 40;
            G.Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Center;

            foreach (UltraGridRow row in G.Rows)
            {
                if (!row.Hidden)
                {
                    
                    childrentIndex++;

                    if (row.Cells[1].Style.Padding.Left.Value > 10)
                    {
                        row.Cells[0].Text = string.Format("{0}.{1}.{2}", p, parentIndex, childrentIndex);
                    }
                    else
                    {
                        parentIndex++; 
                        row.Cells[0].Text = string.Format("{0}.{1}", p, parentIndex);
                        childrentIndex = 0;
                        
                    }
                    if (row.Cells[0].Text == "1.3.2")
                    {
                        row.Cells[1].Text = "Целевое значение численности врачей (физических лиц) государственных (муниципальных) учреждений здравоохранения на 10 тыс. человек населения, человек";
                    }
                    if (row.Cells[0].Text == "1.4.2")
                    {
                        row.Cells[1].Text = "Целевое значение численности среднего медицинского персонала (физических лиц) государственных (муниципальных) учреждений здравоохранения на 10 тыс. человек населения, человек";
                    }

                    if (row.Cells[0].Text == "1.5.2")
                    {
                        row.Cells[1].Text = "Целевое значение численности врачей (физических лиц) государственных (муниципальных) учреждений здравоохранения на 10 тыс. человек населения, человек";
                    }
                    if (row.Cells[0].Text == "1.5.3")
                    {
                        row.Cells[1].Text = "Целевое значение численности среднего медицинского персонала (физических лиц) государственных (муниципальных) учреждений здравоохранения на 10 тыс. человек населения, человек";
                    }
                }
                

            }
        }
        void ActiveRow(UltraWebGrid G,int indexRow)
        {
            G.Rows[indexRow].Selected = 1 == 1; 
            G.Rows[indexRow].Activated = 1==1;
            G.Rows[indexRow].Activate();
        }

        string[] ColumnFormula =
            {"Робщ",
                "Рздр общий",
                @"Дздр = Рздр / Рздр общий х 100%<br>
Показатель рассчитывается путем деления 
объема неэффективных расходов в сфере здравоохранения на объем  расходов субъекта РФ в сфере здравоохранения и умножения на 100%",
                @"Рздр = Р1 + Кнст х Р2 + Кнск х Р3<br>
Показатель представляет собой сумму объема неэффективных расходов на управление кадровыми ресурсами (P1); объема неэффективных расходов на управление объемами стационарной медицинской помощи (P2), умноженного на корректирующий коэффициент стоимости 1 койко-дня в гос. (муницип.) учреждениях и объема неэффективных расходов на управление объемами скорой медицинской помощи (P3), умноженного на корректирующий коэффициент стоимости вызова скорой медицинской помощи.",
                @"Дздр общ = Рздр / Р общ х 100%<br>
Показатель рассчитывается путем деления 
объема неэффективных расходов в сфере здравоохранения  на общий объем расходов консолидированного бюджета и умножения на 100%"
            };

        string[] ColumnFormula2 =
            {"Р1 = (Р11 х Чн)/10000",
                "Чн",
                @"P11 =Рвр + Рср + Рпр ",
                @"Рвр = (Чвр - Чцвр х Кс) х (ЗПрвр х (1+ СВ) х 12 мес.)",
                @"Чвр",
                @"Чцвр",
                @"ЗПрвр",
                @"Кс",
                @"Рср = (Чср - Чцср х Кс) х (ЗПрср х (1+ СВ) х 12 мес.)",
                @"Чср",
                @"Чцср",
                @"ЗПрср",
                @"Кс",
                @"Рпр = (Чпр - 0,4 x (Чцвр + Чцср) x Кс) x (ЗПрпр x (1+ ЕСН ) x 12 мес.)",
                @"Чпр",
                @"Чцвр",
                @"Чцср",
                @"ЗПрпр",
                @"Кс"
            };

        string[] ColumnFormula3 =
            {"P2 = (OCф - ОСн х Кс) х Скд х Чн",
                "ОСф",
                @"Скд",
                @"ОСн ",
                @"Кс",
                @"Чн",
                @"Кнст"
            };

        string[] ColumnFormula4 =
            {"P3 = (ОСПф - ОСПн х Кс) х Св х Чн",
                "ОСПф",
                @"Св",
                @"ОСПн",
                @"Кс",
                @"Кнск",
                @"Чн"
            };



        void _ColumnFormula2(UltraWebGrid G_, string[] columnFormul)
        {
            int lastColumn = G_.Columns.Count-1;

            G_.Columns[lastColumn].Width = 300;
            G_.Columns[lastColumn].CellStyle.Wrap = 1 == 1;
            G_.Columns[lastColumn].Move(1);

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
            if (OlOlO == 0)
            {
                return "-";
            }
            if (Double.IsNaN(OlOlO))
            {
                return "-";
            }

            res = image + OlOlO.ToString("### ### ##0.00") + "%";
            return res;
        }



        void GetOtherCubValue_()
        {
            DataTable dt = GetDSForChart("G0");
            Ks.Value = dt.Rows[0][1].ToString().Replace(',', '.');
            Chsrv.Value = dt.Rows[1][1].ToString().Replace(',', '.');
            Chssr.Value = dt.Rows[2][1].ToString().Replace(',', '.');
            Chn.Value = dt.Rows[3][1].ToString().Replace(',', '.');

        }
        DataTable GDt;

        DataTable GetDsForGrid()
        {  

            int ColCount = 2;
            AddYear.Value = (ColCount).ToString();            
            ColCount--;
            //GetOtherCubValue_();
            DataTable dt = GetDSForChart("G");
            try
            {
                for (; ColCount >= 0; ColCount--)
                {
                    AddYear.Value = (ColCount).ToString();
                    GetOtherCubValue_();
                    DataTable dt_ = GetDSForChart("G");
                    try
                    {
                        dt.Columns.Add(dt_.Columns[dt_.Columns.Count - 1].Caption, dt_.Columns[dt_.Columns.Count - 1].DataType);
                    }
                    catch {
                        dt.Columns.Add(dt_.Columns[dt_.Columns.Count - 1].Caption+" ", dt_.Columns[dt_.Columns.Count - 1].DataType);
                    }
                    for (int i = 0; i < dt_.Rows.Count; i++)
                    {
                        dt.Rows[i][dt.Columns.Count - 1] = dt_.Rows[i][1];
                    }
                }
            }
            catch (Exception e)
            {
                String.Format(e.StackTrace);
            }
            GDt = dt;
            return dt;
        }

        

        protected void G_DataBinding(object sender, EventArgs e)
        {
            G.Rows.Clear();
            G.Columns.Clear();
            G.Bands.Clear();

            DataTable dt = GetDsForGrid();//GetDSForChart("G");

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
                
                if (i > 1)
                {
                    dtNew.Rows[i * 3][0] = "<b>" + dt.Rows[i][0] + "</b>";
                }
                else
                {
                    dtNew.Rows[i * 3][0] = dt.Rows[i][0];
                };
                dtNew.Rows[i * 3 + 1][0] = "<div style=\"FLOAT: Right;\">абсолютные отклонения&nbsp&nbsp</div>";
                dtNew.Rows[i * 3 + 2][0] = "<div style=\"FLOAT: Right;\">темп прироста&nbsp&nbsp</div>";
                //абсолютные отклонения

                for (int j = dt.Columns.Count - 1; j >= 1; j--)
                {
                    try
                    { 
                        down = (System.Double)(System.Decimal)dt.Rows[i][j];
                        dtNew.Rows[i * 3][j] = down.ToString("### ### ##0.##");

                        up = (System.Double)(System.Decimal)dt.Rows[i][j - 1];

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
                        if ((down - up) != 0)
                        {
                            dtNew.Rows[i * 3 + 1][j] = ((System.Double)(down - up)).ToString("### ### ##0.##");
                        }
                        else
                        {
                            dtNew.Rows[i * 3 + 1][j] = "-";
                        }
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
            try
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
                System.Web.HttpBrowserCapabilities myBrowserCaps = Request.Browser;
                BN = ((System.Web.Configuration.HttpCapabilitiesBase)myBrowserCaps).Browser.ToUpper();
                if (BN == "FIREFOX")
                {

                    BigPig += -5;
                }
                if (BN == "IE")
                {

                    BigPig -= 10;
                }
                if (BN == "APPLEMAC-SAFARI")
                {

                }


                for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i++)
                {
                    e.Layout.Bands[0].Columns[i].Header.Style.Wrap = 1 == 1;
                    CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "### ### ##0.00");
                    e.Layout.Bands[0].Columns[i].Width = CustomReportConst.minScreenWidth / 6 + BigPig;
                }

                e.Layout.Bands[0].Columns[1].Hidden = 1 == 1;
                e.Layout.Bands[0].Columns[0].CellStyle.Wrap = 1 == 1;
                e.Layout.Bands[0].Columns[0].Header.Caption = "Показатель";
                if ((UltraWebGrid)(sender) == G) { e.Layout.Bands[0].Columns[1].Hidden = 1 == 1; }


                e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth((500 + BigPig));
                for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i++)
                {
                    e.Layout.Bands[0].Columns[i].Width =  CRHelper.GetColumnWidth(((CustomReportConst.minScreenWidth - 520 - BigPig-40) / (e.Layout.Bands[0].Columns.Count - 1) + BigPig));
                }

                e.Layout.AllowSortingDefault = AllowSorting.No;
                e.Layout.Bands[0].Columns.Add("Формула");
                e.Layout.Bands[0].Columns[e.Layout.Bands[0].Columns.Count - 1].Header.Caption = "Формула";
                
            }
            catch { }            
        }
        void RowDel_(RowEventArgs e)
        {
            e.Row.Cells[0].Text = e.Row.Cells[0].Text.Split('_')[e.Row.Cells[0].Text.Split('_').Length - 1];
        }


        protected void G_InitializeRow(object sender, RowEventArgs e)
        {
            try
            {
                
                if (e.Row.Index > 5 * 3 - 1)
                {
                    e.Row.Hidden = 1 == 1;
                }
                else
                {
                    if (e.Row.Cells[0].Text.Split('<', '>')[1] == "b")
                    {
                        e.Row.Cells[e.Row.Cells.Count - 1].Text = ColumnFormula[e.Row.Index / 3];
                        e.Row.Cells[0].Style.Font.Bold = 1 == 1;
                    }
                        
                }
            }
            catch { }
            try
            {
                RowDel_(e);
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
            for (int i = 0; i < G.Columns.Count; i++)
            {
                e.CurrentWorksheet.Columns[i].Width = 12000;
                for (int j = 0; j < G.Rows.Count; j++)
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
            e.CurrentWorksheet.Rows[0].Cells[0].Value = Hederglobal.Text;

            SetCellFormat(e, 0, 0, 1000, -1, HorizontalCellAlignment.Left, VerticalCellAlignment.Center, new Font("Arial", 1, FontStyle.Bold), 280);

            e.CurrentWorksheet.MergedCellsRegions.Add(0, 0, 0, 3);

            e.CurrentWorksheet.Rows[1].Cells[0].Value = Label6.Text;
            SetCellFormat(e, 0, 1, 1000 , -1, HorizontalCellAlignment.Left, VerticalCellAlignment.Center, new Font("Arial", 11), 240);
            e.CurrentWorksheet.MergedCellsRegions.Add(1, 0, 1, 3);            

            e.CurrentWorksheet.Rows[2].Cells[0].Value = Label1.Text;
            SetCellFormat(e, 0, 2, 1000, -1, HorizontalCellAlignment.Left, VerticalCellAlignment.Center, new Font("Arial", 1, FontStyle.Bold), 260);
            e.CurrentWorksheet.MergedCellsRegions.Add(2, 0, 2, 3);
            e.CurrentWorksheet.Rows[3].Height = 350;


            e.CurrentWorksheet.Name = "Неэффективные расходы";                     
            int offset = 3;
            RenameGDT(GDt);

            Export_DT(GDt, offset, e,0,5,1==1);

            e.CurrentWorksheet.Rows[5].Hidden = 1 == 1;
            e.CurrentWorksheet.Rows[6].Hidden = 1 == 1;

            e.CurrentWorksheet.Rows[8].Hidden = 1 == 1;
            e.CurrentWorksheet.Rows[9].Hidden = 1 == 1;

            e.CurrentWorksheet.Rows[12].Hidden = 1 == 1;
            e.CurrentWorksheet.Rows[18].Hidden = 1 == 1;
             
            e.CurrentWorksheet.Rows[10].Cells[0].CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            e.CurrentWorksheet.Rows[13].Cells[0].CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            e.CurrentWorksheet.Rows[16].Cells[0].CellFormat.Font.Bold = ExcelDefaultableBoolean.True;

            e.CurrentWorksheet.Rows[3].Height = 350;
            if (!CheckBox1.Checked) return;
            e.Workbook.Worksheets.Add("расчет показателей 1");
            e.CurrentWorksheet = e.Workbook.Worksheets["расчет показателей 1"];
            e.CurrentWorksheet.MergedCellsRegions.Add(0, 0, 0, 3);
            e.CurrentWorksheet.Rows[0].Cells[0].Value = Label3.Text;
            SetCellFormat(e, 0, 0, 1000, -1, HorizontalCellAlignment.Left, VerticalCellAlignment.Center, new Font("Arial", 1, FontStyle.Bold), 260);

            
            offset = 1;
            e.CurrentWorksheet.Rows[1].Height = 350;
            Export_DT(GDt, offset, e, 5, 24,1==2);
            e.CurrentWorksheet.Rows[1].Height = 350;
            
            e.CurrentWorksheet.Rows[6-2].Cells[0].CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            e.CurrentWorksheet.Rows[7-2].Cells[0].CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            e.CurrentWorksheet.Rows[12-2].Cells[0].CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            e.CurrentWorksheet.Rows[17-2].Cells[0].CellFormat.Font.Bold = ExcelDefaultableBoolean.True;

            e.CurrentWorksheet.Rows[6 - 2].Cells[0].CellFormat.Indent = 0;
            e.CurrentWorksheet.Rows[7 - 2].Cells[0].CellFormat.Indent = 0;
            e.CurrentWorksheet.Rows[12 - 2].Cells[0].CellFormat.Indent = 0;
            e.CurrentWorksheet.Rows[17 - 2].Cells[0].CellFormat.Indent = 0;
            try
            {
                //___________
                //|-^-/-\-^-|
                //|-{"O`O"}-|
                //|---(@)---|
                //|---~V~---|
                //TTTTTTTTTTT
                //амулет аборигенов австралии, защищает от злых бАгав, ускаряет кодирование, яхоу!
            }
            catch 
            {
                //охо!
            }

            e.Workbook.Worksheets.Add("расчет показателей 2");
            e.CurrentWorksheet = e.Workbook.Worksheets["расчет показателей 2"];
            e.CurrentWorksheet.MergedCellsRegions.Add(0, 0, 0, 3);
            e.CurrentWorksheet.Rows[0].Cells[0].Value = Label4.Text;
            SetCellFormat(e, 0, 0, 1000, -1, HorizontalCellAlignment.Left, VerticalCellAlignment.Center, new Font("Arial", 1, FontStyle.Bold), 260);
            e.CurrentWorksheet.Rows[1].Height = 350;
            Export_DT(GDt, offset, e, 24, 31,1==2);
            e.CurrentWorksheet.Rows[1].Height = 350;

            e.Workbook.Worksheets.Add("расчет показателей 3");
            e.CurrentWorksheet = e.Workbook.Worksheets["расчет показателей 3"];
            e.CurrentWorksheet.MergedCellsRegions.Add(0, 0, 0, 4);
            e.CurrentWorksheet.Rows[0].Cells[0].Value = Label5.Text;
            SetCellFormat(e, 0, 0, 1000, -1, HorizontalCellAlignment.Left, VerticalCellAlignment.Center, new Font("Arial", 1, FontStyle.Bold), 260);
            e.CurrentWorksheet.Rows[1].Height = 350;

            Export_DT(GDt, offset, e, 31, 40-2,1==2);
            e.CurrentWorksheet.Rows[1].Height = 350;

            


        }

        private void RenameGDT(DataTable GDt)
        {
            foreach (DataRow Row in GDt.Rows)
            {
                if (Row[0] != DBNull.Value)
                {
                    //                    if (row.Cells[0].Text == "1.3.2")
                    //{
                    //    row.Cells[1].Text = "Целевое значение численности врачей (физических лиц) государственных (муниципальных) учреждений здравоохранения на 10 тыс. человек населения, человек";
                    //}
                    //if (row.Cells[0].Text == "1.4.2")
                    //{
                    //    row.Cells[1].Text = "Целевое значение численности среднего медицинского персонала (физических лиц) государственных (муниципальных) учреждений здравоохранения на 10 тыс. человек населения, человек";
                    //}

                    //if (row.Cells[0].Text == "1.5.2")
                    //{
                    //    row.Cells[1].Text = "Целевое значение численности врачей (физических лиц) государственных (муниципальных) учреждений здравоохранения на 10 тыс. человек населения, человек";
                    //}
                    //if (row.Cells[0].Text == "1.5.3")
                    //{
                    //    row.Cells[1].Text = "Целевое значение численности среднего медицинского персонала (физических лиц) государственных (муниципальных) учреждений здравоохранения на 10 тыс. человек населения, человек";
                    //}

                    Row[0] = Row[0].ToString()
                        .Replace(
                        "Среднероссийское значение численности врачей (физических лиц) государственных (муниципальных) учреждений здравоохранения на 10 тыс. человек населения",
                        "Целевое значение численности врачей (физических лиц) государственных (муниципальных) учреждений здравоохранения на 10 тыс. человек населения")
                        .Replace(
                        "Среднероссийское значение численности среднего медицинского персонала (физических лиц) государственных (муниципальных) учреждений здравоохранения на 10 тыс. человек населения",
                        "Целевое значение численности среднего медицинского персонала (физических лиц) государственных (муниципальных) учреждений здравоохранения на 10 тыс. человек населения")
                        .Replace(
                        "Среднероссийское значение численности врачей (физических лиц) государственных (муниципальных) учреждений здравоохранения на 10 тыс. человек населения",
                        "Целевое значение численности врачей (физических лиц) государственных (муниципальных) учреждений здравоохранения на 10 тыс. человек населения")
                        .Replace(
                        "Среднероссийское значение численности среднего медицинского персонала (физических лиц) государственных (муниципальных) учреждений здравоохранения на 10 тыс. человек населения",
                        "Целевое значение численности среднего медицинского персонала (физических лиц) государственных (муниципальных) учреждений здравоохранения на 10 тыс. человек населения, человек");
                }
            }
        }

        int Export_DT(DataTable dt, int offset, EndExportEventArgs e,int Start, int End,bool CalcTemp)
        {            
            int xz = CalcTemp ? 3 : 1;
            e.CurrentWorksheet.Columns[1].Hidden = 1 == 1;
            for (int i = 0; i < dt.Columns.Count; i++)
            {                
                e.CurrentWorksheet.Rows[offset].Cells[i].Value = dt.Columns[i].Caption;
                e.CurrentWorksheet.Rows[offset].Cells[i].CellFormat.FillPattern = FillPatternStyle.Default;
                e.CurrentWorksheet.Rows[offset].Cells[i].CellFormat.FillPatternBackgroundColor = Color.Gray;
                e.CurrentWorksheet.Rows[offset].Cells[i].CellFormat.FillPatternForegroundColor = Color.Gray;
                SetCellFormat(e, i, offset, 1000, -1, HorizontalCellAlignment.Left, VerticalCellAlignment.Top, new Font("Arial", 1, FontStyle.Bold), 200);

                e.CurrentWorksheet.Columns[i].Width = 6000;                
                e.CurrentWorksheet.Rows[offset].Height = !CalcTemp ? 1500 : 700;

                for (int j = Start; j < End*xz; j+=xz)
                {
                    try
                    {
                        if (dt.Rows[Start + (j - Start) / xz][i].ToString().Split('_').Length > 1)
                        {
                            dt.Rows[Start + (j - Start) / xz][i] = dt.Rows[Start + (j - Start) / xz][i].ToString().Split('_')[dt.Rows[Start + (j - Start) / xz  ][i].ToString().Split('_').Length - 1];
                        }
                    }
                    catch { }
                    e.CurrentWorksheet.Rows[(j - Start) + offset + 1].Cells[i].Value = string.Format("{0:### ### ##0.00}",(dt.Rows[Start + (j-Start)/xz][i]));
                    SetCellFormat(e, i, (j - Start) + offset + 1, -1, -1, HorizontalCellAlignment.Right, VerticalCellAlignment.Bottom, new Font("Arial", 1), 200);
                    if (i != 0)
                    {
                        e.CurrentWorksheet.Rows[(j - Start) + offset + 1].Cells[i].CellFormat.Alignment = HorizontalCellAlignment.Right;
                        e.CurrentWorksheet.Rows[(j - Start) + offset + 2].Cells[i].CellFormat.Alignment = HorizontalCellAlignment.Right;
                        e.CurrentWorksheet.Rows[(j - Start) + offset + 3].Cells[i].CellFormat.Alignment = HorizontalCellAlignment.Right;
                        if (CalcTemp)
                        {

                            try
                            {
                                SetCellFormat(e, i, (j - Start) + offset + 2, -1, -1, HorizontalCellAlignment.Right, VerticalCellAlignment.Bottom, new Font("Arial", 1), 200);
                                SetCellFormat(e, i, (j - Start) + offset + 3, -1, -1, HorizontalCellAlignment.Right, VerticalCellAlignment.Bottom, new Font("Arial", 1), 200);
                                e.CurrentWorksheet.Rows[(j - Start) + offset + 2].Cells[i].Value = string.Format("{0:### ### ##0.00}", (System.Decimal)(dt.Rows[j / xz][i]) - (System.Decimal)(dt.Rows[j / xz][i - 1]));
                                e.CurrentWorksheet.Rows[(j - Start) + offset + 3].Cells[i].Value = string.Format("{0:### ### ##0.00}", (((System.Decimal)(dt.Rows[j / xz][i]) / (System.Decimal)(dt.Rows[j / xz][i - 1]) - 1) * 100));
                            }
                            catch { }
                        }

                    }
                    else
                    {
                        SetCellFormat(e, i, (j - Start) + offset + 1, -1, -1, HorizontalCellAlignment.Left, VerticalCellAlignment.Top, new Font("Arial", 1), 210);
                        if (CalcTemp)
                        {
                            e.CurrentWorksheet.Rows[(j - Start) + offset + 2].Cells[i].Value = "абсолютные отклонения";
                            e.CurrentWorksheet.Rows[(j - Start) + offset + 2].Cells[i].CellFormat.Alignment = HorizontalCellAlignment.Right;
                            e.CurrentWorksheet.Rows[(j - Start) + offset + 3].Cells[i].Value = "темп прироста";
                            e.CurrentWorksheet.Rows[(j - Start) + offset + 3].Cells[i].CellFormat.Alignment = HorizontalCellAlignment.Right;
                            SetCellFormat(e, i, (j - Start) + offset + 2, -1, -1, HorizontalCellAlignment.Right, VerticalCellAlignment.Bottom, new Font("Arial", 1), 200);
                            SetCellFormat(e, i, (j - Start) + offset + 3, -1, -1, HorizontalCellAlignment.Right, VerticalCellAlignment.Bottom, new Font("Arial", 1), 200);
                        }
                        else
                        {
                            if (((j - Start) + offset + 3) == 4)
                            {
                                e.CurrentWorksheet.Rows[(j - Start) + offset + 1].Cells[0].CellFormat.Font.Bold = ExcelDefaultableBoolean.True;                                
                            }
                            else
                            {
                                //e.CurrentWorksheet.Rows[(j - Start) + offset + 1].Cells[0].Value = "   "+e.CurrentWorksheet.Rows[(j - Start) + offset + 1].Cells[0].Value;
                                e.CurrentWorksheet.Rows[(j - Start) + offset + 1].Cells[0].CellFormat.Indent = 4;
                            }
                        }
                    }
                }

            }
            e.CurrentWorksheet.Rows[offset].Cells[0].Value = "Показатель";
            e.CurrentWorksheet.Columns[0].Width = 24000;
            return End*xz-Start + 5 + offset;
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
            UltraGridExporter1.ExcelExporter.DownloadName = "report.xls";
            UltraGridExporter1.ExcelExporter.Export(G);
        }

        protected void C_DataBinding(object sender, EventArgs e)
        {

        }

        protected void C1_DataBinding(object sender, EventArgs e)
        {

            DataTable dt = new DataTable();
            for (int i = 0; i < GDt.Columns.Count; i++)
            {
                dt.Columns.Add(GDt.Columns[i].Caption,GDt.Columns[i].DataType);
            }
            for (int i = 41-3; i < 43-3; i++)
            {
                dt.Rows.Add(GDt.Rows[i].ItemArray);
                dt.Rows[dt.Rows.Count - 1][0] = dt.Rows[dt.Rows.Count - 1][0].ToString().Split('_')[dt.Rows[dt.Rows.Count - 1][0].ToString().Split('_').Length - 1];
            }
            dt.Columns.Remove(dt.Columns[1]);
             
            dt.Rows[1][0] = @"Доля неэффективных расходов в сфере здравоохранения 
 в общем объеме расходов консолидированного бюджета";
            dt.Rows[0][0] = @"Доля неэффективных расходов в сфере здравоохранения
 в общем объеме расходов консолидированного бюджета 
субъекта РФ на здравоохранение";
            C1.DataSource =dt;

            C1.Axis.X.Extent = 15;
            C1.Axis.Y.Extent = 50;
            C1.LineChart.Thickness = 4; 
            C1.LineChart.StartStyle = Infragistics.UltraChart.Shared.Styles.LineCapStyle.RoundAnchor;
            C1.LineChart.EndStyle = Infragistics.UltraChart.Shared.Styles.LineCapStyle.RoundAnchor;
            C1.LineChart.HighLightLines = true;
            C1.Axis.X.Margin.Far.Value = 5;
            C1.Axis.X.Margin.Near.Value = 5;
            C1.Axis.X.Labels.Font = new Font("Arial", 9);
            C1.Axis.X.Labels.SeriesLabels.Font = new Font("Arial", 9);
                //.Size = 8; 

            C2.Axis.X.Labels.Font = new Font("Arial", 9);
            C2.Axis.X.Labels.SeriesLabels.Font = new Font("Arial", 9);
            C2.Axis.X.Extent = 15;
            C2.Axis.Y.Extent = 50;
        }
        DataTable dt_C2;
        protected void C2_DataBinding(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();
            for (int i = 0; i < GDt.Columns.Count; i++)
            {
                dt.Columns.Add(GDt.Columns[i].Caption, GDt.Columns[i].DataType);
            }
            for (int i = 44-4; i < 47-4; i++)
            {
                dt.Rows.Add(GDt.Rows[i].ItemArray);
                dt.Rows[dt.Rows.Count - 1][0] = dt.Rows[dt.Rows.Count - 1][0].ToString().Split('_')[dt.Rows[dt.Rows.Count - 1][0].ToString().Split('_').Length - 1].Split(',')[dt.Rows[dt.Rows.Count - 1][0].ToString().Split('_')[dt.Rows[dt.Rows.Count - 1][0].ToString().Split('_').Length - 1].Split(',').Length-2];
                //dt.Rows.Add(GDt.Rows[i].
            }
            dt.Rows[1][0] = @"Объем неэффективных расходов на управление объемами стационарной
медицинской помощи";

            dt.Columns.Remove(dt.Columns[1]);
            dt_C2 = dt;
            C2.DataSource = dt;
        }

        protected void UltraWebGrid1_DataBinding(object sender, EventArgs e)
        {

        }

        protected void G2_DataBinding(object sender, EventArgs e)
        {
            G2.Rows.Clear();
            G2.Columns.Clear();
            G2.Bands.Clear();
            
            
            G2.DataSource = GDt;
            
        }

        protected void G3_DataBinding(object sender, EventArgs e)
        {
            G3.Rows.Clear();
            G3.Columns.Clear();
            G3.Bands.Clear();
            G3.DataSource = GDt;
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
            Panel2.Visible = CheckBox1.Checked;
            
        }

        protected void G4_DataBinding(object sender, EventArgs e)
        {
            G4.Rows.Clear();
            G4.Columns.Clear();
            G4.Bands.Clear();
            G4.DataSource = GDt;
        }

        protected void G2_InitializeRow(object sender, RowEventArgs e)
        {
            if ((e.Row.Index >= 5) & (e.Row.Index <= 23))
            {
                e.Row.Cells[e.Row.Cells.Count - 1].Text = ColumnFormula2[e.Row.Index - 5];
                RowDel_(e);
                if ((e.Row.Index == 5) || (e.Row.Index == 7) || (e.Row.Index == 8) || (e.Row.Index == 13) || (e.Row.Index == 18))
                {
                    e.Row.Cells[0].Style.Font.Bold = 1 == 1;
                }
                else
                {
                    e.Row.Cells[0].Style.Padding.Left = 15;
                }
            }
            else
            {
                e.Row.Hidden = 1 == 1;
            }
            if (e.Row.Cells[0].Text[e.Row.Cells[0].Text.Length - 1] == '.')
            {
                e.Row.Cells[0].Text = e.Row.Cells[0].Text.Remove(e.Row.Cells[0].Text.Length - 1);
            }
        }

        protected void G3_InitializeRow(object sender, RowEventArgs e)
        {
            if ((e.Row.Index >= 24) & (e.Row.Index <= 30))
            {
                e.Row.Cells[e.Row.Cells.Count - 1].Text = ColumnFormula3[e.Row.Index - 24];
                RowDel_(e);
                if ((e.Row.Index == 24))
                {
                    e.Row.Cells[0].Style.Font.Bold = 1 == 1;
                }
                else
                {
                    e.Row.Cells[0].Style.Padding.Left = 15;
                }
            }
            else
            {
                e.Row.Hidden = 1 == 1;
            }
        }

        protected void G4_InitializeRow(object sender, RowEventArgs e)
        {
            try
            { }
            catch { }
                if ((e.Row.Index >= 31) & (e.Row.Index <= 37))
                {
                    e.Row.Cells[e.Row.Cells.Count - 1].Text = ColumnFormula4[e.Row.Index - 31];
                    RowDel_(e);
                    if ((e.Row.Index == 31))
                    {
                        e.Row.Cells[0].Style.Font.Bold = 1 == 1;
                    }
                    else
                    {
                        e.Row.Cells[0].Style.Padding.Left = 15;
                    }
                }
                else
                {
                    e.Row.Hidden = 1 == 1;
                }
            
        }




        //#########################################################
        protected void C1_FillSceneGraph(object sender, Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e)
        {
            Infragistics.UltraChart.Core.Primitives.Text textLabel;


            int textWidth = 500;
            int textHeight = 12;
            try
            {
                textLabel = new Infragistics.UltraChart.Core.Primitives.Text();
                textLabel.labelStyle.Font = new Font("Microsoft Sans Serif", 9);
                textLabel.PE.Fill = Color.Black;
                textLabel.bounds = new Rectangle(30, 249-21-8, textWidth, textHeight);
                textLabel.SetTextString("Доля неэффективных расходов в общих расходах на здравоохранение");
                e.SceneGraph.Add(textLabel);
                textLabel = new Infragistics.UltraChart.Core.Primitives.Text();
                textLabel.labelStyle.Font = new Font("Microsoft Sans Serif", 9);
                textLabel.PE.Fill = Color.Black;
                textLabel.bounds = new Rectangle(30, 249-8, textWidth, textHeight);
                textLabel.SetTextString("Доля неэффективных расходов в общих расходах");
                e.SceneGraph.Add(textLabel);
            }
            catch
            {

            }

        }

        protected void C2_FillSceneGraph(object sender, Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e)
        {
            int textWidth = 550;
            int textHeight = 12;
            try
            {
                Infragistics.UltraChart.Core.Primitives.Text textLabel;
                textLabel = new Infragistics.UltraChart.Core.Primitives.Text();
                textLabel.labelStyle.Font = new Font("Microsoft Sans Serif", 9);
                textLabel.PE.Fill = Color.Black;
                textLabel.bounds = new Rectangle(29, 249 - 21 - 8, textWidth, textHeight);
                textLabel.SetTextString(dt_C2.Rows[0][0].ToString());
                //textLabel.SetTextString("1");
                e.SceneGraph.Add(textLabel);

                textLabel = new Infragistics.UltraChart.Core.Primitives.Text();
                textLabel.labelStyle.Font = new Font("Microsoft Sans Serif", 9);
                textLabel.PE.Fill = Color.Black;
                textLabel.bounds = new Rectangle(29, 249-8-5, textWidth, textHeight*2);
                textLabel.SetTextString(dt_C2.Rows[1][0].ToString());
                //textLabel.SetTextString("2");
                e.SceneGraph.Add(textLabel);

                textLabel = new Infragistics.UltraChart.Core.Primitives.Text();
                textLabel.labelStyle.Font = new Font("Microsoft Sans Serif", 9);
                textLabel.PE.Fill = Color.Black;
                textLabel.bounds = new Rectangle(29, 249 - 8+21+2, textWidth, textHeight);
                textLabel.SetTextString(dt_C2.Rows[2][0].ToString());
                //textLabel.SetTextString("3");

                e.SceneGraph.Add(textLabel);
            }
            catch { }
        }

        protected void C1_ChartDrawItem(object sender, Infragistics.UltraChart.Shared.Events.ChartDrawItemEventArgs e)
        {

        }





    }
}
