using System.Collections.Generic;
using System.Web.Mvc;
using Ext.Net;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.Gui;
using Krista.FM.RIA.Extensions.MinSport.Presentation.Views;

namespace Krista.FM.RIA.Extensions.MinSport.Presentation.Controls
{
    public class MinSportGridControl : Control
    {
        public MinSportGridControl()
        {
            JsonReaderFields = new List<string>();
            Columns = new List<ColumnBase>();
            BaseParams = new Dictionary<string, string>();
            ToolBarButtons = new List<Button>();
            GridListeners = new Dictionary<string, string>();
            StoreAutoLoad = false;
        }

        public string GridName { get; set; }

        public string StoreName { get; set; }

        public string Title { get; set; }

        public List<string> JsonReaderFields { get; set; }

        public List<ColumnBase> Columns { get; set; }

        public Dictionary<string, string> BaseParams { get; set; }

        public string ControllerName { get; set; }

        public List<Button> ToolBarButtons { get; set; }

        public RowSelectionModel RowSelModel { get; set; }

        public bool HideEditColumn { get; set; }

        public Dictionary<string, string> GridListeners { get; set; }

        public bool StoreAutoLoad { get; set; }

        public override List<Component> Build(ViewPage page)
        {
            var resourceManager = ResourceManager.GetInstance(page);
            if (!ExtNet.IsAjaxRequest)
            {
                resourceManager.RegisterClientScriptBlock("BaseGridControl", Resource.GeneralFunctionJS);
            }

            var storeForBaseGrid = BuildStoreForBaseGrid();
            page.Controls.Add(storeForBaseGrid);

            return new List<Component> { BuildBaseGrid() };
        }

        protected virtual GridPanel BuildBaseGrid()
        {
            var toolBar = new Toolbar();
            var grid = new GridPanel
            {
                ID = GridName,
                StoreID = StoreName,
                Border = false,
                StripeRows = true,
                ColumnLines = true,
                TrackMouseOver = true,
                AutoWidth = true,
                Anchor = "100% 100%",
                Title = Title,
                ColumnModel =
                {
                    Columns =
                    {
                        new Column { DataIndex = "ID", Hidden = true },
                        new CommandColumn
                        {
                            Width = 30,
                            Hidden = HideEditColumn,
                            Commands =
                                {
                                    new GridCommand
                                    {
                                        CommandName = "EditRecord",
                                        Icon = Icon.Pencil,
                                        ToolTip =
                                            {
                                                Text =
                                                    "Редактировать запись"
                                            },
                                    }
                                }
                        }
                    }
                },
                TopBar = { toolBar }
            };
            
            if (RowSelModel != null)
            {
                grid.SelectionModel.Add(RowSelModel);
            }

            foreach (var column in Columns)
            {
                grid.ColumnModel.Columns.Add(column);
            }

            foreach (var button in ToolBarButtons)
            {
                grid.TopBar[0].Add(button);
            }

            foreach (KeyValuePair<string, string> kvp in GridListeners)
            {
                grid.Listeners.AddListerer(kvp.Key, kvp.Value);
            }

            return grid;
        }

        private Store BuildStoreForBaseGrid()
        {
            var gridStore = new Store
            {
                ID = StoreName,
                AutoLoad = StoreAutoLoad,
                Restful = true,
                ShowWarningOnFailure = true,
                RefreshAfterSaving = RefreshAfterSavingMode.None
            };
            gridStore.SetRestController(ControllerName)
                .SetJsonReader();
            foreach (KeyValuePair<string, string> kvp in BaseParams)
            {
                gridStore.SetBaseParams(kvp.Key, kvp.Value, ParameterMode.Raw);
            }

            foreach (var fields in JsonReaderFields)
            {
                gridStore.AddField(fields);
            }

            gridStore.Listeners.Save.Handler = "saveHandlerForGrid(); ";
            gridStore.Listeners.SaveException.Handler = "Ext.Msg.alert('Ошибка при сохранении', response.responseText);";
            gridStore.Listeners.Exception.Handler = @"
if (response.raw != undefined && response.raw.message != undefined){
   Ext.Msg.alert('Ошибка при сохранении', response.raw.message);
}else{
   Ext.Msg.show({ 
                                title   : 'Ошибочка', 
                                msg     : response.responseText, 
                                minWidth: 1000, 
                                modal   : true, 
                                icon    : Ext.Msg.ERROR, 
                                buttons : Ext.Msg.OK,
                                animEl: 'grdDocuments'
                            });}";
            return gridStore;
        }
    }
}
