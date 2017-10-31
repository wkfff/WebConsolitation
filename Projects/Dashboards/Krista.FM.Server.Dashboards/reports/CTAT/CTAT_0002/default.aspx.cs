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
using System.Drawing;
using System.Collections.ObjectModel;
using System.Text;
using System.Collections.Generic;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

using Infragistics.WebUI.UltraWebChart;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.WebUI.UltraWebGrid;
using Microsoft.AnalysisServices.AdomdClient;

using Dundas.Maps.WebControl;

namespace Krista.FM.Server.Dashboards.reports.CTAT.CTAT_0002
{
    public partial class _default : CustomReportPage
    {
        public const string MeasureFieldName = "MeasureField";
        public const string MeasureMapLabelName = "MapLabelMeasures";
        public const string MapShapeName = "Name";
        public const string MapLegendName = "HighLightLegend";
        public const string MapRuleName = "HighLightRule";
        #region Conf
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
            if (ChartType == Infragistics.UltraChart.Shared.Styles.ChartType.AreaChart)
            {
                //chart.Axis.X.Labels.Font = new Font("arial", 8, FontStyle.Bold);
                //chart.Axis.X.Labels.FontColor = Color.Black;
                chart.Axis.X.Margin.Near.Value = 2;

                chart.AreaChart.ChartText.Add(new Infragistics.UltraChart.Resources.Appearance.ChartTextAppearance(chart, -2, -2, 1 == 1, new Font("arial", 8, FontStyle.Bold), Color.Black, "<DATA_VALUE:### ##0.#>", StringAlignment.Far, StringAlignment.Center, 0));
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
                grid.Columns[i].Width = CRHelper.GetColumnWidth(WidthColumn * 0.943);
                grid.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Left;
                grid.Bands[0].Columns[i].CellStyle.Wrap = 1 == 1;
                CRHelper.FormatNumberColumn(grid.Bands[0].Columns[i], "### ### ### ###.##");
                grid.Bands[0].Columns[i].Header.Style.Wrap = 1 == 1;
            }
            grid.DisplayLayout.GroupByBox.Hidden = 1 == 1;
            //grid.DisplayLayout.RowSelectorsDefault = RowSelectors.Yes;
            //grid.DisplayLayout.RowSelectorStyleDefault.Width = 0;
            grid.Columns[0].CellStyle.BorderDetails.ColorLeft = Color.Gray;
            grid.Columns[0].Header.Caption = "Показатель";
            //grid.Columns[0].
            grid.DisplayLayout.NoDataMessage = "В настоящий момент данные отсутствуют";
            //grid.OnClick += new UltraWebGrid.OnClick(CLIK);
            grid.Height = 0;
        }
        #endregion

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            #region Настройка карты
            map.Meridians.Visible = false;
            map.Parallels.Visible = false;
            //map.ZoomPanel.Visible = false;
            //map.NavigationPanel.Visible = false;

            // добавляем легенду
            /*   Legend legend = new Legend(MapLegendName);
            legend.Visible = true;
            legend.BackColor = Color.White;
            legend.BackSecondaryColor = Color.Gainsboro;
            legend.BackGradientType = GradientType.DiagonalLeft;
            legend.BackHatchStyle = MapHatchStyle.None;
            legend.BorderColor = Color.Gray;
            legend.BorderWidth = 1;
            legend.BorderStyle = MapDashStyle.Solid;
            legend.BackShadowOffset = 4;
            legend.TextColor = Color.Black;
            legend.Font = new Font("MS Sans Serif", 7, FontStyle.Regular);
            legend.Title = string.Empty;
            legend.AutoFitText = true;

            legend.AutoFitMinFontSize = 7;
            map.Legends.Clear();
            map.Legends.Add(legend);*/

            //// добавляем поля
            map.ShapeFields.Clear();
            map.ShapeFields.Add(MapShapeName);
            map.ShapeFields[MapShapeName].Type = typeof(string);
            map.ShapeFields[MapShapeName].UniqueIdentifier = true;
            map.ShapeFields.Add(MeasureFieldName);
            map.ShapeFields[MeasureFieldName].Type = typeof(double);
            map.ShapeFields[MeasureFieldName].UniqueIdentifier = false;

            // добавляем правила раскраски
            map.ShapeRules.Clear();
            ShapeRule rule = new ShapeRule();
            rule.Name = MapRuleName;

            rule.Category = String.Empty;
            rule.ShapeField = MeasureFieldName;
            rule.DataGrouping = DataGrouping.EqualDistribution;
            rule.ColorCount = 7;
            rule.ColoringMode = ColoringMode.ColorRange;
            rule.FromColor = Color.Silver;
            rule.MiddleColor = Color.Gray;
            rule.ToColor = Color.Maroon;
            rule.BorderColor = Color.FromArgb(50, Color.Black);
            rule.GradientType = GradientType.None;
            rule.HatchStyle = MapHatchStyle.None;
            rule.ShowInColorSwatch = true;
            rule.ShowInLegend = MapLegendName;
            map.ShapeRules.Add(rule);

           
            
            #endregion

            // Растянем карту на весь экран
            try
            {
                double dirtyWidth = ((int)Session["width_size"] - 10);
                double dirtyHeight = ((int)Session["height_size"] - 350);

                map.Width = (int)(dirtyWidth);
                map.Height = (int)(dirtyHeight);
            }
            catch
            {
            }
        }

        public void setFont(int typ, Label lab)
        {
            lab.Font.Name = "arial";
            lab.Font.Size = typ;
            if (typ == 14) { lab.Font.Bold = 1 == 1; };
            if (typ == 10) { lab.Font.Bold = 1 == 1; };
            if (typ == 18) { lab.Font.Bold = 1 == 1; };
        }

        static string Pokaz = "";

        /// <summary>
        /// Копирует строку с начала и до 1 вхождения ch символа
        /// </summary>
        /// <param name="s"></param>
        /// <param name="ch"></param>
        /// <returns></returns>
        private string GetString_(string s, char ch)
        {
            string b = s; string res = "";
            try
            {
               
                int i = 0;
                for (i = s.Length - 1; s[i] != ch; i--) ;
                for (int j = 0; j < i; j++)
                {
                    res += s[j];
                }
               

            }
            catch { res = b; }
            return res;
        }


        DataTable dt1 = new DataTable();
        protected override void Page_Load(object sender, EventArgs e)
        {
            // Грузим карту
            map.LoadFromShapeFile(Server.MapPath("../../../maps/CTAT/Водные объекты.shp"), "NAME", true);
            map.Shapes["Shape1"].Color = Color.Blue;
            map.Shapes["Shape1"].Text = "";

            Page.Title = "Отчёт";

            map.LoadFromShapeFile(Server.MapPath("../../../maps/CTAT/Территории.shp"), "NAME", true);
            
            //map.LoadFromShapeFile(Server.MapPath("../../../maps/CTAT/Соседние регионы.shp"), "NAME", true);

            #region DropDounList
            if (!Page.IsPostBack)
            {

                DropDownList1.Items.Add("Масса выбросов в атмосферный воздух загрязняющих веществ, отходящих от стационарных источников");
                DropDownList1.Items.Add("Масса выбросов в атмосферный воздух загрязняющих веществ, отходящих от стационарных источников, в расчете на одного жителя");
                DropDownList1.Items.Add("Масса уловленных и обезвреженных загрязняющих атмосферу веществ, отходящих от стационарных источников");
                DropDownList1.Items.Add("Масса утилизированных загрязняющих веществ, отходящих от стационарных источников");
                DropDownList1.Items.Add("Удельный вес уловленных и обезвреженных загрязняющих веществ, отходящих от стационарных источников, в общем количестве отходящих загрязняющих веществ");
                DropDownList1.Items.Add("Число предприятий, имеющих выбросы загрязняющих веществ в атмосферу");
                for (int i = 2006; i < 2016; i++)
                {
                    DropDownList2.Items.Add(i.ToString());
                }
             //   DropDownList2.SelectedIndex = 1;
            
            
            
            }
            #endregion
            #region DataBind
            TLC.DataBind();
            TRC.DataBind();
          
            G.DataBind();
            Pokaz = "Удельный вес загрязненных сточных вод в общем объеме сбрасываемых вод";
            BC.DataBind();
            //  map.Shapes["a"].
            //map.Serializer
            dt1 = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(@"Select
           non empty {
              [Территории].[РФ Карта].[МР ГО].Members
             
            }on rows  ,
            {
         [Measures].[За период]
            }  on columns  
        from [СТАТ_Экология_Атмосферный воздух] 
        where  
            (  [Период].[Год Квартал Месяц].["+DropDownList2.SelectedItem.Text+@"]
                ,
                [Источники данных].[Источник].[СТАТ Отчетность - Облстат],
                [Группировки].[Экология_Атмосферный воздух].[По городам и районам],
                [Экология].[Атмосферный воздух].["+DropDownList1.SelectedItem.Text+@"]
            )  ", "assdlasdklaldka", dt1);
            
           // map.DataSource = DT; 
            #endregion


            WebAsyncRefreshPanel3.AddLinkedRequestTrigger(G);
            WebAsyncRefreshPanel4.AddLinkedRequestTrigger(Button1);

            TLC.Height = 400;
            TRC.Height = 400;//  CC.Tooltips.FormatString = "<ITEM_LABEL>, <b><DATA_VALUE:00.##></b>"; ;
            TRC.Tooltips.FormatString = "<ITEM_LABEL>, <b><DATA_VALUE:00.##></b>";

            FillMapData();
            map.ColorSwatchPanel.Visible = 1 == 1;
            Label1.Text = DropDownList1.SelectedItem.Text + " в "+DropDownList2.SelectedItem.Text+" году по МР, ГО и ГП";

            if (!Page.IsPostBack)
            {
                setFont(10, Label1);
                setFont(10, Label2);
                setFont(10, Label3);
                setFont(10, Label4);
                setFont(10, Label5);
            }
            #region ED
            string SE = @"WITH   
    MEMBER [Measures].[Единица измерения]   
    AS  '[Экология].[Водные ресурсы].CurrentMember.Properties(" + '"' + "Единица измерения" + '"' + @")'   
Select
    non empty   
    {
 [Measures].[Единица измерения]

    } on columns ,
    non empty   
    {
        Filter
        (
            [Экология].[Водные ресурсы].Members ,
             [Measures].[За период] > 0
        )
    }DIMENSION PROPERTIES MEMBER_UNIQUE_NAME on rows
from [СТАТ_Экология_Водные ресурсы]   
where   
    (
        [Территории].[РФ Карта].[Субъект РФ].[Саратовская обл.]   ,
        [Период].[Год Квартал Месяц].["+DropDownList2.SelectedItem.Text+@"],
        [Источники данных].[Источник].[СТАТ Отчетность - Облстат],
        [Группировки].[Экология_Водные ресурсы].[Без группировки]    
    )   ";
            // e DT = new DataTable();
            CellSet Cs = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(SE);
            for (int i = 0; i < G.Rows.Count; i++)
            {
                try
                {
                    if (Cs.Cells[i].Value.ToString() != "Кубический метр")
                    {
                        G.Rows[i].Cells[0].Text += ", " + Cs.Cells[i].Value.ToString().ToLower();
                    }
                    else
                    {
                        G.Rows[i].Cells[1].Text = (float.Parse(G.Rows[i].Cells[1].Text) / 1000000).ToString();
                        G.Rows[i].Cells[0].Text += ", " + "млн. куб. м.";
                    }
                }
                catch { }
            }
            



            #endregion
            G.Columns[0].Header.Title = "Показатель";
            G.Columns[1].Header.Title = DropDownList2.SelectedItem.Text;

        }

        protected void map_DataBinding(object sender, EventArgs e)
        {

        }



        #region Для карты
        protected MapKindEnum MapKind;
        public enum MapKindEnum
        {
            AllSubjects = 0,
            SingleRegion = 1,
            AllRegions = 2
        }
        protected virtual void FillMapData()
        {
            bool AllFO = MapKind == MapKindEnum.AllRegions;

            if (dt1 == null || map == null) return;

            foreach (DataRow row in dt1.Rows)
            {
                // заполняем карту данными
                string subject = row[0].ToString();
                if (AllFO && RegionsNamingHelper.IsFO(subject) || !AllFO && RegionsNamingHelper.IsSubject(subject))
                {
                    Shape shape = FindMapShape(map, subject, AllFO);
                    if (shape != null)
                    {
                        try
                        {
                            // У фигуры имя есть точно
                            shape[MapShapeName] = subject;
                            shape.ToolTip = "#NAME";
                            shape.TextVisibility = 0;
                            FillShapeMeasuresData(shape, row);
                        }
                        catch { }
                    }
                }
            }
            // Не забываем установить имя легенды в зависимости от выбранного показателя
          //  SetLegendTitle();
        }
              protected Dictionary<string, string> ConvertNamesArray = new Dictionary<string, string>();
        protected virtual Shape FindMapShape(MapControl map, string patternValue, bool searchFO)
        {
            string subject = patternValue;
            string[] subjects = { subject ,""};

            //if (subjects.Length > 1)
            //{
            //    if (ConvertNamesArray.ContainsKey(subjects[0]))
            //    {
            //        subject = ConvertNamesArray[subjects[0]];
            //    }
            //    else
            //    {
            //        bool isRepublic = patternValue.Contains("Республика");
            //        subject = (isRepublic) ? subjects[1] : subjects[0];
            //    };
            //}

            // Встроенный в карту поисковик слишком медленный, так быстрее(раз в 20)
            for (int i = 0; i < map.Shapes.Count; i++)
            {
                if (map.Shapes[i].Name.IndexOf(subject, StringComparison.CurrentCultureIgnoreCase) >= 0)
                {
                    return map.Shapes[i];
                }
            }

            return null;
        }
                            protected virtual void FillShapeMeasuresData(Shape shape, DataRow row)
        {
            // Обрабатываем данные фигуры и формат их вывода
            //switch ((MeasureKindEnum)MeasureKindIndex)
            //{
            //    case MeasureKindEnum.CompletePercent:
                    SetShapeData(shape, row, 1, MeasuresTypesEnum.Money);
            //        break;
            //    case MeasureKindEnum.AvgMenReceipts:
            //        SetShapeData(shape, row, 6, MeasuresTypesEnum.Money);
            //        break;
            //    case MeasureKindEnum.BoostPercent:
            //        SetShapeData(shape, row, 5, MeasuresTypesEnum.Percent);
            //        break;
            //}
        }
        public enum MeasuresTypesEnum
        {
            Money = 0,
            Population = 1,
            Percent = 2
        }
        protected virtual void SetShapeData(Shape shape, DataRow row, int ColumnIndex, MeasuresTypesEnum MeasureType)
        {
            // Данных нет
            if (row[ColumnIndex] == DBNull.Value)
            {
                shape.ToolTip += " <НЕТ ДАННЫХ>";
                return;
            }

            // Если данные есть, то запишем их в поле данных и добавим формат в всплывающую подсказку
            shape[MeasureFieldName] = ConvertMeasureValue(row[ColumnIndex], MeasureType);
            shape.ToolTip += " #MEASUREFIELD{N2}";
            
            shape.Text += (char)(10)+" "+row[1].ToString();

            if (MeasureType == MeasuresTypesEnum.Percent) shape.ToolTip = shape.ToolTip + '%';

            return;
        }
        protected virtual double ConvertMeasureValue(Object CellValue, MeasuresTypesEnum MeasureType)
        {
            if (MeasureType == MeasuresTypesEnum.Percent)
            {
                return 100 * Convert.ToDouble(CellValue);
            }
            else
            {
                return Convert.ToDouble(CellValue);
            }
        }
        #endregion


        protected void TLC_DataBinding(object sender, EventArgs e)
        {
            string SQL = @"Select
    non empty { [ОК].[ОКВЭД].[Раздел].Members  } on columns  ,
    { [Measures].[За период] } on rows 
from [СТАТ_Экология_Атмосферный воздух] 
where 
    (
        [Период].[Год Квартал Месяц].[" + DropDownList2.SelectedItem.Text + @"] ,
        [Территории].[РФ Карта].[Субъект РФ].[Саратовская обл.],
         [Источники данных].[Источник].[СТАТ Отчетность - Облстат],
        [Группировки].[Экология_Атмосферный воздух].[По видам экономической деятельности],
        [Экология].[Атмосферный воздух].[" + DropDownList1.SelectedItem.Text + @"] 
    ) ";


        DataTable DT = new DataTable();
        DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(SQL, "asda", DT);
        TLC.DataSource = DT;
        SetBeautifulChart(TLC, true, Infragistics.UltraChart.Shared.Styles.ChartType.StackBarChart, 36, Infragistics.UltraChart.Shared.Styles.LegendLocation.Bottom, 49);


        Label2.Text = "Структура показателя «"+DropDownList1.SelectedItem.Text+"», по ОКВЭД в "+DropDownList2.SelectedItem.Text+" году";
        }
        /// <summary>
        /// Копирует строку с конца пока не встретит символ ch
        /// </summary>
        /// <param name="s">Где</param>
        /// <param name="ch">Что</param>
        /// <returns>Когда :)</returns>
        private string _GetString_(string s, char ch)
        {
            string res = "";
            try
            {
               
                int i = 0;
                for (i = s.Length - 1; s[i] != ch; i--) { res = s[i] + res; };

               
            }
            catch { }
            return res;
        }

        protected void TRC_DataBinding(object sender, EventArgs e)
        {//Удельный вес загрязненных сточных вод в общем объеме сбрасываемых вод
            string SQL = @"
         Select
    non empty 
    {
        [Период].[Год Квартал Месяц].[1998]    :     [Период].[Год Квартал Месяц].[2020]
    } on columns ,
    non empty   
    {
        [Экология].[Водные ресурсы].[Показатель].["+GetString_(Pokaz,',')+@"]
    }on rows 
from [СТАТ_Экология_Водные ресурсы]  
where  
    (
        [Территории].[РФ Карта].[Субъект РФ].[Саратовская обл.]   ,
         [Measures].[За период],
        [Источники данных].[Источник].[СТАТ Отчетность - Облстат],
        [Группировки].[Экология_Водные ресурсы].[Без группировки]  
    )  ";


        DataTable DT = new DataTable();
        DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(SQL, "askda", DT);




       
        SetBeautifulChart(BC, 1==2, Infragistics.UltraChart.Shared.Styles.ChartType.StackAreaChart, 30, Infragistics.UltraChart.Shared.Styles.LegendLocation.Bottom, 49);
        Label5.Text = ""+GetString_(Pokaz,',')+"";

        if (_GetString_(Pokaz, ',') == " млн. куб. м.")
        {
            object[] O = new object[DT.Rows[0].ItemArray.Length];
           // BC.AreaChart.ChartText.Clear();
            //BC.AreaChart.ChartText.Add(new Infragistics.UltraChart.Resources.Appearance.ChartTextAppearance(BC,-2,-2,
            O[0] = DT.Rows[0].ItemArray[0].ToString();
            for (int i = 1; i < DT.Rows[0].ItemArray.Length; i++)
            {


                O[i] = (Math.Round((float.Parse(DT.Rows[0].ItemArray[i].ToString()) / 1000000),2).ToString());
            }
            DT.Rows.Clear();
            DT.Rows.Add(O);
        }
        else
        {
 
        }

        BC.DataSource = DT;
        }

        protected void TRC_DataBinding1(object sender, EventArgs e)
        {
            string SQL = @"Select
           non empty {
              [Территории].[РФ Карта].[Субъект РФ].[Саратовская обл.]

            }  on columns ,
          non empty  {
            [Экология].[Виды веществ].[Уровень 2].Members
            }on rows 
        from [СТАТ_Экология_Атмосферный воздух]
        where
            (   [Период].[Год Квартал Месяц].[" + DropDownList2.SelectedItem.Text + @"],
                [Measures].[За период],
                [Источники данных].[Источник].[СТАТ Отчетность - Облстат],
                [Группировки].[Экология_Атмосферный воздух].[Группировка].[По видам веществ],
                [Экология].[Атмосферный воздух].[" + DropDownList1.SelectedItem.Text + @"]
            )";


        DataTable DT = new DataTable();
        DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(SQL, "asda", DT);
        TRC.DataSource = DT;
        SetBeautifulChart(TRC, 1==1, Infragistics.UltraChart.Shared.Styles.ChartType.DoughnutChart, 20, Infragistics.UltraChart.Shared.Styles.LegendLocation.Right, 49);


        Label3.Text = "Структура показателя «"+DropDownList1.SelectedItem.Text+"» (жидкие и газообразные загрязняющие вещества), по их видам в "+DropDownList2.SelectedItem.Text+" году";
        }

        protected void G_DataBinding(object sender, EventArgs e)
        {
            string SQL = @"Select
    non empty {    [Measures].[За период] } on columns ,
    non empty  
    {
        [Экология].[Водные ресурсы].Members
    }on rows
from [СТАТ_Экология_Водные ресурсы] 
where 
    (
        [Территории].[РФ Карта].[Субъект РФ].[Саратовская обл.]   ,
        [Период].[Год Квартал Месяц].["+DropDownList2.SelectedItem.Text+@"],
      
        [Источники данных].[Источник].[СТАТ Отчетность - Облстат],
        [Группировки].[Экология_Водные ресурсы].[Без группировки]
        
    )";



        DataTable DT = new DataTable();
        DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(SQL, "asda", DT);
        G.DataSource = DT;

        }

        protected void G_ActiveRowChange(object sender, RowEventArgs e)
        {
            Pokaz = (e.Row.Cells[0].Text); ;
            BC.DataBind();
        }

        protected void G_InitializeLayout(object sender, LayoutEventArgs e)
        {
            SetGridColumn(G, 50);
            G.Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            G.Columns[0].Width = CRHelper.GetColumnWidth(CustomReportConst.minScreenWidth * 0.35);
            G.Columns[1].Width = CRHelper.GetColumnWidth(CustomReportConst.minScreenWidth * 0.10);
            G.Columns[1].Header.Title = DropDownList2.SelectedItem.Text;
            G.Columns[0].Header.Title = "Показатель";
            e.Layout.Bands[0].Columns[0].Header.Title = "Показатель";
            e.Layout.Bands[0].Columns[1].Header.Title = DropDownList2.SelectedItem.Text;
        }
        public static void setChartErrorFont(Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventArgs e)
        {
            e.Text = "В настоящий момент данные отсутствуют";

            e.LabelStyle.FontColor = Color.LightGray;
            e.LabelStyle.VerticalAlign = StringAlignment.Center;
            e.LabelStyle.HorizontalAlign = StringAlignment.Center;
            e.LabelStyle.Font = new Font("Verdana", 30);
            //e.LabelStyle.Font.Size = 15;
        }
        protected void TLC_InvalidDataReceived(object sender, Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventArgs e)
        {
            setChartErrorFont(e);
        }

    }
}
