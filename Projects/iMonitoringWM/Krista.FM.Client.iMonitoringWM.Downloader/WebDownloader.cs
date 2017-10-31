using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Net;
using System.Xml;
using System.Threading;

using Krista.FM.Client.iMonitoringWM.Common;
using System.Windows.Forms;

namespace Krista.FM.Client.iMonitoringWM.Downloader
{
    public class WebDownloader
    {
        #region Делегаты
        public delegate void DownloadCompletedHandler(object sender, string reportid, string errorText, 
            string infoText, bool isSuccess);
        public delegate void StartDownloadingResourcesHandler(object sender, int resourcesCount);
        public delegate void StartResourceHandler(object sender, int numberResource);
        public delegate void EndResourceHandler(object sender, int numberResource, bool success);
        public delegate void DownloadServerDataCompleteHandler(object sender, XmlDocument serverData, string errorText);
        public delegate void ConnectToHostCompleteHandler(object sender, bool success, string errorText);
        #endregion
        
        #region События
        private event DownloadCompletedHandler _downloadCompleted;
        private event StartDownloadingResourcesHandler _startDownloadingResources;
        private event StartResourceHandler _startResource;
        private event EndResourceHandler _endResource;
        private event DownloadServerDataCompleteHandler _downloadServerDataComplete;
        private event ConnectToHostCompleteHandler _connectToHostComplete;
        /// <summary>
        /// Закончили загрузку
        /// </summary>
        public event DownloadCompletedHandler DownloadCompleted
        {
            add { _downloadCompleted += value; }
            remove { _downloadCompleted -= value; }
        }

        /// <summary>
        /// Начали загрузку ресурсов
        /// </summary>
        public event StartDownloadingResourcesHandler StartDownloadingResources
        {
            add { _startDownloadingResources += value; }
            remove { _startDownloadingResources -= value; }
        }

        /// <summary>
        /// Начало загрузки ресурса
        /// </summary>
        public event StartResourceHandler StartResource
        {
            add { _startResource += value; }
            remove { _startResource -= value; }
        }

        /// <summary>
        /// Окончание загрузки ресурса
        /// </summary>
        public event EndResourceHandler EndResource
        {
            add { _endResource += value; }
            remove { _endResource -= value; }
        }

        /// <summary>
        /// Загрузка данных пользователя завершена
        /// </summary>
        public event DownloadServerDataCompleteHandler DownloadServerDataComplete
        {
            add { _downloadServerDataComplete += value; }
            remove { _downloadServerDataComplete -= value; }
        }

        /// <summary>
        /// Соединение с сервером завершено
        /// </summary>
        public event ConnectToHostCompleteHandler ConnectToHostComplete
        {
            add { _connectToHostComplete += value; }
            remove { _connectToHostComplete -= value; }
        }
        #endregion

        #region Поля
        private MatchCollection resourcesList;
        private List<ResourceInfo> resourcesInfo;
        //для асинхронной загрузки отчета
        private string savePath;
        private string address;
        private string reportID;
        private string reportCaption;
        //для асинхоной загрузки данных с сервера
        string reportsHostAddress;
        string login;
        string password;

        private string reportName;
        private string resourcesFolder;
        private string resourcesPath;
        private string _customScriptsPath;
        private string sourceHtmlBody;
        private bool _isOptimazeHTML;
        private StringBuilder _errorText;
        private StringBuilder _infoText;
        private Uri absoluteUrl;
        private int currentResourcesNumber;
        private bool _isDownloading;

        private Thread downloaderThread;
        #endregion

        #region Свойства
        public StringBuilder ErrorText
        {
            get { return _errorText; }
            set { _errorText = value; }
        }

        public StringBuilder InfoText
        {
            get { return _infoText; }
            set { _infoText = value; }
        }

        /// <summary>
        /// Путь к ресурсу с пользовательскими скриптами
        /// </summary>
        public string CustomScriptsPath
        {
            get { return _customScriptsPath; }
            set { _customScriptsPath = value; }
        }

        /// <summary>
        /// Оптимизировать HTML (удяляет лишние пробелы, переносы строк, и т.д.)
        /// </summary>
        public bool IsOptimazeHTML
        {
            get { return _isOptimazeHTML; }
            set { _isOptimazeHTML = value; }
        }

        /// <summary>
        /// Идет ли загрузка в данный момент
        /// </summary>
        public bool IsDownloading
        {
            get { return _isDownloading; }
            set { _isDownloading = value; }
        }
        #endregion
        
        public WebDownloader()
        {
            this.ErrorText = new StringBuilder();
            this.InfoText = new StringBuilder();
            this.IsDownloading = false;
        }

        private void ClearResourcesInfo()
        {
            if (this.resourcesInfo == null)
                this.resourcesInfo = new List<ResourceInfo>();
            else
                this.resourcesInfo.Clear();
        }

        /// <summary>
        /// Асинхроное получение данных пользователя с сервера
        /// </summary>
        /// <param name="reportsHostAddress">адрес хоста</param>
        /// <param name="login">логин</param>
        /// <param name="password">пароль</param>
        public void AsynchGetServerData(string reportsHostAddress, string login, string password)
        {
            this.reportsHostAddress = reportsHostAddress;
            this.login = login;
            this.password = password;

            downloaderThread = new Thread(new ThreadStart(this.AsynchGetServerData));
            downloaderThread.Priority = ThreadPriority.Normal;
            downloaderThread.Start();
        }

        private void AsynchGetServerData()
        {
            string error = string.Empty;
            this.GetServerData(this.reportsHostAddress, this.login, this.password, out error);
        }

        /// <summary>
        /// Получает id сессии
        /// </summary>
        /// <param name="reportsHostAddress">адрес хоста</param>
        /// <returns></returns>
        private string GetSessionID(string reportsHostAddress, out string errorText)
        {
            string result = string.Empty;
            errorText = string.Empty;
            string address = Utils.CombineUri(reportsHostAddress, Consts.sessionPage);
            //address = @"http://fedorov.krista.ru/NewHost/getSessionID.php?use_remote=1&debug_session_id=1000&start_debug=1&debug_start_session=1&debug_host=10.0.200.33%2C127.0.0.1&debug_no_cache=1258535629766&debug_fastfile=1&debug_port=10137&send_sess_end=1&original_url=http://fedorov.krista.ru/NewHost/getSessionID.php&debug_stop=1"; 
            try
            {
                XmlDocument responseXml = this.DownloadXml(address);
                if (responseXml != null)
                {
                    XmlNode sessionNode = responseXml.SelectSingleNode("iMonitoring/sessionID");
                    result = sessionNode.Attributes["value"].Value;
                }
            }
            catch (Exception e)
            {
                errorText = e.Message;
            }

            this.OnConnectToHostComplete(errorText);
            return result;
        }

        private XmlDocument GetUserData(string reportsHostAddress, string login, string password, string sessionID,
            out string errorText)
        {
            XmlDocument result = null;
            errorText = string.Empty;
            //сессионный пароль
            string sessionPassword = this.GetSessionPassword(password, sessionID).ToString();
            //путь к странице авторизации и получения настроек пользователя
            string loginPageAddress = Utils.CombineUri(reportsHostAddress, Consts.loginPage);
            //тип устройства
            string deviceType = this.GetDeviceType();

            HttpWebRequest request = null;
            try
            {
                //loginPageAddress = @"http://fedorov.krista.ru/NewHost/login_action.php?use_remote=1&debug_session_id=1000&start_debug=1&debug_start_session=1&debug_host=10.0.200.33%2C127.0.0.1&debug_no_cache=1258535629766&debug_fastfile=1&debug_port=10137&send_sess_end=1&original_url=http://fedorov.krista.ru/NewHost/login_action.php&debug_stop=1";
                //содержимое запроса
                string queryContent = string.Format("login={0}&password={1}&mobileDeviceType={2}",
                    login, sessionPassword, deviceType);
                //создаем запрос
                request = ConnectionHelper.GetHttpWebRequest(loginPageAddress, queryContent);
                //выполняем его
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    //вытащим из ответа сессии пользователя
                    ConnectionHelper.ParseSessionsIdFromCookie(response);
                    //узнаем куда переадресует нас хост
                    string location = response.Headers["Location"];
                    response.Close();
                    if (!string.IsNullOrEmpty(location))
                    {
                        string responseUri = Utils.GetUriWithoutQuery(response.ResponseUri.ToString());
                        string locationUri = Utils.CombineUri(responseUri, location);
                        result = this.DownloadXml(locationUri);
                    }
                }
            }
            catch (Exception e)
            {
                //errorText = "Неудалось получить данные пользователя";
                errorText = e.Message;
            }
            finally
            {
                if (request != null)
                    request.Abort();
            }
            this.OnDownloadServerDataComplete(result, errorText);
            return result;
        }

        /// <summary>
        /// Получаем данные пользователя
        /// </summary>
        /// <param name="reportsHostAddress">адрес хоста</param>
        /// <param name="login">логин</param>
        /// <param name="password">пароль</param>
        /// <param name="errorText">текст ошибки при получения данных с сервера(если возникнет)</param>
        /// <returns></returns>
        public XmlDocument GetServerData(string reportsHostAddress, string login, string password, 
            out string errorText)
        {
            errorText = string.Empty;
            //получаем ID сессии
            string sessionID = this.GetSessionID(reportsHostAddress, out errorText);

            if (sessionID == string.Empty)
            {
                this.OnDownloadServerDataComplete(null, errorText);
                return null;
            }
            
            return this.GetUserData(reportsHostAddress, login, password, sessionID, out errorText);
        }

        /// <summary>
        /// Асинхронное сохраненине html отчета на диске
        /// </summary>
        /// <param name="address">uri</param>
        /// <param name="savePath">пусть на диске</param>
        /// <param name="reportID">id отчета</param>
        public void AsynchSaveToHTML(string address, string savePath, string reportID, string reportCaption)
        {
            this.address = address;
            this.savePath = savePath;
            this.reportID = reportID;
            this.reportCaption = reportCaption;

            downloaderThread = new Thread(new ThreadStart(this.AsynchSaveToHTML));
            //для того что бы пользователь мог без подтормаживания перемещатся
            //по ленте с отчетами, снизим приоритет у нити, загружающей данные
            downloaderThread.Priority = ThreadPriority.Lowest;
            downloaderThread.Start();
        }

        /// <summary>
        /// Прекратить текущую загрузку HTML
        /// </summary>
        public void AbortAsynchSaveHTML()
        {
            if (downloaderThread != null)
            {
                try
                {
                    this.downloaderThread.Abort();
                    this.downloaderThread = null;
                }
                catch
                {
                }
                this.OnDownloadCompleted(new StringBuilder(), new StringBuilder(Consts.msAbortDownloading),
                    false);
            }
        }

        private void AsynchSaveToHTML()
        {
            this.SaveToHTML(this.address, this.savePath, this.reportID);
        }

        /// <summary>
        /// Сохранение html отчета на диске
        /// </summary>
        /// <param name="address">uri</param>
        /// <param name="savePath">пусть на диске</param>
        /// <param name="reportID">id отчета</param>
        /// <returns>успешна ли загрузка</returns>
        public bool SaveToHTML(string address, string savePath, string reportID)
        {
            this.IsDownloading = true;
            bool result = false;

            this.absoluteUrl = null;
            this.ErrorText.Remove(0, this.ErrorText.Length);
            this.InfoText.Remove(0, this.InfoText.Length);
            this.ClearResourcesInfo();

            this.address = address;
            this.savePath = savePath;
            this.reportID = reportID;

            if (string.IsNullOrEmpty(address) || string.IsNullOrEmpty(savePath))
            {
                this.ErrorText.Append("Некорректный формат параметров");
                this.OnDownloadCompleted(this.ErrorText, this.InfoText, false);
                return result;
            }

            //имя отчета
            this.reportName = Path.GetFileNameWithoutExtension(savePath);
            //имя папки в которой сохраняются ресурсы
            this.resourcesFolder = this.reportName + Consts.resourcesFolderExtension;
            //полный путь к папке с ресурсами
            this.resourcesPath = Path.Combine(Path.GetDirectoryName(savePath), this.resourcesFolder);

            //очистим, или создадим новую
            this.InitResourcesFolder(this.resourcesPath);

            //скачиваем тело отчета
            if ((this.DownloadHTMLBody(address) || (this.DownloadHTMLBody(address))) && this.IsValidHTML(this.sourceHtmlBody))
            {
                this.DetectResources();
                result = true;
            }
            else
            {
                this.InfoText.Append(string.Format("{0}: {1}", "Путь сохранения отчета", this.resourcesPath));
                this.InfoText.Append(string.Format("{0}: {1}", "Абсолютный адресс отчета", this.address));
                this.InfoText.Append(string.Format("{0}: {1}", "Тело страницы", this.sourceHtmlBody));

                this.ErrorText.AppendLine(string.Format(Consts.msReportAbsentByHost, this.reportCaption));
                this.OnDownloadCompleted(this.ErrorText, this.InfoText, false);
            }
            return result;
        }

        private bool IsValidHTML(string html)
        {
            return html.Contains(Consts.successHTMLIndicator);
        }

        private bool DownloadHTMLBody(string address)
        {
            this.InfoText.AppendLine("===================================");
            this.InfoText.AppendLine(string.Empty);
            this.InfoText.AppendLine("Имя отчета: " + this.reportName);
            this.InfoText.AppendLine("Адрес сохранения: " + this.savePath);
            this.InfoText.AppendLine("Начало загрузки тела страницы, адрес: " + address);

            HttpWebRequest request = null;
            try
            {
                //создаем запрос
                request = ConnectionHelper.GetHttpWebRequest(address);
                //получем ответ
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    //вытащим из ответа сессии пользователя
                    ConnectionHelper.ParseSessionsIdFromCookie(response);

                    string locationUri = response.Headers["Location"];
                    //При загрузке тела страницы, запрос идет всегда к странице getReportID,
                    //которая в свою очередь определяет где находится отчет, на этом или соседнем сервере,
                    //и только после этого выдает абсолютный путь
                    if (!string.IsNullOrEmpty(locationUri) && locationUri.Contains("getReport"))//locationUri.Contains("http")
                    {
                        //если в пути содержится "getReport" значит отчет находится на соседнем сервере, идем 
                        //туда, и проходим весь путь заного
                        response.Close();
                        return this.DownloadHTMLBody(locationUri);
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(locationUri))
                            locationUri = response.ResponseUri.ToString();
                        else
                        {
                            string responseUri = Utils.GetUriWithoutQuery(response.ResponseUri.ToString());
                            if (!locationUri.StartsWith(responseUri))
                                locationUri = Utils.CombineUri(responseUri, locationUri);
                        }
                    }

                    this.absoluteUrl = new Uri(locationUri);
                    this.sourceHtmlBody = this.DownloadString(locationUri);
                    response.Close();
                }
            }
            catch
            {
                return false;
            }
            finally
            {
                request.Abort();
            }

            this.InfoText.AppendLine("Загрузили тело страницы.");
            return true;
        }

        private XmlDocument DownloadXml(string address)
        {
            XmlDocument result = null;
            string strXml = this.DownloadString(address);
            if (!string.IsNullOrEmpty(strXml))
            {
                result = new XmlDocument();
                result.LoadXml(strXml);
            }
            return result;
        }

        private string DownloadString(string address)
        {
            string result = string.Empty;
            
            HttpWebRequest request = null;
            try
            {
                //создаем запрос
                request = ConnectionHelper.GetHttpWebRequest(address);
                //получем ответ
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    //вытащим из ответа сессии пользователя
                    ConnectionHelper.ParseSessionsIdFromCookie(response);

                    result = this.ExtractString(response);
                    response.Close();
                }
            }
            finally
            {
                request.Abort();
            }
            return result;
        }

        /// <summary>
        /// Извлекает из веб ответа содержимое в виде строки
        /// </summary>
        /// <param name="response">веб ответ</param>
        /// <returns></returns>
        private string ExtractString(HttpWebResponse response)
        {
            string result = string.Empty;
            if (response != null)
            {
                try
                {
                    using (StreamReader stream = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                    {
                        result = stream.ReadToEnd();

                        stream.Close();
                        stream.Dispose();
                    }
                }
                catch
                {
                    using (Stream stream = response.GetResponseStream())
                    {
                        int contentLenght = (int)response.ContentLength;

                        if (contentLenght > 0)
                        {
                            byte[] bytes = new byte[contentLenght];
                            int numBytesToRead = contentLenght;
                            int numBytesRead = 0;
                            while (numBytesToRead > 0)
                            {
                                int n = stream.Read(bytes, numBytesRead, numBytesToRead);
                                if (n == 0)
                                    break;
                                numBytesRead += n;
                                numBytesToRead -= n;
                            }
                            Encoding enc = new UTF8Encoding();
                            result = enc.GetString(bytes, 0, bytes.Length);
                        }
                        stream.Close();
                        stream.Dispose();
                    }
                }
            }

            return result;
        }

        private bool DownloadFile(Uri requestUri, string savePath)
        {
            //создаем запрос
            HttpWebRequest request = ConnectionHelper.GetHttpWebRequest(requestUri.ToString());
            HttpWebResponse response = null;
            try
            {
                //получем ответ
                response = (HttpWebResponse)request.GetResponse();
                //вытащим из ответа сессии пользователя
                ConnectionHelper.ParseSessionsIdFromCookie(response);
            }
            catch (WebException e)
            {
                this.ErrorText.AppendLine(string.Format("Отчет: {0}. Адрес ресурса: {1}. Ошибка: {2}.", 
                    this.reportName, requestUri.ToString(), e.Message));
                this.OnEndResource(this.currentResourcesNumber, false);
                return false;
            }

            Stream stream = response.GetResponseStream();

            int contentLenght = (int)response.ContentLength;
            this.InfoText.AppendLine("    Размер полученого ресурса: " + contentLenght + "kb");

            if (contentLenght > 0)
            {
                try
                {
                    // сюда будем записывать
                    using (FileStream fs = new FileStream(savePath, FileMode.Create, FileAccess.Write))
                    {
                        using (BinaryWriter writer = new BinaryWriter(fs))
                        {
                            byte[] buffer = new byte[contentLenght]; // буфер
                            int len;       // количество прочитанных байт

                            if (stream.CanSeek)
                                stream.Position = 0;

                            while ((len = stream.Read(buffer, 0, buffer.Length)) != 0)
                                writer.Write(buffer, 0, len);
                            writer.Close();
                        }
                        fs.Close();
                        fs.Dispose();
                    }
                    this.InfoText.AppendLine("    Ресурс успешно сохранен.");
                    this.OnEndResource(this.currentResourcesNumber, true);
                }
                catch (Exception e)
                {
                    this.ErrorText.AppendLine(string.Format("   Отчет: {0}. Путь сохранения: {1}. Ошибка: {2}.",
                        this.reportName, savePath, e.Message));
                    this.OnEndResource(this.currentResourcesNumber, false);
                }
            }

            stream.Close();
            response.Close();
            request.Abort();
            return true;
        }

        private void InitResourcesFolder(string folder)
        {
            try
            {
                if (Directory.Exists(folder))
                    Directory.Delete(folder, true);
                Directory.CreateDirectory(folder);
            }
            catch (Exception e)
            {
                this.ErrorText.AppendLine("Отчет: " + this.reportName);
                this.ErrorText.AppendLine("Ошибка при инициализации папки ресурсов: " + e.Message);
            }
        }

        private void FillResourcesInfo()
        {
            for (int i = 0; i < this.resourcesList.Count; i++)
            {
                Group group = this.GetResourcesGroup(i);
                this.resourcesInfo.Add(new ResourceInfo(group, this.absoluteUrl, this.resourcesFolder));
            }
        }

        /// <summary>
        /// После того как скачили тело страницы, парсим ее на наличие ресурсов
        /// </summary>
        private void DetectResources()
        {
            this.resourcesList = HTMLParser.GetResources(this.sourceHtmlBody);
            this.InfoText.AppendLine("Количество ресурсов: " + this.resourcesList.Count);
            this.OnStartResourcesDownloading(this.resourcesList.Count);

            if (this.resourcesList != null)
            {
                this.currentResourcesNumber = 0;
                this.FillResourcesInfo();
                this.DownloadResources(this.currentResourcesNumber);
            }
        }

        private void SaveHTMLBody()
        {
            //перед сохранением заменим новыми значениями ссылки на ресурсы
            string newBody = HTMLParser.SetResources(this.sourceHtmlBody, this.resourcesInfo);
            Utils.WriteAllText(this.savePath, newBody);
            this.InfoText.AppendLine("В теле отчета заменили адреса ресурсов на новые, и сохранили его");
            bool isSuccess = (this.ErrorText.ToString() == string.Empty);
            this.OnDownloadCompleted(this.ErrorText, this.InfoText, isSuccess);
        }

        private void DownloadResources(int resourceNumber)
        {
            if (resourceNumber > this.resourcesInfo.Count - 1)
            {
                //Это значит что все ресурсы закончились, сохраняем тело отчета
                this.SaveHTMLBody();
                return;
            }

            ResourceInfo info = this.resourcesInfo[resourceNumber];
            if (info.Url != null)
            {
                this.InfoText.AppendLine(string.Empty);
                this.InfoText.AppendLine("    Номер ресурса: " + resourceNumber);
                this.InfoText.AppendLine("    Начало загрузки ресурса, адрес: " + info.Url.ToString());
                this.OnStartResource(resourceNumber);
                //если уже качали файл с такого ресурса, будем качать следующий
                if (this.IsNeedDownload(resourceNumber))
                {
                    info.Name = this.GetValidName(resourceNumber);
                    string path = Path.Combine(this.resourcesPath, info.Name);
                    this.InfoText.AppendLine("    Новый адрес ресурса: " + info.NewValue);

                    info.SuccessLoaded = this.DownloadFile(info.Url, path);

                    //скачиваем следующий ресурс
                    this.DownloadResources(++this.currentResourcesNumber);
                }
                else
                {
                    this.InfoText.AppendLine("    Ресурс с такого адреса уже скачен. Имя ресурса: " + info.Name);
                    this.OnEndResource(this.currentResourcesNumber, true);
                    this.DownloadResources(++this.currentResourcesNumber);
                }
            }
        }

        private string GetValidName(int resourcesNumber)
        {
            ResourceInfo resource = this.resourcesInfo[resourcesNumber];

            string fileName = Path.GetFileNameWithoutExtension(resource.Name);
            string extension = Path.GetExtension(resource.Name);
            bool isNameExist = false;

            //IIS отказывается возвращать ресурсы с разрешением .axd, в нашем случае там содрежатся
            //скрипты, поэтому меняем его на .js
            if (extension == ".axd")
                extension = ".js";

            string result = fileName + extension;

            int i = 1;
            do
            {
                isNameExist = this.IsExistName(result, resourcesNumber);
                if (isNameExist)
                {
                    result = string.Format("{0}({1}){2}", fileName, i, extension);
                    i++;
                }
            }
            while (isNameExist);

            return result;
        }

        private bool IsExistName(string name, int resourcesNumber)
        {
            for (int i = 0; i < resourcesNumber; i++)
            {
                if (this.resourcesInfo[i].Name == name)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// получить сумму символов (ASCI таблицы) строки 
        /// </summary>
        /// <param name="str">строка</param>
        /// <returns>сумма символов строки</returns>
        public int GetCharSumm(string str)
        {
            int result = 0;
            if (!string.IsNullOrEmpty(str))
            {
                foreach (char ch in str)
                {
                    result += (int)ch;
                }
            }
            return result;
        }

        /// <summary>
        /// Пулучаем произведение сумм пароля и сесии
        /// </summary>
        /// <param name="password">пароль пользователя</param>
        /// <param name="sessionID">ID сесии</param>
        /// <returns></returns>
        public int GetSessionPassword(string password, string sessionID)
        {
            int pasSum = this.GetCharSumm(password);
            int sessionSum = this.GetCharSumm(sessionID);
            return pasSum * sessionSum;
        }

        /// <summary>
        /// Вернет строку с типом устройства, и его разрешением
        /// </summary>
        /// <returns></returns>
        public string GetDeviceType()
        {
            string result = "WM";
            switch (Utils.ScreenSize)
            {
                case ScreenSizeMode.s240x320:
                    {
                        result += "240x320";
                        break;
                    }
                case ScreenSizeMode.s480x640:
                    {
                        result += "480x640";
                        break;
                    }
                case ScreenSizeMode.s480x800:
                    {
                        result += "480x800";
                        break;
                    }
            }
            return result;
        }

        /// <summary>
        /// Смотрит требуется ли скачивание, может быть его скачали ранее
        /// </summary>
        /// <param name="resourcesNumber"></param>
        /// <returns></returns>
        private bool IsNeedDownload(int resourcesNumber)
        {
            ResourceInfo resource = this.resourcesInfo[resourcesNumber];

            for (int i = 0; i < resourcesNumber; i++)
            {
                ResourceInfo temp = this.resourcesInfo[i];
                if (temp.SuccessLoaded && (temp.Url.ToString() == resource.Url.ToString()))
                {
                    resource.Name = temp.Name;
                    resource.SuccessLoaded = true;
                    return false;
                }
            }
            return true;
        }

        private Group GetResourcesGroup(int index)
        {
            Group result = null;
            if (index > this.resourcesList.Count - 1)
                return result;

            Match match = this.resourcesList[index];
            result = match.Groups[Consts.srcResources];
            if (this.IsGroupEmpty(result))
            {
                result = match.Groups[Consts.urlResources];
                if (this.IsGroupEmpty(result))
                    result = match.Groups[Consts.hrefResources];
            }
            
            return result;
        }

        private bool IsGroupEmpty(Group group)
        {
            return ((group == null) || ((group != null) && (group.Value == string.Empty)));
        }

        private void OnDownloadCompleted(StringBuilder errorText, StringBuilder infoText, bool isSuccess)
        {
            this.IsDownloading = false;
            this.downloaderThread = null;

            if (_downloadCompleted != null)
                _downloadCompleted(this, this.reportID, errorText.ToString(), infoText.ToString(), isSuccess);
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

        public void OnDownloadServerDataComplete(XmlDocument serverData, string error)
        {
            if (_downloadServerDataComplete != null)
                _downloadServerDataComplete(this, serverData, error);
        }

        public void OnConnectToHostComplete(string error)
        {
            if (_connectToHostComplete != null)
                _connectToHostComplete(this, error == string.Empty, error);
        }

        #region Обработчики событий
        #endregion
    }
}
