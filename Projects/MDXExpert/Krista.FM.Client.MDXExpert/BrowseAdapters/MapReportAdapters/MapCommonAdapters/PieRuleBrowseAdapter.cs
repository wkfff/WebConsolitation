using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Drawing;

namespace Krista.FM.Client.MDXExpert
{
    public class PieRuleBrowseAdapter
    {
        private SerieRule rule;
        private MapSerie serie;

        [Category("Внешний вид")]
        [Description("Цвет сектора")]
        [DisplayName("Цвет сектора")]
        [Browsable(true)]
        public Color PieColor
        {
            get { return this.rule.PieChartColor; }
            set { this.rule.PieChartColor = value; }
        }

        [Category("Данные")]
        [Description("Серия")]
        [DisplayName("Серия")]
        [Browsable(true)]
        public string SerieName
        {
            get { return serie.Name; }
        }

        [Category("Данные")]
        [Description("Показатель")]
        [DisplayName("Показатель")]
        [Browsable(true)]
        public string MeasureName
        {
            get { return rule.Name; }
        }


        public PieRuleBrowseAdapter(MapSerie serie, SerieRule rule)
        {
            this.serie = serie;
            this.rule = rule;

        }
    }
}
