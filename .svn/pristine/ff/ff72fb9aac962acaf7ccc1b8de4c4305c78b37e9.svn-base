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
namespace Krista.FM.Server.Dashboards.reports.PMO_0001_0004
{
    public partial class commerce : Dashboards.CustomReportPage
    {

        protected override void Page_Load(object sender, EventArgs e)
        {
            try
            {
                UserParams.PeriodYearQuater.Value = GLD();
                MainTableTop.DataBind();
                MainTableBottom.DataBind();
                UserParams.KDGroup.Value = MainTableTop.Rows[0].Cells[0].Text;
                UCT.DataBind();
                UserParams.PeriodLastDate.Value = HT(MainTableBottom.Rows[0].Cells[0].Text);
                UserParams.KDGroup.Value = HT1(MainTableBottom.Rows[0].Cells[0].Text);
                UCB.DataBind();
                MainTableTop.Columns[0].CellStyle.Wrap = true;
                LabTopC.Text = "Динамика показателя " + '"' + MainTableTop.Rows[0].Cells[0].Text + '"';
                LabBottomC.Text = "Динамика обеспечености (" + MainTableBottom.Rows[0].Cells[0].Text + ")";
                Label1.Text = "Информация о показателях торговли, общественного питания и платных услугах по данным на  " + ELV(GLD()) + " год (" + ELV(DimensionTree1.ChoiceSet) + ")";
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
            CellSet val;
            val = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(DataProvider.GetQueryText("CTB"));
            MTB.Columns.Add("Обект");
            MTB.Columns.Add("значение");
            MTB.Columns.Add("Еденица измерения");
            for (int i = 0; i <= (val.Cells.Count)/2-1; i++) {

                object[] o = { val.Axes[1].Positions[i].Members[0].Caption, val.Cells[0,i].Value, val.Cells[1,i].Value };            
            MTB.Rows.Add(o);}
            MainTableBottom.DataSource = MTB;




            
        }
        #endregion
        DataTable CT;
        protected void UltraChart1_DataBinding(object sender, EventArgs e)
        {
            CT = new DataTable();
            string s = DataProvider.GetQueryText("CCT");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(s,"a",CT);
            UCT.DataSource = CT;

        }

        protected void MainTableTop_ActiveRowChange(object sender, RowEventArgs e)
        {
            try
            {
                UserParams.KDGroup.Value = e.Row.Cells[0].Text;
                UCT.DataBind();
                LabTopC.Text = "Динамика показателя " + '"' + e.Row.Cells[0].Text + '"';
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
                LabBottomC.Text = "Динамика обеспечености (" + e.Row.Cells[0].Text + ")";
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
            MainTableBottom.Columns[0].Width = 200;
            MainTableBottom.Columns[1].Width = 70;
            MainTableBottom.Columns[2].Width = 130;
            MainTableBottom.BorderColor = Color.White;
            CRHelper.FormatNumberColumn(MainTableBottom.Columns[1], "N2");
                
        }

        protected void MainTableTop_InitializeLayout(object sender, LayoutEventArgs e)
        {
            MainTableTop.Columns[0].Width = 200;
            MainTableTop.Columns[1].Width = 70;
            MainTableTop.Columns[2].Width = 130;
            CRHelper.FormatNumberColumn(MainTableTop.Columns[1], "N2");

            MainTableTop.BorderColor = Color.White;
        }
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

        protected void MainTableTop_DblClick(object sender, ClickEventArgs e)
        {
            e.Cancel = 1 == 1;
        }

    }
}
