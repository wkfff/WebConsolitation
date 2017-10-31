using System.Collections.Generic;

using Krista.FM.Server.Scheme.Classes;

namespace Krista.FM.Server.Scheme.ScriptingEngine.Classes
{
    internal class FixedClassifierEntityScriptingEngine : ClassifierEntityScriptingEngine
    {
        /// <summary>
        /// Инициализация экземпляра.
        /// </summary>
        /// <param name="impl">Реализация СУБД зависимого функционала.</param>
        internal FixedClassifierEntityScriptingEngine(ScriptingEngineImpl impl)
            : base(impl)
        {
        }

        protected override List<string> CreateSequenceScript(Entity entity)
        {
            return new List<string>();
        }

        internal override List<string> CreateDeveloperSequenceScript(Entity entity)
        {
            return new List<string>();
        }

        protected override List<string> CreateTriggersScript(Entity entity, DataAttribute withoutAttribute)
        {
            return new List<string>();
        }
    }
}
