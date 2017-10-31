using System;
using System.Linq;

using Krista.Diagnostics;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.RIA.Extensions.E86N.Auth.Model;
using Krista.FM.ServerLibrary;

using GlobalConsts = Krista.FM.RIA.Extensions.E86N.Utils.GlobalConsts;

namespace Krista.FM.RIA.Extensions.E86N.Auth.Services
{
    internal sealed class AuthService : IAuthService
    {
        private readonly ILinqRepository<Memberships> membershipsRepository;

        private readonly IScheme scheme;

        private readonly ILinqRepository<D_Org_UserProfile> userProfile;

        private readonly IRepository<Users> userRepository;

        public AuthService(
            IScheme scheme, 
            IRepository<Users> userRepository, 
            ILinqRepository<Memberships> membershipsRepository, 
            ILinqRepository<D_Org_UserProfile> userProfile)
        {
            this.scheme = scheme;
            this.userProfile = userProfile;
            this.membershipsRepository = membershipsRepository;
            this.userRepository = userRepository;
        }

        #region IAuthService Members

        public Users User { get; private set; }

        public D_Org_Structure ProfileOrg
        {
            get
            {
                if (Profile == null)
                {
                    throw new InvalidOperationException(GlobalConsts.NullProfile);
                }

                return Profile.RefUchr;
            }
        }

        public D_Org_GRBS ProfileOrgGrbs
        {
            get { throw new NotImplementedException(); }
        }

        public D_Org_PPO ProfileOrgPpo
        {
            get { throw new NotImplementedException(); }
        }

        public D_Org_UserProfile Profile
        {
            get
            {
                // todo: тут восстанавливаем ссылки в профиле - это плохо
                // нужно возвращать модель, а в ней кешировать нужные данные
                // либо расширить интерфейс сделать прямой доступ на все нужные записи
                return userProfile.FindAll().SingleOrDefault(p => p.UserLogin.Equals(User.Name));
            }
        }

        private Groups[] Roles { get; set; }

        public bool IsAdmin()
        {
            // в группе администраторы или администратор по умолчанию
            return IsInRole(AccountsRole.WebAdministrator) || IsKristaRu();
        }

        public bool IsGrbsUser()
        {
            return IsInRole(AccountsRole.Provider);
        }

        public bool IsPpoUser()
        {
            return IsInRole(AccountsRole.SuperProvider);
        }

        public bool IsInstitution()
        {
            return IsInRole(AccountsRole.Consumer);
        }

        public bool IsRegisteredUser()
        {
            return IsInAnyRole(AccountsRole.SuperProvider, AccountsRole.Provider, AccountsRole.Consumer);
        }

        public bool IsAuthority2Sign()
        {
            throw new NotImplementedException();
        }

        public bool IsKristaRu()
        {
            return IsInRole(AccountsRole.KristaRu) || IsInRole(AccountsRole.Administrator) || User.ID == 1;
        }

        public bool IsSpectator()
        {
            return IsInRole(AccountsRole.Spectator);
        }

        #endregion

        public bool Initialize()
        {
            try
            {
                int currentUserId = scheme.UsersManager.GetCurrentUserID();

                User = userRepository.Get(currentUserId);

                Roles = membershipsRepository.FindAll()
                    .Where(p => p.RefUsers.ID == User.ID && p.RefGroups.Blocked != true)
                    .Select(p => p.RefGroups).ToArray();

                // Profile = userProfile.FindAll().SingleOrDefault(p => p.UserLogin.Equals(User.Name));
                return true;
            }
            catch (Exception e)
            {
                Trace.TraceError("Ошибка инициализации AuthService: {0}; UserLogin: {1}; Exception: {2}", e.Message, User.Name, KristaDiagnostics.ExpandException(e));
                return false;
            }
        }

        private bool IsInRole(string role)
        {
            return Roles != null
                   && Roles.Any(group => group.Name.Equals(role));
        }

        private bool IsInAnyRole(params string[] roles)
        {
            return Roles != null
                   && roles.Any(searchrole => Array.FindIndex(Roles, f => f.Name.Equals(searchrole)) >= 0);
        }
    }
}
