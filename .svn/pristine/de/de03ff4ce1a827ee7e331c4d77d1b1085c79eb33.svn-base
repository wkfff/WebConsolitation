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

namespace Krista.FM.Server.Dashboards.reports.PMO_0001_0002
{

    public partial class Default : CustomReportPage
    {
        private CustomParam current_region { get { return (UserParams.CustomParam("current_region")); } }

        // параметр для последней актуальной даты
        private CustomParam last_year { get { return (UserParams.CustomParam("last_year")); } }

        private int screen_width { get { return (int)Session["width_size"]; } }

        private int screen_height { get { return (int)Session["height_size"]; } }
       // private CustomParam current_region { get { return (UserParams.CustomParam("current_region")); } }

        // параметр запроса для региона
        private CustomParam baseRegion { get { return (UserParams.CustomParam("baseRegion")); } }
        string Errors = "";
        DataTable mainT = new DataTable();

        protected override void Page_Load(object sender, EventArgs e)
        {
            //WebPanel1.Attributes.Add("onMouseOver", "elm = document.getElementById('WebPanel1'); elm.expand = true;");
            UltraWebGrid.Width = (int)((screen_width - 55) * 0.5);
            Cmain.Width = (int)((screen_width - 55) * 0.5);
            CYear.Width = (int)(screen_width - 55);

            Errors = "";
                try
                {
                    if (!Page.IsPostBack)
                    {
                        RegionSettingsHelper.Instance.SetWorkingRegion(RegionSettings.Instance.Id);
                        baseRegion.Value = RegionSettingsHelper.Instance.RegionBaseDimension;
                        
                        WebAsyncRefreshPanel1.AddLinkedRequestTrigger(UltraWebGrid);
                        

                        current_region.Value = baseRegion.Value;
                        last_year.Value = getLastDate();
                        string s = DataProvider.GetQueryText("maintable");
                        UltraWebGrid.DataBind();
                        // получаем выбранную строку
                        UltraGridRow row = UltraWebGrid.Rows[0];
                        // устанавливаем ее активной, если необходимо
                        row.Activate();
                        row.Activated = true;

                        UserParams.Region.Value = UltraWebGrid.Rows[0].Cells[0].Text;
                        Cmain.DataBind();
                        page_title.Text = "Сведения o территории «" + RegionSettingsHelper.Instance.Name + "» по данным за&nbsp;" + ELV(last_year.Value) + "&nbsp;год";

                        Label4.Text = "Динамика показателя «" + UltraWebGrid.Rows[0].Cells[0].Text + ", " + UltraWebGrid.Rows[0].Cells[2].Text.ToLower() + "»";
                        CYear.DataBind();
                    }
                }
                catch
                {
                    Errors = "Нет данных";
                }


                try
                {
                    Label2.Text = "Информация о территории и  численности населения по данным на " + ELV(last_year.Value) + ' ' + '(' + RegionSettingsHelper.Instance.Name + ')';
                }
                catch
                { Errors = "в настоящий момент данные отсутствуют"; };
                if (Errors != "")
                {
                    page_title.Text = Errors;
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

        #region UltraWebGrid1_DataBinding(main)
            protected void UltraWebGrid1_DataBinding(object sender, EventArgs e)
            {

            }
            #endregion
        #region UltraChart1_DataBinding
            DataTable yearC = new DataTable();

        protected void CYear_DataBinding(object sender, EventArgs e)
            {
                yearC = new DataTable();
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("chartY"), "Город Губкинск", yearC);

                float min, max;
                min = max = float.Parse(yearC.Rows[0].ItemArray[1].ToString());

                for (int i = 1; i < yearC.Rows[0].ItemArray.Length; i++)
                {
                    if (float.Parse(yearC.Rows[0].ItemArray[i].ToString()) < min)
                    {
                        min = float.Parse(yearC.Rows[0].ItemArray[i].ToString());
                    }
                    if (float.Parse(yearC.Rows[0].ItemArray[i].ToString()) > max)
                    {
                        max = float.Parse(yearC.Rows[0].ItemArray[i].ToString());
                    }
                }
                if ((min / max) > 0.98)
                {
                    CYear.Axis.Y.RangeMin = min * 0.999;
                    CYear.Axis.Y.RangeMax = max * 1.001;
                }
                else
                {
                    CYear.Axis.Y.RangeMin = min * 0.98;
                    CYear.Axis.Y.RangeMax = max * 1.02;
                }

                CYear.DataSource = yearC.DefaultView;
                //double min = 0, max = 0;
            }
            #endregion         
        #region Cmain_DataBinding
            DataTable mainC = new DataTable();

        protected void Cmain_DataBinding(object sender, EventArgs e)
            {
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("mainC"), "Город Губкинский1", mainC);
                Cmain.DataSource = mainC;

            }
            #endregion
        #region UltraWebGrid_DataBinding
            protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
            {

                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("maintable"), "Територия", mainT);
                UltraWebGrid.DataSource = mainT;
            }
            #endregion

        #region Onclick
            protected void UltraWebGrid_ActiveRowChange(object sender, RowEventArgs e)
            {
                try
                {
                    UserParams.Region.Value = getNF(e.Row.Cells[0].Text);
                    Label4.Text = "Динамика показателя «" + getNF(e.Row.Cells[0].Text) + ", " + e.Row.Cells[2].Text.ToLower() + "»";
                    CYear.DataBind();
                    
                }
                catch
                {
                    Errors = "Нет данных";
                    page_title.Text = Errors;
                }

            }
            #endregion
        #region Formating
            protected void UltraWebGrid_InitializeLayout(object sender, LayoutEventArgs e)
            {
                try
                {
                    double tempWidth = UltraWebGrid.Width.Value - 14;
                    UltraWebGrid.DisplayLayout.RowSelectorStyleDefault.Width = 20 - 2;
                    UltraWebGrid.Columns[0].Width = (int)((tempWidth - 90) * 0.75) - 5;
                    UltraWebGrid.Columns[1].Width = 70 - 5;
                    UltraWebGrid.Columns[2].Width = (int)((tempWidth - 90) * 0.25) - 5;
                    UltraWebGrid.Columns[0].CellStyle.Wrap = 1 == 1;
                    UltraWebGrid.Columns[2].CellStyle.Wrap = 1 == 1;
                    UltraWebGrid.Columns[0].Header.Caption = "Вид територии";
                    UltraWebGrid.Columns[1].Header.Caption = "Значение";
                    UltraWebGrid.Columns[2].Header.Caption = "Ед. изм.";
                    UltraWebGrid.Columns[2].Header.Style.Wrap = 1 == 1;                    
                    CRHelper.FormatNumberColumn(UltraWebGrid.Columns[1], "#.##");
//                    HyperLink1.Visible = 1 == 2;
                    
                }
                catch
                {
                    Errors = "Ошибка форматирования";
                    page_title.Text = Errors;
                }
            }
            #endregion 

        #region getNF
        protected string getNF(string s)
        {
            string res = "";
            for (int i = 0; i < s.Length; i++)
            {
                if (s[i] != '<')
                { res += s[i]; }
                else
                { i = s.Length + 1; }

            }
            return res;
        }
        #endregion

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

        protected void CYear_ChartDataClicked(object sender, Infragistics.UltraChart.Shared.Events.ChartDataEventArgs e)
        {

        }

        public static void setChartErrorFont(Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventArgs e)
        {
           // e.Text = chart_error_message;
            e.LabelStyle.Font = new Font("Verdana", 20);
            e.LabelStyle.FontColor = Color.LightGray;
            e.LabelStyle.VerticalAlign = StringAlignment.Center;
            e.LabelStyle.HorizontalAlign = StringAlignment.Center;

        }
        protected void CYear_InvalidDataReceived(object sender, Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventArgs e)
        {
            setChartErrorFont(e);
        }

        protected void Cmain_InvalidDataReceived(object sender, Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventArgs e)
        {
            setChartErrorFont(e);
        }

        }
}
