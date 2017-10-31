using System;
using System.Diagnostics;
using System.Linq;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.ServerLibrary;

namespace Krista.FM.RIA.Extensions.EO10MissivePRF
{
    public class EO10Extension : IEO10Extension
    {
        private readonly IScheme scheme;
        private readonly IRepository<Users> userRepository;
        private readonly IRepository<D_MissivePRF_Execut> executersRepository;

        public EO10Extension(
            IScheme scheme,
            IRepository<Users> userRepository,
            IRepository<D_MissivePRF_Execut> executersRepository,
            IRepository<DataSources> sourceRepository)
        {
            this.scheme = scheme;
            this.userRepository = userRepository;
            this.executersRepository = executersRepository;
        }

        public Users User { get; private set; }

        public int UserGroup { get; private set;  }

        public D_MissivePRF_Execut Executer { get; private set; }

        public bool Initialize()
        {
            try
            {
                var currentUserId = scheme.UsersManager.GetCurrentUserID();
                User = userRepository.Get(currentUserId);

                // если на пользователя есть ссылка 
                // в классификаторе Послание ПРФ.Исполнители мероприятий (d.MissivePRF.Execut) 
                // в атрибуте Пользователь (UserID) 
                var exec = executersRepository.GetAll().Where(x => x.UserID == currentUserId);
                Executer = exec.FirstOrDefault();
                UserGroup = (exec != null && exec.Count() > 0) ? 1 : 0;
                return true;
            }
            catch (Exception e)
            {
                Trace.TraceError("Ошибка инициализации модуля: {0}", e.Message);
                return false;
            }
        }
    }
}
