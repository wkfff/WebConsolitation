using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.IO;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using Infragistics.WebUI.Misc;
using Infragistics.WebUI.UltraWebChart;
using Krista.Diagnostics;
using Krista.FM.Common;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Krista.FM.ServerLibrary;
using Krista.FM.ServerLibrary.TemplatesService;

namespace Krista.FM.Server.Dashboards.Core
{
    //Класс предок всех страниц сайта
    public class CustomReportPage : Page
    {
        /// <summary>
        /// Коллекция пользовательских параметров уровня сесси
        /// </summary>
        private CustomParams userParams;

        public CustomParams UserParams
        {
            get
            {
                if (userParams == null)
                {
                    userParams = new CustomParams();
                }
                return userParams;
            }
        }

        private string CurrentUser
        {
            get
            {
                if (Session[CustomReportConst.currentUserKeyName] != null)
                {
                    return Session[CustomReportConst.currentUserKeyName].ToString();
                }
                return string.Empty;
            }
        }

        #region Обработчики событий

        protected virtual void Page_PreInit(object sender, EventArgs e)
        {
            int start = Environment.TickCount;
            // если не нужно показывать параметры
            if (NeedHideParams)
            {
                Session["ShowParams"] = false;
            }
            // если отчет встроенный
            if (EmbeddedReport)
            {
                Session["Embedded"] = true;
                CRHelper.SetPageTheme(this, "Embed");
                // Session["PrintVersion"] = true;
            }

            // если в урле есть список параметров
            if (Request.Params["paramlist"] != null)
            {
                // передаем его на инициализацию
                InitializeUserParams();
            }
            // если нужен принудительный рендеринг
            if (NeedForcedRender())
            {
                Page.ClientTarget = "uplevel";
            }

            if (RegionSettings.Instance != null)
            {
                if (!EmbeddedReport)
                {
                    CRHelper.SetPageTheme(this, RegionSettings.Instance.Id);
                }
            }
            else
            {
                Theme = "Default";
            }
        }

        protected virtual void Page_PreLoad(object sender, EventArgs e)
        {
            int start = Environment.TickCount;
            if (Session[ConnectionHelper.SCHEME_KEY_NAME] == null || IsAuthorizationRequest)
            {
                if (IsAsyncPostBack())
                {
                    // Сделаем аварийную перезагрузку страницы.
                    Page.ClientScript.RegisterStartupScript(
                        GetType(), "EmergencySubmit", "getSize(); document.forms[0].submit();", true);
                    return;
                }
                // если не удалось создать серверную сессию
                if (!ConnectionHelper.CreateClientSession())
                {
                    // отправляем его на страничку аутентификации.
                    Server.Transfer(String.Format("{0}{1}{2}",
                                                  CustomReportConst.autenticatePageUrl,
                                                  Request.Url.Query, GetRequestedUrlQueryString()),
                                    true);
                }
                else if (!IsAuthorizationRequest)
                {
                    // отправим на начальную.
                    Server.Transfer(String.Format("{0}{1}{2}",
                                                  CustomReportConst.startPageUrl,
                                                  Request.Url.Query, GetRequestedUrlQueryString()),
                                                  true);
                }
                if (IsAuthorizationRequest)
                {
                    Response.Write("Authorization Complete");
                    Response.End();
                }

            }

            SetSizeParams();

            CheckPagePermission();

            if (!Page.IsPostBack && !EmbeddedReport &&
                (Session["Process"] == null ||
                (bool)Session["Process"]))
            {
                DataRow[] rows = AllowedReports.Select(string.Format("DOCUMENTFILENAME like '{0}'", ConvertToLocalReportURL(Request.Url.LocalPath)));
                if (rows.Length > 0)
                {
                    Session["currentReportName"] = rows[0]["NAME"].ToString();
                    Server.Transfer(String.Format("{0}{1}{2}", "~/PageProcessor.aspx",
                                                  Request.Url.Query, GetRequestedUrlQueryString()), true);
                }
            }

            CRHelper.SaveToUserLog(
                string.Format("Запрос отчета: {0}; {1};", CurrentUser, Request.Url.LocalPath));

            Session["PageLoadStart"] = Environment.TickCount;
        }

        /// <summary>
        /// Глобальный обработчик загрузки страницы
        /// </summary>
        protected virtual void Page_Load(object sender, EventArgs e)
        {
            if (ConfigurationManager.AppSettings[StatisticHelper.StatsScriptParamName] != null &&
                !IsAsyncPostBack())
            {
                Page.ClientScript.RegisterClientScriptBlock(GetType(), StatisticHelper.StatsScriptName,
                                                            StatisticHelper.GetStatScriptText(), false);
            }

            Core.Trace.TraceVerbose("Начало загрузки страницы {0}", Request.Url.LocalPath);
        }

        protected virtual void Page_LoadComplete(object sender, EventArgs e)
        {
            //  Response.Write(this.userParams.GetCurrentReportParamList());
            AddSuccessIndicator();
            SaveLoadCompleteToLog();

            DisposeProviderConnections();
            UserParams.UnlockParams();
            // Отключаем нестандартные режимы.
            Session["Embedded"] = false;
            Session["PrintVersion"] = false;
            Session["WordExportMode"] = false;
            Session["Process"] = true;
        }

        protected virtual void Page_Error(object sender, EventArgs e)
        {
            DisposeProviderConnections();
            UserParams.UnlockParams();
            // Отключаем нестандартные режимы.
            Session["Embedded"] = false;
            Session["PrintVersion"] = false;
            Session["WordExportMode"] = false;
            Session["Process"] = true;

            Exception ex = Server.GetLastError();
            Session[CustomReportConst.strAppErrorMessage] = ex.Message;
            CRHelper.SaveToErrorLog(KristaDiagnostics.ExpandException(ex));
            if (!IsAsyncPostBack())
            {
                Server.Transfer(CustomReportConst.userErrorPageUrl);
            }
        }

        #endregion

        /// <summary>
        /// Диспозит подключения провайдеров
        /// </summary>
        private static void DisposeProviderConnections()
        {
            DataProvidersFactory.PrimaryMASDataProvider.DisposeConnection();
            DataProvidersFactory.SecondaryMASDataProvider.DisposeConnection();
        }

        /// <summary>
        /// Сохраняет в лог факт выполнения отчета.
        /// </summary>
        private void SaveLoadCompleteToLog()
        {
            int executeTime = Environment.TickCount - (int)Session["PageLoadStart"];
            CRHelper.SaveToUserLog(
                string.Format("Выполнен отчет: {0}; {1}; {2}", CurrentUser, Request.Url.LocalPath, executeTime));
        }

        /// <summary>
        /// Добавляет на страницу индикатор успешности загрузки.
        /// </summary>
        private void AddSuccessIndicator()
        {
            HtmlInputHidden input = new HtmlInputHidden();
            input.ID = "successIndicator";
            Form.Controls.Add(input);
        }

        private void CheckPagePermission()
        {
            if (withoutCheckPermission)
            {
                return;
            }

            if (!ExistsInAllowedReports(AllowedReports, ConvertToLocalReportURL(Request.Url.LocalPath), false) &&
                !ExistsInAllowedReports(AllowedReportsIPhone, ConvertToIPhoneGroupUrl(Request.Url.LocalPath), false))
            {
                if (!LoginedAsGuest())
                {
                    Session[CustomReportConst.strPermissionErrorMessage] =
                        String.Format("Пользователю {0} запрещен доступ к странице {1}",
                                      CurrentUser, Request.Url.LocalPath);
                }
                Server.Transfer(String.Format("{0}{1}{2}",
                                              CustomReportConst.autenticatePageUrl,
                                              Request.Url.Query, GetRequestedUrlQueryString()),
                                true);
            }
        }

        private bool NeedForcedRender()
        {
            return Session["IsUplevel"] != null && (bool)Session["IsUplevel"];
        }

        private bool NeedHideParams
        {
            get
            {
                return Request.Params["showparams"] != null &&
                       Request.Params["showparams"].ToLower() == "no";
            }
        }


        private bool ExistsInAllowedReports(DataTable reportTable, string filter, bool substringSearch)
        {
            string substringInstruction = substringSearch ? "%" : String.Empty;
            DataRow[] rows = reportTable.Select(string.Format("DOCUMENTFILENAME like '{0}'", filter + substringInstruction));
            return rows.Length > 0;
        }

        private bool withoutCheckPermission
        {
            get
            {
                string path = Request.Url.LocalPath.ToLower();
                return path.Contains("/index.aspx") ||
                       path.Contains("reports/iphone/default.aspx") ||
                       path.Contains("reports/default.aspx") ||
                       path.Contains("/default.aspx") && !path.Contains("reports") ||
                       path.Contains("mobile/default.aspx") ||
                       path.Contains("/hotreports.aspx") ||
                       path.Contains("/embeddedreports.aspx") ||
                       path.Contains("/webnews.aspx");
            }
        }

        private string ConvertToLocalReportURL(string currentReport)
        {
            int trimNum = Request.ApplicationPath.Length == 1 ? 1 : Request.ApplicationPath.Length + 1;
            return currentReport.Remove(0, trimNum);
        }

        private string ConvertToIPhoneGroupUrl(string currentReport)
        {
            int trimNum = Request.ApplicationPath.Length == 1 ? 1 : Request.ApplicationPath.Length + 1;
            currentReport = currentReport.Remove(0, trimNum);
            string[] urlSplitted = currentReport.Split('/');
            string convertedUrl = String.Empty;
            for (int i = 0; i < urlSplitted.Length - 1; i++)
            {
                convertedUrl += urlSplitted[i] + "/";
            }
            return convertedUrl;
        }

        private bool IsAuthorizationRequest
        {
            get
            {
                return Request.Params["isAuthorizationRequest"] != null &&
                       Request.Params["isAuthorizationRequest"] == "true";
            }
        }

        /// <summary>
        /// Является ли отчет встроенным
        /// </summary>
        private bool EmbeddedReport
        {
            get
            {
                // Да, если это указано в урле или сессии
                return (Request.Params["embedded"] != null &&
                        Request.Params["embedded"].ToLower() == "yes") ||
                       (Session["Embedded"] != null &&
                        (bool)Session["Embedded"]);
            }
        }


        private void InitializeUserParams()
        {
            string paramlist = Request.Params["paramlist"];
            CustomParams.InitializeParamsFromList(paramlist);

            // Параметры проинициализировались, но остался парамлист.
            // Если это первый запрос сайта, он порежется на дефолтной странице,
            // Иначе он останется в урле и не даст рулить параметрами.
            // Отрежем его и редиректнем на себя.
            Collection<string> removingKeys = new Collection<string>();
            removingKeys.Add("paramlist");
            string query = CRHelper.RemoveParameterFromQuery(removingKeys, Request.QueryString);
            Response.Redirect(Request.FilePath + query, true);
        }



        private string GetRequestedUrlQueryString()
        {
            return Request.Url.Query ==
                   string.Empty
                       ? string.Format("?requestedUrl={0}", Request.Url.LocalPath)
                       : string.Format("&requestedUrl={0}", Request.Url.LocalPath);
        }

        private bool IsAsyncPostBack()
        {
            bool ass = false;
            IsAsyncPostBack(Controls, ref ass);
            return ass;
        }

        private void IsAsyncPostBack(ControlCollection controls, ref bool isAsync)
        {
            foreach (Control control in controls)
            {
                if (control is WebAsyncRefreshPanel &&
                    ((WebAsyncRefreshPanel)control).IsAsyncPostBack)
                {
                    isAsync = true;
                    return;
                }
                IsAsyncPostBack(control.Controls, ref isAsync);
            }
        }

        private void SetSizeParams()
        {
            SetSizeParamValue(CustomReportConst.ScreenWidthKeyName, CustomReportConst.minScreenWidth);
            SetSizeParamValue(CustomReportConst.ScreenHeightKeyName, CustomReportConst.minScreenHeight);
            if (Session[CustomReportConst.ScreenWidthKeyName] != null)
            {
                Session[CustomReportConst.ScreenWidthKeyName] =
                    Int32.Parse(Session[CustomReportConst.ScreenWidthKeyName].ToString()) - 15;
            }
            if (Session[CustomReportConst.ScreenHeightKeyName] != null)
            {
                Session[CustomReportConst.ScreenHeightKeyName] =
                    Int32.Parse(Session[CustomReportConst.ScreenHeightKeyName].ToString()) - 10;
            }
        }

        private void SetSizeParamValue(string paramName, int minValue)
        {
            // Если значение задано в конфиге
            if (ConfigurationManager.AppSettings[paramName] != null)
            {
                // Берем из конфига и уходим
                Session[paramName] = int.Parse(ConfigurationManager.AppSettings[paramName]);
                return;
            }
            // Если в кукисах не пусто
            if (Request.Cookies != null)
            {
                if (Request.Cookies[paramName] != null)
                {
                    // Возьмем из кукисов
                    HttpCookie cookie = Request.Cookies[paramName];
                    int value = Int32.Parse(cookie.Value);
                    // сверим с минимумом
                    value = (value <= minValue) ? minValue : value;
                    Session[paramName] = value;
                    return;
                }
            }
            // Если параметр уже был проинициализирован
            if (Session[paramName] != null)
            {
                // Оставим старый.
                return;
            }
            // В крайнем случае возьмем минимум.
            Session[paramName] = minValue;
        }

        #region Рендеринг

        public void RenderElement(UltraChart chart, string elementKey, DataTable dt)
        {
            if ((chart != null) && (dt != null))
            {
                string workDir = Server.MapPath(".");

                DataProvidersFactory.PrimaryMASDataProvider.WorkDir = workDir;


#warning need error handler

                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText(elementKey),
                                                                                 "Series Name", dt);

                chart.Visible = (dt != null);
                if (dt == null) return;


                chart.DataSource = dt;
                chart.DataBind();


                string reportElementPreset = string.Empty;
                string presetFileName = string.Format("{0}\\{1}.preset", workDir, elementKey);
                if (File.Exists(presetFileName))
                {
                    reportElementPreset = File.ReadAllText(presetFileName);

                    //Адаптация пресета с вин на веб
                    reportElementPreset =
                        reportElementPreset.Replace("DataBindings-DefaultDataSourceUpdateMode=\"OnValidation\"",
                                                    "BorderColor=\"Black\" BorderWidth=\"1px\" BackColor=\"White\"");
                }

                StringReader stringReader = new StringReader(reportElementPreset);
                chart.LoadPreset(stringReader, true);
                stringReader.Close();
            }
        }

        #endregion

        private static Dictionary<string, string> kdKinds;

        public static Dictionary<string, string> KDKindsDictionary
        {
            get
            {
                if (kdKinds == null || kdKinds.Count == 0)
                {
                    // заполняем словарик
                    FillKDKindsDictionary();
                }
                return kdKinds;
            }
        }

        private static void FillKDKindsDictionary()
        {
            kdKinds = new Dictionary<string, string>();

            kdKinds.Add("Налоговые доходы", "[КД].[Сопоставимый].[Налоговые доходы]");
            kdKinds.Add("Налог на прибыль", "[КД].[Сопоставимый].[Налог на прибыль]");
            kdKinds.Add("НДФЛ", "[КД].[Сопоставимый].[НДФЛ]");
            kdKinds.Add("Налоги на имущество", "[КД].[Сопоставимый].[Налоги на имущество_]");
            kdKinds.Add("Акцизы", "[КД].[Сопоставимый].[Акцизы_]");
            kdKinds.Add("НДПИ", "[КД].[Сопоставимый].[НДПИ]");
            kdKinds.Add("Неналоговые доходы", "[КД].[Сопоставимый].[Неналоговые доходы]");
            kdKinds.Add("Доходы от предпринимательской деятельности",
                        "[КД].[Сопоставимый].[Доходы от предпринимательской деятельности]");
            kdKinds.Add("Налоговые и неналоговые доходы", "[КД].[Сопоставимый].[Налоговые и неналоговые доходы_]");
            kdKinds.Add("Безвозмездные поступления", "[КД].[Сопоставимый].[Безвозмездн. поступления_]");
            kdKinds.Add("Доходы ВСЕГО", " [КД].[Сопоставимый].[Доходы ВСЕГО]");
        }

        #region кукисы

        /// <summary>
        /// Устанавливает кукисы.
        /// </summary>
        /// <param name="paramName">Имя параметра.</param>
        /// <param name="paramValue">Значение параметра.</param>
        protected void SetCookies(string paramName, string paramValue)
        {
            HttpCookie cookie = new HttpCookie(CustomReportConst.cookieSetName);
            cookie[paramName] = paramValue;
            cookie.Expires = DateTime.Now.AddMonths(1);
            if (Response.Cookies != null)
            {
                Response.Cookies.Add(cookie);
            }
        }

        /// <summary>
        /// Извлекает кукисы.
        /// </summary>
        /// <param name="paramName">Имя параметра.</param>
        /// <returns>Значение параметра.</returns>
        protected string GetCookies(string paramName)
        {
            if (Request.Cookies != null)
            {
                HttpCookie cookie = Request.Cookies[CustomReportConst.cookieSetName];
                if (cookie != null)
                {
                    string value = cookie[paramName];
                    return CRHelper.ConvertEncoding(value, Request.ContentEncoding, Encoding.Default);
                }
            }
            return string.Empty;
        }

        #endregion

        #region Таблицы отчетов

        protected DataTable AllowedReports
        {
            get
            {
                if (Session["allowedReports"] == null ||
                    ((DataTable)Session["allowedReports"]).Rows.Count == 0)
                {
                    GetAllowedReports();
                }
                return (DataTable)Session["allowedReports"];
            }
        }

        protected DataTable AllReports
        {
            get
            {
                if (Session["allReports"] == null ||
                    ((DataTable)Session["allReports"]).Rows.Count == 0)
                {
                    GetAllReports();
                }
                return (DataTable)Session["allReports"];
            }
        }

        private static void GetAllowedReports()
        {
            DataTable allowedReports = GetAllowedReportsTable(TemplateTypes.Web);
            HttpContext.Current.Session["allowedReports"] = allowedReports;
        }

        private static void GetAllowedReportsIPhone()
        {
            DataTable allowedReports = GetAllowedReportsTable(TemplateTypes.IPhone);
            HttpContext.Current.Session["allowedReportsIPhone"] = allowedReports;
        }

        protected static void GetAllReports()
        {
            DataTable allowedReports = GetAllReportsTable(TemplateTypes.Web);
            HttpContext.Current.Session["allReports"] = allowedReports;
        }

        private static DataTable GetAllReportsTable(TemplateTypes templateType)
        {
            try
            {
                HttpSessionState sessionState = HttpContext.Current.Session;
                LogicalCallContextData cnt =
                    sessionState[ConnectionHelper.LOGICAL_CALL_CONTEXT_DATA_KEY_NAME] as LogicalCallContextData;
                if (cnt != null)
                    LogicalCallContextData.SetContext(cnt);
                IScheme scheme = (IScheme)sessionState[ConnectionHelper.SCHEME_KEY_NAME];
                return scheme.TemplatesService.Repository.GetTemplatesInfo(templateType);
            }
            catch (Exception e)
            {
                CRHelper.SaveToErrorLog(CRHelper.GetExceptionInfo(e));
                throw new Exception("Не удалось получить список отчетов.", e);
            }
            finally
            {
                CallContext.SetData("Authorization", null);
            }
        }

        private static DataTable GetAllowedReportsTable(TemplateTypes templateType)
        {
            try
            {
                HttpSessionState sessionState = HttpContext.Current.Session;
                LogicalCallContextData cnt =
                    sessionState[ConnectionHelper.LOGICAL_CALL_CONTEXT_DATA_KEY_NAME] as LogicalCallContextData;
                if (cnt != null)
                    LogicalCallContextData.SetContext(cnt);
                IScheme scheme = (IScheme)sessionState[ConnectionHelper.SCHEME_KEY_NAME];
                DataTable dt = scheme.TemplatesService.Repository.GetTemplatesInfo(templateType);
                DataTable allowedReports = dt.Clone();
                DataRow[] rows = dt.Select("IsVisible = True");

                for (int i = 0; i < rows.Length; i++)
                {
                    allowedReports.ImportRow(rows[i]);
                }
                return allowedReports;
            }
            catch (Exception e)
            {
                CRHelper.SaveToErrorLog(CRHelper.GetExceptionInfo(e));
                throw new Exception("Не удалось получить список отчетов.", e);
            }
            finally
            {
                CallContext.SetData("Authorization", null);
            }
        }

        /// <summary>
        /// Получение полного пути к отчету по его коду
        /// </summary>
        /// <param name="code">код отчета</param>
        /// <returns>путь к отчету</returns>
        public string GetReportFullName(string code)
        {
            if (AllowedReports != null && AllowedReports.Rows.Count > 0)
            {
                foreach (DataRow row in AllowedReports.Rows)
                {
                    if (row["CODE"] != DBNull.Value && row["DOCUMENTFILENAME"] != DBNull.Value && row["CODE"].Equals(code))
                    {
                        return String.Format("~/{0}", row["DOCUMENTFILENAME"]);
                    }
                }
            }
            return String.Empty;
        }

        protected DataTable AllowedReportsIPhone
        {
            get
            {
                if (Session["allowedReportsIPhone"] == null ||
                    ((DataTable)Session["allowedReportsIPhone"]).Rows.Count == 0)
                {
                    GetAllowedReportsIPhone();
                }
                return (DataTable)Session["allowedReportsIPhone"];
            }
        }

        #endregion

        protected static bool LoginedAsGuest()
        {
            if (ConfigurationManager.AppSettings[CustomReportConst.GuestUserKeyName] != null)
            {
                string[] guestLogin =
                    ConfigurationManager.AppSettings[CustomReportConst.GuestUserKeyName].Split(';');
                string currentUserName = HttpContext.Current.Session[CustomReportConst.currentUserKeyName].ToString();
                if (guestLogin.Length == 2 &&
                    guestLogin[0] == currentUserName)
                {
                    return true;
                }
            }
            return false;
        }
    }
}