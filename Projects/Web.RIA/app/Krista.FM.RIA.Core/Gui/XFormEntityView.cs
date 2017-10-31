using System.Collections.Generic;
using System.Xml.Linq;
using Krista.FM.RIA.Core.Extensions;
using Krista.FM.ServerLibrary;

namespace Krista.FM.RIA.Core.Gui
{
    public class XFormEntityView : XControl
    {
        public XFormEntityView(IScheme scheme, string config, IParametersService parametersService) 
            : base(typeof(FormEntityView), scheme, config, parametersService)
        {
        }

        protected override void InitializeControl(Control control, XElement xConfig, Dictionary<string, string> parameters)
        {
            var form = (FormEntityView)control;

            form.Id = xConfig.Attribute("id").Value;
            form.Title = xConfig.Attribute("title").Value;

            string entityKey = xConfig.Element("Entity").Attribute("objectKey").Value;
            entityKey = new Expression(ParametersService).Eval(entityKey);
            form.Entity = Scheme.RootPackage.FindEntityByName(entityKey);

            if (xConfig.Element("Presentation") != null)
            {
                string presentationKey = xConfig.Element("Presentation").Attribute("objectKey").Value;
                presentationKey = new Expression(ParametersService).Eval(presentationKey);
                if (form.Entity.Presentations.ContainsKey(presentationKey))
                {
                    form.Presentation = form.Entity.Presentations[presentationKey];
                }
            }

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
