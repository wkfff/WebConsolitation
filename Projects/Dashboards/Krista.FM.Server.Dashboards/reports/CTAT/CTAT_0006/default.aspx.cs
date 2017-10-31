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
using Infragistics.UltraChart.Core.Primitives;

using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.CTAT_0006
{
    public partial class _default : CustomReportPage
    {
        private CustomParam Param1 { get { return (UserParams.CustomParam("1")); } }
        private CustomParam Param2 { get { return (UserParams.CustomParam("2")); } }
        private CustomParam REGION { get { return (UserParams.CustomParam("REGION")); } }
        private CustomParam DATASOURCE { get { return (UserParams.CustomParam("DATASOURCE")); } }
        private CustomParam way_last_year { get { return (UserParams.CustomParam("way_last_year")); } }
        private CustomParam Pokaz { get { return (UserParams.CustomParam("pokaz")); } }
        private CustomParam Lastdate { get { return (UserParams.CustomParam("lastdate")); } }
        private CustomParam Firstyear { get { return (UserParams.CustomParam("firstdate")); } }
        private Int32 screen_width { get { return (int)Session["width_size"]; } }

        private string[] GTQuery = { "GT1", "GT2", "GT3", "GT4"};
        private string[] CTQuery = { "CT11", "CT12", "CT13", "CT14" };
        private string[] GBQuery = { "GB1", "GB2", "GB3", "GB4" };
        private string[] GBE = { "квадратный метр", "квадратный метр", "квадратный метр", "рубль" };
        private string[] CT2Query = { "TC21", "TC22", "TC23", "TC24"};
        private string[] C6Query = { "C61", "C62", "C63", "C64" };
        private static int TGActiveSelect = 1;
        private static int BGActiveSelect = 1;
        private static int IndexRow = 0;

        private string RealLastDate;




        /// <summary>
        /// Метод для рисования на чарте последней даты
        /// используется в делегате чарта поэтому его просто передать надо
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void FillSceneGraph(object sender, Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e)
        {
            try
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
                        Page.Title = "Отчёт";
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
            }
            catch { }
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
        private void setFont(int typ, Label lab)
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
            chart.Width = (int)(CRHelper.GetChartWidth(CustomReportConst.minScreenWidth * (SizePercent / 100)));
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


            //доделать AXis
            chart.Transform3D.Scale = 75;
            chart.Axis.X.Extent = 10;
            chart.Axis.Y.Extent = 50;
            chart.Axis.X.Labels.Orientation = Infragistics.UltraChart.Shared.Styles.TextOrientation.Horizontal;
            chart.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:###,##0.##>";
            //chart.Axis.X.
            chart.Axis.Z.Labels.Visible = 1 == 2;
            chart.Axis.X.Margin.Near.Value = 4;
            if (ChartType == Infragistics.UltraChart.Shared.Styles.ChartType.PieChart3D)
            {
                chart.Transform3D.ZRotation = 0;
                chart.Transform3D.YRotation = 0;
                chart.Transform3D.XRotation = 30;
                chart.Transform3D.Scale = 90;
                chart.PieChart3D.OthersCategoryPercent = 0;
                chart.PieChart3D.OthersCategoryText = "Прочие";
            };
            if (ChartType == Infragistics.UltraChart.Shared.Styles.ChartType.AreaChart) { chart.AreaChart.ChartText.Add(new Infragistics.UltraChart.Resources.Appearance.ChartTextAppearance(chart, -2, -2, 1 == 1, new Font("arial", 8, FontStyle.Bold), Color.Black, "<DATA_VALUE:#>", StringAlignment.Far, StringAlignment.Center, 0)); };
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
                grid.Columns[i].Width = CRHelper.GetColumnWidth(WidthColumn * 0.96);
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
            for (int i = 0; i < query.Length; i++)
            {
                string sb = DataProvider.GetQueryText(query[i]);
                MCS[i] = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(sb);
                DT.Columns.Add(i.ToString());
            }


            //:TODO

            //DT.Columns.Add("Год");


            foreach (Position pos in MCS[0].Axes[0].Positions)
            {
                // создание списка значений для строки UltraWebGrid
                object[] values = new object[query.Length+1];
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



        string BN= "IE";
        protected override void Page_Load(object sender, EventArgs e)
        {
            System.Web.HttpBrowserCapabilities myBrowserCaps = Request.Browser;
            BN = ((System.Web.Configuration.HttpCapabilitiesBase)myBrowserCaps).Browser.ToUpper();

            if (!Page.IsPostBack)
                {

                   
                    REGION.Value = "children";
                    Lastdate.Value = ELV( getLastDate(""));
                    Firstyear.Value = int.Parse((Lastdate.Value)).ToString();
                    TGActiveSelect = 1;
                    IndexRow = 1;

                    setFont(16, TL);
                    setFont(10, Label3);

                    Label3.Text = "Основные показатели по городам и районам за "+Lastdate.Value+" год";

                TG.DataBind();
                GB.DataBind();

                TC1.DataBind();
                TC2.DataBind();
                Lastdate.Value = GB.Columns[GB.Columns.Count - 1].Header.Caption;

                TC3.DataBind();
                TC4.DataBind();
                BGActiveSelect = 1;
                BC1.DataBind();
                BC2.DataBind();
                BC3.DataBind();
                

                TL.Text = "Жилищно-коммунальное хозяйство";

                WebAsyncRefreshPanel1.AddLinkedRequestTrigger(TG);
                WebAsyncRefreshPanel1.AddRefreshTarget(TC1);

                WebAsyncRefreshPanel6.AddLinkedRequestTrigger(GB);

                

                    
                };
        }
        protected void TrTable(UltraWebGrid G)
        {
           
        }



        protected void BG_DataBinding(object sender, EventArgs e)
        {

        }

        protected void CC_DataBinding(object sender, EventArgs e)
        {

        }

        protected void BG_DataBinding1(object sender, EventArgs e)
        {
            
        }

        protected void TG_DataBinding(object sender, EventArgs e)
        {
            Lastdate.Value = ELV(getLastDate(""));
            TG.DataSource = GlueTable(GTQuery);
           
           
            


                ////:TODO

                ////DT.Columns.Add("Год");


                //foreach (Position pos in MCS[0].Axes[0].Positions)
                //{
                //    // создание списка значений для строки UltraWebGrid
                //    object[] values = new object[query.Length + 1];
                //    values[0] = MCS[0].Axes[0].Positions[pos.Ordinal].Members[0].Caption;
                //    for (int i = 1; i != MCS[0].Axes[0].Positions.Count; i++)
                //    {
                //        try
                //        {
                //            if (MCS[i - 1].Cells[pos.Ordinal] == null) { values[i] = ""; }
                //            else
                //            {
                //                values[i] = MCS[i - 1].Cells[pos.Ordinal].Value;
                //            };
                //        }
                //        catch { };
                //    }



                //    // заполнение строки данными
                //    DT.Rows.Add(values);
                //}



            //TG.DataSource = DT;


        }
        private string _GetString_(string s)
        {
            string res = "";
            int i = 0;
            for (i = s.Length - 1; s[i] != ','; i--) { res = s[i] + res; };

            return res;


        }
        protected void CC_DataBinding1(object sender, EventArgs e)
        {
            
        }
        //double Coef=1;
        protected void TC1_DataBinding(object sender, EventArgs e)
        {
            Lastdate.Value = ELV(getLastDate(""));
            REGION.Value = '['+TG.Rows[IndexRow].Cells[0].Text+']';
            Firstyear.Value = (int.Parse(Lastdate.Value)-6).ToString();
            //TC1.DataSource = GetDSForChart(GTQuery[TGActiveSelect-1]);
            TC1.DataSource = GetDSForChart(CTQuery[TGActiveSelect-1]);
            Firstyear.Value = Lastdate.Value;
            
            setFont(10, TCL);
            TCL.Text = "Динамика показателя " + '"' + GetString_(TG.Columns[TGActiveSelect].Header.Caption) + '"' + " (" + ELV(REGION.Value) + "), " + _GetString_(TG.Columns[TGActiveSelect].Header.Caption);

          //  if (BN == "IE") { Coef = 1.05; }
          //  if (BN == "FIREFOX") { Coef = 1.05;}
         //   if (BN == "APPLEMAC-SAFARI") { Coef = 1.05; };


            SetBeautifulChart(TC1, false, Infragistics.UltraChart.Shared.Styles.ChartType.CylinderColumnChart3D, 0, Infragistics.UltraChart.Shared.Styles.LegendLocation.Bottom, 49);
            TC1.Transform3D.XRotation = 125;
            TCL.Width = (int)(0.47 * screen_width - 30);
            TC1.Height = 400;
        }

        protected void TG_Click(object sender, ClickEventArgs e)
        {
            if (e.Cell != null)
            {
                
                TGActiveSelect = e.Cell.Column.Index;
                IndexRow = e.Cell.Row.Index;
                if (TGActiveSelect == 0)
                { TGActiveSelect = 1; }
                TC1.DataBind();
                TC2.DataBind();
                TC4.DataBind();
                TC3.DataBind();
                e.Cell.Row.Activate();
                e.Cell.Row.Activated = 1 == 1;
                e.Cell.Row.Selected = 1 == 1;

                //e.Cell.Column.Selected = 1 == 1;
                TG.Columns[TGActiveSelect].Selected = 1 == 1;
            }
            if (e.Column != null)
            {
                TC1.DataBind();
                TC2.DataBind();
                TC4.DataBind();
                TC3.DataBind();
                
            }


        }

        protected void TC2_ChartDataClicked(object sender, Infragistics.UltraChart.Shared.Events.ChartDataEventArgs e)
        {

        }

        protected void TC2_DataBinding(object sender, EventArgs e)
        {
            try
            {
                TC2.Tooltips.FormatString = "<ITEM_LABEL> " + "<b><DATA_VALUE:00.##></b>" + ", единица ";
                Lastdate.Value = ELV(getLastDate(""));
                REGION.Value = '[' + TG.Rows[IndexRow].Cells[0].Text + ']';
                TC2.DataSource = GetDSForChart(CT2Query[TGActiveSelect-1]);

                TC2L.Width = (int)(screen_width * 0.47 - 30);
                SetBeautifulChart(TC2, 1 == 1, Infragistics.UltraChart.Shared.Styles.ChartType.DoughnutChart, 25, Infragistics.UltraChart.Shared.Styles.LegendLocation.Bottom, 49);
                setFont(10, TC2L);
                if (TGActiveSelect == 1)
                {
                    TC2L.Text = "Число жилых зданий с распределением по типу местности ("+ELV(REGION.Value)+") за "+Lastdate.Value+" год, единица";
                }
                else
                if (TGActiveSelect == 2)
                {
                    TC2L.Text = "Количество жилых квартир с распределением по типу местности  (" + ELV(REGION.Value) + ") за " + Lastdate.Value + " год, единица";                
                }
                else
                if (TGActiveSelect ==4)
                {
                    TC2L.Text = "Число семей, получивших жилые помещения и улучшивших жилищные условия   (" + ELV(REGION.Value) + ") за " + Lastdate.Value + " год, единица";                
                }
                if (TGActiveSelect==3)
                {
                    TC2L.Text = "";
                    //TC2L.Enabled = 1 == 2;
                    TC2.Visible = 8==0;                    
                    //            [__]
                    
                    
                }
                TC2.Height = 400;
            
            
            }
            catch
            {
                TC2L.Text = "";
                //TC2L.Enabled = 1 == 2;
                TC2.Visible = 1 == 2;
            }; 

        }

        protected void TC3_DataBinding(object sender, EventArgs e)
        {
            Lastdate.Value = GB.Columns[GB.Columns.Count - 1].Header.Caption;
            if (TGActiveSelect == 1)
            {

                TC3.Visible = 1 == 1;
                TC4.Visible = 1 == 1;
                TC3L.Visible = 1 == 1;
                TC4L.Visible = 1 == 1;
                REGION.Value = '[' + TG.Rows[IndexRow].Cells[0].Text + ']';
                TC3.DataSource = GetDSForChart("TC3");

                //if (BN == "IE") { Coef = 1.05; }
                //if (BN == "FIREFOX") { Coef = 1.05; }
                //if (BN == "APPLEMAC-SAFARI") { Coef = 1.05; };

                TC3L.Width = (int)( screen_width * 0.47 - 30);
                SetBeautifulChart(TC3, 1 == 1, Infragistics.UltraChart.Shared.Styles.ChartType.PieChart3D, 25, Infragistics.UltraChart.Shared.Styles.LegendLocation.Left, 49);
                setFont(10, TC3L);
                TC3L.Text = "Число жилых зданий с распределением по материалу стен (" + ELV(REGION.Value) + ") за "+ Lastdate.Value+" , единица";
            }
            else
            {
                TC3.Visible = 1 == 2;
                TC4.Visible = 1 == 2;
                TC3L.Text = "";
                TC4L.Text = "";
            }
            
        }

        protected void UltraChart3_ChartDataClicked(object sender, Infragistics.UltraChart.Shared.Events.ChartDataEventArgs e)
        {

        }

        protected void TC4_DataBinding(object sender, EventArgs e)
        {
            Lastdate.Value = GB.Columns[GB.Columns.Count - 1].Header.Caption;
            REGION.Value = '[' + TG.Rows[IndexRow].Cells[0].Text + ']';
            TC4.DataSource = GetDSForChart("TC4");

            //if (BN == "IE") { Coef = 1.05; }
            //if (BN == "FIREFOX") { Coef = 1.05; }
            //if (BN == "APPLEMAC-SAFARI") { Coef = 1.05; };

            TC4L.Width = (int)( screen_width * 0.47 - 30);
            SetBeautifulChart(TC4, 1 == 1, Infragistics.UltraChart.Shared.Styles.ChartType.PieChart3D, 25, Infragistics.UltraChart.Shared.Styles.LegendLocation.Left, 49);
            setFont(10, TC4L);
            TC4L.Text = "Число жилых зданий по времени постройки ("+ELV(REGION.Value)+") за "+ Lastdate.Value+" , единица";
            TC4.Tooltips.FormatString = "<ITEM_LABEL> " + "<b><DATA_VALUE:00.##></b>" + ", единица ";
        }

        protected void UltraWebGrid1_DataBinding(object sender, EventArgs e)
        {

        }

        protected void GB_DataBinding(object sender, EventArgs e)
        {
            Firstyear.Value = (int.Parse(Lastdate.Value) - 6).ToString();
            //GB.DataSource //= GlueTable(GBQuery);
            DataTable DT = new DataTable();
            CellSet[] MCS = new CellSet[GBQuery.Length];
            DT.Columns.Add("Год");
            for (int i = 0; i < GBQuery.Length; i++)
            {
                string sb = DataProvider.GetQueryText(GBQuery[i]);
                MCS[i] = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(sb);
                //DT.Columns.Add(i.ToString());
            }
            for (int i = 0; i < MCS[0].Cells.Count; i++)
            {
                DT.Columns.Add(MCS[0].Axes[0].Positions[i].Members[0].Caption);
            }
            for (int i = 0; i < GBQuery.Length; i++)
            {
                object[] values = new object[MCS[i].Cells.Count+1];
                values[0] = MCS[i].Axes[1].Positions[0].Members[0].Caption;
                for (int j = 1; j < MCS[i].Cells.Count+1; j++)
                {
                    values[j] = MCS[i].Cells[j-1].Value;
                }
                DT.Rows.Add(values);

            }

            GB.DataSource = DT;

            
        }

        protected void BC1_DataBinding(object sender, EventArgs e)
        {
            try
            {
                string sDate = Lastdate.Value;
                Lastdate.Value = GB.Columns[GB.Columns.Count - 1].Header.Caption;
                Firstyear.Value = (int.Parse(Lastdate.Value) - 6).ToString();
                BC1.DataSource = GetDSForChart(GBQuery[BGActiveSelect - 1]);
                Lastdate.Value = sDate;

                //if (BN == "IE") { Coef = 1.03; }
                //if (BN == "FIREFOX") { Coef = 1.03; }
                //if (BN == "APPLEMAC-SAFARI") { Coef = 1.03; };


                SetBeautifulChart(BC1, false, Infragistics.UltraChart.Shared.Styles.ChartType.ColumnChart3D, 0, Infragistics.UltraChart.Shared.Styles.LegendLocation.Left, 49);
                setFont(10, BC1L);
                BC1L.Width = (int)(screen_width * 0.48 - 30);
                BC1L.Text = "Динамика показателя «" + GB.Rows[BGActiveSelect].Cells[0].Text + "», " + GBE[BGActiveSelect - 1];
            }
            catch { }
        }
        static int BGActiveColumn= 6;
        protected void GB_Click(object sender, ClickEventArgs e)
        {
            try
            {
                BGActiveSelect = e.Cell.Row.Index;
                BGActiveColumn = e.Cell.Column.Index;
                Lastdate.Value = e.Cell.Column.Header.Caption;
                BC1.DataBind();
                BC2.DataBind();
                BC3.DataBind();
                e.Cell.Row.Activate();
                e.Cell.Row.Activated = 1 == 1;
                e.Cell.Row.Selected = 1 == 1;

                e.Cell.Column.Selected = 1 == 1;
                GB.Columns[BGActiveColumn].Selected = 1 == 1;
            }
            catch { }

            //:TODO
        }

        protected void UltraChart5_DataBinding(object sender, EventArgs e)
        {
            
        }

        protected void BC2_DataBinding(object sender, EventArgs e)
        {
            Lastdate.Value = GB.Columns[BGActiveColumn].Header.Caption;
            BC2.DataSource = GetDSForChart(C6Query[BGActiveSelect-1]);

            //if (BN == "IE") { Coef = 1.03; }
            //if (BN == "FIREFOX") { Coef = 1.03; }
            //if (BN == "APPLEMAC-SAFARI") { Coef = 1.03; };


            SetBeautifulChart(BC2, true, Infragistics.UltraChart.Shared.Styles.ChartType.PieChart3D, 40, Infragistics.UltraChart.Shared.Styles.LegendLocation.Bottom, 49);
            setFont(10, BC2L);
            BC2L.Text = GB.Rows[BGActiveSelect].Cells[0].Text + " за " + Lastdate.Value + " год, " + GBE[BGActiveSelect-1];
            BC2L.Width = (int)(screen_width * 0.48 - 30);
            BC2.Tooltips.FormatString = "<ITEM_LABEL> " + "<b><DATA_VALUE:00.##></b>" + _GetString_(BC2L.Text);
        }

        protected void UltraChart6_DataBinding(object sender, EventArgs e)
        {

        }

        protected void BC3_DataBinding(object sender, EventArgs e)
        {
            Lastdate.Value = GB.Columns[BGActiveColumn].Header.Caption;
            BC3.DataSource = GetDSForChart("C7");

     //       if (BN == "IE") { Coef = 1.03; }
     //       if (BN == "FIREFOX") { Coef = 1.03; }
     //       if (BN == "APPLEMAC-SAFARI") { Coef = 1.03; };

            SetBeautifulChart(BC3, true, Infragistics.UltraChart.Shared.Styles.ChartType.ColumnChart, 10, Infragistics.UltraChart.Shared.Styles.LegendLocation.Bottom, 99);
            BC3.Axis.Z.Visible = 1 == 1;
            BC3.Axis.X.Labels.Visible = 1 == 2;
            BC3.Axis.X.Extent = 25;
            BC3.Axis.Y.Extent = 80;
            setFont(10, BC3L);
            BC3L.Text = "Доходы и расходы организаций ЖКХ за "+Lastdate.Value+" год, млн. рублей";
            BC3L.Width = (int)(screen_width * 0.97 - 30);
            BC3.Height = 400;
            BC3.Tooltips.FormatString = "<ITEM_LABEL> " + "<b><DATA_VALUE:00.##></b>" + ", единица ";
        }

        protected void TG_InitializeLayout(object sender, LayoutEventArgs e)
        {

            //if (BN == "IE") { Coef = 1.035; }
            //if (BN == "FIREFOX") { Coef = 1.037; }
            //if (BN == "APPLEMAC-SAFARI") { Coef = 1.055; }

            SetGridColumn(TG, 99);
            TG.Columns[0].Header.Caption = "Територия";
            TG.Columns[1].Header.Caption = "Число жилых зданий, единица";
            TG.Columns[2].Header.Caption = "Количество жилых квартир, единица";
            TG.Columns[3].Header.Caption = "Общая заселенная площадь, квадратный метр";
            TG.Columns[4].Header.Caption = "Число семей, получивших жилые помещения и улучшивших жилищные условия, единица";
            e.Layout.RowSelectorsDefault = RowSelectors.No;
            e.Layout.CellClickActionDefault = CellClickAction.CellSelect;
            e.Layout.AllowSortingDefault = AllowSorting.Yes;
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
        }

        protected void GB_InitializeLayout(object sender, LayoutEventArgs e)
        {
            //if (BN == "IE") { Coef = 1.035; }
            //if (BN == "FIREFOX") { Coef = 1.037; }
            //if (BN == "APPLEMAC-SAFARI") { Coef = 1.055; };


            SetGridColumn(GB, 99);

            


            //GB.Columns[0].Header.Caption = "Год";
            //GB.Columns[1].Header.Caption = "Площадь жилищного фонда, " + GBE[0];
            //GB.Columns[2].Header.Caption = "Выбыло площади жилищного фонда, " + GBE[1]; ;
            //GB.Columns[3].Header.Caption = "Численность граждан пользующихся льготами по оплате жилья и коммунальных услуг, " + GBE[2]; ;
            //GB.Columns[4].Header.Caption = "Фактический сбор жилищно-коммунальных платежей от населения, " + GBE[3] ;
            GB.Columns[0].Width = CRHelper.GetColumnWidth(CustomReportConst.minScreenWidth * 0.3);
            for (int i = 1; i < GB.Columns.Count ; i++)
            {
                GB.Columns[i].Width = CRHelper.GetColumnWidth((CustomReportConst.minScreenWidth * 0.7)*(0.94) / (GB.Columns.Count-1));            
            }
                e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.RowSelectorsDefault = RowSelectors.No;
            e.Layout.CellClickActionDefault = CellClickAction.CellSelect;
            GB.Height = 140;
        }

        protected void SetErorFonnL(object sender, Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventArgs e)
        {
            TC3.Visible = 1 == 2;
        }

        protected void SetErorFonnE(object sender, Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventArgs e)
        {
            TC4.Visible = 1 == 2;
        }
    }
}
