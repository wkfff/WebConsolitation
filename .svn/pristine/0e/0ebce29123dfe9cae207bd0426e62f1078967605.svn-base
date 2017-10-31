using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.Extensions;
using Krista.FM.RIA.Core.Gui;
using Krista.FM.RIA.Core.ViewModel;
using Krista.FM.RIA.Extensions.DebtBook.Services.ControlRelationships;
using Krista.FM.ServerLibrary;

namespace Krista.FM.RIA.Extensions.DebtBook.Presentation.ViewBuilders
{
    public class XBebtBookFormView : XView
    {
        public XBebtBookFormView(IScheme scheme, string config, IParametersService parametersService)
            : base(typeof(BebtBookFormView), scheme, config, parametersService)
        {
        }

        protected override void InitializeControl(Control control, XElement xConfig, Dictionary<string, string> parameters)
        {
            base.InitializeControl(control, xConfig, parameters);

            var formView = (BebtBookFormView)control;

            formView.Id = xConfig.Attribute("id").Value;
            formView.Title = xConfig.Attribute("title").Value;
            formView.TabRegionType = Convert.ToInt32(xConfig.Attribute("tabRegionType").Value);

            if (parameters != null && parameters.ContainsKey("recordId"))
            {
                formView.RecordId = Convert.ToInt32(parameters["recordId"]);
            }

            var xReadonly = xConfig.Attribute("readonly");
            if (xReadonly != null)
            {
                formView.Readonly = Convert.ToBoolean(xReadonly.Value);
            }

            string entityKey = xConfig.Element("Entity").Attribute("objectKey").Value;
            entityKey = new Expression(ParametersService).Eval(entityKey);
            formView.Entity = Scheme.RootPackage.FindEntityByName(entityKey);

            if (xConfig.Element("Presentation") != null)
            {
                string presentationKey = xConfig.Element("Presentation").Attribute("objectKey").Value;
                presentationKey = new Expression(ParametersService).Eval(presentationKey);
                if (formView.Entity.Presentations.ContainsKey(presentationKey))
                {
                    formView.Presentation = formView.Entity.Presentations[presentationKey];
                }
            }

            if (xConfig.Element("StoreService") != null)
            {
                Type storeServiceType = Type.GetType(xConfig.Element("StoreService").Attribute("type").Value);
                formView.ViewService = Resolver.Get(storeServiceType) as IViewService;
            }

            var xFields = xConfig.Element("Fields");
            if (xFields != null)
            {
                if (new Condition(ParametersService).Test(xFields))
                {
                    foreach (var xField in xFields.Elements("Field"))
                    {
                        if (new Condition(ParametersService).Test(xField))
                        {
                            var key = xField.Attribute("name").Value;
                            var calc = xField.Attribute("calc").Value.ToUpper();
                            formView.Fields.Add(key.ToUpper(), new ColumnState { CalcFormula = calc });
                        }
                    }
                }
            }

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
                            formView.ControlRelationships.Add(new ControlRelationship(expression, message));
                        }
                    }
                }
            }
        }
    }
}
