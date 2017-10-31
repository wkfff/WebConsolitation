using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.IO;

using Krista.FM.Client.iMonitoringWM.Common;
using Krista.FM.Client.iMonitoringWM.Controls;
using System.Drawing;
using System.Windows.Forms;
using System.Threading;
using Microsoft.Win32;


namespace iMonotoringWM
{
    /// <summary>
    /// Вид отчета
    /// </summary>
    public class ReportView
    {
        #region Делегаты
        public delegate void ReportViewCompleteHandler(object sender, ReportViewMode viewMode,
            ReportViewState viewState);
        #endregion

        #region События
        private event ReportViewCompleteHandler _reportViewCompleted;
        /// <summary>
        /// Происходит при окончании загрузки отчета
        /// </summary>
        public event ReportViewCompleteHandler ReportViewCompleted
        {
            add { _reportViewCompleted += value; }
            remove { _reportViewCompleted -= value; }
        }
        #endregion

        #region Поля
        private Report _report;
        private ReportViewMode _viewMode;
        private ReportViewState _viewState;
        private DateTime _downloadDate;
        private string _id;

        private WMBrowser _webBrowser;
        #endregion

        #region Свойства
        /// <summary>
        /// Вид отображения
        /// </summary>
        public ReportViewMode ViewMode
        {
            get { return this._viewMode; }
        }
        
        /// <summary>
        /// Состояние вида отчета
        /// </summary>
        public ReportViewState ViewState
        {
            get { return _viewState; }
            set { _viewState = value; }
        }

        /// <summary>
        /// Дата загрузки вида отчета в кэш
        /// </summary>
        public DateTime DownloadDate
        {
            get { return this.GetDownloadDate(); }
            set { this.SetDownloadDate(value); }
        }

        /// <summary>
        /// Инициализирован ли браузер
        /// </summary>
        public bool IsInitWebBrowser
        {
            get { return (this.WebBrowser != null); }
        }

        /// <summary>
        /// Путь отчета в кэше
        /// </summary>
        public string CachePath
        {
            get { return this.GetCachePath(); }
        }

        /*public string DownloaderPath
        {
            get { return this.GetDownloaderPath(true); }
        }*/

        /// <summary>
        /// Uri для запроса вида отчета
        /// </summary>
        public string Uri
        {
            get { return this.GetUri(); }
        }

        /// <summary>
        /// Id вида отчета (отличается от id отчета, приписанием к ID признака вида)
        /// </summary>
        public string Id
        {
            get { return _id; }
        }

        /// <summary>
        /// Браузер, в котором отображаем данные отчета
        /// </summary>
        public WMBrowser WebBrowser
        {
            get { return _webBrowser; }
            set { _webBrowser = value; }
        }

        public bool Visible
        {
            get { return this.IsInitWebBrowser ? this.WebBrowser.Visible: false; }
            set 
            {
                if (this.IsInitWebBrowser)
                    this.WebBrowser.Visible = value;
            }
        }

        public Report Report
        {
            get { return this._report; }
        }
        #endregion

        public ReportView(Report report, ReportViewMode viewMode)
        {
            this._report = report;
            this._viewMode = viewMode;

            //Значения по умолчанию
            this._downloadDate = DateTime.MinValue;
            this._viewState = ReportViewState.NeedUpdate;
            this._id = this.AddViewModeStr(this._report.Id);
        }

        /// <summary>
        /// Выбор вида отчета
        /// </summary>
        /// <param name="isInteractiveWebrowser"></param>
        public void Changing()
        {
            //Если браузер до сих пор не инициализированн, сделаем это
            if (!this.IsInitWebBrowser)
                this.InitWebBrowser();

            this.SetBrowserInteractive();

            switch (this.ViewState)
            {
                case ReportViewState.NeedUpdate:
                    {
                        this.LoadOfCache();
                        break;
                    }
                case ReportViewState.LoadedOfCache:
                case ReportViewState.LoadedOfWeb:
                case ReportViewState.LoadedOfCacheAndFailedLoadWeb:
                    {
                        if (!this.Visible)
                            this.Visible = true;
                        break;
                    }
            }
        }

        /// <summary>
        /// Загрузка отчета из кэша
        /// </summary>
        private void LoadOfCache()
        {
            if (File.Exists(this.CachePath))
            {
                try
                {
                    this.WebBrowser.Visible = false;
                    this.WebBrowser.Url = @"file://" + this.CachePath;
                }
                catch
                {
                }
                //Если естьсоединение с сервером и дата в кэше актуальна, то будем считать, что 
                //загрузили из инета, если нет то просто из кэша
                 this.ViewState = (MainForm.Instance.IsConnetctedToHost && this.IsActualDownloadDate()) ?
                    ReportViewState.LoadedOfWeb : ReportViewState.LoadedOfCache;
            }
            else
            {
                this.ViewState = MainForm.Instance.IsConnetctedToHost ? ReportViewState.NeedUpdate : ReportViewState.FailedLoad;
            }
        }

        /// <summary>
        /// Окончание загрузки отчета в кэш из интернета
        /// </summary>
        /// <param name="errorText">текст ошибки</param>
        /// <param name="InfoText">информация о процессе загрузки</param>
        public void DownloadComplete(string errorText, string InfoText, bool isSuccess)
        {
            if (isSuccess)
            {
                this.DownloadDate = DateTime.Now;
                this.LoadOfCache();
            }
            else
            {
                //Если пользователь сам прервал загрузку отчета, то это не будет щитаться ошибкой
                if (InfoText == Consts.msAbortDownloading)
                {
                    this.ViewState = ReportViewState.NeedUpdate;
                }
                else
                {
                    //Если отчет загрузился с ошибкой, но до этого была загружена версия из кэша
                    //выставим соответствующие состояние, иначе просто ошибка при загрузке
                    this.ViewState = (this.ViewState == ReportViewState.LoadedOfCache) ?
                        ReportViewState.LoadedOfCacheAndFailedLoadWeb : ReportViewState.FailedLoad;
                }
                this.OnReportViewCompleted();
            }
        }

        /// <summary>
        /// Вернет урл для данного вида
        /// </summary>
        /// <returns></returns>
        private string GetUri()
        {
            //страница с которой получаем ссылку на закачку отчетов
            string result = Utils.CombineUri(MainForm.Instance.Host, Consts.getReportPage);
            //добавляем атрибуты, сначала id отчета, с учетом режима отображения
            result += "?reportID=" + this.AddViewModeStr(this._report.Id);
            //если требуется добавим id субъекта
            if (this._report.IsSubjectDependent)
                result += "&subjectID=" + MainForm.Instance.UserSettings.EntityIndex.ToString();
            return result;
        }

        /// <summary>
        /// Вернет путь для кэша
        /// </summary>
        /// <returns></returns>
        private string GetCachePath()
        {
            //id с признаком режима отображения
            string idWithViewMode = this.AddViewModeStr(this._report.Id);
            //id отчета с номером субъекта (если нужен)
            string idWithSubjectIndex = this.AddSybjectIndexStr(idWithViewMode);
            //id с расширением
            string idWithExtension = idWithSubjectIndex + ".html";
            //путь отчета внутри кэша
            string reportPath = string.Format("{0}\\{1}\\{2}", this._report.Id, idWithViewMode, idWithExtension);
            //возвращаем путь отчета, вместе с путем кэша
            return Path.Combine(MainForm.Instance.ReportsPath, reportPath);
        }

        /// <summary>
        /// Добавляет к value строки с признаком режима отображения
        /// </summary>
        /// <param name="value">к чему добавлять</param>
        /// <returns>value с признаком режими отображения</returns>
        private string AddViewModeStr(string value)
        {
            string result = value;
            switch (this._viewMode)
            {
                case ReportViewMode.Horizontal: result += "_H"; break;
                case ReportViewMode.Vertical: result += "_V"; break;
            }
            return result;
        }

        /// <summary>
        /// Если отчет субъектно зависимый, добавляет к value номер субъекта
        /// </summary>
        /// <param name="value">к чему добавлять</param>
        /// <returns>value c дописанным номером субъекта</returns>
        private string AddSybjectIndexStr(string value)
        {
            string result = value;
            if (this._report.IsSubjectDependent)
                result += string.Format("_{0}", MainForm.Instance.UserSettings.EntityIndex);
            return result;
        }

        public void InitWebBrowser()
        {
            //создадим браузер
            this.WebBrowser = new WMBrowser();
            this.WebBrowser.Parent = this.Report.webPanel;
            //устанвливаем размеры
            this.WebBrowser.Bounds = new Rectangle(0, 0, this.Report.webPanel.Width, this.Report.webPanel.Height);
            this.WebBrowser.DocumentCompleted += new Old.OpenNETCF.Windows.Forms.WebBrowserDocumentCompletedEventHandler(WebBrowser_DocumentCompleted);
            this.WebBrowser.Visible = false;
            this.WebBrowser.BackColor = Color.Black;

            this.WebBrowser.EnableClearType = true;
            this.WebBrowser.EnableShrink = true;
            this.WebBrowser.EnableScripting = true;
            Application.DoEvents();

            this.InitHandlers();
        }

        /// <summary>
        /// Актуальная ли версия отчета в кэше
        /// </summary>
        /// <returns>актуальна ли</returns>
        public bool IsActualDownloadDate()
        {
            DateTime downloadDate = this.DownloadDate;
            return (MainForm.Instance.WorkMode == WorkMode.Cache) ||
                ((downloadDate != DateTime.MinValue) && (downloadDate >= this.Report.DeployDate));
        }

        /// <summary>
        /// Получаем составной id в который входит сам id, признак вида отчета, и опционально индекс субъекта
        /// </summary>
        /// <param name="isIncludeSubjIndex">включать ли в составной id индекс субъекта</param>
        /// <returns>составной id</returns>
        public string GetCompositeId(bool isIncludeSubjIndex)
        {
            string result = string.Format("{0}{1}{2}", this._report.Id, Consts.dbSeparator, this._viewMode);
            //если отчет субъектно зависимый, дописываем еще и номер субъекта
            if (isIncludeSubjIndex && this._report.IsSubjectDependent)
                result += Consts.dbSeparator + MainForm.Instance.UserSettings.EntityIndex.ToString();
            return result;
        }

        /// <summary>
        /// Дата загрузки отчета (если она еще не ицианализирована, ищем в базе данных)
        /// </summary>
        /// <returns>дата загрузки</returns>
        private DateTime GetDownloadDate()
        {
            DateTime result = this._downloadDate;
            if (result == DateTime.MinValue)
                result = MainForm.Instance.DbHelper.GetReportDownloadDate(this);
            return result;
        }

        /// <summary>
        /// Если изменилась дата загрузки (обновили), запишем ее в базу данных
        /// </summary>
        /// <param name="value">дата загрузки</param>
        private void SetDownloadDate(DateTime value)
        {
            if (this._downloadDate != value)
            {
                this._downloadDate = value;
                if (this._downloadDate != DateTime.MinValue)
                    MainForm.Instance.DbHelper.SetReportDownloadDate(this);
            }
        }

        /// <summary>
        /// Будем дублировать события с панели над браузером, базовому контролу
        /// </summary>
        private void InitHandlers()
        {
            this.WebBrowser.MouseDownEX += new MouseEventHandler(WebBrowser_MouseDown);
            this.WebBrowser.MouseMoveEX += new MouseEventHandler(WebBrowser_MouseMove);
            this.WebBrowser.MouseUpEX += new MouseEventHandler(WebBrowser_MouseUp);
        }

        private void SetBrowserInteractive()
        {
            //Сейчас делаем не интерактивными все виды отчетов, т.к. в контекстном меню нет ничего, что 
            //могло бы пригодится пользователю, а навредить и изменить внешний вид отчета можно...
            this.WebBrowser.IsInteractive = false;//(this.ViewMode != ReportViewMode.Original);
        }

        protected virtual void OnReportViewCompleted()
        {
            if (this._reportViewCompleted != null)
                this._reportViewCompleted(this, this.ViewMode, this.ViewState);
        }

        #region Обработчики
        void WebBrowser_MouseDown(object sender, MouseEventArgs e)
        {
            Point screenPoint = this.WebBrowser.PointToScreen(new Point(e.X, e.Y));
            MouseEventArgs mouseArgs = new MouseEventArgs(e.Button, 1, screenPoint.X, screenPoint.Y, 0);
            this._report.OnMouseDown(mouseArgs);
        }

        void WebBrowser_MouseMove(object sender, MouseEventArgs e)
        {
            this._report.OnMouseMove(e);
        }

        void WebBrowser_MouseUp(object sender, MouseEventArgs e)
        {
            this.SetBrowserInteractive();
            this._report.OnMouseUp(e);
        }

        void WebBrowser_DocumentCompleted(object sender, Old.OpenNETCF.Windows.Forms.WebBrowserDocumentCompletedEventArgs e)
        {
            if (this._report.CurrentView == this)
            {
                this.Visible = true;
                this.OnReportViewCompleted();
            }
        }
        #endregion
    }
}
