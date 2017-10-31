using System;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Web;
using System.Web.UI;
using Krista.FM.Server.Dashboards.Common;

namespace Krista.FM.Server.Dashboards
{
    public partial class Start : Page
    {
        protected void Page_PreInit(object sender, EventArgs e)
        {
            if (ConfigurationManager.AppSettings["SiteName"] != null)
            {
                this.Title = ConfigurationManager.AppSettings["SiteName"];
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                CRHelper.BasePath = HttpContext.Current.Server.MapPath(HttpContext.Current.Request.ApplicationPath);
                // если где-то зарезали userAgent и нет параметра uplevel
                if (HttpContext.Current.Request.UserAgent == null &&
                        !(Session["IsUplevel"] != null && (bool)Session["IsUplevel"]))
                {
                    // то добавим в параметры сессии признак необходимости uplevel
                    Session["IsUplevel"] = true;
                }
                //  Иначе проверяем браузер
                else if (!CRHelper.AllowedBrowser())
                {
                    Session[CustomReportConst.strAppInformationMessage] = CustomReportConst.inadmissibleBrowserMessage;
                    Server.Transfer(CustomReportConst.userErrorPageUrl);
                }
            }

            string redirectPath = Request.Params["requestedUrl"] != null
                                      ? Request.Params["requestedUrl"].ToString()
                                      : CustomReportConst.indexPageUrl;

            if (Page.IsPostBack)
            {
                // Вариант на случай отсутствия кукисов.
                int width = Int32.Parse(screen_width.Value);
                int height = Int32.Parse(screen_height.Value);
                Session["width_size"] = (width <= CustomReportConst.minScreenWidth) ? CustomReportConst.minScreenWidth : width;
                Session["height_size"] = (height <= CustomReportConst.minScreenHeight) ? CustomReportConst.minScreenHeight : height;

                Session["MainIndexRef"] = CustomReportConst.indexPageUrl;

                Collection<string> removingKeys = new Collection<string>();
                removingKeys.Add("requestedUrl");
                removingKeys.Add("AspxAutoDetectCookieSupport");
                removingKeys.Add("paramlist");
                string query = CRHelper.RemoveParameterFromQuery(removingKeys, Request.QueryString);
                Response.Redirect(redirectPath + query);
            }
            
            if (Request.UserAgent != null)
            {
                bool FromiPhone =
                    Request.UserAgent.ToUpper().Contains("IPHONE") ||
                    Request.UserAgent.ToUpper().Contains("IMONITORING") ||
                    Request.UserAgent.ToUpper().Contains("IМОНИТОРИНГ");

                if (FromiPhone)
                {
                    Response.Redirect("~/iReports.xml");
                }
            }
        }
    }
}