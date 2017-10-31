using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Runtime.CompilerServices;
using System.Diagnostics;

using Krista.FM.Common;
using Krista.FM.ServerLibrary;
using Krista.FM.Server.Common;


namespace Krista.FM.Server.Users
{
    /// <summary>
    /// Методы аутентификации
    /// </summary>
    public sealed partial class UsersManager : DisposableObject, IUsersManager
    {
        public bool AuthenticateUser(string dnsName, ref string errStr)
        {
            return InternalAuthenticateUser(AuthenticationType.atWindows, dnsName, String.Empty, String.Empty, ref errStr);
        }

        public bool AuthenticateUser(string login, string pwdHash, ref string errStr)
        {
            return InternalAuthenticateUser(AuthenticationType.adPwdSHA512, String.Empty, login, pwdHash, ref errStr);
        }

        private void UpdateUserLastLogin(IDatabase externalDb, int userID)
        {
            DateTime now = System.DateTime.Now;
            IDatabase db = externalDb;
            try
            {
                lock (syncUsers)
                {
                    Users[userID].LastLogin = now;
                    if (db == null)
                        db = this._scheme.SchemeDWH.DB;
                    db.ExecQuery(
                        "update Users set LastLogin = ? where ID = ?",
                        QueryResultTypes.NonQuery,
                        db.CreateParameter("LastLogin", (object) now),
                        db.CreateParameter("ID", (object) (int) userID)
                        );
                }
            }
            finally
            {
                if (externalDb == null)
                    db.Dispose();
            }
        }

        private bool InternalAuthenticateUser(AuthenticationType authType, string dnsName, string login,
            string pwdHash, ref string errStr)
        {
            bool result = false;
            int? id = null;
            SysUser curUser = null;
            switch (authType)
            {
                case AuthenticationType.atWindows:
                    // проверяем DNSName
                    if (String.IsNullOrEmpty(dnsName))
                    {
                        errStr = "Имя не может быть пустым";
                        break;
                    }
                    // ищем пользователя по DNSName
                    id = this.FindUserIDByDNSName(dnsName);
                    if (id == null)
                    {
                        errStr = String.Format("Пользователь '{0}' не найден", dnsName);
                        break;
                    }
                    curUser = Users[(int)id];
                    // может ли пользователь входить в этом режиме?
                    if (!curUser.AllowDomainAuth)
                    {
                        errStr = String.Format("Пользователю '{0}' запрещен вход в режиме доменной аутентификации", dnsName);
                        break;
                    }
                    // не заблокирован ли пользователь?
                    if (curUser.Blocked)
                    {
                        errStr = String.Format("Пользователь '{0}' заблокирован", dnsName);
                        break;
                    }
                    // все проверки пройдены - проставляем UserID
                    Authentication.UserID = (int)id;
                    UpdateUserLastLogin(null, (int)id);
                    result = true;
                    break;
                case AuthenticationType.adPwdSHA512:
                    // Проверяем логин на пустоту
                    if (String.IsNullOrEmpty(login))
                    {
                        errStr = "Логин не может быть пустым";
                        break;
                    }
                    // Проверяем пароль на пустоту
                    if (String.IsNullOrEmpty(pwdHash))
                    {
                        errStr = "Пароль не может быть пустым";
                        break;
                    }
                    // ищем пользователя по логину
                    id = FindUserByName(login);
                    if (id == null)
                    {
                        errStr = String.Format("Пользователь {0} не найден", login);
                        break;
                    }
                    curUser = Users[(int)id];
                    // может ли пользователь входить в этом режиме?
                    if (!curUser.AllowPwdAuth)
                    {
                        errStr = String.Format("Пользователю '{0}' запрещен вход в режиме 'Логин/Пароль'", login);
                        break;
                    }
                    // не заблокирован ли пользователь?
                    if (Users[(int)id].Blocked)
                    {
                        errStr = String.Format("Пользователь '{0}' заблокирован", login);
                        break;
                    }
                    IDatabase db = null;
                    try
                    {
                        db = this._scheme.SchemeDWH.DB;
                        string query = String.Format("select PwdHashSHA from users where id = {0}", id);
                        DataTable dt = (DataTable)db.ExecQuery(query, QueryResultTypes.DataTable);
                        if (dt.Rows.Count == 0)
                        {
                            errStr = "Внутренняя ошибка. Произошло рассогласование таблицы пользователей и содержимого коллекции. Перезапустите сервис.";
                            break;
                        }
                        if (dt.Rows[0][0] == DBNull.Value)
                        {
                            errStr = String.Format("Не установлен пароль для пользователя '{0}'. Выберите другой тип аутентификации.", login);
                            break;
                        }
                        string userPwdHash = dt.Rows[0][0].ToString();
                        if (userPwdHash != pwdHash)
                        {
                            errStr = String.Format("Неверный пароль для пользователя '{0}'", login);
                        }
                        else
                        {
                            Authentication.UserID = id;
                            UpdateUserLastLogin(db, (int)id);
                            result = true;
                        }
                        break;
                    }
                    finally
                    {
                        if (db != null)
                            db.Dispose();
                    }
                default:
                    errStr = "Неверный тип аутентификации";
                    break;
            }
            return result;
        }
    }
}