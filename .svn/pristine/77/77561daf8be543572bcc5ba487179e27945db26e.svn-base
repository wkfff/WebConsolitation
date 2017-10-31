using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Security.Cryptography;

using Krista.FM.Common;
using Krista.FM.ServerLibrary;
using Krista.FM.Client.Common;

namespace Krista.FM.Client.Workplace
{
    public partial class FormChangePassword : Form
    {
        public FormChangePassword()
        {
            InitializeComponent();
        }

        internal IScheme Scheme;
        internal bool AdminMode;
        internal int? UserID;
        internal string PwdHash;
        private bool _isEntryHashPwd;

        /// <summary>
        /// Вводим хэш пароль
        /// </summary>
        private bool IsEntryHashPwd
        {
            get { return _isEntryHashPwd; }
            set { this.SetEntryHashPwd(value); }
        }

        /// <summary>
        /// Выбор способа ввода пароля, хэш пароль может вводить только администратор
        /// </summary>
        /// <returns></returns>
        private void SetEntryHashPwd(bool value)
        {
            this._isEntryHashPwd = this.AdminMode && value;
            this.InitControls(this._isEntryHashPwd);
        }

        /// <summary>
        /// Получить Хэш пароль пользователя
        /// </summary>
        /// <param name="activeScheme">схема</param>
        /// <param name="userID">ID пользователя</param>
        /// <returns></returns>
        internal string GetUserHashPassword(IScheme activeScheme, int? userID)
        {
            string result = PwdHelper.GetPasswordHash(string.Empty);
            if ((userID != null) && (activeScheme != null))
            {
                using (IDatabase db = activeScheme.SchemeDWH.DB)
                {
                    string query = String.Format("select PwdHashSHA from users where id = {0}", userID);
                    DataTable dt = (DataTable) db.ExecQuery(query, QueryResultTypes.DataTable);
                    if ((dt.Rows.Count > 0) && (dt.Rows[0][0] != DBNull.Value))
                    {
                        result = dt.Rows[0][0].ToString();
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// В зависимости от типа ввода пароля, инциализируем контролы
        /// </summary>
        /// <param name="isEntryHashMode"></param>
        private void InitControls(bool isEntryHashMode)
        {
            this.edLogin.Text = this.Scheme.UsersManager.GetUserNameByID((int)this.UserID);
            this.laConfirmPassword.Visible = !isEntryHashMode;
            this.edNewPassword2.Visible = !isEntryHashMode;

            if (isEntryHashMode)
            {
                this.laPassword.Text = "Хэш пароль";
                this.laNewPassword.Text = "Новый хэш пароль";
                this.edNewPassword1.PasswordChar = Char.MinValue;

                this.edPassword.Enabled = true;
                this.edPassword.ReadOnly = true;
                this.edPassword.Text = this.PwdHash;
            }
            else
            {
                this.laPassword.Text = "Пароль";
                this.laNewPassword.Text = "Новый пароль";
                this.edNewPassword1.PasswordChar = '*';

                if (this.AdminMode)
                {
                    this.edPassword.Text = "*********";
                    this.edPassword.Enabled = false;
                }
                else
                {
                    this.edLogin.Text = this.Scheme.UsersManager.GetCurrentUserName();
                }
            }
        }

        public static void ChangePassword(IScheme activeScheme, bool adminMode, int? userID)
        {
            FormChangePassword tmpFrm = new FormChangePassword();
            tmpFrm.Scheme = activeScheme;
            tmpFrm.AdminMode = adminMode;
            tmpFrm.UserID = userID;
            tmpFrm.PwdHash = tmpFrm.GetUserHashPassword(activeScheme, userID);

            tmpFrm.IsEntryHashPwd = false;
            tmpFrm.ShowDialog();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            if ((edNewPassword1.Text != edNewPassword2.Text) && !this.IsEntryHashPwd)
            {
                edNewPassword1.Text = String.Empty;
                edNewPassword2.Text = String.Empty;
                edNewPassword1.Focus();
                MessageBox.Show("Новый пароль задан неверно. Повторите ввод.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if ((!AdminMode) && (edPassword.Text == edNewPassword1.Text))
            {
                edPassword.Text = String.Empty;
                edNewPassword1.Text = String.Empty;
                edNewPassword2.Text = String.Empty;
                edPassword.Focus();
                MessageBox.Show("Новый пароль не должен быть равен старому. Повторите ввод.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (this.IsEntryHashPwd && (edNewPassword1.Text.Trim() == string.Empty))
            {
                edPassword.Focus();
                MessageBox.Show("Хэш пароль не может быть пустым.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string errStr = String.Empty;
            string oldPassword = this.IsEntryHashPwd ? edPassword.Text : PwdHelper.GetPasswordHash(edPassword.Text);
            string newPassword = this.IsEntryHashPwd ? edNewPassword1.Text : PwdHelper.GetPasswordHash(edNewPassword1.Text);
            if (!AdminMode)
            {
                Scheme.UsersManager.ChangeUserPassword(
                    edLogin.Text,
                    oldPassword,
                    newPassword,
                    ref errStr);
            }
            else
            {
                Scheme.UsersManager.ChangeUserPasswordAdm(
                    (int)UserID,
                    newPassword,
                    ref errStr
                );
            }

            if (String.IsNullOrEmpty(errStr))
            {
                MessageBox.Show("Пароль изменен", "Операция завершена успешно", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
            else
            {
                MessageBox.Show(errStr, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        private void FormChangePassword_KeyDown(object sender, KeyEventArgs e)
        {
            //при нажатии на alt происходит смена типа ввода пароля
            if (e.Alt)
                this.IsEntryHashPwd = !this.IsEntryHashPwd;
        }
    }
}
