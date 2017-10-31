using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

using Ext.Net;

using Krista.FM.Extensions;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Extensions.E86N.Presentation.Views;

namespace Krista.FM.RIA.Extensions.E86N.Presentation.Controls
{
    public static class UiExtensions
    {
        private const string Scope = "E86n.View.Control.UiExtensions.";
        
        /// <summary>
        ///   Устанавливает длинну вводимых данных в редакторе колонки
        /// </summary>
        /// <returns> Колонка грида </returns>
        public static ColumnBase SetMaxLengthEdior(this ColumnBase column, int maxLength)
        {
            var field = column.Editor.Count > 0 ? column.Editor[0] as TextFieldBase : null;
            if (field != null)
            {
                field.MaxLength = maxLength;
            }

            return column;
        }

        /// <summary>
        ///   Устанавливает длинну вводимых данных в редакторе колонки
        /// </summary>
        /// <returns> Колонка грида </returns>
        public static ColumnBase SetMinLengthEdior(this ColumnBase column, int maxLength)
        {
            var fild = column.Editor.Count > 0 ? column.Editor[0] as TextFieldBase : null;
            if (fild != null)
            {
                fild.MinLength = maxLength;
            }

            return column;
        }

        /// <summary>
        ///   Устанавливает строгую длинну вводимых данных в редакторе колонки
        /// </summary>
        /// <returns> Колонка грида </returns>
        public static ColumnBase SetLengthEdior(this ColumnBase column, int length)
        {
            var fild = column.Editor.Count > 0 ? column.Editor[0] as TextFieldBase : null;
            if (fild != null)
            {
                fild.MaxLength = length;
                fild.MinLength = length;
            }

            return column;
        }

        /// <summary>Добавляет кастомный обработчик на событие селект.(Использовать после SetComboBoxEditor). Используется к примеру для наложения маски на результат выбора</summary>
        /// <param name="cm">Обьект от которого вызывается функция</param>
        /// <param name="function">Исполняемая js строка</param>
        /// <returns>Возвращает данный ColumnBase</returns>
        public static ColumnBase SetCustomEditorFunction(this ColumnBase cm, string function)
        {
            var box = cm.Editor[0] as ComboBox;
            if (box != null)
            {
                box.Listeners.Select.Handler += function;
            }

            return cm;
        }

        /// <summary> Простой эдитор  </summary>
        /// <param name="cm">Обьект от которого вызывается функция </param>
        /// <param name="key">Ключь таблицы, по нему заголовок строится</param>
        /// <param name="page">Страница, на которую добавляется эдитор </param>
        /// <param name="fieldId">Поле Id из Store, значение которого будет изменяться </param>
        /// <param name="url">url адрес источнка стравочника</param>
        /// <param name="fieldName">Поле Name из Store, значение которого будет изменяться </param>
        /// <param name="find">Разрешен ли поиск по стравочнику</param>
        /// <param name="mayBeEmpty">Нужна ли кнопка отчистки поля</param>
        /// <param name="baseParams"> Кастомные параметры </param>
        /// <returns>Возвращает данный ColumnBase </returns>
        public static ColumnBase SetComboBoxEditor(
                                                    this ColumnBase cm,
                                                    string key,
                                                    ViewPage page,
                                                    string fieldId,
                                                    string url = null,
                                                    string fieldName = null,
                                                    bool find = true,
                                                    bool mayBeEmpty = true,
                                                    Dictionary<string, string> baseParams = null)
        {
            if (fieldName == null)
            {
                fieldName = cm.ColumnID;
            }

            var mapping = new Dictionary<string, string[]>
                {
                    { fieldId, new[] { "ID" } },
                    { fieldName, new[] { "Name" } }
                };

            return SetComboBoxEditor(cm, key, page, mapping, url, fieldName, "Name", find, mayBeEmpty, baseParams);
        }

        /// <summary>Эдитор с мапингом полей</summary>
        /// <param name="cm">Обьект от которого вызывается функция</param>
        /// <param name="key">Ключь таблицы, по нему заголовок строится</param>
        /// <param name="page">Страница, на которую добавляется эдитор</param>
        /// <param name="mapping">Словарь соответствие полей</param>
        /// <param name="url">url адрес источнка стравочника</param>
        /// <param name="displayField">Запись из mapping, которое будет отображаться в сторе</param>
        /// <param name="setupField">Поле из комбобокса, значение которого пишется в Column(если null - сумма displayFields)</param>
        /// <param name="find">Разрешен ли поиск по стравочнику</param>
        /// <param name="mayBeEmpty">Нужна ли кнопка отчистки поля</param>
        /// <param name="baseParams"> параметры для стора(экшена контроллера) </param>
        /// <returns>Возвращает данный ColumnBase</returns>
        public static ColumnBase SetComboBoxEditor(
                                                    this ColumnBase cm,
                                                    string key,
                                                    ViewPage page,
                                                    Dictionary<string, string[]> mapping,
                                                    string url = null,
                                                    string displayField = null,
                                                    string setupField = null,
                                                    bool find = true,
                                                    bool mayBeEmpty = true,
                                                    Dictionary<string, string> baseParams = null)
        {
            if (displayField == null)
            {
                displayField = cm.ColumnID;
            }

            var comboBox = new DBComboBox
                {
                    ID = string.Concat(cm.ColumnID, "Editor"),
                    Mapping = mapping,
                    LoadController = url,
                    Key = key,
                    DisplayField = displayField,
                    DataIndex = cm.ColumnID,
                    Editable = find,
                    Box = { TriggerIcon = TriggerIcon.SimpleArrowDown },
                    Params = baseParams
                };

            return SetHandler(cm, page, mapping, comboBox, displayField, setupField, url.IsNullOrEmpty(), mayBeEmpty);
        }

        /// <summary>
        /// Исправлена ситуация с отключением кнопки удаления после удаления записи
        /// </summary>
        /// <param name="gp"> Грид с кнопкой</param>
        public static Button AddDeleteRecordWithConfirmButton(this GridPanel gp)
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

            var btnHandler = @"
if (item.hasSelection()) {{
    window.{0}RemoveRecordBtn.setDisabled(false);
}} else {{
    window.{0}RemoveRecordBtn.setDisabled(true);
}}".FormatWith(gp.ID);

            ((RowSelectionModel)gp.GetSelectionModel()).Listeners.RowSelect.AddAfter(btnHandler);
            ((RowSelectionModel)gp.GetSelectionModel()).Listeners.RowDeselect.AddAfter(btnHandler);

            return gp.Toolbar()
                .AddIconButton(
                    "{0}RemoveRecordBtn".FormatWith(gp.ID),
                    Icon.Delete,
                    "Удалить запись",
                    handler).SetDisabled();
        }

        /// <summary>
        ///   Устанавливает маску ввода данных в редакторе колонки
        /// </summary>
        /// <returns> Колонка грида </returns>
        public static ColumnBase SetMaskReEdior(this ColumnBase column, string maskRe)
        {
            if (column.Editor.Any() && column.Editor[0] is TextFieldBase)
            {
                var fild = column.Editor[0] as TextFieldBase;
                fild.MaskRe = maskRe;
            }

            return column;
        }

        private static ColumnBase SetHandler(
            ColumnBase cm, 
            ViewPage page, 
            Dictionary<string, string[]> mapping,
            DBComboBox comboBox,
            string displayField,
            string setupField,
            bool toUpper,
            bool mayBeEmpty)
        {
            var handler = string.Format(
                @"{2}onSelect(record,{0},{1},'{3}','{4}',{5},{6});",
                cm.ParentGrid.ID,
                JSON.Serialize(mapping),
                Scope,
                displayField,
                setupField,
                toUpper.ToString().ToLower(),
                comboBox.Box.ID);

            comboBox.Build(page);
            comboBox.Box.Listeners.Select.Handler = handler;
            comboBox.Box.Listeners.Expand.Handler = "window.{0}.reload()".FormatWith(comboBox.BoxStore.ID);

            if (mayBeEmpty)
            {
                comboBox.Box.AddClearTrigger(cm.ParentGrid.ID, mapping);
            }

            ResourceManager.GetInstance(page).RegisterClientScriptBlock("UiExtensions", Resource.UiExtensions);
            ResourceManager.GetInstance(page).RegisterScript("Combobox.HotKeys", "/Krista.FM.RIA.Core/ExtNet.Extensions/ExcelLikeSelectionModel/js/HotKeysForCombobox.js/extention.axd");
            ResourceManager.GetInstance(page).RegisterStyle("CustomSearch.Style", "/Krista.FM.RIA.Core/Extensions/css/CustomSearch.css/extention.axd");

            cm.Editable = true;
            cm.Editor.Add(comboBox.Box);
            return cm;
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
                    Tag = "TriggerClear"
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
                string.Concat(Scope, @"getClearTrigger(this.triggers).show();"));

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
    }
}
