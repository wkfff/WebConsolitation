using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Infragistics.Win.UltraWinEditors;
using Krista.FM.Client.MDXExpert.Common;

namespace Krista.FM.Client.MDXExpert.Controls
{
    public partial class RangeValueCtrl : UserControl
    {
        public delegate void ValueChangedEventHandler(int number, double value);
        private event ValueChangedEventHandler valueChanged;


        private double value;
        private int number;
        private bool enabled = true;
        private ToolTip toolTip;

        public double Value
        {
            get { return value; }
            set
            {
                this.value = value;
                CheckMaxMinLimit();
                teRangeValue.Value = this.value;
            }
        }

        public double SpinIncrement
        {
            get { return (double)this.teRangeValue.SpinIncrement; }
            set
            {
                this.teRangeValue.SpinIncrement = value;
            }
        }

        public double MinValue
        {
            get { return (double)this.teRangeValue.MinValue; }
            set
            {
                this.teRangeValue.MinValue = value;
            }
        }

        public double MaxValue
        {
            get { return (double)this.teRangeValue.MaxValue; }
            set
            {
                this.teRangeValue.MaxValue = value;
            }
        }

        public string MaskInput
        {
            get
            {
                return this.teRangeValue.MaskInput;
            }
            set
            {
                this.teRangeValue.MaskInput = value;
            }
        }

        public bool Enabled
        {
            get
            {
                return this.teRangeValue.Enabled;
            }
            set
            {
                this.teRangeValue.Enabled = value;
            }
        }

        public Color ValueColor
        {
            get { return teRangeValue.Appearance.ForeColor; }
            set { teRangeValue.Appearance.ForeColor = value; }
        }

        public string ToolTip
        {
            set { this.toolTip.SetToolTip(teRangeValue, value); }
        }

        public event ValueChangedEventHandler ValueChanged
        {
            add { valueChanged += value; }
            remove { valueChanged -= value; }
        }

        public RangeValueCtrl(int rangeLimitNumber, double rangeLimitValue)
        {
            InitializeComponent();
            InitToolTip();
            this.number = rangeLimitNumber;
            lRangeNumber.Text = rangeLimitNumber.ToString();
            teRangeValue.MinValue = Double.MinValue;
            teRangeValue.MaxValue = Double.MaxValue;
            teRangeValue.Value = rangeLimitValue;

        }

        private void InitToolTip()
        {
            this.toolTip = new ToolTip();
            this.toolTip.Active = true;
            this.toolTip.AutoPopDelay = 5000;
            this.toolTip.ShowAlways = true;
            this.toolTip.InitialDelay = 500;
            this.toolTip.UseFading = true;
            this.toolTip.IsBalloon = false;
        }

        private void teRangeValue_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                if ((double)teRangeValue.Value > Consts.maxRangeValue / 100) //10 000 000 000 000)
                {
                    teRangeValue.Value = Math.Truncate((double) teRangeValue.Value);
                }

                if ((double)teRangeValue.Value > Consts.maxRangeValue)
                {
                    teRangeValue.Value = Consts.maxRangeValue;
                }
            }
            catch
            {
                teRangeValue.Value = Consts.maxRangeValue;
            }

            try
            {
                if ((double)teRangeValue.Value < Consts.minRangeValue / 100)// - 10000000000000)
                {
                    teRangeValue.Value = Math.Truncate((double) teRangeValue.Value);
                }

                if ((double)teRangeValue.Value < Consts.minRangeValue)
                {
                    teRangeValue.Value = Consts.minRangeValue;
                }
            }
            catch
            {
                teRangeValue.Value = Consts.minRangeValue;
            }

            try
            {
                this.value = (double) teRangeValue.Value;
            }
            catch
            {
                if (this.value < 0)
                    this.value = Consts.minRangeValue;
                if (this.value > 0)
                    this.value = Consts.maxRangeValue;
            }

            if (this.valueChanged != null)
                this.valueChanged(this.number, this.value);

        }

        private void teRangeValue_EditorSpinButtonClick(object sender, Infragistics.Win.UltraWinEditors.SpinButtonClickEventArgs e)
        {
            switch (e.ButtonType)
            {
                case SpinButtonItem.NextItem:
                    this.value += this.SpinIncrement;
                    break;
                case SpinButtonItem.PreviousItem:
                    this.value -= this.SpinIncrement;
                    break;
            }

            CheckMaxMinLimit();
            teRangeValue.Value = this.value;
        }

        private void teRangeValue_Leave(object sender, EventArgs e)
        {
            CheckMaxMinLimit();
            teRangeValue.Value = this.value;
        }

        private void CheckMaxMinLimit()
        {
            if (this.value > this.MaxValue)
                this.value = this.MaxValue;
            if (this.value < this.MinValue)
                this.value = this.MinValue;
        }

        private void teRangeValue_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                CheckMaxMinLimit();
                teRangeValue.Value = this.value;
            }
        }



    }
}
