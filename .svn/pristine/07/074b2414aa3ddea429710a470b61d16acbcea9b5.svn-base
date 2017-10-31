using System;
using System.Drawing;

namespace Krista.FM.Client.MDXExpert.Controls
{
    /// <summary>
    /// Элемент легенды
    /// </summary>
    [Serializable]
    public class LegendItem
    {
        private string _text;
        private Color _color;
        private ExpertLegend _legend;
        private double _startValue;
        private double _endValue;


        public string Text
        {
            get { return _text; }
            set { _text = value; }
        }

        public Color Color
        {
            get { return _color; }
            set { _color = value; }
        }


        public ExpertLegend Legend
        {
            get { return _legend; }
            set { _legend = value; }
        }

        public double StartValue
        {
            get { return _startValue; }
            set { _startValue = value; }
        }

        public double EndValue
        {
            get { return _endValue; }
            set { _endValue = value; }
        }


        public LegendItem()
        {
            this._color = Color.Empty;
            this._text = "";
        }


        public LegendItem(ExpertLegend legend, Color color, string text, double startValue, double endValue)
        {
            this._legend = legend;
            this._color = color;
            this._text = text;
            this._startValue = startValue;
            this._endValue = endValue;
        }

        public void Draw(Graphics g, int top, int itemHeight)
        {
            if (this._legend == null)
                return;

            Rectangle rect = new Rectangle(10, top, itemHeight, itemHeight);
            g.FillRectangle(new SolidBrush(this.Color), rect);
            g.DrawRectangle(new Pen(Color.Black), rect);

            Rectangle bounds = new Rectangle(10 + itemHeight, top, this._legend.Width - itemHeight - 10 * 2, itemHeight);

            string text = this.Legend.RangeLimitsVisible ? String.Format("{0} ({1:" + this.Legend.FormatString + "} - {2:" + this.Legend.FormatString + "})", this.Text, this.StartValue, this.EndValue) : this.Text;
            g.DrawString(text, this._legend.Font, Brushes.Black, bounds, this._legend.StringFormat);
        }
    }

}
