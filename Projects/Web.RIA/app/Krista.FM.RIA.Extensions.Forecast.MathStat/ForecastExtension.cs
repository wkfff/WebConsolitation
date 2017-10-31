using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.ServerLibrary;

namespace Krista.FM.RIA.Extensions.Forecast.MathStat
{
    public class ForecastExtension : IForecastExtension
    {
        private readonly IRepository<Users> userRepository;

        public ForecastExtension(IScheme scheme, IRepository<Users> userRepository)
        {
            Scheme = scheme;
            this.userRepository = userRepository;
        }

        public Users User { get; private set; }

        public int UserID { get; private set; }

        public Dictionary<string, UserFormsControls> Forms { get; private set; }

        public MathGroups LoadedMathGroups { get; private set; }

        public IScheme Scheme { get; private set; }
        
        public bool Initialize()
        {
            try
            {
                var currentUserId = Scheme.UsersManager.GetCurrentUserID();

                UserID = currentUserId;
                User = userRepository.Get(currentUserId);
                Forms = new Dictionary<string, UserFormsControls>();

                LoadedMathGroups = new MathGroups();

                var methodLoader = new MethodsLoader(this, Core.Resolver.Get<IForecastMethodsRepository>());

                methodLoader.LoadMethods();
                
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
