using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Krista.FM.RIA.Core.Extensions;
using Krista.FM.ServerLibrary;

namespace Krista.FM.RIA.Core.Gui
{
    public class XTabbedView : XControl
    {
        public XTabbedView(IScheme scheme, string config, IParametersService parametersService)
            : base(typeof(TabbedView), scheme, config, parametersService)
        {
        }

        protected override void InitializeControl(Control control, XElement xConfig, Dictionary<string, string> parameters)
        {
            TabbedView view = (TabbedView)control;
            view.Id = xConfig.Attribute("id").Value;

            foreach (var xTab in xConfig.Elements("Tab"))
            {
                if (new Condition(ParametersService).Test(xTab))
                {
                    Type type = Type.GetType(xTab.Attribute("type").Value);
                    XControl c = new XBuilderFactory(Scheme, ParametersService).Create(type, xTab.ToString());
                    view.Tabs.Add(c.Create());
                }
            }
        }
    }
}
