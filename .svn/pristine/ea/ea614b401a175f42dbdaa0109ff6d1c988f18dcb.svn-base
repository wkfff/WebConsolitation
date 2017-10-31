namespace Krista.FM.Server.Dashboards.Common
{
	public struct CustomReportConst
	{
        
        // Файл содержащий тексты запросов в рабочей директории        
        public const string QueryFileName = "queries.mdx";
        public const string QueryFileMasc = "*.mdx";

        ///ИМЕНА ПЕРЕМЕННЫХ В КОЛЛЕКЦИИ СЕССИИ

        /// <summary>
        /// Логин пользователя
        /// </summary>
        public const string currentUserKeyName = "CurrentUser";
        /// <summary>
        /// Фамилия пользователя
        /// </summary>
        public const string currentUserSurnameKeyName = "CurrentUserSurname";
        /// <summary>
        /// Описание пользователя
        /// </summary>
        public const string currentUserDescriptionKeyName = "CurrentUserDescription";

	    ///ИМЕНА ПЕРЕМЕННЫХ В КОЛЛЕКЦИИ ПРИЛОЖЕНИЯ
	
        //приложение запущено нормально
        public const string boolAppIsOK = "IsApplicationRunCorrect";
        // ошибка приложения.
        public const string strAppErrorMessage = "AppErrorMessage";
        // ошибка прав доступа.
        public const string strPermissionErrorMessage = "PermissionErrorMessage";
        // информация приложения.
        public const string strAppInformationMessage = "AppInformationMessage";
		
	    //имя лога запросов
        public const string queryLogFileName = "Query.log";
	    //имя лога ошибок
        public const string crashLogFileName = "Crash.log";
        //имя лога пользователей
        public const string userLogFileName = "User.log";
        //имя лога пользователей
        public const string userAgentLogFileName = "UserAgent.log";
        //имя лога пользователей
        public const string userServerLogFileName = "Server.log";
		
		//разделитель в лог-файле
		public const string logSeparator = "-----------------------------------";
		
		//СООБЩЕНИЯ ОБ ОШИБКАХ
		public const string errTableFormating = "Ошибка при форматировании таблицы";

        //Имя cookie-набора.
        public const string cookieSetName = "CustomReports";

        //АДРЕСА ОСНОВНЫХ СТРАНИЦ
        public const string startPageUrl = "~/Default.aspx";
        public const string autenticatePageUrl = "~/Logon.aspx";
        public const string userErrorPageUrl = "~/UserError.aspx";
        public const string indexPageUrl = "~/Index.aspx";

        //ИМЕНА СПЕЦИАЛЬНЫХ ПАРАМЕТРОВ НАСТРОЙКИ
        public const string BrowserCompatibilityKeyName = "BrowserCompatibility";
        public const string ScreenWidthKeyName = "width_size";
        public const string ScreenHeightKeyName = "height_size";
        public const string GuestUserKeyName = "GuestUser";
        public const string ExternalRefKeyName = "ExternalRef";
        public const string MinScreenWidthKeyName = "MinWidthSize";
        public const string MinScreenHeightKeyName = "MinHeightSize";
	    public const string BootloadServiceNameKeyName = "BootloadServiceName";

      //  public const int minScreenWidth = 1240;
       // public const int minScreenHeight = 850;

        public const string loginParamName = "login";
        public const string passwordParamName = "password";

	    public const string inadmissibleBrowserMessage =
	        "Для просмотра ресурса необходимо использовать браузеры Internet Explorer (версия 7 или старше), Safari (версия 3 или старше) или Mozilla Firefox (версия 3 или старше). <br/>Минимально рекомендуемое разрешение экрана 1280x1024. <br/>При меньшем разрешении экрана или в других браузерах отчеты могут отображаться и работать некорректно.";

        public const string ServerURL = "tcp://{0}/FMServer/Server.rem";
        
        public static int minScreenWidth
        {
            get
            {
                int screenWidth;
                if (int.TryParse(
                        System.Configuration.ConfigurationManager.AppSettings[MinScreenWidthKeyName], 
                        out screenWidth))
                {
                    return screenWidth;
                }
                else
                {
                    return 1240;
                }
            }
        }

        public static int minScreenHeight
        {
            get
            {
                int screenHeight;
                if (int.TryParse(
                        System.Configuration.ConfigurationManager.AppSettings[MinScreenHeightKeyName], 
                        out screenHeight))
                {
                    return screenHeight;
                }
                else
                {
                    return 850;
                }
            }
        }
	}
}