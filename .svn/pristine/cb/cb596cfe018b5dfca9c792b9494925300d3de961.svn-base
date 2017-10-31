using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core.Extensions;
using Krista.FM.ServerLibrary;

namespace Krista.FM.RIA.Core.Gui
{
    public class XView : XControl
    {
        public XView(IScheme scheme, string config, IParametersService parametersService) 
            : this(typeof(View), scheme, config, parametersService)
        {
        }

        public XView(Type controlType, IScheme scheme, string config, IParametersService parametersService)
            : base(controlType, scheme, config, parametersService)
        {
        }

        protected override void InitializeControl(Control control, XElement xConfig, Dictionary<string, string> parameters)
        {
            View view = (View)control;
            view.Id = xConfig.Attribute("id").Value;
            view.Title = xConfig.Attribute("title").Value;
            view.Url = xConfig.Attribute("url").With(x => x.Value);

            foreach (var parameter in parameters)
            {
                view.Params.Add(parameter.Key, parameter.Value);
            }
        }
    }
}
