using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;

namespace Krista.FM.Server.Dashboards
{
    public partial class Logs : Page
    {
        protected void Page_PreInit(object sender, EventArgs e)
        {
            if (ConfigurationManager.AppSettings["SiteName"] != null)
            {
                Title = ConfigurationManager.AppSettings["SiteName"];
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
            this.CustomLogin1.Login.Authenticate += new AuthenticateEventHandler(Login_Authenticate);
            lbScheme.Text = "Ρυεμΰ: " + ConnectionHelper.CheckScheme();
            UpdateFileSizeLabels();
        }

        private void UpdateFileSizeLabels()
        {
            CrashLogFileSize.Text = String.Format("{0:N0} κα", GetFileSize(CustomReportConst.crashLogFileName));
            UserLogFileSize.Text = String.Format("{0:N0} κα", GetFileSize(CustomReportConst.userLogFileName));
            UserAgetnsLogFileSize.Text = String.Format("{0:N0} κα", GetFileSize(CustomReportConst.userAgentLogFileName));
            QueryLogFileSize.Text = String.Format("{0:N0} κα", GetFileSize(CustomReportConst.queryLogFileName));
            ServerLogFileSize.Text = String.Format("{0:N0} κα", GetFileSize(CustomReportConst.userServerLogFileName));
        }

        protected void Login_Authenticate(object sender, AuthenticateEventArgs e)
        {
            string[] adminLogin =
                    ConfigurationManager.AppSettings["AdminUser"].Split(';');
            if (adminLogin.Length == 2 &&
                adminLogin[0] == CustomLogin1.Login.UserName &&
                adminLogin[1] == CustomLogin1.Login.Password)
            {
                CustomLogin1.Visible = false;
                this.GetLogButtonsPanel.Visible = true;
            }
        }

        private void GetLogFile(string logName)
        {
            string path = Path.Combine(Server.MapPath("."), String.Format("logs\\{0}", logName));
            if (File.Exists(path))
            {
                Response.Clear();
                Response.Buffer = true;
                StringBuilder builder = new StringBuilder();
                builder.Append("attachment; ");
                builder.Append("filename=" + logName);
                Response.AddHeader("Content-Disposition", builder.ToString());
                Response.ContentType = "application/msword";
               // Response.ContentType = "text/plain";
                Response.ContentEncoding = Encoding.Default;
                Response.TransmitFile(path);
                Response.End();
            }
        }

        private int GetFileSize(string logName)
        {
            string path = Path.Combine(Server.MapPath("."), String.Format("logs\\{0}", logName));
            if (File.Exists(path))
            {
                return File.ReadAllBytes(path).Length / 1024;
            }
            return 0;
        }

        private void ClearLogFile(string logName)
        {
            string path = Path.Combine(Server.MapPath("."), String.Format("logs\\{0}", logName));
            File.Delete(path);
        }

        protected void GetCrashLog_Click(object sender, EventArgs e)
        {
            GetLogFile(CustomReportConst.crashLogFileName);
        }

        protected void GetUserLog_Click(object sender, EventArgs e)
        {
            GetLogFile(CustomReportConst.userLogFileName);
        }

        protected void GetUserAgentsLog_Click(object sender, EventArgs e)
        {
            GetLogFile(CustomReportConst.userAgentLogFileName);
        }

        protected void GetQueryLog_Click(object sender, EventArgs e)
        {
            GetLogFile(CustomReportConst.queryLogFileName);
        }

        protected void GetServerLog_Click(object sender, EventArgs e)
        {
            GetLogFile(Krista.FM.Server.Dashboards.Common.CustomReportConst.userServerLogFileName);
        }

        protected void ClearLogs_Click(object sender, EventArgs e)
        {
            ClearLogFile(CustomReportConst.crashLogFileName);
            ClearLogFile(CustomReportConst.userLogFileName);
            ClearLogFile(CustomReportConst.userAgentLogFileName);
            ClearLogFile(CustomReportConst.queryLogFileName);
            UpdateFileSizeLabels();
        }

        protected void CheckServer_Click(object sender, EventArgs e)
        {
            lbScheme.Text = "Ρυεμΰ: " + ConnectionHelper.CheckScheme();
        }
    }
}