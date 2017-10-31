using System;
using System.Collections.Generic;
using System.Drawing;

namespace Krista.FM.Client.MDXExpert.Controls
{
    public class LegendItemCollection : List<LegendItem>
    {
        private ExpertLegend _legend;
        private bool _isAllItemsDrawed = true;

        public LegendItem this[int index]
        {
            get
            {
                return (LegendItem)base[index];
            }
            set
            {
                value.Legend = this._legend;
                base[index] = value;
            }
        }
        
        /// <summary>
        /// Признак что все элементы легенды уместились и отображены в легенде
        /// </summary>
        public bool IsAllItemsDrawed
        {
            get { return this._isAllItemsDrawed; }
        }

        public LegendItemCollection(ExpertLegend legend)
        {
            this._legend = legend;
        }


        public void Add(LegendItem item)
        {
            item.Legend = this._legend;
            base.Add(item);
            this._legend.DrawLegend();
        }

        public void Add(Color color, string text, double startValue, double endValue)
        {
            this.Add(new LegendItem(this._legend, color, text, startValue, endValue));
        }

        public void Clear()
        {
            base.Clear();
            this._legend.DrawLegend();
        }

        public void Draw(Graphics g)
        {
            int itemHeight = Math.Max(this._legend.ItemSize, this._legend.Font.Height);

            int currItemTop = 10;
            this._isAllItemsDrawed = true;
            foreach (LegendItem item in this)
            {
                if ((currItemTop + itemHeight + 10) <= this._legend.Height)
                {
                    item.Draw(g, currItemTop, itemHeight);
                    currItemTop += this._legend.ItemSpacing + itemHeight;
                }
                else
                {
                    this._isAllItemsDrawed = false;
                    return;
                }
            }
        }

    }

}
