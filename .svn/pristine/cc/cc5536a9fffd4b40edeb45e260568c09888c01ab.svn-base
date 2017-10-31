using System;
using System.Data;

namespace Krista.FM.Server.Users
{
	/// <summary>
	/// Класс для представления Группы пользователей
	/// </summary>
	public sealed class SysGroup : SysBaseWinObject
	{

        public SysGroup(DataRow row)
        {
            this.ID = Convert.ToInt32(row["ID"]);
            this.Name = Convert.ToString(row["NAME"]);
            this.Description = Convert.ToString(row["DESCRIPTION"]);
            this.DNSName = Convert.ToString(row["DNSNAME"]);
            this.Blocked = Convert.ToBoolean(row["BLOCKED"]);
        }
	}
}