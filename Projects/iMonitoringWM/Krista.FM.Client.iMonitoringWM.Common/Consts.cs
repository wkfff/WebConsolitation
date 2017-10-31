using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Krista.FM.Client.iMonitoringWM.Common
{
    static public class Consts
    {
        #region Версия приложения
        private const int major = 1;
        private const int minor = 0;
        private const int build = 2;

        static public string VersionNumber
        {
            get { return string.Format("{0}.{1}.{2}", major, minor, build); }
        }
        #endregion

        #region константы для сборки ReportsBootloader

        //Строка агента, под которым качаем данные
        public const string userAgent = @"Mozilla/5.0 (Macintosh; U; Intel Mac OS X 10_5_6; ru-ru) AppleWebKit/528.16 (KHTML, like Gecko) Version/4.0 Safari/528.16";

        //имена групп в регулярных выражениях
        public const string urlResources = "urlResources";
        public const string srcResources = "srcResources";
        public const string hrefResources = "hrefResources";
        public const string deployDate = "deployDate";

        /// <summary>
        /// имя группы с куками в заголовке http запроса
        /// </summary>
        public const string headerCookieName = "Cookie";

        /// <summary>
        /// страница на которой получем сессию
        /// </summary>
        public const string sessionPage = "getSessionID.php";
        /// <summary>
        /// страница на которой логинимся и получаем серверные данные для пользователя
        /// </summary>
        public const string loginPage = "login_action.php";
        /// <summary>
        /// страница с которой получаем ссылку на закачку отчетов
        /// </summary>
        public const string getReportPage = "getReport.php";

        /// <summary>
        /// если в теле отчета содержится данный признак, значит отчет без ошибок
        /// </summary>
        public const string successHTMLIndicator = "successIndicator";
        /// <summary>
        /// имя узла с настройками приложения
        /// </summary>
        public const string settigsNodeName = "webReportsSettings";

        public const string resourcesFolderExtension = ".files";
        static public string[] imageExtesions = new string[] { ".jpg", ".gif", ".png", ".bmp" };
        static public string[] scriptExtesions = new string[] { ".axd", ".js" };
        static public string[] cssExtesions = new string[] { ".css" };
        #endregion;

        #region Имена файлов и дерикторий  
        /// <summary>
        /// Имя папки с закэшироваными отчетами
        /// </summary>
        public const string reportsFolderName = "Reports";
        /// <summary>
        /// Имя папки с кэшем на устройстве
        /// </summary>
        public const string cacheFolderName = "Cache";
        /// <summary>
        /// Имя карты памяти
        /// </summary>
        public const string storageCardName = "Storage Card";
        public const string storageCardRusName = "Карта памяти";
        public const string storageSDCardName = "SD card";
        #endregion    

        #region Работа с базой данной
        /// <summary>
        /// Имя базы данной
        /// </summary>
        public const string dbName = "iMonDatabase";

        public const string dbTempName = "iMonDatabase_";

        /// <summary>
        /// Таблица с настройками пользователя
        /// </summary>
        public const string dbUsersTable = "users";

        /// <summary>
        /// Дата последней удачной загрузки отчета в кэш
        /// </summary>
        public const string dbReportsDownloadDate = "reports_download_date";

        /// <summary>
        /// Cписок отчетов
        /// </summary>
        public const string dbUserReports = "reports_list";

        /// <summary>
        /// Список субъектов РФ
        /// </summary>
        public const string dbEntityList = "entity_list";

        /// <summary>
        /// Настройки приложения
        /// </summary>
        public const string dbAppSettings = "app_settings";

        /// <summary>
        /// Разделитель используемый при формировании значений
        /// </summary>
        public const string dbSeparator = "|";
        #endregion

        #region Настройки приложения по умолчанию
        /// <summary>
        /// Логин
        /// </summary>
        public const string defaultLogin = "guest";
        /// <summary>
        /// Пароль
        /// </summary>
        public const string defaultPassword = "guest";
        /// <summary>
        /// Хост
        /// </summary>
        public const string defaultHost = "http://www.imon.krista.ru";//"http://fedorov.krista.ru/NewHost";//"http://fedorov.krista.ru/Yaroslavl";//
        /// <summary>
        /// Автономная работа
        /// </summary>
        public const bool defaultOffLineOperation = false;
        /// <summary>
        /// Загрузка кэша
        /// </summary>
        public const bool defaultDownloadCache = false;
        #endregion

        #region Сообщения
        /// <summary>
        /// Сообщение от том что пользователь прервал загрузку отчетов
        /// </summary>
        public const string msAbortDownloading = "AbortDownloading";
        public const string msMissingReport = "Отчет отсутствует в кэше";
        public const string msFaultAuthentication = "Пользователь не авторизован.";
        public const string msFaultConnectToServ = "Невозможно соединиться с сервером, проверьте интернет соединение, и правильность указания источника данных в настройках.";
        public const string msReportAbsentByHost = "Не удалось загрузить отчет: {0}";
        #endregion

        #region О программе
        public const string supportNumber1 = "88002002072";
        public const string wmPage = "http://www.krista.ru/products/FM/WM";
        public const string kristaPage = "http://www.krista.ru";
        #endregion
    }
}
