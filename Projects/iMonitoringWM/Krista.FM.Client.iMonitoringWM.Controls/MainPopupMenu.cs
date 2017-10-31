using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Krista.FM.Client.iMonitoringWM.Common;
using Krista.FM.Client.iMonitoringWM.Controls.Properties;

namespace Krista.FM.Client.iMonitoringWM.Controls
{
    public partial class MainPopupMenu : PopupMenu
    {
        #region Делегаты
        public delegate void CloseApplicationHandler(bool isMinimize);
        #endregion

        #region События
        private event CloseApplicationHandler _closeApplication;
        private event EventHandler _showSettingsView;
        private event EventHandler _showAbout;

        /// <summary>
        /// Происходи при нажатии кнопки закрыть приложение
        /// </summary>
        public event CloseApplicationHandler СloseApplication
        {
            add { _closeApplication += value; }
            remove { _closeApplication -= value; }
        }

        /// <summary>
        /// Происходит при нажатии кнопки отображения настроек приложения
        /// </summary>
        public event EventHandler ShowSettingsView
        {
            add { _showSettingsView += value; }
            remove { _showSettingsView -= value; }
        }

        /// <summary>
        /// Происходит при нажатии кнопки "О программе"
        /// </summary>
        public event EventHandler ShowAbout
        {
            add { _showAbout += value; }
            remove { _showAbout -= value; }
        }
        #endregion

        #region Поля
        private ReportPopupMenu _reportMenu;
        #endregion

        #region Свойства
        public ReportPopupMenu ReportMenu
        {
            get { return _reportMenu; }
            set { _reportMenu = value; }
        }

        public override PopupMenuPlaceMode PlaceMode
        {
            get
            {
                return base.PlaceMode;
            }
            set
            {
                this.SetMenuIcons(value);
                this.SetImageAlignment(value);
                base.PlaceMode = value;
            }
        }

        public override bool Conceal
        {
            get
            {
                return (Utils.ScreenSize == ScreenSizeMode.s480x800) ? false : base.Conceal;
            }
            set
            {
                //если скрываем основное меню, скроем и меню отчета
                if (value)
                    this.ReportMenu.Conceal = true;
                base.Conceal = value;
            }
        }
        #endregion

        public MainPopupMenu()
        {
            InitializeComponent();
        }

        private void SetMenuIcons(PopupMenuPlaceMode placeMode)
        {
            switch (placeMode)
            {
                case PopupMenuPlaceMode.Bottom:
                    {
                        this.btViewMode.ImageDefault = Resource.eye50V;
                        this.btViewMode.ImageVgaDefault = Resource.eye100V;

                        this.btShowSettingsView.ImageDefault = Resource.settings50V;
                        this.btShowSettingsView.ImageVgaDefault = Resource.settings100V;

                        this.btAbout.ImageDefault = Resource.i50V;
                        this.btAbout.ImageVgaDefault = Resource.i100V;

                        this.btClose.ImageDefault = Resource.off50V;
                        this.btClose.ImageVgaDefault = Resource.off100V;

                        //Для нажатых кнопок
                        this.btViewMode.ImagePressed = Resource.eye50VP;
                        this.btViewMode.ImageVgaPressed = Resource.eye100VP;

                        this.btShowSettingsView.ImagePressed = Resource.settings50VP;
                        this.btShowSettingsView.ImageVgaPressed = Resource.settings100VP;

                        this.btAbout.ImagePressed = Resource.i50VP;
                        this.btAbout.ImageVgaPressed = Resource.i100VP;

                        this.btClose.ImagePressed = Resource.off50VP;
                        this.btClose.ImageVgaPressed = Resource.off100VP;
                        break;
                    }
                case PopupMenuPlaceMode.Left:
                    {
                        this.btViewMode.ImageDefault = Resource.eye50H;
                        this.btViewMode.ImageVgaDefault = Resource.eye100H;

                        this.btShowSettingsView.ImageDefault = Resource.settings50H;
                        this.btShowSettingsView.ImageVgaDefault = Resource.settings100H;

                        this.btAbout.ImageDefault = Resource.i50H;
                        this.btAbout.ImageVgaDefault = Resource.i100H;

                        this.btClose.ImageDefault = Resource.off50H;
                        this.btClose.ImageVgaDefault = Resource.off100H;

                        //Для нажатых кнопок
                        this.btViewMode.ImagePressed = Resource.eye50HP;
                        this.btViewMode.ImageVgaPressed = Resource.eye100HP;

                        this.btShowSettingsView.ImagePressed = Resource.settings50HP;
                        this.btShowSettingsView.ImageVgaPressed = Resource.settings100HP;

                        this.btAbout.ImagePressed = Resource.i50HP;
                        this.btAbout.ImageVgaPressed = Resource.i100HP;

                        this.btClose.ImagePressed = Resource.off50HP;
                        this.btClose.ImageVgaPressed = Resource.off100HP;
                        break;
                    }
            }
        }

        /// <summary>
        /// Установим выравнивание фона
        /// </summary>
        /// <param name="placeMode"></param>
        private void SetImageAlignment(PopupMenuPlaceMode placeMode)
        {
            switch (placeMode)
            {
                case PopupMenuPlaceMode.Bottom:
                    {
                        this.ImageAlignment = ImageAlignment.Left;
                        break;
                    }
                case PopupMenuPlaceMode.Left:
                    {
                        this.ImageAlignment = ImageAlignment.Right;
                        break;
                    }
            }
        }

        protected virtual void OnCloseApplication(bool isMinimize)
        {
            if (this._closeApplication != null)
                this._closeApplication(isMinimize);
        }

        protected virtual void OnShowSettingsView()
        {
            if (this._showSettingsView != null)
                this._showSettingsView(this, new EventArgs());
        }

        protected virtual void OnShowAbout()
        {
            if (this._showAbout != null)
                this._showAbout(this, new EventArgs());
        }

        #region Обработчики
        //Событие нажатие Click у кнопок сделано с ошибкой, т.к. если даже нажали не на кнопку, 
        //но отпустили над ней, срабатывает клик - что не должно быть. Поэтому запоминаем на какую
        //кнопку нажали и какую отпустили, и если это одна и таже, то делаем что хотели.

        private bool btViewDown = false;
        private bool btSettingsDown = false;
        private bool btAboutDown = false;
        private bool btCloseDown = false;

        private void btViewMode_MouseDown(object sender, MouseEventArgs e)
        {
            btViewDown = true;
        }

        private void btShowSettingsView_MouseDown(object sender, MouseEventArgs e)
        {
            btSettingsDown = true;
        }

        private void btAbout_MouseDown(object sender, MouseEventArgs e)
        {
            btAboutDown = true;
        }

        private void btClose_MouseDown(object sender, MouseEventArgs e)
        {
            btCloseDown = true;
        }

        private void btViewMode_MouseUp(object sender, MouseEventArgs e)
        {
            if (btViewDown)
            {
                btViewDown = false;
                if (!this.Conceal)
                {
                    this.RestartConcealTimer();
                    this.ReportMenu.Conceal = !this.ReportMenu.Conceal;
                }
                else
                    this.Conceal = !this.Conceal;
            }
        }

        private void btShowSettingsView_MouseUp(object sender, MouseEventArgs e)
        {
            if (btSettingsDown)
            {
                btSettingsDown = false;

                if (!this.Conceal)
                    this.OnShowSettingsView();
                this.Conceal = !this.Conceal;
            }
        }

        private void btAbout_MouseUp(object sender, MouseEventArgs e)
        {
            if (btAboutDown)
            {
                btAboutDown = false;

                if (!this.Conceal)
                    this.OnShowAbout();
                this.Conceal = !this.Conceal;
            }
        }

        private void btClose_MouseUp(object sender, MouseEventArgs e)
        {
            if (btCloseDown)
            {
                btCloseDown = false;

                if (!this.Conceal)
                    this.OnCloseApplication(false);
                else
                    this.Conceal = !this.Conceal;
            }
        }
        #endregion
    }
}
