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
using Infragistics.UltraChart.Core.Primitives;

using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Common;
using Infragistics.UltraChart.Core.Layers;

using Infragistics.UltraChart.Core;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Styles;
/**
 *  Труд и заработная плата  
 */
namespace Krista.FM.Server.Dashboards.reports.PMO_0001_0004_010
{
    public partial class Default : CustomReportPage
    {
        
            // параметр для последней актуальной даты
            private CustomParam last_year { get { return (UserParams.CustomParam("last_year")); } }
            private CustomParam marks;
            private int screen_width { get { return (int)Session["width_size"]; } }
            private CustomParam current_region;//{ get { return (UserParams.CustomParam("current_region")); } }
            private CustomParam DataY { get { return (UserParams.CustomParam("DataY")); } }
            // параметр запроса для региона
            private CustomParam baseRegion { get { return (UserParams.CustomParam("baseRegion")); } }
            private CustomParam norefresh;
            private CustomParam norefresh2;
        string BN = "IE";
        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            
            base.Page_PreLoad(sender, e);
            System.Web.HttpBrowserCapabilities myBrowserCaps = Request.Browser;
            BN = ((System.Web.Configuration.HttpCapabilitiesBase)myBrowserCaps).Browser.ToUpper();
            current_region = UserParams.CustomParam("current_region");
            marks = UserParams.CustomParam("marks");
            PopupInformer1.HelpPageUrl = RegionSettingsHelper.Instance.GetPropertyValue("nameHelpPage");
            WebAsyncRefreshPanel1.AddLinkedRequestTrigger(MainT);
            /*RefreshPanel.AddLinkedRequestTrigger(WebAsyncRefreshPanel1);
            RefreshPanel.AddRefreshTarget(UltraChart3);
            RefreshPanel.AddRefreshTarget(ChartBottomL);
            RefreshPanel.AddRefreshTarget(Label2);
            RefreshPanel.AddRefreshTarget(Label3);
            RefreshPanel.AddRefreshTarget(Panel1);*/
        }

            protected override void Page_Load(object sender, EventArgs e)
            {
                TestLabel.Text = screen_width.ToString();
                //try
                {
                    //MainT.DataBind();
                    //ChartT.DataBind();
                    //ChartBottomL.DataBind();
                    //UltraChart3.DataBind();
                    MainT.Width = (int)((screen_width ) * 0.38-20);
                    ChartT.Width = (int)((screen_width ) * 0.59-13);
                    ChartBottomL.Width = (int)((screen_width ) * 0.49-20);
                    UltraChart3.Width = (int)((screen_width ) * 0.49-20);
                    if ((BN == "IE") || (BN == "APPLEMAC-SAFARI"))
                    {
                        MainT.Height = CRHelper.GetChartHeight(277);
                        ChartT.Height = CRHelper.GetChartHeight(300);
                    }
                    else
                    {
                        MainT.Height = CRHelper.GetChartHeight(323);
                        ChartT.Height = CRHelper.GetChartHeight(300);
                    }

                    TestLabel.Text += " | " + MainT.Width + " | " + ChartT.Width + " | " + ChartBottomL.Width + " | " + UltraChart3.Width;
                    norefresh = UserParams.CustomParam("r");
                    norefresh2 = UserParams.CustomParam("r2");

                   //if (!Page.IsPostBack)
                    //{   
                        // опрерации которые должны выполняться при только первой загрузке страницы
                      //  RegionSettingsHelper.Instance.SetWorkingRegion(RegionSettings.Instance.Id);
                        baseRegion.Value = RegionSettingsHelper.Instance.RegionBaseDimension;

                        

                        UserParams.PeriodYearQuater.Value = ELV(last_year.Value);

                        current_region.Value = RegionSettingsHelper.Instance.RegionBaseDimension;


                        Label5.Text = RegionSettingsHelper.Instance.GetPropertyValue("pageSubTitle");
                        last_year.Value = getLastDate();
                        LabChTop.Text = RegionSettingsHelper.Instance.GetPropertyValue("Chart1_title1");
                        //Label3.Text = "Структура платных услуг по видам услуг";
                        //Label2.Text = "Структура платных услуг по формам собствености";
                        //UltraChart3.Visible = 1 == 1;
                        //ChartT.Visible = 1 == 1;
                        //ChartBottomL.Visible = 2 == 2;
                        UserParams.KDGroup.Value = RegionSettingsHelper.Instance.GetPropertyValue("kdGroup1");//"Объем платных услуг в действующих ценах на 1-го жителя";
                        //current_region.Value = "sdasd";
                        MainT.DataBind();
                        ChartT.DataBind();
                        ChartT.Tooltips.FormatString = RegionSettingsHelper.Instance.GetPropertyValue("chartT_tooltips1");
                        ChartBottomL.DataBind();
                        ChartBottomL.Tooltips.FormatString = RegionSettingsHelper.Instance.GetPropertyValue("ChartBottomL_tooltips");
                        UltraChart3.DataBind();
                        UltraChart3.Tooltips.FormatString = RegionSettingsHelper.Instance.GetPropertyValue("UltraChart3_tooltips");
                        try
                        {
                            Label4.Text = string.Format(RegionSettingsHelper.Instance.GetPropertyValue("Page_title"), ELV(last_year.Value), ELV(current_region.Value) );//"Информация о показателях платных услугах по данным на  " + ELV(last_year.Value) + " год (" + RegionSettingsHelper.Instance.Name + ")";
                            Page.Title = Label4.Text;
                            Label2.Text = string.Format(RegionSettingsHelper.Instance.GetPropertyValue("Chart2_title"), ELV(last_year.Value), RegionSettingsHelper.Instance.Name);
                            Label3.Text = string.Format(RegionSettingsHelper.Instance.GetPropertyValue("Chart3_title"), ELV(last_year.Value), RegionSettingsHelper.Instance.Name);
                            Label1.Text = String.Format(RegionSettingsHelper.Instance.GetPropertyValue("Grid_title"), ELV(last_year.Value), RegionSettingsHelper.Instance.Name);
                        }
                        catch { }
                        MainT.Rows[MainT.Rows.Count - 1].Cells[1].Activate();
                        MainT.Rows[MainT.Rows.Count - 1].Cells[1].Activated =1==1;
                        MainT.Rows[MainT.Rows.Count - 1].Cells[1].Selected = 1 == 1;
                        MainT.Rows[MainT.Rows.Count - 1].Cells[1].Selected = 1 == 1;

                   // }
                    if (MainT.Rows[0].Cells[0].Text == null)
                    {
                        int a = 0; a = 1 / a;
                    }
                    if ((UltraChart3.DataSource == null) && (ChartBottomL.DataSource == null))
                    {
                        Panel1.Visible = false;
                    }
                    else
                    {
                        Panel1.Visible = true;
                    }
                }
              //  catch
                {
                    //LabChTop.Text = "Нет данынх";
                    //Label2.Text = "Нет данынх";
                    //Label3.Text = "Нет данынх";
                    //UltraChart3.Visible = 1 == 2;
                    //ChartT.Visible = 1 == 3;
                    //ChartBottomL.Visible = 2 == 21;
                }
                    norefresh2.Value = "Yes";
               


 
                MainT.DisplayLayout.CellClickActionDefault = CellClickAction.CellSelect;
            }

            // --------------------------------------------------------------------

            /** <summary>
             *  Метод получения последней актуальной даты 
             *  </summary>
             */
            private String getLastDate()
            {
                try
                {
                    
                    CellSet cs = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(DataProvider.GetQueryText("LD_"));
                    return cs.Axes[1].Positions[0].Members[0].ToString();
                }
                catch (Exception e)
                {
                    return null;
                }
            }

            DataTable MT;
            protected void MainT_DataBinding(object sender, EventArgs e)
            {
                MT = new DataTable();
                marks = ForMarks.SetMarks(marks, ForMarks.Getmarks("grid1_mark_"), true);
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("STT"), "Год", MT);
                MainT.DataSource = MT;
                
            }
            DataTable TC;
            protected void UltraChart2_DataBinding(object sender, EventArgs e)
            {
                TC = new DataTable();
                marks.Value = RegionSettingsHelper.Instance.GetPropertyValue("chart1_mark_1"); //ForMarks.SetMarks(marks, ForMarks.Getmarks("chart1_mark_1"), true);
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("CT"), "s", TC);
                ChartT.DataSource = TC;
            }
            DataTable BLC;
        DataTable GlobalDtRes;
            protected void ChartBottomL_DataBinding(object sender, EventArgs e)
            {
                try
                {
                    marks.Value = RegionSettingsHelper.Instance.GetPropertyValue("chart2_mark_1");//ForMarks.SetMarks(marks, ForMarks.Getmarks("chart2_mark_"), true);
                    string q = RegionSettingsHelper.Instance.GetPropertyValue("chart2_query");
                    DataTable dt = new DataTable();
                    DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText(q), "dfdf", dt);
                    //DataTable dt = GetDSForChart(q);
                    if (dt.Rows.Count > 0)
                    {
                        BLC = new DataTable();
                        // dt.Rows.Remove(dt.Rows[0]);
                        System.Decimal sum = 0;
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            sum += Decimal.Parse(dt.Rows[i].ItemArray[1].ToString());
                        }
                        DataTable resDt = new DataTable();
                        for (int i = 0; i < dt.Columns.Count; i++)
                        {
                            resDt.Columns.Add(dt.Columns[i].Caption, dt.Columns[i].DataType);
                        }
                        string othterString = "";
                        System.Decimal otherSum = 0;
                        object[] o;
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            if (((System.Decimal)(dt.Rows[i].ItemArray[1]) / sum) > (System.Decimal)(0.01))
                            {

                                o = new object[2];
                                string res = dt.Rows[i].ItemArray[0].ToString().Remove(1) + dt.Rows[i].ItemArray[0].ToString().Remove(0, 1).ToLower();

                                for (int j = 1; j < res.Length / 30; j++)
                                {
                                    int k;
                                    for (k = j * 30; k < res.Length; k++)
                                    {
                                        if (res[k] == ' ')
                                        {
                                            break;
                                        }
                                    }

                                    res = !(k == res.Length) ? res.Insert(k, "<br>") : res;
                                }

                                o[0] = res;
                                o[1] = dt.Rows[i].ItemArray[1];
                                resDt.Rows.Add(o);
                            }
                            else
                            {

                                string newO = dt.Rows[i].ItemArray[0].ToString().Remove(1).ToUpper() + dt.Rows[i].ItemArray[0].ToString().Remove(0, 1).ToLower()
                                    + ", <b>" + dt.Rows[i].ItemArray[1].ToString() + "</b>, тысяча рублей (" + Math.Round((System.Decimal)(dt.Rows[i].ItemArray[1]) * 100 / sum, 2).ToString() + "%)" + "<br/>";
                                for (int j = 1; j < newO.Length / 50; j++)
                                {
                                    int k;
                                    for (k = j * 50; k < newO.Length; k++)
                                    {
                                        if (newO[k] == ' ')
                                        {
                                            break;
                                        }
                                    }

                                    newO = !(k == newO.Length) ? newO.Insert(k, "<br>") : newO;
                                }

                                othterString += newO;
                                otherSum += Decimal.Parse(dt.Rows[i].ItemArray[1].ToString());
                            }
                        }
                        o = new object[2];
                        o[0] = othterString + " <b>Всего прочих</b>";
                        o[1] = otherSum;
                        if (otherSum > 0)
                        {
                            resDt.Rows.Add(o);
                        }
                        GlobalDtRes = resDt;

                        BLC = resDt;
                        ChartBottomL.DataSource = BLC;
                    }
                    else
                    {
                        ChartBottomL.DataSource = null;
                    }
                }
                catch 
                {
                    ChartBottomL.DataSource = null;
                }
                


            }

        public DataTable GetDSForChart(string sql)
        {
            DataTable dt = new DataTable();
            string s = DataProvider.GetQueryText(sql);
            DataProvidersFactory.PrimaryMASDataProvider.PopulateDataTableForChart(DataProvidersFactory.PrimaryMASDataProvider.GetCellset(s), dt, "sadad");
            return dt;
        }
            DataTable BRC;
            protected void UltraChart3_DataBinding(object sender, EventArgs e)
            {
                try
                {
                    marks.Value = RegionSettingsHelper.Instance.GetPropertyValue("chart3_mark_1");
                    // marks = ForMarks.SetMarks(marks, ForMarks.Getmarks("chart3_mark_"), true);
                    string q = RegionSettingsHelper.Instance.GetPropertyValue("chart3_query");
                    DataTable dt = new DataTable();
                    DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText(q), "dfdf", dt);
                    //DataTable dt = GetDSForChart(q);
                    if (dt.Rows.Count > 0)
                    {
                        BRC = new DataTable();
                        // dt.Rows.Remove(dt.Rows[0]);
                        System.Decimal sum = 0;
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            sum += Decimal.Parse(dt.Rows[i].ItemArray[1].ToString());
                        }
                        DataTable resDt = new DataTable();
                        for (int i = 0; i < dt.Columns.Count; i++)
                        {
                            resDt.Columns.Add(dt.Columns[i].Caption, dt.Columns[i].DataType);
                        }
                        string othterString = "";
                        System.Decimal otherSum = 0;
                        object[] o;
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            if (((System.Decimal)(dt.Rows[i].ItemArray[1]) / sum) > (System.Decimal)(0.01))
                            {

                                o = new object[2];
                                string res = dt.Rows[i].ItemArray[0].ToString().Remove(1) + dt.Rows[i].ItemArray[0].ToString().Remove(0, 1).ToLower();

                                for (int j = 1; j < res.Length / 30; j++)
                                {
                                    int k;
                                    for (k = j * 30; k < res.Length; k++)
                                    {
                                        if (res[k] == ' ')
                                        {
                                            break;
                                        }
                                    }

                                    res = !(k == res.Length) ? res.Insert(k, "<br>") : res;
                                }

                                o[0] = res;
                                o[1] = dt.Rows[i].ItemArray[1];
                                resDt.Rows.Add(o);
                            }
                            else
                            {

                                string newO = dt.Rows[i].ItemArray[0].ToString().Remove(1).ToUpper() + dt.Rows[i].ItemArray[0].ToString().Remove(0, 1).ToLower()
                                    + ", <b>" + dt.Rows[i].ItemArray[1].ToString() + "</b>, рубль (" + Math.Round((System.Decimal)(dt.Rows[i].ItemArray[1]) * 100 / sum, 2).ToString() + "%)" + "<br/>";
                                for (int j = 1; j < newO.Length / 50; j++)
                                {
                                    int k;
                                    for (k = j * 50; k < newO.Length; k++)
                                    {
                                        if (newO[k] == ' ')
                                        {
                                            break;
                                        }
                                    }

                                    newO = !(k == newO.Length) ? newO.Insert(k, "<br>") : newO;
                                }

                                othterString += newO;
                                otherSum += Decimal.Parse(dt.Rows[i].ItemArray[1].ToString());
                            }
                        }
                        o = new object[2];
                        o[0] = othterString + " <b>Всего прочих</b>";
                        o[1] = otherSum;
                        if (otherSum > 0)
                        {
                            resDt.Rows.Add(o);
                        }
                        GlobalDtRes = resDt;

                        BRC = resDt;
                        UltraChart3.DataSource = BRC;
                    }
                    else
                    {
                        UltraChart3.DataSource = null;
                    }
                }
                catch 
                {
                    UltraChart3.DataSource = null;
                }
               

            }
            bool a = 1 == 1;
            protected void MainT_InitializeLayout(object sender, LayoutEventArgs e)
            {
                double tempWidth = MainT.Width.Value - 14;
                MainT.DisplayLayout.RowSelectorStyleDefault.Width = 20 - 2;
                MainT.Columns[0].Width = (int)((tempWidth - 20) * 0.1) - 5;
                MainT.Columns[1].Width = (int)((tempWidth - 20) * 0.45) - 5;
                MainT.Columns[2].Width = (int)((tempWidth - 20) * 0.45) - 5;

                TestLabel.Text += " * " + MainT.DisplayLayout.RowSelectorStyleDefault.Width + " | " + MainT.Columns[0].Width + " | " + MainT.Columns[1].Width + " | " + MainT.Columns[2].Width + " *";

                e.Layout.Bands[0].Columns[1].Header.Caption = RegionSettingsHelper.Instance.GetPropertyValue("TitleGridFirstColumn");
                e.Layout.Bands[0].Columns[2].Header.Caption = RegionSettingsHelper.Instance.GetPropertyValue("TitleGridSecondColumn");
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[1], "### ### ##0.00");
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[2], "### ### ##0.00");
                MainT.Columns[1].Header.Style.Wrap = 1 == 1;
                MainT.Columns[2].Header.Style.Wrap = 1 == 1;
                MainT.BorderColor = Color.White;
                ChartT.Border.Color = Color.White;
                ChartBottomL.Border.Color = Color.White;
                UltraChart3.Border.Color = Color.White;
                //### ##0
            }

            protected void MainT_DblClick(object sender, ClickEventArgs e)
            {
                e.Cancel = 1 == 1;
            }
            #region //получение низшей иерархии
            private String ELV(String s)
            {
                try
                {
                    int i = s.Length;
                    string res = "";
                    while (s[--i] != ']') ;
                    while (s[--i] != '[')
                    {
                        res = s[i] + res;
                    }
                    return res;
                }
                catch { return ""; }

            }
            #endregion


            protected void MainT_ActiveRowChange(object sender, RowEventArgs e)
            {
                try
                {
                    last_year.Value = "[Период].[Год Квартал Месяц].[Данные всех периодов].[" + e.Row.Cells[0].Text + "]";
                    Label2.Text = String.Format(RegionSettingsHelper.Instance.GetPropertyValue("Chart2_title"), e.Row.Cells[0].Text); //"Структура платных услуг по формам собствености на&nbsp;" + e.Row.Cells[0].Text + "&nbsp;год,&nbsp;%";
                    Label3.Text = String.Format(RegionSettingsHelper.Instance.GetPropertyValue("Chart3_title"), e.Row.Cells[0].Text);//"Структура платных услуг по видам услуг на&nbsp;" + e.Row.Cells[0].Text + "&nbsp;год,&nbsp;%";
                    ChartBottomL.DataBind();
                    UltraChart3.DataBind();
                    if ((UltraChart3.DataSource == null) && (ChartBottomL.DataSource == null))
                    {
                        Panel1.Visible = false;
                    }
                    else
                    {
                        Panel1.Visible = true;
                    }
                }
                catch { }
            }

            public static void setChartErrorFont(Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventArgs e)
            {
                e.Text = "В настоящий момент данные отсутствуют";//chart_error_message;
                e.LabelStyle.Font = new Font("Verdana", 20);
                e.LabelStyle.FontColor = Color.LightGray;
                e.LabelStyle.VerticalAlign = StringAlignment.Center;
                e.LabelStyle.HorizontalAlign = StringAlignment.Center;
            }

            protected void ChartBottomL_InvalidDataReceived(object sender, Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventArgs e)
            {
                setChartErrorFont(e);
            }

            protected void UltraChart3_InvalidDataReceived(object sender, Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventArgs e)
            {
                setChartErrorFont(e);
            }

            protected void ChartT_InvalidDataReceived(object sender, Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventArgs e)
            {
                setChartErrorFont(e);
            }

            protected void ChartT_FillSceneGraph(object sender, Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e)
            {
                int xOct = 0;
                int xNov = 0;
                Text decText = null;
                int year = int.Parse(UserComboBox.getLastBlock(last_year.Value));
                String year1 = (year - 1).ToString();
                String year2 = (year - 2).ToString();


                foreach (Primitive primitive in e.SceneGraph)
                {
                    if (primitive is Text)
                    {
                        Text text = primitive as Text;

                        if (year2 == text.GetTextString())
                        {
                            xOct = text.bounds.X;
                            continue;
                        }
                        if (year1 == text.GetTextString())
                        {
                            xNov = text.bounds.X;
                            decText = new Text();
                            decText.bounds = text.bounds;
                            decText.labelStyle = text.labelStyle;
                            continue;
                        }
                    }
                    if (decText != null)
                    {
                        decText.bounds.X = xNov + (xNov - xOct);
                        decText.SetTextString(year.ToString());
                        e.SceneGraph.Add(decText);
                        break;
                    }
                }
            }
        static public class ForMarks
        {

            public static ArrayList Getmarks(string prefix)
            {
                ArrayList AL = new ArrayList();
                string CurMarks = RegionSettingsHelper.Instance.GetPropertyValue(prefix + "1");
                int i = 2;
                while (!string.IsNullOrEmpty(CurMarks))
                {
                    AL.Add(CurMarks.ToString());

                    CurMarks = RegionSettingsHelper.Instance.GetPropertyValue(prefix + i.ToString());

                    i++;
                }

                return AL;
            }

            public static CustomParam SetMarks(CustomParam param, ArrayList AL, params bool[] clearParam)
            {
                if (clearParam.Length > 0 && clearParam[0]) { param.Value = ""; }
                int i;
                for (i = 0; i < AL.Count - 1; i++)
                {
                    param.Value += AL[i].ToString() + ",";
                }
                param.Value += AL[i].ToString();

                return param;
            }


        }

        protected void MainTManual_ActiveCellChange(int CellIndex)
        {
            try
            {
                if (CellIndex == 1)
                {
                    UserParams.KDGroup.Value = RegionSettingsHelper.Instance.GetPropertyValue("kdGroup1");//"Объем платных услуг в действующих ценах";
                    //LabChTop.Text = "<br>«" + UserParams.KDGroup.Value + ", тысяча&nbsp;рублей»";
                    LabChTop.Text = string.Format(RegionSettingsHelper.Instance.GetPropertyValue("Chart1_title1"), ELV(last_year.Value), RegionSettingsHelper.Instance.Name);
                    ChartT.DataBind();
                    ChartT.Tooltips.FormatString = RegionSettingsHelper.Instance.GetPropertyValue("chartT_tooltips1");
                }

                if (CellIndex == 2)
                {
                    UserParams.KDGroup.Value = RegionSettingsHelper.Instance.GetPropertyValue("kdGroup2");//"Объем платных услуг в действующих ценах на 1-го жителя";
                    //LabChTop.Text = "<br>«" + UserParams.KDGroup.Value + ", тысяча&nbsp;рублей»";
                    LabChTop.Text = RegionSettingsHelper.Instance.GetPropertyValue("Chart1_title2");
                    ChartT.DataBind();
                    ChartT.Tooltips.FormatString = RegionSettingsHelper.Instance.GetPropertyValue("chartT_tooltips2");
                }
            }
            catch
            { }

        }
        protected void MainT_Click(object sender, ClickEventArgs e)
        {
            try
            {
                int CellIndex = e.Column.Index;
                if (CellIndex > 0)
                {
                    MainTManual_ActiveCellChange(CellIndex);
                }
                else
                {
                    MainTManual_ActiveCellChange(1);
                }
            }
            catch { MainTManual_ActiveCellChange(1); }

        }

        protected void ChartBottomL_FillSceneGraph(object sender, Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e)
        {
            try
            {
                if (BLC.Rows.Count > 1)
                {
                    int i = 0;
                    for (i = 0; i < BLC.Rows.Count - 1; i++)
                    {

                        Infragistics.UltraChart.Core.Primitives.Text textLabel = new Infragistics.UltraChart.Core.Primitives.Text(new Rectangle(26, 10 + i * 19 - i + 2, 320, 8), BLC.Rows[i].ItemArray[0].ToString(), new LabelStyle(new Font("Verdana", (float)(7.8)), Color.Black, true, false, false, StringAlignment.Near, StringAlignment.Near, TextOrientation.Horizontal));
                        e.SceneGraph.Add(textLabel);


                    }
                    Infragistics.UltraChart.Core.Primitives.Text textLabel1 = new Infragistics.UltraChart.Core.Primitives.Text(new Rectangle(26, 10 + i * 19 - i + 2, 320, 8), "Прочие", new LabelStyle(new Font("Verdana", (float)(7.8)), Color.Black, true, false, false, StringAlignment.Near, StringAlignment.Near, TextOrientation.Horizontal));
                    e.SceneGraph.Add(textLabel1);
                }
            }
            catch { }


        }

        protected void UltraChart3_FillSceneGraph(object sender, Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e)
        {
            try
            {
                if (BRC.Rows.Count > 1)
                {
                    int i = 0;
                    for (i = 0; i < BRC.Rows.Count - 1; i++)
                    {

                        Infragistics.UltraChart.Core.Primitives.Text textLabel = new Infragistics.UltraChart.Core.Primitives.Text(new Rectangle(26, 10 + i * 19 - i + 2, 320, 8), BRC.Rows[i].ItemArray[0].ToString(), new LabelStyle(new Font("Verdana", (float)(7.8)), Color.Black, true, false, false, StringAlignment.Near, StringAlignment.Near, TextOrientation.Horizontal));
                        e.SceneGraph.Add(textLabel);

                    }
                    Infragistics.UltraChart.Core.Primitives.Text textLabel1 = new Infragistics.UltraChart.Core.Primitives.Text(new Rectangle(26, 10 + i * 19 - i + 2, 320, 8), "Прочие", new LabelStyle(new Font("Verdana", (float)(7.8)), Color.Black, true, false, false, StringAlignment.Near, StringAlignment.Near, TextOrientation.Horizontal));

                    e.SceneGraph.Add(textLabel1);
                }
            }
            catch { }
        }

        protected void MainT_ActiveCellChange(object sender, CellEventArgs e)
        {
            try
            {
                int CellIndex = e.Cell.Column.Index;
                if (CellIndex > 0)
                {
                    MainTManual_ActiveCellChange(CellIndex);
                }
                else
                {
                    MainTManual_ActiveCellChange(1);
                }
            }
            catch { MainTManual_ActiveCellChange(1); }
        }

        protected void ChartBottomL_ChartDataClicked(object sender, Infragistics.UltraChart.Shared.Events.ChartDataEventArgs e)
        {

        }
    }

    }


