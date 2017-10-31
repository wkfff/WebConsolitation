using System;
using System.Configuration;
using System.Collections;

namespace Krista.FM.Client.MobileReports.Common.ConfigElement
{
    public class AuthorizationConfigElement : ConfigurationElement
    {
        private static bool _displayIt = false;
        
        public AuthorizationConfigElement()
        {
        }

        public AuthorizationConfigElement(bool newEnabled,
            string newLogin, string newPassword)
        {
            this.Enabled = newEnabled;
            this.Login = newLogin;
            this.Password = newPassword;
        }

        [ConfigurationProperty("enabled", DefaultValue = false, IsRequired = true)]
        public bool Enabled
        {
            get
            {
                return (bool)this["enabled"];
            }
            set
            {
                this["enabled"] = value;
            }
        }

        [ConfigurationProperty("login", IsRequired = true)]
        public string Login
        {
            get
            {
                return (string)this["login"];
            }
            set
            {
                this["login"] = value;
            }
        }

        [ConfigurationProperty("password", IsRequired = false)]
        public string Password
        {
            get
            {
                return (string)this["password"];
            }
            set
            {
                this["password"] = value;
            }
        }
        /*
        protected override void DeserializeElement(System.Xml.XmlReader reader, bool serializeCollectionKey)
        {
            base.DeserializeElement(reader, serializeCollectionKey);

            if (_displayIt)
            {
                Console.WriteLine(
                   "AuthorizationConfigElement.DeserializeElement({0}, {1}) called",
                   (reader == null) ? "null" : reader.ToString(),
                   serializeCollectionKey.ToString());
            }
        }

        protected override bool SerializeElement(System.Xml.XmlWriter writer, bool serializeCollectionKey)
        {
            bool ret = base.SerializeElement(writer, serializeCollectionKey);

            if (_displayIt)
            {
                Console.WriteLine(
                    "AuthorizationConfigElement.SerializeElement({0}, {1}) called = {2}",
                    (writer == null) ? "null" : writer.ToString(),
                    serializeCollectionKey.ToString(), ret.ToString());
            }
            return ret;

        }
        */
        protected override bool IsModified()
        {
            return base.IsModified();
        }
    }
}
