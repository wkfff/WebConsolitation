using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlServerCe;
using Microsoft.Win32;

using Krista.FM.Client.iMonitoringWM.Common;
using Krista.FM.Client.iMonitoringWM.Common.Cryptography;
using System.IO;

namespace Krista.FM.Client.iMonitoringWM.ApplicationSettings
{
    public partial class SettingsForm : Form
    {
        const string dummyPassword = "dPassword";

        private string password;
        private string _startupPath;
        private bool isMayHook;

        private DatabaseHelper dbHelper;

        public string StartupPath
        {
          get { return _startupPath; }
          set { _startupPath = value; }
        }

        public CachePlace CachePlace
        {
            get { return this.rbCacheInCard.Checked ? CachePlace.storageCard : CachePlace.storagePhone;}
            set { this.SetCachePlace(value); ;}
        }

        public SettingsForm()
        {
            InitializeComponent();
            this.SetDefaultValue();
            this.InitSystemObject();
            this.GetAppSettings();
        }

        private void SetDefaultValue()
        {
            this.StartupPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);
            //Т.к пароль в базе данных хранится в захешированном виде, 
            //а это значит его длинна постоянно равна 64 символам, отображать его целиком будет 
            //не красиво, поэтому настоящий пароль будем хранить в отдельном поле, а пользователю
            //показывать фиктивный
            this.tbPassword.Text = dummyPassword;
        }

        private void InitSystemObject()
        {
            this.dbHelper = new DatabaseHelper(this.StartupPath);
        }

        /// <summary>
        /// Достаем из БД настройки пользователя
        /// </summary>
        private void GetAppSettings()
        {
            this.tbLogin.Text = this.dbHelper.GetStrValue("login", Consts.defaultLogin);
            this.tbHost.Text = this.dbHelper.GetStrValue("host", Consts.defaultHost);
            this.password = this.dbHelper.GetStrValue("password", CryptUtils.GetPasswordHash(Consts.defaultPassword));
            this.cbOffLineOperation.Checked = this.dbHelper.GetBoolValue("offLineOperation", Consts.defaultOffLineOperation);
            
            string cahePlaceStr = this.dbHelper.GetStrValue("cachePlace", CachePlace.storageCard.ToString());
            this.CachePlace = (CachePlace)Enum.Parse(typeof(CachePlace), cahePlaceStr, true);
        }

        /// <summary>
        /// Сохраняем в БД настройки пользователя
        /// </summary>
        private void SetAppSettings()
        {
            this.dbHelper.SetStrValue("login", this.tbLogin.Text);
            this.dbHelper.SetStrValue("host", this.tbHost.Text);
            //если пароль был изменен пользователем, сохраним новый
            if (this.tbPassword.Text != dummyPassword)
            {
                this.password = CryptUtils.GetPasswordHash(this.tbPassword.Text);
                this.dbHelper.SetStrValue("password", this.password);
            }

            this.dbHelper.SetStrValue("offLineOperation", this.cbOffLineOperation.Checked.ToString());
            this.dbHelper.SetStrValue("cachePlace", this.CachePlace.ToString());
        }

        private void SetCachePlace(CachePlace cachePlace)
        {
            this.isMayHook = true;
            try
            {
                if (Utils.IsExistStorageCard())
                {
                    this.rbCacheInCard.Checked = (cachePlace == CachePlace.storageCard);
                    this.rbCacheInPhone.Checked = (cachePlace == CachePlace.storagePhone);
                }
                else
                {
                    this.rbCacheInPhone.Checked = true;
                    this.rbCacheInCard.Enabled = false;
                }
            }
            finally
            {
                this.isMayHook = false;
            }
        }

        private void btClearCache_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Удалить отчеты сохраненные в кэше?", "iМониторинг", 
                MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
            if (dialogResult == DialogResult.Yes)
            {
                Cursor.Current = Cursors.WaitCursor;
                string fullCachePath = Utils.GetCachePath(this.StartupPath, this.CachePlace);
                if (Directory.Exists(fullCachePath))
                    Directory.Delete(fullCachePath, true);
                Cursor.Current = Cursors.Default;
            }
        }

        void SettingsForm_Closing(object sender, CancelEventArgs e)
        {
            //При закрытии формы, сохраним настройки приложения
            this.SetAppSettings();
        }

        private void tbPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (this.tbPassword.Text.Contains(dummyPassword))
                this.tbPassword.Text = String.Empty;
        }

        private void rbCacheInPhone_CheckedChanged(object sender, EventArgs e)
        {
            if (!this.isMayHook)
                this.CachePlace = CachePlace.storagePhone;
        }

        private void rbCacheInCard_CheckedChanged(object sender, EventArgs e)
        {
            if (!this.isMayHook)
                this.CachePlace = CachePlace.storageCard;
        }
    }
}