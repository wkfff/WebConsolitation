using System;
using System.Data;

namespace Krista.FM.Server.Users
{
	/// <summary>
	/// ����� ��� ������������� ������������ �������
	/// </summary>
	public sealed class SysUser : SysBaseWinObject
	{
		/// <summary>
		/// ����� ���������� ����� � �������
		/// </summary>
		public DateTime LastLogin;

        public bool AllowDomainAuth = false;
        public bool AllowPwdAuth = false;

        public SysUser(DataRow row)
        {
            this.ID = Convert.ToInt32(row["ID"]);
            this.Name = Convert.ToString(row["NAME"]);
            this.Description = Convert.ToString(row["DESCRIPTION"]);
            this.DNSName = Convert.ToString(row["DNSNAME"]);
            this.Blocked = Convert.ToBoolean(row["BLOCKED"]);
            try
            {
                this.LastLogin = Convert.ToDateTime(row["LASTLOGIN"]);
            }
            catch (InvalidCastException)
            {
            }
            
			AllowDomainAuth = Convert.ToBoolean(row["AllowDomainAuth"]);
            AllowPwdAuth = Convert.ToBoolean(row["AllowPwdAuth"]);
        }
	}
}
