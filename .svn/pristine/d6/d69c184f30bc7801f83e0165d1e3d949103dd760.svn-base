using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Ext.Net;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.Gui;

namespace Krista.FM.RIA.Extensions.FO41.Presentation.Views.HMAO
{
    public class HMAOChoosePeriodView : View
    {
        /// <summary>
        /// Стиль для контролов
        /// </summary>
        private const string StyleAll = "margin: 0px 0px 5px 0px; font-size: 12px;";

        /// <summary>
        /// Тип налога
        /// </summary>
        private readonly int taxTypeId;

        private readonly IFO41Extension extension;

        public HMAOChoosePeriodView(IFO41Extension extension, int taxTypeId)
        {
            this.extension = extension;
            this.taxTypeId = taxTypeId;
        }

        public override List<Component> Build(ViewPage page)
        {
            var choosePeriodPanel = new Panel
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
                                                     Text = @"Выберите отчетный период:",
                                                     StyleSpec = StyleAll
                                                 },
                                             GetPeriodCombo(page)
                                         }
                                 };

            return new List<Component>
                       {
                           new Viewport
                               {
                                   Items = { new BorderLayout { Center = { Items = { choosePeriodPanel } } } }
                               }
                       };
        }

        private ComboBox GetPeriodCombo(ViewPage page)
        {
            var ds = new Store { ID = "dsPeriodType", AutoLoad = true };

            ds.SetHttpProxy("/FO41HMAOPeriods/LookupPeriods?taxTypeId={0}".FormatWith(taxTypeId))
                .SetJsonReader()
                .AddField("ID")
                .AddField("Name");
            
            var orgCombo = new ComboBox
            {
                ID = "periodsCombo",
                Editable = false,
                AllowBlank = false,
                Width = 300,
                TriggerAction = TriggerAction.All,
                StoreID = ds.ID,
                ValueField = "ID",
                DisplayField = "Name"
            };
            page.Controls.Add(ds);

            var curPeriod = extension.GetPrevPeriod();
            orgCombo.SelectedItem.Value = curPeriod.ToString();
            orgCombo.SelectedItem.Text = "{0} квартал {1} года".FormatWith(curPeriod / 10, curPeriod / 10000);

            orgCombo.DirectEvents.Select.CleanRequest = true;
            orgCombo.DirectEvents.Select.IsUpload = false;
            orgCombo.Listeners.Select.AddAfter("parent.Extension.PeriodView.periodId = periodsCombo.value;");

            orgCombo.AddScript("parent.Extension.PeriodView.periodId = {0}".FormatWith(curPeriod));
            return orgCombo;
        }
    }
}
