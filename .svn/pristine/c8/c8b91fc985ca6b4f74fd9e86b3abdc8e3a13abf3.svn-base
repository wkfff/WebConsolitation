using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Net;
using System.Threading;
using System.Xml;

using Krista.FM.Client.MobileReports.Common;
using Krista.FM.Common.Xml;
using System.Configuration;
using Krista.FM.ServerLibrary.TemplatesService;

namespace Krista.FM.Client.MobileReports.Bootloader
{
    public class ReportsBootloader
    {
        #region Делегаты
        public delegate void StartDownloadingReportHandler(object sender, string reportID);
        public delegate void DownloadReportCompletedHandler(object sender, int downloaderReportsNumber, 
            int reportsCount, string errorText, string infoText);
        public delegate void DownloadReportsCompletedHandler(object sender, bool isSuccess);
        public delegate void StartDownloadingResourcesHandler(object sender, int resourcesCount);
        public delegate void StartResourceHandler(object sender, int numberResource);
        public delegate void EndResourceHandler(object sender, int numberResource, bool success);
        #endregion

        #region События
        private event StartDownloadingReportHandler _startDownloadingReport;
        private event DownloadReportCompletedHandler _downloadReportCompleted;
        private event DownloadReportsCompletedHandler _downloadReportsCompleted;
        private event StartDownloadingResourcesHandler _startDownloadingResources;
        private event StartResourceHandler _startResource;
        private event EndResourceHandler _endResource;

        /// <summary>
        /// Начали загрузку отчета
        /// </summary>
        public event StartDownloadingReportHandler StartDownloadingReport
        {
            add { _startDownloadingReport += value; }
            remove { _startDownloadingReport -= value; }
        }

        /// <summary>
        /// Закончили загрузку отчета
        /// </summary>
        public event DownloadReportCompletedHandler DownloadReportCompleted
        {
            add { _downloadReportCompleted += value; }
            remove { _downloadReportCompleted -= value; }
        }

        /// <summary>
        /// Закончили загрузку всех отчетов
        /// </summary>
        public event DownloadReportsCompletedHandler DownloadReportsCompleted
        {
            add { _downloadReportsCompleted += value; }
            remove { _downloadReportsCompleted -= value; }
        }

        /// <summary>
        /// Начали загружать ресурсы
        /// </summary>
        public event StartDownloadingResourcesHandler StartDownloadingResources
        {
            add { _startDownloadingResources += value; }
            remove { _startDownloadingResources -= value; }
        }

        /// <summary>
        /// Начали загружать ресурс
        /// </summary>
        public event StartResourceHandler StartResource
        {
            add { _startResource += value; }
            remove { _startResource -= value; }
        }

        /// <summary>
        /// Закончили загружать ресурс
        /// </summary>
        public event EndResourceHandler EndResource
        {
            add { _endResource += value; }
            remove { _endResource -= value; }
        }
        #endregion

        #region Поля
        //количество допустимых ошибок в процентах
        private byte _allowableErrorsProcent;
        //количество скачиваемых отчетво
        private int _reportsCount;
        //количество неудавшихся загрузок
        private int _failedDownloadsCount;
        //количество субъектов
        private int _subjectCount;
        //количество закаченных отчетов
        private int _downloaderReportsNumber;
        //нужна ли авторизация
        private bool _enabledAuthorization;
        //логин пользователя
        private string _userLogin;
        //пароль пользователя
        private string _userPassword;
        //адрес хоста отчетов
        private string _reportsHostAddress;
        //корневая папка для сохранения отчетов
        private string _dataBurstSavePath;
        //список ошибок возникших при закачке
        private StringBuilder _errorList;
        //качальщик
        private WebBootloader downloader;
        //состояние загрузчика
        private ReportsDownloaderState _downloaderState;
        //директория запуска приложения
        private string _startupPath;
        //если запущен из asp.net (IIS)
        private bool isWorkFromAsp;
        //загружаемые отчеты
        private List<ReportInfo> downloadingReports;
        //режим сохранения отчета
        private MobileReportsSnapshotMode _snapshotMode;
        #endregion

        #region Свойства
        /// <summary>
        /// Веб прокси
        /// </summary>
        public WebProxy Proxy
        {
            get { return downloader.Proxy; }
            set { downloader.Proxy = value; }
        }

        /// <summary>
        /// Новый режим сохранения отчета, когда после загрузки каждый отчет архивируется. 
        /// Так же изменяются пути сохранения отчетов. Те что генерятся по новой схеме помещаются 
        /// в NewModeSnapshot, старые соответственно в OldModeSnapshot
        /// </summary>
        public MobileReportsSnapshotMode SnapshotMode
        {
            get { return _snapshotMode; }
            set { _snapshotMode = value; }
        }

        /// <summary>
        /// Допустимый процент ошибок
        /// </summary>
        public byte AllowableErrorsProcent
        {
            get { return _allowableErrorsProcent; }
            set 
            {
                if ((value >= 0) && (value <= 100))
                    _allowableErrorsProcent = value;
            }
        }

        /// <summary>
        /// адрес хоста отчетов
        /// </summary>
        public string ReportsHostAddress
        {
            get { return _reportsHostAddress; }
            set { _reportsHostAddress = value; }
        }

        /// <summary>
        /// корневая папка для сохранения отчетов
        /// </summary>
        public string DataBurstSavePath
        {
            get { return _dataBurstSavePath; }
            set { _dataBurstSavePath = value; }
        }

        /// <summary>
        /// список ошибок возникших при закачке
        /// </summary>
        public StringBuilder ErrorList
        {
            get { return _errorList; }
            set { _errorList = value; }
        }

        /// <summary>
        /// Количесто отчетов для закачки
        /// </summary>
        public int ReportsCount
        {
            get { return _reportsCount; }
        }

        /// <summary>
        /// Количество закаченных отчетов
        /// </summary>
        public int DownloaderReportsNumber
        {
            get { return _downloaderReportsNumber; }
            set { _downloaderReportsNumber = value; }
        }

        /// <summary>
        /// Количество неудавшихся загрузок
        /// </summary>
        public int FailedDownloadsCount
        {
            get { return _failedDownloadsCount; }
        }

        /// <summary>
        /// Количество субъектов
        /// </summary>
        public int SubjectCount
        {
            get { return _subjectCount; }
        }

        /// <summary>
        /// Состояние загрузчика отчетов
        /// </summary>
        public ReportsDownloaderState DownloaderState
        {
            get { return _downloaderState; }
        }

        /// <summary>
        /// Путь к ресурсу с пользовательскими скриптами
        /// </summary>
        public string CustomScriptsPath
        {
            get { return this.downloader.CustomScriptsPath; }
            set { this.downloader.CustomScriptsPath = value; }
        }

        /// <summary>
        /// Режим загрузки скриптов
        /// </summary>
        public ScriptsDownloadType ScriptsDownloadMode
        {
            get { return this.downloader.ScriptsDownloadMode; }
            set { this.downloader.ScriptsDownloadMode = value; }
        }

        /// <summary>
        /// Оптимизировать HTML (удяляет лишние пробелы, переносы строк, и т.д.)
        /// </summary>
        public bool IsOptimazeHTML
        {
            get { return this.downloader.IsOptimazeHTML; }
            set { this.downloader.IsOptimazeHTML = value; }
        }

        /// <summary>
        /// Логин пользователя
        /// </summary>
        public string UserLogin
        {
            get { return _userLogin; }
            set { _userLogin = value; }
        }

        /// <summary>
        /// Пароль пользователя
        /// </summary>
        public string UserPassword
        {
            get { return _userPassword; }
            set { _userPassword = value; }
        }

        /// <summary>
        /// Нужна ли авторизация
        /// </summary>
        public bool EnabledAuthorization
        {
            get { return _enabledAuthorization; }
            set { _enabledAuthorization = value; }
        }

        /// <summary>
        /// Директория запуска приложения
        /// </summary>
        public string StartupPath
        {
            get { return _startupPath; }
            set { _startupPath = value; }
        }
        #endregion

        public ReportsBootloader()
            : this(String.Empty)
        {
        }

        public ReportsBootloader(string reportsHostAddress)
            : this(System.Windows.Forms.Application.StartupPath, false, reportsHostAddress)
        {
        }

        public ReportsBootloader(string startupPath, bool isWorkFromAsp, string reportsHostAddress)
        {
            this.downloader = new WebBootloader();
            this.downloadingReports = new List<ReportInfo>();
            this._downloaderState = ReportsDownloaderState.Stop;
            this.isWorkFromAsp = isWorkFromAsp;
            this.StartupPath = startupPath;

            this.ErrorList = new StringBuilder();
            //установим значения по умолчанию
            this.SetValueOnDefault();

            this.ReportsHostAddress = reportsHostAddress;
            //инициализируем обработчики событий
            this.InitHandlers();
        }

        /// <summary>
        /// Выставляем значения по умолчанию
        /// </summary>
        private void SetValueOnDefault()
        {
            //по умолчанию скрипты - загружаем
            this.ScriptsDownloadMode = ScriptsDownloadType.Custom;
            this._subjectCount = 0;
            //по умолчанию количество допустимых ошибок ставим равным 100%
            this.AllowableErrorsProcent = 100;
            //адрес хоста
            this.ReportsHostAddress = string.Empty;
            //оптимизировать HTML
            this.IsOptimazeHTML = true;

            //путь к ресурсам
            string resourcePath = Utils.GetResorcePath(this.StartupPath, this.isWorkFromAsp);
            //путь к пользовательским скриптам
            this.CustomScriptsPath = Path.Combine(resourcePath, Consts.customScriptFolderName);

            this.EnabledAuthorization = false;
            this.UserLogin = string.Empty;
            this.UserPassword = string.Empty;
        }

        private void InitHandlers()
        {
            this.downloader.DownloadCompleted += new WebBootloader.DownloadCompletedHandler(downloader_DownloadCompleted);
            this.downloader.StartResource += new WebBootloader.StartResourceHandler(downloader_StartResource);
            this.downloader.EndResource += new WebBootloader.EndResourceHandler(downloader_EndResource);
            this.downloader.StartDownloadingResources += new WebBootloader.StartDownloadingResourcesHandler(downloader_StartDownloadingResources);
        }

        public void LoadSettings()
        {
            MobileReportsSettingsSection section = (MobileReportsSettingsSection)ConfigurationManager.GetSection("webReportsSettingsSection");
            this.LoadSettings(section);
        }

        public void LoadSettings(MobileReportsSettingsSection settingsSection)
        {
            try
            {
                //загрузили адрес хоста
                if (String.IsNullOrEmpty(this.ReportsHostAddress))
                {
                    this.ReportsHostAddress = settingsSection.BootloaderReportsHostAddress.Value;
                }
                this.SnapshotMode = settingsSection.BootloaderSnapshotMode.Value;
                //папка в которую кладется пакет с данными
                string dataBurstSavePath = settingsSection.DataBurstSavePath.Value;
                if (dataBurstSavePath != string.Empty)
                {
                    dataBurstSavePath = Path.Combine(dataBurstSavePath, Consts.dataBurstName);
                    dataBurstSavePath = Path.Combine(this.StartupPath, dataBurstSavePath);
                    this.DataBurstSavePath = dataBurstSavePath;
                }
                //загрузили максимальный процент ошибок
                this.AllowableErrorsProcent = (byte)settingsSection.BootloaderAllowedErrorProcent.Value;
                //загрузили оптимизировать HTML
                this.IsOptimazeHTML = settingsSection.BootloaderIsOptimazeHTML.Value;
                //загрузили режим сохранения скриптов
                this.ScriptsDownloadMode = settingsSection.BootloaderScriptsDownloadMode.Value;
                //параметры авторизации

                this.EnabledAuthorization = settingsSection.BootloaderReportsHostAuthorization.Enabled;
                this.UserLogin = settingsSection.BootloaderReportsHostAuthorization.Login;
                this.UserPassword = settingsSection.BootloaderReportsHostAuthorization.Password;
            }
            catch (Exception e)
            {
                throw new Exception("Ошибка при загрузке настроек: " + e.Message);
            }
            finally
            {
                this.AddTraceInfo();
            }
        }

        /// <summary>
        /// Информация для трасировки
        /// </summary>
        private void AddTraceInfo()
        {
            Trace.Indent();
            Trace.TraceInformation("Адрес asp.net хоста: {0}", this.ReportsHostAddress);
            Trace.TraceInformation("Максимальный процент ошибок: {0}", this.AllowableErrorsProcent);
            Trace.TraceInformation("Оптимизировать ли HTML: {0}", this.IsOptimazeHTML);
            Trace.TraceInformation("Режим сохранения скриптов: {0}", this.ScriptsDownloadMode);
            Trace.TraceInformation("Режим генерации отчетов: {0}", this.SnapshotMode);
            Trace.TraceInformation("Включена ли авторизация: {0}", this.EnabledAuthorization);
            Trace.TraceInformation("Логин: {0}", this.UserLogin);
            Trace.Unindent();
        }

        /// <summary>
        /// Начать закачку отчетов
        /// </summary>
        /// <param name="subjectCount">количество субъектов</param>
        /// <param name="downloadingReports">список загружаемых отчетов</param>
        public bool DownloadReports(int subjectCount, List<ReportInfo> downloadingReports)
        {          
            this._subjectCount = subjectCount;
            this.downloadingReports = downloadingReports;
            this._downloaderState = ReportsDownloaderState.Downloading;
            this._failedDownloadsCount = 0;
            //узнаем количество отчетов
            this._reportsCount = this.GetReportsCount();
            this.DownloaderReportsNumber = 0;
            //очищаем лог с ошибками
            this.ErrorList.Remove(0, this.ErrorList.Length);

            if (this.IsValidStartParams() && this.CheckAuthorization())
            {
                Trace.TraceInformation("Количество отчетов требующих генерации: {0}", this.ReportsCount);
                Trace.Indent();
                return this.DownloadReports();
            }
            else
                return false;
        }

        private bool DownloadReports()
        {
            foreach (ReportInfo report in this.downloadingReports)
            {
                if (this.IsOverstepLimitErrors())
                    return false;
                else
                    this.DownloadReport(report);
            }

            this._downloaderState = ReportsDownloaderState.Stop;
            bool isSuccess = !this.IsOverstepLimitErrors();
            this.OnDownloadReportsCompleted(isSuccess);

            return isSuccess;
        }

        /// <summary>
        /// Проверка правильности логина и пароля
        /// </summary>
        /// <returns></returns>
        public bool CheckAuthorization()
        {
            return this.CheckAuthorization(this.ReportsHostAddress, this.UserLogin, this.UserPassword);
        }

        /// <summary>
        /// Проверка правильности логина и пароля
        /// </summary>
        /// <returns></returns>
        public bool CheckAuthorization(string reportsHostAddress, string login, string password)
        {
            Trace.TraceInformation("Авторизация c asp.net сайтом:");
            Trace.Indent();
            bool result = true;
            if (this.EnabledAuthorization)
            {
                result = this.downloader.Authorization(reportsHostAddress, login, password);
                if (result)
                {
                    Trace.TraceInformation("Хост: {0}, логин: {1}. Прошла успешно.", reportsHostAddress, login);
                }
                else
                {
                    string error = string.Format("Ошибка при аутентификации. Логин: {0} хост: {1}",
                        this.UserLogin, this.ReportsHostAddress);
                    this.ErrorList.Append(error);
                }
            }
            else
            {
                Trace.TraceInformation("отключена");
            }
            Trace.Unindent();
            return result;
        }
        
        /// <summary>
        /// Перед закачкой отчетов, проверяет правильность стартовых параметров
        /// </summary>
        /// <returns></returns>
        private bool IsValidStartParams()
        {
            Trace.TraceInformation("Проверка начальных параметров.");
            string errorText = string.Empty;

            if (this.SubjectCount < 0)
            {
                errorText = "Количество субъектов не может быть отрицательным";
                this.ErrorList.AppendLine(errorText);
                return false;
            }

            try
            {
                string reportsSavePath = string.Empty;
                switch (this.SnapshotMode)
                {
                    //если генерим для одного режима, просто создадим нужную папку
                    case MobileReportsSnapshotMode.New:
                    case MobileReportsSnapshotMode.Old:
                        {
                            reportsSavePath = Utils.GetReportsSavePath(this.SnapshotMode, this.DataBurstSavePath);
                            Directory.CreateDirectory(reportsSavePath);
                            break;
                        }
                    //если генерим сразу в двух режимах, создадим папки по очереди
                    case MobileReportsSnapshotMode.Both:
                        {
                            reportsSavePath = Utils.GetReportsSavePath(MobileReportsSnapshotMode.New, this.DataBurstSavePath);
                            Directory.CreateDirectory(reportsSavePath);

                            reportsSavePath = Utils.GetReportsSavePath(MobileReportsSnapshotMode.Old, this.DataBurstSavePath);
                            Directory.CreateDirectory(reportsSavePath);
                            break;
                        }
                }
            }
            catch (Exception e)
            {
                errorText = "Неудалось создать директорию под отчеты. Ошибка: " + e.Message;
                this.ErrorList.AppendLine(errorText);
                return false;
            }

            if ((this.ScriptsDownloadMode == ScriptsDownloadType.Custom) &&
                !Directory.Exists(this.CustomScriptsPath))
            {
                errorText = "В режиме сохранения отчетов с пользовательскими скриптами, " +
                    "должен существовать ресурс с ними. Проверте правильность пути к файлу: " + this.CustomScriptsPath;
                this.ErrorList.AppendLine(errorText);
                return false;
            }

            Trace.TraceInformation("Начальные параметры успешно проверены.");
            return true;
        }

        public void SuspendDownloadingReports()
        {
            if (this.DownloaderState == ReportsDownloaderState.Suspend)
            {
                this._downloaderState = ReportsDownloaderState.Downloading;
            }
            else
            {
                this._downloaderState = ReportsDownloaderState.Suspend;
            }
        }

        public void StopDownloadingReports()
        {
            this._downloaderState = ReportsDownloaderState.Stop;
        }

        /// <summary>
        /// Получаем общее количество отчетов
        /// </summary>
        /// <returns></returns>
        private int GetReportsCount()
        {
            int result = 0;
            foreach (ReportInfo report in this.downloadingReports)
            {
                if (report.SubjectDepended)
                    result += this.SubjectCount * report.ViewCount;
                else
                    result += report.ViewCount;
            }

            return result;
        }

        /// <summary>
        /// Проверяет не зашкалила ли количество ошибок, за предельно допустимое, выставленное пользователем
        /// </summary>
        /// <returns>если зашкалило вернет true</returns>
        private bool IsOverstepLimitErrors()
        {
            if ((this.AllowableErrorsProcent == 100) || (this.ReportsCount == 0))
                return false;
            return ((float)this.FailedDownloadsCount / (float)this.ReportsCount) * 100 > this.AllowableErrorsProcent;
        }

        /// <summary>
        /// Генерим виды для указаного отчета
        /// </summary>
        /// <param name="report"></param>
        private void DownloadReport(ReportInfo report)
        {

            //т.к. вида у отчета три, будем генерить каждый отдельно
            for (int i = 0; i < report.ViewCount; i++)
            {
                if (report.SubjectDepended)
                {
                    for (int subIndex = 1; subIndex <= this.SubjectCount; subIndex++)
                    {
                        if (this.IsOverstepLimitErrors())
                            return;
                        else
                            this.DownloadReportView(report,(ReportViewMode)i, subIndex);
                    }
                }
                else
                {
                    if (!this.IsOverstepLimitErrors())
                        this.DownloadReportView(report, (ReportViewMode)i, -1);
                }
            }
        }

        /// <summary>
        /// Генерим указанный вид отчета
        /// </summary>
        /// <param name="reportID">id отчета</param>
        /// <param name="reportViewID">id вида отчета</param>
        /// <param name="subjectIndex"></param>
        private void DownloadReportView(ReportInfo report, ReportViewMode viewMode, int subjectIndex)
        {
            string url = string.Empty;
            string reportFolder = string.Empty;
            string reportViewID = report.GetReportCode(viewMode);
            //id отчета учитивая номер субъекта (если отчет субъектно зависимый)
            string lastReportViewID = reportViewID;
            //составляем url, и путь сохранения отчета
            if (subjectIndex > 0)
            {
                url = string.Format("{0}?reportID={1}&subjectID={2}", this.ReportsHostAddress, reportViewID,
                    subjectIndex);
                lastReportViewID = string.Format("{0}_{1}", reportViewID, subjectIndex);
            }
            else
            {
                url = string.Format("{0}?reportID={1}", this.ReportsHostAddress, reportViewID);
            }

            //Возбуждаем событие о начале загрузке отчета
            this.OnStartDownloadingReport(this.GetDownloadingReportID(reportViewID, subjectIndex));

            switch (this.SnapshotMode)
            {
                case MobileReportsSnapshotMode.New:
                case MobileReportsSnapshotMode.Old:
                    {
                        this.SaveToHTML(report, url, reportViewID, lastReportViewID, this.SnapshotMode);
                        break;
                    }
                case MobileReportsSnapshotMode.Both:
                    {
                        this.SaveToHTML(report, url, reportViewID, lastReportViewID, MobileReportsSnapshotMode.Old);
                        this.SecondarySaveToHTML(report, this.downloader.HtmlBodyAbsoluteUrl, this.downloader.SourceHtmlBody, 
                            reportViewID, lastReportViewID, MobileReportsSnapshotMode.New);
                        break;
                    }
            }

            this.DownloaderReportsNumber++;

            //Если превышен лимит ошибок, прекращаем загрузку
            if (this.IsOverstepLimitErrors())
            {
                this.ErrorList.AppendLine("Загрузка отчетов остановленна, т.к. превышен допустимый процент ошибок.");
                this.ErrorList.AppendLine(string.Format("Допустимый процент: {0}%, количество отчетов загруженных с ошибкой: {1}, всего отчетов: {2}.",
                    this.AllowableErrorsProcent, this.FailedDownloadsCount, this.ReportsCount));
                this._downloaderState = ReportsDownloaderState.Stop;
                this.OnDownloadReportsCompleted(false);
            }
        }

        private void SaveToHTML(ReportInfo report, string url, string reportViewID, string lastReportViewID,
            MobileReportsSnapshotMode snapshotMode)
        {
            string reportsPath = Utils.GetReportsSavePath(snapshotMode, this.DataBurstSavePath);
            string reportPath = string.Format("{0}\\{1}\\{2}\\{3}\\", reportsPath, report.Parent.Code,
                report.Code, reportViewID);
            string extensionString = this.GetExtensionString(snapshotMode);
            bool isNewSnapshotMode = (snapshotMode == MobileReportsSnapshotMode.New);
            if (isNewSnapshotMode)
                //в новом режиме, потребуется дополнительная папка с именем и id отчета,
                //которая пропадет при архивации
                reportPath += string.Format("{0}\\{0}.{1}", lastReportViewID, extensionString);
            else
                reportPath += string.Format("{0}.{1}", lastReportViewID, extensionString);

            this.downloader.SaveToHTML(url, reportPath, reportViewID, report.TemplateType, isNewSnapshotMode);
        }

        private void SecondarySaveToHTML(ReportInfo report, Uri reportUrl, string reportBody, string reportViewID, string lastReportViewID,
            MobileReportsSnapshotMode snapshotMode)
        {
            string reportsPath = Utils.GetReportsSavePath(snapshotMode, this.DataBurstSavePath);
            string reportPath = string.Format("{0}\\{1}\\{2}\\{3}\\", reportsPath, report.Parent.Code,
                report.Code, reportViewID);
            string extensionString = this.GetExtensionString(snapshotMode);
            bool isNewSnapshotMode = (snapshotMode == MobileReportsSnapshotMode.New);
            if (isNewSnapshotMode)
                //в новом режиме, потребуется дополнительная папка с именем и id отчета,
                //которая пропадет при архивации
                reportPath += string.Format("{0}\\{0}.{1}", lastReportViewID, extensionString);
            else
                reportPath += string.Format("{0}.{1}", lastReportViewID, extensionString);

            this.downloader.SaveToHTML(reportUrl, reportBody, reportPath, reportViewID, report.TemplateType, isNewSnapshotMode);
        }

        /// <summary>
        /// Вернет id закачивающегося в данный момент отчета
        /// </summary>
        /// <returns>id отчета</returns>
        private string GetDownloadingReportID(string reportViewID, int subjectIndex)
        {
            string result = reportViewID;
            if (subjectIndex > 0)
            {
                result = string.Format("{0}_{1}", result, subjectIndex);
            }
            return result;
        }

        /// <summary>
        /// Строка с расширением
        /// </summary>
        public string GetExtensionString(MobileReportsSnapshotMode mode)
        {
            return (mode == MobileReportsSnapshotMode.New) ? "html" : "php";
        }



        /// <summary>
        /// Узнаем количество ресурсов в отчете
        /// </summary>
        void downloader_StartDownloadingResources(object sender, int resourcesCount)
        {
            this.OnStartResourcesDownloading(resourcesCount);
        }

        /// <summary>
        /// Закончиалась загрузка отчета
        /// </summary>
        void downloader_DownloadCompleted(object sender, string errorText, string infoText)
        {
            if (errorText != string.Empty)
            {
                this._failedDownloadsCount++;
                //ява скрипт тоже использует скобки, что бы не спутать их со скобками для форматирвания 
                //строк, закоментим...
                errorText = errorText.Replace("{", "{{");
                errorText = errorText.Replace("}", "}}");
                this.ErrorList.AppendLine(errorText);
            }
            this.OnDownloadReportCompleted(errorText, infoText);
        }

        /// <summary>
        /// Началась загрузка ресурса
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="numberResource"></param>
        void downloader_StartResource(object sender, int numberResource)
        {
            this.OnStartResource(numberResource);
        }

        /// <summary>
        /// Закончили загружать ресурс
        /// </summary>
        void downloader_EndResource(object sender, int numberResource, bool success)
        {
            this.OnEndResource(numberResource, success);
        }

        private void OnStartDownloadingReport(string reportID)
        {
            Trace.TraceInformation("Генерим отчет: {0} {1} из {2}", reportID, this.DownloaderReportsNumber,
                this.ReportsCount);
            if (_startDownloadingReport != null)
                _startDownloadingReport(this, reportID);
        }

        private void OnDownloadReportCompleted(string errorText, string infoText)
        {
            if (_downloadReportCompleted != null)
                _downloadReportCompleted(this, this.DownloaderReportsNumber + 1, this.ReportsCount, 
                    errorText.ToString(), infoText.ToString());
        }

        private void OnDownloadReportsCompleted(bool isSuccess)
        {
            Trace.Unindent();
            Trace.TraceInformation("Загрузка отчетов завершена. Статус: {0}", isSuccess);
            if (_downloadReportsCompleted != null)
                _downloadReportsCompleted(this, isSuccess);
        }

        private void OnStartResourcesDownloading(int resourcesCount)
        {
            if (_startDownloadingResources != null)
                _startDownloadingResources(this, resourcesCount);
        }

        private void OnStartResource(int resourceNumber)
        {
            if (_startResource != null)
                _startResource(this, resourceNumber);
        }

        private void OnEndResource(int resourceNumber, bool success)
        {
            if (_endResource != null)
                _endResource(this, resourceNumber, success);
        }
    }

    /// <summary>
    /// Тип отчета
    /// </summary>
    enum ReportType
    {
        SubjectDepedence,
        NotSubjectDependence
    }

    /// <summary>
    /// Состояние загрузчика отчетов
    /// </summary>
    public enum ReportsDownloaderState
    {
        Downloading,
        Suspend,
        Stop
    }
}