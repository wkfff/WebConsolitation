using System;
using System.Collections.Generic;
using System.ComponentModel;

using Krista.FM.Common;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.SMO
{
    public class SmoSessionManager : ServerManagedObject<ISessionManager>, ISessionManager
    {
        public SmoSessionManager(ISessionManager serverControl)
            : base(serverControl)
        {
        }

        #region ISessionManager Members

        [DisplayName("Индексатор (this[string sessionId])")]
        [Description("Индексатор")]
        public ISession this[string sessionId]
        {
            get { return serverControl[sessionId]; }
        }

        [DisplayName("Коллекция сессий (Sessions)")]
        [Description("Коллекция сессий")]
        public IDictionary<string, ISession> Sessions
        {
            get { return serverControl.Sessions; }
        }

        [Browsable(false)]
        public void ClientSessionIsAlive(string sessionID)
        {
            serverControl.ClientSessionIsAlive(sessionID);
        }

        [Browsable(false)]
        public TimeSpan MaxClientResponseDelay
        {
            get { return serverControl.MaxClientResponseDelay; }
        }

        #endregion
    }
}
