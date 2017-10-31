using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Ext.Net;
using Krista.FM.RIA.Core.Gui;
using Krista.FM.RIA.Core.ViewModel;
using Krista.FM.ServerLibrary;

namespace Krista.FM.RIA.Extensions.DebtBook.Presentation.ViewBuilders
{
    public class FormEntityWithDetailsView : FormEntityView
    {
        public IViewService ViewService { get; set; }
        
        public override List<Component> Build(ViewPage page)
        {
            FormPanel entityForm = new FormPanel
                                       {
                                           ID = "entityForm",
                                           Border = false,
                                           Url = "/Entity/Save/",
                                           Layout = "form",
                                           LabelWidth = 230,
                                           Padding = 5,
                                           TrackResetOnLoad = true
                                       };

            Panel mainPanel = new Panel { ID = "mainPanel" };
            mainPanel.Items.Add(entityForm);

            Viewport viewport = new Viewport { ID = "viewportMain" };
            viewport.Items.Add(mainPanel);

            return new List<Component> { viewport };
        }
    }
}
