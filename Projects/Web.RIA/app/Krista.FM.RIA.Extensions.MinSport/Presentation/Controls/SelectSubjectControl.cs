using System.Collections.Generic;
using System.Web.Mvc;
using Ext.Net;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.Gui;

namespace Krista.FM.RIA.Extensions.MinSport.Presentation.Controls
{
    public class SelectSubjectControl : Control
    {
        public string ScriptReloadComponents { get; set; }

        public override List<Component> Build(ViewPage page)
        {
            var subjectsStore = CreateSubjectsStore();
            page.Controls.Add(subjectsStore);

            var sscTopPanel = new Panel
            {
                Padding = 10,
                AutoWidth = true,
                Border = false,
                BodyCssClass = "x-window-mc",
                CssClass = "x-window-mc",
                Items =
                {
                    CreateSubjectsComboBox()
                }
            };

            return new List<Component> { sscTopPanel };
        }

        private ComboBox CreateSubjectsComboBox()
        {
            var subjectsComboBox  = new ComboBox
            {
                ID = "subjectsComboBox",
                StoreID = "subjectsStore",
                FieldLabel = "Субъект РФ",
                Width = 300,
                DisplayField = "Text",
                ValueField = "Value",
                Mode = DataLoadMode.Local
            };
            subjectsComboBox.Listeners.Select.AddAfter(ScriptReloadComponents);
            return subjectsComboBox;
        }

        private Store CreateSubjectsStore()
        {
            var subjectsStore = new Store { ID = "subjectsStore" } .SetHttpProxy("/Territory/LoadComboBox")
                .SetJsonReader("Value", "data")
                .AddField("Text")
                .AddField("Value");
            return subjectsStore;
        }
    }
}
