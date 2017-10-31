using System;
using System.Configuration;
using System.Collections;
using Krista.FM.Client.MobileReports.Common.ConfigElement;

namespace Krista.FM.Client.MobileReports.Common
{
    public class MobileReportsSettingsSection : ConfigurationSection
    {
        private static bool _displayIt = false;

        public MobileReportsSettingsSection()
        {
        }

        [ConfigurationProperty("reportsHostAddress")]
        public StringConfigElement BootloaderReportsHostAddress
        {
            get { return (StringConfigElement)base["reportsHostAddress"]; }
            set { base["reportsHostAddress"] = value; }
        }

        [ConfigurationProperty("reportsHostAuthorization")]
        public AuthorizationConfigElement BootloaderReportsHostAuthorization
        {
            get { return (AuthorizationConfigElement)base["reportsHostAuthorization"]; }
            set { base["reportsHostAuthorization"] = value; }
        }

        [ConfigurationProperty("snapshotMode")]
        public SnapshotModeConfigElement BootloaderSnapshotMode
        {
            get { return (SnapshotModeConfigElement)base["snapshotMode"]; }
            set { base["snapshotMode"] = value; }
        }

        [ConfigurationProperty("isOptimazeHTML")]
        public BoolConfigElement BootloaderIsOptimazeHTML
        {
            get { return (BoolConfigElement)base["isOptimazeHTML"]; }
            set { base["isOptimazeHTML"] = value; }
        }

        [ConfigurationProperty("allowedErrorProcent")]
        public IntConfigElement BootloaderAllowedErrorProcent
        {
            get { return (IntConfigElement)base["allowedErrorProcent"]; }
            set { base["allowedErrorProcent"] = value; }
        }

        [ConfigurationProperty("scriptsDownloadMode")]
        public ScriptDownloadConfigElement BootloaderScriptsDownloadMode
        {
            get { return (ScriptDownloadConfigElement)base["scriptsDownloadMode"]; }
            set { base["scriptsDownloadMode"] = value; }
        }

        [ConfigurationProperty("isRelease")]
        public BoolConfigElement IsRelease
        {
            get { return (BoolConfigElement)base["isRelease"]; }
            set { base["isRelease"] = value; }
        }

        [ConfigurationProperty("dataBurstSavePath")]
        public StringConfigElement DataBurstSavePath
        {
            get { return (StringConfigElement)base["dataBurstSavePath"]; }
            set { base["dataBurstSavePath"] = value; }
        }

        [ConfigurationProperty("proxySettings")]
        public ProxyConfgiElement UploaderProxySettings
        {
            get { return (ProxyConfgiElement)base["proxySettings"]; }
            set { base["proxySettings"] = value; }
        }

        [ConfigurationProperty("distHostIpAddress")]
        public StringConfigElement UploaderDistHostIpAddress
        {
            get { return (StringConfigElement)base["distHostIpAddress"]; }
            set { base["distHostIpAddress"] = value; }
        }

        [ConfigurationProperty("distHostUriAddress")]
        public StringConfigElement UploaderDistHostUriAddress
        {
            get { return (StringConfigElement)base["distHostUriAddress"]; }
            set { base["distHostUriAddress"] = value; }
        }

        [ConfigurationProperty("distHostAuthorization")]
        public AuthorizationConfigElement UploaderDistHostAuthorization
        {
            get { return (AuthorizationConfigElement)base["distHostAuthorization"]; }
            set { base["distHostAuthorization"] = value; }
        }

        [ConfigurationProperty("distHostPath")]
        public StringConfigElement UploaderDistHostPath
        {
            get { return (StringConfigElement)base["distHostPath"]; }
            set { base["distHostPath"] = value; }
        }
        
        
        /*
        protected override void DeserializeSection(System.Xml.XmlReader reader)
        {
            base.DeserializeSection(reader);

            if (_displayIt)
            {
                Console.WriteLine(
                    "WebReportsSettingsSection.DeserializeSection({0}) called",
                    (reader == null) ? "null" : reader.ToString());
            }
        }

        protected override string SerializeSection(ConfigurationElement parentElement, string name, 
            ConfigurationSaveMode saveMode)
        {
            string s = base.SerializeSection(parentElement, name, saveMode);

            if (_displayIt)
            {
                Console.WriteLine(
                   "WebReportsSettingsSection.SerializeSection({0}, {1}, {2}) called = {3}",
                   parentElement.ToString(), name,
                   saveMode.ToString(), s);
            }
            return s;
        }*/

    }
}
