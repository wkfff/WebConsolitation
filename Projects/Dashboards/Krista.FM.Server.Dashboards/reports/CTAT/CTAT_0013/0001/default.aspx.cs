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
using Krista.FM.Server.Dashboards.Common;
using Microsoft.AnalysisServices.AdomdClient;


using Infragistics.WebUI.UltraWebChart;
using Infragistics.UltraChart.Core.Primitives;

using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.CTAT.CTAT_0013._0001
{
    public partial class _default : CustomReportPage
    {






        public void SetGridColumn(UltraWebGrid grid, double sizePercent)
        {

            double Width = CustomReportConst.minScreenWidth * (sizePercent / 100);
            grid.Width = CRHelper.GetGridWidth(Width);
            double WidthColumn = Width / grid.Columns.Count;

            for (int i = 0; i < grid.Columns.Count; i++)
            {
                grid.Columns[i].Width = CRHelper.GetColumnWidth(WidthColumn * 0.92);
                grid.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Left;
                grid.Bands[0].Columns[i].CellStyle.Wrap = 1 == 1;
                CRHelper.FormatNumberColumn(grid.Bands[0].Columns[i], "### ### ### ###.##");
                grid.Bands[0].Columns[i].Header.Style.Wrap = 1 == 1;
            }
            grid.DisplayLayout.GroupByBox.Hidden = 1 == 1;
            //grid.DisplayLayout.RowSelectorsDefault = RowSelectors.Yes;
            //grid.DisplayLayout.RowSelectorStyleDefault.Width = 0;
            grid.Columns[0].CellStyle.BorderDetails.ColorLeft = Color.Silver;
            grid.Columns[0].Header.Caption = "Год";
            //grid.Columns[0].
            grid.DisplayLayout.NoDataMessage = "В настоящий момент данные отсутствуют";
            //grid.OnClick += new UltraWebGrid.OnClick(CLIK);
            grid.Height = 0;
        }




        protected ArrayList GenMounth()
        {
            ArrayList Mounth = new ArrayList();
            Mounth.Add("[Период].[Год Квартал Месяц].[Год].[{0}].[Полугодие 1].[Квартал 1].[Январь]");
            Mounth.Add("[Период].[Год Квартал Месяц].[Год].[{0}].[Полугодие 1].[Квартал 1].[Февраль]");
            Mounth.Add("[Период].[Год Квартал Месяц].[Год].[{0}].[Полугодие 1].[Квартал 1].[Март]");
            Mounth.Add("[Период].[Год Квартал Месяц].[Год].[{0}].[Полугодие 1].[Квартал 2].[Апрель]");
            Mounth.Add("[Период].[Год Квартал Месяц].[Год].[{0}].[Полугодие 1].[Квартал 2].[Май]");
            Mounth.Add("[Период].[Год Квартал Месяц].[Год].[{0}].[Полугодие 1].[Квартал 2].[Июнь]");
            Mounth.Add("[Период].[Год Квартал Месяц].[Год].[{0}].[Полугодие 2].[Квартал 3].[Июль]");
            Mounth.Add("[Период].[Год Квартал Месяц].[Год].[{0}].[Полугодие 2].[Квартал 3].[Август]");
            Mounth.Add("[Период].[Год Квартал Месяц].[Год].[{0}].[Полугодие 2].[Квартал 3].[Сентябрь]");
            Mounth.Add("[Период].[Год Квартал Месяц].[Год].[{0}].[Полугодие 2].[Квартал 4].[Октябрь]");
            Mounth.Add("[Период].[Год Квартал Месяц].[Год].[{0}].[Полугодие 2].[Квартал 4].[Ноябрь]");
            Mounth.Add("[Период].[Год Квартал Месяц].[Год].[{0}].[Полугодие 2].[Квартал 4].[Декабрь]");
            return Mounth;
        }

        public void SetBeautifulChart(UltraChart chart, bool legend, Infragistics.UltraChart.Shared.Styles.ChartType ChartType, int legendPercent, Infragistics.UltraChart.Shared.Styles.LegendLocation LegendLocation, double SizePercent)
        {
            chart.Width = CRHelper.GetChartWidth((CustomReportConst.minScreenWidth * (SizePercent / 100)));
            chart.SplineAreaChart.ChartText.Add(new Infragistics.UltraChart.Resources.Appearance.ChartTextAppearance(chart, -2, -2, 1 == 1, new Font("arial", 8), Color.Black, "<DATA_VALUE:#>", StringAlignment.Far, StringAlignment.Center, 0));
            if (legend)
            {
                chart.Legend.Visible = 1 == 1;
                chart.Legend.Location = LegendLocation;
                chart.Legend.SpanPercentage = legendPercent;

            }
            chart.FunnelChart3D.RadiusMax = 0.5;
            chart.FunnelChart3D.RadiusMin = 0.1;
            chart.FunnelChart3D.OthersCategoryText = "Прочие";
            chart.ChartType = ChartType;
            chart.Axis.X.Margin.Near.Value = 4;

            //доделать AXis
            chart.Transform3D.Scale = 75;
            chart.Axis.X.Extent = 10;
            chart.Axis.Y.Extent = 50;
            chart.Axis.X.Labels.Orientation = Infragistics.UltraChart.Shared.Styles.TextOrientation.Horizontal;
            chart.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:###,##0.##>";
            //chart.Axis.X.
            chart.Axis.Z.Labels.Visible = 1 == 2;

            if (ChartType == Infragistics.UltraChart.Shared.Styles.ChartType.PieChart3D)
            {
                chart.Transform3D.ZRotation = 0;
                chart.Transform3D.YRotation = 0;
                chart.Transform3D.XRotation = 30;
                chart.Transform3D.Scale = 90;
                chart.PieChart3D.OthersCategoryPercent = 2;
                chart.PieChart3D.OthersCategoryText = "Прочие";
                chart.PieChart3D.Labels.FormatString = "<DATA_VALUE:### ##0.##>";


            };
            if (ChartType == Infragistics.UltraChart.Shared.Styles.ChartType.DoughnutChart) { chart.DoughnutChart.ChartText.Add(new Infragistics.UltraChart.Resources.Appearance.ChartTextAppearance(chart, -2, -2, 1 == 1, new Font("arial", 8, FontStyle.Bold), Color.Black, "<DATA_VALUE:#>", StringAlignment.Center, StringAlignment.Center, 50)); }
            if (ChartType == Infragistics.UltraChart.Shared.Styles.ChartType.AreaChart)
            {
                //chart.Axis.X.Labels.Font = new Font("arial", 8, FontStyle.Bold);
                //chart.Axis.X.Labels.FontColor = Color.Black;
                chart.Axis.X.Margin.Near.Value = 2;

                chart.AreaChart.ChartText.Add(new Infragistics.UltraChart.Resources.Appearance.ChartTextAppearance(chart, -2, -2, 1 == 1, new Font("arial", 8, FontStyle.Bold), Color.Black, "<DATA_VALUE:### #0.#>", StringAlignment.Far, StringAlignment.Center, 0));
            };
        }
        public static void setChartErrorFont(Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventArgs e)
        {
            e.Text = "В настоящий момент данные отсутствуют";

            e.LabelStyle.FontColor = Color.LightGray;
            e.LabelStyle.VerticalAlign = StringAlignment.Center;
            e.LabelStyle.HorizontalAlign = StringAlignment.Center;
            e.LabelStyle.Font = new Font("Verdana", 30);
        }
        private void setFont(int typ, Label lab)
        {
            lab.Font.Name = "arial";
            lab.Font.Size = typ;
            if (typ == 14) { lab.Font.Bold = 1 == 1; };
            if (typ == 10) { lab.Font.Bold = 1 == 1; };
            if (typ == 18) { lab.Font.Bold = 1 == 1; };
        }


        protected override void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                Page.Title = "Отчёт";

                for (int i = 2006; i < 2015; i++)
                {
                    D1.Items.Add(i.ToString());

                };
                for (int i = 2007 ; i < 2016; i++)
                {
                    D2.Items.Add(i.ToString());

                };

                D1.SelectedIndex = 0;
                D2.SelectedIndex = 2;
                LD = int.Parse(D2.SelectedItem.Text)+1;
                FD = int.Parse(D1.SelectedItem.Text);


                setFont(10, Label3);
                setFont(10, Label4);
                Label4.Text = "Динамика темпа роста задолженности организаций в % к аналогичному периоду предыдущего года";//"+D2.SelectedItem.Text+")";;
              //  Label5.Text = "Основные показатели предприятий и организаций субъекта РФ Саратовская обл.";
             //   setFont(16, Label5);

                G.DataBind();

                G.Rows[0].Cells[0].ColSpan = 13;
                G.Rows[LD - FD + 1].Cells[0].ColSpan = 13;
                C.DataBind();

                WebAsyncRefreshPanel1.AddLinkedRequestTrigger(Button1);


                //for (int i = 0; i < G.Columns.Count; i++)
                //{

                //    CRHelper.FormatNumberColumn(G.Bands[0].Columns[i], "### ### ### ###.##");
                //}

                //  G.Width = CustomReportConst.minScreenWidth;

                for (int i = 1; i < G.Columns.Count; i++)
                {

                    //CRHelper.FormatNumberColumn(G.Bands[0].Columns[i], "### ### ### ###.##");
                    G.Columns[i].Format = "### ### ### ###.##";
                }
            }
        }
        int LD, FD;
        protected string GenSqlForG(string param)
        {
            ArrayList Mounth = GenMounth();
            string WithSQL, SelectSQL = "";

            WithSQL = "With ";
            SelectSQL = "Select {";
            for (int i = FD; i != LD; i++)
            {
                for (int j = 0; j < 11; j++)
                {
                    string s = " [Период].[Год Квартал Месяц].[" + i.ToString() + '-' + j.ToString() + "] ";
                    
                    WithSQL += " member "+s;
                    WithSQL += " as ";
                    WithSQL += " '" + string.Format((Mounth[j].ToString()), i.ToString())+"' ";

                    SelectSQL += s + " , ";



                }
                string s2 = " [Период].[Год Квартал Месяц].[" + i.ToString() + '-' + "11] ";

                WithSQL += " member " + s2;
                WithSQL += " as ";
                WithSQL += " '" + string.Format((Mounth[11].ToString()), i.ToString()) + "' ";

                if (i+1 == LD) { SelectSQL += s2+"  "; }
                else { SelectSQL += s2 + " , "; }
                

//G.Rows[0].Cells[0].
            }

            string AllSQL = @"    } on rows,
    {
      
"+param+@"
       
      
    } on columns    
from [СТАТ_Организации_Финансы организаций]   
where    
    (
        [Территории].[РФ Карта].[Субъект РФ].[Саратовская обл.],
        [Источники данных].[Источник].[СТАТ Отчетность - Облстат],
        [Measures].[Нарастающий итог],
        [Группировки].[Организации_Финансы организаций].[Без группировки]   
    )  ";

            return WithSQL + SelectSQL+AllSQL;


        }
        protected string GenSqlForC()
        {
            ArrayList Mounth = GenMounth();
            string WithSQL, SelectSQL = "";
            Page.Title = "Отчёт";
            WithSQL = "With ";
            SelectSQL = "Select {";
            for (int i = FD; i != LD; i++)
            {
                for (int j = 0; j < 11; j++)
                {
                    string s = " [Период].[Год Квартал Месяц].["+ELV(Mounth[j].ToString()) + "-"+i.ToString()+" ] ";

                    WithSQL += " member " + s;
                    WithSQL += " as ";
                    int ii = i - 1;
                    WithSQL += " '" + string.Format((Mounth[j].ToString()), i.ToString()) + "/" + string.Format((Mounth[j].ToString()), ii.ToString()) + "' ";

                    SelectSQL += s + " , ";



                }
                string s2 = " [Период].[Год Квартал Месяц].[" + ELV(Mounth[11].ToString()) + "-" + i.ToString() + " ] ";

                WithSQL += " member " + s2;
                WithSQL += " as ";
                int iii = i - 1;
                WithSQL += " '" + string.Format((Mounth[11].ToString()), i.ToString()) + "/" + string.Format((Mounth[11].ToString()), iii.ToString()) + "' ";

                if (i + 1 == LD) { SelectSQL += s2 + "  "; }
                else { SelectSQL += s2 + " , "; }



            }

            string AllSQL = @"    } on columns ,
    {
      

       [Организации].[Финансы организаций].[Кредиторская задолженность организаций],
[Организации].[Финансы организаций].[Дебиторская задолженность организаций]
      
    }  on rows  
from [СТАТ_Организации_Финансы организаций]   
where    
    (
        [Территории].[РФ Карта].[Субъект РФ].[Саратовская обл.],
        [Источники данных].[Источник].[СТАТ Отчетность - Облстат],
        [Measures].[Нарастающий итог],
        [Группировки].[Организации_Финансы организаций].[Без группировки]   
    )  ";

            return WithSQL + SelectSQL + AllSQL;


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


        protected void G_DataBinding(object sender, EventArgs e)
        {
            int d = FD;
            string s = GenSqlForG("[Организации].[Финансы организаций].[Кредиторская задолженность организаций]");
            
            CellSet CS = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(s);
            DataTable DT = new DataTable();
            DT.Columns.Add("asdasd");
            for (int i = 0;i<12;i++)
            {
                DT.Columns.Add(ELV(GenMounth()[i].ToString())+"");
            }



            DT.Rows.Add("Кредиторская задолженность организаций");
            

            for (int j = 0; j < (CS.Cells.Count / 12); j+=1)
            {
                object[] rowa = new object[13];
                rowa[0] = d.ToString();
                d++;
                for (int i = 0; i < 12; i++)
                {
                    try
                    {
                        decimal b = ((decimal)((decimal.Parse(CS.Cells[(j * 12) + i].Value.ToString())) / 1000000));
                        //rowa[i + 1] = b.ToString();//(b/1000).ToString()+" "+(b-(b/1000)).ToString();
                        rowa[i + 1] = ((int)(b / 1000)).ToString() + " " + ((int)((b - ((int)((b / 1000)) * 1000)))).ToString();
                    }
                    catch { rowa[i + 1] = ""; }

                }
                DT.Rows.Add(rowa);
            }

           s = GenSqlForG("[Организации].[Финансы организаций].[Дебиторская задолженность организаций]");
           d = FD;
            CS = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(s);
            DT.Rows.Add("Дебиторская задолженность организаций");
            for (int j = 0; j < (CS.Cells.Count / 12); j += 1)
            {
                object[] rowa = new object[13];
                rowa[0] = d.ToString();
                d++;
                for (int i = 0; i < 12; i++)
                {
                    try
                    {
                        decimal b = ((decimal)((decimal.Parse(CS.Cells[(j * 12) + i].Value.ToString())) / 1000000));
                       // rowa[i + 1] = b.ToString();//((int)(b / 1000)).ToString() + " " + (b - (int)((b / 1000))*1000 ).ToString();
                        rowa[i + 1] = ((int)(b / 1000)).ToString() + " " + ((int)((b - ((int)((b / 1000))*1000)))).ToString();
                    }
                    catch { rowa[i + 1] = ""; }


                }
              DT.Rows.Add(rowa);
            }
            setFont(10, Label3);

            Label3.Text = "Задолженность организаций, млн. р.";

            G.DataSource = DT;
            


            

        }

        protected void G_InitializeLayout(object sender, LayoutEventArgs e)
        {
            SetGridColumn(G, 103);
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;

            e.Layout.RowSelectorsDefault = RowSelectors.No;
            e.Layout.CellClickActionDefault = CellClickAction.NotSet;
            
        }

        protected void C_DataBinding(object sender, EventArgs e)
        {

            FD = LD-3;
            DataTable DT = new DataTable(); string s = GenSqlForC();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(s, "sadasd", DT);
            C.DataSource = DT;
            SetBeautifulChart(C, true, Infragistics.UltraChart.Shared.Styles.ChartType.SplineChart, 10, Infragistics.UltraChart.Shared.Styles.LegendLocation.Bottom, 99.5);
            Label4.Text = "Динамика темпа роста задолженности организаций в % к аналогичному периоду предыдущего года";//("+D2.SelectedItem.Text+")";
            C.SplineChart.ChartText.Add(new Infragistics.UltraChart.Resources.Appearance.ChartTextAppearance(C, -2, -2, 1 == 1, new Font("arial", 8, FontStyle.Bold), Color.Black, "<DATA_VALUE:### #0.00>", StringAlignment.Far, StringAlignment.Center, 0));
            C.Axis.X.Labels.Orientation = Infragistics.UltraChart.Shared.Styles.TextOrientation.VerticalLeftFacing;
            C.Height = 500;
            C.Axis.X.Extent = 80;

            
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            LD = int.Parse(D2.SelectedItem.Text)+1;
            FD = int.Parse(D1.SelectedItem.Text);
            if (LD > FD)
            {
                
                Label3.Font.Bold = 1 == 2;
                Label3.ForeColor = Color.Black;

                G.DataBind();

                G.Rows[0].Cells[0].ColSpan = 13;
                G.Rows[LD - FD + 1].Cells[0].ColSpan = 13;
                C.DataBind();
            }
            else
            {
                Label3.Text = "Задан некорректный временной интервал";
                Label3.Font.Bold = 1 == 1;
                Label3.ForeColor = Color.Red;

                C.DataBind();
            }

        }

        protected void C_InvalidDataReceived(object sender, Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventArgs e)
        {
            setChartErrorFont(e);
        }

        protected void Button3_Click(object sender, EventArgs e)
        {
            Response.Redirect("../default.aspx");
        }
    }
}
