using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Xml;
using System.IO;
using Microsoft.WindowsCE.Forms;

using Krista.FM.Client.iMonitoringWM.Downloader;
using Krista.FM.Client.iMonitoringWM.Common;
using Krista.FM.Client.iMonitoringWM.Controls;
using Krista.FM.Client.iMonitoringWM.Common.Cryptography;
using OpenNETCF.Windows.Forms;

namespace iMonotoringWM
{
    public partial class MainForm : Form
    {
        #region Поля
        static private MainForm _instance;

        private WebDownloader _downloader;
        private bool _isConnetctedToHost;
        private ReportCollection _reports;
        private WorkMode _workMode;
        private string _host;
        private string _startupPath;
        private MainDbHelper _dbHelper;
        private UserSettings _userSettings;
        private ReportViewMode prepareViewMode;
        private ReportViewMode _currentViewMode;
        private SettingsView _settingsView;
        private AboutView _aboutView;
        private bool _isClosingApplication;
        //выбранный элемент, и его индекс
        private Report changedReport;
        private int changedReportIndex;
        //происходит разворот экрана, иницированный пользователем
        private bool isUserScreenRotaion;
        private Timer rotateScreenTimer;
        private CachePlace cachePlace;
        private KeyboardHook _keyboard;
        private int preparePressButtonKey;
        #endregion

        #region Свойства
        /// <summary>
        /// Экземпляр класса
        /// </summary>
        public static MainForm Instance
        {
            get { return MainForm._instance; }
            set { MainForm._instance = value; }
        }

        /// <summary>
        /// Есть ли соединение с сервером
        /// </summary>
        public bool IsConnetctedToHost
        {
            get { return _isConnetctedToHost; }
            set { _isConnetctedToHost = value; }
        }

        /// <summary>
        /// Загрузчик
        /// </summary>
        public WebDownloader Downloader
        {
            get { return _downloader; }
            set { _downloader = value; }
        }

        /// <summary>
        /// Коллекция отчетов
        /// </summary>
        public ReportCollection Reports
        {
            get { return _reports; }
            set { _reports = value; }
        }

        /// <summary>
        /// Режим работы приложения
        /// </summary>
        public WorkMode WorkMode
        {
            get { return _workMode; }
            set { _workMode = value; }
        }

        /// <summary>
        /// Хост, к которому подключаемся
        /// </summary>
        public string Host
        {
            get { return _host; }
            set { _host = value; }
        }

        /// <summary>
        /// Стартовый путь приложения
        /// </summary>
        public string StartupPath
        {
            get { return _startupPath; }
            set { _startupPath = value; }
        }

        /// <summary>
        /// Путь к кэшу
        /// </summary>
        public string CachePath
        {
            get { return Utils.GetCachePath(this.StartupPath, this.cachePlace); }
        }

        /// <summary>
        /// Папка с отчетами в кэше
        /// </summary>
        public string ReportsPath
        {
            get { return Path.Combine(this.CachePath, Consts.reportsFolderName); }
        }

        /// <summary>
        /// Вспомогательный класс для общения с базой данной
        /// </summary>
        public MainDbHelper DbHelper
        {
            get { return _dbHelper; }
            set { _dbHelper = value; }
        }

        /// <summary>
        /// Инкапсулированы настройки пользователя
        /// </summary>
        public UserSettings UserSettings
        {
            get { return _userSettings; }
            set { _userSettings = value; }
        }

        /// <summary>
        /// Текущий режим просмотра отчетов
        /// </summary>
        public ReportViewMode CurrentViewMode
        {
            get { return _currentViewMode; }
            set { this.SetCurrentViewMode(value); }
        }

        /// <summary>
        /// Настройки приложения
        /// </summary>
        public SettingsView SettingsView
        {
            get { return _settingsView; }
            set { _settingsView = value; }
        }

        /// <summary>
        /// О программе
        /// </summary>
        public AboutView AboutView
        {
            get { return _aboutView; }
            set { _aboutView = value; }
        }

        /// <summary>
        /// Признак того, что приложение заверщает работу
        /// </summary>
        public bool IsClosingApplication
        {
            get { return _isClosingApplication; }
        }

        /// <summary>
        /// Вернет, текущий отображаемый на ленте, отчет
        /// </summary>
        public Report CurrentChangedReport
        {
            get
            {
                Control result = this.scrollView.ChangedControl;
                if (result == null)
                    return null;
                else
                    return (Report)this.scrollView.ChangedControl;
            }
        }

        /// <summary>
        /// Отображается ли в данное время отчет (или фокус на настройках или форме "о программе")
        /// </summary>
        public bool IsReportsShowing
        {
            get { return !this.SettingsView.Visible && !this.AboutView.Visible; }
        }

        public KeyboardHook Keyboard
        {
            get { return _keyboard; }
            set { _keyboard = value; }
        }

        #endregion
        

        public MainForm()
        {
            this.InitializeComponent();

            this.SetDefaultValue();
            this.InitSystemObject();
            this.SetHandlers();

            this.LoadSettings();
            this.EnableProgressIndicators(this.WorkMode == WorkMode.Online);

            if (this.WorkMode == WorkMode.Online)
            {
                //Даем команду загрузить данные с сервера, после загрузки сработает 
                //событие DownloadServerDataComplete
                this.Downloader.AsynchGetServerData(this.Host, this.UserSettings.Name,
                    this.UserSettings.Password);
            }
            else //если работаем с кэшем сразу будем загружать отчеты пользователя
            {
                this.InitUserReports();
            }
        }

        /// <summary>
        /// Проверяем авторизацию пользователя
        /// </summary>
        /// <returns></returns>
        private bool CheckUserAuthentication()
        {
            //Если пользователь до сих пор не авторизован, значит не удалось подключится к серверу
            //или стоит автономный режим
            if (!this.UserSettings.IsAuthentication)
            {
                //посмотрим, может такой пользователь подключался раньше
                UserSettings user = this.DbHelper.GetUserSettings(this.UserSettings.Name);
                if (user != null)
                {
                    //если подключался, и пароли совпадают, то будем считать что авторизован
                    this.UserSettings.IsAuthentication = (user.Password == this.UserSettings.Password);
                    this.UserSettings.LastConnection = DateTime.Now;
                }
            }
            return this.UserSettings.IsAuthentication;
        }

        /// <summary>
        /// Устанавливем свойствам значения по умолчанию
        /// </summary>
        private void SetDefaultValue()
        {
            this.CurrentViewMode = ReportViewMode.Original;
            //поумолчанию работаем онлайн
            this.WorkMode = WorkMode.Online;
            //поумолчанию кэш на карте памяти
            this.cachePlace = CachePlace.storageCard;
            this.IsConnetctedToHost = false;

            this.StartupPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);
            this.scrollView.Enabled = true;
            //определяем высоту индикатора
            this.scrollView.ScrollIndicator.Height = (Utils.ScreenSize == ScreenSizeMode.s240x320) ? 10 : 20;

            this.popupMenu.ReportMenu = this.popupReportMenu;
            this.popupReportMenu.CurReportViewMode = this.CurrentViewMode;
            this.InitPopupMenuOverhang();
            this.SetControlsSize();

            bool isSizeMode480x800 = (Utils.ScreenSize == ScreenSizeMode.s480x800);
            this.popupMenu.Conceal = !isSizeMode480x800;

            this.WindowState = isSizeMode480x800 ? FormWindowState.Normal : FormWindowState.Maximized;
            this.ControlBox = isSizeMode480x800;
        }

        /// <summary>
        /// Устанавливаем события
        /// </summary>
        private void SetHandlers()
        {
            this.scrollView.BeforeControlChanged += new ScrollView.ControlChangedHandler(scrollView_BeforeControlChanged);
            this.scrollView.AfterControlChanged += new ScrollView.ControlChangedHandler(scrollView_AfterControlChanged);
            this.scrollView.ScrollIndicatorClick += new EventHandler(scrollView_ScrollIndicatorClick);

            this.popupReportMenu.ChangeReportViewMode += new Krista.FM.Client.iMonitoringWM.Controls.ReportPopupMenu.ChangeReportViewModeHandler(reportPopupMenu_ChangeReportViewMode);
            this.popupReportMenu.RefreshReportView += new EventHandler(reportPopupMenu_RefreshReportView);

            this.popupMenu.СloseApplication += new Krista.FM.Client.iMonitoringWM.Controls.MainPopupMenu.CloseApplicationHandler(popupMenu_СloseApplication);
            this.popupMenu.ShowSettingsView += new EventHandler(popupMenu_ShowSettingsView);
            this.popupMenu.ShowAbout += new EventHandler(popupMenu_ShowAbout);

            this.SettingsView.ApplySettings += new EventHandler(SettingsView_ApplySettings);
            this.AboutView.CloseAboutView += new EventHandler(AboutView_CloseAboutView);

            this.Downloader.ConnectToHostComplete += new WebDownloader.ConnectToHostCompleteHandler(Downloader_ConnectToHostComplete);
            this.Downloader.DownloadServerDataComplete += new WebDownloader.DownloadServerDataCompleteHandler(Downloader_DownloadServerDataComplete);

            this.rotateScreenTimer.Tick += new EventHandler(rotateScreenTimer_Tick);

            this.Keyboard.KeyDetected += new KeyHookEventHandler(Keyboard_KeyDetected);
        }

        /// <summary>
        /// Инициализация системных объектов
        /// </summary>
        private void InitSystemObject()
        {
            Instance = this;
            //Инициализируем дата бейз помошника 
            this.DbHelper = new MainDbHelper(this.StartupPath);
            this.UserSettings = new UserSettings();
            this.Downloader = new WebDownloader();
            this.Reports = new ReportCollection(this.Downloader);
            this.SettingsView = new SettingsView();
            this.SettingsView.Visible = false;
            this.AboutView = new AboutView();
            this.AboutView.Visible = false;

            //this.hardwareNotification = new HWButtonsNotification();
            this.rotateScreenTimer = new Timer();
            this.Keyboard = new KeyboardHook();
            this.Keyboard.Enabled = true;
        }

        /// <summary>
        /// Загружаем настройки пользователя и приложение
        /// </summary>
        private void LoadSettings()
        {
            this.Host = this.DbHelper.GetStrValue("host", Consts.defaultHost);
            this.UserSettings.Name = this.DbHelper.GetStrValue("login", Consts.defaultLogin);
            this.UserSettings.Password = this.DbHelper.GetStrValue("password",
                CryptUtils.GetPasswordHash(Consts.defaultPassword));
            this.UserSettings.EntityIndex = this.DbHelper.GetUserSubjectIndex(this.UserSettings.Name);

            this.GetCachePlace();

            bool isOfflineOperation = this.DbHelper.GetBoolValue("offLineOperation",
                Consts.defaultOffLineOperation);
            this.WorkMode = isOfflineOperation ? WorkMode.Cache : WorkMode.Online;
        }

        /// <summary>
        /// Инициализация размеров выноса всплывающего меню
        /// </summary>
        private void InitPopupMenuOverhang()
        {
            switch (Utils.ScreenSize)
            {
                case ScreenSizeMode.s240x320:
                    {
                        this.popupReportMenu.InitOverhang(1, 50);
                        this.popupMenu.InitOverhang(10, 50);
                        this.popupMenu.Conceal = true;
                        break;
                    }
                case ScreenSizeMode.s480x640:
                    {
                        this.popupReportMenu.InitOverhang(1, 100);
                        this.popupMenu.InitOverhang(20, 100);
                        this.popupMenu.Conceal = true;
                        break;
                    }
                case ScreenSizeMode.s480x800:
                    {
                        this.popupReportMenu.InitOverhang(1, 100);
                        this.popupMenu.InitOverhang(100, 100);
                        this.popupMenu.Conceal = false;
                        break;
                    }
            }
        }

        /// <summary>
        /// В зависимость от вида отчета устанавливает ориентацию экрана
        /// </summary>
        /// <param name="value"></param>
        private void SetCurrentViewMode(ReportViewMode value)
        {
            if (!this.scrollView.Empty)
            {
                this.prepareViewMode = _currentViewMode;
                _currentViewMode = value;
                this.popupReportMenu.CurReportViewMode = value;

                switch (_currentViewMode)
                {
                    case ReportViewMode.Original:
                        {
                            if (this.isUserScreenRotaion ||
                                (SystemSettings.ScreenOrientation != ScreenOrientation.Angle0))
                                this.ShowBlackPanel();

                            this.scrollView.EnabledScroll = true;
                            if (!this.isUserScreenRotaion)
                                SystemSettings.ScreenOrientation = ScreenOrientation.Angle0;
                            break;
                        }
                    case ReportViewMode.Vertical:
                        {
                            if (this.isUserScreenRotaion ||
                                (SystemSettings.ScreenOrientation != ScreenOrientation.Angle0))
                                this.ShowBlackPanel();

                            this.scrollView.EnabledScroll = false;
                            if (!this.isUserScreenRotaion)
                                SystemSettings.ScreenOrientation = ScreenOrientation.Angle0;
                            break;
                        }
                    case ReportViewMode.Horizontal:
                        {
                            if (this.isUserScreenRotaion ||
                                (SystemSettings.ScreenOrientation != ScreenOrientation.Angle270))
                                this.ShowBlackPanel();

                            this.scrollView.EnabledScroll = false;
                            if (!this.isUserScreenRotaion)
                                SystemSettings.ScreenOrientation = ScreenOrientation.Angle270;
                            break;
                        }
                }

                this.SetControlsSize();
                this.ChangeReport(this.CurrentChangedReport);
                this.dummyPanel.Visible = false;
                this.popupMenu.Conceal = true;
            }
        }

        /// <summary>
        /// В зависимости от разрешения и ориентации экрана, выставляем контролам нужный размеры
        /// </summary>
        private void SetControlsSize()
        {
            Rectangle screenBounds = Utils.GetScreenBounds();
            int screenWidth = screenBounds.Width;
            int screenHeight = screenBounds.Height;
            this.popupMenu.PlaceMode = PopupMenuPlaceMode.None;

            switch (SystemSettings.ScreenOrientation)
            {
                case ScreenOrientation.Angle0:
                case ScreenOrientation.Angle180:
                    {
                        bool isSizeMode480x800 = Utils.ScreenSize == ScreenSizeMode.s480x800;
                        this.WindowState = isSizeMode480x800 ? FormWindowState.Normal : FormWindowState.Maximized;

                        this.mainControlsPanel.Bounds = new Rectangle(0, 0, screenWidth,
                            screenHeight - this.popupMenu.MinOverhang);

                        this.popupMenu.Width = screenWidth;
                        this.popupMenu.Height = this.popupMenu.MaxOverhang;
                        this.popupMenu.PlaceMode = PopupMenuPlaceMode.Bottom;

                        this.SetSizeReportPopupMenu(PopupMenuPlaceMode.Bottom);
                        break;
                    }
                case ScreenOrientation.Angle90:
                case ScreenOrientation.Angle270:
                    {
                        this.WindowState = FormWindowState.Maximized;
                        this.mainControlsPanel.Bounds = new Rectangle(this.popupMenu.MinOverhang, 0,
                            screenWidth - this.popupMenu.MinOverhang, screenHeight);

                        this.popupMenu.Width = this.popupMenu.MaxOverhang;
                        this.popupMenu.Height = screenHeight;
                        this.popupMenu.PlaceMode = PopupMenuPlaceMode.Left;

                        this.SetSizeReportPopupMenu(PopupMenuPlaceMode.Left);
                        break;
                    }
            }
        }

        /// <summary>
        /// Т.к. бывают сложности точно нажать на выезжающие меню, при Вертикальных и Горизонтальных 
        /// видах отчета будем на отчете отводить специальную область, при нажатии на которую будет 
        /// появлятся меню
        /// </summary>
        /// <param name="report"></param>
        private void SetCustomClickBounds(Report report)
        {
            if (report != null)
            {
                report.ClickCustomClickBounds -= report_ClickCustomClickBounds;
                //Потребность в такое области есть только на Вертикальных и Горизонтальных видах
                switch (report.CurrentView.ViewMode)
                {
                    case ReportViewMode.Original:
                        {
                            report.CustomClickBounds = Rectangle.Empty;
                            break;
                        }
                    case ReportViewMode.Vertical:
                        {
                            report.ClickCustomClickBounds += new EventHandler(report_ClickCustomClickBounds);
                            report.CustomClickBounds = new Rectangle(0, report.Height - this.popupMenu.MinOverhang,
                                report.Width, this.popupMenu.MinOverhang);
                            break;
                        }
                    case ReportViewMode.Horizontal:
                        {
                            report.ClickCustomClickBounds += new EventHandler(report_ClickCustomClickBounds);
                            report.CustomClickBounds = new Rectangle(0, 0,
                                this.popupMenu.MinOverhang + 10, report.Height);
                            break;
                        }
                }
            }
        }

        private void SetSizeReportPopupMenu(PopupMenuPlaceMode placeMode)
        {
            Rectangle screenBounds = Utils.GetScreenBounds();
            int screenWidth = screenBounds.Width;
            int screenHeight = screenBounds.Height;

            int x = 0;
            int y = 0;
            switch (placeMode)
            {
                case PopupMenuPlaceMode.Bottom:
                    {
                        this.popupReportMenu.Height = this.popupReportMenu.MaxOverhang;
                        this.popupReportMenu.Width = screenWidth;
                        y = this.Height - this.popupMenu.Height - this.popupReportMenu.Height;
                        break;
                    }
                case PopupMenuPlaceMode.Left:
                    {
                        this.popupReportMenu.Height = screenHeight;
                        this.popupReportMenu.Width = this.popupMenu.MaxOverhang;
                        x = this.popupMenu.Width;
                        break;
                    }
            }
            this.popupReportMenu.PlaceMode = placeMode;
            this.popupReportMenu.Location = new Point(x, y);
        }

        /// <summary>
        /// Если требуется подгоним размеры отчета под текущий экран
        /// </summary>
        private void ResizeReport(Report report)
        {
            if ((report != null) && (report.Size != this.scrollView.WorkingArea))
            {
                report.Size = this.scrollView.WorkingArea;
                this.scrollView.ResizeControls();
                this.scrollView.AlignByControl(report);
            }
        }

        private void GetCachePlace()
        {
            string cahePlaceStr = this.DbHelper.GetStrValue("cachePlace", string.Empty);
            if (cahePlaceStr == string.Empty)
                cahePlaceStr = this.cachePlace.ToString();
            else
            {
                if ((cahePlaceStr == CachePlace.storageCard.ToString()) && !Utils.IsExistStorageCard())
                {
                    Messages.ShowError("Не найдена карта памяти", "Ошибка при доступе к кэшу");
                    cahePlaceStr = CachePlace.storagePhone.ToString();
                }
            }
            this.cachePlace = (CachePlace)Enum.Parse(typeof(CachePlace), cahePlaceStr, true);
        }

        /// <summary>
        /// Инициализация формы
        /// </summary>
        private void InitForm()
        {
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
        }

        /// <summary>
        /// Разбираем данные полученные с сервера
        /// </summary>
        private void ParseServerData(XmlDocument serverData)
        {
            this.UserSettings.IsAuthentication = this.IsUserAuthentication(serverData);
            //Если пользователь авторизован, обновим его настройки в базе данных
            if (this.UserSettings.IsAuthentication)
            {
                //Выставим признак, что соеденины с сервером
                this.IsConnetctedToHost = true;
                this.DbHelper.UpdateUserSettings(this.UserSettings, serverData);
                //загрузим доступные ему отчеты
                this.InitUserReports();
            }
            else
            {
                Messages.ShowError(this.GetDataInfo(serverData), "Информация с сервера");
            }
        }

        /// <summary>
        /// Авторизовался ли пользователь
        /// </summary>
        /// <param name="serverData">данные с сервера</param>
        /// <returns></returns>
        private bool IsUserAuthentication(XmlDocument serverData)
        {
            XmlNode authenticationNode = serverData.SelectSingleNode("iMonitoring/isAuthentication");
            if (authenticationNode == null)
                return false;
            else
                return bool.Parse(authenticationNode.Attributes["value"].Value);
        }

        /// <summary>
        /// Получает значение из информационного тега
        /// </summary>
        /// <param name="serverData">данные с сервера</param>
        /// <returns></returns>
        private string GetDataInfo(XmlDocument serverData)
        {
            XmlNode dataInfo = serverData.SelectSingleNode("iMonitoring/info");
            if (dataInfo == null)
                return "Информация отсутствует";
            else
                return dataInfo.Attributes["text"].Value;
        }

        /// <summary>
        /// Отобразим отчеты пользователя
        /// </summary>
        private void InitUserReports()
        {
            if (this.CheckUserAuthentication())
            {
                this.lbProgressIndicator.Visible = false;

                //Инициализируем коллекцию отчетов
                this.InitReportsCollection();
                this.InitScrollView(this.Reports.Items);
            }
            else
            {
                Messages.ShowError(Consts.msFaultAuthentication, "Ошибка доступа");
                this.SetProgressIndicatorText(Consts.msFaultAuthentication, true);
            }
        }

        /// <summary>
        /// Загрузим из базы данных отчеты доступные пользователю
        /// </summary>
        private void InitReportsCollection()
        {
            this.DbHelper.InitUserReports(this.UserSettings.Name, this.Reports);
        }

        /// <summary>
        /// Инициализируем скролл вью
        /// </summary>
        /// <param name="reportList"></param>
        private void InitScrollView(List<Report> reportList)
        {
            this.scrollView.Invoke(new EventHandler(delegate { this.scrollView.Clear(); }));
            this.scrollView.Clear();

            for (int i = 0; i < reportList.Count; i++)
            {
                Report report = this.Reports.ReportByPosition(i);
                if (report.Visible)
                {
                    if (!report.IsInitializatedVisualComponent)
                        report.InitVisualComponent();
                    this.scrollView.AddControl(report, false);
                }
            }
            //пересчитаем координаты добавленных контролов
            this.scrollView.ResizeControls();
            //если отчеты есть
            if (this.scrollView.Count > 0)
            {
                //Сначала попробуем отыскать в заного проинициализированной ленте, ранее 
                //выбранный отчет
                if ((this.changedReport != null) && this.scrollView.RibbonControls.Contains(this.changedReport))
                    this.scrollView.ChangedControl = this.changedReport;
                else
                {
                    //если отчет выключили, тогда будем ориентироваться на его индекс
                    this.scrollView.ChangedControlIndex = Math.Min(this.changedReportIndex,
                        this.scrollView.Count - 1);
                }
            }
        }

        private void ProcessServerData(XmlDocument serverData, string errorText)
        {
            this.EnableProgressIndicators(errorText != string.Empty);
            this.progressIndicator.Visible = false;

            //если данные получены без ошибок, выполняем их обработку
            if ((errorText == string.Empty) && (serverData != null))
            {
                this.ParseServerData(serverData);
            }
            //иначе выдаем сообщение об ошибке, и пытаемся загрузить что есть в кэше
            else
            {
                Messages.ShowError(Consts.msFaultConnectToServ, "Ошибка");
                this.InitUserReports();
            }
        }

        /// <summary>
        /// При изменении ориентации устройства, контролы начинают "дергаться" подстраиваться
        /// под новое разрешение, на это время будем просто вызывать эту функцию, и позывать
        /// черный экран
        /// </summary>
        private void ShowBlackPanel()
        {
            this.dummyPanel.Visible = true;
            this.dummyPanel.Dock = DockStyle.Fill;
            Application.DoEvents();
        }

        /// <summary>
        /// По отчету, получаем элемент для контрола управляющего порядком следования и 
        /// видимостью отчетов в настройках
        /// </summary>
        /// <param name="report">отчет</param>
        /// <returns>скролируемый элемент</returns>
        private ScrollListItem GetScrollListItem(Report report)
        {
            ScrollListItem result = new ScrollListItem();
            result.IsEditMode = true;
            int heightItem = Utils.ScreenSize == ScreenSizeMode.s240x320 ? 30 : 60;
            result.Size = new Size(this.Width, heightItem);
            result.BackColor = Color.Black;
            result.ForeColor = Color.LightGray;
            result.Text = report.Caption;
            result.IsChanged = report.Visible;
            result.Tag = report;

            return result;
        }

        /// <summary>
        /// Отобразим форму с настройками приложения
        /// </summary>
        private void ShowSettingsView()
        {
            if (!this.SettingsView.IsInitializedVisualComponent)
                this.SettingsView.InitVisualComponent();

            //упорядоченый по возрастанию позиции, список отчетов
            List<Report> orderedReportList = this.Reports.GetReportsOrderedByAscendPosition();
            //список для контрола управляющего порядком следования, и видимостью отчетов
            List<ScrollListItem> scrollReportList = new List<ScrollListItem>();
            foreach (Report report in orderedReportList)
            {
                scrollReportList.Add(this.GetScrollListItem(report));
            }
            this.SettingsView.InitSettings(scrollReportList, this.UserSettings.EntityIndex);
            //Если список субъектов еще не инициализирован, получим его
            if (this.SettingsView.EntityList.Count == 0)
                this.SettingsView.EntityList = this.DbHelper.GetEntityList();
           
            this.SettingsView.Dock = DockStyle.Fill;
            this.SettingsView.Visible = true;
            this.SettingsView.Parent = this;
            this.grandPanel.SendToBack();

            //если разрешение 480х800 то на время показа настроек вместо крестика
            //(сворачивания приложения) отображаем "ok", что бы можно было закрыть настройки
            if (Utils.ScreenSize == ScreenSizeMode.s480x800)
                this.MinimizeBox = false;
        }

        private void ApplySettings()
        {
            if (Utils.ScreenSize == ScreenSizeMode.s480x800)
                this.MinimizeBox = true;

            //если изменен порядок следования отчетов
            bool isChangeSettings = false;

            //если изменили субъект, запомним настройки пользователя, 
            //и заставим заного обновлятся все субъектно зависимые отчеты
            if (this.UserSettings.EntityIndex != this.SettingsView.ChangedEntity)
            {
                this.ChangeEntity(this.SettingsView.ChangedEntity);
                isChangeSettings = true;
            }

            //получаем список отчетов из настроек, и синхронизируем их видимость 
            //и порядок с отчетами на ленте
            List<ScrollListItem> scrollReportList = this.SettingsView.ReportList;
            for (int i = 0; i < scrollReportList.Count; i++)
            {
                ScrollListItem scrollReport = scrollReportList[i];
                Report report = (Report)scrollReport.Tag;
                //если поменялись настройки отчета, сохраним их...
                if ((report.Position != i) || (report.Visible != scrollReport.IsChanged))
                {
                    report.Position = i;
                    report.Visible = scrollReport.IsChanged;
                    report.WriteSettingsToDB();
                    isChangeSettings = true;
                }
            }

            this.SettingsView.HideEntityView();
            this.SettingsView.Parent = null;
            this.SettingsView.Visible = false;
            //если были изменены свойства отчетов
            if (isChangeSettings)
                this.InitScrollView(this.Reports.Items);
        }

        private void ChangeEntity(int entityIndex)
        {
            this.UserSettings.EntityIndex = entityIndex;
            this.DbHelper.SetUserSettings(this.UserSettings);
            this.Reports.ChangedEntity();// ClearDownloadingQueoe();
            Application.DoEvents();

            foreach (Report report in this.Reports.Items)
            {
                if (report.IsSubjectDependent)
                    report.ChangedEntity();
            }
        }

        private void ShowAboutView()
        {
            if (!this.AboutView.IsInitializedVisualComponent)
                this.AboutView.InitVisualComponent();

            this.AboutView.Dock = DockStyle.Fill;
            this.AboutView.Visible = true;
            this.AboutView.Parent = this;
            this.grandPanel.SendToBack();

            //если разрешение 480х800 то на время показа настроек вместо крестика
            //(сворачивания приложения) отображаем "ok", что бы можно было закрыть настройки
            if (Utils.ScreenSize == ScreenSizeMode.s480x800)
                this.MinimizeBox = false;
        }

        private void CloseAboutView()
        {
            if (Utils.ScreenSize == ScreenSizeMode.s480x800)
                this.MinimizeBox = true;

            this.AboutView.Parent = null;
            this.AboutView.Visible = false;
        }

        private void EnableProgressIndicators(bool value)
        {
            this.lbProgressIndicator.Visible = value;

            this.progressIndicator.AlignByCentr();
            this.progressIndicator.Visible = value;
        }

        private void SetProgressIndicatorText(string text, bool isError)
        {
            this.lbProgressIndicator.Text = text;
            this.lbProgressIndicator.ForeColor = isError ? Color.Red : Color.White;
        }

        /// <summary>
        /// Сворачиваем приложение в трей
        /// </summary>
        public void Minimize()
        {
            try
            {
                Win32Helper.ShowWindow(this.Handle, Win32Helper.SW_MINIMIZED);
            }
            catch
            {
            }
        }

        /// <summary>
        /// Покажем диалог о закрытии приложения
        /// </summary>
        private void ShowClosedForm()
        {
            ClosedDialog dialog = new ClosedDialog();
            Utils.AlignByCentr(dialog, this);

            this.Controls.Add(dialog);
            this.Controls.SetChildIndex(dialog, 0);
            CloseDialogResult dialogResult = dialog.ShowModal();
            switch (dialogResult)
            {
                case CloseDialogResult.Yes:
                    {
                        this.Close();
                        break;
                    }
                case CloseDialogResult.Minimize:
                    {
                        this.Minimize();
                        break;
                    }
            }
        }

        /// <summary>
        /// При развороте экрана аппаратными средствами устройства, развернем и наши отчеты
        /// </summary>
        private void RotateScreen()
        {
            this.rotateScreenTimer.Enabled = false;
            if (!this.isUserScreenRotaion)
            {
                try
                {
                    this.isUserScreenRotaion = true;
                    switch (SystemSettings.ScreenOrientation)
                    {
                        case ScreenOrientation.Angle0:
                        case ScreenOrientation.Angle180:
                            {
                                if (this.CurrentViewMode == ReportViewMode.Horizontal)
                                {
                                    this.CurrentViewMode = this.prepareViewMode;
                                    this.popupMenu.Conceal = true;
                                }
                                break;
                            }
                        case ScreenOrientation.Angle90:
                        case ScreenOrientation.Angle270:
                            {
                                if (this.CurrentViewMode != ReportViewMode.Horizontal)
                                {
                                    this.CurrentViewMode = ReportViewMode.Horizontal;   
                                    this.popupMenu.Conceal = true;
                                }
                                break;
                            }
                    }
                }
                finally
                {
                    this.isUserScreenRotaion = false;
                }
            }
        }

        /// <summary>
        /// Обрабатываем нажатие кнопок устройства
        /// </summary>
        /// <param name="keyData"></param>
        private void ProcessHardwareButton(KeyData keyData)
        {
            //коды всех кнопок меньше 100
            if (keyData.KeyCode > 100)
                return;
            //обработчик нажатия на кнопки устройства срабатывает два раза, а обрабатывать нам надо лишь одно
            //будем обрабатывать каждое второе
            if (this.preparePressButtonKey == keyData.KeyCode)
            {
                this.preparePressButtonKey = 0;
                HardwareButtons pressButton = Utils.GetHardwareButton(keyData);
                switch (pressButton)
                {
                    case HardwareButtons.Center:
                        {
                            if (this.IsReportsShowing)
                                this.CurrentViewMode = this.GetNextViewMode(this.CurrentViewMode);
                            break;
                        }
                    case HardwareButtons.Left:
                        {
                            if (this.IsReportsShowing && (this.CurrentViewMode == ReportViewMode.Original))
                                this.scrollView.ChangePrepareControl();
                            break;
                        }
                    case HardwareButtons.Right:
                        {
                            if (this.IsReportsShowing && (this.CurrentViewMode == ReportViewMode.Original))
                                this.scrollView.ChangeNextControl();
                            break;
                        }
                    //При скролирование содержимого браузера есть проблема, время от времени он теряет фокус
                    //и теряется возможность скролировать кнопками устройства. Берем эту обязаность целиком на себя
                    //выведем в интерфейс методы PageUp и PageDown, и будем вызывать их в ручную. 
                    case HardwareButtons.Up:
                        {
                            if (this.IsReportsShowing && (this.CurrentChangedReport != null))
                                this.CurrentChangedReport.CurrentView.WebBrowser.PageUp();
                            break;
                        }
                    case HardwareButtons.Bottom:
                        {
                            if (this.IsReportsShowing && (this.CurrentChangedReport != null))
                                this.CurrentChangedReport.CurrentView.WebBrowser.PageDown();
                            break;
                        }
                }
            }
            else
            {
                this.preparePressButtonKey = keyData.KeyCode;
            }
        }

        private ReportViewMode GetNextViewMode(ReportViewMode viewMode)
        {
            int valueCount = 3;
            int intValue = (int)viewMode;
            intValue++;
            if (intValue >= valueCount)
                intValue = 0;
            return (ReportViewMode)intValue;
        }

        protected override void OnClosed(EventArgs e)
        {
            try
            {
                this._isClosingApplication = true;
                this.Downloader.AbortAsynchSaveHTML();
                base.OnClosed(e);
            }
            catch
            {
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            base.OnPaintBackground(e);
        }

        protected override void OnResize(EventArgs e)
        {
            //активно ли в данный момент приложение
            bool isActiveApplication = (this.Handle == Win32Helper.GetForegroundWindow());
            //При развороте экрана будем заряжать таймер, дабы при его срабатывании 
            //адаптировать вид под текущее положение... Если делать это сразу (без паузы) возникают проблемы
            if (!this.isUserScreenRotaion && isActiveApplication)
            {
                this.rotateScreenTimer.Interval = 50;

                this.rotateScreenTimer.Enabled = false;
                this.rotateScreenTimer.Enabled = true;
            }
        }

        #region Обработчики

        void Downloader_ConnectToHostComplete(object sender, bool success, string errorText)
        {
            bool isError = (errorText != string.Empty);
            string text = isError ? Consts.msFaultConnectToServ : "Авторизация";
            this.Invoke(new EventHandler(delegate { this.SetProgressIndicatorText(text, isError); }));
        }

        void Downloader_DownloadServerDataComplete(object sender, XmlDocument serverData, string errorText)
        {
            this.Invoke(new EventHandler(delegate { this.ProcessServerData(serverData, errorText); }));
        }

        void scrollView_BeforeControlChanged(object sender, int index, Control control)
        {
        }

        void scrollView_AfterControlChanged(object sender, int index, Control control)
        {
            Report report = (Report)control;

            this.ChangeReport(report);

            /*Пока отключим подгрузку соседнего отчета
            if (this.scrollView.NextControl != null)
            {
                Report nextReport = (Report)this.scrollView.NextControl;
                this.Reports.ReportChanged(nextReport, this.CurrentViewMode, true);
            }*/
        }

        private void ChangeReport(Report report)
        {
            if (report != null)
            {
                this.changedReport = report;
                this.changedReportIndex = this.scrollView.ChangedControlIndex;

                this.ResizeReport(report);
                this.Reports.ReportChanged(report, this.CurrentViewMode, false);
                this.SetCustomClickBounds(report);
            }
        }

        void reportPopupMenu_ChangeReportViewMode(ReportViewMode viewMode)
        {
            this.CurrentViewMode = viewMode;
        }

        void reportPopupMenu_RefreshReportView(object sender, EventArgs e)
        {
            Report report = this.CurrentChangedReport;
            if (report != null)
                report.Refresh(this.CurrentViewMode);
            this.popupMenu.Conceal = true;
        }

        void popupMenu_СloseApplication(bool isMinimize)
        {
            this.popupMenu.Conceal = true;
            this.ShowClosedForm();
        }

        void popupMenu_ShowSettingsView(object sender, EventArgs e)
        {
            if (this.UserSettings.IsAuthentication)
                this.ShowSettingsView();
        }

        void popupMenu_ShowAbout(object sender, EventArgs e)
        {
            this.ShowAboutView();
        }

        void SettingsView_ApplySettings(object sender, EventArgs e)
        {
            this.ApplySettings();
        }

        void AboutView_CloseAboutView(object sender, EventArgs e)
        {
            this.CloseAboutView();
        }

        void scrollView_GotFocus(object sender, EventArgs e)
        {
            this.popupMenu.Conceal = true;
        }

        void scrollView_MouseDown(object sender, MouseEventArgs e)
        {
            this.popupMenu.Conceal = true;
        }

        void report_ClickCustomClickBounds(object sender, EventArgs e)
        {
            this.popupMenu.Conceal = false;
        }

        void scrollView_ScrollIndicatorClick(object sender, EventArgs e)
        {
            this.popupMenu.Conceal = false;
        }
        
        void MainForm_Closing(object sender, CancelEventArgs e)
        {
            //если отображаются настройки, все лишь закроим их
            if (this.SettingsView.Visible)
            {
                e.Cancel = true;
                this.ApplySettings();
            }
            //если отображается "о программе", все лишь закроим их
            if (this.AboutView.Visible)
            {
                e.Cancel = true;
                this.CloseAboutView();
            }
        }

        void rotateScreenTimer_Tick(object sender, EventArgs e)
        {
            this.RotateScreen();
        }

        void Keyboard_KeyDetected(OpenNETCF.Win32.WM keyMessage, KeyData keyData)
        {
            this.ProcessHardwareButton(keyData);
        }
        #endregion
    }
}