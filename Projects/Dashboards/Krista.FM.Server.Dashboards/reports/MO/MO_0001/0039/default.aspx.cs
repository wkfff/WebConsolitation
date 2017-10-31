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
using Infragistics.Documents.Excel;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
namespace Krista.FM.Server.Dashboards.reports.PMO_0001_0009
{
    public partial class Default : CustomReportPage
    {
        

        #region Разбор SQL
        public DataTable getDataSet(string[] name, string SQL)
        {
            DataTable DS = new DataTable();
            try
            {
                CellSet val;
                string s = DataProvider.GetQueryText(SQL);
                val = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(s);
                for (int i = 0; i < name.Length; i++)
                {
                    DS.Columns.Add(name[i]);
                };

                foreach (Position pos in val.Axes[1].Positions)
                {   // создание списка значений для строки UltraWebGrid
                    object[] values = {
                        val.Axes[1].Positions[pos.Ordinal].Members[0].Caption,
                        pos.Members[0].MemberProperties[0].Value.ToString(),
                        val.Cells[0, pos.Ordinal].FormattedValue
                    };
                    // заполнение строки данными
                    DS.Rows.Add(values);
                }
/*                
                for (int i = 0; i <= (val.Cells.Count) / (name.Length - 1) - 1; i++)
                {
                    object[] ob = new object[name.Length];
                    ob[0] = val.Axes[1].Positions[i].Members[0].Caption;
                    ob[1] = val.Cells[0, i].Value;
                    ob[2] = String.Format(float.Parse(val.Cells[1, i].FormattedValue).ToString(), "#.##");
                    //ob[2] = val.Cells[1, i].FormattedValue;
/*
                    for (int j = 1; j < name.Length; j++)
                    {
                        ob[j] = val.Cells[j - 1, i].Value;
                    }
*/
//                    DS.Rows.Add(ob);
//                }
            }
            catch
            {
                DS = null;
            };
            return DS;
        }
        #endregion
        #region ForChart
        public DataTable GetDSForChart(string sql)
        {
            DataTable dt = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText(sql), "a", dt);
            return dt;
        }

        #endregion



        private CustomParam current_region { get { return (UserParams.CustomParam("current_region")); } }
        private CustomParam current_year { get { return (UserParams.CustomParam("current_year")); } }
        private CustomParam last_year ;//{ get { return (UserParams.CustomParam("last_year")); } }
        private CustomParam last_year5{ get { return (UserParams.CustomParam("last_year5")); } }
        private CustomParam Top_Param1 { get { return (UserParams.CustomParam("Top_Param1")); } }
        private CustomParam Top_Param2 { get { return (UserParams.CustomParam("Top_Param2")); } }
        private CustomParam marks;
        private CustomParam chart2_mark;
        private CellSet CS;
        //private int Width { get { return (int)Session["width_size"]; } }
        private Int32 screen_width { get { return (int)Session["width_size"]; } }
        //int screen_width = 1024;
        // параметр запроса для региона
        private CustomParam baseRegion { get { return (UserParams.CustomParam("baseRegion")); } }  

        private string[] NamesTable = {"Показатель","Ед. Изм.","Значение" };
 

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


        
        private String GLD()
        {
            try
            {
                CellSet cs = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(DataProvider.GetQueryText("lastdate"));
                return cs.Axes[1].Positions[0].Members[0].Caption.ToString();
            }
            catch 
            {
                return null;
            }
        }

        public void SetChartRange()
        {
            double max;
            CS = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(DataProvider.GetQueryText("chart2"));
            max=(double)CS.Cells[0,0].Value;

            UltraChart2.Axis.Y.RangeMax = (int)max * 1.5;
            UltraChart2.Axis.Y.RangeMin = 0;
        }

        private void IniCustomize(LayoutEventArgs e)
        {
            e.Layout.CellClickActionDefault = CellClickAction.RowSelect;
            e.Layout.HeaderTitleModeDefault = CellTitleMode.Always;
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = 1 == 1;
            e.Layout.RowSelectorsDefault = RowSelectors.Yes;
            e.Layout.GroupByBox.Hidden = 1 == 1;
            e.Layout.NoDataMessage = "Нет данных";
            e.Layout.NoDataMessage = "Нет данных";
            e.Layout.Bands[0].Columns[0].Width = (int)(screen_width / 3);
            e.Layout.Bands[0].Columns[1].Width = (int)(screen_width / 10);
        }

        private void Ini_Chart(Infragistics.WebUI.UltraWebChart.UltraChart e)
        {
            e.DoughnutChart3D.Labels.FormatString = " <DATA_VALUE:#> ( <PERCENT_VALUE:#0.00>%)";
            e.Border.Color = Color.White;
            e.Legend.BorderColor = Color.White;
            e.Width = (int) (screen_width * 0.95);
        }
        public static void setChartErrorFont(Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventArgs e)
        {
            e.Text = "Нет данных";

            e.LabelStyle.FontColor = Color.LightGray;
            e.LabelStyle.VerticalAlign = StringAlignment.Center;
            e.LabelStyle.HorizontalAlign = StringAlignment.Center;
            e.LabelStyle.Font = new Font("Verdana", 30);
        }


        string BN="IE";
        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);
            System.Web.HttpBrowserCapabilities myBrowserCaps = Request.Browser;
            BN = ((System.Web.Configuration.HttpCapabilitiesBase)myBrowserCaps).Browser.ToUpper();
            last_year = UserParams.CustomParam("last_year");
            marks = UserParams.CustomParam("marks");
            chart2_mark = UserParams.CustomParam("chart2_mark");
            if (BN == "IE")
            {
                Label1.Height = CRHelper.GetChartHeight(45);
            }
            else
            {
                if (BN == "FIREFOX")
                {
                    Label1.Height = CRHelper.GetChartHeight(44);
                }
                else
                {
                    Label1.Height = CRHelper.GetChartHeight(46);
                }
            }


        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            current_region.Value = RegionSettingsHelper.Instance.RegionBaseDimension;
            string s = UserComboBox.getLastBlock(GLD());
            last_year.Value =s;
           
            if (!Page.IsPostBack)
            {
                marks = SetMarks(marks, Getmarks("grid_mark_"), true);
                chart2_mark.Value = RegionSettingsHelper.Instance.GetPropertyValue("chart2_mark_1");
                WebAsyncRefreshPanel1.AddLinkedRequestTrigger(UltraWebGrid1);
                WebAsyncRefreshPanel1.AddRefreshTarget(Label1);
                WebAsyncRefreshPanel1.AddRefreshTarget(UltraChart1);
                UltraWebGrid1.Width = (int)((screen_width - 55) * 0.4);
                UltraChart1.Width = (int)((screen_width - 55) * 0.6);
                if (BN == "FIREFOX")
                {
                    UltraWebGrid1.Height = 323;
                    UltraChart1.Height = 300;
                }
                else
                {
                    UltraWebGrid1.Height = 293;
                    UltraChart1.Height = 300;
                }

                UltraWebGrid1.BorderColor = Color.White;
                UltraWebGrid1.DataBind();
                UltraWebGrid1.Rows[0].Activated = 1 == 1;
                UltraWebGrid1.Rows[0].Activate();
                UltraWebGrid1.Rows[0].Selected = 1 == 1;
            }

            if (!Page.IsPostBack)
            {
                marks.Value =String.Format(RegionSettingsHelper.Instance.GetPropertyValue("chart1_mark"),UltraWebGrid1.Rows[0].Cells[0].Text);
                Label1.Text = string.Format(RegionSettingsHelper.Instance.GetPropertyValue("dynamicCharttitle"), UltraWebGrid1.Rows[0].Cells[0].Text, UltraWebGrid1.Rows[0].Cells[1].Text.ToLower());
                UltraChart1.Legend.Visible = 1 == 2;
            }

            Ini_Chart(UltraChart2);
            Label5.Text = string.Format(RegionSettingsHelper.Instance.GetPropertyValue("gridtitle"), last_year.Value, UserComboBox.getLastBlock(current_region.Value));
            Label3.Text = string.Format(RegionSettingsHelper.Instance.GetPropertyValue("bottomChartTitle"), last_year.Value, UserComboBox.getLastBlock(current_region.Value));//"Уровень заболеваемости населения, человек на 1000 человек населения";
            Label2.Text = string.Format(RegionSettingsHelper.Instance.GetPropertyValue("pagetitle"), last_year.Value, UserComboBox.getLastBlock(current_region.Value));
            Page.Title = Label2.Text;
            Label4.Text = RegionSettingsHelper.Instance.GetPropertyValue("pageSubTitle");
            {
                SetChartRange();
                UltraChart1.DataBind();
                UltraChart1.Tooltips.FormatString = "<b><DATA_VALUE:### ##0.##></b>, " + UltraWebGrid1.Rows[0].Cells[1].Text.ToLower();

                UltraChart2.DataBind();
                UltraChart2.Tooltips.FormatString = "<ITEM_LABEL>, <b><DATA_VALUE:### ##0.##></b>, " + RegionSettingsHelper.Instance.GetPropertyValue("chart2_tooltip");
            }
            UltraWebGrid1.DisplayLayout.CellClickActionDefault = CellClickAction.RowSelect;



        }

        protected void UltraWebGrid1_DataBinding(object sender, EventArgs e)
        {
                // Установка параметра последней актуальной даты
                DataTable dt = new DataTable();
                string query = DataProvider.GetQueryText("grid");
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Показатель", dt);
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if ((string.IsNullOrEmpty(dt.Rows[i].ItemArray[2].ToString())) || (dt.Rows[i].ItemArray[2].ToString()=="0"))
                    { dt.Rows[i].Delete(); i--; }
                }


                UltraWebGrid1.DataSource = dt;
        }

        protected void UltraChart1_DataBinding(object sender, EventArgs e)
        {
          
                DataTable dt = new DataTable();
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("chart1_danamic"), "a", dt);

                float min, max;
                min = max = float.Parse(dt.Rows[0].ItemArray[1].ToString());
                for (int i = 1; i < dt.Rows[0].ItemArray.Length; i++)
                {
                    if (float.Parse(dt.Rows[0].ItemArray[i].ToString()) < min)
                    {
                        min = float.Parse(dt.Rows[0].ItemArray[i].ToString());
                    }
                    if (float.Parse(dt.Rows[0].ItemArray[i].ToString()) > max)
                    {
                        max = float.Parse(dt.Rows[0].ItemArray[i].ToString());
                    }
                }
                if (max - min < 10 & max - min >= 1)
                {
                    UltraChart1.Axis.Y.TickmarkPercentage = (int)(100 / (max - min));
                    if (max - min == 1)
                    {
                        UltraChart1.Axis.Y.TickmarkStyle = Infragistics.UltraChart.Shared.Styles.AxisTickStyle.Percentage;
                    }
                    else
                    {
                        UltraChart1.Axis.Y.TickmarkStyle = Infragistics.UltraChart.Shared.Styles.AxisTickStyle.Smart;
                    }
                }
                else
                {
                    if (max == min)
                    {
                        UltraChart1.Axis.Y.TickmarkPercentage = 100;
                        UltraChart1.Axis.Y.TickmarkStyle = Infragistics.UltraChart.Shared.Styles.AxisTickStyle.Percentage;
                    }
                    else
                    {
                        UltraChart1.Axis.Y.TickmarkPercentage = 10;
                        UltraChart1.Axis.Y.TickmarkStyle = Infragistics.UltraChart.Shared.Styles.AxisTickStyle.Smart;
                    }
                }
                UltraChart1.DataSource = dt.DefaultView;
       
        }

        protected void UltraWebGrid1_InitializeLayout(object sender, LayoutEventArgs e)
        {

            double tempWidth = UltraWebGrid1.Width.Value - 28;
            UltraWebGrid1.DisplayLayout.RowSelectorStyleDefault.Width = 20 - 2;
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(CustomReportConst.minScreenWidth*0.19);
            e.Layout.Bands[0].Columns[1].Width = CRHelper.GetColumnWidth(CustomReportConst.minScreenWidth * 0.07);
            e.Layout.Bands[0].Columns[2].Width = CRHelper.GetColumnWidth(CustomReportConst.minScreenWidth * 0.07);
            e.Layout.Bands[0].Columns[1].Header.Style.Wrap = true;
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[1].CellStyle.Wrap = true;

            e.Layout.NoDataMessage = "Нет данных";
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[2], "### ##0.00");
        }

        protected void UltraChart2_DataBinding(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("chart2"), "chart2", dt);
            double max = 0, min = 0;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                for (int j = 1; j < dt.Rows[0].ItemArray.Length; j++)
                {
                    if (double.Parse(dt.Rows[i].ItemArray[j].ToString()) > max)
                    {
                        max = double.Parse(dt.Rows[i].ItemArray[j].ToString());
                    }
                    if (double.Parse(dt.Rows[i].ItemArray[j].ToString()) < min)
                    {
                        min = double.Parse(dt.Rows[i].ItemArray[j].ToString());
                    }
                }
            }
            UltraChart2.Axis.Y.RangeMax = max * 1.1;
            UltraChart2.Axis.Y.RangeMin = min *0.9;
            UltraChart2.Axis.Y.RangeType = Infragistics.UltraChart.Shared.Styles.AxisRangeType.Custom;
            UltraChart2.DataSource = dt;
        }

        protected void INI_LAbel(Label lab)
        {

            lab.Width = screen_width - 100;
            lab.Font.Name = "Arial";
            lab.Font.Size = FontUnit.Small;
            lab.Font.Bold = 1 == 1;

        }
        
        
        protected void UltraWebGrid1_ActiveRowChange(object sender, RowEventArgs e)
        {

            int i;
            ArrayList itemGrid = Getmarks("grid_mark_");
           
            for (i = 0; itemGrid.Count > i; i++)
            {
                if (e.Row.Cells[0].Text == UserComboBox.getLastBlock(itemGrid[i].ToString()))
                {
                    break;
                }

            }
             marks.Value = itemGrid[i].ToString();
            Label1.Text = string.Format(
                RegionSettingsHelper.Instance.GetPropertyValue("dynamicCharttitle"), e.Row.Cells[0].Text,e.Row.Cells[1].Text.ToLower());
            UltraChart1.DataBind();
            UltraChart1.Tooltips.FormatString = "<b><DATA_VALUE:### ##0.##></b>, " + e.Row.Cells[1].Text.ToLower();
        }
       

        protected void UltraChart1_InvalidDataReceived(object sender, Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventArgs e)
        {
            setChartErrorFont(e);
            
        }

        protected void UltraChart2_InvalidDataReceived(object sender, Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventArgs e)
        {
            setChartErrorFont(e);
        }

        protected void UltraChart1_FillSceneGraph(object sender, Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e)
        {
            int xOct = 0;
            int xNov = 0;
            Text decText = null;
            int year = int.Parse(last_year.Value);
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

        protected void UltraWebGrid1_ActiveCellChange(object sender, CellEventArgs e)
        {
            e.Cell.Row.Activated = true;
            e.Cell.Row.Selected = true;
            e.Cell.Row.Activate();
        }



    }
}
