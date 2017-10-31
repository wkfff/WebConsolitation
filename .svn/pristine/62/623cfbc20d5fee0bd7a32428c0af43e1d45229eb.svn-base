using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Runtime.InteropServices;

using Krista.FM.Client.iMonitoringWM.Common;
using System.IO;

namespace iMonotoringWM
{
    public partial class Report : UserControl
    {
        const int emptyImageIndex = -1;
        const int grayImageIndex = 0;
        const int redImageIndex = 1;

        #region События
        private event EventHandler _сlickСustomClickBounds;

        /// <summary>
        /// Нажатие на пользовательскую область
        /// </summary>
        public event EventHandler ClickCustomClickBounds
        {
            add { _сlickСustomClickBounds += value; }
            remove { _сlickСustomClickBounds -= value; }
        }
        #endregion

        #region Поля
        //нажата ли пользовательская область
        private bool isClickCustomClickBounds;
        private Rectangle _customClickBounds;

        private string _id;
        private string _caption;
        private bool _isSubjectDependent;
        private int _position;

        private DateTime _deployDate;

        private ReportView _currentView;
        private ReportView _originalView;
        private ReportView _horizontalView;
        private ReportView _verticalView;

        private ReportCollection _collection;
        #endregion

        #region Свойства
        /// <summary>
        /// Область указываемая пользоватлем, при нажатии на которую 
        /// будет происходить событие ClickCustomClickBounds - немного тафтологии
        /// </summary>
        public Rectangle CustomClickBounds
        {
            get { return _customClickBounds; }
            set { _customClickBounds = value; }
        }

        /// <summary>
        /// Id
        /// </summary>
        public string Id
        {
            get { return _id; }
            set { _id = value; }
        }

        /// <summary>
        /// Заголовок
        /// </summary>
        public string Caption
        {
            get { return _caption; }
            set { _caption = value; }
        }

        /// <summary>
        /// Признак субъектно зависимости отчета
        /// </summary>
        public bool IsSubjectDependent
        {
            get { return _isSubjectDependent; }
            set { _isSubjectDependent = value; }
        }

        /// <summary>
        /// Дата выкладывания отчета на сервер
        /// </summary>
        public DateTime DeployDate
        {
            get { return _deployDate; }
            set 
            {
                //База данных не принимает года 0001, поэтому минимальный выставим 1990
                if ((value != null) && (value.Year < 1990))
                    value = value.AddYears(1990 - value.Year);
                _deployDate = value; 
            }
        }

        /// <summary>
        /// Текущий вид отображения
        /// </summary>
        public ReportView CurrentView
        {
            get { return _currentView; }
            set { _currentView = value; }
        }

        /// <summary>
        /// Оригинальный вид
        /// </summary>
        public ReportView OriginalView
        {
            get { return _originalView; }
        }

        /// <summary>
        /// Горизонтальный вид
        /// </summary>
        public ReportView HorizontalView
        {
            get { return _horizontalView; }
        }

        /// <summary>
        /// Вертикальный вид
        /// </summary>
        public ReportView VerticalView
        {
            get { return _verticalView; }
        }

        /// <summary>
        /// Позиция отчета в общем списке
        /// </summary>
        public int Position
        {
            get { return _position; }
            set { _position = value; }
        }

        /// <summary>
        /// Инициализированны ли визуальные компоненты отчета
        /// </summary>
        public bool IsInitializatedVisualComponent
        {
            get { return this.webPanel != null; }
        }

        /// <summary>
        /// Коллекция отчетов
        /// </summary>
        public ReportCollection Collection
        {
            get { return _collection; }
            set { _collection = value; }
        }

        /// <summary>
        /// Происходи ли в данный момент загрузка любого из вида отчета
        /// </summary>
        public bool IsDownloading
        {
            get
            {
                return this.Collection.IsExistsInDownloadingQueoe(this.OriginalView) ||
                    this.Collection.IsExistsInDownloadingQueoe(this.VerticalView) ||
                    this.Collection.IsExistsInDownloadingQueoe(this.HorizontalView);
            }
        }

        /// <summary>
        /// Вернет список брусков для текущего разрешения
        /// </summary>
        private ImageList CurrentImageBarList
        {
            get
            {
                return (Utils.ScreenSize == ScreenSizeMode.s240x320) ? 
                    this.imageBarListFor240x320 : this.imageBarListFor480x640;
            }
        }

        /// <summary>
        /// Вернет список треугольников для текущего разрешения
        /// </summary>
        private ImageList CurrentImageAngleList
        {
            get
            {
                return (Utils.ScreenSize == ScreenSizeMode.s240x320) ?
                    this.imageAngleListFor240x320 : this.imageAngleListFor480x640;
            }
        }
        #endregion

        public Report()
        {
            this.InitStartParam();
        }

        public Report(XmlNode reportNode)
        {
            this.InitStartParam();

            this.Id = XmlHelper.GetStrXmlAttribute(reportNode, "id", string.Empty);
            this.Caption = XmlHelper.GetStrXmlAttribute(reportNode, "name", string.Empty);
            this.IsSubjectDependent = XmlHelper.GetBoolXmlAttribute(reportNode, "subjectDependence", false);
            this.DeployDate = XmlHelper.GetDateTimeXmlAttribute(reportNode, "updateDate", DateTime.Now);
        }

        public Report(string id, string name, DateTime deployDate, bool subjectDepended, 
            bool visible, int position)
        {
            this.InitStartParam();

            this.Id = id;
            this.Caption = name;
            this.IsSubjectDependent = subjectDepended;
            this.DeployDate = deployDate;
            this.Visible = visible;
            this.Position = position;

            this.InitReportsView();
            this.SetHadlers();
        }

        private void InitStartParam()
        {
            this.Id = string.Empty;
            this.Caption = string.Empty;
            this.IsSubjectDependent = false;
            this.DeployDate = DateTime.MinValue;
            this.Visible = true;
            this.Position = -1;

            this.CustomClickBounds = Rectangle.Empty;
        }

        private void InitReportsView()
        {
            this._originalView = new ReportView(this, ReportViewMode.Original);
            this._horizontalView = new ReportView(this, ReportViewMode.Horizontal);
            this._verticalView = new ReportView(this, ReportViewMode.Vertical);
            this.CurrentView = this.OriginalView;
        }

        private void SetHadlers()
        {
            this.OriginalView.ReportViewCompleted += new ReportView.ReportViewCompleteHandler(ReportViewCompleted);
            this.VerticalView.ReportViewCompleted += new ReportView.ReportViewCompleteHandler(ReportViewCompleted);
            this.HorizontalView.ReportViewCompleted += new ReportView.ReportViewCompleteHandler(ReportViewCompleted);
        }

        /// <summary>
        /// Инициализация визуальных компонент отчета
        /// </summary>
        public void InitVisualComponent()
        {
            this.InitializeComponent();
            //Заголовок отчета
            this.pCaption.TextDefault = this.Caption;
        }

        /// <summary>
        /// Сохранить настройки отчета в БД
        /// </summary>
        public void WriteSettingsToDB()
        {
            MainForm.Instance.DbHelper.WriteReportSettings(MainForm.Instance.UserSettings.Name, this);
        }

        public ReportView GetReportView(ReportViewMode viewMode)
        {
            switch (viewMode)
            {
                case ReportViewMode.Original: return this.OriginalView;
                case ReportViewMode.Horizontal: return this.HorizontalView;
                case ReportViewMode.Vertical: return this.VerticalView;
            }
            return this.OriginalView;
        }

        /// <summary>
        /// Устанавливает все видам отчета указаное состояние
        /// </summary>
        /// <param name="state"></param>
        private void SetViewState(ReportViewState state)
        {
            this.OriginalView.ViewState = state;
            this.HorizontalView.ViewState = state;
            this.VerticalView.ViewState = state;
        }

        /// <summary>
        /// Сбросить дату обновления
        /// </summary>
        private void ResetDownloadDate()
        {
            this.OriginalView.DownloadDate = DateTime.MinValue;
            this.HorizontalView.DownloadDate = DateTime.MinValue;
            this.VerticalView.DownloadDate = DateTime.MinValue;
        }

        /// <summary>
        /// Устанавливает видимость веббраузеров в отчетах
        /// </summary>
        /// <param name="value"></param>
        private void SetWebBrowsersVisible(bool value)
        {
            if (this.OriginalView.WebBrowser != null)
                this.OriginalView.WebBrowser.Visible = value;
            if (this.HorizontalView.WebBrowser != null)
                this.HorizontalView.WebBrowser.Visible = value;
            if (this.VerticalView.WebBrowser != null)
                this.VerticalView.WebBrowser.Visible = value;
        }

        /// <summary>
        /// После выбора субъекта, надо сбросить время обновления и состояние всех видов 
        /// отчетов в NeedUpdate
        /// </summary>
        public void ChangedEntity()
        {
            if (this.IsSubjectDependent)
            {
                this.SetViewState(ReportViewState.NeedUpdate);
                this.ResetDownloadDate();
                this.SetWebBrowsersVisible(false);
            }
        }

        /// <summary>
        /// Обновить указанный вид отчета
        /// </summary>
        /// <param name="viewMode"></param>
        public void Refresh(ReportViewMode viewMode)
        {
            ReportView refreshingView = this.GetReportView(viewMode);
            refreshingView.ViewState = refreshingView.ViewState == ReportViewState.LoadedOfWeb ?
                ReportViewState.LoadedOfCache : ReportViewState.NeedUpdate;
            refreshingView.DownloadDate = DateTime.MinValue;

            this.Collection.ReportChanged(this, viewMode, false);
        }

        public void Changing(ReportViewMode viewMode)
        {
            switch (viewMode)
            {
                case ReportViewMode.Original:
                    {
                        this.HorizontalView.Visible = false;
                        this.VerticalView.Visible = false;
                        this.pCaption.Visible = true;

                        this.CurrentView = this.OriginalView;
                        break;
                    }
                case ReportViewMode.Horizontal:
                    {
                        this.OriginalView.Visible = false;
                        this.VerticalView.Visible = false;
                        this.pCaption.Visible = false;

                        this.CurrentView = this.HorizontalView;
                        break;
                    }
                case ReportViewMode.Vertical:
                    {
                        this.OriginalView.Visible = false;
                        this.HorizontalView.Visible = false;
                        this.pCaption.Visible = true;

                        this.CurrentView = this.VerticalView;
                        break;
                    }
            }
            this.ForceResizeWebPanel();
            this.CurrentView.Changing();
            this.InitIndications();
        }

        /// <summary>
        /// Устанавливаем индикацию отчета, если проиходит загрузка хотя бы одного из видов, будем крутить 
        /// шестеренки
        /// </summary>
        public void InitIndications()
        {
            this.ProgressIndicator.AlignByCentr();
            this.ProgressIndicator.Visible = this.IsDownloading;

            this.InitDownloadedIndicators();
        }

        /// <summary>
        /// Инициализируем индикаторы загрузки
        /// </summary>
        private void InitDownloadedIndicators()
        {
            this.lbInfoText.Visible = false;
            switch (this.CurrentView.ViewState)
            {
                case ReportViewState.FailedLoad:
                    {
                        this.SetIndicatorsImage(redImageIndex);
                        //Выводим сообщение о том что отчета нет в кэше
                        this.lbInfoText.Text = Consts.msMissingReport;
                        this.lbInfoText.Visible = true;
                        Utils.AlignByCentr(this.lbInfoText, this.webPanel);
                        break;
                    }
                case ReportViewState.LoadedOfCacheAndFailedLoadWeb:
                    {
                        this.SetIndicatorsImage(redImageIndex);
                        break;
                    }
                case ReportViewState.LoadedOfCache:
                    {
                        if (MainForm.Instance.WorkMode == WorkMode.Cache)
                            this.SetIndicatorsImage(grayImageIndex);
                        else
                            this.SetIndicatorsImage(emptyImageIndex);
                        break;
                    }
                case ReportViewState.LoadedOfWeb:
                    {
                        this.SetIndicatorsImage(emptyImageIndex);
                        break;
                    }
            }
        }

        private void SetIndicatorsImage(int imageIndex)
        {
            this.pbIndicator.Visible = false;
            this.pbTopIndicator.Visible = false;

            if (imageIndex > emptyImageIndex)
            {
                if (this.CurrentView.ViewMode == ReportViewMode.Horizontal)
                {
                    this.pbIndicator.Image = this.CurrentImageAngleList.Images[imageIndex];
                    this.pbIndicator.Visible = true;
                }
                else
                {
                    this.pbTopIndicator.Image = this.CurrentImageBarList.Images[imageIndex];
                    this.pbTopIndicator.Visible = true;
                }
            }
        }

        private void ForceResizeWebPanel()
        {
            this.webPanel.Top = this.pCaption.Visible ? this.pCaption.Height : 0;
            this.webPanel.Size = new Size(this.Width, this.Height - this.webPanel.Top);
        }

        protected virtual void OnClickCustomClickBounds()
        {
            if (this._сlickСustomClickBounds != null)
            {
                //При нажатии на пользовательскую область, если событие пришло от браузера
                //происходит его залипание, и сролирование вних, что бы избежать этого,
                //времено приостановим интерактивность 
                //this.CurrentView.WebBrowser.IsInteractive = false;
                this._сlickСustomClickBounds(this, new EventArgs());
            }
        }

        public new void OnMouseDown(MouseEventArgs e)
        {
            if ((this.CustomClickBounds != Rectangle.Empty) && 
                this.CustomClickBounds.Contains(this.PointToClient(new Point(e.X, e.Y))))
            {
                this.OnClickCustomClickBounds();
                this.isClickCustomClickBounds = true;
            }
            else
                base.OnMouseDown(e);
        }

        public new void OnMouseMove(MouseEventArgs e)
        {
            if (!this.isClickCustomClickBounds)
                base.OnMouseMove(e);
        }

        public new void OnMouseUp(MouseEventArgs e)
        {
            if (!this.isClickCustomClickBounds)
                base.OnMouseUp(e);
            this.isClickCustomClickBounds = false;
        }

        #region С этих контролов тоже будем отправлять события для скролинга
        private void webPanel_MouseDown(object sender, MouseEventArgs e)
        {
            this.OnMouseDown(e);
        }

        private void webPanel_MouseMove(object sender, MouseEventArgs e)
        {
            this.OnMouseMove(e);
        }

        private void webPanel_MouseUp(object sender, MouseEventArgs e)
        {
            this.OnMouseUp(e);
        }

        private void pCaption_MouseDown(object sender, MouseEventArgs e)
        {
            this.OnMouseDown(e);
        }

        private void pCaption_MouseMove(object sender, MouseEventArgs e)
        {
            this.OnMouseMove(e);
        }

        private void pCaption_MouseUp(object sender, MouseEventArgs e)
        {
            this.OnMouseUp(e);
        }
        #endregion

        #region Обработчики
        void ReportViewCompleted(object sender, ReportViewMode viewMode, ReportViewState viewState)
        {
            this.InitIndications();
        }
        #endregion
    }
}
