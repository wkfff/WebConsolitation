using System;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls.WebParts;
using Infragistics.WebUI.Misc;
using Microsoft.AnalysisServices.AdomdClient;
using Infragistics.WebUI.UltraWebChart;
using Infragistics.UltraChart.Core.Primitives;
using System.Collections.ObjectModel;
using System.Text;
using System.Configuration;
using System.Collections.Generic;
using Infragistics.UltraChart.Core;
using Dundas.Maps.WebControl;

//������ ��������� �� ������ ���, � ����������� ����������


namespace Krista.FM.Server.Dashboards.reports.EO.EO_0007.EO_001
{
    public partial class _default : CustomReportPage
    {
        //��������� ��� ��������

        //��������� ���� �� ������� ���� �����
        private CustomParam p1 ;

        //�������� ��������� ������ �� ���������
        private CustomParam p2 ;

        //������ ���������� �� ���������
        string region = "�����-���������� ���������� ����� - ����";

        //����� ��� �������
        string LCT = "��������� ��������� �������";
        string RCT = "��������� ���������� �������";

        //������ ��� ��������������� ���������
        string Economy = "���������� ��������� �������� �� ���� ���������� �������� ���������� ������  ���������<br>{0} ���. ���.";

        //�������� ����, ����� ��� ���������
        string CurentDate = DateTime.Now.ToString();

        //�������� ���������, � �� ��������� � ���� ���������������
        string Textovka = @"
                        <b>{0}</b> 
                        <br><br> 
                        ����� � {1} ���� (�� ��������� �� " + DateTime.Now.Day + '.' + DateTime.Now.Month + '.' + DateTime.Now.Year + @") ���� ��������� ������� �� �����<br>{3} ���. ���." + @"<br>
                        {4}<br><br>
                        <b>���������� ������� �� �������� ���������� �������</b>";
        //������� � 
        string URLS = 
            @"<a href=" + '"' + "../EO_0005/default.aspx" + '"' + ">1 ������ ����������</a><br>";

        /// <summary>
        /// ����� ������� ������ ��������� ��� ���������
        /// </summary>
        /// <param name="sql">��� ��������</param>
        /// <returns>������ ��� ���������</returns>
        Dictionary<string, int> ForParam2(string sql)
        {   //��������� ������, � ����� � ���� ������� ��������� �������
            CellSet cs = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(DataProvider.GetQueryText(sql));
            
            //������ ������ ������ 
            Dictionary<string, int> d = new Dictionary<string, int>();
            
            //��������� ������ �� ���������� �������, � ��������� ������
            for (int i = cs.Axes[1].Positions.Count - 1; i >= 0; i--)
            {
                
                d.Add(cs.Axes[1].Positions[i].Members[0].Caption, 0);
            }
            
            return d;
        }

        //���������� ��������� �������, ����� ��� �������� �������� �����������
        double k = 1;
        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            // ��� ��� ���� ����� ��������� ����������� �� CustomReportPage �� �������������� ���, � ��������� ��� ������, ��� �����������!!!
            base.Page_PreLoad(sender, e);

            //������������� ���������
            p1 = UserParams.CustomParam("1");
            p2 = UserParams.CustomParam("2");
           
            //���� ��� �������� �� �������� ������ ������, ������ ������ �������� � ��� ����� �� � �� �� ���������� ��������� ��������� ������� �����������
            string browser = HttpContext.Current.Request.Browser.Browser;
            
            if (browser == "Firefox")
            {
                {
                    k = 0.985;
                }
            }
            else
                if (browser == "AppleMAC-Safari")
                {
                    k = 0.998;
                }
            //��(����� �������������), ������ Years ��� ���������(������) � �����������
            Years.Width = 500;

            //����� ������� �����,
            Grid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth * 0.68);
            Grid.Height = CRHelper.GetGridHeight(140);
            //�������
            LC.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth * 0.4903);
            RC.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth * 0.4903);
            RC.Height = 400;
            LC.Height = RC.Height;
            //� ����� ������ � ������ � �������
            URLi.Height = CRHelper.GetGridHeight(288 * k * k * k * k * k * k * k * k * k);

            URLi.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth * 0.317);

            TopText.Width = Grid.Width;
        }
        //���� �� ����� ���� ������� ��������� �� ��������� �� ��� �� �� �������� �� �� ������� �� ��� ��������� � ���� �����������
        public static void setChartErrorFont(Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventArgs e)
        {
            e.Text = "� ��������� ������ ������ �����������";

            e.LabelStyle.FontColor = Color.LightGray;
            e.LabelStyle.VerticalAlign = StringAlignment.Center;
            e.LabelStyle.HorizontalAlign = StringAlignment.Center;
            e.LabelStyle.Font = new Font("Verdana", 30);

        }
        /// <summary>
        /// �������� � ����������� ���������� �� ����� �������
        /// </summary>
        /// <param name="sql">��� � ������� ��������</param>
        /// <returns></returns>
        public DataTable GetDSForChart(string sql)
        {
            DataTable dt = new DataTable();
            string s = DataProvider.GetQueryText(sql);
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(s, "����������", dt);
            return dt;
        }
        /// <summary>
        /// ����������� ������� � ��������� ����� �� ������� ���� ������ � ����
        /// </summary>
        /// <param name="qs"></param>
        /// <returns></returns>
        public string LD(string qs)
        {
            CellSet cs = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(DataProvider.GetQueryText(qs));

            return cs.Axes[1].Positions[cs.Axes[1].Positions.Count - 1].Members[0].Caption;
        }
        //������ ��� ������ �� ����(���� ���������� ����� � CRHelper �� �������� ���� ��� �������� ��������, ���� ��� ����� �������� ��� ��� �������� ������ ����� �� �� �� �� �� ������, ������� ����������� ������� ���������� ������ 
        string AddZero(string s)
            {//�� ���� ��� ��������������� ����� ���, �������� ����� �����, �� ������� �� ��������
                try
                {int i;
                    for (i = s.Length-1; s[i] != ','; i--);
                    if (s.Length - i == 2)
                    {
                        return s + "0";
                    }
                    
                }
                catch
                {
                    return s + ",00";
                }
                return s;
                    
            }
        /// <summary>
        /// ���� ����� ������ ������� � �����, ��� ����� ��������� �������� ������, ����� ���...
        /// </summary>
        /// <param name="s"></param>
        /// <param name="cg">������ �� �������� ���������� �����</param>
        /// <returns></returns>
        string AddSpace(string s,char cg)
        {int i;
        try
        {
            for (i = 0; s[i] != cg; i++) ;

            int j = 0;

            for (j = i - 3; j > 0; j -= 3)
            {
                try
                {
                    s = s.Insert(j, " ");
                }
                catch { }
            }
        }
        catch
        { 
        }
            return s;
        }



        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            //���� ��������� ����
            p1.Value = LD("lastdate");
            if (!Page.IsPostBack)
            {
                //���� ������ �� ���������
                p2.Value = region;
                
                //��������� ������ ��������� � ������ �������� �� ���������
                Years.FillDictionaryValues(ForParam2("params"));
                Years.Title = "���������";

                Years.Set�heckedState(region, true);
            }
            else
            {
                //� ������ ������ ���� ����� �������� ��� ����� ����� �������� �� �������� ��������, ������� � �������� �������� ������ �������� ��������� �� �������
                p2.Value = Years.SelectedValue;
                region = p2.Value;
            }
            //��������� ��������� � ����
            Grid.DataBind();
            LC.DataBind();
            RC.DataBind();

            //(������� �� ���������) ��� �������������� �������� ��� ��������(�� ��� � ���������), � ����������� � ���� ����  
            #region Grid
            double sum = 0;
            for (int i = 0; i < Grid.Rows.Count; i++)
            {
                sum += double.Parse((Grid.Rows[i].Cells[1].Text));
                Grid.Rows[i].Cells[1].Text = Math.Round(double.Parse((Grid.Rows[i].Cells[1].Text)) / 1000, 3).ToString();
            }
           // Grid.Columns.Add("col2", "%");
            Grid.Columns[2].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            CRHelper.FormatNumberColumn( Grid.Columns[2], "N0");


            sum = sum / 100000;
            for (int i = 0; i < Grid.Rows.Count; i++)
            {
                
                Grid.Rows[i].Cells[1].Text += " "+'('+AddZero(
                Math.Round(double.Parse((Grid.Rows[i].Cells[1].Text)) / sum, 2).ToString())+')';
                Grid.Rows[i].Cells[1].Text = AddSpace(Grid.Rows[i].Cells[1].Text,',');
            }
            Grid.Columns[0].Width = CRHelper.GetColumnWidth((CustomReportConst.minScreenWidth * 0.68 * k) * 0.45);
            Grid.Columns[1].Width = CRHelper.GetColumnWidth((CustomReportConst.minScreenWidth * 0.68 * k) * 0.26) - 3;
            Grid.Columns[2].Width = CRHelper.GetColumnWidth((CustomReportConst.minScreenWidth * 0.68 * k) * 0.22) - 3;

            sum = 0;
            for (int i = 0; i < Grid.Rows.Count; i++){sum += double.Parse((Grid.Rows[i].Cells[2].Text));}
            sum = sum / 100;
            for (int i = 0; i < Grid.Rows.Count; i++)
            {
                Grid.Rows[i].Cells[2].Text +=" "+ '(' + AddZero(Math.Round(double.Parse((Grid.Rows[i].Cells[2].Text)) / sum, 2).ToString()) + ')';
                Grid.Rows[i].Cells[2].Text = AddSpace(Grid.Rows[i].Cells[2].Text,' ');
            }

            #endregion
            //(������� �� ���������) ��� ������������ �������� ��� ���������
            #region Text

            URLi.Text = URLS;
            sum *= 100;

            HLC.Text = LCT;
            HRC.Text = RCT;

            double CTONMOCTb = 0;
            CellSet CS = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(DataProvider.GetQueryText("CTONMOCTb"));
            CTONMOCTb = (double.Parse(CS.Cells[0].Value.ToString()) - sum) / 1000000;
            TopText.Text = string.Format(Textovka, region, DateTime.Now.Year.ToString(), DateTime.Now.Year.ToString(), Math.Round(sum, 2).ToString(), string.Format(Economy, Math.Round(CTONMOCTb, 2)));
            //TextD.Text = "";


            #endregion
        }

        protected void G_DataBinding1(object sender, EventArgs e)
        {//����������� ���������� � ������ ��� � �����
            Grid.DataSource = GetDSForChart("G");
        }

        protected void LC_DataBinding(object sender, EventArgs e)
        {
            LC.DataSource = GetDSForChart("LC");
        }

        protected void RC_DataBinding(object sender, EventArgs e)
        {
            RC.DataSource = GetDSForChart("RC");
        }

        protected void G_InitializeLayout(object sender, LayoutEventArgs e)
        {//����� ����� ��� ����� �����
            e.Layout.HeaderTitleModeDefault = CellTitleMode.Never;
            e.Layout.Bands[0].Columns[0].Header.Caption = "������ �������";
            e.Layout.Bands[0].Columns[1].Header.Caption = "���������, ���. ���.(%)";
            e.Layout.Bands[0].Columns[2].Header.Caption = "����������, ��.(%)";
            //������� � �������
            e.Layout.Bands[0].Columns[1].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[1], "N3");
            
        }
//��� �������� ������ ��������� �������� � ��� ��� � ������� � ������� ������ �� ������� �������, ������� ������� ����� ��
        protected void LC_FillSceneGraph(object sender, Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e)
        {

            int textWidth = 400;
            int textHeight = 12;
            CellSet cs = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(DataProvider.GetQueryText("G"));
            for (int i = 0; i < cs.Axes[1].Positions.Count; i++)
            {
                Text textLabel = new Text();
                textLabel.PE.Fill = Color.Black;
                textLabel.bounds = new Rectangle(28, 289 + i * 19, textWidth, textHeight);
                textLabel.SetTextString(cs.Axes[1].Positions[i].Members[0].Caption);
                e.SceneGraph.Add(textLabel);



            }
        }

        protected void RC_FillSceneGraph(object sender, Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e)
        {
            int textWidth = 400;
            int textHeight = 12;
            CellSet cs = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(DataProvider.GetQueryText("RC"));
            for (int i = 0; i < cs.Axes[1].Positions.Count; i++)
            {
                Text textLabel = new Text();
                textLabel.PE.Fill = Color.Black;
                textLabel.bounds = new Rectangle(28, 289 + i * 19, textWidth, textHeight);
                textLabel.SetTextString(cs.Axes[1].Positions[i].Members[0].Caption);
                e.SceneGraph.Add(textLabel);
            }
        }
    }
}
