using System;
using System.Configuration;
using System.Web;
using System.Web.Configuration;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Krista.FM.Server.Dashboards.Common;

namespace Krista.FM.Server.Dashboards.Core
{
    public class ContactInformationHelper : ConfigurationSection
    {
        [ConfigurationProperty("title", IsRequired = true)]
        public string Title
        {
            get
            {
                return base["title"].ToString();
            }
        }

        [ConfigurationProperty("email", IsRequired = false)]
        public string Email
        {
            get
            {
                return base["email"].ToString();
            }
        }

        [ConfigurationProperty("phone", IsRequired = false)]
        public string Phone
        {
            get
            {
                return base["phone"].ToString();
            }
        }

        [ConfigurationProperty("externalLink", IsRequired = false)]
        public string ExternalLink
        {
            get
            {
                return base["externalLink"].ToString();
            }
        }

        [ConfigurationProperty("externalLinkText", IsRequired = false)]
        public string ExternalLinkText
        {
            get
            {
                return base["externalLinkText"].ToString();
            }
        }

        [ConfigurationProperty("elements", IsDefaultCollection = false)]
        public ContactElementsCollection ContactElements
        {
            get
            {
                ContactElementsCollection elementsCollection =
                (ContactElementsCollection)base["elements"];
                return elementsCollection;
            }
        }

        private static ContactInformationHelper instance;

        public static ContactInformationHelper Instance
        {
            get
            {
                if (instance == null)
                {
                    Configuration config = WebConfigurationManager.OpenWebConfiguration(HttpContext.Current.Request.ApplicationPath);
                    instance = (ContactInformationHelper)config.GetSection("contactInformation"); ;
                }
                return instance;
            }
        } 

        public Control GetContactInformationControl()
        {
            if (Instance.Title == null)
            {
                return null;
            }
            Page page = new Page();
            Control container = page.LoadControl("~/Components/ContainerPanel.ascx");
            ((IContainerPanel)container).AddContent(GetBody(Instance));
            ((IContainerPanel)container).AddHeader(Instance.Title);
            ((IContainerPanel)container).AddHeaderImage("../images/Partner.png");
            return container;
        }

        private HtmlGenericControl GetBody(ContactInformationHelper contactInformationSection)
        {
            HtmlGenericControl body = new HtmlGenericControl("div");

            // Эта проверка оставлена для совместимости со старой версией.
            if (contactInformationSection.ContactElements == null ||
                contactInformationSection.ContactElements.Count == 0)
            {
                body.Controls.Add(GetExternalSiteLink(this.ExternalLinkText, this.ExternalLink));
                body.Controls.Add(GetMailLink(this.Email));
                body.Controls.Add(GetPhoneText(this.Phone));
            }
            else
            {
                foreach (ContactInformationElement element in contactInformationSection.ContactElements)
                {
                    switch (element.Type)
                    {
                        case ContactInformationElementType.Phone:
                            {
                                body.Controls.Add(GetPhoneText(element.Value));
                                break;
                            }
                        case ContactInformationElementType.Email:
                            {
                                body.Controls.Add(GetMailLink(element.Value));
                                break;
                            }
                        case ContactInformationElementType.Hyperlink:
                            {
                                body.Controls.Add(GetExternalSiteLink(element.Value, element.Link));
                                break;
                            }
                        case ContactInformationElementType.Text:
                            {
                                body.Controls.Add(CustomText(element.Value));
                                break;
                            }
                    }
                }
            }
            return body;
        }

        private static HtmlGenericControl GetPhoneText(string phone)
        {
            HtmlGenericControl phoneControl = new HtmlGenericControl("span");
            phoneControl.Attributes.Add("class", "ReportDescription");
            phoneControl.InnerHtml = "<br /> тел.: " + phone;
            return phoneControl;
        }

        private static HyperLink GetMailLink(string Email)
        {
            HyperLink mailTo = new HyperLink();
            mailTo.Attributes.Add("class", "ReportDescription");
            mailTo.NavigateUrl = "mailto:" + Email;
            mailTo.Text = Email;
            return mailTo;
        }

        private static HtmlGenericControl GetExternalSiteLink(string externalLinkText, string externalLink)
        {
            HtmlGenericControl control = new HtmlGenericControl("span");
            control.InnerHtml = String.Format("<a href='{0}'>{1}</a><br/>", externalLink, externalLinkText);
            control.Attributes.Add("class", "ReportDescription");
            return control;
        }

        private static HtmlGenericControl CustomText(string text)
        {
            HtmlGenericControl textControl = new HtmlGenericControl("span");
            textControl.Attributes.Add("class", "ReportDescription");
            textControl.InnerHtml = text;
            return textControl;
        }
    }
       
    public class ContactElementsCollection : ConfigurationElementCollection
    {
        public override ConfigurationElementCollectionType CollectionType
        {
            get
            {
                return ConfigurationElementCollectionType.AddRemoveClearMap;
            }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new ContactInformationElement();
        }

        protected override Object GetElementKey(ConfigurationElement element)
        {
            return ((ContactInformationElement)element).Name;
        }
        
        new public ContactInformationElement this[string Name]
        {
            get
            {
                return (ContactInformationElement)BaseGet(Name);
            }
        }
    }
   
    public class ContactInformationElement : ConfigurationElement
    {
        [ConfigurationProperty("name", IsRequired = true, IsKey = true)]
        public string Name
        {
            get
            {
                return (string)base["name"];
            }
            set
            {
                base["name"] = value;
            }
        }

        [ConfigurationProperty("type", IsRequired = true)]
        public ContactInformationElementType Type
        {
            get
            {
                return (ContactInformationElementType)base["type"];
            }
            set
            {
                base["type"] = value;
            }
        }

        [ConfigurationProperty("value", IsRequired = true)]
        public string Value
        {
            get
            {
                return (string)base["value"];
            }
            set
            {
                base["value"] = value;
            }
        }

        [ConfigurationProperty("link", IsRequired = false)]
        public string Link
        {
            get
            {
                return (string)base["link"];
            }
            set
            {
                base["link"] = value;
            }
        }
    }

    public enum ContactInformationElementType
    {
        Text,
        Email,
        Hyperlink,
        Phone
    }
}
