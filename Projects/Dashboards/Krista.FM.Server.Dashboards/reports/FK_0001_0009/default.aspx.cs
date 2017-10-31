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
using Krista.FM.Server.Dashboards.reports.FK_0001_0008;

namespace Krista.FM.Server.Dashboards.reports.FK_0001_0009
{
    // Показатель
    public enum MeasureKindEnum
    {
        CompletePercent = 0,
        AvgMenCharges = 1
    }

    public partial class FK_0001_0009Form : FK_0001_0008Form
    {
        /// <summary>
        /// Заполнение списка показателей
        /// <summary>
        protected override void FillMeasuresList()
        {
            Collection<string> MeasuresList = new Collection<string>();
            MeasuresList.Add("Исполнено %");
            MeasuresList.Add("Коэффициент бюджетной обеспечености");

            ComboMeasure.FillValues(MeasuresList);
            ComboMeasure.SelectedIndex = 0;
        }

        /// <summary>
        /// Заполнение списка классификатора(пока не сделан)
        /// <summary>
        protected override void FillClassifierList()
        {
        }
        
        /// <summary>
        /// Заполнение параметра кода расходов(пока не сделан)
        /// <summary>
        protected override void FillClassifierParam()
        {
        }

        /// <summary>
        /// Меняем название легенды в зависимости от выбранного показателя
        /// <summary>
        protected override void SetLegendTitle()
        {
            Legend legend;


            legend = (Legend)map.Legends.GetByName(MapLegendName);
            switch ((MeasureKindEnum)MeasureKindIndex)
            {
                case MeasureKindEnum.CompletePercent:
                    legend.Title = "Исполнено %";
                    break;
                case MeasureKindEnum.AvgMenCharges:
                    legend.Title = "Коэффициент бюджетной обеспечености";
                    break;
            }
        }
        
        /// <summary>
        /// Имя основного запроса данных
        /// <summary>
        protected override string GetMainQueryName()
        {
            return "FK_0001_0009_DataQuery01";
        }

        /// <summary>
        /// Установка дополнительно выводимых данных по выбранному на карте объекту
        /// <summary>        
        protected override void RefreshLabels(Dundas.Maps.WebControl.Shape shape)
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
                case MeasureKindEnum.AvgMenCharges:
                    strBuilder.Append("Исполнено : ");
                    strBuilder.Append(GetTableRowValue(dt1, shapeName, 7, MeasuresTypesEnum.Money));
                    strBuilder.Append("\nЧисленность постоянного населения : ");
                    strBuilder.Append(GetTableRowValue(dt1, shapeName, 8, MeasuresTypesEnum.Population));
                    strBuilder.Append("\nКоэффициент бюджетной обеспечености : ");
                    strBuilder.Append(GetTableRowValue(dt1, shapeName, 6, MeasuresTypesEnum.Money));
                    break;
            }
            Label.Text = strBuilder.ToString();
        }

        /// <summary>
        /// По типу измерителя заполняем нужные параметры фигуры
        /// <summary>
        protected override void FillShapeMeasuresData(Shape shape, DataRow row)
        {
            // Обрабатываем данные фигуры и формат их вывода
            switch ((MeasureKindEnum)MeasureKindIndex)
            {
                case MeasureKindEnum.CompletePercent:
                    SetShapeData(shape, row, 3, MeasuresTypesEnum.Percent);
                    break;
                case MeasureKindEnum.AvgMenCharges:
                    SetShapeData(shape, row, 6, MeasuresTypesEnum.Money);
                    break;
            }
        }

        protected void map_Click(object sender, ClickEventArgs e)
        {
            base.map_Click(sender, e);
        }

        protected void ButtonRefresh_Click(object sender, Infragistics.WebUI.WebDataInput.ButtonEventArgs e)
        {
            base.ButtonRefresh_Click(sender, e);
        }
    }
}
