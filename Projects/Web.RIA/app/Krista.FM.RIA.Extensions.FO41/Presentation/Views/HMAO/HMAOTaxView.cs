using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Ext.Net;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core.Gui;
using Krista.FM.RIA.Extensions.FO41.Services;

namespace Krista.FM.RIA.Extensions.FO41.Presentation.Views.HMAO
{
    public class HMAOTaxView : View
    {
        private readonly IFO41Extension extension;

        private readonly CategoryTaxpayerService categoryRepository;

        public HMAOTaxView(
            IFO41Extension extension, 
            CategoryTaxpayerService categoryRepository)
        {
            this.extension = extension;
            this.categoryRepository = categoryRepository;

            // интерфейс заявок по виду налога, период может варьироваться, по умолчанию - предыдущий период
            PeriodId = extension.GetPrevPeriod();
        }

        public int TaxId { get; set; }

        public int PeriodId { get; set; }

        public override List<Component> Build(ViewPage page)
        {
            if (extension.UserGroup != FO41Extension.GroupOGV)
            {
                ResourceManager.GetInstance(page).RegisterOnReadyScript(
                    ExtNet.Msg.Alert("Ошибка", "Текущему пользователю не сопоставлен Экономический орган.").
                        ToScript());

                return new List<Component>();
            }

            if (TaxId < 1)
            {
                ResourceManager.GetInstance(page).RegisterOnReadyScript(
                    ExtNet.Msg.Alert("Ошибка", "Тип налога не определен").ToScript());

                return new List<Component>();
            }

            var panel = new Panel
            {
                ID = "ApplicPanel",
                Items =
                                    {
                                        new BorderLayout
                                            {
                                                Center = { Items = { CreateRequestTabPanel(page) } }
                                            }
                                    }
            };

            return new List<Component> 
            {
                new Viewport
                {
                    ID = "viewportTax{0}".FormatWith(TaxId),
                    Items = { new BorderLayout { Center = { Items = { panel } } } }
                } 
            };
        }

        private IEnumerable<Component> CreateRequestTabPanel(ViewPage page)
        {
            var tabs = new TabPanel
            {
                ID = "Tax{0}TabPanel".FormatWith(TaxId),
                Border = false,
                ActiveTabIndex = 0,
                EnableTabScroll = true
            };

            var categories = categoryRepository.GetByTax(TaxId);
            foreach (var category in categories)
            {
                var categoryTab = new HMAOTaxCategoryView(extension, category, TaxId, PeriodId) { Title = category.ShortName };
                categoryTab.InitAll(page);
                tabs.Add(categoryTab);
            }

            tabs.Add(new HMAOResultTaxView(categoryRepository, TaxId, PeriodId).Build(page));

            return new List<Component> 
            {                 
                new Hidden { ID = "UrlIconStatus1", Value = ResourceManager.GetInstance().GetIconUrl(Icon.UserEdit) },
                new Hidden { ID = "UrlIconStatus2", Value = ResourceManager.GetInstance().GetIconUrl(Icon.UserMagnify) },
                new Hidden { ID = "UrlIconStatus3", Value = ResourceManager.GetInstance().GetIconUrl(Icon.UserTick) },
                new Hidden { ID = "UrlIconStatus4", Value = ResourceManager.GetInstance().GetIconUrl(Icon.Accept) },
                new Hidden { ID = "UrlIconStatus5", Value = ResourceManager.GetInstance().GetIconUrl(Icon.Cancel) },
                new FormPanel { ID = "HMAOTaxForm{0}".FormatWith(TaxId) },
                tabs 
            };
        }
    }
}
