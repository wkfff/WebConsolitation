using System;
using System.Diagnostics;

using Krista.FM.Server.Scheme.ScriptingEngine.Classes;

namespace Krista.FM.Server.Scheme.ScriptingEngine
{
    internal class ScriptingEngineFactory
    {
        private readonly ScriptingEngineImpl _impl;

        public ScriptingEngineFactory(ScriptingEngineImpl impl)
        {
            if (impl == null)
                throw new ArgumentNullException("impl");
            _impl = impl;
        }

        public NullScriptingEngine NullScriptingEngine
        {
            [DebuggerStepThrough]
            get { return new NullScriptingEngine(_impl); }
        }

        public AttributeScriptingEngine AttributeScriptingEngine
        {
            [DebuggerStepThrough]
            get { return new AttributeScriptingEngine(_impl); }
        }

        public EntityScriptingEngine EntityScriptingEngine
        {
            [DebuggerStepThrough]
            get { return new EntityScriptingEngine(_impl); }
        }

        public ClassifierEntityScriptingEngine ClassifierEntityScriptingEngine
        {
            [DebuggerStepThrough]
            get { return new ClassifierEntityScriptingEngine(_impl); }
        }

        public FixedClassifierEntityScriptingEngine FixedClassifierEntityScriptingEngine
        {
            [DebuggerStepThrough]
            get { return new FixedClassifierEntityScriptingEngine(_impl); }
        }

        public FactEntityScriptingEngine FactEntityScriptingEngine
        {
            [DebuggerStepThrough]
            get { return new FactEntityScriptingEngine(_impl); }
        }

        public VariantDataClassifierScriptingEngine VariantDataClassifierScriptingEngine
        {
            [DebuggerStepThrough]
            get { return new VariantDataClassifierScriptingEngine(_impl); }
        }

        public EntityAssociationScriptingEngine EntityAssociationScriptingEngine
        {
            [DebuggerStepThrough]
            get { return new EntityAssociationScriptingEngine(_impl); }
        }

    }
}
