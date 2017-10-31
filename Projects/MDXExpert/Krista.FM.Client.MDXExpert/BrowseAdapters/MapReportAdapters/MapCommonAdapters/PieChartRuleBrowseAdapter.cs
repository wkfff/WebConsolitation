using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.ComponentModel;

namespace Krista.FM.Client.MDXExpert
{
    public class PieChartRuleBrowseAdapter : FilterablePropertyBase 
    {
        private MapSerie mapSerie;

        [Category("Внешний вид")]
        [Description("Размер диаграммы")]
        [DisplayName("Размер диаграммы")]
        [DynamicPropertyFilter("PieChartBaseType", "Sum, Max, Custom")]
        [Browsable(true)]
        public int PieChartSize
        {
            get { return this.mapSerie.PieChartSize; }
            set 
            { 
                this.mapSerie.PieChartSize = value; 

            }
        }

        [Category("Внешний вид")]
        [Description("Минимальный размер диаграммы")]
        [DisplayName("Минимальный размер диаграммы")]
        [DynamicPropertyFilter("PieChartBaseType", "DynamicSum, DynamicMax")]
        [Browsable(true)]
        public int MinPieChartSize
        {
            get { return this.mapSerie.MinPieChartSize; }
            set
            {
                if (value > 0)
                    this.mapSerie.MinPieChartSize = value;
            }
        }

        [Category("Внешний вид")]
        [Description("Максимальный размер диаграммы")]
        [DisplayName("Максимальный размер диаграммы")]
        [DynamicPropertyFilter("PieChartBaseType", "DynamicSum, DynamicMax")]
        [Browsable(true)]
        public int MaxPieChartSize
        {
            get { return this.mapSerie.MaxPieChartSize; }
            set
            {
                if (value > this.MinPieChartSize)
                    this.mapSerie.MaxPieChartSize = value;
            }
        }

        [Category("Внешний вид")]
        [Description("Основание диаграммы")]
        [DisplayName("Основание диаграммы")]
        [TypeConverter(typeof(EnumTypeConverter))]
        [DefaultValue(PieChartBase.Sum)]
        [Browsable(true)]
        public PieChartBase PieChartBaseType
        {
            get { return this.mapSerie.PieChartBaseType; }
            set
            {
                this.mapSerie.PieChartBaseType = value;
            }
        }

        [Category("Внешний вид")]
        [Description("Пользовательское основание")]
        [DisplayName("Пользовательское основание")]
        [DynamicPropertyFilter("PieChartBaseType", "Custom")]
        [Browsable(true)]
        public double CustomBase
        {
            get { return this.mapSerie.CustomBase; }
            set
            {
                this.mapSerie.CustomBase = value;
            }
        }

        [Category("Данные")]
        [Description("Серия")]
        [DisplayName("Серия")]
        [Browsable(true)]
        public string SerieName
        {
            get { return mapSerie.Name; }
        }

        public PieChartRuleBrowseAdapter(MapSerie mapSerie)
        {
            this.mapSerie = mapSerie;
        }
    }
}
