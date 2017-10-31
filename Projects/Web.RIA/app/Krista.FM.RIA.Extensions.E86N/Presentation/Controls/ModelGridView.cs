using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web.Mvc;

using Ext.Net;

using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.Gui;
using Krista.FM.RIA.Extensions.E86N.Extensions;
using Krista.FM.RIA.Extensions.E86N.Models;
using Krista.FM.ServerLibrary;

using LinqKit;

using Component = Ext.Net.Component;
using GridView = Ext.Net.GridView;

namespace Krista.FM.RIA.Extensions.E86N.Presentation.Controls
{
    /// <summary>
    /// Grid котрый строится автоматически по данным из модели.
    /// </summary>
    public class ModelGridView : Control
    {
        private ViewModelBase model;

        public ModelGridView()
        {
            Store = new Store
            {
                ShowWarningOnFailure = true,
                WarningOnDirty = true,
                DirtyWarningText =
                    "Есть несохраненные изменения, которые будут потеряны при обновлении. Вы уверены, что хотите обновить данные?",
                DirtyWarningTitle = @"Несохраненные изменения",
                Restful = true,
                Reader =
                            {
                                new JsonReader
                                    {
                                        IDProperty = "ID", 
                                        Root = "data", 
                                        SuccessProperty = "success"
                                    }
                            },
                Listeners =
                {
                    LoadException =
                    {
                        Handler = @"Ext.Msg.alert('Ошибка загрузки', response.responseText);"
                    },

                    Save =
                    {
                        Handler =
                            @"Ext.net.Notification.show({ iconCls : 'icon-information', html : arg.raw.message, title : 'Сохранение', hideDelay  : 2000});"
                    },
                    Destroy =
                    {
                        Handler =
                            @"Ext.net.Notification.show({ iconCls : 'icon-information', html : arg.raw.message, title : 'Удаление', hideDelay  : 2000});"
                    },
                    SaveException =
                    {
                        Handler =
                            @"Ext.Msg.alert('Ошибка сохранения', response.responseText);"
                    },
                    Exception =
                    {
                        Handler =
                            @"if (response.raw != undefined && response.raw.message != undefined){Ext.Msg.alert('Общая ошибка', response.raw.message);}else{Ext.Msg.alert('Неизвестная ошибка', response.responseText);}"
                    }
                }
            };

            Grid = new GridPanel
                {
                    Border = false,
                    TrackMouseOver = true,
                    AutoScroll = true,
                    Icon = Icon.DatabaseTable,
                    LoadMask =
                        {
                            ShowMask = true,
                            Msg = "Загрузка..."
                        },
                    Layout = LayoutType.Fit.ToString(),
                    ClearEditorFilter = false,
                    StripeRows = true
                };

            Grid.View.Add(new GridView { ForceFit = true });
            var selMod = new RowSelectionModel { SingleSelect = true };
            Grid.SelectionModel.Add(selMod);
            Grid.ColumnModel.AddColumn("ID", "ID", DataAttributeTypes.dtInteger).SetHidden(true);

            ////Set Default Value
            LoadAction = "Read";
            UpdeteAction = "Update";
            DeleteAction = "Delete";
            ReadOnly = false;
            AutoLoad = false;
            PageSize = 0;
        }

        /// <summary>
        /// Метод для доступа к стору.
        /// </summary>
        public Store Store { get; private set; }

        /// <summary>
        /// Метод для доступа к гриду.
        /// </summary>
        public GridPanel Grid { get; private set; }

        /// <summary>
        /// Модель, которую отображает данный контрол.
        /// </summary>
        public ViewModelBase Model
        {
            get
            {
                return model;
            }

            set
            {
                model = value;
                var name = Model.GetType().Name;
                Grid.ID = string.Concat(name, "Grid");
                Grid.StoreID = Store.ID = string.Concat(name, "Store");
            }
        }

        /// <summary>
        /// Контроллер для работы с контролом.
        /// </summary>
        public Type ActionController { get; set; }

        /// <summary>
        /// Имя метода действия загрузки данных.
        /// </summary>
        [DefaultValue("Read")]
        public string LoadAction { get; set; }

        /// <summary>
        /// Имя метода действия обнвления данных.
        /// </summary>
        [DefaultValue("Update")]
        public string UpdeteAction { get; set; }

        /// <summary>
        /// Имя метода действия удаления данных.
        /// </summary>
        [DefaultValue("Delete")]
        public string DeleteAction { get; set; }

        /// <summary>
        /// Отвечает можно ли изменять данные.
        /// </summary>
        [DefaultValue(false)]
        public bool ReadOnly { get; set; }

        /// <summary>
        /// Отвечает за автозагрузка стора.
        /// </summary>
        [DefaultValue(false)]
        public bool AutoLoad
        {
            set { Store.AutoLoad = value; }
        }

        /// <summary>
        /// Это размер страниц. Если 0 на страницы не разбивает.
        /// </summary>
        [DefaultValue(0)]
        public int PageSize { get; set; }

        public override List<Component> Build(ViewPage page)
        {
            Store.AddFieldsByClass(Model);
            Store.Proxy.Add(
                new HttpProxy
                    {
                        RestAPI =
                            {
                                ReadMethod = HttpMethod.GET,
                                CreateMethod = HttpMethod.POST,
                                UpdateMethod = HttpMethod.POST,
                                DestroyMethod = HttpMethod.DELETE,

                                ReadUrl = string.Concat("/", UiBuilders.GetControllerID(ActionController), "/", LoadAction),
                                CreateUrl = string.Concat("/", UiBuilders.GetControllerID(ActionController), "/", UpdeteAction),
                                UpdateUrl = string.Concat("/", UiBuilders.GetControllerID(ActionController), "/", UpdeteAction),
                                DestroyUrl = string.Concat("/", UiBuilders.GetControllerID(ActionController), "/", DeleteAction)
                            }
                    });

            var gridFilters = new GridFilters { Local = false };
            Model.GetType().GetProperties().ForEach(
                info =>
                {
                    Grid.ColumnModel.AddColumn(info, ReadOnly);
                    gridFilters.Filters.Add(UiBuilders.FilterFactory(info));
                });
            Grid.Plugins.Add(gridFilters);

            if (PageSize > 0)
            {
                Store.BaseParams.Add(new Parameter("limit", "pageSizeServices.getValue()", ParameterMode.Raw));
                Store.BaseParams.Add(new Parameter("start", "0", ParameterMode.Raw));

                var toolbar = new PagingToolbar
                    {
                        ID = "paggingServices",
                        StoreID = Store.ID,
                        PageSize = 100
                    };

                toolbar.Items.Add(
                    new NumberField
                        {
                            ID = "pageSizeServices",
                            FieldLabel = @"Услуг на страницу",
                            LabelWidth = 150,
                            Width = 200,
                            Number = PageSize,
                            Listeners = { Change = { Handler = "#{paggingServices}.pageSize = parseInt(this.getValue());" } }
                        });
                Grid.BottomBar.Add(toolbar);
            }

            page.Controls.Add(Store);
            Grid.AddColumnsWrapStylesToPage(page);
            return new List<Component> { Grid };
        }
    }
}
