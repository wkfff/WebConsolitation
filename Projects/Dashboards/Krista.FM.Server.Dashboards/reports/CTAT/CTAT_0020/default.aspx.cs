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

namespace Krista.FM.Server.Dashboards.reports.CTAT.CTAT_0020
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
                string s = DataProvider.GetQueryText(way_ly);
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
            chart.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth * (SizePercent / 100));
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
        public void SetGridColumn(UltraWebGrid grid, double sizePercent, params bool[] rowSelector)
        {

            double Width = CustomReportConst.minScreenWidth * (sizePercent / 100);

            grid.Width = CRHelper.GetGridWidth(Width);

            double widthFirstColumn = 0;
            //grid.Bands[0].Columns[0].CellStyle.Wrap = 1 == 1;
            //grid.Columns[0].Width = (int)(widthFirstColumn);
            double WidthColumn = (Width - widthFirstColumn) / grid.Columns.Count;

            for (int i = 0; i < grid.Columns.Count; i++)
            {
                grid.Columns[i].Width = CRHelper.GetColumnWidth(WidthColumn*0.93);
                grid.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Left;
                grid.Bands[0].Columns[i].CellStyle.Wrap = 1 == 1;
                CRHelper.FormatNumberColumn(grid.Bands[0].Columns[i], "### ### ### ##0.##");
                grid.Bands[0].Columns[i].Header.Style.Wrap = 1 == 1;
            }
            grid.DisplayLayout.GroupByBox.Hidden = 1 == 1;
            //if ((rowSelector.Length > 0) && (rowSelector[0])) { grid.Columns[0].CellStyle.BorderDetails.ColorLeft = Color.Gray; }
            grid.DisplayLayout.NoDataMessage = "В настоящий момент данные отсутствуют";
            grid.Height = 0;
        }

        bool Load1;
        public static ColumnHeader CH;
        /// <summary>
        /// Формирует колумы для селекта с кроссдЖойном в солумах(тока для простово с двумя измерениями)
        /// </summary>
        /// <param name="e">то что дает inicalizeLayout</param>
        protected void ForCrossJoin(LayoutEventArgs e, int span)
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
                    catch { };
                }



                // заполнение строки данными
                DT.Rows.Add(values);
            }



            return DT;

        }
        private void LoadDateToList(DropDownList DDL, int LastDatee, int FirstDate)
        {
            for (int i = FirstDate; LastDatee >= i; i++)
            {
                DDL.Items.Add(i.ToString());
            }
        }

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);
        }
        protected override void Page_PreInit(object sender, EventArgs e)
        {
            base.Page_PreInit(sender, e);
        }


        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            if (!Page.IsPostBack)
            {
                Param1.Value = "Приволжский ФО";
                Param2.Value = "СТАТ Отчетность - Облстат";
                setFont(16, Label4, null);

                Page.Title = "Отчёт";

                Label4.Text = "Основные показатели по платным услугам в субъекте РФ Саратовская обл.";
                
                LoadDateToList(DropDownList1, 2021, 2006);
                DropDownList1.SelectedIndex = 1;
                

                Lastdate.Value = DropDownList1.SelectedItem.Text;//ELV(getLastDate("last_date"));
                for (int i = 0; i < 12; i++)
                {
                    DropDownList5.Items.Add(_GetString_(GenMounth()[i].ToString(), '['));
                }
                for (int i = 1; i < 12; i++)
                {
                    DropDownList3.Items.Add(_GetString_(GenMounth()[i].ToString(), '['));
                }

                for (int i = 0; i < 12; i++)
                {
                    DropDownList4.Items.Add(_GetString_(GenMounth()[i].ToString(), '['));
                }

                for (int i = 2006; i < 2021; i++)
                {
                    DropDownList2.Items.Add(i.ToString());
                }
                DropDownList2.SelectedIndex = 3;
                DropDownList3.SelectedIndex = 3;
                DropDownList4.SelectedIndex = 0;
                firstMounth = 0;
                CountMounth = 4;
                

                G.DataBind();
                BG.DataBind();
                BG_InitializeLayout(null, null);
                try
                {
                    Pokaz.Value = G.Rows[1].Cells[0].Text;
                    TC.DataBind();
                    //BC.DataBind();
                }
                catch { }
                    WebAsyncRefreshPanel3.AddLinkedRequestTrigger(Button1);
                    WebAsyncRefreshPanel2.AddLinkedRequestTrigger(Button2);
                    G.Rows[0].Hidden = 0 == 0;
                    TC.ColorModel.ModelStyle = Infragistics.UltraChart.Shared.Styles.ColorModels.CustomLinear;
                    setFont(10, BCT, BG);
                
                
            
                
            }
            for (int i = 0; i < G.Rows.Count; i++) { G.Rows[i].Height = 30; };

        }

        protected void G_DataBinding(object sender, EventArgs e)
        {

            Lastdate.Value = string.Format((GenMounth()[DropDownList5.SelectedIndex].ToString()), DropDownList1.SelectedItem.Text)+']';
            G.DataSource = GetDSForChart("G");
            Lastdate.Value = DropDownList1.SelectedItem.Text;
                
            setFont(10, GT, G);
            GT.Width = (int)(screen_width*0.40);
            GT.Text = "Объем платных услуг населению к предыдущему году в сопоставимых ценах по субъектам ФО";
            
        }
        static int rowindex = 0; 
        protected void G_ActiveRowChange(object sender, RowEventArgs e)
        {
            rowindex = 1;
            try
            {
                Pokaz.Value = e.Row.Cells[0].Text;
                rowindex = e.Row.Index;
                TC.DataBind();
                
                //BC.DataBind();
            }
            catch
            {
                try
                {
                    Pokaz.Value = G.Rows[rowindex].Cells[0].Text;
                    TC.DataBind();
                }
                catch { }
            }
            
        }

        protected void TC_DataBinding(object sender, EventArgs e)
        {
            Lastdate.Value = ELV(getLastDate("last_date2"));
            DataTable dt = GetDSForChart("TC");
            TC.DataSource = dt;
            double max,min;
            CellSet CS = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(DataProvider.GetQueryText("TC"));
            max = double.Parse(CS.Cells[0].Value.ToString());
            min = double.Parse(CS.Cells[0].Value.ToString());            
            for(int i = 0 ;i<CS.Cells.Count;i++)
            {
                if (double.Parse(CS.Cells[i].Value.ToString()) > max) { max = double.Parse(CS.Cells[i].Value.ToString()); }
                if (double.Parse(CS.Cells[i].Value.ToString()) < min) { if (double.Parse(CS.Cells[i].Value.ToString()) != 0) { min = double.Parse(CS.Cells[i].Value.ToString()); } }
            }
                       

            TCT.Text = "Объем платных услуг населению к предыдущему году в сопоставимых ценах в субъекте " + Pokaz.Value + " по сравнению с Саратовской обл.";
            SetBeautifulChart(TC, true, Infragistics.UltraChart.Shared.Styles.ChartType.AreaChart, 9, Infragistics.UltraChart.Shared.Styles.LegendLocation.Top, 70);
            setFont(10, TCT, TC);
            TC.Height = 500;
            TC.Axis.X.Margin.Near.Value = 4;
            TC.Axis.X.Margin.Far.Value = 4;
            TC.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:00.##%>";
            TC.ColorModel.ModelStyle = Infragistics.UltraChart.Shared.Styles.ColorModels.CustomLinear;
            TC.Axis.Y.RangeType = Infragistics.UltraChart.Shared.Styles.AxisRangeType.Custom;
            TC.Axis.Y.RangeMax = max+0.01;
            TC.Axis.Y.RangeMin = min-0.01;
            
        }

        protected void BC_DataBinding(object sender, EventArgs e)
        {
            Lastdate.Value = DropDownList1.SelectedItem.Text;
            //BC.DataSource = GetDSForChart("BC");
            
            BCT.Text = "Объем платных услуг населению по субъектам ФО за " + Lastdate.Value + ", рубль";
            //SetBeautifulChart(BC, false, Infragistics.UltraChart.Shared.Styles.ChartType.CylinderColumnChart3D, 0, Infragistics.UltraChart.Shared.Styles.LegendLocation.Bottom, 99);
            //setFont(10, BCT, BC);
            //BC.Height = 500;
            
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            try
            {
                Lastdate.Value = DropDownList1.SelectedItem.Text;
                G.DataBind();
                //BC.DataBind();
                G_ActiveRowChange(null, null);
                for (int i = 0; i < G.Rows.Count; i++) { G.Rows[i].Height = 30; };
                G.Rows[0].Hidden = 1 == 1;
            }
            catch { }
        }

        protected void G_InitializeLayout(object sender, LayoutEventArgs e)
        {
            try
            {
                SetGridColumn(G, 28, true);
                e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(CustomReportConst.minScreenWidth * 0.20);
                e.Layout.Bands[0].Columns[1].Width = CRHelper.GetColumnWidth(CustomReportConst.minScreenWidth * 0.05);
                CRHelper.FormatNumberColumn(G.Bands[0].Columns[1], "### ### ### ##0.##%");
                GT.Width = (int)(CustomReportConst.minScreenWidth * 0.27);

                e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;

                for (int i = 0; i < G.Rows.Count; i++) { e.Layout.Rows[i].Height = 30; };
            }
            catch { G.DisplayLayout.NoDataMessage = "<div style=" + '"' + "font-weight: bolder; font-size: 13pt;  color: red;" + '"' + ">В настоящий момент данные отсутствуют </div>"; }
           
        }
        int  CountMounth,firstMounth;
        
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

            return Mounth;
        }

        public ArrayList Mesiac;
        protected string GenSql()
        {
            Mesiac = GenMounth();

            
            string SQL_with = "  with ";
            string SQL_column = "";

            for (int i = (firstMounth); i < CountMounth; i++)
            {
                SQL_with += " member " + string.Format(Mesiac[i].ToString(), Lastdate.Value) + "-" + Lastdate.Value + "]";
                SQL_column += " " + string.Format(Mesiac[i].ToString(), Lastdate.Value) + "-" + Lastdate.Value + "],";
                SQL_with += " as " + "'" + string.Format(Mesiac[i].ToString(), Lastdate.Value) + "]'";

                SQL_with += " member " + string.Format(Mesiac[i].ToString(), Firstyear.Value) + "-a" + Firstyear.Value + "]";
                SQL_column += "  " + string.Format(Mesiac[i].ToString(), Firstyear.Value) + "-a" + Firstyear.Value + "],";
                SQL_with += " as " + "'" + string.Format(Mesiac[i].ToString(), Firstyear.Value) + "]'";
            }
            int ii = CountMounth;
            SQL_with += " member " + string.Format(Mesiac[ii].ToString(), Lastdate.Value) + "-" + Lastdate.Value + "]";
            SQL_column += " " + string.Format(Mesiac[ii].ToString(), Lastdate.Value) + "-" + Lastdate.Value + "],";
            SQL_with += " as " + "'" + string.Format(Mesiac[ii].ToString(), Lastdate.Value) + "]'";

            SQL_with += " member " + string.Format(Mesiac[ii].ToString(), Firstyear.Value) + "-a" + Firstyear.Value + "]";
            SQL_column += "  " + string.Format(Mesiac[ii].ToString(), Firstyear.Value) + "-a" + Firstyear.Value + "]";
            SQL_with += " as " + "'" + string.Format(Mesiac[ii].ToString(), Firstyear.Value) + "]'";

            string SQL = SQL_with + " select { " + SQL_column + @"
                                        }on columns ,
                                        non empty 
                                        {
                                            [Территории].[РФ Карта].[МР ГО].members 
                                        } on rows 
                                    FROM  [СТАТ_Платные услуги_Основные показатели]  
                                    where  
                                        (
                                            [Платные услуги].[Основные показатели].[Объем платных услуг населению],
                                            [Группировки].[Платные услуги_Основные показатели].[По городам и районам],
                                            [Источники данных].[Источник].[СТАТ Отчетность - Облстат],
                                            [Measures].[Нарастающий итог] 
                                        )";

            return SQL;

        }

        protected void BG_DataBinding(object sender, EventArgs e)
        {
            
            DataTable DT = new DataTable();
            Lastdate.Value = DropDownList2.SelectedItem.Text;
            Firstyear.Value = DropDownList2.SelectedItem.Text;
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(GenSql(), "Територия", DT);
            BG.DataSource = DT;


        }

        protected void BG_InitializeLayout(object sender, LayoutEventArgs e)
        {
            bool noDATA = true;
            if (e == null)
            {
                double[] SumCol = new double[BG.Columns.Count];
                for (int i = 1; i < SumCol.Length; i++)
                    {
                        for (int j = 0; j < BG.Rows.Count; j++)
                        {
                            if (BG.Rows[j].Cells[i].Text != null) { SumCol[i] += double.Parse(BG.Rows[j].Cells[i].Text); noDATA = 1 == 2; };
                        }
                    }

                    if (noDATA)
                    {
                        BG.DisplayLayout.NoDataMessage = "<div style="+'"'+"font-weight: bolder; font-size: 13pt;  color: red;"+'"' +">В настоящий момент данные отсутствуют </div>";
                        
                         BG.DataSource = null; BG.Clear();
                    }else
                        if (firstMounth>CountMounth)
                        { BG.DisplayLayout.NoDataMessage = "<div style="+'"'+"font-weight: bolder; font-size: 13pt;  color: red;"+'"'+ ">Задан некорректный интервал</div>"; BG.DataSource = null; BG.Clear(); }
                    else
                    {


                        for (int i = 0; i < BG.Rows.Count; i++)
                        {
                            try
                            {
                                for (int j = 1; j < (CountMounth + 2 - firstMounth); j++)
                                {
                                    if (double.Parse(BG.Rows[i].Cells[j * 2].Text) != 0)
                                    {
                                        double res = (double.Parse(BG.Rows[i].Cells[j * 2 - 1].Text) / SumCol[j * 2 - 1]) * 100;
                                        {
                                            BG.Rows[i].Cells[j * 2].Text = Math.Round(res, 1).ToString("#0.0#") + "%";  // <img src=" + '"' + "../../../../images/1.gif" + '"' + "></img>";
                                        }


                                    }
                                    else
                                    {
                                        BG.Rows[i].Cells[j * 2].Text = "нет данных";

                                    }

                                }
                            }
                            catch
                            { }
                        }
                        try
                        {
                            for (int j = 1; j < (CountMounth + 2 - firstMounth); j++)
                            {
                                BG.Columns[j * 2].Header.Caption = "Процент от общего объема";
                                BG.Columns[j * 2 - 1].Header.Caption = _GetString_(BG.Columns[j * 2 - 1].Header.Caption, '-');
                            }
                        }
                        catch { }
                        //SetGridColumn(BG, 103);
                    }
            }
            if (e != null)
            {

                SetGridColumn(BG, 101);
                ColumnHeader colHead;
                for (int i = 0; i < e.Layout.Bands[0].HeaderLayout.Count; i++)
                {
                    colHead = e.Layout.Bands[0].HeaderLayout[i] as ColumnHeader;
                    colHead.RowLayoutColumnInfo.OriginY = 1;
                }
                e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(CustomReportConst.minScreenWidth*0.13);
                for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i+=2)
                {
                    e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(CustomReportConst.minScreenWidth * 0.07);
                    e.Layout.Bands[0].Columns[i+1].Width = CRHelper.GetColumnWidth(CustomReportConst.minScreenWidth * 0.07);
                    e.Layout.Bands[0].Columns[i + 1].Header.Style.Wrap = 1 == 1;
                    e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
                    e.Layout.Bands[0].Columns[i+1].CellStyle.HorizontalAlign = HorizontalAlign.Right;
                }
                int c = e.Layout.Bands[0].HeaderLayout.Count;
                try
                {
                    for (int i = 1; i < c; i += 2)
                    {
                        ColumnHeader ch = new ColumnHeader(true);
                        //CH = ch;
                        ch.Caption = GetString_(e.Layout.Bands[0].HeaderLayout[i].Caption, '-');

                        ch.RowLayoutColumnInfo.OriginX = i;//Позицыя по х относительно всех колумав

                        ch.RowLayoutColumnInfo.OriginY = 0;// по у
                        ch.RowLayoutColumnInfo.SpanX = 2;//Скока ячей резервировать
                        e.Layout.Bands[0].HeaderLayout.Add(ch);



                    }

                }
                catch
                {//Baanzzzaaaaaaaaaaaaaaaaaaaaaaaaaaaaaiiiiii!
                }
                //SetGridColumn(BG, 103);
                e.Layout.Grid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth * 1.0);
                e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
                BG.Height = 1000;

            }

        }

        protected void Button2_Click(object sender, EventArgs e)
        {
            CountMounth = DropDownList3.SelectedIndex+1;
            firstMounth = DropDownList4.SelectedIndex;
            //BG.Columns.Clear();
            //BG.Rows.Clear();
            BG.Bands.Clear();
            // = null;
            //BG.Columns = null;
            BG.DataBind();
            BG_InitializeLayout(null, null);

        }

        protected void WebAsyncRefreshPanel2_DataBinding(object sender, EventArgs e)
        {
            
        }
    }
}
