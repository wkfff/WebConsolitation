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
using Infragistics.WebUI.Misc;
using Infragistics.WebUI.UltraWebGrid;
using System.Drawing;
using Microsoft.AnalysisServices.AdomdClient;

using Infragistics.WebUI.UltraWebChart;
using Infragistics.UltraChart.Core.Primitives;

using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.CTAT.CTAT_0010
{
    public partial class _default : CustomReportPage
    {
        private CustomParam Param1 { get { return (UserParams.CustomParam("1")); } }
        private CustomParam Param2 { get { return (UserParams.CustomParam("2")); } }
        private CustomParam Param3 { get { return (UserParams.CustomParam("3")); } }
        private CustomParam REGION { get { return (UserParams.CustomParam("REGION")); } }
        private CustomParam DATASOURCE { get { return (UserParams.CustomParam("DATASOURCE")); } }
        private CustomParam way_last_year { get { return (UserParams.CustomParam("way_last_year")); } }
        private CustomParam Pokaz { get { return (UserParams.CustomParam("pokaz")); } }
        private CustomParam Lastdate { get { return (UserParams.CustomParam("lastdate")); } }
        private CustomParam Firstyear { get { return (UserParams.CustomParam("firstdate")); } }
        private Int32 screen_width { get { return (int)Session["width_size"] - 50; } }

        private string[] CGQuery = { "CG1", "CG2", "CG3", "CG4" };
        private string[] CGeQuery = { "CG1e", "CG2e", "CG3e", "CG4e" };
        private string[] BGQuery = { "BG1", "BG2", "BG3", "BG4", "BG5" };
        //private string[] BGQuery = { "BC51", "BC52", "BC53", "B", "BG5" };
        private string[] BC6Query = { "BC61", "BC62", "BC63", "BC64", "BC65" };
        private static string[] ECC;
        private static string[] EBG;
        int AC = 1;

        /// <summary>
        /// Метод для рисования на чарте последней даты
        /// используется в делегате чарта поэтому его просто передать надо
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void FillSceneGraph(object sender, Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e)
        {
            int xOct = 0;
            int xNov = 0;
            Text decText = null;
            int year = int.Parse(Lastdate.Value);
            String year1 = (year - 1).ToString();
            String year2 = (year - 2).ToString();


            foreach (Primitive primitive in e.SceneGraph)
            {

                {
                    if (primitive is Text)
                    {
                        Text text = primitive as Text;

                        decText = new Text();
                        decText.bounds = text.bounds;
                        decText.labelStyle = text.labelStyle;
                        for (int i = year; i < year - 6; i++)
                        {

                            decText.bounds.X = e.ChartCore.GridLayerBounds.Width + e.ChartCore.GridLayerBounds.X - decText.bounds.Width - (i - year) * 10;
                            decText.SetTextString(i.ToString());
                            e.SceneGraph.Add(decText);
                        }

                        break;
                    }
                }
            }
            //Text decText = null;


        }
        /// <summary>
        /// Метод для активацыи в гриде строчки
        /// </summary>
        /// <param name="Grid">Сам грид</param>
        /// <param name="index">какую</param>
        /// <param name="active">активировать?</param>
        private void GridActiveRow(UltraWebGrid Grid, int index, bool active)
        {
            try
            {
                // получаем выбранную строку
                UltraGridRow row = Grid.Rows[index];
                // устанавливаем ее активной, если необходимо
                if (active)
                {
                    row.Activate();
                    row.Activated = true;
                    row.Selected = true;
                }
                // получение заголовка выбранной отрасли
                //selected_year.Value = row.Cells[0].Value.ToString();
                //UltraChart1.DataBind();
                //chart1_caption.Text = String.Format(chart1_title_caption, row.Cells[0].Value.ToString());
            }
            catch (Exception)
            {
                // выбор сторки пошел с ошибкой ...
            }
        }
        /// <summary>
        /// Получение последнего блока из поля
        /// </summary>
        /// <param name="s">поле</param>
        /// <returns>то что в последних скобачках</returns>
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
        /// <summary>
        /// Этот метод необходимо вынести бы в ядро
        /// </summary>
        /// <param name="e">то что даёт событие при неудачной постройке чарта</param>
        protected void SetErorFonn(object sender, Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventArgs e)
        {
            e.Text = "В настоящий момент данные отсутствуют";

            e.LabelStyle.FontColor = Color.LightGray;
            e.LabelStyle.VerticalAlign = StringAlignment.Center;
            e.LabelStyle.HorizontalAlign = StringAlignment.Center;
            e.LabelStyle.Font = new Font("Verdana", 30);
            //e.LabelStyle.Font.Size = 15;
        }
        /// <summary>
        /// Настраивает лабель
        /// </summary>
        /// <param name="typ">размер</param>
        /// <param name="lab">сам лабель</param>
        private void setFont(int typ, Label lab, WebControl c)
        {
            lab.Font.Name = "arial";
            lab.Font.Size = typ;
            if (typ == 14) { lab.Font.Bold = 1 == 1; };
            if (typ == 10) { lab.Font.Bold = 1 == 1; };
            if (c != null) { lab.Width = c.Width; }
            if (typ == 18) { lab.Font.Bold = 1 == 1; };
            if (typ == 16) { lab.Font.Bold = 1 == 1; };
            //lab.Height = 40;
        }
        /// <summary>
        /// Возврощает выборку для чарта(хотя в принцыпе годиится и для грида)
        /// </summary>
        /// <param name="sql">Адрес в query.mdx</param>
        /// <returns>Выборка(если ошибка то пустая выборка)</returns>
        public DataTable GetDSForChart(string sql)
        {
            DataTable dt = new DataTable();
            string s = DataProvider.GetQueryText(sql);
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(s, "Показатель", dt);
            return dt;
        }
        /// <summary>
        /// Даёт последею дату
        /// требует в  query.mdx соотвествующего запроса(last_date)
        /// </summary>
        /// <param name="way_ly">Показатель который требуется вставить в запрос</param>
        /// <returns></returns>
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

        /// <summary>
        /// Настраивает диаграму
        /// </summary>
        /// <param name="Chart">снарт</param>
        /// <param name="legend">надо?</param>
        /// <param name="ChartType">тип чарта</param>
        /// <param name="legendPercent">отступ легенды</param>
        /// <param name="LegendLocation">позицыя легенды</param>
        /// <param name="SizePercent">размер на странице %</param>
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


            };
            if (ChartType == Infragistics.UltraChart.Shared.Styles.ChartType.DoughnutChart) { chart.DoughnutChart.ChartText.Add(new Infragistics.UltraChart.Resources.Appearance.ChartTextAppearance(chart, -2, -2, 1 == 1, new Font("arial", 8, FontStyle.Bold), Color.Black, "<DATA_VALUE:#>", StringAlignment.Center, StringAlignment.Center, 50)); }
            if (ChartType == Infragistics.UltraChart.Shared.Styles.ChartType.AreaChart)
            {
                //chart.Axis.X.Labels.Font = new Font("arial", 8, FontStyle.Bold);
                //chart.Axis.X.Labels.FontColor = Color.Black;
                chart.Axis.X.Margin.Near.Value = 2;
                //   chart.AreaChart.ChartText.Add(new Infragistics.UltraChart.Resources.Appearance.ChartTextAppearance(chart, -2, -2, 1 == 1, new Font("arial", 8, FontStyle.Bold), Color.Black, "<DATA_VALUE:#>", StringAlignment.Far, StringAlignment.Center, 0));
            };
        }
        /// <summary>
        /// Настраивает формат ячеек, уравнивает ширину, ставит врап итд..
        /// </summary>
        /// <param name="grid">Дай грид</param>
        /// <param name="sizePercent">ширина на странице(в процентах(просто число(желательно от 10 до 100(а то хрен знает как он выщитает))))</param>
        public void SetGridColumn(UltraWebGrid grid, double sizePercent)
        {

            double Width = CustomReportConst.minScreenWidth * (sizePercent / 100);
            grid.Width = CRHelper.GetGridWidth(Width);
            double WidthColumn = Width / grid.Columns.Count;

            for (int i = 0; i < grid.Columns.Count; i++)
            {
                grid.Columns[i].Width =  CRHelper.GetColumnWidth(WidthColumn*0.93);
                grid.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Left;
                grid.Bands[0].Columns[i].CellStyle.Wrap = 1 == 1;
                CRHelper.FormatNumberColumn(grid.Bands[0].Columns[i], "### ### ### ###.##");
                grid.Bands[0].Columns[i].Header.Style.Wrap = 1 == 1;
            }
            grid.DisplayLayout.GroupByBox.Hidden = 1 == 1;
            //grid.DisplayLayout.RowSelectorsDefault = RowSelectors.Yes;
            //grid.DisplayLayout.RowSelectorStyleDefault.Width = 0;
            grid.Columns[0].CellStyle.BorderDetails.ColorLeft = Color.Gray;
            grid.Columns[0].Header.Caption = "Год";
            //grid.Columns[0].
            grid.DisplayLayout.NoDataMessage = "В настоящий момент данные отсутствуют";
            //grid.OnClick += new UltraWebGrid.OnClick(CLIK);
            grid.Height = 0;

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
        /// <summary>
        /// Предположим что он работает
        /// </summary>
        /// <param name="query">квери для склейки</param>
        /// <returns>дататабель</returns>
        protected DataTable GlueTable(params string[] query)
        {
            DataTable DT = new DataTable();
            CellSet[] MCS = new CellSet[query.Length];
            DT.Columns.Add("Год");
            for (int i = 0; i < query.Length; i++)
            {
                string sb = DataProvider.GetQueryText(query[i]);
                MCS[i] = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(sb);
                //DT.Columns.Add(MCS[i].Axes[1].Positions[0].Members[0].Caption);
            }


            //:TODO

            //DT.Columns.Add("Год");


            foreach (Position pos in MCS[0].Axes[0].Positions)
            {
                // создание списка значений для строки UltraWebGrid
                object[] values = new object[MCS[0].Axes[1].Positions.Count+1];
                DT.Columns.Add();
                values[0] = MCS[0].Axes[0].Positions[pos.Ordinal].Members[0].Caption;
                for (int i = 1; i != MCS[0].Axes[1].Positions.Count; i++)
                {
                    try
                    {
                        if (MCS[0].Cells[i] == null) { values[i] = ""; }
                        else
                        {
                            DT.Columns.Add(MCS[0].Axes[1].Positions[i].Members[0].Caption);
                            values[i] = MCS[0].Cells[i].Value;
                        };
                    }
                    catch { };
                }



                // заполнение строки данными
                DT.Rows.Add(values);
            }



            return DT;

        }

        
        protected override void Page_Load(object sender, EventArgs e)
        {
            if(!Page.IsPostBack)
            {
                Page.Title = "Отчёт";

                Param1.Value = "Саратовская обл.";
                Lastdate.Value = ELV(getLastDate("Без группировки"));
                for (int i = 2020; i > (int.Parse(Lastdate.Value) - 3); i--)
                {
                    DropDownList1.Items.Add(i.ToString()); 
                }
                DropDownList1.SelectedIndex = DropDownList1.Items.Count - 3;
                Lastdate.Value = DropDownList1.SelectedItem.Text;
                G.DataBind();
                Pokaz.Value = GetString_(G.Rows[0].Cells[0].Text);
                //Lastdate.Value = ELV(getLastDate("По городам и районам"));
                BC.DataBind();
                TC.DataBind();
               // Lastdate.Value = ELV(getLastDate("Без группировки"));
                
                setFont(16, Label1,null);
                Label1.Text = "Основные показатели туризма по субъекту РФ "+Param1.Value;
                


                WebAsyncRefreshPanel1.AddLinkedRequestTrigger(G);
                PanelGrid.AddLinkedRequestTrigger(Button1);

            }
        }

        protected void G_DataBinding(object sender, EventArgs e)
        {
            DataTable DT = new DataTable();

            DT.Columns.Add("Показатель");
            DT.Columns.Add(Lastdate.Value);


            int j = 0;
            CellSet MCS = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(DataProvider.GetQueryText("G"));

            for (int i = 0; i < MCS.Cells.Count; )
            {
                object[] values = new object[2];
                try
                {
                    values[0] = MCS.Axes[1].Positions[j++].Members[0].Caption + ", " + MCS.Cells[i++].Value.ToString().ToLower() + "";
                }
                catch
                {
                    values[0] = MCS.Axes[1].Positions[j].Members[0].Caption;
                }
                try
                {
                    values[1] = MCS.Cells[i++].Value.ToString();
                }
                catch
                {

                    values[1] = "";
                }
                DT.Rows.Add(values);
            }
            G.DataSource = DT;
            setFont(10, GT, G);
            GT.Text = "Основные показатели гостиничного бизнеса " + Lastdate.Value + " год"; 
            
        }

        protected void TC_DataBinding(object sender, EventArgs e)
        {
            Lastdate.Value = ELV(getLastDate("По городам и районам"));
            TC.DataSource = GetDSForChart("TC");
            SetBeautifulChart(TC, false, Infragistics.UltraChart.Shared.Styles.ChartType.ColumnChart, 0, Infragistics.UltraChart.Shared.Styles.LegendLocation.Bottom, 50);
            TC.Height = 607;
            setFont(10,TCT,TC);
            TCT.Text =  "«"+Pokaz.Value+"» по городам за "+Lastdate.Value+" год";
            TC.Axis.Y.Extent = 15;
            TC.Axis.X.Extent = 95;
            TC.Axis.X.Labels.Orientation = Infragistics.UltraChart.Shared.Styles.TextOrientation.VerticalLeftFacing;
            
        }


        protected void BC_DataBinding(object sender, EventArgs e)
        {
           // Lastdate.Value = ELV(getLastDate("Без группировки"));
            BC.DataSource = GetDSForChart("BC");
            SetBeautifulChart(BC, true, Infragistics.UltraChart.Shared.Styles.ChartType.PieChart3D, 25, Infragistics.UltraChart.Shared.Styles.LegendLocation.Bottom, 50);
            setFont(10,BCT,BC);
            BCT.Text = "Структура показателя «" + Pokaz.Value + "» по видам средств размещения " + Lastdate.Value + " год"; 
            BC.Height = 470;
            BC.Tooltips.FormatString = "<b><DATA_VALUE:00.##></b>"  ;
            
        }

        protected void G_ActiveRowChange(object sender, RowEventArgs e)
        {
            Pokaz.Value =GetString_(e.Row.Cells[0].Text);
            TC.DataBind();
            BC.DataBind();
        }

        protected void G_InitializeLayout(object sender, LayoutEventArgs e)
        {
            //G.Columns[1].Hidden = 1 == 1;
            SetGridColumn(G, 50);
            G.DisplayLayout.RowSelectorsDefault = RowSelectors.Yes;
            G.Columns[0].CellStyle.BorderDetails.ColorLeft = Color.White;
           
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            Lastdate.Value = DropDownList1.SelectedItem.Text;
            G.DataBind();
            BC.DataBind();
            TC.DataBind();
        }


    }
}
