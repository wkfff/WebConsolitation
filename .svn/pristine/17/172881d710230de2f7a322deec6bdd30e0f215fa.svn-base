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
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core.DataProviders;

using Infragistics.WebUI.UltraWebChart;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.WebUI.UltraWebGrid;
using Microsoft.AnalysisServices.AdomdClient;

using Dundas.Maps.WebControl;
using Infragistics.Documents.Reports.Report.Section;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Infragistics.WebUI.UltraWebGrid.DocumentExport;
using Infragistics.Documents.Reports.Report.Flow;
using Infragistics.Documents.Reports.Report.Table;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Band;
using Infragistics.Documents.Reports.Report.Segment;
using Infragistics.Documents.Reports.Report.QuickText;
using Infragistics.Documents.Reports.Report.QuickList;
using Infragistics.Documents.Reports.Report.QuickTable;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.Documents.Reports.Report.Grid;
using Infragistics.Documents.Reports.Report.Tree;
using Infragistics.Documents.Reports.Report.TOC;
using Infragistics.Documents.Reports.Report.Index;
using Infragistics.Documents.Excel;

//using Infragistics.Documents.Reports.Graphics;

/*
using System.IO;
using Infragistics.Documents.Reports.Report.List;
using Infragistics.Documents.Excel;
using EndExportEventArgs = Infragistics.WebUI.UltraWebGrid.ExcelExport.EndExportEventArgs;
using InitializeRowEventHandler = Infragistics.WebUI.UltraWebGrid.InitializeRowEventHandler;
using MASSpace = Microsoft.AnalysisServices.AdomdClient;
*/

namespace Krista.FM.Server.Dashboards.reports.CTAT.CTAT_0105
{
    public partial class _default : CustomReportPage
    {
        // отношение height/width
        private double mapSizeProportion = 1;

        protected CustomParam Param1 { get { return (UserParams.CustomParam("1")); } }
        protected CustomParam Param2 { get { return (UserParams.CustomParam("2")); } }
        protected CustomParam Param3 { get { return (UserParams.CustomParam("3")); } }
        protected CustomParam DATASOURCE { get { return (UserParams.CustomParam("DATASOURCE")); } }
        private CustomParam way_last_year { get { return (UserParams.CustomParam("way_last_year")); } }
        protected CustomParam Pokaz { get { return (UserParams.CustomParam("pokaz")); } }
        protected CustomParam Lastdate { get { return (UserParams.CustomParam("lastdate")); } }
        protected CustomParam Firstyear { get { return (UserParams.CustomParam("firstdate")); } }
        private Int32 screen_width { get { return (int)Session["width_size"] - 50; } }
        string BN = "IE";
        // параметр запроса для региона
        private CustomParam baseRegion { get { return (UserParams.CustomParam("baseRegion")); } }
        // путь к карте
        private CustomParam FileMapName { get { return (UserParams.CustomParam("FileMapName")); } }

        public const string MeasureFieldName = "MeasureField";
        public const string MeasureMapLabelName = "MapLabelMeasures";
        public const string MapShapeName = "Name";
        public const string MapLegendName = "HighLightLegend";
        public const string MapRuleName = "HighLightRule";


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

            if (dt1 == null || DundasMap == null) return;

            foreach (DataRow row in dt1.Rows)
            {
                // заполняем карту данными
                //string subject = row[0].ToString();
                
                string subject = string.Empty;
                subject = row[0].ToString();
                string[] subjects = subject.Split(' ');
                subject = subjects[0] + " р-н";
                

                if (AllFO && RegionsNamingHelper.IsFO(subject) || !AllFO && RegionsNamingHelper.IsSubject(subject))
                {


                    Shape shape = FindMapShape(DundasMap, subject, AllFO);
                    if (shape != null)
                    {
                        //try
                        //{
                        // У фигуры имя есть точно
                        shape[MapShapeName] = subject;
                        shape.ToolTip = "#NAME";
                        shape.TextVisibility = 0;
                        FillShapeMeasuresData(shape, row);
                        //}
                        //catch { }
                    }
                }
            }
            // Не забываем установить имя легенды в зависимости от выбранного показателя
            //  SetLegendTitle();
        }
        string rang = "";
        protected Dictionary<string, string> ConvertNamesArray = new Dictionary<string, string>();

        protected virtual Shape FindMapShape(MapControl DundasMap, string patternValue, bool searchFO)
        {
            string subject = patternValue;
            string[] subjects = { subject, "" };



            // Встроенный в карту поисковик слишком медленный, так быстрее(раз в 20)
            for (int i = 0; i < BG.Rows.Count; i++)
            {
                if (subject == BG.Rows[i].Cells[0].Text)
                {
                    rang = BG.Rows[i].Cells[indexColumn+1].Text;
                    break;
                }
            }

            for (int i = 0; i < DundasMap.Shapes.Count; i++)
            {
                if (DundasMap.Shapes[i].Name.IndexOf(subject, StringComparison.CurrentCultureIgnoreCase) >= 0)
                {
                    return DundasMap.Shapes[i];
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
            shape[MeasureFieldName] = ConvertMeasureValue( (row[ColumnIndex]), MeasureType);
            shape.ToolTip += " #MEASUREFIELD{N2}";

            shape.Text += (char)(10) + " " + row[1].ToString()+"("+rang+")";

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

            e.LabelStyle.FontColor = System.Drawing.Color.LightGray;
            e.LabelStyle.VerticalAlign = StringAlignment.Center;
            e.LabelStyle.HorizontalAlign = StringAlignment.Center;
            e.LabelStyle.Font = new System.Drawing.Font("Verdana", 30);
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
            double size = 1;
            if (BN == "IE")
            {
                //     size = 1;
            }
            if (BN == "FIREFOX")
            {
                // size = 0.90;
            }
            if (BN == "APPLEMAC-SAFARI")
            {
                //       size = 0.90;
            }



            for (int i = 0; i < grid.Columns.Count; i++)
            {
                grid.Columns[i].Width = CRHelper.GetColumnWidth(WidthColumn * 0.93 * size);
                grid.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Left;
                grid.Bands[0].Columns[i].CellStyle.Wrap = 1 == 1;
                CRHelper.FormatNumberColumn(grid.Bands[0].Columns[i], "### ### ### ##0.##");
                grid.Bands[0].Columns[i].Header.Style.Wrap = 1 == 1;
            }
            grid.Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            grid.DisplayLayout.GroupByBox.Hidden = 1 == 1;
            //if ((rowSelector.Length > 0) && (rowSelector[0])) { grid.Columns[0].CellStyle.BorderDetails.ColorLeft = Color.Gray; }
            grid.DisplayLayout.NoDataMessage = "В настоящий момент данные отсутствуют";


            //grid.Height = 0;
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

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            if (e != null) { base.Page_PreLoad(sender, e); };

            #region Настройка карты
            DundasMap.Meridians.Visible = false;
            DundasMap.Parallels.Visible = false;

            //// добавляем поля
            DundasMap.ShapeFields.Clear();
            DundasMap.ShapeFields.Add(MapShapeName);
            DundasMap.ShapeFields[MapShapeName].Type = typeof(string);
            DundasMap.ShapeFields[MapShapeName].UniqueIdentifier = true;
            DundasMap.ShapeFields.Add(MeasureFieldName);
            DundasMap.ShapeFields[MeasureFieldName].Type = typeof(double);
            DundasMap.ShapeFields[MeasureFieldName].UniqueIdentifier = false;

            // добавляем правила раскраски
            DundasMap.ShapeRules.Clear();
            ShapeRule rule = new ShapeRule();
            rule.Name = MapRuleName;

            rule.Category = String.Empty;
            rule.ShapeField = MeasureFieldName;
            rule.DataGrouping = DataGrouping.EqualDistribution;
            rule.ColorCount = 10;
            rule.ColoringMode = ColoringMode.ColorRange;
            rule.FromColor = Color.Maroon;
            rule.MiddleColor =  Color.Gray;
            rule.ToColor =  Color.Silver;
            rule.BorderColor = Color.FromArgb(50, Color.Black);
            rule.GradientType = GradientType.None;
            rule.HatchStyle = MapHatchStyle.None;
            rule.ShowInColorSwatch = true;
            rule.ShowInLegend = MapLegendName;
            DundasMap.ShapeRules.Add(rule);

            //HighLightLegend



            #endregion

            // Растянем карту на весь экран
            try
            {
                double dirtyWidth = ((int)Session["width_size"] /2);
                double dirtyHeight = ((int)Session["height_size"]/2);

                DundasMap.Width = (int)(dirtyWidth) - 5;
                DundasMap.Height = (int)(dirtyHeight);

                UltraGridExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
                UltraGridExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
                UltraGridExporter1.ExcelExporter.EndExport += new EndExportEventHandler(ExcelExporter_EndExport);
                UltraGridExporter1.PdfExportButton.Visible = false;
                UltraGridExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
                UltraGridExporter1.PdfExporter.BeginExport += new EventHandler<DocumentExportEventArgs>(PdfExporter_BeginExport);
            }
            catch
            {
            }
        }

        public static Collection<string> getDates(string q)
        {
            CellSet CS = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(DataProvider.GetQueryText(q));
            Collection<string> C = new Collection<string>();
            for (int i = 0;i<CS.Axes[1].Positions.Count;i++)
            {
                C.Add(CS.Axes[1].Positions[i].Members[0].Caption);
            }
            return C;
        }



        private void LoadDateToList(DropDownList DDL, int LastDatee, int FirstDate)
        {
            for (int i = FirstDate; LastDatee >= i; i++)
            {
                DDL.Items.Add(i.ToString());
            }
        }

        DataTable dt1;
         static int indexColumn = 1;
        protected override void Page_Load(object sender, EventArgs e)
        {
            try
            {
                base.Page_Load(sender, e);


                if (!Page.IsPostBack)
                {
                    RegionSettingsHelper.Instance.SetWorkingRegion(RegionSettings.Instance.Id);

                    #region Информация
                    string innerText = string.Empty;
                    try
                    {
                        string filePath = HttpContext.Current.Server.MapPath("default.html");
                        System.IO.StreamReader sr = new System.IO.StreamReader(filePath);
                        string line;
                        while ((line = sr.ReadLine()) != null)
                            innerText = innerText + line;
                    }
                    catch (Exception) { }

                    Control container = Page.LoadControl("~/Components/ContainerPanel.ascx");
                    HtmlTable htmlTable = new HtmlTable();
                    HtmlTableRow htmlRow = new HtmlTableRow();
                    HtmlTableCell htmlCell = new HtmlTableCell();
                    Label shortBody = new Label();
                    shortBody.Text = innerText;
                    shortBody.CssClass = "ReportDescription";
                    htmlCell.Controls.Add(shortBody);
                    htmlRow.Cells.Add(htmlCell);
                    htmlTable.Rows.Add(htmlRow);

                    ((ContainerPanel)container).AddContent(htmlTable);
                    ((ContainerPanel)container).AddHeader("ПРЕДЛОЖЕНИЯ ПО ПОВЫШЕНИЮ ЭФФЕКТИВНОСТИ ДЕЯТЕЛЬНОСТИ ОРГАНОВ МЕСТНОГО СМОУПРАВЛЕНИЯ");
                    ContactInformationPlaceHolder.Controls.Add(container);
                    #endregion


                    baseRegion.Value = RegionSettingsHelper.Instance.RegionBaseDimension;
                    FileMapName.Value = RegionSettingsHelper.Instance.FileMapName;

                    Lastdate.Value = ELV(getLastDate("last_date"));
                    
                    CustomMultiCombo1.FillValues(getDates("dates"));
                    CustomMultiCombo1.SetСheckedState(Lastdate.Value, true);
                    CustomMultiCombo1.Title = "Год";

                    System.Web.HttpBrowserCapabilities myBrowserCaps = Request.Browser;
                    BN = ((System.Web.Configuration.HttpCapabilitiesBase)myBrowserCaps).Browser.ToUpper();
                    TG.DataBind();
                    C.DataBind();
                    BG.DataBind();
                    BG.DisplayLayout.CellClickActionDefault = CellClickAction.CellSelect;



                    MP.AddLinkedRequestTrigger(BG);


                     { BG_Click(null, null); }




                }
                else
                {
                    try
                    {
                        Lastdate.Value = CustomMultiCombo1.SelectedValue;
                        TG.DataBind();
                        C.DataBind();
                        BG.DataBind();
                        if (!Page.IsPostBack) { BG_Click(null, null); }
                        TG.Visible = 1 == 1;
                        BG.Visible = 1 == 1;
                        Label6.Text = Label7.Text = "";
                    }
                    catch
                    {
                        TG.DataSource = null;
                        BG.DataSource = null;
                        TG.Visible = 1 == 2;
                        BG.Visible = 1 == 2;
                        Label6.Text = Label7.Text = "Нет данных";
                    }
                }
                try
                { }
                catch
                { }
                BG.Columns[0].Width = (int)(CustomReportConst.minScreenWidth * 0.09);
                TG.Columns[0].Width = (int)(CustomReportConst.minScreenWidth * 0.09);


                //TG.Columns[0].Width = (int)(CustomReportConst.minScreenWidth * 0.08);
                for (int i = 1; i < TG.Columns.Count; i += 2)
                {
                    TG.Columns[i].Width = (int)(CustomReportConst.minScreenWidth * 0.1);
                    for (int j = 0; j < TG.Rows.Count; j++)
                    {
                        if (TG.Rows[j].Cells[i].Text.Length < 6) { TG.Rows[j].Cells[i].Text += '0'; }

                    }


                }
                //  SetGridColumn(TG, 50);
                for (int i = 2; i < TG.Columns.Count; i += 2)
                {
                    TG.Columns[i].Width = (int)(CustomReportConst.minScreenWidth * 0.027);

                }
                for (int i = 1; i < BG.Columns.Count; i += 2)
                {
                    BG.Columns[i].Width = (int)(CustomReportConst.minScreenWidth * 0.1);

                    for (int j = 0; j < BG.Rows.Count; j++)
                    {
                        if (BG.Rows[j].Cells[i].Text.Length < 6) { BG.Rows[j].Cells[i].Text += '0'; }

                    }

                }
                //  SetGridColumn(TG, 50);
                for (int i = 2; i < BG.Columns.Count; i += 2)
                {
                    BG.Columns[i].Width = (int)(CustomReportConst.minScreenWidth * 0.027);

                }
                BG.Columns[2].Hidden = 2 == 2;
                BG.Columns[4].Hidden = 2 == 2;
                TG.Columns[2].Hidden = 2 == 2;
                TG.Columns[4].Hidden = 2 == 2;


                BG.Height = BG.Rows.Count * 25;
                DundasMap.Height = DundasMap.Width;

            }
            catch
            { }


            
        }

        protected void UltraWebGrid1_DataBinding(object sender, EventArgs e)
        {
            For_rang(TG, "TG", "Город");

        }

        protected void For_rang(UltraWebGrid web_grid1, string sql, string FirstColumn)
        {
            try
            {
                //DataTable grid1_table = new DataTable();
                //DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("grid1"), "Область", grid1_table);
                //web_grid1.DataSource = grid1_table.DefaultView;


                CellSet grid_set = null;
                DataTable grid_table = new DataTable();
                // Загрузка таблицы цен и товаров в CellSet
                grid_set = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(DataProvider.GetQueryText(sql));
                // Добавление столбцов в таблицу и заполнение данных в DataTable
                int columnsCount = grid_set.Axes[0].Positions.Count;
                int rowsCount = grid_set.Axes[1].Positions.Count;
                grid_table.Columns.Add(FirstColumn);
                for (int i = 0; i < columnsCount; ++i)
                {
                    grid_table.Columns.Add(grid_set.Axes[0].Positions[i].Members[0].Caption.Replace("\"", "&quot;"));
                    //grid_table.Columns[grid_table.Columns.Count - 1].Caption = grid_columns[i];
                    grid_table.Columns.Add();
                    grid_table.Columns[grid_table.Columns.Count - 1].Caption = "Ранг";
                }

                object[,] cells = new object[rowsCount, columnsCount * 2 + 1];
                for (int i = 0; i < rowsCount; ++i)
                {
                    cells[i, 0] = grid_set.Axes[1].Positions[i].Members[0].Caption.Replace("муниципальный район", "р-н"); ;
                    for (int j = 0; j < columnsCount; ++j)
                    {
                        cells[i, j * 2 + 1] = grid_set.Cells[j, i].Value;
                    }
                }


                string[,] range = new string[columnsCount, rowsCount];
                for (int i = 0; i < columnsCount; ++i)
                {
                    Boolean[] checkedVal = new Boolean[rowsCount];
                    double realMax = 0;
                    double max = 0;
                    int rangeCount = 0;
                    int minIndex_ = 0;

                    for (int j = 0; j < rowsCount; ++j)
                    {
                        rangeCount++;
                        //поиск максимального элемента
                        Boolean first = true;
                        double min = 0;
                        int minIndex = 0;
                        for (int k = 0; k < rowsCount; ++k)
                        {
                            if (checkedVal[k]) continue;
                            if (first)
                            {
                                min = Convert.ToDouble(cells[k, i * 2 + 1]);
                                first = false;
                                minIndex = k;
                            }
                            if (Convert.ToDouble(cells[k, i * 2 + 1]) > min)
                            {
                                min = Convert.ToDouble(cells[k, i * 2 + 1]);
                                minIndex = k;
                            }
                        }
                        if (!checkedVal[minIndex])
                        {
                            realMax = Convert.ToDouble(cells[minIndex, i * 2 + 1]);
                            range[i, minIndex] = rangeCount.ToString();
                            checkedVal[minIndex] = true;
                            minIndex_ = minIndex;
                        }

                        // поиск равных элементов
                        //                        Boolean zeroFind = false;
                        for (int k = 0; k < rowsCount; ++k)
                        {
                            if (checkedVal[k]) continue;
                            if (Convert.ToDouble(cells[k, i * 2 + 1]) == realMax)
                            {
                                //                                if (realMax == 0)
                                //                                    zeroFind = true;
                                range[i, k] = rangeCount.ToString();
                                checkedVal[k] = true;
                            }
                        }
                        //                        if (zeroFind) rangeCount++;


                    }
                    string maxR = range[i, minIndex_];
                    for (int k = 0; k < rowsCount; k++)
                        if (range[i, k] == maxR)
                            range[i, k] = "+" + maxR;
                }


                for (int i = 0; i < rowsCount; ++i)
                {
                    int t = 0;
                    for (int j = 2; j < columnsCount * 2 + 1; j = j + 2)
                    {
                        cells[i, j] = range[t, i];
                        t++;
                    }
                }

                for (int i = 0; i < rowsCount; ++i)
                {
                    object[] values = new object[columnsCount * 2 + 1];
                    for (int j = 0; j < columnsCount * 2 + 1; ++j)
                    {
                        values[j] = cells[i, j];

                    }
                    if (values[6].ToString().Length == 1) { values[6] = " " + values[6]; }
                    grid_table.Rows.Add(values);
                }

                web_grid1.DataSource = grid_table.DefaultView;

            }
            catch (Exception exception) // блок для обработки исключений
            {
                throw new Exception(exception.Message, exception);
            }

        }


        protected void C_DataBinding(object sender, EventArgs e)
        {
            C.DataSource = GetDSForChart("C");
            SetBeautifulChart(C, true, Infragistics.UltraChart.Shared.Styles.ChartType.ColumnChart, 30, Infragistics.UltraChart.Shared.Styles.LegendLocation.Bottom, 50);
        }

        protected void TG_InitializeLayout(object sender, LayoutEventArgs e)
        {
            SetGridColumn(TG, 48);



        }

        protected void BG_DataBinding(object sender, EventArgs e)
        {
            For_rang(BG, "BG", "Район"); ///GetDSForChart("BG");
        }

        protected void BG_ClickCellButton(object sender, CellEventArgs e)
        {

        }

        protected void BG_Click(object sender, Infragistics.WebUI.UltraWebGrid.ClickEventArgs e)
        {
            try
            {
                try
                {
                    e.Column.Header.Caption = e.Column.Header.Caption;
                }
                catch
                
                {
                    if (e == null) { indexColumn = 5; } else { indexColumn = e.Cell.Column.Index; }
                    if (indexColumn == 0) { indexColumn = 5; }
                    int res;
                    Math.DivRem(indexColumn, 2, out res);
                    if ((res == 0))
                    {
                        indexColumn--;
                    }
                }
            }
            catch 
            {
                try
                {
                    //int res;
                    //indexColumn = e.Column.Index;
                    //Math.DivRem(indexColumn, 2, out res);
                    //if ((res == 0))
                    //{
                    //    indexColumn--;
                    //}
                }
                catch
                {
                    //indexColumn = 5;
                }
            }


            Pokaz.Value = BG.Columns[indexColumn].Header.Caption;
            DundasMap.Shapes.Clear();
            DundasMap.LoadFromShapeFile(Server.MapPath(FileMapName.Value), "NAME", true);
            dt1 = GetDSForChart("map");

            string subject = string.Empty;
            foreach (DataRow row in dt1.Rows)
            {
                subject = row[0].ToString();
                row[0] = subject.Replace("муниципальный район", "р-н");
            }

            FillMapData();
            Label4.Text = Pokaz.Value;
            BG.Height = BG.Rows.Count * 25;
            try
            {
                
                //for (int i = 0; i < BG.Columns.Count; i++)
                //{
                //    BG.Columns[i].Selected = 1 == 2;
                //}
                //BG.Columns[indexColumn].Selected = 1 == 1;
            
            }
            catch
            { }
            
        }

        protected void BG_InitializeLayout(object sender, LayoutEventArgs e)
        {
            SetGridColumn(BG, 48);
        }

        protected void BG_InitializeRow(object sender, RowEventArgs e)
        {
            for (int i = 2; i < e.Row.Cells.Count; i++)
            {
                if ((e.Row.Cells[i].Text == " 1") & (i % 2 == 0))
                {
                    if ((i != 2) & (i != 4))
                    {
                        e.Row.Cells[i - 1].Style.BackgroundImage = "~/images/greenRect.gif";
                        e.Row.Cells[i - 1].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
                    }
                }
                if ((e.Row.Cells[i].Text.Substring(0, 1) == "+") & (i % 2 == 0))
                {

                    e.Row.Cells[i].Text = e.Row.Cells[i].Text.Substring(1);
                    if ((i != 2) & (i != 4))
                    {
                        e.Row.Cells[i - 1].Style.BackgroundImage = "~/images/redRect.gif";
                        e.Row.Cells[i - 1].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px; ";
                    }
                }
            }
        }

        protected void C_ChartDataClicked(object sender, Infragistics.UltraChart.Shared.Events.ChartDataEventArgs e)
        {

        }





        #region Экпорт в Excel

        private void ExcelExporter_BeginExport(object sender, Infragistics.WebUI.UltraWebGrid.ExcelExport.BeginExportEventArgs e)
        {
            e.Workbook.Worksheets["Городские округа"].Rows[0].Cells[0].Value = Label2.Text;
            e.Workbook.Worksheets["Городские округа"].Rows[0].Cells[0].CellFormat.Font.Bold = Infragistics.Documents.Excel.ExcelDefaultableBoolean.True;
            e.Workbook.Worksheets["Городские округа"].Rows[1].Cells[0].Value = CustomMultiCombo1.SelectedValue + " год";

            e.Workbook.Worksheets["Муниципальные районы"].Rows[0].Cells[0].Value = Label2.Text;
            e.Workbook.Worksheets["Муниципальные районы"].Rows[0].Cells[0].CellFormat.Font.Bold = Infragistics.Documents.Excel.ExcelDefaultableBoolean.True;
            e.Workbook.Worksheets["Муниципальные районы"].Rows[1].Cells[0].Value = CustomMultiCombo1.SelectedValue + " год";
        }

        private void ExcelExporter_EndExport(object sender, Infragistics.WebUI.UltraWebGrid.ExcelExport.EndExportEventArgs e)
        {
            foreach (Worksheet sheet in e.Workbook.Worksheets)
            {
                sheet.Columns[0].Width = 120 * 37;
                sheet.Columns[1].Width = 200 * 37;
                sheet.Columns[2].Width = 200 * 37;
                sheet.Columns[3].Width = 200 * 37;
            }
        }

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            Workbook workbook = new Workbook();
            Worksheet sheet1 = workbook.Worksheets.Add("Городские округа");
            Worksheet sheet2 = workbook.Worksheets.Add("Муниципальные районы");

            UltraGridExporter1.ExcelExporter.DownloadName = "report.xls";

            UltraGridExporter1.ExcelExporter.ExcelStartRow = 4;
            UltraGridExporter1.ExcelExporter.Export(TG, sheet1);

            UltraGridExporter1.ExcelExporter.ExcelStartRow = 4;
            UltraGridExporter1.ExcelExporter.Export(BG, sheet2);
        }

        #endregion

        #region Экспорт в PDF

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            UltraGridExporter1.PdfExporter.DownloadName = "report.pdf";
            Report report = new Report();
            ReportSection section1 = new ReportSection(report, true);

            

            UltraGridExporter1.PdfExporter.Export(TG, section1);
            UltraGridExporter1.PdfExporter.Export(BG, section1);
            section1.AddFlowColumnBreak();
/*
            UltraGridExporter1.PdfExporter.Export(grid1, section1);
            UltraGridExporter1.PdfExporter.Export(grid2, section1);
            UltraGridExporter1.PdfExporter.Export(grid5, section1);
*/
        }

        private bool titleAndMapAdded = false;

        private void PdfExporter_BeginExport(object sender, Infragistics.WebUI.UltraWebGrid.DocumentExport.DocumentExportEventArgs e)
        {
            
            //InitializeExportLayout(e);
            if (titleAndMapAdded)
                return;
           
            IText title = e.Section.AddText();
            System.Drawing.Font font = new System.Drawing.Font("Verdana", 16);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.Style.Font.Bold = true;
            title.AddContent(Label2.Text);

            title = e.Section.AddText();
            font = new System.Drawing.Font("Verdana", 14);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.AddContent(CustomMultiCombo1.SelectedValue + " год");

            Infragistics.Documents.Reports.Graphics.Image img = UltraGridExporter.GetImageFromMap(DundasMap);
            e.Section.AddImage(img);

            titleAndMapAdded = true;
        }

        private void InitializeExportLayout(DocumentExportEventArgs e)
        {
            string gridCaption = e.Layout.Bands[0].Grid.Caption;
            int i = 0;
            /*
            if (gridCaption == EKRGridCaption)
            {
                i = 1;
                e.Layout.Grid.Columns[1].Hidden = true;
            }
            
            e.Layout.Grid.Columns[i + 1].Width = 108;
            e.Layout.Grid.Columns[i + 2].Width = 100;
            e.Layout.Grid.Columns[i + 3].Width = 65;

            if (gridCaption != incomesGridCaption && gridCaption != balanceGridCaption)
            {
                e.Layout.Grid.Columns[0].Width = 320;
            }
            else
            {
                e.Layout.Grid.Columns[0].Width = 245;
            }
            */
            DundasMap.Width = (int)(CustomReportConst.minScreenWidth * 0.17 + 260);
            DundasMap.Height = (int)(DundasMap.Width.Value * mapSizeProportion);
        }

        #endregion
    }

    public class ReportSection : ISection
    {
        private readonly bool withFlowColumns;
        private readonly ISection section;
        private IFlow flow;
        private ITableCell titleCell;

        public ReportSection(Report report, bool withFlowColumns)
        {
            this.withFlowColumns = withFlowColumns;
            section = report.AddSection();
            ITable table = section.AddTable();
            ITableRow row = table.AddRow();
            titleCell = row.AddCell();
            if (this.withFlowColumns)
            {
                flow = section.AddFlow();
                IFlowColumn col = flow.AddColumn();
                col.Width = new FixedWidth(415);
                col = flow.AddColumn();
                col.Width = new FixedWidth(525);
            }
        }

        public void AddFlowColumnBreak()
        {
            if (flow != null)
                flow.AddColumnBreak();
        }

        public IBand AddBand()
        {
            if (flow != null)
                return flow.AddBand();
            return section.AddBand();
        }

        #region ISection members
        public ISectionHeader AddHeader()
        {
            throw new NotImplementedException();
        }

        public ISectionFooter AddFooter()
        {
            throw new NotImplementedException();
        }

        public IStationery AddStationery()
        {
            throw new NotImplementedException();
        }

        public IDecoration AddDecoration()
        {
            throw new NotImplementedException();
        }

        public ISectionPage AddPage()
        {
            throw new NotImplementedException();
        }

        public ISectionPage AddPage(PageSize size)
        {
            throw new NotImplementedException();
        }

        public ISectionPage AddPage(float width, float height)
        {
            throw new NotImplementedException();
        }

        public ISegment AddSegment()
        {
            throw new NotImplementedException();
        }

        public IQuickText AddQuickText(string text)
        {
            throw new NotImplementedException();
        }

        public IQuickImage AddQuickImage(Infragistics.Documents.Reports.Graphics.Image image)
        {
            throw new NotImplementedException();
        }

        public IQuickList AddQuickList()
        {
            throw new NotImplementedException();
        }

        public IQuickTable AddQuickTable()
        {
            throw new NotImplementedException();
        }

        public IText AddText()
        {
            return this.titleCell.AddText();
        }

        public IImage AddImage(Infragistics.Documents.Reports.Graphics.Image image)
        {
            if (flow != null)
                return flow.AddImage(image);
            return this.section.AddImage(image);
        }

        public IMetafile AddMetafile(Infragistics.Documents.Reports.Graphics.Metafile metafile)
        {
            throw new NotImplementedException();
        }

        public IRule AddRule()
        {
            throw new NotImplementedException();
        }

        public IGap AddGap()
        {
            throw new NotImplementedException();
        }

        public IGroup AddGroup()
        {
            throw new NotImplementedException();
        }

        public IChain AddChain()
        {
            throw new NotImplementedException();
        }

        public ITable AddTable()
        {
            if (flow != null)
                return flow.AddTable();
            return this.section.AddTable();
        }

        public IGrid AddGrid()
        {
            throw new NotImplementedException();
        }

        public IFlow AddFlow()
        {
            throw new NotImplementedException();
        }

        public Infragistics.Documents.Reports.Report.List.IList AddList()
        {
            throw new NotImplementedException();
        }

        public ITree AddTree()
        {
            throw new NotImplementedException();
        }

        public ISite AddSite()
        {
            throw new NotImplementedException();
        }

        public ICanvas AddCanvas()
        {
            throw new NotImplementedException();
        }

        public IRotator AddRotator()
        {
            throw new NotImplementedException();
        }

        public IContainer AddContainer(string name)
        {
            throw new NotImplementedException();
        }

        public ICondition AddCondition(IContainer container, bool fit)
        {
            throw new NotImplementedException();
        }

        public IStretcher AddStretcher()
        {
            throw new NotImplementedException();
        }

        public void AddPageBreak()
        {
            throw new NotImplementedException();
        }

        public ITOC AddTOC()
        {
            throw new NotImplementedException();
        }

        public IIndex AddIndex()
        {
            throw new NotImplementedException();
        }

        public bool Flip
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public PageSize PageSize
        {
            get { throw new NotImplementedException(); }
            set { this.section.PageSize = new PageSize(960, 1350); }
        }

        public PageOrientation PageOrientation
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public Infragistics.Documents.Reports.Report.ContentAlignment PageAlignment
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public Borders PageBorders
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public Infragistics.Documents.Reports.Report.Margins PageMargins
        {
            get { return this.section.PageMargins; }
            set { throw new NotImplementedException(); }
        }

        public Paddings PagePaddings
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public Background PageBackground
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public Infragistics.Documents.Reports.Report.Section.PageNumbering PageNumbering
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public SectionLineNumbering LineNumbering
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public Report Parent
        {
            get { return this.section.Parent; }
        }
        
        public IEnumerable Content
        {
            get { throw new NotImplementedException(); }
        }
		        
        #endregion
    }
}
