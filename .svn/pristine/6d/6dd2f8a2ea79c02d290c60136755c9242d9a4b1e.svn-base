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
    public partial class ReportPopupMenu : PopupMenu
    {
        #region События
        public delegate void ChangeReportViewModeHandler(ReportViewMode viewMode);

        private event ChangeReportViewModeHandler _changeReportViewMode;
        private event EventHandler _refreshReportView;

        /// <summary>
        /// Происходи при переключении вида отображения отчета
        /// </summary>
        public event ChangeReportViewModeHandler ChangeReportViewMode
        {
            add { _changeReportViewMode += value; }
            remove { _changeReportViewMode -= value; }
        }

        /// <summary>
        /// Обновить текущий вид отчета
        /// </summary>
        public event EventHandler RefreshReportView
        {
            add { _refreshReportView += value; }
            remove { _refreshReportView -= value; }
        }
        #endregion

        private ReportViewMode _curReportViewMode;

        #region Свойства
        /// <summary>
        /// Текущий режим отображения отчета
        /// </summary>
        public ReportViewMode CurReportViewMode
        {
            get { return _curReportViewMode; }
            set { _curReportViewMode = value; }
        }

        public override bool Conceal
        {
            get { return base.Conceal; }
            set
            {
                base.Conceal = value;
                this.Visible = !value;
            }
        }

        public override PopupMenuPlaceMode PlaceMode
        {
            get { return base.PlaceMode; }
            set 
            { 
                this.SetMenuIcons(value);
                base.PlaceMode = value;
            }
        }
        #endregion

        public ReportPopupMenu()
        {
            InitializeComponent();
            this.WithoutDock = true;
            this.ImageAlignment = ImageAlignment.Left;
        }

        private void SetMenuIcons(PopupMenuPlaceMode placeMode)
        {
            switch (placeMode)
            {
                case PopupMenuPlaceMode.Bottom:
                    {
                        //если иконка совпадает с текущим видом отображения, то рисуем вдавленную кнопку
                        bool isPressed = this.CurReportViewMode == ReportViewMode.Original;
                        this.btOrignalView.ImageDefault = isPressed ? Resource.o50VP : Resource.o50V;
                        this.btOrignalView.ImageVgaDefault = isPressed ? Resource.o100VP : Resource.o100V;

                        isPressed = this.CurReportViewMode == ReportViewMode.Vertical;
                        this.btVerticalView.ImageDefault = isPressed ? Resource.v50VP : Resource.v50V;
                        this.btVerticalView.ImageVgaDefault = isPressed ? Resource.v100VP : Resource.v100V;

                        isPressed = this.CurReportViewMode == ReportViewMode.Horizontal;
                        this.btHorizontalView.ImageDefault = isPressed ? Resource.h50VP : Resource.h50V;
                        this.btHorizontalView.ImageVgaDefault = isPressed ? Resource.h100VP : Resource.h100V;

                        this.btRefresh.ImageDefault = Resource.reload50V;
                        this.btRefresh.ImageVgaDefault = Resource.reload100V;

                        //для нажатых кнопок
                        this.btOrignalView.ImagePressed = Resource.o50VP;
                        this.btOrignalView.ImageVgaPressed = Resource.o100VP;

                        this.btVerticalView.ImagePressed = Resource.v50VP;
                        this.btVerticalView.ImageVgaPressed = Resource.v100VP;

                        this.btHorizontalView.ImagePressed = Resource.h50VP;
                        this.btHorizontalView.ImageVgaPressed = Resource.h100VP;

                        this.btRefresh.ImagePressed = Resource.reload50VP;
                        this.btRefresh.ImageVgaPressed = Resource.reload100VP;

                        break;
                    }
                case PopupMenuPlaceMode.Left:
                    {
                        bool isPressed = this.CurReportViewMode == ReportViewMode.Original;
                        this.btOrignalView.ImageDefault = isPressed ? Resource.o50HP : Resource.o50H;
                        this.btOrignalView.ImageVgaDefault = isPressed ? Resource.o100HP : Resource.o100H;

                        isPressed = this.CurReportViewMode == ReportViewMode.Vertical;
                        this.btVerticalView.ImageDefault = isPressed ? Resource.v50HP : Resource.v50H;
                        this.btVerticalView.ImageVgaDefault = isPressed ? Resource.v100HP : Resource.v100H;

                        isPressed = this.CurReportViewMode == ReportViewMode.Horizontal;
                        this.btHorizontalView.ImageDefault = isPressed ? Resource.h50HP : Resource.h50H;
                        this.btHorizontalView.ImageVgaDefault = isPressed ? Resource.h100HP : Resource.h100H;

                        this.btRefresh.ImageDefault = Resource.reload50H;
                        this.btRefresh.ImageVgaDefault = Resource.reload100H;

                        //для нажатых кнопок
                        this.btOrignalView.ImagePressed = Resource.o50HP;
                        this.btOrignalView.ImageVgaPressed = Resource.o100HP;

                        this.btVerticalView.ImagePressed = Resource.v50HP;
                        this.btVerticalView.ImageVgaPressed = Resource.v100HP;

                        this.btHorizontalView.ImagePressed = Resource.h50HP;
                        this.btHorizontalView.ImageVgaPressed = Resource.h100HP;

                        this.btRefresh.ImagePressed = Resource.reload50HP;
                        this.btRefresh.ImageVgaPressed = Resource.reload100HP;
                        break;
                    }
            }
        }



        protected virtual void OnChangeReportViewMode(ReportViewMode viewMode)
        {
            if (this._changeReportViewMode != null)
                this._changeReportViewMode(viewMode);
        }

        protected virtual void OnRefreshReportView()
        {
            if (this._refreshReportView != null)
                this._refreshReportView(this, new EventArgs());
        }

        private void btRefresh_Click(object sender, EventArgs e)
        {
            this.OnRefreshReportView();
        }

        private void btOrignalView_Click(object sender, EventArgs e)
        {
            this.OnChangeReportViewMode(ReportViewMode.Original);
        }

        private void btVerticalView_Click(object sender, EventArgs e)
        {
            this.OnChangeReportViewMode(ReportViewMode.Vertical);
        }

        private void btHorizontalView_Click(object sender, EventArgs e)
        {
            this.OnChangeReportViewMode(ReportViewMode.Horizontal);
        }
    }
}
