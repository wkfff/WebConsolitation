using System;
using System.Collections.Generic;

using Krista.FM.ServerLibrary;

namespace Krista.FM.Server.Scheme.ScriptingEngine.Classes
{
    internal class NullScriptingEngine : ScriptingEngineAbstraction
    {
        internal NullScriptingEngine(ScriptingEngineImpl impl)
            : base(impl)
        {
        }

        internal override List<string> CreateDependentScripts(IEntity entity, IDataAttribute withoutAttribute)
        {
			return new List<string>();
		}

        internal override List<string> CreateScript(ICommonDBObject obj)
        {
			return new List<string>();
		}

        internal override List<string> DropScript(ICommonDBObject obj)
        {
        	return new List<string>();
        }

        /// <summary>
        /// Определяет режим работы сервера с базой данных.
        /// Если с базой данных уже работает какой либо сервер приложений, то режим будет MultiServerMode.
        /// </summary>
        internal bool DetectMultiServerMode()
        {
            return _impl.DetectMultiServerMode();
        }
    }
}
