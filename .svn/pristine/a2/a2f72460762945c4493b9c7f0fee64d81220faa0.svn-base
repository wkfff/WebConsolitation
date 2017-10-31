using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.ServerLibrary;

namespace Krista.FM.RIA.Extensions.MinSport
{
    public class MinSportExtension : IMinSportExtension
    {
        private readonly IRepository<Users> userRepository;

        public MinSportExtension(IScheme scheme, IRepository<Users> userRepository)
        {
            Scheme = scheme;
            this.userRepository = userRepository;
        }

        public Users CurrUser { get; private set; }

        public int UserID { get; private set; }

        public IScheme Scheme { get; private set; }
        
        public bool Initialize()
        {
            try
            {
                var currentUserId = Scheme.UsersManager.GetCurrentUserID();

                UserID = currentUserId;
                CurrUser = userRepository.Get(currentUserId);
                
                return true;
            }
            catch (Exception e)
            {
                Trace.TraceError(
                    "Ошибка инициализации модуля MinSport");

                return false;
            }
        }
    }
}
