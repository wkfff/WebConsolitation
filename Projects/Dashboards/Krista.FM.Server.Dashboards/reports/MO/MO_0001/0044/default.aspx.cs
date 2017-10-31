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
using Infragistics.UltraChart.Core;
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

using Color = System.Drawing.Color;
using EndExportEventArgs = Infragistics.WebUI.UltraWebGrid.ExcelExport.EndExportEventArgs;
using Font = Infragistics.Documents.Reports.Graphics.Font;
using Image = Infragistics.Documents.Reports.Graphics.Image;

namespace Krista.FM.Server.Dashboards.reports.MO.MO_0001._0044
{
    public partial class _default : CustomReportPage
    {
        private CustomParam YearSelect;
        private CustomParam YearSelect2;
        private CustomParam YearSelect3;
        private CustomParam YearSelect4;
        private CustomParam RegionSelect;
        private CustomParam GridSelect;
        private CustomParam GridSelectIndex;

        private CustomParam member;

        string Economic_Prefix = "[Показатели].[Методики Сопоставимый].[Раздел].[Оценка СЭР МО].[Показатели социальной сферы]";
        string Social_Prefix = "[Показатели].[Методики Сопоставимый].[Раздел].[Оценка СЭР МО].[Показатели экономической сферы]";
        string Date_Prefix = ",[Период].[Год Квартал Месяц].[Год].[{0}].lag({1})";
        string Date_Prefix2 = ",[Период].[Год Квартал Месяц].[Год].[{0}].lag({1})";
        string LBC = "{0}";



        

        private string LParam;

        public DataTable GetDSForChart(string sql)
        {
            DataTable dt = new DataTable();
            string s = DataProvider.GetQueryText(sql);
            DataProvidersFactory.PrimaryMASDataProvider.PopulateDataTableForChart(DataProvidersFactory.PrimaryMASDataProvider.GetCellset(s), dt, "sadad");
            return dt;
        }
        string[] l_Ar;
        string[] ldAr;

        Dictionary<string, int> ForParam(string query)
        {
            Dictionary<string, int> d = new Dictionary<string, int>();


            DataTable dt = GetDSForChart(query);
            l_Ar = new string[dt.Rows.Count];
            for (int i = dt.Rows.Count - 1; i >= 0; i--)
            {
                try
                {

                    d.Add(dt.Rows[i][0].ToString(), 0);
                    l_Ar[i] = dt.Rows[i][0].ToString();
                }
                catch { }
            }
            
            
            LParam = dt.Rows[dt.Rows.Count - 1][0].ToString();
            return d;
        }

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);
            Page.UICulture = "af-ZA";
            Page.Title = "Оценка социально-экономического развития территории (для  муниципального образования)";
            G.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 15-5);
            G.Height = CRHelper.GetGridHeight(550);
            //if (BN == "FIREFOX")
            //{
            //    G.Height = 640;
            //}
             
            //Да я сам не знаю зачем они нужны, но если их убрать то не компилится :(
            YearSelect = UserParams.CustomParam("YearSelect");
            YearSelect2 = UserParams.CustomParam("YearSelect2");
            YearSelect3 = UserParams.CustomParam("YearSelect3");
            YearSelect4 = UserParams.CustomParam("YearSelect4");

            RegionSelect = UserParams.CustomParam("RegionSelect");
            GridSelect = UserParams.CustomParam("GridSelect");

            GridSelectIndex = UserParams.CustomParam("");

            member = UserParams.CustomParam("member");

            C2.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth / 2.0-10-5);
            C3.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth / 2.0 -10);


            C2.Height = 500;
            C3.Height = 500;

            C2_Panel.AddLinkedRequestTrigger(G);
            C3_Panel_.LinkedRefreshControlID = C2_Panel.ID;

            C3.ChartType = Infragistics.UltraChart.Shared.Styles.ChartType.PieChart;
            C3.PieChart.OthersCategoryPercent = 1;
            C3.Legend.Visible = 1 == 1; 
            C3.Legend.Location = Infragistics.UltraChart.Shared.Styles.LegendLocation.Bottom;
            C3.Legend.SpanPercentage = 40;
            L_PageText.Height = 323;  

            C1.Width = CRHelper.GetChartWidth(2*CustomReportConst.minScreenWidth / 3.0- 40-5);
            L_PageText.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth / 3.0 + 25);
        }
        string BN = "IE";
        protected override void Page_Load(object sender, EventArgs e)
        {
            System.Web.HttpBrowserCapabilities myBrowserCaps = Request.Browser;
            BN = ((System.Web.Configuration.HttpCapabilitiesBase)myBrowserCaps).Browser.ToUpper();


            try { }
            catch
            {

            }
               // RegionSettingsHelper.Instance.SetWorkingRegion("PRTEST5");
            //RegionSettingsHelper.Instance.SetWorkingRegion(RegionSettings.Instance.Id);

                //влом....
                bool regionViseble = RegionSettingsHelper.Instance.GetPropertyValue("RegionIsMO")=="True";
                base.Page_Load(sender, e);                                    
                if (!Page.IsPostBack)
                {                    
                    
                    
                    member.Value = DataProvider.GetQueryText("member");                    

                    Year.FillDictionaryValues(ForParam("AllLD"));
                    ldAr = l_Ar;
                    Year.Width = 100;
                    Year.SetСheckedState(LParam, 1 == 1);
                    YearSelect.Value = String.Format(Date_Prefix,Year.SelectedValue,"0");
                    YearSelect2.Value = String.Format(Date_Prefix2, Year.SelectedValue,"1");
                    YearSelect3.Value = Year.SelectedValue;
                    RegionSelect.Value = RegionSettingsHelper.Instance.GetPropertyValue("RegionsLevel");
                    if (regionViseble)
                    {                        
                        Regions.FillDictionaryValues(ForParam("AllRegion"));
                        Regions.SetСheckedState(LParam, 1 == 1);
                        Regions.Width = 200;
                        Regions.Title = "Территория";
                    }
                    Regions.Visible = regionViseble;
                    RegionSelect.Value = regionViseble ? RegionSettingsHelper.Instance.GetPropertyValue("RegionsLevel") + ".[" + Regions.SelectedValue + "]" : RegionSettingsHelper.Instance.GetPropertyValue("RegionsLevel");
                    GridSelectIndex.Value = (1).ToString();
                    
                }
                ActiveIndexGrid = int.Parse(GridSelectIndex.Value);
                
                Label1.Text = string.Format("Интегральные показатели оценки социальной и экономической сферы и их составляющие в {0} году", Year.SelectedValue);
                

                YearSelect.Value = String.Format(Date_Prefix, Year.SelectedValue,"0");
                YearSelect2.Value = String.Format(Date_Prefix2, Year.SelectedValue,"1");
                YearSelect3.Value = Year.SelectedValue;
                member.Value = DataProvider.GetQueryText("member");
                RegionSelect.Value = regionViseble ? RegionSettingsHelper.Instance.GetPropertyValue("RegionsLevel") + ".[" + Regions.SelectedValue + "]" : RegionSettingsHelper.Instance.GetPropertyValue("RegionsLevel");
                try
                {
                    G.DataBind();
                    G.Rows[ActiveIndexGrid].Activate();
                    G.Rows[ActiveIndexGrid].Selected = 1 == 1;
                    G.Rows[ActiveIndexGrid].Activated = 1 == 1;
                }
                catch 
                {
                    //G.Bands[0].Columns.Clear();
                    //G.Bands.Clear();
                    //L_PageText.Text = "";
                
                }
                post_Grid(G);

                C3.DataBind();
                YearSelect.Value = "";
                YearSelect2.Value = "";                
                C2.DataBind();
                C1.DataBind();
                Label5.Text = string.Format(LBC, G.Rows[ActiveIndexGrid].Cells[1].Text.Split('<'));
          
        }

        int Econom_index = 11,Social_index = 0;

        System.Decimal SumSoc = 0;
        System.Decimal SumEconom = 0;

        protected DataTable CalculationGrid()
        {
            SumSoc = 0;
            SumEconom = 0;
            dtRes = new DataTable();
            DataTable dt = GetDSForChart("Grid");
            DataTable dt_Mass = GetDSForChart("Massa");
            dtRes.Columns.Add("Показатель_Hidde", dt.Columns[0].DataType);
            dtRes.Columns.Add("Показатель", dt.Columns[0].DataType);
            dtRes.Columns.Add("Значение по МО", dt.Columns[2].DataType);
            dtRes.Columns.Add("Отклонение от значения по субъекту", dt.Columns[2].DataType);
            dtRes.Columns.Add("Отклонение от значения по РФ", dt.Columns[2].DataType);
            dtRes.Columns.Add("Индикатор", dt.Columns[2].DataType);
            dtRes.Columns.Add("Значение 4* значение свойства «Весовой коэффициент»", dt.Columns[2].DataType);
            
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                dtRes.Rows.Add(new object[5]);
            }
            /**/
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                try
                {
                    dtRes.Rows[i][0] = dt.Rows[i][0];                    
                    dtRes.Rows[i][1] = dt_Mass.Rows[i][2].ToString() + "<br> <div style=\"font-size: 7pt; font-family: Arial;\">&nbsp&nbsp&nbspВесовой коэффициент " + ((System.Decimal)(dt_Mass.Rows[i][1])).ToString() + "</div>";
                    dtRes.Rows[i][2] = dt.Rows[i][2];
                    
                }catch{}

                

                try
                {//Обеспеченность врачами (на 10000 человек населения)

                    CRHelper.SaveToErrorLog(dt.Rows[i][0].ToString());
                    dtRes.Rows[i][5] = Math.Abs((System.Decimal)(dt.Rows[i][6])) > 10000 ? null :
                        (object)(((dt.Rows[i][0].ToString() == "Обеспеченность врачами (на 10000 человек населения)") 
                        || 
                        //(dt.Rows[i][0].ToString() == "Численность населения с впервые выявленным диагнозом (на 1000 человек населения)") ||
                        (dt.Rows[i][0].ToString() == "Число зарегистрированных правонарушений (на 10000 человек населения), единиц")) ?
                        1 / (System.Decimal)(dt.Rows[i][6]) : (System.Decimal)(dt.Rows[i][6]));



                    }
                catch { }
                    try
                    {
                        dtRes.Rows[i][3] = (System.Decimal)(dt.Rows[i][2]) - (System.Decimal)(dt.Rows[i][3]);
                        dtRes.Rows[i][4] = (System.Decimal)(dt.Rows[i][2]) - (System.Decimal)(dt.Rows[i][4]);
                    }
                    catch { }
                    
                
                try
                {
                    dtRes.Rows[i][6] = 
                            (System.Decimal)(dtRes.Rows[i][5]) * (System.Decimal)(dt_Mass.Rows[i][1]);
                }
                catch { }

                //CRHelper.SaveToErrorLog(dtRes.Rows[i][0].ToString());   
                if (dtRes.Rows[i][0].ToString() == "Показатели экономической сферы")
                {
                    Econom_index = i;
                }
                if (dtRes.Rows[i][0].ToString() == "Показатели социальной сферы")
                {
                    Social_index = i;
                }
            }
            for (int i = Social_index + 1; i < Econom_index; i++)
            {
                try
                {
                    SumSoc += (System.Decimal)(dtRes.Rows[i][6]);
                }
                catch { }
            }
            for (int i = Econom_index + 1; i < dtRes.Rows.Count; i++)
            {
                try
                {
                    SumEconom += (System.Decimal)(dtRes.Rows[i][6]);
                }
                catch { }
            }
            //int
            //CRHelper.SaveToErrorLog(Social_index.ToString() + "|" + Econom_index.ToString());
            SocMassa = (System.Decimal)(dt_Mass.Rows[Social_index][1]);
            EconomMassa = (System.Decimal)(dt_Mass.Rows[Econom_index][1]);
            dtRes.Rows[Social_index][5] = SumSoc ;//== 0 ? "-" : SumSoc;
            dtRes.Rows[Econom_index][5] = SumEconom ;//== 0 ? "-" : SumEconom;
            return dtRes;
        }
        System.Decimal SocMassa = 0;
        System.Decimal EconomMassa = 0;


        DataTable dtRes = new DataTable();
        protected void G_DataBinding(object sender, EventArgs e)
        {
            G.DataSource = CalculationGrid();
        }

        protected void post_Grid(UltraWebGrid G)
        {

            //CRHelper.SaveToErrorLog(G.Rows.Count.ToString());
            try
            {
                G.Rows[Econom_index].Cells[1].ColSpan = 4;
                G.Rows[Social_index].Cells[1].ColSpan = 4;
            }
            catch { }
            for (int i = 0; i < G.Columns.Count; i++)
            {
                G.Rows[Econom_index].Cells[i].Style.Font.Bold = 1 == 1;
                G.Rows[Social_index].Cells[i].Style.Font.Bold = 1 == 1;
            }   
            G.Rows[Econom_index].Cells[1].Text = G.Rows[Econom_index].Cells[1].Text.Split('<')[0];
            G.Rows[Social_index].Cells[1].Text = G.Rows[Social_index].Cells[1].Text.Split('<')[0];

        }

        protected void G_InitializeLayout(object sender, LayoutEventArgs e)
        {
            for (int i = 0; e.Layout.Bands[0].Columns.Count>i;  i++)
            {
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(BN == "FIREFOX" ? 98 : BN == "IE" ? 95 : 80);//CRHelper.GetColumnWidth(95);
                e.Layout.Bands[0].Columns[i].CellStyle.Wrap = 1 == 1;
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "N2");
                e.Layout.Bands[0].Columns[i].Header.Style.Wrap = 1 == 1;
                e.Layout.Bands[0].Columns[i].Header.Style.HorizontalAlign = HorizontalAlign.Center;
            }
            e.Layout.Bands[0].Columns[0].Hidden = 1 == 1;
            e.Layout.Bands[0].Columns[6].Hidden = 1 == 1;
            e.Layout.Bands[0].Columns[1].Width = CRHelper.GetColumnWidth(BN=="FIREFOX"?740:BN=="IE"?750:780);            
            e.Layout.Bands[0].Columns[1].CellStyle.HorizontalAlign = HorizontalAlign.Left;            
            e.Layout.Bands[0].Columns[1].Header.Style.Wrap = 1 == 1;
            e.Layout.AllowSortingDefault = AllowSorting.No;
            
        }

        protected void С2_DataBinding(object sender, EventArgs e)
        {
            YearSelect.Value = "";
            YearSelect2.Value = "";

            
            //G.DisplayLayout.a
            try
            {
                Infragistics.WebUI.UltraWebNavigator.Node n = Year.SelectedNode;
                for (int i = 0; i < 5; i++)
                {
                    YearSelect4.Value = n.Text;
                    n = n.NextNode;

                }
            }
            catch { }


                ActiveIndexGrid = int.Parse(GridSelectIndex.Value);
            if ((ActiveIndexGrid == Econom_index) || (ActiveIndexGrid == Social_index))
            {
                ActiveIndexGrid++;
            }
            if (Econom_index < ActiveIndexGrid)
            {
                GridSelect.Value = Social_Prefix + ".[" + G.Rows[ActiveIndexGrid].Cells[0].Text + "]";
            }
            else
            {
                GridSelect.Value = Economic_Prefix + ".[" + G.Rows[ActiveIndexGrid].Cells[0].Text + "]";
            }
            member.Value = DataProvider.GetQueryText("member");

            DataTable dt = GetDSForChart("C2");

            DataTable dtNew = new DataTable();

            dtNew.Columns.Add("xz", dt.Columns[0].DataType);
            dtNew.Columns.Add(dt.Columns[1].Caption.Replace("ДАННЫЕ)", ""), dt.Columns[1].DataType);
            for (int i = 2; i < dt.Columns.Count; i++)
            {
                dtNew.Columns.Add(dt.Columns[i].Caption.Replace("ДАННЫЕ)", "").Remove(0, 1), dt.Columns[i].DataType);                

            }
            bool noEmpty = 1 == 2;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                object[] o_O = dt.Rows[i].ItemArray; 
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    o_O[j] = o_O[j] == null ? (System.Decimal)(0) : o_O[j];
                }
                dtNew.Rows.Add(o_O);
                for (int j = 1; j < dt.Columns.Count; j++)
                {
                    if (dtNew.Rows[i][j].ToString() != "0")
                    {
                        noEmpty = 1==1;
                    }
                }
            }
            C2.DataSource =noEmpty?dtNew:null;

            C2.Axis.X.Labels.Visible = 1 == 2;
            C2.Axis.X.Labels.SeriesLabels.Visible = 1 == 1;
            C2.Legend.Location = Infragistics.UltraChart.Shared.Styles.LegendLocation.Bottom;
            C2.Legend.SpanPercentage = 14;
            C2.Legend.Visible = 1 == 1;
            C2.ColumnChart.NullHandling = Infragistics.UltraChart.Shared.Styles.NullHandling.Zero;
            Label5.Text = string.Format(LBC, G.Rows[ActiveIndexGrid].Cells[1].Text.Split('<'));
            C2.Tooltips.FormatString = "<ITEM_LABEL>, <b><DATA_VALUE:### ##0.##></b>, " + G.Rows[ActiveIndexGrid].Cells[1].Text.Split('<')[0].Split(',')[G.Rows[ActiveIndexGrid].Cells[1].Text.Split('<')[0].Split(',').Length-1];
            //хм... а интересно, хотят ли пингвины научится летать..?
        }

        protected void C3_DataBinding(object sender, EventArgs e)
        {
            C3.Series.Clear();

            ActiveIndexGrid = int.Parse(GridSelectIndex.Value);

            YearSelect.Value = String.Format(Date_Prefix, Year.SelectedValue, "0");
            YearSelect2.Value = String.Format(Date_Prefix2, Year.SelectedValue, "1");
            YearSelect3.Value = Year.SelectedValue;
            member.Value = DataProvider.GetQueryText("member");

            CalculationGrid();
            int start = ActiveIndexGrid>=Econom_index?Econom_index:Social_index;
            int end = ActiveIndexGrid >= Econom_index?dtRes.Rows.Count:Econom_index;

            DataTable dt = new DataTable();
            dt.Columns.Add("Показатель", dtRes.Columns[0].DataType);
            dt.Columns.Add("Значение", dtRes.Columns[2].DataType);
           //Label9.Text = string.Format("Структура интегрального показателя в {0} году ", Year.SelectedValue);
            Label10.Text = dtRes.Rows[start][1].ToString().Split('<')[0]+" в "+Year.SelectedValue+" году";
            //bool nonempty = 1 == 2 ;
            //Label1.Text = "";
            for(int  i = start+1;i<end;i++)
            {
                //Label1.Text += dtRes.Rows[i][6] + "|" + dtRes.Rows[i][6].GetType().ToString();
                try
                {
                    if ((dtRes.Rows[i][6] != DBNull.Value) & (dtRes.Rows[i][6].ToString() != "0,00"))
                        dt.Rows.Add(dtRes.Rows[i][1].ToString().Split('<')[0], dtRes.Rows[i][6]);
                    CRHelper.SaveToErrorLog(dtRes.Rows[i][6].ToString());
                }
                catch { }
            }
            C3.DataSource = dt;
            C3.Tooltips.FormatString = "<ITEM_LABEL><br> Доля в интегральном показателе     <b><DATA_VALUE:0.##></b>";
            
        }
        int ActiveIndexGrid = 1;
        protected void G_ActiveRowChange(object sender, RowEventArgs e)
        {            
            //RegionSettingsHelper.Instance.SetWorkingRegion("PRTEST5");
            
            GridSelectIndex.Value = e.Row.Index.ToString();



            C2.DataBind();
            C3.DataBind();
            //RegionSettingsHelper.Instance.SetWorkingRegion("Novoorsk");
                        
        }

        string PageText =
@"В <b>{5}</b> году <b>интегральный показатель оценки СЭР МО</b> составил <b>{12}</b> <b>{0}</b> (абсолютное изменение по отношению к предыдущему году составило {11} {13}, темп прироста <br>равен  {10}%):<br><br>
&nbsp&nbsp&nbsp&nbsp<b>- интегральный показатель оценки социальной<br>
&nbsp&nbsp&nbsp&nbsp&nbspсферы</b> равен <b>{1}</b> (абсолютное изменение по<br>
&nbsp&nbsp&nbsp&nbsp&nbspотношению к предыдущему году составило {8} {14},<br>
&nbsp&nbsp&nbsp&nbsp&nbspтемп прироста равен {6}%);<br>
&nbsp&nbsp&nbsp&nbsp&nbspвесовой коэффициент в оценке СЭР – <b>{2}</b>;<br><br>
&nbsp&nbsp&nbsp&nbsp<b>- интегральный показатель оценки экономической<br> 
&nbsp&nbsp&nbsp&nbsp&nbspсферы </b>равен<b>{3}</b> (абсолютное изменение по<br> 
&nbsp&nbsp&nbsp&nbsp&nbspотношению к предыдущему году составило {9} {15},<br> 
&nbsp&nbsp&nbsp&nbsp&nbspтемп прироста равен {7}%);<br>
&nbsp&nbsp&nbsp&nbsp&nbspвесовой коэффициент в оценке СЭР – <b>{4 }</b>.";

        string PageText_ =
@"В <b>{5}</b> году <b>интегральный показатель оценки СЭР МО</b> составил <b>{6}</b> <b>{0 }</b>:<br><br>
        &nbsp&nbsp&nbsp&nbsp<b>-интегральный показатель оценки социальной<br> 
        &nbsp&nbsp&nbsp&nbsp&nbspсферы</b> равен <b>{1 }</b>;<br>
        &nbsp&nbsp&nbsp&nbsp&nbspвесовой коэффициент в оценке СЭР – <b>{2 }</b>;<br><br>
        &nbsp&nbsp&nbsp&nbsp<b>-интегральный показатель оценки экономической<br>
        &nbsp&nbsp&nbsp&nbsp&nbspсферы</b> равен <b>{3 }</b>;<br>
        &nbsp&nbsp&nbsp&nbsp&nbspвесовой коэффициент в оценке СЭР – <b>{4 }</b>.";

        protected void C1_DataBinding(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();
            int colcount= 0;
            dt.Columns.Add("Интегральный показатель",dtRes.Columns[0].DataType);
            colcount++;

            C1.ColorModel.ModelStyle = Infragistics.UltraChart.Shared.Styles.ColorModels.CustomSkin;

            Color color1 = Color.Red;
            Color color2 = Color.Blue;
            Color color3 = Color.Green;
            C1.Axis.Y.RangeType = Infragistics.UltraChart.Shared.Styles.AxisRangeType.Custom;
            C1.Axis.Y.RangeMax = 1.5;
            C1.Axis.Y.RangeMin = 0;

            C1.ColorModel.Skin.PEs.Add(CRHelper.GetFillPaintElement(color1, 150));
            C1.ColorModel.Skin.PEs.Add(CRHelper.GetFillPaintElement(color2, 150));
            C1.ColorModel.Skin.PEs.Add(CRHelper.GetFillPaintElement(color3, 150));
            C1.ColorModel.Skin.PEs[0].StrokeWidth = 3;
            C1.ColorModel.Skin.PEs[2].StrokeWidth = 3;
            C1.ColorModel.Skin.PEs[1].StrokeWidth = 3;
            C1.ColorModel.Skin.ApplyRowWise = true; 
            //C1.LineChart.Thickness = 12;
            try
            {   
                {
                    dt.Columns.Add(Year.SelectedNode.NextNode.NextNode.NextNode.NextNode.Text, dtRes.Columns[2].DataType);
                    colcount++;
                }

            }
            catch { }
            try
            {

                dt.Columns.Add(Year.SelectedNode.NextNode.NextNode.NextNode.Text, dtRes.Columns[2].DataType);
                colcount++;
            }
            catch { }        
            try
            {

                dt.Columns.Add(Year.SelectedNode.NextNode.NextNode.Text, dtRes.Columns[2].DataType);
                colcount++;
            }
            catch { }        
            try
            { 
                dt.Columns.Add(Year.SelectedNode.NextNode.Text, dtRes.Columns[2].DataType);
                colcount++;                
            }
            catch { }        
            try
            {
                dt.Columns.Add(Year.SelectedValue, dtRes.Columns[2].DataType);
                colcount++;
            }
            catch { }
            try
            {
                double PrirostSoc = 0;
                double PrirostEconom = 0;
                L_PageText.Text = "Нет данных";

                dt.Rows.Add(new object[colcount]);
                dt.Rows.Add(new object[colcount]);
                dt.Rows.Add(new object[colcount]);


                dt.Rows[0][0] = "Интегральный показатель оценки СЭР МО";
                dt.Rows[1][0] = "Интегральный показатель оценки социальной сферы";
                dt.Rows[2][0] = "Интегральный показатель оценки экономической сферы";

                CRHelper.SaveToQueryLog("Col0");

                dt.Rows[0][dt.Columns.Count - 1] = (System.Decimal)(SocMassa) * SumSoc + (EconomMassa) * SumEconom;
                dt.Rows[1][dt.Columns.Count - 1] = SumSoc;
                dt.Rows[2][dt.Columns.Count - 1] = SumEconom;

                PrirostSoc = Convert.ToDouble(SumSoc);
                PrirostEconom = Convert.ToDouble(SumEconom);
//C1.ColorModel.ModelStyle

                if ((SumSoc!=0)&(SumEconom!=0))
                L_PageText.Text =
                   string.Format(
                   PageText_,
                   "",
                   SumSoc.ToString("### ##0.00"),
                   SocMassa.ToString("### ##0.00").Replace('.', ','),
                   SumEconom.ToString("### ##0.00").Replace('.', ','),
                   EconomMassa.ToString("### ##0.00").Replace('.', ','),
                   Year.SelectedValue,
                  ((System.Decimal)(dt.Rows[0][dt.Columns.Count - 1])).ToString("### ##0.##")
                   );	

                System.Decimal SumSocBuf = SumSoc, SocMassaBuf = SocMassa, SumEconomBuf = SumEconom, EconomMassaBuf = EconomMassa;

                YearSelect.Value = String.Format(Date_Prefix, Year.SelectedNode.NextNode.Text, "0");
                YearSelect2.Value = String.Format(Date_Prefix2, Year.SelectedNode.NextNode.Text, "1");
                member.Value = DataProvider.GetQueryText("member");
                
                CalculationGrid();

                L_PageText.Text =
                    string.Format(
                    PageText,
/*0*/                  "",
/*1*/                  SumSocBuf.ToString("### ##0.00").Replace('.', ','),
/*2*/                  SocMassaBuf.ToString("### ##0.00").Replace('.', ','),
/*3*/                    SumEconomBuf.ToString("### ##0.00").Replace('.', ','),
/*4*/                    EconomMassaBuf.ToString("### ##0.00").Replace('.', ','),
/*5*/                    Year.SelectedValue,
/*6*/                    ((100 * SumSocBuf / SumSoc - 100) > 0) ? (100 * SumSocBuf / SumSoc - 100).ToString("### ##0.##") : "-" + (100 - 100 * SumSocBuf / SumSoc).ToString("### ##0.00").Replace(" ", "")
                    ,
/*7*/                    ((100 * SumEconomBuf / SumEconom - 100) > 0) ? (100 * SumEconomBuf / SumEconom - 100).ToString("### ##0.##") : "-" + (100 - 100 * SumEconomBuf / SumEconom).ToString("### ##0.##").Replace(" ", "")
                    ,
/*8*/                    (100 * SumSocBuf / SumSoc - 100) > 0 ? "<img src =\"../../../../Images/arrowGreenUpBB.png\">" : "<img src =\"../../../../Images/arrowRedDownBB.png\">"
                    ,
/*9*/                    (100 * SumEconomBuf / SumEconom - 100) > 0 ? "<img src =\"../../../../Images/arrowGreenUpBB.png\">" : "<img src =\"../../../../Images/arrowRedDownBB.png\">"
                    ,

/*10*/                    (100 * ((System.Decimal)(SocMassaBuf) * SumSocBuf + (EconomMassaBuf) * SumEconomBuf) / ((System.Decimal)(SocMassa) * SumSoc + (EconomMassa) * SumEconom) - 100).ToString("### ##0.00").Replace(" ", "")
                    ,
/*11*/                    (100 * ((System.Decimal)(SocMassaBuf) * SumSocBuf + (EconomMassaBuf) * SumEconomBuf) / ((System.Decimal)(SocMassa) * SumSoc + (EconomMassa) * SumEconom) - 100) > 0 ? "<img src =\"../../../../Images/arrowGreenUpBB.png\">" : "<img src =\"../../../../Images/arrowRedDownBB.png\">"
                    ,
/*12*/                    ((System.Decimal)(dt.Rows[0][dt.Columns.Count - 1])).ToString("### ##0.00"),
/*13*/                    (((System.Decimal)(SocMassaBuf) * SumSocBuf + (EconomMassaBuf) * SumEconomBuf) - ((System.Decimal)(SocMassa) * SumSoc + (EconomMassa) * SumEconom)).ToString("### ##0.00").Replace(" ", ""),
/*14*/                   (SumSocBuf - SumSoc).ToString("### ##0.00").Replace(" ","") ,
/*15*/                   (SumEconomBuf - SumEconom).ToString("### ##0.00").Replace(" ", ""));
                
                dt.Rows[0][dt.Columns.Count - 2] = (System.Decimal)(SocMassa) * SumSoc + (EconomMassa) * SumEconom;//(System.Decimal)(dtRes.Rows[Social_index][6]) * SumSoc + (System.Decimal)(dtRes.Rows[Econom_index][6]) * SumEconom;
                dt.Rows[1][dt.Columns.Count - 2] = SumSoc;
                dt.Rows[2][dt.Columns.Count - 2] = SumEconom;                

                YearSelect.Value = String.Format(Date_Prefix, Year.SelectedNode.NextNode.NextNode.Text.ToString(), "0");
                YearSelect2.Value = String.Format(Date_Prefix2, Year.SelectedNode.NextNode.NextNode.Text.ToString(), "1");
                member.Value = DataProvider.GetQueryText("member");

                CalculationGrid();

                dt.Rows[0][dt.Columns.Count - 3] = (System.Decimal)(SocMassa) * SumSoc + (EconomMassa) * SumEconom; ;//(System.Decimal)(dtRes.Rows[Social_index][6]) * SumSoc + (System.Decimal)(dtRes.Rows[Econom_index][6]) * SumEconom;
                dt.Rows[1][dt.Columns.Count - 3] = SumSoc;
                dt.Rows[2][dt.Columns.Count - 3] = SumEconom;

                YearSelect.Value = String.Format(Date_Prefix, Year.SelectedNode.NextNode.NextNode.NextNode.Text.ToString(), "0");
               YearSelect2.Value = String.Format(Date_Prefix2,Year.SelectedNode.NextNode.NextNode.NextNode.Text.ToString(), "1");
                member.Value = DataProvider.GetQueryText("member");

                CalculationGrid();

                dt.Rows[0][dt.Columns.Count - 4] = (System.Decimal)(SocMassa) * SumSoc + (EconomMassa) * SumEconom; ;//(System.Decimal)(dtRes.Rows[Social_index][6]) * SumSoc + (System.Decimal)(dtRes.Rows[Econom_index][6]) * SumEconom;
                dt.Rows[1][dt.Columns.Count - 4] = SumSoc;
                dt.Rows[2][dt.Columns.Count - 4] = SumEconom;

                YearSelect.Value = String.Format(Date_Prefix, Year.SelectedNode.NextNode.NextNode.NextNode.NextNode.Text.ToString(), "0");
               YearSelect2.Value = String.Format(Date_Prefix2,Year.SelectedNode.NextNode.NextNode.NextNode.NextNode.Text.ToString(), "1");
                member.Value = DataProvider.GetQueryText("member");

                CalculationGrid();
                

                dt.Rows[0][dt.Columns.Count - 5] = (System.Decimal)(SocMassa) * SumSoc + (EconomMassa) * SumEconom; ;//(System.Decimal)(dtRes.Rows[Social_index][6]) * SumSoc + (System.Decimal)(dtRes.Rows[Econom_index][6]) * SumEconom;
                dt.Rows[1][dt.Columns.Count - 5] = SumSoc;
                dt.Rows[2][dt.Columns.Count - 5] = SumEconom;


            }
            catch { }

            System.Double max = 0;
            
            for (int i = 0;i<dt.Rows.Count;i++)
                for (int j = 1; j < dt.Columns.Count; j++)
                {
                    try
                    {
                        max = (System.Double)((System.Decimal)(max) > (System.Decimal)(dt.Rows[i][j]) ? (System.Decimal)max : (System.Decimal)(dt.Rows[i][j]));
                    }
                    catch { }
                }
            if (max < 1.1)
            {
                max = 1;
            }
            C1.Axis.Y.RangeMax = max;

            C1.DataSource = dt.Columns.Count<3?null:dt;            
        }

        protected void C1_InvalidDataReceived(object sender, Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventArgs e)
        {
            CRHelper.UltraChartInvalidDataReceived(sender, e);
        }

        protected void G_InitializeRow(object sender, RowEventArgs e)
        {
            for (int i = 2; i < e.Row.Cells.Count; i++)
            {
                //if ((string.IsNullOrEmpty(e.Row.Cells[i].Text))||((System.Decimal) e.Row.Cells[i].Value ==0))
                if ((string.IsNullOrEmpty(e.Row.Cells[i].Text)) || (e.Row.Cells[i].Value.ToString() == "0") || (System.Decimal.Parse(e.Row.Cells[i].Value.ToString())==0))
                {
                    e.Row.Cells[i].Text = "-";
                }
            }
        }

        protected void C1_FillSceneGraph(object sender, Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e)
        {
            IAdvanceAxis xAxis = (IAdvanceAxis)e.Grid["X"];
            IAdvanceAxis yAxis = (IAdvanceAxis)e.Grid["Y"];

            if (xAxis == null || yAxis == null)
                return;

            int xMin = (int)xAxis.MapMinimum;
            int xMax = (int)xAxis.MapMaximum;
            //if (double.TryParse(dtChartAverage.Rows[0][0].ToString(), out urfoAverage))
            {
                int fmY = (int)yAxis.Map(1);
                Line line = new Line();
                line.lineStyle.DrawStyle = Infragistics.UltraChart.Shared.Styles.LineDrawStyle.Dot;//LineDrawStyle.Dot;
                line.PE.Stroke = Color.DarkGray;
                line.PE.StrokeWidth = 2;
                line.p1 = new Point(xMin, fmY);
                line.p2 = new Point(xMax, fmY);
                e.SceneGraph.Add(line);

                Text text = new Text();
                text.labelStyle.Font = new System.Drawing.Font("Verdana", 8,FontStyle.Bold);
                text.PE.Fill = Color.Black;
                text.bounds = new Rectangle(xMin + 3, fmY - 14, 780, 15);
                text.SetTextString("Уровень стабильности");
                e.SceneGraph.Add(text);
            }
        }

    }
}
