using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.ServerLibrary;

namespace Krista.FM.RIA.Extensions.EO14InvestAreas
{
    public class UserCredentials : IUserCredentials
    {
        private readonly IScheme scheme;
        private readonly IRepository<Users> userRepository;
        private readonly ILinqRepository<Memberships> membershipsRepository;
       
        public UserCredentials(
                            IScheme scheme, 
                            IRepository<Users> userRepository,
                            ILinqRepository<Memberships> membershipsRepository)
        {
            this.scheme = scheme;
            this.userRepository = userRepository;
            this.membershipsRepository = membershipsRepository;
        }

        public Users User { get; private set; }

        public Groups[] Roles { get; private set; }

        public bool IsInRole(string role)
        {
            if (this.Roles != null)
            {
                foreach (var group in Roles)
                {
                    if (group.Name == role)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public bool IsInAnyRole(params string[] roles)
        {
            foreach (string searchrole in roles)
            {
                if (Array.FindIndex(this.Roles, f => f.Name == searchrole) >= 0)
                {
                    return true;
                }
            }

            return false;
        }

        public bool Initialize()
        {
            try
            {
                int currentUserId = scheme.UsersManager.GetCurrentUserID();

                User = this.userRepository.Get(currentUserId);

                Roles = (from p in this.membershipsRepository.FindAll()
                         where p.RefUsers.ID == User.ID
                               && p.RefGroups.Blocked != true
                         select p.RefGroups).ToArray();
                
                return true;
            }
            catch (Exception e)
            {
                Trace.TraceError("Ошибка инициализации модуля: {0}", Diagnostics.KristaDiagnostics.ExpandException(e));
                return false;
            }
        }
    }
}

//// TODO: реализовать свой Principal вместо System.Web.HttpContext.Current.User
////if (System.Web.HttpContext.Current.User != null)
////            {
////                if (System.Web.HttpContext.Current.User.Identity.IsAuthenticated)
////                {
////                    if (System.Web.HttpContext.Current.User.Identity is FormsIdentity)
////                    {
////                        // Get Forms Identity From Current User
////                        FormsIdentity id = (FormsIdentity)System.Web.HttpContext.Current.User.Identity;

////                        // Create a custom Principal Instance and assign to Current User (with caching)
////                        CustomPrincipal principal = (CustomPrincipal)System.Web.HttpContext.Current.Cache.Get(id.Name);
////                        if (principal == null)
////                        {
////                            // Create and populate your Principal object with the needed data and Roles.
////                            principal = new CustomPrincipal(id, new [] { "role1" });
////                            System.Web.HttpContext.Current.Cache.Add(
////                            id.Name,
////                            principal,
////                            null,
////                            System.Web.Caching.Cache.NoAbsoluteExpiration,
////                            new TimeSpan(0, 30, 0),
////                            System.Web.Caching.CacheItemPriority.Default,
////                            null);
////                        }

////                        System.Web.HttpContext.Current.User = principal;
////                    }
////                }
////            }