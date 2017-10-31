using System;
using System.Web.SessionState;
using System.Web;
using System.Configuration;
using Krista.FM.Server.Dashboards.Common;

namespace Krista.FM.Server.Dashboards.Core
{
    /// <summary>
	/// Класс для поддержки ведения статистики посещения страниц сайта.
    /// </summary>
    public class StatisticHelper
    {
        /// <summary>
		/// Внутренне имя скрипта на странице.
        /// </summary>
        public const string StatsScriptName = "StatScript";
        
		/// <summary>
		/// Имя параметра из Web.config, содержащего путь к скрипту сбора статистики.
        /// </summary>
        public const string StatsScriptParamName = "StatisticScriptPath";

        /// <summary>
		/// Генерация текста скрипта внедряемого в страницу.
        /// </summary>
        /// <returns></returns>
        public static string GetStatScriptText()
        {
			// Если урл сайта статистики не задан, то ничего не делаем
			if (String.IsNullOrEmpty(ConfigurationManager.AppSettings[StatsScriptParamName]))
				return String.Empty;
            // По умолчанию все гости
            const string defaultUser = "guest";
            HttpSessionState session = HttpContext.Current.Session;
            string strLogin = defaultUser;
			// Проверяем создали ли сессию(на странице аутентификации может не работать)
            if (session != null)
            {
                // Пробуем вытянуть из сессии имя текущего пользователя(??? default.aspx, autenticate.aspx)
                if (session[CustomReportConst.currentUserKeyName] != null)
                {
                    strLogin = session[CustomReportConst.currentUserKeyName].ToString();
                    // если была доменная аутентификация
                    if (HttpContext.Current.User.Identity.IsAuthenticated)
                    {
                        strLogin = HttpContext.Current.User.Identity.Name;
                    }
                    // Если в урле есть параметры аутентификации
                    if (HttpContext.Current.Request.Params[CustomReportConst.loginParamName] != null)
                    {
                        strLogin = HttpContext.Current.Request.Params[CustomReportConst.loginParamName];
                    }
					// Доменный логин нужно порезать на части и взять последнюю
                    string[] strLoginParts = strLogin.Split('\\');
					strLogin = strLoginParts.Length > 0 
						? strLoginParts[strLoginParts.Length - 1] 
						: defaultUser;
                }
            }
            
			// Текст скрипта
        	return String.Format(
				"<script type='text/javascript' language='JavaScript' src='{0}?login={1}'></script>",
        	    ConfigurationManager.AppSettings[StatsScriptParamName], strLogin);
        }
    }
}
