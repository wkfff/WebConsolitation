using System.Collections.Generic;
using System.Web.Mvc;
using Ext.Net;
using Krista.FM.RIA.Core.Gui;

namespace Krista.FM.RIA.Extensions.MinSport.Presentation.Views.Propaganda
{
    public class PropagandaView : View
    {
        public override List<Component> Build(ViewPage page)
        {
            var tabPanel = new TabPanel
            {
                ID = "PropagandaTabPanel",
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
                Title = "Периодические печатные издания",
                AutoLoad =
                {
                    Url = "/View/tabPrints",
                    Mode = LoadMode.IFrame,
                    TriggerEvent = "show",
                    ShowMask = true,
                    MaskMsg = "Загрузка..."
                }
            });

            tabPanel.Items.Add(new Panel
            {
                Title = "Радио- теле- видео- кинохроникальные программы",
                AutoLoad =
                {
                    Url = "/View/tabMedia",
                    Mode = LoadMode.IFrame,
                    TriggerEvent = "show",
                    ShowMask = true,
                    MaskMsg = "Загрузка..."
                }
            });

            tabPanel.Items.Add(new Panel
            {
                Title = "Интернет-ресурсы",
                AutoLoad =
                {
                    Url = "/View/tabInternet",
                    Mode = LoadMode.IFrame,
                    TriggerEvent = "show",
                    ShowMask = true,
                    MaskMsg = "Загрузка..."
                }
            });

            tabPanel.Items.Add(new Panel
            {
                Title = "Массовые акции, мероприятия, проекты",
                AutoLoad =
                {
                    Url = "/View/tabStock",
                    Mode = LoadMode.IFrame,
                    TriggerEvent = "show",
                    ShowMask = true,
                    MaskMsg = "Загрузка..."
                }
            });

            tabPanel.Items.Add(new Panel
            {
                Title = "Работа со СМИ",
                AutoLoad =
                {
                    Url = "/View/tabSmi",
                    Mode = LoadMode.IFrame,
                    TriggerEvent = "show",
                    ShowMask = true,
                    MaskMsg = "Загрузка..."
                }
            });

            tabPanel.Items.Add(new Panel
            {
                Title = "Меры по формированию ЗОЖ; исследования по выявлению интересов; мотиваций различных групп населения",
                AutoLoad =
                {
                    Url = "/View/tabStudy",
                    Mode = LoadMode.IFrame,
                    TriggerEvent = "show",
                    ShowMask = true,
                    MaskMsg = "Загрузка..."
                }
            });

            tabPanel.Items.Add(new Panel
            {
                Title = "Издание методических пособий",
                AutoLoad =
                {
                    Url = "/View/tabToolkit",
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
