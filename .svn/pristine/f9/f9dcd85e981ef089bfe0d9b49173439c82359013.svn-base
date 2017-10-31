using System.Collections.Generic;
using Krista.FM.RIA.Core.Extensions;
using Krista.FM.RIA.Core.Gui;
using Krista.FM.RIA.Extensions.DebtBook.Services.ControlRelationships;
using Krista.FM.ServerLibrary;

namespace Krista.FM.RIA.Extensions.DebtBook.Presentation.ViewBuilders
{
    public class XBebtBookGridView : XGridView
    {
        public XBebtBookGridView(IScheme scheme, string config, IParametersService parametersService) 
            : base(scheme, config, parametersService)
        {
        }

        protected override void InitializeControl(Control control, System.Xml.Linq.XElement xConfig, Dictionary<string, string> parameters)
        {
            base.InitializeControl(control, xConfig, parameters);

            if (control is BebtBookGridView)
            {
                var gridView = (BebtBookGridView)control;

                var xRelationships = xConfig.Element("ControlRelationships");
                if (xRelationships != null)
                {
                    if (new Condition(ParametersService).Test(xRelationships))
                    {
                        foreach (var xRelationship in xRelationships.Elements("Relationship"))
                        {
                            if (new Condition(ParametersService).Test(xRelationship))
                            {
                                var expression = xRelationship.Attribute("expression").Value;
                                var message = xRelationship.Attribute("errorMessage").Value;
                                gridView.ControlRelationships.Add(new ControlRelationship(expression, message));
                            }
                        }
                    }
                }
            }
        }
    }
}
