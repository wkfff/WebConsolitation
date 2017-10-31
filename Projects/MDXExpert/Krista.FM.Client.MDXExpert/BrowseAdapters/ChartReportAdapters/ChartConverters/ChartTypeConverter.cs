using System;
using System.ComponentModel;
using System.Globalization;
using Infragistics.UltraChart.Shared.Styles;

namespace Krista.FM.Client.MDXExpert
{
    class ChartTypeConverter : EnumConverter
    {
        const string ctAreaChart = "С областями";
        const string ctAreaChart3D = "Объемная с областями";
        const string ctBarChart = "Линейчатая";
        const string ctBarChart3D = "Объемная линейчатая";
        const string ctBoxChart = "Блочная";
        const string ctBubbleChart = "Пузырьковая";
        const string ctBubbleChart3D = "Объемная пузырьковая";
        const string ctCandleChart = "Биржевая";
        const string ctColumnChart = "Гистограмма";
        const string ctColumnChart3D = "Объемная гистограмма";
        const string ctColumnLineChart = "Гистограмма с графиком";
        const string ctComposite = "Композитная";
        const string ctConeChart3D = "Коническая";
        const string ctCylinderBarChart3D = "Горизонтальная цилиндрическая";
        const string ctCylinderColumnChart3D = "Цилиндрическая";
        const string ctCylinderStackBarChart3D = "Горизонтальная цилиндрическая с накоплением";
        const string ctCylinderStackColumnChart3D = "Цилиндрическая с накоплением";
        const string ctDoughnutChart = "Кольцевая";
        const string ctDoughnutChart3D = "Объемная кольцевая";
        const string ctFunnelChart = "Ворончатая";
        const string ctFunnelChart3D = "Объемная ворончатая";
        const string ctGanttChart = "Диаграмма Ганта";
        const string ctHeatMapChart = "Поверхность";
        const string ctHeatMapChart3D = "Объемная поверхность";
        const string ctHistogramChart = "Гистограмма с гладким графиком";
        const string ctLineChart = "График";
        const string ctLineChart3D = "Объемный график";
        const string ctParetoChart = "Диаграмма Парето";
        const string ctPieChart = "Круговая";
        const string ctPieChart3D = "Объемная круговая";
        const string ctPointChart3D = "Объемная точечная";
        const string ctPolarChart = "Полярная";
        const string ctProbabilityChart = "Вероятностный график";
        const string ctPyramidChart = "Пирамидальная";
        const string ctPyramidChart3D = "Объемная пирамидальная";
        const string ctRadarChart = "Лепестковая";
        const string ctScatterChart = "Точечная";
        const string ctScatterLineChart = "Точечная с графиком";
        const string ctSplineAreaChart = "Сплайн с областями";
        const string ctSplineAreaChart3D = "Объемный сплайн с областями";
        const string ctSplineChart = "Сплайн";
        const string ctSplineChart3D = "Объемный сплайн";
        const string ctStack3DBarChart = "Объемная линейчатая с накоплением";
        const string ctStack3DColumnChart = "Объемная гистограмма с накоплением";
        const string ctStackAreaChart = "С областями и накоплением";
        const string ctStackBarChart = "Линейчатая с накоплением";
        const string ctStackColumnChart = "Гистограмма с накоплением";
        const string ctStackLineChart = "График с накоплением";
        const string ctStackSplineAreaChart = "Сплайн с областями и накоплением";
        const string ctStackSplineChart = "Сплайн с накоплением";
        const string ctStepAreaChart = "Ступенчатая с областями";
        const string ctStepLineChart = "Ступенчатый график";
        const string ctTreeMapChart = "Иерархическая";


        public ChartTypeConverter(Type type)
            : base(type)
        {
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value,
                                         Type destType)
        {
            return ToString(value);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            switch ((string)value)
            {
                case ctAreaChart: return ChartType.AreaChart;
                case ctAreaChart3D: return ChartType.AreaChart3D;
                case ctBarChart: return ChartType.BarChart;
                case ctBarChart3D: return ChartType.BarChart3D;
                case ctBoxChart: return ChartType.BoxChart;
                case ctBubbleChart: return ChartType.BubbleChart;
                case ctBubbleChart3D: return ChartType.BubbleChart3D;
                case ctCandleChart: return ChartType.CandleChart;
                case ctColumnChart: return ChartType.ColumnChart;
                case ctColumnChart3D: return ChartType.ColumnChart3D;
                case ctColumnLineChart: return ChartType.ColumnLineChart;
                case ctComposite: return ChartType.Composite;
                case ctConeChart3D: return ChartType.ConeChart3D;
                case ctCylinderBarChart3D: return ChartType.CylinderBarChart3D;
                case ctCylinderColumnChart3D: return ChartType.CylinderColumnChart3D;
                case ctCylinderStackBarChart3D: return ChartType.CylinderStackBarChart3D;
                case ctCylinderStackColumnChart3D: return ChartType.CylinderStackColumnChart3D;
                case ctDoughnutChart: return ChartType.DoughnutChart;
                case ctDoughnutChart3D: return ChartType.DoughnutChart3D;
                case ctFunnelChart: return ChartType.FunnelChart;
                case ctFunnelChart3D: return ChartType.FunnelChart3D;
                case ctGanttChart: return ChartType.GanttChart;
                case ctHeatMapChart: return ChartType.HeatMapChart;
                case ctHeatMapChart3D: return ChartType.HeatMapChart3D;
                case ctHistogramChart: return ChartType.HistogramChart;
                case ctLineChart: return ChartType.LineChart;
                case ctLineChart3D: return ChartType.LineChart3D;
                case ctParetoChart: return ChartType.ParetoChart;
                case ctPieChart: return ChartType.PieChart;
                case ctPieChart3D: return ChartType.PieChart3D;
                case ctPointChart3D: return ChartType.PointChart3D;
                case ctPolarChart: return ChartType.PolarChart;
                case ctProbabilityChart: return ChartType.ProbabilityChart;
                case ctPyramidChart: return ChartType.PyramidChart;
                case ctPyramidChart3D: return ChartType.PyramidChart3D;
                case ctRadarChart: return ChartType.RadarChart;
                case ctScatterChart: return ChartType.ScatterChart;
                case ctScatterLineChart: return ChartType.ScatterLineChart;
                case ctSplineAreaChart: return ChartType.SplineAreaChart;
                case ctSplineAreaChart3D: return ChartType.SplineAreaChart3D;
                case ctSplineChart: return ChartType.SplineChart;
                case ctSplineChart3D: return ChartType.SplineChart3D;
                case ctStack3DBarChart: return ChartType.Stack3DBarChart;
                case ctStack3DColumnChart: return ChartType.Stack3DColumnChart;
                case ctStackAreaChart: return ChartType.StackAreaChart;
                case ctStackBarChart: return ChartType.StackBarChart;
                case ctStackColumnChart: return ChartType.StackColumnChart;
                case ctStackLineChart: return ChartType.StackLineChart;
                case ctStackSplineAreaChart: return ChartType.StackSplineAreaChart;
                case ctStackSplineChart: return ChartType.StackSplineChart;
                case ctStepAreaChart: return ChartType.StepAreaChart;
                case ctStepLineChart: return ChartType.StepLineChart;
                case ctTreeMapChart: return ChartType.TreeMapChart;
            }
            return null;
        }

        public static string ToString(object value)
        {
            return GetLocalizedChartType((ChartType)value);
        }

        /// <summary>
        /// Получение русского названия типа диаграммы
        /// </summary>
        /// <param name="chartType">тип диаграммы</param>
        /// <returns>русское название</returns>
        public static string GetLocalizedChartType(ChartType chartType)
        {
            switch (chartType)
            {
                case ChartType.AreaChart: return ctAreaChart;
                case ChartType.AreaChart3D: return ctAreaChart3D;
                case ChartType.BarChart: return ctBarChart;
                case ChartType.BarChart3D: return ctBarChart3D;
                case ChartType.BoxChart: return ctBoxChart;
                case ChartType.BubbleChart: return ctBubbleChart;
                case ChartType.BubbleChart3D: return ctBubbleChart3D;
                case ChartType.CandleChart: return ctCandleChart;
                case ChartType.ColumnChart: return ctColumnChart;
                case ChartType.ColumnChart3D: return ctColumnChart3D;
                case ChartType.ColumnLineChart: return ctColumnLineChart;
                case ChartType.Composite: return ctComposite;
                case ChartType.ConeChart3D: return ctConeChart3D;
                case ChartType.CylinderBarChart3D: return ctCylinderBarChart3D;
                case ChartType.CylinderColumnChart3D: return ctCylinderColumnChart3D;
                case ChartType.CylinderStackBarChart3D: return ctCylinderStackBarChart3D;
                case ChartType.CylinderStackColumnChart3D: return ctCylinderStackColumnChart3D;
                case ChartType.DoughnutChart: return ctDoughnutChart;
                case ChartType.DoughnutChart3D: return ctDoughnutChart3D;
                case ChartType.FunnelChart: return ctFunnelChart;
                case ChartType.FunnelChart3D: return ctFunnelChart3D;
                case ChartType.GanttChart: return ctGanttChart;
                case ChartType.HeatMapChart: return ctHeatMapChart;
                case ChartType.HeatMapChart3D: return ctHeatMapChart3D;
                case ChartType.HistogramChart: return ctHistogramChart;
                case ChartType.LineChart: return ctLineChart;
                case ChartType.LineChart3D: return ctLineChart3D;
                case ChartType.ParetoChart: return ctParetoChart;
                case ChartType.PieChart: return ctPieChart;
                case ChartType.PieChart3D: return ctPieChart3D;
                case ChartType.PointChart3D: return ctPointChart3D;
                case ChartType.PolarChart: return ctPolarChart;
                case ChartType.ProbabilityChart: return ctProbabilityChart;
                case ChartType.PyramidChart: return ctPyramidChart;
                case ChartType.PyramidChart3D: return ctPyramidChart3D;
                case ChartType.RadarChart: return ctRadarChart;
                case ChartType.ScatterChart: return ctScatterChart;
                case ChartType.ScatterLineChart: return ctScatterLineChart;
                case ChartType.SplineAreaChart: return ctSplineAreaChart;
                case ChartType.SplineAreaChart3D: return ctSplineAreaChart3D;
                case ChartType.SplineChart: return ctSplineChart;
                case ChartType.SplineChart3D: return ctSplineChart3D;
                case ChartType.Stack3DBarChart: return ctStack3DBarChart;
                case ChartType.Stack3DColumnChart: return ctStack3DColumnChart;
                case ChartType.StackAreaChart: return ctStackAreaChart;
                case ChartType.StackBarChart: return ctStackBarChart;
                case ChartType.StackColumnChart: return ctStackColumnChart;
                case ChartType.StackLineChart: return ctStackLineChart;
                case ChartType.StackSplineAreaChart: return ctStackSplineAreaChart;
                case ChartType.StackSplineChart: return ctStackSplineChart;
                case ChartType.StepAreaChart: return ctStepAreaChart;
                case ChartType.StepLineChart: return ctStepLineChart;
                case ChartType.TreeMapChart: return ctTreeMapChart;
            }
            return string.Empty;

            }
        

    }
}