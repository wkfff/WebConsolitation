using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using Krista.FM.Client.iMonitoringWM.Common;
using Microsoft.WindowsCE.Forms;
using System.Runtime.InteropServices;
using Krista.FM.Client.iMonitoringWM.Controls.Properties;

namespace Krista.FM.Client.iMonitoringWM.Controls
{
    public partial class PopupMenu : UserControl
    {
        #region Поля
        private int _minOverhang;
        private int _maxOverhang;
        private bool _conceal;
        private bool _withoutDock;

        private PopupMenuPlaceMode _placeMode;
        private Timer concealTimer;
        #endregion

        #region Свойства
        /// <summary>
        /// Минимальный размер выступа меню
        /// </summary>
        public int MinOverhang
        {
            get { return _minOverhang; }
        }

        /// <summary>
        /// Максимальный размер выступа меню
        /// </summary>
        public int MaxOverhang
        {
            get { return _maxOverhang; }
        }

        /// <summary>
        /// Скрывать ли от пользователя контрол
        /// </summary>
        public virtual bool Conceal
        {
            get { return _conceal; }
            set
            {
                this.SetConceal(value);
            }
        }

        /// <summary>
        /// Время через которое скрывается меню
        /// </summary>
        public int ConcealTime
        {
            get { return this.concealTimer.Interval; }
            set { this.concealTimer.Interval = value; }
        }

        /// <summary>
        /// Режим отображения меню
        /// </summary>
        public virtual PopupMenuPlaceMode PlaceMode
        {
            get { return _placeMode; }
            set
            {
                _placeMode = value;
                this.SetBackGroundImage(_placeMode);
                this.SetPlaceMode(_placeMode);
            }
        }

        public new ControlCollection Controls
        {
            get { return this.gradientPanel.Controls; }
        }

        /// <summary>
        /// При установке расположения меню не будет прикреплять его, и растягивать 
        /// на всю ширину/длинну
        /// </summary>
        public bool WithoutDock
        {
            get { return _withoutDock; }
            set { _withoutDock = value; }
        }

        public Bitmap Image
        {
            get { return this.gradientPanel.Image; }
            set { this.gradientPanel.Image = value; }
        }

        public Bitmap VgaImage
        {
            get { return this.gradientPanel.VgaImage; }
            set { this.gradientPanel.VgaImage = value; }
        }

        public ImageAlignment ImageAlignment
        {
            get { return this.gradientPanel.ImageAlignment; }
            set { this.gradientPanel.ImageAlignment = value; }
        }
        #endregion

        public PopupMenu()
        {
            InitializeComponent();

            this.concealTimer = new Timer();
            this.concealTimer.Enabled = false;
            this.concealTimer.Interval = 0;
            this.concealTimer.Tick += new EventHandler(concealTimer_Tick);
        }

        private void SetBackGroundImage(PopupMenuPlaceMode placeMode)
        {
            switch (placeMode)
            {
                case PopupMenuPlaceMode.Bottom:
                    {
                        this.Image = Resource.vniz_poloska_320_50px;
                        this.VgaImage = Resource.vniz_poloska_800_100px;
                        break;
                    }
                case PopupMenuPlaceMode.Left:
                    {
                        this.Image = Resource.vniz_poloska_320_50pxV;
                        this.VgaImage = Resource.vniz_poloska_800_100pxV;
                        break;
                    }
            }
        }

        /// <summary>
        /// Вернет ширину всех контролов в меню
        /// </summary>
        /// <returns></returns>
        public int GetWidthControls()
        {
            int result = 0;
            foreach (Control item in this.gradientPanel.Controls)
            {
                result += item.Width;
            }
            return result;
        }

        /// <summary>
        /// Вернет высоту всех контролов в меню
        /// </summary>
        /// <returns></returns>
        public int GetHeightControls()
        {
            int result = 0;
            foreach (Control item in this.gradientPanel.Controls)
            {
                result += item.Height;
            }
            return result;
        }

        public void InitOverhang(int minOverhang, int maxOverhang)
        {
            this._minOverhang = minOverhang;
            this._maxOverhang = maxOverhang;
        }

        private void SetConceal(bool value)
        {
            try
            {
                _conceal = value;
                //если отображаем меню, взведем таймер скрытия
                if (!_conceal)
                    this.RestartConcealTimer();

                switch (this.PlaceMode)
                {
                    case PopupMenuPlaceMode.Bottom:
                        {
                            this.gradientPanel.FillDirection = GradientFill.FillDirection.TopToBottom;
                            Utils.SetControlsAnchor(this.gradientPanel.Controls, AnchorStyles.Top, false);
                            /*
                            bool isDo = value ? (this.Height > this.MinOverhang) : (this.Height < this.MaxOverhang);
                            while (isDo)
                            {
                                this.Height = value ? (this.Height - this.MinOverhang) : (this.Height + this.MinOverhang);
                                Application.DoEvents();
                                isDo = value ? (this.Height > this.MinOverhang) : (this.Height < this.MaxOverhang);
                            }*/
                            this.Height = value ? this.MinOverhang : this.MaxOverhang;
                            Application.DoEvents();
                            break;
                        }
                    case PopupMenuPlaceMode.Left:
                        {
                            Utils.SetControlsAnchor(this.gradientPanel.Controls, AnchorStyles.Right, false);
                            /*bool isDo = value ? (this.Width > this.MinOverhang) : (this.Width < this.MaxOverhang);
                            while (isDo)
                            {
                                this.Width = value ? (this.Width - this.MinOverhang) : (this.Width + this.MinOverhang);
                                Application.DoEvents();
                                isDo = value ? (this.Width > this.MinOverhang) : (this.Width < this.MaxOverhang);
                            }*/
                            this.Width = value ? this.MinOverhang : this.MaxOverhang;
                            this.gradientPanel.FillDirection = GradientFill.FillDirection.LeftToRight;
                            Application.DoEvents();
                            break;
                        }
                }
                Utils.SetControlsAnchor(this.gradientPanel.Controls, AnchorStyles.None, false);
            }
            catch
            {
            }
        }

        /// <summary>
        /// Меняем положение меню
        /// </summary>
        /// <param name="spaceMode">положение меню</param>
        private void SetPlaceMode(PopupMenuPlaceMode spaceMode)
        {
            if (this.WithoutDock)
            {
                this.Dock = DockStyle.None;
            }
            else
            {
                switch (spaceMode)
                {
                    case PopupMenuPlaceMode.Bottom:
                        {
                            this.Dock = DockStyle.Bottom;
                            break;
                        }
                    case PopupMenuPlaceMode.Left:
                        {
                            this.Dock = DockStyle.Left;
                            break;
                        }
                    case PopupMenuPlaceMode.None:
                        {
                            this.Dock = DockStyle.None;
                            return;
                        }
                }
            }
            this.AdaptControls(spaceMode);
        }

        /// <summary>
        /// Адаптировать контролы расположениые в меню, к режиму расположения
        /// </summary>
        /// <param name="spaceMode">положение меню</param>
        private void AdaptControls(PopupMenuPlaceMode spaceMode)
        {
            //размер контролов
            int controlsSize = 0;
            int menuSize = (spaceMode == PopupMenuPlaceMode.Bottom) ? this.Width : this.Height;
            foreach (Control item in this.gradientPanel.Controls)
            {
                controlsSize += (spaceMode == PopupMenuPlaceMode.Bottom) ? item.Width : item.Height;
            }
            //пространстова не занятое контролами
            int allEmptySpace = menuSize - controlsSize;
            //пространство между контролами
            int inControlsSpace = (int)((float)allEmptySpace / (float)(this.gradientPanel.Controls.Count + 1));
            //не всегда получается ровное значение inControlsSpace, здесь узнаем сколько все даки 
            //нехватает для заполнения всего пустого пространства
            int excessSpace = allEmptySpace - inControlsSpace * (this.gradientPanel.Controls.Count + 1);

            int locationX = (spaceMode == PopupMenuPlaceMode.Bottom) ? inControlsSpace : 0;
            int locationY = (spaceMode == PopupMenuPlaceMode.Left) ? inControlsSpace : 0;
            List<Control> controls = this.GetControls();
            foreach (Control item in controls)
            {
                int appendExcellSpace = 0;
                if (excessSpace > 0)
                {
                    appendExcellSpace = this.CalculatAppendExcellSpace(excessSpace, this.gradientPanel.Controls.Count + 1);
                    excessSpace -= appendExcellSpace;
                }

                switch (spaceMode)
                {
                    case PopupMenuPlaceMode.Bottom:
                        {
                            item.Left = locationX;
                            //размещаем по середине
                            item.Top = (this.MaxOverhang - item.Height) / 2;
                            locationX += inControlsSpace + item.Width + appendExcellSpace;
                            break;
                        }
                    case PopupMenuPlaceMode.Left:
                        {
                            item.Top = locationY;
                            //размещаем по середине
                            item.Left = (this.MaxOverhang - item.Width) / 2;
                            locationY += inControlsSpace + item.Height + appendExcellSpace;
                            break;
                        }
                }
            }
        }

        /// <summary>
        /// Вычислим добавляемое лишние пространство
        /// </summary>
        /// <returns></returns>
        private int CalculatAppendExcellSpace(int allExcessSpace, int controlsCount)
        {
            int result = 1;
            return Math.Max(result, (int)((float)allExcessSpace / (float)controlsCount));
        }

        /// <summary>
        /// Получаем контролы в том порядке, в котором они расположены в меню
        /// </summary>
        /// <param name="spaceMode">положение меню</param>
        /// <returns>список контролов в меню</returns>
        private List<Control> GetControls()
        {
            List<Control> result = new List<Control>();

            int prepareTabIndex = -10000;
            for (int i = 0; i < this.gradientPanel.Controls.Count; i++)
            {
                int minTabIndex = 10000;
                foreach (Control item in this.gradientPanel.Controls)
                {
                    int itemTabIndex = item.TabIndex;
                    if (itemTabIndex > prepareTabIndex)
                        minTabIndex = Math.Min(itemTabIndex, minTabIndex);
                }

                prepareTabIndex = minTabIndex;
                foreach (Control item in this.gradientPanel.Controls)
                {
                    int itemTabIndex = item.TabIndex;
                    if (itemTabIndex == minTabIndex)
                        result.Add(item);
                }
            }
            return result;
        }

        protected void RestartConcealTimer()
        {
            if (this.concealTimer.Interval > 0)
            {
                this.concealTimer.Enabled = false;
                this.concealTimer.Enabled = true;
            }
        }

        private void gradientPanel_MouseDown(object sender, MouseEventArgs e)
        {
            this.Conceal = false;
        }

        void concealTimer_Tick(object sender, EventArgs e)
        {
            //как сработал таймер, скроем меню, и выключим таймер
            this.Conceal = true;
            this.concealTimer.Enabled = false;
        }
    }

    /// <summary>
    /// Режимы отображения меню
    /// </summary>
    public enum PopupMenuPlaceMode
    {
        /// <summary>
        /// Снизу
        /// </summary>
        Bottom,
        /// <summary>
        /// Слева
        /// </summary>
        Left,
        /// <summary>
        /// Без привязки
        /// </summary>
        None
    }
}
