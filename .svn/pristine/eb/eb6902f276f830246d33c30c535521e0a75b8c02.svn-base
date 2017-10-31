using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.Extensions;
using Krista.FM.RIA.Core.Gui;
using Krista.FM.RIA.Core.ViewModel;
using Krista.FM.ServerLibrary;

namespace Krista.FM.RIA.Extensions.DebtBook.Presentation.ViewBuilders
{
    public class XFormEntityWithDetailsView : XView
    {
        public XFormEntityWithDetailsView(IScheme scheme, string config, IParametersService parametersService) 
            : base(typeof(FormEntityWithDetailsView), scheme, config, parametersService)
        {
        }

        protected override void InitializeControl(Control control, XElement xConfig, Dictionary<string, string> parameters)
        {
            var form = (FormEntityWithDetailsView)control;

            form.Id = xConfig.Attribute("id").Value;
            form.Title = xConfig.Attribute("title").Value;

            string entityKey = xConfig.Element("Entity").Attribute("objectKey").Value;
            form.Entity = Scheme.RootPackage.FindEntityByName(entityKey);

            string presentationKey = xConfig.Element("Presentation").Attribute("objectKey").Value;
            presentationKey = new Expression(ParametersService).Eval(presentationKey);
            if (form.Entity.Presentations.ContainsKey(presentationKey))
            {
                form.Presentation = form.Entity.Presentations[presentationKey];
            }

            Type storeServiceType = Type.GetType(xConfig.Element("StoreService").Attribute("type").Value);
            form.ViewService = Resolver.Get(storeServiceType) as IViewService;

            // Параметры
            var xParams = xConfig.Element("Params");
            if (xParams != null)
            {
                foreach (var xParameter in xParams.Elements("Parameter"))
                {
                    form.Params.Add(
                        xParameter.Attribute("name").Value,
                        xParameter.Attribute("value").Value);
                }
            }

            // Обработчики событий
            var xStoreListeners = xConfig.Element("StoreListeners");
            if (xStoreListeners != null)
            {
                foreach (var xListener in xStoreListeners.Elements("Listener"))
                {
                    form.StoreListeners.Add(
                        xListener.Attribute("name").Value,
                        xListener.Value);
                }
            }
        }
    }
}
