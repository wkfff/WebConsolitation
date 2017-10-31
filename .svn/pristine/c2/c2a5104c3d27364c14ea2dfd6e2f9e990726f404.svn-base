using System;
using System.Net;
using System.Threading;
using System.Data.OleDb;
using System.IO;
using System.Data;
using System.Xml;

using Krista.FM.Common.Xml;
using Krista.FM.Client.MobileReports.Common;
using System.Configuration;

namespace Krista.FM.Client.MobileReports.Uploader
{

    public class ReportsUploader
    {
        #region Константы
        /// <summary>
        /// Таймаут запроса
        /// </summary>
        const int requestTimeout = 100000;
        /// <summary>
        /// Время между запросами
        /// </summary>
        const int requestInterarrival = 5000;
        #endregion

        #region Делегаты
        public delegate void TransferStartHandler(object sender, string src, string dst, long totalBytes);
        public delegate void TransferProgressHandler(object sender, int percentDone);
        public delegate void TransferEndHandler(object sender, string src, string dst);
        public delegate void RollOutOnDistHostStartHandler(object sender);
        public delegate void RollOutOnDistHostProgressHandler(object sender, int percentDone);
        public delegate void RollOutOnDistHostEndHandler(object sender, bool success);
        #endregion

        #region События
        private event TransferStartHandler _transferStart;
        private event TransferProgressHandler _transferProgress;
        private event TransferEndHandler _transferEnd;
        private event RollOutOnDistHostStartHandler _rollOutOnDistHostStart;
        private event RollOutOnDistHostProgressHandler _rollOutOnDistHostProgress;
        private event RollOutOnDistHostEndHandler _rollOutOnDistHostEnd;

        /// <summary>
        /// Начали передачу
        /// </summary>
        public event TransferStartHandler TransferStart
        {
            add { _transferStart += value; }
            remove { _transferStart -= value; }
        }

        /// <summary>
        /// Процесс передачи
        /// </summary>
        public event TransferProgressHandler TransferProgress
        {
            add { _transferProgress += value; }
            remove { _transferProgress -= value; }
        }

        /// <summary>
        /// Закончили передачу
        /// </summary>
        public event TransferEndHandler TransferEnd
        {
            add { _transferEnd += value; }
            remove { _transferEnd -= value; }
        }

        /// <summary>
        /// Разворачиваем пакет на сервере
        /// </summary>
        public event RollOutOnDistHostStartHandler RollOutOnDistHostStart
        {
            add { _rollOutOnDistHostStart += value; }
            remove { _rollOutOnDistHostStart -= value; }
        }

        /// <summary>
        /// Прогресс разворачивания пакета на сервере
        /// </summary>
        public event RollOutOnDistHostProgressHandler RollOutOnDistHostProgress
        {
            add { _rollOutOnDistHostProgress += value; }
            remove { _rollOutOnDistHostProgress -= value; }
        }

        /// <summary>
        /// Закончили разворачивание пакета на сервере
        /// </summary>
        public event RollOutOnDistHostEndHandler RollOutOnDistHostEnd
        {
            add { _rollOutOnDistHostEnd += value; }
            remove { _rollOutOnDistHostEnd -= value; }
        }

        #endregion

        #region Поля
        //proxy
        private WebProxy _proxy;
        //ip сайта на который закачиваем пакет с данными
        private string _distHostIpAddress;
        //Uri сайта на который закачиваем пакет с данными
        private string _distHostUriAddress;
        //нужна ли авторизация
        private bool _enabledAuthorization;
        //логин пользователя
        private string _userLogin;
        //пароль пользователя
        private string _userPassword;
        //каталог сайта на удаленом сервере
        private string _distHostPath;
        //контейнер кук
        private CookieContainer _cookieContainer;
        #endregion
            
        #region Свойства
        /// <summary>
        /// Proxy
        /// </summary>
        public WebProxy Proxy
        {
            get { return _proxy; }
            set { _proxy = value; }
        }

        /// <summary>
        /// Ip сайта на который закачиваем пакет с данными
        /// </summary>
        public string DistHostIpAddress
        {
            get { return _distHostIpAddress; }
            set { _distHostIpAddress = value; }
        }

        /// <summary>
        /// Uri сайта на который закачиваем пакет с данными
        /// </summary>
        public string DistHostUriAddress
        {
            get { return _distHostUriAddress; }
            set { _distHostUriAddress = value; }
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
        /// Каталог сайта на удаленом сервере
        /// </summary>
        public string DistHostPath
        {
            get { return _distHostPath; }
            set { _distHostPath = value; }
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
        /// Контейнер куков
        /// </summary>
        public CookieContainer CookieContainer
        {
            get { return _cookieContainer; }
            set { _cookieContainer = value; }
        }
        #endregion

        public ReportsUploader()
        {
            //Выставляем значения по умолчанию
            this.SetValueOnDefault();
            //создаем коллекцию кукисов
            this.CookieContainer = new CookieContainer();
        }

        /// <summary>
        /// Выставляем значения по умолчанию
        /// </summary>
        private void SetValueOnDefault()
        {
            //адрес хоста
            this.DistHostIpAddress = string.Empty;
            //каталог сайта на удаленом сервере
            this.DistHostPath = string.Empty;

            this.EnabledAuthorization = false;
            this.UserLogin = string.Empty;
            this.UserPassword = string.Empty;
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
                Trace.Indent();
                //загрузили ip хоста
                this.DistHostIpAddress = settingsSection.UploaderDistHostIpAddress.Value;
                this.DistHostUriAddress = settingsSection.UploaderDistHostUriAddress.Value;
                //каталог сайта на удаленом сервере
                this.DistHostPath = settingsSection.UploaderDistHostPath.Value;

                this.EnabledAuthorization = settingsSection.UploaderDistHostAuthorization.Enabled;
                this.UserLogin = settingsSection.UploaderDistHostAuthorization.Login;
                this.UserPassword = settingsSection.UploaderDistHostAuthorization.Password;
            }
            catch (Exception e)
            {
                throw new Exception("Ошибка при загрузке настроек: " + e.Message);
            }
            finally
            {
                this.AddTraceInfo();
                Trace.Unindent();
            }
        }

        /// <summary>
        /// Информация для трасировки
        /// </summary>
        private void AddTraceInfo()
        {
            Trace.TraceInformation("Ip сайта на который закачиваем пакет с данными: {0}", this.DistHostIpAddress);
            Trace.TraceInformation("Uri сайта на который закачиваем пакет с данными: {0}", this.DistHostUriAddress);
            Trace.TraceInformation("Каталог сайта на удаленом сервере: {0}", this.DistHostPath);
            Trace.TraceInformation("Включена ли авторизация: {0}", this.EnabledAuthorization);
            Trace.TraceInformation("Логин: {0}", this.UserLogin);
        }

        /// <summary>
        /// Немного модифицированая версия Path.Combine(), просто адаптирована к Unix путям
        /// </summary>
        /// <param name="path1">первый путь</param>
        /// <param name="path2">втрой путь</param>
        /// <returns>результат объединения</returns>
        private string CombinePath(string path1, string path2)
        {
            string result = Path.Combine(path1, path2);
            if (result.Contains("/"))
                result = result.Replace('\\', '/');
            return result;
        }

        /// <summary>
        /// Начать загрузку файла
        /// </summary>
        /// <param name="fileName">имя файла</param>
        public void StartUploading(string fileName)
        {
            Trace.TraceInformation("Рулит Uploader.");
            Trace.Indent();
            //полный путь файла на удаленом сервере
            string fullDistHostFilePath = this.CombinePath(this.DistHostPath, Path.GetFileName(fileName));
            //Пока умеем закачивать только двумя способами, простыми копированием если это один домен
            //и по SSH на Unix хостинг
            if (this.IsInsideDomen())
                this.UploadByTCP(fileName, fullDistHostFilePath);
            else
                this.UploadBySSH(fileName, fullDistHostFilePath);
            Trace.Unindent();
        }

        /// <summary>
        /// Передача по TCP
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="fullDistHostFilePath"></param>
        private void UploadByTCP(string fileName, string fullDistHostFilePath)
        {
            Trace.TraceInformation(@"Проверили настройки, выяснили что закачивать далеко не надо, все находится
                    в одном домене, просто скопируем файл: {0}, в директорию: {1}", fileName, fullDistHostFilePath);
            try
            {
                if (File.Exists(fullDistHostFilePath))
                    File.Delete(fullDistHostFilePath);
                File.Copy(fileName, fullDistHostFilePath);
            }
            catch (Exception e)
            {
                throw new Exception("Не удалось скоприровать файл: " + e.Message);
            }
        }

        /// <summary>
        /// Передача данных по SSH протоколу
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="fullDistHostFilePath"></param>
        private void UploadBySSH(string fileName, string fullDistHostFilePath)
        {
            Trace.TraceInformation("Подготовка к загрузке, IP хоста: {0}, логин: {1}", this.DistHostIpAddress,
                this.UserLogin);
            Chilkat.SFtp sftp = new Chilkat.SFtp();
            sftp.EnableEvents = true;
            sftp.OnPercentDone += new Chilkat.SFtp.PercentDoneEventHandler(sftp_OnPercentDone);

            //введем ключ активации, если надо генерим тутА Tools\ChilKat\Keygen.exe
            if (!sftp.UnlockComponent("SSHT34MB34N_72FA33DD58nT"))
                throw new Exception(sftp.LastErrorText);

            //устанавливаем тайм ауты в милесекундах
            sftp.ConnectTimeoutMs = 5000;
            sftp.IdleTimeoutMs = 10000;
            //порт, через которое идет соединение
            int port = 22;
            //ip хоста
            string hostname = this.DistHostIpAddress;

            Trace.TraceInformation("Соединяемся с сервером.");
            if (!sftp.Connect(hostname, port))
                throw new Exception("Ошибка при соединение: " + sftp.LastErrorText);
            try
            {
                if (!sftp.AuthenticatePw(this.UserLogin, this.UserPassword))
                    throw new Exception("Ошибка при аутентификации: " + sftp.LastErrorText);

                if (!sftp.InitializeSftp())
                    throw new Exception("Ошибка при инциализации: " + sftp.LastErrorText);

                //куда копируем
                string handle = sftp.OpenFile(fullDistHostFilePath, "writeOnly", "createTruncate");
                if (handle == null)
                    throw new Exception("Ошибка при создании файла на сервере: " + sftp.LastErrorText);

                try
                {
                    long sizeFile = new FileInfo(fileName).Length;
                    this.OnTransferStart(fileName, fullDistHostFilePath, sizeFile);
                    //копируем
                    if (!sftp.UploadFile(handle, fileName))
                        throw new Exception("Не удалось передать файл: " + sftp.LastErrorText);
                    this.OnTransferEnd(fileName, fullDistHostFilePath);
                }
                finally
                {
                    sftp.CloseHandle(handle);
                }
            }
            finally
            {
                sftp.Disconnect();
            }
        }

        /// <summary>
        /// Лежит ли удаленый компьютер в одном домене с машиной с которой идет передача файлов
        /// </summary>
        /// <returns></returns>
        private bool IsInsideDomen()
        {
            //если строка с IP пустая, или локальный IP, или в пути к файлу нашли "\:" || "\\" значит 
            //передача будет идти в одном домене
            return ((this.DistHostIpAddress == string.Empty) || (this.DistHostIpAddress == "127.0.0.1") 
                || this.DistHostPath.Contains(@":\") || this.DistHostPath.Contains(@"\\"));
        }

        private string RequestCommand(UploaderOperation operation)
        {
            string result = string.Empty;
            string address = string.Format("{0}/servicePage.php?commandID={1}", this.DistHostUriAddress, (int)operation);

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(address);
            request.Proxy = this.Proxy;
            request.Timeout = requestTimeout;
            request.CookieContainer = this.CookieContainer;

            HttpWebResponse response = null;
            try
            {
                response = (HttpWebResponse)request.GetResponse();
            }
            catch (WebException e)
            {
                request.Abort();
                return string.Format("Error:{0} Status:{1}", e.Message, e.Status);
            }

            StreamReader streamReader = new StreamReader(response.GetResponseStream());
            result = streamReader.ReadToEnd();

            streamReader.Close();
            response.Close();
            request.Abort();

            return result;
        }

        /// <summary>
        /// Пробуем разобрать ответ, и вернуть текущее состояние операции, 
        /// если ответ завершен по таймауту, значит операция все еще выполняется
        /// </summary>
        /// <param name="value">строка с ответом с сервера</param>
        /// <param name="state">состояине операции</param>
        /// <returns></returns>
        private bool TryConvertToOperationState(string value, out OperationState state)
        {
            bool result = false;
            state = OperationState.FailedEnded;
            if (!string.IsNullOrEmpty(value))
            {
                try
                {
                    //если ответ завершен по таймауту, значит операция все еще выполняется
                    if (value.Contains("Status:Timeout"))
                        state = OperationState.Executing;
                    else
                        state = (OperationState)Enum.Parse(typeof(OperationState), value, true);
                    result = true;
                }
                catch { }
            }
            return result;
        }

        /// <summary>
        /// Выполнение операции
        /// </summary>
        /// <param name="operation">операция</param>
        /// <param name="rollOutPercent">процент исполнения всей операции развертывания</param>
        private void ExecuteCommand(UploaderOperation operation, int rollOutPercent)
        {
            OperationState operationState = OperationState.None;
            string responseStr = string.Empty;
            string error = string.Empty;

            this.RequestCommand(operation);
            do
            {
                Thread.Sleep(requestInterarrival);
                responseStr = this.RequestCommand(UploaderOperation.CheckState);
                if (!this.TryConvertToOperationState(responseStr, out operationState))
                    operationState = OperationState.FailedEnded;
             }
            while ((operationState != OperationState.FailedEnded) && (operationState != OperationState.SuccessEnded));

            if (operationState == OperationState.FailedEnded)
            {
                string serverlog = this.RequestCommand(UploaderOperation.Log);
                error = string.Format("Результа запроса: {0}\r\n Сообщение с сервера: {1}", 
                    responseStr, serverlog);
                throw new Exception(error);
            }

            this.OnRollOutOnDistHostProgress(rollOutPercent);
        }

        /// <summary>
        /// Развертывание пакета на сервере
        /// </summary>
        public void RollOutOnDistHost()
        {
            Trace.Indent();
            this.OnRollOutOnDistHostStart();

            OperationState operationState = OperationState.None;
            this.OnRollOutOnDistHostProgress(0);

            //получим сессию
            string responeStr = this.RequestCommand(UploaderOperation.StartSession);
            if (this.TryConvertToOperationState(responeStr, out operationState) 
                && (operationState == OperationState.Ready))
            {
                Trace.TraceInformation("Распаковывавем пакет с данными на PHP сайте");
                this.ExecuteCommand(UploaderOperation.ExtractDataBurst, 30);

                Trace.TraceInformation("Переносим отчеты из пакета в рабочий каталог");
                this.ExecuteCommand(UploaderOperation.UpdateReports, 60);

                Trace.TraceInformation("Обновляем данные в БД");
                this.ExecuteCommand(UploaderOperation.UpdateDB, 100);
            }
            else
                throw new Exception(string.Format("Сообщение с сервера: {0}", responeStr));

            Trace.Unindent();
            Trace.TraceInformation("Обновление данных на PHP web сервисе закончено.");
            this.OnRollOutOnDistHostEnd(true);
        }

        private void OnTransferStart(string src, string dst, long totalBytes)
        {
            Trace.TraceInformation("Начинаем трансферт файла: {0}, в катало хоста: {1}. Размер файла: {2} байт.",
                src, dst, totalBytes);
            if (_transferStart != null)
                _transferStart(this, src, dst, totalBytes);
        }

        private void OnTransferProgress(int percentDone)
        {
            Trace.TraceInformation("Процентов передано: {0}%", percentDone);
            if (_transferProgress != null)
                _transferProgress(this, percentDone);
        }

        private void OnTransferEnd(string src, string dst)
        {
            Trace.TraceInformation("Закончили трансферт файла: {0}, в катало хоста: {1}.", src, dst);
            if (_transferEnd != null)
                _transferEnd(this, src, dst);
        }

        private void OnRollOutOnDistHostStart()
        {
            if (_rollOutOnDistHostStart != null)
                _rollOutOnDistHostStart(this);
        }

        private void OnRollOutOnDistHostProgress(int precentDone)
        {
            if (_rollOutOnDistHostProgress != null)
                _rollOutOnDistHostProgress(this, precentDone);
        }

        private void OnRollOutOnDistHostEnd(bool success)
        {
            if (_rollOutOnDistHostEnd != null)
                _rollOutOnDistHostEnd(this, success);
        }

        void sftp_OnPercentDone(object sender, Chilkat.PercentDoneEventArgs args)
        {
            this.OnTransferProgress(args.PercentDone);
        }
    }

    /// <summary>
    /// Состояние операции развертывания пакета
    /// </summary>
    enum UploaderOperation
    {	
        /// <summary>
        /// распаковать архив
        /// </summary>
        ExtractDataBurst = 0,

        /// <summary>
        /// перенести отчеты из архива
        /// </summary>
		UpdateReports = 1,

        /// <summary>
        /// обновить информацю в базе данных
        /// </summary>
		UpdateDB = 2,

        /// <summary>
        /// проверить состояние выше перечисленных операций
        /// </summary>
        CheckState = 10,

        /// <summary>
        /// лог операции
        /// </summary>
        Log = 11,

        /// <summary>
        /// Получение сессии
        /// </summary>
        StartSession = 12
    }

    /// <summary>
    /// Состояние операции развертывания пакета
    /// </summary>
    enum OperationState
    {
        /// <summary>
        /// готова к запуску
        /// </summary>
        Ready = 0,

        /// <summary>
        /// начата
        /// </summary>
        Start = 1,

        /// <summary>
        /// выполняется
        /// </summary>
        Executing = 2,

        /// <summary>
        /// успешно завершена
        /// </summary>
        SuccessEnded = 3,
        
        /// <summary>
        /// завершена с ошибкой
        /// </summary>
        FailedEnded = 4,

        /// <summary>
        /// Нет
        /// </summary>
        None = 5
    }
}
