using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Text;
using Krista.FM.Client.ViewObjects.BaseViewObject;

namespace Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Gui.Presentations
{
    public class Presentations
    {
        private XDocument XmlDoc
        {
            get; set;
        }

        public string GetPresentationKey(BaseViewObj viewObj)
        {
            if (XmlDoc == null)
                XmlDoc = GetPresentationsConfig();
            string oktmo =
                FinSourcePlanningNavigation.Instance.Workplace.ActiveScheme.GlobalConstsManager.Consts["OKTMO"].Value.
                    ToString();
            string type = viewObj.GetType().ToString();
            foreach (var guiElement in XmlDoc.Element("Presentations").Elements("Gui").Where(w => w.Attribute("key").Value == type))
            {
                foreach (var presentation in guiElement.Elements("Presentation").Where(w => w.Attribute("oktmo").Value == oktmo))
                    return presentation.Attribute("key").Value;
            }
            
            return string.Empty;
        }

        private XDocument GetPresentationsConfig()
        {
            var stream = GetType().Module.Assembly.GetManifestResourceStream("Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Gui.Presentations.Presentations.xml");
            if (stream == null)
                return null;
            var reader = XmlReader.Create(stream);
            return XDocument.Load(reader);
        }
    }
}
