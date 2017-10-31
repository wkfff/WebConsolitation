using System.Collections.Generic;
using System.Web.Mvc;
using Ext.Net;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.Gui;
using Krista.FM.ServerLibrary;
using Control = System.Web.UI.Control;
using GridView = Ext.Net.GridView;

namespace Krista.FM.RIA.Extensions.Consolidation
{
    public class ReglView : View
    {
        public override List<Component> Build(ViewPage page)
        {
            RestActions restActions = ResourceManager.GetInstance(page).RestAPI;
            restActions.Create = HttpMethod.POST;
            restActions.Read = HttpMethod.GET;
            restActions.Update = HttpMethod.POST;
            restActions.Destroy = HttpMethod.DELETE;

            page.Controls.Add(CreateReglamentStore());
            page.Controls.Add(CreateRoleLookupStore());
            page.Controls.Add(CreateLevelLookupStore());
            page.Controls.Add(CreateFormLookupStore());
            page.Controls.Add(CreateTemplateLookupStore());

            return new List<Component>
            {
                new Viewport
                {
                    ID = "viewportMain",
                    Items = 
                    {
                        new BorderLayout
                        {
                            West = { Items = { CreateFilterPanel() } },
                            Center = { Items = { CreateReglamentGridPanel(page) } }
                        }
                    }
                }
            };
        }

        private static Control CreateReglamentStore()
        {
            var ds = new Store
            {
                ID = "dsRegl",
                Restful = true,
                ShowWarningOnFailure = false,
                SkipIdForNewRecords = false,
                RefreshAfterSaving = RefreshAfterSavingMode.None
            };

            ds.SetRestController("ConsRegl")
                .SetJsonReader()
                .AddField("ID")
                .AddField("Role")
                .AddField("Level")
                .AddField("RepKind")
                .AddField("Template")
                .AddField("TemplateGroup")
                .AddField("Laboriousness")
                .AddField("Workdays", new RecordField.Config { DefaultValue = "false" })
                .AddField("Note")
                .AddField("RefRole")
                .AddField("RefLevel")
                .AddField("RefRepKind")
                .AddField("RefTemplate");

            ds.Listeners.Exception.Handler =
                @"
                    Ext.net.Notification.show({
                        iconCls    : 'icon-exclamation', 
                        html       : e && e.message ? e.message : response.message || response.statusText, 
                        title      : 'EXCEPTION', 
                        hideDelay  : 5000
                    });";

            ds.Listeners.Save.Handler =
                @" Ext.net.Notification.show({
                        iconCls    : 'icon-information', 
                        html       : arg.message, 
                        title      : 'Success', 
                        hideDelay  : 5000
                    });";

            return ds;
        }

        private static Control CreateRoleLookupStore()
        {
            var ds = new Store { ID = "dsRole", AutoLoad = false };

            ds.SetHttpProxy("/ConsRegl/LookupRole")
                .SetJsonReader()
                .AddField("ID")
                .AddField("Name");

            return ds;
        }

        private static Control CreateLevelLookupStore()
        {
            var ds = new Store { ID = "dsLevel", AutoLoad = false };

            ds.SetHttpProxy("/ConsRegl/LookupLevel")
                .SetJsonReader()
                .AddField("ID")
                .AddField("Code");

            return ds;
        }

        private static Control CreateFormLookupStore()
        {
            var ds = new Store { ID = "dsRepKind", AutoLoad = false };

            ds.SetHttpProxy("/ConsRegl/LookupRepKind")
                .SetJsonReader()
                .AddField("ID")
                .AddField("Name");

            return ds;
        }

        private static Control CreateTemplateLookupStore()
        {
            var ds = new Store { ID = "dsTemplate", AutoLoad = false };

            ds.SetHttpProxy("/ConsRegl/LookupTemplate")
                .SetJsonReader()
                .AddField("ID")
                .AddField("Name")
                .AddField("NameCD");

            return ds;
        }

        private static IEnumerable<Component> CreateReglamentGridPanel(ViewPage page)
        {
            var gp = new GridPanel
            {
                ID = "gpRegl",
                Icon = Icon.Table,
                Frame = true,
                Title = "Регламент",
                StoreID = "dsRegl",
                SelectionModel = { new RowSelectionModel() },
                View = { new GridView { ForceFit = false } },
            };

            gp.ColumnModel.AddColumn("RepKind", "Вид отчетности", DataAttributeTypes.dtString)
                .SetWidth(150).SetLookup("RefRepKind", "dsRepKind", "ID", "Name");
            
            gp.ColumnModel.AddColumn("Role", "Роль субъекта", DataAttributeTypes.dtString)
                .SetLookup("RefRole", "dsRole", "ID", "Name");

            gp.ColumnModel.AddColumn("Level", "Уровень субъекта", DataAttributeTypes.dtString)
                .SetLookup("RefLevel", "dsLevel", "ID", "Code");

            gp.ColumnModel.AddColumn("Template", "Форма отчета", DataAttributeTypes.dtString)
                .SetWidth(200).SetLookup("RefTemplate", "dsTemplate", "ID", "Name", "srec.set('TemplateGroup', record.get('NameCD'));");
            
            gp.ColumnModel.AddColumn("TemplateGroup", "Сбор", DataAttributeTypes.dtString)
                .SetWidth(150).SetEditable(false);
            
            gp.ColumnModel.AddColumn("Laboriousness", "Трудоемкость", DataAttributeTypes.dtInteger)
                .SetEditableString().SetWidth(70);
            
            gp.ColumnModel.AddColumn("Workdays", "Рабочие дни", DataAttributeTypes.dtBoolean)
                .SetEditableBoolean().SetWidth(50);
            
            gp.ColumnModel.AddColumn("Note", "Примечание", DataAttributeTypes.dtString)
                .SetEditableString().SetWidth(200);

            gp.AddColumnsWrapStylesToPage(page);

            gp.AddRefreshButton();
            gp.AddSaveButton();
            gp.AddNewRecordButton();
            gp.AddRemoveRecordButton();

            return new List<Component> { gp };
        }

        private static Component CreateFilterPanel()
        {
            return new Panel.Builder()
                .ID("pnlFilter")
                .Title("Фильтры")
                .Width(150)
                .Border(false)
                .Collapsible(true)
                .Collapsed(true)
                .TitleCollapse(false);
        }
    }
}
