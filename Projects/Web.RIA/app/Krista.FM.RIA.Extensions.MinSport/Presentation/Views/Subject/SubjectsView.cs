using System.Collections.Generic;
using System.Web.Mvc;
using Ext.Net;
using Krista.FM.RIA.Core.Gui;

namespace Krista.FM.RIA.Extensions.MinSport
{
    public class SubjectsView : View
    {
        public override List<Component> Build(ViewPage page)
        {
            var tabPanel = new TabPanel
            {
                ID = "SubjectsTabPanel",
                IDMode = IDMode.Explicit,
                Border = false,
                ActiveTabIndex = 0,
                EnableTabScroll = true,
                MonitorResize = true,
                LayoutOnTabChange = true,
                Closable = true
            };

            tabPanel.Items.Add(new Panel
            {
                Title = "Муниципальные образования",
                AutoLoad =
                {
                    Url = "/View/tabTerritory",
                    Mode = LoadMode.IFrame,
                    TriggerEvent = "show",
                    ShowMask = true,
                    MaskMsg = "Загрузка..."
                }
            });
            tabPanel.Items.Add(new Panel
            {
                Title = "Кураторы проекта",
                AutoLoad =
                {
                    Url = "/View/tabPersonCurator",
                    Mode = LoadMode.IFrame,
                    TriggerEvent = "show",
                    ShowMask = true,
                    MaskMsg = "Загрузка..."
                }
            });          
            return new List<Component> { new Viewport { Items = { new FitLayout { Items = { tabPanel } } } } };
        }
    }
}
