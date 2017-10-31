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
using Krista.FM.Server.Dashboards.Common;
using Microsoft.AnalysisServices.AdomdClient;


using Infragistics.WebUI.UltraWebChart;
using Infragistics.UltraChart.Core.Primitives;

using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.CTAT.CTAT_0007
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
        private Int32 screen_width { get { return (int)Session["width_size"]; } }

        private string[] CGQuery = { "CG1","CG2","CG3","CG4" };
        private string[] CGeQuery = { "CG1e", "CG2e", "CG3e", "CG4e" };
        private string[] BGQuery = { "BG1", "BG2", "BG3", "BG4", "BG5" };
        //private string[] BGQuery = { "BC51", "BC52", "BC53", "B", "BG5" };
        private string[] BC6Query = { "BC61", "BC62", "BC63", "BC64", "BC65" };
        private static string[] ECC;
        private static string[] EBG;
        int AC = 1;








        /// <summary>
        /// Стандартный метод выбора столбца
        /// </summary>
        /// <param name="sender">передавать не чё не надо это нужна вать делегату на клик</param>
        /// <param name="e"></param>
        /*  protected void GRID_CLIK(object sender, ClickEventArgs e)
            {
                Pokaz.Value = e.Cell.Column.Header.Key;

                foreach (UltraChart Chart in Charts)
                {   //генератор текста над чартом лутше вынести в метод датабинд(чарта) 
                    Chart.DataBind();
                }
            }*/


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
        public void setFont(int typ, Label lab)
        {
            lab.Font.Name = "arial";
            lab.Font.Size = typ;
            if (typ == 14) { lab.Font.Bold = 1 == 1; };
            if (typ == 10) { lab.Font.Bold = 1 == 1; };
            if (typ == 18) { lab.Font.Bold = 1 == 1; };
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
                chart.PieChart3D.Labels.FormatString = "<DATA_VALUE:###,##0.##>";
                 

            };
            if (ChartType == Infragistics.UltraChart.Shared.Styles.ChartType.DoughnutChart) { chart.DoughnutChart.ChartText.Add(new Infragistics.UltraChart.Resources.Appearance.ChartTextAppearance(chart, -2, -2, 1 == 1, new Font("arial", 8, FontStyle.Bold), Color.Black, "<DATA_VALUE:#>", StringAlignment.Center, StringAlignment.Center, 50)); }
            if (ChartType == Infragistics.UltraChart.Shared.Styles.ChartType.AreaChart) {
                //chart.Axis.X.Labels.Font = new Font("arial", 8, FontStyle.Bold);
                //chart.Axis.X.Labels.FontColor = Color.Black;
                chart.Axis.X.Margin.Near.Value = 2;

                chart.AreaChart.ChartText.Add(new Infragistics.UltraChart.Resources.Appearance.ChartTextAppearance(chart, -2, -2, 1 == 1, new Font("arial", 8, FontStyle.Bold), Color.Black, "<DATA_VALUE:### #0.#>", StringAlignment.Far, StringAlignment.Center, 0)); };
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
                grid.Columns[i].Width = CRHelper.GetColumnWidth(WidthColumn*0.943);
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
        protected DataTable GlueTable(string[] query)
        {
            DataTable DT = new DataTable();
            CellSet[] MCS = new CellSet[query.Length];
            DT.Columns.Add("Год");
            try
            {
                for (int i = 0; i < query.Length; i++)
                {
                    string sb = DataProvider.GetQueryText(query[i]);
                    MCS[i] = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(sb);
                    DT.Columns.Add(MCS[i].Axes[1].Positions[0].Members[0].Caption);
                }
            }
            catch { }


            //:TODO

            //DT.Columns.Add("Год");
            

            foreach (Position pos in MCS[0].Axes[0].Positions)
            {
                bool b = false;
                // создание списка значений для строки UltraWebGrid
                object[] values = new object[query.Length + 1];
                values[0] = MCS[0].Axes[0].Positions[pos.Ordinal].Members[0].Caption;
                for (int i = 1; i != MCS[0].Axes[0].Positions.Count; i++)
                {
                    try
                    {
                        if (MCS[i - 1].Cells[pos.Ordinal].Value == null) { values[i] = ""; }
                        else
                        {
                            b = true;
                            values[i] = MCS[i - 1].Cells[pos.Ordinal].Value;
                        };
                    }
                    catch { };
                }



                // заполнение строки данными
                if (b) { DT.Rows.Add(values); };
            }



            return DT;

        }



     
        
        protected override void Page_Load(object sender, EventArgs e)
        {

            base.Page_Load(sender,e);
            if (!Page.IsPostBack){

                Page.Title = "Отчёт";

                Param2.Value = "Саратовская область";
                Param3.Value = "Приволжский ФО";
                
                Lastdate.Value =ELV( getLastDate(""));
                TG.DataBind();
                for (int i = int.Parse(((Lastdate.Value))) + 1; i > 1998; i--)
                {
                    BList.Items.Add(i.ToString());
                    TLIST.Items.Add(i.ToString()); };
                    TLIST.SelectedIndex = 1;
                    BList.SelectedIndex = 1;
                CG.DataBind();
                BG.DataBind();





                EBG = new string[BG.Columns.Count-1];
                for (int i = 1; i < BG.Columns.Count; i++)
                {
                    CellSet CLS = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(DataProvider.GetQueryText(BGQuery[i-1]+"e"));
                    EBG[i - 1] = CLS.Cells[0].Value.ToString().ToLower();
                    BG.Columns[i].Header.Caption += ", " + CLS.Cells[0].Value.ToString().ToLower();

                }

                ECC = new string[CG.Columns.Count-1];
                for (int i = 1; i < CG.Columns.Count; i++)
                {
                    CellSet CLS = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(DataProvider.GetQueryText(CGeQuery[i-1]));
                    ECC[i - 1] = CLS.Cells[0].Value.ToString().ToLower();
                    CG.Columns[i].Header.Caption += ", " + CLS.Cells[0].Value.ToString().ToLower();

                }



                for (int i = 0; i < BG.Rows.Count; i++)
                {
                    bool isempty = false;
                    for (int j = 1; j < BG.Columns.Count; j++)
                    {
                        if (BG.Rows[i].Cells[j].Value == null) { isempty = true; break; }
                    }
                    if (isempty) { BG.Rows[i].Hidden = 1 == 1; }


                }
                Lastdate.Value = TLIST.SelectedItem.Text.ToString();
                Pokaz.Value = TG.Rows[0].Cells[0].Text;
                
                TLC.DataBind();
                TRC.DataBind();
                TCC.DataBind();
                CC.DataBind();
                BLC.DataBind();
                BRC.DataBind();

                HT.Text = "Образование";
                setFont(16, HT);
                WebAsyncRefreshPanel6.AddLinkedRequestTrigger(BG);
                WebAsyncRefreshPanel1.AddLinkedRequestTrigger(CG);
                WebAsyncRefreshPanel7.AddLinkedRequestTrigger(Button1);
                WebAsyncRefreshPanel1.AddLinkedRequestTrigger(Button2);

                TLC.ColorModel.ColorBegin = Color.DarkOrchid;
                TLC.ColorModel.ColorEnd = Color.SeaGreen;
                TLC.ColorModel.ModelStyle = Infragistics.UltraChart.Shared.Styles.ColorModels.CustomLinear;
                TCC.ColorModel.ColorBegin = Color.DarkOrchid;
                TCC.ColorModel.ColorEnd = Color.SeaGreen;
                TCC.ColorModel.ModelStyle = Infragistics.UltraChart.Shared.Styles.ColorModels.CustomLinear;
                TRC.ColorModel.ColorBegin = Color.DarkOrchid;
                TRC.ColorModel.ColorEnd = Color.SeaGreen;
                TRC.ColorModel.ModelStyle = Infragistics.UltraChart.Shared.Styles.ColorModels.CustomLinear;
                for (int i = 0; i < BG.Rows.Count; i++)
                {
                    BG.Rows[i].Hidden = 1 == 2;
                }
                TG.Rows[0].Selected = 1 == 1;
                TG.Rows[0].Activate();
                TG.Rows[0].Activated = 1 == 1;
            }

        }

        protected void TG_DataBinding(object sender, EventArgs e)
        {
            setFont(10, TGT);
            TG.DataSource = GetDSForChart("TG");
            TGT.Text = "Основные показатели сферы образования по городам и районам за " + Lastdate.Value + " год ("+Param2.Value+")";

        }


        protected void TLC_DataBinding(object sender, EventArgs e)
        {
            setFont(10, TLCT);

            TLC.DataSource = GetDSForChart("TLC");
            TLCT.Text = "Численность детей в дошкольных образовательных учреждениях на конец " + Lastdate.Value + " года, человек ("+Pokaz.Value+")";
            SetBeautifulChart(TLC, false, Infragistics.UltraChart.Shared.Styles.ChartType.DoughnutChart, 0, Infragistics.UltraChart.Shared.Styles.LegendLocation.Bottom, 33);
            TLC.DoughnutChart.Labels.FormatString = "<ITEM_LABEL>"+(char)(13)+" <PERCENT_VALUE:#0.00>%";//ChartText.Add(new Infragistics.UltraChart.Resources.Appearance.ChartTextAppearance(TLC, -2, -2, true, new Font("arial", 8, FontStyle.Bold), Color.Black, "<ITEM_LABEL> <PERCENT_VALUE:#0.00>%", StringAlignment.Center, StringAlignment.Center, 0));        
            TLCT.Width = TLC.Width;
            TLC.DoughnutChart.StartAngle = 45;
        }

        protected void TCC_DataBinding(object sender, EventArgs e)
        {
            setFont(10, TCCT);
            TCC.DataSource = GetDSForChart("TCC");
            TCCT.Text = "Число дошкольных образовательных учреждений на конец " + Lastdate.Value + " год, единиц ("+Pokaz.Value+")";
            SetBeautifulChart(TCC, false, Infragistics.UltraChart.Shared.Styles.ChartType.DoughnutChart, 0, Infragistics.UltraChart.Shared.Styles.LegendLocation.Bottom, 33);
            TCC.DoughnutChart.Labels.FormatString = "<ITEM_LABEL> <PERCENT_VALUE:#0.00>%";

            TCCT.Width = TCC.Width;
            TCC.DoughnutChart.StartAngle = 45;
        }

        protected void TRC_DataBinding(object sender, EventArgs e)
        {
            setFont(10,TRCT);
            TRC.DataSource = GetDSForChart("TRC");
            TRCT.Text = "Число мест в дошкольных образовательных учреждениях на конец " + Lastdate.Value + " год, мест ("+Pokaz.Value+")";
            SetBeautifulChart(TRC, false, Infragistics.UltraChart.Shared.Styles.ChartType.DoughnutChart, 0, Infragistics.UltraChart.Shared.Styles.LegendLocation.Bottom, 33);
            TRC.DoughnutChart.Labels.FormatString = "<ITEM_LABEL> <PERCENT_VALUE:#0.00>%";
            TRC.Width = TRCT.Width;
            TRC.DoughnutChart.StartAngle = 45;
        }

        protected void TG_ActiveRowChange(object sender, RowEventArgs e)
        {
            Pokaz.Value = e.Row.Cells[0].Text;
            Lastdate.Value = TLIST.SelectedItem.Text.ToString();
            TRC.DataBind();
            TCC.DataBind();
            TLC.DataBind();
            BG.DataBind();
        }

        protected void CG_DataBinding(object sender, EventArgs e)
        {
            setFont(10, CGT);
            CG.DataSource = GlueTable(CGQuery);
            CGT.Text = "Основные показатели сферы образования за "+Lastdate.Value+" год ("+Param3.Value+")";
        }

        protected void CG_Click(object sender, ClickEventArgs e)
        {
            try
            {
                if (e.Cell.Column.Index != 0)
                { AC = e.Cell.Column.Index; }
                else
                {
                    AC = e.Cell.Column.Index - 1;
                }
                Lastdate.Value = BList.SelectedItem.Text;

                CC.DataBind();
                e.Cell.Column.Selected = 1 == 1;
                CG.Columns[e.Cell.Column.Index].Selected = 1 == 1;

            }
            catch { }
        }

        protected void CC_DataBinding(object sender, EventArgs e)
        {
            setFont(10, CCT);
            CC.DataSource = GetDSForChart(CGQuery[AC]);
            SetBeautifulChart(CC, false, Infragistics.UltraChart.Shared.Styles.ChartType.AreaChart,0, Infragistics.UltraChart.Shared.Styles.LegendLocation.Bottom, 99);
            CCT.Text = CG.Columns[AC+1].Header.Key+ " за "+Lastdate.Value+" год, "+ ECC[AC];
            CC.Axis.X.Extent = 95;
            CC.Axis.Y.Extent = 20;
            CC.Axis.X.Labels.Orientation = Infragistics.UltraChart.Shared.Styles.TextOrientation.VerticalLeftFacing;
            CC.ColorModel.ModelStyle = Infragistics.UltraChart.Shared.Styles.ColorModels.LinearRange;
            CC.ColorModel.ColorBegin = Color.HotPink;
        }

        protected void BG_DataBinding(object sender, EventArgs e)
        {
            BG.DataSource = GlueTable(BGQuery);
            BG.Rows.ColumnFilters.Clear();
          
            setFont(10, BGT);
            BGT.Text = "Основные показатели сферы образования ("+Param2.Value+")";
        }

        protected void BG_InitializeLayout(object sender, LayoutEventArgs e)
        {
            SetGridColumn(BG, 101);
            e.Layout.CellClickActionDefault = CellClickAction.CellSelect;
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(CustomReportConst.minScreenWidth*0.1);
            for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i++)
                {
                    e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth((CustomReportConst.minScreenWidth * 0.85)/(e.Layout.Bands[0].Columns.Count-1));
                }
                e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;    
            //
        }

        protected void BRC_DataBinding(object sender, EventArgs e)
        {
            BRC.ColorModel.ModelStyle = Infragistics.UltraChart.Shared.Styles.ColorModels.CustomLinear;
            setFont(10, BRCT);
            BRC.DataSource = GetDSForChart(BC6Query[AC]);
            SetBeautifulChart(BRC, true, Infragistics.UltraChart.Shared.Styles.ChartType.DoughnutChart3D, 35, Infragistics.UltraChart.Shared.Styles.LegendLocation.Bottom, 49);
            if (AC == 1)
            {
                BRCT.Text = "Численность детей в дошкольных образовательных учреждениях на конец "+Lastdate.Value+" года, человек";
                BRC.Tooltips.FormatString = "<ITEM_LABEL>, человек " + "<b><DATA_VALUE:00.##></b>";

                BRC.ColorModel.ModelStyle = Infragistics.UltraChart.Shared.Styles.ColorModels.PureRandom;
            }
            else
                if (AC == 2)
                {
                    BRCT.Text = "Число мест в дошкольных образовательных учреждениях на " + Lastdate.Value + " год, единиц";
                    BRC.Tooltips.FormatString = "<ITEM_LABEL>, единиц " + "<b><DATA_VALUE:00.##></b>";
                    BRC.ColorModel.ModelStyle = Infragistics.UltraChart.Shared.Styles.ColorModels.LinearRange;
                    BRC.ColorModel.ColorBegin = Color.DarkOrchid;
                    BRC.ColorModel.ColorEnd = Color.SeaGreen;


                }
                else if (AC == 3) {
       
                    BRC.Tooltips.FormatString = "<ITEM_LABEL>, человек " + "<b><DATA_VALUE:00.##></b>"; BRCT.Text = "Численность студентов государственных средних специальных учебных заведений за " + Lastdate.Value + " год, человек"; }
                    else
                    if (AC == 4)
                    {
  
                        BRCT.Text = "Численность студентов государственных высших учебных заведений  за "+Lastdate.Value+" год, человек";
                        BRC.Tooltips.FormatString = "<ITEM_LABEL>, человек " + "<b><DATA_VALUE:00.##></b>";
                    }
            if (AC == 5)
            {
                BRCT.Text = "Численность студентов негосударственных высших учебных заведений  за "+Lastdate.Value+" год, человек";
                BRC.Tooltips.FormatString = "<ITEM_LABEL>, человек " + "<b><DATA_VALUE:00.##></b>";
            }
           


        }

        protected void BLC_DataBinding(object sender, EventArgs e)
        {
            setFont(10, BLCT);
            string b = Lastdate.Value;
            Lastdate.Value = BG.Rows[BG.Rows.Count - 1].Cells[0].Text;
            BLC.DataSource = GetDSForChart(BGQuery[AC]);
            SetBeautifulChart(BLC, false, Infragistics.UltraChart.Shared.Styles.ChartType.ColumnChart3D, 0, Infragistics.UltraChart.Shared.Styles.LegendLocation.Bottom, 49);
            BLCT.Text = "Динамика показателя «"+GetString_(BG.Columns[AC+1].Header.Caption)+"», "+EBG[AC];
            Lastdate.Value = b;
       
        }

        protected void BG_Click(object sender, ClickEventArgs e)
        {
            if ((e.Cell != null) && (e.Cell.Column.Index > 0))
            {
                AC = e.Cell.Column.Index - 1;
                Lastdate.Value = e.Cell.Row.Cells[0].Text;
                BG.Columns[e.Cell.Column.Index].Selected = 1 == 1;
                e.Cell.Row.Activate();
                e.Cell.Row.Selected = 1 == 1;
                e.Cell.Row.Activated = 1 == 1;
            }
            else
            {
                AC = 1;
                
                
            }
            
            BRC.DataBind();
            BLC.DataBind();
            
        }

        protected void WebImageButton1_Click(object sender, Infragistics.WebUI.WebDataInput.ButtonEventArgs e)
        {
            


                        
        }

        protected void WebImageButton2_Click(object sender, Infragistics.WebUI.WebDataInput.ButtonEventArgs e)
        {

        }
        string[] TGE = { "человек", "единица", "место" };
        protected void TG_InitializeLayout(object sender, LayoutEventArgs e)
        {
            SetGridColumn(TG, 99);
            e.Layout.CellClickActionDefault = CellClickAction.RowSelect;
            TG.Columns[0].CellStyle.BorderDetails.ColorLeft = Color.White;
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[0].Header.Caption = "Территории";
            e.Layout.Bands[0].Columns[1].Header.Caption += ", "+TGE[0];
            e.Layout.Bands[0].Columns[2].Header.Caption += ", "+TGE[1];
            e.Layout.Bands[0].Columns[3].Header.Caption += ", "+TGE[2];
            e.Layout.CellClickActionDefault = CellClickAction.RowSelect;
            TG.Height = 350;
        }

        protected void CG_InitializeLayout(object sender, LayoutEventArgs e)
        {
            SetGridColumn(CG, 101);
            e.Layout.CellClickActionDefault = CellClickAction.CellSelect;
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[0].Header.Caption = "Територии";
            e.Layout.RowSelectorsDefault = RowSelectors.No;
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            Lastdate.Value = TLIST.SelectedItem.Text;
            //Pokaz.Value = 
            TG.DataBind();
            TLC.DataBind();
            TRC.DataBind();
            TCC.DataBind();
        }

        protected void Button2_Click(object sender, EventArgs e)
        {
            Lastdate.Value = BList.SelectedItem.Text;
            CG.DataBind();
            CC.DataBind();
        }
        
    }
}
