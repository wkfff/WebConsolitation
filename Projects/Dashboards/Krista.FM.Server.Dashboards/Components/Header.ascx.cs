using System;
using System.Configuration;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using Krista.FM.Server.Dashboards.Common;


namespace Krista.FM.Server.Dashboards.Components
{
    public partial class Header : UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["PrintVersion"] != null && (bool)Session["PrintVersion"])
            {
                this.Visible = false;
            }
            if (Session["IsUplevel"] != null && (bool)Session["IsUplevel"])
            {
                SetBrowserAttentionText();
            }
            if (Session["Embedded"] != null && (bool)Session["Embedded"])
            {
                HideControls();
            }
            if (!IsSmallResolution)
            {
                headerTitle.Attributes.Add("class", "PageHeaderTitle");
            }
            else
            {
                headerTitle.Attributes.Add("class", "PageHeaderTitleShort");
            }

            SetCurrentUserName();
            SetExternalRef();

            if (!Request.UserAgent.ToLower().Contains("ipad"))
            {
                forumLinkRef.Attributes.Add("target=", "_blank");
                forumLinkRef2.Attributes.Add("target=", "_blank");
            }

            if (Page.Theme != "Minfin")
            {
                forumlink.Visible = false;
                forumlink.Style.Add("display", "none");
                forumlink2.Visible = false;
                forumlink2.Style.Add("display", "none");

                Image1.Visible = false;
                Image2.Visible = false;
            }
        }

        private void HideControls()
        {
            this.Button1.Visible = false;
            this.Button2.Visible = false;
            this.CurrentUser.Visible = false;
        }

        #warning Дублирование метода из CustomReportPage
        private string GetUserName()
        {
            Button1.Text = "Выход";
            if (ConfigurationManager.AppSettings[CustomReportConst.GuestUserKeyName] != null)
            {
                string[] guestLogin =
                    ConfigurationManager.AppSettings[CustomReportConst.GuestUserKeyName].Split(';');
                string currentUserName = HttpContext.Current.Session[CustomReportConst.currentUserKeyName].ToString();
                if (guestLogin.Length == 2 &&
                    guestLogin[0] == currentUserName)
                {
                    Button1.Text = "Вход";
                    return "гость";
                }
            }
            return HttpContext.Current.Session[CustomReportConst.currentUserSurnameKeyName].ToString();
        }

        private void SetCurrentUserName()
        {
            CurrentUser.Text = string.Format("&nbsp;Пользователь:&nbsp;{0}&nbsp;", GetUserName());
        }

        private void SetBrowserAttentionText()
        {
            browserError.Text =
                "Не удалось определить тип браузера, поэтому отчеты могут отображаться и работать некорректно. Попробуйте использовать другой браузер.";
            browserError.Visible = true;
            browserError.ToolTip =
                "Не удалось определить тип браузера, поэтому отчеты могут отображаться и работать некорректно. Попробуйте использовать другой браузер.\n" +
                "В некоторых случаях это может быть связано с настройками прокси-сервера.\n" +
                "Для просмотра ресурса необходимо использовать браузеры Internet Explorer (версия 7 или старше), Safari (версия 3 или старше) или Mozilla Firefox (версия 3 или старше).";
                
            browserError.Font.Bold = true;
        }

        private void SetExternalRef()
        {
            Button2.PostBackUrl = CustomReportConst.indexPageUrl;
            if (Request.RawUrl.ToString().ToLower().Contains("index.aspx"))
            {
                SetCssClass(headerTitle);
                SetCssClass(pageHeader);
                SetCssClass(pageHeaderControls);
                SetCssClass(pageHeaderRight);
                SetCssClass(pageHeaderBody);

                Button1.CssClass = String.Format("{0}Index", Button1.CssClass);
                Button2.CssClass = String.Format("{0}Index", Button2.CssClass);

                if (ConfigurationManager.AppSettings[CustomReportConst.ExternalRefKeyName] != null)
                {
                    string[] externalSiteRef =
                        ConfigurationManager.AppSettings[CustomReportConst.ExternalRefKeyName].ToString().Split(';');
                    if (externalSiteRef.Length == 2)
                    {
                        Button2.Text = externalSiteRef[0];
                        Button2.PostBackUrl = externalSiteRef[1];
                        return;
                    }                
                }
                Button2.Visible = false;
            }
            else
            {
                forumlink.Visible = false;
                forumlink.Style.Add("display", "none");
                forumlink2.Visible = false;
                forumlink2.Style.Add("display", "none");

                Image1.Visible = false;
                Image2.Visible = false;
            }
        }

        private void SetCssClass(HtmlGenericControl control)
        {
            control.Attributes["class"] = String.Format("{0}Index", control.Attributes["class"]);
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            Session.Abandon();
            Server.Transfer(CustomReportConst.autenticatePageUrl);
        }

        protected void Button2_Click(object sender, EventArgs e)
        {
            Response.Redirect(((Button)sender).PostBackUrl);
        }
        
        private bool IsSmallResolution
        {
            get { return CRHelper.GetScreenWidth < 1000; }
        }
    }
}