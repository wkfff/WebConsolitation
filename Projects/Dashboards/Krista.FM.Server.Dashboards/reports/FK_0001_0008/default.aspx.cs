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
using Dundas.Maps.WebControl;
using System.Drawing;
using System.Collections.ObjectModel;
using System.Text;
using System.Collections.Generic;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.FK_0001_0008
{
    // Тип показателя
    public enum MeasuresTypesEnum
    {
        Money = 0,
        Population = 1,
        Percent = 2
    }
    // Область карты для выода
    public enum MapKindEnum
    {
        AllSubjects = 0,
        SingleRegion = 1,
        AllRegions = 2
    }
    // Показатель
    public enum MeasureKindEnum
    {
        CompletePercent = 0,
        AvgMenReceipts = 1,
        BoostPercent = 2
    }

    public partial class FK_0001_0008Form : CustomReportPage
    {
        public const string MeasureFieldName = "MeasureField";
        public const string MeasureMapLabelName = "MapLabelMeasures";
        public const string MapShapeName = "Name";
        public const string MapLegendName = "HighLightLegend";
        public const string MapRuleName = "HighLightRule";

        protected DataTable dt1 = new DataTable();

        protected int MeasureKindIndex;
        protected MapKindEnum MapKind;

        protected Dictionary<string, string> ConvertNamesArray = new Dictionary<string, string>();

        /// <summary>
        /// Возвращает данные по какой карте выводить(регион, все регионы, все субъекты)
        /// <summary>
        protected virtual MapKindEnum CheckMapIndex(int ListIndex)
        {
            if (ListIndex == 0) return MapKindEnum.AllSubjects;
            if (ListIndex > (RegionsNamingHelper.FoNames.Count)) return MapKindEnum.AllRegions;
            return MapKindEnum.SingleRegion;
        }

        /// <summary>
        /// Заполнение списка показателей
        /// <summary>
        protected virtual void FillMeasuresList()
        {
                Collection<string> MeasuresList = new Collection<string>();
                MeasuresList.Add("Исполнено %");
                MeasuresList.Add("Среднедушевые доходы");
                MeasuresList.Add("Темп роста доходов к аналогичному периоду предыдущего года");                
                ComboMeasure.FillValues(MeasuresList);
                ComboMeasure.SelectedIndex = 0;
        }

        /// <summary>
        /// Заполнение списка классификатора
        /// <summary>
        protected virtual void FillClassifierList()
        {
            ComboClassifier.FillKDNames(KDKindsDictionary);
            ComboClassifier.SelectedIndex = KDKindsDictionary.Count - 1;
        }

        /// <summary>
        /// Заполнение списка карты
        /// <summary>
        protected virtual void FillMapList()
        {
            Collection<string> MapList = new Collection<string>();
            MapList.Add("РФ по субъектам");
            for (int i = 0; i < RegionsNamingHelper.FoNames.Count; i++)
            {
                MapList.Add(RegionsNamingHelper.FoNames[i]);
            }
            MapList.Add("РФ по федеральным округам");
            ComboMap.FillValues(MapList);
            ComboMap.SelectedIndex = 1;
        }


        /// <summary>
        /// Настраиваем параметры
        /// <summary>
        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            map.Labels[MeasureMapLabelName].Text = string.Empty;

            LabelTitle.Text = Page.Title;

            // Словарик исключений по именам субъектов
            ConvertNamesArray.Add("Чеченская", "Чечня");
            ConvertNamesArray.Add("Карачаево-Черкесская", "Карачаево-Черкессия");
            ConvertNamesArray.Add("Кабардино-Балкарская", "Кабардино-Балкария");
            ConvertNamesArray.Add("Удмуртская", "Удмуртия");
            ConvertNamesArray.Add("Чувашская", "Чувашия");

            if (!Page.IsPostBack)
            {
                ComboYear.FillYearValues(2000, System.DateTime.Now.Year);
                ComboYear.SelectValue(System.DateTime.Now.Year.ToString());

                ComboMonth.FillMonthValues();

                FillMapList();
                FillMeasuresList();
                FillClassifierList();

                if (map.IsCallback)
                {
                    ComboYear.SelectedIndex = Convert.ToInt32(Session["ReportParamYear"]);
                    ComboMonth.SelectedIndex = Convert.ToInt32(Session["ReportParamMonth"]);
                    ComboMap.SelectedIndex = Convert.ToInt32(Session["ReportParamMap"]);
                    ComboClassifier.SelectedIndex = Convert.ToInt32(Session["ReportParamKD"]);
                    ComboMeasure.SelectedIndex = Convert.ToInt32(Session["ReportParamMeasure"]);
                }
                ChangeParams();
            }
        }

        /// <summary>
        /// Настраиваем карту
        /// <summary>
        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);           
           
            #region Настройка карты
            map.Meridians.Visible = false;
            map.Parallels.Visible = false;
            //map.ZoomPanel.Visible = false;
            //map.NavigationPanel.Visible = false;

            // добавляем легенду
            Legend legend = new Legend(MapLegendName);
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
            map.Legends.Add(legend);

            // добавляем поля
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
            rule.FromColor = Color.Red;
            rule.MiddleColor = Color.Yellow;
            rule.ToColor = Color.Green;
            rule.BorderColor = Color.FromArgb(50, Color.Black);
            rule.GradientType = GradientType.None;
            rule.HatchStyle = MapHatchStyle.None;
            rule.ShowInColorSwatch = false;
            rule.ShowInLegend = MapLegendName;
            map.ShapeRules.Add(rule);

            #endregion

            // Растянем карту на весь экран
            try
            {
                double dirtyWidth = ((int)Session["width_size"] - 100);
                double dirtyHeight = ((int)Session["height_size"] - 350);

                map.Width = (int)(dirtyWidth);
                map.Height = (int)(dirtyHeight);
            }
            catch
            {
            }
        }
        
        /// <summary>
        /// Меняем название легенды в зависимости от выбранного показателя
        /// <summary>
        protected virtual void SetLegendTitle()
        {
            Legend legend;

            legend = (Legend)map.Legends.GetByName(MapLegendName);
            switch ((MeasureKindEnum)MeasureKindIndex)
            {
                case MeasureKindEnum.CompletePercent:
                    legend.Title = "Исполнено %";
                    break;
                case MeasureKindEnum.AvgMenReceipts:
                    legend.Title = "Среднедушевые доходы";
                    break;
                case MeasureKindEnum.BoostPercent:
                    legend.Title = "Темп роста доходов к аналогичному периоду предыдущего года";
                    break;
            }
        }

        /// <summary>
        /// Имя основного запроса данных
        /// <summary>
        protected virtual string GetMainQueryName()
        {
            return "FK_0001_0008_DataQuery01";
        }

        /// <summary>
        /// Заполнение параметра кода доходов
        /// <summary>
        protected virtual void FillClassifierParam()
        {
            UserParams.KDGroup.Value = KDKindsDictionary[ComboClassifier.SelectedValue];
        }

        /// <summary>
        /// Обновление данных отчета
        /// <summary>
        protected virtual void ChangeParams()
        {
            // Карта при кликах сбрасывает страницу - храним параметры в сессии чтобы потом восттанавливать
            Session["ReportParamYear"] = ComboYear.SelectedIndex;
            Session["ReportParamMonth"] = ComboMonth.SelectedIndex;
            Session["ReportParamMap"] = ComboMap.SelectedIndex;
            Session["ReportParamKD"] = ComboClassifier.SelectedIndex;
            Session["ReportParamMeasure"] = ComboMeasure.SelectedIndex;

            MapKind = CheckMapIndex(ComboMap.SelectedIndex);
            MeasureKindIndex = ComboMeasure.SelectedIndex;

            // заполняем карту формами
            map.Shapes.Clear();
            switch (MapKind)
            {
                case MapKindEnum.AllSubjects:
                    map.LoadFromShapeFile(Server.MapPath("../../maps/РФ по субъектам/РФ.shp"), "NAME", true);
                    break;
                case MapKindEnum.AllRegions:
                    map.LoadFromShapeFile(Server.MapPath(string.Format("../../maps/{0}/{0}.shp", "РФ")), "NAME", true);
                    break;
                case MapKindEnum.SingleRegion:
                    string regionStr = ComboMap.SelectedValue;
                    map.LoadFromShapeFile(Server.MapPath(string.Format("../../maps/{0}/{0}.shp", RegionsNamingHelper.ShortName(regionStr))), "NAME", true);
                    break;
            }

            // Настройка пользовательских параметров
            UserParams.PeriodMonth.Value = ComboMonth.SelectedValue;
            UserParams.PeriodYear.Value = ComboYear.SelectedValue;
            UserParams.PeriodHalfYear.Value = string.Format("Полугодие {0}", CRHelper.HalfYearNumByMonthNum(ComboMonth.SelectedIndex + 1));
            UserParams.PeriodQuater.Value = string.Format("Квартал {0}", CRHelper.QuarterNumByMonthNum(ComboMonth.SelectedIndex + 1));

            FillClassifierParam();

            switch (MapKind)
            {
                case MapKindEnum.AllSubjects:
                    UserParams.Region.Value = "[Территории].[Сопоставимый].[Субъект РФ].AllMembers";
                    break;
                case MapKindEnum.AllRegions:
                    UserParams.Region.Value = "[Территории].[Сопоставимый].[Федеральный округ].AllMembers";
                    break;
                case MapKindEnum.SingleRegion:
                    UserParams.Region.Value = string.Format("[Территории].[Сопоставимый].[Все территории].[Российская  Федерация].[{0}].Children", RegionsNamingHelper.FullName(ComboMap.SelectedValue));
                    break;
            }

            // Выкачиваем данные
            string query = DataProvider.GetQueryText(GetMainQueryName().ToString());
            dt1.TableName = "Субъекты РФ";
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, dt1.TableName, dt1);

            // заполняем карту данными            
            FillMapData();
        }

        /// <summary>
        /// Поиск фигуры на карте по части имени
        /// <summary>
        protected virtual Shape FindMapShape(MapControl map, string patternValue, bool searchFO)
        {
            string subject = string.Empty;
            string[] subjects = patternValue.Split(' ');
            
            if (subjects.Length > 1)
            {
                if (ConvertNamesArray.ContainsKey(subjects[0]))
                {
                    subject = ConvertNamesArray[subjects[0]];
                }
                else
                {
                    bool isRepublic = patternValue.Contains("Республика");
                    subject = (isRepublic) ? subjects[1] : subjects[0];
                };
            }
            
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

        /// <summary>
        /// В зависимости от типа значения показателя(процент, тыс.чел и тп) конвертирует в нужный вид
        /// <summary>
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

        /// <summary>
        /// Заполнение полей данных выбранной фигуры
        /// <summary>
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

            if (MeasureType == MeasuresTypesEnum.Percent) shape.ToolTip = shape.ToolTip + '%';

            return;
        }
        
        /// <summary>
        /// По типу измерителя заполняем нужные параметры фигуры
        /// <summary>
        protected virtual void FillShapeMeasuresData(Shape shape, DataRow row)
        {
            // Обрабатываем данные фигуры и формат их вывода
            switch ((MeasureKindEnum)MeasureKindIndex)
            {
                case MeasureKindEnum.CompletePercent:
                    SetShapeData(shape, row, 3, MeasuresTypesEnum.Percent);
                    break;
                case MeasureKindEnum.AvgMenReceipts:
                    SetShapeData(shape, row, 6, MeasuresTypesEnum.Money);
                    break;
                case MeasureKindEnum.BoostPercent:
                    SetShapeData(shape, row, 5, MeasuresTypesEnum.Percent);
                    break;
            }
        }

        /// <summary>
        /// Заполнение данными всех фигур карты
        /// <summary>
        protected virtual void FillMapData()
        {
            bool AllFO = MapKind == MapKindEnum.AllRegions;

            if (dt1 == null || map == null)  return;

            foreach (DataRow row in dt1.Rows)
            {
                // заполняем карту данными
                string subject = row[0].ToString();
                if (AllFO && RegionsNamingHelper.IsFO(subject) || !AllFO && RegionsNamingHelper.IsSubject(subject))
                {
                    Shape shape = FindMapShape(map, subject, AllFO);
                    if (shape != null)
                    {
                        // У фигуры имя есть точно
                        shape[MapShapeName] = subject;
                        shape.ToolTip = "#NAME";
                        shape.TextVisibility = 0;
                        FillShapeMeasuresData(shape, row);
                    }
                }
            }
            // Не забываем установить имя легенды в зависимости от выбранного показателя
            SetLegendTitle();
        }
        
        /// <summary>
        /// Получает значение клетки таблицы данных по имени строки и номеру столбца
        /// <summary>
        protected virtual string GetTableRowValue(DataTable dt, string RowName, int ColumnIndex, MeasuresTypesEnum MeasureType)
        {
            Object cell;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (dt.Rows[i][0].ToString() == RowName)
                {
                    cell = dt.Rows[i][ColumnIndex];
                    if (cell.ToString() != string.Empty)
                    {
                        // Форматируем текст подсказки
                        switch (MeasureType)
                        {
                            case MeasuresTypesEnum.Percent:
                                return string.Format("{0:N2}", 100 * Convert.ToDouble(cell.ToString())) + " %";
                            case MeasuresTypesEnum.Money:
                                return string.Format("{0:C2}", Convert.ToDouble(cell.ToString()));
                            case MeasuresTypesEnum.Population:
                                return string.Format("{0:N0}", 1000 * Convert.ToDouble(cell.ToString())) + " чел.";
                        }
                    }
                }
            }
            return "<НЕТ ДАННЫХ>";
        }

        /// <summary>
        /// Установка дополнительно выводимых данных по выбранному на карте объекту
        /// <summary>        
        protected virtual void RefreshLabels(Dundas.Maps.WebControl.Shape shape)
        {
            StringBuilder strBuilder = new StringBuilder(string.Empty);
            string shapeName;
            Dundas.Maps.WebControl.MapLabel Label;

            Label = map.Labels[MeasureMapLabelName];
            shapeName = shape[MapShapeName].ToString();

            strBuilder.Append(shapeName + "\n");
            // В зависимости от типа показателя выводим нужные данные в нужном формате
            switch ((MeasureKindEnum)MeasureKindIndex)
            {
                case MeasureKindEnum.CompletePercent:
                    strBuilder.Append("Назначено : ");
                    strBuilder.Append(GetTableRowValue(dt1, shapeName, 1, MeasuresTypesEnum.Money));
                    strBuilder.Append("\nИсполнено : ");
                    strBuilder.Append(GetTableRowValue(dt1, shapeName, 7, MeasuresTypesEnum.Money));
                    strBuilder.Append("\nИсполнено % : ");
                    strBuilder.Append(GetTableRowValue(dt1, shapeName, 3, MeasuresTypesEnum.Percent));
                    break; 
                case MeasureKindEnum.AvgMenReceipts:
                    strBuilder.Append("Исполнено : ");
                    strBuilder.Append(GetTableRowValue(dt1, shapeName, 7, MeasuresTypesEnum.Money));
                    strBuilder.Append("\nЧисленность постоянного населения : ");
                    strBuilder.Append(GetTableRowValue(dt1, shapeName, 8, MeasuresTypesEnum.Population));
                    strBuilder.Append("\nСреднедушевые доходы : ");
                    strBuilder.Append(GetTableRowValue(dt1, shapeName, 6, MeasuresTypesEnum.Money));
                    break;
                case MeasureKindEnum.BoostPercent:
                    strBuilder.Append("Исполнено : ");
                    strBuilder.Append(GetTableRowValue(dt1, shapeName, 7, MeasuresTypesEnum.Money));
                    strBuilder.Append("\nИсполнено за аналогичный период предыдущего года : ");
                    strBuilder.Append(GetTableRowValue(dt1, shapeName, 4, MeasuresTypesEnum.Money));
                    strBuilder.Append("\nТемп роста к аналогичному периоду предыдущего года : ");
                    strBuilder.Append(GetTableRowValue(dt1, shapeName, 5, MeasuresTypesEnum.Percent));
                    break;
            }            
            Label.Text = strBuilder.ToString();
        }

        /// <summary>
        /// При клике по карте, обновляем информацию по выбранному объекту
        /// <summary>
        protected virtual void map_Click(object sender, ClickEventArgs e)
        {
            Dundas.Maps.WebControl.HitTestResult result = e.MapControl.HitTest(e.X, e.Y);

            map.Labels[MeasureMapLabelName].Text = string.Empty;

            if (result == null || result.Object == null)
                return;

            if (result.Object is Dundas.Maps.WebControl.Shape)
            {
                Dundas.Maps.WebControl.Shape shape = (Dundas.Maps.WebControl.Shape)result.Object;
                RefreshLabels(shape);
            } 
        }

        /// <summary>
        /// Обновляем данные
        /// <summary>
        protected virtual void ButtonRefresh_Click(object sender, Infragistics.WebUI.WebDataInput.ButtonEventArgs e)
        {
            ChangeParams();
        }
    }
}
