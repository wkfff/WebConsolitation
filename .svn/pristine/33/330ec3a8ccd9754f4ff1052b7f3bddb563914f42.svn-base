using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Krista.FM.RIA.Core.Extensions;
using Krista.FM.RIA.Core.Gui;
using Krista.FM.ServerLibrary;

namespace Krista.FM.RIA.Extensions.FO41.Presentation.Views.HMAO
{
    public class XTaxView : XView
    {
        public XTaxView(IScheme scheme, string config, IParametersService parametersService)
            : base(typeof(HMAOTaxView), scheme, config, parametersService)
        {
        }

        protected override void InitializeControl(Control control, XElement xConfig, Dictionary<string, string> parameters)
        {
            var form = (HMAOTaxView)control;
            form.TaxId = -1;
            try
            {
                var tax = GetConfig().Element("Tax").Attribute("id").Value;
                form.TaxId = Convert.ToInt32(tax);
            }
            catch (Exception)
            {
            }
        }
    }
}
