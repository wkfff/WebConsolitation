//обработчик на кнопку "Сохранить" модального окна
var saveChanges = function (gridPanel, window, formPanel) {
    if (formPanel.record != null) {
        if (formPanel.getForm().isValid()) {
            formPanel.getForm().updateRecord(formPanel.record);
            gridPanel.store.save();
            gridPanel.store.commitChanges();
            window.hide();
        }
        else {
            Ext.Msg.show({ icon: Ext.MessageBox.ERROR, msg: 'Введены не все обязательные поля', buttons: Ext.Msg.OK });
        }
    }
    else {
        if (formPanel.getForm().isValid()) {
            formPanel.getForm().submit({
                waitMsg: 'Сохранение...',
                success: successSaveNew,
                failure: failureFormSaveHandler
            });
            window.hide();
        }
        else {
            Ext.Msg.show({ icon: Ext.MessageBox.ERROR, msg: 'Введены не все обязательные поля', buttons: Ext.Msg.OK });
        }
    }
};

//обработчик удаления записи
var deleteRecord = function (gridPanel) {
    Ext.Msg.confirm('Предупреждение', 'Удалить выделенную запись?', function (btn) {
        if (btn == 'yes') {
            gridPanel.deleteSelected();
            gridPanel.store.save();
            if (!gridPanel.hasSelection()) {
                btnDelete.disable();
            }
        }
    }
    );
};

//обработчик создания записи
var insertRecord = function (window, formPanel) {
    window.show();
    formPanel.getForm().reset();
    formPanel.record = null;
};

//обработчик добавления записи
var updateRecord = function (gridPanel, window, formPanel) {
    window.show();
    formPanel.getForm().loadRecord(gridPanel.getSelectionModel().getSelected());
};


function pressEditRecord(record, command, window, formPanel) {
    switch(command) {
    case 'EditRecord': {
        window.show(); 
        formPanel.getForm().loadRecord(record); 
        formPanel.record = record;
        }
    };
};

function closeWindowCard(window) {
    window.hide(); 
};

function hasRowSelection(gridPanel) {
    if (!gridPanel.hasSelection()) {
        btnDelete.disable(); 
    }
};

function successSaveNew(form, action) {
   if (action.result.script) {
        eval(action.result.script);
    }
    if (action.result != undefined && action.result.extraParams.msg != undefined) {
        Ext.net.Notification.show({
            iconCls: 'icon-information',
            html: action.result.extraParams.msg,
            title: 'Уведомление',
            hideDelay: 2500
        });
    } else {
        Ext.net.Notification.show({
            iconCls: 'icon-information',
            html: 'Ок',
            title: 'Уведомление',
            hideDelay: 2500
        });
    };
};

function failureFormSaveHandler(form, action) {
    switch (action.failureType) {
        case Ext.form.Action.CLIENT_INVALID:
            Ext.Msg.alert('Ошибка', 'Все поля должны иметь корректные значения');
            break;
        case Ext.form.Action.CONNECT_FAILURE:
            Ext.Msg.alert('Ошибка', 'action.response.responseText');
            break;
        case Ext.form.Action.SERVER_INVALID:
            if (action.result != undefined) {
                if (action.result.extraParams != undefined && action.result.extraParams.responseText != undefined) {
                    Ext.Msg.alert('Ошибка', action.result.extraParams.responseText);
                } else if (action.result.msg != undefined) {
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
};

function saveHandlerForGrid() {
    Ext.net.Notification.show({ iconCls: 'icon-information', title: 'Сохранение.', html: arg.raw.message, hideDelay: 2500 });
};

