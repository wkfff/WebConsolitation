using System;
using System.Collections.Generic;
using System.Linq;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.ServerLibrary;

namespace Krista.FM.RIA.Extensions.Consolidation
{
    public class UserSessionState : IUserSessionState
    {
        private readonly IScheme scheme;
        private readonly IRepository<Users> userRepository;
        private readonly IRepository<D_Regions_Analysis> regionRepository;
        private readonly ILinqRepository<D_CD_Subjects> subjectRepository;

        public UserSessionState(
            IScheme scheme,
            IRepository<Users> userRepository,
            IRepository<D_Regions_Analysis> regionRepository,
            ILinqRepository<D_CD_Subjects> subjectRepository)
        {
            this.scheme = scheme;
            this.userRepository = userRepository;
            this.regionRepository = regionRepository;
            this.subjectRepository = subjectRepository;
        }

        public Users User { get; private set; }

        public IList<D_CD_Subjects> Subjects { get; private set; }

        public D_Regions_Analysis UserRegion { get; private set; }
        
        public bool Initialize()
        {
            try
            {
                int currentUserId = scheme.UsersManager.GetCurrentUserID();

                User = userRepository.Get(currentUserId);

                if (User.RefRegion != null)
                {
                    UserRegion = regionRepository.Get((int)User.RefRegion);
                }

                Subjects = subjectRepository.FindAll().Where(x => x.UserId == currentUserId).ToList();

                return true;
            }
            catch (Exception e)
            {
                Trace.TraceError(
                    "Ошибка инициализации модуля: {0}",
                    Diagnostics.KristaDiagnostics.ExpandException(e));

                return false;
            }
        }
    }
}
