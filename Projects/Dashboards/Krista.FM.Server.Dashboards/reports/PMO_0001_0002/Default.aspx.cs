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
using Krista.FM.Server.Dashboards.Core;
using Microsoft.AnalysisServices.AdomdClient;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.PMO_0001_0002
{
    public partial class Default : Dashboards.CustomReportPage
    {

        string Errors = "";
        #region page_load
        DataTable mainT = new DataTable();
        protected override void Page_Load(object sender, EventArgs e)
        {
            Errors = "";
           
                try
                {
                    UltraWebGrid.DataBind();
                    UserParams.Region.Value = UltraWebGrid.Rows[1].Cells[0].Text;
                    CYear.DataBind();
                    Cmain.DataBind();
                    page_title.Text = "Сведенья о територии "  + ELV(DimensionTree1.ChoiceSet)  + " по данным за " + ELV(GLD());

                    Label4.Text = "Динамика показателя" + '"' + UltraWebGrid.Rows[0].Cells[0].Text + '"';
                }
                catch
                {
                    Errors = "Нет данных";
                }
                try
                {
                    if (Errors == "")
                    {
                       /* for (int i = 0; i <= UltraWebGrid.Rows.Count - 1; i++)
                        {
                            /// Доделать еденицу измерения        
                            UltraWebGrid.Rows[i].Cells[0].Text += "</br>&nbsp <font style=" + '"' + "color:teal;font-size: X-small;" + '"' + ">Еденица измерения:</font> &nbsp  " + "<font style=" + '"' + "font-size: X-small;" + '"' + ">" + UltraWebGrid.Rows[i].Cells[2].Text + "<font>";


                        }
                        UltraWebGrid.Columns[2].Hidden = 1 == 1;
                      */
                       // UltraWebGrid.Columns[0].CellStyle.BackColor = HyperLink1.BorderColor;
                    };
                    Label2.Text = "Информация територии и  числености населения по данным на "  + ELV(GLD())  + ' ' + '(' + ELV(DimensionTree1.ChoiceSet) + ')';
                }
                catch
                { Errors = "Неизвестная ошибка"; };
                if (Errors != "")
                { page_title.Text = Errors;
                
                }



            }
            #endregion

        #region UltraWebGrid1_DataBinding(main)
            protected void UltraWebGrid1_DataBinding(object sender, EventArgs e)
            {

            }
            #endregion
        #region UltraChart1_DataBinding
            DataTable yearC = new DataTable();

        protected void UltraChart1_DataBinding(object sender, EventArgs e)
            {
                yearC = new DataTable();
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("chartY"), "Город Губкинск", yearC);
                CYear.DataSource = yearC;
                double min = 0, max = 0;
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
                    Label4.Text = "Динамика показателя " + '"' + getNF(e.Row.Cells[0].Text) + '"';
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

                    UltraWebGrid.Columns[0].Width = 300;
                    UltraWebGrid.Columns[1].Width = 60;
                    UltraWebGrid.Columns[0].CellStyle.Wrap = 1 == 1;
                    UltraWebGrid.Columns[1].Header.Caption = "Значение";
                    UltraWebGrid.Columns[0].Header.Caption = "Вид територии";
                    UltraWebGrid.Columns[2].Width = 144;
                    CRHelper.FormatNumberColumn(UltraWebGrid.Columns[1], "N2");
                    HyperLink1.Visible = 1 == 2;
                    
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
        #region  //Получение последнй актульной даты
            private String GLD()
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

        protected void UltraWebGrid_DblClick(object sender, ClickEventArgs e)
        {
          //  e.Cancel = 1 == 1;
        }

        }
}
