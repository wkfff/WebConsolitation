using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Krista.FM.Client.iMonitoringWM.Common
{
    /// <summary>
    /// Настройки пользователя
    /// </summary>
    public class UserSettings
    {
        private string _name;
        private string _password;
        private int _entityIndex;
        private bool _isAuthentication;
        private DateTime _lastConnection;

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public string Password
        {
            get { return _password; }
            set { _password = value; }
        }

        public int EntityIndex
        {
            get { return _entityIndex; }
            set { _entityIndex = value; }
        }

        public bool IsAuthentication
        {
            get { return _isAuthentication; }
            set { _isAuthentication = value; }
        }

        public DateTime LastConnection
        {
            get { return _lastConnection; }
            set { _lastConnection = value; }
        }

        public UserSettings()
        {
            this.EntityIndex = 1;
            this.Name = string.Empty;
            this.Password = string.Empty;
            this.IsAuthentication = false;
            this.LastConnection = DateTime.MinValue;
        }
    }
}
