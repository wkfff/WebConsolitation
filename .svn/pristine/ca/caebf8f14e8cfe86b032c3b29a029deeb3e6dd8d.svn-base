using System;
using System.Collections;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebChart;
using Infragistics.WebUI.UltraWebGrid;

using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

using Microsoft.AnalysisServices.AdomdClient;

namespace Krista.FM.Server.Dashboards.reports.CTAT_0003._0001
{
    public partial class _default : CustomReportPage
    {
        private CustomParam Cub { get { return (UserParams.CustomParam("cub")); } }
        private CustomParam way_last_year { get { return (UserParams.CustomParam("way_last_year")); } }
        private CustomParam Pokaz { get { return (UserParams.CustomParam("pokaz")); } }
        private CustomParam Region { get { return (UserParams.CustomParam("region")); } }
        private CustomParam Lastdate { get { return (UserParams.CustomParam("lastdate")); } }
        private CustomParam Firstyear { get { return (UserParams.CustomParam("firstdate")); } }
        private CustomParam Mounth { get { return (UserParams.CustomParam("mounth")); } }
        private CustomParam Mounth1 { get { return (UserParams.CustomParam("mounth1")); } }
        private string[] GLQ= new string[] { "1", "2", "3", "4", "5" };

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
        public static void setChartErrorFont(ChartDataInvalidEventArgs e)
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
        public DataTable GetDSForChart(string sql)
        {
            DataTable dt = new DataTable();
            string s = DataProvider.GetQueryText(sql);
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(s, "Показатель", dt);
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

        public void SetBeautifulChart(UltraChart chart, bool legend, ChartType ChartType, int legendPercent, LegendLocation LegendLocation, double SizePercent)
        {
            try
            {
                chart.Width = CRHelper.GetChartWidth((int)(CustomReportConst.minScreenWidth * (SizePercent / 100)));
                chart.SplineAreaChart.ChartText.Add(new ChartTextAppearance(chart, -2, -2, 1 == 1, new Font("arial", 8), Color.Black, "<DATA_VALUE:#>", StringAlignment.Far, StringAlignment.Center, 0));
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
                if (ChartType == ChartType.AreaChart) { chart.AreaChart.ChartText.Add(new ChartTextAppearance(chart, -2, -2, 1 == 1, new Font("arial", 8), Color.Black, "<DATA_VALUE:#>", StringAlignment.Far, StringAlignment.Center, 0)); };

                chart.Axis.X.Extent = 100;
                chart.Axis.Y.Extent = 20;
                chart.Axis.X.Labels.Orientation = TextOrientation.VerticalLeftFacing;
                chart.Axis.X.Labels.SeriesLabels.Orientation = TextOrientation.VerticalLeftFacing;
                chart.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:###,##0.##>";
                chart.Axis.Z.Labels.Visible = 1 == 2;
            }
            catch
            { }

        }
        public void SetGridColumn(UltraWebGrid grid, double sizePercent)
        {
            try
            {

                double Width = CustomReportConst.minScreenWidth * (sizePercent / 100);
                grid.Width = CRHelper.GetGridWidth(Width);
                grid.Height = 0;
                double WidthColumn = Width / grid.Columns.Count;

                for (int i = 0; i < grid.Columns.Count; i++)
                {
                    grid.Columns[i].Width = CRHelper.GetColumnWidth(WidthColumn * 0.93);
                    grid.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Left;
                    grid.Bands[0].Columns[i].CellStyle.Wrap = 1 == 1;
                    CRHelper.FormatNumberColumn(grid.Bands[0].Columns[i], "### ### ### ###.##");
                    grid.Bands[0].Columns[i].Header.Style.Wrap = 1 == 1;
                }
                grid.DisplayLayout.GroupByBox.Hidden = 1 == 1;
                grid.Columns[0].CellStyle.BorderDetails.ColorLeft = Color.Gray;
                grid.Columns[0].Header.Caption = "Год";
                grid.DisplayLayout.NoDataMessage = "В настоящий момент данные отсутствуют";
            }
            catch { }
        }
        protected ArrayList GenMounth()
        {
            ArrayList Mounth = new ArrayList();

            Mounth.Add("[Период].[Год Квартал Месяц].[Год].[{0}].[Полугодие 1].[Квартал 1].[Январь");
            Mounth.Add("[Период].[Год Квартал Месяц].[Год].[{0}].[Полугодие 1].[Квартал 1].[Февраль");
            Mounth.Add("[Период].[Год Квартал Месяц].[Год].[{0}].[Полугодие 1].[Квартал 1].[Март");
            Mounth.Add("[Период].[Год Квартал Месяц].[Год].[{0}].[Полугодие 1].[Квартал 2].[Апрель");
            Mounth.Add("[Период].[Год Квартал Месяц].[Год].[{0}].[Полугодие 1].[Квартал 2].[Май");
            Mounth.Add("[Период].[Год Квартал Месяц].[Год].[{0}].[Полугодие 1].[Квартал 2].[Июнь");
            Mounth.Add("[Период].[Год Квартал Месяц].[Год].[{0}].[Полугодие 2].[Квартал 3].[Июль");
            Mounth.Add("[Период].[Год Квартал Месяц].[Год].[{0}].[Полугодие 2].[Квартал 3].[Август");
            Mounth.Add("[Период].[Год Квартал Месяц].[Год].[{0}].[Полугодие 2].[Квартал 3].[Сентябрь");
            Mounth.Add("[Период].[Год Квартал Месяц].[Год].[{0}].[Полугодие 2].[Квартал 4].[Октябрь");
            Mounth.Add("[Период].[Год Квартал Месяц].[Год].[{0}].[Полугодие 2].[Квартал 4].[Ноябрь");
            Mounth.Add("[Период].[Год Квартал Месяц].[Год].[{0}].[Полугодие 2].[Квартал 4].[Декабрь");

        Mounth.Add(@"[Группировки].[Население_Браки и разводы].[Без группировки],
        [ОК].[Отдельные возрастные группы].[Все отдельные группы].[Значение не указано],
        [Половая принадлежность].[Все типы полы].[Значение не указано],
        [Тип местности].[Все типы местности].[Значение не указано],
        [Население].[Браки и разводы].[Число зарегистрированных браков],");

        Mounth.Add(@"[Группировки].[Население_Браки и разводы].[Без группировки],
        [ОК].[Отдельные возрастные группы].[Все отдельные группы].[Значение не указано],
        [Половая принадлежность].[Все типы полы].[Значение не указано],
        [Тип местности].[Все типы местности].[Значение не указано],
        [Население].[Браки и разводы].[Число зарегистрированных разводов],");

        Mounth.Add(@"[Группировки].[Население_Естественное движение].[Без группировки],
        [Население].[Очередность рождения].[Все рождения].[Значение не указано],
        [ОК].[Основные возрастные группы].[Все возрастные группы].[Значение не указано],
        [ОК].[Отдельные возрастные группы].[Все отдельные группы].[Значение не указано],
        [Половая принадлежность].[Все типы полы].[Значение не указано],
        [Тип местности].[Все типы местности].[Значение не указано],
        [Население].[Естественное движение].[Число родившихся],");
        Mounth.Add(@"[Группировки].[Население_Естественное движение].[Без группировки],
        [Население].[Очередность рождения].[Все рождения].[Значение не указано],
        [ОК].[Основные возрастные группы].[Все возрастные группы].[Значение не указано],
        [ОК].[Отдельные возрастные группы].[Все отдельные группы].[Значение не указано],
        [Половая принадлежность].[Все типы полы].[Значение не указано],
        [Тип местности].[Все типы местности].[Значение не указано],
        [Население].[Естественное движение].[Число умерших],");
        Mounth.Add(@"[Группировки].[Население_Естественное движение].[Без группировки],
        [Население].[Очередность рождения].[Все рождения].[Значение не указано],
        [ОК].[Основные возрастные группы].[Все возрастные группы].[Значение не указано],
        [ОК].[Отдельные возрастные группы].[Все отдельные группы].[Значение не указано],
        [Половая принадлежность].[Все типы полы].[Значение не указано],
        [Тип местности].[Все типы местности].[Значение не указано],
        [Население].[Естественное движение].[Естественный прирост (убыль) населения],");
        Mounth.Add("[СТАТ_Население_Браки и разводы]");
        Mounth.Add("[СТАТ_Население_Браки и разводы]");
        Mounth.Add("[СТАТ_Население_Естественное движение]");
        Mounth.Add("[СТАТ_Население_Естественное движение]");
        Mounth.Add("[СТАТ_Население_Естественное движение]");
            return Mounth;
        }



        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);
        }
        protected override void Page_PreInit(object sender, EventArgs e)
        {
            base.Page_PreInit(sender, e);
        }

        public ArrayList Mesiac;
        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            if (!Page.IsPostBack)
            {
                Page.Title = "Отчёт";
                setFont(16, TT);

                setFont(10, LT);
                setFont(10, RT);

                TT.Text = "Основные показатели (Приволжский ФО)";

                Lastdate.Value = ELV(getLastDate(""));
                DropDownList1.Items.Add(new ListItem("Число зарегистрированных браков", GLQ[0]));
                DropDownList1.Items.Add(new ListItem("Число зарегистрированных разводов", GLQ[1]));
                DropDownList1.Items.Add(new ListItem("Число родившихся", GLQ[2]));
                DropDownList1.Items.Add(new ListItem("Число умерших", GLQ[3]));
                DropDownList1.Items.Add(new ListItem("Естественный прирост (убыль) населения", GLQ[4]));




                for (int i = int.Parse(Lastdate.Value); i > (int.Parse(Lastdate.Value) - 9); i--)
                {
                    DropDownList2.Items.Add(i.ToString());
                }



                Pokaz.Value = Lastdate.Value;
                G.DataBind();

                Mesiac = GenMounth();

                WebAsyncRefreshPanel3.AddLinkedRequestTrigger(Button1);
                
                WebAsyncRefreshPanel1.AddLinkedRequestTrigger(G);
                LC.DataBind();
                UltraChart2.DataBind();


                
            }
        }




        protected void G_DataBinding(object sender, EventArgs e)
        {
            
            G.DataSource = GetDSForChart("G"+DropDownList1.SelectedItem.Value);
            


        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            Pokaz.Value = DropDownList2.SelectedItem.Value;
            G.DataBind();
        }

        protected void G_InitializeLayout(object sender, LayoutEventArgs e)
        {
            setFont(10, GT);
            GT.Text = DropDownList1.SelectedItem.Text + " в (" + DropDownList2.SelectedItem.Value + ") году, единиц (Приволжский ФО)";
            SetGridColumn(G, 99);
            e.Layout.CellClickActionDefault = CellClickAction.CellSelect;
            e.Layout.RowSelectorsDefault = RowSelectors.No;

        }

        protected void UltraChart1_DataBinding(object sender, EventArgs e)
        {
            Mesiac = GenMounth();
            Lastdate.Value = DropDownList2.SelectedItem.Value;
            Firstyear.Value = (int.Parse(DropDownList2.SelectedItem.Value) - 1).ToString();
            Mounth.Value = string.Format(Mesiac[CS].ToString(),DropDownList2.SelectedItem.Value);
            Mounth1.Value = string.Format(Mesiac[CS].ToString(), (int.Parse(DropDownList2.SelectedItem.Value) - 1).ToString());
            Pokaz.Value = Mesiac[int.Parse((DropDownList1.SelectedItem.Value))+11].ToString();
            Cub.Value = Mesiac[int.Parse((DropDownList1.SelectedItem.Value)) + 16].ToString();
            LC.DataSource = GetDSForChart("CL");
            SetBeautifulChart(LC, true, ChartType.ColumnChart, 20, LegendLocation.Bottom, 49);
            LCT.Text = DropDownList2.SelectedItem.Text + ", единиц  (Приволжский ФО)";
            LC.Height = 500;
        }

        int CS = 1;
        int RS = 1;
        protected void G_Click(object sender, ClickEventArgs e)
        {
            CS = e.Cell.Column.Index;
            RS = e.Cell.Row.Index;
            LC.DataBind();
            UltraChart2.DataBind();
        }

        protected void UltraChart2_DataBinding(object sender, EventArgs e)
        {
            try
            {
                setFont(10, RCT);
                setFont(10, LCT);
                RCT.Text = DropDownList2.SelectedItem.Text + ", единиц (" + G.Rows[RS].Cells[0].Text + ")";

                Mesiac = GenMounth();
                Lastdate.Value = DropDownList2.SelectedItem.Value;
                //Firstyear.Value = (int.Parse(DropDownList2.SelectedItem.Value) - 1).ToString();
                //Mounth.Value = string.Format(Mesiac[CS].ToString(),DropDownList2.SelectedItem.Value);
                //Mounth1.Value = string.Format(Mesiac[CS].ToString(), (int.Parse(DropDownList2.SelectedItem.Value) - 1).ToString());
                Region.Value = G.Rows[RS].Cells[0].Text;
                Pokaz.Value = Mesiac[int.Parse((DropDownList1.SelectedItem.Value)) + 11].ToString();
                Cub.Value = Mesiac[int.Parse((DropDownList1.SelectedItem.Value)) + 16].ToString();


                Lastdate.Value = DropDownList2.SelectedItem.Value;
                DataTable DT1 = GetDSForChart("CR");
                Lastdate.Value = (int.Parse(DropDownList2.SelectedItem.Value) - 1).ToString();
                DataTable DT2 = GetDSForChart("CR");
                DT1.Rows.Add(DT2.Rows[0].ItemArray);
                UltraChart2.DataSource = DT1;
                UltraChart2.Height = 500;
                SetBeautifulChart(UltraChart2, true, ChartType.LineChart, 20, LegendLocation.Bottom, 49);
            }
            catch { }
        }

        protected void LC_InvalidDataReceived(object sender, ChartDataInvalidEventArgs e)
        {
            setChartErrorFont(e);
        }
    }
}
