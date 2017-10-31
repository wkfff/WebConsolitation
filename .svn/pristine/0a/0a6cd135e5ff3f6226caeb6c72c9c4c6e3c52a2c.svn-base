using System;
using System.Security.Principal;
using Krista.FM.Domain;

namespace Krista.FM.RIA.Core.Principal
{
    public class BasePrincipal : IPrincipal
    {
        private IIdentity identity;
        private string[] roles;
         
        public BasePrincipal(IIdentity identity, string[] roles, Users databaseUser)
        {
            this.identity = identity;
            this.DbUser = databaseUser;
            if (roles != null)
            {
                this.roles = new string[roles.Length];
                roles.CopyTo(this.roles, 0);
                Array.Sort(this.roles);
            }
        }

        /// <summary>
        /// Атрибуты пользователя системы
        /// </summary>
        public Users DbUser { get; private set; }

        public IIdentity Identity
        {
            get { return identity; }
        }
       
        public bool IsInRole(string role)
        {
            return Array.BinarySearch(this.roles, role) >= 0 ? true : false;
        }

        public bool IsInAllRoles(params string[] roles)
        {
            foreach (string searchrole in roles)
            {
                if (Array.BinarySearch(this.roles, searchrole) < 0)
                {
                    return false;
                }
            }

            return true;
        }

        public bool IsInAnyRoles(params string[] roles)
        {
            foreach (string searchrole in roles)
            {
                if (Array.BinarySearch(this.roles, searchrole) >= 0)
                {
                    return true;
                }
            }
            
            return false;
        }
    }
}
