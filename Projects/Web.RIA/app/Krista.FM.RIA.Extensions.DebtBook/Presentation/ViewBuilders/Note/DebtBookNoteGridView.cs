using System.Collections.Generic;
using System.Web.Mvc;
using Ext.Net;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core;
using View = Krista.FM.RIA.Core.Gui.View;

namespace Krista.FM.RIA.Extensions.DebtBook.Presentation.ViewBuilders.Note
{
    public class DebtBookNoteGridView : View
    {
        private const string NoteGridID = "gpNote";
        private const string NoteGridStoreID = "dsNote";
        private const string NoteFormPanelID = "formNote";

        public override List<Component> Build(ViewPage page)
        {
            var view = new Viewport
            {
                ID = "viewportMain",
                Items = 
                {
                    new FitLayout
                        {
                            Items =
                                {
                                    new FormPanel { ID = NoteFormPanelID, Layout = LayoutType.Fit.ToString(), Items = { CreateNoteListPanel() }, Border = false }
                                }
                        }
                }
            };

            return new List<Component> { view };
        }

        private Toolbar CreateTopBar()
        {
            var toolbar = new Toolbar();
            
            toolbar.Add(new Button
            {
                ID = "btnRefresh",
                Icon = Icon.ArrowRefresh,
                ToolTip = "Обновить",
                Listeners = { Click = { Handler = "{0}.reload();".FormatWith(NoteGridStoreID) } }
            });

            return toolbar;
        }

        private List<Component> CreateNoteListPanel()
        {
            var table = new GridPanel
            {
                ID = NoteGridID,
                Store = { GetNoteListStore() },
                TopBar = { CreateTopBar() },
                Border = false,
                AutoScroll = true,
                Layout = LayoutType.Fit.ToString(),
                TrackMouseOver = true,
                LoadMask = { ShowMask = true },
                ColumnModel =
                {
                    Columns =
                    {   
                        new Column { Hidden = true, DataIndex = "ID", MenuDisabled = true },
                        new ImageCommandColumn
                            {
                                Header = "Файл",
                                Commands  = { new ImageCommand { Icon = Icon.PageExcel, CommandName  = "OpenDocument", ToolTip = { Text = "Открыть документ" } } },
                                Width = 40, 
                                Fixed = true,
                                Align = Alignment.Center,
                                Css = "padding-left: 11px;"
                            },
                        new Column
                            {
                                Header = "Муниципальное образование", 
                                DataIndex = "RefTerritoryName", 
                                Width = 180,
                                Align = Alignment.Left
                            },
                    }
                },
                AutoExpandColumn = "RefTerritoryName",
                SelectionModel = { new RowSelectionModel { ID = "RowSelectionModel1", SingleSelect = true } },
                Listeners = { Command = { Handler = "if (command != undefined && command=='OpenDocument'){DownloadDoc(record.data.ID)}" } }
            };
            
            table.AddScript(@"
var DownloadDoc = function(id){{
  if (id != undefined){{
    Ext.net.DirectMethod.request({{
       url: '{0}',
       isUpload: true,
       formProxyArg: '{1}',
       cleanRequest: true,
       params: {{ id: id }},
       failure: failureSaveHandler
    }});
  }}
}}
".FormatWith("/DebtBookNote/DownloadNote", NoteFormPanelID));

            table.AddScript(@"
var failureSaveHandler = function (response, result) {
    if (result.extraParams != undefined && result.extraParams.responseText != undefined) {
        Ext.Msg.alert('Ошибка', result.extraParams.responseText);
    } else {
        var responseParams = Ext.decode(result.responseText);
        if (responseParams != undefined && responseParams.extraParams != undefined && responseParams.extraParams.responseText != undefined) {
            Ext.Msg.alert('Ошибка', responseParams.extraParams.responseText);
        } else {
            Ext.Msg.alert('Ошибка', 'Server failed');
        }
    }
};
");
            return new List<Component> { table };
        }

        private Store GetNoteListStore()
        {
            var store = new Store
            {
                ID = NoteGridStoreID,
                AutoLoad = true
            };

            store.SetHttpProxy("/DebtBookNote/GetNotesTable");
            store.Reader.Add(new JsonReader
            {
                IDProperty = "ID",
                Root = "data",
                Fields =
                { 
                    new RecordField("ID"),
                    new RecordField("RefTerritoryName")
                }
            });
            
            return store;
        }
    }
}
