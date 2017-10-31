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

using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

using Infragistics.WebUI.UltraWebChart;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.WebUI.UltraWebGrid;

using Dundas.Maps.WebControl;
namespace Krista.FM.Server.Dashboards.reports.CTAT.CTAT_0005
{
    public partial class _default : CustomReportPage
    {

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

            if (DT == null || map == null) return;

            foreach (DataRow row in DT.Rows)
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
                            //shape.Text = "";

                            FillShapeMeasuresData(shape, row);
                            shape[MapShapeName] = subject;
                            shape.ToolTip = "#NAME";
                            shape.TextVisibility = 0;
                            
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
            string[] subjects = { subject, "" };

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
            //shape[MeasureFieldName] = ConvertMeasureValue(row[ColumnIndex], MeasureType);
            shape.ToolTip += " #MEASUREFIELD{N2}";

            shape.Text += (char)(10) + " " + row[1].ToString();

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





        public void setFont(int typ, Label lab)
        {
            lab.Font.Name = "arial";
            lab.Font.Size = typ;
            if (typ == 14) { lab.Font.Bold = 1 == 1; };
            if (typ == 10) { lab.Font.Bold = 1 == 1; };
            if (typ == 18) { lab.Font.Bold = 1 == 1; };
        }

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
            //map.ShapeFields.Clear();
            //map.ShapeFields.Add(MapShapeName);
            //map.ShapeFields[MapShapeName].Type = typeof(string);
            //map.ShapeFields[MapShapeName].UniqueIdentifier = true;
            //map.ShapeFields.Add(MeasureFieldName);
            //map.ShapeFields[MeasureFieldName].Type = typeof(double);
            //map.ShapeFields[MeasureFieldName].UniqueIdentifier = false;

            //// добавляем правила раскраски
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
               // double dirtyWidth = ((int)Session["width_size"] - 10);
               // double dirtyHeight = ((int)Session["height_size"] - 350);

            //    map.Width = (int)(dirtyWidth);
            //    map.Height = (int)(dirtyHeight);
            //
            }
            catch
            {
            }
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            BG.DataBind();
            BC.DataBind();
            TG.DataBind();
            TC.DataBind();

            #region mapa
            map.LoadFromShapeFile(Server.MapPath("../../../maps/CTAT/Приволжский федеральный округ.shp"), "NAME", true);
            q = @"Select
    non empty                 
    {
        [Территории].[РФ Карта].[Федеральный округ].[Приволжский ФО].Children
    }    on rows,
    { [Measures].[За период]     }   on columns
from [СТАТ_Уровень жизни_Правонарушения]      
where                 
    (
        [Уровень жизни].[Правонарушения].[Количество зарегистрированных преступлений],
        [Период].[Год Квартал Месяц].[2006],
        [Группировки].[Уровень жизни_Правонарушения].[По видам правонарушений],
         [Уровень жизни].[Виды правонарушений].[Все виды правонарушений].[Кража],
        [Источники данных].[Источник].[СТАТ Отчетность - Облстат]   
    )      ";
            DT = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(q, "adada", DT);
            FillMapData();
            #endregion 


        }
        string q = "";
        protected void TG_DataBinding(object sender, EventArgs e)
        {
                   q = @"Select
            non empty  
            {
                [Территории].[РФ].[Все территории].[Российская Федерация].[Приволжский федеральный округ] 
            }on columns  ,
            {
                [Уровень жизни].[Виды правонарушений].[Все виды правонарушений].[Умышленные убийства и покушения на убийство],
                [Уровень жизни].[Виды правонарушений].[Все виды правонарушений].[Умышленное причинение тяжкого вреда здоровью],
                [Уровень жизни].[Виды правонарушений].[Все виды правонарушений].[Изнасилование и покушение на изнасилование],
                [Уровень жизни].[Виды правонарушений].[Все виды правонарушений].[Разбой],
                [Уровень жизни].[Виды правонарушений].[Все виды правонарушений].[Грабеж],
                [Уровень жизни].[Виды правонарушений].[Все виды правонарушений].[Кража] 
            }   on rows
        from [СТАТ_Уровень жизни_Правонарушения]  
        where  
            (
                [Период].[Год Квартал Месяц].[2006] ,
                [Уровень жизни].[Правонарушения].[Количество зарегистрированных преступлений],
                [Источники данных].[Источник].[СТАТ Отчетность - Облстат],
                [Группировки].[Уровень жизни_Правонарушения].[По видам правонарушений],
                [Measures].[За период]  
            )";
             DT = new DataTable();
             DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(q, "asda", DT);
             TG.DataSource = DT;

        }
        DataTable DT;
        protected void TC_DataBinding(object sender, EventArgs e)
        {
            q = @"
            Select
                non empty  
                {
                    [Территории].[РФ].[Все территории].[Российская Федерация].[Приволжский федеральный округ].[Саратовская область]
                }on columns  ,
                {
                    [Период].[Год Квартал Месяц].[2002]:[Период].[Год Квартал Месяц].[2016]  
                }   on rows  
            from [СТАТ_Уровень жизни_Правонарушения]  
            where  
                (
                    [Уровень жизни].[Виды правонарушений].[Все виды правонарушений].[Умышленные убийства и покушения на убийство],
                    [Уровень жизни].[Правонарушения].[Количество зарегистрированных преступлений],
                    [Источники данных].[Источник].[СТАТ Отчетность - Облстат],
                    [Группировки].[Уровень жизни_Правонарушения].[По видам правонарушений],
                    [Measures].[За период]  
                )  ";
            DT = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(q, "sad", DT);
            TC.DataSource = DT;    
        }

        protected void BG_DataBinding(object sender, EventArgs e)
        {
                    q = @"Select
            non empty           
            {
                [Период].[Год Квартал Месяц].[2002]:[Период].[Год Квартал Месяц].[2016]         
            }on columns  ,
            {
                [Уровень жизни].[Доходы и расходы населения].[Денежные доходы населения в расчете на душу населения],
                [Уровень жизни].[Доходы и расходы населения].[Денежные расходы населения в расчете на душу населения],
                [Уровень жизни].[Доходы и расходы населения].[Доля прироста, отлива (-) сбережений населения во вкладах в учреждениях Сбербанка России в объёме текущих денежных доходов],
                [Уровень жизни].[Доходы и расходы населения].[Реальные денежные доходы],
                [Уровень жизни].[Доходы и расходы населения].[Стоимость минимального набора продуктов питания],
                [Уровень жизни].[Доходы и расходы населения].[Стоимость фиксированного набора потребительских товаров и услуг для межрегиональных сопоставлений покупательной способности населения]   
            }   on rows           
        from [СТАТ_Уровень жизни_Доходы и расходы населения]         
        where           
            (
                [Территории].[РФ].[Все территории].[Российская Федерация].[Приволжский федеральный округ].[Саратовская область],
                [Группировки].[Уровень жизни_Доходы и расходы].[Без группировки],
                [Источники данных].[Источник].[СТАТ Отчетность - Облстат],
                [Measures].[За период]           
            )";
                    DT = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(q, "asd", DT);
            BG.DataSource = DT;
        }

        protected void BC_DataBinding(object sender, EventArgs e)
        {
            q = @"Select
            non empty           
            {
                [Период].[Год Квартал Месяц].[2002]:[Период].[Год Квартал Месяц].[2016]         
            }on columns  ,
            {
                [Уровень жизни].[Доходы и расходы населения].[Денежные доходы населения в расчете на душу населения]
            }   on rows           
        from [СТАТ_Уровень жизни_Доходы и расходы населения]         
        where           
            (
                [Территории].[РФ].[Все территории].[Российская Федерация].[Приволжский федеральный округ].[Саратовская область],
                [Группировки].[Уровень жизни_Доходы и расходы].[Без группировки],
                [Источники данных].[Источник].[СТАТ Отчетность - Облстат],
                [Measures].[За период]           
            )";
            DT = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(q, "sda", DT);
            BC.DataSource = DT;
        }
    }
}
