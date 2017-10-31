using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using Ext.Net;
using Krista.FM.Domain;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.Helpers;
using Krista.FM.RIA.Extensions.E86N.Presentation.Views;
using Krista.FM.ServerLibrary;

[assembly: WebResource("Krista.FM.RIA.Core.Extensions.css.CustomSearch.css", "text/css")]

namespace Krista.FM.RIA.Extensions.E86N.Presentation.Controls.SearchCombobox
{
    public static class ComboboxEditorExtensions
    {
        private const string Scope = "E86n.View.Control.SearchComboboxControl.";

        /// <summary>
        ///   Выпадающий список для поля, подставляет значения Name и ID справочника.
        /// </summary>
        /// <param name="column"> Колонка, к которой добавляется выпадающий список в качестве редактора. </param>
        /// <param name="fieldId"> Идентификатор колонки, соответстующей полю ID. </param>
        /// <param name="fieldName"> Идентификатор колонки, соответствующей полю Name. </param>
        /// <param name="url"> Url, по которому получать справочник. Там должны быть поля ID и NAME (именно большими буквами)</param>
        /// <param name="editable"> Доступно ли поле для ввода. </param>
        /// <param name="page"> Страница page. </param>
        public static ColumnBase AddLookupEditorForColumn(
            this ColumnBase column, 
            string fieldId, 
            string fieldName, 
            string url, 
            bool editable, 
            Page page)
        {
            string storeId = "{0}Store{1}".FormatWith(column.ParentGrid.ID, column.ColumnID);
            page.Controls.Add(CreateStore(storeId, url));
            column.Editor.Add(
                CreateCombo(
                    column, storeId, fieldId, fieldName, !page.Request.Browser.Browser.ToUpper().Equals("IE"), editable));
            return column;
        }

        public static ColumnBase AddLookupEditorForColumnHardCode(
            this ColumnBase column, 
            string fieldId, 
            string fieldName, 
            string url, 
            bool editable, 
            Page page)
        {
            string storeIdAdd = "{0}AddStore{1}".FormatWith(column.ParentGrid.ID, column.ColumnID);
            var ds = new Store { ID = storeIdAdd, AutoLoad = false };
            ds.UpdateProxy.Add(
                new HttpWriteProxy
                    {
                        Url = "/EntityExt/Create?objectKey={0}"
                            .FormatWith(D_Services_CPotr.Key), 
                        Method = HttpMethod.POST
                    });
            ds.AddField("ID");
            ds.AddField("NAME");
            ds.Reader.Add(new JsonReader { IDProperty = "ID", Root = "data", MessageProperty = "MessageProperty" });
            page.Controls.Add(ds);

            string storeId = "{0}Store{1}".FormatWith(column.ParentGrid.ID, column.ColumnID);
            page.Controls.Add(CreateStore(storeId, url));
            column.Editor.Add(
                CreateComboHardCode(
                    column, 
                    storeId, 
                    storeIdAdd, 
                    fieldId, 
                    fieldName, 
                    !page.Request.Browser.Browser.ToUpper().Equals("IE"), 
                    editable));
            return column;
        }

        public static ColumnBase AddLookupEditorForColumnHardCode1(
            this ColumnBase column, 
            string displayField, 
            Dictionary<string, string[]> fieldsMapping, 
            ReferencerInfo fieldInfos, 
            string url, 
            Page page)
        {
            string storeId = "{0}Store{1}".FormatWith(column.ParentGrid.ID, column.ColumnID);

            // page.Controls.Add(
            // CreateStore(
            // storeId,
            // url,
            // fieldInfos.GetPrimaryFields().Concat(fieldInfos.GetSecondaryFields())
            // .Select(info => info.Name).ToList()));
            page.Controls.Add(
                CreateStore(
                    storeId, 
                    url, 
                    fieldsMapping.Values.SelectMany(strings => strings).ToList()));
            ComboBox combobox = CreateComboHardCode1(
                column, 
                storeId, 
                displayField, 
                fieldsMapping, 
                fieldInfos);
            combobox.AddClearTrigger(column.ParentGrid.ID, fieldsMapping);
            column.Editor.Add(combobox);
            ResourceManager.GetInstance(page).RegisterClientScriptBlock(
                "SearchComboboxControl", 
                Resource.SearchComboboxControl);
            ResourceManager.GetInstance(page).RegisterScript(
                "Combobox.HotKeys", 
                "/Krista.FM.RIA.Core/ExtNet.Extensions/ExcelLikeSelectionModel/js/HotKeysForCombobox.js/extention.axd");
            ResourceManager.GetInstance(page).RegisterStyle(
                "CustomSearch.Style", "/Krista.FM.RIA.Core/Extensions/css/CustomSearch.css/extention.axd");
            return column;
        }

        /// <summary>
        ///   Выпадающий список для поля с настройкой соответствия полей колонкам, подставляет заданные поля.
        ///   FieldsMapping:
        ///   ID = RefTemp
        ///   Name = RefTempName
        ///   при выборе значение поля ID присвоится колонке RefTemp грида интерфейса
        ///   соответственно Name присвоится колонке RefTempName
        /// </summary>
        /// <param name="column"> Колонка, к которой добавляется выпадающий список в качестве редактора. </param>
        /// <param name="displayField"> Поле из стора отображаемое в списке </param>
        /// <param name="fieldsMapping"> Список соответсвия полей из контроллера и интерфейса(поле из контроллера, поле из грида) </param>
        /// <param name="url"> Url, по которому получать справочник. </param>
        /// <param name="editable"> Доступно ли поле для ввода. </param>
        /// <param name="page"> Страница page. </param>
        public static ColumnBase AddLookupEditorForColumn(
            this ColumnBase column,
            string displayField,
            Dictionary<string, string> fieldsMapping,
            string url,
            bool editable,
            Page page)
        {
            string storeId = "{0}Store{1}".FormatWith(column.ParentGrid.ID, column.ColumnID);
            page.Controls.Add(CreateStore(storeId, url, fieldsMapping));
            column.Editor.Add(
                CreateCombo(
                    column,
                    storeId,
                    displayField,
                    fieldsMapping,
                    !page.Request.Browser.Browser.ToUpper().Equals("IE"),
                    editable));
            return column;
        }

        public static ReferencerInfo BuildReferencerInfo(
            IEntity entity, 
            IEnumerable<string> primary, 
            IEnumerable<string> secondary = null)
        {
            return new ReferencerInfo(
                entity.ObjectKey, 
                FieldInfos(entity, primary), 
                secondary.Return(enumerable => FieldInfos(entity, enumerable), new List<ReferencerInfo.FieldInfo>()));
        }

        private static void AddClearTrigger(
            this ComboBox combobox, 
            string gridId, 
            Dictionary<string, string[]> fieldsMapping)
        {
            combobox.Triggers.Add(
                new FieldTrigger
                    {
                        HideTrigger = true, 
                        Icon = TriggerIcon.SimpleCross, 
                        Qtip = "Очистить значение поля", 
                        Tag = "TriggerClear", 
                    });

            combobox.Listeners.Show.AddAfter(
                string.Format(
                    @"var srec = {0}getGridActiveRecord({1});
                      var clear_trigger = {0}getClearTrigger(this.triggers);

                      if (!Ext.isEmpty(srec) && !Ext.isEmpty(clear_trigger) && !{0}isEmptyLockupField(srec, {2})) {{
                          clear_trigger.show();
                      }} else {{
                          clear_trigger.hide();
                      }}", 
                    Scope, 
                    gridId, 
                    JSON.Serialize(fieldsMapping.Keys)));

            combobox.Listeners.Select.AddAfter(
                string.Format(
                    @"{0}getClearTrigger(this.triggers).show();", 
                    Scope));

            combobox.Listeners.TriggerClick.AddAfter(
                string.Format(
                    @"if(tag === 'TriggerClear') {{
                        var srec = {0}getGridActiveRecord({1});
                        var clear_trigger = {0}getClearTrigger(this.triggers).hide();
                        {0}clearLockupField(srec, {2});
                        this.reset()
                        //this.clear();
                    }};", 
                    Scope, 
                    gridId, 
                    JSON.Serialize(fieldsMapping.Keys)));
        }

        private static ComboBox CreateCombo(
            ColumnBase column, 
            string storeId, 
            string fieldId, 
            string fieldName, 
            bool useAutoWidth, 
            bool editable)
        {
            column.Editable = true;

            // TODO: этот блок нужно выносить
            string handler = @"
            var srec;
            var grid = {0};
            if (grid.getSelectionModel() instanceof Ext.grid.RowSelectionModel) {{
                srec = grid.getSelectionModel().getSelected();
            }}
            else {{
                srec = grid.getSelectionModel().selection.record;
            }}

            if (srec != null)
            {{
                srec.beginEdit();
                srec.set('{1}', record.get('ID'));
                srec.set('{2}', record.get('NAME'));
                srec.endEdit();
            }}".FormatWith(column.ParentGrid.ID, fieldId, fieldName);

            var combobox = new ComboBox
                {
                    ID = "combo" + storeId, 
                    MinChars = 3, 
                    TriggerAction = TriggerAction.All, 
                    StoreID = storeId,
                    ValueField = "NAME",
                    DisplayField = "NAME", 
                    ItemSelector = "tr.search-item", 
                    ForceSelection = true, 
                    TypeAhead = false, 
                    PageSize = 10, 
                    LoadingText = @"Поиск...", 
                    
                    // AutoWidth = true,
                    // MinWidth = column.Width,
                    MinListWidth = 270, 
                    EmptyText = @"Выберите значение", 
                    HideTrigger = false, 
                    Editable = editable, 
                    Resizable = true, 
                    Grow = true, 
                    TriggerIcon = TriggerIcon.SimpleArrowDown, 
                    Listeners =
                        {
                            Select =
                                {
                                    Handler = handler, 
                                }, 
                        }, 
                };

            if (!useAutoWidth)
            {
                column.Width = combobox.Width;
            }

            var tableTemplate = new StringBuilder(@"
<table width=100%>
        <tr>");
            tableTemplate.Append(@"
        </tr>
    <tpl for=""."">
        <tr class=""search-item"">
            ");
            tableTemplate.Append(@"<td class=""search-item secondary""> {NAME} </td>");
            tableTemplate.Append(@"
        </tr>
    </tpl>
</table>");

            combobox.Template.Html = tableTemplate.ToString();

            return combobox;
        }

        private static ComboBox CreateComboHardCode(
            ColumnBase column, 
            string storeId, 
            string storeIdAdd, 
            string fieldId, 
            string fieldName, 
            bool useAutoWidth, 
            bool editable)
        {
            column.Editable = true;

            // TODO: этот блок нужно выносить
            string handler = @"
            var srec;
            var grid = {0};
            if (grid.getSelectionModel() instanceof Ext.grid.RowSelectionModel) {{
                srec = grid.getSelectionModel().getSelected();
            }}
            else {{
                srec = grid.getSelectionModel().selection.record;
            }}

            if (srec != null)
            {{
                srec.beginEdit();
                srec.set('{1}', record.get('ID'));
                srec.set('{2}', record.get('NAME'));
                srec.endEdit();
            }}".FormatWith(column.ParentGrid.ID, fieldId, fieldName);

            var combobox = new ComboBox
                {
                    ID = "combo" + storeId, 
                    MinChars = 3, 
                    TriggerAction = TriggerAction.All, 
                    StoreID = storeId, 
                    ValueField = "NAME", 
                    DisplayField = "NAME", 
                    ItemSelector = "tr.search-item", 
                    ForceSelection = true, 
                    TypeAhead = false, 
                    PageSize = 10, 
                    LoadingText = @"Поиск...", 
                    AutoWidth = true, 
                    MinWidth = column.Width, 
                    MinListWidth = 270, 
                    EmptyText = @"Выберите значение", 
                    HideTrigger = false, 
                    Editable = editable, 
                    Resizable = true, 
                    GrowMin = column.Width, 
                    Grow = true, 
                    TriggerIcon = TriggerIcon.SimpleArrowDown, 
                    Listeners =
                        {
                            Select =
                                {
                                    Handler = handler, 
                                }, 
                        }, 
                };

            ////Enum.GetValues(typeof (TriggerIcon)).Each<TriggerIcon>(
            ////    icon => combobox.Triggers.Add(
            ////        new FieldTrigger
            ////            {
            ////                Icon = icon,
            ////                Qtip = icon.ToString(),
            ////            }));
            combobox.Triggers.Add(
                new FieldTrigger
                    {
                        HideTrigger = true, 
                        Icon = TriggerIcon.SimpleLightning, 
                        Qtip = "Значение не найдено. Добавить новое?", 
                        Tag = "TriggerAdd", 
                    });

            combobox.Triggers.Add(
                new FieldTrigger
                    {
                        HideTrigger = true, 
                        Icon = TriggerIcon.SimpleCross, 
                        Qtip = "Очистить значение поля", 
                        Tag = "TriggerClear", 
                    });

            combobox.Listeners.Show.Handler = @"
            //TODO: этот блок нужно выносить
            var srec;
            var grid = {0};
            if (grid.getSelectionModel() instanceof Ext.grid.RowSelectionModel) {{
                srec = grid.getSelectionModel().getSelected();
            }}
            else {{
                srec = grid.getSelectionModel().selection.record;
            }}
            //TODO: тоже выносится
            //var clear_trigger = this.getTrigger(Ext.each(this.triggersConfig.findIndex('tag', 'TriggerClear'));
            var clear_trigger = this.getTrigger(1);

            if (!Ext.isEmpty(srec) && !Ext.isEmpty(clear_trigger) && !Ext.isEmpty(srec.data.{1})) {{
                clear_trigger.show();
            }} else {{
                clear_trigger.hide();
            }}".FormatWith(column.ParentGrid.ID, fieldId, fieldName);

            combobox.Listeners.Select.AddAfter(@"
            //TODO: тоже выносится
            //var clear_trigger = this.getTrigger(this.triggersConfig.findIndex('tag', 'TriggerClear'));
            var clear_trigger = this.getTrigger(1);
            clear_trigger.show();");

            combobox.Listeners.BeforeQuery.Handler = @"
this.store.on('load', function(s, r) {{
   //var add_trigger = this.getTrigger(this.triggersConfig.findIndex('tag', 'TriggerAdd'));
   var add_trigger = this.getTrigger(0);
   add_trigger[s.getCount() > 0 ? 'hide' : 'show']();
   }}, this, {single: true})";
            combobox.Listeners.TriggerClick.Handler = @"
            switch (tag) {{
            case 'TriggerAdd':
                TaskLocation = Ext.data.Record.create([{{name: 'ID', type: 'string'}}, {{name: 'NAME', type: 'string'}}, {{name: 'CODE', type: 'string'}}, {{name: 'ROWTYPE', type: 'int'}}]);
                var record_add = new TaskLocation({{ID: 0, NAME: this.lastQuery, CODE: '', ROWTYPE: 0}});
                record_add.newRecord = true;
                //    record_add.phantom = true;

            {3}.on('save', function(s, b, d) {{
                var lastQuery = this.lastQuery;
                this.lastQuery = null;
                this.doQuery(lastQuery);
            }}, this, {{single: true}});

                {3}.add(record_add);
                {3}.save();

                console.log('combobox.Listeners.TriggerClick.Handler');
break;

            case 'TriggerClear':
                {{
//TODO: этот блок нужно выносить
            var srec;
            var grid = {0};
            if (grid.getSelectionModel() instanceof Ext.grid.RowSelectionModel) {{
                srec = grid.getSelectionModel().getSelected();
            }}
            else {{
                srec = grid.getSelectionModel().selection.record;
            }}

            if (srec != null)
            {{
                srec.beginEdit();
                srec.set('{1}', null);
                srec.set('{2}', null);
                srec.endEdit();
            }}
this.clear();
}};
            default:
                break;
            }}".FormatWith(
                column.ParentGrid.ID, fieldId, fieldName, storeIdAdd);

            // combobox.Listeners.TriggerClick.EventArgument
            if (!useAutoWidth)
            {
                column.Width = combobox.Width;
            }

            var tableTemplate = new StringBuilder(@"
<table width=100%>
        <tr>");
            tableTemplate.Append(@"
        </tr>
    <tpl for=""."">
        <tr class=""search-item"">
            ");
            tableTemplate.Append(@"<td class=""search-item secondary""> {NAME} </td>");
            tableTemplate.Append(@"
        </tr>
    </tpl>
</table>");

            combobox.Template.Html = tableTemplate.ToString();

            return combobox;
        }

        private static ComboBox CreateComboHardCode1(
            ColumnBase column, 
            string storeId, 
            string displayField, 
            Dictionary<string, string[]> fieldsMapping, 
            ReferencerInfo fieldInfos)
        {
            column.Editable = true;

            var combobox =
                new ComboBox
                    {
                        ID = "combo" + storeId, 
                        MinChars = 1, 
                        TriggerAction = TriggerAction.All, 
                        StoreID = storeId, 
                        ValueField = displayField, 
                        DisplayField = displayField, 
                        ItemSelector = "tr.search-item", 
                        ForceSelection = true, 
                        EnableKeyEvents = true, 
                        TypeAhead = false, 
                        PageSize = 10, 
                        LoadingText = @"Поиск...", 
                        MinWidth = column.Width, 
                        MinListWidth = 350, 
                        EmptyText = @"Выберите значение", 
                        HideTrigger = false, 
                        Resizable = true, 
                        
                        // Grow = true,
                        TriggerIcon = TriggerIcon.SimpleArrowDown, 
                        Listeners =
                            {
                                Select =
                                    {
                                        Handler =
                                            string.Format(
                                                @"{2}onSelect(record, {0}, {1}, '{3}');", 
                                                column.ParentGrid.ID, 
                                                JSON.Serialize(fieldsMapping), 
                                                Scope, 
                                                displayField), 
                                    }, 
                            }, 
                    };            

            // combobox.Triggers.Add(
            // new FieldTrigger
            // {
            // HideTrigger = true,
            // Icon = TriggerIcon.SimpleLightning,
            // Qtip = "Значение не найдено. Добавить новое?",
            // Tag = "TriggerAdd",
            // });

            // combobox.Listeners.BeforeQuery.Handler = @"
            // this.store.on('load', function(s, r) {{
            // //var add_trigger = this.getTrigger(this.triggersConfig.findIndex('tag', 'TriggerAdd'));
            // var add_trigger = this.getTrigger(0);
            // add_trigger[s.getCount() > 0 ? 'hide' : 'show']();
            // }}, this, {single: true})";
            // combobox.Listeners.TriggerClick.Handler = @"
            // switch (tag) {{
            // case 'TriggerAdd':
            // TaskLocation = Ext.data.Record.create([{{name: 'ID', type: 'string'}}, {{name: 'NAME', type: 'string'}}, {{name: 'CODE', type: 'string'}}, {{name: 'ROWTYPE', type: 'int'}}]);
            // var record_add = new TaskLocation({{ID: 0, NAME: this.lastQuery, CODE: '', ROWTYPE: 0}});
            // record_add.newRecord = true;
            // //    record_add.phantom = true;
            // {3}.on('save', function(s, b, d) {{
            // var lastQuery = this.lastQuery;
            // this.lastQuery = null;
            // this.doQuery(lastQuery);
            // }}, this, {{single: true}});
            // {3}.add(record_add);
            // {3}.save();
            // console.log('combobox.Listeners.TriggerClick.Handler');
            // break;
            // case 'TriggerClear':
            // {{
            //// TODO: этот блок нужно выносить
            // var srec;
            // var grid = {0};
            // if (grid.getSelectionModel() instanceof Ext.grid.RowSelectionModel) {{
            // srec = grid.getSelectionModel().getSelected();
            // }}
            // else {{
            // srec = grid.getSelectionModel().selection.record;
            // }}
            // if (srec != null)
            // {{
            // srec.beginEdit();
            // srec.set('{1}', null);
            // srec.set('{2}', null);
            // srec.endEdit();
            // }}
            // this.clear();
            // }};
            // default:
            // break;
            // }}".FormatWith(column.ParentGrid.ID, fieldId, fieldName, storeIdAdd);
            // combobox.Listeners.TriggerClick.EventArgument
            // if (!useAutoWidth)
            // {
            // column.Width = combobox.Width;
            //// }
            
            combobox.Template.Html = new SearchResultTableTpl { FieldInfos = fieldInfos }.TransformText();

            return combobox;
        }

        private static Store CreateStore(string id, string url, IEnumerable<string> fields)
        {
            return new Store
                {
                    ID = id, 
                    AutoLoad = false, 
                    BaseParams = { new Parameter("fields", JSON.Serialize(fields), ParameterMode.Raw) }, 
                }

                .SetHttpProxy(url)
                .SetJsonReader()
                .Do(store => fields.Each(field => store.AddField(field)));
        }

        private static Store CreateStore(string id, string url)
        {
            var ds = new Store { ID = id, AutoLoad = false };
            ds.SetHttpProxy(url).SetJsonReader();
            ds.BaseParams.Add(new Parameter("fields", "['ID','NAME']", ParameterMode.Raw));
            ds.AddField("ID");
            ds.AddField("NAME");

            return ds;
        }

        private static List<ReferencerInfo.FieldInfo> FieldInfos(IEntity entity, IEnumerable<string> fields)
        {
            return fields
                .Where(field => entity.Attributes.Values.Any(attribute => attribute.Name.Equals(field)))
                .Select(
                    field => new ReferencerInfo.FieldInfo
                        {
                            Name = field, 
                            Caption = entity.Attributes.Values
                                .First(attribute => attribute.Name.Equals(field))
                                .Caption
                        }).ToList();
        }

        /// <summary>
        /// Комбо с произвольными полями
        /// </summary>
        /// <param name="column">
        /// The column.
        /// </param>
        /// <param name="storeId">
        /// The store Id.
        /// </param>
        /// <param name="displayField">
        /// Отображаемое в списке поле 
        /// </param>
        /// <param name="fieldsMapping">
        /// Настройки соответствия полей колонкам 
        /// </param>
        /// <param name="useAutoWidth">
        /// The use Auto Width.
        /// </param>
        /// <param name="editable">
        /// The editable.
        /// </param>
        private static ComboBox CreateCombo(
            ColumnBase column, 
            string storeId, 
            string displayField, 
            Dictionary<string, string> fieldsMapping, 
            bool useAutoWidth, 
            bool editable)
        {
            column.Editable = true;

            string getFieldsValues = string.Empty;
            foreach (var field in fieldsMapping)
            {
                getFieldsValues += "srec.set('{0}', record.get('{1}'));".FormatWith(field.Value, field.Key);
            }

            // TODO: этот блок нужно выносить
            string handler = @"
                var srec;
                var grid = {0};
                if (grid.getSelectionModel() instanceof Ext.grid.RowSelectionModel) {{
                    srec = grid.getSelectionModel().getSelected();
                }}
                else {{
                    srec = grid.getSelectionModel().selection.record;
                }}

                if (srec != null)
                {{
                    srec.beginEdit();
                    {1}
                    srec.endEdit();
                }}".FormatWith(column.ParentGrid.ID, getFieldsValues);

            var combobox = new ComboBox
                {
                    ID = "combo" + storeId, 
                    MinChars = 3, 
                    TriggerAction = TriggerAction.All, 
                    StoreID = storeId, 
                    ValueField = displayField, 
                    DisplayField = displayField, 
                    ItemSelector = "tr.search-item", 
                    ForceSelection = true, 
                    TypeAhead = false, 
                    PageSize = 10, 
                    LoadingText = @"Поиск...", 
                    
                    // AutoWidth = true,
                    // MinWidth = column.Width,
                    MinListWidth = 270, 
                    EmptyText = @"Выберите значение", 
                    HideTrigger = false, 
                    Editable = editable, 
                    Resizable = true, 
                    Grow = true, 
                    TriggerIcon = TriggerIcon.SimpleArrowDown, 
                    Listeners =
                        {
                            Select =
                                {
                                    Handler = handler, 
                                }, 
                        }, 
                };

            if (!useAutoWidth)
            {
                column.Width = combobox.Width;
            }

            var tableTemplate = new StringBuilder(
                @"
                                                         <table width=100%>
                                                         <tr>");
            tableTemplate.Append(
                @"
                                       </tr>
                                       <tpl for=""."">
                                            <tr class=""search-item"">");
            tableTemplate.Append(
                @"<td class=""search-item secondary""> {{{0}}} </td>"
                    .FormatWith(displayField));
            tableTemplate.Append(@"
                                                </tr>
                                            </tpl>
                                        </table>");

            combobox.Template.Html = tableTemplate.ToString();

            return combobox;
        }

        /// <summary>
        ///   Стор для комбо с произвольными полями
        /// </summary>
        private static Store CreateStore(string id, string url, Dictionary<string, string> fieldsMapping)
        {
            var ds = new Store { ID = id, AutoLoad = false };
            ds.SetHttpProxy(url).SetJsonReader();

            foreach (var field in fieldsMapping)
            {
                ds.AddField(field.Key);
            }

            ds.BaseParams.Add(new Parameter("fields", JSON.Serialize(fieldsMapping.Values), ParameterMode.Raw));

            return ds;
        }
    }
}
