using System;
using System.Collections;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using Krista.FM.Update.Framework.Utils;

namespace Krista.FM.Update.Framework.Forms
{
    public partial class ConfigEditorForm : Form
    {
        #region Fields

        private IUpdateManager _manager;

        private PropertyControl propertyControl;

        private string _configFilename = "";

        private bool HasChanges { get; set; }

        #endregion

        #region Properties

        public IUpdateManager Manager
        {
            get { return _manager; }
            set { _manager = value; }
        }

        public virtual string ConfigFilename
        {
            get { return _configFilename; }
            set
            {
                _configFilename = value;
                txtConfigurationFile.Text = _configFilename;
            }
        }

        #endregion

        #region Constructor

        public ConfigEditorForm(IUpdateManager manager)
        {
            _manager = manager;

            InitializeComponent();

            string dir = Path.GetDirectoryName(UpdateManager.GetProcessModule().FileName);

            if (dir != null)
            {
                string[] configFiles = Directory.GetFiles(dir, "Krista.FM.Update.ShedulerUpdateService.exe.config");
                if (configFiles.Length > 0)
                {
                    ConfigFilename = configFiles[0];
                }
                else
                {
                    ConfigFilename = "";
                }
            }

            HasChanges = false;
        }

        #endregion

        #region Form Layout and Event Handlers
       
        private void BtnCloseClick(object sender, EventArgs e)
        {
            if (HasChanges)
            {
                DialogResult result = MessageBox.Show("Сохранить опции обновления перед закрытием?", "Опции обновления", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    try
                    {
                        SaveConfiguration(this.ConfigFilename, propertyControl);
                        MessageBox.Show("Измененные опции обновления были сохранены. Для вступления их в силу необходимо перезапустить приложение.", "Опции обновления", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        Close();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Ошибка при сохранении конфигурации настроек. Текст ошибки(" + ex.Message + ")", "Опции обновления", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                else if (result == DialogResult.No)
                {
                    Close();
                }
            }
            else
            {
                Close();
            }
        }

        private void BtnSaveClick(object sender, EventArgs e)
        {
            try
            {
                SaveConfiguration(ConfigFilename, propertyControl);
                MessageBox.Show("Измененные опции обновления были сохранены. Для вступления их в силу необходимо перезапустить приложение.", "Опции обновления", MessageBoxButtons.OK, MessageBoxIcon.Information);
                HasChanges = false;
                propertyGrid.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при сохранении конфигурации настроек. Текст ошибки(" + ex.Message + ")", "Опции обновления", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }


        /// <summary>
        /// Load a selected configuration file.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLoad_Click(object sender, System.EventArgs e)
        {
            openFileDialog1.Filter = "Файл конфигурации (*.config)| *.config";
            DialogResult result = openFileDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                ConfigFilename = openFileDialog1.FileName;
                propertyControl = LoadConfiguration(this.ConfigFilename);
                propertyGrid.SelectedObject = propertyControl;
                propertyGrid.Focus();
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Обработчик нажатия с клавиатуры
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="keyData"></param>
        /// <returns></returns>
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Escape)
                Close();

            return base.ProcessCmdKey(ref msg, keyData);
        }

        public PropertyControl LoadConfiguration(string configurationFile)
        {
            PropertyControl propertyControl = new PropertyControl();

            ExeConfigurationFileMap fileMap = new ExeConfigurationFileMap
                                                  {
                                                      ExeConfigFilename =
                                                          configurationFile
                                                  };

            Configuration config = null;
            try
            {
                if (!File.Exists(fileMap.ExeConfigFilename))
                {
                    MessageBox.Show("Файл конфигурации не найден", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                config =
                    ConfigurationManager.OpenMappedExeConfiguration(fileMap,
                                                                    ConfigurationUserLevel.None);
            }
            catch (ConfigurationErrorsException e)
            {
                MessageBox.Show(String.Format("Ошибка при открытии файла конфигурации: {0}",
                                              e.Message), "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            foreach (KeyValueConfigurationElement appSetting in config.AppSettings.Settings)
            {
                switch (appSetting.Key)
                {
                    case "PublicFMPatchStore":
                        if (Manager.IsServerMode)
                        {
                            propertyControl.AddProperty(appSetting.Key, appSetting.Value,
                                                        "Глобальный каталог с обновлениями", "Общее", typeof (string),
                                                        false, false);
                        }
                        continue;
                    case "BaseUri":
                        propertyControl.AddProperty(appSetting.Key, appSetting.Value,
                                                    "Локальный каталог с обновлениями(Shared Folder)", "Общее", typeof(string),
                                                    false, false);
                        break;
                    case "LocalFMPatchStore":
                        if (Manager.IsServerMode)
                        {
                            propertyControl.AddProperty(appSetting.Key, appSetting.Value,
                                                        "Локальный каталог с обновлениями", "Общее", typeof(string),
                                                        false, false);
                        }
                        break;
                    case "CronTrigger":
                        if (Manager.IsServerMode)
                        {
                            propertyControl.AddProperty(appSetting.Key, appSetting.Value,
                                                        "Расписаие обновления", "Общее", typeof(string),
                                                        false, false);
                        }
                        break;
                    case "Proxy":
                        if (Manager.IsServerMode)
                        {
                            propertyControl.AddProperty(appSetting.Key, appSetting.Value, "Прокси-сервер", "Настройки прокси", typeof(string), false, false);
                        }
                        break;
                    case "ProxyPort":
                        if (Manager.IsServerMode)
                        {
                            propertyControl.AddProperty(appSetting.Key, appSetting.Value, "Порт прокси-сервера", "Настройки прокси", typeof(string), false, false);
                        }
                        break;
                    case "User":
                        if (Manager.IsServerMode)
                        {
                            propertyControl.AddProperty(appSetting.Key, appSetting.Value, "Имя пользователя", "Настройки прокси", typeof(string), false, false);
                        }
                        break;
                    case "Password":
                        if (Manager.IsServerMode)
                        {
                            propertyControl.AddProperty(appSetting.Key, appSetting.Value, "Пароль", "Настройки прокси", typeof(string), false, false);
                        }
                        break;
                    case "OKTMO":
                        if(Manager.IsServerMode)
                        {
                            propertyControl.AddProperty("OKTMO", appSetting.Value, "ОКТМО", "Общее", typeof(string), false, false);
                        }
                        break;
                    default:
                        continue;
                }
            }

            if (propertyControl["OKTMO"] == null && Manager.IsServerMode)
            {
                config.AppSettings.Settings.Add("OKTMO", String.Empty);
                config.Save();

                propertyControl.AddProperty("OKTMO", String.Empty, "ОКТМО", "Общее", typeof(string), false, false);
            }

            return propertyControl;
        }

        public void SaveConfiguration(string configurationFile, PropertyControl propControl)
        {
            if (!File.Exists(configurationFile))
                throw new Exception(String.Format("Не найден файл с конфигурацией обновления : {0}", configurationFile));

            if (propControl == null)
                throw new Exception("Параметер propertyControl is NULL");

            XmlDocument xmlDoc = new XmlDocument();

            xmlDoc.Load(configurationFile);
            PropertyDescriptorCollection props = propControl.GetProperties();
            if (props["Password"] != null)
            {
                props["Password"].SetValue(null,
                                           props["Password"].GetValue(null).ToString().Length < 50
                                               ? SecureConfigStringProvider.EncryptString(
                                                   props["Password"].GetValue(null).ToString())
                                               : props["Password"].GetValue(null).ToString());
            }
                
            RepopulateXmlSection("appSettings", xmlDoc, props);
            xmlDoc.Save(configurationFile);
        }

        private static void RepopulateXmlSection(string sectionName, XmlDocument xmlDoc, PropertyDescriptorCollection props)
        {
            XmlNodeList nodes = xmlDoc.SelectNodes("configuration/" + sectionName + "/add");
            for (int i = 0; i < nodes.Count; i++)
            {
                PropertyControl.DynamicProperty property = (PropertyControl.DynamicProperty)props[nodes[i].Attributes["key"].Value];
                if (property != null)
                {
                    nodes[i].Attributes["value"].Value = property.GetValue(null).ToString();
                }
            }
        }

        private void ConfigEditorFormLoad(object sender, EventArgs e)
        {
            try
            {
                if (File.Exists(ConfigFilename))
                {
                    propertyControl = LoadConfiguration(ConfigFilename);
                    propertyGrid.SelectedObject = propertyControl;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при загрузке конфигурации настроек. Текст ошибки(" + ex.Message + ")", "Опции обновления", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            propertyGrid.Focus();
        }

        private void PropertyGridPropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            HasChanges = true;
        }

        #endregion

    }
}
