using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web.Mvc;
using Ext.Net;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.Controllers;
using Krista.FM.RIA.Core.Gui;
using Krista.FM.RIA.Extensions.E86N.Presentation.Controls.SearchCombobox;
using Krista.FM.RIA.Extensions.E86N.Presentation.Views;
using Krista.FM.ServerLibrary;

namespace Krista.FM.RIA.Extensions.E86N.Presentation.Controls
{
    /// <summary>
    /// Универсальный ComboBox отображающий данные из БД
    /// Агрегирует Store и ComboBox, предоставляет к ним доступ.
    /// </summary>
    public sealed class DBComboBox : Control
    {
        private bool valueFieldIsSet;
        private string objectKey;
        private IEnumerable<string> mappingFild;
        private string mappingDisplay;
        private Dictionary<string, string[]> mapping;

        /// <summary>
        /// Initializes a new instance of the <see cref="DBComboBox"/> class. 
        /// Конструктор по умолчанию, создает дефалтные Store и ComboBox
        /// </summary>
        public DBComboBox()
        {
            BoxStore = new Store { AutoLoad = false };
            Box = new ComboBox
                {
                    TriggerAction = TriggerAction.All,
                    FireSelectOnLoad = true,
                    AllowBlank = false,
                    Editable = false,
                    ItemSelector = "tr.search-item"
                };
            valueFieldIsSet = false;
            objectKey = string.Empty;
            mappingFild = null;
            mappingDisplay = null;
            mapping = null;
        }

        /// <summary>
        /// Делает поле открытым для ввода. Позволяет производить поиск.
        /// Контроллер должен поддерживать параметры limit,start,fields,query
        /// </summary>
        public bool Editable
        {
            set { Box.Editable = value; }
        }

        /// <summary>
        /// Описание комбобокса
        /// </summary>
        public string FieldLabel
        {
            set { Box.FieldLabel = value; }
        }

        /// <summary>
        /// Идентификатор для ComboBox
        /// Store.ID = "store" + ComboBox.ID
        /// </summary>
        public string ID
        {
            get 
            { 
                return Box.ID; 
            }

            set
            {
                if (value == Box.ID)
                {
                    return;
                }

                Box.ID = value;
                BoxStore.ID = Box.StoreID = string.Concat("store", value);
            }
        }

        /// <summary>
        /// URL адрес контроллера
        /// </summary>
        public string LoadController { private get; set; }

        /// <summary>
        /// Параметр Key для EntityController 
        /// Вызывать после установки Mapping(если он присутствует)!!!
        /// </summary>
        public string Key
        {
            set
            {
                objectKey = value;
                if (mapping != null)
                {
                    return;
                }

                ValueField = "ID";
                DisplayField = "NAME";
            }
        }

        /// <summary>
        /// Маппинг полей для грида. Не совмесно с ValueField
        /// </summary>
        public Dictionary<string, string[]> Mapping
        {
            set 
            {
                if (valueFieldIsSet)
                {
                    return;
                }

                mapping = value;
                valueFieldIsSet = true;
                mappingFild = value.Values.SelectMany(strings => strings).ToList();
            }
        }

        /// <summary>
        /// Поле ID справочника. Не совмесно с Mapping
        /// </summary>
        public string ValueField
        {
            set
            {
                if (valueFieldIsSet)
                {
                    return;
                }

                valueFieldIsSet = true;
                BoxStore.SetJsonReader(value, "data");
                BoxStore.AddField(value);
                Box.ValueField = value;
            }
        }

        /// <summary>
        /// Поле Name справочника.
        /// Вызывать после Mapping(если он нужен) 
        /// </summary>
        public string DisplayField
        {
            set
            {
                if (mapping == null)
                {
                    BoxStore.AddField(value);
                }
                else
                {
                    mappingDisplay = value;
                }

                Box.DisplayField = value;
            }
        }

        /// <summary>
        /// Поле внешнего Store, значение которого отображается
        /// </summary>
        public string DataIndex
        {
            set { Box.DataIndex = value; }
        }

        /// <summary>
        /// Экземпляр класса Store для данного ComboBox
        /// </summary>
        public Store BoxStore { get; private set; }

        /// <summary>
        /// Непосредственно сам ComboBox
        /// </summary>
        public ComboBox Box { get; private set; }

        /// <summary>
        /// Параметры для стора(контроллера)
        /// </summary>
        public Dictionary<string, string> Params { private get; set; }

        public override List<Component> Build(ViewPage page)
        {
            if (!valueFieldIsSet)
            {
                // Если ValueField не установлено значение по умолчанию
                ValueField = "ID";
                DisplayField = "Name";
            }

            if (mapping != null)
            {
                // Если mapping установлено кладем все поля в Store
                BoxStore.SetJsonReader();
                BoxStore.Do(
                    store => mappingFild.Each(
                        field => store.AddField(LoadController.IsNotNullOrEmpty() ? field : field.ToUpper())));
                Box.ValueField = Box.DisplayField;

                // Когда задан LoadController все работает хорошо
                // Когда LoadController не задан используется EntityController а там все поля переводятся в верхний регистр
                // todo: не делает заголовок для полей второго уровня типа RefOrgPPO.Name. Вопрос: а надо ли?
                var fieldInfos = ComboboxEditorExtensions.BuildReferencerInfo(Resolver.Get<IScheme>().RootPackage.FindEntityByName(objectKey), mapping[mappingDisplay]);
                Box.Template.Html = new SearchResultTableTpl { FieldInfos = fieldInfos, ToUpped = LoadController.IsNullOrEmpty() }
                    .TransformText();

                ResourceManager.GetInstance(page).RegisterClientScriptBlock("UiExtensions", Resource.UiExtensions);
                Box.Listeners.Select.Handler = string.Concat(
                    "E86n.View.Control.DBComboBox.onSelect(record,",
                    JSON.Serialize(mapping),
                    ",'",
                    mappingDisplay,
                    "',",
                    LoadController.IsNullOrEmpty().ToString().ToLower(),
                    ",",
                    Box.ID,
                    ");");
            }
            else
            {
                var tableTemplate = new StringBuilder(@"<table width=100%><tr>");
                tableTemplate.Append(@"</tr><tpl for="".""><tr class=""search-item"">");
                tableTemplate.Append(@"<td class=""search-item secondary""> {");
                tableTemplate.Append(Box.DisplayField);
                tableTemplate.Append(@"} </td></tr></tpl></table>");
                Box.Template.Html = tableTemplate.ToString();
            }

            if (Box.Editable)
            {
                // Если разрешено изменение поля(включён поиск)
                // Настраиваем внешний вид
                Box.PageSize = 10;
                Box.MinChars = 1;
                Box.Resizable = true;
                Box.LoadingText = @"Поиск...";
                Box.EmptyText = @"Выберите значение";
                Box.MinListWidth = 270;

                BoxStore.BaseParams.Add(
                    mapping != null
                        ? new Parameter("fields", JSON.Serialize(mapping[mappingDisplay]), ParameterMode.Raw)
                        : new Parameter("fields", string.Concat("['", Box.DisplayField, "']"), ParameterMode.Raw));
            }

            if (LoadController.IsNullOrEmpty())
            {
                if (Box.Editable)
                {
                    LoadController = UiBuilders.GetUrl<EntityController>("DataWithCustomSearch", new Dictionary<string, object> { { "objectKey", objectKey } });
                }
                else
                {
                    LoadController = UiBuilders.GetUrl<EntityController>(
                        "DataWithCustomSearch", new Dictionary<string, object> { { "objectKey", objectKey }, { "limit", -1 }, { "start", 0 } });
                }
            }

            BoxStore.SetHttpProxy(LoadController);
            
            // Добавляем параметры если они есть
            if (Params != null)
            {
                Params.Each(x => BoxStore.BaseParams.Add(new Parameter(x.Key, x.Value, ParameterMode.Raw)));
            }

            page.Controls.Add(BoxStore);
            return new List<Component> { Box };
        }

        public void DisplayFieldExp<T>(Expression<Func<T>> value)
        {
            ValueField = UiBuilders.NameOf(value);
        }

        public void ValueFieldExp<T>(Expression<Func<T>> value)
        {
            ValueField = UiBuilders.NameOf(value);
        }

        public void DataIndexExp<T>(Expression<Func<T>> value)
        {
            ValueField = UiBuilders.NameOf(value);
        }
    }
}
