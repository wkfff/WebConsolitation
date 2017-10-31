using System.Web.Mvc;
using Ext.Net;

namespace Krista.FM.RIA.Core.Gui
{
    public class GridViewEditFormWindowBuilder
    {
        public Window Create(ViewPage page)
        {
            Window w = new Window();
            w.ID = "DetailsWindow";
            w.Icon = Icon.ApplicationFormEdit;
            w.Width = 800;
            w.Height = 600;
            w.Hidden = true;
            w.Modal = true;
            w.Maximizable = true;
            w.Constrain = true;

            w.AutoLoad.Url = "/";
            w.AutoLoad.Mode = LoadMode.IFrame;
            w.AutoLoad.TriggerEvent = "show";
            w.AutoLoad.ReloadOnEvent = true;
            w.AutoLoad.ShowMask = true;
            w.AutoLoad.MaskMsg = "Загрузка формы...";

            Button button = new Button();
            button.ID = "btnDetailsCancel";
            button.Text = "Закрыть";
            button.Enabled = false;
            button.Listeners.Click.Handler = "DetailsWindow.hide();";
            w.Buttons.Add(button);

            w.Listeners.BeforeHide.Handler = @"
if (forceHideDetailsWindow) {
    forceHideDetailsWindow = false;
    return true;
}
var viewPersistence = item.getBody().ViewPersistence;
if (!viewPersistence.isValid()) {
    Ext.Msg.show({
        title: 'Анализ и планирование',
        msg: 'В карточке не все поля заполнены корректно. Закрыть карточку без сохранения изменений?',
        buttons: Ext.MessageBox.YESNO,
        multiline: false,
        animEl: item,
        icon: Ext.MessageBox.WARNING,
        fn: function (btn) {
            if (btn == 'yes') {
                forceHideDetailsWindow = true;
                DetailsWindow.hide();
                return true;
            }
            return false;
        }
    });
    return false;
}
if (viewPersistence.isDirty()) {
    Ext.Msg.show({
        title: 'Анализ и планирование',
        msg: 'Сохранить изменения в карточке?',
        buttons: Ext.MessageBox.YESNOCANCEL,
        multiline: false,
        animEl: item,
        icon: Ext.MessageBox.WARNING,
        fn: function (btn) {
            if (btn == 'yes') {
                viewPersistence.save.call(this, false);
                viewPersistence.asynSave = true;
                forceHideDetailsWindow = true;
                DetailsWindow.hide();
                return true;
            }
            if (btn == 'no') {
                forceHideDetailsWindow = true;
                DetailsWindow.hide();
                return true;
            }
            return false;
        }
    });
    return false;
}
return true;
";

            w.Listeners.Hide.Handler = @"
// Если данные в форме редактирования строки были изменены,
// то необходимо обновить грид
var viewPersistence = DetailsWindow.getBody().ViewPersistence;
if (viewPersistence.isChanged){
    if (viewPersistence.asynSave){
        viewPersistence.onSave = function(){
            viewPersistence.onSave = function(){};
            #{tabPanel}.activeTab.store.load();
        };
        viewPersistence.asynSave = false;
    }else{
        #{tabPanel}.activeTab.store.load();
    }
}";

            Ext.Net.ResourceManager.GetInstance(page).RegisterOnReadyScript(
                "window.forceHideDetailsWindow = false;");

            return w;
        }
    }
}
