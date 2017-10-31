using System;
using System.Configuration;
using System.Web.UI;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;

namespace Krista.FM.Server.Dashboards
{
    public partial class UserErrorMessage : Page
    {
        protected void Page_PreInit(object sender, EventArgs e)
        {
            if (ConfigurationManager.AppSettings["SiteName"] != null)
            {
                Title = ConfigurationManager.AppSettings["SiteName"];
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (ConfigurationManager.AppSettings[StatisticHelper.StatsScriptParamName] != null)
            {
                Page.ClientScript.RegisterClientScriptBlock(this.GetType(), StatisticHelper.StatsScriptName, StatisticHelper.GetStatScriptText(), false);
            }

            string informationText;
            if (Session[CustomReportConst.strAppInformationMessage] != null)
            {
                informationText = Session[CustomReportConst.strAppInformationMessage].ToString();
                AppErrorMessage.CssClass = "PageSubTitle";
            }
            else
            {
                Title = "Ошибка приложения";
                Response.Write(String.Format("<div><h1>{0}</h1><hr /></div>", Title));
                informationText = Session[CustomReportConst.strAppErrorMessage].ToString();
            }
            AppErrorMessage.Text = informationText;
        }
    }
}
