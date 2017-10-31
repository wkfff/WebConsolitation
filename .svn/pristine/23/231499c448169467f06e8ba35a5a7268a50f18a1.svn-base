using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.Utilities;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.Gui;

namespace Krista.FM.RIA.Extensions.FO51PassportMO.Presentation.Views
{
    public class ChoosePeriodView : View
    {
        /// <summary>
        /// Стиль для контролов
        /// </summary>
        private const string StyleAll = "margin: 0px 0px 5px 0px; font-size: 12px;";
        
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
                                                     Text = @"Выберите отчетный год:",
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

        private static ComboBox GetPeriodCombo(ViewPage page)
        {
            var ds = new Store { ID = "dsPeriodType", AutoLoad = true };

            ds.SetHttpProxy("/FO51Periods/LookupYearPeriod")
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

            var curYear = DateTime.Today.Month > 1 ? DateTime.Today.Year : DateTime.Today.Year - 1;
            orgCombo.SelectedItem.Value = curYear.ToString();
            orgCombo.SelectedItem.Text = "{0} год".FormatWith(curYear);

            orgCombo.DirectEvents.Select.CleanRequest = true;
            orgCombo.DirectEvents.Select.IsUpload = false;
            orgCombo.Listeners.Select.AddAfter("parent.Extension.PeriodView.year = periodsCombo.value;");

            orgCombo.AddScript("parent.Extension.PeriodView.year = {0}".FormatWith(curYear));
            return orgCombo;
        }
    }
}
