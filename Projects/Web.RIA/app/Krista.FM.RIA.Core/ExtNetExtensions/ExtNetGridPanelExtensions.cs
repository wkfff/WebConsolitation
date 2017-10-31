using System;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Ext.Net;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core.ExtNet.Extensions.PeriodField;
using Krista.FM.ServerLibrary;

namespace Krista.FM.RIA.Core
{
    public static class ExtNetGridPanelExtensions
    {
        public static ColumnBase AddColumn(this ColumnModel cm, IDataAttribute attribute)
        {
            return cm.AddColumn(attribute.Name, attribute.Caption, attribute.Type, attribute.IsNullable ? Mandatory.Nullable : Mandatory.NotNull);
        }

        public static ColumnBase AddColumn(this ColumnModel cm, string columnId, string header, DataAttributeTypes attributeType)
        {
            return cm.AddColumn(columnId, columnId, header, attributeType, Mandatory.NotNull);
        }

        public static ColumnBase AddColumn(this ColumnModel cm, string columnId, string header, DataAttributeTypes attributeType, Mandatory mandatory)
        {
            ColumnBase column = cm.AddColumn(columnId, columnId.ToUpper(), header, attributeType, mandatory);
            if (mandatory == Mandatory.Nullable)
            {
                column.SetNullable();
            }

            return column;
        }

        public static ColumnBase AddColumn(this ColumnModel cm, string columnId, string dataIndex, string header, DataAttributeTypes attributeType, Mandatory mandatory)
        {
            ColumnBase column = ColumnFactory(attributeType);
            column.ColumnID = columnId;
            column.DataIndex = dataIndex;
            column.Header = header;
            column.Wrap = true;

            return cm.AddColumn(column);
        }

        public static ColumnBase AddColumn(this ColumnModel cm, ColumnBase column)
        {
            cm.Columns.Add(column);

            return column;
        }

        public static ColumnBase IsDate(this ColumnBase column)
        {
            column.Renderer = new Renderer(@"
if(value){
    if(typeof(value) == 'string'){
        if (/\d{2}.\d{2}.\d{4}/.test(value)){
            return Date.parseDate(value, 'd.m.Y').format('d.m.Y');
        };
        if (/\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}.\d{3}/.test(value)){
            return Date.parseDate(value, 'Y-m-dTH:i:s.u').format('d.m.Y');
        };
        //перестраховался        
        if (/\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}/.test(value)){
            return Date.parseDate(value, 'Y-m-dTH:i:s').format('d.m.Y');
        };        
    }
    return value.format('d.m.Y');
}
return value;");

            return column;
        }

        public static ColumnBase IsYesNo(this ColumnBase column)
        {
            column.Renderer = new Renderer("if(value){ return 'Да'; } return 'Нет';");

            return column;
        }

        public static ColumnBase SetNullable(this ColumnBase column)
        {
            column.CustomConfig.Add(new ConfigItem("AllowBlank", "true"));
            column.Css = "font-style: italic;";
            column.Header = String.Format("<i>{0}</i>", column.Header);

            return column;
        }

        public static ColumnBase AllowBlank(this ColumnBase column)
        {
            column.CustomConfig.Add(new ConfigItem("AllowBlank", "true"));
            return column;
        }

        public static ColumnBase GetColumnById(this ColumnModel cm, string id)
        {
            return cm.Columns.First(x => x.ColumnID == id);
        }

        public static ColumnBase SetEditable(this ColumnBase column, bool editable)
        {
            column.Editable = editable;

            return column;
        }

        public static ColumnBase SetEditable(this ColumnBase column, IDataAttribute attribute)
        {
            if (attribute.Class == DataAttributeClassTypes.Typed)
            {
                switch (attribute.Type)
                {
                    case DataAttributeTypes.dtDouble:
                        return column.SetEditableDouble(attribute.Scale);
                    case DataAttributeTypes.dtInteger:
                        return column.SetEditableInteger();
                    case DataAttributeTypes.dtString:
                        return column.SetEditableString();
                    case DataAttributeTypes.dtDate:
                    case DataAttributeTypes.dtDateTime:
                        return column.SetEditableDate();
                    case DataAttributeTypes.dtBoolean:
                        return column.SetEditableBoolean();
                }
            }

            return column;
        }

        public static ColumnBase SetEditableBoolean(this ColumnBase column)
        {
            if (column != null)
            {
                column.Editable = true;
            }

            return column;
        }

        public static ColumnBase SetEditableDouble(this ColumnBase column, int scale)
        {
            if (column != null)
            {
                column.Editable = true;
                column.Align = Alignment.Right;

                column.Editor.Add(new NumberField
                {
                    AllowBlank = column.CustomConfig.Contains("AllowBlank"),
                    AllowDecimals = true,
                    DecimalPrecision = scale
                });

                // Увеличиваем количество знаков десятичной части
                if (column is NumberColumn && scale > 2)
                {
                    var numberColumn = (NumberColumn)column;
                    numberColumn.Format += String.Empty.PadRight(scale - 2, '0');
                }
            }

            return column;
        }

        public static ColumnBase SetEditableInteger(this ColumnBase column)
        {
            if (column != null)
            {
                column.Editable = true;

                column.Editor.Add(new NumberField
                {
                    AllowBlank = column.CustomConfig.Contains("AllowBlank"),
                    AllowDecimals = false
                });
            }

            return column;
        }
        
        public static ColumnBase SetEditableString(this ColumnBase column)
        {
            if (column != null)
            {
                column.Editable = true;

                column.Editor.Add(new TextArea
                {
                    AllowBlank = column.CustomConfig.Contains("AllowBlank"),
                });
            }

            return column;
        }

        public static ColumnBase SetEditableDate(this ColumnBase column)
        {
            if (column != null)
            {
                column.Editable = true;

                column.Editor.Add(new DateField
                {
                    AllowBlank = column.CustomConfig.Contains("AllowBlank"),
                    Format = "d.m.Y"
                });
            }

            return column;
        }

        public static ColumnBase SetEditablePeriod(this ColumnBase column)
        {
            return SetEditablePeriod(column, new PeriodFieldConfig());
        }

        public static ColumnBase SetEditablePeriod(this ColumnBase column, PeriodFieldConfig config)
        {
            if (column != null)
            {
                column.Editable = true;

                config.AfterSelectHandler += " {0}.stopEditing(false);".FormatWith(column.ParentGrid.ID);

                var periodField = new PeriodField(column.DataIndex, config);

                column.RendererFn(periodField.GetRenderer());

                column.Editor.Add(periodField);
            }

            return column;
        }

        public static ColumnBase SetGroup(this ColumnBase column, string groupName)
        {
            if (column != null)
            {
                column.GroupName = groupName;
            }

            return column;
        }

        public static ColumnBase SetGroupable(this ColumnBase column)
        {
            if (column != null)
            {
                column.Groupable = true;
            }

            return column;
        }

        public static ColumnBase SetGroupable(this ColumnBase column, bool groupable)
        {
            if (column != null)
            {
                column.Groupable = groupable;
            }

            return column;
        }

        public static ColumnBase SetVisible(this ColumnBase column, bool visible)
        {
            if (column != null)
            {
                column.Hidden = !visible;
            }

            return column;
        }

        public static ColumnBase SetWidth(this ColumnBase column, int width)
        {
            if (column != null)
            {
                column.Width = width;
            }
            
            return column;
        }

        public static ColumnBase SetWidthFixed(this ColumnBase column, int width)
        {
            if (column != null)
            {
                column.Width = width;
                column.Fixed = true;
            }

            return column;
        }

        public static ColumnBase SetWrap(this ColumnBase column)
        {
            if (column != null)
            {
                column.Wrap = true;
            }

            return column;
        }

        public static ColumnBase SetWrap(this ColumnBase column, bool wrap)
        {
            if (column != null)
            {
                column.Wrap = wrap;
            }

            return column;
        }

        public static ColumnBase SetHidden(this ColumnBase column, bool value)
        {
            if (column != null)
            {
                column.Hidden = value;
            }

            return column;
        }

        public static ColumnBase SetLocked(this ColumnBase column)
        {
            if (column != null)
            {
                column.Locked = true;
            }

            return column;
        }

        public static ColumnBase SetLookup(this ColumnBase column, string valueIdField, string lookupStoreId, string lookupValueField, string lookupDisplayField)
        {
            return column.SetLookup(valueIdField, lookupStoreId, lookupValueField, lookupDisplayField, String.Empty);
        }

        public static ColumnBase SetLookup(this ColumnBase column, string valueIdField, string lookupStoreId, string lookupValueField, string lookupDisplayField, string handler)
        {
            if (column != null)
            {
                column.Editable = true;
                string handlerScript = @"
                    var srec = {0}.getSelectionModel().getSelected(); 
                    if (srec != null) {{ 
                        srec.set('{1}', record.get('{2}')); 
                        var f = function(srec, record){{{3}}};
                        f(srec, record);
                    }}"
                    .FormatWith(column.ParentGrid.ID, valueIdField, lookupValueField, handler);

                column.Editor.Add(new ComboBox
                {
                    TriggerAction = TriggerAction.All,
                    StoreID = lookupStoreId,
                    ValueField = lookupDisplayField,
                    EnableShadow = true,
                    LoadingText = "Загрузка...",
                    Resizable = true,
                    DisplayField = lookupDisplayField,
                    Listeners = { Select = { Handler = handlerScript } },
                    MinListWidth = 200,
                });
            }

            return column;
        }

        public static ColumnBase ForceRemote(this ColumnBase column)
        {
            if (column.Editor.First() is ComboBox)
            {
                var comboBox = (ComboBox)column.Editor.First();
                comboBox.Mode = DataLoadMode.Remote;
                comboBox.ID = column.ColumnID + "EditorComboBox";
                comboBox.Listeners.BeforeQuery.Handler = "delete {0}.lastQuery; {1}; return true;".FormatWith(comboBox.ID, comboBox.Listeners.BeforeQuery.Handler);
            }

            return column;
        }
        
        public static ColumnBase WithAutosearch(this ColumnBase column)
        {
            if (column.Editor.First() is ComboBox)
            {
                var comboBox = (ComboBox)column.Editor.First();
                comboBox.Mode = DataLoadMode.Remote;
                comboBox.MinChars = 1;
                comboBox.LoadingText = "Поиск...";
            }

            return column;
        }

        public static ColumnBase RendererFn(this ColumnBase column, string function)
        {
            if (column != null)
            {
                column.Renderer.Fn = function;
            }

            return column;
        }

        public static ColumnBase AddCss(this ColumnBase column, string css)
        {
            if (column != null)
            {
                column.Css += css;
            }

            return column;
        }

        /// <summary>
        /// Добавляет на страницу page стили для столбцов и заголовков разрешающие 
        /// перенос по словам.
        /// </summary>
        /// <param name="gp">Компонент GridPanel.</param>
        /// <param name="page">Страница в которую будут добавлены стили.</param>
        public static void AddColumnsWrapStylesToPage(this GridPanel gp, ViewPage page)
        {
            if (X.IsAjaxRequest)
            {
                // Нам не нужно выполнять DataBind для событий DirectEvent
                return;
            }

            StringBuilder style = new StringBuilder(".x-grid3-hd-inner{white-space:normal;}");
            foreach (ColumnBase column in gp.ColumnModel.Columns)
            {
                if (column.Wrap)
                {
                    style.Append(".x-grid3-col-")
                        .Append(column.ColumnID)
                        .AppendLine("{white-space: normal;}");
                }
            }

            Ext.Net.ResourceManager.GetInstance(page)
                .RegisterClientStyleBlock("RefColumnStyles{0}".FormatWith(gp.ID), style.ToString());
        }

        /// <summary>
        /// Добавляет на страницу page стили для заголовков для выравнивания их по центру
        /// </summary>
        /// <param name="gp">Компонент GridPanel.</param>
        /// <param name="page">Страница в которую будут добавлены стили.</param>
        public static void AddColumnsHeaderAlignStylesToPage(this GridPanel gp, ViewPage page)
        {
            if (X.IsAjaxRequest)
            {
                // Нам не нужно выполнять DataBind для событий DirectEvent
                return;
            }

            var style = new StringBuilder(".x-grid3-hd-inner{text-align:center;}");

            Ext.Net.ResourceManager.GetInstance(page)
                .RegisterClientStyleBlock("RefColHeaderAlignCenterStyles{0}".FormatWith(gp.ID), style.ToString());
        }

        public static GridPanel SetReadonly(this GridPanel gp)
        {
            return SetReadonly(gp, true);
        }

        public static GridPanel SetReadonly(this GridPanel gp, bool isReadonly)
        {
            if (isReadonly)
            {
                foreach (var column in gp.ColumnModel.Columns)
                {
                    column.Editable = false;
                }
            }

            return gp;
        }

        public static GridPanel SetReadonlyNoCommands(this GridPanel gp)
        {
            SetReadonly(gp);

            foreach (var column in gp.ColumnModel.Columns
                        .Where(column => column.GetType() == typeof(Column)))
            {
                ((Column)column).Commands.Clear();
            }

            return gp;
        }
        
        public static ToolbarBase Toolbar(this GridPanel gp)
        {
            if (gp.TopBar.Count == 1 && (gp.TopBar[0] is Toolbar || gp.TopBar[0] is PagingToolbar))
            {
                return gp.TopBar[0];
            }

            Toolbar toolbar = new Toolbar { ID = "{0}Toolbar".FormatWith(gp.ID), };
            gp.TopBar.Add(toolbar);
            return toolbar;
        }

        public static Button AddNewRecordButton(this GridPanel gp)
        {
            return gp.Toolbar()
                .AddIconButton(
                    "{0}NewRecordBtn".FormatWith(gp.ID), 
                    Icon.Add, 
                    "Добавить новую запись",
                    "{0}.insertRecord();{0}.getSelectionModel().selectFirstRow();{0}.getRowEditor().startEditing(0);".FormatWith(gp.ID));
        }

        public static Button AddNewRecordNoEditButton(this GridPanel gp)
        {
            return gp.Toolbar()
                .AddIconButton(
                    "{0}NewRecordBtn".FormatWith(gp.ID),
                    Icon.Add,
                    "Добавить новую запись",
                    "{0}.insertRecord();{0}.getSelectionModel().selectFirstRow();".FormatWith(gp.ID));
        }

        public static Button AddRemoveRecordButton(this GridPanel gp)
        {
            return gp.Toolbar()
                .AddIconButton(
                    "{0}RemoveRecordBtn".FormatWith(gp.ID), 
                    Icon.Delete, 
                    "Удалить запись", 
                    "{0}.deleteSelected();".FormatWith(gp.ID));
        }

        public static Button AddRemoveRecordWithConfirmButton(this GridPanel gp)
        {
            var handler = @"
Ext.Msg.show({{
    title: 'Подтверждение',
    msg: 'Удалить запись?',
    width: 300,
    buttons: Ext.Msg.YESNO,
    fn: function (btn) {{
        if (btn == 'yes') {{
            {0}.deleteSelected();
            if (!{0}.hasSelection())
                {0}RemoveRecordBtn.disable();
        }}
    }},
    modal: true,
    icon: Ext.Msg.QUESTION,
    animEl: {0}RemoveRecordBtn,
    closable: false,
}});".FormatWith(gp.ID);
            return gp.Toolbar()
                .AddIconButton(
                    "{0}RemoveRecordBtn".FormatWith(gp.ID),
                    Icon.Delete,
                    "Удалить запись",
                    handler);
        }

        public static Button AddRefreshButton(this GridPanel gp)
        {
            return gp.Toolbar()
                .AddIconButton(
                    "{0}RefreshBtn".FormatWith(gp.ID), 
                    Icon.PageRefresh, 
                    "Обновить", 
                    "{0}.reload();".FormatWith(gp.StoreID));
        }

        public static Button AddRefreshAndDeselectButton(this GridPanel gp)
        {
            return gp.Toolbar()
                .AddIconButton(
                    "{0}RefreshBtn".FormatWith(gp.ID),
                    Icon.PageRefresh,
                    "Обновить",
                    @"{0}.getSelectionModel().clearSelections(); {1}.reload();".FormatWith(gp.ID, gp.StoreID));
        }

        public static Button AddSaveButton(this GridPanel gp)
        {
            return gp.Toolbar()
                .AddIconButton(
                    "{0}SaveBtn".FormatWith(gp.ID), 
                    Icon.Disk, 
                    "Сохранить изменения", 
                    "{0}.save();".FormatWith(gp.StoreID));
        }

        private static ColumnBase ColumnFactory(DataAttributeTypes attributeType)
        {
            switch (attributeType)
            {
                case DataAttributeTypes.dtBoolean:
                    return new CheckColumn();
                case DataAttributeTypes.dtDate:
                case DataAttributeTypes.dtDateTime:
                    return new Column().IsDate();
                case DataAttributeTypes.dtDouble:
                    return new NumberColumn();
                default:
                    return new Column();
            }
        }
    }
}
