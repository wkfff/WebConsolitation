using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Caching;
using Krista.FM.Common;
using Krista.FM.Domain;
using Krista.FM.ServerLibrary;

namespace Krista.FM.RIA.Core.Principal
{
    public class PrincipalProvider : IPrincipalProvider
    {
        public void SetBasePrincipal()
        {
            Trace.TraceVerbose("Getting user principal");

            var identity = HttpContext.Current.User.Identity;
            
            // Try get principal from Cache
            var principal = (BasePrincipal)HttpContext.Current.Cache.Get(identity.Name);
            
            if (principal == null)
            {
                principal = GeneratePrincipal(identity);

                // Caching generated Principal
                HttpContext.Current.Cache.Add(
                    identity.Name,
                    principal,
                    null,
                    Cache.NoAbsoluteExpiration,
                    new TimeSpan(0, 0, 30),
                    CacheItemPriority.Default,
                    null);
            }

            HttpContext.Current.User = principal;
        }

        private BasePrincipal GeneratePrincipal(IIdentity identity)
        {
            Trace.TraceVerbose("!!! Generating user principal !!!");
            string[] roles;
            Users user;
            GetUserAttributes(identity.Name, out roles, out user);
            var principal = new BasePrincipal(identity, roles, user);

            return principal;
        }

        private void GetUserAttributes(string userName, out string[] roles, out Users user)
        {
            roles = null;
            user = null;

            if (HttpContext.Current.Session != null)
            {
                IScheme scheme = HttpContext.Current.Session[ConnectionHelper.SchemeKeyName] as IScheme;

                if (scheme == null)
                {
                    throw new KeyNotFoundException("Вызов метода вне контекста сессии.");
                }

                using (new ServerContext())
                using (var db = scheme.SchemeDWH.DB)
                {
                    string query = @"
                select G.name
                  from groups G, 
                       users U, 
                       memberships M
                 where M.refusers = U.id
                       and M.refgroups = G.id
                       and upper(U.name) = upper(?)";
                    var groupsTable = (DataTable)db.ExecQuery(query, QueryResultTypes.DataTable, new DbParameterDescriptor("p0", userName));

                    roles = groupsTable
                        .AsEnumerable()
                        .Select(row => row.Field<string>("name"))
                        .ToArray();

                    groupsTable.Dispose();

                    var userTable = (DataTable)db.ExecQuery(
                        "select U.* from users U where upper(U.name) = upper(?)",
                        QueryResultTypes.DataTable,
                        new DbParameterDescriptor("p0", userName));
                    if (userTable != null)
                    {
                        user = new Users();
                        
                        try
                        {
                            var userRow = userTable.Rows[0];
                            user.ID = Convert.ToInt32(userRow["ID"]);
                            user.Name = ConvertToString(userRow["Name"]);
                            user.Description = ConvertToString(userRow["Description"]);
                            user.UserType = Convert.ToInt32(userRow["UserType"]);
                            user.Blocked = ConvertToString(userRow["Blocked"]) == "1" ? true : false;
                            user.DNSName = ConvertToString(userRow["DNSName"]);
                            user.LastLogin = Convert.ToDateTime(userRow["LastLogin"]);
                            user.FirstName = ConvertToString(userRow["FirstName"]);
                            user.LastName = ConvertToString(userRow["LastName"]);
                            user.Patronymic = ConvertToString(userRow["Patronymic"]);
                            user.JobTitle = ConvertToString(userRow["JobTitle"]);
                            user.AllowDomainAuth = ConvertToString(userRow["AllowDomainAuth"]) == "1" ? true : false;
                            user.AllowPwdAuth = ConvertToString(userRow["AllowPwdAuth"]) == "1" ? true : false;
                            if (userRow["RefDepartments"] != DBNull.Value)
                            {
                                user.RefDepartments = Convert.ToInt32(userRow["RefDepartments"]);
                            }

                            if (userRow["RefOrganizations"] != DBNull.Value)
                            {
                                user.RefOrganizations = Convert.ToInt32(userRow["RefOrganizations"]);
                            }

                            if (userRow["RefRegion"] != DBNull.Value)
                            {
                                user.RefRegion = Convert.ToInt32(userRow["RefRegion"]);
                            }
                        }
                        catch (Exception e)
                        {
                            Trace.TraceVerbose("Convert User Property exception: {0}", e.StackTrace);
                        }
                    }
                }
            }
        }

        private string ConvertToString(object param)
        {
            return param == DBNull.Value ? null : Convert.ToString(param);
        }
    }
}
