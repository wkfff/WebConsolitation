using System;
using System.Collections.Generic;
using System.ComponentModel;
using Krista.FM.Common;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.SMO
{
    public class SmoSession : ServerManagedObject<ISession>, ISession
    {
        public SmoSession(ISession serverControl)
            : base(serverControl)
        {
        }

        #region ISession Members

        [DisplayName("ID сессии (SessionId)")]
        [Description("ID сессии")]
        public string SessionId
        {
            get { return serverControl.SessionId; }
        }

        [Browsable(false)]
        public System.Security.Principal.IPrincipal Principal
        {
            get { return serverControl.Principal; }
        }

        [Browsable(false)]
        public DateTime LogonTime
        {
            get { return serverControl.LogonTime; }
        }

        [DisplayName("Время подключения (LogonTime2)")]
        [Description("Время подключения")]
        public string LogonTime2
        {
            get
            {
                DateTime dt = serverControl.LogonTime;
                return dt.ToString();
            }
        }

        [DisplayName("Количество выделенных ресурсов (ResourcesCount)")]
        [Description("Количество выделенных ресурсов")]
        public int ResourcesCount
        {
            get { return serverControl.ResourcesCount; }
        }

        #endregion

        #region Calculated Members

        [Category("Identity")]
        public string Name
        {
            get { return serverControl.Principal.Identity.Name; }
        }

        [Category("Identity")]
        public string AuthenticationType
        {
            get { return serverControl.Principal.Identity.AuthenticationType; }
        }

        [Category("Identity")]
        public bool IsAuthenticated
        {
            get { return serverControl.Principal.Identity.IsAuthenticated; }
        }

        [DisplayName("Тип клиентской сессий (ClientType)")]
        [Description("Тип клиентской сессий")]
        public SessionClientType ClientType
        {
            get { return serverControl.ClientType; }
        }

        [DisplayName("Прилоложение, из которого подключились (Application)")]
        [Description("Прилоложение, из которого подключились")]
        public string Application
        {
            get { return serverControl.Application; }
        }

        [DisplayName("Имя машины (Host)")]
        [Description("Имя машины, с которой подключилось клиентское приложение")]
        public string Host
        {
            get { return serverControl.Host; }
        }

        [Editor(typeof(System.ComponentModel.Design.MultilineStringEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public string ExecutedActions
        {
            get { return serverControl.ExecutedActions; }
            set { }
        }

        [DisplayName("Сессия заблокирована (IsBlocked)")]
        public bool IsBlocked
        {
            get { return serverControl.IsBlocked; }
            set { serverControl.IsBlocked = value; }
        }

        public void Register(IDisposable obj)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void Unregister(IDisposable obj)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void PostAction(string actionText)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion
    }
}
