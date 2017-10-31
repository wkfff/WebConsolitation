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
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Krista.FM.Server.Dashboards.Core;

namespace Krista.FM.Server.Dashboards.reports.PMO_0001_0002
{
    public partial class Population : Dashboards.CustomReportPage
    {

        #region Page_Load
        protected override void Page_Load(object sender, EventArgs e)
        {

            try
            {
                ChartHideColumn.Visible = 1 == 1;
                UltraChart1.Visible = 1 == 1;
                string s = "Численность постоянного населения на начало года";
                UltraWebGrid1.DataBind();
                UserParams.KDGroup.Value = s;
                UltraChart1.DataBind();
                UserParams.KDGroup.Value = s;
                UserParams.KDGroup.Value = caseParam(s);
                ChartHideColumn.DataBind();
                UltraChart1.TitleTop.Text = s;
                page_title.Text = "Основне показатели числености населения на " + ELV(GLD()) + " год &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp Динамика показателя "+'"' + caseParam(s)+'"';
                Label2.Text = "Информация o територии и  числености населения по данным на " + ELV(GLD()) +" "+'('+ELV(DimensionTree1.ChoiceSet)+')';
                Label1.Visible = 1 == 2;
                Label1.Font.Size = FontUnit.XXSmall;
            }
            catch
            {
                ChartHideColumn.Visible = 1 == 2;
                UltraChart1.Visible = 1 == 2;
                page_title.Text = "Нет данных";
                ChartHideColumn.DataSource = new DataTable();
                UltraChart1.DataSource = new DataTable();
                Label1.Visible = 1 == 1;
                Label1.Font.Size = FontUnit.XXLarge;
                
            }
        }
        #endregion


        #region UltraWebGrid1_DataBinding(main)
        DataTable mainT = new DataTable();
        protected void UltraWebGrid1_DataBinding(object sender, EventArgs e)
        {
            mainT = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("mainTP"), "s1", mainT);
            UltraWebGrid1.DataSource = mainT;
            
        }
        #endregion MT
        #region UltraChart1_DataBinding
        DataTable mainC = new DataTable();
        protected void UltraChart1_DataBinding(object sender, EventArgs e)
        {
            mainC = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("mainCP"), "s1", mainC);
            UltraChart1.DataSource = mainC;
        }
        #endregion Gc;        
        #region ChartHideColumn_DataBinding
        DataTable HCC;
        protected void ChartHideColumn_DataBinding(object sender, EventArgs e)
        {
            HCC = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("mainCP"), "s", HCC);
            ChartHideColumn.DataSource = HCC;
            UltraWebGrid1.BorderColor = Color.White;
        }
        #endregion
        #region GridChart_DataBinding
        DataTable GC;
        protected void GridChart_DataBinding(object sender, EventArgs e)
        {
            GC = new DataTable();

            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("mainCP"), "s", GC);
           // GridChart.DataSource = GC;
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
        #region //Сопастовление параметров
        string caseParam(string s)
        {
            if (s == "Численность постоянного населения на начало года")
            { s = "Среднегодовая численность населения"; }
            else
                if (s == "Численность постоянного населения на конец года")
                { s = "Среднегодовая численность населения"; }
                else
                    if (s == "Число родившихся")
                    { s = "Коэффициент рождаемости"; }
                    else
                        if (s == "Число умерших")
                        { s = "Коэффициент смертности"; }
                        else
                            if (s == "Естественный прирост")
                            { s = "Коэффициент естественного прироста"; }
                            else
                                if (s == "Механический прирост")
                                { s = "Коэффициент механического прироста"; }
                                else
                                    if (s == "Число зарегистрированных браков")
                                    { s = "Коэффициент брачности"; }
                                    else
                                        if (s == "Число зарегистрированных разводов")
                                        { s = "Коэффициент брачности"; }
                                        else
                                        {
                                            s = "";
                                        }
            return s;
        }
        #endregion


        #region OnChange
        protected void UltraWebGrid1_ActiveRowChange(object sender, RowEventArgs e)
        {
            try
            {
                ChartHideColumn.Visible = 1 == 1;
                UltraChart1.Visible = 1 == 1;
                string s = e.Row.Cells[0].Text;
                UserParams.KDGroup.Value = s;

                    UltraChart1.TitleTop.Text = "Динамика показателя " + '"' + e.Row.Cells[0].Text + '"';
                    UltraChart1.DataBind();
                    page_title.Text = "Основне показатели числености населения на " + ELV(GLD()) + "год &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp Динамика показателя " + '"' + caseParam(s) + '"';

                    s = caseParam(e.Row.Cells[0].Text);
                    if (s == "")
                    {
                        UserParams.KDGroup.Value = s;
                        ChartHideColumn.Visible = 1 == 2;
  
                        Label1.Visible = 1 == 1;
                        Label1.Font.Size = FontUnit.XXLarge;
                        page_title.Text = "Основне показатели числености населения на " + ELV(GLD()) + "год &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp     Нет данных ";

                    
                    }
                    else
                    {
                        UserParams.KDGroup.Value = s;
                        ChartHideColumn.Visible = 1 == 1;
                        ChartHideColumn.DataBind();
                        Label1.Visible = 1 == 2;
                        Label1.Font.Size = FontUnit.XXSmall;
                    }

     
            }
            catch
            {
                ChartHideColumn.DataSource = new DataTable();
                UltraChart1.DataSource = new DataTable();
                
            }
        }
        #endregion 

        #region Formating
        protected void UltraWebGrid1_InitializeLayout(object sender, LayoutEventArgs e)
        {
            try
            {
                UltraWebGrid1.Columns[0].Width = 200;
                UltraWebGrid1.Columns[1].Width = 65;
                UltraWebGrid1.Columns[2].Width = 90;
                UltraWebGrid1.Columns[0].CellStyle.Wrap = 1 == 1;
                UltraWebGrid1.Columns[0].Header.Caption = "Показатель";
                UltraWebGrid1.Columns[1].Header.Caption = "Всего";
                UltraWebGrid1.Columns[2].Header.Style.Wrap = 1 == 1;
                UltraWebGrid1.Columns[2].Header.Caption = "Изменение за год";
                CRHelper.FormatNumberColumn(UltraWebGrid1.Columns[1], "N0");
                CRHelper.FormatNumberColumn(UltraWebGrid1.Columns[2], "N0");
                HyperLink1.Visible = 1 == 2;
            }
            catch
            {
                page_title.Text = "Ошибка форматирования";
            }
            
        }

        #endregion

        protected void ChartHideColumn_DataBound(object sender, EventArgs e)
        {
            UltraWebGrid1.BorderColor = Color.White;
        }

        protected void WebAsyncRefreshPanel2_ContentRefresh(object sender, EventArgs e)
        {
            UltraWebGrid1.BorderColor = Color.White;
        }

        protected void UltraWebGrid1_DblClick(object sender, ClickEventArgs e)
        {
            e.Cancel = 1 == 1;
        }

        protected void ChartHideColumn_ChartDataClicked(object sender, Infragistics.UltraChart.Shared.Events.ChartDataEventArgs e)
        {

        }

        protected void ChartHideColumn_ChartDataDoubleClicked(object sender, Infragistics.UltraChart.Shared.Events.ChartDataEventArgs e)
        {

        }

        protected void ChartHideColumn_InvalidDataReceived(object sender, Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventArgs e)
        {

        }

 


    }
}
