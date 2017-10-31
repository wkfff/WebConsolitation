using System;
using System.Collections.Generic;
using System.Drawing;
using Infragistics.Win.UltraWinGauge;

namespace Krista.FM.Client.MDXExpert.Controls
{
    public class ExpertGaugeCollection : List<ExpertGauge>
    {
        private MultipleGaugeReportElement _element;

        public MultipleGaugeReportElement ReportElement
        {
            get { return this._element; }
            set { this._element = value; }
        }

        public ExpertGaugeCollection(MultipleGaugeReportElement element)
        {
            this._element = element;
        }
        
        public void Refresh()
        {
            foreach (ExpertGauge gauge in this)
            {
                gauge.Refresh();
            }
        }

        public void Add(ExpertGauge item)
        {
            base.Add(item);
        }

    }

}
