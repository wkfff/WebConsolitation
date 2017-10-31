using System;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Net;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Xml;
using System.Xml.Serialization;
using System.Runtime.InteropServices;
using System.Reflection;

using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.PlaningServiceProxy
{
    /// <summary>
    /// Обертка для доступа к веб сервису. Получена из кода, сгенерированного визардом "Добавить веб-ссылку"
    /// путем удаления лишнего. Правильнее было бы использовать утилиту SoapSuds, 
    /// но у меня она с первого раза не заработала. Надо разбираться.
    /// </summary>
    [ComVisible(false)]
    [DesignerCategoryAttribute("code")]
    [WebServiceBindingAttribute(Name = "PlaningServiceSoap", Namespace = "http://tempuri.org/")]
    [XmlIncludeAttribute(typeof(object[]))]
    public partial class PlaningServiceWrapper : SoapHttpClientProtocol
    {

        private bool useDefaultCredentialsSetExplicitly;

        public PlaningServiceWrapper()
        {
            // включаем поддержку Cookie
            this.CookieContainer = new CookieContainer();

            #region Заглушка. Стандартный механизм System.Configuration.ConfigurationManager.AppSettings[...] не работает для DLL
            string settingsPath = Assembly.GetExecutingAssembly().CodeBase + ".config";
            XmlDocument doc = new XmlDocument();
            doc.Load(settingsPath);
            XmlNode urlNode = doc.DocumentElement.SelectSingleNode("appSettings/add[@key=\"WebServiceURL\"]");
            string url = urlNode.Attributes.GetNamedItem("value").InnerXml;
            #endregion

            this.Url = url;
            //if ((this.IsLocalFileSystemWebService(this.Url) == true))
            //{
                this.UseDefaultCredentials = true;
                this.useDefaultCredentialsSetExplicitly = false;
            /*}
            else
            {
                this.useDefaultCredentialsSetExplicitly = true;
            }*/
        }

        public new string Url
        {
            get
            {
                return base.Url;
            }
            set
            {
                if ((((this.IsLocalFileSystemWebService(base.Url) == true)
                            && (this.useDefaultCredentialsSetExplicitly == false))
                            && (this.IsLocalFileSystemWebService(value) == false)))
                {
                    base.UseDefaultCredentials = false;
                }
                base.Url = value;
            }
        }

        public new bool UseDefaultCredentials
        {
            get
            {
                return base.UseDefaultCredentials;
            }
            set
            {
                base.UseDefaultCredentials = value;
                this.useDefaultCredentialsSetExplicitly = true;
            }
        }

        [SoapDocumentMethodAttribute("http://tempuri.org/GetSchemeObjectsNames", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public string GetSchemeObjectsNames()
        {
            object[] results = this.Invoke("GetSchemeObjectsNames", new object[0]);
            return (string)results[0];
        }

        [SoapDocumentMethodAttribute("http://tempuri.org/GetObjectData", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public string GetObjectData(string objectName, string filter)
        {
            object[] results = this.Invoke("GetObjectData", new object[] { objectName, filter });
            return (string)results[0];
        }

        [SoapDocumentMethodAttribute("http://tempuri.org/Connect", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public string Connect()
        {
            object[] param = new object[8];
            param[0] = "authType";
            param[1] = AuthType;
            param[2] = "login";
            param[3] = Login;
            param[4] = "pwd";
            param[5] = Password;
            param[6] = "errStr";
            param[7] = string.Empty;

            object[] results = this.Invoke("Connect", param);
            return (string)param[7];
        }

        private bool IsLocalFileSystemWebService(string url)
        {
            if (String.IsNullOrEmpty(url))
            {
                return false;
            }
            System.Uri wsUri = new System.Uri(url);
            if (((wsUri.Port >= 1024) && (string.Compare(wsUri.Host, "localHost", StringComparison.OrdinalIgnoreCase) == 0)))
            {
                return true;
            }
            return false;
        }

        private AuthenticationType _authType = AuthenticationType.atUndefined;
        public AuthenticationType AuthType
        {
            get { return _authType; }
            set { _authType = value; }
        }

        private string _password = string.Empty;
        public string Password
        {
            get { return _password; }
            set { _password = value; }
        }

        private string _login = string.Empty;
        public string Login
        {
            get { return _login; }
            set { _login = value; }
        }
    }
}

