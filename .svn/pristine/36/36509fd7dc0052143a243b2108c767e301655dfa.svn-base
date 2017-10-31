using System;
using System.Collections.Generic;
using System.Linq;
using Ext.Net;
using Krista.FM.Extensions;
using Krista.FM.ServerLibrary;
using GridView = Krista.FM.RIA.Core.Gui.GridView;

namespace Krista.FM.RIA.Extensions.DebtBook.Presentation.ViewBuilders
{
    public class VariantSchuldbuchGridView : GridView
    {
        private const string VariantComment = "VariantComment";
    
        public VariantSchuldbuchGridView()
        {
            PagingSettings.Size = Int32.MaxValue;
        }

        public override List<Component> Build(System.Web.Mvc.ViewPage page)
        {
            var components = base.Build(page);

            GridPanel gridPanel = components.OfType<Viewport>().First()
                .Items.OfType<BorderLayout>().First()
                .Center.Items.OfType<GridPanel>().First();

            // Перемещаем первую колонку (Комментарий к варианту) в конец
            var column = gridPanel.ColumnModel.Columns[0];
            gridPanel.ColumnModel.Columns.RemoveAt(0);
            gridPanel.ColumnModel.Columns.Add(column);

            // Добавляем кнопку копирования варианта
            gridPanel.TopBar[0].Items.Add(CreateCopyVariantButton(gridPanel.ID));

            // Подменяем у store метод/контроллер, вызываемый при сохранении
            Store store = page.Controls.OfType<Store>().First();
            var writeProxy = store.UpdateProxy.OfType<HttpWriteProxy>().First();
            writeProxy.Url = "/BebtBookVariant/Save";
            store.WriteBaseParams.Add(new Parameter("objectKey", ObjectKey, ParameterMode.Value));

            // Подменяем событие на удаление записи
            var deleteButton = gridPanel.TopBar[0].Items.Where(f => f.ClientID == "{0}DeleteRowButton".FormatWith(gridPanel.ID)).OfType<Button>().First();
            deleteButton.Handler = deleteButton.Handler.Replace(@"Удалить выделенные записи?", @"Удаление варианта повлечет удаление всех связанных с ним данных! Удалить вариант?"); 

            // Добавляем скрипт в страницу для получения выбранного варианта
            var rm = gridPanel.ResourceManager;
            var script = @"
var getSelectedVariantId = function () {{
    var grid = Ext.getCmp('{0}');
    var record = grid.getSelectionModel().getSelections()[0];
    if (record) {{
        return record.id;
    }}
    return -1;
}}".FormatWith(gridPanel.ID);
            rm.RegisterClientScriptBlock("getSelectedVariantId", script);

            gridPanel.Listeners.BeforeEdit.Fn = "checkChangeCurrentVariant";
            script = @"
var checkChangeCurrentVariant = function (e) {
    if (e.field != 'CURRENTVARIANT'){
        e.cancel = false;
        return;
    }
    if (e.value == false){
	    Ext.Msg.show({
	        title:'Предупреждение',
	        msg: 'Сменить текущий вариант по умолчанию?',
	        buttons: Ext.Msg.YESNO,
	        icon: Ext.MessageBox.QUESTION,
            fn: function(buttonId){
                if (buttonId == 'yes'){
					// Находим текущий вариант и снимаем галку
					var i = e.grid.store.findBy(function(o){ return o.data.CURRENTVARIANT == 1;});
                    if (i > -1){
					    var r = e.grid.store.getAt(i);
					    r.set(e.field, false);
                    }
					// Устанавливаем галку для выбранного варианта
                    e.record.set(e.field, true);
                }
            }
	    });
    }
    e.cancel = true;
}
";
            rm.RegisterClientScriptBlock("checkChangeCurrentVariant", script);

            return components;
        }

        protected override bool FilterAttributes(IDataAttribute attribute)
        {
            return attribute.Name == VariantComment || (attribute.Class != DataAttributeClassTypes.System
                   && attribute.Class != DataAttributeClassTypes.Fixed);
        }

        protected override ColumnBase CreateGridColumn(IDataAttribute attribute)
        {
            ColumnBase column;
            if (attribute.Name == VariantComment)
            {
                column = AddColumn(attribute);

                if (!Readonly)
                {
                    column.Editable = true;
                    column.Editor.Add(new TextArea { AllowBlank = attribute.IsNullable });
                }
            }
            else
            {
                column = base.CreateGridColumn(attribute);
            }

            return column;
        }

        private Button CreateCopyVariantButton(string gridPanelId)
        {
            Button btnCopyVariant = new Button
            {
                ID = "btnCopyVariant",
                Text = "Копировать вариант",
                Icon = Icon.PageCopy
            };

            btnCopyVariant.DirectEvents.Click.Before = @"
if (extraParams.variantId == -1){
	Ext.Msg.show({
	    title:'Предупреждение',
	    msg: 'Необходимо выбрать вариант, который нужно скопировать.',
	    buttons: Ext.Msg.OK,
	    icon: Ext.MessageBox.WARNING
	});
    return false;
}
return true";

            btnCopyVariant.DirectEvents.Click.EventMask.Msg = "Копирование варианта...";
            btnCopyVariant.DirectEvents.Click.EventMask.ShowMask = true;
            btnCopyVariant.DirectEvents.Click.Url = "/BebtBookVariant/Copy/";
            btnCopyVariant.DirectEvents.Click.Timeout = 180000;
            btnCopyVariant.DirectEvents.Click.IsUpload = false;
            btnCopyVariant.DirectEvents.Click.CleanRequest = true;
            btnCopyVariant.DirectEvents.Click.Success = "{0}.reload();".FormatWith(gridPanelId);
            btnCopyVariant.DirectEvents.Click.Failure = @"
if (result.result){
    alert('Ошибка: ' + result.result);
} else {
    Ext.Msg.show({
       title:'Ошибка',
       msg: result.errorMessage,
       buttons: Ext.Msg.OK,
       icon: Ext.MessageBox.ERROR,
       maxWidth: 1000
    });
}
";
            btnCopyVariant.DirectEvents.Click.ExtraParams.Add(new Parameter
            {
                Name = "objectKey",
                Value = Entity.ObjectKey,
                Mode = ParameterMode.Value
            });
            btnCopyVariant.DirectEvents.Click.ExtraParams.Add(new Parameter
            {
                Name = "variantId",
                Value = "getSelectedVariantId()",
                Mode = ParameterMode.Raw
            });
            
            return btnCopyVariant;
        }
    }
}
