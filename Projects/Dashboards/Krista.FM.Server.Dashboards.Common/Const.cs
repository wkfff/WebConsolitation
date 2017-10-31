namespace Krista.FM.Server.Dashboards.Common
{
	public struct CustomReportConst
	{
        
        // ���� ���������� ������ �������� � ������� ����������        
        public const string QueryFileName = "queries.mdx";
        public const string QueryFileMasc = "*.mdx";

        ///����� ���������� � ��������� ������

        /// <summary>
        /// ����� ������������
        /// </summary>
        public const string currentUserKeyName = "CurrentUser";
        /// <summary>
        /// ������� ������������
        /// </summary>
        public const string currentUserSurnameKeyName = "CurrentUserSurname";
        /// <summary>
        /// �������� ������������
        /// </summary>
        public const string currentUserDescriptionKeyName = "CurrentUserDescription";

	    ///����� ���������� � ��������� ����������
	
        //���������� �������� ���������
        public const string boolAppIsOK = "IsApplicationRunCorrect";
        // ������ ����������.
        public const string strAppErrorMessage = "AppErrorMessage";
        // ������ ���� �������.
        public const string strPermissionErrorMessage = "PermissionErrorMessage";
        // ���������� ����������.
        public const string strAppInformationMessage = "AppInformationMessage";
		
	    //��� ���� ��������
        public const string queryLogFileName = "Query.log";
	    //��� ���� ������
        public const string crashLogFileName = "Crash.log";
        //��� ���� �������������
        public const string userLogFileName = "User.log";
        //��� ���� �������������
        public const string userAgentLogFileName = "UserAgent.log";
        //��� ���� �������������
        public const string userServerLogFileName = "Server.log";
		
		//����������� � ���-�����
		public const string logSeparator = "-----------------------------------";
		
		//��������� �� �������
		public const string errTableFormating = "������ ��� �������������� �������";

        //��� cookie-������.
        public const string cookieSetName = "CustomReports";

        //������ �������� �������
        public const string startPageUrl = "~/Default.aspx";
        public const string autenticatePageUrl = "~/Logon.aspx";
        public const string userErrorPageUrl = "~/UserError.aspx";
        public const string indexPageUrl = "~/Index.aspx";

        //����� ����������� ���������� ���������
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
	        "��� ��������� ������� ���������� ������������ �������� Internet Explorer (������ 7 ��� ������), Safari (������ 3 ��� ������) ��� Mozilla Firefox (������ 3 ��� ������). <br/>���������� ������������� ���������� ������ 1280x1024. <br/>��� ������� ���������� ������ ��� � ������ ��������� ������ ����� ������������ � �������� �����������.";

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