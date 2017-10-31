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
    public partial class Service : CustomReportPage
    {
        // параметр для последней актуальной даты
        private CustomParam last_year { get { return (UserParams.CustomParam("last_year")); } }

        private int screen_width { get { return (int)Session["width_size"]; } }
        private CustomParam current_region { get { return (UserParams.CustomParam("current_region")); } }
        private CustomParam DataY { get { return (UserParams.CustomParam("DataY")); } }
        // параметр запроса для региона
        private CustomParam baseRegion { get { return (UserParams.CustomParam("baseRegion")); } }

        protected override void Page_Load(object sender, EventArgs e)
        {
            TestLabel.Text = screen_width.ToString();
            try
            {
                MainT.DataBind();
                ChartT.DataBind();
                ChartBottomL.DataBind();
                UltraChart3.DataBind();
                MainT.Width = (int)((screen_width - 55) * 0.4);
                ChartT.Width = (int)((screen_width - 55) * 0.6);
                ChartBottomL.Width = (int)((screen_width - 55)* 0.5);
                UltraChart3.Width = (int)((screen_width - 55) * 0.5);

                TestLabel.Text += " | " + MainT.Width + " | " + ChartT.Width + " | " + ChartBottomL.Width + " | " + UltraChart3.Width;


                if (!Page.IsPostBack)
                {   // опрерации которые должны выполняться при только первой загрузке страницы
                    RegionSettingsHelper.Instance.SetWorkingRegion(RegionSettings.Instance.Id);
                    baseRegion.Value = RegionSettingsHelper.Instance.RegionBaseDimension;

                    WebAsyncRefreshPanel1.AddLinkedRequestTrigger(MainT);

                    UserParams.PeriodYearQuater.Value = ELV(last_year.Value);

                    current_region.Value = baseRegion.Value;
                    last_year.Value = getLastDate();
                    LabChTop.Text = "Динамика показателя<br>«Объем платных услуг в действующих ценах на 1-го жителя, тысяча&nbsp;рублей» ";
                    //Label3.Text = "Структура платных услуг по видам услуг";
                    //Label2.Text = "Структура платных услуг по формам собствености";
                    UltraChart3.Visible = 1 == 1;
                    ChartT.Visible = 1 == 1;
                    ChartBottomL.Visible = 2 == 2;
                    UserParams.KDGroup.Value = "Объем платных услуг в действующих ценах на 1-го жителя";
                     
                    MainT.DataBind();
                    ChartT.DataBind();
                    ChartBottomL.DataBind();
                    UltraChart3.DataBind();
                    Label4.Text = "Информация о показателях платных услугах по данным на  " + ELV(last_year.Value) + " год (" + RegionSettingsHelper.Instance.Name + ")";
                    Label2.Text = "Структура платных услуг по формам собствености на&nbsp;" + ELV(last_year.Value) + "&nbsp;год,&nbsp;%";
                    Label3.Text = "Структура платных услуг по видам услуг на&nbsp;" + ELV(last_year.Value) + "&nbsp;год,&nbsp;%";
                }
                if (MainT.Rows[0].Cells[0].Text == null)
                {
                    int a = 0; a = 1 / a;
                }
            }
            catch
            {
                LabChTop.Text = "Нет данынх";
                Label2.Text = "Нет данынх";
                Label3.Text = "Нет данынх";
                UltraChart3.Visible = 1 == 2;
                ChartT.Visible = 1 == 3;
                ChartBottomL.Visible = 2 == 21;
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
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("STT"), "Год", MT);
            MainT.DataSource = MT;
           
        }
        DataTable TC;
        protected void UltraChart2_DataBinding(object sender, EventArgs e)
        {
            TC = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("CT"), "s", TC);

         
            
                ChartT.DataSource = TC;
           

        }
        DataTable BLC;
        protected void ChartBottomL_DataBinding(object sender, EventArgs e)
        {
            try
            {
                BLC = new DataTable();
                string s = DataProvider.GetQueryText("CBL");
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(s, "s", BLC);
                if (BLC.Columns.Count > 1)
                {
                    ChartBottomL.DataSource = BLC;
                }
                else
                {
                    ChartBottomL.DataSource = null;
                }
            }
            catch
            {
            }


        }
        DataTable BRC;
        protected void UltraChart3_DataBinding(object sender, EventArgs e)
        {
            try
            {
                BRC = new DataTable();
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("CBR"), "a", BRC);
                if (BLC.Columns.Count > 1)
                {
                    UltraChart3.DataSource = BRC;
                }
                else
                {
                    UltraChart3.DataSource = null;
                }
            }
            catch
            { }
            
            

        }

        protected void MainT_ActiveCellChange(object sender, CellEventArgs e)
        {
            e.Cell.Column.Selected = 1 == 1;
        }
        bool a=1==2;

        protected void MainT_ActiveCellChange1(object sender, CellEventArgs e)
        {
            try
            {
                a = 1 == 1;
                UserParams.PeriodYearQuater.Value = e.Cell.Row.Cells[0].Text;
                if (e.Cell.Column.Index == 1)
                {
                    UserParams.KDGroup.Value = "Объем платных услуг в действующих ценах";
                    LabChTop.Text = "Динамика показателя<br>«" + UserParams.KDGroup.Value + ", тысяча&nbsp;рублей»";
                }
                else
                    if (e.Cell.Column.Index == 2)
                    {
                        UserParams.KDGroup.Value = "Объем платных услуг в действующих ценах на 1-го жителя";
                        LabChTop.Text = "Динамика показателя<br>«" + UserParams.KDGroup.Value + ", тысяча&nbsp;рублей»";
                        ChartT.DataBind();
                    }
                Label2.Text = "Структура платных услуг по формам собствености на&nbsp;" + e.Cell.Row.Cells[0].Text + "&nbsp;год,&nbsp;%";
                Label3.Text = "Структура платных услуг по видам услуг на&nbsp;" + e.Cell.Row.Cells[0].Text + "&nbsp;год,&nbsp;%";
                ChartT.DataBind();
                ChartBottomL.DataBind();
                UltraChart3.DataBind();


            }
            catch
            {
                LabChTop.Text = "Нет данынх";
                Label2.Text = "Нет данынх";
                Label3.Text = "Нет данынх";
                UltraChart3.Visible = 1 == 2;
                ChartT.Visible = 1 == 3;
                ChartBottomL.Visible = 2 == 21; 
            }

            }

        protected void MainT_InitializeLayout(object sender, LayoutEventArgs e)
        {
            double tempWidth = MainT.Width.Value - 14;
            MainT.DisplayLayout.RowSelectorStyleDefault.Width = 20 - 2;
            MainT.Columns[0].Width = (int)((tempWidth - 20) * 0.1) - 5;
            MainT.Columns[1].Width = (int)((tempWidth - 20) * 0.45) - 5;
            MainT.Columns[2].Width = (int)((tempWidth - 20) * 0.45) - 5;

            TestLabel.Text += " * " + MainT.DisplayLayout.RowSelectorStyleDefault.Width + " | " + MainT.Columns[0].Width + " | " + MainT.Columns[1].Width + " | " + MainT.Columns[2].Width + " *";

            MainT.Columns[1].Header.Style.Wrap = 1 == 1;
            MainT.Columns[2].Header.Style.Wrap = 1 == 1;
            CRHelper.FormatNumberColumn(MainT.Columns[2], "N0");
            CRHelper.FormatNumberColumn(MainT.Columns[1], "N0");
            MainT.BorderColor = Color.White;
            ChartT.Border.Color = Color.White;
            ChartBottomL.Border.Color = Color.White;
            UltraChart3.Border.Color = Color.White;
        }

        protected void MainT_DblClick(object sender, ClickEventArgs e)
        {
            e.Cancel = 1 == 1;
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


        protected void MainT_ActiveRowChange(object sender, RowEventArgs e)
        {
            //UserParams.KDGroup.Value = ParamC;
            if (!a)
            {
            UserParams.PeriodYearQuater.Value = e.Row.Cells[0].Text;
            Label2.Text = "Структура платных услуг по формам собствености на&nbsp;" + e.Row.Cells[0].Text + "&nbsp;год,&nbsp;%";
            Label3.Text = "Структура платных услуг по видам услуг на&nbsp;" + e.Row.Cells[0].Text + "&nbsp;год,&nbsp;%";

            ChartT.DataBind();
            ChartBottomL.DataBind();
            UltraChart3.DataBind();}
        }

        public static void setChartErrorFont(Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventArgs e)
        {
          //  e.Text = chart_error_message;
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
    }
}
