using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using Dundas.Maps.WinControl;

namespace Krista.FM.Client.MDXExpert
{
    public class MapPieChartSymbol
    {
        private Symbol pieChartSymbol;
        private double chartBase;
        private List<Double> values;
        private Color baseColor;
        private bool isEmpty;

        /// <summary>
        /// Значок, на котором рисуется диаграмма
        /// </summary>
        public Symbol PieChartSymbol
        {
            get { return this.pieChartSymbol; }
            set { this.pieChartSymbol = value; }
        }

        /// <summary>
        /// Величина основания диаграммы
        /// </summary>
        public double ChartBase
        {
            get { return chartBase; }
        }

        /// <summary>
        /// Значения секторной диаграммы
        /// </summary>
        public List<double> Values
        {
            get { return values; }
        }

        /// <summary>
        /// Цвет основания диаграммы
        /// </summary>
        public Color BaseColor
        {
            get { return baseColor; }
            set { baseColor = value; }
        }

        /// <summary>
        /// Показывать диаграмму или нет
        /// </summary>
        public bool Visible
        {
            get { return this.pieChartSymbol.Visible; }
            set { this.pieChartSymbol.Visible = value; }
        }

        /// <summary>
        /// Если нет значений для диаграммы - true
        /// </summary>
        public bool IsEmpty
        {
            get { return isEmpty; }
            set { isEmpty = value; }
        }


        public MapPieChartSymbol(Symbol pieChartSymbol, double chartBase, List<Double> values, Color baseColor)
        {
            this.pieChartSymbol = pieChartSymbol;
            this.pieChartSymbol.BorderColor = Color.Empty;
            this.chartBase = chartBase;
            this.values = values;
            this.baseColor = baseColor;
            this.isEmpty = false;
        }

        public MapPieChartSymbol(Symbol pieChartSymbol, double chartBase, List<Double> values, Color baseColor, bool isEmptyChart)
        {
            this.pieChartSymbol = pieChartSymbol;
            this.pieChartSymbol.BorderColor = Color.Empty;
            this.chartBase = chartBase;
            this.values = values;
            this.baseColor = baseColor;
            this.isEmpty = isEmptyChart;
        }

    }
}
