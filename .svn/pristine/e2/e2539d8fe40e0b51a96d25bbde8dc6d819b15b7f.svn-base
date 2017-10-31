using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Runtime.CompilerServices;

using Krista.FM.Common;
using Krista.FM.Server.Common;

using Krista.FM.ServerLibrary;

namespace Krista.FM.Server.Users
{
    /// <summary>
    /// Различные методы связанные с пользователями
    /// </summary>
    public sealed partial class UsersManager : DisposableObject, IUsersManager
    {
        public string GetCurrentUserName()
        {
            return Authentication.UserName;
        }

        // временный 
        public int GetCurrentUserID()
        {
            return (int)Authentication.UserID;
        }

        public void CheckCurrentUser()
        {
            string userName = GetCurrentUserName();
            int? userID = FindUserIDByDNSName(userName);
            using (IDatabase db = _scheme.SchemeDWH.DB)
            {
                if (userID == null)
                {
                    lock (syncUsers)
                    {
                        db.ExecQuery(
                            "insert into Users (Name, UserType, Blocked, DNSName, LastLogin, PwdHashSha) values (?, ?, ?, ?, ?, ?)",
                            QueryResultTypes.NonQuery,
                            db.CreateParameter("Name", userName),
                            db.CreateParameter("UserType", 0),
                            db.CreateParameter("Blocked", false),
                            db.CreateParameter("DNSName", userName),
                            db.CreateParameter("LastLogin", DateTime.Now),
                            db.CreateParameter("PwdHashSha", PwdHelper.GetPasswordHash(String.Empty))
                            );
                    }
                    // чтобы избежать повторной регистрации перезагружаем пользователей
                    LoadUsers();
                }
            }
        }

        public string GetUserNameByID(int userID)
        {
            string userName = "Неизвестный пользователь";
            if (Users.ContainsKey(userID))
            {
                SysUser us = Users[userID];
                userName = us.Name;
            }
            return userName;
        }

        // временный
        public int? FindUserIDByDNSName(string dnsName)
        {
            int? userID = null;
            foreach (SysUser user in Users.Values)
            {
                if (user.DNSName.ToUpper() == dnsName.ToUpper())
                {
                    userID = user.ID;
                    break;
                }
            }
            return userID;
        }

        private int? FindUserByName(string name)
        {
            int? userID = null;
            foreach (SysUser user in Users.Values)
            {
                if (user.Name.ToUpper() == name.ToUpper())
                {
                    userID = user.ID;
                    break;
                }
            }
            return userID;
        }

        public bool ChangeUserPassword(string login, string pwdHash, string newPwdHash, ref string errStr)
        {
            if (!AuthenticateUser(login, pwdHash, ref errStr))
                return false;
            else
                return InternalChangeUserPassword((int)Authentication.UserID, newPwdHash, ref errStr);
        }

        public bool ChangeUserPasswordAdm(int userID, string newPwdHash, ref string errStr)
        {
            if (!CurrentUserIsAdmin())
            {
                errStr = "Текущий пользователь не является администратором системы";
                return false;
            }
            return InternalChangeUserPassword(userID, newPwdHash, ref errStr);
        }

        private bool InternalChangeUserPassword(int userID, string newPwdHash, ref string errStr)
        {
			using (IDatabase db = _scheme.SchemeDWH.DB)
			{
				try
				{
					string query = String.Format("update users set PwdHashSHA = ? where ID = {0}", userID);
                    lock (syncUsers)
                    {
                        db.ExecQuery(query, QueryResultTypes.Scalar, db.CreateParameter("PwdHashSHA", newPwdHash));
                    }
				    return true;
				}
				catch (Exception e)
				{
					errStr = e.Message;
					return false;
				}
			}
        }

        private bool CurrentUserIsAdmin()
        {
            int curUser = this.GetCurrentUserID();
            string filter = String.Format("(REFUSERS = {0}) and (REFGROUPS = {1})", curUser, this.administratorsGroupsObjID);
            DataRow[] filtered = MembershipsTable.Select(filter);
            return filtered.Length > 0;
        }

        /// <summary>
        /// Являестся ли текущий пользователь Web-администратором
        /// </summary>
        /// <returns></returns>
        private bool CurrentUserIsWebAdmin()
        {
            int curUser = this.GetCurrentUserID();
            string filter = String.Format("(REFUSERS = {0}) and (REFGROUPS = {1})", curUser, this.webAdministratorsGroupsObjID);
            DataRow[] filtered = MembershipsTable.Select(filter);
            return filtered.Length > 0;
        }

        private string GetGroupNameByID(int groupID)
        {
            string groupName = "Неизвестная группа";
            if (Groups.ContainsKey(groupID))
            {
                SysGroup grp = Groups[groupID];
                groupName = grp.Name;
            }
            return groupName;
        }
    }
}