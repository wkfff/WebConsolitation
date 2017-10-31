using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using Ext.Net;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core.Helpers;

namespace Krista.FM.RIA.Core.Extensions
{
    public static class ColumnBaseExtensions
    {
        public static void AddLookupForColumn(
            this ColumnBase column,
            List<ReferencerInfo.FieldInfo> primaryFields,
            List<ReferencerInfo.FieldInfo> secondaryFields, 
            string url, 
            Page page)
        {
            var storeId = "{0}Store{1}".FormatWith(column.ParentGrid.ID, column.ColumnID);
            page.Controls.Add(CreateStore(storeId, primaryFields, secondaryFields, url));
            column.Editor.Add(CreateCombo(column, storeId, "ID", primaryFields, secondaryFields, !page.Request.Browser.Browser.ToUpper().Equals("IE")));
        }

        /// <summary>
        /// Выпадающий список для поля, подставляет значения Name и ID справочника.
        /// </summary>
        /// <param name="column">Колонка, к которой добавляется выпадающий список в качестве редактора.</param>
        /// <param name="fieldId">Идентификатор колонки, соответстующей полю ID.</param>
        /// <param name="fieldName">Идентификатор колонки, соответствующей полю Name.</param>
        /// <param name="url">Url, по которому получать справочник.</param>
        /// <param name="editable">Доступно ли поле для ввода.</param>
        /// <param name="page">Страница page.</param>
        public static void AddLookupIdNameForColumn(
            this ColumnBase column,
            string fieldId,
            string fieldName,
            string url,
            bool editable,
            Page page)
        {
            var storeId = "{0}Store{1}".FormatWith(column.ParentGrid.ID, column.ColumnID);
            page.Controls.Add(CreateStore(storeId, url));
            column.Editor.Add(CreateCombo(column, storeId, fieldId, fieldName, !page.Request.Browser.Browser.ToUpper().Equals("IE"), editable));
        }

        private static string GetScriptFor(string fieldName, string prevDisplayField, out string displayField)
        {
            string df = fieldName.ToUpper();
            displayField = string.Empty;

            if (prevDisplayField.IsNullOrEmpty())
            {
                displayField = fieldName.ToUpper();
            }

            return "record.get('{0}')".FormatWith(df);
        }

        private static ComboBox CreateCombo(
            ColumnBase column, 
            string storeId,
            string lookupValueField,
            IEnumerable<ReferencerInfo.FieldInfo> primaryFields,
            IEnumerable<ReferencerInfo.FieldInfo> secondaryFields,
            bool useAutoWidth)
        {
            column.Editable = true;
            var setLookups = string.Empty;
            var displayField = string.Empty;
            var plus = string.Empty;

            foreach (var field in primaryFields)
            {
                    setLookups += plus + GetScriptFor(field.Name, displayField, out displayField);
                    plus = " + ';' + ";
            }

            if (setLookups.IsNullOrEmpty())
            {
                setLookups = "record.get('ID')";
            }

            foreach (var field in secondaryFields)
            {
                if (displayField.IsNullOrEmpty())
                {
                    displayField = field.Name.ToUpper();
                }
            }

            var handler = @"
            var srec;
            if (#{{{0}}}.getSelectionModel() instanceof Ext.grid.RowSelectionModel) {{
                srec = #{{{0}}}.getSelectionModel().getSelected();
            }}   
            else {{
                srec = #{{{0}}}.getSelectionModel().selection.record;
            }}

            if (srec != null) 
            {{
                srec.endEdit();
                srec.beginEdit();
                srec.set('{1}', record.get('{2}')); 
                srec.set('LP_{1}', {3});
                srec.endEdit();
                {0}.activeEditor.setValue({3});
            }}"
                .FormatWith(column.ParentGrid.ID, column.ColumnID.ToUpper(), lookupValueField, setLookups);

            var combobox = new ComboBox
            {
                ID = "combo" + storeId,
                MinChars = 1,
                TriggerAction = TriggerAction.All,
                StoreID = storeId,
                ValueField = displayField,
                DisplayField = displayField,
                ItemSelector = "tr.search-item",
                TypeAhead = false,
                LoadingText = "Поиск...",
                PageSize = 10,
                AutoWidth = useAutoWidth,
                Width = 270,
                EmptyText = "Выберите значение",
                HideTrigger = true,
                Listeners =
                {
                    Select =
                    {
                        Handler = handler
                    }
                }
            };

            if (!useAutoWidth)
            {
                column.Width = combobox.Width;
            }

            var tableTemplate = new StringBuilder(@"
<table width=100%>
        <tr>"); 

            foreach (var field in primaryFields)
            {
                tableTemplate.Append(@"
            <th class=""search-header"">
                    {0}
            </th>".FormatWith(field.Caption));
            }

            foreach (var field in secondaryFields)
            {
                tableTemplate.Append(@"
            <th class=""search-header"">
                    {0}
            </th>".FormatWith(field.Caption));
            }

            tableTemplate.Append(@"
        </tr>
    <tpl for=""."">
        <tr class=""search-item"">
            ");
            foreach (var field in primaryFields)
            {
                tableTemplate.Append(@"<td class=""search-item primary""> {{{0}}} </td>".FormatWith(field.Name.ToUpper()));
            }

            foreach (var field in secondaryFields)
            {
                tableTemplate.Append(@"<td class=""search-item secondary""> {{{0}}} </td>".FormatWith(field.Name.ToUpper()));
            }

            tableTemplate.Append(@"
        </tr>
    </tpl>
</table>");

            combobox.Template.Html = tableTemplate.ToString();

            return combobox;
        }

        private static Store CreateStore(string id, IEnumerable<ReferencerInfo.FieldInfo> primaryFields, IEnumerable<ReferencerInfo.FieldInfo> secondaryFields, string url)
        {
            var ds = new Store { ID = id, AutoLoad = false };
            ds.SetHttpProxy(url).SetJsonReader();

            var fieldsList = primaryFields
                                   .Aggregate(
                                       string.Empty,
                                       (current, primaryField) => current + "'{0}',".FormatWith(primaryField.Name));
            fieldsList += secondaryFields
                .Aggregate(
                    string.Empty,
                    (current, secondaryField) => current + "'{0}',".FormatWith(secondaryField.Name));
            fieldsList = fieldsList.Substring(0, fieldsList.Length - 1);

            ds.BaseParams.Add(new Parameter("fields", "[{0}]".FormatWith(fieldsList), ParameterMode.Raw));

            foreach (var fieldName in primaryFields)
            {
                ds.AddField(fieldName.Name.ToUpper());
            }

            foreach (var fieldName in secondaryFields)
            {
                ds.AddField(fieldName.Name.ToUpper());
            }

            ds.AddField("ID");

            return ds;
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

        private static ComboBox CreateCombo(
            ColumnBase column,
            string storeId,
            string fieldId,
            string fieldName,
            bool useAutoWidth,
            bool editable)
        {
            column.Editable = true;

            var handler = @"
            var srec;
            if (#{{{0}}}.getSelectionModel() instanceof Ext.grid.RowSelectionModel) {{
                srec = #{{{0}}}.getSelectionModel().getSelected();
            }}   
            else {{
                srec = #{{{0}}}.getSelectionModel().selection.record;
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
                MinChars = 1,
                TriggerAction = TriggerAction.All,
                StoreID = storeId,
                ValueField = "NAME",
                DisplayField = "NAME",
                ItemSelector = "tr.search-item",
                TypeAhead = false,
                PageSize = 10,
                LoadingText = "Поиск...",
                AutoWidth = useAutoWidth,
                Width = 270,
                EmptyText = "Выберите значение",
                HideTrigger = true,
                Editable = editable,
                Listeners = { Select = { Handler = handler } }
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
    }
}
