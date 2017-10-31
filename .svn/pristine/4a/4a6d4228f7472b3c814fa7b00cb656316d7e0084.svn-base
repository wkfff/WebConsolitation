using System;
using System.Reflection;
using Krista.FM.RIA.Core.Extensions;
using Krista.FM.ServerLibrary;

namespace Krista.FM.RIA.Core.Gui
{
    public class XBuilderFactory
    {
        private readonly IScheme scheme;
        private readonly IParametersService parametersService;

        public XBuilderFactory(IScheme scheme, IParametersService parametersService)
        {
            this.scheme = scheme;
            this.parametersService = parametersService;
        }

        public XControl Create(View view)
        {
            return Create(view.Type, view.Config);
        }

        public XControl Create(Type type, string config)
        {
            object obj = Activator.CreateInstance(
                type,
                BindingFlags.CreateInstance,
                null,
                new object[] { scheme, config, parametersService },
                null);
            XControl xControl = obj as XControl;
            return xControl;
        }
    }
}
