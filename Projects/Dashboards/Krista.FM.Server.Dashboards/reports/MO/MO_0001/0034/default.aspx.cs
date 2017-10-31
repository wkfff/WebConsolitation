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

namespace Krista.FM.Server.Dashboards.reports.PMO_0001_0004
{
    public partial class Default : CustomReportPage
    {
        // параметр для последней актуальной даты
        private CustomParam last_year { get { return (UserParams.CustomParam("last_year")); } }

        private int screen_width { get { return (int)Session["width_size"]; } }

        private int screen_height { get { return (int)Session["height_size"]; } }

        private CustomParam current_region { get { return (UserParams.CustomParam("current_region")); } }

        // параметр запроса для региона
        private CustomParam baseRegion { get { return (UserParams.CustomParam("baseRegion")); } }


        private void GetChat(Infragistics.WebUI.UltraWebChart.UltraChart Chart,string TypeChart)
        {

            switch (TypeChart)
            {
                case "Area2d":
                    Chart.ChartType = Infragistics.UltraChart.Shared.Styles.ChartType.AreaChart;

                    break;
                case "Area3d":
                    Chart.ChartType = Infragistics.UltraChart.Shared.Styles.ChartType.AreaChart3D;

                    break;
                case "BoxChart":
                    Chart.ChartType = Infragistics.UltraChart.Shared.Styles.ChartType.BarChart;

                    break;
                case "Barchart":
                    Chart.ChartType = Infragistics.UltraChart.Shared.Styles.ChartType.BoxChart;

                    break;
                case "BubbleChart":
                    Chart.ChartType = Infragistics.UltraChart.Shared.Styles.ChartType.BubbleChart;

                    break;
                case "BubbleChart3D":
                    Chart.ChartType = Infragistics.UltraChart.Shared.Styles.ChartType.BubbleChart3D;

                    break;
                case "CandleChart":
                    Chart.ChartType = Infragistics.UltraChart.Shared.Styles.ChartType.CandleChart;

                    break;
                default:
                    Chart.ChartType = Infragistics.UltraChart.Shared.Styles.ChartType.BoxChart;
                    break;
            }





        }



        protected override void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!Page.IsPostBack)
                {
                    RegionSettingsHelper.Instance.SetWorkingRegion(RegionSettings.Instance.Id);
                    baseRegion.Value = RegionSettingsHelper.Instance.RegionBaseDimension;

                    WebAsyncRefreshPanel1.AddLinkedRequestTrigger(MainTableTop);
                    WebAsyncRefreshPanel2.AddLinkedRequestTrigger(MainTableBottom);
                    

                }

                #region Разметка страницы
                MainTableTop.Width = (int)((screen_width - 55)* 0.4);
                MainTableBottom.Width = (int)((screen_width - 55) * 0.4);
                UCT.Width = (int)((screen_width - 55) * 0.6);
                UCB.Width = (int)((screen_width - 55) * 0.6);

                #endregion
                current_region.Value = baseRegion.Value;

                last_year.Value = getLastDate();

                UserParams.PeriodYearQuater.Value = last_year.Value;

                MainTableTop.DataBind();
                // получаем выбранную строку
                UltraGridRow row = MainTableTop.Rows[0];
                // устанавливаем ее активной, если необходимо
                row.Activate();

                MainTableBottom.DataBind();
                // получаем выбранную строку
                row = MainTableBottom.Rows[0];
                // устанавливаем ее активной, если необходимо
                row.Activate();

                UserParams.KDGroup.Value = MainTableTop.Rows[0].Cells[0].Text;
                UCT.DataBind();
                UserParams.PeriodLastDate.Value = HT(MainTableBottom.Rows[0].Cells[0].Text);
                UserParams.KDGroup.Value = HT1(MainTableBottom.Rows[0].Cells[0].Text);
                UCB.DataBind();
                MainTableTop.Columns[0].CellStyle.Wrap = true;
                LabTopC.Text = "Динамика показателя «" + MainTableTop.Rows[0].Cells[0].Text + ", " + MainTableTop.Rows[0].Cells[2].Text.ToLower() + "»";
                LabBottomC.Text = "Динамика обеспеченности «" + MainTableBottom.Rows[0].Cells[0].Text + ", " + MainTableBottom.Rows[0].Cells[2].Text.ToLower() + "»";
                Label1.Text = "Информация о показателях торговли, общественного питания по данным на  " + ELV(last_year.Value) + " год (" + RegionSettingsHelper.Instance.Name + ")";
                 int a = 0; if (MainTableBottom.Rows[0].Cells[1].Text == null) { int i = (1 / a);}
            }
            catch
            {
                LabTopC.Text = "Нет данных";
                LabBottomC.Text = "Нет данных";
                UCT.Visible = 1 == 2;
                UCB.Visible = 1 == 2;
            }
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
                    CellSet cs = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(DataProvider.GetQueryText("LD"));
                    return cs.Axes[1].Positions[0].Members[0].ToString();
                }
                catch (Exception e)
                {
                    return null;
                }
            }

        #region MainTableTop_DataBinding
        DataTable MTT;
        protected void MainTableTop_DataBinding(object sender, EventArgs e)
        {
            MTT = new DataTable();
            string s = DataProvider.GetQueryText("CTT");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(s, "Показатель", MTT);
            MainTableTop.DataSource = MTT;
            }
        #endregion

        #region MainTableBottom_DataBinding

        DataTable MTB;
        protected void MainTableBottom_DataBinding(object sender, EventArgs e)
        {
            MTB = new DataTable();
            string s = DataProvider.GetQueryText("CTB");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(s, "Объект", MTB);
            MainTableBottom.DataSource = MTB;

/*
            MTB = new DataTable();
            CellSet val;
            val = SecondaryMASDataProvider.GetCellset(DataProvider.GetQueryText("CTB"));
            MTB.Columns.Add("Объект");
            MTB.Columns.Add("Значение");
            MTB.Columns.Add("Единица измерения");
            for (int i = 0; i <= (val.Cells.Count)/2-1; i++) {

                object[] o = { val.Axes[1].Positions[i].Members[0].Caption, val.Cells[0,i].Value, val.Cells[1,i].Value };            
            MTB.Rows.Add(o);}
            MainTableBottom.DataSource = MTB;
*/




            
        }
        #endregion
        DataTable CT;
        protected void UltraChart1_DataBinding(object sender, EventArgs e)
        {
            CT = new DataTable();
            string s = DataProvider.GetQueryText("CCT");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(s, "a", CT);
            UCT.DataSource = CT;

        }

        protected void MainTableTop_ActiveRowChange(object sender, RowEventArgs e)
        {
            try
            {
                UserParams.KDGroup.Value = e.Row.Cells[0].Text;
                UCT.DataBind();
                LabTopC.Text = "Динамика показателя «" + e.Row.Cells[0].Text + ", " + e.Row.Cells[2].Text.ToLower() + "»";
                int a = 0; if (e.Row.Cells[1].Text == null) { int i = (1 / a); }
            }
            catch
            {
                LabTopC.Text = "Нет данных";
                //LabBottomC.Text = "Нет данных";
                UCT.Visible = 1 == 2;
                //UCB.Visible = 1 == 2;
            }
        }


      string  HT1(string s)
        {
            string res = "";
          if (s=="Магазины"){res="Норматив по магазинам";}
          else
          if (s=="Хранилища"){res="Норматив по хранилищам";}
          else
          if (s=="Холодильники"){res="Норматив по холодильникам";}
          else
          if (s=="Склады"){res="Норматив по складам";}
          else
          if (s == "Предприятия общепита") { res = "Норматив по предприятиям общепита"; };


            return res;



        }
      string  HT(string s)
        {
            string res = "";
          if (s=="Магазины"){res="Обеспеченность по магазинам";}
          else
          if (s=="Хранилища"){res="Обеспеченность по хранилищам";}
          else
          if (s=="Холодильники"){res="Обеспеченность по холодильникам";}
          else
          if (s=="Склады"){res="Обеспеченность по складам";}
          else
          if (s == "Предприятия общепита") { res = "Обеспеченность по предприятиям общепита"; };

            return res;



        }

        protected void MainTableBottom_ActiveRowChange(object sender, RowEventArgs e)
        {
            
            try
            {
                UserParams.PeriodLastDate.Value = HT(e.Row.Cells[0].Text);
                UserParams.KDGroup.Value = HT1(e.Row.Cells[0].Text);
                UCB.DataBind();
                LabBottomC.Text = "Динамика обеспеченности «" + e.Row.Cells[0].Text + ", " + e.Row.Cells[2].Text.ToLower() + "»";
                int a = 0; if (e.Row.Cells[1].Text == null) { int i = (1 / a); } 
            }
            catch
            {
                //LabTopC.Text = "Нет данных";
                LabBottomC.Text = "Нет данных";
               // UCT.Visible = 1 == 2;
                UCB.Visible = 1 == 2;
            }

        }
        DataTable CB;
        protected void UCB_DataBinding(object sender, EventArgs e)
        {
            CB = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("CCB"), "a", CB);
            UCB.DataSource = CB;
        }

        protected void MainTableBottom_InitializeLayout(object sender, LayoutEventArgs e)
        {
            double tempWidth = MainTableBottom.Width.Value - 14;
            MainTableBottom.DisplayLayout.RowSelectorStyleDefault.Width = 20 - 2;
            MainTableBottom.Columns[0].Width = (int)((tempWidth - 90) * 0.65) - 5;
            MainTableBottom.Columns[1].Width = 70 - 5;
            MainTableBottom.Columns[2].Width = (int)((tempWidth - 90) * 0.35) - 5;
            MainTableBottom.BorderColor = Color.White;
            MainTableBottom.Columns[2].Header.Style.Wrap = true;
            MainTableBottom.Columns[2].CellStyle.Wrap = true;
            MainTableBottom.Columns[2].Header.Caption = "Ед. изм.";
            CRHelper.FormatNumberColumn(MainTableBottom.Columns[1], "#.##");
            //LabBottomT.Width = (int) (screen_width * 0.35);
        }

        protected void MainTableTop_InitializeLayout(object sender, LayoutEventArgs e)
        {
            double tempWidth = MainTableTop.Width.Value - 14;
            MainTableTop.DisplayLayout.RowSelectorStyleDefault.Width = 20 - 2;
            MainTableTop.Columns[0].Width = (int)((tempWidth - 90) * 0.65) - 5;
            MainTableTop.Columns[1].Width = 70 - 5;
            MainTableTop.Columns[2].Width = (int)((tempWidth - 90) * 0.35) - 5;
            MainTableTop.Columns[2].Header.Style.Wrap = true;
            MainTableTop.Columns[2].CellStyle.Wrap = true;
            MainTableTop.Columns[2].Header.Caption = "Ед. изм.";
            CRHelper.FormatNumberColumn(MainTableTop.Columns[1], "#.##");
            MainTableTop.BorderColor = Color.White;
        }

        #region //получение низшей иерархии
        private String ELV(String s)
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
        #endregion

        protected void MainTableTop_DblClick(object sender, ClickEventArgs e)
        {
            e.Cancel = 1 == 1;
        }

        public static void setChartErrorFont(Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventArgs e)
        {
            //e.Text = chart_error_message;
            e.LabelStyle.Font = new Font("Verdana", 20);
            e.LabelStyle.FontColor = Color.LightGray;
            e.LabelStyle.VerticalAlign = StringAlignment.Center;
            e.LabelStyle.HorizontalAlign = StringAlignment.Center;
        }
        protected void UCB_InvalidDataReceived(object sender, Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventArgs e)
        {
            setChartErrorFont(e);
        }

        protected void UCT_InvalidDataReceived(object sender, Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventArgs e)
        {
            setChartErrorFont(e);
        }

        protected void UCT_Init(object sender, EventArgs e)
        {
            //UCT.Axis.X.Labels.Orientation = Infragistics.UltraChart.Shared.Styles.TextOrientation.Horizontal;
        }

        protected void UCB_FillSceneGraph(object sender, Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e)
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


    }
}
