using System;
using System.Configuration;
using System.Collections;

namespace Krista.FM.Client.MobileReports.Common.ConfigElement
{
    public class ProxyConfgiElement : ConfigurationElement
    {
        private static bool _displayIt = false;

        public ProxyConfgiElement()
        {
        }

        public ProxyConfgiElement(string newAddress, int newPort, string newLogin, 
            string newPassword)
        {
            this.Address = newAddress;
            this.Port = newPort;
            this.Login = newLogin;
            this.Password = newPassword;
        }

        [ConfigurationProperty("address", IsRequired = true)]
        public string Address
        {
            get { return (string)this["address"]; }
            set { this["address"] = value; }
        }

        [ConfigurationProperty("port", DefaultValue = (int)0, IsRequired = false)]
        [IntegerValidator(MinValue = 0, MaxValue = 8080, ExcludeRange = false)]
        public int Port
        {
            get { return (int)this["port"]; }
            set { this["port"] = value; }
        }

        [ConfigurationProperty("login", IsRequired = true)]
        public string Login
        {
            get { return (string)this["login"]; }
            set { this["login"] = value; }
        }

        [ConfigurationProperty("password", IsRequired = false)]
        public string Password
        {
            get { return (string)this["password"]; }
            set { this["password"] = value; }
        }
        /*
        protected override void DeserializeElement(System.Xml.XmlReader reader, bool serializeCollectionKey)
        {
            base.DeserializeElement(reader, serializeCollectionKey);

            if (_displayIt)
            {
                Console.WriteLine(
                   "ProxyConfgiElement.DeserializeElement({0}, {1}) called",
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
                    "ProxyConfgiElement.SerializeElement({0}, {1}) called = {2}",
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
