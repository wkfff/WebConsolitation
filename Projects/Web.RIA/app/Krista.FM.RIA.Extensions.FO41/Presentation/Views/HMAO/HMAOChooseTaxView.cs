using System.Collections.Generic;
using System.Web.Mvc;
using Ext.Net;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.Gui;

namespace Krista.FM.RIA.Extensions.FO41.Presentation.Views.HMAO
{
    public class HMAOChooseTaxView : View
    {
        /// <summary>
        /// Стиль для контролов
        /// </summary>
        private const string StyleAll = "margin: 0px 0px 5px 0px; font-size: 12px;";

        private readonly IFO41Extension extension;

        public HMAOChooseTaxView(IFO41Extension extension)
        {
            this.extension = extension;
        }

        public override List<Component> Build(ViewPage page)
        {
            if (extension.UserGroup != FO41Extension.GroupTaxpayer)
            {
                ResourceManager.GetInstance(page).RegisterOnReadyScript(
                    ExtNet.Msg.Alert("Ошибка", "Текущему пользователю не сопоставлен налогоплательщик.").
                        ToScript());

                return new List<Component>();
            }

            var chooseTaxPanel = new Panel
                                 {
                                     CssClass = "x-window-mc",
                                     BodyCssClass = "x-window-mc",
                                     Border = false,
                                     StyleSpec = StyleAll + "padding-top: 10px; padding-left: 15px;",
                                     Layout = "RowLayout",
                                     Items =
                                         {
                                             new DisplayField
                                                 {
                                                     Text = @"Выберите налог, по которому создается заявка:",
                                                     StyleSpec = StyleAll
                                                 },
                                             GetTaxTypeCombo(page)
                                         }
                                 };

            return new List<Component>
                       {
                           new Viewport
                               {
                                   Items = { new BorderLayout { Center = { Items = { chooseTaxPanel } } } }
                               }
                       };
        }

        private static ComboBox GetTaxTypeCombo(ViewPage page)
        {
            var ds = new Store { ID = "dsTaxType", AutoLoad = true };

            ds.SetHttpProxy("/FO41HMAORequestsList/LookupTaxType")
                .SetJsonReader()
                .AddField("ID")
                .AddField("Name");
            
            var orgCombo = new ComboBox
            {
                ID = "taxTypeCombo",
                AllowBlank = false,
                Width = 300,
                TriggerAction = TriggerAction.All,
                StoreID = ds.ID,
                ValueField = "ID",
                DisplayField = "Name"
            };
            page.Controls.Add(ds);

            orgCombo.SelectedItem.Value = "4";
            orgCombo.SelectedItem.Text = "Налог на прибыль организаций";

            orgCombo.DirectEvents.Select.CleanRequest = true;
            orgCombo.DirectEvents.Select.IsUpload = false;

            return orgCombo;
        }
    }
}
