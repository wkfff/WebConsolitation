using System;
using System.Collections;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;

using Dundas.Maps.WebControl;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebGrid;

using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.iPad
{
    public partial class STAT_0001_0005_v : CustomReportPage
    {
        //private DataTable dt;
        private DataTable dtChart;
        private DateTime debtsCurrDateTime;
        private DateTime debtsLastDateTime;

        private DateTime currDateTime;
        private DateTime lastDateTime;
        private DataTable dtDebtsDateNso;

        // Текущая дата
        private CustomParam periodCurrentDate;
        // Текущая дата для задолженности
        private CustomParam debtsPeriodCurrentDate;
        // На неделю назад для задолженности
        private CustomParam debtsPeriodLastWeekDate;
        // На неделю назад
        private CustomParam periodLastDate;

        // Текущая дата для уровня безработицы по РФ
        private CustomParam redundantLevelRFDate;

        //private DateTime redundantLevelRFDateTime;

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            periodCurrentDate = UserParams.CustomParam("period_current_date");
            debtsPeriodCurrentDate = UserParams.CustomParam("period_current_date_debts");
            debtsPeriodLastWeekDate = UserParams.CustomParam("period_last_week_date_debts");
            periodLastDate = UserParams.CustomParam("period_last_date");
            redundantLevelRFDate = UserParams.CustomParam("redundant_level_RF_date");

            string query = DataProvider.GetQueryText("STAT_0001_0005_debts_mo_date");
            dtDebtsDateNso = new DataTable();
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Дата", dtDebtsDateNso);

            if (dtDebtsDateNso.Rows.Count > 1)
            {
                if (dtDebtsDateNso.Rows[0][1] != DBNull.Value && dtDebtsDateNso.Rows[0][1].ToString() != String.Empty)
                {
                    debtsPeriodLastWeekDate.Value = dtDebtsDateNso.Rows[0][1].ToString();
                    debtsLastDateTime = CRHelper.DateByPeriodMemberUName(dtDebtsDateNso.Rows[0][1].ToString(), 3);
                }
                if (dtDebtsDateNso.Rows[1][1] != DBNull.Value && dtDebtsDateNso.Rows[1][1].ToString() != String.Empty)
                {
                    debtsPeriodCurrentDate.Value = dtDebtsDateNso.Rows[1][1].ToString();
                    debtsCurrDateTime = CRHelper.DateByPeriodMemberUName(dtDebtsDateNso.Rows[1][1].ToString(), 3);
                }
            }

            debtsPeriodCurrentDate.Value = dtDebtsDateNso.Rows[1][1].ToString();
            debtsPeriodLastWeekDate.Value = dtDebtsDateNso.Rows[0][1].ToString();

            query = DataProvider.GetQueryText("STAT_0001_0005_part1_date");
            DataTable dtDateNso = new DataTable();
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Показатель", dtDateNso);

            currDateTime = CRHelper.DateByPeriodMemberUName(dtDateNso.Rows[1][1].ToString(), 3);
            lastDateTime = CRHelper.DateByPeriodMemberUName(dtDateNso.Rows[0][1].ToString(), 3);

            periodCurrentDate.Value = CRHelper.PeriodMemberUName("[Период__Период].[Период__Период].[Данные всех периодов]", currDateTime, 5);
            periodLastDate.Value = CRHelper.PeriodMemberUName("[Период__Период].[Период__Период].[Данные всех периодов]", lastDateTime, 5);

            UltraChart4.Width = 740;
            UltraChart4.Height = 240;

            #region Настройка диаграммы 4

            UltraChart4.ChartType = ChartType.AreaChart;
            UltraChart4.Border.Thickness = 0;

            UltraChart4.Tooltips.FormatString = "<ITEM_LABEL>";
            UltraChart4.Axis.X.Labels.Layout.Behavior = AxisLabelLayoutBehaviors.None;
            UltraChart4.Axis.X.Extent = 50;
            UltraChart4.Axis.X.Labels.Font = new Font("Verdana", 8);
            UltraChart4.Axis.X.Labels.Visible = true;
            //  UltraChart4.Axis.X.Labels.FontColor = Color.Black;
            UltraChart4.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N0>";
            UltraChart4.Axis.Y.Labels.Font = new Font("Verdana", 8);
            //  UltraChart4.Axis.Y.Labels.FontColor = Color.Black;
            UltraChart4.Axis.Y.Extent = 40;

            UltraChart4.TitleLeft.Visible = true;
            UltraChart4.TitleLeft.Text = "тыс.руб.";
            UltraChart4.TitleLeft.HorizontalAlign = StringAlignment.Center;
            UltraChart4.TitleLeft.Orientation = TextOrientation.VerticalLeftFacing;
            UltraChart4.TitleLeft.Extent = 40;
            UltraChart4.TitleLeft.Margins.Top = 0;
            UltraChart4.TitleLeft.Font = new Font("Verdana", 10);
            UltraChart4.TitleLeft.FontColor = Color.White;

            UltraChart4.ColorModel.ModelStyle = ColorModels.CustomLinear;
            UltraChart4.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart_FillSceneGraph);

            UltraChart4.Data.EmptyStyle.Text = " ";
            UltraChart4.EmptyChartText = " ";

            UltraChart4.AreaChart.NullHandling = NullHandling.Zero;

            LineAppearance lineAppearance = new LineAppearance();
            lineAppearance.IconAppearance.Icon = SymbolIcon.Circle;
            lineAppearance.IconAppearance.IconSize = SymbolIconSize.Medium;
            lineAppearance.Thickness = 3;
            UltraChart4.AreaChart.LineAppearances.Add(lineAppearance);

            UltraChart4.Legend.Visible = true;
            UltraChart4.Legend.Location = LegendLocation.Top;
            //  UltraChart4.Legend.Margins.Right = IsSmallResolution ? 5 : Convert.ToInt32(UltraChart4.Width.Value) / 2;
            UltraChart4.Legend.SpanPercentage = 14;
            UltraChart4.Legend.Font = new Font("Verdana", 10);

            UltraChart4.InvalidDataReceived +=
                new Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventHandler(
                    CRHelper.UltraChartInvalidDataReceived);
            UltraChart4.FillSceneGraph +=
                new Infragistics.UltraChart.Shared.Events.FillSceneGraphEventHandler(UltraChart4_FillSceneGraph);

            #endregion

            SetMapSettings(MapControl1, "Уровень\nбезработицы", "#FROMVALUE{N2}% - #TOVALUE{N2}%");
            Map1Caption.Text = String.Format("Уровень безработицы по муниципальным образованиям на {0:dd.MM.yyyy}", currDateTime);
            SetMapSettings(MapControl2, "Уровень напряженности\nна рынке труда", "#FROMVALUE{N2} - #TOVALUE{N2}");
            Map2Caption.Text = String.Format("Уровень напряженности на рынке труда на {0:dd.MM.yyyy}, единиц", currDateTime);
            SetMapSettings(MapControl3, "Суммарный объем\nпросроченной\nзадолженности\nповыпалте\nзаработной платы", "#FROMVALUE{N0} - #TOVALUE{N0}");
            Map3Caption.Text = String.Format("Суммарный объем просроченной задолженности по выплате заработной платы на {0:dd.MM.yyyy}, тыс. руб.", debtsCurrDateTime);

            FillMapData(MapControl1, "STAT_0001_0005_tagCloud_data", 1, "{0}\n{1:P2}", 100);
            FillMapData(MapControl2, "STAT_0001_0005_grid_v", 11, "{0}\n{1:N2}", 1);
            FillMapData(MapControl3, "STAT_0001_0005_grid_v", 16, "{0}\n{1:N0}", 1);

            UltraChart4.DataBind();

            UltraWebGrid.DataBinding += new EventHandler(UltraWebGrid_DataBinding);
            UltraWebGrid.InitializeLayout += new InitializeLayoutEventHandler(UltraWebGrid_InitializeLayout);
            UltraWebGrid.InitializeRow += new InitializeRowEventHandler(UltraWebGrid_InitializeRow);
            UltraWebGrid.DataBind();

            Text();
        }

        #region Карты

        public void SetMapSettings(MapControl dundasMap, string legendTitle, string format)
        {
            mapCalloutOffsetY = 0.70;

            dundasMap.Width = 800;
            dundasMap.Height = 500;
            dundasMap.Shapes.Clear();

            string mapFolderName = "Новособл";

            dundasMap.ShapeFields.Add("Name");
            dundasMap.ShapeFields["Name"].Type = typeof(string);
            dundasMap.ShapeFields["Name"].UniqueIdentifier = true;
            dundasMap.ShapeFields.Add("Complete");
            dundasMap.ShapeFields["Complete"].Type = typeof(double);
            dundasMap.ShapeFields["Complete"].UniqueIdentifier = false;

            AddMapLayer(dundasMap, mapFolderName, "Территор", CRHelper.MapShapeType.Areas);
            AddMapLayer(dundasMap, mapFolderName, "Города", CRHelper.MapShapeType.Towns);
            AddMapLayer(dundasMap, mapFolderName, "Выноски", CRHelper.MapShapeType.CalloutTowns);

            dundasMap.Meridians.Visible = false;
            dundasMap.Parallels.Visible = false;
            dundasMap.ZoomPanel.Visible = false;
            dundasMap.ZoomPanel.Dock = PanelDockStyle.Right;
            dundasMap.NavigationPanel.Visible = false;
            dundasMap.NavigationPanel.Dock = PanelDockStyle.Right;
            dundasMap.Viewport.EnablePanning = true;
            dundasMap.Viewport.OptimizeForPanning = false;
            dundasMap.Viewport.BackColor = Color.Black;

            // добавляем легенду
            Legend legend = new Legend("CompleteLegend");
            legend.Visible = true;
            legend.Dock = PanelDockStyle.Right;
            legend.BackColor = Color.FromArgb(75, 255, 255, 255);
            legend.BackSecondaryColor = Color.Black;
            legend.BackGradientType = GradientType.TopBottom;
            legend.BackHatchStyle = MapHatchStyle.None;
            legend.BorderColor = Color.Black;
            legend.BorderWidth = 1;
            legend.BorderStyle = MapDashStyle.Solid;
            legend.BackShadowOffset = 4;
            legend.TextColor = Color.FromArgb(192, 192, 192);
            legend.Font = new Font("MS Sans Serif", 7, FontStyle.Regular);
            legend.AutoFitText = true;
            legend.TitleColor = Color.White;
            legend.Title = legendTitle;
            legend.AutoFitMinFontSize = 7;
            //legend.
            dundasMap.Legends.Clear();
            dundasMap.Legends.Add(legend);

            // добавляем правила раскраски
            dundasMap.ShapeRules.Clear();
            ShapeRule rule = new ShapeRule();
            rule.Name = "CompleteRule";
            rule.Category = String.Empty;
            rule.ShapeField = "Complete";
            rule.DataGrouping = DataGrouping.EqualDistribution;
            rule.ColorCount = 5;
            rule.ColoringMode = ColoringMode.ColorRange;
            rule.FromColor = Color.Green;
            rule.MiddleColor = Color.Orange;
            rule.ToColor = Color.Red;
            rule.BorderColor = Color.FromArgb(50, Color.Black);
            rule.GradientType = GradientType.None;
            rule.HatchStyle = MapHatchStyle.None;
            rule.ShowInColorSwatch = false;
            rule.ShowInLegend = "CompleteLegend";
            rule.LegendText = format;
            dundasMap.ShapeRules.Add(rule);
        }

        // сдвиг по вертикали текста городов-выносок карты
        private double mapCalloutOffsetY;

        /// <summary>
        /// Является ли форма городом-выноской
        /// </summary>
        /// <param name="shape">форма</param>
        /// <returns>true, если является</returns>
        public static bool IsCalloutTownShape(Shape shape)
        {
            return shape.Layer == CRHelper.MapShapeType.CalloutTowns.ToString();
        }

        /// <summary>
        /// Получение имени формы (с выделением имени из города-выноски)
        /// </summary>
        /// <param name="shape">форма</param>
        /// <returns>имя формы</returns>
        public static string GetShapeName(Shape shape)
        {
            string shapeName = shape.Name;
            if (IsCalloutTownShape(shape) && shape.Name.Split('_').Length > 1)
            {
                shapeName = shape.Name.Split('_')[0];
            }

            return shapeName;
        }

        private void AddMapLayer(MapControl map, string mapFolder, string layerFileName, CRHelper.MapShapeType shapeType)
        {
            string layerName = Server.MapPath(String.Format("../../../maps/Субъекты/{0}/{1}.shp", mapFolder, layerFileName));
            int oldShapesCount = map.Shapes.Count;

            map.LoadFromShapeFile(layerName, "NAME", true);
            map.Layers.Add(shapeType.ToString());

            for (int i = oldShapesCount; i < map.Shapes.Count; i++)
            {
                Shape shape = map.Shapes[i];
                shape.Layer = shapeType.ToString();
            }
        }

        /// <summary>
        /// Поиск формы карты
        /// </summary>
        /// <param name="map">карта</param>
        /// <param name="patternValue">искомое имя формы</param>
        /// <returns>найденные формы</returns>
        public static ArrayList FindMapShape(MapControl map, string patternValue)
        {
            patternValue = patternValue.Replace(" муниципальный", String.Empty).Replace("Город ", "г. ");
            ArrayList shapeList = new ArrayList();
            foreach (Shape shape in map.Shapes)
            {
                if (GetShapeName(shape) == patternValue)
                {
                    shapeList.Add(shape);
                }
            }

            return shapeList;
        }

        public void FillMapData(MapControl dundasMap, string queryName, int dataColumnIndex, string format, int ratio)
        {
            DataTable dtMap = new DataTable();

            string query = DataProvider.GetQueryText(queryName);
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Наименование показателя", dtMap);

            foreach (DataRow row in dtMap.Rows)
            {
                // заполняем карту данными
                if (row[0] != DBNull.Value && row[0].ToString() != String.Empty &&
                    row[dataColumnIndex] != DBNull.Value && row[dataColumnIndex].ToString() != String.Empty)
                {
                    string subject = row[0].ToString();
                    double value = Convert.ToDouble(row[dataColumnIndex]) / ratio;
                    ArrayList shapeList = FindMapShape(dundasMap, subject);
                    foreach (Shape shape in shapeList)
                    {
                        string shapeName = GetShapeName(shape);
                        shape["Name"] = subject;
                        shape["Complete"] = Convert.ToDouble(row[dataColumnIndex]);

                        if (IsCalloutTownShape(shape))
                        {
                            shape.Text = String.Format(format, shapeName, value);
                            shape.TextVisibility = TextVisibility.Shown;
                            shape.TextAlignment = ContentAlignment.BottomCenter;
                            shape.TextColor = Color.White;
                            shape.Font = new Font(shape.Font.Name, 10);
                            shape.CentralPointOffset.Y = mapCalloutOffsetY;
                        }
                        else
                        {
                            shape.Text = String.Format(format, shapeName.Replace(" ", "\n"), value);
                            shape.Font = new Font(shape.Font.Name, 10);
                            shape.TextColor = Color.White;
                        }
                    }
                }
            }
        }
        
        #endregion

        #region Грид

        private void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            UltraGridRow row = e.Row;
            for (int i = 1; i < row.Cells.Count - 1; ++i)
                row.Cells[i].Title = String.Format("На {0:dd.MM.yyyy}", currDateTime);
            row.Cells[e.Row.Cells.Count - 1].Title = String.Format("На {0:dd.MM.yyyy}", debtsCurrDateTime);

            int[] columnsWithArrow = { 1, 3, 4 };
            foreach (int cIndex in columnsWithArrow)
            {
                UltraGridCell cell = row.Cells[cIndex];
                if (cell.Value == null)
                    continue;
                string[] separator = { "<br/>" };
                object value = cell.Value.ToString().Split(separator, StringSplitOptions.None)[1].Replace("%", String.Empty);
                if (MathHelper.AboveZero(value))
                {
                    cell.Style.BackgroundImage = "~/images/arrowRedUpBB.png";
                    cell.Style.CustomRules = "background-repeat: no-repeat; background-position: 5% 50%;";
                }
                else if (MathHelper.SubZero(value))
                {
                    cell.Style.BackgroundImage = "~/images/arrowGreenDownBB.png";
                    cell.Style.CustomRules = "background-repeat: no-repeat; background-position: 5% 50%;";
                }
            }
        }

        private GridHeaderLayout headerLayout;
        
        void UltraWebGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.Bands[0].Grid.Width = Unit.Empty;
            e.Layout.Bands[0].Grid.Height = Unit.Empty;

            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[0].Width = 174;
            e.Layout.Bands[0].Columns[1].Width = 90;
            e.Layout.Bands[0].Columns[2].Width = 55;
            e.Layout.Bands[0].Columns[3].Width = 130;
            e.Layout.Bands[0].Columns[4].Width = 100;
            e.Layout.Bands[0].Columns[5].Width = 55;
            e.Layout.Bands[0].Columns[6].Width = 160;

            headerLayout = new GridHeaderLayout(e.Layout.Bands[0].Grid);

            headerLayout.AddCell("");
            GridHeaderCell cell = headerLayout.AddCell(String.Format("Уровень безработицы на {0:dd.MM.yyyy}, процент", currDateTime));
            cell.AddCell("");
            cell.AddCell("Ранг");

            headerLayout.AddCell(String.Format("Зарегистрир. безработных на {0:dd.MM.yyyy}, чел.", currDateTime));
            cell = headerLayout.AddCell(String.Format("Уровень напряженности на рынке труда на {0:dd.MM.yyyy}", currDateTime));
            cell.AddCell("");
            cell.AddCell("Ранг");
            headerLayout.AddCell(String.Format("Задолженность по выплате заработной платы на {0:dd.MM.yyyy}", debtsCurrDateTime));

            headerLayout.ApplyHeaderInfo();

            foreach (UltraGridColumn c in e.Layout.Grid.Columns)
                c.Header.Style.VerticalAlign = VerticalAlign.Top;
        }

        void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            DataTable dtGrid = new DataTable();
            string query = DataProvider.GetQueryText("STAT_0001_0005_grid_v");
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Дата", dtGrid);

            DataTable dtSource = new DataTable();
            for (int i = 0; i < 11; i++)
            {
                dtSource.Columns.Add(new DataColumn(i.ToString(), typeof(string)));
            }

            for (int i = 0; i < dtGrid.Rows.Count; i++)
            {
                dtGrid.Rows[i][1] = MathHelper.Div(dtGrid.Rows[i][1], 100);
                
                DataRow row = dtSource.NewRow();

                row[0] = dtGrid.Rows[i][0].ToString().Replace("муниципальный район", "р-н").Replace("Город ", "г.");

                AddColumnGroup(dtGrid, i, row, 1, 1);
                AddColumnGroup(dtGrid, i, row, 6, 3);
                AddColumnGroup(dtGrid, i, row, 11, 5);

                if ((MathHelper.AboveZero(dtGrid.Rows[i][16]) || MathHelper.SubZero(dtGrid.Rows[i][16]))
                    && (MathHelper.AboveZero(dtGrid.Rows[i][21]) || MathHelper.SubZero(dtGrid.Rows[i][21])))
                {
                    row[7] = String.Format("{0:N0} тыс.руб.<br/>{1:N0} человек", dtGrid.Rows[i][16], dtGrid.Rows[i][21]);
                }
                else if ((MathHelper.AboveZero(dtGrid.Rows[i][16]) || MathHelper.SubZero(dtGrid.Rows[i][16])))
                {
                    row[7] = String.Format("{0:N0} тыс.руб.", dtGrid.Rows[i][16]);
                }
                else
                {
                    row[7] = "Отсутствует";
                }

                dtSource.Rows.Add(row);
            }

            dtSource.Columns.RemoveAt(10);
            dtSource.Columns.RemoveAt(9);
            dtSource.Columns.RemoveAt(8);
            dtSource.Columns.RemoveAt(4);

            UltraWebGrid.DataSource = dtSource;
        }

        private void AddColumnGroup(DataTable dtGrid, int i, DataRow row, int group, int column)
        {
            if (dtGrid.Rows[i][group] != DBNull.Value)
            {
                double value = Convert.ToDouble(dtGrid.Rows[i][group].ToString());
                double absoluteGrownValue = Convert.ToDouble(dtGrid.Rows[i][group + 1].ToString());
                string absoluteGrown;
                if (absoluteGrownValue > 0)
                {
                    switch (column)
                    {
                        case 1:
                            absoluteGrown = String.Format("+{0:P2}", absoluteGrownValue);
                            break;
                        case 3:
                            absoluteGrown = String.Format("+{0:N0}", absoluteGrownValue);
                            break;
                        default:
                            absoluteGrown =String.Format("+{0:N2}", absoluteGrownValue);
                            break;
                    }
                    
                }
                else if(absoluteGrownValue < 0)
                {
                    switch (column)
                    {
                        case 1:
                            absoluteGrown = String.Format("-{0:P2}", Math.Abs(absoluteGrownValue));
                            break;
                        case 3:
                            absoluteGrown = String.Format("-{0:N0}", Math.Abs(absoluteGrownValue));
                            break;
                        default:
                            absoluteGrown = String.Format("-{0:N2}", Math.Abs(absoluteGrownValue));
                            break;
                    }
                }
                else
                {
                    switch (column)
                    {
                        case 1:
                            absoluteGrown = String.Format("{0:P2}", Math.Abs(absoluteGrownValue));
                            break;
                        case 3:
                            absoluteGrown = String.Format("{0:N0}", Math.Abs(absoluteGrownValue));
                            break;
                        default:
                            absoluteGrown = String.Format("{0:N2}", Math.Abs(absoluteGrownValue));
                            break;
                    }
                }


                double grownValue = Convert.ToDouble(dtGrid.Rows[i][group + 2].ToString());

                string grown = String.Empty;
                if (grownValue == 0)
                {
                    grown = String.Format("{0:P2}", grownValue);
                }
                else
                {
                    grown = grownValue > 0
                                   ? String.Format("+{0:P2}", grownValue)
                                   : String.Format("-{0:P2}", Math.Abs(grownValue));
                }
                

                string img = String.Empty;
                if (absoluteGrownValue != 0)
                {
                    img = absoluteGrownValue > 0
                              ? "<img src='../../../images/arrowRedUpBB.png'>"
                              : "<img src='../../../images/arrowGreenDownBB.png'>";
                }

                switch (column)
                {
                    case 1:
                        row[column] = String.Format("{0:P2}<br/>{2}<br/>{3}", value, img, grown, absoluteGrown);
                        break;
                    case 3:
                        row[column] = String.Format("{0:N0}<br/>{2}<br/>{3}", value, img, grown, absoluteGrown);
                        break;
                    default:
                        row[column] = String.Format("{0:N2}<br/>{2}<br/>{3}", value, img, grown, absoluteGrown);
                        break;
                }

                

                string rankImg = String.Empty;
                if (dtGrid.Rows[i][group + 3].ToString() == "1")
                {
                    rankImg = "<img src='../../../images/StarYellow.png'>";
                }
                else if (dtGrid.Rows[i][group + 3].ToString() == dtGrid.Rows[i][group + 4].ToString())
                {
                    rankImg = "<img src='../../../images/StarGray.png'>";
                }

                row[column + 1] = String.Format("{0}&nbsp;{1}", rankImg, dtGrid.Rows[i][group + 3]);
            }
        }

        #endregion

        #region Текст

        private void Text()
        {
            string query = DataProvider.GetQueryText("STAT_0001_0005_debts_Nso");
            DataTable dtDebtsNso = new DataTable();
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Дата", dtDebtsNso);

            double totalDebts = GetDoubleDTValue(dtDebtsNso, "Cумма задолженности");
            double totalLastWeekDebts = GetDoubleDTValue(dtDebtsNso, "Cумма задолженности предыдущий месяц");
            string dateTimeDebtsStr = String.Format("{0:dd.MM.yyyy}", debtsCurrDateTime);
            string dateLastTimeDebtsStr = String.Format("{0:dd.MM.yyyy}", debtsLastDateTime);
            double slavesCount = GetDoubleDTValue(dtDebtsNso, "Количество граждан, имеющих задолженность");
            double debtsPercent = GetDoubleDTValue(dtDebtsNso, "Прирост задолженности");
            string debtsPercentArrow = debtsPercent == 0
                                           ? "не изменилась"
                                           : debtsPercent > 0
                                           ? String.Format("увеличилась&nbsp;<img src='../../../images/arrowRedUpBB.png' width='13px' height='16px'>&nbsp;на&nbsp;<span class='DigitsValue'>{0:N3}</span>&nbsp;тыс.руб", Math.Abs(debtsPercent))
                                           : String.Format("уменьшилась&nbsp;<img src='../../../images/arrowGreenDownBB.png' width='13px' height='16px'>&nbsp;на&nbsp;<span class='DigitsValue'>{0:N3}</span>&nbsp;тыс.руб", Math.Abs(debtsPercent));

            string str10;
            if (totalLastWeekDebts == 0 && totalDebts == 0)
            {
                str10 = String.Format(
                    "На&nbsp;<span class='DigitsValue'>{0}</span>&nbsp;отсутствует задолженность по выплате заработной платы<br/>",
                    dateTimeDebtsStr);
            }
            else if (totalDebts == 0)
            {
                str10 = String.Format(@"На&nbsp;<span class='DigitsValue'>{0}</span>&nbsp;отсутствует задолженность по выплате заработной платы. 
Задолженность в сумме&nbsp;<span class='DigitsValue'>{1:N3}</span>&nbsp;тыс.руб. была погашена за период с&nbsp;<span class='DigitsValue'>{2}</span>&nbsp;по&nbsp;<span class='DigitsValue'>{0}</span>.<br/>",
dateTimeDebtsStr, totalLastWeekDebts, dateLastTimeDebtsStr);
            }
            else
            {
                str10 = String.Format(@"На&nbsp;<span class='DigitsValue'>{0}</span>&nbsp;задолженность по выплате заработной платы составляет 
&nbsp;<span class='DigitsValue'>{1:N3}</span>&nbsp;тыс.руб. (<span class='DigitsValue'>{2:N0}</span>&nbsp;чел.).<br/>За период с&nbsp;<span class='DigitsValue'>{4}</span>&nbsp;по&nbsp;<span class='DigitsValue'>{0}</span>&nbsp;задолженность {3}",
dateTimeDebtsStr, totalDebts, slavesCount, debtsPercentArrow, dateLastTimeDebtsStr);
            }
            DataRow row = dtDebtsNso.Rows[0];
            if (row["Cумма задолженности начало года"] != DBNull.Value && String.Format("{0:dd.MM}", debtsCurrDateTime) != "01.01")
            {
                if (MathHelper.AboveZero(row["Прирост задолженности начало года"]))
                {
                    str10 += String.Format(
                        "<br/>За период с&nbsp;<span class='DigitsValue'>01.01</span>&nbsp;по&nbsp;<span class='DigitsValue'>{0:dd.MM}</span>&nbsp;" +
                        "задолженность увеличилась&nbsp;<img src='../../../images/arrowRedUpBB.png' width='13px' height='16px'>&nbsp;" +
                        "на&nbsp;<span class='DigitsValue'>{1:N3}</span>&nbsp;тыс.руб.",
                        debtsCurrDateTime, row["Прирост задолженности начало года"]);
                }
                else if (MathHelper.SubZero(row["Прирост задолженности начало года"]))
                {
                    str10 += String.Format(
                        "<br/>За период с&nbsp;<span class='DigitsValue'>01.01</span>&nbsp;по&nbsp;<span class='DigitsValue'>{0:dd.MM}</span>&nbsp;" +
                        "задолженность уменьшилась&nbsp;<img src='../../../images/arrowGreenDownBB.png' width='13px' height='16px'>&nbsp;" +
                        "на&nbsp;<span class='DigitsValue'>{1:N3}</span>&nbsp;тыс.руб.",
                        debtsCurrDateTime, -Convert.ToDouble(row["Прирост задолженности начало года"]));
                }
                else
                {
                    str10 += "<br/>С начала текущего года объём задолженности не изменился.<br/>";
                }
            }
            if (row["Cумма задолженности АППГ"] != DBNull.Value)
            {
                if (MathHelper.AboveZero(row["Прирост задолженности АППГ"]))
                {
                    str10 += String.Format(
                        "<br/>В сравнении с аналогичной датой прошлого года " +
                        "задолженность увеличилась&nbsp;<img src='../../../images/arrowRedUpBB.png' width='13px' height='16px'>&nbsp;" +
                        "на&nbsp;<span class='DigitsValue'>{0:N3}</span>&nbsp;тыс.руб.<br/>",
                        row["Прирост задолженности АППГ"]);
                }
                else if (MathHelper.SubZero(row["Прирост задолженности АППГ"]))
                {
                    str10 += String.Format(
                        "<br/>В сравнении с аналогичной датой прошлого года " +
                        "задолженность уменьшилась&nbsp;<img src='../../../images/arrowGreenDownBB.png' width='13px' height='16px'>&nbsp;" +
                        "на&nbsp;<span class='DigitsValue'>{0:N3}</span>&nbsp;тыс.руб.<br/>",
                        -Convert.ToDouble(row["Прирост задолженности АППГ"]));
                }
                else
                {
                    str10 += "Объём задолженности на соответствующую дату прошлого года был таким же.<br/>";
                }
            }

            string debtsMoList = String.Empty;

            if (dtDebtsDateNso.Rows.Count > 0)
            {

                query = DataProvider.GetQueryText("STAT_0001_0005_debts_mo");
                DataTable dtDebtsMo = new DataTable();
                DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Показатель", dtDebtsMo);
                if (dtDebtsMo.Rows.Count > 0)
                {
                    debtsMoList =
                        String.Format("Задолженность по выплате заработной платы присутствует в&nbsp;<span class='DigitsValue'>{0}</span>&nbsp;муниципальных образованиях", dtDebtsMo.Rows.Count);
                }
            }

            lbDebtDescription.Text = String.Format("{0} {1}", str10, debtsMoList);
        }
        
        #endregion

        #region Диаграмма

        private static Double GetDoubleDTValue(DataTable dt, string columnName)
        {
            return GetDoubleDTValue(dt, columnName, 0);
        }

        private static Double GetDoubleDTValue(DataTable dt, string columnName, double defaultValue)
        {
            if (dt.Rows[0][columnName] != DBNull.Value && dt.Rows[0][columnName].ToString() != String.Empty)
            {
                return Convert.ToDouble(dt.Rows[0][columnName].ToString());
            }
            return defaultValue;
        }

        protected void UltraChart4_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("STAT_0001_0005_chart4");
            dtChart = new DataTable();
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Дата", dtChart);

            if (dtChart.Rows.Count > 0)
            {
                dtChart.Columns.RemoveAt(0);

                foreach (DataRow row in dtChart.Rows)
                {
                    if (row[0] != DBNull.Value)
                    {
                        DateTime dateTime = CRHelper.PeriodDayFoDate(row[0].ToString());
                        row[0] = String.Format("{0:dd.MM.yy}", dateTime);
                    }
                }

                UltraChart4.Series.Clear();
                for (int i = 1; i < dtChart.Columns.Count; i++)
                {
                    NumericSeries series = CRHelper.GetNumericSeries(i, dtChart);
                    series.Label = dtChart.Columns[i].ColumnName;
                    UltraChart4.Series.Add(series);
                }
            }
        }

        void UltraChart4_FillSceneGraph(object sender, Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e)
        {
            for (int i = 0; i < e.SceneGraph.Count; i++)
            {
                Primitive primitive = e.SceneGraph[i];

                if (primitive is Polyline)
                {
                    Polyline polyline = (Polyline)primitive;
                    foreach (DataPoint point in polyline.points)
                    {
                        if (point.Series != null)
                        {
                            string unit = " тыс.руб.";
                            point.DataPoint.Label = String.Format("{2}\nна {3}\n {0:N2}{1}", ((NumericDataPoint)point.DataPoint).Value, unit, point.Series.Label, point.DataPoint.Label);

                        }
                    }
                }
            }
        }

        void UltraChart_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            string currentYear = String.Empty;

            for (int i = 0; i < e.SceneGraph.Count; i++)
            {
                Primitive primitive = e.SceneGraph[i];
                if (primitive is PointSet)
                {
                    PointSet pointSet = (PointSet)primitive;
                    foreach (DataPoint point in pointSet.points)
                    {
                        point.hitTestRadius = 15;
                    }
                }
            }
        }

        #endregion

    }
}

