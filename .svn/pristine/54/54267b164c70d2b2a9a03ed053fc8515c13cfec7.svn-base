using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Krista.FM.RIA.Core.Extensions;
using Krista.FM.ServerLibrary;

namespace Krista.FM.RIA.Core.Gui
{
    public abstract class XControl 
    {
        private readonly IScheme scheme;
        private readonly string config;
        private readonly IParametersService parametersService;
        private readonly Type controlType;

        protected XControl(Type controlType, IScheme scheme, string config, IParametersService parametersService)
        {
            this.controlType = controlType;
            this.scheme = scheme;
            this.config = config;
            this.parametersService = parametersService;
        }

        public IParametersService ParametersService
        {
            get { return parametersService; }
        }

        public IScheme Scheme
        {
            get { return scheme; }
        }

        public Control Create()
        {
            return Create(new Dictionary<string, string>());
        }
        
        public Control Create(Dictionary<string, string> parameters)
        {
            Control control;
            XElement xConfig = GetConfig();

            var xControlType = xConfig.Attribute("controlType");
            Type cntType = controlType;
            if (xControlType != null)
            {
                cntType = Type.GetType(xControlType.Value);
            }

            control = ControlBuilder.Current.GetControllerFactory()
                .CreateControl(cntType);

            InitializeControl(control, xConfig, parameters);
            return control;
        }

        protected abstract void InitializeControl(Control control, XElement xConfig, Dictionary<string, string> parameters);

        protected XElement GetConfig()
        {
            return XElement.Parse(config);
        }
    }
}
