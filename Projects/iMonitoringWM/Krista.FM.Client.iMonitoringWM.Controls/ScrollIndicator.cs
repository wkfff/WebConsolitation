using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Krista.FM.Client.iMonitoringWM.Controls
{
    public partial class ScrollIndicator : UserControl
    {
        /// <summary>
        /// Размер индикатора отностительно высоты контрола
        /// </summary>
        const float percentageOfHeight = 60;

        #region Поля
        private int _controlCount;
        private int _changedControlIndex;
        private SolidBrush _changedBrush;
        private SolidBrush _unChangedBrush;
        #endregion

        #region Свойства
        public int ControlCount
        {
            get { return _controlCount; }
            set { this.SetControlCount(value); }
        }

        public int ChangedControlIndex
        {
            get { return _changedControlIndex; }
            set { this.SetChangedControlIndex(value); }
        }

        public SolidBrush ChangedBrush
        {
            get { return _changedBrush; }
            set { _changedBrush = value; }
        }

        public SolidBrush UnChangedBrush
        {
            get { return _unChangedBrush; }
            set { _unChangedBrush = value; }
        }
        #endregion

        public ScrollIndicator()
        {
            InitializeComponent();
            this.SetDefaultValue();
        }

        private void SetDefaultValue()
        {
            this.ChangedControlIndex = 0;
            this.ChangedControlIndex = -1;
            this.ChangedBrush = new SolidBrush(Color.White);
            this.UnChangedBrush = new SolidBrush(Color.Gray);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            this.SuspendLayout();

            base.OnPaint(e);
            this.PaintScrollIndicator(e.Graphics);

            this.ResumeLayout();
        }

        private void PaintScrollIndicator(Graphics graphics)
        {
            if (this.ControlCount > 0)
            {
                //диаметр индикаторов
                int indicatorDiameter = this.GetIndicatorDiameter();
                //растояние между индикаторами
                int indicatorSpacing = indicatorDiameter;
                Point indicatorLocation = this.GetFirstIndicatorLocation(indicatorDiameter);
                Rectangle indicatorBounds = new Rectangle(indicatorLocation.X, indicatorLocation.Y,
                    indicatorDiameter, indicatorDiameter);

                for (int i = 0; i < this.ControlCount; i++)
                {
                    bool isChanged = (i == this.ChangedControlIndex);
                    graphics.FillEllipse(isChanged ? this.ChangedBrush : this.UnChangedBrush, indicatorBounds);
                    indicatorBounds.X += indicatorDiameter * 2;
                }
            }
        }
        /// <summary> 
        /// Получаем диаметр индикатора
        /// </summary>
        /// <returns>диаметр</returns>
        private int GetIndicatorDiameter()
        {
            return (int)((float)this.Height / 100f * percentageOfHeight);
        }

        /// <summary>
        /// Расположение первого индикатора
        /// </summary>
        /// <param name="indicatorDiametr">диаметр индикатора</param>
        /// <returns>точка расположения</returns>
        private Point GetFirstIndicatorLocation(int indicatorDiametr)
        {
            int x = (int)((float)this.Width / 2f - (float)(indicatorDiametr * this.ControlCount) +
                ((float)indicatorDiametr / 2f));
            int y = (int)((float)(this.Height - indicatorDiametr) / 2f);
            return new Point(x,y);
        }

        private void SetControlCount(int value)
        {
            _controlCount = value;
            this.Invalidate();
        }

        private void SetChangedControlIndex(int value)
        {
            _changedControlIndex = value;
            this.Invalidate();
        }
    }
}
