using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Krista.FM.Client.MDXExpert.Controls
{
    public partial class RangeColorCtrl : UserControl
    {
        public delegate void ValueChangedEventHandler(int index, Color color, string text);
        private event ValueChangedEventHandler valueChanged;



        private Color _color;
        private string _text;
        private int index;

        public Color Color
        {
            get { return this._color; }
            set
            {
                this._color = value;
                SetColor(value);
            }
        }

        public string Text
        {
            get { return this._text; }
            set
            {
                this._text = value;
                SetText(value);
            }
        }

        public event ValueChangedEventHandler ValueChanged
        {
            add { valueChanged += value; }
            remove { valueChanged -= value; }
        }

        public RangeColorCtrl(int rangeIndex, Color color, string text)
        {
            InitializeComponent();
            this.Color = color;
            this.Text = text;
            this.index = rangeIndex;
        }

        private void SetColor(Color value)
        {
            this.cpColor.Color = value;
        }

        private void SetText(string value)
        {
            this.teRangeText.Text = value;
        }

        private void cpColor_ColorChanged(object sender, EventArgs e)
        {
            this._color = this.cpColor.Color;
            if (this.valueChanged != null)
                this.valueChanged(this.index, this.Color, this.Text);
        }

        private void teRangeText_ValueChanged(object sender, EventArgs e)
        {
            this._text = this.teRangeText.Text;
            if (this.valueChanged != null)
                this.valueChanged(this.index, this.Color, this.Text);
        }


    }
}
