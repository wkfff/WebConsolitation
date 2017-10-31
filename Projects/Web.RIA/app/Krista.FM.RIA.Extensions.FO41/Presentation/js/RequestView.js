// добавляем сообщение при закрытии вкладки с заявкой
var beforeCloseTab = function (currentTab) {
    if (currentTab.forceClose != undefined && currentTab.forceClose) {
        return true;
    }

    currentTab.ownerCt.setActiveTab(currentTab);
    if (isDirty()) {
        Ext.Msg.show({
            title: 'Внимание',
            msg: 'Все несохраненные изменения будут потеряны. Сохранить заявку?',
            buttons: { yes: 'Сохранить', no: 'Закрыть', cancel: 'Отмена' },
            animEl: 'MdiTab',
            icon: Ext.MessageBox.WARNING,
            fn: function (buttonId) {
                if (buttonId == 'yes') {
                    saveApplication(currentTab);
                }
                else if (buttonId == 'no') {
                    currentTab.forceClose = true;
                    currentTab.ownerCt.closeTab(currentTab);
                }
            }
        });
        return false;
    }
    else {
        return true;
    }
};

// проверка - есть ли в заявке несохраненные данные
var isDirty = function() {
    return false;
};

// метод на сохранение заявки (нужно переопределять)
var saveApplication = function (currentTab) {
    return true;
}

// обработчик в случае неудачи сохранения заявки
function failureSaveHandler(form, action) {
    switch (action.failureType) {
        case Ext.form.Action.CLIENT_INVALID:
             Ext.Msg.show({
                    title: 'Внимание',
                    msg: 'Перед сохранением заявки необходимо заполнить все обязательные поля',
                    buttons: Ext.MessageBox.OK,
                    multiline: false,
                    animEl: 'ApplicPanel',
                    icon: Ext.MessageBox.WARNING
                });
            break;
        case Ext.form.Action.CONNECT_FAILURE:
            Ext.Msg.alert('Ошибка', 'Ajax communication failed');
            break;
        case Ext.form.Action.SERVER_INVALID:
            if (action.result != undefined) {
                if (action.result.msg != undefined) {
                    Ext.Msg.alert('Ошибка', action.result.msg);
                } else {
                    if (action.result.errors != undefined) {
                        Ext.Msg.alert('Ошибка', action.result.errors[0].msg);
                    }
                }
            } else {
                Ext.Msg.alert('Ошибка', 'Server failed');
            }
            break;
    }
}

// проверка, правильно ли заполнены реквизиты
function formValidate() {
    if (!DetailsForm.isValid()) {
        Ext.Msg.show({
            title: 'Внимание',
            msg: 'Перед сохранением заявки необходимо заполнить все обязательные поля',
            buttons: Ext.MessageBox.OK,
            multiline: false,
            animEl: 'ApplicPanel',
            icon: Ext.MessageBox.WARNING
        });
        return false;
    }
    return true;
}

// если в диалоговом окне о передаче на рассмотрение ОГВ выбрана кнопка yes - изменения статуса заявки. Сохранение заявки.
function state2Save(btn) {
    if (btn == 'yes') {
        dsDetail.getAt(0).data.StateId = 2;
    }
    saveApp();
 }

function saveApp() {
     // сохранить заявку
    DetailsForm.form.submit({
        waitMsg: 'Сохранение...',
        success: successSaveHandlerReload,
        failure: failureSaveHandler, 
        params: { 'state': dsDetail.getAt(0).data.StateId} });
}