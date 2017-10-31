using System;
using Krista.FM.ServerLibrary;

namespace Krista.FM.RIA.Extensions.EO15TargetPrograms
{
    public class Extension : IExtension
    {
        private readonly IScheme scheme;

        public Extension(IScheme scheme)
        {
            this.scheme = scheme;
        }

        public string OKTMO { get; private set; }
        
        public bool Initialize()
        {
            try
            {
                OKTMO = Convert.ToString(scheme.GlobalConstsManager.Consts["OKTMO"].Value);
                return true;
            }
            catch (Exception e)
            {
                Trace.TraceError("Ошибка инициализации модуля \"Целевые программы\": {0}", e.Message);
                return false;
            }
        }
    }
}
