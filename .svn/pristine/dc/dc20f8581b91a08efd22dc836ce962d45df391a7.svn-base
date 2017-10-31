using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using Krista.FM.Client.MDXExpert.Common;

namespace Krista.FM.Client.MDXExpert.Controls
{
    public partial class RangeCollectionControl : UserControl
    {
        #region поля

        private List<double> rangeLimits;
        private List<Color> rangeColors;
        private List<string> rangeTexts;

        private List<RangeValueCtrl> rangeCtrls = new List<RangeValueCtrl>();
        private List<RangeColorCtrl> colorCtrls = new List<RangeColorCtrl>();

        private bool isMarkPositionChanging = false;
        private double totalMinimum;
        private double totalMaximum;

        #endregion

        #region свойства

        public List<double> RangeLimits
        {
            get { return rangeLimits; }
            set { rangeLimits = value; }
        }

        public int DigitCount
        {
            get { return (int)this.neDecimalCount.Value; }
            set
            {
                this.neDecimalCount.Value = value;
            }
        }

        public List<Color> RangeColors
        {
            get { return rangeColors; }
            set { rangeColors = value; }
        }

        public List<string> RangeTexts
        {
            get { return rangeTexts; }
            set { rangeTexts = value; }
        }

        #endregion

        public RangeCollectionControl(List<double> rangeLimits, double totalMinimum, double totalMaximum)
        {
            InitializeComponent();

            this.totalMinimum = totalMinimum;
            this.totalMaximum = totalMaximum;

            this.RangeLimits = rangeLimits;
            Init();
        }

        public RangeCollectionControl(List<double> rangeLimits, List<Color> colors, List<string> rangeText, double totalMinimum, double totalMaximum)
        {
            InitializeComponent();

            this.totalMinimum = totalMinimum;
            this.totalMaximum = totalMaximum;

            this.RangeLimits = rangeLimits;
            this.RangeColors = colors;
            this.RangeTexts = rangeText;

            InitColorRanges();
        }

        private void SetPosition()
        {
            if (this.Parent == null)
                return;

            if (this.Parent.Top < 0)
            {
                this.Parent.Top = 0;
            }
        }


        private void Init()
        {
            if (this.RangeLimits.Count > 0)
            {
                this.rangeBar.Minimum = RangeLimits[0];
                this.rangeBar.Maximum = RangeLimits[RangeLimits.Count - 1];
            }

            this.Height = this.pTracBarCont.Height;
            this.Height += this.ultraPanel1.Height;


            for (int i = 0; i < this.RangeLimits.Count; i++)
            {
                this.rangeCtrls.Add(new RangeValueCtrl(i, this.RangeLimits[i]));
                this.rangeCtrls[i].SpinIncrement = GetSpinIncrementValue();
                this.rangeCtrls[i].ValueChanged += MapRangeControl_ValueChanged;
                this.pRangeValuesCont.ClientArea.Controls.Add(this.rangeCtrls[i]);
                this.rangeCtrls[i].Dock = DockStyle.Top;
                this.Height += this.rangeCtrls[i].Height;
                this.rangeCtrls[i].BringToFront();

                if (i == 0)
                {
                    this.rangeCtrls[i].MinValue = Double.MinValue;
                    this.rangeCtrls[i].MaxValue = Double.MaxValue;
                    continue;
                }
                else
                if (i == this.RangeLimits.Count - 1)
                {
                    this.rangeCtrls[i].MinValue = this.RangeLimits[i - 1];
                    this.rangeCtrls[i].MaxValue = Double.MaxValue;
                    this.rangeCtrls[i - 1].MaxValue = this.RangeLimits[i];
                    continue;
                }
                else
                {
                    this.rangeCtrls[i].MinValue = this.RangeLimits[i - 1];
                    this.rangeCtrls[i - 1].MaxValue = this.RangeLimits[i];
                }
                this.rangeBar.Marks.Add(new TrackBarMark(this.rangeBar, this.RangeLimits[i], i));
            }
            CheckValues();
            
        }

        private void InitColorRanges()
        {
            if (this.RangeLimits.Count > 0)
            {
                this.rangeBar.Minimum = RangeLimits[0];
                this.rangeBar.Maximum = RangeLimits[RangeLimits.Count - 1];
            }

            this.Height = this.pTracBarCont.Height;
            this.Height += this.ultraPanel1.Height;


            for (int i = 0; i < this.RangeLimits.Count; i++)
            {
                this.rangeCtrls.Add(new RangeValueCtrl(i, this.RangeLimits[i]));
                this.rangeCtrls[i].SpinIncrement = GetSpinIncrementValue();
                this.rangeCtrls[i].ValueChanged += MapRangeControl_ValueChanged;
                this.pRangeValuesCont.ClientArea.Controls.Add(this.rangeCtrls[i]);
                this.rangeCtrls[i].Dock = DockStyle.Top;
                this.Height += this.rangeCtrls[i].Height;
                this.rangeCtrls[i].BringToFront();
                this.AddColorCtrl(i);

                if (i == 0)
                {
                    this.rangeCtrls[i].MinValue = Double.MinValue;
                    this.rangeCtrls[i].MaxValue = Double.MaxValue;
                    continue;
                }
                else
                    if (i == this.RangeLimits.Count - 1)
                    {
                        this.rangeCtrls[i].MinValue = this.RangeLimits[i - 1];
                        this.rangeCtrls[i].MaxValue = Double.MaxValue;
                        this.rangeCtrls[i - 1].MaxValue = this.RangeLimits[i];
                        continue;
                    }
                    else
                    {
                        this.rangeCtrls[i].MinValue = this.RangeLimits[i - 1];
                        this.rangeCtrls[i - 1].MaxValue = this.RangeLimits[i];
                    }
                this.rangeBar.Marks.Add(new TrackBarMark(this.rangeBar, this.RangeLimits[i], i));



            }
            CheckValues();
        }

        private void AddColorCtrl(int index)
        {
            if (index < (this.RangeLimits.Count - 1))
            {
                this.colorCtrls.Add(new RangeColorCtrl(index, this.RangeColors[index], this.RangeTexts[index]));
                this.colorCtrls[index].ValueChanged += new RangeColorCtrl.ValueChangedEventHandler(RangeCollectionControl_ValueChanged);
                this.pRangeValuesCont.ClientArea.Controls.Add(this.colorCtrls[index]);
                this.colorCtrls[index].Dock = DockStyle.Top;
                this.Height += this.colorCtrls[index].Height;
                this.colorCtrls[index].BringToFront();
            }

        }

        void RangeCollectionControl_ValueChanged(int index, Color color, string text)
        {
            this.RangeColors[index] = color;
            this.RangeTexts[index] = text;
        }


        private double GetSpinIncrementValue()
        {
            double value = CommonUtils.SetNumberPrecision((this.rangeBar.Maximum - this.rangeBar.Minimum)/100, this.DigitCount);
            if (value < Math.Pow(10, - this.DigitCount))
            {
                value = Math.Pow(10, - this.DigitCount);
                value = CommonUtils.SetNumberPrecision(value, this.DigitCount);
            }
            return value;
        }

        private void CheckValues()
        {
            if (this.rangeCtrls.Count == 0)
            {
                return;
            }

            if (rangeCtrls[0].Value > this.totalMinimum)
            {
                rangeCtrls[0].ValueColor = Color.Red;
                rangeCtrls[0].ToolTip = "Минимальное значение показателя (" + this.totalMinimum.ToString() + ")\nвыходит за границу интервала";
            }
            else
            {
                rangeCtrls[0].ValueColor = Color.Black;
                rangeCtrls[0].ToolTip = "";
            }

            if (rangeCtrls[rangeCtrls.Count - 1].Value < this.totalMaximum)
            {
                rangeCtrls[rangeCtrls.Count - 1].ValueColor = Color.Red;
                rangeCtrls[rangeCtrls.Count - 1].ToolTip = "Максимальное значение показателя (" + this.totalMaximum.ToString() + ")\nвыходит за границу интервала";
            }
            else
            {
                rangeCtrls[rangeCtrls.Count - 1].ValueColor = Color.Black;
                rangeCtrls[rangeCtrls.Count - 1].ToolTip = "";
            }

        }

        /// <summary>
        /// Обрезает число после заданного кол-ва цифр после запятой
        /// </summary>
        /// <param name="number"></param>
        /// <param name="digitCount"></param>
        /// <returns></returns>
        private double TruncNumber(double number, int digitCount)
        {
            try
            {
                number *= Math.Pow(10, digitCount);
                number = Math.Truncate(number);
                number /= Math.Pow(10, digitCount);
                return number;
            }
            catch
            {
                return number;
            }
        }


        private void CalcRangeLimits()
        {
            if (this.RangeLimits.Count < 2)
                return;

            this.RangeLimits[0] = (this.totalMinimum > 0) ? TruncNumber(this.totalMinimum, this.DigitCount) : this.totalMinimum;
            this.RangeLimits[this.RangeLimits.Count - 1] = (this.totalMaximum < 0) ? TruncNumber(this.totalMaximum, this.DigitCount) : this.totalMaximum;
            

            for (int i = 1; i < this.RangeLimits.Count - 1; i++)
            {
                if (this.RangeLimits[i] < this.totalMinimum)
                    this.RangeLimits[i] = this.totalMinimum;
                if (this.RangeLimits[i] > this.totalMaximum)
                    this.RangeLimits[i] = this.totalMaximum;
            }


            this.rangeBar.Marks.Clear();
            this.rangeBar.Minimum = this.totalMinimum;
            this.rangeBar.Maximum = this.totalMaximum;
            isMarkPositionChanging = true;

            for (int i = 0; i < this.RangeLimits.Count; i++)
            {
                this.rangeCtrls[i].SpinIncrement = GetSpinIncrementValue();
                this.rangeCtrls[i].MinValue = Double.MinValue;
                this.rangeCtrls[i].MaxValue = Double.MaxValue;
                this.rangeCtrls[i].Value = RangeLimits[i];

                if (i == 0)
                {
                    this.rangeCtrls[i].MinValue = Double.MinValue;
                    this.rangeCtrls[i].MaxValue = Double.MaxValue;
                    continue;
                }
                else
                    if (i == this.RangeLimits.Count - 1)
                    {
                        this.rangeCtrls[i].MinValue = this.RangeLimits[i - 1];
                        this.rangeCtrls[i].MaxValue = Double.MaxValue;
                        this.rangeCtrls[i - 1].MaxValue = this.RangeLimits[i];
                        continue;
                    }
                    else
                    {
                        this.rangeCtrls[i].MinValue = this.RangeLimits[i - 1];
                        this.rangeCtrls[i - 1].MaxValue = this.RangeLimits[i];
                    }
                this.rangeBar.Marks.Add(new TrackBarMark(this.rangeBar, this.RangeLimits[i], i));
            }
            isMarkPositionChanging = false;

        }

        private void MapRangeControl_ValueChanged(int number, double value)
        {
            
            if (number <= this.rangeBar.Marks.Count + 1)
            {
                value = CommonUtils.SetNumberPrecision(value, this.DigitCount);
                if (!isMarkPositionChanging)
                {
                    if (number == 0)
                    {
                        this.rangeBar.Minimum = value;
                    }
                    else
                    if (number == this.rangeLimits.Count - 1)
                    {
                        this.rangeBar.Maximum = value;
                    }
                    else
                    {
                        this.rangeBar.Marks[number - 1].Value = value;
                    }
                }
                rangeLimits[number] = value;
                if (number > 0)
                    this.rangeCtrls[number - 1].MaxValue = value;
                if (number < rangeCtrls.Count - 1)
                    this.rangeCtrls[number + 1].MinValue = value;

                CheckValues();
            }
        }

        private void OnRangeChanged(object sender, System.EventArgs e)
        {
            isMarkPositionChanging = true;
            for (int i = 0; i < this.rangeBar.Marks.Count; i++ )
            {
                this.rangeCtrls[i + 1].Value = CommonUtils.SetNumberPrecision(this.rangeBar.Marks[i].Value, this.DigitCount); 
                rangeLimits[i + 1] = this.rangeBar.Marks[i].Value;
                this.rangeCtrls[i].MaxValue = this.rangeBar.Marks[i].Value;
                this.rangeCtrls[i + 2].MinValue = this.rangeBar.Marks[i].Value;

            }
            CheckValues();
            isMarkPositionChanging = false;
        }

        private void OnRangeChanging(object sender, System.EventArgs e)
        {
            isMarkPositionChanging = true;
            for (int i = 0; i < this.rangeBar.Marks.Count; i++)
            {
                this.rangeCtrls[i + 1].Value = CommonUtils.SetNumberPrecision(this.rangeBar.Marks[i].Value, this.DigitCount);
                rangeLimits[i + 1] = this.rangeBar.Marks[i].Value;
                this.rangeCtrls[i].MaxValue = this.rangeBar.Marks[i].Value;
                this.rangeCtrls[i + 2].MinValue = this.rangeBar.Marks[i].Value;

            }
            CheckValues();
            isMarkPositionChanging = false;
        }

        private void btOK_Click(object sender, EventArgs e)
        {
            if (Tag != null)
            {
                ((IWindowsFormsEditorService)Tag).CloseDropDown();
            }
            else
            {
                this.Hide();
            }

        }

        private void btCalcLimits_Click(object sender, EventArgs e)
        {
            CalcRangeLimits();
        }

        private void neDecimalCount_ValueChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < this.rangeCtrls.Count; i++)
            {
                this.rangeBar.DigitCount = this.DigitCount;
                double value = CommonUtils.SetNumberPrecision(this.rangeCtrls[i].Value, this.DigitCount);
                this.rangeCtrls[i].Value = value;
                this.rangeLimits[i] = value;
                this.rangeCtrls[i].MaskInput = "{double:-25." + neDecimalCount.Value.ToString() + ":c}";
                this.rangeCtrls[i].SpinIncrement = GetSpinIncrementValue();
            }
            CheckValues();

        }

        private void RangeCollectionControl_Load(object sender, EventArgs e)
        {
            SetPosition();
        }
    }
}
