using System;
using System.Configuration;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Server.Dashboards
{
    public partial class Autenticate : Page
    {
        protected void Page_PreInit(object sender, EventArgs e)
        {
            if (ConfigurationManager.AppSettings["SiteName"] != null)
            {
                Title = ConfigurationManager.AppSettings["SiteName"];
            }
            if (Request.Browser.Browser == "Firefox")
            {
                loginDiv.Style["top"] = "185px";
                loginDiv.Style["left"] = "101px";
            }
            if (RegionSettings.Instance != null)
            {
                CRHelper.SetPageTheme(this, RegionSettings.Instance.Id);
            }
            else
            {
                this.Theme = "Default";
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session[CustomReportConst.strPermissionErrorMessage] != null)
            {
                WriteErrorMessage(Session[CustomReportConst.strPermissionErrorMessage].ToString());
                Session[CustomReportConst.strPermissionErrorMessage] = null;
            }
        }

        protected void Login_Authenticate(object sender, AuthenticateEventArgs e)
        {
            try
            {
                // Новый пользователь - новые таблицы
                Session["allowedReports"] = null;
                Session["allowedReportsIPhone"] = null;
                ConnectionHelper.Connect(AuthenticationType.adPwdSHA512, Login.UserName, Login.Password);
                if (Login.RememberMeSet)
                {
                    // Сохраняем пользователя в кукисы
                    // SetCookies();
                }
                Server.Transfer(CustomReportConst.startPageUrl + "?" + Request.QueryString);
            }
            catch(Exception ex)
            {
                WriteErrorMessage(ex.Message);
                CRHelper.SaveToUserLog(string.Format("Неудачная попытка подключения: {0}; {1}", Login.UserName, ex.Message));
                CRHelper.SaveToErrorLog(string.Format("Неудачная попытка подключения: {0}; {1};", Login.UserName, CRHelper.GetExceptionInfo(ex)));
            }
        }

        /// <summary>
        /// Устанавливает кукисы.
        /// </summary>
        private void SetCookies()
        {
            HttpCookie cookie = new HttpCookie("Login");
            cookie[CustomReportConst.loginParamName] = Login.UserName;
            cookie[CustomReportConst.passwordParamName] = Login.Password;
            cookie.Expires = DateTime.Now.AddMonths(1);
            Response.Cookies.Add(cookie);
        }

        private void WriteErrorMessage(string message)
        {
            errorTable.Rows[0].Cells[0].Text = string.Format("{0}<br/><b>Неудачная попытка входа. Повторите попытку.</b>", message);
            errorTable.Visible = true;
            errorTableHeader.Visible = true;
        }
    }
}