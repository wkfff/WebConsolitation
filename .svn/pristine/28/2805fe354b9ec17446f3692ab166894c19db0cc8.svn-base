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


namespace Krista.FM.Server.Dashboards.reports.PMO_0001_0004
{
    public partial class service : Dashboards.CustomReportPage
    {
        protected override void Page_Load(object sender, EventArgs e)
        {
            try
            {
                LabChTop.Text = "Объем платных услуг в действующих ценах на 1-го жителя";
                Label3.Text = "Структура платных услуг по видам услуг";
                Label2.Text = "Структура платных услуг по формам собствености";
                UltraChart3.Visible = 1 == 1;
                ChartT.Visible = 1 == 1;
                ChartBottomL.Visible = 2 == 2;
                UserParams.KDGroup.Value = "Объем платных услуг в действующих ценах на 1-го жителя";
                MainT.DataBind();
                ChartT.DataBind();
                ChartBottomL.DataBind();
                UltraChart3.DataBind();
                Label4.Text = "Информация о показателях торговли, общественного питания и платных услугах по данным на  " + ELV(GLD()) + " год (" + ELV(DimensionTree1.ChoiceSet) + ")";
                if (MainT.Rows[0].Cells[0].Text == null) { int a = 0; a = 1 / a; }
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
            BLC = new DataTable();
            string s = DataProvider.GetQueryText("CBL");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(s, "s", BLC);
            ChartBottomL.DataSource = BLC;


        }
        DataTable BRC;
        protected void UltraChart3_DataBinding(object sender, EventArgs e)
        {
            BRC = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("CBR"), "a", BRC);
            UltraChart3.DataSource = BRC;
            

        }

        protected void MainT_ActiveCellChange(object sender, CellEventArgs e)
        {
            e.Cell.Column.Selected = 1 == 1;
        }

        protected void MainT_SelectedColumnsChange(object sender, SelectedColumnsEventArgs e)
        {
            
        }

        protected void MainT_ActiveCellChange1(object sender, CellEventArgs e)
        {
            if (e.Cell.Column.Index == 1)
            { UserParams.KDGroup.Value = "Объем платных услуг в действующих ценах"; LabChTop.Text = UserParams.KDGroup.Value; ChartT.DataBind(); };
            if (e.Cell.Column.Index == 2)
            { UserParams.KDGroup.Value = "Объем платных услуг в действующих ценах на 1-го жителя";LabChTop.Text = UserParams.KDGroup.Value; ChartT.DataBind(); };
            
            }

        protected void MainT_InitializeLayout(object sender, LayoutEventArgs e)
        {
            MainT.Columns[0].Width = 50;
            MainT.Columns[1].Width = 200;
            MainT.Columns[2].Width = 200;
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
    }
}
