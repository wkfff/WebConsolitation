using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Data;
using System.IO;
using System.Text.RegularExpressions;

using Krista.FM.Common;
using Krista.FM.Common.Xml;
using Krista.FM.ServerLibrary;
using Krista.FM.ServerLibrary.TemplatesService;
using Krista.FM.Client;
using Krista.FM.Client.WebReportsCommon;
using System.Threading;
using System.Configuration;

namespace Krista.FM.Client.WebReportsHelper
{
    public class WebReportsHelper
    {
        #region Делегаты
        public delegate void StartDeployDataHandler(object sender);
        public delegate void EndDeployDataHandler(object sender);
        public delegate void ChangeUpdateStateHandler(object sender, UpdateState state);
        public delegate void CurrentStateProgressHandler(object sender, int percentDone);
        #endregion

        #region События
        private event StartDeployDataHandler _startDeployData;
        private event EndDeployDataHandler _endDeployData;
        private event ChangeUpdateStateHandler _changeUpdateState;
        private event CurrentStateProgressHandler _currentStateProgress;
        private event ErrorEventHandler _errorEvent;

        /// <summary>
        /// Начали процесс обновления данных на сервере
        /// </summary>
        public event StartDeployDataHandler StartDeployData
        {
            add { _startDeployData += value; }
            remove { _startDeployData -= value; }
        }

        /// <summary>
        /// Закончили процесс обновления данных на сервер
        /// </summary>
        public event EndDeployDataHandler EndDeployData
        {
            add { _endDeployData += value; }
            remove { _endDeployData -= value; }
        }

        /// <summary>
        /// Смена состояния процесса обновления
        /// </summary>
        public event ChangeUpdateStateHandler ChangeUpdateState
        {
            add { _changeUpdateState += value; }
            remove { _changeUpdateState -= value; }
        }

        /// <summary>
        /// Процент выполнения текущей операции
        /// </summary>
        public event CurrentStateProgressHandler CurrentStateProgress
        {
            add { _currentStateProgress += value; }
            remove { _currentStateProgress -= value; }
        }

        /// <summary>
        /// Критическая ошибка во время обновления данных
        /// </summary>
        public event ErrorEventHandler ErrorEvent
        {
            add { _errorEvent += value; }
            remove { _errorEvent -= value; }
        }
        #endregion

        #region Поля
        //релиз или тестовая версия
        private bool _isRelease;
        //схема с которой работает приложение
        private IScheme _scheme;
        //качальщик
        private ReportsBootloader.ReportsBootloader _bootloader;
        //выкладывальщик
        private ReportsUploader.ReportsUploader _uploader;
        //строка с дампом базы данных
        private string _dataBaseDump;
        //путь сохранения пакета данных для сайта
        private string _dataBurstSavePath;
        //В эту папку складываем скрипты неободимые для создания и заполнения базы данных
        private string _dataBaseScriptsPath;
        private List<ReportInfo> downloadingReports;
        //определяет формат в котором создается пакет данных
        private DataBurstArchiveMode _dataBurstArchiveMode;
        //директория запуска приложения
        private string _startupPath;
        //если запущен из asp.net (IIS)
        private bool isWorkFromAsp;
        //состояние обновления
        private UpdateState _currentUpdateState;
        #endregion

        #region Свойства
        /// <summary>
        /// Релиз, или тестовая версия (сейчас разница лишь в том, что для не релизной, 
        /// на среднем звене, не обновляются даты выкладывания отетов)
        /// </summary>
        public bool IsRelease
        {
            get { return _isRelease; }
            set { _isRelease = value; }
        }

        /// <summary>
        /// Компонент отвечающий за сохранение отчетов
        /// </summary>
        public ReportsBootloader.ReportsBootloader Bootloader
        {
            get { return _bootloader; }
            set { _bootloader = value; }
        }

        /// <summary>
        /// Компонент отвечающий за выкладывание данных на сервер
        /// </summary>
        public ReportsUploader.ReportsUploader Uploader
        {
            get { return _uploader; }
            set { _uploader = value; }
        }

        /// <summary>
        /// Схема с которой работает приложение
        /// </summary>
        public IScheme Scheme
        {
            get { return _scheme; }
            set { _scheme = value; }
        }

        /// <summary>
        /// Строка с дампом базы данных
        /// </summary>
        public string DataBaseDump
        {
            get { return _dataBaseDump; }
            set { _dataBaseDump = value; }
        }

        /// <summary>
        /// Путь сохранения пакета данных для сайта
        /// </summary>
        public string DataBurstSavePath
        {
            get { return _dataBurstSavePath; }
            set { _dataBurstSavePath = value; }
        }

        /// <summary>
        /// В эту папку складываем скрипты неободимые для создания и заполнения базы данных
        /// </summary>
        public string DataBaseScriptsPath
        {
            get { return _dataBaseScriptsPath; }
            set { _dataBaseScriptsPath = value; }
        }

        /// <summary>
        /// Определяет формат в котором создается пакета данных
        /// </summary>
        public DataBurstArchiveMode DataBurstArchiveMode
        {
            get { return _dataBurstArchiveMode; }
            set { _dataBurstArchiveMode = value; }
        }

        /// <summary>
        /// Директория запуска приложения
        /// </summary>
        public string StartupPath
        {
            get { return _startupPath; }
            set { _startupPath = value; }
        }

        /// <summary>
        /// Текущее состояние процесса обновления
        /// </summary>
        public UpdateState CurrentUpdateState
        {
            get { return _currentUpdateState; }
            set 
            { 
                _currentUpdateState = value;
                this.OnChangeUpdateState(value);
            }
        }
        #endregion

        public WebReportsHelper(IScheme scheme)
            : this(scheme, String.Empty)
        {
        }

        public WebReportsHelper(IScheme scheme, string reportsHostAddress)
            : this(scheme, System.Windows.Forms.Application.StartupPath, false, reportsHostAddress)
        {
        }

        public WebReportsHelper(IScheme scheme, string startupPath, bool isWorkFromAsp, string reportsHostAddress)
        {
            this.Scheme = scheme;
            this.isWorkFromAsp = isWorkFromAsp;
            this.StartupPath = startupPath;

            Trace.TraceInformation("Создаем экземпляр класса для генерации отчетов для PHP web сервеса.");
            this.Bootloader = new ReportsBootloader.ReportsBootloader(this.StartupPath, this.isWorkFromAsp, reportsHostAddress);
            this.Bootloader.DownloadReportCompleted += new Krista.FM.Client.ReportsBootloader.ReportsBootloader.DownloadReportCompletedHandler(Bootloader_DownloadReportCompleted);
            Trace.TraceInformation("Создали экземпляр класса для загрузки отчетов на PHP web сервес.");
            this.Uploader = new ReportsUploader.ReportsUploader();
            this.Uploader.TransferProgress += new Krista.FM.Client.ReportsUploader.ReportsUploader.TransferProgressHandler(Uploader_TransferProgress);
            this.Uploader.RollOutOnDistHostProgress += new Krista.FM.Client.ReportsUploader.ReportsUploader.RollOutOnDistHostProgressHandler(Uploader_RollOutOnDistHostProgress);
            //выставляем значения по умолчанию
            this.SetValueOnDefault();
            //информация об отчетах требующих генерации
            this.downloadingReports = new List<ReportInfo>();
        }

        /// <summary>
        /// Начать процедуру обновления данных
        /// </summary>
        /// <param name="startParams">xml с кодами отчетов требующих генерации</param>
        public void DeployData(XmlNode startParams)
        {
            this.DeployData(startParams, true);
        }

        /// <summary>
        /// Начать процедуру обновления данных
        /// </summary>
        /// <param name="startParams">xml с кодами отчетов требующих генерации</param>
        /// <param name="uploadAfterBotloading">признак начала загрузки данных на сервер, сразу после
        /// их генерации отчетов</param>
        public void DeployData(XmlNode startParams, bool uploadAfterBotloading)
        {
            try
            {
                this.OnStrartDeployData();
                Trace.TraceInformation("Загружаем настройки WebReportsHelper.");
                this.LoadSettings();
                Trace.Indent();
                //Начинаем формировать пакет для PHP web сервиса
                this.BuildDataBurst(startParams);

                Trace.TraceInformation("Приступаем ко второму этапу формирования пакета");
                this.BuildDataBurstSecondStep();
                //если в настройках установлено загрузка сразу после генерации, то сделаем это
                if (uploadAfterBotloading)
                {
                    Trace.TraceInformation("Третий этап, Uploader передает и разворачивает собранный пакет");
                    this.UploadAndSynchronize();
                }
                this.OnEndDeployData();
                Trace.Unindent();
            }
            catch (Exception e)
            {
                Trace.TraceError(e.Message);
                this.OnErrorEvent(e);
            }
        }

        /// <summary>
        /// Загрузка, развертывание данных на сервер и обновление в реляционной базе даты выкладывания отчетов.
        /// </summary>
        public void UploadAndSynchronize()
        {
            Chilkat.SFtp sftp = new Chilkat.SFtp();
            //передаем данные
            this.CurrentUpdateState = UpdateState.UploadDataBurst;
            this.Uploader.StartUploading(this.GetArchiveDataBurstName());
            //Разворачиваем пакета на сервере
            this.CurrentUpdateState = UpdateState.RollOutOnDistHost;
            this.Uploader.RollOutOnDistHost();
            //Данные будем синхронизировать только при релизе
            if (this.IsRelease)
            {
                //Обновим в реляционной базе дату выкладывания отчетов.
                this.SynchronizeDeployDate(this.downloadingReports);
            }
        }

        /// <summary>
        /// Выставляем значения по умолчанию
        /// </summary>
        private void SetValueOnDefault()
        {
            //по умолчанию - релиз
            this.IsRelease = true;
            //путь сохранения пакета данных для сайта
            this.DataBurstSavePath = string.Empty;
            //путь сохранения скриптов для базы данных
            this.DataBaseScriptsPath = string.Empty;
            //формат пакета данных
            this.DataBurstArchiveMode = DataBurstArchiveMode.GZip;
        }

        public void LoadSettings()
        {
            this.CurrentUpdateState = UpdateState.LoadSettings;
            WebReportsSettingsSection section = (WebReportsSettingsSection)ConfigurationManager.GetSection("webReportsSettingsSection");
            this.LoadSettings(section);
        }

        public void LoadSettings(WebReportsSettingsSection settingsSection)
        {
            try
            {
                Trace.Indent();
                //релизная ли версия
                this.IsRelease = settingsSection.IsRelease.Value;
                //загрузили путь сохранения пакета данных для сайта
                string dataBurstSavePath = settingsSection.DataBurstSavePath.Value;
                if (dataBurstSavePath != string.Empty)
                {
                    dataBurstSavePath = Path.Combine(dataBurstSavePath, Consts.dataBurstName);
                    dataBurstSavePath = Path.Combine(this.StartupPath, dataBurstSavePath);
                    this.DataBurstSavePath = Utils.PathWithoutLastSlash(dataBurstSavePath);

                    //путь сохранения скриптов для базы данных
                    string dataBaseScriptsPath = Path.Combine(dataBurstSavePath, Consts.dataBaseBurstName);
                    this.DataBaseScriptsPath = Utils.PathWithoutLastSlash(dataBaseScriptsPath);
                }
                this.AddTraceInfo();

                Trace.TraceInformation("Загружаем настройки ReportsBootloader.");
                this.Bootloader.LoadSettings(settingsSection);
                Trace.TraceInformation("Загружаем настройки ReportsUploader.");
                this.Uploader.LoadSettings(settingsSection);
                
            }
            finally
            {
                Trace.Unindent();
            }
        }

        /// <summary>
        /// Информация для трасировки
        /// </summary>
        private void AddTraceInfo()
        {
            Trace.TraceInformation("Путь сохранения пакета с данными для PHP сервиса: {0}", this.DataBurstSavePath);
            Trace.TraceInformation("Путь сохранения скриптов для базы данных PHP сервиса: {0}", this.DataBurstSavePath);
        }

        /// <summary>
        /// Скомпоновать пакет данных с обновлением для сайта
        /// </summary>
        /// <param name="startParams">Стартовые параметры (список отчетов требующих генерации)</param>
        /// <returns></returns>
        public void BuildDataBurst(XmlNode startParams)
        {
            if ((this.Scheme == null))
            {
                string error = "Отсутствует подключение к схеме.";
                throw new Exception(error);
            }

            //параметры необходимые для начала загрузки
            int subjectCount = this.GetSubjectCount();
            Trace.TraceVerbose("Количество субъектов: {0}", subjectCount);
          
            //список отчетов, требующих генерации
            string[] reportsID = this.GetReportsID(startParams);
           
            Trace.TraceInformation("Отчеты требующие генерации: {0}", this.ArrayToString(reportsID));
            //Получим со среднего звена всю инетерсующую информацию об отчетах которые будем закачивать
            this.downloadingReports = this.GetDownloadingReportsInfo(reportsID);

            //Начинаем генерацию отчетов
            this.BuildDataBurst(subjectCount, this.downloadingReports);
        }

        private string ArrayToString(string[] array)
        {
            string result = string.Empty;
            foreach (string item in array)
            {
                result = (result == string.Empty) ? item : result + ", " + item;
            }
            return result;
        }

        /// <summary>
        /// Скомпоновать пакет данных с обновлением для сайта
        /// </summary>
        /// <param name="subjectCount">Количество субъектов</param>
        /// <param name="downloadingReports">Информация об отчетах требующих генерации</param>
        public void BuildDataBurst(int subjectCount, List<ReportInfo> downloadingReports)
        {
            //Очистим директорию под пакет
            if (Directory.Exists(this.DataBurstSavePath))
                Directory.Delete(this.DataBurstSavePath, true);

            //требуется ли генерация отчетов
            bool isNeedGeneretionReports = downloadingReports.Count > 0;

            if (isNeedGeneretionReports)
            {
                Trace.TraceInformation("Начинаем генерацию отчетов");
                Trace.Indent();
                this.CurrentUpdateState = UpdateState.BootloadReports;
                if (this.Bootloader.DownloadReports(subjectCount, downloadingReports))
                {
                    Trace.TraceInformation("Закончили генерацию отчетов.");
                }
                else
                {
                    string error = "Не удалось сгенерировать отчеты для iPhone. " + 
                        this.Bootloader.ErrorList.ToString();
                    throw new Exception(error);
                }
                Trace.Unindent();
            }
        }

        /// <summary>
        /// Второй этап формирования пакета:
        ///     1)создаем слепок базы даннах
        ///     2)перемещаем в пакет все необходимые скрипты, для создания базы данных на PHP сервере
        ///     3)архивируем пакет
        /// </summary>
        private void BuildDataBurstSecondStep()
        {
            Trace.TraceInformation("Формируем дамп базы данной для PHP сервиса.");
            this.CurrentUpdateState = UpdateState.BuildDataBaseDump;
            this.DataBaseDump = SQLBuilder.BuildSQLScripts(this.Scheme);

            Trace.TraceInformation("Обновляем в дампе даты последних загрузок отчетов.");
            this.DataBaseDump = this.UpdateReportsDeployDate(this.DataBaseDump, this.downloadingReports);

            Trace.TraceInformation(@"Сохраняем в пакете все необходимые скрипты для (создания, удаления) 
                БД на PHP сервисе.");
            this.SaveDataBaseScripts();

            Trace.TraceInformation(@"На данном этапе пакет полностью сформирован, архивируем его если требуется.");
            this.ArchiveDataBurst();
        }

        /// <summary>
        /// Получить имя заархивированого пакета данных
        /// </summary>
        /// <returns></returns>
        private string GetArchiveDataBurstName()
        {
            switch (this.DataBurstArchiveMode)
            {
                case DataBurstArchiveMode.GZip:
                    {
                        return this.DataBurstSavePath + ".tar.gz";
                    }
                case DataBurstArchiveMode.Zip:
                    {
                        return this.DataBurstSavePath + ".zip";
                    }
            }
            return string.Empty;
        }

        /// <summary>
        /// Архивирует собранный пакет данных
        /// </summary>
        private void ArchiveDataBurst()
        {
            this.CurrentUpdateState = UpdateState.ArchiveDataBurst;
            switch (this.DataBurstArchiveMode)
            {
                case DataBurstArchiveMode.GZip:
                    {
                        Chilkat.Tar tar = new Chilkat.Tar();
                        tar.EnableEvents = true;
                        tar.OnPercentDone += new Chilkat.Tar.PercentDoneEventHandler(tar_OnPercentDone);
                        Trace.TraceInformation("Архивируем в GZip.");

                        //введем ключ активации, если надо генерим тутА Tools\ChilKat\Keygen.exe
                        if (!tar.UnlockComponent("TarArchT34MB34N_5219B6AEl6jp"))
                            throw new Exception(tar.LastErrorText);

                        tar.WriteFormat = "gnu";
                        //сейчас небольшой фокус, надо что бы в архиве оказалась сама папка DataBurst,
                        //для этого надо архивировать на уровень выше, или переместь все содержимое папки
                        //на уровень ниже, выбрал второй путь
                        string temDir = Path.Combine(this.DataBurstSavePath, Consts.dataBurstName);
                        Utils.MoveDirectory(this.DataBurstSavePath, temDir, false);

                        //директория которую архивируем
                        if (!tar.AddDirRoot(this.DataBurstSavePath))
                            throw new Exception(tar.LastErrorText);

                        //архивируем, указываем имя файла куда генерить
                        if (!tar.WriteTarGz(this.GetArchiveDataBurstName()))
                            throw new Exception(tar.LastErrorText);
                        //перемещаем содержимое обратно
                        Utils.MoveDirectory(temDir, this.DataBurstSavePath, true);
                        break;
                    }
                case DataBurstArchiveMode.Zip:
                    {
                        throw new Exception("Not Emplementation");
                    }
            }
        }

        /// <summary>
        /// Переносим все скрипты из ресурсов в пакет данных
        /// </summary>
        private void SaveDataBaseScripts()
        {
            Directory.CreateDirectory(this.DataBaseScriptsPath);

            string savePath = Path.Combine(this.DataBaseScriptsPath, Consts.dataBaseScriptFileName);
            File.WriteAllText(savePath, this.DataBaseDump, Encoding.Default);

            //путь к ресурсам
            string resourcePath = Utils.GetResorcePath(this.StartupPath, this.isWorkFromAsp);
            //полный путь к ресурсам базы данной
            string dataBaseResorcesPath = Path.Combine(resourcePath, Consts.dataBaseFolderName);
            //Перемещаем файли из ресурса в пакет
            string sourcePath = Path.Combine(dataBaseResorcesPath, Consts.createExpImpTables);
            savePath = Path.Combine(this.DataBaseScriptsPath, Consts.createExpImpTables);
            File.Copy(sourcePath, savePath, true);

            sourcePath = Path.Combine(dataBaseResorcesPath, Consts.createSystemTables);
            savePath = Path.Combine(this.DataBaseScriptsPath, Consts.createSystemTables);
            File.Copy(sourcePath, savePath, true);

            sourcePath = Path.Combine(dataBaseResorcesPath, Consts.dropExpImpTables);
            savePath = Path.Combine(this.DataBaseScriptsPath, Consts.dropExpImpTables);
            File.Copy(sourcePath, savePath, true);

            sourcePath = Path.Combine(dataBaseResorcesPath, Consts.dropSystemTables);
            savePath = Path.Combine(this.DataBaseScriptsPath, Consts.dropSystemTables);
            File.Copy(sourcePath, savePath, true);
        }

        /// <summary>
        /// Если id отчетов производные (горизонтальные/вертикальные) вернет true
        /// </summary>
        /// <param name="id">id отчета</param>
        /// <returns></returns>
        private bool SecondaryID(string id)
        {
            return (id.LastIndexOf("_V") == (id.Length - 1)) || (id.LastIndexOf("_H") == (id.Length - 1));
        }

        /// <summary>
        /// Обновим дату загрузку в скрипте, и запомним у каких отчетов обновляли ее
        /// </summary>
        /// <param name="sourceScript">исходный скрипт</param>
        /// <param name="reportsInfo">информация об отчетах</param>
        /// <returns>скрипт с актуальными датами обновления</returns>
        private string UpdateReportsDeployDate(string sourceScript, List<ReportInfo> reportsInfo)
        {
            string result = sourceScript;
            if (reportsInfo.Count == 0)
                return result;

            foreach (ReportInfo reportInfo in reportsInfo)
            {
                string pattern = string.Format("'{0}'[\\s\\S]*?<LastDeployDate>(?<deployDate>[\\s\\S]*?)</LastDeployDate>",
                    reportInfo.Code);
                MatchCollection matches = Regex.Matches(result, pattern, RegexOptions.IgnoreCase);
                if (matches.Count > 0)
                {
                    Match match = matches[0];
                    Group group = match.Groups[Consts.deployDate];
                    if (!Utils.IsGroupEmpty(group))
                    {
                        DateTime newDeployDate = DateTime.Now;
                        DateTime oldDeployDate = DateTime.Parse(group.Value);
                        reportInfo.NewDeployDate = newDeployDate;
                        reportInfo.OldDeployDate = oldDeployDate;

                        result = result.Remove(group.Index, group.Length);
                        result = result.Insert(group.Index, reportInfo.NewDeployDate.ToString("yyyy-MM-dd HH:mm:ss"));
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Получить количество субъектов
        /// </summary>
        /// <returns></returns>
        private int GetSubjectCount()
        {
            //когда Герман сделает таблицу с субъектами, это значение будем брать от туда
            return 83;
        }

        /// <summary>
        /// Из узла извлекаем id отчетов которые будем генерировать
        /// </summary>
        /// <param name="startParams"></param>
        /// <returns></returns>
        private string[] GetReportsID(XmlNode startParams)
        {
            if (startParams == null)
                return new string[0];
            XmlNodeList reports = startParams.SelectNodes(@"//report");
            string[] result = new string[reports.Count];

            for (int i = 0; i < reports.Count; i++)
            {
                result[i] = XmlHelper.GetStringAttrValue(reports[i], "id", string.Empty).ToLower();
            }
            return result;
        }


        /// <summary>
        /// //Получим со среднего звена всю инетерсующую информацию об отчетах которые будем закачивать
        /// </summary>
        /// <param name="reportsID">не упорядоченные id отчетов</param>
        private List<ReportInfo> GetDownloadingReportsInfo(string[] reportsID)
        {
            List<ReportInfo> result = new List<ReportInfo>();
            this.CurrentUpdateState = UpdateState.QueryReportsParams;
            List<string> reportsIDList = new List<string>();
            reportsIDList.AddRange(reportsID);

            ITemplatesRepository repository = this.Scheme.TemplatesService.Repository;
            DataTable iPhoneReports = repository.GetTemplatesInfo(TemplateTypes.IPhone);

            foreach (DataRow row in iPhoneReports.Rows)
            {
                string reportCode = (row[TemplateFields.Code] is DBNull) ? "EmptyCode" :
                    (string)row[TemplateFields.Code];

                //id отчетов должны быть в нижнем регистре
                reportCode = reportCode.ToLower();

                //если в списке интересующих отчетов нет текущего, продолжаем поиск
                if (!reportsIDList.Contains(reportCode))
                    continue;

                byte[] document = repository.GetDocument(Convert.ToInt32(row[TemplateFields.ID]));
                ReportInfo reportInfo = new ReportInfo(reportCode, false, MobileTemplateTypes.IPhone);
                if (document != null)
                {
                    string xmlDescriptor = Encoding.UTF8.GetString(document);
                    IPhoteTemplateDescriptor descriptor = XmlHelper.XmlStr2Obj<IPhoteTemplateDescriptor>(xmlDescriptor);
                    reportInfo.SubjectDepended = descriptor.SubjectDepended;
                    reportInfo.TemplateType = descriptor.TemplateType;
                }
                if (!result.Contains(reportInfo))
                    result.Add(reportInfo);
            }
            return result;
        }

        /// <summary>
        /// Обновим в реляционной базе дату выкладывания отчетов.
        /// </summary>
        private void SynchronizeDeployDate(List<ReportInfo> reportsInfo)
        {
            this.CurrentUpdateState = UpdateState.SynchronizeDeployDate;
            Trace.TraceInformation("Обновим в реляционной базе дату выкладывания отчетов.");
            ITemplatesRepository repository = this.Scheme.TemplatesService.Repository;
            DataTable iPhoneReports = null;
            using (IDatabase db = this.Scheme.SchemeDWH.DB)
            {
                int maxRecordCount = 10000;
                string sqlScript = string.Format("SELECT * FROM Templates WHERE RefTemplatesTypes = {0}", (int)TemplateTypes.IPhone);
                iPhoneReports = db.SelectData(sqlScript, maxRecordCount);
            }

            foreach (DataRow row in iPhoneReports.Rows)
            {
                string reportCode = (row[TemplateFields.Code] is DBNull) ? "EmptyCode" : 
                    (string)row[TemplateFields.Code];
                reportCode = reportCode.ToLower();

                foreach (ReportInfo reportInfo in reportsInfo)
                {
                    if (reportInfo.Code == reportCode)
                    {
                        // получаем дескриптор
                        byte[] document = null;
                        if (!(row[TemplateFields.Document] is DBNull))
                            document = (byte[])row[TemplateFields.Document];
                        string xmlDescriptor = string.Empty; 
                        IPhoteTemplateDescriptor descriptor;
                        if (document == null)
                            descriptor = new IPhoteTemplateDescriptor();
                        else
                        {
                            xmlDescriptor = Encoding.UTF8.GetString(document);
                            descriptor = XmlHelper.XmlStr2Obj<IPhoteTemplateDescriptor>(xmlDescriptor);
                        }

                        // изменяем свойство дескриптора
                        descriptor.LastDeployDate = reportInfo.NewDeployDate;

                        // сохраняем значение дескриптора
                        xmlDescriptor = XmlHelper.Obj2XmlStr(descriptor);
                        repository.SetDocument(Encoding.UTF8.GetBytes(xmlDescriptor),
                            Convert.ToInt32(row[TemplateFields.ID]));
                        break;
                    }
                }
            }
        }

        #region Обработчики событий
        /// <summary>
        /// Прогресс генерации отчетов 
        /// </summary>
        /// <param name="sender">генератор отчетов</param>
        /// <param name="downloaderReportsNumber">номер скаченого отчета</param>
        /// <param name="reportsCount">общее количество отчетов</param>
        /// <param name="errorText">текст ошибки при закачке отчета</param>
        /// <param name="infoText">подробная информация о загрузке отчета</param>
        void Bootloader_DownloadReportCompleted(object sender, int downloaderReportsNumber, int reportsCount, string errorText, string infoText)
        {
            //процент выполнения
            int percentDone = (int)((float)downloaderReportsNumber / (float)reportsCount * 100);
            if (reportsCount <= 100)
                this.OnCurrentStateProgress(percentDone);
            else
            {
                //коэффицент срабатывания
                int ratio = reportsCount / 100;
                //если сгенерируемый отчет равен 0 или количеству отчетов или коэффициенту срабатывания,
                //вызываем событие
                if ((downloaderReportsNumber == 1) || (downloaderReportsNumber == reportsCount) 
                    || ((downloaderReportsNumber % ratio) == 0))
                    this.OnCurrentStateProgress(percentDone);
            }
        }

        /// <summary>
        /// Загрузка пакета на удаленый сервер
        /// </summary>
        /// <param name="percentDone">прогресс</param>
        void Uploader_TransferProgress(object sender, int percentDone)
        {
            this.OnCurrentStateProgress(percentDone);
        }

        /// <summary>
        /// Прогресс разворачивания пакета на удаленом сервере
        /// </summary>
        /// <param name="percentDone">прогресс</param>
        void Uploader_RollOutOnDistHostProgress(object sender, int percentDone)
        {
            this.OnCurrentStateProgress(percentDone);
        }

        /// <summary>
        /// Прогресс архивации данных
        /// </summary>
        /// <param name="args"></param>
        void tar_OnPercentDone(object sender, Chilkat.PercentDoneEventArgs args)
        {
            this.OnCurrentStateProgress(args.PercentDone);
        }
        #endregion

        private void OnStrartDeployData()
        {
            Trace.TraceInformation("Начинаем формировать пакет для PHP web сервиса");
            if (this._startDeployData != null)
                this._startDeployData(this);
        }

        private void OnEndDeployData()
        {
            this.CurrentUpdateState = UpdateState.FinishUpdateData;
            Trace.TraceInformation("Процесс обновления данных закончен...");
            if (this._endDeployData != null)
                this._endDeployData(this);
        }

        private void OnChangeUpdateState(UpdateState state)
        {
            if (this._changeUpdateState != null)
                this._changeUpdateState(this, state);
        }

        private void OnCurrentStateProgress(int percentDone)
        {
            if (this._currentStateProgress != null)
                this._currentStateProgress(this, percentDone);
        }

        private void OnErrorEvent(Exception e)
        {
            if (this._errorEvent != null)
                this._errorEvent(this, new ErrorEventArgs(e));
        }
    }

    /// <summary>
    /// Формат пакета
    /// </summary>
    public enum DataBurstArchiveMode
    {
        None,
        GZip,
        Zip
    }
}
