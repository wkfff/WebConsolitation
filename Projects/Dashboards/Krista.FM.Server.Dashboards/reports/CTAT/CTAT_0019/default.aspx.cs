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

namespace Krista.FM.Server.Dashboards.reports.CTAT.CTAT_0019
{
    public partial class _default : CustomReportPage
    {
        protected CustomParam Param1 { get { return (UserParams.CustomParam("1")); } }
        protected CustomParam Param2 { get { return (UserParams.CustomParam("2")); } }
        protected CustomParam Param3 { get { return (UserParams.CustomParam("3")); } }
        protected CustomParam DATASOURCE { get { return (UserParams.CustomParam("DATASOURCE")); } }
        private CustomParam way_last_year { get { return (UserParams.CustomParam("way_last_year")); } }
        protected CustomParam Pokaz { get { return (UserParams.CustomParam("pokaz")); } }
        protected CustomParam Lastdate { get { return (UserParams.CustomParam("lastdate")); } }
        protected CustomParam Firstyear { get { return (UserParams.CustomParam("firstdate")); } }
        private Int32 screen_width { get { return (int)Session["width_size"] - 50; } }


        /// <summary>
        /// Копирует строку с конца пока не встретит символ ch
        /// </summary>
        /// <param name="s">Где</param>
        /// <param name="ch">Что</param>
        /// <returns>Когда :)</returns>
        private string _GetString_(string s, char ch)
        {
            string res = "";
            int i = 0;
            for (i = s.Length - 1; s[i] != ch; i--) { res = s[i] + res; };

            return res;
        }
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
        protected void SetErorFonn(object sender, Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventArgs e)
        {
            e.Text = "В настоящий момент данные отсутствуют";

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
        /// <param name="way_ly">Показатель который требуется вставить в запрос(если нужен в квере)</param>
        /// <returns></returns>
        private String getLastDate(String way_ly)
        {
            try
            {
                way_last_year.Value = way_ly;
                string s = DataProvider.GetQueryText("last_date");
                CellSet cs = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(s);
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
            chart.Width = (int)(screen_width * (SizePercent / 100));
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

                chart.Axis.X.Margin.Near.Value = 2;
                
            };
        }
        /// <summary>
        /// Настраивает формат ячеек, уравнивает ширину, ставит врап итд..
        /// </summary>
        /// <param name="grid">Дай грид</param>
        /// <param name="sizePercent">ширина на странице(в процентах(просто число(желательно от 10 до 100(а то хрен знает как он выщитает))))</param>
        public void SetGridColumn(UltraWebGrid grid, double sizePercent, params bool[] rowSelector)
        {

            double Width = (CustomReportConst.minScreenWidth * (sizePercent / 100));
            grid.Width = CRHelper.GetGridWidth(Width);
            double widthFirstColumn = 0;
            //grid.Bands[0].Columns[0].CellStyle.Wrap = 1 == 1;
            //grid.Columns[0].Width = (int)(widthFirstColumn);
            double WidthColumn = (Width - widthFirstColumn) / grid.Columns.Count;

            for (int i = 0; i < grid.Columns.Count; i++)
            {
                grid.Columns[i].Width =CRHelper.GetColumnWidth(WidthColumn*0.95);
                grid.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Left;
                grid.Bands[0].Columns[i].CellStyle.Wrap = 1 == 1;
                CRHelper.FormatNumberColumn(grid.Bands[0].Columns[i], "### ### ### ###.##");
                grid.Bands[0].Columns[i].Header.Style.Wrap = 1 == 1;
            }
            grid.DisplayLayout.GroupByBox.Hidden = 1 == 1;
            if ((rowSelector.Length > 0) && (rowSelector[0])) { grid.Columns[0].CellStyle.BorderDetails.ColorLeft = Color.Gray; }
            grid.DisplayLayout.NoDataMessage = "В настоящий момент данные отсутствуют";
            grid.Height = 0;
        }

        public static ColumnHeader CH;
        /// <summary>
        /// Формирует колумы для селекта с кроссдЖойном в солумах(тока для простово с двумя измерениями)
        /// </summary>
        /// <param name="e">то что дает inicalizeLayout</param>
        protected void ForCrossJoin(LayoutEventArgs e,int span)
        {
            if (e.Layout.Bands[0].Columns.Count > 1)
            {
                ColumnHeader colHead;
                for (int i = 0; i < e.Layout.Bands[0].HeaderLayout.Count; i++)
                {
                    colHead = e.Layout.Bands[0].HeaderLayout[i] as ColumnHeader;
                    colHead.RowLayoutColumnInfo.OriginY = 1;
                }
                int dva = span;// :[)
                if (!Load1) { e.Layout.Bands[0].HeaderLayout.Remove(e.Layout.Bands[0].HeaderLayout[0]); }
                int c = e.Layout.Bands[0].HeaderLayout.Count;
                try
                {
                    for (int i = 1; i < c; i += dva)
                    {
                        ColumnHeader ch = new ColumnHeader(true);
                        CH = ch;
                        ch.Caption = GetString_(e.Layout.Bands[0].HeaderLayout[i].Caption, (char)59);
                        try
                        {
                            e.Layout.Bands[0].HeaderLayout[i].Caption = _GetString_(e.Layout.Bands[0].HeaderLayout[i].Caption, (char)59);
                            e.Layout.Bands[0].HeaderLayout[i].Style.Wrap = true;
                            e.Layout.Bands[0].HeaderLayout[i + 1].Caption = _GetString_(e.Layout.Bands[0].HeaderLayout[i + 1].Caption, (char)59);
                            e.Layout.Bands[0].HeaderLayout[i + 1].Style.Wrap = true;
                        }
                        catch
                        {
                        }


                        ch.RowLayoutColumnInfo.OriginX = i;//Позицыя по х относительно всех колумав

                        ch.RowLayoutColumnInfo.OriginY = 0;// по у
                        ch.RowLayoutColumnInfo.SpanX = dva;//Скока ячей резервировать
                        e.Layout.Bands[0].HeaderLayout.Add(ch);



                    }
                }
                catch
                {//Baanzzzaaaaaaaaaaaaaaaaaaaaaaaaaaaaaiiiiii!
                }
            }

        }
        /// <summary>
        /// Копирует строку с начала и до 1 вхождения ch символа
        /// </summary>
        /// <param name="s"></param>
        /// <param name="ch"></param>
        /// <returns></returns>
        private string GetString_(string s, char ch)
        {

            string res = "";
            int i = 0;
            for (i = s.Length - 1; s[i] != ch; i--) ;
            for (int j = 0; j < i; j++)
            {
                res += s[j];
            }
            return res;


        }

        private string delFirstChar(string s)
        {

            string res = "";
            for (int i = 1; i < s.Length; i++)
            {
                res += s[i];
            }
            return res;


        }

        /// <summary>
        /// Предположим что он работает
        /// </summary>
        /// <param name="query">квери для склейки</param>
        /// <returns>дататабель</returns>
        protected DataTable GlueTable(string[] query)
        {
            DataTable DT = new DataTable();
            CellSet[] MCS = new CellSet[query.Length];
            DT.Columns.Add("Год");
            for (int i = 0; i < query.Length; i++)
            {
                string sb = DataProvider.GetQueryText(query[i]);
                MCS[i] = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(sb);
                DT.Columns.Add(MCS[i].Axes[1].Positions[0].Members[0].Caption);
            }


            foreach (Position pos in MCS[0].Axes[0].Positions)
            {
                // создание списка значений для строки UltraWebGrid
                object[] values = new object[query.Length + 1];
                values[0] = MCS[0].Axes[0].Positions[pos.Ordinal].Members[0].Caption;
                for (int i = 1; i != MCS[0].Axes[0].Positions.Count; i++)
                {
                    try
                    {
                        if (MCS[i - 1].Cells[pos.Ordinal] == null) { values[i] = ""; }
                        else
                        {
                            values[i] = MCS[i - 1].Cells[pos.Ordinal].Value;
                        };
                    }
                    catch {};
                }



                // заполнение строки данными
                DT.Rows.Add(values);
            }



            return DT;

        }






        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender,e);
            if (!Page.IsPostBack) 
            {
                Page.Title = "Отчёт";
                Param1.Value = "&[1000000840]";
                Param2.Value = "СТАТ Отчетность - Облстат";
                Param3.Value = "2007";

                TT.Text = "Основные показатели торговли по субъекту РФ Саратовская обл.";
                setFont(16, TT, null);
                Lastdate.Value = ELV(getLastDate(""));
                for (int i = 2007; i < 2021;i++)
                       DropDownList1.Items.Add(i.ToString());
                Firstyear.Value = (int.Parse(Lastdate.Value) - 3).ToString();
                G.DataBind();
                CC.DataBind();
                BG.DataBind();
                Pokaz.Value = G.Rows[0].Cells[0].Text;
                TC.DataBind();
                CC.DataBind();
                Pokaz.Value = BG.Rows[1].Cells[0].Text;
                BC.DataBind();

                WebAsyncRefreshPanel1.AddLinkedRequestTrigger(G);
                WebAsyncRefreshPanel4.AddLinkedRequestTrigger(BG);
                WebAsyncRefreshPanel2.RemoveLinkedRequestTrigger(Button1);
                G.Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
                G.Columns[0].CellStyle.BorderDetails.ColorLeft = Color.Gray;
                
            }
       //     BG.Rows[0].Hidden = true;

        }

        protected void TC_DataBinding(object sender, EventArgs e)
        {

        }

        
        protected void G_DataBinding1(object sender, EventArgs e)
        {
            
            G.DataSource = GetDSForChart("TG");
            setFont(10, GT, G);
            GT.Text = "Оборот торговых организаций по видам торговли, по городам и районам, рублей";
        }

        protected void G_InitializeLayout(object sender, LayoutEventArgs e)
        {
            SetGridColumn(G, 43);
            if (Load1) { ForCrossJoin(e, 3); }
            else
            { /*e.Layout.Bands[0].Columns.Remove(e.Layout.Bands[0].Columns[0])*/
                ColumnHeader colHead;
                for (int i = 0; i < e.Layout.Bands[0].HeaderLayout.Count; i++)
                {
                    colHead = e.Layout.Bands[0].HeaderLayout[i] as ColumnHeader;
                    colHead.RowLayoutColumnInfo.OriginY = 0;
                }
                ForCrossJoin(e, 3);

            }
            try
            {
                

                e.Layout.CellClickActionDefault = CellClickAction.CellSelect;
                e.Layout.RowSelectorsDefault = RowSelectors.No;
               // e.Layout.Rows[0].Cells[0].Style.BorderDetails.ColorLeft = Color.Gray;
               // e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            }
            catch { }

            
        }

        protected void TC_DataBinding1(object sender, EventArgs e)
        {
            TC.DataSource = GetDSForChart("TC");
            SetBeautifulChart(TC, true, Infragistics.UltraChart.Shared.Styles.ChartType.DoughnutChart, 30, Infragistics.UltraChart.Shared.Styles.LegendLocation.Left, 57);
            setFont(10, TCT, TC);
            TCT.Text = "Распределение оборота торговых организаций по видам торговли, по годам ("+Pokaz.Value+")";

            TC.Height = (G.Rows.Count*10);
            

        }

        protected void G_Click(object sender, ClickEventArgs e)
        {
            try
            {
                Pokaz.Value = e.Cell.Row.Cells[0].Text;
                TC.DataBind();
                CC.DataBind();
            }
            catch { }
           
        }

        protected void CC_DataBinding(object sender, EventArgs e)
        {
            Firstyear.Value = (int.Parse(Lastdate.Value) - 3).ToString();
            CC.DataSource = GetDSForChart("CC");
            setFont(10, CCT, CC);
            CCT.Text = "Нарастание оборота платных услуг организаций ("+Pokaz.Value+")";
            SetBeautifulChart(CC, false, Infragistics.UltraChart.Shared.Styles.ChartType.AreaChart, 30, Infragistics.UltraChart.Shared.Styles.LegendLocation.Left, 57);
            CC.Height = (G.Rows.Count * 10);
           // CC.Axis.Z.Labels.Visible = 1 == 2;
           // CC.Axis.X.Labels.Visible = 1 == 2;

            CC.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:0,.##> тыс.р.";
            CC.Axis.Y.Extent = 80;
            CC.Axis.X.Margin.Near.Value = 6;
        }

        protected void BG_DataBinding(object sender, EventArgs e)
        {
            BG.DataSource = GetDSForChart("BG");
            //setFont(10, BGT, BG);
            BGT.Text = "Основные показатели розничной торговли, рублей";
            setFont(10, BGT, BG);
        }

        protected void BC_DataBinding(object sender, EventArgs e)
        {
            Lastdate.Value = ELV(getLastDate(""));
            Firstyear.Value = (int.Parse(Lastdate.Value) - 9).ToString();
            BC.DataSource = GetDSForChart("BC");
            setFont(10, RCT, BC);
            RCT.Text = "Динамика темпа роста показателя «"+Pokaz.Value+"»";
            SetBeautifulChart(BC, false, Infragistics.UltraChart.Shared.Styles.ChartType.LineChart, 0, Infragistics.UltraChart.Shared.Styles.LegendLocation.Left, 57);
            BC.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:00%>";
        }

        protected void BG_ActiveRowChange(object sender, RowEventArgs e)
        {
            Pokaz.Value = e.Row.Cells[0].Text;
            BC.DataBind();
        }

        protected void BG_InitializeLayout(object sender, LayoutEventArgs e)
        {
            SetGridColumn(BG, 42);
            try
            {
                e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
                BG.Columns[0].Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth * 0.29);
                BG.Columns[1].Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth * 0.10);
            }
            catch { }
        }
        bool Load1 = true;

        protected void Button1_Click(object sender, EventArgs e)
        {
            Param3.Value = DropDownList1.SelectedItem.Value;
            Load1 = false;
            //G;// = new UltraWebGrid();

            G.Rows.Clear();
            G.Columns.Clear();

            G.DataBind();

            BG.DataBind();
            try
            {
                Pokaz.Value = G.Rows[0].Cells[0].Text;
                TC.DataBind();
                CC.DataBind();
            }
            catch 
            {
            }
            try
            {
                Pokaz.Value = BG.Rows[0].Cells[0].Text;
                BC.DataBind();
            }
            catch
            { 
            }
        }
    }
}
