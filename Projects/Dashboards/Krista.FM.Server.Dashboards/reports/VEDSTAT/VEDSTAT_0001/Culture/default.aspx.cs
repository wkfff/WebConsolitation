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


using Infragistics.WebUI.UltraWebChart;
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.UltraChart.Core.Primitives;

using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Common;

namespace Krista.FM.Server.Dashboards.reports.VEDSTAT.VEDSTAT_00010._022
{
    public partial class Default : CustomReportPage
    {

        private CustomParam way_last_year { get { return (UserParams.CustomParam("way_last_year")); } }
        private CustomParam Pokaz { get { return (UserParams.CustomParam("Pokaz")); } }
        private CustomParam param { get { return (UserParams.CustomParam("param")); } }
        private CustomParam Lastdate { get { return (UserParams.CustomParam("lastdate")); } }
        private Int32 screen_width { get { return (int)Session["width_size"]; } }
        // �������� ������� ��� �������
        private CustomParam baseRegion { get { return (UserParams.CustomParam("baseRegion")); } }
        private CustomParam grid1Marks;
        private CustomParam grid2Marks;
        private CustomParam chart1Marks;
        private CustomParam chart2Marks;
        private CustomParam chart3Marks;
        private CustomParam textMarks;
        protected void FillSceneGraph(object sender, Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e)
        {
            int xOct = 0;
            int xNov = 0;
            Text decText = null;
            int year = int.Parse(UserComboBox.getLastBlock(Lastdate.Value));
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


        private void GridActiveRow(UltraWebGrid Grid, int index, bool active)
        {
            try
            {
                // �������� ��������� ������
                UltraGridRow row = Grid.Rows[index];
                // ������������� �� ��������, ���� ����������
                if (active)
                {
                    row.Activate();
                    row.Activated = true;
                    row.Selected = true;
                }
                // ��������� ��������� ��������� �������
                //selected_year.Value = row.Cells[0].Value.ToString();
                //UltraChart1.DataBind();
                //chart1_caption.Text = String.Format(chart1_title_caption, row.Cells[0].Value.ToString());
            }
            catch (Exception)
            {
                // ����� ������ ����� � ������� ...
            }
        }

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

        public static void setChartErrorFont(Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventArgs e)
        {
            e.Text = "� ��������� ������ ������ �����������";

            e.LabelStyle.FontColor = Color.LightGray;
            e.LabelStyle.VerticalAlign = StringAlignment.Center;
            e.LabelStyle.HorizontalAlign = StringAlignment.Center;
            e.LabelStyle.Font = new Font("Verdana", 30);
            //e.LabelStyle.Font.Size = 15;
        }
        private void setFont(int typ, Label lab, WebControl c)
        {
            lab.Font.Name = "arial";
            lab.Font.Size = typ;
            if (typ == 14) { lab.Font.Bold = 1 == 1; };
            if (typ == 10) { lab.Font.Bold = 1 == 1; };
            if (c != null) { lab.Width = c.Width; }
            if (typ == 18) { lab.Font.Bold = 1 == 1; };
            if (typ == 16)
            {
                lab.Font.Bold = 1 == 1;

                lab.Font.Size = FontUnit.Medium;
            };

        }
        public DataTable GetDSForChart(string sql)
        {
            DataTable dt = new DataTable();
            string s = DataProvider.GetQueryText(sql);
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(s, "����������", dt);
            return dt;
        }
        private String getLastDate(String way_ly)
        {
            try
            {
                way_last_year.Value = way_ly;
                CellSet cs = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(DataProvider.GetQueryText("last_date"));
                return cs.Axes[1].Positions[0].Members[0].ToString();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e);
            }
        }


        private string GetString_(string s)
        {
            string res = "";
            int i = 0;
            for (i = s.Length - 1; s[i] != ','; i--) ;
            for (int j = 0; j < i; j++)
            {
                res += s[j];
            }
            return res;


        }

        private void conf_Grid(int sizePercent, UltraWebGrid grid)
        {
            //grid.Width = screen_width * sizePercent / 100;
            grid.Bands[0].Columns[0].Header.Style.Wrap = 1 == 1;
            grid.Bands[0].Columns[0].CellStyle.Wrap = 1 == 1;
            // grid.Bands[0].Columns[0].Width = (int)((screen_width * sizePercent / 100) * 0.5);
            for (int i = 1; i < grid.Columns.Count; i++)
            {
                grid.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Left;
                grid.Bands[0].Columns[i].CellStyle.Wrap = 1 == 1;
                CRHelper.FormatNumberColumn(grid.Bands[0].Columns[i], "### ### ### ###.##");
                grid.Bands[0].Columns[i].Header.Style.Wrap = 1 == 1;
            }
            //grid.DisplayLayout.CellClickActionDefault = CellClickAction.Select;
            //grid.DisplayLayout.HeaderTitleModeDefault = CellTitleMode.Always;
            //grid.DisplayLayout.RowSelectorsDefault = RowSelectors.Yes;
            grid.DisplayLayout.GroupByBox.Hidden = 1 == 1;
            grid.DisplayLayout.NoDataMessage = "��� ������";
            grid.DisplayLayout.NoDataMessage = "��� ������";
        }
        private void conf_Chart(int sizePercent, Infragistics.WebUI.UltraWebChart.UltraChart chart, bool leg, Infragistics.UltraChart.Shared.Styles.ChartType typ)
        {
            chart.Width = screen_width * sizePercent / 100;
            //chart.SplineAreaChart.ChartText.Add(new Infragistics.UltraChart.Resources.Appearance.ChartTextAppearance(chart, -2, -2, 1 == 1, new Font("arial", 8), Color.Black, "<DATA_VALUE:#>", StringAlignment.Far, StringAlignment.Center, 0));
            if (leg)
            {
                chart.Legend.Visible = 1 == 1;
                chart.Legend.Location = Infragistics.UltraChart.Shared.Styles.LegendLocation.Bottom;
                chart.Legend.SpanPercentage = 15;

            }
            chart.FunnelChart3D.RadiusMax = 0.5;
            chart.FunnelChart3D.RadiusMin = 0.1;
            chart.FunnelChart3D.OthersCategoryText = "������";
            //chart.ChartType = typ;
            chart.PieChart3D.OthersCategoryText = "������";
            chart.DoughnutChart.OthersCategoryPercent = 1;
            chart.PieChart3D.OthersCategoryPercent = 1;
            chart.DoughnutChart.OthersCategoryText = "������";
            chart.DoughnutChart.RadiusFactor = 100;
            chart.DoughnutChart.InnerRadius = 25;
            chart.Transform3D.XRotation = 120;
            if (typ == Infragistics.UltraChart.Shared.Styles.ChartType.PieChart3D) { chart.Transform3D.XRotation = 30; }

            chart.Transform3D.YRotation = 0;
            chart.Transform3D.ZRotation = 0;
            //chart.PyramidChart.Axis = Infragistics.UltraChart.Shared.Styles.HierarchicalChartAxis.Radius;
            //chart.DeploymentScenario.FilePath = "../../TemporaryImages";
            //chart.DeploymentScenario.ImageURL = "../../TemporaryImages/Chart_#SEQNUM(100).png";
            chart.AreaChart.ChartText.Add(new Infragistics.UltraChart.Resources.Appearance.ChartTextAppearance(chart, -2, -2, 1 == 1, new Font("arial", 9, FontStyle.Bold), Color.Black, "<DATA_VALUE:###,##0.##>", StringAlignment.Far, StringAlignment.Center, 0));
            chart.Axis.Y.RangeMin = 0;
            chart.Axis.X.Margin.Far.Value = 3;
            chart.Axis.X.Margin.Near.Value = 3;
            //chart.Axis.Y.Margin.Far.Value = 3;
            //chart.Axis.Y.Margin.Near.Value = 3;
        }


        private string _GetString_(string s)
        {
            string res = "";
            int i = 0;
            for (i = s.Length - 1; s[i] != ','; i--) { res = s[i] + res; };

            return res;


        }

        /*  private string GetString_(string s)
          {
              string res = "";
              int i = 0;
              for (i = s.Length - 1; s[i] != ','; i--) ;
              for (int j = 0; j < i; j++)
              {
                  res += s[j];
              }
              return res;


          }*/
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
        string GetTexto(DataTable dt)
        {
            int count = 0;
            string ChildrenXZ = "";
            for (int i = 7;i<dt.Rows.Count-1;i++ )
            {
                ChildrenXZ += string.Format("&nbsp&nbsp&nbsp&nbsp{0} - <b>{1}</b> ��.<br>", dt.Rows[i][0].ToString(), dt.Rows[i][1].ToString());
            }
            string res ="";
            //string res = //@"�� ��������� �� <b>{0}</b> ���:<br>
//1. ����� ������������� (���������) ��������� - <b>{1}</b> ��. �� ���:<br>


//2. ����� ���������� ���������-���������� ���� � <b>{4}</b>  ��. �� ���:<br>


            //res = string.Format(res,
                //,
                //dt.Rows[0][1].ToString() == "" ? "0" : dt.Rows[0][1].ToString(),
                // == "" ? "0" : dt.Rows[1][1].ToString(),
                //dt.Rows[2][1].ToString() == "" ? "0" : dt.Rows[2][1].ToString(),
                //dt.Rows[3][1].ToString() == "" ? "0" : dt.Rows[3][1].ToString(),
                //dt.Rows[4][1].ToString() == "" ? "0" : dt.Rows[4][1].ToString(),
                //dt.Rows[5][1].ToString() == "" ? "0" : dt.Rows[5][1].ToString(),
                //dt.Rows[6][1].ToString() == "" ? "0" : dt.Rows[6][1].ToString(),                
                
                res += string.Format(@"�� ��������� �� <b>{0}</b> ���:<br>",dt.Columns[1].Caption);
            if (dt.Rows[0][1].ToString() !="")
            {
                count++;
                res +=string.Format("{0}. ����� ������������� (���������) ��������� � <b>{1}</b> ��. �� ���:<br>",count,dt.Rows[0][1].ToString());
            }
                res +=string.Format("&nbsp&nbsp&nbsp&nbsp������� ���������� � <b>{0}</b> ��.<br>",dt.Rows[1][1].ToString());
                res +=string.Format("&nbsp&nbsp&nbsp&nbsp������������ ���������� � <b>{0}</b> ��.<br>",dt.Rows[2][1].ToString());
            if (dt.Rows[3][1].ToString() !="")
            {
                count++;
                res +=string.Format("{0}. ����� ���������� ���������-���������� ���� � <b>{1}</b>  ��. �� ���:<br>",count,dt.Rows[3][1].ToString());
            }
            	res +=string.Format("{0}",ChildrenXZ);
            if (dt.Rows[4][1].ToString() !="")
            {
count++;
                res +=string.Format("{0}. ����� ����������� � �������������� ���� � <b>{1}</b> ��.<br>", count,dt.Rows[4][1].ToString());
            }
            if (dt.Rows[5][1].ToString() !="")
            {
count++;
                res +=string.Format("{0}. ����������� ���������� ������������� (���������) ��������� � <b>{1}</b>  ���.<br>", count,dt.Rows[5][1].ToString());
            }
            if (dt.Rows[6][1].ToString() !="")
            {
count++;
                res +=string.Format("{0}. ����������� ���������� ����� �������� � ������ (��� �������� ����������) - <b>{1}</b> ���.", count,dt.Rows[6][1].ToString());
            }
                





            //);
                




            return res;
        }




        string BN = "";
        protected override void Page_Load(object sender, EventArgs e)
        {
           // try
            {

                System.Web.HttpBrowserCapabilities myBrowserCaps = Request.Browser;
                BN = ((System.Web.Configuration.HttpCapabilitiesBase)myBrowserCaps).Browser.ToUpper();

                if (!Page.IsPostBack)
                {
                    RegionSettingsHelper.Instance.SetWorkingRegion(RegionSettings.Instance.Id);
                    baseRegion.Value = RegionSettingsHelper.Instance.RegionBaseDimension;
                    WebAsyncRefreshPanel1.AddLinkedRequestTrigger(GT);
                    WebAsyncRefreshPanel2.AddLinkedRequestTrigger(GB);
                    grid1Marks = UserParams.CustomParam("grid1Marks");
                    grid2Marks = UserParams.CustomParam("grid2Marks");
                    chart1Marks = UserParams.CustomParam("chart1Marks");
                    chart2Marks = UserParams.CustomParam("chart2Marks");
                    chart3Marks = UserParams.CustomParam("chart3Marks");
                    textMarks = UserParams.CustomParam("textMarks");
                    grid1Marks = ForMarks.SetMarks(grid1Marks, ForMarks.Getmarks("grid1_mark_"), true);
                    grid2Marks = ForMarks.SetMarks(grid2Marks, ForMarks.Getmarks("grid2_mark_"), true);
                    chart1Marks = ForMarks.SetMarks(chart1Marks, ForMarks.Getmarks("chart1_mark_"), true);
                    chart2Marks = ForMarks.SetMarks(chart2Marks, ForMarks.Getmarks("chart2_mark_"), true);
                    chart3Marks = ForMarks.SetMarks(chart3Marks, ForMarks.Getmarks("chart3_mark_"), true);
                    textMarks = ForMarks.SetMarks(textMarks, ForMarks.Getmarks("text_mark_"), true);
                    // Request.Browser;
                    //((System.Web.Configuration.HttpCapabilitiesBase) (Request.Browser)).Browser.ToString
                    //(((System.Web.Configuration.HttpCapabilitiesBase)).Browser.ToUpper().IndexOf("IE") >= 0)
                    //{ }

                    Lastdate.Value = ELV(getLastDate(RegionSettingsHelper.Instance.GetPropertyValue("way_last_year_mark")));
                    GT.DataBind();
                    GB.DataBind();
                    GridActiveRow(GT, 0, true);
                    GridActiveRow(GB, GB.Rows.Count - 1, true);
                    GB.Rows[GB.Rows.Count - 1].Cells[1].Activate();
                    GB.Rows[GB.Rows.Count - 1].Cells[1].Selected = 1 == 1;
                    GB.Rows[GB.Rows.Count - 1].Cells[1].Activated = 1 == 1;
                    GB.DisplayLayout.RowSelectorsDefault = RowSelectors.No;

                    Pokaz.Value = GT.Rows[0].Cells[0].Text;
                    Label3.Text =
"�������� ���������� " + '"' + Pokaz.Value + '"' + ", ������ �����������";
                    //Label8.Text = RegionSettingsHelper.Instance.GetPropertyValue("chart1_title1");
                    UltraChart1.DataBind();
                    Pokaz.Value = GB.Rows[GB.Rows.Count - 1].Cells[0].Text;
                    //e.Row.Cells[0].Text;
                    Label4.Text = string.Format("����������� ����� ��������� �� ����� ���������� ���������-���������� ���� � {0} ����, %", Pokaz.Value);//"����������� ����� ��������� �� ����� ���������� ���������-���������� ���� � " + Pokaz.Value + " ����, ���������";

                    UltraChart2.DataBind();
                    Pokaz.Value = GB.Columns[1].Header.Key;
                    Label5.Text = string.Format("���� ����� ���������� ������ ��������� ����������: {0}�, %", Pokaz.Value);  //"���� ����� ����� ��������� ���������� " + Pokaz.Value;//Label5.Text = "���� ����� ����� ��������� ���������� " + Pokaz.Value;
                    UltraChart3.DataBind();
                    //DataTable DT = GetDSForChart("TT");
                    Texto.Text = GetTexto(GetDSForChart("TT"));
                    //Label7.Text = String.Format(RegionSettingsHelper.Instance.GetPropertyValue("text_title"), Lastdate.Value);
                    //Label7.Text = "�� ��������� �� <b>" + Lastdate.Value + "</b> ���: <br/>";
                    Label1.Text = RegionSettingsHelper.Instance.GetPropertyValue("grid1_title");
                    Label2.Text = RegionSettingsHelper.Instance.GetPropertyValue("grid2_title");
                    //Label9.Text=RegionSettingsHelper.Instance.GetPropertyValue("chart2_title1");
                    //Label10.Text=RegionSettingsHelper.Instance.GetPropertyValue("chart3_title1");
                    //            Texto.Text =
                    //"1. ����� ������������� (���������) ��������� - <b>" + DT.Rows[0].ItemArray[1].ToString() + "</b> ��. �� ���:<br/>" +
                    //     "   &nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp     ������� ���������� - <b>" + DT.Rows[1].ItemArray[1].ToString() + "</b> ��.<br/>";

                    //            Texto.Text += "   &nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp     ������������ ���������� - <b>" + DT.Rows[2].ItemArray[1].ToString() + "</b> ��.<br/>" +
                    //"2. ����� ���������� ���������-���������� ���� � <b>" + DT.Rows[3].ItemArray[1].ToString() + "</b> ��. �� ���:<br/>";
                    //            for (int i = 7; i < DT.Rows.Count; i++)
                    //            {
                    //                Texto.Text += "  &nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp      " + DT.Rows[i].ItemArray[0].ToString() + " - <b>" + DT.Rows[i].ItemArray[1].ToString() + "</b> ��. <br/>";
                    //            };
                    //            Texto.Text += "3. ����� ����������� � �������������� ���� � <b>" + DT.Rows[4].ItemArray[1].ToString() + "</b> ��.<br/>" +
                    //"4. ����������� ���������� ������������� (���������) ��������� � <b>" + DT.Rows[5].ItemArray[1].ToString() + " </b> ���.<br/>";
                    //            Texto.Text += "5. ����������� ���������� ����� �������� � ������ (��� �������� ����������) - <b>" + DT.Rows[6].ItemArray[1].ToString() + "</b> ���.";

                    int size = 10;
                    // setFont(size, Label1, GT);
                    //setFont(size, Label2, GB);
                    Label6.Text = RegionSettingsHelper.Instance.GetPropertyValue("page_title");
                    //setFont(size, Label3, UltraChart1);


                    // setFont(size, Label4, UltraChart2);

                    // setFont(size, Label5, UltraChart3);


                    // setFont(16, Label6, null);
                    //Label6.Text = "������������ ���������� ��������";

                    //setFont(size, Texto, null);
                    if (BN == "FIREFOX")
                    {
                        GT.Height = Unit.Empty;
                        GB.Height = Unit.Empty;// 345;
                    }
                    if (BN == "APPLEMAC-SAFARI")
                    {
                        GT.Height = Unit.Empty;// 308;
                        GB.Height = Unit.Empty;// 300;
                    }
                    CellSet CLS = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(DataProvider.GetQueryText("GBE"));
                    for (int i = 0; i < CLS.Cells.Count; i++)
                    {
                        GB.Columns[i + 1].Header.Caption += ", " + CLS.Cells[i].Value.ToString().ToLower();

                    }
                    CLS = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(DataProvider.GetQueryText("GTE"));
                    try
                    {
                        for (int i = 0; i < CLS.Cells.Count; i++)
                        {
                            GT.Rows[i].Cells[0].Text += ", " + CLS.Cells[i].Value.ToString().ToLower();

                        }
                    }

                    catch { };


                }


            }
            //catch { }
            ///Label1.Text = Request.Browser.ToString();
            ///

            //System.Web.HttpBrowserCapabilities C = Request.Browser;
            //Label1.Text = ((System.Web.Configuration.HttpCapabilitiesBase)C).Browser.ToUpper();
        }

        protected void GT_InitializeLayout(object sender, LayoutEventArgs e)
        {
            conf_Grid(45, GT);
            double GW = screen_width * 42 / 100;
            if (BN == "IE")
            {
                e.Layout.Bands[0].Columns[0].Width = (int)(GW * 0.7);
                e.Layout.Bands[0].Columns[1].Width = (int)(GW * 0.28);
                //Label1.Width = (int)(GW - 50);
                //Label2.Width = (int)(GW - 50);
            }
            if (BN == "APPLEMAC-SAFARI")
            {
                e.Layout.Bands[0].Columns[0].Width = (int)(GW * 0.7);
                e.Layout.Bands[0].Columns[1].Width = (int)(GW * 0.28);
                //Label1.Width = (int)(GW - 50);
                //Label2.Width = (int)(GW - 50);
                //BN = "FIREFOX";
            }
            if (BN == "FIREFOX")
            {
                e.Layout.Bands[0].Columns[0].Width = (int)(GW * 0.7);
                e.Layout.Bands[0].Columns[1].Width = (int)(GW * 0.25);
                //Label1.Width = (int)(GW - 50);
                //Label2.Width = (int)(GW - 50);
            }
            e.Layout.Bands[0].Columns[1].Header.Style.HorizontalAlign = HorizontalAlign.Center;
        }


        protected void GT_DataBinding(object sender, EventArgs e)
        {
            GT.DataSource = GetDSForChart("GT");
        }

        protected void GB_DataBinding(object sender, EventArgs e)
        {
            GB.DataSource = GetDSForChart("GB");
        }

        protected void UltraChart1_DataBinding(object sender, EventArgs e)
        {
            DataTable dt = GetDSForChart("CT");
            float min, max;
            min = max = float.Parse(dt.Rows[0].ItemArray[1].ToString());
            for (int j = 0; j < dt.Rows.Count; j++)
            {
                for (int i = 1; i < dt.Rows[j].ItemArray.Length; i++)
                {
                    if (float.Parse(dt.Rows[j].ItemArray[i].ToString()) < min)
                    {
                        min = float.Parse(dt.Rows[j].ItemArray[i].ToString());
                    }
                    if (float.Parse(dt.Rows[j].ItemArray[i].ToString()) > max)
                    {
                        max = float.Parse(dt.Rows[j].ItemArray[i].ToString());
                    }
                }
            }

            UltraChart1.Axis.X.RangeMax = max * 1.1;
            UltraChart1.Axis.X.RangeMin = min * 0.8;
            UltraChart1.Axis.X.RangeType = Infragistics.UltraChart.Shared.Styles.AxisRangeType.Custom;
            UltraChart1.Axis.X2.RangeMax = max * 1.1;
            UltraChart1.Axis.X2.RangeMin = min * 0.8;
            UltraChart1.Axis.X2.RangeType = Infragistics.UltraChart.Shared.Styles.AxisRangeType.Custom;
            UltraChart1.DataSource = dt;

            int width = 50;
            if (BN == "IE")
            {
                width = 50;
            }
            if (BN == "APPLEMAC-SAFARI")
            {
                width = 50;

                //BN = "FIREFOX";
            }
            if (BN == "FIREFOX")
            {
                width = 50;
                if (screen_width == 1024)
                {
                    width = 49;
                }
            }



            conf_Chart(width, UltraChart1, false, Infragistics.UltraChart.Shared.Styles.ChartType.BarChart3D);

            UltraChart1.Tooltips.FormatString = "<b><DATA_VALUE:00.##></b>, " + Ed.ToLower();
        }

        protected void UltraChart2_DataBinding(object sender, EventArgs e)
        {
            UltraChart2.DataSource = GetDSForChart("CB1");
            int width = 50;
            if (BN == "IE")
            {
                width = 50;
            }
            if (BN == "APPLEMAC-SAFARI")
            {
                width = 50;
            }
            if (BN == "FIREFOX")
            {
                width = 50;
                if (screen_width == 1024)
                {
                    width = 49;
                }
            }
            conf_Chart(width, UltraChart2, 1 == 1, Infragistics.UltraChart.Shared.Styles.ChartType.PieChart3D);
            UltraChart2.Tooltips.FormatString = "<ITEM_LABEL>, ���������" + " <b><DATA_VALUE:00.##></b>";
            UltraChart2.PieChart3D.PieThickness = 23;

        }

        protected void UltraChart3_DataBinding(object sender, EventArgs e)
        {
            try
            {
                DataTable DT = GetDSForChart("CB2");
                decimal res = 0;
                object[] MO = new object[DT.Columns.Count];

                MO[0] = DT.Rows[0].ItemArray[0];

                for (int i = DT.Columns.Count - 1; i > 1; i--)
                {
                    MO[i] = ((decimal)DT.Rows[0].ItemArray[i] / (decimal)DT.Rows[0].ItemArray[i - 1]) * 100;
                    //DT.Rows[0].ItemArray[i] = res.ToString();

                }
                MO[1] = 0;
                DT.Rows[0].ItemArray = MO;
                DT.Columns.Remove(DT.Columns[1]);
                UltraChart3.DataSource = DT;

                if (BN == "IE")
                {
                    conf_Chart(96, UltraChart3, false, Infragistics.UltraChart.Shared.Styles.ChartType.AreaChart);
                }
                if (BN == "APPLEMAC-SAFARI")
                {
                    conf_Chart(96, UltraChart3, false, Infragistics.UltraChart.Shared.Styles.ChartType.AreaChart);
                }
                if (BN == "FIREFOX")
                {
                    conf_Chart(97, UltraChart3, false, Infragistics.UltraChart.Shared.Styles.ChartType.AreaChart);

                }

            }
            catch
            { }


        }
        string Ed = "������ �����������";
        protected void GT_ActiveRowChange(object sender, RowEventArgs e)
        {
            Ed = _GetString_(e.Row.Cells[0].Text);
            Pokaz.Value = GetString_(e.Row.Cells[0].Text);
            Label3.Text = String.Format(RegionSettingsHelper.Instance.GetPropertyValue("chart1_title"), Pokaz.Value, Ed);
            UltraChart1.DataBind();

        }

        protected void GB_ActiveRowChange(object sender, RowEventArgs e)
        {


        }

        protected void GB_Click(object sender, ClickEventArgs e)
        {
            int CellIndex = 0;
            try
            {
                CellIndex = e.Cell.Column.Index;
            }
            catch
            {
                CellIndex = e.Column.Index;
            }

            Pokaz.Value = GB.Rows[e.Cell.Row.Index].Cells[0].Text;
            //Label4.Text = String.Format(RegionSettingsHelper.Instance.GetPropertyValue("chart2_title"), Pokaz.Value);
            Label4.Text = string.Format("����������� ����� ��������� �� ����� ���������� ���������-���������� ���� � {0} ����, %", Pokaz.Value);//"����������� ����� ��������� �� ����� ���������� ���������-���������� ���� � " + Pokaz.Value + " ����, ���������";
            UltraChart2.DataBind();

            if (e.Cell.Column.Index > 0)
                Pokaz.Value = GB.Columns[CellIndex].Header.Key;
            else
                Pokaz.Value = GB.Columns[CellIndex + 1].Header.Key;
            Label5.Text = string.Format("���� ����� ���������� ������ ��������� ����������: {0}�, %", Pokaz.Value);  //"���� ����� ����� ��������� ���������� " + Pokaz.Value;//Label5.Text=String.Format(RegionSettingsHelper.Instance.GetPropertyValue("chart3_title"),Pokaz.Value);
            UltraChart3.DataBind();
        }

        protected void GB_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.Bands[0].Columns[0].CellStyle.BorderDetails.ColorLeft = Color.Gray;
            conf_Grid(45, GB);
            double GW = screen_width * 39.3 / 100; ;
            if (BN == "IE")
            {
                for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
                {
                    e.Layout.Bands[0].Columns[i].Width = (int)(GW / e.Layout.Bands[0].Columns.Count);
                }

            }
            if (BN == "APPLEMAC-SAFARI")
            {
                for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
                {
                    e.Layout.Bands[0].Columns[i].Width = (int)((GW + 20) / e.Layout.Bands[0].Columns.Count);
                }
            }
            if (BN == "FIREFOX")
            {
                for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
                {
                    e.Layout.Bands[0].Columns[i].Width = (int)((GW + 10) / e.Layout.Bands[0].Columns.Count);
                }
            }

        }

        protected void UltraChart3_InvalidDataReceived(object sender, Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventArgs e)
        {
            setChartErrorFont(e);
        }

        protected void GB_ActiveCellChange(object sender, CellEventArgs e)
        {
            int CellIndex = 0;
            int RowIndex = 0;
            try
            {
                CellIndex = e.Cell.Column.Index;
            }
            catch
            {
            }

            try
            {
                RowIndex = e.Cell.Row.Index;
            }
            catch
            {
            }


            Pokaz.Value = GB.Rows[RowIndex].Cells[0].Text;

            Label4.Text = string.Format("����������� ����� ��������� �� ����� ���������� ���������-���������� ���� � {0} ����, %", Pokaz.Value);//"����������� ����� ��������� �� ����� ���������� ���������-���������� ���� � " + Pokaz.Value + " ����, ���������";//Label4.Text = "����������� ����� ��������� �� ����� ���������� ���������-���������� ���� � " + Pokaz.Value + " ����, ���������";

            UltraChart2.DataBind();
            if (CellIndex > 0)
                Pokaz.Value = GB.Columns[CellIndex].Header.Key;
            else
                Pokaz.Value = GB.Columns[CellIndex + 1].Header.Key;
            Label5.Text = string.Format("���� ����� ���������� ������ ��������� ����������: {0}�, %", Pokaz.Value);  //"���� ����� ����� ��������� ���������� " + Pokaz.Value;
            UltraChart3.DataBind();
        }

        protected void GT_DataBound(object sender, EventArgs e)
        {

        }
        int addY = 0;
        protected void UltraChart2_ChartDrawItem(object sender, Infragistics.UltraChart.Shared.Events.ChartDrawItemEventArgs e)
        {
            //UltraChart UltraChart_ = (UltraChart)sender;
            //������������� ������ ������ ������� 
            Infragistics.UltraChart.Core.Primitives.Text text = e.Primitive as Infragistics.UltraChart.Core.Primitives.Text;
            if ((text != null) && !(string.IsNullOrEmpty(text.Path)) && text.Path.EndsWith("Legend"))
            {
                int widthLegendLabel;
                //UltraChart_.Legend.Location =
                if ((UltraChart2.Legend.Location == Infragistics.UltraChart.Shared.Styles.LegendLocation.Top) || (UltraChart2.Legend.Location == Infragistics.UltraChart.Shared.Styles.LegendLocation.Bottom))
                {
                    widthLegendLabel = (int)UltraChart2.Width.Value - 20;

                }
                else
                {
                    widthLegendLabel = ((int)UltraChart2.Legend.SpanPercentage * (int)UltraChart2.Width.Value / 100) - 20;
                }

                widthLegendLabel -= UltraChart2.Legend.Margins.Left + UltraChart2.Legend.Margins.Right;
                if (text.labelStyle.Trimming != StringTrimming.None)
                {
                    text.bounds.Width = widthLegendLabel;



                }

                //text.labelStyle.Dy += addY;
                //string Capt = text.GetTextString();
                //if (Capt.Length > 30)
                //{
                //    int i = 30;
                //    for (; Capt[i] != ' '; i--) { }

                //    text.SetTextString(Capt.Insert(i, "\n"));
                //    //������)
                //    text.labelStyle.Dy -= 5;
                //    addY += 5;
                //}





            }

        }

        protected void UltraChart1_DataBound(object sender, EventArgs e)
        {

        }

    }
}
