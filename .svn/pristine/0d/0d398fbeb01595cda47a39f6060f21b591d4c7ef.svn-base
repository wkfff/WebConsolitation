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

//Зарнее извеняюсь за кривой код, и неграмотные коментарии


namespace Krista.FM.Server.Dashboards.reports.EO.EO_0007.EO_001
{
    public partial class _default : CustomReportPage
    {
        //параметры для запросов

        //Последняя дата на которую есть даные
        private CustomParam p1 ;

        //Выбраная територия берётся из параметра
        private CustomParam p2 ;

        //Регион грузящийся по умолчанию
        string region = "Ханты-Мансийский автономный округ - Югра";

        //Имена для диограм
        string LCT = "Структура стоимости закупок";
        string RCT = "Структура количества закупок";

        //Шаблон для вспомогательной текстовки
        string Economy = "Сокращение бюджетных расходов за счет проведения процедур размещения заказа  составило<br>{0} тыс. руб.";

        //Текущаая дата, нужна для текстовки
        string CurentDate = DateTime.Now.ToString();

        //Основная текстовка, в неё встраиваю в коде вспомогательную
        string Textovka = @"
                        <b>{0}</b> 
                        <br><br> 
                        Всего в {1} году (по состоянию на " + DateTime.Now.Day + '.' + DateTime.Now.Month + '.' + DateTime.Now.Year + @") было размещено заказов на сумму<br>{3} тыс. руб." + @"<br>
                        {4}<br><br>
                        <b>Размещение заказов по способам проведения закупок</b>";
        //Строчка с 
        string URLS = 
            @"<a href=" + '"' + "../EO_0005/default.aspx" + '"' + ">1 Реестр контрактов</a><br>";

        /// <summary>
        /// Метод готовит список територий для параметра
        /// </summary>
        /// <param name="sql">Имя запрроса</param>
        /// <returns>Список для параметра</returns>
        Dictionary<string, int> ForParam2(string sql)
        {   //Обевлется Целсет, и сразу в негт тащится результат запроса
            CellSet cs = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(DataProvider.GetQueryText(sql));
            
            //Создаём пустой список 
            Dictionary<string, int> d = new Dictionary<string, int>();
            
            //Пробегаем циклом по результату запроса, и заполняем список
            for (int i = cs.Axes[1].Positions.Count - 1; i >= 0; i--)
            {
                
                d.Add(cs.Axes[1].Positions[i].Members[0].Caption, 0);
            }
            
            return d;
        }

        //Глобальный коэфицент размера, нужен для подгонки размеров компонентов
        double k = 1;
        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            // Так как клас нашей странички наследуется от CustomReportPage то переопределяем его, и выполняем код предка, это обязательно!!!
            base.Page_PreLoad(sender, e);

            //Иницылизируем параметры
            p1 = UserParams.CustomParam("1");
            p2 = UserParams.CustomParam("2");
           
            //Беру имя браузера от которого пришёл запрос, паидее должно работать и без этого но у мя не получилось нормально подогнать размеры кампонентов
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
            //Ой(заьыл переименовать), вобщем Years это компонент(фильтр) с териториями
            Years.Width = 500;

            //Задаём размеры грида,
            Grid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth * 0.68);
            Grid.Height = CRHelper.GetGridHeight(140);
            //диограм
            LC.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth * 0.4903);
            RC.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth * 0.4903);
            RC.Height = 400;
            LC.Height = RC.Height;
            //а также лабеля с урлями и текстом
            URLi.Height = CRHelper.GetGridHeight(288 * k * k * k * k * k * k * k * k * k);

            URLi.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth * 0.317);

            TopText.Width = Grid.Width;
        }
        //Если по какой либо причине диограмма не отобразит то что от неё хотелось бы то покажет то что определен в этом обработчике
        public static void setChartErrorFont(Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventArgs e)
        {
            e.Text = "В настоящий момент данные отсутствуют";

            e.LabelStyle.FontColor = Color.LightGray;
            e.LabelStyle.VerticalAlign = StringAlignment.Center;
            e.LabelStyle.HorizontalAlign = StringAlignment.Center;
            e.LabelStyle.Font = new Font("Verdana", 30);

        }
        /// <summary>
        /// Обявляет и вытаскивает дататабель по имени запроса
        /// </summary>
        /// <param name="sql">Имя в хмельке запросов</param>
        /// <returns></returns>
        public DataTable GetDSForChart(string sql)
        {
            DataTable dt = new DataTable();
            string s = DataProvider.GetQueryText(sql);
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(s, "Показатель", dt);
            return dt;
        }
        /// <summary>
        /// Вытаскивает строчку с последней датой на которую есть данные в базе
        /// </summary>
        /// <param name="qs"></param>
        /// <returns></returns>
        public string LD(string qs)
        {
            CellSet cs = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(DataProvider.GetQueryText(qs));

            return cs.Axes[1].Positions[cs.Axes[1].Positions.Count - 1].Members[0].Caption;
        }
        //вобщем так делать не надо(есть нормальный метод в CRHelper но работает тока для числовых занчений, если там будут скобачки или ещо какянють лишняя фигня то он не чё не делает, паэтому приходиться ручками дописывать нолики 
        string AddZero(string s)
            {//Не заню как откаментировать такой код, написано очень криво, но главное он работает
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
        /// Этот метод ставит пробелы в числе, тем самым визуально разделяя тысячи, милёны итд...
        /// </summary>
        /// <param name="s"></param>
        /// <param name="cg">символ от которого начинается число</param>
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

            //Берём последнюю дату
            p1.Value = LD("lastdate");
            if (!Page.IsPostBack)
            {
                //Берём регион по умолчанию
                p2.Value = region;
                
                //Заполняем фильтр територий и ставим занчение по умолчанию
                Years.FillDictionaryValues(ForParam2("params"));
                Years.Title = "Територия";

                Years.SetСheckedState(region, true);
            }
            else
            {
                //В данном отчёте сюда будет заходить ток тогда когда щолкнули по кнопочке обновить, поэтому в параметр заганяем заново значение територии из фильтра
                p2.Value = Years.SelectedValue;
                region = p2.Value;
            }
            //Заполняем диограммы и грид
            Grid.DataBind();
            LC.DataBind();
            RC.DataBind();

            //(Советую не открывать) Там расщитываються занчения для столбцов(то что в скобочках), и приводяться к норм виду  
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
            //(Советую не открывать) Там вычисляються занчения для текстовки
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
        {//вытаскиваем дататабель и пихаем его в гриду
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
        {//Сталю имена для шапки грида
            e.Layout.HeaderTitleModeDefault = CellTitleMode.Never;
            e.Layout.Bands[0].Columns[0].Header.Caption = "Способ закупки";
            e.Layout.Bands[0].Columns[1].Header.Caption = "Стоимость, тыс. руб.(%)";
            e.Layout.Bands[0].Columns[2].Header.Caption = "Количество, ед.(%)";
            //Смещеня в ячейках
            e.Layout.Bands[0].Columns[1].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[1], "N3");
            
        }
//Два следущих метода устроняэт проблему с тем что в легенде у диограм иногда не влезают подписи, поэтому ручками рисем их
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
